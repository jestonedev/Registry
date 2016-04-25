using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private DataModel tenancy_building_assoc;
        private DataModel tenancy_premises_assoc;
        private DataModel tenancy_sub_premises_assoc;
        private CalcDataModel tenancies_aggregate;
        private DataModel rent_types;
        #endregion Models

        #region Views
        private BindingSource GeneralBindingSource_aggregate;
        private BindingSource v_rent_types;
        #endregion Views

        //Forms
        private SearchForm stExtendedSearchForm;
        private SearchForm stSimpleSearchForm;

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
            IEnumerable<int> ids = null;
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
            var Filter = StaticFilter;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(Filter) && !string.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            GeneralBindingSource.Filter = Filter + DynamicFilter;
        }

        private void SetViewportCaption()
        {
            if (ParentRow == null)
                Text = "Процессы найма жилья";
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
            GeneralDataModel = DataModel.GetInstance<TenancyProcessesDataModel>();
            rent_types = DataModel.GetInstance<RentTypesDataModel>();
            tenancies_aggregate = CalcDataModel.GetInstance<CalcDataModelTenancyAggregated>();

            //Ожидаем загрузки данных, если это необходимо
            GeneralDataModel.Select();
            rent_types.Select();

            SetViewportCaption();

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "tenancy_processes";
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataSource = ds;
            RebuildStaticFilter();
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;

            GeneralBindingSource_aggregate = new BindingSource();
            GeneralBindingSource_aggregate.DataSource = tenancies_aggregate.Select();

            v_rent_types = new BindingSource();
            v_rent_types.DataMember = "rent_types";
            v_rent_types.DataSource = ds;

            GeneralDataModel.Select().RowChanged += TenancyListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += TenancyListViewport_RowDeleted;
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        tenancy_building_assoc = DataModel.GetInstance<TenancyBuildingsAssocDataModel>();
                        tenancy_building_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_building_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    case ParentTypeEnum.Premises:
                        tenancy_premises_assoc = DataModel.GetInstance<TenancyPremisesAssocDataModel>();
                        tenancy_premises_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_premises_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    case ParentTypeEnum.SubPremises:
                        tenancy_sub_premises_assoc = DataModel.GetInstance<TenancySubPremisesAssocDataModel>();
                        tenancy_sub_premises_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_sub_premises_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
            dataGridView.RowCount = GeneralBindingSource.Count;
            tenancies_aggregate.RefreshEvent += tenancies_aggregate_RefreshEvent;
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот процесс найма жилья?", "Внимание",
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
                    if (stSimpleSearchForm == null)
                        stSimpleSearchForm = new SimpleSearchTenancyForm();
                    if (stSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = stSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (stExtendedSearchForm == null)
                        stExtendedSearchForm = new ExtendedSearchTenancyForm();
                    if (stExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = stExtendedSearchForm.GetFilter();
                    break;
            }
            var Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            dataGridView.RowCount = 0;
            GeneralBindingSource.Filter = Filter;
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
                {"columnHeaders", "["+columnHeaders+"]"},
                {"columnPatterns", "["+columnPatterns+"]"}
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
                MessageBox.Show("Для формирования отчетной документации необходимо указать нанимателя процесса найма","Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(row, "registration_date") == null || ViewportHelper.ValueOrNull(row, "registration_num") == null)
            {
                MessageBox.Show("Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации","Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            GeneralDataModel.Select().RowChanged -= TenancyListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= TenancyListViewport_RowDeleted;
            tenancies_aggregate.RefreshEvent -= tenancies_aggregate_RefreshEvent;
            GeneralBindingSource.CurrentItemChanged -= GeneralBindingSource_CurrentItemChanged;
            if (tenancy_building_assoc != null)
            {
                tenancy_building_assoc.Select().RowChanged -= TenancyAssocViewport_RowChanged;
                tenancy_building_assoc.Select().RowDeleted -= TenancyAssocViewport_RowDeleted;
            }
            if (tenancy_premises_assoc != null)
            {
                tenancy_premises_assoc.Select().RowChanged -= TenancyAssocViewport_RowChanged;
                tenancy_premises_assoc.Select().RowDeleted -= TenancyAssocViewport_RowDeleted;
            }
            if (tenancy_sub_premises_assoc != null)
            {
                tenancy_sub_premises_assoc.Select().RowChanged -= TenancyAssocViewport_RowChanged;
                tenancy_sub_premises_assoc.Select().RowDeleted -= TenancyAssocViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (CanOpenDetails())
                OpenDetails();
        }

        void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            if (dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
            dataGridView.Refresh();
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
                case "residence_warrant_num":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["residence_warrant_num"];
                    break;
                case "tenant":
                    var row_index = GeneralBindingSource_aggregate.Find("id_process", ((DataRowView)GeneralBindingSource[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)GeneralBindingSource_aggregate[row_index])["tenant"];
                    break;
                case "rent_type":
                    row_index = v_rent_types.Find("id_rent_type", ((DataRowView)GeneralBindingSource[e.RowIndex])["id_rent_type"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_rent_types[row_index])["rent_type"];
                    break;
                case "address":
                    row_index = GeneralBindingSource_aggregate.Find("id_process", ((DataRowView)GeneralBindingSource[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)GeneralBindingSource_aggregate[row_index])["address"];
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

        void TenancyListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
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
            if (dataGridView.Size.Width > 1260)
            {
                if (dataGridView.Columns["address"].AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    dataGridView.Columns["address"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (dataGridView.Columns["address"].AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    dataGridView.Columns["address"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }
    }
}
