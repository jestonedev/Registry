using System;
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
using Registry.SearchForms;
using Security;
using VIBlend.Utilities;
using VIBlend.WinForms.Controls;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class BuildingViewport : FormViewport
    {
        #region Models
        private DataModel kladr;
        private DataModel structureTypes;
        private DataModel restrictions;
        private DataModel restrictionTypes;
        private DataModel restrictionBuildingsAssoc;
        private DataModel ownershipRights;
        private DataModel ownershipRightTypes;
        private DataModel ownershipBuildingsAssoc;
        private DataModel fundTypes;
        private DataModel object_states;
        private CalcDataModel buildingsPremisesFunds;
        private CalcDataModel buildingsCurrentFund;
        private CalcDataModel buildingsPremisesSumArea;
        #endregion Models

        #region Views
        private BindingSource v_kladr = null;
        private BindingSource v_structureTypes = null;
        private BindingSource v_restrictions = null;
        private BindingSource v_restrictonTypes = null;
        private BindingSource v_restrictionBuildingsAssoc = null;
        private BindingSource v_ownershipRights = null;
        private BindingSource v_ownershipRightTypes = null;
        private BindingSource v_ownershipBuildingsAssoc = null;
        private BindingSource v_fundType = null;
        private BindingSource v_object_states = null;
        private BindingSource v_buildingPremisesFunds = null;
        private BindingSource v_buildingCurrentFund = null;
        private BindingSource v_buildingPremisesSumArea = null;
        #endregion Views

        //Forms
        private SearchForm sbSimpleSearchForm = null;
        private SearchForm sbExtendedSearchForm = null;
        
        //State
        private CheckBox checkBoxRubbishChute;

        private BuildingViewport()
            : this(null)
        {
        }

        public BuildingViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
        }

        public BuildingViewport(BuildingViewport buildingListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = buildingListViewport.DynamicFilter;
            this.StaticFilter = buildingListViewport.StaticFilter;
            this.ParentRow = buildingListViewport.ParentRow;
            this.ParentType = buildingListViewport.ParentType;
        }

        private void RestrictionsFilterRebuild()
        {
            string restrictionsFilter = "id_restriction IN (0";
            for (int i = 0; i < v_restrictionBuildingsAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)v_restrictionBuildingsAssoc[i])["id_restriction"].ToString() + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(new char[] { ',' });
            restrictionsFilter += ")";
            v_restrictions.Filter = restrictionsFilter;
        }

        private void OwnershipsFilterRebuild()
        {
            string ownershipFilter = "id_ownership_right IN (0";
            for (int i = 0; i < v_ownershipBuildingsAssoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownershipBuildingsAssoc[i])["id_ownership_right"].ToString() + ",";
            ownershipFilter = ownershipFilter.TrimEnd(new char[] { ',' });
            ownershipFilter += ")";
            v_ownershipRights.Filter = ownershipFilter;
        }

        private void FiltersRebuild()
        {
            if (v_buildingPremisesFunds != null)
            {
                var position = -1;
                if ((GeneralBindingSource.Position != -1) && !(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] is DBNull))
                    position =
                        v_buildingPremisesFunds.Find("id_building", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                if (position != -1)
                {
                    decimal social_count = Convert.ToDecimal(((DataRowView)v_buildingPremisesFunds[position])["social_premises_count"], CultureInfo.InvariantCulture);
                    decimal special_count = Convert.ToDecimal(((DataRowView)v_buildingPremisesFunds[position])["special_premises_count"], CultureInfo.InvariantCulture);
                    decimal commercial_count = Convert.ToDecimal(((DataRowView)v_buildingPremisesFunds[position])["commercial_premises_count"], CultureInfo.InvariantCulture);
                    decimal other_count = Convert.ToDecimal(((DataRowView)v_buildingPremisesFunds[position])["other_premises_count"], CultureInfo.InvariantCulture);
                    numericUpDownSocialPremisesCount.Minimum = social_count;
                    numericUpDownSpecialPremisesCount.Minimum = special_count;
                    numericUpDownOtherPremisesCount.Minimum = other_count;
                    numericUpDownCommercialPremisesCount.Minimum = commercial_count;
                    numericUpDownSocialPremisesCount.Maximum = social_count;
                    numericUpDownSpecialPremisesCount.Maximum = special_count;
                    numericUpDownOtherPremisesCount.Maximum = other_count;
                    numericUpDownCommercialPremisesCount.Maximum = commercial_count;
                    numericUpDownSocialPremisesCount.Value = social_count;
                    numericUpDownSpecialPremisesCount.Value = special_count;
                    numericUpDownOtherPremisesCount.Value = other_count;
                    numericUpDownCommercialPremisesCount.Value = commercial_count;
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
            if (v_buildingCurrentFund != null)
            {
                int position = -1;
                if ((GeneralBindingSource.Position != -1) && !(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] is DBNull))
                    position =
                        v_buildingCurrentFund.Find("id_building", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                if (position != -1)
                    comboBoxCurrentFundType.SelectedValue = ((DataRowView)v_buildingCurrentFund[position])["id_fund_type"];
                else
                    comboBoxCurrentFundType.SelectedValue = DBNull.Value;
                ShowOrHideCurrentFund();
            }
            if (v_buildingPremisesSumArea != null)
            {
                int position = -1;
                if ((GeneralBindingSource.Position != -1) && !(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] is DBNull))
                    position = v_buildingPremisesSumArea.Find("id_building", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                if (position != -1)
                {
                    decimal value = Convert.ToDecimal((double)((DataRowView)v_buildingPremisesSumArea[position])["sum_area"], CultureInfo.InvariantCulture);
                    numericUpDownMunicipalArea.Minimum = value;
                    numericUpDownMunicipalArea.Maximum = value;
                    numericUpDownMunicipalArea.Value = value;
                }
                else
                {
                    numericUpDownMunicipalArea.Maximum = 0;
                    numericUpDownMunicipalArea.Minimum = 0;
                    numericUpDownMunicipalArea.Value = 0;
                }
            }
        }

        private void ShowOrHideCurrentFund()
        {
            if (comboBoxCurrentFundType.SelectedValue != null && GeneralBindingSource.Position != -1 &&
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state"] != DBNull.Value &&
                (new int[] { 1, 4, 5, 9 }).Contains((int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state"]))
            {
                label19.Visible = true;
                comboBoxCurrentFundType.Visible = true;
                checkBoxRubbishChute.Location = new Point(331, 177);
                checkBoxImprovement.Location = new Point(175, 177);
                checkBoxElevator.Location = new Point(19, 177);
                this.tableLayoutPanel.RowStyles[0].Height = 235F;
            }
            else
            {
                label19.Visible = false;
                comboBoxCurrentFundType.Visible = false; 
                checkBoxRubbishChute.Location = new Point(331, 151);
                checkBoxImprovement.Location = new Point(175, 151);
                checkBoxElevator.Location = new Point(19, 151);
                this.tableLayoutPanel.RowStyles[0].Height = 210F;
            }
        }

        private void UnbindedCheckBoxesUpdate()
        {
            DataRowView row = (GeneralBindingSource.Position >= 0) ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if ((GeneralBindingSource.Position >= 0) && (row["state_date"] != DBNull.Value))
                dateTimePickerStateDate.Checked = true;
            else
            {
                dateTimePickerStateDate.Value = DateTime.Now.Date;
                dateTimePickerStateDate.Checked = false;
            }
        }

        private void DataBind()
        {
            comboBoxStreet.DataSource = v_kladr;
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

            comboBoxStructureType.DataSource = v_structureTypes;
            comboBoxStructureType.ValueMember = "id_structure_type";
            comboBoxStructureType.DisplayMember = "structure_type";
            comboBoxStructureType.DataBindings.Clear();
            comboBoxStructureType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_structure_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxCurrentFundType.DataSource = v_fundType;
            comboBoxCurrentFundType.ValueMember = "id_fund_type";
            comboBoxCurrentFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = v_object_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";
            comboBoxState.DataBindings.Clear();
            comboBoxState.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_state", true, DataSourceUpdateMode.Never, DBNull.Value);

            dataGridViewRestrictions.DataSource = v_restrictions;
            id_restriction_type.DataSource = v_restrictonTypes;
            id_restriction_type.DataPropertyName = "id_restriction_type";
            id_restriction_type.ValueMember = "id_restriction_type";
            id_restriction_type.DisplayMember = "restriction_type";
            restriction_number.DataPropertyName = "number";
            restriction_date.DataPropertyName = "date";
            restriction_description.DataPropertyName = "description";

            dataGridViewOwnerships.DataSource = v_ownershipRights;
            id_ownership_type.DataSource = v_ownershipRightTypes;
            id_ownership_type.DataPropertyName = "id_ownership_right_type";
            id_ownership_type.ValueMember = "id_ownership_right_type";
            id_ownership_type.DisplayMember = "ownership_right_type";
            ownership_number.DataPropertyName = "number";
            ownership_date.DataPropertyName = "date";
            ownership_description.DataPropertyName = "description";
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (!(AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))))
            {
                viewportState = ViewportState.ReadState;
                return true;
            }
            switch (state)
            {
                case ViewportState.ReadState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            return true;
                        case ViewportState.NewRowState:
                        case ViewportState.ModifyRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else return false;
                            if (viewportState == ViewportState.ReadState)
                                return true;
                            else
                                return false;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            if (GeneralDataModel.EditingNewRecord)
                                return false;
                            viewportState = ViewportState.NewRowState;
                            return true;
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.NewRowState);
                            else
                                return false;
                    }
                    break;
                case ViewportState.ModifyRowState: ;
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.ModifyRowState);
                            else
                                return false;
                    }
                    break;
            }
            return false;
        }

        private bool ValidateBuilding(Building building)
        {
            if (building.IdStreet == null)
            {
                MessageBox.Show("Необходимо выбрать улицу", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return false;
            }
            if (building.IdState == null)
            {
                MessageBox.Show("Необходимо выбрать состояние здания", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxState.Focus();
                return false;
            }
            if ((building.House == null) || String.IsNullOrEmpty(building.House.Trim()))
            {
                MessageBox.Show("Необходимо указать номер дома", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return false;
            }
            if (!Regex.IsMatch(building.House, @"^[0-9]+[а-яА-Я]{0,1}([\/][0-9]+[а-яА-Я]{0,1}){0,1}$"))
            {
                MessageBox.Show("Некорректно задан номер дома. Можно использовать только цифры, строчные и прописные буквы кириллицы буквы и дробный разделитель. Например: \"11а/3\"", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return false;
            }
            if (building.IdStructureType == null)
            {
                MessageBox.Show("Необходимо выбрать материал здания", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStructureType.Focus();
                return false;
            }
            // Проверяем права на модификацию муниципального или не муниципального здания
            var buildingFromView = (Building)EntityFromView();
            if (buildingFromView.IdBuilding != null && DataModelHelper.HasMunicipal(buildingFromView.IdBuilding.Value, EntityType.Building)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("Вы не можете изменить информацию по данному зданию, т.к. оно является муниципальным или содержит в себе муниципальные помещения", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (buildingFromView.IdBuilding != null && DataModelHelper.HasNotMunicipal(buildingFromView.IdBuilding.Value, EntityType.Building)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("Вы не можете изменить информацию по данному зданию, т.к. оно является немуниципальным или содержит в себе немуниципальные помещения",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new int[] { 4, 5, 9 }.Contains(building.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу муниципальных жилых зданий", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new int[] { 1, 3, 6, 7, 8 }.Contains(building.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу немуниципальных жилых зданий", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            // Подверждение на износ выше 100%
            if (building.Wear > 100)
                if (MessageBox.Show("Вы задали износ здания выше 100%. Все равно продолжить сохранение?", "Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                {
                    numericUpDownWear.Focus();
                    return false;
                }
            // Проверяем дубликаты адресов домов
            if ((building.House != buildingFromView.House) || (building.IdStreet != buildingFromView.IdStreet))
                if (DataModelHelper.BuildingsDuplicateCount(building) != 0 &&
                    MessageBox.Show("В базе уже имеется здание с таким адресом. Все равно продолжить сохранение?", "Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
            return true;
        }

        public void LocateBuildingBy(int id)
        {
            int Position = GeneralBindingSource.Find("id_building", id);
            is_editable = false;
            if (Position > 0)
                GeneralBindingSource.Position = Position;
            is_editable = true;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
                this.Text = "Новое здание";
            else
                if (GeneralBindingSource.Position != -1)
                    this.Text = String.Format(CultureInfo.InvariantCulture, "Здание №{0}", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                else
                    this.Text = "Здания отсутствуют";
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.BuildingsDataModel);
            kladr = DataModel.GetInstance(DataModelType.KladrStreetsDataModel);
            structureTypes = DataModel.GetInstance(DataModelType.StructureTypesDataModel);
            restrictions = DataModel.GetInstance(DataModelType.RestrictionsDataModel);
            restrictionTypes = DataModel.GetInstance(DataModelType.RestrictionTypesDataModel);
            restrictionBuildingsAssoc = DataModel.GetInstance(DataModelType.RestrictionsBuildingsAssocDataModel);
            ownershipRights = DataModel.GetInstance(DataModelType.OwnershipsRightsDataModel);
            ownershipRightTypes = DataModel.GetInstance(DataModelType.OwnershipRightTypesDataModel);
            ownershipBuildingsAssoc = DataModel.GetInstance(DataModelType.OwnershipBuildingsAssocDataModel);
            fundTypes = DataModel.GetInstance(DataModelType.FundTypesDataModel);
            object_states = DataModel.GetInstance(DataModelType.ObjectStatesDataModel);

            //Вычисляемые модели
            buildingsPremisesFunds = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelBuildingsPremisesFunds);
            buildingsCurrentFund = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelBuildingsCurrentFunds);
            buildingsPremisesSumArea = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelBuildingsPremisesSumArea);

            //Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            kladr.Select();
            structureTypes.Select();
            restrictions.Select();
            restrictionTypes.Select();
            restrictionBuildingsAssoc.Select();
            ownershipRights.Select();
            ownershipRightTypes.Select();
            ownershipBuildingsAssoc.Select();
            fundTypes.Select();
            object_states.Select();

            var ds = DataModel.DataSet;

            v_kladr = new BindingSource();
            v_kladr.DataMember = "kladr";
            v_kladr.DataSource = ds;

            v_structureTypes = new BindingSource();
            v_structureTypes.DataMember = "structure_types";
            v_structureTypes.DataSource = ds;

            v_restrictions = new BindingSource();
            v_restrictions.DataMember = "restrictions";
            v_restrictions.DataSource = ds;

            v_ownershipRights = new BindingSource();
            v_ownershipRights.DataMember = "ownership_rights";
            v_ownershipRights.DataSource = ds;

            v_restrictonTypes = new BindingSource();
            v_restrictonTypes.DataMember = "restriction_types";
            v_restrictonTypes.DataSource = ds;

            v_ownershipRightTypes = new BindingSource();
            v_ownershipRightTypes.DataMember = "ownership_right_types";
            v_ownershipRightTypes.DataSource = ds;

            v_buildingPremisesFunds = new BindingSource();
            v_buildingPremisesFunds.DataMember = "buildings_premises_funds";
            v_buildingPremisesFunds.DataSource = buildingsPremisesFunds.Select();

            v_buildingCurrentFund = new BindingSource();
            v_buildingCurrentFund.DataMember = "buildings_current_funds";
            v_buildingCurrentFund.DataSource = buildingsCurrentFund.Select();

            v_buildingPremisesSumArea = new BindingSource();
            v_buildingPremisesSumArea.DataMember = "buildings_premises_sum_area";
            v_buildingPremisesSumArea.DataSource = buildingsPremisesSumArea.Select();

            v_fundType = new BindingSource();
            v_fundType.DataMember = "fund_types";
            v_fundType.DataSource = ds;

            v_object_states = new BindingSource();
            v_object_states.DataMember = "object_states";
            v_object_states.DataSource = ds;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += v_building_CurrentItemChanged;
            GeneralBindingSource.DataMember = "buildings";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralDataModel.Select().RowDeleted += BuildingViewport_RowDeleted;
            GeneralDataModel.Select().RowChanged += BuildingViewport_RowChanged;

            v_restrictionBuildingsAssoc = new BindingSource();
            v_restrictionBuildingsAssoc.CurrentItemChanged += v_restrictionBuildingsAssoc_CurrentItemChanged;
            v_restrictionBuildingsAssoc.DataMember = "buildings_restrictions_buildings_assoc";
            v_restrictionBuildingsAssoc.DataSource = GeneralBindingSource;
            RestrictionsFilterRebuild();
            restrictionBuildingsAssoc.Select().RowChanged += RestrictionsAssoc_RowChanged;
            restrictionBuildingsAssoc.Select().RowDeleted += RestrictionsAssoc_RowDeleted;

            v_ownershipBuildingsAssoc = new BindingSource();
            v_ownershipBuildingsAssoc.CurrentItemChanged += v_ownershipBuildingsAssoc_CurrentItemChanged;
            v_ownershipBuildingsAssoc.DataMember = "buildings_ownership_buildings_assoc";
            v_ownershipBuildingsAssoc.DataSource = GeneralBindingSource;
            v_ownershipBuildingsAssoc_CurrentItemChanged(null, new EventArgs());
            OwnershipsFilterRebuild();
            ownershipBuildingsAssoc.Select().RowChanged += OwnershipsAssoc_RowChanged;
            ownershipBuildingsAssoc.Select().RowDeleted += OwnershipsAssoc_RowDeleted;

            DataBind();

            buildingsCurrentFund.RefreshEvent += buildingsCurrentFund_RefreshEvent;
            buildingsPremisesFunds.RefreshEvent += buildingsPremisesFunds_RefreshEvent;
            buildingsPremisesSumArea.RefreshEvent += buildingsPremisesSumArea_RefreshEvent; 
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
            if (!String.IsNullOrEmpty(DynamicFilter))
                return true;
            else
                return false;
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (sbSimpleSearchForm == null)
                        sbSimpleSearchForm = new SimpleSearchBuildingForm();
                    if (sbSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = sbSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (sbExtendedSearchForm == null)
                        sbExtendedSearchForm = new ExtendedSearchBuildingForm();
                    if (sbExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = sbExtendedSearchForm.GetFilter();
                    break;
            }
            string Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            is_editable = false;
            GeneralBindingSource.Filter = Filter;
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
            string Filter = "";
            if (!String.IsNullOrEmpty(GeneralBindingSource.Filter))
                Filter += " OR ";
            else
                Filter += "(1 = 1) OR ";
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    int id_building = GeneralDataModel.Insert(building);
                    if (id_building == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    building.IdBuilding = id_building;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    Filter += String.Format(CultureInfo.CurrentCulture, "(id_building = {0})", building.IdBuilding);
                    GeneralBindingSource.Filter += Filter;
                    FillRowFromBuilding(building, newRow);
                    Text = @"Здание №" + id_building.ToString(CultureInfo.InvariantCulture);
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
                        dialogResult = MessageBox.Show(@"Хотите ли вы оставить состояние помещений, расположенным в здании, неизмененным?" +
                                        @" Если вы нажмете ""Нет"", состояние здания применится ко всем, расположенным в нем, помещениям", @"Внимание",
                                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    if (dialogResult == DialogResult.Cancel)
                        return;
                    if (GeneralDataModel.Update(building) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    is_editable = false;
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_building = {0})", building.IdBuilding);
                    GeneralBindingSource.Filter += Filter;
                    FillRowFromBuilding(building, row);
                    if (dialogResult == DialogResult.No)
                    {
                        var premises = from premisesRow in DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows()
                                       where premisesRow.Field<int>("id_building") == building.IdBuilding
                                       select premisesRow;
                        foreach (var premiseRow in premises)
                        {
                            var premise = PremiseFromDataRow(premiseRow);
                            premise.IdState = building.IdState;
                            premise.StateDate = building.StateDate;
                            if (DataModel.GetInstance(DataModelType.PremisesDataModel).Update(premise) == -1)
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
                        this.Text = "Здания отсутствуют";
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    v_kladr.Filter = "";
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
            if (MessageBox.Show(@"Вы действительно хотите удалить это здание?", "Внимание",
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

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            BuildingViewport viewport = new BuildingViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (GeneralBindingSource.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool HasAssocPremises()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocOwnerships()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (GeneralBindingSource.Position > -1);
        }
        
        public override bool HasAssocTenancies()
        {
            return (GeneralBindingSource.Position > -1);
        }

        public override void ShowPremises()
        {
            ShowAssocViewport(ViewportType.PremisesListViewport);
        }

        public override void ShowOwnerships()
        {
            ShowAssocViewport(ViewportType.OwnershipListViewport);
        }

        public override void ShowRestrictions()
        {
            ShowAssocViewport(ViewportType.RestrictionListViewport);
        }

        public override void ShowFundHistory()
        {         
            ShowAssocViewport(ViewportType.FundsHistoryViewport);
        }

        public override void ShowTenancies()
        {
            ShowAssocViewport(ViewportType.TenancyListViewport);
        }

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано здание", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_building = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Building);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                restrictionBuildingsAssoc.Select().RowChanged -= RestrictionsAssoc_RowChanged;
                restrictionBuildingsAssoc.Select().RowDeleted -= RestrictionsAssoc_RowDeleted;
                ownershipBuildingsAssoc.Select().RowChanged -= OwnershipsAssoc_RowChanged;
                ownershipBuildingsAssoc.Select().RowDeleted -= OwnershipsAssoc_RowDeleted;
                buildingsCurrentFund.RefreshEvent -= buildingsCurrentFund_RefreshEvent;
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
            v_kladr.Filter = "";
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
                string text = comboBoxStreet.Text;
                int selectionStart = comboBoxStreet.SelectionStart;
                int selectionLength = comboBoxStreet.SelectionLength;
                v_kladr.Filter = "street_name like '%" + comboBoxStreet.Text + "%'";
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
                    comboBoxStreet.SelectedValue = v_kladr[v_kladr.Position];
                comboBoxStreet.Text = ((DataRowView)v_kladr[v_kladr.Position])["street_name"].ToString();
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
            if (HasAssocRestrictions())
                ShowRestrictions();
        }

        private void dataGridViewOwnerships_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocOwnerships())
                ShowOwnerships();
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
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано здание", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (RestrictionsEditor editor = new RestrictionsEditor())
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
            if (v_restrictions.Position == -1)
            {
                MessageBox.Show(@"Не выбран реквизит для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить этот реквизит?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            int idRestriction = (int)((DataRowView)v_restrictions[v_restrictions.Position])["id_restriction"];
            if (restrictions.Delete(idRestriction) == -1)
                return;
            restrictions.Select().Rows.Find(idRestriction).Delete();
        }

        private void vButtonRestrictionEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано здание", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_restrictions.Position == -1)
            {
                MessageBox.Show("Не выбран реквизит для редактирования", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            Restriction restriction = new Restriction();
            DataRowView row = (DataRowView)v_restrictions[v_restrictions.Position];
            restriction.IdRestriction = (int?)row["id_restriction"];
            restriction.IdRestrictionType = (int?)row["id_restriction_type"];
            restriction.Number = row["number"].ToString();
            restriction.Date = (DateTime?)row["date"];
            restriction.Description = row["description"].ToString();
            using (RestrictionsEditor editor = new RestrictionsEditor())
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
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано здание", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (OwnershipsEditor editor = new OwnershipsEditor())
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
            if (v_ownershipRights.Position == -1)
            {
                MessageBox.Show(@"Не выбрано ограничение для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить это ограничение?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var idOwnershipRight = (int)((DataRowView)v_ownershipRights[v_ownershipRights.Position])["id_ownership_right"];
            if (ownershipRights.Delete(idOwnershipRight) == -1)
                return;
            ownershipRights.Select().Rows.Find(idOwnershipRight).Delete();
        }

        private void vButtonOwnershipEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано здание", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_ownershipRights.Position == -1)
            {
                MessageBox.Show("Не выбрано ограничение для редактирования", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            OwnershipRight ownershipRight = new OwnershipRight();
            DataRowView row = (DataRowView)v_ownershipRights[v_ownershipRights.Position];
            ownershipRight.IdOwnershipRight = (int?)row["id_ownership_right"];
            ownershipRight.IdOwnershipRightType = (int?)row["id_ownership_right_type"];
            ownershipRight.Number = row["number"].ToString();
            ownershipRight.Date = (DateTime?)row["date"];
            ownershipRight.Description = row["description"].ToString();
            using (OwnershipsEditor editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
                editor.OwnershipRightValue = ownershipRight;
                editor.ShowDialog();
            }
        }
    }
}
