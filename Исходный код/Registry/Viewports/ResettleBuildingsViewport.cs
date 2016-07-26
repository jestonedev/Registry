using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ResettleBuildingsViewport : DataGridViewport
    {
        #region Models
        private DataModel _kladr;
        private DataModel _resettleBuildings;
        private DataTable _snapshotResettleBuildings;
        #endregion Models

        #region Views
        private BindingSource _vKladr;
        private BindingSource _vResettleBuildings;
        private BindingSource _vSnapshotResettleBuildings;
        #endregion Views

        //Forms
        private SearchForm _sbSimpleSearchForm;
        private SearchForm _sbExtendedSearchForm;

        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private ResettleEstateObjectWay _way = ResettleEstateObjectWay.From;

        public ResettleEstateObjectWay Way { get { return _way; } set { _way = value; } }

        private ResettleBuildingsViewport()
            : this(null, null)
        {
        }

        public ResettleBuildingsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = ResettleBuildingsFromView();
            var listFromViewport = ResettleBuildingsFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            for (var i = 0; i < listFromView.Count; i++)
            {
                var founded = false;
                for (var j = 0; j < listFromViewport.Count; j++)
                    if (listFromView[i] == listFromViewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        public void LocateBuildingBy(int id)
        {
            GeneralBindingSource.Position = GeneralBindingSource.Find("id_building", id);
        }

        private List<ResettleObject> ResettleBuildingsFromViewport()
        {
            var list = new List<ResettleObject>();
            for (var i = 0; i < _snapshotResettleBuildings.Rows.Count; i++)
            {
                var row = _snapshotResettleBuildings.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var ro = new ResettleObject
                {
                    IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                    IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                    IdObject = ViewportHelper.ValueOrNull<int>(row, "id_building")
                };
                list.Add(ro);
            }
            return list;
        }

        private List<ResettleObject> ResettleBuildingsFromView()
        {
            var list = new List<ResettleObject>();
            for (var i = 0; i < _vResettleBuildings.Count; i++)
            {
                var row = (DataRowView)_vResettleBuildings[i];
                list.Add(ResettleBuildingConverter.FromRow(row));
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
            GeneralDataModel = DataModel.GetInstance<BuildingsDataModel>();
            _kladr = DataModel.GetInstance<KladrStreetsDataModel>();
            _resettleBuildings = _way == ResettleEstateObjectWay.From ? 
                DataModel.GetInstance<ResettleBuildingsFromAssocDataModel>() : 
                DataModel.GetInstance<ResettleBuildingsToAssocDataModel>();
            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _kladr.Select();
            _resettleBuildings.Select();

            // Инициализируем snapshot-модель
            _snapshotResettleBuildings = new DataTable("selected_buildings") {Locale = CultureInfo.InvariantCulture};
            _snapshotResettleBuildings.Columns.Add("id_assoc").DataType = typeof(int);
            _snapshotResettleBuildings.Columns.Add("id_building").DataType = typeof(int);
            _snapshotResettleBuildings.Columns.Add("is_checked").DataType = typeof(bool);

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource
            {
                DataMember = "buildings",
                DataSource = ds,
                Filter = DynamicFilter
            };
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.ResettleProcess))
            {
                if (_way == ResettleEstateObjectWay.From)
                    Text = @"Здания (из) переселения №" + ParentRow["id_process"];
                else
                    Text = @"Здания (в) переселения №" + ParentRow["id_process"];
            }
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            _vKladr = new BindingSource
            {
                DataMember = "kladr",
                DataSource = ds
            };

            _vResettleBuildings = new BindingSource
            {
                DataMember =
                    _way == ResettleEstateObjectWay.From
                        ? "resettle_buildings_from_assoc"
                        : "resettle_buildings_to_assoc",
                Filter = StaticFilter,
                DataSource = ds
            };

            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vResettleBuildings.Count; i++)
                _snapshotResettleBuildings.Rows.Add(ResettleBuildingConverter.ToArray((DataRowView)_vResettleBuildings[i]));
            _vSnapshotResettleBuildings = new BindingSource {DataSource = _snapshotResettleBuildings};

            id_street.DataSource = _vKladr;
            id_street.ValueMember = "id_street";
            id_street.DisplayMember = "street_name";

            //Строим фильтр арендуемых зданий
            if (string.IsNullOrEmpty(DynamicFilter))
            {
                if (_vResettleBuildings.Count > 0)
                {
                    DynamicFilter = "id_building IN (0";
                    for (var i = 0; i < _vResettleBuildings.Count; i++)
                        DynamicFilter += "," + ((DataRowView)_vResettleBuildings[i])["id_building"];
                    DynamicFilter += ")";
                }
            }
            GeneralBindingSource.Filter = DynamicFilter;

            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", BuildingsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", BuildingsViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(_resettleBuildings.Select(), "RowChanged", ResettleBuildingsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_resettleBuildings.Select(), "RowDeleting", ResettleBuildingsViewport_RowDeleting);
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
            if (MessageBox.Show(@"Вы действительно хотите удалить это здание?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (DataModelHelper.HasMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_building"], EntityType.Building)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на удаление муниципальных жилых зданий и зданий, в которых присутствуют муниципальные помещения",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (DataModelHelper.HasNotMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_building"], EntityType.Building)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на удаление немуниципальных жилых зданий и зданий, в которых присутствуют немуниципальные помещения",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_building"]) == -1)
                return;
            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
            MenuCallback.ForceCloseDetachedViewports();
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
                    if (_sbSimpleSearchForm == null)
                        _sbSimpleSearchForm = new SimpleSearchBuildingForm();
                    if (_sbSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _sbSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (_sbExtendedSearchForm == null)
                        _sbExtendedSearchForm = new ExtendedSearchBuildingForm();
                    if (_sbExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _sbExtendedSearchForm.GetFilter();
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
            var viewport = new BuildingViewport(null, MenuCallback)
            {
                StaticFilter = "",
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocateEntityBy("id_building", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            var viewport = new BuildingViewport(null, MenuCallback)
            {
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
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            var viewport = new BuildingViewport(null, MenuCallback)
            {
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocateEntityBy("id_building", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotResettleBuildings.Clear();
            for (var i = 0; i < _vResettleBuildings.Count; i++)
                _snapshotResettleBuildings.Rows.Add(ResettleBuildingConverter.ToArray((DataRowView)_vResettleBuildings[i]));
            dataGridView.Refresh();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void SaveRecord()
        {
            _syncViews = false;
            dataGridView.EndEdit();
            var resettleBuildingsFromAssoc = DataModel.GetInstance<ResettleBuildingsFromAssocDataModel>();
            var resettleBuildingsToAssoc = DataModel.GetInstance<ResettleBuildingsToAssocDataModel>();
            resettleBuildingsFromAssoc.EditingNewRecord = true;
            resettleBuildingsToAssoc.EditingNewRecord = true;
            var list = ResettleBuildingsFromViewport();
            if (!ValidateResettleBuildings(list))
            {
                _syncViews = true;
                resettleBuildingsFromAssoc.EditingNewRecord = false;
                resettleBuildingsToAssoc.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = _resettleBuildings.Select().Rows.Find(list[i].IdAssoc);
                if (row != null) continue;
                var idAssoc = _way == ResettleEstateObjectWay.From ? 
                    resettleBuildingsFromAssoc.Insert(list[i]) : 
                    resettleBuildingsToAssoc.Insert(list[i]);
                if (idAssoc == -1)
                {
                    _syncViews = true;
                    resettleBuildingsFromAssoc.EditingNewRecord = false;
                    resettleBuildingsToAssoc.EditingNewRecord = false;
                    return;
                }
                ((DataRowView)_vSnapshotResettleBuildings[
                    _vSnapshotResettleBuildings.Find("id_building", list[i].IdObject)])["id_assoc"] = idAssoc;
                _resettleBuildings.Select().Rows.Add(idAssoc, list[i].IdObject, list[i].IdProcess, 0);
            }
            list = ResettleBuildingsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < _vSnapshotResettleBuildings.Count; j++)
                {
                    var row = (DataRowView)_vSnapshotResettleBuildings[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture))
                        rowIndex = j;
                }
                if (rowIndex == -1)
                {
                    var affected = _way == ResettleEstateObjectWay.From ? 
                        resettleBuildingsFromAssoc.Delete(list[i].IdAssoc.Value) : 
                        resettleBuildingsToAssoc.Delete(list[i].IdAssoc.Value);
                    if (affected == -1)
                    {
                        _syncViews = true;
                        resettleBuildingsFromAssoc.EditingNewRecord = false;
                        resettleBuildingsToAssoc.EditingNewRecord = false;
                        return;
                    }
                    var snapshotRowIndex = -1;
                    for (var j = 0; j < _vSnapshotResettleBuildings.Count; j++)
                        if (((DataRowView)_vSnapshotResettleBuildings[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)_vSnapshotResettleBuildings[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshotRowIndex = j;
                    if (snapshotRowIndex != -1)
                    {
                        var buildingRowIndex = GeneralBindingSource.Find("id_building", list[i].IdObject);
                        ((DataRowView)_vSnapshotResettleBuildings[snapshotRowIndex]).Delete();
                        if (buildingRowIndex != -1)
                            dataGridView.InvalidateRow(buildingRowIndex);
                    }
                    _resettleBuildings.Select().Rows.Find(list[i].IdAssoc).Delete();
                }
            }
            _syncViews = true;
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

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.PremisesListViewport,
                ViewportType.OwnershipListViewport,
                ViewportType.RestrictionListViewport,
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport<T>()
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание для отображения связных объектов", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, 
                "id_building = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Building);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveRecord();
                        break;
                    case DialogResult.No:
                        CancelRecord();
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }
            base.OnClosing(e);
        }

        private void ResettleBuildingsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = _vSnapshotResettleBuildings.Find("id_building", e.Row["id_building"]);
                if (rowIndex != -1)
                    ((DataRowView)_vSnapshotResettleBuildings[rowIndex]).Delete();
            }
            dataGridView.Invalidate();
        }

        private void ResettleBuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            var rowIndex = _vSnapshotResettleBuildings.Find("id_building", e.Row["id_building"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                _snapshotResettleBuildings.Rows.Add(ResettleBuildingConverter.ToArray(e.Row));
                dataGridView.Invalidate();
            }
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
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
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        private void BuildingsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void BuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = GeneralBindingSource.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
            dataGridView.Refresh();
        }

        private void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var idBuilding = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_building"], CultureInfo.InvariantCulture);
            var rowIndex = _vSnapshotResettleBuildings.Find("id_building", idBuilding);
            _syncViews = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (rowIndex == -1)
                        _snapshotResettleBuildings.Rows.Add(null, idBuilding, e.Value);
                    else
                        ((DataRowView)_vSnapshotResettleBuildings[rowIndex])["is_checked"] = e.Value;
                    break;
            }
            _syncViews = true;
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var row = (DataRowView) GeneralBindingSource[e.RowIndex];
            var idBuilding = Convert.ToInt32(row["id_building"], CultureInfo.InvariantCulture);
            var rowIndex = _vSnapshotResettleBuildings.Find("id_building", idBuilding);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vSnapshotResettleBuildings[rowIndex])["is_checked"];
                    break;
                case "id_building":
                case "id_street":
                case "house":
                case "floors":
                case "living_area":
                case "cadastral_num":
                case "startup_year":
                    e.Value = row[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1140)
            {
                if (id_street.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (id_street.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }
    }
}
