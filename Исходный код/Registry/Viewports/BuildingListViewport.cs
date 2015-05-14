using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;
using System.Threading;
using Registry.SearchForms;
using Registry.CalcDataModels;
using Security;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class BuildingListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_building;
        private DataGridViewComboBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn floors;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn cadastral_num;
        private DataGridViewTextBoxColumn startup_year;
        #endregion Components

        #region Models
        private BuildingsDataModel buildings = null;
        private KladrStreetsDataModel kladr = null;
        #endregion

        #region Views
        private BindingSource v_buildings = null;
        private BindingSource v_kladr = null;
        #endregion

        //Forms
        private SearchForm sbSimpleSearchForm = null;
        private SearchForm sbExtendedSearchForm = null;

        private BuildingListViewport()
            : this(null)
        {
        }

        public BuildingListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
        }

        public BuildingListViewport(BuildingListViewport buildingListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = buildingListViewport.DynamicFilter;
            this.StaticFilter = buildingListViewport.StaticFilter;
            this.ParentRow = buildingListViewport.ParentRow;
            this.ParentType = buildingListViewport.ParentType;
        }

        public void LocateBuildingBy(int id)
        {
            int Position = v_buildings.Find("id_building", id);
            if (Position > 0)
                v_buildings.Position = Position;
        }

        public override int GetRecordCount()
        {
            return v_buildings.Count;
        }

        public override bool CanMoveFirst()
        {
            return v_buildings.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_buildings.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_buildings.Position > -1) && (v_buildings.Position < (v_buildings.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_buildings.Position > -1) && (v_buildings.Position < (v_buildings.Count - 1));
        }

        public override void MoveFirst()
        {
            v_buildings.MoveFirst();
        }

        public override void MovePrev()
        {
            v_buildings.MovePrevious();
        }

        public override void MoveNext()
        {
            v_buildings.MoveNext();
        }

        public override void MoveLast()
        {
            v_buildings.MoveLast();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            buildings = BuildingsDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();
            // Ожидаем дозагрузки данных, если это необходимо
            buildings.Select();
            kladr.Select();

            DataSet ds = DataSetManager.DataSet;

            v_buildings = new BindingSource();
            v_buildings.DataMember = "buildings";
            v_buildings.CurrentItemChanged += new EventHandler(v_buildings_CurrentItemChanged);
            v_buildings.DataSource = ds;
            v_buildings.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_buildings.Filter += " AND ";
            v_buildings.Filter += DynamicFilter;

            v_kladr = new BindingSource();
            v_kladr.DataMember = "kladr";
            v_kladr.DataSource = ds;

            id_street.DataSource = v_kladr;
            id_street.ValueMember = "id_street";
            id_street.DisplayMember = "street_name";

            buildings.Select().RowChanged += new DataRowChangeEventHandler(BuildingListViewport_RowChanged);
            buildings.Select().RowDeleted += new DataRowChangeEventHandler(BuildingListViewport_RowDeleted);
            dataGridView.RowCount = v_buildings.Count;
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            if (!String.IsNullOrEmpty(DynamicFilter))
                return true;
            else
                return false;
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (sbSimpleSearchForm == null)
                        sbSimpleSearchForm = new SimpleSearchBuildingForm();
                    if (sbSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = sbSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (sbExtendedSearchForm == null)
                        sbExtendedSearchForm = new ExtendedSearchBuildingForm();
                    if (sbExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = sbExtendedSearchForm.GetFilter();
                    break;
            }
            string Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            dataGridView.RowCount = 0;
            v_buildings.Filter = Filter;
            dataGridView.RowCount = v_buildings.Count;
        }

        public override void ClearSearch()
        {
            v_buildings.Filter = StaticFilter;
            dataGridView.RowCount = v_buildings.Count;
            DynamicFilter = "";
        }

        public override bool CanOpenDetails()
        {
            if (v_buildings.Position == -1)
                return false;
            else
                return true;
        }

        public override void OpenDetails()
        {
            BuildingViewport viewport = new BuildingViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanDeleteRecord()
        {
            return (v_buildings.Position > -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это здание?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)v_buildings.Current)["id_building"], EntityType.Building)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление муниципальных жилых зданий и зданий, в которых присутствуют муниципальные помещения",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)v_buildings.Current)["id_building"], EntityType.Building)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление немуниципальных жилых зданий и зданий, в которых присутствуют немуниципальные помещения",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (BuildingsDataModel.Delete((int)((DataRowView)v_buildings.Current)["id_building"]) == -1)
                    return;
                ((DataRowView)v_buildings[v_buildings.Position]).Delete();
                MenuCallback.ForceCloseDetachedViewports();
                CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, true);
                CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, true);
            }
        }

        public override bool CanInsertRecord()
        {
            return (!buildings.EditingNewRecord) && 
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            BuildingViewport viewport = new BuildingViewport(MenuCallback);
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
            return (v_buildings.Position != -1) && (!buildings.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            BuildingViewport viewport = new BuildingViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            BuildingListViewport viewport = new BuildingListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool HasAssocPremises()
        {
            return (v_buildings.Position > -1);
        }

        public override bool HasAssocOwnerships()
        {
            return (v_buildings.Position > -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (v_buildings.Position > -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (v_buildings.Position > -1);
        }

        public override bool HasAssocTenancies()
        {
            return (v_buildings.Position > -1);
        }

        public override void ShowPremises()
        {
            ShowAssocViewport(ViewportType.PremisesListViewport);
        }

        public override void ShowOwnerships()
        {
            ShowAssocViewport(ViewportType.OwnershipListViewport);
        }

        public override void ShowRestrictions()
        {
            ShowAssocViewport(ViewportType.RestrictionListViewport);
        }

        public override void ShowFundHistory()
        {
            ShowAssocViewport(ViewportType.FundsHistoryViewport);
        }

        public override void ShowTenancies()
        {
            ShowAssocViewport(ViewportType.TenancyListViewport);
        }

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения истории найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"], CultureInfo.InvariantCulture),
                ((DataRowView)v_buildings[v_buildings.Position]).Row,
                ParentTypeEnum.Building);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            buildings.Select().RowChanged -= new DataRowChangeEventHandler(BuildingListViewport_RowChanged);
            buildings.Select().RowDeleted -= new DataRowChangeEventHandler(BuildingListViewport_RowDeleted);
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
                v_buildings.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            if (dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
            dataGridView.Refresh();
        }

        void BuildingListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.RowCount = v_buildings.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void BuildingListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_buildings.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                v_buildings.Position = dataGridView.SelectedRows[0].Index;
            else
                v_buildings.Position = -1;
            dataGridView.Refresh();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_buildings.Count <= e.RowIndex) return;
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_building":
                    e.Value = ((DataRowView)v_buildings[e.RowIndex])["id_building"];
                    break;
                case "id_street":
                    e.Value = ((DataRowView)v_buildings[e.RowIndex])["id_street"];
                    break;
                case "house":
                    e.Value = ((DataRowView)v_buildings[e.RowIndex])["house"];
                    break;
                case "floors":
                    e.Value = ((DataRowView)v_buildings[e.RowIndex])["floors"];
                    break;
                case "living_area":
                    e.Value = ((DataRowView)v_buildings[e.RowIndex])["living_area"];
                    break;
                case "cadastral_num":
                    e.Value = ((DataRowView)v_buildings[e.RowIndex])["cadastral_num"];
                    break;
                case "startup_year":
                    e.Value = ((DataRowView)v_buildings[e.RowIndex])["startup_year"];
                    break;
            }
        }

        void v_buildings_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_buildings.Position == -1 || dataGridView.RowCount == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            if (v_buildings.Position >= dataGridView.RowCount)
            {
                dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridView.Rows[v_buildings.Position].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[v_buildings.Position].Cells[0];
            }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1100)
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

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildingListViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_building = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_street = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.house = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.floors = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startup_year = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_building,
            this.id_street,
            this.house,
            this.floors,
            this.living_area,
            this.cadastral_num,
            this.startup_year});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(1099, 723);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // id_building
            // 
            this.id_building.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_building.HeaderText = "№";
            this.id_building.MinimumWidth = 100;
            this.id_building.Name = "id_building";
            this.id_building.ReadOnly = true;
            // 
            // id_street
            // 
            this.id_street.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_street.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_street.HeaderText = "Адрес";
            this.id_street.MinimumWidth = 250;
            this.id_street.Name = "id_street";
            this.id_street.ReadOnly = true;
            this.id_street.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.id_street.Width = 250;
            // 
            // house
            // 
            this.house.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.house.HeaderText = "Дом";
            this.house.MinimumWidth = 100;
            this.house.Name = "house";
            this.house.ReadOnly = true;
            // 
            // floors
            // 
            this.floors.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.floors.HeaderText = "Этажность";
            this.floors.MinimumWidth = 100;
            this.floors.Name = "floors";
            this.floors.ReadOnly = true;
            // 
            // living_area
            // 
            this.living_area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            this.living_area.DefaultCellStyle = dataGridViewCellStyle2;
            this.living_area.HeaderText = "Жилая площадь";
            this.living_area.MinimumWidth = 150;
            this.living_area.Name = "living_area";
            this.living_area.ReadOnly = true;
            this.living_area.Width = 150;
            // 
            // cadastral_num
            // 
            this.cadastral_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cadastral_num.HeaderText = "Кадастровый номер";
            this.cadastral_num.MinimumWidth = 170;
            this.cadastral_num.Name = "cadastral_num";
            this.cadastral_num.ReadOnly = true;
            this.cadastral_num.Width = 170;
            // 
            // startup_year
            // 
            this.startup_year.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.startup_year.HeaderText = "Год ввода в эксплуатацию";
            this.startup_year.MinimumWidth = 190;
            this.startup_year.Name = "startup_year";
            this.startup_year.ReadOnly = true;
            this.startup_year.Width = 190;
            // 
            // BuildingListViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1105, 729);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BuildingListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень зданий";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

    }
}
