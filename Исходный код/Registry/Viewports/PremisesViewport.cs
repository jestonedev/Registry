using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels;

namespace Registry.Viewport
{
    internal class PremisesViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel3 = new TableLayoutPanel();
        private TableLayoutPanel tableLayoutPanel4 = new TableLayoutPanel();
        private TableLayoutPanel tableLayoutPanel5 = new TableLayoutPanel();
        private GroupBox groupBox8 = new GroupBox();
        private GroupBox groupBox9 = new GroupBox();
        private GroupBox groupBox10 = new GroupBox();
        private GroupBox groupBox11 = new GroupBox();
        private GroupBox groupBox12 = new GroupBox();
        private GroupBox groupBox13 = new GroupBox();
        private GroupBox groupBoxRooms = new GroupBox();
        private Panel panel3 = new Panel();
        private Panel panel4 = new Panel();
        private DataGridView dataGridViewRestrictions = new DataGridView();
        private DataGridView dataGridViewOwnerships = new DataGridView();
        private DataGridView dataGridViewRooms = new DataGridView();
        private DataGridViewComboBoxColumn field_id_restriction_type = new DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_restriction_number = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_restriction_date = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_restriction_description = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_ownership_type = new DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_ownership_number = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_ownership_date = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_ownership_description = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_sub_premises_num = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_sub_premises_total_area = new DataGridViewTextBoxColumn();
        private NumericUpDown numericUpDownFloor = new NumericUpDown();
        private NumericUpDown numericUpDownBalanceCost = new NumericUpDown();
        private NumericUpDown numericUpDownCadastralCost = new NumericUpDown();
        private NumericUpDown numericUpDownLivingArea = new NumericUpDown();
        private NumericUpDown numericUpDownTotalArea = new NumericUpDown();
        private NumericUpDown numericUpDownNumBeds = new NumericUpDown();
        private CheckBox checkBoxForOrphans = new CheckBox();
        private CheckBox checkBoxAcceptByExchange = new CheckBox();
        private CheckBox checkBoxAcceptByDonation = new CheckBox();
        private CheckBox checkBoxAcceptByOther = new CheckBox();
        private Label label19 = new Label();
        private Label label20 = new Label();
        private Label label21 = new Label();
        private Label label22 = new Label();
        private Label label23 = new Label();
        private Label label24 = new Label();
        private Label label25 = new Label();
        private Label label26 = new Label();
        private Label label27 = new Label();
        private Label label28 = new Label();
        private Label label29 = new Label();
        private ComboBox comboBoxHouse = new ComboBox();
        private ComboBox comboBoxStreet = new ComboBox();
        private TextBox textBoxPremisesNumber = new TextBox();
        private TextBox textBoxDescription = new TextBox();
        private TextBox textBoxSubPremisesNumber = new TextBox();
        private ComboBox comboBoxPremisesType = new ComboBox();
        private ComboBox comboBoxPremisesKind = new ComboBox();
        private MaskedTextBox maskedTextBoxCadastralNum = new MaskedTextBox();
        #endregion Components

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        //Models
        private PremisesDataModel premises = null;
        private BuildingsDataModel buildings = null;
        private KladrDataModel kladr = null;
        private PremisesTypesDataModel premises_types = null;
        private PremisesKindsDataModel premises_kinds = null;
        private SubPremisesDataModel sub_premises = null;
        private RestrictionsDataModel restrictions = null;
        private RestrictionTypesDataModel restrictionTypes = null;
        private RestrictionsPremisesAssocDataModel restrictionPremisesAssoc = null;
        private OwnershipsRightsDataModel ownershipRights = null;
        private OwnershipRightTypesDataModel ownershipRightTypes = null;
        private OwnershipPremisesAssocDataModel ownershipPremisesAssoc = null;
        

        //Views
        private BindingSource v_premises = null;
        private BindingSource v_buildings = null;
        private BindingSource v_kladr = null;
        private BindingSource v_premises_types = null;
        private BindingSource v_premises_kinds = null;
        private BindingSource v_sub_premises = null;
        private BindingSource v_restrictions = null;
        private BindingSource v_restrictonTypes = null;
        private BindingSource v_restrictionPremisesAssoc = null;
        private BindingSource v_ownershipRights = null;
        private BindingSource v_ownershipRightTypes = null;
        private BindingSource v_ownershipPremisesAssoc = null;

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;
        private bool is_first_visibility = true;

