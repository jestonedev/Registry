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
using Registry.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class PremisesListViewport : DataGridViewport
    {

        #region Models
        private DataModel buildings;
        private DataModel kladr;
        private DataModel premises_types;
        private DataModel object_states;
        private CalcDataModel premises_funds;
        private CalcDataModel _premisesTenanciesInfo;
        private DataModel fund_types;
        #endregion Models

        #region Views
        private BindingSource v_buildings;
        private BindingSource v_premises_types;
        #endregion Views

        //Forms
        private SearchForm spExtendedSearchForm;
        private SearchForm spSimpleSearchForm;

        private PremisesListViewport()
            : this(null, null)
        {
        }

        public PremisesListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.PremisesDataModel);
            kladr = DataModel.GetInstance(DataModelType.KladrStreetsDataModel);
            buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel);
            premises_types = DataModel.GetInstance(DataModelType.PremisesTypesDataModel);
            object_states = DataModel.GetInstance(DataModelType.ObjectStatesDataModel);
            fund_types = DataModel.GetInstance(DataModelType.FundTypesDataModel);
            premises_funds = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelPremisesCurrentFunds);

            // Ожидаем дозагрузки данных, если это необходимо
            kladr.Select();
            buildings.Select();
            premises_types.Select();
            object_states.Select();
            premises_funds.Select();
            fund_types.Select();

            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                _premisesTenanciesInfo = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelPremisesTenanciesInfo);
                _premisesTenanciesInfo.Select();
                var registrationNumColumn = new DataGridViewTextBoxColumn
                {
                    Name = "registration_num",
                    HeaderText = @"№ договора найма",
                    Width = 150,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var registrationDateColumn = new DataGridViewTextBoxColumn
                {
                    Name = "registration_date",
                    HeaderText = @"Дата договора найма",
                    Width = 150,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var endDateColumn = new DataGridViewTextBoxColumn
                {
                    Name = "end_date",
                    HeaderText = @"Дата окончания договора",
                    Width = 170,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var residenceWarrantNumColumn = new DataGridViewTextBoxColumn
                {
                    Name = "residence_warrant_num",
                    HeaderText = @"№ ордера найма",
                    Width = 150,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var residenceWarrantDateColumn = new DataGridViewTextBoxColumn
                {
                    Name = "residence_warrant_date",
                    HeaderText = @"Дата ордера найма",
                    Width = 150,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var tenantColumn = new DataGridViewTextBoxColumn
                {
                    Name = "tenant",
                    HeaderText = @"Наниматель",
                    Width = 250,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                dataGridView.Columns.Add(registrationNumColumn);
                dataGridView.Columns.Add(registrationDateColumn);
                dataGridView.Columns.Add(endDateColumn);
                dataGridView.Columns.Add(residenceWarrantNumColumn);
                dataGridView.Columns.Add(residenceWarrantDateColumn);
                dataGridView.Columns.Add(tenantColumn);
                _premisesTenanciesInfo.RefreshEvent += _premisesTenanciesInfo_RefreshEvent;
            }

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataMember = "premises";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                Text = @"Помещения здания №" + ParentRow["id_building"];

            v_buildings = new BindingSource
            {
                DataMember = "buildings",
                DataSource = ds
            };

            v_premises_types = new BindingSource
            {
                DataMember = "premises_types",
                DataSource = ds
            };

            id_premises_type.DataSource = v_premises_types;
            id_premises_type.ValueMember = "id_premises_type";
            id_premises_type.DisplayMember = "premises_type";

            GeneralDataModel.Select().RowChanged += PremisesListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += PremisesListViewport_RowDeleted;
            premises_funds.RefreshEvent += premises_funds_RefreshEvent;
            dataGridView.RowCount = GeneralBindingSource.Count;

            ViewportHelper.SetDoubleBuffered(dataGridView);
        }
        
        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это помещение?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_premises"]) == -1)
                    return;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_premisesTenanciesInfo != null)
                _premisesTenanciesInfo.RefreshEvent -= _premisesTenanciesInfo_RefreshEvent;
            if (GeneralBindingSource != null)
                GeneralBindingSource.CurrentItemChanged -= GeneralBindingSource_CurrentItemChanged;
            if (GeneralDataModel != null)
            {
                GeneralDataModel.Select().RowChanged -= PremisesListViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= PremisesListViewport_RowDeleted;
            }
            if (premises_funds != null)
                premises_funds.RefreshEvent -= premises_funds_RefreshEvent;
            base.OnClosing(e);
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            return !string.IsNullOrEmpty(DynamicFilter);
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (spSimpleSearchForm == null)
                        spSimpleSearchForm = new SimpleSearchPremiseForm();
                    if (spSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = spSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (spExtendedSearchForm == null)
                        spExtendedSearchForm = new ExtendedSearchPremisesForm();
                    if (spExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = spExtendedSearchForm.GetFilter();
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
            var viewport = new PremisesViewport(null, MenuCallback)
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
                viewport.LocateEntityBy("id_premises", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            var viewport = new PremisesViewport(null, MenuCallback)
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
            return (GeneralBindingSource.Position != -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            var viewport = new PremisesViewport(null, MenuCallback)
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
                viewport.LocateEntityBy("id_premises", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool HasAssocViewport(ViewportType viewportType)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.SubPremisesViewport,
                ViewportType.OwnershipListViewport,
                ViewportType.RestrictionListViewport,
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport,
                ViewportType.PaymentsAccountsViewport
            };
            return reports.Contains(viewportType) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport(ViewportType viewportType)
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_premises = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Premises);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            var reports = new List<ReporterType>
            {
                ReporterType.ExportReporter,
                ReporterType.RegistryExcerptReporterPremise,
                ReporterType.RegistryExcerptReporterAllMunSubPremises
            };
            return reports.Contains(reporterType);
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            var reporter = ReporterFactory.CreateReporter(reporterType);
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.ExportReporter:
                    arguments = ExportReportArguments();
                    break;
                case ReporterType.RegistryExcerptReporterPremise:
                    arguments = RegistryExcerptPremiseReportArguments();
                    break;
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    arguments = RegistryExcerptReporterAllMunSubPremisesArguments();
                    break;
            }
            reporter.Run(arguments);
        }

        private Dictionary<string, string> RegistryExcerptPremiseReportArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_premises"].ToString()},
                {"excerpt_type", "1"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterAllMunSubPremisesArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_premises"].ToString()},
                {"excerpt_type", "3"}
            };
            return arguments;
        }

        private Dictionary<string, string> ExportReportArguments()
        {
            var columnHeaders = dataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnHeader\":\"" + column.HeaderText + "\"}");
            var columnPatterns = dataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnPattern\":\"$column" + column.DisplayIndex + "$\"}");
            var arguments = new Dictionary<string, string>
            {
                {"type", "2"},
                {"filter", GeneralBindingSource.Filter.Trim() == "" ? "(1=1)" : GeneralBindingSource.Filter},
                {"columnHeaders", "["+columnHeaders+"]"},
                {"columnPatterns", "["+columnPatterns+"]"}
            };
            return arguments;
        }

        void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (CanOpenDetails())
                OpenDetails();
        }

        void premises_funds_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        void PremisesListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void PremisesListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = GeneralBindingSource.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
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
            var row = ((DataRowView)GeneralBindingSource[e.RowIndex]);
            var buildingRow = buildings.Select().Rows.Find(row["id_building"]);
            if (buildingRow == null)
                return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_premises":
                    e.Value = row["id_premises"];
                    break;
                case "id_street":
                    var kladrRow = kladr.Select().Rows.Find(buildingRow["id_street"]);
                    string streetName = null;
                    if (kladrRow != null)
                        streetName = kladrRow["street_name"].ToString();
                    e.Value = streetName;
                    break;
                case "house":
                    e.Value = buildingRow["house"];
                    break;
                case "premises_num":
                    e.Value = row["premises_num"];
                    break;
                case "id_premises_type":
                    e.Value = row["id_premises_type"];
                    break;
                case "total_area":
                    e.Value = row["total_area"];
                    break;
                case "id_state":
                    var stateRow = object_states.Select().Rows.Find(row["id_state"]);
                    if (stateRow != null)
                        e.Value = stateRow["state_female"];
                    break;
                case "current_fund":
                    if ((new object[] { 1, 4, 5, 9 }).Contains(row["id_state"]))
                    {
                        var fundRow = premises_funds.Select().Rows.Find(row["id_premises"]);
                        if (fundRow != null)
                            e.Value = fund_types.Select().Rows.Find(fundRow["id_fund_type"])["fund_type"];
                    }
                    break;
                case "registration_num":
                case "registration_date":
                case "residence_warrant_num":
                case "residence_warrant_date":
                case "tenant":
                case "end_date":
                    var tenancyInfoRows =
                        from tenancyInfoRow in _premisesTenanciesInfo.FilterDeletedRows()
                        where tenancyInfoRow.Field<int>("id_premises") == (int?) row["id_premises"]
                        orderby tenancyInfoRow.Field<DateTime?>("registration_date") ?? 
                            DateTime.Now descending 
                        select tenancyInfoRow;
                    if (!tenancyInfoRows.Any())
                        return;
                    var tenancyRow = tenancyInfoRows.First();
                    switch (dataGridView.Columns[e.ColumnIndex].Name)
                    {
                        case "registration_date":
                        case "residence_warrant_date":
                        case "end_date":
                            var date = tenancyRow.Field<DateTime?>(dataGridView.Columns[e.ColumnIndex].Name);
                            e.Value = date != null ? date.Value.ToString("dd.MM.yyyy") : null;
                            if (dataGridView.Columns[e.ColumnIndex].Name == "end_date" && tenancyRow.Field<bool?>("until_dismissal") == true)
                                e.Value = "на период ТО";
                            break;
                        case "registration_num":
                        case "residence_warrant_num":
                        case "tenant":
                                e.Value = tenancyRow.Field<string>(dataGridView.Columns[e.ColumnIndex].Name);
                            break;
                    }
                    if (tenancyRow.Field<int?>("object_type") == 2)
                    {
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Green;
                    }
                    else
                    {
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = SystemColors.Highlight;
                    }
                    break;
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 960)
            {
                if (dataGridView.Columns["id_street"].AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    dataGridView.Columns["id_street"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (dataGridView.Columns["id_street"].AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    dataGridView.Columns["id_street"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        void _premisesTenanciesInfo_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        internal int GetCurrentId()
        {
            if (GeneralBindingSource.Position < 0) return -1;
            if (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] != DBNull.Value)
                return (int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"];
            return -1;
        }

        internal string GetFilter()
        {
            return GeneralBindingSource.Filter;
        }
    }
}
