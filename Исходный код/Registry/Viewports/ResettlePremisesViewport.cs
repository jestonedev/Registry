using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.SearchForms;
using Registry.Viewport.Properties;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ResettlePremisesViewport: DataGridViewport
    {
        #region Models
        private DataModel buildings;
        private DataModel kladr;
        private DataModel premises_types;
        private DataModel sub_premises;
        private DataModel resettle_premises;
        private DataTable snapshot_resettle_premises;
        #endregion Models

        #region Views
        private BindingSource v_buildings;
        private BindingSource v_premises_types;
        private BindingSource v_sub_premises;
        private BindingSource v_resettle_premises;
        private BindingSource v_snapshot_resettle_premises;
        #endregion Views

        //Forms
        private SearchForm spExtendedSearchForm;
        private SearchForm spSimpleSearchForm;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        //Идентификатор развернутого помещения
        private int id_expanded = -1;

        private ResettleEstateObjectWay way = ResettleEstateObjectWay.From;

        public ResettleEstateObjectWay Way { get { return way; } set { way = value; } }

        private ResettlePremisesViewport()
            : this(null)
        {
        }

        public ResettlePremisesViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        public ResettlePremisesViewport(ResettlePremisesViewport resettlePremisesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = resettlePremisesViewport.DynamicFilter;
            StaticFilter = resettlePremisesViewport.StaticFilter;
            ParentRow = resettlePremisesViewport.ParentRow;
            ParentType = resettlePremisesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            //Проверяем помещения
            var list_from_view = ResettlePremisesFromView();
            var list_from_viewport = ResettlePremisesFromViewport();
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
            //Проверяем комнаты
            list_from_view = ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromView();
            list_from_viewport = ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            founded = false;
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
                dataRowView["id_premises"], 
                true
            };
        }

        public void LocatePremisesBy(int id)
        {
            var Position = GeneralBindingSource.Find("id_premises", id);
            if (Position > 0)
                GeneralBindingSource.Position = Position;
        }

        private List<ResettleObject> ResettlePremisesFromViewport()
        {
            var list = new List<ResettleObject>();
            for (var i = 0; i < snapshot_resettle_premises.Rows.Count; i++)
            {
                var row = snapshot_resettle_premises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var to = new ResettleObject();
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises");
                list.Add(to);
            }
            return list;
        }

        private List<ResettleObject> ResettlePremisesFromView()
        {
            var list = new List<ResettleObject>();
            for (var i = 0; i < v_resettle_premises.Count; i++)
            {
                var to = new ResettleObject();
                var row = ((DataRowView)v_resettle_premises[i]);
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises");
                list.Add(to);
            }
            return list;
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.PremisesDataModel);
            kladr = DataModel.GetInstance(DataModelType.KladrStreetsDataModel);
            buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel);
            premises_types = DataModel.GetInstance(DataModelType.PremisesTypesDataModel);
            sub_premises = DataModel.GetInstance(DataModelType.SubPremisesDataModel);

            if (way == ResettleEstateObjectWay.From)
                resettle_premises = DataModel.GetInstance(DataModelType.ResettlePremisesFromAssocDataModel);
            else
                resettle_premises = DataModel.GetInstance(DataModelType.ResettlePremisesToAssocDataModel);

            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            kladr.Select();
            buildings.Select();
            premises_types.Select();
            sub_premises.Select();
            resettle_premises.Select();

            // Инициализируем snapshot-модель
            snapshot_resettle_premises = new DataTable("selected_premises");
            snapshot_resettle_premises.Locale = CultureInfo.InvariantCulture;
            snapshot_resettle_premises.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_resettle_premises.Columns.Add("id_premises").DataType = typeof(int);
            snapshot_resettle_premises.Columns.Add("is_checked").DataType = typeof(bool);

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataMember = "premises";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter += DynamicFilter;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.ResettleProcess))
            {
                if (way == ResettleEstateObjectWay.From)
                    Text = @"Помещения (из) переселения №" + ParentRow["id_process"];
                else
                    Text = @"Помещения (в) переселения №" + ParentRow["id_process"];
            }
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_resettle_premises = new BindingSource();
            if (way == ResettleEstateObjectWay.From)
                v_resettle_premises.DataMember = "resettle_premises_from_assoc";
            else
                v_resettle_premises.DataMember = "resettle_premises_to_assoc";
            v_resettle_premises.Filter = StaticFilter;
            v_resettle_premises.DataSource = ds;

            v_buildings = new BindingSource();
            v_buildings.DataMember = "buildings";
            v_buildings.DataSource = ds;

            v_premises_types = new BindingSource();
            v_premises_types.DataMember = "premises_types";
            v_premises_types.DataSource = ds;

            v_sub_premises = new BindingSource();
            v_sub_premises.DataSource = GeneralBindingSource;
            v_sub_premises.DataMember = "premises_sub_premises";

            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_resettle_premises.Count; i++)
                snapshot_resettle_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_premises[i])));
            v_snapshot_resettle_premises = new BindingSource();
            v_snapshot_resettle_premises.DataSource = snapshot_resettle_premises;

            id_premises_type.DataSource = v_premises_types;
            id_premises_type.ValueMember = "id_premises_type";
            id_premises_type.DisplayMember = "premises_type";

            // Настраивем компонент отображения комнат
            var details = new ResettleSubPremisesDetails();
            details.v_sub_premises = v_sub_premises;
            details.sub_premises = sub_premises.Select();
            details.StaticFilter = StaticFilter;
            details.ParentRow = ParentRow;
            details.ParentType = ParentType;
            details.menuCallback = MenuCallback;
            details.Way = way;
            details.InitializeControl();
            dataGridView.DetailsControl = details;

            GeneralDataModel.Select().RowChanged += PremisesListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += PremisesListViewport_RowDeleted;
            resettle_premises.Select().RowChanged += ResettlePremisesViewport_RowChanged;
            resettle_premises.Select().RowDeleting += ResettlePremisesViewport_RowDeleting;
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
                var id_building = (int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"];
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
            var viewport = new PremisesViewport(MenuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_resettle_premises.Clear();
            for (var i = 0; i < v_resettle_premises.Count; i++)
                snapshot_resettle_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_premises[i])));
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
            sync_views = false;
            var resettlePremisesFromAssoc = DataModel.GetInstance(DataModelType.ResettlePremisesFromAssocDataModel);
            var resettlePremisesToAssoc = DataModel.GetInstance(DataModelType.ResettlePremisesToAssocDataModel);
            resettlePremisesFromAssoc.EditingNewRecord = true;
            resettlePremisesToAssoc.EditingNewRecord = true;
            var list = ResettlePremisesFromViewport();
            // Проверяем данные о помещениях
            if (!ValidateResettlePremises(list))
            {
                sync_views = true;
                resettlePremisesFromAssoc.EditingNewRecord = false;
                resettlePremisesToAssoc.EditingNewRecord = false;
                return;
            }
            // Проверяем данные о комнатах
            if (!ResettleSubPremisesDetails.ValidateResettleSubPremises(
                ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromViewport()))
            {
                sync_views = true; 
                resettlePremisesFromAssoc.EditingNewRecord = false;
                resettlePremisesToAssoc.EditingNewRecord = false;
                return;
            }
            // Сохраняем помещения в базу данных
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = resettle_premises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {

                    var id_assoc = -1;
                    if (way == ResettleEstateObjectWay.From)
                        id_assoc = resettlePremisesFromAssoc.Insert(list[i]);
                    else
                        id_assoc = resettlePremisesToAssoc.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        resettlePremisesFromAssoc.EditingNewRecord = false;
                        resettlePremisesToAssoc.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_resettle_premises[
                        v_snapshot_resettle_premises.Find("id_premises", list[i].IdObject)])["id_assoc"] = id_assoc;
                    resettle_premises.Select().Rows.Add(id_assoc, list[i].IdObject, list[i].IdProcess, 0);
                }
            }
            list = ResettlePremisesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < v_snapshot_resettle_premises.Count; j++)
                {
                    var row = (DataRowView)v_snapshot_resettle_premises[j];
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
                        affected = resettlePremisesFromAssoc.Delete(list[i].IdAssoc.Value);
                    else
                        affected = resettlePremisesToAssoc.Delete(list[i].IdAssoc.Value);
                    if (affected == -1)
                    {
                        sync_views = true;
                        resettlePremisesFromAssoc.EditingNewRecord = false;
                        resettlePremisesToAssoc.EditingNewRecord = false;
                        return;
                    }
                    var snapshot_row_index = -1;
                    for (var j = 0; j < v_snapshot_resettle_premises.Count; j++)
                        if (((DataRowView)v_snapshot_resettle_premises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_resettle_premises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        var premises_row_index = GeneralBindingSource.Find("id_premises", list[i].IdObject);
                        ((DataRowView)v_snapshot_resettle_premises[snapshot_row_index]).Delete();
                        if (premises_row_index != -1)
                            dataGridView.InvalidateRow(premises_row_index);
                    }
                    resettle_premises.Select().Rows.Find(list[i].IdAssoc).Delete();
                }
            }
            sync_views = true;
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
            var viewport = new PremisesViewport(MenuCallback);
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
            var viewport = new PremisesViewport(MenuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (GeneralBindingSource.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] as int?) ?? -1);
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
            GeneralDataModel.Select().RowChanged -= PremisesListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= PremisesListViewport_RowDeleted;
            resettle_premises.Select().RowChanged -= ResettlePremisesViewport_RowChanged;
            resettle_premises.Select().RowDeleting -= ResettlePremisesViewport_RowDeleting;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= PremisesListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= PremisesListViewport_RowDeleted;
            resettle_premises.Select().RowChanged -= ResettlePremisesViewport_RowChanged;
            resettle_premises.Select().RowDeleting -= ResettlePremisesViewport_RowDeleting;
            base.ForceClose();
        }

        public override bool HasAssocViewport(ViewportType viewportType)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.SubPremisesViewport,
                ViewportType.OwnershipListViewport,
                ViewportType.RestrictionListViewport,
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport
            };
            return reports.Contains(viewportType) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport(ViewportType viewportType)
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_premises = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Premises);
        }

        void ResettlePremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = v_snapshot_resettle_premises.Find("id_premises", e.Row["id_premises"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_resettle_premises[row_index]).Delete();
            }
            dataGridView.Refresh();
        }

        void ResettlePremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            var row_index = v_snapshot_resettle_premises.Find("id_premises", e.Row["id_premises"]);
            if (row_index == -1 && v_resettle_premises.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_resettle_premises.Rows.Add(e.Row["id_assoc"], e.Row["id_premises"], true);
            }
            dataGridView.Invalidate();
        }

        void PremisesListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void PremisesListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void dataGridView_BeforeCollapseDetails(object sender, DataGridViewDetailsEventArgs e)
        {
            dataGridView.Rows[e.RowIndex].Cells["is_checked"].Style.Alignment = DataGridViewContentAlignment.TopCenter;
        }

        void dataGridView_BeforeExpandDetails(object sender, DataGridViewDetailsEventArgs e)
        {
            dataGridView.Rows[e.RowIndex].Cells["is_checked"].Style.Alignment = DataGridViewContentAlignment.TopCenter;
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).CalcControlHeight();
            var width = 0;
            for (var i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
        }

        void dataGridView_Resize(object sender, EventArgs e)
        {
            var width = 0;
            for (var i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
            if (dataGridView.Size.Width > 1240)
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

        void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].Name == "image")
            {
                if (id_expanded != Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture))
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Resource.minus;
                    id_expanded = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
                    dataGridView.ExpandDetails(e.RowIndex);
                }
                else
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Resource.plus;
                    id_expanded = -1;
                    dataGridView.CollapseDetails();
                }
            }
        }

        void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            dataGridView.CollapseDetails();
            id_expanded = -1;
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

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
            id_expanded = -1;
            dataGridView.CollapseDetails();
            dataGridView.Refresh();
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var id_premises = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_resettle_premises.Find("id_premises", id_premises);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_resettle_premises.Rows.Add(null, id_premises, e.Value);
                    else
                        ((DataRowView)v_snapshot_resettle_premises[row_index])["is_checked"] = e.Value;
                    break;
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var id_premises = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_resettle_premises.Find("id_premises", id_premises);
            var row = ((DataRowView)GeneralBindingSource[e.RowIndex]);
            var building_row = buildings.Select().Rows.Find(row["id_building"]);
            if (building_row == null)
                return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "image":
                    if (id_expanded == id_premises)
                        e.Value = Resource.minus;
                    else
                        e.Value = Resource.plus;
                    break;
                case "is_checked":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_resettle_premises[row_index])["is_checked"];
                    break;
                case "id_premises":
                    e.Value = row["id_premises"];
                    break;
                case "id_street":
                    var kladr_row = kladr.Select().Rows.Find(building_row["id_street"]);
                    string street_name = null;
                    if (kladr_row != null)
                        street_name = kladr_row["street_name"].ToString();
                    e.Value = street_name;
                    break;
                case "house":
                    e.Value = building_row["house"];
                    break;
                case "premises_num":
                    e.Value = row["premises_num"];
                    break;
                case "id_premises_type":
                    e.Value = row["id_premises_type"];
                    break;
                case "cadastral_num":
                    e.Value = row["cadastral_num"];
                    break;
                case "total_area":
                    e.Value = row["total_area"];
                    break;
                case "living_area":
                    e.Value = row["living_area"];
                    break;
            }
        }

        void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }
    }
}