        public PremisesViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPagePremise";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Помещение";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public PremisesViewport(PremisesViewport premsiesListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = premsiesListViewport.DynamicFilter;
            this.StaticFilter = premsiesListViewport.StaticFilter;
            this.ParentRow = premsiesListViewport.ParentRow;
            this.ParentType = premsiesListViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            premises = PremisesDataModel.GetInstance();
            kladr = KladrDataModel.GetInstance();
            buildings = BuildingsDataModel.GetInstance();
            premises_types = PremisesTypesDataModel.GetInstance();
            premises_kinds = PremisesKindsDataModel.GetInstance();
            sub_premises = SubPremisesDataModel.GetInstance();
            restrictions = RestrictionsDataModel.GetInstance();
            restrictionTypes = RestrictionTypesDataModel.GetInstance();
            restrictionPremisesAssoc = RestrictionsPremisesAssocDataModel.GetInstance();
            ownershipRights = OwnershipsRightsDataModel.GetInstance();
            ownershipRightTypes = OwnershipRightTypesDataModel.GetInstance();
            ownershipPremisesAssoc = OwnershipPremisesAssocDataModel.GetInstance();

            DataSet ds = DataSetManager.GetDataSet();

            v_kladr = new BindingSource();
            v_kladr.DataMember = "kladr";
            v_kladr.DataSource = ds;

            v_buildings = new BindingSource();
            v_buildings.DataMember = "kladr_buildings";
            v_buildings.DataSource = v_kladr;

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += new EventHandler(v_premises_CurrentItemChanged);
            v_premises.DataMember = "premises";
            v_premises.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_premises.Filter += " AND ";
            v_premises.Filter += DynamicFilter;
            v_premises.DataSource = ds;

            v_premises_types = new BindingSource();
            v_premises_types.DataMember = "premises_types";
            v_premises_types.DataSource = ds;

            v_premises_kinds = new BindingSource();
            v_premises_kinds.DataMember = "premises_kinds";
            v_premises_kinds.DataSource = ds;

            v_restrictions = new BindingSource();
            v_restrictions.DataMember = "restrictions";
            v_restrictions.DataSource = ds;

            v_ownershipRights = new BindingSource();
            v_ownershipRights.DataMember = "ownership_rights";
            v_ownershipRights.DataSource = ds;

            v_restrictonTypes = new BindingSource();
            v_restrictonTypes.DataSource = ds;
            v_restrictonTypes.DataMember = "restriction_types";

            v_ownershipRightTypes = new BindingSource();
            v_ownershipRightTypes.DataMember = "ownership_right_types";
            v_ownershipRightTypes.DataSource = ds;

            v_sub_premises = new BindingSource();
            v_sub_premises.DataMember = "premises_sub_premises";
            v_sub_premises.DataSource = v_premises;

            v_restrictionPremisesAssoc = new BindingSource();
            v_restrictionPremisesAssoc.DataMember = "premises_restrictions_premises_assoc";
            v_restrictionPremisesAssoc.CurrentItemChanged += new EventHandler(v_restrictionPremisesAssoc_CurrentItemChanged);
            v_restrictionPremisesAssoc.DataSource = v_premises;
            v_restrictionPremisesAssoc_CurrentItemChanged(null, new EventArgs());

            v_ownershipPremisesAssoc = new BindingSource();
            v_ownershipPremisesAssoc.DataMember = "premises_ownership_premises_assoc";
            v_ownershipPremisesAssoc.CurrentItemChanged += new EventHandler(v_ownershipPremisesAssoc_CurrentItemChanged);
            v_ownershipPremisesAssoc.DataSource = v_premises;
            v_ownershipPremisesAssoc_CurrentItemChanged(null, new EventArgs());

            DataBind();

            comboBoxStreet.Leave += new EventHandler(comboBoxStreet_Leave);
            comboBoxStreet.KeyUp += new KeyEventHandler(comboBoxStreet_KeyUp);
            comboBoxStreet.DropDownClosed += new EventHandler(comboBoxStreet_DropDownClosed);
            comboBoxStreet.SelectedIndexChanged += new EventHandler(comboBoxStreet_SelectedIndexChanged);
            comboBoxStreet.VisibleChanged += new EventHandler(comboBoxStreet_VisibleChanged);
            comboBoxHouse.SelectedIndexChanged += new EventHandler(comboBoxHouse_SelectedIndexChanged);
            comboBoxPremisesType.SelectedIndexChanged += new EventHandler(comboBoxPremisesType_SelectedIndexChanged);
            comboBoxPremisesKind.SelectedIndexChanged += new EventHandler(comboBoxPremisesKind_SelectedIndexChanged);
            textBoxPremisesNumber.TextChanged += new EventHandler(textBoxPremisesNumber_TextChanged);
            numericUpDownFloor.ValueChanged += new EventHandler(numericUpDownFloor_ValueChanged);
            maskedTextBoxCadastralNum.TextChanged += new EventHandler(maskedTextBoxCadastralNum_TextChanged);
            numericUpDownCadastralCost.ValueChanged += new EventHandler(numericUpDownCadastralCost_ValueChanged);
            numericUpDownBalanceCost.ValueChanged += new EventHandler(numericUpDownBalanceCost_ValueChanged);
            numericUpDownNumBeds.ValueChanged += new EventHandler(numericUpDownNumBeds_ValueChanged);
            numericUpDownTotalArea.ValueChanged += new EventHandler(numericUpDownTotalArea_ValueChanged);
            numericUpDownLivingArea.ValueChanged += new EventHandler(numericUpDownLivingArea_ValueChanged);
            checkBoxAcceptByDonation.CheckedChanged += new EventHandler(checkBoxAcceptByDonation_CheckedChanged);
            checkBoxAcceptByExchange.CheckedChanged += new EventHandler(checkBoxAcceptByExchange_CheckedChanged);
            checkBoxAcceptByOther.CheckedChanged += new EventHandler(checkBoxAcceptByOther_CheckedChanged);
            checkBoxForOrphans.CheckedChanged += new EventHandler(checkBoxForOrphans_CheckedChanged);
        }

        void comboBoxStreet_VisibleChanged(object sender, EventArgs e)
        {
            if (is_first_visibility)
            {
                SelectCurrentBuilding();
                is_first_visibility = false;
            }
        }

        void comboBoxHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxStreet_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
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

        void checkBoxForOrphans_CheckedChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void checkBoxAcceptByOther_CheckedChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void checkBoxAcceptByExchange_CheckedChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void checkBoxAcceptByDonation_CheckedChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownLivingArea_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownTotalArea_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownNumBeds_ValueChanged(object sender, EventArgs e)
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

