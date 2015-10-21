using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyBuildingsViewport : DataGridViewport
    {
        #region Models
        private DataModel kladr;
        private DataModel tenancy_buildings;
        private DataTable snapshot_tenancy_buildings;
        #endregion Models

        #region Views
        private BindingSource v_kladr;
        private BindingSource v_tenancy_buildings;
        private BindingSource v_snapshot_tenancy_buildings;
        #endregion Views

        //Forms
        private SearchForm sbSimpleSearchForm;
        private SearchForm sbExtendedSearchForm;

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
            DynamicFilter = tenancyBuildingsViewport.DynamicFilter;
            StaticFilter = tenancyBuildingsViewport.StaticFilter;
            ParentRow = tenancyBuildingsViewport.ParentRow;
            ParentType = tenancyBuildingsViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = TenancyBuildingsFromView();
            var list_from_viewport = TenancyBuildingsFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            var founded = false;
            for (var i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (var j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_assoc"],
                dataRowView["id_building"], 
                true, 
                dataRowView["rent_total_area"],
                dataRowView["rent_living_area"]
            };
        }

        public void LocateBuildingBy(int id)
        {
            GeneralBindingSource.Position = GeneralBindingSource.Find("id_building", id);
        }

        private static TenancyObject RowToTenancyBuilding(DataRow row)
        {
            var to = new TenancyObject();
            to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
            to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_building");
            to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
            to.RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area");
            return to;
        }

        private List<TenancyObject> TenancyBuildingsFromViewport()
        {
            var list = new List<TenancyObject>();
            for (var i = 0; i < snapshot_tenancy_buildings.Rows.Count; i++)
            {
                var row = snapshot_tenancy_buildings.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var to = new TenancyObject();
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
            var list = new List<TenancyObject>();
            for (var i = 0; i < v_tenancy_buildings.Count; i++)
            {
                var to = new TenancyObject();
                var row = ((DataRowView)v_tenancy_buildings[i]);
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_building");
                to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
                to.RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area");
                list.Add(to);
            }
            return list;
        }

        private bool ValidateTenancyBuildings(List<TenancyObject> tenancyBuildings)
        {
            foreach (var building in tenancyBuildings)
            {
                if (!ViewportHelper.BuildingFundAndRentMatch(building.IdObject.Value, (int)ParentRow["id_rent_type"]) &&
                            MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемого здания. Все равно продолжить сохранение?",
                            "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
                else
                    return true;
            }
            return true;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.BuildingsDataModel);
            kladr = DataModel.GetInstance(DataModelType.KladrStreetsDataModel);
            tenancy_buildings = DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel);
            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
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

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "buildings";
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataSource = ds;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = "Здания найма №" + ParentRow["id_process"];
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
            for (var i = 0; i < v_tenancy_buildings.Count; i++)
                snapshot_tenancy_buildings.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_buildings[i])));
            v_snapshot_tenancy_buildings = new BindingSource();
            v_snapshot_tenancy_buildings.DataSource = snapshot_tenancy_buildings;

            id_street.DataSource = v_kladr;
            id_street.ValueMember = "id_street";
            id_street.DisplayMember = "street_name";

            //Строим фильтр арендуемых зданий
            if (string.IsNullOrEmpty(DynamicFilter))
            {
                if (v_tenancy_buildings.Count > 0)
                {
                    DynamicFilter = "id_building IN (0";
                    for (var i = 0; i < v_tenancy_buildings.Count; i++)
                        DynamicFilter += "," + ((DataRowView)v_tenancy_buildings[i])["id_building"];
                    DynamicFilter += ")";
                }
            }
            GeneralBindingSource.Filter = DynamicFilter;

            GeneralDataModel.Select().RowChanged += BuildingsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += BuildingsViewport_RowDeleted;
            tenancy_buildings.Select().RowChanged += TenancyBuildingsViewport_RowChanged;
            tenancy_buildings.Select().RowDeleting += TenancyBuildingsViewport_RowDeleting;
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
            if (MessageBox.Show("Вы действительно хотите удалить это здание?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_building"], EntityType.Building)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление муниципальных жилых зданий и зданий, в которых присутствуют муниципальные помещения",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_building"], EntityType.Building)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление немуниципальных жилых зданий и зданий, в которых присутствуют немуниципальные помещения",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_building"]) == -1)
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
            GeneralBindingSource.Filter = DynamicFilter;
            dataGridView.RowCount = GeneralBindingSource.Count;

        }

        public override void ClearSearch()
        {
            GeneralBindingSource.Filter = "";
            dataGridView.RowCount = GeneralBindingSource.Count;
            DynamicFilter = "";
        }

        public override bool CanOpenDetails()
        {
            return (GeneralBindingSource.Position != -1) && AccessControl.HasPrivelege(Priveleges.RegistryRead);
        }

        public override void OpenDetails()
        {
            var viewport = new BuildingViewport(MenuCallback);
            viewport.StaticFilter = "";
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            var viewport = new BuildingViewport(MenuCallback);
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
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            var viewport = new BuildingViewport(MenuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as int?) ?? -1);
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
            for (var i = 0; i < v_tenancy_buildings.Count; i++)
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
            tenancy_buildings.EditingNewRecord = true;
            var list = TenancyBuildingsFromViewport();
            if (!ValidateTenancyBuildings(list))
            {
                sync_views = true;
                tenancy_buildings.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = tenancy_buildings.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    var id_assoc = GeneralDataModel.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        tenancy_buildings.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_buildings[
                        v_snapshot_tenancy_buildings.Find("id_building", list[i].IdObject)])["id_assoc"] = id_assoc;
                    tenancy_buildings.Select().Rows.Add(id_assoc, list[i].IdObject, list[i].IdProcess, list[i].RentTotalArea, list[i].RentLivingArea, 0);
                }
                else
                {
                    if (RowToTenancyBuilding(row) == list[i])
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        tenancy_buildings.EditingNewRecord = false;
                        return;
                    }
                    row["rent_total_area"] = list[i].RentTotalArea == null ? DBNull.Value : (object)list[i].RentTotalArea;
                    row["rent_living_area"] = list[i].RentLivingArea == null ? DBNull.Value : (object)list[i].RentLivingArea;
                }
            }
            list = TenancyBuildingsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < v_snapshot_tenancy_buildings.Count; j++)
                {
                    var row = (DataRowView)v_snapshot_tenancy_buildings[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    if (GeneralDataModel.Delete(list[i].IdAssoc.Value) == -1)
                    {
                        sync_views = true;
                        tenancy_buildings.EditingNewRecord = false;
                        return;
                    }
                    var snapshot_row_index = -1;
                    for (var j = 0; j < v_snapshot_tenancy_buildings.Count; j++)
                        if (((DataRowView)v_snapshot_tenancy_buildings[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_tenancy_buildings[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        var building_row_index = GeneralBindingSource.Find("id_building", list[i].IdObject);
                        ((DataRowView)v_snapshot_tenancy_buildings[snapshot_row_index]).Delete();
                        if (building_row_index != -1)
                            dataGridView.InvalidateRow(building_row_index);
                    }
                    tenancy_buildings.Select().Rows.Find(list[i].IdAssoc).Delete();
                }
            }
            sync_views = true;
            tenancy_buildings.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new TenancyBuildingsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (GeneralBindingSource.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as int?) ?? -1);
            return viewport;
        }

        public override bool HasAssocPremises()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocOwnerships()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (GeneralBindingSource.Position > -1);
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
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения истории найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_building = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Building);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
            GeneralDataModel.Select().RowChanged -= BuildingsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= BuildingsViewport_RowDeleted;
            tenancy_buildings.Select().RowChanged -= TenancyBuildingsViewport_RowChanged;
            tenancy_buildings.Select().RowDeleting -= TenancyBuildingsViewport_RowDeleting;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= BuildingsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= BuildingsViewport_RowDeleted;
            tenancy_buildings.Select().RowChanged -= TenancyBuildingsViewport_RowChanged;
            tenancy_buildings.Select().RowDeleting -= TenancyBuildingsViewport_RowDeleting;
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
                var row_index = v_snapshot_tenancy_buildings.Find("id_building", e.Row["id_building"]);
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
            var row_index = v_snapshot_tenancy_buildings.Find("id_building", e.Row["id_building"]);
            if (row_index == -1 && v_tenancy_buildings.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_tenancy_buildings.Rows.Add(e.Row["id_assoc"], e.Row["id_building"], true, e.Row["rent_total_area"], e.Row["rent_living_area"]);
            }
            else
            if (row_index != -1)
            {
                var row = ((DataRowView)v_snapshot_tenancy_buildings[row_index]);
                row["rent_total_area"] = e.Row["rent_total_area"];
                row["rent_living_area"] = e.Row["rent_living_area"];
            }
            dataGridView.Invalidate();
        }

        void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || dataGridView.RowCount == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            if (GeneralBindingSource.Position >= dataGridView.RowCount)
            {
                dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[GeneralBindingSource.Position].Cells[0];
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

        void BuildingsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            dataGridView.RowCount = GeneralBindingSource.Count;
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
            dataGridView.RowCount = GeneralBindingSource.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var id_building = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_building"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_tenancy_buildings.Find("id_building", id_building);
            double value = 0;
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_tenancy_buildings.Rows.Add(null, id_building, e.Value, null);
                    else
                        ((DataRowView)v_snapshot_tenancy_buildings[row_index])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                    value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_buildings.Rows.Add(null, id_building, false, value == 0 ? DBNull.Value : (object)value, DBNull.Value);
                    else
                        ((DataRowView)v_snapshot_tenancy_buildings[row_index])["rent_total_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
                case "rent_living_area":
                    value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_buildings.Rows.Add(null, id_building, false, DBNull.Value, value == 0 ? DBNull.Value : (object)value);
                    else
                        ((DataRowView)v_snapshot_tenancy_buildings[row_index])["rent_living_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var id_building = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_building"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_tenancy_buildings.Find("id_building", id_building);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
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
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["id_building"];
                    break;
                case "id_street":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["id_street"];
                    break;
                case "house":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["house"];
                    break;
                case "floors":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["floors"];
                    break;
                case "living_area":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["living_area"];
                    break;
                case "cadastral_num":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["cadastral_num"];
                    break;
                case "startup_year":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["cadastral_num"];
                    break;
            }
        }

        void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if ((dataGridView.CurrentCell.OwningColumn.Name == "rent_total_area") ||
                (dataGridView.CurrentCell.OwningColumn.Name == "rent_living_area"))
            {
                dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
                dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
                if (string.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
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
            if (dataGridView.Size.Width > 1260)
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

        void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }
    }
}
