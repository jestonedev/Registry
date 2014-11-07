﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;
using System.Drawing;
using Registry.SearchForms;
using Registry.CalcDataModels;

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
        #endregion Components

        #region Models
        private BuildingsDataModel buildings = null;
        private KladrStreetsDataModel kladr = null;
        private StructureTypesDataModel structureTypes = null;
        private RestrictionsDataModel restrictions = null;
        private RestrictionTypesDataModel restrictionTypes = null;
        private RestrictionsBuildingsAssocDataModel restrictionBuildingsAssoc = null;
        private OwnershipsRightsDataModel ownershipRights = null;
        private OwnershipRightTypesDataModel ownershipRightTypes = null;
        private OwnershipBuildingsAssocDataModel ownershipBuildingsAssoc = null;
        private FundTypesDataModel fundTypes = null;
        private ObjectStatesDataModel object_states = null;
        private CalcDataModelBuildingsPremisesFunds buildingsPremisesFunds = null;
        private CalcDataModelBuildingsCurrentFunds buildingsCurrentFund = null;
        private CalcDataModelBuildingsPremisesSumArea buildingsPremisesSumArea = null;
        #endregion Models

        #region Views
        private BindingSource v_buildings = null;
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
        private BindingSource v_buildingsPremisesFunds = null;
        private BindingSource v_buildingsCurrentFund = null;
        private BindingSource v_buildingsPremisesSumArea = null;
        #endregion Views

        //Forms
        private SearchForm sbSimpleSearchForm = null;
        private SearchForm sbExtendedSearchForm = null;
        
        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private DataGridViewTextBoxColumn restriction_number;
        private DataGridViewTextBoxColumn restriction_date;
        private DataGridViewTextBoxColumn restriction_description;
        private DataGridViewComboBoxColumn id_restriction_type;
        private DataGridViewTextBoxColumn ownership_number;
        private DataGridViewTextBoxColumn ownership_date;
        private DataGridViewTextBoxColumn ownership_description;
        private DataGridViewComboBoxColumn id_ownership_type;

        private bool is_editable = false;

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
            if (v_buildingsPremisesFunds != null)
            {
                if ((v_buildings.Position != -1) && !(((DataRowView)v_buildings[v_buildings.Position])["id_building"] is DBNull))
                    v_buildingsPremisesFunds.Filter = "id_building = " + ((DataRowView)v_buildings[v_buildings.Position])["id_building"].ToString();
                else
                    v_buildingsPremisesFunds.Filter = "id_building = 0";
            }
            if (v_buildingsCurrentFund != null)
            {
                if ((v_buildings.Position != -1) && !(((DataRowView)v_buildings[v_buildings.Position])["id_building"] is DBNull))
                    v_buildingsCurrentFund.Filter = "id_building = " + ((DataRowView)v_buildings[v_buildings.Position])["id_building"].ToString();
                else
                    v_buildingsCurrentFund.Filter = "id_building = 0";
                ShowOrHideCurrentFund();
            }
            if (v_buildingsPremisesSumArea != null)
            {
                if ((v_buildings.Position != -1) && !(((DataRowView)v_buildings[v_buildings.Position])["id_building"] is DBNull))
                    v_buildingsPremisesSumArea.Filter = "id_building = " + ((DataRowView)v_buildings[v_buildings.Position])["id_building"].ToString();
                else
                    v_buildingsPremisesSumArea.Filter = "id_building = 0";
            }
        }

        private void ShowOrHideCurrentFund()
        {
            if (v_buildingsCurrentFund.Count > 0)
            {
                label19.Visible = true;
                comboBoxCurrentFundType.Visible = true;
                checkBoxImprovement.Location = new System.Drawing.Point(159, 154);
                checkBoxElevator.Location = new System.Drawing.Point(19, 154);
                this.tableLayoutPanel.RowStyles[0].Height = 210F;
            }
            else
            {
                label19.Visible = false;
                comboBoxCurrentFundType.Visible = false;
                checkBoxImprovement.Location = new System.Drawing.Point(159, 125);
                checkBoxElevator.Location = new System.Drawing.Point(19, 125);
                this.tableLayoutPanel.RowStyles[0].Height = 185F;
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

            comboBoxStructureType.DataSource = v_structureTypes;
            comboBoxStructureType.ValueMember = "id_structure_type";
            comboBoxStructureType.DisplayMember = "structure_type";
            comboBoxStructureType.DataBindings.Clear();
            comboBoxStructureType.DataBindings.Add("SelectedValue", v_buildings, "id_structure_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxCurrentFundType.DataSource = v_fundType;
            comboBoxCurrentFundType.ValueMember = "id_fund_type";
            comboBoxCurrentFundType.DisplayMember = "fund_type";
            comboBoxCurrentFundType.DataBindings.Clear();
            comboBoxCurrentFundType.DataBindings.Add("SelectedValue", v_buildingsCurrentFund, "id_fund_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxState.DataSource = v_object_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";
            comboBoxState.DataBindings.Clear();
            comboBoxState.DataBindings.Add("SelectedValue", v_buildings, "id_state", true, DataSourceUpdateMode.Never, DBNull.Value);

            numericUpDownSocialPremisesCount.DataBindings.Clear();
            numericUpDownSocialPremisesCount.DataBindings.Add("Minimum", v_buildingsPremisesFunds, "social_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSocialPremisesCount.DataBindings.Add("Maximum", v_buildingsPremisesFunds, "social_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSocialPremisesCount.DataBindings.Add("Value", v_buildingsPremisesFunds, "social_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownCommercialPremisesCount.DataBindings.Clear();
            numericUpDownCommercialPremisesCount.DataBindings.Add("Minimum", v_buildingsPremisesFunds, "commercial_premises_count", 
                true, DataSourceUpdateMode.Never, 0);
            numericUpDownCommercialPremisesCount.DataBindings.Add("Maximum", v_buildingsPremisesFunds, "commercial_premises_count", 
                true, DataSourceUpdateMode.Never, 0);
            numericUpDownCommercialPremisesCount.DataBindings.Add("Value", v_buildingsPremisesFunds, "commercial_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSpecialPremisesCount.DataBindings.Clear();
            numericUpDownSpecialPremisesCount.DataBindings.Add("Minimum", v_buildingsPremisesFunds, "special_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSpecialPremisesCount.DataBindings.Add("Maximum", v_buildingsPremisesFunds, "special_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSpecialPremisesCount.DataBindings.Add("Value", v_buildingsPremisesFunds, "special_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownOtherPremisesCount.DataBindings.Clear();
            numericUpDownOtherPremisesCount.DataBindings.Add("Minimum", v_buildingsPremisesFunds, "other_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownOtherPremisesCount.DataBindings.Add("Maximum", v_buildingsPremisesFunds, "other_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownOtherPremisesCount.DataBindings.Add("Value", v_buildingsPremisesFunds, "other_premises_count", true, DataSourceUpdateMode.Never, 0);

            numericUpDownMunicipalArea.DataBindings.Clear();
            numericUpDownMunicipalArea.DataBindings.Add("Minimum", v_buildingsPremisesSumArea, "sum_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownMunicipalArea.DataBindings.Add("Maximum", v_buildingsPremisesSumArea, "sum_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownMunicipalArea.DataBindings.Add("Value", v_buildingsPremisesSumArea, "sum_area", true, DataSourceUpdateMode.Never, 0);

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
            if (!this.ContainsFocus)
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
            menuCallback.EditingStateUpdate();
        }

        private bool ChangeViewportStateTo(ViewportState state)
        {
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
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                            if (buildings.EditingNewRecord)
                                return false;
                            else
                            {
                                viewportState = ViewportState.NewRowState;
                                return true;
                            }
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
            if (building.id_street == null)
            {
                MessageBox.Show("Необходимо выбрать улицу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxStreet.Focus();
                return false;
            }
            if (building.id_state == null)
            {
                MessageBox.Show("Необходимо выбрать состояние здания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxState.Focus();
                return false;
            }
            if ((building.house == null) || (building.house.Trim() == ""))
            {
                MessageBox.Show("Необходимо указать номер дома", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxHouse.Focus();
                return false;
            }
            if (building.id_structure_type == null)
            {
                MessageBox.Show("Необходимо выбрать материал здания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxStructureType.Focus();
                return false;
            }
            return true;
        }

        public void LocateBuildingBy(int id)
        {
            int Position = v_buildings.Find("id_building", id);
            is_editable = false;
            if (Position > 0)
                v_buildings.Position = Position;
            is_editable = true;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
                this.Text = "Новое здание";
            else
                if (v_buildings.Position != -1)
                    this.Text = String.Format("Здание №{0}", ((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
                else
                    this.Text = "Здания отсутствуют";
        }

        private Building BuildingFromViewport()
        {
            Building building = new Building();
            if (v_buildings.Position == -1)
                building.id_building = null;
            else
                building.id_building = ViewportHelper.ValueOrNull<int>((DataRowView)v_buildings[v_buildings.Position],"id_building");
            building.id_street = ViewportHelper.ValueOrNull(comboBoxStreet);
            building.id_state = ViewportHelper.ValueOrNull<int>(comboBoxState);
            building.house = ViewportHelper.ValueOrNull(textBoxHouse);
            building.id_structure_type = ViewportHelper.ValueOrNull<int>(comboBoxStructureType);
            building.cadastral_num = ViewportHelper.ValueOrNull(textBoxCadastralNum);
            building.description = ViewportHelper.ValueOrNull(textBoxDescription);
            building.floors = Convert.ToInt16(numericUpDownFloors.Value);
            building.startup_year = Convert.ToInt32(numericUpDownStartupYear.Value);
            building.cadastral_cost = numericUpDownCadastralCost.Value;
            building.balance_cost = numericUpDownBalanceCost.Value;
            building.improvement = checkBoxImprovement.Checked;
            building.elevator = checkBoxElevator.Checked;
            building.num_premises = Convert.ToInt32(numericUpDownPremisesCount.Value);
            building.num_rooms = Convert.ToInt32(numericUpDownRoomsCount.Value);
            building.num_apartments = Convert.ToInt32(numericUpDownApartmentsCount.Value);
            building.num_shared_apartments = Convert.ToInt32(numericUpDownSharedApartmentsCount.Value);
            building.living_area = Convert.ToDouble(numericUpDownLivingArea.Value);
            building.total_area = Convert.ToDouble(numericUpDownTotalArea.Value);
            return building;
        }

        private Building BuildingFromView()
        {
            Building building = new Building();
            DataRowView row = (DataRowView)v_buildings[v_buildings.Position];
            building.id_building = ViewportHelper.ValueOrNull<int>(row, "id_building");
            building.id_street = ViewportHelper.ValueOrNull(row, "id_street");
            building.id_state = ViewportHelper.ValueOrNull<int>(row, "id_state");
            building.id_structure_type = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
            building.house = ViewportHelper.ValueOrNull(row, "house");
            building.floors = ViewportHelper.ValueOrNull<short>(row, "floors");
            building.num_premises = ViewportHelper.ValueOrNull<int>(row, "num_premises");
            building.num_rooms = ViewportHelper.ValueOrNull<int>(row, "num_rooms");
            building.num_apartments = ViewportHelper.ValueOrNull<int>(row, "num_apartments");
            building.num_shared_apartments = ViewportHelper.ValueOrNull<int>(row, "num_shared_apartments");
            building.living_area = ViewportHelper.ValueOrNull<double>(row, "living_area");
            building.total_area = ViewportHelper.ValueOrNull<double>(row, "total_area");
            building.cadastral_num = ViewportHelper.ValueOrNull(row, "cadastral_num");
            building.cadastral_cost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost");
            building.balance_cost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost");
            building.description = ViewportHelper.ValueOrNull(row, "description");
            building.startup_year = ViewportHelper.ValueOrNull<int>(row, "startup_year");
            building.improvement = ViewportHelper.ValueOrNull<bool>(row, "improvement");
            building.elevator = ViewportHelper.ValueOrNull<bool>(row, "elevator");
            return building;
        }

        private void ViewportFromBuilding(Building building)
        {
            comboBoxStreet.SelectedValue = ViewportHelper.ValueOrDBNull(building.id_street);
            comboBoxState.SelectedValue = ViewportHelper.ValueOrDBNull(building.id_state);
            comboBoxStructureType.SelectedValue = ViewportHelper.ValueOrDBNull(building.id_structure_type);
            numericUpDownFloors.Value = ViewportHelper.ValueOrDefault(building.floors);
            numericUpDownStartupYear.Value = ViewportHelper.ValueOrDefault(building.startup_year);
            checkBoxImprovement.Checked = ViewportHelper.ValueOrDefault(building.improvement);
            checkBoxElevator.Checked = ViewportHelper.ValueOrDefault(building.elevator);
            numericUpDownPremisesCount.Value = ViewportHelper.ValueOrDefault(building.num_premises);
            numericUpDownApartmentsCount.Value = ViewportHelper.ValueOrDefault(building.num_apartments);
            numericUpDownSharedApartmentsCount.Value = ViewportHelper.ValueOrDefault(building.num_shared_apartments);
            numericUpDownLivingArea.Value = (decimal)ViewportHelper.ValueOrDefault(building.living_area);
            numericUpDownTotalArea.Value = (decimal)ViewportHelper.ValueOrDefault(building.total_area);
            numericUpDownCadastralCost.Value = ViewportHelper.ValueOrDefault(building.cadastral_cost);
            numericUpDownBalanceCost.Value = ViewportHelper.ValueOrDefault(building.balance_cost);
            textBoxHouse.Text = building.house;
            textBoxCadastralNum.Text = building.cadastral_num;
            textBoxDescription.Text = building.description;
        }

        private static void FillRowFromBuilding(Building building, DataRowView row)
        {
            row.BeginEdit();
            row["id_building"] = ViewportHelper.ValueOrDBNull(building.id_building);
            row["id_state"] = ViewportHelper.ValueOrDBNull(building.id_state);
            row["id_structure_type"] = ViewportHelper.ValueOrDBNull(building.id_structure_type);
            row["id_street"] = ViewportHelper.ValueOrDBNull(building.id_street);
            row["house"] = ViewportHelper.ValueOrDBNull(building.house);
            row["floors"] = ViewportHelper.ValueOrDBNull(building.floors);
            row["num_premises"] = ViewportHelper.ValueOrDBNull(building.num_premises);
            row["num_rooms"] = ViewportHelper.ValueOrDBNull(building.num_rooms);
            row["num_apartments"] = ViewportHelper.ValueOrDBNull(building.num_apartments);
            row["num_shared_apartments"] = ViewportHelper.ValueOrDBNull(building.num_shared_apartments);
            row["cadastral_num"] = ViewportHelper.ValueOrDBNull(building.cadastral_num);
            row["cadastral_cost"] = ViewportHelper.ValueOrDBNull(building.cadastral_cost);
            row["balance_cost"] = ViewportHelper.ValueOrDBNull(building.balance_cost);
            row["description"] = ViewportHelper.ValueOrDBNull(building.description);
            row["startup_year"] = ViewportHelper.ValueOrDBNull(building.startup_year);
            row["improvement"] = ViewportHelper.ValueOrDBNull(building.improvement);
            row["elevator"] = ViewportHelper.ValueOrDBNull(building.elevator);
            row["living_area"] = ViewportHelper.ValueOrDBNull(building.living_area);
            row["total_area"] = ViewportHelper.ValueOrDBNull(building.total_area);
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
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

            DataSet ds = DataSetManager.GetDataSet();

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

            v_buildingsPremisesFunds = new BindingSource();
            v_buildingsPremisesFunds.DataMember = "buildings_premises_funds";
            v_buildingsPremisesFunds.DataSource = buildingsPremisesFunds.Select();

            v_buildingsCurrentFund = new BindingSource();
            v_buildingsCurrentFund.DataMember = "buildings_current_funds";
            v_buildingsCurrentFund.DataSource = buildingsCurrentFund.Select();

            v_buildingsPremisesSumArea = new BindingSource();
            v_buildingsPremisesSumArea.DataMember = "buildings_sum_area";
            v_buildingsPremisesSumArea.DataSource = buildingsPremisesSumArea.Select();

            v_fundType = new BindingSource();
            v_fundType.DataMember = "fund_types";
            v_fundType.DataSource = ds;

            v_object_states = new BindingSource();
            v_object_states.DataMember = "object_states";
            v_object_states.DataSource = ds;

            v_buildings = new BindingSource();
            v_buildings.CurrentItemChanged += new EventHandler(v_buildings_CurrentItemChanged);
            v_buildings.DataMember = "buildings";
            v_buildings.DataSource = ds;
            v_buildings.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_buildings.Filter += " AND ";
            v_buildings.Filter += DynamicFilter;

            v_restrictionBuildingsAssoc = new BindingSource();
            v_restrictionBuildingsAssoc.CurrentItemChanged += new EventHandler(v_restrictionBuildingsAssoc_CurrentItemChanged);
            v_restrictionBuildingsAssoc.DataMember = "buildings_restrictions_buildings_assoc";
            v_restrictionBuildingsAssoc.DataSource = v_buildings;
            RestrictionsFilterRebuild();
            restrictionBuildingsAssoc.Select().RowChanged += new DataRowChangeEventHandler(RestrictionsAssoc_RowChanged);
            restrictionBuildingsAssoc.Select().RowDeleted += new DataRowChangeEventHandler(RestrictionsAssoc_RowDeleted);

            v_ownershipBuildingsAssoc = new BindingSource();
            v_ownershipBuildingsAssoc.CurrentItemChanged += new EventHandler(v_ownershipBuildingsAssoc_CurrentItemChanged);
            v_ownershipBuildingsAssoc.DataMember = "buildings_ownership_buildings_assoc";
            v_ownershipBuildingsAssoc.DataSource = v_buildings;
            v_ownershipBuildingsAssoc_CurrentItemChanged(null, new EventArgs());
            OwnershipsFilterRebuild();
            ownershipBuildingsAssoc.Select().RowChanged += new DataRowChangeEventHandler(OwnershipsAssoc_RowChanged);
            ownershipBuildingsAssoc.Select().RowDeleted += new DataRowChangeEventHandler(OwnershipsAssoc_RowDeleted);

            DataBind();

            buildingsCurrentFund.RefreshEvent += new EventHandler<EventArgs>(buildingsCurrentFund_RefreshEvent);
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
            if (DynamicFilter != "")
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
            if (StaticFilter != "" && DynamicFilter != "")
                Filter += " AND ";
            Filter += DynamicFilter;
            is_editable = false;
            v_buildings.Filter = Filter;
            is_editable = true;
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void SaveRecord()
        {
            Building building = BuildingFromViewport();
            bool updatePremisesState = false;
            if (!ValidateBuilding(building))
                return;
            if ((viewportState == ViewportState.ModifyRowState) && (building.id_state != BuildingFromView().id_state))
            {
                if (MessageBox.Show("Вы пытаетесь изменить состояние здания. В результате всем помещениям данного здания будет назначено то же состояние. " +
                    "Вы уверены, что хотите сохранить данные?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
                updatePremisesState = true;
            }
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ViewportState.NewRowState:
                    int id_building = buildings.Insert(building);
                    if (id_building == -1)
                        return;
                    DataRowView newRow;
                    building.id_building = id_building;
                    is_editable = false;
                    if (v_buildings.Position == -1)
                        newRow = (DataRowView)v_buildings.AddNew();
                    else
                        newRow = ((DataRowView)v_buildings[v_buildings.Position]);
                    FillRowFromBuilding(building, newRow);
                    is_editable = true;
                    buildings.EditingNewRecord = false;
                    this.Text = "Здание №" + id_building.ToString();
                    viewportState = ViewportState.ReadState;
                    is_editable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (building.id_building == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить здание без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (buildings.Update(building) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_buildings[v_buildings.Position]);
                    is_editable = false;
                    FillRowFromBuilding(building, row);
                    is_editable = true;
                    if (updatePremisesState)
                    {
                        if (DataSetManager.GetDataSet().Tables.Contains("premises"))
                        {
                            DataTable premises = DataSetManager.GetDataSet().Tables["premises"];
                            List<int> idPremises = new List<int>(); //Идентификаторы помещений, включенных в каскадное обновление состояний
                            for (int i = 0; i < premises.Rows.Count; i++)
                            {
                                if ((premises.Rows[i]["id_building"] != DBNull.Value) && ((int)premises.Rows[i]["id_building"] == building.id_building))
                                {
                                    premises.Rows[i]["id_state"] = building.id_state;
                                    premises.Rows[i].EndEdit();
                                    idPremises.Add((int)premises.Rows[i]["id_premises"]);
                                }
                            }
                            if (DataSetManager.GetDataSet().Tables.Contains("sub_premises"))
                            {
                                DataTable sub_premises = DataSetManager.GetDataSet().Tables["sub_premises"];
                                for (int i = 0; i < sub_premises.Rows.Count; i++)
                                {
                                    if ((sub_premises.Rows[i]["id_premises"] != DBNull.Value) &&
                                        (idPremises.Contains((int)sub_premises.Rows[i]["id_premises"])))
                                    {
                                        sub_premises.Rows[i]["id_state"] = building.id_state;
                                        sub_premises.Rows[i].EndEdit();
                                    }
                                }
                            }
                        }
                    }
                    viewportState = ViewportState.ReadState;
                    CalcDataModeTenancyAggregated.GetInstance().Refresh(CalcDataModelFilterEnity.Building, building.id_building);
                    break;
            }
            menuCallback.EditingStateUpdate();
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
                        is_editable = true;
                    }
                    else
                        this.Text = "Здания отсутствуют";
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    v_kladr.Filter = "";
                    is_editable = false;
                    DataBind();
                    is_editable = true;
                    viewportState = ViewportState.ReadState;
                    break;
            }
            menuCallback.EditingStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            if ((viewportState  == ViewportState.ReadState || viewportState == ViewportState.ModifyRowState) && !buildings.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            DataRowView row = (DataRowView)v_buildings.AddNew();
            is_editable = true;
            buildings.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return ((v_buildings.Position != -1) && (!buildings.EditingNewRecord));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            Building building = BuildingFromView();
            DataRowView row = (DataRowView)v_buildings.AddNew();
            buildings.EditingNewRecord = true;
            ViewportFromBuilding(building);
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            if ((v_buildings.Position == -1) || (viewportState == ViewportState.NewRowState))
                return false;
            else
                return true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это здание?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (buildings.Delete((int)((DataRowView)v_buildings.Current)["id_building"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_buildings[v_buildings.Position]).Delete();
                is_editable = true;
                menuCallback.ForceCloseDetachedViewports();
                if (ParentType == ParentTypeEnum.Tenancy)
                    CalcDataModeTenancyAggregated.GetInstance().Refresh(CalcDataModelFilterEnity.All, null);
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            BuildingViewport viewport = new BuildingViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
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

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ShowAssocViewport(menuCallback, viewportType,
                "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]),
                ((DataRowView)v_buildings[v_buildings.Position]).Row,
                ParentTypeEnum.Building);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                restrictionBuildingsAssoc.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionsAssoc_RowChanged);
                restrictionBuildingsAssoc.Select().RowDeleted -= new DataRowChangeEventHandler(RestrictionsAssoc_RowDeleted);
                ownershipBuildingsAssoc.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipsAssoc_RowChanged);
                ownershipBuildingsAssoc.Select().RowDeleted -= new DataRowChangeEventHandler(OwnershipsAssoc_RowDeleted);
                buildingsCurrentFund.RefreshEvent -= new EventHandler<EventArgs>(buildingsCurrentFund_RefreshEvent);
            }
        }

        void buildingsCurrentFund_RefreshEvent(object sender, EventArgs e)
        {
            ShowOrHideCurrentFund();
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
                menuCallback.NavigationStateUpdate();
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

        void comboBoxStreet_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back))
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
        
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildingViewport));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.numericUpDownFloors = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStartupYear = new System.Windows.Forms.NumericUpDown();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.comboBoxStructureType = new System.Windows.Forms.ComboBox();
            this.textBoxHouse = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label40 = new System.Windows.Forms.Label();
            this.comboBoxState = new System.Windows.Forms.ComboBox();
            this.comboBoxCurrentFundType = new System.Windows.Forms.ComboBox();
            this.numericUpDownBalanceCost = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownCadastralCost = new System.Windows.Forms.NumericUpDown();
            this.checkBoxImprovement = new System.Windows.Forms.CheckBox();
            this.checkBoxElevator = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.textBoxCadastralNum = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRoomsCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownApartmentsCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSharedApartmentsCount = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDownOtherPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSpecialPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownCommercialPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSocialPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownTotalArea = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.numericUpDownLivingArea = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMunicipalArea = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.dataGridViewRestrictions = new System.Windows.Forms.DataGridView();
            this.restriction_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_restriction_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dataGridViewOwnerships = new System.Windows.Forms.DataGridView();
            this.ownership_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_ownership_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tableLayoutPanel.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartupYear)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoomsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownApartmentsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSharedApartmentsCount)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOtherPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpecialPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCommercialPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSocialPremisesCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunicipalArea)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.groupBox4, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.groupBox3, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.groupBox5, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.groupBox6, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.groupBox7, 1, 3);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1002, 723);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.tableLayoutPanel.SetColumnSpan(this.groupBox4, 2);
            this.groupBox4.Controls.Add(this.tableLayoutPanel2);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(996, 204);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Общие сведения";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(990, 185);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.numericUpDownFloors);
            this.panel1.Controls.Add(this.numericUpDownStartupYear);
            this.panel1.Controls.Add(this.comboBoxStreet);
            this.panel1.Controls.Add(this.comboBoxStructureType);
            this.panel1.Controls.Add(this.textBoxHouse);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(489, 179);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Улица";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Номер дома";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Этажность дома";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 97);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(142, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Год ввода в эксплуатацию";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 126);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(134, 13);
            this.label18.TabIndex = 4;
            this.label18.Text = "Тип строения (материал)";
            // 
            // numericUpDownFloors
            // 
            this.numericUpDownFloors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownFloors.Location = new System.Drawing.Point(157, 66);
            this.numericUpDownFloors.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownFloors.Name = "numericUpDownFloors";
            this.numericUpDownFloors.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownFloors.TabIndex = 2;
            this.numericUpDownFloors.ValueChanged += new System.EventHandler(this.numericUpDownFloors_ValueChanged);
            // 
            // numericUpDownStartupYear
            // 
            this.numericUpDownStartupYear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownStartupYear.Location = new System.Drawing.Point(157, 95);
            this.numericUpDownStartupYear.Maximum = new decimal(new int[] {
            2014,
            0,
            0,
            0});
            this.numericUpDownStartupYear.Minimum = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            this.numericUpDownStartupYear.Name = "numericUpDownStartupYear";
            this.numericUpDownStartupYear.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownStartupYear.TabIndex = 3;
            this.numericUpDownStartupYear.Value = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            this.numericUpDownStartupYear.ValueChanged += new System.EventHandler(this.numericUpDownStartupYear_ValueChanged);
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(157, 7);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(328, 21);
            this.comboBoxStreet.TabIndex = 0;
            this.comboBoxStreet.SelectedIndexChanged += new System.EventHandler(this.comboBoxStreet_SelectedIndexChanged);
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // comboBoxStructureType
            // 
            this.comboBoxStructureType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStructureType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStructureType.FormattingEnabled = true;
            this.comboBoxStructureType.Location = new System.Drawing.Point(157, 123);
            this.comboBoxStructureType.Name = "comboBoxStructureType";
            this.comboBoxStructureType.Size = new System.Drawing.Size(328, 21);
            this.comboBoxStructureType.TabIndex = 4;
            this.comboBoxStructureType.SelectedIndexChanged += new System.EventHandler(this.comboBoxStructureType_SelectedIndexChanged);
            // 
            // textBoxHouse
            // 
            this.textBoxHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHouse.Location = new System.Drawing.Point(157, 36);
            this.textBoxHouse.MaxLength = 4;
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new System.Drawing.Size(328, 20);
            this.textBoxHouse.TabIndex = 1;
            this.textBoxHouse.TextChanged += new System.EventHandler(this.textBoxHouse_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label40);
            this.panel2.Controls.Add(this.comboBoxState);
            this.panel2.Controls.Add(this.comboBoxCurrentFundType);
            this.panel2.Controls.Add(this.numericUpDownBalanceCost);
            this.panel2.Controls.Add(this.numericUpDownCadastralCost);
            this.panel2.Controls.Add(this.checkBoxImprovement);
            this.panel2.Controls.Add(this.checkBoxElevator);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.label19);
            this.panel2.Controls.Add(this.textBoxCadastralNum);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(498, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(489, 179);
            this.panel2.TabIndex = 1;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(16, 98);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(108, 13);
            this.label40.TabIndex = 31;
            this.label40.Text = "Текущее состояние";
            // 
            // comboBoxState
            // 
            this.comboBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxState.FormattingEnabled = true;
            this.comboBoxState.Location = new System.Drawing.Point(159, 95);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new System.Drawing.Size(327, 21);
            this.comboBoxState.TabIndex = 3;
            this.comboBoxState.SelectedIndexChanged += new System.EventHandler(this.comboBoxState_SelectedIndexChanged);
            // 
            // comboBoxCurrentFundType
            // 
            this.comboBoxCurrentFundType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCurrentFundType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.comboBoxCurrentFundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCurrentFundType.Enabled = false;
            this.comboBoxCurrentFundType.ForeColor = System.Drawing.Color.Black;
            this.comboBoxCurrentFundType.FormattingEnabled = true;
            this.comboBoxCurrentFundType.Location = new System.Drawing.Point(159, 124);
            this.comboBoxCurrentFundType.Name = "comboBoxCurrentFundType";
            this.comboBoxCurrentFundType.Size = new System.Drawing.Size(328, 21);
            this.comboBoxCurrentFundType.TabIndex = 4;
            this.comboBoxCurrentFundType.Visible = false;
            // 
            // numericUpDownBalanceCost
            // 
            this.numericUpDownBalanceCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownBalanceCost.DecimalPlaces = 2;
            this.numericUpDownBalanceCost.Location = new System.Drawing.Point(159, 66);
            this.numericUpDownBalanceCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownBalanceCost.Name = "numericUpDownBalanceCost";
            this.numericUpDownBalanceCost.Size = new System.Drawing.Size(327, 20);
            this.numericUpDownBalanceCost.TabIndex = 2;
            this.numericUpDownBalanceCost.ThousandsSeparator = true;
            this.numericUpDownBalanceCost.ValueChanged += new System.EventHandler(this.numericUpDownBalanceCost_ValueChanged);
            // 
            // numericUpDownCadastralCost
            // 
            this.numericUpDownCadastralCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownCadastralCost.DecimalPlaces = 2;
            this.numericUpDownCadastralCost.Location = new System.Drawing.Point(159, 36);
            this.numericUpDownCadastralCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownCadastralCost.Name = "numericUpDownCadastralCost";
            this.numericUpDownCadastralCost.Size = new System.Drawing.Size(327, 20);
            this.numericUpDownCadastralCost.TabIndex = 1;
            this.numericUpDownCadastralCost.ThousandsSeparator = true;
            this.numericUpDownCadastralCost.ValueChanged += new System.EventHandler(this.numericUpDownCadastralCost_ValueChanged);
            // 
            // checkBoxImprovement
            // 
            this.checkBoxImprovement.AutoSize = true;
            this.checkBoxImprovement.Location = new System.Drawing.Point(159, 153);
            this.checkBoxImprovement.Name = "checkBoxImprovement";
            this.checkBoxImprovement.Size = new System.Drawing.Size(113, 17);
            this.checkBoxImprovement.TabIndex = 6;
            this.checkBoxImprovement.Text = "Благоустройство";
            this.checkBoxImprovement.UseVisualStyleBackColor = true;
            this.checkBoxImprovement.CheckedChanged += new System.EventHandler(this.checkBoxImprovement_CheckedChanged);
            // 
            // checkBoxElevator
            // 
            this.checkBoxElevator.AutoSize = true;
            this.checkBoxElevator.Location = new System.Drawing.Point(19, 153);
            this.checkBoxElevator.Name = "checkBoxElevator";
            this.checkBoxElevator.Size = new System.Drawing.Size(103, 17);
            this.checkBoxElevator.TabIndex = 5;
            this.checkBoxElevator.Text = "Наличие лифта";
            this.checkBoxElevator.UseVisualStyleBackColor = true;
            this.checkBoxElevator.CheckedChanged += new System.EventHandler(this.checkBoxElevator_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(16, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(110, 13);
            this.label14.TabIndex = 32;
            this.label14.Text = "Кадастровый номер";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(16, 39);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(130, 13);
            this.label15.TabIndex = 33;
            this.label15.Text = "Кадастровая стоимость";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 68);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(125, 13);
            this.label16.TabIndex = 34;
            this.label16.Text = "Балансовая стоимость";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(16, 126);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(107, 13);
            this.label19.TabIndex = 35;
            this.label19.Text = "Текущий тип найма";
            this.label19.Visible = false;
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCadastralNum.Location = new System.Drawing.Point(159, 7);
            this.textBoxCadastralNum.MaxLength = 15;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new System.Drawing.Size(327, 20);
            this.textBoxCadastralNum.TabIndex = 0;
            this.textBoxCadastralNum.TextChanged += new System.EventHandler(this.textBoxCadastralNum_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownPremisesCount);
            this.groupBox1.Controls.Add(this.numericUpDownRoomsCount);
            this.groupBox1.Controls.Add(this.numericUpDownApartmentsCount);
            this.groupBox1.Controls.Add(this.numericUpDownSharedApartmentsCount);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 213);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(495, 134);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Количество жилых помещений";
            // 
            // numericUpDownPremisesCount
            // 
            this.numericUpDownPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownPremisesCount.Location = new System.Drawing.Point(163, 18);
            this.numericUpDownPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPremisesCount.Name = "numericUpDownPremisesCount";
            this.numericUpDownPremisesCount.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownPremisesCount.TabIndex = 0;
            this.numericUpDownPremisesCount.ValueChanged += new System.EventHandler(this.numericUpDownPremisesCount_ValueChanged);
            // 
            // numericUpDownRoomsCount
            // 
            this.numericUpDownRoomsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownRoomsCount.Location = new System.Drawing.Point(163, 47);
            this.numericUpDownRoomsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownRoomsCount.Name = "numericUpDownRoomsCount";
            this.numericUpDownRoomsCount.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownRoomsCount.TabIndex = 1;
            this.numericUpDownRoomsCount.ValueChanged += new System.EventHandler(this.numericUpDownRoomsCount_ValueChanged);
            // 
            // numericUpDownApartmentsCount
            // 
            this.numericUpDownApartmentsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownApartmentsCount.Location = new System.Drawing.Point(163, 76);
            this.numericUpDownApartmentsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownApartmentsCount.Name = "numericUpDownApartmentsCount";
            this.numericUpDownApartmentsCount.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownApartmentsCount.TabIndex = 2;
            this.numericUpDownApartmentsCount.ValueChanged += new System.EventHandler(this.numericUpDownApartmentsCount_ValueChanged);
            // 
            // numericUpDownSharedApartmentsCount
            // 
            this.numericUpDownSharedApartmentsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSharedApartmentsCount.Location = new System.Drawing.Point(163, 105);
            this.numericUpDownSharedApartmentsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSharedApartmentsCount.Name = "numericUpDownSharedApartmentsCount";
            this.numericUpDownSharedApartmentsCount.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownSharedApartmentsCount.TabIndex = 3;
            this.numericUpDownSharedApartmentsCount.ValueChanged += new System.EventHandler(this.numericUpDownSharedApartmentsCount_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Всего";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Квартир";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Комнат";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(129, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Квартир с подселением";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numericUpDownOtherPremisesCount);
            this.groupBox3.Controls.Add(this.numericUpDownSpecialPremisesCount);
            this.groupBox3.Controls.Add(this.numericUpDownCommercialPremisesCount);
            this.groupBox3.Controls.Add(this.numericUpDownSocialPremisesCount);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(504, 213);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(495, 134);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Количество помещений по типу найма";
            // 
            // numericUpDownOtherPremisesCount
            // 
            this.numericUpDownOtherPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownOtherPremisesCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericUpDownOtherPremisesCount.Location = new System.Drawing.Point(159, 105);
            this.numericUpDownOtherPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownOtherPremisesCount.Name = "numericUpDownOtherPremisesCount";
            this.numericUpDownOtherPremisesCount.ReadOnly = true;
            this.numericUpDownOtherPremisesCount.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownOtherPremisesCount.TabIndex = 3;
            // 
            // numericUpDownSpecialPremisesCount
            // 
            this.numericUpDownSpecialPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSpecialPremisesCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericUpDownSpecialPremisesCount.Location = new System.Drawing.Point(159, 76);
            this.numericUpDownSpecialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSpecialPremisesCount.Name = "numericUpDownSpecialPremisesCount";
            this.numericUpDownSpecialPremisesCount.ReadOnly = true;
            this.numericUpDownSpecialPremisesCount.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownSpecialPremisesCount.TabIndex = 2;
            // 
            // numericUpDownCommercialPremisesCount
            // 
            this.numericUpDownCommercialPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownCommercialPremisesCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericUpDownCommercialPremisesCount.Location = new System.Drawing.Point(158, 47);
            this.numericUpDownCommercialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownCommercialPremisesCount.Name = "numericUpDownCommercialPremisesCount";
            this.numericUpDownCommercialPremisesCount.ReadOnly = true;
            this.numericUpDownCommercialPremisesCount.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownCommercialPremisesCount.TabIndex = 1;
            // 
            // numericUpDownSocialPremisesCount
            // 
            this.numericUpDownSocialPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSocialPremisesCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericUpDownSocialPremisesCount.Location = new System.Drawing.Point(158, 18);
            this.numericUpDownSocialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSocialPremisesCount.Name = "numericUpDownSocialPremisesCount";
            this.numericUpDownSocialPremisesCount.ReadOnly = true;
            this.numericUpDownSocialPremisesCount.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownSocialPremisesCount.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 107);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Прочие";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 78);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(118, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Специализированный";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 49);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(83, 13);
            this.label12.TabIndex = 6;
            this.label12.Text = "Коммерческий";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 7;
            this.label13.Text = "Социальный";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericUpDownTotalArea);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.numericUpDownLivingArea);
            this.groupBox2.Controls.Add(this.numericUpDownMunicipalArea);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 353);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(495, 104);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Площадь";
            // 
            // numericUpDownTotalArea
            // 
            this.numericUpDownTotalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownTotalArea.DecimalPlaces = 3;
            this.numericUpDownTotalArea.Location = new System.Drawing.Point(163, 18);
            this.numericUpDownTotalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            this.numericUpDownTotalArea.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownTotalArea.TabIndex = 0;
            this.numericUpDownTotalArea.ThousandsSeparator = true;
            this.numericUpDownTotalArea.ValueChanged += new System.EventHandler(this.numericUpDownTotalArea_ValueChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(16, 20);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(42, 13);
            this.label20.TabIndex = 5;
            this.label20.Text = "Общая";
            // 
            // numericUpDownLivingArea
            // 
            this.numericUpDownLivingArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownLivingArea.DecimalPlaces = 3;
            this.numericUpDownLivingArea.Location = new System.Drawing.Point(163, 47);
            this.numericUpDownLivingArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            this.numericUpDownLivingArea.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownLivingArea.TabIndex = 1;
            this.numericUpDownLivingArea.ThousandsSeparator = true;
            this.numericUpDownLivingArea.ValueChanged += new System.EventHandler(this.numericUpDownLivingArea_ValueChanged);
            // 
            // numericUpDownMunicipalArea
            // 
            this.numericUpDownMunicipalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownMunicipalArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericUpDownMunicipalArea.DecimalPlaces = 3;
            this.numericUpDownMunicipalArea.Location = new System.Drawing.Point(163, 76);
            this.numericUpDownMunicipalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownMunicipalArea.Name = "numericUpDownMunicipalArea";
            this.numericUpDownMunicipalArea.ReadOnly = true;
            this.numericUpDownMunicipalArea.Size = new System.Drawing.Size(328, 20);
            this.numericUpDownMunicipalArea.TabIndex = 2;
            this.numericUpDownMunicipalArea.ThousandsSeparator = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Муниципальных ЖП";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Жилая";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxDescription);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(504, 353);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(495, 104);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 16);
            this.textBoxDescription.MaxLength = 255;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(489, 85);
            this.textBoxDescription.TabIndex = 0;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.dataGridViewRestrictions);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(3, 463);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(495, 257);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Реквизиты";
            // 
            // dataGridViewRestrictions
            // 
            this.dataGridViewRestrictions.AllowUserToAddRows = false;
            this.dataGridViewRestrictions.AllowUserToDeleteRows = false;
            this.dataGridViewRestrictions.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewRestrictions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRestrictions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.restriction_number,
            this.restriction_date,
            this.restriction_description,
            this.id_restriction_type});
            this.dataGridViewRestrictions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRestrictions.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewRestrictions.Name = "dataGridViewRestrictions";
            this.dataGridViewRestrictions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRestrictions.Size = new System.Drawing.Size(489, 238);
            this.dataGridViewRestrictions.TabIndex = 0;
            this.dataGridViewRestrictions.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRestrictions_CellDoubleClick);
            this.dataGridViewRestrictions.Resize += new System.EventHandler(this.dataGridViewRestrictions_Resize);
            // 
            // restriction_number
            // 
            this.restriction_number.HeaderText = "Номер";
            this.restriction_number.MinimumWidth = 100;
            this.restriction_number.Name = "restriction_number";
            this.restriction_number.ReadOnly = true;
            // 
            // restriction_date
            // 
            this.restriction_date.HeaderText = "Дата";
            this.restriction_date.MinimumWidth = 100;
            this.restriction_date.Name = "restriction_date";
            this.restriction_date.ReadOnly = true;
            // 
            // restriction_description
            // 
            this.restriction_description.HeaderText = "Наименование";
            this.restriction_description.MinimumWidth = 200;
            this.restriction_description.Name = "restriction_description";
            this.restriction_description.ReadOnly = true;
            this.restriction_description.Width = 200;
            // 
            // id_restriction_type
            // 
            this.id_restriction_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_restriction_type.HeaderText = "Тип права собственности";
            this.id_restriction_type.MinimumWidth = 200;
            this.id_restriction_type.Name = "id_restriction_type";
            this.id_restriction_type.ReadOnly = true;
            this.id_restriction_type.Width = 200;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dataGridViewOwnerships);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(504, 463);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(495, 257);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Ограничения";
            // 
            // dataGridViewOwnerships
            // 
            this.dataGridViewOwnerships.AllowUserToAddRows = false;
            this.dataGridViewOwnerships.AllowUserToDeleteRows = false;
            this.dataGridViewOwnerships.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewOwnerships.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOwnerships.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ownership_number,
            this.ownership_date,
            this.ownership_description,
            this.id_ownership_type});
            this.dataGridViewOwnerships.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewOwnerships.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewOwnerships.Name = "dataGridViewOwnerships";
            this.dataGridViewOwnerships.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewOwnerships.Size = new System.Drawing.Size(489, 238);
            this.dataGridViewOwnerships.TabIndex = 0;
            this.dataGridViewOwnerships.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewOwnerships_CellDoubleClick);
            this.dataGridViewOwnerships.Resize += new System.EventHandler(this.dataGridViewOwnerships_Resize);
            // 
            // ownership_number
            // 
            this.ownership_number.HeaderText = "Номер";
            this.ownership_number.MinimumWidth = 100;
            this.ownership_number.Name = "ownership_number";
            this.ownership_number.ReadOnly = true;
            // 
            // ownership_date
            // 
            this.ownership_date.HeaderText = "Дата";
            this.ownership_date.MinimumWidth = 100;
            this.ownership_date.Name = "ownership_date";
            this.ownership_date.ReadOnly = true;
            // 
            // ownership_description
            // 
            this.ownership_description.HeaderText = "Наименование";
            this.ownership_description.MinimumWidth = 200;
            this.ownership_description.Name = "ownership_description";
            this.ownership_description.ReadOnly = true;
            this.ownership_description.Width = 200;
            // 
            // id_ownership_type
            // 
            this.id_ownership_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_ownership_type.HeaderText = "Тип ограничения";
            this.id_ownership_type.MinimumWidth = 200;
            this.id_ownership_type.Name = "id_ownership_type";
            this.id_ownership_type.ReadOnly = true;
            this.id_ownership_type.Width = 200;
            // 
            // BuildingViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(600, 540);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BuildingViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Здание";
            this.tableLayoutPanel.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartupYear)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoomsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownApartmentsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSharedApartmentsCount)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOtherPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpecialPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCommercialPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSocialPremisesCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunicipalArea)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).EndInit();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
