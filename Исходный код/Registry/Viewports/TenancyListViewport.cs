using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyListViewport: DataGridViewport
    {

        #region Models
        private DataModel _tenancyBuildingAssoc;
        private DataModel _tenancyPremisesAssoc;
        private DataModel _tenancySubPremisesAssoc;
        private CalcDataModel _tenanciesAggregate;
        private DataModel _rentTypes;
        #endregion Models

        #region Views
        private BindingSource _generalBindingSourceAggregate;
        private BindingSource _vRentTypes;
        #endregion Views

        //Forms
        private SearchForm _stExtendedSearchForm;
        private SearchForm _stSimpleSearchForm;

        private TenancyListViewport()
            : this(null, null)
        {
        }

        public TenancyListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private void RebuildStaticFilter()
        {
            IEnumerable<int> ids;
            if (ParentRow == null)
                return;
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    ids = DataModelHelper.TenancyProcessIDsByBuildingId(Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.Premises:
                    ids = DataModelHelper.TenancyProcessIDsByPremisesId(Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.SubPremises:
                    ids = DataModelHelper.TenancyProcessIDsBySubPremisesId(Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture));
                    break;
                default: 
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
            if (ids != null)
            {
                StaticFilter = "id_process IN (0";
                foreach (var id in ids)
                    StaticFilter += id.ToString(CultureInfo.InvariantCulture) + ",";
                StaticFilter = StaticFilter.TrimEnd(',') + ")";
            }
            var filter = StaticFilter;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            GeneralBindingSource.Filter = filter + DynamicFilter;
        }

        private void SetViewportCaption()
        {
            if (ParentRow == null)
                Text = @"Процессы найма жилья";
            else
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        Text = string.Format(CultureInfo.InvariantCulture, "Найм здания №{0}", ParentRow["id_building"]);
                        break;
                    case ParentTypeEnum.Premises:
                        Text = string.Format(CultureInfo.InvariantCulture, "Найм помещения №{0}", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        Text = string.Format(CultureInfo.InvariantCulture, "Найм комнаты №{0}", ParentRow["id_sub_premises"]);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = EntityDataModel<TenancyProcess>.GetInstance();
            _rentTypes = DataModel.GetInstance<RentTypesDataModel>();
            _tenanciesAggregate = CalcDataModel.GetInstance<CalcDataModelTenancyAggregated>();

            //Ожидаем загрузки данных, если это необходимо
            GeneralDataModel.Select();
            _rentTypes.Select();

            SetViewportCaption();

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource {DataMember = "tenancy_processes"};
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            GeneralBindingSource.DataSource = ds;
            RebuildStaticFilter();
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;

            _generalBindingSourceAggregate = new BindingSource {DataSource = _tenanciesAggregate.Select()};

            _vRentTypes = new BindingSource
            {
                DataMember = "rent_types",
                DataSource = ds
            };

            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", TenancyListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", TenancyListViewport_RowDeleted);

            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        _tenancyBuildingAssoc = DataModel.GetInstance<TenancyBuildingsAssocDataModel>();
                        AddEventHandler<DataRowChangeEventArgs>(_tenancyBuildingAssoc.Select(), "RowChanged", TenancyAssocViewport_RowChanged);
                        AddEventHandler<DataRowChangeEventArgs>(_tenancyBuildingAssoc.Select(), "RowDeleted", TenancyAssocViewport_RowDeleted);
                        break;
                    case ParentTypeEnum.Premises:
                        _tenancyPremisesAssoc = DataModel.GetInstance<TenancyPremisesAssocDataModel>();
                        AddEventHandler<DataRowChangeEventArgs>(_tenancyPremisesAssoc.Select(), "RowChanged", TenancyAssocViewport_RowChanged);
                        AddEventHandler<DataRowChangeEventArgs>(_tenancyPremisesAssoc.Select(), "RowDeleted", TenancyAssocViewport_RowDeleted);
                        break;
                    case ParentTypeEnum.SubPremises:
                        _tenancySubPremisesAssoc = DataModel.GetInstance<TenancySubPremisesAssocDataModel>();
                        AddEventHandler<DataRowChangeEventArgs>(_tenancySubPremisesAssoc.Select(), "RowChanged", TenancyAssocViewport_RowChanged);
                        AddEventHandler<DataRowChangeEventArgs>(_tenancySubPremisesAssoc.Select(), "RowDeleted", TenancyAssocViewport_RowDeleted);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
            dataGridView.RowCount = GeneralBindingSource.Count;
            AddEventHandler<EventArgs>(_tenanciesAggregate, "RefreshEvent", tenancies_aggregate_RefreshEvent);
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить этот процесс найма жилья?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_process"]) == -1)
                    return;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            if (!string.IsNullOrEmpty(DynamicFilter))
                return true;
            else
                return false;
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (_stSimpleSearchForm == null)
                        _stSimpleSearchForm = new SimpleSearchTenancyForm();
                    if (_stSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _stSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (_stExtendedSearchForm == null)
                        _stExtendedSearchForm = new ExtendedSearchTenancyForm();
                    if (_stExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _stExtendedSearchForm.GetFilter();
                    break;
            }
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            dataGridView.RowCount = 0;                              
            GeneralBindingSource.Filter = filter;
            dataGridView.RowCount = GeneralBindingSource.Count;
        }

        public override void ClearSearch()
        {
            GeneralBindingSource.Filter = StaticFilter;
            dataGridView.RowCount = GeneralBindingSource.Count;
            DynamicFilter = "";
        }

        public override bool CanOpenDetails()
        {
            if (GeneralBindingSource.Position == -1)
                return false;
            else
                return true;
        }

        public override void OpenDetails()
        {
            var viewport = new TenancyViewport(null, MenuCallback)
            {
                StaticFilter = StaticFilter,
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocateEntityBy("id_process", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            var viewport = new TenancyViewport(null, MenuCallback)
            {
                StaticFilter = StaticFilter,
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            viewport.InsertRecord();
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanCopyRecord()
        {
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            var viewport = new TenancyViewport(null, MenuCallback)
            {
                StaticFilter = StaticFilter,
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocateEntityBy("id_process", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.TenancyPersonsViewport,
                ViewportType.TenancyReasonsViewport,
                ViewportType.TenancyBuildingsViewport,
                ViewportType.TenancyPremisesViewport,
                ViewportType.TenancyAgreementsViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport<T>()
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, 
                "id_process = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Tenancy);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            if (GeneralBindingSource.Position == -1)
                return false;
            var idProcess = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"] != DBNull.Value
                ? (int?)Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"],
                    CultureInfo.InvariantCulture) : null;
            var idRentType = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_rent_type"] != DBNull.Value
                ? (int?)Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_rent_type"],
                    CultureInfo.InvariantCulture) : null;
            switch (reporterType)
            {
                case ReporterType.ExportReporter:
                    return true;
                case ReporterType.TenancyContractCommercialReporter:
                    return idProcess != null && idRentType == 1;
                case ReporterType.TenancyContractSocialReporter:
                    return idProcess != null && idRentType == 3;
                case ReporterType.TenancyContractSpecial1711Reporter:
                case ReporterType.TenancyContractSpecial1712Reporter:
                    return idProcess != null && idRentType == 2;
                case ReporterType.TenancyActReporter:
                case ReporterType.TenancyNotifyContractAgreement:
                    return idProcess != null;
                case ReporterType.TenancyAgreementReporter:
                    return idProcess != null && (DataModelHelper.TenancyAgreementsForProcess(idProcess.Value) > 0);
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!TenancyValidForReportGenerate())
                return;
            var reporter = ReporterFactory.CreateReporter(reporterType);
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.ExportReporter:
                    arguments = ExportReporterArguments();
                    break;
                case ReporterType.TenancyContractCommercialReporter:
                case ReporterType.TenancyContractSocialReporter:
                case ReporterType.TenancyContractSpecial1711Reporter:
                case ReporterType.TenancyContractSpecial1712Reporter:
                    arguments = TenancyContractReporterArguments();
                    break;
                case ReporterType.TenancyNotifyContractAgreement:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "1");
                    break;
                case ReporterType.TenancyActReporter:
                    arguments = TenancyActReporterArguments();
                    break;
            }
            reporter.Run(arguments);
        }

        private Dictionary<string, string> ExportReporterArguments()
        {
            var columnHeaders = dataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnHeader\":\"" + column.HeaderText + "\"}");
            var columnPatterns = dataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnPattern\":\"$column" + column.DisplayIndex + "$\"}");
            var arguments = new Dictionary<string, string>
            {
                {"type", "3"},
                {"filter", GeneralBindingSource.Filter.Trim() == "" ? "(1=1)" : GeneralBindingSource.Filter},
                {"columnHeaders", "["+columnHeaders+",{\"columnHeader\":\"Дополнительные сведения\"}]"},
                {"columnPatterns", "["+columnPatterns+",{\"columnPattern\":\"$description$\"}]"}
            };
            return arguments;
        }

        private Dictionary<string, string> TenancyContractReporterArguments()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return new Dictionary<string, string> {{"id_process", row["id_process"].ToString()}}; 
        }

        private Dictionary<string, string> TenancyActReporterArguments()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return new Dictionary<string, string> { { "id_process", row["id_process"].ToString() } }; 
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (GeneralBindingSource.Position == -1)
                return false;
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            if (!DataModelHelper.TenancyProcessHasTenant(Convert.ToInt32(row["id_process"], CultureInfo.InvariantCulture)))
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо указать нанимателя процесса найма",@"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(row, "registration_date") == null || ViewportHelper.ValueOrNull(row, "registration_num") == null)
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации",@"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (CanOpenDetails())
                OpenDetails();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_process":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["id_process"];
                    break;
                case "registration_num":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["registration_num"];
                    break;
                case "registration_date":
                    var date = ((DataRowView)GeneralBindingSource[e.RowIndex])["registration_date"];
                    e.Value = date is DateTime ? ((DateTime)date).ToString("dd.MM.yyyy") : null;
                    break;
                case "end_date":
                    var date2 = ((DataRowView)GeneralBindingSource[e.RowIndex])["end_date"];
                    e.Value = date2 is DateTime ? ((DateTime)date2).ToString("dd.MM.yyyy") : null;
                    break;
                case "residence_warrant_num":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["residence_warrant_num"];
                    break;
                case "tenant":
                    var rowIndex = _generalBindingSourceAggregate.Find("id_process", ((DataRowView)GeneralBindingSource[e.RowIndex])["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_generalBindingSourceAggregate[rowIndex])["tenant"];
                    break;
                case "rent_type":
                    rowIndex = _vRentTypes.Find("id_rent_type", ((DataRowView)GeneralBindingSource[e.RowIndex])["id_rent_type"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vRentTypes[rowIndex])["rent_type"];
                    break;
                case "address":
                    rowIndex = _generalBindingSourceAggregate.Find("id_process", ((DataRowView)GeneralBindingSource[e.RowIndex])["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_generalBindingSourceAggregate[rowIndex])["address"];
                    break;
            }
        }

        void tenancies_aggregate_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        void TenancyListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void TenancyListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = GeneralBindingSource.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void TenancyAssocViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RebuildStaticFilter();
            dataGridView.RowCount = GeneralBindingSource.Count;
        }

        private void TenancyAssocViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RebuildStaticFilter();
            dataGridView.RowCount = GeneralBindingSource.Count;
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1170)
            {
                if (address.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    address.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (address.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    address.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }
    }
}
