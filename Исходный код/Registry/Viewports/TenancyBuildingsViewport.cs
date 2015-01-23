﻿using System;
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
using Security;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class TenancyBuildingsViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        private BuildingsDataModel buildings = null;
        private KladrStreetsDataModel kladr = null;
        private TenancyBuildingsAssocDataModel tenancy_buildings = null;
        private DataTable snapshot_tenancy_buildings = null;
        #endregion Models

        #region Views
        private BindingSource v_buildings = null;
        private BindingSource v_kladr = null;
        private BindingSource v_tenancy_buildings = null;
        private BindingSource v_snapshot_tenancy_buildings = null;
        #endregion Views

        //Forms
        private SearchForm sbSimpleSearchForm = null;
        private SearchForm sbExtendedSearchForm = null;
        private DataGridViewCheckBoxColumn is_checked;
        private DataGridViewTextBoxColumn rent_total_area;
        private DataGridViewTextBoxColumn rent_living_area;
        private DataGridViewTextBoxColumn id_building;
        private DataGridViewComboBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn floors;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn cadastral_num;
        private DataGridViewTextBoxColumn startup_year;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private TenancyBuildingsViewport()
            : this(null)
        {
        }

        public TenancyBuildingsViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
        }

        public TenancyBuildingsViewport(TenancyBuildingsViewport tenancyBuildingsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = tenancyBuildingsViewport.DynamicFilter;
            this.StaticFilter = tenancyBuildingsViewport.StaticFilter;
            this.ParentRow = tenancyBuildingsViewport.ParentRow;
            this.ParentType = tenancyBuildingsViewport.ParentType;
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

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_assoc"],
                dataRowView["id_building"], 
                true, 
                dataRowView["rent_total_area"],
                dataRowView["rent_living_area"]
            };
        }

        public void LocateBuildingBy(int id)
        {
            v_buildings.Position = v_buildings.Find("id_building", id);
        }

        private static TenancyObject RowToTenancyBuilding(DataRow row)
        {
            TenancyObject to = new TenancyObject();
            to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
            to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_building");
            to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
            to.RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area");
            return to;
        }

        private List<TenancyObject> TenancyBuildingsFromViewport()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < snapshot_tenancy_buildings.Rows.Count; i++)
            {
                DataRow row = snapshot_tenancy_buildings.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                TenancyObject to = new TenancyObject();
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_building");
                to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
                to.RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area");
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
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_building");
                to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
                to.RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area");
                list.Add(to);
            }
            return list;
        }

        private static bool ValidateTenancyBuildings(List<TenancyObject> tenancyBuildings)
        {
            return true;
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
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            buildings = BuildingsDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();
            tenancy_buildings = TenancyBuildingsAssocDataModel.GetInstance();
            // Ожидаем дозагрузки данных, если это необходимо
            buildings.Select();
            kladr.Select();
            tenancy_buildings.Select();

            // Инициализируем snapshot-модель
            snapshot_tenancy_buildings = new DataTable("selected_buildings");
            snapshot_tenancy_buildings.Locale = CultureInfo.InvariantCulture;
            snapshot_tenancy_buildings.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_tenancy_buildings.Columns.Add("id_building").DataType = typeof(int);
            snapshot_tenancy_buildings.Columns.Add("is_checked").DataType = typeof(bool);
            snapshot_tenancy_buildings.Columns.Add("rent_total_area").DataType = typeof(double);
            snapshot_tenancy_buildings.Columns.Add("rent_living_area").DataType = typeof(double);

            DataSet ds = DataSetManager.DataSet;

            v_buildings = new BindingSource();
            v_buildings.DataMember = "buildings";
            v_buildings.CurrentItemChanged += new EventHandler(v_buildings_CurrentItemChanged);
            v_buildings.DataSource = ds;
            v_buildings.Filter = DynamicFilter;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = "Здания найма №" + ParentRow["id_process"].ToString();
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

            id_street.DataSource = v_kladr;
            id_street.ValueMember = "id_street";
            id_street.DisplayMember = "street_name";

            buildings.Select().RowChanged += new DataRowChangeEventHandler(BuildingsViewport_RowChanged);
            buildings.Select().RowDeleted += new DataRowChangeEventHandler(BuildingsViewport_RowDeleted);
            tenancy_buildings.Select().RowChanged += new DataRowChangeEventHandler(TenancyBuildingsViewport_RowChanged);
            tenancy_buildings.Select().RowDeleting += new DataRowChangeEventHandler(TenancyBuildingsViewport_RowDeleting);
            dataGridView.RowCount = v_buildings.Count;
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public override bool CanDeleteRecord()
        {
            return (v_buildings.Position > -1) && AccessControl.HasPrivelege(Priveleges.RegistryWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это здание?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (BuildingsDataModel.Delete((int)((DataRowView)v_buildings.Current)["id_building"]) == -1)
                    return;
                ((DataRowView)v_buildings[v_buildings.Position]).Delete();
                MenuCallback.ForceCloseDetachedViewports();
                CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
                CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            }
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
            return (v_buildings.Position != -1) && AccessControl.HasPrivelege(Priveleges.RegistryWrite);
        }

        public override void OpenDetails()
        {
            BuildingViewport viewport = new BuildingViewport(MenuCallback);
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
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return (!buildings.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.RegistryRead);
        }

        public override void InsertRecord()
        {
            BuildingViewport viewport = new BuildingViewport(MenuCallback);
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
            return (v_buildings.Position != -1) && (!buildings.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.RegistryWrite);
        }

        public override void CopyRecord()
        {
            BuildingViewport viewport = new BuildingViewport(MenuCallback);
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

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_tenancy_buildings.Clear();
            for (int i = 0; i < v_tenancy_buildings.Count; i++)
                snapshot_tenancy_buildings.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_buildings[i])));
            dataGridView.Refresh();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
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
                if (((TenancyObject)list[i]).IdAssoc != null)
                    row = tenancy_buildings.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    int id_assoc = TenancyBuildingsAssocDataModel.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_buildings[
                        v_snapshot_tenancy_buildings.Find("id_building", list[i].IdObject)])["id_assoc"] = id_assoc;
                    tenancy_buildings.Select().Rows.Add(new object[] { 
                        id_assoc, list[i].IdObject, list[i].IdProcess, list[i].RentTotalArea, list[i].RentLivingArea, 0
                    });
                }
                else
                {
                    if (RowToTenancyBuilding(row) == list[i])
                        continue;
                    if (TenancyBuildingsAssocDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["rent_total_area"] = list[i].RentTotalArea == null ? DBNull.Value : (object)list[i].RentTotalArea;
                    row["rent_living_area"] = list[i].RentLivingArea == null ? DBNull.Value : (object)list[i].RentLivingArea;
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
                        !String.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    if (TenancyBuildingsAssocDataModel.Delete(list[i].IdAssoc.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    int snapshot_row_index = -1;
                    for (int j = 0; j < v_snapshot_tenancy_buildings.Count; j++)
                        if (((DataRowView)v_snapshot_tenancy_buildings[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_tenancy_buildings[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        int building_row_index = v_buildings.Find("id_building", list[i].IdObject);
                        ((DataRowView)v_snapshot_tenancy_buildings[snapshot_row_index]).Delete();
                        if (building_row_index != -1)
                            dataGridView.InvalidateRow(building_row_index);
                    }
                    tenancy_buildings.Select().Rows.Find(((TenancyObject)list[i]).IdAssoc).Delete();
                }
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
            if (ParentType == ParentTypeEnum.Tenancy)
                CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.TenancyProcess, (int)ParentRow["id_process"], true);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            TenancyBuildingsViewport viewport = new TenancyBuildingsViewport(this, MenuCallback);
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
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
            }
            buildings.Select().RowChanged -= new DataRowChangeEventHandler(BuildingsViewport_RowChanged);
            buildings.Select().RowDeleted -= new DataRowChangeEventHandler(BuildingsViewport_RowDeleted);
            tenancy_buildings.Select().RowChanged -= new DataRowChangeEventHandler(TenancyBuildingsViewport_RowChanged);
            tenancy_buildings.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyBuildingsViewport_RowDeleting);
        }

        public override void ForceClose()
        {
            buildings.Select().RowChanged -= new DataRowChangeEventHandler(BuildingsViewport_RowChanged);
            buildings.Select().RowDeleted -= new DataRowChangeEventHandler(BuildingsViewport_RowDeleted);
            tenancy_buildings.Select().RowChanged -= new DataRowChangeEventHandler(TenancyBuildingsViewport_RowChanged);
            tenancy_buildings.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyBuildingsViewport_RowDeleting);
            base.ForceClose();
        }

        void TenancyBuildingsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_tenancy_buildings.Find("id_building", e.Row["id_building"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_tenancy_buildings[row_index]).Delete();
            }
            dataGridView.Invalidate();
        }

        void TenancyBuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            int row_index = v_snapshot_tenancy_buildings.Find("id_building", e.Row["id_building"]);
            if (row_index == -1 && v_tenancy_buildings.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_tenancy_buildings.Rows.Add(new object[] { 
                        e.Row["id_assoc"],
                        e.Row["id_building"], 
                        true,   
                        e.Row["rent_total_area"],
                        e.Row["rent_living_area"]
                    });
            }
            else
            if (row_index != -1)
            {
                DataRowView row = ((DataRowView)v_snapshot_tenancy_buildings[row_index]);
                row["rent_total_area"] = e.Row["rent_total_area"];
                row["rent_living_area"] = e.Row["rent_living_area"];
            }
            dataGridView.Invalidate();
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
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void BuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
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
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            int id_building = Convert.ToInt32(((DataRowView)v_buildings[e.RowIndex])["id_building"], CultureInfo.InvariantCulture);
            int row_index = v_snapshot_tenancy_buildings.Find("id_building", id_building);
            double value = 0;
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_tenancy_buildings.Rows.Add(new object[] { null, id_building, e.Value, null });
                    else
                        ((DataRowView)v_snapshot_tenancy_buildings[row_index])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                    value = 0;
                    if (e.Value != null)
                        Double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_buildings.Rows.Add(new object[] { null, id_building, false, value == 0 ? DBNull.Value : (object)value, DBNull.Value });
                    else
                        ((DataRowView)v_snapshot_tenancy_buildings[row_index])["rent_total_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
                case "rent_living_area":
                    value = 0;
                    if (e.Value != null)
                        Double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_buildings.Rows.Add(new object[] { null, id_building, false, DBNull.Value, value == 0 ? DBNull.Value : (object)value });
                    else
                        ((DataRowView)v_snapshot_tenancy_buildings[row_index])["rent_living_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_buildings.Count <= e.RowIndex || v_buildings.Count == 0) return;
            int id_building = Convert.ToInt32(((DataRowView)v_buildings[e.RowIndex])["id_building"], CultureInfo.InvariantCulture);
            int row_index = v_snapshot_tenancy_buildings.Find("id_building", id_building);
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_buildings[row_index])["is_checked"];
                    break;
                case "rent_total_area":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_buildings[row_index])["rent_total_area"];
                    break;
                case "rent_living_area":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_buildings[row_index])["rent_living_area"];
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

        void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if ((dataGridView.CurrentCell.OwningColumn.Name == "rent_total_area") ||
                (dataGridView.CurrentCell.OwningColumn.Name == "rent_living_area"))
            {
                dataGridView.EditingControl.KeyPress -= new KeyPressEventHandler(EditingControl_KeyPress);
                dataGridView.EditingControl.KeyPress += new KeyPressEventHandler(EditingControl_KeyPress);
                if (String.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text = "0";
                else
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text.Substring(0, ((TextBox)e.Control).Text.Length - 3);
            }
        }

        void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((dataGridView.CurrentCell.OwningColumn.Name == "rent_total_area") ||
                (dataGridView.CurrentCell.OwningColumn.Name == "rent_living_area"))
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == (char)8))
                    e.Handled = false;
                else
                    if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    {
                        e.KeyChar = ',';
                        if (((TextBox)dataGridView.EditingControl).Text.IndexOf(',') != -1)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                    else
                        e.Handled = true;
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1400)
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyBuildingsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.is_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.rent_total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rent_living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.is_checked,
            this.rent_total_area,
            this.rent_living_area,
            this.id_building,
            this.id_street,
            this.house,
            this.floors,
            this.living_area,
            this.cadastral_num,
            this.startup_year});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(1410, 723);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValuePushed);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView_EditingControlShowing);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // is_checked
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.NullValue = false;
            this.is_checked.DefaultCellStyle = dataGridViewCellStyle2;
            this.is_checked.HeaderText = "";
            this.is_checked.MinimumWidth = 30;
            this.is_checked.Name = "is_checked";
            this.is_checked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.is_checked.Width = 30;
            // 
            // rent_total_area
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Format = "#0.0## м²";
            this.rent_total_area.DefaultCellStyle = dataGridViewCellStyle3;
            this.rent_total_area.HeaderText = "Арендуемая S общ.";
            this.rent_total_area.MinimumWidth = 140;
            this.rent_total_area.Name = "rent_total_area";
            this.rent_total_area.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rent_total_area.Width = 140;
            // 
            // rent_living_area
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Format = "#0.0## м²";
            this.rent_living_area.DefaultCellStyle = dataGridViewCellStyle4;
            this.rent_living_area.HeaderText = "Арендуемая S жил.";
            this.rent_living_area.MinimumWidth = 140;
            this.rent_living_area.Name = "rent_living_area";
            this.rent_living_area.Width = 140;
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
            dataGridViewCellStyle5.Format = "#0.0## м²";
            this.living_area.DefaultCellStyle = dataGridViewCellStyle5;
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
            // TenancyBuildingsViewport
            // 
            this.ClientSize = new System.Drawing.Size(1416, 729);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyBuildingsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Здания по процессу найма №{0}";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
