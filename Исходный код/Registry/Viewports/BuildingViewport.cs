using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.Entities;
using Registry.SearchForms;
using Security;
using VIBlend.Utilities;
using VIBlend.WinForms.Controls;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class BuildingViewport : Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel;
        private TableLayoutPanel tableLayoutPanel2;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private GroupBox groupBox6;
        private GroupBox groupBox7;
        private NumericUpDown numericUpDownFloors;
        private NumericUpDown numericUpDownBalanceCost;
        private NumericUpDown numericUpDownCadastralCost;
        private NumericUpDown numericUpDownStartupYear;
        private NumericUpDown numericUpDownLivingArea;
        private NumericUpDown numericUpDownTotalArea;
        private NumericUpDown numericUpDownMunicipalArea;
        private NumericUpDown numericUpDownPremisesCount;
        private NumericUpDown numericUpDownRoomsCount;
        private NumericUpDown numericUpDownApartmentsCount;
        private NumericUpDown numericUpDownSharedApartmentsCount;
        private NumericUpDown numericUpDownCommercialPremisesCount;
        private NumericUpDown numericUpDownSpecialPremisesCount;
        private NumericUpDown numericUpDownSocialPremisesCount;
        private NumericUpDown numericUpDownOtherPremisesCount;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label17;
        private Label label18;
        private Label label19;
        private Label label20;
        private Label label40;
        private Panel panel1;
        private Panel panel2;
        private TextBox textBoxHouse;
        private TextBox textBoxDescription;
        private ComboBox comboBoxCurrentFundType;
        private ComboBox comboBoxState;
        private TextBox textBoxCadastralNum;
        private DataGridView dataGridViewRestrictions;
        private DataGridView dataGridViewOwnerships;
        private ComboBox comboBoxStreet;
        private ComboBox comboBoxStructureType;
        private CheckBox checkBoxImprovement;
        private CheckBox checkBoxElevator;
        private DataGridViewTextBoxColumn restriction_number;
        private DataGridViewTextBoxColumn restriction_date;
        private DataGridViewTextBoxColumn restriction_description;
        private DataGridViewComboBoxColumn id_restriction_type;
        private DataGridViewTextBoxColumn ownership_number;
        private DataGridViewTextBoxColumn ownership_date;
        private DataGridViewTextBoxColumn ownership_description;
        private DataGridViewComboBoxColumn id_ownership_type;
        private NumericUpDown numericUpDownWear;
        private Label label21;
        private Panel panel3;
        private vButton vButtonRestrictionEdit;
        private vButton vButtonRestrictionDelete;
        private vButton vButtonRestrictionAdd;
        private Panel panel4;
        private vButton vButtonOwnershipEdit;
        private vButton vButtonOwnershipDelete;
        private vButton vButtonOwnershipAdd;
        private DateTimePicker dateTimePickerStateDate;
        private Label label22;
        #endregion Components

        #region Models
        private BuildingsDataModel buildings;
        private KladrStreetsDataModel kladr;
        private StructureTypesDataModel structureTypes;
        private RestrictionsDataModel restrictions;
        private RestrictionTypesDataModel restrictionTypes;
        private RestrictionsBuildingsAssocDataModel restrictionBuildingsAssoc;
        private OwnershipsRightsDataModel ownershipRights;
        private OwnershipRightTypesDataModel ownershipRightTypes;
        private OwnershipBuildingsAssocDataModel ownershipBuildingsAssoc;
        private FundTypesDataModel fundTypes;
        private ObjectStatesDataModel object_states;
        private CalcDataModelBuildingsPremisesFunds buildingsPremisesFunds;
        private CalcDataModelBuildingsCurrentFunds buildingsCurrentFund;
        private CalcDataModelBuildingsPremisesSumArea buildingsPremisesSumArea;
        #endregion Models

        #region Views
        private BindingSource v_buildings;
        private BindingSource v_kladr;
        private BindingSource v_structureTypes;
        private BindingSource v_restrictions;
        private BindingSource v_restrictonTypes;
        private BindingSource v_restrictionBuildingsAssoc;
        private BindingSource v_ownershipRights;
        private BindingSource v_ownershipRightTypes;
        private BindingSource v_ownershipBuildingsAssoc;
        private BindingSource v_fundType;
        private BindingSource v_object_states;
        private BindingSource v_buildingsPremisesFunds;
        private BindingSource v_buildingsCurrentFund;
        private BindingSource v_buildingsPremisesSumArea;
        #endregion Views

        //Forms
        private SearchForm sbSimpleSearchForm;
        private SearchForm sbExtendedSearchForm;
        
        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable;

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
            DynamicFilter = buildingListViewport.DynamicFilter;
            StaticFilter = buildingListViewport.StaticFilter;
            ParentRow = buildingListViewport.ParentRow;
            ParentType = buildingListViewport.ParentType;
        }

        private void RestrictionsFilterRebuild()
        {
            var restrictionsFilter = "id_restriction IN (0";
            for (var i = 0; i < v_restrictionBuildingsAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)v_restrictionBuildingsAssoc[i])["id_restriction"] + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(',');
            restrictionsFilter += ")";
            v_restrictions.Filter = restrictionsFilter;
        }

        private void OwnershipsFilterRebuild()
        {
            var ownershipFilter = "id_ownership_right IN (0";
            for (var i = 0; i < v_ownershipBuildingsAssoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownershipBuildingsAssoc[i])["id_ownership_right"] + ",";
            ownershipFilter = ownershipFilter.TrimEnd(',');
            ownershipFilter += ")";
            v_ownershipRights.Filter = ownershipFilter;
        }

        private void FiltersRebuild()
        {
            if (v_buildingsPremisesFunds != null)
            {
                var position = -1;
                if ((v_buildings.Position != -1) && !(((DataRowView)v_buildings[v_buildings.Position])["id_building"] is DBNull))
                    position =
                        v_buildingsPremisesFunds.Find("id_building", ((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
                if (position != -1)
                {
                    var socialCount = Convert.ToDecimal(((DataRowView)v_buildingsPremisesFunds[position])["social_premises_count"], CultureInfo.InvariantCulture);
                    var specialCount = Convert.ToDecimal(((DataRowView)v_buildingsPremisesFunds[position])["special_premises_count"], CultureInfo.InvariantCulture);
                    var commercialCount = Convert.ToDecimal(((DataRowView)v_buildingsPremisesFunds[position])["commercial_premises_count"], CultureInfo.InvariantCulture);
                    var otherCount = Convert.ToDecimal(((DataRowView)v_buildingsPremisesFunds[position])["other_premises_count"], CultureInfo.InvariantCulture);
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
            if (v_buildingsCurrentFund != null)
            {
                var position = -1;
                if ((v_buildings.Position != -1) && !(((DataRowView)v_buildings[v_buildings.Position])["id_building"] is DBNull))
                    position =
                        v_buildingsCurrentFund.Find("id_building", ((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
                comboBoxCurrentFundType.SelectedValue = position != -1 ? ((DataRowView)v_buildingsCurrentFund[position])["id_fund_type"] : DBNull.Value;
                ShowOrHideCurrentFund();
            }
            if (v_buildingsPremisesSumArea != null)
            {
                var position = -1;
                if ((v_buildings.Position != -1) && !(((DataRowView)v_buildings[v_buildings.Position])["id_building"] is DBNull))
                    position = v_buildingsPremisesSumArea.Find("id_building", ((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
                if (position != -1)
                {
                    var value = Convert.ToDecimal((double)((DataRowView)v_buildingsPremisesSumArea[position])["sum_area"], CultureInfo.InvariantCulture);
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
            if (comboBoxCurrentFundType.SelectedValue != null && v_buildings.Position != -1 &&
                ((DataRowView)v_buildings[v_buildings.Position])["id_state"] != DBNull.Value &&
                (new[] { 1, 4, 5, 9 }).Contains((int)((DataRowView)v_buildings[v_buildings.Position])["id_state"]))
            {
                label19.Visible = true;
                comboBoxCurrentFundType.Visible = true;
                checkBoxImprovement.Location = new Point(175, 177);
                checkBoxElevator.Location = new Point(19, 177);
                tableLayoutPanel.RowStyles[0].Height = 235F;
            }
            else
            {
                label19.Visible = false;
                comboBoxCurrentFundType.Visible = false;
                checkBoxImprovement.Location = new Point(175, 151);
                checkBoxElevator.Location = new Point(19, 151);
                tableLayoutPanel.RowStyles[0].Height = 210F;
            }
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = (v_buildings.Position >= 0) ? (DataRowView)v_buildings[v_buildings.Position] : null;
            if (row != null && ((v_buildings.Position >= 0) && (row["state_date"] != DBNull.Value)))
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
            comboBoxStreet.DataBindings.Add("SelectedValue", v_buildings, "id_street", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxHouse.DataBindings.Clear();
            textBoxHouse.DataBindings.Add("Text", v_buildings, "house", true, DataSourceUpdateMode.Never, "");
            numericUpDownFloors.DataBindings.Clear();
            numericUpDownFloors.DataBindings.Add("Value", v_buildings, "floors", true, DataSourceUpdateMode.Never, 5);
            numericUpDownStartupYear.DataBindings.Clear();
            numericUpDownStartupYear.Maximum = DateTime.Now.Year;
            numericUpDownStartupYear.DataBindings.Add("Value", v_buildings, "startup_year", true, DataSourceUpdateMode.Never, DateTime.Now.Year);
            textBoxCadastralNum.DataBindings.Clear();
            textBoxCadastralNum.DataBindings.Add("Text", v_buildings, "cadastral_num", true, DataSourceUpdateMode.Never, "");
            numericUpDownCadastralCost.DataBindings.Clear();
            numericUpDownCadastralCost.DataBindings.Add("Value", v_buildings, "cadastral_cost", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceCost.DataBindings.Clear();
            numericUpDownBalanceCost.DataBindings.Add("Value", v_buildings, "balance_cost", true, DataSourceUpdateMode.Never, 0);
            checkBoxImprovement.DataBindings.Clear();
            checkBoxImprovement.DataBindings.Add("Checked", v_buildings, "improvement", true, DataSourceUpdateMode.Never, true);
            checkBoxElevator.DataBindings.Clear();
            checkBoxElevator.DataBindings.Add("Checked", v_buildings, "elevator", true, DataSourceUpdateMode.Never, false);
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_buildings, "description", true, DataSourceUpdateMode.Never, "");
            numericUpDownPremisesCount.DataBindings.Clear();
            numericUpDownPremisesCount.DataBindings.Add("Value", v_buildings, "num_premises", true, DataSourceUpdateMode.Never, 0);
            numericUpDownRoomsCount.DataBindings.Clear();
            numericUpDownRoomsCount.DataBindings.Add("Value", v_buildings, "num_rooms", true, DataSourceUpdateMode.Never, 0);
            numericUpDownApartmentsCount.DataBindings.Clear();
            numericUpDownApartmentsCount.DataBindings.Add("Value", v_buildings, "num_apartments", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSharedApartmentsCount.DataBindings.Clear();
            numericUpDownSharedApartmentsCount.DataBindings.Add("Value", v_buildings, "num_shared_apartments", true, DataSourceUpdateMode.Never, 0);
            numericUpDownLivingArea.DataBindings.Clear();
            numericUpDownLivingArea.DataBindings.Add("Value", v_buildings, "living_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownTotalArea.DataBindings.Clear();
            numericUpDownTotalArea.DataBindings.Add("Value", v_buildings, "total_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownWear.DataBindings.Clear();
            numericUpDownWear.DataBindings.Add("Value", v_buildings, "wear", true, DataSourceUpdateMode.Never, 0);

            dateTimePickerStateDate.DataBindings.Clear();
            dateTimePickerStateDate.DataBindings.Add("Value", v_buildings, "state_date", true, DataSourceUpdateMode.Never, null);

            comboBoxStructureType.DataSource = v_structureTypes;
            comboBoxStructureType.ValueMember = "id_structure_type";
            comboBoxStructureType.DisplayMember = "structure_type";
            comboBoxStructureType.DataBindings.Clear();
            comboBoxStructureType.DataBindings.Add("SelectedValue", v_buildings, "id_structure_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxCurrentFundType.DataSource = v_fundType;
            comboBoxCurrentFundType.ValueMember = "id_fund_type";
            comboBoxCurrentFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = v_object_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";
            comboBoxState.DataBindings.Clear();
            comboBoxState.DataBindings.Add("SelectedValue", v_buildings, "id_state", true, DataSourceUpdateMode.Never, DBNull.Value);

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

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if (!ContainsFocus)
                return;
            if ((v_buildings.Position != -1) && (BuildingFromView() != BuildingFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                    viewportState = ViewportState.ModifyRowState;
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                    viewportState = ViewportState.ReadState;
            }
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }

        private bool ChangeViewportStateTo(ViewportState state)
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
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else return false;
                            return viewportState == ViewportState.ReadState;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            if (buildings.EditingNewRecord)
                                return false;
                            viewportState = ViewportState.NewRowState;
                            return true;
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            switch (result)
                            {
                                case DialogResult.Yes:
                                    SaveRecord();
                                    break;
                                case DialogResult.No:
                                    CancelRecord();
                                    break;
                                default:
                                    return false;
                            }
                            return viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.NewRowState);
                    }
                    break;
                case ViewportState.ModifyRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            return viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.ModifyRowState);
                    }
                    break;
            }
            return false;
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
            if ((building.House == null) || string.IsNullOrEmpty(building.House.Trim()))
            {
                MessageBox.Show(@"Необходимо указать номер дома", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return false;
            }
            if (!Regex.IsMatch(building.House, @"^[0-9]+[а-я]{0,1}([\/][0-9]+[а-я]{0,1}){0,1}$"))
            {
                MessageBox.Show(@"Некорректно задан номер дома. Можно использовать только цифры, строчные буквы кириллицы буквы и дробный разделитель. Например: ""11а/3""", @"Ошибка",
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
            var buildingFromView = BuildingFromView();
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
            if (new[] { 4, 5, 9 }.Contains(building.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на добавление в базу муниципальных жилых зданий", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new[] { 1, 3, 6, 7, 8 }.Contains(building.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
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
            if ((building.House == buildingFromView.House) && (building.IdStreet == buildingFromView.IdStreet))
                return true;
            return DataModelHelper.BuildingsDuplicateCount(building) == 0 || 
                MessageBox.Show(@"В базе уже имеется здание с таким адресом. Все равно продолжить сохранение?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
        }

        public void LocateBuildingBy(int id)
        {
            var position = v_buildings.Find("id_building", id);
            is_editable = false;
            if (position > 0)
                v_buildings.Position = position;
            is_editable = true;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
                Text = @"Новое здание";
            else
                if (v_buildings.Position != -1)
                    Text = string.Format(CultureInfo.InvariantCulture, "Здание №{0}", ((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
                else
                    Text = @"Здания отсутствуют";
        }

        private Building BuildingFromViewport()
        {
            var building = new Building
            {
                IdBuilding =
                    v_buildings.Position == -1
                        ? null
                        : ViewportHelper.ValueOrNull<int>((DataRowView) v_buildings[v_buildings.Position], "id_building"),
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

        private Building BuildingFromView()
        {
            var row = (DataRowView)v_buildings[v_buildings.Position];
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
            row["living_area"] = ViewportHelper.ValueOrDBNull(building.LivingArea);
            row["total_area"] = ViewportHelper.ValueOrDBNull(building.TotalArea);
            row["wear"] = ViewportHelper.ValueOrDBNull(building.Wear);
            row["state_date"] = ViewportHelper.ValueOrDBNull(building.StateDate);
            row.EndEdit();
        }

        public override int GetRecordCount()
        {
            return v_buildings.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_buildings.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_buildings.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_buildings.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_buildings.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_buildings.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_buildings.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_buildings.Position > -1) && (v_buildings.Position < (v_buildings.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_buildings.Position > -1) && (v_buildings.Position < (v_buildings.Count - 1));
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
            buildings = BuildingsDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();
            structureTypes = StructureTypesDataModel.GetInstance();
            restrictions = RestrictionsDataModel.GetInstance();
            restrictionTypes = RestrictionTypesDataModel.GetInstance();
            restrictionBuildingsAssoc = RestrictionsBuildingsAssocDataModel.GetInstance();
            ownershipRights = OwnershipsRightsDataModel.GetInstance();
            ownershipRightTypes = OwnershipRightTypesDataModel.GetInstance();
            ownershipBuildingsAssoc = OwnershipBuildingsAssocDataModel.GetInstance();
            fundTypes = FundTypesDataModel.GetInstance();
            object_states = ObjectStatesDataModel.GetInstance();

            //Вычисляемые модели
            buildingsPremisesFunds = CalcDataModelBuildingsPremisesFunds.GetInstance();
            buildingsCurrentFund = CalcDataModelBuildingsCurrentFunds.GetInstance();
            buildingsPremisesSumArea = CalcDataModelBuildingsPremisesSumArea.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            buildings.Select();
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

            var ds = DataSetManager.DataSet;

            v_kladr = new BindingSource
            {
                DataMember = "kladr",
                DataSource = ds
            };

            v_structureTypes = new BindingSource
            {
                DataMember = "structure_types",
                DataSource = ds
            };

            v_restrictions = new BindingSource
            {
                DataMember = "restrictions",
                DataSource = ds
            };

            v_ownershipRights = new BindingSource
            {
                DataMember = "ownership_rights",
                DataSource = ds
            };

            v_restrictonTypes = new BindingSource
            {
                DataMember = "restriction_types",
                DataSource = ds
            };

            v_ownershipRightTypes = new BindingSource
            {
                DataMember = "ownership_right_types",
                DataSource = ds
            };

            v_buildingsPremisesFunds = new BindingSource
            {
                DataMember = "buildings_premises_funds",
                DataSource = buildingsPremisesFunds.Select()
            };

            v_buildingsCurrentFund = new BindingSource
            {
                DataMember = "buildings_current_funds",
                DataSource = buildingsCurrentFund.Select()
            };

            v_buildingsPremisesSumArea = new BindingSource
            {
                DataMember = "buildings_premises_sum_area",
                DataSource = buildingsPremisesSumArea.Select()
            };

            v_fundType = new BindingSource
            {
                DataMember = "fund_types",
                DataSource = ds
            };

            v_object_states = new BindingSource
            {
                DataMember = "object_states",
                DataSource = ds
            };

            v_buildings = new BindingSource();
            v_buildings.CurrentItemChanged += v_buildings_CurrentItemChanged;
            v_buildings.DataMember = "buildings";
            v_buildings.DataSource = ds;
            v_buildings.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_buildings.Filter += " AND ";
            v_buildings.Filter += DynamicFilter;
            buildings.Select().RowDeleted += BuildingViewport_RowDeleted;
            buildings.Select().RowChanged += BuildingViewport_RowChanged;

            v_restrictionBuildingsAssoc = new BindingSource();
            v_restrictionBuildingsAssoc.CurrentItemChanged += v_restrictionBuildingsAssoc_CurrentItemChanged;
            v_restrictionBuildingsAssoc.DataMember = "buildings_restrictions_buildings_assoc";
            v_restrictionBuildingsAssoc.DataSource = v_buildings;
            RestrictionsFilterRebuild();
            restrictionBuildingsAssoc.Select().RowChanged += RestrictionsAssoc_RowChanged;
            restrictionBuildingsAssoc.Select().RowDeleted += RestrictionsAssoc_RowDeleted;

            v_ownershipBuildingsAssoc = new BindingSource();
            v_ownershipBuildingsAssoc.CurrentItemChanged += v_ownershipBuildingsAssoc_CurrentItemChanged;
            v_ownershipBuildingsAssoc.DataMember = "buildings_ownership_buildings_assoc";
            v_ownershipBuildingsAssoc.DataSource = v_buildings;
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
            v_buildings.Filter = StaticFilter;
            is_editable = true;
            DynamicFilter = "";
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
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            is_editable = false;
            v_buildings.Filter = filter;
            is_editable = true;
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            var building = BuildingFromViewport();
            var buildingFromView = BuildingFromView();
            if (!ValidateBuilding(building))
                return;
            var filter = "";
            if (!string.IsNullOrEmpty(v_buildings.Filter))
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
                    var idBuilding = BuildingsDataModel.Insert(building);
                    if (idBuilding == -1)
                    {
                        buildings.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    building.IdBuilding = idBuilding;
                    is_editable = false;
                    if (v_buildings.Position == -1)
                        newRow = (DataRowView)v_buildings.AddNew();
                    else
                        newRow = ((DataRowView)v_buildings[v_buildings.Position]);
                    filter += string.Format(CultureInfo.CurrentCulture, "(id_building = {0})", building.IdBuilding);
                    v_buildings.Filter += filter;
                    FillRowFromBuilding(building, newRow);
                    Text = @"Здание №" + idBuilding.ToString(CultureInfo.InvariantCulture);
                    viewportState = ViewportState.ReadState;
                    buildings.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (building.IdBuilding == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить здание без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    var dialogResult = DialogResult.No;
                    if (building.IdState != buildingFromView.IdState || building.StateDate != buildingFromView.StateDate)
                        dialogResult = MessageBox.Show(@"Хотите ли вы установить помещениям, расположенным в здании, такое же состояние?", @"Внимание",
                                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    if (dialogResult == DialogResult.Cancel)
                        return;
                    if (BuildingsDataModel.Update(building) == -1)
                        return;
                    var row = ((DataRowView)v_buildings[v_buildings.Position]);
                    is_editable = false;
                    filter += string.Format(CultureInfo.CurrentCulture, "(id_building = {0})", building.IdBuilding);
                    v_buildings.Filter += filter;
                    FillRowFromBuilding(building, row);
                    if (dialogResult == DialogResult.Yes)
                    {
                        var premises = from premisesRow in DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select())
                                       where premisesRow.Field<int>("id_building") == building.IdBuilding
                                       select premisesRow;
                        foreach (var premise in premises)
                        {
                            var idPremises = ViewportHelper.ValueOrNull<int>(premise, "id_premises");
                            if (idPremises == null) continue;
                            if (PremisesDataModel.UpdateState(idPremises.Value, building.IdState,
                                building.StateDate) == -1)
                                return;
                        }
                    }
                    viewportState = ViewportState.ReadState;
                    CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
                    CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            ShowOrHideCurrentFund();
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
                    buildings.EditingNewRecord = false;
                    if (v_buildings.Position != -1)
                    {
                        is_editable = false;
                        ((DataRowView)v_buildings[v_buildings.Position]).Delete();
                    }
                    else
                        Text = @"Здания отсутствуют";
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
            return (!buildings.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            v_buildings.AddNew();
            is_editable = true;
            buildings.EditingNewRecord = true;
            UnbindedCheckBoxesUpdate();
        }

        public override bool CanCopyRecord()
        {
            return (v_buildings.Position != -1) && (!buildings.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var building = BuildingFromView();
            v_buildings.AddNew();
            buildings.EditingNewRecord = true;
            ViewportFromBuilding(building);
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (v_buildings.Position > -1)
                && (viewportState != ViewportState.NewRowState) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это здание?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)v_buildings.Current)["id_building"], EntityType.Building)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление муниципальных жилых зданий и зданий, в которых присутствуют муниципальные помещения",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)v_buildings.Current)["id_building"], EntityType.Building)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление немуниципальных жилых зданий и зданий, в которых присутствуют немуниципальные помещения",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (BuildingsDataModel.Delete((int)((DataRowView)v_buildings.Current)["id_building"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_buildings[v_buildings.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
                CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new BuildingViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as int?) ?? -1);
            return viewport;
        }

        public override bool HasAssocPremises()
        {
            return (v_buildings.Position > -1);
        }

        public override bool HasAssocOwnerships()
        {
            return (v_buildings.Position > -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (v_buildings.Position > -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (v_buildings.Position > -1);
        }
        
        public override bool HasAssocTenancies()
        {
            return (v_buildings.Position > -1);
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
            if (v_buildings.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"], CultureInfo.InvariantCulture),
                ((DataRowView)v_buildings[v_buildings.Position]).Row,
                ParentTypeEnum.Building);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
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

        void v_buildings_CurrentItemChanged(object sender, EventArgs e)
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
            if (v_buildings.Position == -1)
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

        void comboBoxState_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownLivingArea_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownSharedApartmentsCount_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownApartmentsCount_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownRoomsCount_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownPremisesCount_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownBalanceCost_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownCadastralCost_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownStartupYear_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownFloors_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void numericUpDownTotalArea_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void checkBoxImprovement_CheckedChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void checkBoxElevator_CheckedChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxStructureType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxCadastralNum_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxHouse_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void dateTimePickerStateDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxStreet_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxStreet.Text;
                var selectionStart = comboBoxStreet.SelectionStart;
                var selectionLength = comboBoxStreet.SelectionLength;
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
            if (e.KeyChar >= 'А' && e.KeyChar <= 'Я')
                e.KeyChar = e.KeyChar.ToString().ToLower(CultureInfo.CurrentCulture)[0];
            if (e.KeyChar == '\\')
                e.KeyChar = '/';
            if (e.KeyChar == ' ')
                e.Handled = true;
        }

        private void numericUpDownWear_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void vButtonRestrictionAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
                editor.ShowDialog();
            }
        }

        private void vButtonRestrictionDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
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
            var idRestriction = (int)((DataRowView)v_restrictions[v_restrictions.Position])["id_restriction"];
            if (RestrictionsDataModel.Delete(idRestriction) == -1)
                return;
            restrictions.Select().Rows.Find(idRestriction).Delete();
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                    int.Parse(((DataRowView)v_buildings[v_buildings.Position])["id_building"].ToString(), CultureInfo.InvariantCulture), true);
        }

        private void vButtonRestrictionEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_restrictions.Position == -1)
            {
                MessageBox.Show(@"Не выбран реквизит для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var restriction = new Restriction();
            var row = (DataRowView)v_restrictions[v_restrictions.Position];
            restriction.IdRestriction = (int?)row["id_restriction"];
            restriction.IdRestrictionType = (int?)row["id_restriction_type"];
            restriction.Number = row["number"].ToString();
            restriction.Date = (DateTime?)row["date"];
            restriction.Description = row["description"].ToString();
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
                editor.RestrictionValue = restriction;
                editor.ShowDialog();
            }
        }

        private void vButtonOwnershipAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
                editor.ShowDialog();
            }
        }

        private void vButtonOwnershipDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
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
            if (OwnershipsRightsDataModel.Delete(idOwnershipRight) == -1)
                return;
            ownershipRights.Select().Rows.Find(idOwnershipRight).Delete();
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                    int.Parse(((DataRowView)v_buildings[v_buildings.Position])["id_building"].ToString(), CultureInfo.InvariantCulture), true);
        }

        private void vButtonOwnershipEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_ownershipRights.Position == -1)
            {
                MessageBox.Show(@"Не выбрано ограничение для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var ownershipRight = new OwnershipRight();
            var row = (DataRowView)v_ownershipRights[v_ownershipRights.Position];
            ownershipRight.IdOwnershipRight = (int?)row["id_ownership_right"];
            ownershipRight.IdOwnershipRightType = (int?)row["id_ownership_right_type"];
            ownershipRight.Number = row["number"].ToString();
            ownershipRight.Date = (DateTime?)row["date"];
            ownershipRight.Description = row["description"].ToString();
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Building;
                editor.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
                editor.OwnershipRightValue = ownershipRight;
                editor.ShowDialog();
            }
        }

        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(BuildingViewport));
            tableLayoutPanel = new TableLayoutPanel();
            groupBox4 = new GroupBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            panel1 = new Panel();
            numericUpDownWear = new NumericUpDown();
            label21 = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label17 = new Label();
            label18 = new Label();
            numericUpDownFloors = new NumericUpDown();
            numericUpDownStartupYear = new NumericUpDown();
            comboBoxStreet = new ComboBox();
            comboBoxStructureType = new ComboBox();
            textBoxHouse = new TextBox();
            panel2 = new Panel();
            dateTimePickerStateDate = new DateTimePicker();
            label22 = new Label();
            label40 = new Label();
            comboBoxState = new ComboBox();
            comboBoxCurrentFundType = new ComboBox();
            numericUpDownBalanceCost = new NumericUpDown();
            numericUpDownCadastralCost = new NumericUpDown();
            checkBoxImprovement = new CheckBox();
            checkBoxElevator = new CheckBox();
            label14 = new Label();
            label15 = new Label();
            label16 = new Label();
            label19 = new Label();
            textBoxCadastralNum = new TextBox();
            groupBox1 = new GroupBox();
            numericUpDownPremisesCount = new NumericUpDown();
            numericUpDownRoomsCount = new NumericUpDown();
            numericUpDownApartmentsCount = new NumericUpDown();
            numericUpDownSharedApartmentsCount = new NumericUpDown();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            groupBox3 = new GroupBox();
            numericUpDownOtherPremisesCount = new NumericUpDown();
            numericUpDownSpecialPremisesCount = new NumericUpDown();
            numericUpDownCommercialPremisesCount = new NumericUpDown();
            numericUpDownSocialPremisesCount = new NumericUpDown();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            groupBox2 = new GroupBox();
            numericUpDownTotalArea = new NumericUpDown();
            label20 = new Label();
            numericUpDownLivingArea = new NumericUpDown();
            numericUpDownMunicipalArea = new NumericUpDown();
            label8 = new Label();
            label9 = new Label();
            groupBox5 = new GroupBox();
            textBoxDescription = new TextBox();
            groupBox6 = new GroupBox();
            panel3 = new Panel();
            vButtonRestrictionEdit = new vButton();
            vButtonRestrictionDelete = new vButton();
            vButtonRestrictionAdd = new vButton();
            dataGridViewRestrictions = new DataGridView();
            restriction_number = new DataGridViewTextBoxColumn();
            restriction_date = new DataGridViewTextBoxColumn();
            restriction_description = new DataGridViewTextBoxColumn();
            id_restriction_type = new DataGridViewComboBoxColumn();
            groupBox7 = new GroupBox();
            panel4 = new Panel();
            vButtonOwnershipEdit = new vButton();
            vButtonOwnershipDelete = new vButton();
            vButtonOwnershipAdd = new vButton();
            dataGridViewOwnerships = new DataGridView();
            ownership_number = new DataGridViewTextBoxColumn();
            ownership_date = new DataGridViewTextBoxColumn();
            ownership_description = new DataGridViewTextBoxColumn();
            id_ownership_type = new DataGridViewComboBoxColumn();
            tableLayoutPanel.SuspendLayout();
            groupBox4.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            panel1.SuspendLayout();
            ((ISupportInitialize)(numericUpDownWear)).BeginInit();
            ((ISupportInitialize)(numericUpDownFloors)).BeginInit();
            ((ISupportInitialize)(numericUpDownStartupYear)).BeginInit();
            panel2.SuspendLayout();
            ((ISupportInitialize)(numericUpDownBalanceCost)).BeginInit();
            ((ISupportInitialize)(numericUpDownCadastralCost)).BeginInit();
            groupBox1.SuspendLayout();
            ((ISupportInitialize)(numericUpDownPremisesCount)).BeginInit();
            ((ISupportInitialize)(numericUpDownRoomsCount)).BeginInit();
            ((ISupportInitialize)(numericUpDownApartmentsCount)).BeginInit();
            ((ISupportInitialize)(numericUpDownSharedApartmentsCount)).BeginInit();
            groupBox3.SuspendLayout();
            ((ISupportInitialize)(numericUpDownOtherPremisesCount)).BeginInit();
            ((ISupportInitialize)(numericUpDownSpecialPremisesCount)).BeginInit();
            ((ISupportInitialize)(numericUpDownCommercialPremisesCount)).BeginInit();
            ((ISupportInitialize)(numericUpDownSocialPremisesCount)).BeginInit();
            groupBox2.SuspendLayout();
            ((ISupportInitialize)(numericUpDownTotalArea)).BeginInit();
            ((ISupportInitialize)(numericUpDownLivingArea)).BeginInit();
            ((ISupportInitialize)(numericUpDownMunicipalArea)).BeginInit();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            panel3.SuspendLayout();
            ((ISupportInitialize)(dataGridViewRestrictions)).BeginInit();
            groupBox7.SuspendLayout();
            panel4.SuspendLayout();
            ((ISupportInitialize)(dataGridViewOwnerships)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(groupBox4, 0, 0);
            tableLayoutPanel.Controls.Add(groupBox1, 0, 1);
            tableLayoutPanel.Controls.Add(groupBox3, 1, 1);
            tableLayoutPanel.Controls.Add(groupBox2, 0, 2);
            tableLayoutPanel.Controls.Add(groupBox5, 1, 2);
            tableLayoutPanel.Controls.Add(groupBox6, 0, 3);
            tableLayoutPanel.Controls.Add(groupBox7, 1, 3);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(3, 3);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 235F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new Size(769, 596);
            tableLayoutPanel.TabIndex = 0;
            // 
            // groupBox4
            // 
            tableLayoutPanel.SetColumnSpan(groupBox4, 2);
            groupBox4.Controls.Add(tableLayoutPanel2);
            groupBox4.Dock = DockStyle.Fill;
            groupBox4.Location = new Point(3, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(763, 229);
            groupBox4.TabIndex = 0;
            groupBox4.TabStop = false;
            groupBox4.Text = @"Общие сведения";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(panel1, 0, 0);
            tableLayoutPanel2.Controls.Add(panel2, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 17);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            tableLayoutPanel2.Size = new Size(757, 209);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(numericUpDownWear);
            panel1.Controls.Add(label21);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label17);
            panel1.Controls.Add(label18);
            panel1.Controls.Add(numericUpDownFloors);
            panel1.Controls.Add(numericUpDownStartupYear);
            panel1.Controls.Add(comboBoxStreet);
            panel1.Controls.Add(comboBoxStructureType);
            panel1.Controls.Add(textBoxHouse);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(372, 203);
            panel1.TabIndex = 0;
            // 
            // numericUpDownWear
            // 
            numericUpDownWear.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                       | AnchorStyles.Right;
            numericUpDownWear.DecimalPlaces = 2;
            numericUpDownWear.Location = new Point(174, 149);
            numericUpDownWear.Maximum = new decimal(new[] {
            999,
            0,
            0,
            0});
            numericUpDownWear.Name = "numericUpDownWear";
            numericUpDownWear.Size = new Size(194, 21);
            numericUpDownWear.TabIndex = 5;
            numericUpDownWear.ThousandsSeparator = true;
            numericUpDownWear.ValueChanged += numericUpDownWear_ValueChanged;
            numericUpDownWear.Enter += selectAll_Enter;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(10, 152);
            label21.Name = "label21";
            label21.Size = new Size(59, 15);
            label21.TabIndex = 36;
            label21.Text = @"Износ, %";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 9);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 0;
            label1.Text = @"Улица";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 38);
            label2.Name = "label2";
            label2.Size = new Size(79, 15);
            label2.TabIndex = 1;
            label2.Text = @"Номер дома";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(10, 67);
            label3.Name = "label3";
            label3.Size = new Size(105, 15);
            label3.TabIndex = 2;
            label3.Text = @"Этажность дома";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(10, 96);
            label17.Name = "label17";
            label17.Size = new Size(160, 15);
            label17.TabIndex = 3;
            label17.Text = @"Год ввода в эксплуатацию";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(10, 125);
            label18.Name = "label18";
            label18.Size = new Size(155, 15);
            label18.TabIndex = 4;
            label18.Text = @"Тип строения (материал)";
            // 
            // numericUpDownFloors
            // 
            numericUpDownFloors.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                         | AnchorStyles.Right;
            numericUpDownFloors.Location = new Point(175, 65);
            numericUpDownFloors.Maximum = new decimal(new[] {
            255,
            0,
            0,
            0});
            numericUpDownFloors.Name = "numericUpDownFloors";
            numericUpDownFloors.Size = new Size(193, 21);
            numericUpDownFloors.TabIndex = 2;
            numericUpDownFloors.ValueChanged += numericUpDownFloors_ValueChanged;
            numericUpDownFloors.Enter += selectAll_Enter;
            // 
            // numericUpDownStartupYear
            // 
            numericUpDownStartupYear.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                              | AnchorStyles.Right;
            numericUpDownStartupYear.Location = new Point(175, 93);
            numericUpDownStartupYear.Maximum = new decimal(new[] {
            2014,
            0,
            0,
            0});
            numericUpDownStartupYear.Minimum = new decimal(new[] {
            1900,
            0,
            0,
            0});
            numericUpDownStartupYear.Name = "numericUpDownStartupYear";
            numericUpDownStartupYear.Size = new Size(193, 21);
            numericUpDownStartupYear.TabIndex = 3;
            numericUpDownStartupYear.Value = new decimal(new[] {
            1900,
            0,
            0,
            0});
            numericUpDownStartupYear.ValueChanged += numericUpDownStartupYear_ValueChanged;
            numericUpDownStartupYear.Enter += selectAll_Enter;
            // 
            // comboBoxStreet
            // 
            comboBoxStreet.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                    | AnchorStyles.Right;
            comboBoxStreet.FormattingEnabled = true;
            comboBoxStreet.Location = new Point(175, 7);
            comboBoxStreet.Name = "comboBoxStreet";
            comboBoxStreet.Size = new Size(193, 23);
            comboBoxStreet.TabIndex = 0;
            comboBoxStreet.DropDownClosed += comboBoxStreet_DropDownClosed;
            comboBoxStreet.SelectedValueChanged += comboBoxStreet_SelectedValueChanged;
            comboBoxStreet.Enter += selectAll_Enter;
            comboBoxStreet.KeyUp += comboBoxStreet_KeyUp;
            comboBoxStreet.Leave += comboBoxStreet_Leave;
            // 
            // comboBoxStructureType
            // 
            comboBoxStructureType.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            comboBoxStructureType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxStructureType.FormattingEnabled = true;
            comboBoxStructureType.Location = new Point(175, 121);
            comboBoxStructureType.Name = "comboBoxStructureType";
            comboBoxStructureType.Size = new Size(193, 23);
            comboBoxStructureType.TabIndex = 4;
            comboBoxStructureType.SelectedIndexChanged += comboBoxStructureType_SelectedIndexChanged;
            // 
            // textBoxHouse
            // 
            textBoxHouse.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                  | AnchorStyles.Right;
            textBoxHouse.Location = new Point(175, 37);
            textBoxHouse.MaxLength = 20;
            textBoxHouse.Name = "textBoxHouse";
            textBoxHouse.Size = new Size(193, 21);
            textBoxHouse.TabIndex = 1;
            textBoxHouse.TextChanged += textBoxHouse_TextChanged;
            textBoxHouse.Enter += selectAll_Enter;
            textBoxHouse.KeyPress += textBoxHouse_KeyPress;
            // 
            // panel2
            // 
            panel2.Controls.Add(dateTimePickerStateDate);
            panel2.Controls.Add(label22);
            panel2.Controls.Add(label40);
            panel2.Controls.Add(comboBoxState);
            panel2.Controls.Add(comboBoxCurrentFundType);
            panel2.Controls.Add(numericUpDownBalanceCost);
            panel2.Controls.Add(numericUpDownCadastralCost);
            panel2.Controls.Add(checkBoxImprovement);
            panel2.Controls.Add(checkBoxElevator);
            panel2.Controls.Add(label14);
            panel2.Controls.Add(label15);
            panel2.Controls.Add(label16);
            panel2.Controls.Add(label19);
            panel2.Controls.Add(textBoxCadastralNum);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(381, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(373, 203);
            panel2.TabIndex = 1;
            // 
            // dateTimePickerStateDate
            // 
            dateTimePickerStateDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            dateTimePickerStateDate.Location = new Point(175, 121);
            dateTimePickerStateDate.Name = "dateTimePickerStateDate";
            dateTimePickerStateDate.ShowCheckBox = true;
            dateTimePickerStateDate.Size = new Size(194, 21);
            dateTimePickerStateDate.TabIndex = 4;
            dateTimePickerStateDate.ValueChanged += dateTimePickerStateDate_ValueChanged;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(16, 124);
            label22.Name = "label22";
            label22.Size = new Size(147, 15);
            label22.TabIndex = 37;
            label22.Text = @"Состояние установлено";
            // 
            // label40
            // 
            label40.AutoSize = true;
            label40.Location = new Point(16, 95);
            label40.Name = "label40";
            label40.Size = new Size(119, 15);
            label40.TabIndex = 31;
            label40.Text = @"Текущее состояние";
            // 
            // comboBoxState
            // 
            comboBoxState.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                   | AnchorStyles.Right;
            comboBoxState.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxState.FormattingEnabled = true;
            comboBoxState.Location = new Point(175, 91);
            comboBoxState.Name = "comboBoxState";
            comboBoxState.Size = new Size(194, 23);
            comboBoxState.TabIndex = 3;
            comboBoxState.SelectedIndexChanged += comboBoxState_SelectedIndexChanged;
            // 
            // comboBoxCurrentFundType
            // 
            comboBoxCurrentFundType.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            comboBoxCurrentFundType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCurrentFundType.Enabled = false;
            comboBoxCurrentFundType.ForeColor = Color.Black;
            comboBoxCurrentFundType.FormattingEnabled = true;
            comboBoxCurrentFundType.Location = new Point(175, 149);
            comboBoxCurrentFundType.Name = "comboBoxCurrentFundType";
            comboBoxCurrentFundType.Size = new Size(194, 23);
            comboBoxCurrentFundType.TabIndex = 5;
            comboBoxCurrentFundType.Visible = false;
            // 
            // numericUpDownBalanceCost
            // 
            numericUpDownBalanceCost.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                              | AnchorStyles.Right;
            numericUpDownBalanceCost.DecimalPlaces = 2;
            numericUpDownBalanceCost.Location = new Point(175, 63);
            numericUpDownBalanceCost.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownBalanceCost.Name = "numericUpDownBalanceCost";
            numericUpDownBalanceCost.Size = new Size(194, 21);
            numericUpDownBalanceCost.TabIndex = 2;
            numericUpDownBalanceCost.ThousandsSeparator = true;
            numericUpDownBalanceCost.ValueChanged += numericUpDownBalanceCost_ValueChanged;
            numericUpDownBalanceCost.Enter += selectAll_Enter;
            // 
            // numericUpDownCadastralCost
            // 
            numericUpDownCadastralCost.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            numericUpDownCadastralCost.DecimalPlaces = 2;
            numericUpDownCadastralCost.Location = new Point(175, 35);
            numericUpDownCadastralCost.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownCadastralCost.Name = "numericUpDownCadastralCost";
            numericUpDownCadastralCost.Size = new Size(194, 21);
            numericUpDownCadastralCost.TabIndex = 1;
            numericUpDownCadastralCost.ThousandsSeparator = true;
            numericUpDownCadastralCost.ValueChanged += numericUpDownCadastralCost_ValueChanged;
            numericUpDownCadastralCost.Enter += selectAll_Enter;
            // 
            // checkBoxImprovement
            // 
            checkBoxImprovement.AutoSize = true;
            checkBoxImprovement.Location = new Point(175, 177);
            checkBoxImprovement.Name = "checkBoxImprovement";
            checkBoxImprovement.Size = new Size(126, 19);
            checkBoxImprovement.TabIndex = 7;
            checkBoxImprovement.Text = @"Благоустройство";
            checkBoxImprovement.UseVisualStyleBackColor = true;
            checkBoxImprovement.CheckedChanged += checkBoxImprovement_CheckedChanged;
            // 
            // checkBoxElevator
            // 
            checkBoxElevator.AutoSize = true;
            checkBoxElevator.Location = new Point(19, 177);
            checkBoxElevator.Name = "checkBoxElevator";
            checkBoxElevator.Size = new Size(118, 19);
            checkBoxElevator.TabIndex = 6;
            checkBoxElevator.Text = @"Наличие лифта";
            checkBoxElevator.UseVisualStyleBackColor = true;
            checkBoxElevator.CheckedChanged += checkBoxElevator_CheckedChanged;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(16, 11);
            label14.Name = "label14";
            label14.Size = new Size(126, 15);
            label14.TabIndex = 32;
            label14.Text = @"Кадастровый номер";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(16, 39);
            label15.Name = "label15";
            label15.Size = new Size(150, 15);
            label15.TabIndex = 33;
            label15.Text = @"Кадастровая стоимость";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(16, 67);
            label16.Name = "label16";
            label16.Size = new Size(143, 15);
            label16.TabIndex = 34;
            label16.Text = @"Балансовая стоимость";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(16, 151);
            label19.Name = "label19";
            label19.Size = new Size(90, 15);
            label19.TabIndex = 35;
            label19.Text = @"Текущий фонд";
            label19.Visible = false;
            // 
            // textBoxCadastralNum
            // 
            textBoxCadastralNum.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                         | AnchorStyles.Right;
            textBoxCadastralNum.Location = new Point(175, 7);
            textBoxCadastralNum.MaxLength = 20;
            textBoxCadastralNum.Name = "textBoxCadastralNum";
            textBoxCadastralNum.Size = new Size(194, 21);
            textBoxCadastralNum.TabIndex = 0;
            textBoxCadastralNum.TextChanged += textBoxCadastralNum_TextChanged;
            textBoxCadastralNum.Enter += selectAll_Enter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(numericUpDownPremisesCount);
            groupBox1.Controls.Add(numericUpDownRoomsCount);
            groupBox1.Controls.Add(numericUpDownApartmentsCount);
            groupBox1.Controls.Add(numericUpDownSharedApartmentsCount);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label7);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(3, 238);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(378, 134);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = @"Количество жилых помещений";
            // 
            // numericUpDownPremisesCount
            // 
            numericUpDownPremisesCount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            numericUpDownPremisesCount.Location = new Point(181, 18);
            numericUpDownPremisesCount.Maximum = new decimal(new[] {
            65535,
            0,
            0,
            0});
            numericUpDownPremisesCount.Name = "numericUpDownPremisesCount";
            numericUpDownPremisesCount.Size = new Size(193, 21);
            numericUpDownPremisesCount.TabIndex = 0;
            numericUpDownPremisesCount.ValueChanged += numericUpDownPremisesCount_ValueChanged;
            numericUpDownPremisesCount.Enter += selectAll_Enter;
            // 
            // numericUpDownRoomsCount
            // 
            numericUpDownRoomsCount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            numericUpDownRoomsCount.Location = new Point(181, 47);
            numericUpDownRoomsCount.Maximum = new decimal(new[] {
            65535,
            0,
            0,
            0});
            numericUpDownRoomsCount.Name = "numericUpDownRoomsCount";
            numericUpDownRoomsCount.Size = new Size(193, 21);
            numericUpDownRoomsCount.TabIndex = 1;
            numericUpDownRoomsCount.ValueChanged += numericUpDownRoomsCount_ValueChanged;
            numericUpDownRoomsCount.Enter += selectAll_Enter;
            // 
            // numericUpDownApartmentsCount
            // 
            numericUpDownApartmentsCount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                  | AnchorStyles.Right;
            numericUpDownApartmentsCount.Location = new Point(181, 76);
            numericUpDownApartmentsCount.Maximum = new decimal(new[] {
            65535,
            0,
            0,
            0});
            numericUpDownApartmentsCount.Name = "numericUpDownApartmentsCount";
            numericUpDownApartmentsCount.Size = new Size(193, 21);
            numericUpDownApartmentsCount.TabIndex = 2;
            numericUpDownApartmentsCount.ValueChanged += numericUpDownApartmentsCount_ValueChanged;
            numericUpDownApartmentsCount.Enter += selectAll_Enter;
            // 
            // numericUpDownSharedApartmentsCount
            // 
            numericUpDownSharedApartmentsCount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                        | AnchorStyles.Right;
            numericUpDownSharedApartmentsCount.Location = new Point(181, 105);
            numericUpDownSharedApartmentsCount.Maximum = new decimal(new[] {
            65535,
            0,
            0,
            0});
            numericUpDownSharedApartmentsCount.Name = "numericUpDownSharedApartmentsCount";
            numericUpDownSharedApartmentsCount.Size = new Size(193, 21);
            numericUpDownSharedApartmentsCount.TabIndex = 3;
            numericUpDownSharedApartmentsCount.ValueChanged += numericUpDownSharedApartmentsCount_ValueChanged;
            numericUpDownSharedApartmentsCount.Enter += selectAll_Enter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(16, 20);
            label4.Name = "label4";
            label4.Size = new Size(40, 15);
            label4.TabIndex = 4;
            label4.Text = @"Всего";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(16, 78);
            label5.Name = "label5";
            label5.Size = new Size(57, 15);
            label5.TabIndex = 5;
            label5.Text = @"Квартир";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(16, 49);
            label6.Name = "label6";
            label6.Size = new Size(52, 15);
            label6.TabIndex = 6;
            label6.Text = @"Комнат";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(16, 107);
            label7.Name = "label7";
            label7.Size = new Size(147, 15);
            label7.TabIndex = 7;
            label7.Text = @"Квартир с подселением";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(numericUpDownOtherPremisesCount);
            groupBox3.Controls.Add(numericUpDownSpecialPremisesCount);
            groupBox3.Controls.Add(numericUpDownCommercialPremisesCount);
            groupBox3.Controls.Add(numericUpDownSocialPremisesCount);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(label12);
            groupBox3.Controls.Add(label13);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(387, 238);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(379, 134);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = @"Количество помещений по типу найма";
            // 
            // numericUpDownOtherPremisesCount
            // 
            numericUpDownOtherPremisesCount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                     | AnchorStyles.Right;
            numericUpDownOtherPremisesCount.Location = new Point(175, 105);
            numericUpDownOtherPremisesCount.Maximum = new decimal(new[] {
            65535,
            0,
            0,
            0});
            numericUpDownOtherPremisesCount.Name = "numericUpDownOtherPremisesCount";
            numericUpDownOtherPremisesCount.ReadOnly = true;
            numericUpDownOtherPremisesCount.Size = new Size(194, 21);
            numericUpDownOtherPremisesCount.TabIndex = 3;
            numericUpDownOtherPremisesCount.Enter += selectAll_Enter;
            // 
            // numericUpDownSpecialPremisesCount
            // 
            numericUpDownSpecialPremisesCount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                       | AnchorStyles.Right;
            numericUpDownSpecialPremisesCount.Location = new Point(175, 76);
            numericUpDownSpecialPremisesCount.Maximum = new decimal(new[] {
            65535,
            0,
            0,
            0});
            numericUpDownSpecialPremisesCount.Name = "numericUpDownSpecialPremisesCount";
            numericUpDownSpecialPremisesCount.ReadOnly = true;
            numericUpDownSpecialPremisesCount.Size = new Size(194, 21);
            numericUpDownSpecialPremisesCount.TabIndex = 2;
            numericUpDownSpecialPremisesCount.Enter += selectAll_Enter;
            // 
            // numericUpDownCommercialPremisesCount
            // 
            numericUpDownCommercialPremisesCount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                          | AnchorStyles.Right;
            numericUpDownCommercialPremisesCount.Location = new Point(175, 47);
            numericUpDownCommercialPremisesCount.Maximum = new decimal(new[] {
            65535,
            0,
            0,
            0});
            numericUpDownCommercialPremisesCount.Name = "numericUpDownCommercialPremisesCount";
            numericUpDownCommercialPremisesCount.ReadOnly = true;
            numericUpDownCommercialPremisesCount.Size = new Size(194, 21);
            numericUpDownCommercialPremisesCount.TabIndex = 1;
            numericUpDownCommercialPremisesCount.Enter += selectAll_Enter;
            // 
            // numericUpDownSocialPremisesCount
            // 
            numericUpDownSocialPremisesCount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                      | AnchorStyles.Right;
            numericUpDownSocialPremisesCount.Location = new Point(175, 18);
            numericUpDownSocialPremisesCount.Maximum = new decimal(new[] {
            65535,
            0,
            0,
            0});
            numericUpDownSocialPremisesCount.Name = "numericUpDownSocialPremisesCount";
            numericUpDownSocialPremisesCount.ReadOnly = true;
            numericUpDownSocialPremisesCount.Size = new Size(194, 21);
            numericUpDownSocialPremisesCount.TabIndex = 0;
            numericUpDownSocialPremisesCount.Enter += selectAll_Enter;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(16, 107);
            label10.Name = "label10";
            label10.Size = new Size(50, 15);
            label10.TabIndex = 4;
            label10.Text = @"Прочие";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(16, 78);
            label11.Name = "label11";
            label11.Size = new Size(135, 15);
            label11.TabIndex = 5;
            label11.Text = @"Специализированный";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(16, 49);
            label12.Name = "label12";
            label12.Size = new Size(93, 15);
            label12.TabIndex = 6;
            label12.Text = @"Коммерческий";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(16, 20);
            label13.Name = "label13";
            label13.Size = new Size(80, 15);
            label13.TabIndex = 7;
            label13.Text = @"Социальный";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(numericUpDownTotalArea);
            groupBox2.Controls.Add(label20);
            groupBox2.Controls.Add(numericUpDownLivingArea);
            groupBox2.Controls.Add(numericUpDownMunicipalArea);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label9);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(3, 378);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(378, 104);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = @"Площадь";
            // 
            // numericUpDownTotalArea
            // 
            numericUpDownTotalArea.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            numericUpDownTotalArea.DecimalPlaces = 3;
            numericUpDownTotalArea.Location = new Point(181, 18);
            numericUpDownTotalArea.Maximum = new decimal(new[] {
            9999,
            0,
            0,
            0});
            numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            numericUpDownTotalArea.Size = new Size(193, 21);
            numericUpDownTotalArea.TabIndex = 0;
            numericUpDownTotalArea.ThousandsSeparator = true;
            numericUpDownTotalArea.ValueChanged += numericUpDownTotalArea_ValueChanged;
            numericUpDownTotalArea.Enter += selectAll_Enter;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(16, 20);
            label20.Name = "label20";
            label20.Size = new Size(46, 15);
            label20.TabIndex = 5;
            label20.Text = @"Общая";
            // 
            // numericUpDownLivingArea
            // 
            numericUpDownLivingArea.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            numericUpDownLivingArea.DecimalPlaces = 3;
            numericUpDownLivingArea.Location = new Point(181, 47);
            numericUpDownLivingArea.Maximum = new decimal(new[] {
            9999,
            0,
            0,
            0});
            numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            numericUpDownLivingArea.Size = new Size(193, 21);
            numericUpDownLivingArea.TabIndex = 1;
            numericUpDownLivingArea.ThousandsSeparator = true;
            numericUpDownLivingArea.ValueChanged += numericUpDownLivingArea_ValueChanged;
            numericUpDownLivingArea.Enter += selectAll_Enter;
            // 
            // numericUpDownMunicipalArea
            // 
            numericUpDownMunicipalArea.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            numericUpDownMunicipalArea.DecimalPlaces = 3;
            numericUpDownMunicipalArea.Location = new Point(181, 76);
            numericUpDownMunicipalArea.Maximum = new decimal(new[] {
            9999,
            0,
            0,
            0});
            numericUpDownMunicipalArea.Name = "numericUpDownMunicipalArea";
            numericUpDownMunicipalArea.ReadOnly = true;
            numericUpDownMunicipalArea.Size = new Size(193, 21);
            numericUpDownMunicipalArea.TabIndex = 2;
            numericUpDownMunicipalArea.ThousandsSeparator = true;
            numericUpDownMunicipalArea.Enter += selectAll_Enter;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(16, 78);
            label8.Name = "label8";
            label8.Size = new Size(124, 15);
            label8.TabIndex = 2;
            label8.Text = @"Муниципальных ЖП";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(16, 49);
            label9.Name = "label9";
            label9.Size = new Size(46, 15);
            label9.TabIndex = 3;
            label9.Text = @"Жилая";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(textBoxDescription);
            groupBox5.Dock = DockStyle.Fill;
            groupBox5.Location = new Point(387, 378);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(379, 104);
            groupBox5.TabIndex = 4;
            groupBox5.TabStop = false;
            groupBox5.Text = @"Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Location = new Point(3, 17);
            textBoxDescription.MaxLength = 255;
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(373, 84);
            textBoxDescription.TabIndex = 0;
            textBoxDescription.TextChanged += textBoxDescription_TextChanged;
            textBoxDescription.Enter += selectAll_Enter;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(panel3);
            groupBox6.Controls.Add(dataGridViewRestrictions);
            groupBox6.Dock = DockStyle.Fill;
            groupBox6.Location = new Point(3, 488);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(378, 105);
            groupBox6.TabIndex = 5;
            groupBox6.TabStop = false;
            groupBox6.Text = @"Реквизиты";
            // 
            // panel3
            // 
            panel3.Controls.Add(vButtonRestrictionEdit);
            panel3.Controls.Add(vButtonRestrictionDelete);
            panel3.Controls.Add(vButtonRestrictionAdd);
            panel3.Dock = DockStyle.Right;
            panel3.Location = new Point(337, 17);
            panel3.Margin = new Padding(0);
            panel3.Name = "panel3";
            panel3.Size = new Size(38, 85);
            panel3.TabIndex = 2;
            // 
            // vButtonRestrictionEdit
            // 
            vButtonRestrictionEdit.AllowAnimations = true;
            vButtonRestrictionEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonRestrictionEdit.BackColor = Color.Transparent;
            vButtonRestrictionEdit.Image = ((Image)(resources.GetObject("vButtonRestrictionEdit.Image")));
            vButtonRestrictionEdit.Location = new Point(3, 57);
            vButtonRestrictionEdit.Name = "vButtonRestrictionEdit";
            vButtonRestrictionEdit.RoundedCornersMask = 15;
            vButtonRestrictionEdit.Size = new Size(32, 25);
            vButtonRestrictionEdit.TabIndex = 2;
            vButtonRestrictionEdit.UseVisualStyleBackColor = false;
            vButtonRestrictionEdit.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonRestrictionEdit.Click += vButtonRestrictionEdit_Click;
            // 
            // vButtonRestrictionDelete
            // 
            vButtonRestrictionDelete.AllowAnimations = true;
            vButtonRestrictionDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonRestrictionDelete.BackColor = Color.Transparent;
            vButtonRestrictionDelete.Image = ((Image)(resources.GetObject("vButtonRestrictionDelete.Image")));
            vButtonRestrictionDelete.Location = new Point(3, 30);
            vButtonRestrictionDelete.Name = "vButtonRestrictionDelete";
            vButtonRestrictionDelete.RoundedCornersMask = 15;
            vButtonRestrictionDelete.Size = new Size(32, 25);
            vButtonRestrictionDelete.TabIndex = 1;
            vButtonRestrictionDelete.UseVisualStyleBackColor = false;
            vButtonRestrictionDelete.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonRestrictionDelete.Click += vButtonRestrictionDelete_Click;
            // 
            // vButtonRestrictionAdd
            // 
            vButtonRestrictionAdd.AllowAnimations = true;
            vButtonRestrictionAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonRestrictionAdd.BackColor = Color.Transparent;
            vButtonRestrictionAdd.Image = ((Image)(resources.GetObject("vButtonRestrictionAdd.Image")));
            vButtonRestrictionAdd.Location = new Point(3, 3);
            vButtonRestrictionAdd.Name = "vButtonRestrictionAdd";
            vButtonRestrictionAdd.RoundedCornersMask = 15;
            vButtonRestrictionAdd.Size = new Size(32, 25);
            vButtonRestrictionAdd.TabIndex = 0;
            vButtonRestrictionAdd.UseVisualStyleBackColor = false;
            vButtonRestrictionAdd.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonRestrictionAdd.Click += vButtonRestrictionAdd_Click;
            // 
            // dataGridViewRestrictions
            // 
            dataGridViewRestrictions.AllowUserToAddRows = false;
            dataGridViewRestrictions.AllowUserToDeleteRows = false;
            dataGridViewRestrictions.AllowUserToResizeRows = false;
            dataGridViewRestrictions.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) 
                                               | AnchorStyles.Left) 
                                              | AnchorStyles.Right;
            dataGridViewRestrictions.BackgroundColor = Color.White;
            dataGridViewRestrictions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewRestrictions.Columns.AddRange(restriction_number, restriction_date, restriction_description, id_restriction_type);
            dataGridViewRestrictions.Location = new Point(3, 17);
            dataGridViewRestrictions.Name = "dataGridViewRestrictions";
            dataGridViewRestrictions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewRestrictions.Size = new Size(333, 85);
            dataGridViewRestrictions.TabIndex = 0;
            dataGridViewRestrictions.CellDoubleClick += dataGridViewRestrictions_CellDoubleClick;
            dataGridViewRestrictions.Resize += dataGridViewRestrictions_Resize;
            // 
            // restriction_number
            // 
            restriction_number.HeaderText = @"Номер";
            restriction_number.MinimumWidth = 100;
            restriction_number.Name = "restriction_number";
            restriction_number.ReadOnly = true;
            // 
            // restriction_date
            // 
            restriction_date.HeaderText = @"Дата";
            restriction_date.MinimumWidth = 100;
            restriction_date.Name = "restriction_date";
            restriction_date.ReadOnly = true;
            // 
            // restriction_description
            // 
            restriction_description.HeaderText = @"Наименование";
            restriction_description.MinimumWidth = 200;
            restriction_description.Name = "restriction_description";
            restriction_description.ReadOnly = true;
            restriction_description.Width = 200;
            // 
            // id_restriction_type
            // 
            id_restriction_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_restriction_type.HeaderText = @"Тип права собственности";
            id_restriction_type.MinimumWidth = 200;
            id_restriction_type.Name = "id_restriction_type";
            id_restriction_type.ReadOnly = true;
            id_restriction_type.Width = 200;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(panel4);
            groupBox7.Controls.Add(dataGridViewOwnerships);
            groupBox7.Dock = DockStyle.Fill;
            groupBox7.Location = new Point(387, 488);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(379, 105);
            groupBox7.TabIndex = 6;
            groupBox7.TabStop = false;
            groupBox7.Text = @"Ограничения";
            // 
            // panel4
            // 
            panel4.Controls.Add(vButtonOwnershipEdit);
            panel4.Controls.Add(vButtonOwnershipDelete);
            panel4.Controls.Add(vButtonOwnershipAdd);
            panel4.Dock = DockStyle.Right;
            panel4.Location = new Point(338, 17);
            panel4.Margin = new Padding(0);
            panel4.Name = "panel4";
            panel4.Size = new Size(38, 85);
            panel4.TabIndex = 3;
            // 
            // vButtonOwnershipEdit
            // 
            vButtonOwnershipEdit.AllowAnimations = true;
            vButtonOwnershipEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonOwnershipEdit.BackColor = Color.Transparent;
            vButtonOwnershipEdit.Image = ((Image)(resources.GetObject("vButtonOwnershipEdit.Image")));
            vButtonOwnershipEdit.Location = new Point(3, 57);
            vButtonOwnershipEdit.Name = "vButtonOwnershipEdit";
            vButtonOwnershipEdit.RoundedCornersMask = 15;
            vButtonOwnershipEdit.Size = new Size(32, 25);
            vButtonOwnershipEdit.TabIndex = 2;
            vButtonOwnershipEdit.UseVisualStyleBackColor = false;
            vButtonOwnershipEdit.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonOwnershipEdit.Click += vButtonOwnershipEdit_Click;
            // 
            // vButtonOwnershipDelete
            // 
            vButtonOwnershipDelete.AllowAnimations = true;
            vButtonOwnershipDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonOwnershipDelete.BackColor = Color.Transparent;
            vButtonOwnershipDelete.Image = ((Image)(resources.GetObject("vButtonOwnershipDelete.Image")));
            vButtonOwnershipDelete.Location = new Point(3, 30);
            vButtonOwnershipDelete.Name = "vButtonOwnershipDelete";
            vButtonOwnershipDelete.RoundedCornersMask = 15;
            vButtonOwnershipDelete.Size = new Size(32, 25);
            vButtonOwnershipDelete.TabIndex = 1;
            vButtonOwnershipDelete.UseVisualStyleBackColor = false;
            vButtonOwnershipDelete.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonOwnershipDelete.Click += vButtonOwnershipDelete_Click;
            // 
            // vButtonOwnershipAdd
            // 
            vButtonOwnershipAdd.AllowAnimations = true;
            vButtonOwnershipAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonOwnershipAdd.BackColor = Color.Transparent;
            vButtonOwnershipAdd.Image = ((Image)(resources.GetObject("vButtonOwnershipAdd.Image")));
            vButtonOwnershipAdd.Location = new Point(3, 3);
            vButtonOwnershipAdd.Name = "vButtonOwnershipAdd";
            vButtonOwnershipAdd.RoundedCornersMask = 15;
            vButtonOwnershipAdd.Size = new Size(32, 25);
            vButtonOwnershipAdd.TabIndex = 0;
            vButtonOwnershipAdd.UseVisualStyleBackColor = false;
            vButtonOwnershipAdd.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonOwnershipAdd.Click += vButtonOwnershipAdd_Click;
            // 
            // dataGridViewOwnerships
            // 
            dataGridViewOwnerships.AllowUserToAddRows = false;
            dataGridViewOwnerships.AllowUserToDeleteRows = false;
            dataGridViewOwnerships.AllowUserToResizeRows = false;
            dataGridViewOwnerships.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) 
                                             | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            dataGridViewOwnerships.BackgroundColor = Color.White;
            dataGridViewOwnerships.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewOwnerships.Columns.AddRange(ownership_number, ownership_date, ownership_description, id_ownership_type);
            dataGridViewOwnerships.Location = new Point(3, 17);
            dataGridViewOwnerships.Name = "dataGridViewOwnerships";
            dataGridViewOwnerships.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOwnerships.Size = new Size(334, 85);
            dataGridViewOwnerships.TabIndex = 0;
            dataGridViewOwnerships.CellDoubleClick += dataGridViewOwnerships_CellDoubleClick;
            dataGridViewOwnerships.Resize += dataGridViewOwnerships_Resize;
            // 
            // ownership_number
            // 
            ownership_number.HeaderText = @"Номер";
            ownership_number.MinimumWidth = 100;
            ownership_number.Name = "ownership_number";
            ownership_number.ReadOnly = true;
            // 
            // ownership_date
            // 
            ownership_date.HeaderText = @"Дата";
            ownership_date.MinimumWidth = 100;
            ownership_date.Name = "ownership_date";
            ownership_date.ReadOnly = true;
            // 
            // ownership_description
            // 
            ownership_description.HeaderText = @"Наименование";
            ownership_description.MinimumWidth = 200;
            ownership_description.Name = "ownership_description";
            ownership_description.ReadOnly = true;
            ownership_description.Width = 200;
            // 
            // id_ownership_type
            // 
            id_ownership_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_ownership_type.HeaderText = @"Тип ограничения";
            id_ownership_type.MinimumWidth = 200;
            id_ownership_type.Name = "id_ownership_type";
            id_ownership_type.ReadOnly = true;
            id_ownership_type.Width = 200;
            // 
            // BuildingViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(630, 570);
            BackColor = Color.White;
            ClientSize = new Size(775, 602);
            Controls.Add(tableLayoutPanel);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "BuildingViewport";
            Padding = new Padding(3);
            Text = @"Здание";
            tableLayoutPanel.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((ISupportInitialize)(numericUpDownWear)).EndInit();
            ((ISupportInitialize)(numericUpDownFloors)).EndInit();
            ((ISupportInitialize)(numericUpDownStartupYear)).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((ISupportInitialize)(numericUpDownBalanceCost)).EndInit();
            ((ISupportInitialize)(numericUpDownCadastralCost)).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((ISupportInitialize)(numericUpDownPremisesCount)).EndInit();
            ((ISupportInitialize)(numericUpDownRoomsCount)).EndInit();
            ((ISupportInitialize)(numericUpDownApartmentsCount)).EndInit();
            ((ISupportInitialize)(numericUpDownSharedApartmentsCount)).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((ISupportInitialize)(numericUpDownOtherPremisesCount)).EndInit();
            ((ISupportInitialize)(numericUpDownSpecialPremisesCount)).EndInit();
            ((ISupportInitialize)(numericUpDownCommercialPremisesCount)).EndInit();
            ((ISupportInitialize)(numericUpDownSocialPremisesCount)).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((ISupportInitialize)(numericUpDownTotalArea)).EndInit();
            ((ISupportInitialize)(numericUpDownLivingArea)).EndInit();
            ((ISupportInitialize)(numericUpDownMunicipalArea)).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox6.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewRestrictions)).EndInit();
            groupBox7.ResumeLayout(false);
            panel4.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewOwnerships)).EndInit();
            ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
