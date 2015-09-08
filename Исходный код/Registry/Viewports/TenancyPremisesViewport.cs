using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.Entities;
using Registry.SearchForms;
using Registry.Viewport.Properties;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class TenancyPremisesViewport: Viewport
    {
        #region Components
        private DataGridViewWithDetails dataGridView;
        #endregion Components

        #region Models
        private PremisesDataModel premises;
        private BuildingsDataModel buildings;
        private KladrStreetsDataModel kladr;
        private PremisesTypesDataModel premises_types;
        private SubPremisesDataModel sub_premises;
        private TenancyPremisesAssocDataModel tenancy_premises;
        private DataTable snapshot_tenancy_premises;
        private ObjectStatesDataModel object_states;
        private CalcDataModelPremisesCurrentFunds premises_funds;
        private FundTypesDataModel fund_types;
        #endregion Models

        #region Views
        private BindingSource v_premises;
        private BindingSource v_buildings;
        private BindingSource v_premises_types;
        private BindingSource v_sub_premises;
        private BindingSource v_tenancy_premises;
        private BindingSource v_snapshot_tenancy_premises;
        #endregion Views

        //Forms
        private SearchForm spExtendedSearchForm;
        private SearchForm spSimpleSearchForm;

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
            DynamicFilter = tenancyPremisesViewport.DynamicFilter;
            StaticFilter = tenancyPremisesViewport.StaticFilter;
            ParentRow = tenancyPremisesViewport.ParentRow;
            ParentType = tenancyPremisesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            //Проверяем помещения
            var list_from_view = TenancyPremisesFromView();
            var list_from_viewport = TenancyPremisesFromViewport();
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
            list_from_view = ((TenancySubPremisesDetails)dataGridView.DetailsControl).TenancySubPremisesFromView();
            list_from_viewport = ((TenancySubPremisesDetails)dataGridView.DetailsControl).TenancySubPremisesFromViewport();
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
                true, 
                dataRowView["rent_total_area"],
                dataRowView["rent_living_area"]
            };
        }

        public void LocatePremisesBy(int id)
        {
            var Position = v_premises.Find("id_premises", id);
            if (Position > 0)
                v_premises.Position = Position;
        }

        private static TenancyObject RowToTenancyPremises(DataRow row)
        {
            var to = new TenancyObject();
            to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
            to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises");
            to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
            to.RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area");
            return to;
        }

        private List<TenancyObject> TenancyPremisesFromViewport()
        {
            var list = new List<TenancyObject>();
            for (var i = 0; i < snapshot_tenancy_premises.Rows.Count; i++)
            {
                var row = snapshot_tenancy_premises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var to = new TenancyObject();
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
            var list = new List<TenancyObject>();
            for (var i = 0; i < v_tenancy_premises.Count; i++)
            {
                var to = new TenancyObject();
                var row = ((DataRowView)v_tenancy_premises[i]);
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
            foreach (var premises in tenancyPremises)
            {
                if (!ViewportHelper.PremiseFundAndRentMatch(premises.IdObject.Value, (int)ParentRow["id_rent_type"]))
                {
                    var idBuilding = (int)PremisesDataModel.GetInstance().Select().Rows.Find(premises.IdObject.Value)["id_building"];
                    if (!ViewportHelper.BuildingFundAndRentMatch(idBuilding, (int)ParentRow["id_rent_type"]) &&
                                MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемого помещения. Все равно продолжить сохранение?",
                                "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != 
                                DialogResult.Yes)
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
            DockAreas = DockAreas.Document;
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

            var ds = DataSetManager.DataSet;

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += v_premises_CurrentItemChanged;
            v_premises.DataMember = "premises";
            v_premises.DataSource = ds;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = "Помещения найма №" + ParentRow["id_process"];
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
            for (var i = 0; i < v_tenancy_premises.Count; i++)
                snapshot_tenancy_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_premises[i])));
            v_snapshot_tenancy_premises = new BindingSource();
            v_snapshot_tenancy_premises.DataSource = snapshot_tenancy_premises;

            id_premises_type.DataSource = v_premises_types;
            id_premises_type.ValueMember = "id_premises_type";
            id_premises_type.DisplayMember = "premises_type";
            // Настраивем компонент отображения комнат
            var details = new TenancySubPremisesDetails();
            details.v_sub_premises = v_sub_premises;
            details.sub_premises = sub_premises.Select();
            details.StaticFilter = StaticFilter;
            details.ParentRow = ParentRow;
            details.ParentType = ParentType;
            details.menuCallback = MenuCallback;
            details.InitializeControl();
            dataGridView.DetailsControl = details;

            //Строим фильтр арендуемых квартир и комнат во время первой загрузки
            if (string.IsNullOrEmpty(DynamicFilter))
            {
                if (v_tenancy_premises.Count > 0)
                {
                    DynamicFilter = "id_premises IN (0";
                    for (var i = 0; i < v_tenancy_premises.Count; i++)
                        DynamicFilter += "," + ((DataRowView)v_tenancy_premises[i])["id_premises"];
                    DynamicFilter += ")";
                }
                if (details.v_snapshot_tenancy_sub_premises.Count > 0)
                {
                    if (!string.IsNullOrEmpty(DynamicFilter))
                        DynamicFilter += " OR ";
                    DynamicFilter += "id_premises IN (0";
                    for (var i = 0; i < details.v_snapshot_tenancy_sub_premises.Count; i++)
                    {
                        var row = ((DataRowView)details.v_snapshot_tenancy_sub_premises[i]);
                        var subPremisesRow = sub_premises.Select().Rows.Find(row["id_sub_premises"]);
                        if (subPremisesRow != null)
                            DynamicFilter += "," + subPremisesRow["id_premises"];
                    }
                    DynamicFilter += ")";
                }
            }

            v_premises.Filter += DynamicFilter;

            premises.Select().RowChanged += PremisesListViewport_RowChanged;
            premises.Select().RowDeleted += PremisesListViewport_RowDeleted;
            tenancy_premises.Select().RowChanged += TenancyPremisesViewport_RowChanged;
            tenancy_premises.Select().RowDeleting += TenancyPremisesViewport_RowDeleting;
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
                var id_building = (int)((DataRowView)v_premises[v_premises.Position])["id_building"];
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
            snapshot_tenancy_premises.Clear();
            for (var i = 0; i < v_tenancy_premises.Count; i++)
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
            var list = TenancyPremisesFromViewport();
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
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = tenancy_premises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    var id_assoc = TenancyPremisesAssocDataModel.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        tenancy_premises.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_premises[
                        v_snapshot_tenancy_premises.Find("id_premises", list[i].IdObject)])["id_assoc"] = id_assoc;
                    tenancy_premises.Select().Rows.Add(id_assoc, list[i].IdObject, list[i].IdProcess, list[i].RentTotalArea, list[i].RentLivingArea, 0);
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
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < v_snapshot_tenancy_premises.Count; j++)
                {
                    var row = (DataRowView)v_snapshot_tenancy_premises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_assoc"].ToString()) &&
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
                    var snapshot_row_index = -1;
                    for (var j = 0; j < v_snapshot_tenancy_premises.Count; j++)
                        if (((DataRowView)v_snapshot_tenancy_premises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_tenancy_premises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        var premises_row_index = v_premises.Find("id_premises", list[i].IdObject);
                        ((DataRowView)v_snapshot_tenancy_premises[snapshot_row_index]).Delete();
                        if (premises_row_index != -1)
                            dataGridView.InvalidateRow(premises_row_index);
                    }
                    tenancy_premises.Select().Rows.Find(list[i].IdAssoc).Delete();
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
            var viewport = new TenancyPremisesViewport(this, MenuCallback);
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
            tenancy_premises.Select().RowChanged -= TenancyPremisesViewport_RowChanged;
            tenancy_premises.Select().RowDeleting -= TenancyPremisesViewport_RowDeleting;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            premises.Select().RowChanged -= PremisesListViewport_RowChanged;
            premises.Select().RowDeleted -= PremisesListViewport_RowDeleted;
            tenancy_premises.Select().RowChanged -= TenancyPremisesViewport_RowChanged;
            tenancy_premises.Select().RowDeleting -= TenancyPremisesViewport_RowDeleting;
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
                var row_index = v_snapshot_tenancy_premises.Find("id_premises", e.Row["id_premises"]);
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
            var row_index = v_snapshot_tenancy_premises.Find("id_premises", e.Row["id_premises"]);
            if (row_index == -1 && v_tenancy_premises.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_tenancy_premises.Rows.Add(e.Row["id_assoc"], e.Row["id_premises"], true, e.Row["rent_total_area"], e.Row["rent_living_area"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)v_snapshot_tenancy_premises[row_index]);
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
            var width = 0;
            for (var i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((TenancySubPremisesDetails)dataGridView.DetailsControl).SetControlWidth(width);
        }

        void dataGridView_Resize(object sender, EventArgs e)
        {
            var width = 0;
            for (var i = 0; i < dataGridView.Columns.Count; i++)
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
            var row_index = v_snapshot_tenancy_premises.Find("id_premises", id_premises);
            double value = 0;
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_tenancy_premises.Rows.Add(null, id_premises, e.Value, null);
                    else
                        ((DataRowView)v_snapshot_tenancy_premises[row_index])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                    value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_premises.Rows.Add(null, id_premises, false, value == 0 ? DBNull.Value : (object)value, DBNull.Value);
                    else
                        ((DataRowView)v_snapshot_tenancy_premises[row_index])["rent_total_area"] = value == 0 ? DBNull.Value : (object)value;
                    break;
                case "rent_living_area":
                    value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_premises.Rows.Add(null, id_premises, false, DBNull.Value, value == 0 ? DBNull.Value : (object)value);
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
            var id_premises = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_tenancy_premises.Find("id_premises", id_premises);
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
                case "total_area":
                    e.Value = row["total_area"];
                    break;
                case "id_state":
                    var state_row = object_states.Select().Rows.Find(row["id_state"]);
                    if (state_row != null)
                        e.Value = state_row["state_female"];
                    break;
                case "current_fund":
                    if ((new object[] { 1, 4, 5, 9 }).Contains(row["id_state"]))
                    {
                        var fund_row = premises_funds.Select().Rows.Find(row["id_premises"]);
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
        void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle14 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyPremisesViewport));
            var dataGridViewCellStyle3 = new DataGridViewCellStyle();
            var dataGridViewCellStyle4 = new DataGridViewCellStyle();
            var dataGridViewCellStyle5 = new DataGridViewCellStyle();
            var dataGridViewCellStyle6 = new DataGridViewCellStyle();
            var dataGridViewCellStyle7 = new DataGridViewCellStyle();
            var dataGridViewCellStyle8 = new DataGridViewCellStyle();
            var dataGridViewCellStyle9 = new DataGridViewCellStyle();
            var dataGridViewCellStyle10 = new DataGridViewCellStyle();
            var dataGridViewCellStyle11 = new DataGridViewCellStyle();
            var dataGridViewCellStyle12 = new DataGridViewCellStyle();
            var dataGridViewCellStyle13 = new DataGridViewCellStyle();
            dataGridView = new DataGridViewWithDetails();
            image = new DataGridViewImageColumn();
            is_checked = new DataGridViewCheckBoxColumn();
            rent_total_area = new DataGridViewTextBoxColumn();
            rent_living_area = new DataGridViewTextBoxColumn();
            id_premises = new DataGridViewTextBoxColumn();
            id_street = new DataGridViewTextBoxColumn();
            house = new DataGridViewTextBoxColumn();
            premises_num = new DataGridViewTextBoxColumn();
            id_premises_type = new DataGridViewComboBoxColumn();
            total_area = new DataGridViewTextBoxColumn();
            id_state = new DataGridViewTextBoxColumn();
            current_fund = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(image, is_checked, rent_total_area, rent_living_area, id_premises, id_street, house, premises_num, id_premises_type, total_area, id_state, current_fund);
            dataGridViewCellStyle14.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = Color.FromArgb(224, 224, 224);
            dataGridViewCellStyle14.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle14.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = DataGridViewTriState.False;
            dataGridView.DefaultCellStyle = dataGridViewCellStyle14;
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
            dataGridView.Size = new Size(1364, 298);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.BeforeExpandDetails += dataGridView_BeforeExpandDetails;
            dataGridView.BeforeCollapseDetails += dataGridView_BeforeCollapseDetails;
            dataGridView.CellContentClick += dataGridView_CellContentClick;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.CellValuePushed += dataGridView_CellValuePushed;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.CurrentCellDirtyStateChanged += dataGridView_CurrentCellDirtyStateChanged;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
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
            // rent_total_area
            // 
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.BackColor = Color.White;
            dataGridViewCellStyle4.Format = "#0.0## м²";
            rent_total_area.DefaultCellStyle = dataGridViewCellStyle4;
            rent_total_area.HeaderText = "Площадь койко-места";
            rent_total_area.MinimumWidth = 160;
            rent_total_area.Name = "rent_total_area";
            rent_total_area.SortMode = DataGridViewColumnSortMode.NotSortable;
            rent_total_area.Width = 160;
            // 
            // rent_living_area
            // 
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.Format = "#0.0## м²";
            rent_living_area.DefaultCellStyle = dataGridViewCellStyle5;
            rent_living_area.HeaderText = "Арендуемая S жил.";
            rent_living_area.MinimumWidth = 130;
            rent_living_area.Name = "rent_living_area";
            rent_living_area.SortMode = DataGridViewColumnSortMode.NotSortable;
            rent_living_area.Visible = false;
            rent_living_area.Width = 130;
            // 
            // id_premises
            // 
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.TopLeft;
            id_premises.DefaultCellStyle = dataGridViewCellStyle6;
            id_premises.HeaderText = "№";
            id_premises.MinimumWidth = 100;
            id_premises.Name = "id_premises";
            id_premises.ReadOnly = true;
            // 
            // id_street
            // 
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.TopLeft;
            id_street.DefaultCellStyle = dataGridViewCellStyle7;
            id_street.HeaderText = "Адрес";
            id_street.MinimumWidth = 300;
            id_street.Name = "id_street";
            id_street.ReadOnly = true;
            id_street.SortMode = DataGridViewColumnSortMode.NotSortable;
            id_street.Width = 300;
            // 
            // house
            // 
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.TopLeft;
            house.DefaultCellStyle = dataGridViewCellStyle8;
            house.HeaderText = "Дом";
            house.MinimumWidth = 100;
            house.Name = "house";
            house.ReadOnly = true;
            house.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // premises_num
            // 
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.TopLeft;
            premises_num.DefaultCellStyle = dataGridViewCellStyle9;
            premises_num.HeaderText = "Помещение";
            premises_num.MinimumWidth = 100;
            premises_num.Name = "premises_num";
            premises_num.ReadOnly = true;
            // 
            // id_premises_type
            // 
            dataGridViewCellStyle10.Alignment = DataGridViewContentAlignment.TopLeft;
            id_premises_type.DefaultCellStyle = dataGridViewCellStyle10;
            id_premises_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_premises_type.HeaderText = "Тип помещения";
            id_premises_type.MinimumWidth = 150;
            id_premises_type.Name = "id_premises_type";
            id_premises_type.ReadOnly = true;
            id_premises_type.Width = 150;
            // 
            // total_area
            // 
            dataGridViewCellStyle11.Alignment = DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle11.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle11;
            total_area.HeaderText = "Общая площадь";
            total_area.MinimumWidth = 130;
            total_area.Name = "total_area";
            total_area.ReadOnly = true;
            total_area.Width = 130;
            // 
            // id_state
            // 
            dataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.TopLeft;
            id_state.DefaultCellStyle = dataGridViewCellStyle12;
            id_state.HeaderText = "Текущее состояние";
            id_state.MinimumWidth = 170;
            id_state.Name = "id_state";
            id_state.ReadOnly = true;
            id_state.Width = 170;
            // 
            // current_fund
            // 
            dataGridViewCellStyle13.Alignment = DataGridViewContentAlignment.TopLeft;
            current_fund.DefaultCellStyle = dataGridViewCellStyle13;
            current_fund.HeaderText = "Текущий фонд";
            current_fund.MinimumWidth = 170;
            current_fund.Name = "current_fund";
            current_fund.ReadOnly = true;
            current_fund.SortMode = DataGridViewColumnSortMode.NotSortable;
            current_fund.Width = 170;
            // 
            // TenancyPremisesViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(1370, 304);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyPremisesViewport";
            Padding = new Padding(3);
            Text = "Перечень помещений";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
