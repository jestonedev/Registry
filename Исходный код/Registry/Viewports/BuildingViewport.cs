using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ModalEditors;
using Registry.Viewport.Presenters;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class BuildingViewport : FormViewport
    {
        private BuildingViewport()
            : this(null, null)
        {
        }

        public BuildingViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new BuildingPresenter())
        {
            InitializeComponent();
            dataGridViewOwnerships.AutoGenerateColumns = false;
            dataGridViewRestrictions.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            numericUpDownStartupYear.Maximum = DateTime.Now.Year;
        }

        private void FiltersRebuild()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                ResetMunicipalInfo();
                return;
            }
            if (row[Presenter.ViewModel["general"].PrimaryKeyFirst] == DBNull.Value)
            {
                ResetMunicipalInfo(); 
                return;
            }
            IsEditable = false;
            var idBuilding = (int)row["id_building"];

            var currentFundBindingSource = Presenter.ViewModel["buildings_current_funds"].BindingSource;
            var position = currentFundBindingSource.Find("id_building", row["id_building"]);
            comboBoxCurrentFundType.SelectedValue = position != -1 ? ((DataRowView)currentFundBindingSource[position])["id_fund_type"] : DBNull.Value;
            ShowOrHideCurrentFund();

            var municipalPremises = Presenter.ViewModel["municipal_premises"].DataSource.AsEnumerable();
            var socialCount = municipalPremises.Count(s => s.Field<int>("id_building") == idBuilding && s.Field<int>("id_fund_type") == 1);
            var commercialCount = municipalPremises.Count(s => s.Field<int>("id_building") == idBuilding && s.Field<int>("id_fund_type") == 2);
            var specialCount = municipalPremises.Count(s => s.Field<int>("id_building") == idBuilding && s.Field<int>("id_fund_type") == 3);
            var otherCount = municipalPremises.Count(s => s.Field<int>("id_building") == idBuilding && s.Field<int>("id_fund_type") == 0);
            numericUpDownSocialPremisesCount.Value = socialCount;
            numericUpDownSpecialPremisesCount.Value = specialCount;
            numericUpDownOtherPremisesCount.Value = otherCount;
            numericUpDownCommercialPremisesCount.Value = commercialCount;
            var totalMunCount = Convert.ToDecimal(municipalPremises.Count(s => s.Field<int>("id_building") == idBuilding));
            var totalPremisesCount = Convert.ToDecimal(
                Math.Max(Presenter.ViewModel["premises"].Model.FilterDeletedRows().Count(b => b.Field<int>("id_building") == idBuilding),
                row["num_apartments"] == DBNull.Value ? 0 : (int)row["num_apartments"]));
            if (totalPremisesCount > 0)
            {
                var percentage = (totalMunCount/totalPremisesCount)*100;
                numericUpDownMunPremisesPercentage.Value = percentage;
            }
            else
            {
                numericUpDownMunPremisesPercentage.Value = 0;
            }
            var sumArea = Convert.ToDecimal(municipalPremises.Where(s => s.Field<int>("id_building") == idBuilding).Sum(m => m.Field<double>("total_area")));
            numericUpDownMunicipalArea.Value = sumArea;
            numericUpDownMunPremisesCount.Value = totalMunCount;
            IsEditable = true;
        }

        private void ResetMunicipalInfo()
        {
            numericUpDownSocialPremisesCount.Value = 0;
            numericUpDownSpecialPremisesCount.Value = 0;
            numericUpDownOtherPremisesCount.Value = 0;
            numericUpDownCommercialPremisesCount.Value = 0;
            numericUpDownMunPremisesPercentage.Value = 0;
            numericUpDownMunicipalArea.Value = 0;
            numericUpDownMunPremisesCount.Value = 0;    
        }

        private void ShowOrHideCurrentFund()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (comboBoxCurrentFundType.SelectedValue != null && row != null && row["id_state"] != DBNull.Value &&
                DataModelHelper.MunicipalAndUnknownObjectStates().Contains((int)row["id_state"]))
            {
                label19.Visible = true;
                comboBoxCurrentFundType.Visible = true;
            }
            else
            {
                label19.Visible = false;
                comboBoxCurrentFundType.Visible = false;
            }
        }

        private void UnbindedUpdate()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            IsEditable = false;
            if (row != null && (row["state_date"] != DBNull.Value))
                dateTimePickerStateDate.Checked = true;
            else
            {
                dateTimePickerStateDate.Value = DateTime.Now.Date;
                dateTimePickerStateDate.Checked = false;
            }

            if (row == null || row["id_building"] == DBNull.Value)
            {
                numericUpDownRentCoefficientAuto.Value = 0;
            }
            else
            {
                var isEmergency = (from emergencyId in BuildingService.BuildingIDsByOwnershipType(2)
                    where emergencyId == (int) row["id_building"]
                    select emergencyId).Any();
                numericUpDownRentCoefficientAuto.Value =
                    BuildingService.GetRentCoefficient(BuildingService.GetRentCategory(row.Row, isEmergency));
            }

            IsEditable = true;
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindSource(comboBoxStreet, Presenter.ViewModel["kladr"].BindingSource, "street_name", 
                Presenter.ViewModel["kladr"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxStreet, "SelectedValue", bindingSource, 
                Presenter.ViewModel["kladr"].PrimaryKeyFirst, DBNull.Value);
            ViewportHelper.BindProperty(textBoxDescription, "Text", bindingSource, "description", "");
            ViewportHelper.BindProperty(textBoxHouse, "Text", bindingSource, "house", "");
            ViewportHelper.BindProperty(numericUpDownFloors, "Value", bindingSource, "floors", 5m);
            ViewportHelper.BindProperty(numericUpDownStartupYear, "Value", bindingSource, "startup_year", (decimal)DateTime.Now.Year);
            ViewportHelper.BindProperty(textBoxCadastralNum, "Text", bindingSource, "cadastral_num", "");
            ViewportHelper.BindProperty(numericUpDownCadastralCost, "Value", bindingSource, "cadastral_cost", 0m);
            ViewportHelper.BindProperty(numericUpDownBalanceCost, "Value", bindingSource, "balance_cost", 0m);
            ViewportHelper.BindProperty(checkBoxImprovement, "Checked", bindingSource, "improvement", true);
            ViewportHelper.BindProperty(checkBoxElevator, "Checked", bindingSource, "elevator", false);
            ViewportHelper.BindProperty(checkBoxRubbishChute, "Checked", bindingSource, "rubbish_chute", false);
            ViewportHelper.BindProperty(checkBoxPlumbing, "Checked", bindingSource, "plumbing", true);
            ViewportHelper.BindProperty(checkBoxHotWaterSupply, "Checked", bindingSource, "hot_water_supply", true);
            ViewportHelper.BindProperty(checkBoxCanalization, "Checked", bindingSource, "canalization", true);
            ViewportHelper.BindProperty(checkBoxElectricity, "Checked", bindingSource, "electricity", true);
            ViewportHelper.BindProperty(checkBoxRadioNetwork, "Checked", bindingSource, "radio_network", false);
            ViewportHelper.BindProperty(textBoxRoomsBTI, "Text", bindingSource, "BTI_rooms", "");
            ViewportHelper.BindProperty(textBoxHousingCooperative, "Text", bindingSource, "housing_cooperative", "");
            ViewportHelper.BindProperty(numericUpDownPremisesCount, "Value", bindingSource, "num_premises", 0m);
            ViewportHelper.BindProperty(numericUpDownRoomsCount, "Value", bindingSource, "num_rooms", 0m);
            ViewportHelper.BindProperty(numericUpDownApartmentsCount, "Value", bindingSource, "num_apartments", 0m);
            ViewportHelper.BindProperty(numericUpDownSharedApartmentsCount, "Value", bindingSource, "num_shared_apartments", 0m);
            ViewportHelper.BindProperty(numericUpDownLivingArea, "Value", bindingSource, "living_area", 0m);
            ViewportHelper.BindProperty(numericUpDownTotalArea, "Value", bindingSource, "total_area", 0m);
            ViewportHelper.BindProperty(numericUpDownWear, "Value", bindingSource, "wear", 0m);
            ViewportHelper.BindProperty(dateTimePickerStateDate, "Value", bindingSource, "state_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(dateTimePickerRegDate, "Value", bindingSource, "reg_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(numericUpDownRentCoefficient, "Value", bindingSource, "rent_coefficient", 0m);


            ViewportHelper.BindSource(comboBoxStructureType, Presenter.ViewModel["structure_types"].BindingSource, "structure_type",
                Presenter.ViewModel["structure_types"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxStructureType, "SelectedValue", bindingSource,
                Presenter.ViewModel["structure_types"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindSource(comboBoxHeatingType, Presenter.ViewModel["heating_types"].BindingSource, "heating_type",
                Presenter.ViewModel["heating_types"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxHeatingType, "SelectedValue", bindingSource,
                Presenter.ViewModel["heating_types"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindSource(comboBoxState, Presenter.ViewModel["object_states"].BindingSource, "state_neutral",
                Presenter.ViewModel["object_states"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxState, "SelectedValue", bindingSource,
                Presenter.ViewModel["object_states"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindSource(comboBoxCurrentFundType, Presenter.ViewModel["fund_types"].BindingSource, "fund_type",
                Presenter.ViewModel["fund_types"].PrimaryKeyFirst);
            
            dataGridViewRestrictions.DataSource = Presenter.ViewModel["restrictions"].BindingSource;
            restriction_number.DataPropertyName = "number";
            restriction_date.DataPropertyName = "date";
            restriction_date_state_reg.DataPropertyName = "date_state_reg";
            restriction_description.DataPropertyName = "description";
            ViewportHelper.BindSource(id_restriction_type, Presenter.ViewModel["restriction_types"].BindingSource, "restriction_type",
                Presenter.ViewModel["restriction_types"].PrimaryKeyFirst);

            dataGridViewOwnerships.DataSource = Presenter.ViewModel["ownership_rights"].BindingSource;
            ownership_number.DataPropertyName = "number";
            ownership_date.DataPropertyName = "date";
            ownership_description.DataPropertyName = "description";
            ViewportHelper.BindSource(id_ownership_type, Presenter.ViewModel["ownership_right_types"].BindingSource, "ownership_right_type",
                Presenter.ViewModel["ownership_right_types"].PrimaryKeyFirst);
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) ||
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)))
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidateBuilding(Building building)
        {
            if (building.IdStreet == null)
            {
                MessageBox.Show(@"Необходимо выбрать улицу", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return false;
            }
            if (building.IdState == null)
            {
                MessageBox.Show(@"Необходимо выбрать состояние здания", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxState.Focus();
                return false;
            }
            if ((building.House == null) || String.IsNullOrEmpty(building.House.Trim()))
            {
                MessageBox.Show(@"Необходимо указать номер дома", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return false;
            }
            if (!Regex.IsMatch(building.House, @"^[0-9]+[а-яА-Я]{0,1}([\/][0-9]+[а-яА-Я]{0,1}){0,1}$"))
            {
                MessageBox.Show(@"Некорректно задан номер дома. Можно использовать только цифры, строчные и прописные буквы кириллицы буквы и дробный разделитель. Например: ""11а/3""", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return false;
            }
            if (building.IdStructureType == null)
            {
                MessageBox.Show(@"Необходимо выбрать материал здания", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStructureType.Focus();
                return false;
            }
            if (building.IdHeatingType == null)
            {
                MessageBox.Show(@"Не указан тип отопления здания", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            // Проверяем права на модификацию муниципального или не муниципального здания
            var buildingFromView = (Building)EntityFromView();
            if (buildingFromView.IdBuilding != null && OtherService.HasMunicipal(buildingFromView.IdBuilding.Value, EntityType.Building)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"Вы не можете изменить информацию по данному зданию, т.к. оно является муниципальным или содержит в себе муниципальные помещения", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (buildingFromView.IdBuilding != null && OtherService.HasNotMunicipal(buildingFromView.IdBuilding.Value, EntityType.Building)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"Вы не можете изменить информацию по данному зданию, т.к. оно является немуниципальным или содержит в себе немуниципальные помещения",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.MunicipalObjectStates().Contains(building.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на добавление в базу муниципальных жилых зданий", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.NonMunicipalAndUnknownObjectStates().Contains(building.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на добавление в базу немуниципальных жилых зданий", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            // Подверждение на износ выше 100%
            if (building.Wear > 100)
                if (MessageBox.Show(@"Вы задали износ здания выше 100%. Все равно продолжить сохранение?", @"Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                {
                    numericUpDownWear.Focus();
                    return false;
                }
            // Проверяем дубликаты адресов домов
            if ((building.House != buildingFromView.House) || (building.IdStreet != buildingFromView.IdStreet))
                if (BuildingService.BuildingsDuplicateCount(building) != 0 &&
                    MessageBox.Show(@"В базе уже имеется здание с таким адресом. Все равно продолжить сохранение?", @"Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
            return true;
        }

        private void SetViewportCaption()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (ViewportState == ViewportState.NewRowState)
                Text = @"Новое здание";
            else if (row != null)
                Text = string.Format(CultureInfo.InvariantCulture, "Здание №{0}", row["id_building"]);
            else
                Text = @"Здания отсутствуют";
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var building = new Building
            {
                IdBuilding = row == null ? null : ViewportHelper.ValueOrNull<int>(row, "id_building"),
                IdStreet = ViewportHelper.ValueOrNull(comboBoxStreet),
                IdState = ViewportHelper.ValueOrNull<int>(comboBoxState),
                House = ViewportHelper.ValueOrNull(textBoxHouse),
                IdStructureType = ViewportHelper.ValueOrNull<int>(comboBoxStructureType),
                CadastralNum = ViewportHelper.ValueOrNull(textBoxCadastralNum),
                Description = ViewportHelper.ValueOrNull(textBoxDescription),
                Floors = Convert.ToInt16(numericUpDownFloors.Value),
                StartupYear = Convert.ToInt32(numericUpDownStartupYear.Value),
                CadastralCost = numericUpDownCadastralCost.Value,
                BalanceCost = numericUpDownBalanceCost.Value,
                Improvement = checkBoxImprovement.Checked,
                Elevator = checkBoxElevator.Checked,
                RubbishChute = checkBoxRubbishChute.Checked,
                NumPremises = Convert.ToInt32(numericUpDownPremisesCount.Value),
                NumRooms = Convert.ToInt32(numericUpDownRoomsCount.Value),
                NumApartments = Convert.ToInt32(numericUpDownApartmentsCount.Value),
                NumSharedApartments = Convert.ToInt32(numericUpDownSharedApartmentsCount.Value),
                LivingArea = Convert.ToDouble(numericUpDownLivingArea.Value),
                TotalArea = Convert.ToDouble(numericUpDownTotalArea.Value),
                Wear = Convert.ToDouble(numericUpDownWear.Value),
                StateDate = ViewportHelper.ValueOrNull(dateTimePickerStateDate),
                RegDate = ViewportHelper.ValueOrNull(dateTimePickerRegDate),
                Plumbing = checkBoxPlumbing.Checked,
                HotWaterSupply = checkBoxHotWaterSupply.Checked,
                Canalization = checkBoxCanalization.Checked,
                Electricity = checkBoxElectricity.Checked,
                RadioNetwork = checkBoxRadioNetwork.Checked,
                IdHeatingType =ViewportHelper.ValueOrNull<int>(comboBoxHeatingType),
                RoomsBTI = ViewportHelper.ValueOrNull(textBoxRoomsBTI),
                HousingCooperative = ViewportHelper.ValueOrNull(textBoxHousingCooperative),
                RentCoefficient = numericUpDownRentCoefficient.Value
            };
            return building;
        }

        protected override Entity EntityFromView()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return row == null ? new Building() : EntityConverter<Building>.FromRow(row);
        }

        private void ViewportFromBuilding(Building building)
        {
            comboBoxStreet.SelectedValue = ViewportHelper.ValueOrDbNull(building.IdStreet);
            comboBoxState.SelectedValue = ViewportHelper.ValueOrDbNull(building.IdState);
            comboBoxStructureType.SelectedValue = ViewportHelper.ValueOrDbNull(building.IdStructureType);
            numericUpDownFloors.Value = ViewportHelper.ValueOrDefault(building.Floors);
            numericUpDownStartupYear.Value = ViewportHelper.ValueOrDefault(building.StartupYear);
            checkBoxImprovement.Checked = ViewportHelper.ValueOrDefault(building.Improvement);
            checkBoxElevator.Checked = ViewportHelper.ValueOrDefault(building.Elevator);
            checkBoxRubbishChute.Checked = ViewportHelper.ValueOrDefault(building.RubbishChute);
            numericUpDownPremisesCount.Value = ViewportHelper.ValueOrDefault(building.NumPremises);
            numericUpDownApartmentsCount.Value = ViewportHelper.ValueOrDefault(building.NumApartments);
            numericUpDownSharedApartmentsCount.Value = ViewportHelper.ValueOrDefault(building.NumSharedApartments);
            numericUpDownLivingArea.Value = (decimal)ViewportHelper.ValueOrDefault(building.LivingArea);
            numericUpDownTotalArea.Value = (decimal)ViewportHelper.ValueOrDefault(building.TotalArea);
            numericUpDownCadastralCost.Value = ViewportHelper.ValueOrDefault(building.CadastralCost);
            numericUpDownBalanceCost.Value = ViewportHelper.ValueOrDefault(building.BalanceCost);
            numericUpDownWear.Value = (decimal)ViewportHelper.ValueOrDefault(building.Wear);
            numericUpDownRentCoefficient.Value = ViewportHelper.ValueOrDefault(building.RentCoefficient);
            textBoxHouse.Text = building.House;
            textBoxCadastralNum.Text = building.CadastralNum;
            textBoxDescription.Text = building.Description;
            dateTimePickerStateDate.Value = ViewportHelper.ValueOrDefault(building.StateDate);
            dateTimePickerRegDate.Value = ViewportHelper.ValueOrDefault(building.RegDate);
            checkBoxPlumbing.Checked = ViewportHelper.ValueOrDefault(building.Plumbing);
            checkBoxHotWaterSupply.Checked = ViewportHelper.ValueOrDefault(building.HotWaterSupply);
            checkBoxCanalization.Checked = ViewportHelper.ValueOrDefault(building.Canalization);
            checkBoxElectricity.Checked = ViewportHelper.ValueOrDefault(building.Electricity);
            checkBoxRadioNetwork.Checked = ViewportHelper.ValueOrDefault(building.RadioNetwork);
            comboBoxHeatingType.SelectedValue = ViewportHelper.ValueOrDefault(building.IdHeatingType);
            textBoxRoomsBTI.Text = building.RoomsBTI;
            textBoxHousingCooperative.Text = building.HousingCooperative;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            var restricitonAssoc = Presenter.ViewModel["buildings_restrictions_buildings_assoc"].DataSource;
            AddEventHandler<DataRowChangeEventArgs>(restricitonAssoc, "RowDeleted", RestrictionsAssoc_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(restricitonAssoc, "RowChanged", RestrictionsAssoc_RowChanged);

            var ownershipAssoc = Presenter.ViewModel["buildings_ownership_buildings_assoc"].DataSource;
            AddEventHandler<DataRowChangeEventArgs>(ownershipAssoc, "RowDeleted", OwnershipsAssoc_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(ownershipAssoc, "RowChanged", OwnershipsAssoc_RowChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", BuildingViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", BuildingViewport_RowChanged);

            AddEventHandler<EventArgs>(Presenter.ViewModel["buildings_current_funds"].Model, "RefreshEvent", buildingsCurrentFund_RefreshEvent);
            AddEventHandler<EventArgs>(Presenter.ViewModel["buildings_premises_funds"].Model, "RefreshEvent", buildingsPremisesFunds_RefreshEvent);
            AddEventHandler<EventArgs>(Presenter.ViewModel["buildings_premises_sum_area"].Model, "RefreshEvent", buildingsPremisesSumArea_RefreshEvent);

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", v_building_CurrentItemChanged);
            AddEventHandler<EventArgs>(Presenter.ViewModel["buildings_ownership_buildings_assoc"].BindingSource, "CurrentItemChanged", 
                v_ownershipBuildingsAssoc_CurrentItemChanged);
            AddEventHandler<EventArgs>(Presenter.ViewModel["buildings_restrictions_buildings_assoc"].BindingSource, "CurrentItemChanged", 
                v_restrictionBuildingsAssoc_CurrentItemChanged);

            DataBind();

            v_building_CurrentItemChanged(null, new EventArgs());
            v_ownershipBuildingsAssoc_CurrentItemChanged(null, new EventArgs());
            v_restrictionBuildingsAssoc_CurrentItemChanged(null, new EventArgs());

            ((BuildingPresenter)Presenter).OwnershipsFilterRebuild();
            ((BuildingPresenter)Presenter).RestrictionsFilterRebuild();

            DataChangeHandlersInit();
            IsEditable = true;
        }
        
        public override bool CanSearchRecord()
        {
            return true;
        }

        public override void ClearSearch()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].BindingSource.Filter = StaticFilter;
            IsEditable = true;
            DynamicFilter = "";
        }

        public override bool SearchedRecords()
        {
            return !string.IsNullOrEmpty(DynamicFilter);
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (Presenter.SimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.SimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (Presenter.ExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.ExtendedSearchForm.GetFilter();
                    break;
            }
            IsEditable = false;
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            IsEditable = true;
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            var building = (Building) EntityFromViewport();
            if (!ValidateBuilding(building))
                return;
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((BuildingPresenter) Presenter).InsertRecord(building))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((BuildingPresenter) Presenter).UpdateRecord(building))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            UnbindedUpdate();
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            ShowOrHideCurrentFund();
        }

        public override bool CanCancelRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    Presenter.ViewModel["general"].Model.EditingNewRecord = false;
                    var row = Presenter.ViewModel["general"].CurrentRow;
                    if (row != null)
                    {
                        IsEditable = false;
                        row.Delete();
                    }
                    break;
                case ViewportState.ModifyRowState:
                    Presenter.ViewModel["kladr"].BindingSource.Filter = "";
                    IsEditable = false;
                    DataBind();
                    break;
            }
            UnbindedUpdate();
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            UnbindedUpdate();
            IsEditable = true;
        }

        public override bool CanCopyRecord()
        {
            return Presenter.ViewModel["general"].CurrentRow != null && !Presenter.ViewModel["general"].Model.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            var building = (Building)EntityFromView();
            building.RoomsBTI = null;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            ViewportFromBuilding(building);
            IsEditable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) && (ViewportState != ViewportState.NewRowState) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            var hasResettles = ((BuildingPresenter) Presenter).HasResettles();
            var hasTenancies = ((BuildingPresenter) Presenter).HasTenancies();
            if (hasResettles || hasTenancies)
            {
                if (MessageBox.Show(@"К зданию или одному из его помещений привязаны процессы"+
                    (hasTenancies ? " найма" : "")+
                    (hasTenancies && hasResettles ? " и" : "")+
                    (hasResettles ? " переселения" : "") +
                    @". Вы действительно хотите удалить это здание?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return;
            } else
            if (MessageBox.Show(@"Вы действительно хотите удалить это здание?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            IsEditable = false;
            if (!((BuildingPresenter) Presenter).DeleteRecord())
            {
                IsEditable = true;
                return;
            }
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
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
            return reports.Any(v => v.ToString() == typeof(T).Name) && Presenter.ViewModel["general"].CurrentRow != null;
        }

        public override void ShowAssocViewport<T>()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var row = Presenter.ViewModel["general"].CurrentRow;
            var columnName = Presenter.ViewModel["general"].PrimaryKeyFirst;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано здание для отображения истории найма", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, columnName+" = " + Convert.ToInt32(row[columnName], CultureInfo.InvariantCulture), 
                row.Row, ParentTypeEnum.Building);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedUpdate();
            dataGridViewRestrictions.Focus();
            base.OnVisibleChanged(e);
        }

        private void buildingsPremisesSumArea_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        private void buildingsPremisesFunds_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        private void buildingsCurrentFund_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        private void BuildingViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                UnbindedUpdate();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void BuildingViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            ShowOrHideCurrentFund();
            UnbindedUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        private void RestrictionsAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                ((BuildingPresenter)Presenter).RestrictionsFilterRebuild();
        }

        private void RestrictionsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                ((BuildingPresenter)Presenter).RestrictionsFilterRebuild();
        }

        private void OwnershipsAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                ((BuildingPresenter)Presenter).OwnershipsFilterRebuild();
        }

        private void OwnershipsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                ((BuildingPresenter)Presenter).OwnershipsFilterRebuild();
        }

        private void v_building_CurrentItemChanged(object sender, EventArgs e)
        {
            var isEditable = IsEditable;
            SetViewportCaption();
            FiltersRebuild();
            Presenter.ViewModel["kladr"].BindingSource.Filter = "";
            UnbindedUpdate();
            IsEditable = isEditable;

            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void v_ownershipBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            ((BuildingPresenter)Presenter).OwnershipsFilterRebuild();
        }

        private void v_restrictionBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            ((BuildingPresenter)Presenter).RestrictionsFilterRebuild();
        }

        private void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxStreet.Text;
                var selectionStart = comboBoxStreet.SelectionStart;
                var selectionLength = comboBoxStreet.SelectionLength;
                Presenter.ViewModel["kladr"].BindingSource.Filter = "street_name like '%" + comboBoxStreet.Text + "%'";
                comboBoxStreet.Text = text;
                comboBoxStreet.SelectionStart = selectionStart;
                comboBoxStreet.SelectionLength = selectionLength;
            }
        }

        private void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
        }

        private void comboBoxStreet_Leave(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count > 0)
            {
                if (comboBoxStreet.SelectedValue == null)
                    comboBoxStreet.SelectedValue = Presenter.ViewModel["kladr"].CurrentRow;
                comboBoxStreet.Text = Presenter.ViewModel["kladr"].CurrentRow["street_name"].ToString();
            }
            if (comboBoxStreet.SelectedValue == null)
                comboBoxStreet.Text = "";
        }

        private void dataGridViewRestrictions_Resize(object sender, EventArgs e)
        {
            if (dataGridViewRestrictions.Size.Width > 700)
            {
                if (restriction_description.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    restriction_description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (restriction_description.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    restriction_description.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        private void dataGridViewOwnerships_Resize(object sender, EventArgs e)
        {
            if (dataGridViewOwnerships.Size.Width > 600)
            {
                if (ownership_description.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    ownership_description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (ownership_description.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    ownership_description.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        private void dataGridViewRestrictions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocViewport<RestrictionListViewport>())
                ShowAssocViewport<RestrictionListViewport>();
        }

        private void dataGridViewOwnerships_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocViewport<OwnershipListViewport>())
                ShowAssocViewport<OwnershipListViewport>();
        }

        private void textBoxHouse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\\')
                e.KeyChar = '/';
            if (e.KeyChar == ' ')
                e.Handled = true;
        }

        private void vButtonRestrictionAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["restrictions"].Model.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок реквизитов уже находится в режиме добавления новой записи. " +
                    @"Одновременно можно добавлять не более одного реквизита.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = row.Row;
                editor.ShowDialog();
            }
        }

        private void vButtonRestrictionDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (Presenter.ViewModel["restrictions"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбран реквизит для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить этот реквизит?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var id = (int)Presenter.ViewModel["restrictions"].CurrentRow[Presenter.ViewModel["restrictions"].PrimaryKeyFirst];
            Presenter.ViewModel["restrictions"].Delete(id);
        }

        private void vButtonRestrictionEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["restrictions"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран реквизит для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var restriction = EntityConverter<Restriction>.FromRow(row);
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = Presenter.ViewModel["general"].CurrentRow.Row;
                editor.RestrictionValue = restriction;
                editor.ShowDialog();
            }
        }

        private void vButtonOwnershipAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["ownership_rights"].Model.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок ограничений уже находится в режиме добавления новой записи. " +
                    @"Одновременно можно добавлять не более одного ограничения.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = Presenter.ViewModel["general"].CurrentRow.Row;
                editor.ShowDialog();
            }
        }

        private void vButtonOwnershipDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["ownership_rights"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано ограничение для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить это ограничение?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var id = (int)row[Presenter.ViewModel["ownership_rights"].PrimaryKeyFirst];
            Presenter.ViewModel["ownership_rights"].Delete(id);
        }

        private void vButtonOwnershipEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["ownership_rights"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано ограничение для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var ownershipRight = EntityConverter<OwnershipRight>.FromRow(row);
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = Presenter.ViewModel["general"].CurrentRow.Row;
                editor.OwnershipRightValue = ownershipRight;
                editor.ShowDialog();
            }
        }

        internal int GetCurrentId()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var columnName = Presenter.ViewModel["general"].PrimaryKeyFirst;
            if (row == null) return -1;
            if (row[columnName] != DBNull.Value)
                return (int)row[columnName];
            return -1;
        }
    }
}