using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Registry.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class PremisesListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn premises_num;
        private DataGridViewComboBoxColumn id_premises_type;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn id_state;
        private DataGridViewTextBoxColumn current_fund;
        #endregion Components

        #region Models
        private PremisesDataModel premises;
        private BuildingsDataModel buildings;
        private KladrStreetsDataModel kladr;
        private PremisesTypesDataModel premises_types;
        private ObjectStatesDataModel object_states;
        private CalcDataModelPremisesCurrentFunds premises_funds;
        private CalcDataModelPremisesTenanciesInfo _premisesTenanciesInfo;
        private FundTypesDataModel fund_types;
        #endregion Models

        #region Views
        private BindingSource v_premises;
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
            var Position = v_premises.Find("id_premises", id);
            if (Position > 0)
                v_premises.Position = Position;
        }

        public override int GetRecordCount()
        {
            return v_premises.Count;
        }

        public override bool CanMoveFirst()
        {
            return v_premises.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_premises.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override void MoveFirst()
        {
            v_premises.MoveFirst();
        }

        public override void MovePrev()
        {
            v_premises.MovePrevious();
        }

        public override void MoveNext()
        {
            v_premises.MoveNext();
        }

        public override void MoveLast()
        {
            v_premises.MoveLast();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            premises = PremisesDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();
            buildings = BuildingsDataModel.GetInstance();
            premises_types = PremisesTypesDataModel.GetInstance();
            object_states = ObjectStatesDataModel.GetInstance();
            premises_funds = CalcDataModelPremisesCurrentFunds.GetInstance();
            fund_types = FundTypesDataModel.GetInstance();

            // Ожидаем дозагрузки данных, если это необходимо
            kladr.Select();
            buildings.Select();
            premises_types.Select();
            object_states.Select();
            premises_funds.Select();
            fund_types.Select();

            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                _premisesTenanciesInfo = CalcDataModelPremisesTenanciesInfo.GetInstance();
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
                dataGridView.Columns.Add(residenceWarrantNumColumn);
                dataGridView.Columns.Add(residenceWarrantDateColumn);
                dataGridView.Columns.Add(tenantColumn);
                _premisesTenanciesInfo.RefreshEvent += _premisesTenanciesInfo_RefreshEvent;
            }

            var ds = DataSetManager.DataSet;

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += v_premises_CurrentItemChanged;
            v_premises.DataMember = "premises";
            v_premises.DataSource = ds;
            v_premises.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_premises.Filter += " AND ";
            v_premises.Filter += DynamicFilter;
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

            premises.Select().RowChanged += PremisesListViewport_RowChanged;
            premises.Select().RowDeleted += PremisesListViewport_RowDeleted;
            premises_funds.RefreshEvent += premises_funds_RefreshEvent;
            dataGridView.RowCount = v_premises.Count;

            ViewportHelper.SetDoubleBuffered(dataGridView);
        }
        
        public override bool CanDeleteRecord()
        {
            return (v_premises.Position > -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это помещение?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)v_premises.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)v_premises.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                var id_building = (int)((DataRowView)v_premises[v_premises.Position])["id_building"];
                if (PremisesDataModel.Delete((int)((DataRowView)v_premises.Current)["id_premises"]) == -1)
                    return;
                ((DataRowView)v_premises[v_premises.Position]).Delete();
                MenuCallback.ForceCloseDetachedViewports();
                CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(EntityType.Building, id_building, true);
                CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building, id_building, true);
                CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, true);
                CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, true);
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
            v_premises.Filter = Filter;
            dataGridView.RowCount = v_premises.Count;
        }

        public override void ClearSearch()
        {
            v_premises.Filter = StaticFilter;
            dataGridView.RowCount = v_premises.Count;
            DynamicFilter = "";
        }

        public override bool CanOpenDetails()
        {
            if (v_premises.Position == -1)
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
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return (!premises.EditingNewRecord) &&
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
            return (v_premises.Position != -1) &&
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
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override Viewport Duplicate()
        {
            var viewport = new PremisesListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as int?) ?? -1);
            return viewport;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override bool HasAssocOwnerships()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocSubPremises()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocTenancies()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasRegistryExcerptPremiseReport()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasRegistryExcerptSubPremisesReport()
        {
            return (v_premises.Position > -1);
        }

        public override void RegistryExcerptPremiseReportGenerate()
        {
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)v_premises[v_premises.Position])["id_premises"].ToString());
            arguments.Add("excerpt_type", "1");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremisesReportGenerate()
        {
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)v_premises[v_premises.Position])["id_premises"].ToString());
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
                {"filter", v_premises.Filter.Trim() == "" ? "(1=1)" : v_premises.Filter},
                {"columnHeaders", "["+columnHeaders+"]"},
                {"columnPatterns", "["+columnPatterns+"]"}
            };
            reporter.Run(arguments);
        }

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_premises = " + Convert.ToInt32(((DataRowView)v_premises[v_premises.Position])["id_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)v_premises[v_premises.Position]).Row,
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
            dataGridView.RowCount = v_premises.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void PremisesListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_premises.Count;
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
                v_premises.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
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
                v_premises.Position = dataGridView.SelectedRows[0].Index;
            else
                v_premises.Position = -1;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_premises.Count <= e.RowIndex) return;
            var row = ((DataRowView)v_premises[e.RowIndex]);
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
                    var tenancyInfoRows =
                        from tenancyInfoRow in DataModelHelper.FilterRows(_premisesTenanciesInfo.Select())
                        where tenancyInfoRow.Field<int>("id_premises") == (int?) row["id_premises"]
                        orderby tenancyInfoRow.Field<DateTime?>("registration_date") descending 
                        select tenancyInfoRow;
                    if (!tenancyInfoRows.Any())
                        return;
                    switch (dataGridView.Columns[e.ColumnIndex].Name)
                    {
                        case "registration_date":
                        case "residence_warrant_date":
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
            if (v_premises.Position == -1 || dataGridView.RowCount == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            if (v_premises.Position >= dataGridView.RowCount)
            {
                dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                if (dataGridView.CurrentCell != null)
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[dataGridView.CurrentCell.ColumnIndex];
                else
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridView.Rows[v_premises.Position].Selected = true;
                if (dataGridView.CurrentCell != null)
                    dataGridView.CurrentCell = dataGridView.Rows[v_premises.Position].Cells[dataGridView.CurrentCell.ColumnIndex];
                else
                    dataGridView.CurrentCell = dataGridView.Rows[v_premises.Position].Cells[0];
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

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(PremisesListViewport));
            dataGridView = new DataGridView();
            id_premises = new DataGridViewTextBoxColumn();
            id_street = new DataGridViewTextBoxColumn();
            house = new DataGridViewTextBoxColumn();
            premises_num = new DataGridViewTextBoxColumn();
            id_premises_type = new DataGridViewComboBoxColumn();
            total_area = new DataGridViewTextBoxColumn();
            id_state = new DataGridViewTextBoxColumn();
            current_fund = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_premises, id_street, house, premises_num, id_premises_type, total_area, id_state, current_fund);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(1125, 704);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.CellDoubleClick += dataGridView_CellDoubleClick;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            dataGridView.Resize += dataGridView_Resize;
            // 
            // id_premises
            // 
            id_premises.HeaderText = "№";
            id_premises.MinimumWidth = 100;
            id_premises.Name = "id_premises";
            id_premises.ReadOnly = true;
            // 
            // id_street
            // 
            id_street.HeaderText = "Адрес";
            id_street.MinimumWidth = 250;
            id_street.Name = "id_street";
            id_street.ReadOnly = true;
            id_street.SortMode = DataGridViewColumnSortMode.NotSortable;
            id_street.Width = 250;
            // 
            // house
            // 
            house.HeaderText = "Дом";
            house.MinimumWidth = 100;
            house.Name = "house";
            house.ReadOnly = true;
            house.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // premises_num
            // 
            premises_num.HeaderText = "Помещение";
            premises_num.MinimumWidth = 100;
            premises_num.Name = "premises_num";
            premises_num.ReadOnly = true;
            // 
            // id_premises_type
            // 
            id_premises_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_premises_type.HeaderText = "Тип помещения";
            id_premises_type.MinimumWidth = 150;
            id_premises_type.Name = "id_premises_type";
            id_premises_type.ReadOnly = true;
            id_premises_type.SortMode = DataGridViewColumnSortMode.Automatic;
            id_premises_type.Width = 150;
            // 
            // total_area
            // 
            dataGridViewCellStyle2.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle2;
            total_area.HeaderText = "Общая площадь";
            total_area.MinimumWidth = 140;
            total_area.Name = "total_area";
            total_area.ReadOnly = true;
            total_area.Width = 140;
            // 
            // id_state
            // 
            id_state.HeaderText = "Текущее состояние";
            id_state.MinimumWidth = 170;
            id_state.Name = "id_state";
            id_state.ReadOnly = true;
            id_state.Width = 170;
            // 
            // current_fund
            // 
            current_fund.HeaderText = "Текущий фонд";
            current_fund.MinimumWidth = 170;
            current_fund.Name = "current_fund";
            current_fund.ReadOnly = true;
            current_fund.SortMode = DataGridViewColumnSortMode.NotSortable;
            current_fund.Width = 170;
            // 
            // PremisesListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(1131, 710);
            Controls.Add(dataGridView);
            DoubleBuffered = true;
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "PremisesListViewport";
            Padding = new Padding(3);
            Text = "Перечень помещений";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
