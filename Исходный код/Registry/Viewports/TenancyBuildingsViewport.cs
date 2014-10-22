using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.SearchForms;
using System.Data;
using Registry.Entities;
using System.Text.RegularExpressions;
using Registry.CalcDataModels;

namespace Registry.Viewport
{
    internal sealed class TenancyBuildingsViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewCheckBoxColumn field_checked = new DataGridViewCheckBoxColumn();
        private DataGridViewTextBoxColumn field_beds = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_id_building = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_street = new DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_house = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_floors = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_living_area = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_cadastral_num = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_startup_year = new DataGridViewTextBoxColumn();
        #endregion Components

        //Models
        private BuildingsDataModel buildings = null;
        private KladrStreetsDataModel kladr = null;
        private TenancyBuildingsAssocDataModel tenancy_buildings = null;
        private DataTable snapshot_tenancy_buildings = null;

        //Views
        private BindingSource v_buildings = null;
        private BindingSource v_kladr = null;
        private BindingSource v_tenancy_buildings = null;
        private BindingSource v_snapshot_tenancy_buildings = null;

        //Forms
        private SearchForm sbSimpleSearchForm = null;
        private SearchForm sbExtendedSearchForm = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public TenancyBuildingsViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageTenancies";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Здания по процессу найма №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public TenancyBuildingsViewport(TenancyBuildingsViewport tenancyBuildingsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = tenancyBuildingsViewport.DynamicFilter;
            this.StaticFilter = tenancyBuildingsViewport.StaticFilter;
            this.ParentRow = tenancyBuildingsViewport.ParentRow;
            this.ParentType = tenancyBuildingsViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            buildings = BuildingsDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();
            tenancy_buildings = TenancyBuildingsAssocDataModel.GetInstance();
            // Ожидаем дозагрузки данных, если это необходимо
            buildings.Select();
            kladr.Select();
            tenancy_buildings.Select();

            // Инициализируем snapshot-модель
            snapshot_tenancy_buildings = new DataTable("selected_buildings");
            snapshot_tenancy_buildings.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_tenancy_buildings.Columns.Add("id_building").DataType = typeof(int);
            snapshot_tenancy_buildings.Columns.Add("checked").DataType = typeof(bool);
            snapshot_tenancy_buildings.Columns.Add("beds").DataType = typeof(string);

            DataSet ds = DataSetManager.GetDataSet();

            v_buildings = new BindingSource();
            v_buildings.DataMember = "buildings";
            v_buildings.CurrentItemChanged += new EventHandler(v_buildings_CurrentItemChanged);
            v_buildings.DataSource = ds;
            v_buildings.Filter = DynamicFilter;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = "Здания найма №" + ParentRow["id_contract"].ToString();
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_kladr = new BindingSource();
            v_kladr.DataMember = "kladr";
            v_kladr.DataSource = ds;

