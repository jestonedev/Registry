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
    internal sealed partial class ResettleBuildingsViewport : DataGridViewport
    {
        #region Models
        private DataModel kladr;
        private DataTable snapshot_resettle_buildings;
        #endregion Models

        #region Views
        private BindingSource v_kladr;
        private BindingSource v_snapshot_resettle_buildings;
        #endregion Views

        //Forms
        private SearchForm sbSimpleSearchForm;
        private SearchForm sbExtendedSearchForm;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private ResettleEstateObjectWay way = ResettleEstateObjectWay.From;

        public ResettleEstateObjectWay Way { get { return way; } set { way = value; } }

        private ResettleBuildingsViewport()
            : this(null)
        {
        }

        public ResettleBuildingsViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
        }

        public ResettleBuildingsViewport(ResettleBuildingsViewport resettleBuildingsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = resettleBuildingsViewport.DynamicFilter;
            StaticFilter = resettleBuildingsViewport.StaticFilter;
            ParentRow = resettleBuildingsViewport.ParentRow;
            ParentType = resettleBuildingsViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = ResettleBuildingsFromView();
            var list_from_viewport = ResettleBuildingsFromViewport();
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
                true
            };
        }

        public void LocateBuildingBy(int id)
        {
            GeneralBindingSource.Position = GeneralBindingSource.Find("id_building", id);
        }

        private List<ResettleObject> ResettleBuildingsFromViewport()
        {
            var list = new List<ResettleObject>();
            for (var i = 0; i < snapshot_resettle_buildings.Rows.Count; i++)
            {
                var row = snapshot_resettle_buildings.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var ro = new ResettleObject();
                ro.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                ro.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
                ro.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_building");
                list.Add(ro);
            }
            return list;
        }

        private List<ResettleObject> ResettleBuildingsFromView()
        {
            var list = new List<ResettleObject>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var ro = new ResettleObject();
                var row = ((DataRowView)GeneralBindingSource[i]);
                ro.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                ro.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                ro.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_building");
                list.Add(ro);
            }
            return list;
        }

        private static bool ValidateResettleBuildings(List<ResettleObject> resettleObject)
        {
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
            if (way == ResettleEstateObjectWay.From)
                GeneralDataModel = DataModel.GetInstance(DataModelType.ResettleBuildingsFromAssocDataModel);
            else
                GeneralDataModel = DataModel.GetInstance(DataModelType.ResettleBuildingsToAssocDataModel);
            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            kladr.Select();
            GeneralDataModel.Select();

            // Инициализируем snapshot-модель
            snapshot_resettle_buildings = new DataTable("selected_buildings");
            snapshot_resettle_buildings.Locale = CultureInfo.InvariantCulture;
            snapshot_resettle_buildings.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_resettle_buildings.Columns.Add("id_building").DataType = typeof(int);
            snapshot_resettle_buildings.Columns.Add("is_checked").DataType = typeof(bool);

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "buildings";
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = DynamicFilter;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.ResettleProcess))
            {
                if (way == ResettleEstateObjectWay.From)
                    Text = "Здания (из) переселения №" + ParentRow["id_process"];
                else
                    Text = "Здания (в) переселения №" + ParentRow["id_process"];
            }
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_kladr = new BindingSource();
            v_kladr.DataMember = "kladr";
            v_kladr.DataSource = ds;

            GeneralBindingSource = new BindingSource();
            if (way == ResettleEstateObjectWay.From)
                GeneralBindingSource.DataMember = "resettle_buildings_from_assoc";
            else
                GeneralBindingSource.DataMember = "resettle_buildings_to_assoc";
            GeneralBindingSource.Filter = StaticFilter;
            GeneralBindingSource.DataSource = ds;

            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                snapshot_resettle_buildings.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            v_snapshot_resettle_buildings = new BindingSource();
            v_snapshot_resettle_buildings.DataSource = snapshot_resettle_buildings;

            id_street.DataSource = v_kladr;
            id_street.ValueMember = "id_street";
            id_street.DisplayMember = "street_name";

            GeneralDataModel.Select().RowChanged += BuildingsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += BuildingsViewport_RowDeleted;
            GeneralDataModel.Select().RowChanged += ResettleBuildingsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += ResettleBuildingsViewport_RowDeleting;
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
            snapshot_resettle_buildings.Clear();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                snapshot_resettle_buildings.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            dataGridView.Refresh();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            var resettleBuildingsFromAssoc = DataModel.GetInstance(DataModelType.ResettleBuildingsFromAssocDataModel);
            var resettleBuildingsToAssoc = DataModel.GetInstance(DataModelType.ResettleBuildingsToAssocDataModel);
            resettleBuildingsFromAssoc.EditingNewRecord = true;
            resettleBuildingsToAssoc.EditingNewRecord = true;
            var list = ResettleBuildingsFromViewport();
            if (!ValidateResettleBuildings(list))
            {
                sync_views = true;
                resettleBuildingsFromAssoc.EditingNewRecord = false;
                resettleBuildingsToAssoc.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = GeneralDataModel.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    var id_assoc = -1;
                    if (way == ResettleEstateObjectWay.From)
                        id_assoc = resettleBuildingsFromAssoc.Insert(list[i]);
                    else
                        id_assoc = resettleBuildingsToAssoc.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        resettleBuildingsFromAssoc.EditingNewRecord = false;
                        resettleBuildingsToAssoc.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_resettle_buildings[
                        v_snapshot_resettle_buildings.Find("id_building", list[i].IdObject)])["id_assoc"] = id_assoc;
                    GeneralDataModel.Select().Rows.Add(id_assoc, list[i].IdObject, list[i].IdProcess, 0);
                }
            }
            list = ResettleBuildingsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < v_snapshot_resettle_buildings.Count; j++)
                {
                    var row = (DataRowView)v_snapshot_resettle_buildings[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    var affected = -1;
                    if (way == ResettleEstateObjectWay.From)
                        affected = resettleBuildingsFromAssoc.Delete(list[i].IdAssoc.Value);
                    else
                        affected = resettleBuildingsToAssoc.Delete(list[i].IdAssoc.Value);
                    if (affected == -1)
                    {
                        sync_views = true;
                        resettleBuildingsFromAssoc.EditingNewRecord = false;
                        resettleBuildingsToAssoc.EditingNewRecord = false;
                        return;
                    }
                    var snapshot_row_index = -1;
                    for (var j = 0; j < v_snapshot_resettle_buildings.Count; j++)
                        if (((DataRowView)v_snapshot_resettle_buildings[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_resettle_buildings[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        var building_row_index = GeneralBindingSource.Find("id_building", list[i].IdObject);
                        ((DataRowView)v_snapshot_resettle_buildings[snapshot_row_index]).Delete();
                        if (building_row_index != -1)
                            dataGridView.InvalidateRow(building_row_index);
                    }
                    GeneralDataModel.Select().Rows.Find(list[i].IdAssoc).Delete();
                }
            }
            sync_views = true;
            resettleBuildingsFromAssoc.EditingNewRecord = false;
            resettleBuildingsToAssoc.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new ResettleBuildingsViewport(this, MenuCallback);
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
            GeneralDataModel.Select().RowChanged -= ResettleBuildingsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= ResettleBuildingsViewport_RowDeleting;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= BuildingsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= BuildingsViewport_RowDeleted;
            GeneralDataModel.Select().RowChanged -= ResettleBuildingsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= ResettleBuildingsViewport_RowDeleting;
            base.ForceClose();
        }

        void ResettleBuildingsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = v_snapshot_resettle_buildings.Find("id_building", e.Row["id_building"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_resettle_buildings[row_index]).Delete();
            }
            dataGridView.Invalidate();
        }

        void ResettleBuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            var row_index = v_snapshot_resettle_buildings.Find("id_building", e.Row["id_building"]);
            if (row_index == -1 && GeneralBindingSource.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_resettle_buildings.Rows.Add(e.Row["id_assoc"], e.Row["id_building"], true);
                dataGridView.Invalidate();
            }
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
            dataGridView.Refresh();
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var id_building = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_building"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_resettle_buildings.Find("id_building", id_building);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_resettle_buildings.Rows.Add(null, id_building, e.Value);
                    else
                        ((DataRowView)v_snapshot_resettle_buildings[row_index])["is_checked"] = e.Value;
                    break;
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var id_building = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_building"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_resettle_buildings.Find("id_building", id_building);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_resettle_buildings[row_index])["is_checked"];
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

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1140)
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
