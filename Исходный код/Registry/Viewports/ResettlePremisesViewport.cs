using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.Properties;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;
using System.Linq;
using Registry.Viewport.EntityConverters;

namespace Registry.Viewport
{
    internal sealed partial class ResettlePremisesViewport: DataGridViewport
    {
        #region Models
        private DataModel _buildings;
        private DataModel _kladr;
        private DataModel _premisesTypes;
        private DataModel _subPremises;
        private DataModel _resettlePremises;
        private DataTable _snapshotResettlePremises;
        #endregion Models

        #region Views
        private BindingSource _vPremisesTypes;
        private BindingSource _vSubPremises;
        private BindingSource _vResettlePremises;
        private BindingSource _vSnapshotResettlePremises;
        #endregion Views

        //Forms
        private SearchForm _spExtendedSearchForm;
        private SearchForm _spSimpleSearchForm;

        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        //Идентификатор развернутого помещения
        private int _idExpanded = -1;

        private ResettleEstateObjectWay _way = ResettleEstateObjectWay.From;

        public ResettleEstateObjectWay Way { get { return _way; } set { _way = value; } }

        private ResettlePremisesViewport()
            : this(null, null)
        {
        }

        public ResettlePremisesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private bool SnapshotHasChanges()
        {
            //Проверяем помещения
            var listFromView = ResettlePremisesFromView();
            var listFromViewport = ResettlePremisesFromViewport();
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
            listFromView = ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromView();
            listFromViewport = ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromViewport();
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

        public void LocatePremisesBy(int id)
        {
            var position = GeneralBindingSource.Find("id_premises", id);
            if (position > 0)
                GeneralBindingSource.Position = position;
        }

        private List<ResettleObject> ResettlePremisesFromViewport()
        {
            var list = new List<ResettleObject>();
            for (var i = 0; i < _snapshotResettlePremises.Rows.Count; i++)
            {
                var row = _snapshotResettlePremises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var ro = new ResettleObject
                {
                    IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                    IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                    IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises")
                };
                list.Add(ro);
            }
            return list;
        }

        private List<ResettleObject> ResettlePremisesFromView()
        {
            return (from DataRowView row in _vResettlePremises select ResettlePremiseConverter.FromRow(row)).ToList();
        }

        private static bool ValidateResettlePremises(List<ResettleObject> resettlePremises)
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
            GeneralDataModel = EntityDataModel<Premise>.GetInstance();
            _kladr = DataModel.GetInstance<KladrStreetsDataModel>();
            _buildings = DataModel.GetInstance<EntityDataModel<Building>>();
            _premisesTypes = DataModel.GetInstance<PremisesTypesDataModel>();
            _subPremises = EntityDataModel<SubPremise>.GetInstance();

            _resettlePremises = _way == ResettleEstateObjectWay.From ? 
                DataModel.GetInstance<ResettlePremisesFromAssocDataModel>() : 
                DataModel.GetInstance<ResettlePremisesToAssocDataModel>();

            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _kladr.Select();
            _buildings.Select();
            _premisesTypes.Select();
            _subPremises.Select();
            _resettlePremises.Select();

            // Инициализируем snapshot-модель
            _snapshotResettlePremises = new DataTable("selected_premises") {Locale = CultureInfo.InvariantCulture};
            _snapshotResettlePremises.Columns.Add("id_assoc").DataType = typeof(int);
            _snapshotResettlePremises.Columns.Add("id_premises").DataType = typeof(int);
            _snapshotResettlePremises.Columns.Add("is_checked").DataType = typeof(bool);

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);
            GeneralBindingSource.DataMember = "premises";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter += DynamicFilter;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.ResettleProcess))
            {
                if (_way == ResettleEstateObjectWay.From)
                    Text = @"Помещения (из) переселения №" + ParentRow["id_process"];
                else
                    Text = @"Помещения (в) переселения №" + ParentRow["id_process"];
            }
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            _vResettlePremises = new BindingSource
            {
                DataMember =
                    _way == ResettleEstateObjectWay.From ? "resettle_premises_from_assoc" : "resettle_premises_to_assoc",
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
            for (var i = 0; i < _vResettlePremises.Count; i++)
                _snapshotResettlePremises.Rows.Add(ResettlePremiseConverter.ToArray((DataRowView)_vResettlePremises[i]));
            _vSnapshotResettlePremises = new BindingSource {DataSource = _snapshotResettlePremises};

            id_premises_type.DataSource = _vPremisesTypes;
            id_premises_type.ValueMember = "id_premises_type";
            id_premises_type.DisplayMember = "premises_type";

            // Настраивем компонент отображения комнат
            var details = new ResettleSubPremisesDetails
            {
                v_sub_premises = _vSubPremises,
                sub_premises = _subPremises.Select(),
                StaticFilter = StaticFilter,
                ParentRow = ParentRow,
                ParentType = ParentType,
                menuCallback = MenuCallback,
                Way = _way
            };
            details.InitializeControl();
            dataGridView.DetailsControl = details;

            //Строим фильтр квартир и комнат выбранных во время первой загрузки
            if (string.IsNullOrEmpty(DynamicFilter))
            {
                if (_vResettlePremises.Count > 0)
                {
                    DynamicFilter = "id_premises IN (0";
                    for (var i = 0; i < _vResettlePremises.Count; i++)
                        DynamicFilter += "," + ((DataRowView)_vResettlePremises[i])["id_premises"];
                    DynamicFilter += ")";
                }
                if (details.v_snapshot_resettle_sub_premises.Count > 0)
                {
                    if (!string.IsNullOrEmpty(DynamicFilter))
                        DynamicFilter += " OR ";
                    DynamicFilter += "id_premises IN (0";
                    for (var i = 0; i < details.v_snapshot_resettle_sub_premises.Count; i++)
                    {
                        var row = ((DataRowView)details.v_snapshot_resettle_sub_premises[i]);
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

            AddEventHandler<DataRowChangeEventArgs>(_resettlePremises.Select(), "RowChanged", ResettlePremisesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_resettlePremises.Select(), "RowDeleting", ResettlePremisesViewport_RowDeleting);

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
                viewport.LocateEntityBy("id_premises", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotResettlePremises.Clear();
            for (var i = 0; i < _vResettlePremises.Count; i++)
                _snapshotResettlePremises.Rows.Add(ResettlePremiseConverter.ToArray((DataRowView)_vResettlePremises[i]));
            dataGridView.Refresh();
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).CancelRecord();
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
            var resettlePremisesFromAssoc = DataModel.GetInstance<ResettlePremisesFromAssocDataModel>();
            var resettlePremisesToAssoc = DataModel.GetInstance<ResettlePremisesToAssocDataModel>();
            resettlePremisesFromAssoc.EditingNewRecord = true;
            resettlePremisesToAssoc.EditingNewRecord = true;
            var list = ResettlePremisesFromViewport();
            // Проверяем данные о помещениях
            if (!ValidateResettlePremises(list))
            {
                _syncViews = true;
                resettlePremisesFromAssoc.EditingNewRecord = false;
                resettlePremisesToAssoc.EditingNewRecord = false;
                return;
            }
            // Проверяем данные о комнатах
            if (!ResettleSubPremisesDetails.ValidateResettleSubPremises(
                ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromViewport()))
            {
                _syncViews = true; 
                resettlePremisesFromAssoc.EditingNewRecord = false;
                resettlePremisesToAssoc.EditingNewRecord = false;
                return;
            }
            // Сохраняем помещения в базу данных
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = _resettlePremises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    var idAssoc = _way == ResettleEstateObjectWay.From ? 
                        resettlePremisesFromAssoc.Insert(list[i]) : 
                        resettlePremisesToAssoc.Insert(list[i]);
                    if (idAssoc == -1)
                    {
                        _syncViews = true;
                        resettlePremisesFromAssoc.EditingNewRecord = false;
                        resettlePremisesToAssoc.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)_vSnapshotResettlePremises[
                        _vSnapshotResettlePremises.Find("id_premises", list[i].IdObject)])["id_assoc"] = idAssoc;
                    _resettlePremises.Select().Rows.Add(idAssoc, list[i].IdObject, list[i].IdProcess, 0);
                }
            }
            list = ResettlePremisesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < _vSnapshotResettlePremises.Count; j++)
                {
                    var row = (DataRowView)_vSnapshotResettlePremises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture))
                        rowIndex = j;
                }
                if (rowIndex == -1)
                {
                    var affected = _way == ResettleEstateObjectWay.From ? 
                        resettlePremisesFromAssoc.Delete(list[i].IdAssoc.Value) : 
                        resettlePremisesToAssoc.Delete(list[i].IdAssoc.Value);
                    if (affected == -1)
                    {
                        _syncViews = true;
                        resettlePremisesFromAssoc.EditingNewRecord = false;
                        resettlePremisesToAssoc.EditingNewRecord = false;
                        return;
                    }
                    var snapshotRowIndex = -1;
                    for (var j = 0; j < _vSnapshotResettlePremises.Count; j++)
                        if (((DataRowView)_vSnapshotResettlePremises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)_vSnapshotResettlePremises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshotRowIndex = j;
                    if (snapshotRowIndex != -1)
                    {
                        var premisesRowIndex = GeneralBindingSource.Find("id_premises", list[i].IdObject);
                        ((DataRowView)_vSnapshotResettlePremises[snapshotRowIndex]).Delete();
                        if (premisesRowIndex != -1)
                            dataGridView.InvalidateRow(premisesRowIndex);
                    }
                    _resettlePremises.Select().Rows.Find(list[i].IdAssoc).Delete();
                }
            }
            _syncViews = true;
            resettlePremisesFromAssoc.EditingNewRecord = false;
            resettlePremisesToAssoc.EditingNewRecord = false;
            // Сохраняем комнаты в базу данных
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).SaveRecord();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
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
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
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
                viewport.LocateEntityBy("id_premises", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new ResettlePremisesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (GeneralBindingSource.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
            return viewport;
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

        private void ResettlePremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = _vSnapshotResettlePremises.Find("id_premises", e.Row["id_premises"]);
                if (rowIndex != -1)
                    ((DataRowView)_vSnapshotResettlePremises[rowIndex]).Delete();
            }
            dataGridView.Refresh();
        }

        private void ResettlePremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            var rowIndex = _vSnapshotResettlePremises.Find("id_premises", e.Row["id_premises"]);
            if (rowIndex == -1 && _vResettlePremises.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                _snapshotResettlePremises.Rows.Add(e.Row["id_assoc"], e.Row["id_premises"], true);
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
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).CalcControlHeight();
            var width = 0;
            for (var i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            var width = 0;
            for (var i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
            if (dataGridView.Size.Width > 1240)
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
            if (dataGridView.Columns[e.ColumnIndex].Name == "image")
            {
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
                GeneralBindingSource.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
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
            dataGridView.Refresh();
        }

        private void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var idPremises = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var rowIndex = _vSnapshotResettlePremises.Find("id_premises", idPremises);
            _syncViews = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (rowIndex == -1)
                        _snapshotResettlePremises.Rows.Add(null, idPremises, e.Value);
                    else
                        ((DataRowView)_vSnapshotResettlePremises[rowIndex])["is_checked"] = e.Value;
                    break;
            }
            _syncViews = true;
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var idPremises = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var rowIndex = _vSnapshotResettlePremises.Find("id_premises", idPremises);
            var row = ((DataRowView)GeneralBindingSource[e.RowIndex]);
            var buildingRow = _buildings.Select().Rows.Find(row["id_building"]);
            if (buildingRow == null)
                return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "image":
                    e.Value = _idExpanded == idPremises ? Resource.minus : Resource.plus;
                    break;
                case "is_checked":
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vSnapshotResettlePremises[rowIndex])["is_checked"];
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
                case "cadastral_num":
                case "total_area":
                case "living_area":
                    e.Value = row[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
            }
        }

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }
    }
}
