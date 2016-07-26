using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.Properties;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyPremisesViewport: DataGridViewport
    {
        #region Models
        private DataModel _buildings;
        private DataModel _kladr;
        private DataModel _premisesTypes;
        private DataModel _subPremises;
        private DataModel _tenancyPremises;
        private DataTable _snapshotTenancyPremises;
        private DataModel _objectStates;
        private CalcDataModel _premisesFunds;
        private DataModel _fundTypes;
        #endregion Models

        #region Views
        private BindingSource _vPremisesTypes;
        private BindingSource _vSubPremises;
        private BindingSource _vTenancyPremises;
        private BindingSource _vSnapshotTenancyPremises;
        #endregion Views

        //Forms
        private SearchForm _spExtendedSearchForm;
        private SearchForm _spSimpleSearchForm;

        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        //Идентификатор развернутого помещения
        private int _idExpanded = -1;

        private TenancyPremisesViewport()
            : this(null, null)
        {
        }

        public TenancyPremisesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private bool SnapshotHasChanges()
        {
            //Проверяем помещения
            var listFromView = TenancyPremisesFromView();
            var listFromViewport = TenancyPremisesFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            bool founded;
            for (var i = 0; i < listFromView.Count; i++)
            {
                founded = false;
                for (var j = 0; j < listFromViewport.Count; j++)
                    if (listFromView[i] == listFromViewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            //Проверяем комнаты
            listFromView = ((TenancySubPremisesDetails)dataGridView.DetailsControl).TenancySubPremisesFromView();
            listFromViewport = ((TenancySubPremisesDetails)dataGridView.DetailsControl).TenancySubPremisesFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            for (var i = 0; i < listFromView.Count; i++)
            {
                founded = false;
                for (var j = 0; j < listFromViewport.Count; j++)
                    if (listFromView[i] == listFromViewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private List<TenancyObject> TenancyPremisesFromViewport()
        {
            var list = new List<TenancyObject>();
            for (var i = 0; i < _snapshotTenancyPremises.Rows.Count; i++)
            {
                var row = _snapshotTenancyPremises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var to = new TenancyObject
                {
                    IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                    IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                    IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                    RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area"),
                    RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area")
                };
                list.Add(to);
            }
            return list;
        }

        private List<TenancyObject> TenancyPremisesFromView()
        {
            var list = new List<TenancyObject>();
            for (var i = 0; i < _vTenancyPremises.Count; i++)
            {
                var row = (DataRowView)_vTenancyPremises[i];
                list.Add(TenancyPremiseConverter.FromRow(row));
            }
            return list;
        }

        private bool ValidateTenancyPremises(IEnumerable<TenancyObject> tenancyPremises)
        {
            foreach (var premises in tenancyPremises)
            {
                if (ViewportHelper.PremiseFundAndRentMatch(premises.IdObject.Value, (int) ParentRow["id_rent_type"]))
                    continue;
                var idBuilding = (int)DataModel.GetInstance<PremisesDataModel>().Select().Rows.Find(premises.IdObject.Value)["id_building"];
                if (!ViewportHelper.BuildingFundAndRentMatch(idBuilding, (int)ParentRow["id_rent_type"]) &&
                    MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемого помещения. Все равно продолжить сохранение?",
                        @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != 
                    DialogResult.Yes)
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
            GeneralDataModel = DataModel.GetInstance<PremisesDataModel>();
            _kladr = DataModel.GetInstance<KladrStreetsDataModel>();
            _buildings = DataModel.GetInstance<BuildingsDataModel>();
            _premisesTypes = DataModel.GetInstance<PremisesTypesDataModel>();
            _subPremises = DataModel.GetInstance<SubPremisesDataModel>();
            _tenancyPremises = DataModel.GetInstance<TenancyPremisesAssocDataModel>();
            _objectStates = DataModel.GetInstance<ObjectStatesDataModel>();
            _premisesFunds = CalcDataModel.GetInstance<CalcDataModelPremisesCurrentFunds>();
            _fundTypes = DataModel.GetInstance<FundTypesDataModel>();
            _objectStates.Select();
            _premisesFunds.Select();
            _fundTypes.Select();

            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _kladr.Select();
            _buildings.Select();
            _premisesTypes.Select();
            _subPremises.Select();
            _tenancyPremises.Select();

            // Инициализируем snapshot-модель
            _snapshotTenancyPremises = new DataTable("selected_premises") {Locale = CultureInfo.InvariantCulture};
            _snapshotTenancyPremises.Columns.Add("id_assoc").DataType = typeof(int);
            _snapshotTenancyPremises.Columns.Add("id_premises").DataType = typeof(int);
            _snapshotTenancyPremises.Columns.Add("is_checked").DataType = typeof(bool);
            _snapshotTenancyPremises.Columns.Add("rent_total_area").DataType = typeof(double);
            _snapshotTenancyPremises.Columns.Add("rent_living_area").DataType = typeof(double);

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);
            GeneralBindingSource.DataMember = "premises";
            GeneralBindingSource.DataSource = ds;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = @"Помещения найма №" + ParentRow["id_process"];
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            _vTenancyPremises = new BindingSource
            {
                DataMember = "tenancy_premises_assoc",
                Filter = StaticFilter,
                DataSource = ds
            };

            _vPremisesTypes = new BindingSource
            {
                DataMember = "premises_types",
                DataSource = ds
            };

            _vSubPremises = new BindingSource
            {
                DataSource = GeneralBindingSource,
                DataMember = "premises_sub_premises"
            };

            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vTenancyPremises.Count; i++)
                _snapshotTenancyPremises.Rows.Add(TenancyPremiseConverter.ToArray((DataRowView)_vTenancyPremises[i]));
            _vSnapshotTenancyPremises = new BindingSource {DataSource = _snapshotTenancyPremises};

            id_premises_type.DataSource = _vPremisesTypes;
            id_premises_type.ValueMember = "id_premises_type";
            id_premises_type.DisplayMember = "premises_type";
            // Настраивем компонент отображения комнат
            var details = new TenancySubPremisesDetails
            {
                v_sub_premises = _vSubPremises,
                sub_premises = _subPremises.Select(),
                StaticFilter = StaticFilter,
                ParentRow = ParentRow,
                ParentType = ParentType,
                menuCallback = MenuCallback
            };
            details.InitializeControl();
            dataGridView.DetailsControl = details;

            //Строим фильтр арендуемых квартир и комнат во время первой загрузки
            if (string.IsNullOrEmpty(DynamicFilter))
            {
                if (_vTenancyPremises.Count > 0)
                {
                    DynamicFilter = "id_premises IN (0";
                    for (var i = 0; i < _vTenancyPremises.Count; i++)
                        DynamicFilter += "," + ((DataRowView)_vTenancyPremises[i])["id_premises"];
                    DynamicFilter += ")";
                }
                if (details.v_snapshot_tenancy_sub_premises.Count > 0)
                {
                    if (!string.IsNullOrEmpty(DynamicFilter))
                        DynamicFilter += " OR ";
                    DynamicFilter += "id_premises IN (0";
                    for (var i = 0; i < details.v_snapshot_tenancy_sub_premises.Count; i++)
                    {
                        var row = (DataRowView)details.v_snapshot_tenancy_sub_premises[i];
                        var subPremisesRow = _subPremises.Select().Rows.Find(row["id_sub_premises"]);
                        if (subPremisesRow != null)
                            DynamicFilter += "," + subPremisesRow["id_premises"];
                    }
                    DynamicFilter += ")";
                }
            }

            GeneralBindingSource.Filter += DynamicFilter;

            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", PremisesListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", PremisesListViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(_tenancyPremises.Select(), "RowChanged", TenancyPremisesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_tenancyPremises.Select(), "RowDeleting", TenancyPremisesViewport_RowDeleting);

            AddEventHandler<EventArgs>(_premisesFunds, "RefreshEvent", premises_funds_RefreshEvent);

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
            var viewport = new PremisesViewport(null, MenuCallback)
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
                viewport.LocateEntityBy("id_premises", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int? ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotTenancyPremises.Clear();
            for (var i = 0; i < _vTenancyPremises.Count; i++)
                _snapshotTenancyPremises.Rows.Add(TenancyPremiseConverter.ToArray((DataRowView)_vTenancyPremises[i]));
            dataGridView.Refresh();
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).CancelRecord();
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
            _tenancyPremises.EditingNewRecord = true;
            var list = TenancyPremisesFromViewport();
            // Проверяем данные о помещениях
            if (!ValidateTenancyPremises(list))
            {
                _syncViews = true;
                _tenancyPremises.EditingNewRecord = false;
                return;
            }
            // Проверяем данные о комнатах
            if (!((TenancySubPremisesDetails)dataGridView.DetailsControl).ValidateTenancySubPremises(
                ((TenancySubPremisesDetails)dataGridView.DetailsControl).TenancySubPremisesFromViewport()))
            {
                _syncViews = true;
                _tenancyPremises.EditingNewRecord = false;
                return;
            }
            // Сохраняем помещения в базу данных
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = _tenancyPremises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    var idAssoc = DataModel.GetInstance<TenancyPremisesAssocDataModel>().Insert(list[i]);
                    if (idAssoc == -1)
                    {
                        _syncViews = true;
                        _tenancyPremises.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)_vSnapshotTenancyPremises[
                        _vSnapshotTenancyPremises.Find("id_premises", list[i].IdObject)])["id_assoc"] = idAssoc;
                    _tenancyPremises.Select().Rows.Add(idAssoc, list[i].IdObject, list[i].IdProcess, list[i].RentTotalArea, list[i].RentLivingArea, 0);
                }
                else
                {
                    if (TenancyPremiseConverter.FromRow(row) == list[i])
                        continue;
                    if (DataModel.GetInstance<TenancyPremisesAssocDataModel>().Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        _tenancyPremises.EditingNewRecord = false;
                        return;
                    }
                    row["rent_total_area"] = list[i].RentTotalArea == null ? DBNull.Value : (object)list[i].RentTotalArea;
                    row["rent_living_area"] = list[i].RentLivingArea == null ? DBNull.Value : (object)list[i].RentLivingArea;
                }
            }
            list = TenancyPremisesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < _vSnapshotTenancyPremises.Count; j++)
                {
                    var row = (DataRowView)_vSnapshotTenancyPremises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture))
                        rowIndex = j;
                }
                if (rowIndex == -1)
                {
                    if (DataModel.GetInstance<TenancyPremisesAssocDataModel>().Delete(list[i].IdAssoc.Value) == -1)
                    {
                        _syncViews = true;
                        _tenancyPremises.EditingNewRecord = false;
                        return;
                    }
                    var snapshotRowIndex = -1;
                    for (var j = 0; j < _vSnapshotTenancyPremises.Count; j++)
                        if (((DataRowView)_vSnapshotTenancyPremises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)_vSnapshotTenancyPremises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshotRowIndex = j;
                    if (snapshotRowIndex != -1)
                    {
                        var premisesRowIndex = GeneralBindingSource.Find("id_premises", list[i].IdObject);
                        ((DataRowView)_vSnapshotTenancyPremises[snapshotRowIndex]).Delete();
                        if (premisesRowIndex != -1)
                            dataGridView.InvalidateRow(premisesRowIndex);
                    }
                    _tenancyPremises.Select().Rows.Find(list[i].IdAssoc).Delete();
                }
            }
            _syncViews = true;
            _tenancyPremises.EditingNewRecord = false;
            // Сохраняем комнаты в базу данных
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).SaveRecord();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return !GeneralDataModel.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void InsertRecord()
        {
            var viewport = new PremisesViewport(null, MenuCallback)
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
            var viewport = new PremisesViewport(null, MenuCallback)
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
                viewport.LocateEntityBy("id_premises", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int? ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
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
            base.OnClosing(e);
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.SubPremisesViewport,
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
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, 
                "id_premises = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Premises);
        }

        private void premises_funds_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        private void TenancyPremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = _vSnapshotTenancyPremises.Find("id_premises", e.Row["id_premises"]);
                if (rowIndex != -1)
                    ((DataRowView)_vSnapshotTenancyPremises[rowIndex]).Delete();
            }
            dataGridView.Refresh();
        }

        private void TenancyPremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            var rowIndex = _vSnapshotTenancyPremises.Find("id_premises", e.Row["id_premises"]);
            if (rowIndex == -1 && _vTenancyPremises.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                _snapshotTenancyPremises.Rows.Add(e.Row["id_assoc"], e.Row["id_premises"], true, e.Row["rent_total_area"], e.Row["rent_living_area"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = (DataRowView)_vSnapshotTenancyPremises[rowIndex];
                    row["rent_total_area"] = e.Row["rent_total_area"];
                    row["rent_living_area"] = e.Row["rent_living_area"];
                }
            dataGridView.Invalidate();
        }

        private void PremisesListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void PremisesListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void dataGridView_BeforeCollapseDetails(object sender, DataGridViewDetailsEventArgs e)
        {
            dataGridView.Rows[e.RowIndex].Cells["is_checked"].Style.Alignment = DataGridViewContentAlignment.TopCenter;
        }

        private void dataGridView_BeforeExpandDetails(object sender, DataGridViewDetailsEventArgs e)
        {
            dataGridView.Rows[e.RowIndex].Cells["is_checked"].Style.Alignment = DataGridViewContentAlignment.TopCenter;
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).CalcControlHeight();
            var width = 0;
            for (var i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            var width = 0;
            for (var i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
            if (dataGridView.Size.Width > 1540)
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

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].Name != "image") return;
            if (_idExpanded != Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture))
            {
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Resource.minus;
                _idExpanded = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
                dataGridView.ExpandDetails(e.RowIndex);
            }
            else
            {
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Resource.plus;
                _idExpanded = -1;
                dataGridView.CollapseDetails();
            }
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            dataGridView.CollapseDetails();
            _idExpanded = -1;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + (way == SortOrder.Ascending ? "ASC" : "DESC");
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
            _idExpanded = -1;
            dataGridView.CollapseDetails();
        }

        private void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var idPremises = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var rowIndex = _vSnapshotTenancyPremises.Find("id_premises", idPremises);
            double value;
            _syncViews = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (rowIndex == -1)
                        _snapshotTenancyPremises.Rows.Add(null, idPremises, e.Value, null);
                    else
                        ((DataRowView)_vSnapshotTenancyPremises[rowIndex])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                    value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (rowIndex == -1)
                        _snapshotTenancyPremises.Rows.Add(null, idPremises, false, value == 0 ? DBNull.Value : (object)value, DBNull.Value);
                    else
                        ((DataRowView)_vSnapshotTenancyPremises[rowIndex])["rent_total_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
                case "rent_living_area":
                    value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (rowIndex == -1)
                        _snapshotTenancyPremises.Rows.Add(null, idPremises, false, DBNull.Value, value == 0 ? DBNull.Value : (object)value);
                    else
                        ((DataRowView)_vSnapshotTenancyPremises[rowIndex])["rent_living_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
            }
            _syncViews = true;
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var idPremises = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var rowIndex = _vSnapshotTenancyPremises.Find("id_premises", idPremises);
            var row = (DataRowView)GeneralBindingSource[e.RowIndex];
            var buildingRow = _buildings.Select().Rows.Find(row["id_building"]);
            if (buildingRow == null)
                return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "image":
                    e.Value = _idExpanded == idPremises ? Resource.minus : Resource.plus;
                    break;
                case "is_checked":
                case "rent_total_area":
                case "rent_living_area":
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vSnapshotTenancyPremises[rowIndex])[dataGridView.Columns[e.ColumnIndex].Name];
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
                case "id_premises":
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
            }
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if ((dataGridView.CurrentCell.OwningColumn.Name != "rent_total_area") &&
                (dataGridView.CurrentCell.OwningColumn.Name != "rent_living_area")) return;
            dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
            dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
            if (string.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
                ((TextBox)e.Control).Text = ((TextBox)e.Control).Text = @"0";
            else
                ((TextBox)e.Control).Text = ((TextBox)e.Control).Text.Substring(0, ((TextBox)e.Control).Text.Length - 3);
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

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }
    }
}
