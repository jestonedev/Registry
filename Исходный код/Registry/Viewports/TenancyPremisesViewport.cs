using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
    internal sealed partial class TenancyPremisesViewport: DataGridViewport
    {
        #region Models
        private DataModel buildings;
        private DataModel kladr;
        private DataModel premises_types;
        private DataModel sub_premises;
        private DataModel tenancy_premises;
        private DataTable snapshot_tenancy_premises;
        private DataModel object_states;
        private CalcDataModel premises_funds;
        private DataModel fund_types;
        #endregion Models

        #region Views
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
            var Position = GeneralBindingSource.Find("id_premises", id);
            if (Position > 0)
                GeneralBindingSource.Position = Position;
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
                if (ViewportHelper.PremiseFundAndRentMatch(premises.IdObject.Value, (int) ParentRow["id_rent_type"]))
                    continue;
                var idBuilding = (int)DataModel.GetInstance(DataModelType.PremisesDataModel).Select().Rows.Find(premises.IdObject.Value)["id_building"];
                if (!ViewportHelper.BuildingFundAndRentMatch(idBuilding, (int)ParentRow["id_rent_type"]) &&
                    MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемого помещения. Все равно продолжить сохранение?",
                        "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != 
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.PremisesDataModel);
            kladr = DataModel.GetInstance(DataModelType.KladrStreetsDataModel);
            buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel);
            premises_types = DataModel.GetInstance(DataModelType.PremisesTypesDataModel);
            sub_premises = DataModel.GetInstance(DataModelType.SubPremisesDataModel);
            tenancy_premises = DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel);
            object_states = DataModel.GetInstance(DataModelType.ObjectStatesDataModel);
            premises_funds = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelPremisesCurrentFunds);
            fund_types = DataModel.GetInstance(DataModelType.FundTypesDataModel);
            object_states.Select();
            premises_funds.Select();
            fund_types.Select();

            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
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

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += v_premises_CurrentItemChanged;
            GeneralBindingSource.DataMember = "premises";
            GeneralBindingSource.DataSource = ds;

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
            v_sub_premises.DataSource = GeneralBindingSource;
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

            GeneralBindingSource.Filter += DynamicFilter;

            GeneralDataModel.Select().RowChanged += PremisesListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += PremisesListViewport_RowDeleted;
            tenancy_premises.Select().RowChanged += TenancyPremisesViewport_RowChanged;
            tenancy_premises.Select().RowDeleting += TenancyPremisesViewport_RowDeleting;
            premises_funds.RefreshEvent += premises_funds_RefreshEvent;

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
            if (MessageBox.Show("Вы действительно хотите удалить это помещение?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                    var id_assoc = DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).Insert(list[i]);
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
                    if (DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).Update(list[i]) == -1)
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
                    if (DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).Delete(list[i].IdAssoc.Value) == -1)
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
                        var premises_row_index = GeneralBindingSource.Find("id_premises", list[i].IdObject);
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
            var viewport = new TenancyPremisesViewport(this, MenuCallback);
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
            tenancy_premises.Select().RowChanged -= TenancyPremisesViewport_RowChanged;
            tenancy_premises.Select().RowDeleting -= TenancyPremisesViewport_RowDeleting;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= PremisesListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= PremisesListViewport_RowDeleted;
            tenancy_premises.Select().RowChanged -= TenancyPremisesViewport_RowChanged;
            tenancy_premises.Select().RowDeleting -= TenancyPremisesViewport_RowDeleting;
            base.ForceClose();
        }

        public override bool HasAssocOwnerships()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocSubPremises()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (GeneralBindingSource.Position > -1);
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
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_premises = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Premises);
        }

        void premises_funds_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }
        
        void v_premises_CurrentItemChanged(object sender, EventArgs e)
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
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var id_premises = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
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
            if (GeneralBindingSource.Count <= e.RowIndex || GeneralBindingSource.Count == 0) return;
            var id_premises = Convert.ToInt32(((DataRowView)GeneralBindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_tenancy_premises.Find("id_premises", id_premises);
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
    }
}
