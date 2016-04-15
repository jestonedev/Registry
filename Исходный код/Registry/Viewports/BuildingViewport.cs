using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class BuildingViewport : FormViewport
    {
        #region Models
        private DataModel _kladr;
        private DataModel _structureTypes;
        private DataModel _restrictions;
        private DataModel _restrictionTypes;
        private DataModel _restrictionBuildingsAssoc;
        private DataModel _ownershipRights;
        private DataModel _ownershipRightTypes;
        private DataModel _ownershipBuildingsAssoc;
        private DataModel _fundTypes;
        private DataModel _objectStates;
        private DataModel _buildings;
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
        private BindingSource _vBuildingPremisesFunds;
        private BindingSource _vBuildingCurrentFund;
        private BindingSource _vBuildingPremisesSumArea;
        private BindingSource _vMunicipalPremises;
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
                restrictionsFilter += ((DataRowView)_vRestrictionBuildingsAssoc[i])["id_restriction"].ToString() + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(',');
            restrictionsFilter += ")";
            _vRestrictions.Filter = restrictionsFilter;
        }

        private void OwnershipsFilterRebuild()
        {
            var ownershipFilter = "id_ownership_right IN (0";
            for (var i = 0; i < _vOwnershipBuildingsAssoc.Count; i++)
                ownershipFilter += ((DataRowView)_vOwnershipBuildingsAssoc[i])["id_ownership_right"].ToString() + ",";
            ownershipFilter = ownershipFilter.TrimEnd(',');
            ownershipFilter += ")";
            _vOwnershipRights.Filter = ownershipFilter;
        }

        private void FiltersRebuild()
        {
            var id_building = (int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"];
            if (_vBuildingPremisesFunds != null)
            {
                var position = -1;                
                if ((GeneralBindingSource.Position != -1) && !(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] is DBNull))
                    position =
                        _vBuildingPremisesFunds.Find("id_building", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                if (position != -1)
                {                    
                    //var socialCount = Convert.ToDecimal(((DataRowView)_vBuildingPremisesFunds[position])["social_premises_count"], CultureInfo.InvariantCulture);
                    //var specialCount = Convert.ToDecimal(((DataRowView)_vBuildingPremisesFunds[position])["special_premises_count"], CultureInfo.InvariantCulture);
                    //var commercialCount = Convert.ToDecimal(((DataRowView)_vBuildingPremisesFunds[position])["commercial_premises_count"], CultureInfo.InvariantCulture);
                    //var otherCount = Convert.ToDecimal(((DataRowView)_vBuildingPremisesFunds[position])["other_premises_count"], CultureInfo.InvariantCulture);
                    var socialCount = _municipalPremises.Select().AsEnumerable().Where(s => s.Field<int>("id_building") == id_building && s.Field<int>("id_fund_type") == 1).Count();
                    var commercialCount = _municipalPremises.Select().AsEnumerable().Where(s => s.Field<int>("id_building") == id_building && s.Field<int>("id_fund_type") == 2).Count();
                    var specialCount = _municipalPremises.Select().AsEnumerable().Where(s => s.Field<int>("id_building") == id_building && s.Field<int>("id_fund_type") == 3).Count();                   
                    var otherCount = _municipalPremises.Select().AsEnumerable().Where(s => s.Field<int>("id_building") == id_building && s.Field<int>("id_fund_type") == 4).Count();
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
                }
                else
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
                }
            }
            if (_vBuildingCurrentFund != null)
            {
                var position = -1;
                if ((GeneralBindingSource.Position != -1) && !(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] is DBNull))
                    position =
                        _vBuildingCurrentFund.Find("id_building", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                if (position != -1)
                    comboBoxCurrentFundType.SelectedValue = ((DataRowView)_vBuildingCurrentFund[position])["id_fund_type"];
                else
                    comboBoxCurrentFundType.SelectedValue = DBNull.Value;
                ShowOrHideCurrentFund();
            }
            if (_vBuildingPremisesSumArea != null)
            {
                var position = -1;
                if ((GeneralBindingSource.Position != -1) && !(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] is DBNull))
                    position = _vBuildingPremisesSumArea.Find("id_building", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                if (position != -1)
                {
                    var row = ((DataRowView) _vBuildingPremisesSumArea[position]);
                    var sumArea = Convert.ToDecimal((double)row["sum_area"], CultureInfo.InvariantCulture);
                    var totalMunCount =Convert.ToDecimal(_municipalPremises.Select().AsEnumerable().Where(s => s.Field<int>("id_building") == id_building).Count());
                    //var totalMunCount = Convert.ToDecimal((int)row["mun_premises_count"], CultureInfo.InvariantCulture);
                    //var totalPremisesCount = 
                    //    (from premisesRow in DataModel.GetInstance<PremisesDataModel>().FilterDeletedRows()
                    //    where premisesRow.Field<int>("id_building") == (int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]
                    //    group premisesRow by premisesRow.Field<int>("id_building") into gs
                    //    select gs.Count()).First();
                    var totalPremisesCount =Convert.ToDecimal(_buildings.Select().Rows.Find((int)row["id_building"]).Field<int>("num_apartments"));

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
                    numericUpDownMunicipalArea.Minimum = sumArea;
                    numericUpDownMunicipalArea.Maximum = sumArea;
                    numericUpDownMunicipalArea.Value = sumArea;
                    numericUpDownMunPremisesCount.Minimum = totalMunCount;
                    numericUpDownMunPremisesCount.Maximum = totalMunCount;
                    numericUpDownMunPremisesCount.Value = totalMunCount;
                }
                else
                {
                    numericUpDownMunicipalArea.Maximum = 0;
                    numericUpDownMunicipalArea.Minimum = 0;
                    numericUpDownMunicipalArea.Value = 0;
                    numericUpDownMunPremisesCount.Maximum = 0;
                    numericUpDownMunPremisesCount.Minimum = 0;
                    numericUpDownMunPremisesCount.Value = 0;
                    numericUpDownMunPremisesPercentage.Maximum = 0;
                    numericUpDownMunPremisesPercentage.Minimum = 0;
                    numericUpDownMunPremisesPercentage.Value = 0;
                }
            }
        }

        private void ShowOrHideCurrentFund()
        {
            if (comboBoxCurrentFundType.SelectedValue != null && GeneralBindingSource.Position != -1 &&
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state"] != DBNull.Value &&
                (new[] { 1, 4, 5, 9, 11 }).Contains((int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state"]))
            {
                label19.Visible = true;
                comboBoxCurrentFundType.Visible = true;
                checkBoxRubbishChute.Location = new Point(331, 152);
                checkBoxImprovement.Location = new Point(175, 152);
                checkBoxElevator.Location = new Point(19, 152);
            }
            else
            {
                label19.Visible = false;
                comboBoxCurrentFundType.Visible = false;
                checkBoxRubbishChute.Location = new Point(331, 124);
                checkBoxImprovement.Location = new Point(175, 124);
                checkBoxElevator.Location = new Point(19, 124);
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
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
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
            if (new [] { 4, 5, 9, 11 }.Contains(building.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на добавление в базу муниципальных жилых зданий", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new [] { 1, 3, 6, 7, 8, 10 }.Contains(building.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
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
                StateDate = ViewportHelper.ValueOrNull(dateTimePickerStateDate)
            };
            return building;
        }

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            var building = new Building
            {
                IdBuilding = ViewportHelper.ValueOrNull<int>(row, "id_building"),
                IdStreet = ViewportHelper.ValueOrNull(row, "id_street"),
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type"),
                House = ViewportHelper.ValueOrNull(row, "house"),
                Floors = ViewportHelper.ValueOrNull<short>(row, "floors"),
                NumPremises = ViewportHelper.ValueOrNull<int>(row, "num_premises"),
                NumRooms = ViewportHelper.ValueOrNull<int>(row, "num_rooms"),
                NumApartments = ViewportHelper.ValueOrNull<int>(row, "num_apartments"),
                NumSharedApartments = ViewportHelper.ValueOrNull<int>(row, "num_shared_apartments"),
                LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area"),
                TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area"),
                CadastralNum = ViewportHelper.ValueOrNull(row, "cadastral_num"),
                CadastralCost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost"),
                BalanceCost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                StartupYear = ViewportHelper.ValueOrNull<int>(row, "startup_year"),
                Improvement = ViewportHelper.ValueOrNull<bool>(row, "improvement"),
                Elevator = ViewportHelper.ValueOrNull<bool>(row, "elevator"),
                RubbishChute = ViewportHelper.ValueOrNull<bool>(row, "rubbish_chute"),
                Wear = ViewportHelper.ValueOrNull<double>(row, "wear"),
                StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date")
            };
            return building;
        }

        private void ViewportFromBuilding(Building building)
        {
            comboBoxStreet.SelectedValue = ViewportHelper.ValueOrDBNull(building.IdStreet);
            comboBoxState.SelectedValue = ViewportHelper.ValueOrDBNull(building.IdState);
            comboBoxStructureType.SelectedValue = ViewportHelper.ValueOrDBNull(building.IdStructureType);
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
        }

        private static void FillRowFromBuilding(Building building, DataRowView row)
        {
            row.BeginEdit();
            row["id_building"] = ViewportHelper.ValueOrDBNull(building.IdBuilding);
            row["id_state"] = ViewportHelper.ValueOrDBNull(building.IdState);
            row["id_structure_type"] = ViewportHelper.ValueOrDBNull(building.IdStructureType);
            row["id_street"] = ViewportHelper.ValueOrDBNull(building.IdStreet);
            row["house"] = ViewportHelper.ValueOrDBNull(building.House);
            row["floors"] = ViewportHelper.ValueOrDBNull(building.Floors);
            row["num_premises"] = ViewportHelper.ValueOrDBNull(building.NumPremises);
            row["num_rooms"] = ViewportHelper.ValueOrDBNull(building.NumRooms);
            row["num_apartments"] = ViewportHelper.ValueOrDBNull(building.NumApartments);
            row["num_shared_apartments"] = ViewportHelper.ValueOrDBNull(building.NumSharedApartments);
            row["cadastral_num"] = ViewportHelper.ValueOrDBNull(building.CadastralNum);
            row["cadastral_cost"] = ViewportHelper.ValueOrDBNull(building.CadastralCost);
            row["balance_cost"] = ViewportHelper.ValueOrDBNull(building.BalanceCost);
            row["description"] = ViewportHelper.ValueOrDBNull(building.Description);
            row["startup_year"] = ViewportHelper.ValueOrDBNull(building.StartupYear);
            row["improvement"] = ViewportHelper.ValueOrDBNull(building.Improvement);
            row["elevator"] = ViewportHelper.ValueOrDBNull(building.Elevator);
            row["rubbish_chute"] = ViewportHelper.ValueOrDBNull(building.RubbishChute);
            row["living_area"] = ViewportHelper.ValueOrDBNull(building.LivingArea);
            row["total_area"] = ViewportHelper.ValueOrDBNull(building.TotalArea);
            row["wear"] = ViewportHelper.ValueOrDBNull(building.Wear);
            row["state_date"] = ViewportHelper.ValueOrDBNull(building.StateDate);
            row.EndEdit();
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
            _kladr = DataModel.GetInstance<KladrStreetsDataModel>();
            _structureTypes = DataModel.GetInstance<StructureTypesDataModel>();
            _restrictions = DataModel.GetInstance<RestrictionsDataModel>();
            _restrictionTypes = DataModel.GetInstance<RestrictionTypesDataModel>();
            _restrictionBuildingsAssoc = DataModel.GetInstance<RestrictionsBuildingsAssocDataModel>();
            _ownershipRights = DataModel.GetInstance<OwnershipsRightsDataModel>();
            _ownershipRightTypes = DataModel.GetInstance<OwnershipRightTypesDataModel>();
            _ownershipBuildingsAssoc = DataModel.GetInstance<OwnershipBuildingsAssocDataModel>();
            _fundTypes = DataModel.GetInstance<FundTypesDataModel>();
            _objectStates = DataModel.GetInstance<ObjectStatesDataModel>();
            _buildings = DataModel.GetInstance<BuildingsDataModel>();

            //Вычисляемые модели
            _buildingsPremisesFunds = CalcDataModel.GetInstance<CalcDataModelBuildingsPremisesFunds>();
            _buildingsCurrentFund = CalcDataModel.GetInstance<CalcDataModelBuildingsCurrentFunds>();
            _buildingsPremisesSumArea = CalcDataModel.GetInstance<CalcDataModelBuildingsPremisesSumArea>();
            _municipalPremises = CalcDataModel.GetInstance<CalcDataModelMunicipalPremises>();            

            //Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _kladr.Select();
            _structureTypes.Select();
            _restrictions.Select();
            _restrictionTypes.Select();
            _restrictionBuildingsAssoc.Select();
            _ownershipRights.Select();
            _ownershipRightTypes.Select();
            _ownershipBuildingsAssoc.Select();
            _fundTypes.Select();
            _objectStates.Select();
            _buildings.Select();

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

            _vBuildingPremisesFunds = new BindingSource
            {
                DataMember = "buildings_premises_funds",
                DataSource = _buildingsPremisesFunds.Select()
            };

            _vMunicipalPremises = new BindingSource
            {
                DataMember = "municipal_premises_current_funds",
                DataSource = _municipalPremises.Select()
            };

            _vBuildingCurrentFund = new BindingSource
            {
                DataMember = "buildings_current_funds",
                DataSource = _buildingsCurrentFund.Select()
            };

            _vBuildingPremisesSumArea = new BindingSource
            {
                DataMember = "buildings_premises_sum_area",
                DataSource = _buildingsPremisesSumArea.Select()
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
            GeneralBindingSource.CurrentItemChanged += v_building_CurrentItemChanged;
            GeneralBindingSource.DataMember = "buildings";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralDataModel.Select().RowDeleted += BuildingViewport_RowDeleted;
            GeneralDataModel.Select().RowChanged += BuildingViewport_RowChanged;
            
            _vRestrictionBuildingsAssoc = new BindingSource();
            _vRestrictionBuildingsAssoc.CurrentItemChanged += v_restrictionBuildingsAssoc_CurrentItemChanged;
            _vRestrictionBuildingsAssoc.DataMember = "buildings_restrictions_buildings_assoc";
            _vRestrictionBuildingsAssoc.DataSource = GeneralBindingSource;
            RestrictionsFilterRebuild();
            _restrictionBuildingsAssoc.Select().RowChanged += RestrictionsAssoc_RowChanged;
            _restrictionBuildingsAssoc.Select().RowDeleted += RestrictionsAssoc_RowDeleted;

            _vOwnershipBuildingsAssoc = new BindingSource();
            _vOwnershipBuildingsAssoc.CurrentItemChanged += v_ownershipBuildingsAssoc_CurrentItemChanged;
            _vOwnershipBuildingsAssoc.DataMember = "buildings_ownership_buildings_assoc";
            _vOwnershipBuildingsAssoc.DataSource = GeneralBindingSource;
            v_ownershipBuildingsAssoc_CurrentItemChanged(null, new EventArgs());
            OwnershipsFilterRebuild();
            _ownershipBuildingsAssoc.Select().RowChanged += OwnershipsAssoc_RowChanged;
            _ownershipBuildingsAssoc.Select().RowDeleted += OwnershipsAssoc_RowDeleted;

            DataBind();

            _buildingsCurrentFund.RefreshEvent += buildingsCurrentFund_RefreshEvent;
            _buildingsPremisesFunds.RefreshEvent += buildingsPremisesFunds_RefreshEvent;
            _buildingsPremisesSumArea.RefreshEvent += buildingsPremisesSumArea_RefreshEvent; 
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
            var buildingFromView = (Building) EntityFromView();
            if (!ValidateBuilding(building))
                return;
            var filter = "";
            if (!string.IsNullOrEmpty(GeneralBindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idBuilding = GeneralDataModel.Insert(building);
                    if (idBuilding == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    building.IdBuilding = idBuilding;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    filter += string.Format(CultureInfo.CurrentCulture, "(id_building = {0})", building.IdBuilding);
                    GeneralBindingSource.Filter += filter;
                    FillRowFromBuilding(building, newRow);
                    Text = @"Здание №" + idBuilding.ToString(CultureInfo.InvariantCulture);
                    viewportState = ViewportState.ReadState;
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (building.IdBuilding == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить здание без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    var dialogResult = DialogResult.Yes;
                    if (building.IdState != buildingFromView.IdState || building.StateDate != buildingFromView.StateDate)
                        dialogResult = MessageBox.Show(@"Хотите ли вы оставить состояние помещений, расположенным в здании, неизмененным? - нажмите ""Да""." +
                                        @" Если вы нажмете ""Нет"", состояние здания применится ко всем, расположенным в нем, помещениям", @"Внимание",
                                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    if (dialogResult == DialogResult.Cancel)
                        return;
                    if (GeneralDataModel.Update(building) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    is_editable = false;
                    filter += string.Format(CultureInfo.CurrentCulture, "(id_building = {0})", building.IdBuilding);
                    GeneralBindingSource.Filter += filter;
                    FillRowFromBuilding(building, row);
                    if (dialogResult == DialogResult.No)
                    {
                        var premises = from premisesRow in DataModel.GetInstance<PremisesDataModel>().FilterDeletedRows()
                                       where premisesRow.Field<int>("id_building") == building.IdBuilding
                                       select premisesRow;
                        foreach (var premiseRow in premises)
                        {
                            var premise = PremiseFromDataRow(premiseRow);
                            premise.IdState = building.IdState;
                            premise.StateDate = building.StateDate;
                            if (DataModel.GetInstance<PremisesDataModel>().Update(premise) == -1)
                                return;
                        }
                    }
                    viewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            ShowOrHideCurrentFund();
        }

        private static Premise PremiseFromDataRow(DataRow row)
        {
            var premise = new Premise
            {
                IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                IdBuilding = ViewportHelper.ValueOrNull<int>(row, "id_building"),
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                PremisesNum = ViewportHelper.ValueOrNull(row, "premises_num"),
                LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area"),
                TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area"),
                Height = ViewportHelper.ValueOrNull<double>(row, "height"),
                NumRooms = ViewportHelper.ValueOrNull<short>(row, "num_rooms"),
                NumBeds = ViewportHelper.ValueOrNull<short>(row, "num_beds"),
                IdPremisesType = ViewportHelper.ValueOrNull<int>(row, "id_premises_type"),
                IdPremisesKind = ViewportHelper.ValueOrNull<int>(row, "id_premises_kind"),
                Floor = ViewportHelper.ValueOrNull<short>(row, "floor"),
                CadastralNum = ViewportHelper.ValueOrNull(row, "cadastral_num"),
                CadastralCost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost"),
                BalanceCost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                IsMemorial = ViewportHelper.ValueOrNull<bool>(row, "is_memorial"),
                Account = ViewportHelper.ValueOrNull(row, "account"),
                RegDate = ViewportHelper.ValueOrNull<DateTime>(row, "reg_date"),
                StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date")
            };
            return premise;
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
            //return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
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
            else
            {
                _buildingsCurrentFund.RefreshEvent -= buildingsCurrentFund_RefreshEvent;
                _buildingsPremisesFunds.RefreshEvent -= buildingsPremisesFunds_RefreshEvent;
                _buildingsPremisesSumArea.RefreshEvent -= buildingsPremisesSumArea_RefreshEvent; 
                _restrictionBuildingsAssoc.Select().RowChanged -= RestrictionsAssoc_RowChanged;
                _restrictionBuildingsAssoc.Select().RowDeleted -= RestrictionsAssoc_RowDeleted;
                _ownershipBuildingsAssoc.Select().RowChanged -= OwnershipsAssoc_RowChanged;
                _ownershipBuildingsAssoc.Select().RowDeleted -= OwnershipsAssoc_RowDeleted;
                _vRestrictionBuildingsAssoc.CurrentItemChanged -= v_restrictionBuildingsAssoc_CurrentItemChanged;
                _vOwnershipBuildingsAssoc.CurrentItemChanged -= v_ownershipBuildingsAssoc_CurrentItemChanged;
                GeneralBindingSource.CurrentItemChanged -= v_building_CurrentItemChanged;
                GeneralDataModel.Select().RowDeleted -= BuildingViewport_RowDeleted;
                GeneralDataModel.Select().RowChanged -= BuildingViewport_RowChanged;
            }
            base.OnClosing(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            textBoxHouse.Focus();           
            base.OnVisibleChanged(e);
        }

        void buildingsPremisesSumArea_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        void buildingsPremisesFunds_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }
        
        void buildingsCurrentFund_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        void BuildingViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                UnbindedCheckBoxesUpdate();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void BuildingViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            ShowOrHideCurrentFund();
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        void RestrictionsAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                RestrictionsFilterRebuild();
        }

        void RestrictionsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                RestrictionsFilterRebuild();
        }

        void OwnershipsAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                OwnershipsFilterRebuild();
        }

        void OwnershipsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                OwnershipsFilterRebuild();
        }

        void v_building_CurrentItemChanged(object sender, EventArgs e)
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

        void v_ownershipBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            OwnershipsFilterRebuild();
        }

        void v_restrictionBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            RestrictionsFilterRebuild();
        }

        void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
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

        void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
        }

        void comboBoxStreet_Leave(object sender, EventArgs e)
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
                if (dataGridViewRestrictions.Columns["restriction_description"].AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    dataGridViewRestrictions.Columns["restriction_description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (dataGridViewRestrictions.Columns["restriction_description"].AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    dataGridViewRestrictions.Columns["restriction_description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        private void dataGridViewOwnerships_Resize(object sender, EventArgs e)
        {
            if (dataGridViewOwnerships.Size.Width > 600)
            {
                if (dataGridViewOwnerships.Columns["ownership_description"].AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    dataGridViewOwnerships.Columns["ownership_description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (dataGridViewOwnerships.Columns["ownership_description"].AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    dataGridViewOwnerships.Columns["ownership_description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
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
