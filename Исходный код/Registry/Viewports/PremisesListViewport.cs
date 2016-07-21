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
    internal sealed partial class PremisesListViewport : DataGridViewport
    {

        #region Models
        private DataModel _buildings;
        private DataModel _kladr;
        private DataModel _premisesTypes;
        private DataModel _objectStates;
        private CalcDataModel _premisesFunds;
        private CalcDataModel _premisesTenanciesInfo;
        private DataModel _fundTypes;
        #endregion Models

        #region Views
        private BindingSource _vPremisesTypes;
        #endregion Views

        //Forms
        private SearchForm _spExtendedSearchForm;
        private SearchForm _spSimpleSearchForm;

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
            GeneralDataModel = DataModel.GetInstance<PremisesDataModel>();
            _kladr = DataModel.GetInstance<KladrStreetsDataModel>();
            _buildings = DataModel.GetInstance<BuildingsDataModel>();
            _premisesTypes = DataModel.GetInstance<PremisesTypesDataModel>();
            _objectStates = DataModel.GetInstance<ObjectStatesDataModel>();
            _fundTypes = DataModel.GetInstance<FundTypesDataModel>();
            _premisesFunds = CalcDataModel.GetInstance<CalcDataModelPremisesCurrentFunds>();

            // Ожидаем дозагрузки данных, если это необходимо
            _kladr.Select();
            _buildings.Select();
            _premisesTypes.Select();
            _objectStates.Select();
            _premisesFunds.Select();
            _fundTypes.Select();

            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                _premisesTenanciesInfo = CalcDataModel.GetInstance<CalcDataModelPremisesTenanciesInfo>();
                _premisesTenanciesInfo.Select();
                var registrationNumColumn = new DataGridViewTextBoxColumn
                {
                    Name = "registration_num",
                    HeaderText = @"№ договора найма",
                    Width = 80,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var registrationDateColumn = new DataGridViewTextBoxColumn
                {
                    Name = "registration_date",
                    HeaderText = @"Дата договора найма",
                    Width = 80,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var endDateColumn = new DataGridViewTextBoxColumn
                {
                    Name = "end_date",
                    HeaderText = @"Дата окончания договора",
                    Width = 80,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var residenceWarrantNumColumn = new DataGridViewTextBoxColumn
                {
                    Name = "residence_warrant_num",
                    HeaderText = @"№ ордера найма",
                    Width = 80,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var residenceWarrantDateColumn = new DataGridViewTextBoxColumn
                {
                    Name = "residence_warrant_date",
                    HeaderText = @"Дата ордера найма",
                    Width = 80,
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
            {
                Text = @"Помещения здания №" + ParentRow["id_building"];
            }
            else
            if ((ParentRow != null) && (ParentType == ParentTypeEnum.PaymentAccount))
            {
                Text = string.Format("Помещения по лицевому счету №{0}", ParentRow["account"]);
            }

            _vPremisesTypes = new BindingSource
            {
                DataMember = "premises_types",
                DataSource = ds
            };

            id_premises_type.DataSource = _vPremisesTypes;
            id_premises_type.ValueMember = "id_premises_type";
            id_premises_type.DisplayMember = "premises_type";

            GeneralDataModel.Select().RowChanged += PremisesListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += PremisesListViewport_RowDeleted;
            _premisesFunds.RefreshEvent += premises_funds_RefreshEvent;
            DataModel.GetInstance<OwnershipBuildingsAssocDataModel>().Select().RowChanged += BuildingsOwnershipChanged;
            DataModel.GetInstance<OwnershipBuildingsAssocDataModel>().Select().RowDeleted += BuildingsOwnershipChanged;
            DataModel.GetInstance<OwnershipPremisesAssocDataModel>().Select().RowChanged += PremisesOwnershipChanged;
            DataModel.GetInstance<OwnershipPremisesAssocDataModel>().Select().RowDeleted += PremisesOwnershipChanged;
            dataGridView.RowCount = GeneralBindingSource.Count;

            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        private void BuildingsOwnershipChanged(object sender, DataRowChangeEventArgs dataRowChangeEventArgs)
        {
            _demolishedBuildings = DataModelHelper.DemolishedBuildingIDs().ToList();
            dataGridView.Refresh();
        }

        private void PremisesOwnershipChanged(object sender, DataRowChangeEventArgs dataRowChangeEventArgs)
        {
            _demolishedPremises = DataModelHelper.DemolishedPremisesIDs().ToList();
            dataGridView.Refresh();
        }


        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это помещение?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
            if (_premisesFunds != null)
                _premisesFunds.RefreshEvent -= premises_funds_RefreshEvent;
            DataModel.GetInstance<OwnershipBuildingsAssocDataModel>().Select().RowChanged -= BuildingsOwnershipChanged;
            DataModel.GetInstance<OwnershipBuildingsAssocDataModel>().Select().RowDeleted -= BuildingsOwnershipChanged;
            DataModel.GetInstance<OwnershipPremisesAssocDataModel>().Select().RowChanged -= PremisesOwnershipChanged;
            DataModel.GetInstance<OwnershipPremisesAssocDataModel>().Select().RowDeleted -= PremisesOwnershipChanged;
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
                    if (_spSimpleSearchForm == null)
                        _spSimpleSearchForm = new SimpleSearchPremiseForm();
                    if (_spSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _spSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (_spExtendedSearchForm == null)
                        _spExtendedSearchForm = new ExtendedSearchPremisesForm();
                    if (_spExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _spExtendedSearchForm.GetFilter();
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
            return GeneralBindingSource.Position != -1;
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

        public override bool HasAssocViewport<T>()
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
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport<T>()
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, 
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
                {"columnHeaders", "["+columnHeaders+",{\"columnHeader\":\"Дополнительные сведения\"}]"},
                {"columnPatterns", "["+columnPatterns+",{\"columnPattern\":\"$description$\"}]"}
            };
            return arguments;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (CanOpenDetails())
                OpenDetails();
        }

        private void premises_funds_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        private void PremisesListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            _idPremises = int.MinValue;
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void PremisesListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            _idPremises = int.MinValue;
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = GeneralBindingSource.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
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

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        private int _idPremises = int.MinValue;
        private IEnumerable<DataRow> _tenancyInfoRows;
        private IEnumerable<int> _demolishedBuildings = DataModelHelper.DemolishedBuildingIDs().ToList();
        private IEnumerable<int> _demolishedPremises = DataModelHelper.DemolishedPremisesIDs().ToList();

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            var row = ((DataRowView)GeneralBindingSource[e.RowIndex]);
            var buildingRow = _buildings.Select().Rows.Find(row["id_building"]);
            if (buildingRow == null)
                return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_premises":
                    e.Value = row["id_premises"];
                    if (_demolishedBuildings.Contains((int) row["id_building"]) ||
                        _demolishedPremises.Contains((int) row["id_premises"]))
                    {
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.DarkRed;
                    }
                    else
                    {
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = SystemColors.Highlight;
                    }
                    break;
                case "id_street":
                    var kladrRow = _kladr.Select().Rows.Find(buildingRow["id_street"]);
                    string streetName = null;
                    if (kladrRow != null)
                        streetName = kladrRow["street_name"].ToString();
                    e.Value = streetName;
                    break;
                case "house":
                    e.Value = buildingRow["house"];
                    break;
                case "premises_num":
                case "id_premises_type":
                case "total_area":
                    e.Value = row[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "id_state":
                    var stateRow = _objectStates.Select().Rows.Find(row["id_state"]);
                    if (stateRow != null)
                        e.Value = stateRow["state_female"];
                    break;
                case "current_fund":
                    if (DataModelHelper.MunicipalAndUnknownObjectStates().ToList().Contains((int)row["id_state"]))
                    {
                        var fundRow = _premisesFunds.Select().Rows.Find(row["id_premises"]);
                        if (fundRow != null)
                            e.Value = _fundTypes.Select().Rows.Find(fundRow["id_fund_type"])["fund_type"];
                    }
                    break;
                case "registration_num":
                case "registration_date":
                case "residence_warrant_num":
                case "residence_warrant_date":
                case "tenant":
                case "end_date":
                    if ((int)row["id_premises"] != _idPremises || _tenancyInfoRows.Any(entry => entry.RowState == DataRowState.Deleted || entry.RowState == DataRowState.Detached))
                    {   
                        _tenancyInfoRows =
                            (from tenancyInfoRow in _premisesTenanciesInfo.FilterDeletedRows()
                            where tenancyInfoRow.Field<int>("id_premises") == (int?) row["id_premises"]
                            orderby tenancyInfoRow.Field<DateTime?>("registration_date") ?? 
                                DateTime.Now descending 
                            select tenancyInfoRow).ToList();
                        _idPremises = (int)row["id_premises"];
                    }
                    if (_tenancyInfoRows == null || !_tenancyInfoRows.Any())
                    {                        
                        break;
                    }
                        
                    var tenancyRow = _tenancyInfoRows.First();
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

        private void _premisesTenanciesInfo_RefreshEvent(object sender, EventArgs e)
        {
            _idPremises = int.MinValue;
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

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if ((AccessControl.HasPrivelege(Priveleges.TenancyRead) && dataGridView.Size.Width > 1495) ||
                (!AccessControl.HasPrivelege(Priveleges.TenancyRead) && dataGridView.Size.Width > 845))
            {
                premises_num.Frozen = false;
                house.Frozen = false;
                id_street.Frozen = false;
                id_premises.Frozen = false;
                if (id_street.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                premises_num.Frozen = true;
                house.Frozen = true;
                id_street.Frozen = true;
                id_premises.Frozen = true;
                if (id_street.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }
    }
}