            v_tenancy_buildings = new BindingSource();
            v_tenancy_buildings.DataMember = "tenancy_buildings_assoc";
            v_tenancy_buildings.Filter = StaticFilter;
            v_tenancy_buildings.DataSource = ds;

            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_tenancy_buildings.Count; i++)
                snapshot_tenancy_buildings.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_buildings[i])));
            v_snapshot_tenancy_buildings = new BindingSource();
            v_snapshot_tenancy_buildings.DataSource = snapshot_tenancy_buildings;

            field_id_street.DataSource = v_kladr;
            field_id_street.ValueMember = "id_street";
            field_id_street.DisplayMember = "street_name";

            v_buildings.PositionChanged += new EventHandler(v_buildings_PositionChanged);
            buildings.Select().RowChanged += new DataRowChangeEventHandler(BuildingsViewport_RowChanged);
            buildings.Select().RowDeleted += new DataRowChangeEventHandler(BuildingsViewport_RowDeleted);
            tenancy_buildings.Select().RowChanged += new DataRowChangeEventHandler(TenancyBuildingsViewport_RowChanged);
            tenancy_buildings.Select().RowDeleting += new DataRowChangeEventHandler(TenancyBuildingsViewport_RowDeleting);
            dataGridView.CellValueNeeded += new DataGridViewCellValueEventHandler(dataGridView_CellValueNeeded);
            dataGridView.CellValuePushed += new DataGridViewCellValueEventHandler(dataGridView_CellValuePushed);
            dataGridView.SelectionChanged += new EventHandler(dataGridView_SelectionChanged);
            dataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dataGridView_ColumnHeaderMouseClick);
            dataGridView.RowCount = v_buildings.Count;
        }

        void TenancyBuildingsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_contract"]) != Convert.ToInt32(ParentRow["id_contract"]))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_tenancy_buildings.Find("id_building", e.Row["id_building"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_tenancy_buildings[row_index]).Delete();
            }
            dataGridView.Refresh();
        }

        void TenancyBuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_contract"]) != Convert.ToInt32(ParentRow["id_contract"]))
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_tenancy_buildings.Find("id_building", e.Row["id_building"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_tenancy_buildings[row_index]);
                    row["beds"] = e.Row["beds"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    int row_index = v_tenancy_buildings.Find("id_assoc", e.Row["id_assoc"]);
                    if (row_index != -1)
                        snapshot_tenancy_buildings.Rows.Add(new object[] { 
                            e.Row["id_assoc"],
                            e.Row["id_building"], 
                            true,   
                            e.Row["beds"]
                        });
                }
            dataGridView.Refresh();
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_assoc"],
                dataRowView["id_building"], 
                true, 
                dataRowView["beds"]
            };
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

        void BuildingsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            dataGridView.RowCount = v_buildings.Count;
            dataGridView.Refresh();
        }

        void BuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_buildings.Count;
        }

        void v_buildings_PositionChanged(object sender, EventArgs e)
        {
            if (v_buildings.Position == -1 || dataGridView.Rows.Count == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            dataGridView.Rows[v_buildings.Position].Selected = true;
            dataGridView.CurrentCell = dataGridView.Rows[v_buildings.Position].Cells[0];
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                v_buildings.Position = dataGridView.SelectedRows[0].Index;
            else
                v_buildings.Position = -1;
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            int id_building = Convert.ToInt32(((DataRowView)v_buildings[e.RowIndex])["id_building"]);
            int row_index = v_snapshot_tenancy_buildings.Find("id_building", id_building);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "checked":
                    if (row_index == -1)
                        snapshot_tenancy_buildings.Rows.Add(new object[] { null, id_building, e.Value, null });
                    else
                         ((DataRowView)v_snapshot_tenancy_buildings[row_index])["checked"] = e.Value;
                    break;
                case "beds":
                    if (row_index == -1)
                        snapshot_tenancy_buildings.Rows.Add(new object[] { null, id_building, false, e.Value == null ? null : e.Value.ToString().ToLower() });
                    else
                        ((DataRowView)v_snapshot_tenancy_buildings[row_index])["beds"] = e.Value == null ? null : e.Value.ToString().ToLower();
                    break;
            }
            sync_views = true;
            menuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_buildings.Count <= e.RowIndex || v_buildings.Count == 0) return;
            int id_building = Convert.ToInt32(((DataRowView)v_buildings[e.RowIndex])["id_building"]);
            int row_index = v_snapshot_tenancy_buildings.Find("id_building", id_building);
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "checked":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_buildings[row_index])["checked"];
                    break;
                case "beds":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_buildings[row_index])["beds"];
                    if (e.Value != null && e.Value.ToString() != "" && !Regex.IsMatch(e.Value.ToString().ToLower(), "^[0-9а-яА-Я]+([,][0-9а-яА-Я]*)*$"))
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Некорректное значение для номеров койко-мест";
                    else
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
                    break;
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
                    e.Value = ((DataRowView)v_buildings[e.RowIndex])["cadastral_num"];
                    break;
            }
        }

        public void LocateBuildingBy(int id)
        {
            v_buildings.Position = v_buildings.Find("id_building", id);
        }

        void v_buildings_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
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

        public override bool CanDeleteRecord()
        {
            if (v_buildings.Position == -1)
                return false;
            else
                return true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это здание?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (buildings.Delete((int)((DataRowView)v_buildings.Current)["id_building"]) == -1)
                    return;
                ((DataRowView)v_buildings[v_buildings.Position]).Delete();
                menuCallback.ForceCloseDetachedViewports();
            }
        }

        public override int GetRecordCount()
        {
            return v_buildings.Count;
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            if (DynamicFilter != "")
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
            dataGridView.RowCount = 0;
            v_buildings.Filter = DynamicFilter;
            dataGridView.RowCount = v_buildings.Count;

        }

        public override void ClearSearch()
        {
            v_buildings.Filter = "";
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
            BuildingViewport viewport = new BuildingViewport(menuCallback);
            viewport.StaticFilter = "";
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
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

        public override void ShowPremises()
        {
            PremisesListViewport viewport = new PremisesListViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowOwnerships()
        {
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения ограничений", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            OwnershipListViewport viewport = new OwnershipListViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowRestrictions()
        {
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения реквизитов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RestrictionListViewport viewport = new RestrictionListViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowFundHistory()
        {
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения истории найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FundsHistoryViewport viewport = new FundsHistoryViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            if (!buildings.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            BuildingViewport viewport = new BuildingViewport(menuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            viewport.InsertRecord();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override bool CanCopyRecord()
        {
            if ((v_buildings.Position != -1) && (!buildings.EditingNewRecord))
                return true;
            else
                return false;
        }

        public override void CopyRecord()
        {
            BuildingViewport viewport = new BuildingViewport(menuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
            viewport.CopyRecord();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else return;
            }
            buildings.Select().RowChanged -= new DataRowChangeEventHandler(BuildingsViewport_RowChanged);
            buildings.Select().RowDeleted -= new DataRowChangeEventHandler(BuildingsViewport_RowDeleted);
            tenancy_buildings.Select().RowChanged -= new DataRowChangeEventHandler(TenancyBuildingsViewport_RowChanged);
            tenancy_buildings.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyBuildingsViewport_RowDeleting);
            base.Close();
        }

        public override void ForceClose()
        {
            buildings.Select().RowChanged -= new DataRowChangeEventHandler(BuildingsViewport_RowChanged);
            buildings.Select().RowDeleted -= new DataRowChangeEventHandler(BuildingsViewport_RowDeleted);
            tenancy_buildings.Select().RowChanged -= new DataRowChangeEventHandler(TenancyBuildingsViewport_RowChanged);
            tenancy_buildings.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyBuildingsViewport_RowDeleting);
            base.ForceClose();
        }

        public override Viewport Duplicate()
        {
            TenancyBuildingsViewport viewport = new TenancyBuildingsViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        private bool SnapshotHasChanges()
        {
            List<TenancyObject> list_from_view = TenancyBuildingsFromView();
            List<TenancyObject> list_from_viewport = TenancyBuildingsFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            bool founded = false;
            for (int i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (int j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private List<TenancyObject> TenancyBuildingsFromViewport()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < snapshot_tenancy_buildings.Rows.Count; i++)
            {
                DataRow row = snapshot_tenancy_buildings.Rows[i];
                if (Convert.ToBoolean(row["checked"]) == false)
                    continue;
                TenancyObject to = new TenancyObject();
                to.id_assoc = row["id_assoc"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_assoc"]);
                to.id_contract = (ParentType == ParentTypeEnum.Tenancy) ? (int?)Convert.ToInt32(ParentRow["id_contract"]) : null;
                to.id_object = row["id_building"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_building"]);
                to.beds = row["beds"] == DBNull.Value ? null : row["beds"].ToString();
                list.Add(to);
            }
            return list;
        }

        private List<TenancyObject> TenancyBuildingsFromView()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < v_tenancy_buildings.Count; i++)
            {
                TenancyObject to = new TenancyObject();
                DataRowView row = ((DataRowView)v_tenancy_buildings[i]);
                to.id_assoc = row["id_assoc"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_assoc"]);
                to.id_contract = row["id_contract"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_contract"]);
                to.id_object = row["id_building"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_building"]);
                to.beds = row["beds"] == DBNull.Value ? null : row["beds"].ToString();
                list.Add(to);
            }
            return list;
        }

        public override void CancelRecord()
        {
            snapshot_tenancy_buildings.Clear();
            for (int i = 0; i < v_tenancy_buildings.Count; i++)
                snapshot_tenancy_buildings.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_buildings[i])));
            dataGridView.Refresh();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        private bool ValidateTenancyBuildings(List<TenancyObject> tenancyBuildings)
        {
            foreach (TenancyObject tenancyBuilding in tenancyBuildings)
                if ((tenancyBuilding.beds != null) && !Regex.IsMatch(tenancyBuilding.beds, "^[0-9а-яА-Я]+([,][0-9а-яА-Я]*)*$"))
                {
                    MessageBox.Show("Некорректное значение для номеров койко-мест", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<TenancyObject> list = TenancyBuildingsFromViewport();
            if (!ValidateTenancyBuildings(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (((TenancyObject)list[i]).id_assoc != null)
                    row = tenancy_buildings.Select().Rows.Find(list[i].id_assoc);
                if (row == null)
                {
                    int id_assoc = tenancy_buildings.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_buildings[
                        v_snapshot_tenancy_buildings.Find("id_building", list[i].id_object)])["id_assoc"] = id_assoc;
                    tenancy_buildings.Select().Rows.Add(new object[] { 
                        id_assoc, list[i].id_object, list[i].id_contract, list[i].beds, 0
                    });
                }
                else
                {
                    if (RowToTenancyBuilding(row) == list[i])
                        continue;
                    if (tenancy_buildings.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["beds"] = list[i].beds == null ? DBNull.Value : (object)list[i].beds;
                }
            }
            list = TenancyBuildingsFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < v_snapshot_tenancy_buildings.Count; j++)
                {
                    DataRowView row = (DataRowView)v_snapshot_tenancy_buildings[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        (row["id_assoc"].ToString() != "") &&
                        ((int)row["id_assoc"] == list[i].id_assoc) &&
                        (Convert.ToBoolean(row["checked"]) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    if (tenancy_buildings.Delete(list[i].id_assoc.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    int snapshot_row_index = v_snapshot_tenancy_buildings.Find("id_assoc",list[i].id_assoc);
                    if (snapshot_row_index != -1)
                    {
                        int building_row_index = v_buildings.Find("id_building", list[i].id_object);
                        ((DataRowView)v_snapshot_tenancy_buildings[snapshot_row_index]).Delete();
                        dataGridView.InvalidateRow(building_row_index);
                    }
                    tenancy_buildings.Select().Rows.Find(((TenancyObject)list[i]).id_assoc).Delete();
                }
            }
            sync_views = true;
            if (ParentType == ParentTypeEnum.Tenancy)
                CalcDataModeTenancyAggregated.GetInstance().Refresh(CalcDataModelFilterEnity.Tenancy, (int)ParentRow["id_contract"]);
        }

        private TenancyObject RowToTenancyBuilding(DataRow row)
        {
            TenancyObject to = new TenancyObject();
            to.id_assoc = row["id_assoc"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_assoc"]);
            to.id_contract = row["id_contract"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_contract"]);
            to.id_object = row["id_building"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_building"]);
            to.beds = row["beds"] == DBNull.Value ? null : row["beds"].ToString();
            return to;
        }

        private void ConstructViewport()
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).BeginInit();
            this.Controls.Add(dataGridView);
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = 
                new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                field_checked, 
                field_beds,
                field_id_building,
                field_id_street,
                field_house,
                field_floors,
                field_living_area,
                field_cadastral_num,
                field_startup_year});
            dataGridView.Location = new System.Drawing.Point(6, 6);
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView.TabIndex = 0;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.MultiSelect = false;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLight;
            //
            // field_checked
            //
            field_checked.HeaderText = "";
            field_checked.Name = "checked";
            field_checked.Width = 30;
            field_checked.Resizable = DataGridViewTriState.False;
            field_checked.DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLightLight;
            field_checked.ReadOnly = false;
            field_checked.SortMode = DataGridViewColumnSortMode.NotSortable;
            //
            // field_beds
            //
            field_beds.HeaderText = "№ койко-мест";
            field_beds.Name = "beds";
            field_beds.MinimumWidth = 130;
            field_beds.DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLightLight;
            field_beds.ReadOnly = false;
            field_beds.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // field_id_building
            // 
            field_id_building.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_id_building.HeaderText = "№";
            field_id_building.Name = "id_building";
            field_id_building.ReadOnly = true;
            // 
            // field_id_street
            // 
            field_id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_id_street.HeaderText = "Адрес";
            field_id_street.MinimumWidth = 300;
            field_id_street.Name = "id_street";
            field_id_street.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            field_id_street.SortMode = DataGridViewColumnSortMode.Automatic;
            field_id_street.ReadOnly = true;
            // 
            // field_house
            // 
            field_house.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_house.HeaderText = "Дом";
            field_house.Name = "house";
            field_house.ReadOnly = true;
            // 
            // field_floors
            // 
            field_floors.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_floors.HeaderText = "Этажность";
            field_floors.Name = "floors";
            field_floors.ReadOnly = true;
            // 
            // field_living_area
            // 
            field_living_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_living_area.HeaderText = "Жилая площадь";
            field_living_area.MinimumWidth = 150;
            field_living_area.Name = "living_area";
            field_living_area.DefaultCellStyle.Format = "#0.0## м²";
            field_living_area.ReadOnly = true;
            // 
            // field_cadastral_num
            // 
            field_cadastral_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_cadastral_num.HeaderText = "Кадастровый номер";
            field_cadastral_num.MinimumWidth = 150;
            field_cadastral_num.Name = "cadastral_num";
            field_cadastral_num.ReadOnly = true;
            // 
            // field_startup_year
            // 
            field_startup_year.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_startup_year.HeaderText = "Год ввода в эксплуатацию";
            field_startup_year.MinimumWidth = 200;
            field_startup_year.Name = "startup_year";
            field_startup_year.ReadOnly = true;

            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
        }
    }
}
