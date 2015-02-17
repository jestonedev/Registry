using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.SearchForms;
using System.Data;
using Registry.Entities;
using Microsoft.TeamFoundation.Client;
using System.Text.RegularExpressions;
using Registry.CalcDataModels;
using Security;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class ResettlePremisesViewport: Viewport
    {
        #region Components
        private DataGridViewWithDetails dataGridView;
        #endregion Components

        #region Models
        private PremisesDataModel premises = null;
        private BuildingsDataModel buildings = null;
        private KladrStreetsDataModel kladr = null;
        private PremisesTypesDataModel premises_types = null;
        private SubPremisesDataModel sub_premises = null;
        private DataModel resettle_premises = null;
        private DataTable snapshot_resettle_premises = null;
        #endregion Models

        #region Views
        private BindingSource v_premises = null;
        private BindingSource v_buildings = null;
        private BindingSource v_premises_types = null;
        private BindingSource v_sub_premises = null;
        private BindingSource v_resettle_premises = null;
        private BindingSource v_snapshot_resettle_premises = null;
        #endregion Views

        //Forms
        private SearchForm spExtendedSearchForm = null;
        private SearchForm spSimpleSearchForm = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;
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
            this.DynamicFilter = resettlePremisesViewport.DynamicFilter;
            this.StaticFilter = resettlePremisesViewport.StaticFilter;
            this.ParentRow = resettlePremisesViewport.ParentRow;
            this.ParentType = resettlePremisesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            //Проверяем помещения
            List<ResettleObject> list_from_view = ResettlePremisesFromView();
            List<ResettleObject> list_from_viewport = ResettlePremisesFromViewport();
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
            //Проверяем комнаты
            list_from_view = ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromView();
            list_from_viewport = ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            founded = false;
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
                dataRowView["id_premises"], 
                true
            };
        }

        public void LocatePremisesBy(int id)
        {
            int Position = v_premises.Find("id_premises", id);
            if (Position > 0)
                v_premises.Position = Position;
        }

        private static ResettleObject RowToResettlePremises(DataRow row)
        {
            ResettleObject to = new ResettleObject();
            to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
            to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises");
            return to;
        }

        private List<ResettleObject> ResettlePremisesFromViewport()
        {
            List<ResettleObject> list = new List<ResettleObject>();
            for (int i = 0; i < snapshot_resettle_premises.Rows.Count; i++)
            {
                DataRow row = snapshot_resettle_premises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                ResettleObject to = new ResettleObject();
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises");
                list.Add(to);
            }
            return list;
        }

        private List<ResettleObject> ResettlePremisesFromView()
        {
            List<ResettleObject> list = new List<ResettleObject>();
            for (int i = 0; i < v_resettle_premises.Count; i++)
            {
                ResettleObject to = new ResettleObject();
                DataRowView row = ((DataRowView)v_resettle_premises[i]);
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            premises = PremisesDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();
            buildings = BuildingsDataModel.GetInstance();
            premises_types = PremisesTypesDataModel.GetInstance();
            sub_premises = SubPremisesDataModel.GetInstance();

            if (way == ResettleEstateObjectWay.From)
                resettle_premises = ResettlePremisesFromAssocDataModel.GetInstance();
            else
                resettle_premises = ResettlePremisesToAssocDataModel.GetInstance();

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

            DataSet ds = DataSetManager.DataSet;

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += new EventHandler(v_premises_CurrentItemChanged);
            v_premises.DataMember = "premises";
            v_premises.DataSource = ds;
            v_premises.Filter += DynamicFilter;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.ResettleProcess))
            {
                if (way == ResettleEstateObjectWay.From)
                    Text = "Помещения (из) переселения №" + ParentRow["id_process"].ToString();
                else
                    Text = "Помещения (в) переселения №" + ParentRow["id_process"].ToString();
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
            for (int i = 0; i < v_resettle_premises.Count; i++)
                snapshot_resettle_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_premises[i])));
            v_snapshot_resettle_premises = new BindingSource();
            v_snapshot_resettle_premises.DataSource = snapshot_resettle_premises;

            id_premises_type.DataSource = v_premises_types;
            id_premises_type.ValueMember = "id_premises_type";
            id_premises_type.DisplayMember = "premises_type";

            // Настраивем компонент отображения комнат
            ResettleSubPremisesDetails details = new ResettleSubPremisesDetails();
            details.v_sub_premises = v_sub_premises;
            details.sub_premises = sub_premises.Select();
            details.StaticFilter = StaticFilter;
            details.ParentRow = ParentRow;
            details.ParentType = ParentType;
            details.menuCallback = MenuCallback;
            details.Way = way;
            details.InitializeControl();
            dataGridView.DetailsControl = details;

            premises.Select().RowChanged += new DataRowChangeEventHandler(PremisesListViewport_RowChanged);
            premises.Select().RowDeleted += new DataRowChangeEventHandler(PremisesListViewport_RowDeleted);
            resettle_premises.Select().RowChanged += new DataRowChangeEventHandler(ResettlePremisesViewport_RowChanged);
            resettle_premises.Select().RowDeleting += new DataRowChangeEventHandler(ResettlePremisesViewport_RowDeleting);
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
            if (MessageBox.Show("Вы действительно хотите удалить это помещение?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)v_premises.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)v_premises.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                int id_building = (int)((DataRowView)v_premises[v_premises.Position])["id_building"];
                if (PremisesDataModel.Delete((int)((DataRowView)v_premises.Current)["id_premises"]) == -1)
                    return;
                ((DataRowView)v_premises[v_premises.Position]).Delete();
                MenuCallback.ForceCloseDetachedViewports();
                if (ParentType == ParentTypeEnum.ResettleProcess)
                {
                    CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(EntityType.Building, id_building, true);
                    CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building, id_building, true);
                    CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
                    CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
                }
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
            PremisesViewport viewport = new PremisesViewport(MenuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_resettle_premises.Clear();
            for (int i = 0; i < v_resettle_premises.Count; i++)
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
            List<ResettleObject> list = ResettlePremisesFromViewport();
            // Проверяем данные о помещениях
            if (!ValidateResettlePremises(list))
            {
                sync_views = true;
                return;
            }
            // Проверяем данные о комнатах
            if (!ResettleSubPremisesDetails.ValidateResettleSubPremises(
                ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromViewport()))
            {
                sync_views = true;
                return;
            }
            // Сохраняем помещения в базу данных
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (((ResettleObject)list[i]).IdAssoc != null)
                    row = resettle_premises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {

                    int id_assoc = -1;
                    if (way == ResettleEstateObjectWay.From) 
                        id_assoc = ResettlePremisesFromAssocDataModel.Insert(list[i]);
                    else
                        id_assoc = ResettlePremisesToAssocDataModel.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_resettle_premises[
                        v_snapshot_resettle_premises.Find("id_premises", list[i].IdObject)])["id_assoc"] = id_assoc;
                    resettle_premises.Select().Rows.Add(new object[] { 
                        id_assoc, list[i].IdObject, list[i].IdProcess, 0
                    });
                }
            }
            list = ResettlePremisesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < v_snapshot_resettle_premises.Count; j++)
                {
                    DataRowView row = (DataRowView)v_snapshot_resettle_premises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !String.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    int affected = -1;
                    if (way == ResettleEstateObjectWay.From)
                        affected = ResettlePremisesFromAssocDataModel.Delete(list[i].IdAssoc.Value);
                    else
                        affected = ResettlePremisesToAssocDataModel.Delete(list[i].IdAssoc.Value);
                    if (affected == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    int snapshot_row_index = -1;
                    for (int j = 0; j < v_snapshot_resettle_premises.Count; j++)
                        if (((DataRowView)v_snapshot_resettle_premises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_resettle_premises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        int premises_row_index = v_premises.Find("id_premises", list[i].IdObject);
                        ((DataRowView)v_snapshot_resettle_premises[snapshot_row_index]).Delete();
                        if (premises_row_index != -1)
                            dataGridView.InvalidateRow(premises_row_index);
                    }
                    resettle_premises.Select().Rows.Find(((ResettleObject)list[i]).IdAssoc).Delete();
                }
            }
            sync_views = true;
            // Сохраняем комнаты в базу данных
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).SaveRecord();
            MenuCallback.EditingStateUpdate();
            // Обновляем зависимую агрегационную модель
            if (ParentType == ParentTypeEnum.ResettleProcess)
                CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.ResettleProcess, (int)ParentRow["id_process"], true);
        }

        public override bool CanInsertRecord()
        {
            return (!premises.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            PremisesViewport viewport = new PremisesViewport(MenuCallback);
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
            PremisesViewport viewport = new PremisesViewport(MenuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            ResettlePremisesViewport viewport = new ResettlePremisesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            return viewport;
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
            premises.Select().RowChanged -= new DataRowChangeEventHandler(PremisesListViewport_RowChanged);
            premises.Select().RowDeleted -= new DataRowChangeEventHandler(PremisesListViewport_RowDeleted);
            resettle_premises.Select().RowChanged -= new DataRowChangeEventHandler(ResettlePremisesViewport_RowChanged);
            resettle_premises.Select().RowDeleting -= new DataRowChangeEventHandler(ResettlePremisesViewport_RowDeleting);
        }

        public override void ForceClose()
        {
            premises.Select().RowChanged -= new DataRowChangeEventHandler(PremisesListViewport_RowChanged);
            premises.Select().RowDeleted -= new DataRowChangeEventHandler(PremisesListViewport_RowDeleted);
            resettle_premises.Select().RowChanged -= new DataRowChangeEventHandler(ResettlePremisesViewport_RowChanged);
            resettle_premises.Select().RowDeleting -= new DataRowChangeEventHandler(ResettlePremisesViewport_RowDeleting);
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
                int row_index = v_snapshot_resettle_premises.Find("id_premises", e.Row["id_premises"]);
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
            int row_index = v_snapshot_resettle_premises.Find("id_premises", e.Row["id_premises"]);
            if (row_index == -1 && v_resettle_premises.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_resettle_premises.Rows.Add(new object[] { 
                        e.Row["id_assoc"],
                        e.Row["id_premises"], 
                        true
                    });
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
            int width = 0;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((ResettleSubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
        }

        void dataGridView_Resize(object sender, EventArgs e)
        {
            int width = 0;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
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
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Registry.Viewport.Properties.Resource.minus;
                    id_expanded = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
                    dataGridView.ExpandDetails(e.RowIndex);
                }
                else
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Registry.Viewport.Properties.Resource.plus;
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
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_premises.Count <= e.RowIndex || v_premises.Count == 0) return;
            int id_premises = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            int row_index = v_snapshot_resettle_premises.Find("id_premises", id_premises);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_resettle_premises.Rows.Add(new object[] { null, id_premises, e.Value });
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
            int id_premises = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            int row_index = v_snapshot_resettle_premises.Find("id_premises", id_premises);
            DataRowView row = ((DataRowView)v_premises[e.RowIndex]);
            DataRow building_row = buildings.Select().Rows.Find(row["id_building"]);
            if (building_row == null)
                return;
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "image":
                    if (id_expanded == id_premises)
                        e.Value = Registry.Viewport.Properties.Resource.minus;
                    else
                        e.Value = Registry.Viewport.Properties.Resource.plus;
                    break;
                case "is_checked":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_resettle_premises[row_index])["is_checked"];
                    break;
                case "id_premises":
                    e.Value = row["id_premises"];
                    break;
                case "id_street":
                    DataRow kladr_row = kladr.Select().Rows.Find(building_row["id_street"]);
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

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResettlePremisesViewport));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new Microsoft.TeamFoundation.Client.DataGridViewWithDetails();
            this.image = new System.Windows.Forms.DataGridViewImageColumn();
            this.is_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_street = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.house = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_premises_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.image,
            this.is_checked,
            this.id_premises,
            this.id_street,
            this.house,
            this.premises_num,
            this.id_premises_type,
            this.total_area,
            this.living_area,
            this.cadastral_num});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridView.DetailsBackColor = System.Drawing.SystemColors.Window;
            this.dataGridView.DetailsCollapsedImage = null;
            this.dataGridView.DetailsExpandedImage = null;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(1269, 298);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.BeforeExpandDetails += new System.EventHandler<Microsoft.TeamFoundation.Client.DataGridViewDetailsEventArgs>(this.dataGridView_BeforeExpandDetails);
            this.dataGridView.BeforeCollapseDetails += new System.EventHandler<Microsoft.TeamFoundation.Client.DataGridViewDetailsEventArgs>(this.dataGridView_BeforeCollapseDetails);
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValuePushed);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // image
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle2.NullValue")));
            this.image.DefaultCellStyle = dataGridViewCellStyle2;
            this.image.HeaderText = "";
            this.image.MinimumWidth = 23;
            this.image.Name = "image";
            this.image.ReadOnly = true;
            this.image.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.image.Width = 23;
            // 
            // is_checked
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.NullValue = false;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.is_checked.DefaultCellStyle = dataGridViewCellStyle3;
            this.is_checked.HeaderText = "";
            this.is_checked.MinimumWidth = 30;
            this.is_checked.Name = "is_checked";
            this.is_checked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.is_checked.Width = 30;
            // 
            // id_premises
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.id_premises.DefaultCellStyle = dataGridViewCellStyle4;
            this.id_premises.HeaderText = "№";
            this.id_premises.MinimumWidth = 100;
            this.id_premises.Name = "id_premises";
            this.id_premises.ReadOnly = true;
            // 
            // id_street
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.id_street.DefaultCellStyle = dataGridViewCellStyle5;
            this.id_street.HeaderText = "Адрес";
            this.id_street.MinimumWidth = 300;
            this.id_street.Name = "id_street";
            this.id_street.ReadOnly = true;
            this.id_street.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.id_street.Width = 300;
            // 
            // house
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.house.DefaultCellStyle = dataGridViewCellStyle6;
            this.house.HeaderText = "Дом";
            this.house.MinimumWidth = 100;
            this.house.Name = "house";
            this.house.ReadOnly = true;
            this.house.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // premises_num
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.premises_num.DefaultCellStyle = dataGridViewCellStyle7;
            this.premises_num.HeaderText = "Помещение";
            this.premises_num.MinimumWidth = 100;
            this.premises_num.Name = "premises_num";
            this.premises_num.ReadOnly = true;
            // 
            // id_premises_type
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.id_premises_type.DefaultCellStyle = dataGridViewCellStyle8;
            this.id_premises_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_premises_type.HeaderText = "Тип помещения";
            this.id_premises_type.MinimumWidth = 150;
            this.id_premises_type.Name = "id_premises_type";
            this.id_premises_type.ReadOnly = true;
            this.id_premises_type.Width = 150;
            // 
            // total_area
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle9.Format = "#0.0## м²";
            this.total_area.DefaultCellStyle = dataGridViewCellStyle9;
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.MinimumWidth = 130;
            this.total_area.Name = "total_area";
            this.total_area.ReadOnly = true;
            this.total_area.Width = 130;
            // 
            // living_area
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle10.Format = "#0.0## м²";
            this.living_area.DefaultCellStyle = dataGridViewCellStyle10;
            this.living_area.HeaderText = "Жилая площадь";
            this.living_area.MinimumWidth = 130;
            this.living_area.Name = "living_area";
            this.living_area.ReadOnly = true;
            this.living_area.Width = 130;
            // 
            // cadastral_num
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.cadastral_num.DefaultCellStyle = dataGridViewCellStyle11;
            this.cadastral_num.HeaderText = "Кадастровый номер";
            this.cadastral_num.MinimumWidth = 170;
            this.cadastral_num.Name = "cadastral_num";
            this.cadastral_num.ReadOnly = true;
            this.cadastral_num.Width = 170;
            // 
            // ResettlePremisesViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1275, 304);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ResettlePremisesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень помещений";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
