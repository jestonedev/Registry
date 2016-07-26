using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class BuildingViewport : FormViewport
    {
        #region Models
        private DataModel _restrictions;
        private DataModel _restrictionBuildingsAssoc;
        private DataModel _ownershipRights;
        private DataModel _ownershipBuildingsAssoc;
        private CalcDataModel _buildingsPremisesFunds;
        private CalcDataModel _buildingsCurrentFund;
        private CalcDataModel _buildingsPremisesSumArea;
        private CalcDataModel _municipalPremises;
        #endregion Models

        #region Views
        private BindingSource _vKladr;
        private BindingSource _vStructureTypes;
        private BindingSource _vRestrictions;
        private BindingSource _vRestrictonTypes;
        private BindingSource _vRestrictionBuildingsAssoc;
        private BindingSource _vOwnershipRights;
        private BindingSource _vOwnershipRightTypes;
        private BindingSource _vOwnershipBuildingsAssoc;
        private BindingSource _vFundType;
        private BindingSource _vObjectStates;
        private BindingSource _vBuildingCurrentFund;
        private BindingSource _vHeatingType;

        #endregion Views

        //Forms
        private SearchForm _sbSimpleSearchForm;
        private SearchForm _sbExtendedSearchForm;

        private BuildingViewport()
            : this(null, null)
        {
        }

        public BuildingViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
        }


        private void RestrictionsFilterRebuild()
        {
            var restrictionsFilter = "id_restriction IN (0";
            for (var i = 0; i < _vRestrictionBuildingsAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)_vRestrictionBuildingsAssoc[i])["id_restriction"] + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(',');
            restrictionsFilter += ")";
            _vRestrictions.Filter = restrictionsFilter;
        }

        private void OwnershipsFilterRebuild()
        {
            var ownershipFilter = "id_ownership_right IN (0";
            for (var i = 0; i < _vOwnershipBuildingsAssoc.Count; i++)
                ownershipFilter += ((DataRowView)_vOwnershipBuildingsAssoc[i])["id_ownership_right"] + ",";
            ownershipFilter = ownershipFilter.TrimEnd(',');
            ownershipFilter += ")";
            _vOwnershipRights.Filter = ownershipFilter;
        }

        private void FiltersRebuild()
        {
            if (GeneralBindingSource.Position == -1)
            {
                ResetMunicipalInfo();
                return;
            }
            var row = (DataRowView) GeneralBindingSource[GeneralBindingSource.Position];
            if (row["id_building"] == DBNull.Value)
            {
                ResetMunicipalInfo(); 
                return;
            }
            var idBuilding = (int)row["id_building"];
            if (_vBuildingCurrentFund != null)
            {
                var position = _vBuildingCurrentFund.Find("id_building", row["id_building"]);
                comboBoxCurrentFundType.SelectedValue = position != -1 ? ((DataRowView)_vBuildingCurrentFund[position])["id_fund_type"] : DBNull.Value;
                ShowOrHideCurrentFund();
            }
            var socialCount = _municipalPremises.Select().AsEnumerable().Count(s => s.Field<int>("id_building") == idBuilding && s.Field<int>("id_fund_type") == 1);
            var commercialCount = _municipalPremises.Select().AsEnumerable().Count(s => s.Field<int>("id_building") == idBuilding && s.Field<int>("id_fund_type") == 2);
            var specialCount = _municipalPremises.Select().AsEnumerable().Count(s => s.Field<int>("id_building") == idBuilding && s.Field<int>("id_fund_type") == 3);
            var otherCount = _municipalPremises.Select().AsEnumerable().Count(s => s.Field<int>("id_building") == idBuilding && s.Field<int>("id_fund_type") == 0);
            numericUpDownSocialPremisesCount.Minimum = socialCount;
            numericUpDownSpecialPremisesCount.Minimum = specialCount;
            numericUpDownOtherPremisesCount.Minimum = otherCount;
            numericUpDownCommercialPremisesCount.Minimum = commercialCount;
            numericUpDownSocialPremisesCount.Maximum = socialCount;
            numericUpDownSpecialPremisesCount.Maximum = specialCount;
            numericUpDownOtherPremisesCount.Maximum = otherCount;
            numericUpDownCommercialPremisesCount.Maximum = commercialCount;
            numericUpDownSocialPremisesCount.Value = socialCount;
            numericUpDownSpecialPremisesCount.Value = specialCount;
            numericUpDownOtherPremisesCount.Value = otherCount;
            numericUpDownCommercialPremisesCount.Value = commercialCount;
            var totalMunCount = Convert.ToDecimal(_municipalPremises.Select().AsEnumerable().Count(s => s.Field<int>("id_building") == idBuilding));
            var totalPremisesCount = Convert.ToDecimal(
                Math.Max(DataModel.GetInstance<PremisesDataModel>().FilterDeletedRows().Count(b => b.Field<int>("id_building") == idBuilding),
                row["num_apartments"] == DBNull.Value ? 0 : (int)row["num_apartments"]));
            if (totalPremisesCount > 0)
            {
                var percentage = (totalMunCount/totalPremisesCount)*100;
                numericUpDownMunPremisesPercentage.Maximum = percentage;
                numericUpDownMunPremisesPercentage.Minimum = percentage;
                numericUpDownMunPremisesPercentage.Value = percentage;
            }
            else
            {
                numericUpDownMunPremisesPercentage.Maximum = 0;
                numericUpDownMunPremisesPercentage.Minimum = 0;
                numericUpDownMunPremisesPercentage.Value = 0;
            }
            var sumArea = Convert.ToDecimal(_municipalPremises.Select().AsEnumerable().Where(s => s.Field<int>("id_building") == idBuilding).Sum(m => m.Field<double>("total_area")));
            numericUpDownMunicipalArea.Minimum = sumArea;
            numericUpDownMunicipalArea.Maximum = sumArea;
            numericUpDownMunicipalArea.Value = sumArea;
            numericUpDownMunPremisesCount.Minimum = totalMunCount;
            numericUpDownMunPremisesCount.Maximum = totalMunCount;
            numericUpDownMunPremisesCount.Value = totalMunCount;           
        }

        private void ResetMunicipalInfo()
        {
            numericUpDownSocialPremisesCount.Minimum = 0;
            numericUpDownSpecialPremisesCount.Minimum = 0;
            numericUpDownOtherPremisesCount.Minimum = 0;
            numericUpDownCommercialPremisesCount.Minimum = 0;
            numericUpDownSocialPremisesCount.Maximum = 0;
            numericUpDownSpecialPremisesCount.Maximum = 0;
            numericUpDownOtherPremisesCount.Maximum = 0;
            numericUpDownCommercialPremisesCount.Maximum = 0;
            numericUpDownSocialPremisesCount.Value = 0;
            numericUpDownSpecialPremisesCount.Value = 0;
            numericUpDownOtherPremisesCount.Value = 0;
            numericUpDownCommercialPremisesCount.Value = 0;
            numericUpDownMunPremisesPercentage.Maximum = 0;
            numericUpDownMunPremisesPercentage.Minimum = 0;
            numericUpDownMunPremisesPercentage.Value = 0;
            numericUpDownMunicipalArea.Minimum = 0;
            numericUpDownMunicipalArea.Maximum = 0;
            numericUpDownMunicipalArea.Value = 0;
            numericUpDownMunPremisesCount.Minimum = 0;
            numericUpDownMunPremisesCount.Maximum = 0;
            numericUpDownMunPremisesCount.Value = 0;    
        }

        private void ShowOrHideCurrentFund()
        {
            if (comboBoxCurrentFundType.SelectedValue != null && GeneralBindingSource.Position != -1 &&
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state"] != DBNull.Value &&
                DataModelHelper.MunicipalAndUnknownObjectStates().Contains((int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state"]))
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

        private void UnbindedCheckBoxesUpdate()
        {
            if (GeneralBindingSource.Count == 0) return;
            var row = GeneralBindingSource.Position >= 0 ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if (row != null && (row["state_date"] != DBNull.Value))
                dateTimePickerStateDate.Checked = true;
            else
            {
                dateTimePickerStateDate.Value = DateTime.Now.Date;
                dateTimePickerStateDate.Checked = false;
            }
        }

        private void DataBind()
        {
            comboBoxStreet.DataSource = _vKladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";
            comboBoxStreet.DataBindings.Clear();
            comboBoxStreet.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_street", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            textBoxHouse.DataBindings.Clear();
            textBoxHouse.DataBindings.Add("Text", GeneralBindingSource, "house", true, DataSourceUpdateMode.Never, "");
            numericUpDownFloors.DataBindings.Clear();
            numericUpDownFloors.DataBindings.Add("Value", GeneralBindingSource, "floors", true, DataSourceUpdateMode.Never, 5);
            numericUpDownStartupYear.DataBindings.Clear();
            numericUpDownStartupYear.Maximum = DateTime.Now.Year;
            numericUpDownStartupYear.DataBindings.Add("Value", GeneralBindingSource, "startup_year", true, DataSourceUpdateMode.Never, DateTime.Now.Year);
            textBoxCadastralNum.DataBindings.Clear();
            textBoxCadastralNum.DataBindings.Add("Text", GeneralBindingSource, "cadastral_num", true, DataSourceUpdateMode.Never, "");
            numericUpDownCadastralCost.DataBindings.Clear();
            numericUpDownCadastralCost.DataBindings.Add("Value", GeneralBindingSource, "cadastral_cost", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceCost.DataBindings.Clear();
            numericUpDownBalanceCost.DataBindings.Add("Value", GeneralBindingSource, "balance_cost", true, DataSourceUpdateMode.Never, 0);
            checkBoxImprovement.DataBindings.Clear();
            checkBoxImprovement.DataBindings.Add("Checked", GeneralBindingSource, "improvement", true, DataSourceUpdateMode.Never, true);
            checkBoxElevator.DataBindings.Clear();
            checkBoxElevator.DataBindings.Add("Checked", GeneralBindingSource, "elevator", true, DataSourceUpdateMode.Never, false);
            checkBoxRubbishChute.DataBindings.Clear();
            checkBoxRubbishChute.DataBindings.Add("Checked", GeneralBindingSource, "rubbish_chute", true, DataSourceUpdateMode.Never, false);
            checkBoxPlumbing.DataBindings.Clear();
            checkBoxPlumbing.DataBindings.Add("Checked", GeneralBindingSource, "plumbing", true, 
                DataSourceUpdateMode.Never, false);
            checkBoxHotWaterSupply.DataBindings.Clear();
            checkBoxHotWaterSupply.DataBindings.Add("Checked", GeneralBindingSource, "hot_water_supply", true,
                DataSourceUpdateMode.Never, false);
            checkBoxCanalization.DataBindings.Clear();
            checkBoxCanalization.DataBindings.Add("Checked", GeneralBindingSource, "canalization", true,
                DataSourceUpdateMode.Never, false);
            checkBoxElectricity.DataBindings.Clear();
            checkBoxElectricity.DataBindings.Add("Checked", GeneralBindingSource, "electricity", true,
                DataSourceUpdateMode.Never, false);
            checkBoxRadioNetwork.DataBindings.Clear();
            checkBoxRadioNetwork.DataBindings.Add("Checked", GeneralBindingSource, "radio_network", true,
                DataSourceUpdateMode.Never, false);                      
            textBoxRoomsBTI.DataBindings.Clear();
            textBoxRoomsBTI.DataBindings.Add("Text", GeneralBindingSource, "BTI_rooms", true, DataSourceUpdateMode.Never, "");
            textBoxHousingCooperative.DataBindings.Clear();
            textBoxHousingCooperative.DataBindings.Add("Text", GeneralBindingSource, "housing_cooperative", true, DataSourceUpdateMode.Never, "");
            numericUpDownPremisesCount.DataBindings.Clear();
            numericUpDownPremisesCount.DataBindings.Add("Value", GeneralBindingSource, "num_premises", true, DataSourceUpdateMode.Never, 0);
            numericUpDownRoomsCount.DataBindings.Clear();
            numericUpDownRoomsCount.DataBindings.Add("Value", GeneralBindingSource, "num_rooms", true, DataSourceUpdateMode.Never, 0);
            numericUpDownApartmentsCount.DataBindings.Clear();
            numericUpDownApartmentsCount.DataBindings.Add("Value", GeneralBindingSource, "num_apartments", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSharedApartmentsCount.DataBindings.Clear();
            numericUpDownSharedApartmentsCount.DataBindings.Add("Value", GeneralBindingSource, "num_shared_apartments", true, DataSourceUpdateMode.Never, 0);
            numericUpDownLivingArea.DataBindings.Clear();
            numericUpDownLivingArea.DataBindings.Add("Value", GeneralBindingSource, "living_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownTotalArea.DataBindings.Clear();
            numericUpDownTotalArea.DataBindings.Add("Value", GeneralBindingSource, "total_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownWear.DataBindings.Clear();
            numericUpDownWear.DataBindings.Add("Value", GeneralBindingSource, "wear", true, DataSourceUpdateMode.Never, 0);

            dateTimePickerStateDate.DataBindings.Clear();
            dateTimePickerStateDate.DataBindings.Add("Value", GeneralBindingSource, "state_date", true, DataSourceUpdateMode.Never, null);

            comboBoxStructureType.DataSource = _vStructureTypes;
            comboBoxStructureType.ValueMember = "id_structure_type";
            comboBoxStructureType.DisplayMember = "structure_type";
            comboBoxStructureType.DataBindings.Clear();
            comboBoxStructureType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_structure_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxHeatingType.DataSource = _vHeatingType;
            comboBoxHeatingType.ValueMember = "id_heating_type";
            comboBoxHeatingType.DisplayMember = "heating_type";
            comboBoxHeatingType.DataBindings.Clear();
            comboBoxHeatingType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_heating_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxCurrentFundType.DataSource = _vFundType;
            comboBoxCurrentFundType.ValueMember = "id_fund_type";
            comboBoxCurrentFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = _vObjectStates;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";
            comboBoxState.DataBindings.Clear();
            comboBoxState.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_state", true, DataSourceUpdateMode.Never, DBNull.Value);

            dataGridViewRestrictions.DataSource = _vRestrictions;
            id_restriction_type.DataSource = _vRestrictonTypes;
            id_restriction_type.DataPropertyName = "id_restriction_type";
            id_restriction_type.ValueMember = "id_restriction_type";
            id_restriction_type.DisplayMember = "restriction_type";
            restriction_number.DataPropertyName = "number";
            restriction_date.DataPropertyName = "date";
            restriction_description.DataPropertyName = "description";

            dataGridViewOwnerships.DataSource = _vOwnershipRights;
            id_ownership_type.DataSource = _vOwnershipRightTypes;
            id_ownership_type.DataPropertyName = "id_ownership_right_type";
            id_ownership_type.ValueMember = "id_ownership_right_type";
            id_ownership_type.DisplayMember = "ownership_right_type";
            ownership_number.DataPropertyName = "number";
            ownership_date.DataPropertyName = "date";
            ownership_description.DataPropertyName = "description";
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) ||
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)))
                return base.ChangeViewportStateTo(state);
            viewportState = ViewportState.ReadState;
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
            if (buildingFromView.IdBuilding != null && DataModelHelper.HasMunicipal(buildingFromView.IdBuilding.Value, EntityType.Building)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"Вы не можете изменить информацию по данному зданию, т.к. оно является муниципальным или содержит в себе муниципальные помещения", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (buildingFromView.IdBuilding != null && DataModelHelper.HasNotMunicipal(buildingFromView.IdBuilding.Value, EntityType.Building)
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
                if (DataModelHelper.BuildingsDuplicateCount(building) != 0 &&
                    MessageBox.Show(@"В базе уже имеется здание с таким адресом. Все равно продолжить сохранение?", @"Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
            return true;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
                Text = @"Новое здание";
            else
                if (GeneralBindingSource.Position != -1)
                    Text = string.Format(CultureInfo.InvariantCulture, "Здание №{0}", 
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                else
                    Text = @"Здания отсутствуют";
        }

        protected override Entity EntityFromViewport()
        {
            var building = new Building
            {
                IdBuilding =
                    GeneralBindingSource.Position == -1 ? null : 
                    ViewportHelper.ValueOrNull<int>(
                            (DataRowView) GeneralBindingSource[GeneralBindingSource.Position], "id_building"),
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
                Plumbing = checkBoxPlumbing.Checked,
                HotWaterSupply = checkBoxHotWaterSupply.Checked,
                Canalization = checkBoxCanalization.Checked,
                Electricity = checkBoxElectricity.Checked,
                RadioNetwork = checkBoxRadioNetwork.Checked,
                IdHeatingType =ViewportHelper.ValueOrNull<int>(comboBoxHeatingType),
                RoomsBTI = ViewportHelper.ValueOrNull(textBoxRoomsBTI),
                HousingCooperative = ViewportHelper.ValueOrNull(textBoxHousingCooperative)
            };
            return building;
        }

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return BuildingConverter.FromRow(row);
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
            textBoxHouse.Text = building.House;
            textBoxCadastralNum.Text = building.CadastralNum;
            textBoxDescription.Text = building.Description;
            dateTimePickerStateDate.Value = ViewportHelper.ValueOrDefault(building.StateDate);
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
            dataGridViewOwnerships.AutoGenerateColumns = false;
            dataGridViewRestrictions.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance<BuildingsDataModel>();
            DataModel.GetInstance<KladrStreetsDataModel>().Select();
            DataModel.GetInstance<StructureTypesDataModel>().Select();
            DataModel.GetInstance<HeatingTypesDataModel>().Select();
            _restrictions = DataModel.GetInstance<RestrictionsDataModel>();
            DataModel.GetInstance<RestrictionTypesDataModel>().Select();
            _restrictionBuildingsAssoc = DataModel.GetInstance<RestrictionsBuildingsAssocDataModel>();
            _ownershipRights = DataModel.GetInstance<OwnershipsRightsDataModel>();
            DataModel.GetInstance<OwnershipRightTypesDataModel>().Select();
            _ownershipBuildingsAssoc = DataModel.GetInstance<OwnershipBuildingsAssocDataModel>();
            DataModel.GetInstance<FundTypesDataModel>().Select();
            DataModel.GetInstance<ObjectStatesDataModel>().Select();

            //Вычисляемые модели
            _buildingsPremisesFunds = CalcDataModel.GetInstance<CalcDataModelBuildingsPremisesFunds>();
            _buildingsCurrentFund = CalcDataModel.GetInstance<CalcDataModelBuildingsCurrentFunds>();
            _buildingsPremisesSumArea = CalcDataModel.GetInstance<CalcDataModelBuildingsPremisesSumArea>();
            _municipalPremises = CalcDataModel.GetInstance<CalcDataModelMunicipalPremises>();            

            //Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _restrictions.Select();
            _restrictionBuildingsAssoc.Select();
            _ownershipRights.Select();
            _ownershipBuildingsAssoc.Select();
            _buildingsPremisesFunds.Select();
            _municipalPremises.Select();
            _buildingsPremisesSumArea.Select();

            var ds = DataModel.DataSet;

            _vKladr = new BindingSource
            {
                DataMember = "kladr",
                DataSource = ds
            };

            _vStructureTypes = new BindingSource
            {
                DataMember = "structure_types",
                DataSource = ds
            };

            _vRestrictions = new BindingSource
            {
                DataMember = "restrictions",
                DataSource = ds,
                Sort = "date"
            };

            _vOwnershipRights = new BindingSource
            {
                DataMember = "ownership_rights",
                DataSource = ds,
                Sort = "date"
            };

            _vRestrictonTypes = new BindingSource
            {
                DataMember = "restriction_types",
                DataSource = ds
            };

            _vOwnershipRightTypes = new BindingSource
            {
                DataMember = "ownership_right_types",
                DataSource = ds
            };

            _vHeatingType = new BindingSource
            {
                DataMember = "heating_type",
                DataSource = ds
            };

            _vBuildingCurrentFund = new BindingSource
            {
                DataMember = "buildings_current_funds",
                DataSource = _buildingsCurrentFund.Select()
            };

            _vFundType = new BindingSource
            {
                DataMember = "fund_types",
                DataSource = ds
            };

            _vObjectStates = new BindingSource
            {
                DataMember = "object_states",
                DataSource = ds
            };

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", v_building_CurrentItemChanged);
            GeneralBindingSource.DataMember = "buildings";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", BuildingViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", BuildingViewport_RowChanged);

            _vRestrictionBuildingsAssoc = new BindingSource();
            AddEventHandler<EventArgs>(_vRestrictionBuildingsAssoc, "CurrentItemChanged", v_restrictionBuildingsAssoc_CurrentItemChanged);
            _vRestrictionBuildingsAssoc.DataMember = "buildings_restrictions_buildings_assoc";
            _vRestrictionBuildingsAssoc.DataSource = GeneralBindingSource;
            RestrictionsFilterRebuild();
            AddEventHandler<DataRowChangeEventArgs>(_restrictionBuildingsAssoc.Select(), "RowDeleted", RestrictionsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_restrictionBuildingsAssoc.Select(), "RowChanged", RestrictionsAssoc_RowDeleted);

            _vOwnershipBuildingsAssoc = new BindingSource();
            AddEventHandler<EventArgs>(_vOwnershipBuildingsAssoc, "CurrentItemChanged", v_ownershipBuildingsAssoc_CurrentItemChanged);
            _vOwnershipBuildingsAssoc.DataMember = "buildings_ownership_buildings_assoc";
            _vOwnershipBuildingsAssoc.DataSource = GeneralBindingSource;
            v_ownershipBuildingsAssoc_CurrentItemChanged(null, new EventArgs());
            OwnershipsFilterRebuild();
            AddEventHandler<DataRowChangeEventArgs>(_ownershipBuildingsAssoc.Select(), "RowDeleted", OwnershipsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_ownershipBuildingsAssoc.Select(), "RowChanged", OwnershipsAssoc_RowDeleted);

            DataBind();

            AddEventHandler<EventArgs>(_buildingsCurrentFund, "RefreshEvent", buildingsCurrentFund_RefreshEvent);
            AddEventHandler<EventArgs>(_buildingsPremisesFunds, "RefreshEvent", buildingsPremisesFunds_RefreshEvent);
            AddEventHandler<EventArgs>(_buildingsPremisesSumArea, "RefreshEvent", buildingsPremisesSumArea_RefreshEvent);
            FiltersRebuild();
            SetViewportCaption();
            DataChangeHandlersInit();
        }
        
        public override bool CanSearchRecord()
        {
            return true;
        }

        public override void ClearSearch()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            GeneralBindingSource.Filter = StaticFilter;
            is_editable = true;
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
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            is_editable = false;
            GeneralBindingSource.Filter = filter;
            is_editable = true;
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            var building = (Building) EntityFromViewport();
            if (!ValidateBuilding(building))
                return;
            is_editable = false;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    InsertRecord(building);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    UpdateRecord(building);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            ShowOrHideCurrentFund();
        }

        private void InsertRecord(Building building)
        {
            var idBuilding = GeneralDataModel.Insert(building);
            if (idBuilding == -1)
            {
                return;
            }
            DataRowView newRow;
            building.IdBuilding = idBuilding;
            RebuildFilterAfterSave(GeneralBindingSource, building.IdBuilding);
            if (GeneralBindingSource.Position == -1)
                newRow = (DataRowView) GeneralBindingSource.AddNew();
            else
                newRow = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            BuildingConverter.FillRow(building, newRow);
        }

        private void UpdateRecord(Building building)
        {
            if (building.IdBuilding == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить здание без внутренного номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (GeneralDataModel.Update(building) == -1)
                return;
            RebuildFilterAfterSave(GeneralBindingSource, building.IdBuilding);
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            BuildingConverter.FillRow(building, row);
        }

        private void RebuildFilterAfterSave(IBindingListView bindingSource, int? idBuilding)
        {
            var filter = "";
            if (!string.IsNullOrEmpty(bindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            filter += string.Format(CultureInfo.CurrentCulture, "(id_building = {0})", idBuilding);
            bindingSource.Filter += filter;
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        is_editable = false;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                    }
                    else
                        Text = @"Здания отсутствуют";
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    _vKladr.Filter = "";
                    is_editable = false;
                    DataBind();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            GeneralBindingSource.AddNew();
            is_editable = true;
            GeneralDataModel.EditingNewRecord = true;
            UnbindedCheckBoxesUpdate();
        }

        public override bool CanCopyRecord()
        {
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var building = (Building) EntityFromView();
            GeneralBindingSource.AddNew();
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromBuilding(building);
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (viewportState != ViewportState.NewRowState) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
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
                is_editable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
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
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание для отображения истории найма", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, 
                "id_building = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Building);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            base.OnClosing(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            textBoxHouse.Focus();           
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
                UnbindedCheckBoxesUpdate();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void BuildingViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            ShowOrHideCurrentFund();
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        private void RestrictionsAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                RestrictionsFilterRebuild();
        }

        private void RestrictionsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                RestrictionsFilterRebuild();
        }

        private void OwnershipsAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                OwnershipsFilterRebuild();
        }

        private void OwnershipsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                OwnershipsFilterRebuild();
        }

        private void v_building_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            FiltersRebuild();
            _vKladr.Filter = "";
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            if (GeneralBindingSource.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        private void v_ownershipBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            OwnershipsFilterRebuild();
        }

        private void v_restrictionBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            RestrictionsFilterRebuild();
        }

        private void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxStreet.Text;
                var selectionStart = comboBoxStreet.SelectionStart;
                var selectionLength = comboBoxStreet.SelectionLength;
                _vKladr.Filter = "street_name like '%" + comboBoxStreet.Text + "%'";
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
                    comboBoxStreet.SelectedValue = _vKladr[_vKladr.Position];
                comboBoxStreet.Text = ((DataRowView)_vKladr[_vKladr.Position])["street_name"].ToString();
            }
            if (comboBoxStreet.SelectedValue == null)
                comboBoxStreet.Text = "";
        }

        private void dataGridViewRestrictions_Resize(object sender, EventArgs e)
        {
            if (dataGridViewRestrictions.Size.Width > 600)
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
            if (_restrictions.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок реквизитов уже находится в режиме добавления новой записи. " +
                    @"Одновременно можно добавлять не более одного реквизита.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
                editor.ShowDialog();
            }
        }

        private void vButtonRestrictionDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vRestrictions.Position == -1)
            {
                MessageBox.Show(@"Не выбран реквизит для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить этот реквизит?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var idRestriction = (int)((DataRowView)_vRestrictions[_vRestrictions.Position])["id_restriction"];
            if (_restrictions.Delete(idRestriction) == -1)
                return;
            _restrictions.Select().Rows.Find(idRestriction).Delete();
        }

        private void vButtonRestrictionEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vRestrictions.Position == -1)
            {
                MessageBox.Show(@"Не выбран реквизит для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var restriction = new Restriction();
            var row = (DataRowView)_vRestrictions[_vRestrictions.Position];
            restriction.IdRestriction = (int?)row["id_restriction"];
            restriction.IdRestrictionType = (int?)row["id_restriction_type"];
            restriction.Number = row["number"].ToString();
            restriction.Date = (DateTime?)row["date"];
            restriction.Description = row["description"].ToString();
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
                editor.RestrictionValue = restriction;
                editor.ShowDialog();
            }
        }

        private void vButtonOwnershipAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (_ownershipRights.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок ограничений уже находится в режиме добавления новой записи. " +
                    @"Одновременно можно добавлять не более одного ограничения.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
                editor.ShowDialog();
            }
        }

        private void vButtonOwnershipDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vOwnershipRights.Position == -1)
            {
                MessageBox.Show(@"Не выбрано ограничение для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить это ограничение?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var idOwnershipRight = (int)((DataRowView)_vOwnershipRights[_vOwnershipRights.Position])["id_ownership_right"];
            if (_ownershipRights.Delete(idOwnershipRight) == -1)
                return;
            _ownershipRights.Select().Rows.Find(idOwnershipRight).Delete();
        }

        private void vButtonOwnershipEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vOwnershipRights.Position == -1)
            {
                MessageBox.Show(@"Не выбрано ограничение для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var ownershipRight = new OwnershipRight();
            var row = (DataRowView)_vOwnershipRights[_vOwnershipRights.Position];
            ownershipRight.IdOwnershipRight = (int?)row["id_ownership_right"];
            ownershipRight.IdOwnershipRightType = (int?)row["id_ownership_right_type"];
            ownershipRight.Number = row["number"].ToString();
            ownershipRight.Date = (DateTime?)row["date"];
            ownershipRight.Description = row["description"].ToString();
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
                editor.OwnershipRightValue = ownershipRight;
                editor.ShowDialog();
            }
        }

        internal int GetCurrentId()
        {
            if (GeneralBindingSource.Position < 0) return -1;
            if (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] != DBNull.Value)
                return (int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"];
            return -1;
        }
       
    }
}