        void maskedTextBoxCadastralNum_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxPremisesKind_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxPremisesType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownFloor_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxPremisesNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void v_ownershipPremisesAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            string ownershipFilter = "id_ownership_right IN (0";
            for (int i = 0; i < v_ownershipPremisesAssoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownershipPremisesAssoc[i])["id_ownership_right"].ToString() + ",";
            ownershipFilter = ownershipFilter.TrimEnd(new char[] { ',' });
            ownershipFilter += ")";
            v_ownershipRights.Filter = ownershipFilter;
        }

        void v_restrictionPremisesAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            string restrictionsFilter = "id_restriction IN (0";
            for (int i = 0; i < v_restrictionPremisesAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)v_restrictionPremisesAssoc[i])["id_restriction"].ToString() + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(new char[] { ',' });
            restrictionsFilter += ")";
            v_restrictions.Filter = restrictionsFilter;
        }

        void v_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            if (viewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                {
                    this.Text = String.Format("Новое помещение здания №{0}", ParentRow["id_building"]);
                } else
                    this.Text = "Новое помещение";
            }
            else
                if (v_premises.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                        this.Text = String.Format("Помещение №{0} здания №{1}",
                            ((DataRowView)v_premises[v_premises.Position])["id_premises"], ParentRow["id_building"]);
                    else
                        this.Text = String.Format("Помещение №{0}", ((DataRowView)v_premises[v_premises.Position])["id_premises"]);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                        this.Text = String.Format("Помещения в здании {0} отсутствуют", ParentRow["id_building"]);
                    else
                        this.Text = "Помещения отсутствуют";
                }
            SelectCurrentBuilding();
            menuCallback.NavigationStateUpdate();
            if (v_premises.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        private void SelectCurrentBuilding()
        {
            if ((comboBoxHouse.DataSource != null) && (comboBoxStreet.DataSource != null))
            {
                int? id_building = null;
                if ((v_premises.Position != -1) && (((DataRowView)v_premises[v_premises.Position])["id_building"] != DBNull.Value))
                    id_building = Convert.ToInt32(((DataRowView)v_premises[v_premises.Position])["id_building"]);
                else 
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                    id_building = Convert.ToInt32(ParentRow["id_building"]);
                string id_street = null;
                if (id_building != null)
                {
                    DataRow building_row = buildings.Select().Rows.Find(id_building);
                    if (building_row != null)
                        id_street = building_row["id_street"].ToString();
                }
                if (id_street != null)
                    comboBoxStreet.SelectedValue = id_street;
                else
                    comboBoxStreet.SelectedValue = DBNull.Value;
                if (id_building != null)
                    comboBoxHouse.SelectedValue = id_building;
                else
                    comboBoxHouse.SelectedValue = DBNull.Value;
            }
        }

        private void DataBind()
        {
            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";
            comboBoxHouse.DataSource = v_buildings;
            comboBoxHouse.ValueMember = "id_building";
            comboBoxHouse.DisplayMember = "house";
            comboBoxPremisesKind.DataSource = v_premises_kinds;
            comboBoxPremisesKind.ValueMember = "id_premises_kind";
            comboBoxPremisesKind.DisplayMember = "premises_kind";
            comboBoxPremisesKind.DataBindings.Clear();
            comboBoxPremisesKind.DataBindings.Add("SelectedValue", v_premises, "id_premises_kind", true, DataSourceUpdateMode.Never, 1);
            comboBoxPremisesType.DataSource = v_premises_types;
            comboBoxPremisesType.ValueMember = "id_premises_type";
            comboBoxPremisesType.DisplayMember = "premises_type_as_num";
            comboBoxPremisesType.DataBindings.Clear();
            comboBoxPremisesType.DataBindings.Add("SelectedValue", v_premises, "id_premises_type", true, DataSourceUpdateMode.Never, 1);
            textBoxPremisesNumber.DataBindings.Clear();
            textBoxPremisesNumber.DataBindings.Add("Text", v_premises, "premises_num", true, DataSourceUpdateMode.Never, "");
            numericUpDownFloor.DataBindings.Clear();
            numericUpDownFloor.DataBindings.Add("Value", v_premises, "floor", true, DataSourceUpdateMode.Never, 0);
            maskedTextBoxCadastralNum.DataBindings.Clear();
            maskedTextBoxCadastralNum.DataBindings.Add("Text", v_premises, "cadastral_num");
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_premises, "description", true, DataSourceUpdateMode.Never, "");
            numericUpDownCadastralCost.DataBindings.Clear();
            numericUpDownCadastralCost.DataBindings.Add("Value", v_premises, "cadastral_cost", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceCost.DataBindings.Clear();
            numericUpDownBalanceCost.DataBindings.Add("Value", v_premises, "balance_cost", true, DataSourceUpdateMode.Never, 0);
            numericUpDownNumBeds.DataBindings.Clear();
            numericUpDownNumBeds.DataBindings.Add("Value", v_premises, "num_beds", true, DataSourceUpdateMode.Never, 0);
            numericUpDownTotalArea.DataBindings.Clear();
            numericUpDownTotalArea.DataBindings.Add("Value", v_premises, "total_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownLivingArea.DataBindings.Clear();
            numericUpDownLivingArea.DataBindings.Add("Value", v_premises, "living_area", true, DataSourceUpdateMode.Never, 0);
            checkBoxForOrphans.DataBindings.Clear();
            checkBoxForOrphans.DataBindings.Add("Checked", v_premises, "for_orphans", true, DataSourceUpdateMode.Never, false);
            checkBoxAcceptByDonation.DataBindings.Clear();
            checkBoxAcceptByDonation.DataBindings.Add("Checked", v_premises, "accepted_by_donation", true, DataSourceUpdateMode.Never, false);
            checkBoxAcceptByExchange.DataBindings.Clear();
            checkBoxAcceptByExchange.DataBindings.Add("Checked", v_premises, "accepted_by_exchange", true, DataSourceUpdateMode.Never, false);
            checkBoxAcceptByOther.DataBindings.Clear();
            checkBoxAcceptByOther.DataBindings.Add("Checked", v_premises, "accepted_by_other", true, DataSourceUpdateMode.Never, false);

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

            dataGridViewRooms.DataSource = v_sub_premises;
            field_sub_premises_num.DataPropertyName = "sub_premises_num";
            field_sub_premises_total_area.DataPropertyName = "total_area";
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if (!this.ContainsFocus)
                return;
            if ((v_premises.Position != -1) && (PremiseFromView() != PremiseFromViewport()))
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

        private Premise PremiseFromView()
        {
            Premise premise = new Premise();
            DataRowView row = (DataRowView)v_premises[v_premises.Position];
            if (row["id_premises"] is DBNull)
                premise.id_premises = null;
            else
                premise.id_premises = Convert.ToInt32(row["id_premises"]);
            if (row["id_building"] is DBNull)
                premise.id_building = null;
            else
                premise.id_building = Convert.ToInt32(row["id_building"]);
            if (row["premises_num"] is DBNull)
                premise.premises_num = null;
            else
                premise.premises_num = row["premises_num"].ToString();
            premise.living_area = Convert.ToDouble(row["living_area"]);
            premise.total_area = Convert.ToDouble(row["total_area"]);
            premise.num_beds = Convert.ToInt16(row["num_beds"]);
            if (row["id_premises_type"] is DBNull)
                premise.id_premises_type = null;
            else
                premise.id_premises_type = Convert.ToInt32(row["id_premises_type"]);
            if (row["id_premises_kind"] is DBNull)
                premise.id_premises_kind = null;
            else
                premise.id_premises_kind = Convert.ToInt32(row["id_premises_kind"]);
            premise.floor = Convert.ToInt16(row["floor"]);
            premise.for_orphans = Convert.ToBoolean(row["for_orphans"]);
            premise.accepted_by_exchange = Convert.ToBoolean(row["accepted_by_exchange"]);
            premise.accepted_by_donation = Convert.ToBoolean(row["accepted_by_donation"]);
            premise.accepted_by_other = Convert.ToBoolean(row["accepted_by_other"]);
            if (row["cadastral_num"] is DBNull)
                premise.cadastral_num = null;
            else
                premise.cadastral_num = row["cadastral_num"].ToString();
            premise.cadastral_cost = Convert.ToDecimal(row["cadastral_cost"]);
            premise.balance_cost = Convert.ToDecimal(row["balance_cost"]);
            if (row["description"] is DBNull)
                premise.description = null;
            else
                premise.description = row["description"].ToString();
            return premise;
        }

        private Premise PremiseFromViewport()
        {
            Premise premise = new Premise();
            if ((v_premises.Position == -1) || ((DataRowView)v_premises[v_premises.Position])["id_premises"] is DBNull)
                premise.id_premises = null;
            else
                premise.id_premises = Convert.ToInt32(((DataRowView)v_premises[v_premises.Position])["id_premises"]);
            if (comboBoxHouse.SelectedValue != null)
                premise.id_building = Convert.ToInt32(comboBoxHouse.SelectedValue);
            else
                premise.id_building = null;
            if (textBoxPremisesNumber.Text.Trim() == "")
                premise.premises_num = null;
            else
                premise.premises_num = textBoxPremisesNumber.Text.Trim();
            premise.total_area = Convert.ToDouble(numericUpDownTotalArea.Value);
            premise.living_area = Convert.ToDouble(numericUpDownLivingArea.Value);
            premise.num_beds = Convert.ToInt16(numericUpDownNumBeds.Value);
            if (comboBoxPremisesType.SelectedValue == null)
                premise.id_premises_type = null;
            else
                premise.id_premises_type = Convert.ToInt32(comboBoxPremisesType.SelectedValue);
            if (comboBoxPremisesKind.SelectedValue == null)
                premise.id_premises_kind = null;
            else
                premise.id_premises_kind = Convert.ToInt32(comboBoxPremisesKind.SelectedValue);
            premise.floor = Convert.ToInt16(numericUpDownFloor.Value);
            premise.for_orphans = checkBoxForOrphans.Checked;
            premise.accepted_by_donation = checkBoxAcceptByDonation.Checked;
            premise.accepted_by_exchange = checkBoxAcceptByExchange.Checked;
            premise.accepted_by_other = checkBoxAcceptByOther.Checked;
            if (maskedTextBoxCadastralNum.Text.Trim() == "")
                premise.cadastral_num = null;
            else
                premise.cadastral_num = maskedTextBoxCadastralNum.Text.Trim();
            premise.cadastral_cost = numericUpDownCadastralCost.Value;
            premise.balance_cost = numericUpDownBalanceCost.Value;
            if (textBoxDescription.Text.Trim() == "")
                premise.description = null;
            else
                premise.description = textBoxDescription.Text.Trim().ToString();
            return premise;
        }

        private void ViewportFromPremise(Premise premise)
        {
            if (premise.id_building != null)
            {
                DataRow building_row = buildings.Select().Rows.Find(premise.id_building);
                if (building_row != null)
                {
                    string id_street = building_row["id_street"].ToString();
                    comboBoxStreet.SelectedValue = id_street;
                    comboBoxHouse.SelectedValue = premise.id_building;
                }
            }
            if (premise.id_premises_type != null)
                comboBoxPremisesType.SelectedValue = premise.id_premises_type;
            if (premise.id_premises_kind != null)
                comboBoxPremisesKind.SelectedValue = premise.id_premises_kind;
            textBoxPremisesNumber.Text = premise.premises_num;
            numericUpDownFloor.Value = premise.floor.Value;
            maskedTextBoxCadastralNum.Text = premise.cadastral_num;
            numericUpDownCadastralCost.Value = premise.cadastral_cost.Value;
            numericUpDownBalanceCost.Value = premise.balance_cost.Value;
            numericUpDownNumBeds.Value = premise.num_beds.Value;
            numericUpDownLivingArea.Value = (decimal)premise.living_area.Value;
            numericUpDownTotalArea.Value = (decimal)premise.total_area.Value;
            textBoxDescription.Text = premise.description;
            checkBoxForOrphans.Checked = premise.for_orphans.Value;
            checkBoxAcceptByDonation.Checked = premise.accepted_by_donation.Value;
            checkBoxAcceptByExchange.Checked = premise.accepted_by_exchange.Value;
            checkBoxAcceptByOther.Checked = premise.accepted_by_other.Value;
        }

        public void LocatePremisesBy(int id)
        {
            v_premises.Position = v_premises.Find("id_premises", id);
        }

        public override void Close()
        {
            if (ChangeViewportStateTo(ViewportState.ReadState))
                base.Close();
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanCopyRecord()
        {
            return (v_premises.Position != -1) && (viewportState == ViewportState.ReadState || viewportState == ViewportState.ModifyRowState)
                    && !premises.EditingNewRecord;
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            Premise premise = PremiseFromView();
            DataRowView row = (DataRowView)v_premises.AddNew();
            premises.EditingNewRecord = true;
            menuCallback.NavigationStateUpdate();
            menuCallback.EditingStateUpdate();
            if (premise.id_building != null)
            {
                comboBoxStreet.SelectedValue = premise.id_building;
                comboBoxHouse.SelectedValue = premise.id_building;
            }
            ViewportFromPremise(premise);
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        private bool ValidatePremise(Premise premise)
        {
            if (premise.id_building == null)
            {
                MessageBox.Show("Необходимо выбрать здание","Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxHouse.Focus();
                return false;
            }
            if ((premise.cadastral_num != null) && (premise.cadastral_num.Length > 15))
            {
                MessageBox.Show("Длина кадастрового номера не может превышать 15 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                maskedTextBoxCadastralNum.Focus();
                return false;
            }
            return true;
        }

        public override void SaveRecord()
        {
            Premise premise = PremiseFromViewport();
            if (!ValidatePremise(premise))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ViewportState.NewRowState:
                    int id_premise = premises.Insert(premise);
                    if (id_premise == -1)
                        return;
                    DataRowView newRow;
                    if (v_premises.Position == -1)
                        newRow = (DataRowView)v_premises.AddNew();
                    else
                        newRow = ((DataRowView)v_premises[v_premises.Position]);
                    premise.id_premises = id_premise;
                    FillRowFromPremise(premise, newRow);
                    newRow.EndEdit();
                    premises.EditingNewRecord = false;
                    this.Text = "Помещение №" + id_premise.ToString();
                    viewportState = ViewportState.ReadState;
                    is_editable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (premise.id_premises == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить помещение без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (premises.Update(premise) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_premises[v_premises.Position]);
                    FillRowFromPremise(premise, row);
                    row.EndEdit();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        private static void FillRowFromPremise(Premise premise, DataRowView row)
        {
            row["id_premises"] = premise.id_premises;
            row["id_building"] = premise.id_building;
            row["premises_num"] = premise.premises_num;
            row["total_area"] = premise.total_area;
            row["living_area"] = premise.living_area;
            row["num_beds"] = premise.num_beds;
            row["id_premises_type"] = premise.id_premises_type;
            row["id_premises_kind"] = premise.id_premises_kind;
            row["floor"] = premise.floor;
            row["for_orphans"] = premise.for_orphans;
            row["accepted_by_exchange"] = premise.accepted_by_exchange;
            row["accepted_by_donation"] = premise.accepted_by_donation;
            row["accepted_by_other"] = premise.accepted_by_other;
            row["cadastral_num"] = premise.cadastral_num;
            row["cadastral_cost"] = premise.cadastral_cost;
            row["balance_cost"] = premise.balance_cost;
            row["description"] = premise.description;
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    viewportState = ViewportState.ReadState;
                    premises.EditingNewRecord = false;
                    if (v_premises.Position != -1)
                    {
                        ((DataRowView)v_premises[v_premises.Position]).Delete();
                        premises.Select().AcceptChanges();
                    }
                    else
                        this.Text = "Здания отсутствуют";
                    menuCallback.EditingStateUpdate();
                    menuCallback.NavigationStateUpdate();
                    break;
                case ViewportState.ModifyRowState:
                    DataBind();
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
                            if (premises.EditingNewRecord)
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

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_premises.MoveFirst();
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_premises.MoveLast();
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_premises.MoveNext();
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_premises.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_premises.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_premises.Position > 0;
        }

        public override void ClearSearch()
        {
            v_premises.Filter = StaticFilter;
            DynamicFilter = "";
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanMoveNext()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override bool CanSearchRecord()
        {
            return (viewportState == ViewportState.ReadState);
        }

        public override bool CanInsertRecord()
        {
            if ((viewportState == ViewportState.ReadState || viewportState == ViewportState.ModifyRowState) 
                    && !premises.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            DataRowView row = (DataRowView)v_premises.AddNew();
            premises.EditingNewRecord = true;
            menuCallback.NavigationStateUpdate();
            menuCallback.EditingStateUpdate();
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
            SearchPremisesForm spForm = new SearchPremisesForm();
            if (spForm.ShowDialog() == DialogResult.OK)
            {
                DynamicFilter = spForm.GetFilter();
                v_premises.Filter = StaticFilter;
                if (StaticFilter != "" && DynamicFilter != "")
                    v_premises.Filter += " AND ";
                v_premises.Filter += DynamicFilter;
                if (DynamicFilter != "")
                    menuCallback.NavigationStateUpdate();
            }
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это помещение?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ((DataRowView)v_premises[v_premises.Position]).Delete();
                menuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanDeleteRecord()
        {
            if ((v_premises.Position == -1) || (viewportState == ViewportState.NewRowState))
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
            PremisesViewport viewport = new PremisesViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            return viewport;
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                premises.EditingNewRecord = false;
            base.Close();
        }

        public override bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        public override bool HasAssocOwnerships()
        {
            return true;
        }

        public override bool HasAssocRestrictions()
        {
            return true;
        }

        public override bool HasAssocSubPremises()
        {
            return true;
        }

        public override bool HasFundHistory()
        {
            return true;
        }

        public override void ShowOwnerships()
        {
            //TODO
        }

        public override void ShowRestrictions()
        {
            //TODO
        }

        public override void ShowSubPremises()
        {
            //TODO
        }

        public override void ShowFundHistory()
        {
            //TODO
        }

        private void ConstructViewport()
        {
            this.Controls.Add(this.tableLayoutPanel3);
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBoxRooms.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumBeds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRooms)).BeginInit();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox13, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.groupBox12, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupBox9, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.groupBox10, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.groupBox8, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupBoxRooms, 0, 2);
            

            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 185F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 131F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(984, 531);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.panel3, 0, 0); 
            this.tableLayoutPanel4.Controls.Add(this.panel4, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(972, 160);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Controls.Add(this.groupBox11, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 188);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(486, 73);
            this.tableLayoutPanel5.TabIndex = 16;
            // 
            // groupBox8
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox8, 2);
            this.groupBox8.Controls.Add(this.tableLayoutPanel4);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(978, 179);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Общие сведения";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.dataGridViewRestrictions);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(3, 333);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(486, 195);
            this.groupBox9.TabIndex = 13;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Реквизиты НПА";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.dataGridViewOwnerships);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Location = new System.Drawing.Point(495, 333);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(486, 195);
            this.groupBox10.TabIndex = 11;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Ограничения";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.numericUpDownLivingArea);
            this.groupBox11.Controls.Add(this.numericUpDownTotalArea);
            this.groupBox11.Controls.Add(this.label25);
            this.groupBox11.Controls.Add(this.label26);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(0, 0);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(486, 69);
            this.groupBox11.TabIndex = 15;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Площадь";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.checkBoxAcceptByOther);
            this.groupBox12.Controls.Add(this.checkBoxAcceptByDonation);
            this.groupBox12.Controls.Add(this.checkBoxAcceptByExchange);
            this.groupBox12.Controls.Add(this.checkBoxForOrphans);
            this.groupBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox12.Location = new System.Drawing.Point(495, 188);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(486, 139);
            this.groupBox12.TabIndex = 15;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Основания на включение в муниципальную собственность";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.textBoxDescription);
            this.groupBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox13.Location = new System.Drawing.Point(0, 69);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(486, 70);
            this.groupBox13.TabIndex = 16;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Дополнительные сведения";
            // 
            // groupBoxRooms
            // 
            this.groupBoxRooms.Controls.Add(this.dataGridViewRooms);
            this.groupBoxRooms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRooms.Location = new System.Drawing.Point(3, 267);
            this.groupBoxRooms.Name = "groupBoxRooms";
            this.groupBoxRooms.Size = new System.Drawing.Size(486, 125);
            this.groupBoxRooms.TabIndex = 17;
            this.groupBoxRooms.TabStop = false;
            this.groupBoxRooms.Text = "Комнаты";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.comboBoxPremisesKind);
            this.panel3.Controls.Add(this.label28);
            this.panel3.Controls.Add(this.comboBoxPremisesType);
            this.panel3.Controls.Add(this.textBoxPremisesNumber);
            this.panel3.Controls.Add(this.label21);
            this.panel3.Controls.Add(this.numericUpDownFloor);
            this.panel3.Controls.Add(this.comboBoxStreet);
            this.panel3.Controls.Add(this.label20);
            this.panel3.Controls.Add(this.comboBoxHouse);
            this.panel3.Controls.Add(this.label19);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(480, 154);
            this.panel3.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label27);
            this.panel4.Controls.Add(this.numericUpDownNumBeds);
            this.panel4.Controls.Add(this.numericUpDownBalanceCost);
            this.panel4.Controls.Add(this.label22);
            this.panel4.Controls.Add(this.maskedTextBoxCadastralNum);
            this.panel4.Controls.Add(this.numericUpDownCadastralCost);
            this.panel4.Controls.Add(this.label23);
            this.panel4.Controls.Add(this.label24);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(489, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(480, 154);
            this.panel4.TabIndex = 1;
            // 
            // dataGridViewRestrictions
            // 
            this.dataGridViewRestrictions.AllowUserToAddRows = false;
            this.dataGridViewRestrictions.AllowUserToDeleteRows = false;
            this.dataGridViewRestrictions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRestrictions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_restriction_number,
            this.field_restriction_date,
            this.field_restriction_description,
            this.field_id_restriction_type});
            this.dataGridViewRestrictions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRestrictions.Name = "dataGridViewRestrictions";
            this.dataGridViewRestrictions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRestrictions.TabIndex = 0;
            this.dataGridViewRestrictions.AutoGenerateColumns = false;
            // 
            // field_restriction_number
            // 
            this.field_restriction_number.HeaderText = "Номер";
            this.field_restriction_number.Name = "number";
            this.field_restriction_number.ReadOnly = true;
            // 
            // field_restriction_date
            // 
            this.field_restriction_date.HeaderText = "Дата";
            this.field_restriction_date.Name = "date";
            this.field_restriction_date.ReadOnly = true;
            // 
            // field_restriction_description
            // 
            this.field_restriction_description.HeaderText = "Наименование";
            this.field_restriction_description.Name = "description";
            this.field_restriction_description.ReadOnly = true;
            //
            // field_id_restriction_type
            //
            field_id_restriction_type.HeaderText = "Тип права собственности";
            field_id_restriction_type.Name = "id_restriction_type";
            field_id_restriction_type.ReadOnly = true;
            field_id_restriction_type.MinimumWidth = 150;
            field_id_restriction_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // 
            // dataGridViewOwnerships
            // 
            this.dataGridViewOwnerships.AllowUserToAddRows = false;
            this.dataGridViewOwnerships.AllowUserToDeleteRows = false;
            this.dataGridViewOwnerships.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOwnerships.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_ownership_number,
            this.field_ownership_date,
            this.field_ownership_description,
            this.field_id_ownership_type});
            this.dataGridViewOwnerships.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewOwnerships.Name = "dataGridViewOwnerships";
            this.dataGridViewOwnerships.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewOwnerships.TabIndex = 0;
            this.dataGridViewOwnerships.AutoGenerateColumns = false;
            // 
            // field_ownership_number
            // 
            this.field_ownership_number.HeaderText = "Номер";
            this.field_ownership_number.Name = "number";
            this.field_ownership_number.ReadOnly = true;
            // 
            // field_ownership_date
            // 
            this.field_ownership_date.HeaderText = "Дата";
            this.field_ownership_date.Name = "date";
            this.field_ownership_date.ReadOnly = true;
            // 
            // field_ownership_description
            // 
            this.field_ownership_description.HeaderText = "Наименование";
            this.field_ownership_description.Name = "description";
            this.field_ownership_description.ReadOnly = true;
            //
            // field_id_ownership_type
            //
            field_id_ownership_type.HeaderText = "Тип ограничения";
            field_id_ownership_type.Name = "id_ownership_type";
            field_id_ownership_type.ReadOnly = true;
            field_id_ownership_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // dataGridViewRooms
            // 
            this.dataGridViewRooms.AllowUserToAddRows = false;
            this.dataGridViewRooms.AllowUserToDeleteRows = false;
            this.dataGridViewRooms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRooms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_sub_premises_num,
            this.field_sub_premises_total_area});
            this.dataGridViewRooms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRooms.Name = "dataGridViewRooms";
            this.dataGridViewRooms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRooms.TabIndex = 0;
            this.dataGridViewRooms.AutoGenerateColumns = false;
            // 
            // sub_premises_num
            // 
            this.field_sub_premises_num.HeaderText = "Номер";
            this.field_sub_premises_num.Name = "sub_premises_num";
            this.field_sub_premises_num.ReadOnly = true;
            // 
            // subremises_total_area
            // 
            this.field_sub_premises_total_area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_sub_premises_total_area.HeaderText = "Общая площадь";
            this.field_sub_premises_total_area.MinimumWidth = 100;
            this.field_sub_premises_total_area.Name = "subremises_total_area";
            this.field_sub_premises_total_area.DefaultCellStyle.Format = "#0.0## м²";
            this.field_sub_premises_total_area.ReadOnly = true;
            // 
            // numericUpDownFloor
            // 
            this.numericUpDownFloor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownFloor.Location = new System.Drawing.Point(157, 91);
            this.numericUpDownFloor.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownFloor.Name = "numericUpDownFloor";
            this.numericUpDownFloor.Size = new System.Drawing.Size(319, 20);
            this.numericUpDownFloor.TabIndex = 17;
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
            this.numericUpDownBalanceCost.Size = new System.Drawing.Size(318, 20);
            this.numericUpDownBalanceCost.TabIndex = 23;
            this.numericUpDownBalanceCost.ThousandsSeparator = true;
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
            this.numericUpDownCadastralCost.Size = new System.Drawing.Size(318, 20);
            this.numericUpDownCadastralCost.TabIndex = 20;
            this.numericUpDownCadastralCost.ThousandsSeparator = true;
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
            this.numericUpDownLivingArea.Size = new System.Drawing.Size(319, 20);
            this.numericUpDownLivingArea.TabIndex = 23;
            this.numericUpDownLivingArea.ThousandsSeparator = true;
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
            this.numericUpDownTotalArea.Size = new System.Drawing.Size(319, 20);
            this.numericUpDownTotalArea.TabIndex = 22;
            this.numericUpDownTotalArea.ThousandsSeparator = true;
            // 
            // numericUpDownNumBeds
            // 
            this.numericUpDownNumBeds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownNumBeds.Location = new System.Drawing.Point(159, 95);
            this.numericUpDownNumBeds.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownNumBeds.Name = "numericUpDownNumBeds";
            this.numericUpDownNumBeds.Size = new System.Drawing.Size(318, 20);
            this.numericUpDownNumBeds.TabIndex = 25;
            // 
            // checkBoxForOrphans
            // 
            this.checkBoxForOrphans.AutoSize = true;
            this.checkBoxForOrphans.Location = new System.Drawing.Point(19, 19);
            this.checkBoxForOrphans.Name = "checkBoxForOrphans";
            this.checkBoxForOrphans.Size = new System.Drawing.Size(173, 17);
            this.checkBoxForOrphans.TabIndex = 0;
            this.checkBoxForOrphans.Text = "Приобретено детям-сиротам";
            this.checkBoxForOrphans.UseVisualStyleBackColor = true;
            // 
            // checkBoxAcceptByExchange
            // 
            this.checkBoxAcceptByExchange.AutoSize = true;
            this.checkBoxAcceptByExchange.Location = new System.Drawing.Point(19, 48);
            this.checkBoxAcceptByExchange.Name = "checkBoxAcceptByExchange";
            this.checkBoxAcceptByExchange.Size = new System.Drawing.Size(113, 17);
            this.checkBoxAcceptByExchange.TabIndex = 1;
            this.checkBoxAcceptByExchange.Text = "Принято по мене";
            this.checkBoxAcceptByExchange.UseVisualStyleBackColor = true;
            // 
            // checkBoxAcceptByDonation
            // 
            this.checkBoxAcceptByDonation.AutoSize = true;
            this.checkBoxAcceptByDonation.Location = new System.Drawing.Point(222, 19);
            this.checkBoxAcceptByDonation.Name = "checkBoxAcceptByDonation";
            this.checkBoxAcceptByDonation.Size = new System.Drawing.Size(131, 17);
            this.checkBoxAcceptByDonation.TabIndex = 2;
            this.checkBoxAcceptByDonation.Text = "Принято по дарению";
            this.checkBoxAcceptByDonation.UseVisualStyleBackColor = true;
            // 
            // checkBoxAcceptByOther
            // 
            this.checkBoxAcceptByOther.AutoSize = true;
            this.checkBoxAcceptByOther.Location = new System.Drawing.Point(222, 48);
            this.checkBoxAcceptByOther.Name = "checkBoxAcceptByOther";
            this.checkBoxAcceptByOther.Size = new System.Drawing.Size(120, 17);
            this.checkBoxAcceptByOther.TabIndex = 3;
            this.checkBoxAcceptByOther.Text = "Прочее основание";
            this.checkBoxAcceptByOther.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 10);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(39, 13);
            this.label19.TabIndex = 11;
            this.label19.Text = "Улица";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 39);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(70, 13);
            this.label20.TabIndex = 13;
            this.label20.Text = "Номер дома";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 93);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(33, 13);
            this.label21.TabIndex = 16;
            this.label21.Text = "Этаж";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(16, 68);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(125, 13);
            this.label22.TabIndex = 22;
            this.label22.Text = "Балансовая стоимость";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(16, 10);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(110, 13);
            this.label23.TabIndex = 18;
            this.label23.Text = "Кадастровый номер";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(16, 39);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(130, 13);
            this.label24.TabIndex = 19;
            this.label24.Text = "Кадастровая стоимость";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(16, 49);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(42, 13);
            this.label25.TabIndex = 21;
            this.label25.Text = "Жилая";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(16, 20);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(42, 13);
            this.label26.TabIndex = 20;
            this.label26.Text = "Общая";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(16, 97);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(127, 13);
            this.label27.TabIndex = 24;
            this.label27.Text = "Количество койко-мест";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(10, 122);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(88, 13);
            this.label28.TabIndex = 21;
            this.label28.Text = "Вид помещения";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(10, 97);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(53, 13);
            this.label29.TabIndex = 22;
            this.label29.Text = "Комнаты";
            // 
            // textBoxHouse
            // 
            this.comboBoxHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxHouse.Location = new System.Drawing.Point(157, 36);
            this.comboBoxHouse.Name = "textBoxHouse";
            this.comboBoxHouse.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxHouse.Size = new System.Drawing.Size(319, 20);
            this.comboBoxHouse.TabIndex = 14;
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStreet.Location = new System.Drawing.Point(157, 7);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.DropDownStyle = ComboBoxStyle.DropDown;
            this.comboBoxStreet.Size = new System.Drawing.Size(319, 20);
            this.comboBoxStreet.TabIndex = 15;
            // 
            // textBoxPremisesNumber
            // 
            this.textBoxPremisesNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPremisesNumber.Location = new System.Drawing.Point(157, 65);
            this.textBoxPremisesNumber.Name = "textBoxPremisesNumber";
            this.textBoxPremisesNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxPremisesNumber.TabIndex = 19;
            this.textBoxPremisesNumber.MaxLength = 5;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right | AnchorStyles.Bottom)));
            this.textBoxDescription.Location = new System.Drawing.Point(9, 18);
            this.textBoxDescription.MaxLength = 255;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(468, 45);
            this.textBoxDescription.TabIndex = 1;
            // 
            // textBoxSubPremisesNumber
            // 
            this.textBoxSubPremisesNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSubPremisesNumber.Location = new System.Drawing.Point(157, 94);
            this.textBoxSubPremisesNumber.Name = "textBoxSubPremisesNumber";
            this.textBoxSubPremisesNumber.ReadOnly = true;
            this.textBoxSubPremisesNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxSubPremisesNumber.TabIndex = 21;
            // 
            // comboBoxPremisesType
            // 
            this.comboBoxPremisesType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPremisesType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxPremisesType.FormattingEnabled = true;
            this.comboBoxPremisesType.Items.AddRange(new object[] {
            "Номер квартиры"});
            this.comboBoxPremisesType.Location = new System.Drawing.Point(9, 65);
            this.comboBoxPremisesType.Name = "comboBoxPremisesType";
            this.comboBoxPremisesType.Size = new System.Drawing.Size(143, 21);
            this.comboBoxPremisesType.TabIndex = 20;
            this.comboBoxPremisesType.SelectedValueChanged += new EventHandler(comboBoxPremisesType_SelectedValueChanged);
            // 
            // comboBoxPremisesKind
            // 
            this.comboBoxPremisesKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPremisesKind.FormattingEnabled = true;
            this.comboBoxPremisesKind.Location = new System.Drawing.Point(157, 119);
            this.comboBoxPremisesKind.Name = "comboBoxPremisesKind";
            this.comboBoxPremisesKind.Size = new System.Drawing.Size(319, 21);
            this.comboBoxPremisesKind.TabIndex = 22;
            this.comboBoxPremisesKind.DropDownStyle = ComboBoxStyle.DropDownList;
            // 
            // maskedTextBoxCadastralNum
            // 
            this.maskedTextBoxCadastralNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.maskedTextBoxCadastralNum.Location = new System.Drawing.Point(159, 7);
            this.maskedTextBoxCadastralNum.Name = "maskedTextBoxCadastralNum";
            this.maskedTextBoxCadastralNum.Size = new System.Drawing.Size(318, 20);
            this.maskedTextBoxCadastralNum.TabIndex = 21;

            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBoxRooms.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRooms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumBeds)).EndInit();
        }

        void comboBoxPremisesType_SelectedValueChanged(object sender, EventArgs e)
        {
            menuCallback.HousingRefBooksStateUpdate();
        }
    }
}
