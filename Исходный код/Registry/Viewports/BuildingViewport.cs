using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport
{
    internal sealed class BuildingViewport : Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
        private TableLayoutPanel tableLayoutPanel2 = new TableLayoutPanel();
        private GroupBox groupBox1 = new GroupBox();
        private GroupBox groupBox2 = new GroupBox();
        private GroupBox groupBox3 = new GroupBox();
        private GroupBox groupBox4 = new GroupBox();
        private GroupBox groupBox5 = new GroupBox();
        private GroupBox groupBox6 = new GroupBox();
        private GroupBox groupBox7 = new GroupBox();
        private NumericUpDown numericUpDownFloors = new NumericUpDown();
        private NumericUpDown numericUpDownBalanceCost = new NumericUpDown();
        private NumericUpDown numericUpDownCadastralCost = new NumericUpDown();
        private NumericUpDown numericUpDownStartupYear = new NumericUpDown();
        private NumericUpDown numericUpDownLivingArea = new NumericUpDown();
        private NumericUpDown numericUpDownMunicipalArea = new NumericUpDown();
        private NumericUpDown numericUpDownPremisesCount = new NumericUpDown();
        private NumericUpDown numericUpDownRoomsCount = new NumericUpDown();
        private NumericUpDown numericUpDownApartmentsCount = new NumericUpDown();
        private NumericUpDown numericUpDownSharedApartmentsCount = new NumericUpDown();
        private NumericUpDown numericUpDownCommercialPremisesCount = new NumericUpDown();
        private NumericUpDown numericUpDownSpecialPremisesCount = new NumericUpDown();
        private NumericUpDown numericUpDownSocialPremisesCount = new NumericUpDown();
        private NumericUpDown numericUpDownOtherPremisesCount = new NumericUpDown();
        private Label label1 = new Label();
        private Label label2 = new Label();
        private Label label3 = new Label();
        private Label label4 = new Label();
        private Label label5 = new Label();
        private Label label6 = new Label();
        private Label label7 = new Label();
        private Label label8 = new Label();
        private Label label9 = new Label();
        private Label label10 = new Label();
        private Label label11 = new Label();
        private Label label12 = new Label();
        private Label label13 = new Label();
        private Label label14 = new Label();
        private Label label15 = new Label();
        private Label label16 = new Label();
        private Label label17 = new Label();
        private Label label18 = new Label();
        private Label label19 = new Label();
        private Label label40 = new Label();
        private Panel panel1 = new Panel();
        private Panel panel2 = new Panel();
        private TextBox textBoxHouse = new TextBox();
        private TextBox textBoxDescription = new TextBox();
        private ComboBox comboBoxCurrentFundType = new ComboBox();
        private ComboBox comboBoxState = new ComboBox();
        private MaskedTextBox maskedTextBoxCadastralNum = new MaskedTextBox();
        private DataGridView dataGridViewRestrictions = new DataGridView();
        private DataGridView dataGridViewOwnerships = new DataGridView();
        private DataGridViewTextBoxColumn field_restriction_number = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_restriction_date = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_restriction_type = new DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_restriction_description = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_ownership_number = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_ownership_date = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_ownership_type = new DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_ownership_description = new DataGridViewTextBoxColumn();
        private ComboBox comboBoxStreet = new ComboBox();
        private ComboBox comboBoxStructureType = new ComboBox();
        private CheckBox checkBoxImprovement = new CheckBox();
        private CheckBox checkBoxElevator = new CheckBox();
        #endregion Components
        
        //Models
        private BuildingsDataModel buildings = null;
        private BuildingsAggregatedDataModel buildingsAggreagate = null;
        private BuildingsCurrentFundsDataModel buildingsCurrentFund = null;
        private KladrStreetsDataModel kladr = null;
        private StructureTypesDataModel structureTypes = null;
        private RestrictionsDataModel restrictions = null;
        private RestrictionTypesDataModel restrictionTypes = null;
        private RestrictionsBuildingsAssocDataModel restrictionBuildingsAssoc = null;
        private OwnershipsRightsDataModel ownershipRights = null;
        private OwnershipRightTypesDataModel ownershipRightTypes = null;
        private OwnershipBuildingsAssocDataModel ownershipBuildingsAssoc = null;
        private FundTypesDataModel fundTypes = null;
        private StatesDataModel states = null;

        //Views
        private BindingSource v_buildings = null;
        private BindingSource v_buildingsAggreagate = null;
        private BindingSource v_buildingsCurrentFund = null;
        private BindingSource v_kladr = null;
        private BindingSource v_structureTypes = null;
        private BindingSource v_restrictions = null;
        private BindingSource v_restrictonTypes = null;
        private BindingSource v_restrictionBuildingsAssoc = null;
        private BindingSource v_ownershipRights = null;
        private BindingSource v_ownershipRightTypes = null;
        private BindingSource v_ownershipBuildingsAssoc = null;
        private BindingSource v_fundType = null;
        private BindingSource v_states = null;

        //Текущее состояние viewport'а
        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;

        public BuildingViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageBuilding";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Здание";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public BuildingViewport(BuildingViewport buildingListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = buildingListViewport.DynamicFilter;
            this.StaticFilter = buildingListViewport.StaticFilter;
            this.ParentRow = buildingListViewport.ParentRow;
            this.ParentType = buildingListViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            //Асинхронные модели
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
            states = StatesDataModel.GetInstance();

            //Синхронные модели
            buildingsAggreagate = BuildingsAggregatedDataModel.GetInstance();
            buildingsCurrentFund = BuildingsCurrentFundsDataModel.GetInstance();

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
            states.Select();

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

            v_buildingsAggreagate = new BindingSource();
            v_buildingsAggreagate.DataMember = "buildings_aggregated";
            v_buildingsAggreagate.DataSource = buildingsAggreagate.Select();

            v_buildingsCurrentFund = new BindingSource();
            v_buildingsCurrentFund.DataMember = "buildings_current_funds";
            v_buildingsCurrentFund.DataSource = buildingsCurrentFund.Select();

            v_fundType = new BindingSource();
            v_fundType.DataMember = "fund_types";
            v_fundType.DataSource = ds;

            v_states = new BindingSource();
            v_states.DataMember = "states";
            v_states.DataSource = ds;

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
            restrictionBuildingsAssoc.Select().RowDeleting += new DataRowChangeEventHandler(RestrictionsAssoc_RowDeleting);

            v_ownershipBuildingsAssoc = new BindingSource();
            v_ownershipBuildingsAssoc.CurrentItemChanged += new EventHandler(v_ownershipBuildingsAssoc_CurrentItemChanged);
            v_ownershipBuildingsAssoc.DataMember = "buildings_ownership_buildings_assoc";
            v_ownershipBuildingsAssoc.DataSource = v_buildings;
            v_ownershipBuildingsAssoc_CurrentItemChanged(null, new EventArgs());
            OwnershipsFilterRebuild();
            ownershipBuildingsAssoc.Select().RowChanged += new DataRowChangeEventHandler(OwnershipsAssoc_RowChanged);
            ownershipBuildingsAssoc.Select().RowDeleting += new DataRowChangeEventHandler(OwnershipsAssoc_RowDeleting);

            DataBind();

            comboBoxStreet.Leave += new EventHandler(comboBoxStreet_Leave);
            comboBoxStreet.KeyUp += new KeyEventHandler(comboBoxStreet_KeyUp);
            comboBoxStreet.DropDownClosed += new EventHandler(comboBoxStreet_DropDownClosed);
            comboBoxStreet.SelectedIndexChanged += new EventHandler(comboBoxStreet_SelectedIndexChanged);
            comboBoxStructureType.SelectedIndexChanged += new EventHandler(comboBoxStructureType_SelectedIndexChanged);
            comboBoxState.SelectedIndexChanged += new EventHandler(comboBoxState_SelectedIndexChanged);
            textBoxHouse.TextChanged += new EventHandler(textBoxHouse_TextChanged);
            textBoxDescription.TextChanged += new EventHandler(textBoxDescription_TextChanged);
            maskedTextBoxCadastralNum.TextChanged += new EventHandler(maskedTextBoxCadastralNum_TextChanged);
            checkBoxElevator.CheckedChanged += new EventHandler(checkBoxElevator_CheckedChanged);
            checkBoxImprovement.CheckedChanged += new EventHandler(checkBoxImprovement_CheckedChanged);

            numericUpDownFloors.ValueChanged += new EventHandler(numericUpDownFloors_ValueChanged);
            numericUpDownStartupYear.ValueChanged += new EventHandler(numericUpDownStartupYear_ValueChanged);
            numericUpDownCadastralCost.ValueChanged += new EventHandler(numericUpDownCadastralCost_ValueChanged);
            numericUpDownBalanceCost.ValueChanged += new EventHandler(numericUpDownBalanceCost_ValueChanged);
            numericUpDownPremisesCount.ValueChanged += new EventHandler(numericUpDownPremisesCount_ValueChanged);
            numericUpDownRoomsCount.ValueChanged += new EventHandler(numericUpDownRoomsCount_ValueChanged);
            numericUpDownApartmentsCount.ValueChanged += new EventHandler(numericUpDownApartmentsCount_ValueChanged);
            numericUpDownSharedApartmentsCount.ValueChanged += new EventHandler(numericUpDownSharedApartmentsCount_ValueChanged);
            numericUpDownLivingArea.ValueChanged += new EventHandler(numericUpDownLivingArea_ValueChanged);

            buildingsCurrentFund.RefreshEvent += new EventHandler<EventArgs>(buildingsCurrentFund_RefreshEvent);
        }

        void buildingsCurrentFund_RefreshEvent(object sender, EventArgs e)
        {
            ShowOrHideCurrentFund();
        }

        void RestrictionsAssoc_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                RestrictionsFilterRebuild();
        }

        void RestrictionsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                RestrictionsFilterRebuild();
        }

        void OwnershipsAssoc_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                OwnershipsFilterRebuild();
        }

        void OwnershipsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                OwnershipsFilterRebuild();
        }

        void RestrictionsFilterRebuild()
        {
            string restrictionsFilter = "id_restriction IN (0";
            for (int i = 0; i < v_restrictionBuildingsAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)v_restrictionBuildingsAssoc[i])["id_restriction"].ToString() + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(new char[] { ',' });
            restrictionsFilter += ")";
            v_restrictions.Filter = restrictionsFilter;
        }

        void OwnershipsFilterRebuild()
        {
            string ownershipFilter = "id_ownership_right IN (0";
            for (int i = 0; i < v_ownershipBuildingsAssoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownershipBuildingsAssoc[i])["id_ownership_right"].ToString() + ",";
            ownershipFilter = ownershipFilter.TrimEnd(new char[] { ',' });
            ownershipFilter += ")";
            v_ownershipRights.Filter = ownershipFilter;
        }

        void v_buildings_CurrentItemChanged(object sender, EventArgs e)
        {
            if (viewportState == ViewportState.NewRowState)
                this.Text = "Новое здание";
            else
                if (v_buildings.Position != -1)
                    this.Text = String.Format("Здание №{0}", ((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
                else
                    this.Text = "Здания отсутствуют";
            if (v_buildingsAggreagate != null)
            {
                if ((v_buildings.Position != -1) && !(((DataRowView)v_buildings[v_buildings.Position])["id_building"] is DBNull))
                    v_buildingsAggreagate.Filter = "id_building = " + ((DataRowView)v_buildings[v_buildings.Position])["id_building"].ToString();
                else
                    v_buildingsAggreagate.Filter = "id_building = 0";  
            }
            if (v_buildingsCurrentFund != null)
            {
                if ((v_buildings.Position != -1) && !(((DataRowView)v_buildings[v_buildings.Position])["id_building"] is DBNull))
                    v_buildingsCurrentFund.Filter = "id_building = " + ((DataRowView)v_buildings[v_buildings.Position])["id_building"].ToString();
                else
                    v_buildingsCurrentFund.Filter = "id_building = 0";
                ShowOrHideCurrentFund();
            }
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

        void v_ownershipBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            OwnershipsFilterRebuild();
        }

        void v_restrictionBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            RestrictionsFilterRebuild();
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
            maskedTextBoxCadastralNum.DataBindings.Clear();
            maskedTextBoxCadastralNum.DataBindings.Add("Text", v_buildings, "cadastral_num");
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

            comboBoxState.DataSource = v_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";
            comboBoxState.DataBindings.Clear();
            comboBoxState.DataBindings.Add("SelectedValue", v_buildings, "id_state", true, DataSourceUpdateMode.Never, DBNull.Value);

            numericUpDownSocialPremisesCount.DataBindings.Clear();
            numericUpDownSocialPremisesCount.DataBindings.Add("Minimum", v_buildingsAggreagate, "social_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSocialPremisesCount.DataBindings.Add("Maximum", v_buildingsAggreagate, "social_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSocialPremisesCount.DataBindings.Add("Value", v_buildingsAggreagate, "social_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownCommercialPremisesCount.DataBindings.Clear();
            numericUpDownCommercialPremisesCount.DataBindings.Add("Minimum", v_buildingsAggreagate, "commercial_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownCommercialPremisesCount.DataBindings.Add("Maximum", v_buildingsAggreagate, "commercial_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownCommercialPremisesCount.DataBindings.Add("Value", v_buildingsAggreagate, "commercial_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSpecialPremisesCount.DataBindings.Clear();
            numericUpDownSpecialPremisesCount.DataBindings.Add("Minimum", v_buildingsAggreagate, "special_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSpecialPremisesCount.DataBindings.Add("Maximum", v_buildingsAggreagate, "special_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownSpecialPremisesCount.DataBindings.Add("Value", v_buildingsAggreagate, "special_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownOtherPremisesCount.DataBindings.Clear();
            numericUpDownOtherPremisesCount.DataBindings.Add("Minimum", v_buildingsAggreagate, "other_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownOtherPremisesCount.DataBindings.Add("Maximum", v_buildingsAggreagate, "other_premises_count", true, DataSourceUpdateMode.Never, 0);
            numericUpDownOtherPremisesCount.DataBindings.Add("Value", v_buildingsAggreagate, "other_premises_count", true, DataSourceUpdateMode.Never, 0);
            
            dataGridViewRestrictions.DataSource = v_restrictions;
            field_id_restriction_type.DataSource = v_restrictonTypes;
            field_id_restriction_type.DataPropertyName = "id_restriction_type";
            field_id_restriction_type.ValueMember = "id_restriction_type";
            field_id_restriction_type.DisplayMember = "restriction_type";
            field_restriction_number.DataPropertyName = "number";
            field_restriction_date.DataPropertyName = "date";
            field_restriction_description.DataPropertyName = "description";

            dataGridViewOwnerships.DataSource = v_ownershipRights;
            field_id_ownership_type.DataSource = v_ownershipRightTypes;
            field_id_ownership_type.DataPropertyName = "id_ownership_right_type";
            field_id_ownership_type.ValueMember = "id_ownership_right_type";
            field_id_ownership_type.DisplayMember = "ownership_right_type";
            field_ownership_number.DataPropertyName = "number";
            field_ownership_date.DataPropertyName = "date";
            field_ownership_description.DataPropertyName = "description";
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

        void maskedTextBoxCadastralNum_TextChanged(object sender, EventArgs e)
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

        public override void Close()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            restrictionBuildingsAssoc.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionsAssoc_RowChanged);
            restrictionBuildingsAssoc.Select().RowDeleting -= new DataRowChangeEventHandler(RestrictionsAssoc_RowDeleting);
            ownershipBuildingsAssoc.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipsAssoc_RowChanged);
            ownershipBuildingsAssoc.Select().RowDeleting -= new DataRowChangeEventHandler(OwnershipsAssoc_RowDeleting);
            buildingsCurrentFund.RefreshEvent -= new EventHandler<EventArgs>(buildingsCurrentFund_RefreshEvent);
            base.Close();
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanCopyRecord()
        {
            return ((v_buildings.Position != -1) && (!buildings.EditingNewRecord));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            Building building = BuildingFromView();
            DataRowView row = (DataRowView)v_buildings.AddNew();
            buildings.EditingNewRecord = true;
            ViewportFromBuilding(building);
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
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
            if ((building.cadastral_num != null) && building.cadastral_num.Length > 15)
            {
                MessageBox.Show("Длина кадастрового номера не может превышать 15 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                maskedTextBoxCadastralNum.Focus();
                return false;
            }
            return true;
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
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору","Ошибка",      
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ViewportState.NewRowState:
                    int id_building = buildings.Insert(building);
                    if (id_building == -1)
                        return;
                    DataRowView newRow;
                    if (v_buildings.Position == -1)
                        newRow = (DataRowView)v_buildings.AddNew();
                    else
                        newRow = ((DataRowView)v_buildings[v_buildings.Position]);
                    building.id_building = id_building;
                    FillRowFromBuilding(building, newRow);
                    buildings.EditingNewRecord = false;
                    this.Text = "Здание №" + id_building.ToString();
                    viewportState = ViewportState.ReadState;
                    is_editable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (building.id_building == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить здание без внутренного номера. "+
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (buildings.Update(building) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_buildings[v_buildings.Position]);
                    FillRowFromBuilding(building, row);
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
                    break;
            }
        }

        public override int GetRecordCount()
        {
            return v_buildings.Count;
        }

        private static void FillRowFromBuilding(Building building, DataRowView row)
        {
            row.BeginEdit();
            row["id_building"] = building.id_building;
            row["id_state"] = building.id_state;
            row["id_structure_type"] = building.id_structure_type;
            row["id_street"] = building.id_street;
            row["house"] = building.house;
            row["floors"] = building.floors;
            row["num_premises"] = building.num_premises;
            row["num_rooms"] = building.num_rooms;
            row["num_apartments"] = building.num_apartments;
            row["num_shared_apartments"] = building.num_shared_apartments;
            row["cadastral_num"] = building.cadastral_num;
            row["cadastral_cost"] = building.cadastral_cost;
            row["balance_cost"] = building.balance_cost;
            row["description"] = building.description;
            row["startup_year"] = building.startup_year;
            row["improvement"] = building.improvement;
            row["elevator"] = building.elevator;
            row.EndEdit();
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    viewportState = ViewportState.ReadState;
                    buildings.EditingNewRecord = false;
                    if (v_buildings.Position != -1)
                    {
                        ((DataRowView)v_buildings[v_buildings.Position]).Delete();
                        buildings.Select().AcceptChanges();
                    }
                    else
                        this.Text = "Здания отсутствуют";
                    break;
                case ViewportState.ModifyRowState:
                    v_kladr.Filter = "";
                    is_editable = false;
                    DataBind();
                    is_editable = true;
                    viewportState = ViewportState.ReadState;
                    break;
            }
        }

        bool ChangeViewportStateTo(ViewportState state)
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

        public void LocateBuildingBy(int id)
        {
            int Position = v_buildings.Find("id_building", id);
            if (Position > 0)
                v_buildings.Position = Position;
        }

        //Загружает текущие данные из формы
        private Building BuildingFromViewport()
        {
            Building building = new Building();
            if ((v_buildings.Position == -1) || ((DataRowView)v_buildings[v_buildings.Position])["id_building"] is DBNull)
                building.id_building = null;
            else
                building.id_building = Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            if (comboBoxStreet.SelectedValue == null)
                building.id_street = null;
            else
                building.id_street = comboBoxStreet.SelectedValue.ToString();
            if (comboBoxState.SelectedValue == null)
                building.id_state = null;
            else
                building.id_state = Convert.ToInt32(comboBoxState.SelectedValue);
            if (textBoxHouse.Text.Trim() == "")
                building.house = null;
            else
                building.house = textBoxHouse.Text.Trim();
            building.floors = Convert.ToInt16(numericUpDownFloors.Value);
            building.startup_year = Convert.ToInt32(numericUpDownStartupYear.Value);
            if (comboBoxStructureType.SelectedValue == null)
                building.id_structure_type = null;
            else
                building.id_structure_type = Convert.ToInt32(comboBoxStructureType.SelectedValue);
            if (maskedTextBoxCadastralNum.Text.Trim() == "")
                building.cadastral_num = null;
            else
                building.cadastral_num = maskedTextBoxCadastralNum.Text.Trim();
            building.cadastral_cost = numericUpDownCadastralCost.Value;
            building.balance_cost = numericUpDownBalanceCost.Value;
            building.improvement = checkBoxImprovement.Checked;
            building.elevator = checkBoxElevator.Checked;
            building.num_premises = Convert.ToInt32(numericUpDownPremisesCount.Value);
            building.num_rooms = Convert.ToInt32(numericUpDownRoomsCount.Value);
            building.num_apartments = Convert.ToInt32(numericUpDownApartmentsCount.Value);
            building.num_shared_apartments = Convert.ToInt32(numericUpDownSharedApartmentsCount.Value);
            building.living_area = Convert.ToDouble(numericUpDownLivingArea.Value);
            if (textBoxDescription.Text.Trim() == "")
                building.description = null;
            else
                building.description = textBoxDescription.Text.Trim().ToString();
            return building;
        }

        //Загружает оригинальные данные из view
        private Building BuildingFromView()
        {
            Building building = new Building();
            DataRowView row = (DataRowView)v_buildings[v_buildings.Position];
            if (row["id_building"] is DBNull)
                building.id_building = null;
            else
                building.id_building = Convert.ToInt32(row["id_building"]);
            if (row["id_street"] is DBNull)
                building.id_street = null;
            else
                building.id_street = row["id_street"].ToString();
            if (row["id_state"] is DBNull)
                building.id_state = null;
            else
                building.id_state = Convert.ToInt32(row["id_state"]);
            if (row["id_structure_type"] is DBNull)
                building.id_structure_type = null;
            else
                building.id_structure_type = Convert.ToInt32(row["id_structure_type"]);
            building.house = row["house"].ToString();
            building.floors = Convert.ToInt16(row["floors"]);
            building.num_premises = Convert.ToInt32(row["num_premises"]);
            building.num_rooms = Convert.ToInt32(row["num_rooms"]);
            building.num_apartments = Convert.ToInt32(row["num_apartments"]);
            building.num_shared_apartments = Convert.ToInt32(row["num_shared_apartments"]);
            building.living_area = Convert.ToDouble(row["living_area"]);
            if (row["cadastral_num"] is DBNull)
                building.cadastral_num = null;
            else
                building.cadastral_num = row["cadastral_num"].ToString();
            building.cadastral_cost = Convert.ToDecimal(row["cadastral_cost"]);
            building.balance_cost = Convert.ToDecimal(row["balance_cost"]);
            if (row["description"] is DBNull)
                building.description = null;
            else
                building.description = row["description"].ToString();
            building.startup_year = Convert.ToInt32(row["startup_year"]);
            building.improvement = Convert.ToBoolean(row["improvement"]);
            building.elevator = Convert.ToBoolean(row["elevator"]);
            return building;
        }

        //Загружаем данные в форму из объекта building
        private void ViewportFromBuilding(Building building)
        {
            if (building.id_street != null)
                comboBoxStreet.SelectedValue = building.id_street;
            textBoxHouse.Text = building.house;
            numericUpDownFloors.Value = building.floors.Value;
            numericUpDownStartupYear.Value = building.startup_year.Value;
            if (building.id_state != null)
                comboBoxState.SelectedValue = building.id_state;
            if (building.id_structure_type != null)
                comboBoxStructureType.SelectedValue = building.id_structure_type;
            maskedTextBoxCadastralNum.Text = building.cadastral_num;
            numericUpDownCadastralCost.Value = building.cadastral_cost.Value;
            numericUpDownBalanceCost.Value = building.balance_cost.Value;
            checkBoxImprovement.Checked = building.improvement.Value;
            checkBoxElevator.Checked = building.elevator.Value;
            numericUpDownPremisesCount.Value = building.num_premises.Value;
            numericUpDownRoomsCount.Value = building.num_rooms.Value;
            numericUpDownApartmentsCount.Value = building.num_apartments.Value;
            numericUpDownSharedApartmentsCount.Value = building.num_shared_apartments.Value;
            numericUpDownLivingArea.Value = (decimal)building.living_area.Value;
            textBoxDescription.Text = building.description;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_buildings.MoveFirst();
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_buildings.MoveLast();
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_buildings.MoveNext();
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_buildings.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_buildings.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_buildings.Position > 0;
        }

        public override void ClearSearch()
        {
            v_buildings.Filter = StaticFilter;
            DynamicFilter = "";
        }

        public override bool CanMoveNext()
        {
            return (v_buildings.Position > -1) && (v_buildings.Position < (v_buildings.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_buildings.Position > -1) && (v_buildings.Position < (v_buildings.Count - 1));
        }

        public override bool CanSearchRecord()
        {
            return (viewportState == ViewportState.ReadState);
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

        public override bool HasFundHistory()
        {
            return (v_buildings.Position > -1);
        }

        public override void ShowPremises()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения перечня квартир", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PremisesListViewport viewport = new PremisesListViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowOwnerships()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения ограничений", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            OwnershipListViewport viewport = new OwnershipListViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowRestrictions()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения реквизитов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RestrictionListViewport viewport = new RestrictionListViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowFundHistory()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения истории найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FundsHistoryViewport viewport = new FundsHistoryViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
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
            DataRowView row = (DataRowView)v_buildings.AddNew();
            buildings.EditingNewRecord = true;
        }

        public override bool SearchedRecords()
        {
            if (DynamicFilter != "")
                return true;
            else
                return false;
        }

        public override void SearchRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            SearchBuildingForm sbForm = new SearchBuildingForm();
            if (sbForm.ShowDialog() == DialogResult.OK)
            {
                DynamicFilter = sbForm.GetFilter();
                v_buildings.Filter = StaticFilter;
                if (StaticFilter != "" && DynamicFilter != "")
                    v_buildings.Filter += " AND ";
                v_buildings.Filter += DynamicFilter;
            }
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это здание?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (buildings.Delete((int)((DataRowView)v_buildings.Current)["id_building"]) == -1)
                    return;
                ((DataRowView)v_buildings[v_buildings.Position]).Delete();
                menuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanDeleteRecord()
        {
            if ((v_buildings.Position == -1) || (viewportState == ViewportState.NewRowState))
                return false;
            else
                return true;
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

        private void ConstructViewport()
        {
            this.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownFloors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownBalanceCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownCadastralCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownStartupYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownLivingArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownMunicipalArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownOtherPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownSpecialPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownRoomsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownApartmentsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownSharedApartmentsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownCommercialPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownSocialPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(dataGridViewRestrictions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(dataGridViewOwnerships)).BeginInit();
            tableLayoutPanel2.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();

            // 
            // tableLayoutPanel
            // 
            this.Controls.Add(tableLayoutPanel);
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(groupBox4, 0, 0);
            tableLayoutPanel.Controls.Add(groupBox1, 0, 1);
            tableLayoutPanel.Controls.Add(groupBox3, 1, 1);
            tableLayoutPanel.Controls.Add(groupBox2, 0, 2);
            tableLayoutPanel.Controls.Add(groupBox5, 1, 2);
            tableLayoutPanel.Controls.Add(groupBox6, 0, 3);
            tableLayoutPanel.Controls.Add(groupBox7, 1, 3);
            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel.Name = "tableLayoutPanel1";
            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.Size = new System.Drawing.Size(984, 531);
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(panel1, 0, 0);
            tableLayoutPanel2.Controls.Add(panel2, 1, 0);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            tableLayoutPanel2.Size = new System.Drawing.Size(972, 160);
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
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(3, 188);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(486, 134);
            groupBox1.TabStop = false;
            groupBox1.Text = "Количество жилых помещений";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(numericUpDownLivingArea);
            groupBox2.Controls.Add(numericUpDownMunicipalArea);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label9);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(3, 328);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(486, 74);
            groupBox2.TabStop = false;
            groupBox2.Text = "Площадь помещений";
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
            groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox3.Location = new System.Drawing.Point(495, 188);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(486, 134);
            groupBox3.TabStop = false;
            groupBox3.Text = "Количество помещений по типу найма";
            // 
            // groupBox4
            // 
            tableLayoutPanel.SetColumnSpan(groupBox4, 2);
            groupBox4.Controls.Add(tableLayoutPanel2);
            groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox4.Location = new System.Drawing.Point(3, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(978, 179);
            groupBox4.TabStop = false;
            groupBox4.Text = "Общие сведения";       
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(textBoxDescription);
            groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox5.Location = new System.Drawing.Point(495, 328);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new System.Drawing.Size(486, 74);
            groupBox5.TabStop = false;
            groupBox5.Text = "Дополнительные сведения";
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(dataGridViewRestrictions);
            groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox6.Location = new System.Drawing.Point(3, 408);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new System.Drawing.Size(486, 120);
            groupBox6.TabStop = false;
            groupBox6.Text = "Реквизиты НПА";
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(dataGridViewOwnerships);
            groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox7.Location = new System.Drawing.Point(495, 408);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new System.Drawing.Size(486, 120);
            groupBox7.TabStop = false;
            groupBox7.Text = "Ограничения";
            // 
            // numericUpDownFloors
            // 
            numericUpDownFloors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownFloors.Location = new System.Drawing.Point(157, 66);
            numericUpDownFloors.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            numericUpDownFloors.Name = "numericUpDown1";
            numericUpDownFloors.Size = new System.Drawing.Size(319, 20);
            numericUpDownFloors.TabStop = true;
            numericUpDownFloors.TabIndex = 2;
            // 
            // numericUpDownBalanceCost
            // 
            numericUpDownBalanceCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownBalanceCost.DecimalPlaces = 2;
            numericUpDownBalanceCost.Location = new System.Drawing.Point(159, 66);
            numericUpDownBalanceCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownBalanceCost.Name = "numericUpDown2";
            numericUpDownBalanceCost.Size = new System.Drawing.Size(318, 20);
            numericUpDownBalanceCost.ThousandsSeparator = true;
            numericUpDownBalanceCost.TabStop = true;
            numericUpDownBalanceCost.TabIndex = 7;
            // 
            // numericUpDownCadastralCost
            // 
            numericUpDownCadastralCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownCadastralCost.DecimalPlaces = 2;
            numericUpDownCadastralCost.Location = new System.Drawing.Point(159, 36);
            numericUpDownCadastralCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownCadastralCost.Name = "numericUpDownCadastralCost";
            numericUpDownCadastralCost.Size = new System.Drawing.Size(318, 20);
            numericUpDownCadastralCost.ThousandsSeparator = true;
            numericUpDownCadastralCost.TabStop = true;
            numericUpDownCadastralCost.TabIndex = 6;
            // 
            // numericUpDownStartupYear
            // 
            numericUpDownStartupYear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownStartupYear.Location = new System.Drawing.Point(157, 95);
            numericUpDownStartupYear.Maximum = new decimal(new int[] {
            DateTime.Now.Year,
            0,
            0,
            0});
            numericUpDownStartupYear.Minimum = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            numericUpDownStartupYear.Name = "numericUpDownStartupYear";
            numericUpDownStartupYear.Size = new System.Drawing.Size(319, 20);
            numericUpDownStartupYear.Value = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            numericUpDownStartupYear.TabStop = true;
            numericUpDownStartupYear.TabIndex = 3;
            // 
            // numericUpDownLivingArea
            // 
            numericUpDownLivingArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownLivingArea.DecimalPlaces = 3;
            numericUpDownLivingArea.Location = new System.Drawing.Point(163, 18);
            numericUpDownLivingArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            numericUpDownLivingArea.Size = new System.Drawing.Size(319, 20);
            numericUpDownLivingArea.ThousandsSeparator = true;
            // 
            // numericUpDownMunicipalArea
            // 
            numericUpDownMunicipalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownMunicipalArea.DecimalPlaces = 3;
            numericUpDownMunicipalArea.Location = new System.Drawing.Point(163, 47);
            numericUpDownMunicipalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            numericUpDownMunicipalArea.Name = "numericUpDownMunicipalArea";
            numericUpDownMunicipalArea.ReadOnly = true;
            numericUpDownMunicipalArea.Size = new System.Drawing.Size(319, 20);
            numericUpDownMunicipalArea.ThousandsSeparator = true;
            // 
            // numericUpDownOtherPremisesCount
            // 
            numericUpDownOtherPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownOtherPremisesCount.Location = new System.Drawing.Point(159, 105);
            numericUpDownOtherPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            numericUpDownOtherPremisesCount.Name = "numericUpDownOtherPremisesCount";
            numericUpDownOtherPremisesCount.ReadOnly = true;
            numericUpDownOtherPremisesCount.Size = new System.Drawing.Size(319, 20);
            numericUpDownOtherPremisesCount.TabIndex = 3;
            numericUpDownOtherPremisesCount.TabStop = true;
            // 
            // numericUpDownSpecialPremisesCount
            // 
            numericUpDownSpecialPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownSpecialPremisesCount.Location = new System.Drawing.Point(159, 76);
            numericUpDownSpecialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            numericUpDownSpecialPremisesCount.Name = "numericUpDownSpecialPremisesCount";
            numericUpDownSpecialPremisesCount.ReadOnly = true;
            numericUpDownSpecialPremisesCount.Size = new System.Drawing.Size(319, 20);
            numericUpDownSpecialPremisesCount.TabIndex = 2;
            numericUpDownSpecialPremisesCount.TabStop = true;
            // 
            // numericUpDownPremisesCount
            // 
            numericUpDownPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownPremisesCount.Location = new System.Drawing.Point(163, 18);
            numericUpDownPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            numericUpDownPremisesCount.Name = "numericUpDownPremisesCount";
            numericUpDownPremisesCount.Size = new System.Drawing.Size(319, 20);
            // 
            // numericUpDownRoomsCount
            // 
            numericUpDownRoomsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownRoomsCount.Location = new System.Drawing.Point(163, 47);
            numericUpDownRoomsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            numericUpDownRoomsCount.Name = "numericUpDownRoomsCount";
            numericUpDownRoomsCount.Size = new System.Drawing.Size(319, 20);
            // 
            // numericUpDownApartmentsCount
            // 
            numericUpDownApartmentsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownApartmentsCount.Location = new System.Drawing.Point(163, 76);
            numericUpDownApartmentsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            numericUpDownApartmentsCount.Name = "numericUpDownApartmentsCount";
            numericUpDownApartmentsCount.Size = new System.Drawing.Size(319, 20);
            // 
            // numericUpDownSharedApartmentsCount
            // 
            numericUpDownSharedApartmentsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownSharedApartmentsCount.Location = new System.Drawing.Point(163, 105);
            numericUpDownSharedApartmentsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            numericUpDownSharedApartmentsCount.Name = "numericUpDownSharedApartmentsCount";
            numericUpDownSharedApartmentsCount.Size = new System.Drawing.Size(319, 20);
            // 
            // numericUpDownCommercialPremisesCount
            // 
            numericUpDownCommercialPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownCommercialPremisesCount.Location = new System.Drawing.Point(158, 47);
            numericUpDownCommercialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            numericUpDownCommercialPremisesCount.Name = "numericUpDownCommercialPremisesCount";
            numericUpDownCommercialPremisesCount.ReadOnly = true;
            numericUpDownCommercialPremisesCount.Size = new System.Drawing.Size(319, 20);
            numericUpDownCommercialPremisesCount.TabIndex = 1;
            numericUpDownCommercialPremisesCount.TabStop = true;
            // 
            // numericUpDownSocialPremisesCount
            // 
            numericUpDownSocialPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            numericUpDownSocialPremisesCount.Location = new System.Drawing.Point(158, 18);
            numericUpDownSocialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            numericUpDownSocialPremisesCount.Name = "numericUpDownSocialPremisesCount";
            numericUpDownSocialPremisesCount.ReadOnly = true;
            numericUpDownSocialPremisesCount.Size = new System.Drawing.Size(319, 20);
            numericUpDownSocialPremisesCount.TabIndex = 0;
            numericUpDownSocialPremisesCount.TabStop = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 10);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 13);
            label1.Text = "Улица";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 39);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(70, 13);
            label2.Text = "Номер дома";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(10, 68);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(91, 13);
            label3.Text = "Этажность дома";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(16, 20);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(37, 13);
            label4.Text = "Всего";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(16, 78);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(49, 13);
            label5.Text = "Квартир";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(16, 49);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(45, 13);
            label6.Text = "Комнат";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(16, 107);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(129, 13);
            label7.Text = "Квартир с подселением";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(16, 49);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(88, 13);
            label8.Text = "Муниципальных";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(16, 20);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(43, 13);
            label9.Text = "Жилых";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(16, 107);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(44, 13);
            label10.Text = "Прочие";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(16, 78);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(118, 13);
            label11.Text = "Специализированный";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(16, 49);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(83, 13);
            label12.Text = "Коммерческий";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(16, 20);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(70, 13);
            label13.Text = "Социальный";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(16, 10);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(110, 13);
            label14.Text = "Кадастровый номер";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(16, 39);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(130, 13);
            label15.Text = "Кадастровая стоимость";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(16, 68);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(125, 13);
            label16.Text = "Балансовая стоимость";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(10, 97);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(142, 13);
            label17.Text = "Год ввода в эксплуатацию";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new System.Drawing.Point(10, 126);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(134, 13);
            label18.Text = "Тип строения (материал)";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(16, 126);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(142, 13);
            label19.Text = "Текущий тип найма";
            label19.Visible = false;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(15, 98);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(108, 13);
            this.label40.TabIndex = 31;
            this.label40.Text = "Текущее состояние";
            // 
            // panel1
            // 
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
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(480, 154);
            // 
            // panel2
            // 
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
            panel2.Controls.Add(maskedTextBoxCadastralNum);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(489, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(480, 154);
            // 
            // textBoxHouse
            // 
            textBoxHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            textBoxHouse.Location = new System.Drawing.Point(157, 36);
            textBoxHouse.Name = "textBoxHouse";
            textBoxHouse.Size = new System.Drawing.Size(319, 20);
            textBoxHouse.TabStop = true;
            textBoxHouse.TabIndex = 1;
            textBoxHouse.MaxLength = 4;
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.MaxLength = 255;
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.MaxLength = 255;
            // 
            // maskedTextBoxCadastralNum
            // 
            maskedTextBoxCadastralNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            maskedTextBoxCadastralNum.Location = new System.Drawing.Point(159, 7);
            maskedTextBoxCadastralNum.Name = "maskedTextBoxCadastralNum";
            maskedTextBoxCadastralNum.Size = new System.Drawing.Size(318, 20);
            maskedTextBoxCadastralNum.TabStop = true;
            maskedTextBoxCadastralNum.TabIndex = 5;
            //
            // dataGridViewOwnerships
            // 
            dataGridViewOwnerships.AllowUserToAddRows = false;
            dataGridViewOwnerships.AllowUserToDeleteRows = false;
            dataGridViewOwnerships.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewOwnerships.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            field_ownership_number,
            field_ownership_date,
            field_ownership_description,
            field_id_ownership_type});
            dataGridViewOwnerships.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridViewOwnerships.Location = new System.Drawing.Point(3, 16);
            dataGridViewOwnerships.Name = "dataGridViewOwnerships";
            dataGridViewOwnerships.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOwnerships.Size = new System.Drawing.Size(480, 101);
            dataGridViewOwnerships.AutoGenerateColumns = false;
            // 
            // field_ownership_number
            // 
            field_ownership_number.HeaderText = "Номер";
            field_ownership_number.Name = "number";
            field_ownership_number.ReadOnly = true;
            // 
            // field_ownership_date
            // 
            field_ownership_date.HeaderText = "Дата";
            field_ownership_date.Name = "date";
            field_ownership_date.ReadOnly = true;
            //
            // field_ownership_description
            // 
            field_ownership_description.HeaderText = "Наименование";
            field_ownership_description.Name = "description";
            field_ownership_description.ReadOnly = true;
            //
            // field_id_ownership_type
            //
            field_id_ownership_type.HeaderText = "Тип ограничения";
            field_id_ownership_type.Name = "id_ownership_type";
            field_id_ownership_type.ReadOnly = true;
            field_id_ownership_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // dataGridViewRestrictions
            // 
            dataGridViewRestrictions.AllowUserToAddRows = false;
            dataGridViewRestrictions.AllowUserToDeleteRows = false;
            dataGridViewRestrictions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewRestrictions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            field_restriction_number,
            field_restriction_date,
            field_restriction_description,
            field_id_restriction_type});
            dataGridViewRestrictions.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridViewRestrictions.Location = new System.Drawing.Point(3, 16);
            dataGridViewRestrictions.Name = "dataGridViewRestrictions";
            dataGridViewRestrictions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridViewRestrictions.Size = new System.Drawing.Size(480, 101);
            dataGridViewRestrictions.AutoGenerateColumns = false;
            // 
            // field_restriction_number
            // 
            field_restriction_number.HeaderText = "Номер";
            field_restriction_number.Name = "number";
            field_restriction_number.ReadOnly = true;
            // 
            // field_restriction_date
            // 
            field_restriction_date.HeaderText = "Дата";
            field_restriction_date.Name = "date";
            field_restriction_date.ReadOnly = true;
            // 
            // field_restriction_description
            // 
            field_restriction_description.HeaderText = "Наименование";
            field_restriction_description.Name = "description";
            field_restriction_description.ReadOnly = true;
            //
            // field_id_restriction_type
            //
            field_id_restriction_type.HeaderText = "Тип права собственности";
            field_id_restriction_type.Name = "id_restriction_type";
            field_id_restriction_type.ReadOnly = true;
            field_id_restriction_type.MinimumWidth = 150;
            field_id_restriction_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // comboBoxStreet
            // 
            comboBoxStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            comboBoxStreet.FormattingEnabled = true;
            comboBoxStreet.Location = new System.Drawing.Point(157, 7);
            comboBoxStreet.Name = "comboBoxStreet";
            comboBoxStreet.Size = new System.Drawing.Size(319, 21);
            comboBoxStreet.TabStop = true;
            comboBoxStreet.TabIndex = 0;
            // 
            // comboBoxCurrentFundType
            // 
            comboBoxCurrentFundType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            comboBoxCurrentFundType.FormattingEnabled = true;
            comboBoxCurrentFundType.Location = new System.Drawing.Point(159, 124);
            comboBoxCurrentFundType.Name = "comboBoxStreet";
            comboBoxCurrentFundType.Size = new System.Drawing.Size(319, 21);
            comboBoxCurrentFundType.TabStop = true;
            comboBoxCurrentFundType.TabIndex = 8;
            comboBoxCurrentFundType.Visible = false;
            comboBoxCurrentFundType.Enabled = false;
            comboBoxCurrentFundType.DropDownStyle = ComboBoxStyle.DropDownList;
            // 
            // comboBoxState
            // 
            this.comboBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxState.FormattingEnabled = true;
            this.comboBoxState.Location = new System.Drawing.Point(159, 95);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new System.Drawing.Size(318, 21);
            this.comboBoxState.TabIndex = 30;
            // 
            // comboBoxStructureType
            // 
            comboBoxStructureType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            comboBoxStructureType.FormattingEnabled = true;
            comboBoxStructureType.Location = new System.Drawing.Point(157, 123);
            comboBoxStructureType.Name = "comboBoxStructureType";
            comboBoxStructureType.Size = new System.Drawing.Size(319, 21);
            comboBoxStructureType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxStructureType.TabStop = true;
            comboBoxStructureType.TabIndex = 4;
            // 
            // checkBoxImprovement
            // 
            checkBoxImprovement.AutoSize = true;
            checkBoxImprovement.Location = new System.Drawing.Point(19, 96);
            checkBoxImprovement.Name = "checkBoxImprovement";
            checkBoxImprovement.Size = new System.Drawing.Size(159, 125);
            checkBoxImprovement.Text = "Благоустройство";
            checkBoxImprovement.UseVisualStyleBackColor = true;
            checkBoxImprovement.TabStop = true;
            checkBoxImprovement.TabIndex = 9;
            // 
            // checkBoxElevator
            // 
            checkBoxElevator.AutoSize = true;
            checkBoxElevator.Location = new System.Drawing.Point(19, 125);
            checkBoxElevator.Name = "checkBoxElevator";
            checkBoxElevator.Size = new System.Drawing.Size(103, 17);
            checkBoxElevator.Text = "Наличие лифта";
            checkBoxElevator.UseVisualStyleBackColor = true;
            checkBoxElevator.TabStop = true;
            checkBoxElevator.TabIndex = 10;

            tableLayoutPanel.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(numericUpDownFloors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownBalanceCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownCadastralCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownStartupYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownLivingArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownMunicipalArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownOtherPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownSpecialPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownRoomsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownApartmentsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownSharedApartmentsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownCommercialPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(numericUpDownSocialPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(dataGridViewRestrictions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(dataGridViewOwnerships)).EndInit();
            tableLayoutPanel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
