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
    internal sealed partial class TenancyBuildingsViewport : DataGridViewport
    {
        #region Models
        private DataModel _kladr;
        private DataModel _tenancyBuildings;
        private DataTable _snapshotTenancyBuildings;
        #endregion Models

        #region Views
        private BindingSource _vKladr;
        private BindingSource _vTenancyBuildings;
        private BindingSource _vSnapshotTenancyBuildings;
        #endregion Views

        //Forms
        private SearchForm _sbSimpleSearchForm;
        private SearchForm _sbExtendedSearchForm;

        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private TenancyBuildingsViewport()
            : this(null, null)
        {
        }

        public TenancyBuildingsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = TenancyBuildingsFromView();
            var listFromViewport = TenancyBuildingsFromViewport();
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

        private List<TenancyObject> TenancyBuildingsFromViewport()
        {
            var list = new List<TenancyObject>();
            for (var i = 0; i < _snapshotTenancyBuildings.Rows.Count; i++)
            {
                var row = _snapshotTenancyBuildings.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var to = new TenancyObject
                {
                    IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                    IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                    IdObject = ViewportHelper.ValueOrNull<int>(row, "id_building"),
                    RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area"),
                    RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area")
                };
                list.Add(to);
            }
            return list;
        }

        private List<TenancyObject> TenancyBuildingsFromView()
        {
            var list = new List<TenancyObject>();
            for (var i = 0; i < _vTenancyBuildings.Count; i++)
            {
                var row = (DataRowView)_vTenancyBuildings[i];
                list.Add(TenancyBuildingConverter.FromRow(row));
            }
            return list;
        }

        private bool ValidateTenancyBuildings(IEnumerable<TenancyObject> tenancyBuildings)
        {
            foreach (var building in tenancyBuildings)
            {
                if (!ViewportHelper.BuildingFundAndRentMatch(building.IdObject.Value, (int)ParentRow["id_rent_type"]) &&
                            MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемого здания. Все равно продолжить сохранение?",
                            @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
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
            GeneralDataModel = DataModel.GetInstance<EntityDataModel<Building>>();
            _kladr = DataModel.GetInstance<KladrStreetsDataModel>();
            _tenancyBuildings = DataModel.GetInstance<TenancyBuildingsAssocDataModel>();
            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _kladr.Select();
            _tenancyBuildings.Select();

            // Инициализируем snapshot-модель
            _snapshotTenancyBuildings = new DataTable("selected_buildings") {Locale = CultureInfo.InvariantCulture};
            _snapshotTenancyBuildings.Columns.Add("id_assoc").DataType = typeof(int);
            _snapshotTenancyBuildings.Columns.Add("id_building").DataType = typeof(int);
            _snapshotTenancyBuildings.Columns.Add("is_checked").DataType = typeof(bool);
            _snapshotTenancyBuildings.Columns.Add("rent_total_area").DataType = typeof(double);
            _snapshotTenancyBuildings.Columns.Add("rent_living_area").DataType = typeof(double);

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource {DataMember = "buildings"};
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);
            GeneralBindingSource.DataSource = ds;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = @"Здания найма №" + ParentRow["id_process"];
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            _vKladr = new BindingSource
            {
                DataMember = "kladr",
                DataSource = ds
            };

            _vTenancyBuildings = new BindingSource
            {
                DataMember = "tenancy_buildings_assoc",
                Filter = StaticFilter,
                DataSource = ds
            };

            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vTenancyBuildings.Count; i++)
                _snapshotTenancyBuildings.Rows.Add(TenancyBuildingConverter.ToArray((DataRowView)_vTenancyBuildings[i]));
            _vSnapshotTenancyBuildings = new BindingSource {DataSource = _snapshotTenancyBuildings};

            id_street.DataSource = _vKladr;
            id_street.ValueMember = "id_street";
            id_street.DisplayMember = "street_name";

            //Строим фильтр арендуемых зданий
            if (string.IsNullOrEmpty(DynamicFilter))
            {
                if (_vTenancyBuildings.Count > 0)
                {
                    DynamicFilter = "id_building IN (0";
                    for (var i = 0; i < _vTenancyBuildings.Count; i++)
                        DynamicFilter += "," + ((DataRowView)_vTenancyBuildings[i])["id_building"];
                    DynamicFilter += ")";
                }
            }
            GeneralBindingSource.Filter = DynamicFilter;

            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", BuildingsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", BuildingsViewport_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(_tenancyBuildings.Select(), "RowChanged", TenancyBuildingsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_tenancyBuildings.Select(), "RowDeleting", TenancyBuildingsViewport_RowDeleting);

            dataGridView.RowCount = GeneralBindingSource.Count;
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это здание?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
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
                viewport.LocateEntityBy("id_building", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as int? ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return !GeneralDataModel.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
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
            return (GeneralBindingSource.Position != -1) && !GeneralDataModel.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
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
                viewport.LocateEntityBy("id_building", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as int? ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotTenancyBuildings.Clear();
            for (var i = 0; i < _vTenancyBuildings.Count; i++)
                _snapshotTenancyBuildings.Rows.Add(TenancyBuildingConverter.ToArray((DataRowView)_vTenancyBuildings[i]));
            dataGridView.Refresh();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            _syncViews = false;
            dataGridView.EndEdit();
            _tenancyBuildings.EditingNewRecord = true;
            var list = TenancyBuildingsFromViewport();
            if (!ValidateTenancyBuildings(list))
            {
                _syncViews = true;
                _tenancyBuildings.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = _tenancyBuildings.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    var idAssoc = DataModel.GetInstance<TenancyBuildingsAssocDataModel>().Insert(list[i]);
                    if (idAssoc == -1)
                    {
                        _syncViews = true;
                        _tenancyBuildings.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)_vSnapshotTenancyBuildings[
                        _vSnapshotTenancyBuildings.Find("id_building", list[i].IdObject)])["id_assoc"] = idAssoc;
                    _tenancyBuildings.Select().Rows.Add(idAssoc, list[i].IdObject, list[i].IdProcess, list[i].RentTotalArea, list[i].RentLivingArea, 0);
                }
                else
                {
                    if (TenancyBuildingConverter.FromRow(row) == list[i])
                        continue;
                    if (DataModel.GetInstance<TenancyBuildingsAssocDataModel>().Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        _tenancyBuildings.EditingNewRecord = false;
                        return;
                    }
                    row["rent_total_area"] = list[i].RentTotalArea == null ? DBNull.Value : (object)list[i].RentTotalArea;
                    row["rent_living_area"] = list[i].RentLivingArea == null ? DBNull.Value : (object)list[i].RentLivingArea;
                }
            }
            list = TenancyBuildingsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < _vSnapshotTenancyBuildings.Count; j++)
                {
                    var row = (DataRowView)_vSnapshotTenancyBuildings[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture))
                        rowIndex = j;
                }
                if (rowIndex == -1)
                {
                    if (DataModel.GetInstance<TenancyBuildingsAssocDataModel>().Delete(list[i].IdAssoc.Value) == -1)
                    {
                        _syncViews = true;
                        _tenancyBuildings.EditingNewRecord = false;
                        return;
                    }
                    var snapshotRowIndex = -1;
                    for (var j = 0; j < _vSnapshotTenancyBuildings.Count; j++)
                        if (((DataRowView)_vSnapshotTenancyBuildings[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)_vSnapshotTenancyBuildings[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshotRowIndex = j;
                    if (snapshotRowIndex != -1)
                    {
                        var buildingRowIndex = GeneralBindingSource.Find("id_building", list[i].IdObject);
                        ((DataRowView)_vSnapshotTenancyBuildings[snapshotRowIndex]).Delete();
                        if (buildingRowIndex != -1)
                            dataGridView.InvalidateRow(buildingRowIndex);
                    }
                    _tenancyBuildings.Select().Rows.Find(list[i].IdAssoc).Delete();
                }
            }
            _syncViews = true;
            _tenancyBuildings.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
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

        private void TenancyBuildingsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = _vSnapshotTenancyBuildings.Find("id_building", e.Row["id_building"]);
                if (rowIndex != -1)
                    ((DataRowView)_vSnapshotTenancyBuildings[rowIndex]).Delete();
            }
            dataGridView.Invalidate();
        }

        private void TenancyBuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            var rowIndex = _vSnapshotTenancyBuildings.Find("id_building", e.Row["id_building"]);
            if (rowIndex == -1 && _vTenancyBuildings.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                _snapshotTenancyBuildings.Rows.Add(TenancyBuildingConverter.ToArray(e.Row));
            }
            else
            if (rowIndex != -1)
            {
                var row = (DataRowView)_vSnapshotTenancyBuildings[rowIndex];
                row["rent_total_area"] = e.Row["rent_total_area"];
                row["rent_living_area"] = e.Row["rent_living_area"];
            }
            dataGridView.Invalidate();
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + (way == SortOrder.Ascending ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            if (dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
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
        }

        private void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var idBuilding = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_building"], CultureInfo.InvariantCulture);
            var rowIndex = _vSnapshotTenancyBuildings.Find("id_building", idBuilding);
            double value;
            _syncViews = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (rowIndex == -1)
                        _snapshotTenancyBuildings.Rows.Add(null, idBuilding, e.Value, null);
                    else
                        ((DataRowView)_vSnapshotTenancyBuildings[rowIndex])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                    value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (rowIndex == -1)
                        _snapshotTenancyBuildings.Rows.Add(null, idBuilding, false, value == 0 ? DBNull.Value : (object)value, DBNull.Value);
                    else
                        ((DataRowView)_vSnapshotTenancyBuildings[rowIndex])["rent_total_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
                case "rent_living_area":
                    value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (rowIndex == -1)
                        _snapshotTenancyBuildings.Rows.Add(null, idBuilding, false, DBNull.Value, value == 0 ? DBNull.Value : (object)value);
                    else
                        ((DataRowView)_vSnapshotTenancyBuildings[rowIndex])["rent_living_area"] = value == 0 ? DBNull.Value : (object)value;
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
            var rowIndex = _vSnapshotTenancyBuildings.Find("id_building", idBuilding);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vSnapshotTenancyBuildings[rowIndex])["is_checked"];
                    break;
                case "rent_total_area":
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vSnapshotTenancyBuildings[rowIndex])["rent_total_area"];
                    break;
                case "rent_living_area":
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vSnapshotTenancyBuildings[rowIndex])["rent_living_area"];
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

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if ((dataGridView.CurrentCell.OwningColumn.Name == "rent_total_area") ||
                (dataGridView.CurrentCell.OwningColumn.Name == "rent_living_area"))
            {
                dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
                dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
                if (string.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text = @"0";
                else
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text.Substring(0, ((TextBox)e.Control).Text.Length - 3);
            }
        }

        private void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
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
                        e.Handled = ((TextBox)dataGridView.EditingControl).Text.IndexOf(',') != -1;
                    }
                    else
                        e.Handled = true;
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1260)
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
