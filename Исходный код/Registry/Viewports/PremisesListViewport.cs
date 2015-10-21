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
            : this(null)
        {
        }

        public PremisesListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
        }

        public PremisesListViewport(PremisesListViewport premisesListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = premisesListViewport.DynamicFilter;
            StaticFilter = premisesListViewport.StaticFilter;
            ParentRow = premisesListViewport.ParentRow;
            ParentType = premisesListViewport.ParentType;
        }

        public void LocatePremisesBy(int id)
        {
            var Position = GeneralBindingSource.Find("id_premises", id);
            if (Position > 0)
                GeneralBindingSource.Position = Position;
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
            GeneralBindingSource.CurrentItemChanged += v_premises_CurrentItemChanged;
            GeneralBindingSource.DataMember = "premises";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                Text = "Помещения здания №" + ParentRow["id_building"];

            v_buildings = new BindingSource();
            v_buildings.DataMember = "buildings";
            v_buildings.DataSource = ds;

            v_premises_types = new BindingSource();
            v_premises_types.DataMember = "premises_types";
            v_premises_types.DataSource = ds;

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

        public override void Close()
        {
            if (_premisesTenanciesInfo != null)
                _premisesTenanciesInfo.RefreshEvent -= _premisesTenanciesInfo_RefreshEvent;
            base.Close();
        }

        public override void ForceClose()
        {
            if (_premisesTenanciesInfo != null)
                _premisesTenanciesInfo.RefreshEvent -= _premisesTenanciesInfo_RefreshEvent;
            base.ForceClose();
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
            var viewport = new PremisesViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            var viewport = new PremisesViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
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
            var viewport = new PremisesViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override Viewport Duplicate()
        {
            var viewport = new PremisesListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (GeneralBindingSource.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
            return viewport;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override bool HasAssocOwnerships()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocSubPremises()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocTenancies()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasRegistryExcerptPremiseReport()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasRegistryExcerptSubPremisesReport()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override void RegistryExcerptPremiseReportGenerate()
        {
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"].ToString());
            arguments.Add("excerpt_type", "1");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremisesReportGenerate()
        {
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"].ToString());
            arguments.Add("excerpt_type", "3");
            reporter.Run(arguments);
        }

        public override void ShowOwnerships()
        {
            ShowAssocViewport(ViewportType.OwnershipListViewport);
        }

        public override void ShowRestrictions()
        {
            ShowAssocViewport(ViewportType.RestrictionListViewport);
        }

        public override void ShowSubPremises()
        {
            ShowAssocViewport(ViewportType.SubPremisesViewport);
        }

        public override void ShowFundHistory()
        {
            ShowAssocViewport(ViewportType.FundsHistoryViewport);
        }

        public override void ShowTenancies()
        {
            ShowAssocViewport(ViewportType.TenancyListViewport);
        }

        public override bool HasExportToOds()
        {
            return true;
        }

        public override void ExportToOds()
        {
            var reporter = ReporterFactory.CreateReporter(ReporterType.ExportReporter);
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
            reporter.Run(arguments);
        }

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_premises = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Premises);
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
                        orderby tenancyInfoRow.Field<DateTime?>("registration_date") descending 
                        select tenancyInfoRow;
                    if (!tenancyInfoRows.Any())
                        return;
                    switch (dataGridView.Columns[e.ColumnIndex].Name)
                    {
                        case "registration_date":
                        case "residence_warrant_date":
                        case "end_date":
                            var date = tenancyInfoRows.First().Field<DateTime?>(dataGridView.Columns[e.ColumnIndex].Name);
                            e.Value =date != null ? date.Value.ToString("dd.MM.yyyy") : null;
                            break;
                        case "registration_num":
                        case "residence_warrant_num":
                        case "tenant":
                                e.Value = tenancyInfoRows.First().Field<string>(dataGridView.Columns[e.ColumnIndex].Name);
                            break;
                    }
                    break;
            }
        }

        void v_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || dataGridView.RowCount == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            if (GeneralBindingSource.Position >= dataGridView.RowCount)
            {
                dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                if (dataGridView.CurrentCell != null)
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[dataGridView.CurrentCell.ColumnIndex];
                else
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                if (dataGridView.CurrentCell != null)
                    dataGridView.CurrentCell = dataGridView.Rows[GeneralBindingSource.Position].Cells[dataGridView.CurrentCell.ColumnIndex];
                else
                    dataGridView.CurrentCell = dataGridView.Rows[GeneralBindingSource.Position].Cells[0];
            }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.EditingStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.DocumentsStateUpdate();
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1150)
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
    }
}
