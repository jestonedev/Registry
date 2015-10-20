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
    internal sealed class ResettlePremisesViewport: Viewport
    {
        #region Components
        private DataGridViewWithDetails dataGridView;
        private DataGridViewImageColumn image;
        private DataGridViewCheckBoxColumn is_checked;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn premises_num;
        private DataGridViewComboBoxColumn id_premises_type;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn cadastral_num;
        #endregion Components

        #region Models
        private DataModel premises;
        private DataModel buildings;
        private DataModel kladr;
        private DataModel premises_types;
        private DataModel sub_premises;
        private DataModel resettle_premises;
        private DataTable snapshot_resettle_premises;
        #endregion Models

        #region Views
        private BindingSource v_premises;
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
            var Position = v_premises.Find("id_premises", id);
            if (Position > 0)
                v_premises.Position = Position;
        }

        private static ResettleObject RowToResettlePremises(DataRow row)
        {
            var to = new ResettleObject();
            to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
            to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises");
            return to;
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

        public override int GetRecordCount()
        {
            return v_premises.Count;
        }

        public override bool CanMoveFirst()
        {
            return v_premises.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_premises.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override void MoveFirst()
        {
            v_premises.MoveFirst();
        }

        public override void MovePrev()
        {
            v_premises.MovePrevious();
        }

        public override void MoveNext()
        {
            v_premises.MoveNext();
        }

        public override void MoveLast()
        {
            v_premises.MoveLast();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            premises = DataModel.GetInstance(DataModelType.PremisesDataModel);
            kladr = DataModel.GetInstance(DataModelType.KladrStreetsDataModel);
            buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel);
            premises_types = DataModel.GetInstance(DataModelType.PremisesTypesDataModel);
            sub_premises = DataModel.GetInstance(DataModelType.SubPremisesDataModel);

            if (way == ResettleEstateObjectWay.From)
                resettle_premises = DataModel.GetInstance(DataModelType.ResettlePremisesFromAssocDataModel);
            else
                resettle_premises = DataModel.GetInstance(DataModelType.ResettlePremisesToAssocDataModel);

            // Ожидаем дозагрузки данных, если это необходимо
            premises.Select();
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

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += v_premises_CurrentItemChanged;
            v_premises.DataMember = "premises";
            v_premises.DataSource = ds;
            v_premises.Filter += DynamicFilter;

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
            v_sub_premises.DataSource = v_premises;
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

            premises.Select().RowChanged += PremisesListViewport_RowChanged;
            premises.Select().RowDeleted += PremisesListViewport_RowDeleted;
            resettle_premises.Select().RowChanged += ResettlePremisesViewport_RowChanged;
            resettle_premises.Select().RowDeleting += ResettlePremisesViewport_RowDeleting;
            dataGridView.RowCount = v_premises.Count;
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public override bool CanDeleteRecord()
        {
            return (v_premises.Position > -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это помещение?", @"Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)v_premises.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)v_premises.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                var id_building = (int)((DataRowView)v_premises[v_premises.Position])["id_building"];
                if (premises.Delete((int)((DataRowView)v_premises.Current)["id_premises"]) == -1)
                    return;
                ((DataRowView)v_premises[v_premises.Position]).Delete();
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
            v_premises.Filter = DynamicFilter;
            dataGridView.RowCount = v_premises.Count;
        }

        public override void ClearSearch()
        {
            v_premises.Filter = "";
            dataGridView.RowCount = v_premises.Count;
            DynamicFilter = "";
        }

        public override bool CanOpenDetails()
        {
            return (v_premises.Position != -1) && AccessControl.HasPrivelege(Priveleges.RegistryRead);
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
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as int?) ?? -1);
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
                        var premises_row_index = v_premises.Find("id_premises", list[i].IdObject);
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
            return (!premises.EditingNewRecord) &&
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
            return (v_premises.Position != -1) && (!premises.EditingNewRecord) &&
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
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as int?) ?? -1);
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
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as int?) ?? -1);
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
            premises.Select().RowChanged -= PremisesListViewport_RowChanged;
            premises.Select().RowDeleted -= PremisesListViewport_RowDeleted;
            resettle_premises.Select().RowChanged -= ResettlePremisesViewport_RowChanged;
            resettle_premises.Select().RowDeleting -= ResettlePremisesViewport_RowDeleting;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            premises.Select().RowChanged -= PremisesListViewport_RowChanged;
            premises.Select().RowDeleted -= PremisesListViewport_RowDeleted;
            resettle_premises.Select().RowChanged -= ResettlePremisesViewport_RowChanged;
            resettle_premises.Select().RowDeleting -= ResettlePremisesViewport_RowDeleting;
            base.ForceClose();
        }

        public override bool HasAssocOwnerships()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocSubPremises()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (v_premises.Position > -1);
        }

        public override void ShowOwnerships()
        {
            ShowAssocViewport(ViewportType.OwnershipListViewport);
        }

        public override void ShowRestrictions()
        {
            ShowAssocViewport(ViewportType.RestrictionListViewport);
        }

        public override void ShowSubPremises()
        {
            ShowAssocViewport(ViewportType.SubPremisesViewport);
        }

        public override void ShowFundHistory()
        {
            ShowAssocViewport(ViewportType.FundsHistoryViewport);
        }

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_premises = " + Convert.ToInt32(((DataRowView)v_premises[v_premises.Position])["id_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)v_premises[v_premises.Position]).Row,
                ParentTypeEnum.Premises);
        }

        void v_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_premises.Position == -1 || dataGridView.RowCount == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            if (v_premises.Position >= dataGridView.RowCount)
            {
                dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridView.Rows[v_premises.Position].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[v_premises.Position].Cells[0];
            }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
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
            dataGridView.RowCount = v_premises.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void PremisesListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            dataGridView.RowCount = v_premises.Count;
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
                if (id_expanded != Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture))
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Resource.minus;
                    id_expanded = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
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
                v_premises.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
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
                v_premises.Position = dataGridView.SelectedRows[0].Index;
            else
                v_premises.Position = -1;
            id_expanded = -1;
            dataGridView.CollapseDetails();
            dataGridView.Refresh();
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_premises.Count <= e.RowIndex || v_premises.Count == 0) return;
            var id_premises = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
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
            if (v_premises.Count <= e.RowIndex || v_premises.Count == 0) return;
            var id_premises = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_resettle_premises.Find("id_premises", id_premises);
            var row = ((DataRowView)v_premises[e.RowIndex]);
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

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle12 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(ResettlePremisesViewport));
            var dataGridViewCellStyle3 = new DataGridViewCellStyle();
            var dataGridViewCellStyle4 = new DataGridViewCellStyle();
            var dataGridViewCellStyle5 = new DataGridViewCellStyle();
            var dataGridViewCellStyle6 = new DataGridViewCellStyle();
            var dataGridViewCellStyle7 = new DataGridViewCellStyle();
            var dataGridViewCellStyle8 = new DataGridViewCellStyle();
            var dataGridViewCellStyle9 = new DataGridViewCellStyle();
            var dataGridViewCellStyle10 = new DataGridViewCellStyle();
            var dataGridViewCellStyle11 = new DataGridViewCellStyle();
            dataGridView = new DataGridViewWithDetails();
            image = new DataGridViewImageColumn();
            is_checked = new DataGridViewCheckBoxColumn();
            id_premises = new DataGridViewTextBoxColumn();
            id_street = new DataGridViewTextBoxColumn();
            house = new DataGridViewTextBoxColumn();
            premises_num = new DataGridViewTextBoxColumn();
            id_premises_type = new DataGridViewComboBoxColumn();
            total_area = new DataGridViewTextBoxColumn();
            living_area = new DataGridViewTextBoxColumn();
            cadastral_num = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(image, is_checked, id_premises, id_street, house, premises_num, id_premises_type, total_area, living_area, cadastral_num);
            dataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = Color.FromArgb(224, 224, 224);
            dataGridViewCellStyle12.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle12.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = DataGridViewTriState.False;
            dataGridView.DefaultCellStyle = dataGridViewCellStyle12;
            dataGridView.DetailsBackColor = SystemColors.Window;
            dataGridView.DetailsCollapsedImage = null;
            dataGridView.DetailsExpandedImage = null;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(1269, 298);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.BeforeExpandDetails += dataGridView_BeforeExpandDetails;
            dataGridView.BeforeCollapseDetails += dataGridView_BeforeCollapseDetails;
            dataGridView.CellContentClick += dataGridView_CellContentClick;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.CellValuePushed += dataGridView_CellValuePushed;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.CurrentCellDirtyStateChanged += dataGridView_CurrentCellDirtyStateChanged;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            dataGridView.Resize += dataGridView_Resize;
            // 
            // image
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle2.BackColor = Color.LightGray;
            dataGridViewCellStyle2.NullValue = resources.GetObject("dataGridViewCellStyle2.NullValue");
            image.DefaultCellStyle = dataGridViewCellStyle2;
            image.HeaderText = "";
            image.MinimumWidth = 23;
            image.Name = "image";
            image.ReadOnly = true;
            image.Resizable = DataGridViewTriState.False;
            image.Width = 23;
            // 
            // is_checked
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.NullValue = false;
            dataGridViewCellStyle3.Padding = new Padding(0, 2, 0, 0);
            is_checked.DefaultCellStyle = dataGridViewCellStyle3;
            is_checked.HeaderText = "";
            is_checked.MinimumWidth = 30;
            is_checked.Name = "is_checked";
            is_checked.Resizable = DataGridViewTriState.False;
            is_checked.Width = 30;
            // 
            // id_premises
            // 
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.TopLeft;
            id_premises.DefaultCellStyle = dataGridViewCellStyle4;
            id_premises.HeaderText = "№";
            id_premises.MinimumWidth = 100;
            id_premises.Name = "id_premises";
            id_premises.ReadOnly = true;
            // 
            // id_street
            // 
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.TopLeft;
            id_street.DefaultCellStyle = dataGridViewCellStyle5;
            id_street.HeaderText = "Адрес";
            id_street.MinimumWidth = 300;
            id_street.Name = "id_street";
            id_street.ReadOnly = true;
            id_street.SortMode = DataGridViewColumnSortMode.NotSortable;
            id_street.Width = 300;
            // 
            // house
            // 
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.TopLeft;
            house.DefaultCellStyle = dataGridViewCellStyle6;
            house.HeaderText = "Дом";
            house.MinimumWidth = 100;
            house.Name = "house";
            house.ReadOnly = true;
            house.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // premises_num
            // 
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.TopLeft;
            premises_num.DefaultCellStyle = dataGridViewCellStyle7;
            premises_num.HeaderText = "Помещение";
            premises_num.MinimumWidth = 100;
            premises_num.Name = "premises_num";
            premises_num.ReadOnly = true;
            // 
            // id_premises_type
            // 
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.TopLeft;
            id_premises_type.DefaultCellStyle = dataGridViewCellStyle8;
            id_premises_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_premises_type.HeaderText = "Тип помещения";
            id_premises_type.MinimumWidth = 150;
            id_premises_type.Name = "id_premises_type";
            id_premises_type.ReadOnly = true;
            id_premises_type.Width = 150;
            // 
            // total_area
            // 
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle9.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle9;
            total_area.HeaderText = "Общая площадь";
            total_area.MinimumWidth = 130;
            total_area.Name = "total_area";
            total_area.ReadOnly = true;
            total_area.Width = 130;
            // 
            // living_area
            // 
            dataGridViewCellStyle10.Alignment = DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle10.Format = "#0.0## м²";
            living_area.DefaultCellStyle = dataGridViewCellStyle10;
            living_area.HeaderText = "Жилая площадь";
            living_area.MinimumWidth = 130;
            living_area.Name = "living_area";
            living_area.ReadOnly = true;
            living_area.Width = 130;
            // 
            // cadastral_num
            // 
            dataGridViewCellStyle11.Alignment = DataGridViewContentAlignment.TopLeft;
            cadastral_num.DefaultCellStyle = dataGridViewCellStyle11;
            cadastral_num.HeaderText = "Кадастровый номер";
            cadastral_num.MinimumWidth = 170;
            cadastral_num.Name = "cadastral_num";
            cadastral_num.ReadOnly = true;
            cadastral_num.Width = 170;
            // 
            // ResettlePremisesViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(1275, 304);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ResettlePremisesViewport";
            Padding = new Padding(3);
            Text = "Перечень помещений";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
