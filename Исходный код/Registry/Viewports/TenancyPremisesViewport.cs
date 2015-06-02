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
    internal sealed class TenancyPremisesViewport: Viewport
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
        private TenancyPremisesAssocDataModel tenancy_premises = null;
        private DataTable snapshot_tenancy_premises = null;
        private ObjectStatesDataModel object_states = null;
        private CalcDataModelPremisesCurrentFunds premises_funds = null;
        private FundTypesDataModel fund_types = null;
        #endregion Models

        #region Views
        private BindingSource v_premises = null;
        private BindingSource v_buildings = null;
        private BindingSource v_premises_types = null;
        private BindingSource v_sub_premises = null;
        private BindingSource v_tenancy_premises = null;
        private BindingSource v_snapshot_tenancy_premises = null;
        #endregion Views

        //Forms
        private SearchForm spExtendedSearchForm = null;
        private SearchForm spSimpleSearchForm = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;
        private DataGridViewImageColumn image;
        private DataGridViewCheckBoxColumn is_checked;
        private DataGridViewTextBoxColumn rent_total_area;
        private DataGridViewTextBoxColumn rent_living_area;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn premises_num;
        private DataGridViewComboBoxColumn id_premises_type;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn id_state;
        private DataGridViewTextBoxColumn current_fund;

        //Идентификатор развернутого помещения
        private int id_expanded = -1;

        private TenancyPremisesViewport()
            : this(null)
        {
        }

        public TenancyPremisesViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
        }

        public TenancyPremisesViewport(TenancyPremisesViewport tenancyPremisesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = tenancyPremisesViewport.DynamicFilter;
            this.StaticFilter = tenancyPremisesViewport.StaticFilter;
            this.ParentRow = tenancyPremisesViewport.ParentRow;
            this.ParentType = tenancyPremisesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            //Проверяем помещения
            List<TenancyObject> list_from_view = TenancyPremisesFromView();
            List<TenancyObject> list_from_viewport = TenancyPremisesFromViewport();
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
            list_from_view = ((TenancySubPremisesDetails)dataGridView.DetailsControl).TenancySubPremisesFromView();
            list_from_viewport = ((TenancySubPremisesDetails)dataGridView.DetailsControl).TenancySubPremisesFromViewport();
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
                true, 
                dataRowView["rent_total_area"],
                dataRowView["rent_living_area"]
            };
        }

        public void LocatePremisesBy(int id)
        {
            int Position = v_premises.Find("id_premises", id);
            if (Position > 0)
                v_premises.Position = Position;
        }

        private static TenancyObject RowToTenancyPremises(DataRow row)
        {
            TenancyObject to = new TenancyObject();
            to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
            to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises");
            to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
            to.RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area");
            return to;
        }

        private List<TenancyObject> TenancyPremisesFromViewport()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < snapshot_tenancy_premises.Rows.Count; i++)
            {
                DataRow row = snapshot_tenancy_premises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                TenancyObject to = new TenancyObject();
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises");
                to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
                to.RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area");
                list.Add(to);
            }
            return list;
        }

        private List<TenancyObject> TenancyPremisesFromView()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < v_tenancy_premises.Count; i++)
            {
                TenancyObject to = new TenancyObject();
                DataRowView row = ((DataRowView)v_tenancy_premises[i]);
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises");
                to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
                to.RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area");
                list.Add(to);
            }
            return list;
        }

        private bool ValidateTenancyPremises(List<TenancyObject> tenancyPremises)
        {
            foreach (TenancyObject premises in tenancyPremises)
            {
                if (!ViewportHelper.PremiseFundAndRentMatch(premises.IdObject.Value, (int)ParentRow["id_rent_type"]))
                {
                    int idBuilding = (int)PremisesDataModel.GetInstance().Select().Rows.Find(premises.IdObject.Value)["id_building"];
                    if (!ViewportHelper.BuildingFundAndRentMatch(idBuilding, (int)ParentRow["id_rent_type"]) &&
                                MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемого помещения. Все равно продолжить сохранение?",
                                "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != 
                                System.Windows.Forms.DialogResult.Yes)
                        return false;
                    else
                        return true;
                }
            }
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
            tenancy_premises = TenancyPremisesAssocDataModel.GetInstance();
            object_states = ObjectStatesDataModel.GetInstance();
            premises_funds = CalcDataModelPremisesCurrentFunds.GetInstance();
            fund_types = FundTypesDataModel.GetInstance();
            object_states.Select();
            premises_funds.Select();
            fund_types.Select();

            // Ожидаем дозагрузки данных, если это необходимо
            premises.Select();
            kladr.Select();
            buildings.Select();
            premises_types.Select();
            sub_premises.Select();
            tenancy_premises.Select();

            // Инициализируем snapshot-модель
            snapshot_tenancy_premises = new DataTable("selected_premises");
            snapshot_tenancy_premises.Locale = CultureInfo.InvariantCulture;
            snapshot_tenancy_premises.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_tenancy_premises.Columns.Add("id_premises").DataType = typeof(int);
            snapshot_tenancy_premises.Columns.Add("is_checked").DataType = typeof(bool);
            snapshot_tenancy_premises.Columns.Add("rent_total_area").DataType = typeof(double);
            snapshot_tenancy_premises.Columns.Add("rent_living_area").DataType = typeof(double);

            DataSet ds = DataSetManager.DataSet;

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += new EventHandler(v_premises_CurrentItemChanged);
            v_premises.DataMember = "premises";
            v_premises.DataSource = ds;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = "Помещения найма №" + ParentRow["id_process"].ToString();
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_tenancy_premises = new BindingSource();
            v_tenancy_premises.DataMember = "tenancy_premises_assoc";
            v_tenancy_premises.Filter = StaticFilter;
            v_tenancy_premises.DataSource = ds;

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
            for (int i = 0; i < v_tenancy_premises.Count; i++)
                snapshot_tenancy_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_premises[i])));
            v_snapshot_tenancy_premises = new BindingSource();
            v_snapshot_tenancy_premises.DataSource = snapshot_tenancy_premises;

            id_premises_type.DataSource = v_premises_types;
            id_premises_type.ValueMember = "id_premises_type";
            id_premises_type.DisplayMember = "premises_type";
            // Настраивем компонент отображения комнат
            TenancySubPremisesDetails details = new TenancySubPremisesDetails();
            details.v_sub_premises = v_sub_premises;
            details.sub_premises = sub_premises.Select();
            details.StaticFilter = StaticFilter;
            details.ParentRow = ParentRow;
            details.ParentType = ParentType;
            details.menuCallback = MenuCallback;
            details.InitializeControl();
            dataGridView.DetailsControl = details;

            //Строим фильтр арендуемых квартир и комнат во время первой загрузки
            if (String.IsNullOrEmpty(DynamicFilter))
            {
                if (v_tenancy_premises.Count > 0)
                {
                    DynamicFilter = "id_premises IN (0";
                    for (int i = 0; i < v_tenancy_premises.Count; i++)
                        DynamicFilter += "," + ((DataRowView)v_tenancy_premises[i])["id_premises"].ToString();
                    DynamicFilter += ")";
                }
                if (details.v_snapshot_tenancy_sub_premises.Count > 0)
                {
                    if (!String.IsNullOrEmpty(DynamicFilter))
                        DynamicFilter += " OR ";
                    DynamicFilter += "id_premises IN (0";
                    for (int i = 0; i < details.v_snapshot_tenancy_sub_premises.Count; i++)
                    {
                        DataRowView row = ((DataRowView)details.v_snapshot_tenancy_sub_premises[i]);
                        DataRow subPremisesRow = sub_premises.Select().Rows.Find(row["id_sub_premises"]);
                        if (subPremisesRow != null)
                            DynamicFilter += "," + subPremisesRow["id_premises"].ToString();
                    }
                    DynamicFilter += ")";
                }
            }

            v_premises.Filter += DynamicFilter;

            premises.Select().RowChanged += new DataRowChangeEventHandler(PremisesListViewport_RowChanged);
            premises.Select().RowDeleted += new DataRowChangeEventHandler(PremisesListViewport_RowDeleted);
            tenancy_premises.Select().RowChanged += new DataRowChangeEventHandler(TenancyPremisesViewport_RowChanged);
            tenancy_premises.Select().RowDeleting += new DataRowChangeEventHandler(TenancyPremisesViewport_RowDeleting);
            premises_funds.RefreshEvent += premises_funds_RefreshEvent;

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
                if (ParentType == ParentTypeEnum.Tenancy)
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
            snapshot_tenancy_premises.Clear();
            for (int i = 0; i < v_tenancy_premises.Count; i++)
                snapshot_tenancy_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_premises[i])));
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
            sync_views = false;
            tenancy_premises.EditingNewRecord = true;
            List<TenancyObject> list = TenancyPremisesFromViewport();
            // Проверяем данные о помещениях
            if (!ValidateTenancyPremises(list))
            {
                sync_views = true;
                tenancy_premises.EditingNewRecord = false;
                return;
            }
            // Проверяем данные о комнатах
            if (!((TenancySubPremisesDetails)dataGridView.DetailsControl).ValidateTenancySubPremises(
                ((TenancySubPremisesDetails)dataGridView.DetailsControl).TenancySubPremisesFromViewport()))
            {
                sync_views = true;
                tenancy_premises.EditingNewRecord = false;
                return;
            }
            // Сохраняем помещения в базу данных
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (((TenancyObject)list[i]).IdAssoc != null)
                    row = tenancy_premises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    int id_assoc = TenancyPremisesAssocDataModel.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        tenancy_premises.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_premises[
                        v_snapshot_tenancy_premises.Find("id_premises", list[i].IdObject)])["id_assoc"] = id_assoc;
                    tenancy_premises.Select().Rows.Add(new object[] { 
                        id_assoc, list[i].IdObject, list[i].IdProcess, list[i].RentTotalArea, list[i].RentLivingArea, 0
                    });
                }
                else
                {
                    if (RowToTenancyPremises(row) == list[i])
                        continue;
                    if (TenancyPremisesAssocDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        tenancy_premises.EditingNewRecord = false;
                        return;
                    }
                    row["rent_total_area"] = list[i].RentTotalArea == null ? DBNull.Value : (object)list[i].RentTotalArea;
                    row["rent_living_area"] = list[i].RentLivingArea == null ? DBNull.Value : (object)list[i].RentLivingArea;
                }
            }
            list = TenancyPremisesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < v_snapshot_tenancy_premises.Count; j++)
                {
                    DataRowView row = (DataRowView)v_snapshot_tenancy_premises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !String.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    if (TenancyPremisesAssocDataModel.Delete(list[i].IdAssoc.Value) == -1)
                    {
                        sync_views = true;
                        tenancy_premises.EditingNewRecord = false;
                        return;
                    }
                    int snapshot_row_index = -1;
                    for (int j = 0; j < v_snapshot_tenancy_premises.Count; j++)
                        if (((DataRowView)v_snapshot_tenancy_premises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_tenancy_premises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        int premises_row_index = v_premises.Find("id_premises", list[i].IdObject);
                        ((DataRowView)v_snapshot_tenancy_premises[snapshot_row_index]).Delete();
                        if (premises_row_index != -1)
                            dataGridView.InvalidateRow(premises_row_index);
                    }
                    tenancy_premises.Select().Rows.Find(((TenancyObject)list[i]).IdAssoc).Delete();
                }
            }
            sync_views = true;
            tenancy_premises.EditingNewRecord = false;
            // Сохраняем комнаты в базу данных
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).SaveRecord();
            MenuCallback.EditingStateUpdate();
            // Обновляем зависимую агрегационную модель
            if (ParentType == ParentTypeEnum.Tenancy)
                CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.TenancyProcess, (int)ParentRow["id_process"], true);
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
            TenancyPremisesViewport viewport = new TenancyPremisesViewport(this, MenuCallback);
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
            tenancy_premises.Select().RowChanged -= new DataRowChangeEventHandler(TenancyPremisesViewport_RowChanged);
            tenancy_premises.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyPremisesViewport_RowDeleting);
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            premises.Select().RowChanged -= new DataRowChangeEventHandler(PremisesListViewport_RowChanged);
            premises.Select().RowDeleted -= new DataRowChangeEventHandler(PremisesListViewport_RowDeleted);
            tenancy_premises.Select().RowChanged -= new DataRowChangeEventHandler(TenancyPremisesViewport_RowChanged);
            tenancy_premises.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyPremisesViewport_RowDeleting);
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

        void premises_funds_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
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

        void TenancyPremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_tenancy_premises.Find("id_premises", e.Row["id_premises"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_tenancy_premises[row_index]).Delete();
            }
            dataGridView.Refresh();
        }

        void TenancyPremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            int row_index = v_snapshot_tenancy_premises.Find("id_premises", e.Row["id_premises"]);
            if (row_index == -1 && v_tenancy_premises.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_tenancy_premises.Rows.Add(new object[] { 
                        e.Row["id_assoc"],
                        e.Row["id_premises"], 
                        true,   
                        e.Row["rent_total_area"],
                        e.Row["rent_living_area"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_tenancy_premises[row_index]);
                    row["rent_total_area"] = e.Row["rent_total_area"];
                    row["rent_living_area"] = e.Row["rent_living_area"];
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
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).CalcControlHeight();
            int width = 0;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
        }

        void dataGridView_Resize(object sender, EventArgs e)
        {
            int width = 0;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
            if (dataGridView.Size.Width > 1540)
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
            dataGridView.Refresh();
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_premises.Count <= e.RowIndex || v_premises.Count == 0) return;
            int id_premises = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            int row_index = v_snapshot_tenancy_premises.Find("id_premises", id_premises);
            double value = 0;
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_tenancy_premises.Rows.Add(new object[] { null, id_premises, e.Value, null });
                    else
                        ((DataRowView)v_snapshot_tenancy_premises[row_index])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                    value = 0;
                    if (e.Value != null)
                        Double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_premises.Rows.Add(new object[] { null, id_premises, false, value == 0 ? DBNull.Value : (object)value, DBNull.Value });
                    else
                        ((DataRowView)v_snapshot_tenancy_premises[row_index])["rent_total_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
                case "rent_living_area":
                    value = 0;
                    if (e.Value != null)
                        Double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_premises.Rows.Add(new object[] { null, id_premises, false, DBNull.Value, value == 0 ? DBNull.Value : (object)value });
                    else
                        ((DataRowView)v_snapshot_tenancy_premises[row_index])["rent_living_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_premises.Count <= e.RowIndex || v_premises.Count == 0) return;
            int id_premises = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            int row_index = v_snapshot_tenancy_premises.Find("id_premises", id_premises);
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
                        e.Value = ((DataRowView)v_snapshot_tenancy_premises[row_index])["is_checked"];
                    break;
                case "rent_total_area":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_premises[row_index])["rent_total_area"];
                    break;
                case "rent_living_area":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_premises[row_index])["rent_living_area"];
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
                case "total_area":
                    e.Value = row["total_area"];
                    break;
                case "id_state":
                    DataRow state_row = object_states.Select().Rows.Find(row["id_state"]);
                    if (state_row != null)
                        e.Value = state_row["state_female"];
                    break;
                case "current_fund":
                    if ((new object[] { 1, 4, 5 }).Contains(row["id_state"]))
                    {
                        DataRow fund_row = premises_funds.Select().Rows.Find(row["id_premises"]);
                        if (fund_row != null)
                            e.Value = fund_types.Select().Rows.Find(fund_row["id_fund_type"])["fund_type"];
                    }
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
        void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyPremisesViewport));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new Microsoft.TeamFoundation.Client.DataGridViewWithDetails();
            this.image = new System.Windows.Forms.DataGridViewImageColumn();
            this.is_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.rent_total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rent_living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_street = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.house = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_premises_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.current_fund = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.rent_total_area,
            this.rent_living_area,
            this.id_premises,
            this.id_street,
            this.house,
            this.premises_num,
            this.id_premises_type,
            this.total_area,
            this.id_state,
            this.current_fund});
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle14;
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
            this.dataGridView.Size = new System.Drawing.Size(1364, 298);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.BeforeExpandDetails += new System.EventHandler<Microsoft.TeamFoundation.Client.DataGridViewDetailsEventArgs>(this.dataGridView_BeforeExpandDetails);
            this.dataGridView.BeforeCollapseDetails += new System.EventHandler<Microsoft.TeamFoundation.Client.DataGridViewDetailsEventArgs>(this.dataGridView_BeforeCollapseDetails);
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValuePushed);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView_CurrentCellDirtyStateChanged);
            this.dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView_EditingControlShowing);
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
            // rent_total_area
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Format = "#0.0## м²";
            this.rent_total_area.DefaultCellStyle = dataGridViewCellStyle4;
            this.rent_total_area.HeaderText = "Площадь койко-места";
            this.rent_total_area.MinimumWidth = 160;
            this.rent_total_area.Name = "rent_total_area";
            this.rent_total_area.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rent_total_area.Width = 160;
            // 
            // rent_living_area
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Format = "#0.0## м²";
            this.rent_living_area.DefaultCellStyle = dataGridViewCellStyle5;
            this.rent_living_area.HeaderText = "Арендуемая S жил.";
            this.rent_living_area.MinimumWidth = 130;
            this.rent_living_area.Name = "rent_living_area";
            this.rent_living_area.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rent_living_area.Visible = false;
            this.rent_living_area.Width = 130;
            // 
            // id_premises
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.id_premises.DefaultCellStyle = dataGridViewCellStyle6;
            this.id_premises.HeaderText = "№";
            this.id_premises.MinimumWidth = 100;
            this.id_premises.Name = "id_premises";
            this.id_premises.ReadOnly = true;
            // 
            // id_street
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.id_street.DefaultCellStyle = dataGridViewCellStyle7;
            this.id_street.HeaderText = "Адрес";
            this.id_street.MinimumWidth = 300;
            this.id_street.Name = "id_street";
            this.id_street.ReadOnly = true;
            this.id_street.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.id_street.Width = 300;
            // 
            // house
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.house.DefaultCellStyle = dataGridViewCellStyle8;
            this.house.HeaderText = "Дом";
            this.house.MinimumWidth = 100;
            this.house.Name = "house";
            this.house.ReadOnly = true;
            this.house.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // premises_num
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.premises_num.DefaultCellStyle = dataGridViewCellStyle9;
            this.premises_num.HeaderText = "Помещение";
            this.premises_num.MinimumWidth = 100;
            this.premises_num.Name = "premises_num";
            this.premises_num.ReadOnly = true;
            // 
            // id_premises_type
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.id_premises_type.DefaultCellStyle = dataGridViewCellStyle10;
            this.id_premises_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_premises_type.HeaderText = "Тип помещения";
            this.id_premises_type.MinimumWidth = 150;
            this.id_premises_type.Name = "id_premises_type";
            this.id_premises_type.ReadOnly = true;
            this.id_premises_type.Width = 150;
            // 
            // total_area
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle11.Format = "#0.0## м²";
            this.total_area.DefaultCellStyle = dataGridViewCellStyle11;
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.MinimumWidth = 130;
            this.total_area.Name = "total_area";
            this.total_area.ReadOnly = true;
            this.total_area.Width = 130;
            // 
            // id_state
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.id_state.DefaultCellStyle = dataGridViewCellStyle12;
            this.id_state.HeaderText = "Текущее состояние";
            this.id_state.MinimumWidth = 170;
            this.id_state.Name = "id_state";
            this.id_state.ReadOnly = true;
            this.id_state.Width = 170;
            // 
            // current_fund
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.current_fund.DefaultCellStyle = dataGridViewCellStyle13;
            this.current_fund.HeaderText = "Текущий фонд";
            this.current_fund.MinimumWidth = 170;
            this.current_fund.Name = "current_fund";
            this.current_fund.ReadOnly = true;
            this.current_fund.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.current_fund.Width = 170;
            // 
            // TenancyPremisesViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1370, 304);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyPremisesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень помещений";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
