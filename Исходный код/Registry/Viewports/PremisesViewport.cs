using System;
using System.Collections.Generic;
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
using Registry.Reporting;
using Registry.SearchForms;
using Security;
using VIBlend.Utilities;
using VIBlend.WinForms.Controls;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class PremisesViewport : Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel5;
        private GroupBox groupBox8;
        private GroupBox groupBox9;
        private GroupBox groupBox10;
        private GroupBox groupBox11;
        private GroupBox groupBox13;
        private GroupBox groupBoxRooms;
        private Panel panel3 = new Panel();
        private Panel panel4 = new Panel();
        private DataGridView dataGridViewRestrictions;
        private DataGridView dataGridViewOwnerships;
        private DataGridView dataGridViewRooms;
        private NumericUpDown numericUpDownFloor;
        private NumericUpDown numericUpDownBalanceCost;
        private NumericUpDown numericUpDownCadastralCost;
        private NumericUpDown numericUpDownLivingArea;
        private NumericUpDown numericUpDownTotalArea;
        private NumericUpDown numericUpDownNumBeds;
        private Label label19;
        private Label label20;
        private Label label21;
        private Label label22;
        private Label label23;
        private Label label24;
        private Label label25;
        private Label label26;
        private Label label27;
        private Label label28;
        private Label label29;
        private Label label38;
        private Label label39;
        private ComboBox comboBoxHouse;
        private ComboBox comboBoxStreet;
        private TextBox textBoxPremisesNumber;
        private TextBox textBoxDescription;
        private TextBox textBoxSubPremisesNumber;
        private TextBox textBoxCadastralNum;
        private ComboBox comboBoxPremisesType;
        private ComboBox comboBoxPremisesKind;
        private ComboBox comboBoxCurrentFundType;
        private ComboBox comboBoxState;
        private Label label1;
        private NumericUpDown numericUpDownNumRooms;
        private NumericUpDown numericUpDownMunicipalArea;
        private Label label2;
        private NumericUpDown numericUpDownHeight;
        private Label label3;
        private DateTimePicker dateTimePickerRegDate;
        private Label label4;
        private CheckBox checkBoxIsMemorial;
        private Panel panel1;
        private vButton vButtonRestrictionAdd;
        private vButton vButtonRestrictionEdit;
        private vButton vButtonRestrictionDelete;
        private Panel panel2;
        private vButton vButtonOwnershipEdit;
        private vButton vButtonOwnershipDelete;
        private vButton vButtonOwnershipAdd;
        private Panel panel5;
        private vButton vButtonRoomEdit;
        private vButton vButtonRoomDelete;
        private vButton vButtonRoomAdd;
        private DataGridViewTextBoxColumn id_restriction;
        private DataGridViewTextBoxColumn restriction_number;
        private DataGridViewTextBoxColumn restriction_date;
        private DataGridViewTextBoxColumn restriction_description;
        private DataGridViewComboBoxColumn id_restriction_type;
        private DataGridViewTextBoxColumn restriction_relation;
        private DataGridViewTextBoxColumn id_ownership_right;
        private DataGridViewTextBoxColumn ownership_number;
        private DataGridViewTextBoxColumn ownership_date;
        private DataGridViewTextBoxColumn ownership_description;
        private DataGridViewComboBoxColumn id_ownership_type;
        private DataGridViewTextBoxColumn ownership_relation;
        private TextBox textBoxAccount;
        private Label label5;
        private DateTimePicker dateTimePickerStateDate;
        private Label label6;
        private DataGridViewTextBoxColumn sub_premises_num;
        private DataGridViewTextBoxColumn sub_premises_total_area;
        private DataGridViewComboBoxColumn sub_premises_id_state;
        private DataGridViewTextBoxColumn current_fund;
        #endregion Components

        #region Models
        private PremisesDataModel premises;
        private BuildingsDataModel buildings;
        private KladrStreetsDataModel kladr;
        private PremisesTypesDataModel premises_types;
        private PremisesKindsDataModel premises_kinds;
        private SubPremisesDataModel sub_premises;
        private RestrictionsDataModel restrictions;
        private RestrictionTypesDataModel restrictionTypes;
        private RestrictionsPremisesAssocDataModel restrictionPremisesAssoc;
        private RestrictionsBuildingsAssocDataModel restrictionBuildingsAssoc;
        private OwnershipsRightsDataModel ownershipRights;
        private OwnershipRightTypesDataModel ownershipRightTypes;
        private OwnershipPremisesAssocDataModel ownershipPremisesAssoc;
        private OwnershipBuildingsAssocDataModel ownershipBuildingsAssoc;
        private FundTypesDataModel fundTypes;
        private ObjectStatesDataModel object_states;
        private CalcDataModelPremisesCurrentFunds premisesCurrentFund;
        private CalcDataModelPremiseSubPremisesSumArea premiseSubPremisesSumArea;
        private CalcDataModelSubPremisesCurrentFunds subPremisesCurrentFund;
        #endregion Models

        #region Views
        private BindingSource v_premises;
        private BindingSource v_premisesCurrentFund;
        private BindingSource v_buildings;
        private BindingSource v_kladr;
        private BindingSource v_premises_types;
        private BindingSource v_premises_kinds;
        private BindingSource v_sub_premises;
        private BindingSource v_restrictions;
        private BindingSource v_restrictonTypes;
        private BindingSource v_restrictionPremisesAssoc;
        private BindingSource v_restrictionBuildingsAssoc;
        private BindingSource v_ownershipRights;
        private BindingSource v_ownershipRightTypes;
        private BindingSource v_ownershipPremisesAssoc;
        private BindingSource v_ownershipBuildingsAssoc;
        private BindingSource v_fundType;
        private BindingSource v_object_states;
        private BindingSource v_sub_premises_object_states;
        private BindingSource v_premisesSubPremisesSumArea;
        private BindingSource v_subPremisesCurrentFund;
        #endregion Views

        //Forms
        private SearchForm spExtendedSearchForm;
        private SearchForm spSimpleSearchForm;

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable;
        private bool is_first_visibility = true;

        private PremisesViewport()
            : this(null)
        {
        }

        public PremisesViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
        }

        public PremisesViewport(PremisesViewport premsiesListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = premsiesListViewport.DynamicFilter;
            StaticFilter = premsiesListViewport.StaticFilter;
            ParentRow = premsiesListViewport.ParentRow;
            ParentType = premsiesListViewport.ParentType;
        }

        private void RestrictionsFilterRebuild()
        {
            var restrictionsFilter = "id_restriction IN (0";
            for (var i = 0; i < v_restrictionPremisesAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)v_restrictionPremisesAssoc[i])["id_restriction"] + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(',');
            restrictionsFilter += ")";
            if (v_premises.Position > -1 && v_restrictionBuildingsAssoc != null && ((DataRowView)v_premises[v_premises.Position])["id_building"] != DBNull.Value)
            {
                v_restrictionBuildingsAssoc.Filter = "id_building = " + ((DataRowView)v_premises[v_premises.Position])["id_building"];
                restrictionsFilter += " OR id_restriction IN (0";
                for (var i = 0; i < v_restrictionBuildingsAssoc.Count; i++)
                    restrictionsFilter += ((DataRowView)v_restrictionBuildingsAssoc[i])["id_restriction"] + ",";
                restrictionsFilter = restrictionsFilter.TrimEnd(',');
                restrictionsFilter += ")";
            }
            v_restrictions.Filter = restrictionsFilter;
            if (dataGridViewRestrictions.Columns.Contains("id_restriction"))
                dataGridViewRestrictions.Columns["id_restriction"].Visible = false;
            RedrawRestrictionDataGridRows();
        }

        private void OwnershipsFilterRebuild()
        {
            var ownershipFilter = "id_ownership_right IN (0";
            for (var i = 0; i < v_ownershipPremisesAssoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownershipPremisesAssoc[i])["id_ownership_right"] + ",";
            ownershipFilter = ownershipFilter.TrimEnd(',');
            ownershipFilter += ")";
            if (v_premises.Position > -1 && v_ownershipBuildingsAssoc != null && ((DataRowView)v_premises[v_premises.Position])["id_building"] != DBNull.Value)
            {
                v_ownershipBuildingsAssoc.Filter = "id_building = " + ((DataRowView)v_premises[v_premises.Position])["id_building"];
                ownershipFilter += " OR id_ownership_right IN (0";
                for (var i = 0; i < v_ownershipBuildingsAssoc.Count; i++)
                    ownershipFilter += ((DataRowView)v_ownershipBuildingsAssoc[i])["id_ownership_right"] + ",";
                ownershipFilter = ownershipFilter.TrimEnd(',');
                ownershipFilter += ")";
            }
            v_ownershipRights.Filter = ownershipFilter;
            
            if (dataGridViewOwnerships.Columns.Contains("id_ownership_right"))
                dataGridViewOwnerships.Columns["id_ownership_right"].Visible = false;
            RedrawOwnershipDataGridRows();
        }

        private void FiltersRebuild()
        {
            if (v_premisesCurrentFund != null)
            {
                var position = -1;
                if ((v_premises.Position != -1) && !(((DataRowView)v_premises[v_premises.Position])["id_premises"] is DBNull))
                    position = 
                        v_premisesCurrentFund.Find("id_premises", ((DataRowView)v_premises[v_premises.Position])["id_premises"]);
                if (position != -1)
                    comboBoxCurrentFundType.SelectedValue = ((DataRowView)v_premisesCurrentFund[position])["id_fund_type"];
                else
                    comboBoxCurrentFundType.SelectedValue = DBNull.Value;
                ShowOrHideCurrentFund();
            }
            if (v_premisesSubPremisesSumArea != null)
            {
                var position = -1;
                if ((v_premises.Position != -1) && !(((DataRowView)v_premises[v_premises.Position])["id_premises"] is DBNull))
                    position = v_premisesSubPremisesSumArea.Find("id_premises", ((DataRowView)v_premises[v_premises.Position])["id_premises"]);
                if (position != -1)
                {
                    var value = Convert.ToDecimal((double)((DataRowView)v_premisesSubPremisesSumArea[position])["sum_area"]);
                    numericUpDownMunicipalArea.Minimum = value;
                    numericUpDownMunicipalArea.Maximum = value;
                    numericUpDownMunicipalArea.Value = value;
                }
                else
                {
                    numericUpDownMunicipalArea.Minimum = 0;
                    numericUpDownMunicipalArea.Maximum = 0;
                    numericUpDownMunicipalArea.Value = 0;
                }
            }
        }

        private void RedrawRestrictionDataGridRows()
        {
            for (var i = 0; i < dataGridViewRestrictions.Rows.Count; i++)
                if (v_restrictionBuildingsAssoc.Find("id_restriction", dataGridViewRestrictions.Rows[i].Cells["id_restriction"].Value) != -1)
                {
                    dataGridViewRestrictions.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    dataGridViewRestrictions.Rows[i].DefaultCellStyle.SelectionBackColor = Color.Green;
                    dataGridViewRestrictions.Rows[i].Cells["restriction_relation"].Value = "Здание";
                }
                else
                {
                    dataGridViewRestrictions.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dataGridViewRestrictions.Rows[i].DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    dataGridViewRestrictions.Rows[i].Cells["restriction_relation"].Value = "Помещение";
                }
        }

        private void RedrawOwnershipDataGridRows()
        {
            for (var i = 0; i < dataGridViewOwnerships.Rows.Count; i++)
                if (v_ownershipBuildingsAssoc.Find("id_ownership_right", dataGridViewOwnerships.Rows[i].Cells["id_ownership_right"].Value) != -1)
                {
                    dataGridViewOwnerships.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    dataGridViewOwnerships.Rows[i].DefaultCellStyle.SelectionBackColor = Color.Green;
                    dataGridViewOwnerships.Rows[i].Cells["ownership_relation"].Value = "Здание";
                }
                else
                {
                    dataGridViewOwnerships.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dataGridViewOwnerships.Rows[i].DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    dataGridViewOwnerships.Rows[i].Cells["ownership_relation"].Value = "Помещение";
                }
        }

        private void RedrawSubPremiseDataGridRows()
        {
            if (v_sub_premises == null)
                return;
            if (v_sub_premises.Count != dataGridViewRooms.Rows.Count)
                return;
            for (var i = 0; i < v_sub_premises.Count; i++)
            {
                var id_sub_premises = (int)((DataRowView)v_sub_premises[i])["id_sub_premises"];
                var id = v_subPremisesCurrentFund.Find("id_sub_premises", id_sub_premises);
                if (id == -1)
                    continue;
                var id_fund_type = (int)((DataRowView)v_subPremisesCurrentFund[id])["id_fund_type"];
                var fundType = ((DataRowView)v_fundType[v_fundType.Find("id_fund_type", id_fund_type)])["fund_type"].ToString();
                if ((new[] {1, 4, 5, 9}).Contains((int)((DataRowView)v_sub_premises[i])["id_state"]))
                    dataGridViewRooms.Rows[i].Cells["current_fund"].Value = fundType;
                else
                    dataGridViewRooms.Rows[i].Cells["current_fund"].Value = "";
            }
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                {
                    Text = string.Format(CultureInfo.InvariantCulture, "Новое помещение здания №{0}", ParentRow["id_building"]);
                }
                else
                    Text = "Новое помещение";
            }
            else
                if (v_premises.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                        Text = string.Format(CultureInfo.InvariantCulture, "Помещение №{0} здания №{1}",
                            ((DataRowView)v_premises[v_premises.Position])["id_premises"], ParentRow["id_building"]);
                    else
                        Text = string.Format(CultureInfo.InvariantCulture, "Помещение №{0}", ((DataRowView)v_premises[v_premises.Position])["id_premises"]);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                        Text = string.Format(CultureInfo.InvariantCulture, "Помещения в здании №{0} отсутствуют", ParentRow["id_building"]);
                    else
                        Text = "Помещения отсутствуют";
                }
        }

        private void ShowOrHideCurrentFund()
        {
            if (comboBoxCurrentFundType.SelectedValue != null && v_premises.Position != -1 &&
                ((DataRowView)v_premises[v_premises.Position])["id_state"] != DBNull.Value &&
                (new[] { 1, 4, 5, 9 }).Contains((int)((DataRowView)v_premises[v_premises.Position])["id_state"]))
            {
                label38.Visible = true;
                comboBoxCurrentFundType.Visible = true;
                checkBoxIsMemorial.Location = new Point(19, 209);
                tableLayoutPanel3.RowStyles[0].Height = 269F;
            }
            else
            {
                label38.Visible = false;
                comboBoxCurrentFundType.Visible = false;
                checkBoxIsMemorial.Location = new Point(19, 183);
                tableLayoutPanel3.RowStyles[0].Height = 240F;
            }
        }

        private void SelectCurrentBuilding()
        {
            if ((comboBoxHouse.DataSource != null) && (comboBoxStreet.DataSource != null))
            {
                int? id_building = null;
                if ((v_premises.Position != -1) && (((DataRowView)v_premises[v_premises.Position])["id_building"] != DBNull.Value))
                    id_building = Convert.ToInt32(((DataRowView)v_premises[v_premises.Position])["id_building"], CultureInfo.InvariantCulture);
                else 
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                    id_building = Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture);
                string id_street = null;
                if (id_building != null)
                {
                    var building_row = buildings.Select().Rows.Find(id_building);
                    if (building_row != null)
                        id_street = building_row["id_street"].ToString();
                }
                v_kladr.Filter = "";
                if (id_street != null)
                    comboBoxStreet.SelectedValue = id_street;
                else
                    comboBoxStreet.SelectedValue = DBNull.Value;
                if (id_building != null)
                    comboBoxHouse.SelectedValue = id_building;
                else
                    comboBoxHouse.SelectedValue = DBNull.Value;
                CheckViewportModifications();
            }
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = (v_premises.Position >= 0) ? (DataRowView)v_premises[v_premises.Position] : null;
            if ((v_premises.Position >= 0) && (row["state_date"] != DBNull.Value))
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
            textBoxCadastralNum.DataBindings.Clear();
            textBoxCadastralNum.DataBindings.Add("Text", v_premises, "cadastral_num", true, DataSourceUpdateMode.Never, "");
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_premises, "description", true, DataSourceUpdateMode.Never, "");
            textBoxAccount.DataBindings.Clear();
            textBoxAccount.DataBindings.Add("Text", v_premises, "account", true, DataSourceUpdateMode.Never, "");
            numericUpDownCadastralCost.DataBindings.Clear();
            numericUpDownCadastralCost.DataBindings.Add("Value", v_premises, "cadastral_cost", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceCost.DataBindings.Clear();
            numericUpDownBalanceCost.DataBindings.Add("Value", v_premises, "balance_cost", true, DataSourceUpdateMode.Never, 0);
            numericUpDownNumRooms.DataBindings.Clear();
            numericUpDownNumRooms.DataBindings.Add("Value", v_premises, "num_rooms", true, DataSourceUpdateMode.Never, 0);
            numericUpDownNumBeds.DataBindings.Clear();
            numericUpDownNumBeds.DataBindings.Add("Value", v_premises, "num_beds", true, DataSourceUpdateMode.Never, 0);
            numericUpDownTotalArea.DataBindings.Clear();
            numericUpDownTotalArea.DataBindings.Add("Value", v_premises, "total_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownLivingArea.DataBindings.Clear();
            numericUpDownLivingArea.DataBindings.Add("Value", v_premises, "living_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownHeight.DataBindings.Clear();
            numericUpDownHeight.DataBindings.Add("Value", v_premises, "height", true, DataSourceUpdateMode.Never, 0);

            comboBoxCurrentFundType.DataSource = v_fundType;
            comboBoxCurrentFundType.ValueMember = "id_fund_type";
            comboBoxCurrentFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = v_object_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";
            comboBoxState.DataBindings.Clear();
            comboBoxState.DataBindings.Add("SelectedValue", v_premises, "id_state", true, DataSourceUpdateMode.Never, DBNull.Value);

            checkBoxIsMemorial.DataBindings.Clear();
            checkBoxIsMemorial.DataBindings.Add("Checked", v_premises, "is_memorial", true, DataSourceUpdateMode.Never, true);

            dateTimePickerRegDate.DataBindings.Clear();
            dateTimePickerRegDate.DataBindings.Add("Value", v_premises, "reg_date", true, DataSourceUpdateMode.Never, null);

            dateTimePickerStateDate.DataBindings.Clear();
            dateTimePickerStateDate.DataBindings.Add("Value", v_premises, "state_date", true, DataSourceUpdateMode.Never, null);

            dataGridViewRestrictions.DataSource = v_restrictions;
            id_restriction.DataPropertyName = "id_restriction";
            id_restriction_type.DataSource = v_restrictonTypes;
            id_restriction_type.DataPropertyName = "id_restriction_type";
            id_restriction_type.ValueMember = "id_restriction_type";
            id_restriction_type.DisplayMember = "restriction_type";
            restriction_number.DataPropertyName = "number";
            restriction_date.DataPropertyName = "date";
            restriction_description.DataPropertyName = "description";

            dataGridViewOwnerships.DataSource = v_ownershipRights;
            id_ownership_right.DataPropertyName = "id_ownership_right";
            id_ownership_type.DataSource = v_ownershipRightTypes;
            id_ownership_type.DataPropertyName = "id_ownership_right_type";
            id_ownership_type.ValueMember = "id_ownership_right_type";
            id_ownership_type.DisplayMember = "ownership_right_type";
            ownership_number.DataPropertyName = "number";
            ownership_date.DataPropertyName = "date";
            ownership_description.DataPropertyName = "description";

            dataGridViewRooms.DataSource = v_sub_premises;
            sub_premises_num.DataPropertyName = "sub_premises_num";
            sub_premises_total_area.DataPropertyName = "total_area";
            sub_premises_id_state.DataSource = v_sub_premises_object_states;
            sub_premises_id_state.DataPropertyName = "id_state";
            sub_premises_id_state.ValueMember = "id_state";
            sub_premises_id_state.DisplayMember = "state_female";

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
                            var result = MessageBox.Show("Сохранить изменения о помещениях в базу данных?", "Внимание",
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
                            var result = MessageBox.Show("Сохранить изменения о помещениях в базу данных?", "Внимание",
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
                            var result = MessageBox.Show("Сохранить изменения о помещениях в базу данных?", "Внимание",
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

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if (!ContainsFocus)
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
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }

        public void LocatePremisesBy(int id)
        {
            var Position = v_premises.Find("id_premises", id);
            is_editable = false;
            if (Position > 0)
                v_premises.Position = Position;
            is_editable = true;
        }

        private bool ValidatePremise(Premise premise)
        {
            if (premise.IdBuilding == null)
            {
                MessageBox.Show("Необходимо выбрать здание","Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxHouse.Focus();
                return false;
            }
            if (premise.PremisesNum == null || string.IsNullOrEmpty(premise.PremisesNum.Trim()))
            {
                MessageBox.Show("Необходимо указать номер помещения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNumber.Focus();
                return false;
            }
            if (!Regex.IsMatch(premise.PremisesNum, @"^[0-9]+[а-я]{0,1}([/][0-9]+[а-я]{0,1})?([,][0-9]+[а-я]{0,1}([/][0-9]+[а-я]{0,1})?)*$"))
            {
                MessageBox.Show("Некорректно задан номер помещения. Можно использовать только цифры и не более одной строчной буквы кирилицы, а также знак дроби /. Для объединенных квартир номера должны быть перечислены через запятую. Например: \"1а,2а,3б/4\"", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNumber.Focus();
                return false;
            }
            if (premise.IdState == null)
            {
                MessageBox.Show("Необходимо выбрать текущее состояние помещения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxState.Focus();
                return false;
            }
            // Проверяем права на модификацию муниципального или не муниципального здания
            var premiseFromView = PremiseFromView();
            if (premiseFromView.IdPremises != null && DataModelHelper.HasMunicipal(premiseFromView.IdPremises.Value, EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("Вы не можете изменить информацию по данному помещению, т.к. оно является муниципальным или содержит в себе муниципальные комнаты",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (premiseFromView.IdPremises != null && DataModelHelper.HasNotMunicipal(premiseFromView.IdPremises.Value, EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("Вы не можете изменить информацию по данному помещения, т.к. оно является немуниципальным или содержит в себе немуниципальные комнаты",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new[] { 4, 5, 9 }.Contains(premise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу муниципальных жилых помещений", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new[] { 1, 3, 6, 7, 8 }.Contains(premise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу немуниципальных жилых помещений", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            // Проверяем дубликаты квартир
            if ((premise.PremisesNum != premiseFromView.PremisesNum) || (premise.IdBuilding != premiseFromView.IdBuilding))
                if (DataModelHelper.PremisesDuplicateCount(premise) != 0 &&
                    MessageBox.Show("В указанном доме уже есть квартира с таким номером. Все равно продолжить сохранение?", "Внимание", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
            return true;
        }

        private Premise PremiseFromView()
        {
            var premise = new Premise();
            var row = (DataRowView)v_premises[v_premises.Position];
            premise.IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises");
            premise.IdBuilding = ViewportHelper.ValueOrNull<int>(row, "id_building");
            premise.IdState = ViewportHelper.ValueOrNull<int>(row, "id_state");
            premise.PremisesNum = ViewportHelper.ValueOrNull(row, "premises_num");
            premise.LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area");
            premise.TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area");
            premise.Height = ViewportHelper.ValueOrNull<double>(row, "height");
            premise.NumRooms = ViewportHelper.ValueOrNull<short>(row, "num_rooms");
            premise.NumBeds = ViewportHelper.ValueOrNull<short>(row, "num_beds");
            premise.IdPremisesType = ViewportHelper.ValueOrNull<int>(row, "id_premises_type");
            premise.IdPremisesKind = ViewportHelper.ValueOrNull<int>(row, "id_premises_kind");
            premise.Floor = ViewportHelper.ValueOrNull<short>(row, "floor");
            premise.CadastralNum = ViewportHelper.ValueOrNull(row, "cadastral_num");
            premise.CadastralCost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost");
            premise.BalanceCost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost");
            premise.Description = ViewportHelper.ValueOrNull(row, "description");
            premise.IsMemorial = ViewportHelper.ValueOrNull<bool>(row, "is_memorial");
            premise.Account = ViewportHelper.ValueOrNull(row, "account");
            premise.RegDate = ViewportHelper.ValueOrNull<DateTime>(row, "reg_date");
            premise.StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date");
            return premise;
        }

        private Premise PremiseFromViewport()
        {
            var premise = new Premise();
            if (v_premises.Position == -1)
                premise.IdPremises = null;
            else
                premise.IdPremises = ViewportHelper.ValueOrNull<int>((DataRowView)v_premises[v_premises.Position],"id_premises");
            premise.IdBuilding = ViewportHelper.ValueOrNull<int>(comboBoxHouse);
            premise.IdState = ViewportHelper.ValueOrNull<int>(comboBoxState);
            premise.PremisesNum = ViewportHelper.ValueOrNull(textBoxPremisesNumber);
            premise.TotalArea = Convert.ToDouble(numericUpDownTotalArea.Value);
            premise.LivingArea = Convert.ToDouble(numericUpDownLivingArea.Value);
            premise.Height = Convert.ToDouble(numericUpDownHeight.Value);
            premise.NumRooms = Convert.ToInt16(numericUpDownNumRooms.Value);
            premise.NumBeds = Convert.ToInt16(numericUpDownNumBeds.Value);
            premise.IdPremisesType = ViewportHelper.ValueOrNull<int>(comboBoxPremisesType);
            premise.IdPremisesKind = ViewportHelper.ValueOrNull<int>(comboBoxPremisesKind);
            // Костыль, возникший после того, как спрятал Вид помещения. Удалять вид помещения не стал, т.к. мало ли что у пользователей на уме
            if (premise.IdPremisesKind == null)
                premise.IdPremisesKind = 1;
            // Конец костыля
            premise.Floor = Convert.ToInt16(numericUpDownFloor.Value);
            premise.CadastralNum = ViewportHelper.ValueOrNull(textBoxCadastralNum);
            premise.CadastralCost = numericUpDownCadastralCost.Value;
            premise.BalanceCost = numericUpDownBalanceCost.Value;
            premise.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            premise.IsMemorial = checkBoxIsMemorial.Checked;
            premise.Account = ViewportHelper.ValueOrNull(textBoxAccount);
            premise.RegDate = ViewportHelper.ValueOrNull(dateTimePickerRegDate);
            premise.StateDate = ViewportHelper.ValueOrNull(dateTimePickerStateDate);
            return premise;
        }

        private void ViewportFromPremise(Premise premise)
        {
            if (premise.IdBuilding != null)
            {
                var building_row = buildings.Select().Rows.Find(premise.IdBuilding);
                if (building_row != null)
                {
                    v_kladr.Filter = "";
                    comboBoxStreet.SelectedValue = building_row["id_street"];
                    comboBoxHouse.SelectedValue = ViewportHelper.ValueOrDBNull(premise.IdBuilding);
                }
            }
            comboBoxState.SelectedValue = ViewportHelper.ValueOrDBNull(premise.IdState);
            comboBoxPremisesType.SelectedValue = ViewportHelper.ValueOrDBNull(premise.IdPremisesType);
            comboBoxPremisesKind.SelectedValue = ViewportHelper.ValueOrDBNull(premise.IdPremisesKind);
            numericUpDownFloor.Value = ViewportHelper.ValueOrDefault(premise.Floor);
            numericUpDownCadastralCost.Value = ViewportHelper.ValueOrDefault(premise.CadastralCost);
            numericUpDownBalanceCost.Value = ViewportHelper.ValueOrDefault(premise.BalanceCost);
            numericUpDownNumBeds.Value = ViewportHelper.ValueOrDefault(premise.NumBeds);
            numericUpDownNumRooms.Value = ViewportHelper.ValueOrDefault(premise.NumRooms);
            numericUpDownHeight.Value = (decimal)ViewportHelper.ValueOrDefault(premise.Height);
            numericUpDownLivingArea.Value = (decimal)ViewportHelper.ValueOrDefault(premise.LivingArea);
            numericUpDownTotalArea.Value = (decimal)ViewportHelper.ValueOrDefault(premise.TotalArea);
            dateTimePickerRegDate.Value = ViewportHelper.ValueOrDefault(premise.RegDate);
            dateTimePickerStateDate.Value = ViewportHelper.ValueOrDefault(premise.StateDate);
            checkBoxIsMemorial.Checked = ViewportHelper.ValueOrDefault(premise.IsMemorial);
            textBoxPremisesNumber.Text = premise.PremisesNum;
            textBoxCadastralNum.Text = premise.CadastralNum;
            textBoxDescription.Text = premise.Description;
            textBoxAccount.Text = premise.Account;
        }

        private static void FillRowFromPremise(Premise premise, DataRowView row)
        {
            row.BeginEdit();
            row["id_premises"] = ViewportHelper.ValueOrDBNull(premise.IdPremises);
            row["id_building"] = ViewportHelper.ValueOrDBNull(premise.IdBuilding);
            row["id_state"] = ViewportHelper.ValueOrDBNull(premise.IdState);
            row["premises_num"] = ViewportHelper.ValueOrDBNull(premise.PremisesNum);
            row["total_area"] = ViewportHelper.ValueOrDBNull(premise.TotalArea);
            row["living_area"] = ViewportHelper.ValueOrDBNull(premise.LivingArea);
            row["height"] = ViewportHelper.ValueOrDBNull(premise.Height);
            row["num_rooms"] = ViewportHelper.ValueOrDBNull(premise.NumRooms);
            row["num_beds"] = ViewportHelper.ValueOrDBNull(premise.NumBeds);
            row["id_premises_type"] = ViewportHelper.ValueOrDBNull(premise.IdPremisesType);
            row["id_premises_kind"] = ViewportHelper.ValueOrDBNull(premise.IdPremisesKind);
            row["floor"] = ViewportHelper.ValueOrDBNull(premise.Floor);
            row["cadastral_num"] = ViewportHelper.ValueOrDBNull(premise.CadastralNum);
            row["cadastral_cost"] = ViewportHelper.ValueOrDBNull(premise.CadastralCost);
            row["balance_cost"] = ViewportHelper.ValueOrDBNull(premise.BalanceCost);
            row["description"] = ViewportHelper.ValueOrDBNull(premise.Description);
            row["reg_date"] = ViewportHelper.ValueOrDBNull(premise.RegDate);
            row["is_memorial"] = ViewportHelper.ValueOrDBNull(premise.IsMemorial);
            row["account"] = ViewportHelper.ValueOrDBNull(premise.Account);
            row["state_date"] = ViewportHelper.ValueOrDBNull(premise.StateDate);
            row.EndEdit();
        }

        public override int GetRecordCount()
        {
            return v_premises.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_premises.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_premises.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_premises.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_premises.MovePrevious();
            is_editable = true;
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridViewOwnerships.AutoGenerateColumns = false;
            dataGridViewRestrictions.AutoGenerateColumns = false;
            dataGridViewRooms.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;

            premises = PremisesDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();
            buildings = BuildingsDataModel.GetInstance();
            premises_types = PremisesTypesDataModel.GetInstance();
            premises_kinds = PremisesKindsDataModel.GetInstance();
            sub_premises = SubPremisesDataModel.GetInstance();
            restrictions = RestrictionsDataModel.GetInstance();
            restrictionTypes = RestrictionTypesDataModel.GetInstance();
            restrictionPremisesAssoc = RestrictionsPremisesAssocDataModel.GetInstance();
            restrictionBuildingsAssoc = RestrictionsBuildingsAssocDataModel.GetInstance();
            ownershipRights = OwnershipsRightsDataModel.GetInstance();
            ownershipRightTypes = OwnershipRightTypesDataModel.GetInstance();
            ownershipPremisesAssoc = OwnershipPremisesAssocDataModel.GetInstance();
            ownershipBuildingsAssoc = OwnershipBuildingsAssocDataModel.GetInstance();
            fundTypes = FundTypesDataModel.GetInstance();
            object_states = ObjectStatesDataModel.GetInstance();

            // Вычисляемые модели
            premisesCurrentFund = CalcDataModelPremisesCurrentFunds.GetInstance();
            premiseSubPremisesSumArea = CalcDataModelPremiseSubPremisesSumArea.GetInstance();
            subPremisesCurrentFund = CalcDataModelSubPremisesCurrentFunds.GetInstance();

            // Ожидаем дозагрузки, если это необходмо
            premises.Select();
            kladr.Select();
            buildings.Select();
            premises_types.Select();
            premises_kinds.Select();
            sub_premises.Select();
            restrictions.Select();
            restrictionTypes.Select();
            restrictionPremisesAssoc.Select();
            restrictionBuildingsAssoc.Select();
            ownershipRights.Select();
            ownershipRightTypes.Select();
            ownershipPremisesAssoc.Select();
            ownershipBuildingsAssoc.Select();
            fundTypes.Select();
            object_states.Select();

            var ds = DataSetManager.DataSet;

            v_kladr = new BindingSource();
            v_kladr.DataMember = "kladr";
            v_kladr.DataSource = ds;

            v_buildings = new BindingSource();
            v_buildings.DataMember = "kladr_buildings";
            v_buildings.DataSource = v_kladr;

            v_premisesCurrentFund = new BindingSource();
            v_premisesCurrentFund.DataMember = "premises_current_funds";
            v_premisesCurrentFund.DataSource = premisesCurrentFund.Select();

            v_premisesSubPremisesSumArea = new BindingSource();
            v_premisesSubPremisesSumArea.DataMember = "premise_sub_premises_sum_area";
            v_premisesSubPremisesSumArea.DataSource = premiseSubPremisesSumArea.Select();

            v_subPremisesCurrentFund = new BindingSource();
            v_subPremisesCurrentFund.DataMember = "sub_premises_current_funds";
            v_subPremisesCurrentFund.DataSource = subPremisesCurrentFund.Select();

            v_fundType = new BindingSource();
            v_fundType.DataMember = "fund_types";
            v_fundType.DataSource = ds;

            v_object_states = new BindingSource();
            v_object_states.DataMember = "object_states";
            v_object_states.DataSource = ds;

            v_sub_premises_object_states = new BindingSource();
            v_sub_premises_object_states.DataMember = "object_states";
            v_sub_premises_object_states.DataSource = ds;

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += v_premises_CurrentItemChanged;
            v_premises.DataMember = "premises";
            v_premises.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_premises.Filter += " AND ";
            v_premises.Filter += DynamicFilter;
            v_premises.DataSource = ds;
            premises.Select().RowChanged += PremisesViewport_RowChanged;
            premises.Select().RowDeleted += PremisesViewport_RowDeleted;

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
            v_sub_premises.CurrentItemChanged += v_sub_premises_CurrentItemChanged;

            v_restrictionPremisesAssoc = new BindingSource();
            v_restrictionPremisesAssoc.DataMember = "premises_restrictions_premises_assoc";
            v_restrictionPremisesAssoc.CurrentItemChanged += v_restrictionPremisesAssoc_CurrentItemChanged;
            v_restrictionPremisesAssoc.DataSource = v_premises;

            v_restrictionBuildingsAssoc = new BindingSource();
            v_restrictionBuildingsAssoc.DataMember = "restrictions_buildings_assoc";
            v_restrictionBuildingsAssoc.CurrentItemChanged += v_restrictionBuildingsAssoc_CurrentItemChanged;
            v_restrictionBuildingsAssoc.DataSource = ds;

            RestrictionsFilterRebuild();
            restrictionPremisesAssoc.Select().RowChanged += RestrictionsAssoc_RowChanged;
            restrictionPremisesAssoc.Select().RowDeleted += RestrictionsAssoc_RowDeleted;
            restrictionBuildingsAssoc.Select().RowChanged += restrictionBuildingsAssoc_RowChanged;
            restrictionBuildingsAssoc.Select().RowDeleted += restrictionBuildingsAssoc_RowDeleted;

            v_ownershipPremisesAssoc = new BindingSource();
            v_ownershipPremisesAssoc.DataMember = "premises_ownership_premises_assoc";
            v_ownershipPremisesAssoc.CurrentItemChanged += v_ownershipPremisesAssoc_CurrentItemChanged;
            v_ownershipPremisesAssoc.DataSource = v_premises;

            v_ownershipBuildingsAssoc = new BindingSource();
            v_ownershipBuildingsAssoc.DataMember = "ownership_buildings_assoc";
            v_ownershipBuildingsAssoc.CurrentItemChanged += v_ownershipBuildingsAssoc_CurrentItemChanged;
            v_ownershipBuildingsAssoc.DataSource = ds;

            OwnershipsFilterRebuild();
            ownershipPremisesAssoc.Select().RowChanged += OwnershipsAssoc_RowChanged;
            ownershipPremisesAssoc.Select().RowDeleted += OwnershipsAssoc_RowDeleted;
            ownershipBuildingsAssoc.Select().RowChanged += ownershipBuildingsAssoc_RowChanged;
            ownershipBuildingsAssoc.Select().RowDeleted += ownershipBuildingsAssoc_RowDeleted;

            sub_premises.Select().RowChanged += SubPremises_RowChanged;

            DataBind();

            premisesCurrentFund.RefreshEvent += premisesCurrentFund_RefreshEvent;
            premiseSubPremisesSumArea.RefreshEvent += premiseSubPremisesSumArea_RefreshEvent;
            subPremisesCurrentFund.RefreshEvent += subPremisesCurrentFund_RefreshEvent;
            FiltersRebuild();
            SetViewportCaption();
        }
        
        public override bool CanCopyRecord()
        {
            return ((v_premises.Position != -1) && (!premises.EditingNewRecord)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var premise = PremiseFromView();
            v_premises.AddNew();
            premises.EditingNewRecord = true;
            if (premise.IdBuilding != null)
            {
                comboBoxStreet.SelectedValue = premise.IdBuilding;
                comboBoxHouse.SelectedValue = premise.IdBuilding;
            }
            ViewportFromPremise(premise);
            is_editable = true;
        }

        public override bool CanInsertRecord()
        {
            return (!premises.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            v_premises.AddNew();
            is_editable = true;
            premises.EditingNewRecord = true;
            UnbindedCheckBoxesUpdate();
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
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
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
            var Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            is_editable = false;
            v_premises.Filter = Filter;
            is_editable = true;
        }

        public override void ClearSearch()
        {
            is_editable = false;
            v_premises.Filter = StaticFilter;
            is_editable = true;
            DynamicFilter = "";
        }

        public override bool CanDeleteRecord()
        {
            return (v_premises.Position > -1)
                && (viewportState != ViewportState.NewRowState) &&
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
                is_editable = false;
                ((DataRowView)v_premises[v_premises.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(EntityType.Building, id_building, true);
                CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building, id_building, true);
                CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, true);
                CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, true);
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new PremisesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as int?) ?? -1);
            return viewport;
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            var premise = PremiseFromViewport();
            var premiseFromView = PremiseFromView();            
            if (!ValidatePremise(premise))
                return;       
            var Filter = "";
            if (!string.IsNullOrEmpty(v_premises.Filter))
                Filter += " OR ";
            else
                Filter += "(1 = 1) OR ";
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var id_premise = PremisesDataModel.Insert(premise);
                    if (id_premise == -1)
                    {
                        premises.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    premise.IdPremises = id_premise;
                    is_editable = false;
                    if (v_premises.Position == -1)
                        newRow = (DataRowView)v_premises.AddNew();
                    else
                        newRow = ((DataRowView)v_premises[v_premises.Position]);
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_premises = {0})", premise.IdPremises);
                    v_premises.Filter += Filter;
                    FillRowFromPremise(premise, newRow);
                    viewportState = ViewportState.ReadState;
                    is_editable = true;
                    premises.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (premise.IdPremises == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить помещение без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }                   
                    if (PremisesDataModel.Update(premise) == -1)
                        return;
                    var row = ((DataRowView)v_premises[v_premises.Position]);
                    is_editable = false;
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_premises = {0})", premise.IdPremises);
                    v_premises.Filter += Filter;
                    FillRowFromPremise(premise, row);                    
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
            CalcDataModelPremiseSubPremisesSumArea.GetInstance().Refresh(EntityType.Premise, premise.IdPremises, true);
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building, premise.IdBuilding, true);
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
                    premises.EditingNewRecord = false;
                    if (v_premises.Position != -1)
                    {
                        is_editable = false;
                        ((DataRowView)v_premises[v_premises.Position]).Delete();
                    }
                    else
                        Text = "Здания отсутствуют";
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    is_editable = false;
                    DataBind();
                    SelectCurrentBuilding();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                ownershipPremisesAssoc.Select().RowChanged -= OwnershipsAssoc_RowChanged;
                ownershipPremisesAssoc.Select().RowDeleted -= OwnershipsAssoc_RowDeleted;
                ownershipBuildingsAssoc.Select().RowChanged -= ownershipBuildingsAssoc_RowChanged;
                ownershipBuildingsAssoc.Select().RowDeleted -= ownershipBuildingsAssoc_RowDeleted;
                restrictionPremisesAssoc.Select().RowChanged -= RestrictionsAssoc_RowChanged;
                restrictionPremisesAssoc.Select().RowDeleted -= RestrictionsAssoc_RowDeleted;
                restrictionBuildingsAssoc.Select().RowChanged -= restrictionBuildingsAssoc_RowChanged;
                restrictionBuildingsAssoc.Select().RowDeleted -= restrictionBuildingsAssoc_RowDeleted;
                sub_premises.Select().RowChanged -= SubPremises_RowChanged;
                premisesCurrentFund.RefreshEvent -= premisesCurrentFund_RefreshEvent;
                premiseSubPremisesSumArea.RefreshEvent -= premiseSubPremisesSumArea_RefreshEvent;
                subPremisesCurrentFund.RefreshEvent -= subPremisesCurrentFund_RefreshEvent;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                premises.EditingNewRecord = false;
            ownershipPremisesAssoc.Select().RowChanged -= OwnershipsAssoc_RowChanged;
            ownershipPremisesAssoc.Select().RowDeleted -= OwnershipsAssoc_RowDeleted;
            ownershipBuildingsAssoc.Select().RowChanged -= ownershipBuildingsAssoc_RowChanged;
            ownershipBuildingsAssoc.Select().RowDeleted -= ownershipBuildingsAssoc_RowDeleted;
            restrictionPremisesAssoc.Select().RowChanged -= RestrictionsAssoc_RowChanged;
            restrictionPremisesAssoc.Select().RowDeleted -= RestrictionsAssoc_RowDeleted;
            restrictionBuildingsAssoc.Select().RowChanged -= restrictionBuildingsAssoc_RowChanged;
            restrictionBuildingsAssoc.Select().RowDeleted -= restrictionBuildingsAssoc_RowDeleted;
            sub_premises.Select().RowChanged -= SubPremises_RowChanged;
            premisesCurrentFund.RefreshEvent -= premisesCurrentFund_RefreshEvent;
            premiseSubPremisesSumArea.RefreshEvent -= premiseSubPremisesSumArea_RefreshEvent;
            subPremisesCurrentFund.RefreshEvent -= subPremisesCurrentFund_RefreshEvent;
            Close();
        }

        public override bool HasAssocOwnerships()
        {
            return (v_premises.Position != -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (v_premises.Position != -1);
        }

        public override bool HasAssocSubPremises()
        {
            return (v_premises.Position != -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (v_premises.Position != -1);
        }

        public override bool HasAssocTenancies()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasRegistryExcerptPremiseReport()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasRegistryExcerptSubPremiseReport()
        {
            return (v_sub_premises.Position > -1);
        }

        public override bool HasRegistryExcerptSubPremisesReport()
        {
            return (v_premises.Position > -1);
        }

        public override void RegistryExcerptPremiseReportGenerate()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)v_premises[v_premises.Position])["id_premises"].ToString());
            arguments.Add("excerpt_type", "1");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremiseReportGenerate()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)v_sub_premises[v_sub_premises.Position])["id_sub_premises"].ToString());
            arguments.Add("excerpt_type", "2");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremisesReportGenerate()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)v_premises[v_premises.Position])["id_premises"].ToString());
            arguments.Add("excerpt_type", "3");
            reporter.Run(arguments);
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

        public override void ShowTenancies()
        {
            ShowAssocViewport(ViewportType.TenancyListViewport);
        }

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
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

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawRestrictionDataGridRows();
            RedrawOwnershipDataGridRows();
            RedrawSubPremiseDataGridRows();
            UnbindedCheckBoxesUpdate();
            textBoxPremisesNumber.Focus();
            base.OnVisibleChanged(e);
        }

        void premisesCurrentFund_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        void premiseSubPremisesSumArea_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        void subPremisesCurrentFund_RefreshEvent(object sender, EventArgs e)
        {
            RedrawSubPremiseDataGridRows();
        }

        private void SubPremises_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RedrawSubPremiseDataGridRows();
        }

        void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void PremisesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                UnbindedCheckBoxesUpdate();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void PremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            ShowOrHideCurrentFund();
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        private void ownershipBuildingsAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                OwnershipsFilterRebuild();
        }

        private void ownershipBuildingsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                OwnershipsFilterRebuild();
        }

        private void restrictionBuildingsAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                RestrictionsFilterRebuild();
        }

        private void restrictionBuildingsAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                RestrictionsFilterRebuild();
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

        void v_ownershipPremisesAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            OwnershipsFilterRebuild();
        }

        void v_restrictionPremisesAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            RestrictionsFilterRebuild();
        }

        void v_ownershipBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            OwnershipsFilterRebuild();
        }

        void v_restrictionBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            RestrictionsFilterRebuild();
        }

        void comboBoxStreet_VisibleChanged(object sender, EventArgs e)
        {
            if (is_first_visibility)
            {
                SelectCurrentBuilding();
                is_first_visibility = false;
            }
        }

        void comboBoxState_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxStreet_SelectedValueChanged(object sender, EventArgs e)
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

        void numericUpDownLivingArea_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownTotalArea_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void numericUpDownHeight_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void numericUpDownNumRooms_ValueChanged(object sender, EventArgs e)
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

        void textBoxCadastralNum_TextChanged(object sender, EventArgs e)
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

        private void dateTimePickerStateDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxPremisesNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void textBoxAccount_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void dateTimePickerRegDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void checkBoxIsMemorial_CheckedChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void v_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            FiltersRebuild();
            SelectCurrentBuilding();
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            if (v_premises.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        void v_sub_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.DocumentsStateUpdate();
                RedrawSubPremiseDataGridRows();
            }
        }

        private void dataGridViewRestrictions_Resize(object sender, EventArgs e)
        {
            if (dataGridViewRestrictions.Size.Width > 700)
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
            if (dataGridViewOwnerships.Size.Width > 700)
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

        private void dataGridViewRooms_Resize(object sender, EventArgs e)
        {
            if (dataGridViewRooms.Size.Width > 525)
            {
                if (dataGridViewRooms.Columns["sub_premises_id_state"].AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    dataGridViewRooms.Columns["sub_premises_id_state"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (dataGridViewRooms.Columns["sub_premises_id_state"].AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    dataGridViewRooms.Columns["sub_premises_id_state"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
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

        private void dataGridViewRooms_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocSubPremises())
                ShowSubPremises();
        }

        private void textBoxPremisesNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 'А' && e.KeyChar <= 'Я')
                e.KeyChar = e.KeyChar.ToString().ToLower(CultureInfo.CurrentCulture)[0];
            if (e.KeyChar == ' ')
                e.Handled = true;
        }

        private void vButtonRestrictionAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
                editor.ShowDialog();
            }
        }

        private void vButtonRestrictionEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_restrictions.Position == -1)
            {
                MessageBox.Show("Не выбран реквизит для редактирования", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var restriction = new Restriction();
            var row = (DataRowView)v_restrictions[v_restrictions.Position];
            restriction.IdRestriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction");
            restriction.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
            restriction.Number = row["number"].ToString();
            restriction.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
            restriction.Description = row["description"].ToString();
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                if (v_restrictionBuildingsAssoc.Find("id_restriction", restriction.IdRestriction) != -1)
                {
                    editor.ParentType = ParentTypeEnum.Building;
                    editor.ParentRow = BuildingsDataModel.GetInstance().Select().Rows.Find(((DataRowView)v_premises[v_premises.Position])["id_building"]);
                }
                else
                {
                    editor.ParentType = ParentTypeEnum.Premises;
                    editor.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
                }
                editor.RestrictionValue = restriction;
                editor.ShowDialog();
            }
        }

        private void vButtonRestrictionDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_restrictions.Position == -1)
            {
                MessageBox.Show("Не выбран реквизит для удаления", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show("Вы уверены, что хотите удалить этот реквизит?", "Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var idRestriction = (int)((DataRowView)v_restrictions[v_restrictions.Position])["id_restriction"];
            if (RestrictionsDataModel.Delete(idRestriction) == -1)
                return;
            restrictions.Select().Rows.Find(idRestriction).Delete();
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                    int.Parse(((DataRowView)v_premises[v_premises.Position])["id_building"].ToString(), CultureInfo.InvariantCulture), true);
        }

        private void vButtonOwnershipAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
                editor.ShowDialog();
            }
        }

        private void vButtonOwnershipDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_ownershipRights.Position == -1)
            {
                MessageBox.Show("Не выбрано ограничение для удаления", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show("Вы уверены, что хотите удалить это ограничение?", "Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var idOwnershipRight = (int)((DataRowView)v_ownershipRights[v_ownershipRights.Position])["id_ownership_right"];
            if (OwnershipsRightsDataModel.Delete(idOwnershipRight) == -1)
                return;
            ownershipRights.Select().Rows.Find(idOwnershipRight).Delete();
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                    int.Parse(((DataRowView)v_premises[v_premises.Position])["id_building"].ToString(), CultureInfo.InvariantCulture), true);
        }

        private void vButtonOwnershipEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_ownershipRights.Position == -1)
            {
                MessageBox.Show("Не выбрано ограничение для редактирования", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var ownershipRight = new OwnershipRight();
            var row = (DataRowView)v_ownershipRights[v_ownershipRights.Position];
            ownershipRight.IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right");
            ownershipRight.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
            ownershipRight.Number = row["number"].ToString();
            ownershipRight.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
            ownershipRight.Description = row["description"].ToString();
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                if (v_ownershipBuildingsAssoc.Find("id_ownership_right", ownershipRight.IdOwnershipRight) != -1)
                {
                    editor.ParentType = ParentTypeEnum.Building;
                    editor.ParentRow = BuildingsDataModel.GetInstance().Select().Rows.Find(((DataRowView)v_premises[v_premises.Position])["id_building"]);
                }
                else
                {
                    editor.ParentType = ParentTypeEnum.Premises;
                    editor.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
                }
                editor.OwnershipRightValue = ownershipRight;
                editor.ShowDialog();
            }
        }

        private void vButtonSubPremisesAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new SubPremisesEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
                editor.ShowDialog();
            }
        }

        private void vButtonSubPremisesDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_sub_premises.Position == -1)
            {
                MessageBox.Show("Не выбрана комната для удаления", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show("Вы уверены, что хотите удалить эту комнату?", "Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var idSubPremise = (int)((DataRowView)v_sub_premises[v_sub_premises.Position])["id_sub_premises"];
            if (SubPremisesDataModel.Delete(idSubPremise) == -1)
                return;
            sub_premises.Select().Rows.Find(idSubPremise).Delete();
            var ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
            CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            CalcDataModelPremiseSubPremisesSumArea.GetInstance().Refresh(EntityType.Premise,
                int.Parse(ParentRow["id_premises"].ToString(), CultureInfo.InvariantCulture), true);
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                int.Parse(ParentRow["id_building"].ToString(), CultureInfo.InvariantCulture), true);
        }

        private void vButtonSubPremisesEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_sub_premises.Position == -1)
            {
                MessageBox.Show("Не выбрана комната для редактирования", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var subPremise = new SubPremise();
            var row = (DataRowView)v_sub_premises[v_sub_premises.Position];
            subPremise.IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
            subPremise.IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises");
            subPremise.IdState = ViewportHelper.ValueOrNull<int>(row, "id_state");
            subPremise.SubPremisesNum = row["sub_premises_num"].ToString();
            subPremise.TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area");
            subPremise.LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area");
            subPremise.Description = row["description"].ToString();
            subPremise.StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date");
            using (var editor = new SubPremisesEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;             
                editor.SubPremiseValue = subPremise;
                editor.ShowDialog();
            }
        }

        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(PremisesViewport));
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            label29 = new Label();
            textBoxSubPremisesNumber = new TextBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            groupBox13 = new GroupBox();
            textBoxDescription = new TextBox();
            groupBox9 = new GroupBox();
            panel1 = new Panel();
            vButtonRestrictionEdit = new vButton();
            vButtonRestrictionDelete = new vButton();
            vButtonRestrictionAdd = new vButton();
            dataGridViewRestrictions = new DataGridView();
            id_restriction = new DataGridViewTextBoxColumn();
            restriction_number = new DataGridViewTextBoxColumn();
            restriction_date = new DataGridViewTextBoxColumn();
            restriction_description = new DataGridViewTextBoxColumn();
            id_restriction_type = new DataGridViewComboBoxColumn();
            restriction_relation = new DataGridViewTextBoxColumn();
            groupBox10 = new GroupBox();
            panel2 = new Panel();
            vButtonOwnershipEdit = new vButton();
            vButtonOwnershipDelete = new vButton();
            vButtonOwnershipAdd = new vButton();
            dataGridViewOwnerships = new DataGridView();
            id_ownership_right = new DataGridViewTextBoxColumn();
            ownership_number = new DataGridViewTextBoxColumn();
            ownership_date = new DataGridViewTextBoxColumn();
            ownership_description = new DataGridViewTextBoxColumn();
            id_ownership_type = new DataGridViewComboBoxColumn();
            ownership_relation = new DataGridViewTextBoxColumn();
            groupBox8 = new GroupBox();
            tableLayoutPanel4 = new TableLayoutPanel();
            panel3 = new Panel();
            dateTimePickerRegDate = new DateTimePicker();
            label4 = new Label();
            label1 = new Label();
            comboBoxPremisesKind = new ComboBox();
            label28 = new Label();
            numericUpDownNumRooms = new NumericUpDown();
            comboBoxPremisesType = new ComboBox();
            textBoxPremisesNumber = new TextBox();
            label27 = new Label();
            label21 = new Label();
            numericUpDownNumBeds = new NumericUpDown();
            numericUpDownFloor = new NumericUpDown();
            comboBoxStreet = new ComboBox();
            label20 = new Label();
            comboBoxHouse = new ComboBox();
            label19 = new Label();
            panel4 = new Panel();
            dateTimePickerStateDate = new DateTimePicker();
            label6 = new Label();
            checkBoxIsMemorial = new CheckBox();
            textBoxAccount = new TextBox();
            label5 = new Label();
            label39 = new Label();
            comboBoxState = new ComboBox();
            label38 = new Label();
            comboBoxCurrentFundType = new ComboBox();
            numericUpDownBalanceCost = new NumericUpDown();
            label22 = new Label();
            textBoxCadastralNum = new TextBox();
            numericUpDownCadastralCost = new NumericUpDown();
            label23 = new Label();
            label24 = new Label();
            tableLayoutPanel5 = new TableLayoutPanel();
            groupBox11 = new GroupBox();
            numericUpDownHeight = new NumericUpDown();
            label3 = new Label();
            numericUpDownMunicipalArea = new NumericUpDown();
            label2 = new Label();
            numericUpDownLivingArea = new NumericUpDown();
            numericUpDownTotalArea = new NumericUpDown();
            label25 = new Label();
            label26 = new Label();
            groupBoxRooms = new GroupBox();
            panel5 = new Panel();
            vButtonRoomEdit = new vButton();
            vButtonRoomDelete = new vButton();
            vButtonRoomAdd = new vButton();
            dataGridViewRooms = new DataGridView();
            sub_premises_num = new DataGridViewTextBoxColumn();
            sub_premises_total_area = new DataGridViewTextBoxColumn();
            sub_premises_id_state = new DataGridViewComboBoxColumn();
            current_fund = new DataGridViewTextBoxColumn();
            tableLayoutPanel3.SuspendLayout();
            groupBox13.SuspendLayout();
            groupBox9.SuspendLayout();
            panel1.SuspendLayout();
            ((ISupportInitialize)(dataGridViewRestrictions)).BeginInit();
            groupBox10.SuspendLayout();
            panel2.SuspendLayout();
            ((ISupportInitialize)(dataGridViewOwnerships)).BeginInit();
            groupBox8.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            panel3.SuspendLayout();
            ((ISupportInitialize)(numericUpDownNumRooms)).BeginInit();
            ((ISupportInitialize)(numericUpDownNumBeds)).BeginInit();
            ((ISupportInitialize)(numericUpDownFloor)).BeginInit();
            panel4.SuspendLayout();
            ((ISupportInitialize)(numericUpDownBalanceCost)).BeginInit();
            ((ISupportInitialize)(numericUpDownCadastralCost)).BeginInit();
            tableLayoutPanel5.SuspendLayout();
            groupBox11.SuspendLayout();
            ((ISupportInitialize)(numericUpDownHeight)).BeginInit();
            ((ISupportInitialize)(numericUpDownMunicipalArea)).BeginInit();
            ((ISupportInitialize)(numericUpDownLivingArea)).BeginInit();
            ((ISupportInitialize)(numericUpDownTotalArea)).BeginInit();
            groupBoxRooms.SuspendLayout();
            panel5.SuspendLayout();
            ((ISupportInitialize)(dataGridViewRooms)).BeginInit();
            SuspendLayout();
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(10, 97);
            label29.Name = "label29";
            label29.Size = new Size(53, 13);
            label29.TabIndex = 0;
            label29.Text = "Комнаты";
            // 
            // textBoxSubPremisesNumber
            // 
            textBoxSubPremisesNumber.Location = new Point(0, 0);
            textBoxSubPremisesNumber.Name = "textBoxSubPremisesNumber";
            textBoxSubPremisesNumber.Size = new Size(100, 20);
            textBoxSubPremisesNumber.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(groupBox13, 0, 2);
            tableLayoutPanel3.Controls.Add(groupBox9, 0, 3);
            tableLayoutPanel3.Controls.Add(groupBox10, 1, 3);
            tableLayoutPanel3.Controls.Add(groupBox8, 0, 0);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel5, 0, 1);
            tableLayoutPanel3.Controls.Add(groupBoxRooms, 1, 1);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 3;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 269F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(918, 665);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // groupBox13
            // 
            groupBox13.Controls.Add(textBoxDescription);
            groupBox13.Dock = DockStyle.Fill;
            groupBox13.Location = new Point(3, 412);
            groupBox13.Name = "groupBox13";
            groupBox13.Size = new Size(453, 74);
            groupBox13.TabIndex = 4;
            groupBox13.TabStop = false;
            groupBox13.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Location = new Point(3, 17);
            textBoxDescription.MaxLength = 65535;
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(447, 54);
            textBoxDescription.TabIndex = 0;
            textBoxDescription.TextChanged += textBoxDescription_TextChanged;
            textBoxDescription.Enter += selectAll_Enter;
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(panel1);
            groupBox9.Controls.Add(dataGridViewRestrictions);
            groupBox9.Dock = DockStyle.Fill;
            groupBox9.Location = new Point(3, 492);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(453, 170);
            groupBox9.TabIndex = 5;
            groupBox9.TabStop = false;
            groupBox9.Text = "Реквизиты";
            // 
            // panel1
            // 
            panel1.Controls.Add(vButtonRestrictionEdit);
            panel1.Controls.Add(vButtonRestrictionDelete);
            panel1.Controls.Add(vButtonRestrictionAdd);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(412, 17);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(38, 150);
            panel1.TabIndex = 1;
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
            dataGridViewRestrictions.Columns.AddRange(id_restriction, restriction_number, restriction_date, restriction_description, id_restriction_type, restriction_relation);
            dataGridViewRestrictions.Location = new Point(3, 17);
            dataGridViewRestrictions.Name = "dataGridViewRestrictions";
            dataGridViewRestrictions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewRestrictions.Size = new Size(408, 150);
            dataGridViewRestrictions.TabIndex = 0;
            dataGridViewRestrictions.CellDoubleClick += dataGridViewRestrictions_CellDoubleClick;
            dataGridViewRestrictions.Resize += dataGridViewRestrictions_Resize;
            // 
            // id_restriction
            // 
            id_restriction.HeaderText = "Идентификатор";
            id_restriction.Name = "id_restriction";
            id_restriction.Visible = false;
            // 
            // restriction_number
            // 
            restriction_number.HeaderText = "Номер";
            restriction_number.MinimumWidth = 100;
            restriction_number.Name = "restriction_number";
            restriction_number.ReadOnly = true;
            // 
            // restriction_date
            // 
            restriction_date.HeaderText = "Дата";
            restriction_date.MinimumWidth = 100;
            restriction_date.Name = "restriction_date";
            restriction_date.ReadOnly = true;
            // 
            // restriction_description
            // 
            restriction_description.HeaderText = "Наименование";
            restriction_description.MinimumWidth = 200;
            restriction_description.Name = "restriction_description";
            restriction_description.ReadOnly = true;
            restriction_description.Width = 200;
            // 
            // id_restriction_type
            // 
            id_restriction_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_restriction_type.HeaderText = "Тип права собственности";
            id_restriction_type.MinimumWidth = 200;
            id_restriction_type.Name = "id_restriction_type";
            id_restriction_type.ReadOnly = true;
            id_restriction_type.Width = 200;
            // 
            // restriction_relation
            // 
            restriction_relation.HeaderText = "Принадлежность";
            restriction_relation.MinimumWidth = 150;
            restriction_relation.Name = "restriction_relation";
            restriction_relation.SortMode = DataGridViewColumnSortMode.NotSortable;
            restriction_relation.Width = 150;
            // 
            // groupBox10
            // 
            groupBox10.Controls.Add(panel2);
            groupBox10.Controls.Add(dataGridViewOwnerships);
            groupBox10.Dock = DockStyle.Fill;
            groupBox10.Location = new Point(462, 492);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new Size(453, 170);
            groupBox10.TabIndex = 6;
            groupBox10.TabStop = false;
            groupBox10.Text = "Ограничения";
            // 
            // panel2
            // 
            panel2.Controls.Add(vButtonOwnershipEdit);
            panel2.Controls.Add(vButtonOwnershipDelete);
            panel2.Controls.Add(vButtonOwnershipAdd);
            panel2.Dock = DockStyle.Right;
            panel2.Location = new Point(412, 17);
            panel2.Margin = new Padding(0);
            panel2.Name = "panel2";
            panel2.Size = new Size(38, 150);
            panel2.TabIndex = 2;
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
            dataGridViewOwnerships.Columns.AddRange(id_ownership_right, ownership_number, ownership_date, ownership_description, id_ownership_type, ownership_relation);
            dataGridViewOwnerships.Location = new Point(3, 17);
            dataGridViewOwnerships.Name = "dataGridViewOwnerships";
            dataGridViewOwnerships.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOwnerships.Size = new Size(408, 150);
            dataGridViewOwnerships.TabIndex = 0;
            dataGridViewOwnerships.CellDoubleClick += dataGridViewOwnerships_CellDoubleClick;
            dataGridViewOwnerships.Resize += dataGridViewOwnerships_Resize;
            // 
            // id_ownership_right
            // 
            id_ownership_right.HeaderText = "Идентификатор";
            id_ownership_right.Name = "id_ownership_right";
            id_ownership_right.Visible = false;
            // 
            // ownership_number
            // 
            ownership_number.HeaderText = "Номер";
            ownership_number.MinimumWidth = 100;
            ownership_number.Name = "ownership_number";
            ownership_number.ReadOnly = true;
            // 
            // ownership_date
            // 
            ownership_date.HeaderText = "Дата";
            ownership_date.MinimumWidth = 100;
            ownership_date.Name = "ownership_date";
            ownership_date.ReadOnly = true;
            // 
            // ownership_description
            // 
            ownership_description.HeaderText = "Наименование";
            ownership_description.MinimumWidth = 200;
            ownership_description.Name = "ownership_description";
            ownership_description.ReadOnly = true;
            ownership_description.Width = 200;
            // 
            // id_ownership_type
            // 
            id_ownership_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_ownership_type.HeaderText = "Тип ограничения";
            id_ownership_type.MinimumWidth = 200;
            id_ownership_type.Name = "id_ownership_type";
            id_ownership_type.ReadOnly = true;
            id_ownership_type.Width = 200;
            // 
            // ownership_relation
            // 
            ownership_relation.HeaderText = "Принадлежность";
            ownership_relation.MinimumWidth = 150;
            ownership_relation.Name = "ownership_relation";
            ownership_relation.SortMode = DataGridViewColumnSortMode.NotSortable;
            ownership_relation.Width = 150;
            // 
            // groupBox8
            // 
            tableLayoutPanel3.SetColumnSpan(groupBox8, 2);
            groupBox8.Controls.Add(tableLayoutPanel4);
            groupBox8.Dock = DockStyle.Fill;
            groupBox8.Location = new Point(3, 3);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(912, 263);
            groupBox8.TabIndex = 0;
            groupBox8.TabStop = false;
            groupBox8.Text = "Общие сведения";
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(panel3, 0, 0);
            tableLayoutPanel4.Controls.Add(panel4, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(3, 17);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            tableLayoutPanel4.Size = new Size(906, 243);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.Controls.Add(dateTimePickerRegDate);
            panel3.Controls.Add(label4);
            panel3.Controls.Add(label1);
            panel3.Controls.Add(comboBoxPremisesKind);
            panel3.Controls.Add(label28);
            panel3.Controls.Add(numericUpDownNumRooms);
            panel3.Controls.Add(comboBoxPremisesType);
            panel3.Controls.Add(textBoxPremisesNumber);
            panel3.Controls.Add(label27);
            panel3.Controls.Add(label21);
            panel3.Controls.Add(numericUpDownNumBeds);
            panel3.Controls.Add(numericUpDownFloor);
            panel3.Controls.Add(comboBoxStreet);
            panel3.Controls.Add(label20);
            panel3.Controls.Add(comboBoxHouse);
            panel3.Controls.Add(label19);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(3, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(447, 237);
            panel3.TabIndex = 1;
            // 
            // dateTimePickerRegDate
            // 
            dateTimePickerRegDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            dateTimePickerRegDate.Location = new Point(169, 179);
            dateTimePickerRegDate.Name = "dateTimePickerRegDate";
            dateTimePickerRegDate.Size = new Size(272, 21);
            dateTimePickerRegDate.TabIndex = 7;
            dateTimePickerRegDate.ValueChanged += dateTimePickerRegDate_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(10, 182);
            label4.Name = "label4";
            label4.Size = new Size(144, 15);
            label4.TabIndex = 10;
            label4.Text = "Дата включения в РМИ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 126);
            label1.Name = "label1";
            label1.Size = new Size(122, 15);
            label1.TabIndex = 9;
            label1.Text = "Количество комнат";
            // 
            // comboBoxPremisesKind
            // 
            comboBoxPremisesKind.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                          | AnchorStyles.Right;
            comboBoxPremisesKind.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxPremisesKind.FormattingEnabled = true;
            comboBoxPremisesKind.Location = new Point(169, 206);
            comboBoxPremisesKind.Name = "comboBoxPremisesKind";
            comboBoxPremisesKind.Size = new Size(272, 23);
            comboBoxPremisesKind.TabIndex = 4;
            comboBoxPremisesKind.Visible = false;
            comboBoxPremisesKind.SelectedIndexChanged += comboBoxPremisesKind_SelectedIndexChanged;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new Point(10, 210);
            label28.Name = "label28";
            label28.Size = new Size(99, 15);
            label28.TabIndex = 5;
            label28.Text = "Вид помещения";
            label28.Visible = false;
            // 
            // numericUpDownNumRooms
            // 
            numericUpDownNumRooms.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            numericUpDownNumRooms.Location = new Point(169, 123);
            numericUpDownNumRooms.Maximum = new decimal(new[] {
            255,
            0,
            0,
            0});
            numericUpDownNumRooms.Name = "numericUpDownNumRooms";
            numericUpDownNumRooms.Size = new Size(272, 21);
            numericUpDownNumRooms.TabIndex = 5;
            numericUpDownNumRooms.ValueChanged += numericUpDownNumRooms_ValueChanged;
            numericUpDownNumRooms.Enter += selectAll_Enter;
            // 
            // comboBoxPremisesType
            // 
            comboBoxPremisesType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxPremisesType.FlatStyle = FlatStyle.Flat;
            comboBoxPremisesType.FormattingEnabled = true;
            comboBoxPremisesType.Items.AddRange(new object[] {
            "Номер квартиры"});
            comboBoxPremisesType.Location = new Point(9, 66);
            comboBoxPremisesType.Name = "comboBoxPremisesType";
            comboBoxPremisesType.Size = new Size(154, 23);
            comboBoxPremisesType.TabIndex = 2;
            comboBoxPremisesType.SelectedIndexChanged += comboBoxPremisesType_SelectedIndexChanged;
            // 
            // textBoxPremisesNumber
            // 
            textBoxPremisesNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            textBoxPremisesNumber.Location = new Point(169, 67);
            textBoxPremisesNumber.MaxLength = 255;
            textBoxPremisesNumber.Name = "textBoxPremisesNumber";
            textBoxPremisesNumber.Size = new Size(272, 21);
            textBoxPremisesNumber.TabIndex = 3;
            textBoxPremisesNumber.TextChanged += textBoxPremisesNumber_TextChanged;
            textBoxPremisesNumber.Enter += selectAll_Enter;
            textBoxPremisesNumber.KeyPress += textBoxPremisesNumber_KeyPress;
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new Point(10, 154);
            label27.Name = "label27";
            label27.Size = new Size(145, 15);
            label27.TabIndex = 4;
            label27.Text = "Количество койко-мест";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(10, 98);
            label21.Name = "label21";
            label21.Size = new Size(38, 15);
            label21.TabIndex = 6;
            label21.Text = "Этаж";
            // 
            // numericUpDownNumBeds
            // 
            numericUpDownNumBeds.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                          | AnchorStyles.Right;
            numericUpDownNumBeds.Location = new Point(169, 151);
            numericUpDownNumBeds.Maximum = new decimal(new[] {
            255,
            0,
            0,
            0});
            numericUpDownNumBeds.Name = "numericUpDownNumBeds";
            numericUpDownNumBeds.Size = new Size(272, 21);
            numericUpDownNumBeds.TabIndex = 6;
            numericUpDownNumBeds.ValueChanged += numericUpDownNumBeds_ValueChanged;
            numericUpDownNumBeds.Enter += selectAll_Enter;
            // 
            // numericUpDownFloor
            // 
            numericUpDownFloor.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                        | AnchorStyles.Right;
            numericUpDownFloor.Location = new Point(169, 95);
            numericUpDownFloor.Maximum = new decimal(new[] {
            255,
            0,
            0,
            0});
            numericUpDownFloor.Name = "numericUpDownFloor";
            numericUpDownFloor.Size = new Size(272, 21);
            numericUpDownFloor.TabIndex = 4;
            numericUpDownFloor.ValueChanged += numericUpDownFloor_ValueChanged;
            numericUpDownFloor.Enter += selectAll_Enter;
            // 
            // comboBoxStreet
            // 
            comboBoxStreet.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                    | AnchorStyles.Right;
            comboBoxStreet.Location = new Point(169, 7);
            comboBoxStreet.Name = "comboBoxStreet";
            comboBoxStreet.Size = new Size(272, 23);
            comboBoxStreet.TabIndex = 0;
            comboBoxStreet.DropDownClosed += comboBoxStreet_DropDownClosed;
            comboBoxStreet.SelectedValueChanged += comboBoxStreet_SelectedValueChanged;
            comboBoxStreet.VisibleChanged += comboBoxStreet_VisibleChanged;
            comboBoxStreet.Enter += selectAll_Enter;
            comboBoxStreet.KeyUp += comboBoxStreet_KeyUp;
            comboBoxStreet.Leave += comboBoxStreet_Leave;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(10, 41);
            label20.Name = "label20";
            label20.Size = new Size(79, 15);
            label20.TabIndex = 7;
            label20.Text = "Номер дома";
            // 
            // comboBoxHouse
            // 
            comboBoxHouse.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                   | AnchorStyles.Right;
            comboBoxHouse.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxHouse.Location = new Point(169, 37);
            comboBoxHouse.Name = "comboBoxHouse";
            comboBoxHouse.Size = new Size(272, 23);
            comboBoxHouse.TabIndex = 1;
            comboBoxHouse.SelectedIndexChanged += comboBoxHouse_SelectedIndexChanged;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(10, 11);
            label19.Name = "label19";
            label19.Size = new Size(43, 15);
            label19.TabIndex = 8;
            label19.Text = "Улица";
            // 
            // panel4
            // 
            panel4.Controls.Add(dateTimePickerStateDate);
            panel4.Controls.Add(label6);
            panel4.Controls.Add(checkBoxIsMemorial);
            panel4.Controls.Add(textBoxAccount);
            panel4.Controls.Add(label5);
            panel4.Controls.Add(label39);
            panel4.Controls.Add(comboBoxState);
            panel4.Controls.Add(label38);
            panel4.Controls.Add(comboBoxCurrentFundType);
            panel4.Controls.Add(numericUpDownBalanceCost);
            panel4.Controls.Add(label22);
            panel4.Controls.Add(textBoxCadastralNum);
            panel4.Controls.Add(numericUpDownCadastralCost);
            panel4.Controls.Add(label23);
            panel4.Controls.Add(label24);
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(456, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(447, 237);
            panel4.TabIndex = 2;
            // 
            // dateTimePickerStateDate
            // 
            dateTimePickerStateDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            dateTimePickerStateDate.Location = new Point(170, 152);
            dateTimePickerStateDate.Name = "dateTimePickerStateDate";
            dateTimePickerStateDate.ShowCheckBox = true;
            dateTimePickerStateDate.Size = new Size(272, 21);
            dateTimePickerStateDate.TabIndex = 6;
            dateTimePickerStateDate.ValueChanged += dateTimePickerStateDate_ValueChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(16, 154);
            label6.Name = "label6";
            label6.Size = new Size(147, 15);
            label6.TabIndex = 15;
            label6.Text = "Состояние установлено";
            // 
            // checkBoxIsMemorial
            // 
            checkBoxIsMemorial.AutoSize = true;
            checkBoxIsMemorial.Location = new Point(19, 209);
            checkBoxIsMemorial.Name = "checkBoxIsMemorial";
            checkBoxIsMemorial.Size = new Size(141, 19);
            checkBoxIsMemorial.TabIndex = 8;
            checkBoxIsMemorial.Text = "Памятник культуры";
            checkBoxIsMemorial.UseVisualStyleBackColor = true;
            checkBoxIsMemorial.CheckedChanged += checkBoxIsMemorial_CheckedChanged;
            // 
            // textBoxAccount
            // 
            textBoxAccount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                    | AnchorStyles.Right;
            textBoxAccount.Location = new Point(170, 38);
            textBoxAccount.MaxLength = 20;
            textBoxAccount.Name = "textBoxAccount";
            textBoxAccount.Size = new Size(272, 21);
            textBoxAccount.TabIndex = 1;
            textBoxAccount.TextChanged += textBoxAccount_TextChanged;
            textBoxAccount.Enter += selectAll_Enter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(16, 41);
            label5.Name = "label5";
            label5.Size = new Size(116, 15);
            label5.TabIndex = 13;
            label5.Text = "Лицевой счет ФКР";
            // 
            // label39
            // 
            label39.AutoSize = true;
            label39.Location = new Point(16, 127);
            label39.Name = "label39";
            label39.Size = new Size(119, 15);
            label39.TabIndex = 0;
            label39.Text = "Текущее состояние";
            // 
            // comboBoxState
            // 
            comboBoxState.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                   | AnchorStyles.Right;
            comboBoxState.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxState.FormattingEnabled = true;
            comboBoxState.Location = new Point(170, 123);
            comboBoxState.Name = "comboBoxState";
            comboBoxState.Size = new Size(272, 23);
            comboBoxState.TabIndex = 5;
            comboBoxState.SelectedIndexChanged += comboBoxState_SelectedIndexChanged;
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.Location = new Point(16, 182);
            label38.Name = "label38";
            label38.Size = new Size(90, 15);
            label38.TabIndex = 2;
            label38.Text = "Текущий фонд";
            // 
            // comboBoxCurrentFundType
            // 
            comboBoxCurrentFundType.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            comboBoxCurrentFundType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCurrentFundType.Enabled = false;
            comboBoxCurrentFundType.ForeColor = Color.Black;
            comboBoxCurrentFundType.FormattingEnabled = true;
            comboBoxCurrentFundType.Location = new Point(170, 179);
            comboBoxCurrentFundType.Name = "comboBoxCurrentFundType";
            comboBoxCurrentFundType.Size = new Size(272, 23);
            comboBoxCurrentFundType.TabIndex = 7;
            // 
            // numericUpDownBalanceCost
            // 
            numericUpDownBalanceCost.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                              | AnchorStyles.Right;
            numericUpDownBalanceCost.DecimalPlaces = 2;
            numericUpDownBalanceCost.Location = new Point(170, 95);
            numericUpDownBalanceCost.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownBalanceCost.Name = "numericUpDownBalanceCost";
            numericUpDownBalanceCost.Size = new Size(272, 21);
            numericUpDownBalanceCost.TabIndex = 3;
            numericUpDownBalanceCost.ThousandsSeparator = true;
            numericUpDownBalanceCost.ValueChanged += numericUpDownBalanceCost_ValueChanged;
            numericUpDownBalanceCost.Enter += selectAll_Enter;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(16, 98);
            label22.Name = "label22";
            label22.Size = new Size(143, 15);
            label22.TabIndex = 9;
            label22.Text = "Балансовая стоимость";
            // 
            // textBoxCadastralNum
            // 
            textBoxCadastralNum.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                         | AnchorStyles.Right;
            textBoxCadastralNum.Location = new Point(170, 8);
            textBoxCadastralNum.MaxLength = 20;
            textBoxCadastralNum.Name = "textBoxCadastralNum";
            textBoxCadastralNum.Size = new Size(272, 21);
            textBoxCadastralNum.TabIndex = 0;
            textBoxCadastralNum.TextChanged += textBoxCadastralNum_TextChanged;
            textBoxCadastralNum.Enter += selectAll_Enter;
            // 
            // numericUpDownCadastralCost
            // 
            numericUpDownCadastralCost.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            numericUpDownCadastralCost.DecimalPlaces = 2;
            numericUpDownCadastralCost.Location = new Point(170, 68);
            numericUpDownCadastralCost.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownCadastralCost.Name = "numericUpDownCadastralCost";
            numericUpDownCadastralCost.Size = new Size(272, 21);
            numericUpDownCadastralCost.TabIndex = 2;
            numericUpDownCadastralCost.ThousandsSeparator = true;
            numericUpDownCadastralCost.ValueChanged += numericUpDownCadastralCost_ValueChanged;
            numericUpDownCadastralCost.Enter += selectAll_Enter;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(16, 11);
            label23.Name = "label23";
            label23.Size = new Size(126, 15);
            label23.TabIndex = 10;
            label23.Text = "Кадастровый номер";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new Point(16, 70);
            label24.Name = "label24";
            label24.Size = new Size(150, 15);
            label24.TabIndex = 11;
            label24.Text = "Кадастровая стоимость";
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel5.Controls.Add(groupBox11, 0, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 272);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Size = new Size(453, 134);
            tableLayoutPanel5.TabIndex = 1;
            // 
            // groupBox11
            // 
            groupBox11.Controls.Add(numericUpDownHeight);
            groupBox11.Controls.Add(label3);
            groupBox11.Controls.Add(numericUpDownMunicipalArea);
            groupBox11.Controls.Add(label2);
            groupBox11.Controls.Add(numericUpDownLivingArea);
            groupBox11.Controls.Add(numericUpDownTotalArea);
            groupBox11.Controls.Add(label25);
            groupBox11.Controls.Add(label26);
            groupBox11.Dock = DockStyle.Fill;
            groupBox11.Location = new Point(0, 0);
            groupBox11.Margin = new Padding(0);
            groupBox11.Name = "groupBox11";
            groupBox11.Size = new Size(453, 134);
            groupBox11.TabIndex = 1;
            groupBox11.TabStop = false;
            groupBox11.Text = "Геометрия помещения";
            // 
            // numericUpDownHeight
            // 
            numericUpDownHeight.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                         | AnchorStyles.Right;
            numericUpDownHeight.DecimalPlaces = 3;
            numericUpDownHeight.Location = new Point(175, 105);
            numericUpDownHeight.Maximum = new decimal(new[] {
            9999,
            0,
            0,
            0});
            numericUpDownHeight.Name = "numericUpDownHeight";
            numericUpDownHeight.Size = new Size(272, 21);
            numericUpDownHeight.TabIndex = 3;
            numericUpDownHeight.ThousandsSeparator = true;
            numericUpDownHeight.ValueChanged += numericUpDownHeight_ValueChanged;
            numericUpDownHeight.Enter += selectAll_Enter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(16, 108);
            label3.Name = "label3";
            label3.Size = new Size(121, 15);
            label3.TabIndex = 17;
            label3.Text = "Высота помещения";
            // 
            // numericUpDownMunicipalArea
            // 
            numericUpDownMunicipalArea.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            numericUpDownMunicipalArea.DecimalPlaces = 3;
            numericUpDownMunicipalArea.Location = new Point(175, 76);
            numericUpDownMunicipalArea.Maximum = new decimal(new[] {
            9999,
            0,
            0,
            0});
            numericUpDownMunicipalArea.Name = "numericUpDownMunicipalArea";
            numericUpDownMunicipalArea.ReadOnly = true;
            numericUpDownMunicipalArea.Size = new Size(272, 21);
            numericUpDownMunicipalArea.TabIndex = 2;
            numericUpDownMunicipalArea.ThousandsSeparator = true;
            numericUpDownMunicipalArea.Enter += selectAll_Enter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 79);
            label2.Name = "label2";
            label2.Size = new Size(133, 15);
            label2.TabIndex = 15;
            label2.Text = "Площадь мун. комнат";
            // 
            // numericUpDownLivingArea
            // 
            numericUpDownLivingArea.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            numericUpDownLivingArea.DecimalPlaces = 3;
            numericUpDownLivingArea.Location = new Point(175, 47);
            numericUpDownLivingArea.Maximum = new decimal(new[] {
            9999,
            0,
            0,
            0});
            numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            numericUpDownLivingArea.Size = new Size(272, 21);
            numericUpDownLivingArea.TabIndex = 1;
            numericUpDownLivingArea.ThousandsSeparator = true;
            numericUpDownLivingArea.ValueChanged += numericUpDownLivingArea_ValueChanged;
            numericUpDownLivingArea.Enter += selectAll_Enter;
            // 
            // numericUpDownTotalArea
            // 
            numericUpDownTotalArea.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            numericUpDownTotalArea.DecimalPlaces = 3;
            numericUpDownTotalArea.Location = new Point(175, 18);
            numericUpDownTotalArea.Maximum = new decimal(new[] {
            9999,
            0,
            0,
            0});
            numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            numericUpDownTotalArea.Size = new Size(272, 21);
            numericUpDownTotalArea.TabIndex = 0;
            numericUpDownTotalArea.ThousandsSeparator = true;
            numericUpDownTotalArea.ValueChanged += numericUpDownTotalArea_ValueChanged;
            numericUpDownTotalArea.Enter += selectAll_Enter;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new Point(16, 50);
            label25.Name = "label25";
            label25.Size = new Size(100, 15);
            label25.TabIndex = 12;
            label25.Text = "Жилая площадь";
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(16, 21);
            label26.Name = "label26";
            label26.Size = new Size(100, 15);
            label26.TabIndex = 13;
            label26.Text = "Общая площадь";
            // 
            // groupBoxRooms
            // 
            groupBoxRooms.Controls.Add(panel5);
            groupBoxRooms.Controls.Add(dataGridViewRooms);
            groupBoxRooms.Dock = DockStyle.Fill;
            groupBoxRooms.Location = new Point(462, 272);
            groupBoxRooms.Name = "groupBoxRooms";
            tableLayoutPanel3.SetRowSpan(groupBoxRooms, 2);
            groupBoxRooms.Size = new Size(453, 214);
            groupBoxRooms.TabIndex = 3;
            groupBoxRooms.TabStop = false;
            groupBoxRooms.Text = "Комнаты";
            // 
            // panel5
            // 
            panel5.Controls.Add(vButtonRoomEdit);
            panel5.Controls.Add(vButtonRoomDelete);
            panel5.Controls.Add(vButtonRoomAdd);
            panel5.Dock = DockStyle.Right;
            panel5.Location = new Point(412, 17);
            panel5.Margin = new Padding(0);
            panel5.Name = "panel5";
            panel5.Size = new Size(38, 194);
            panel5.TabIndex = 3;
            // 
            // vButtonRoomEdit
            // 
            vButtonRoomEdit.AllowAnimations = true;
            vButtonRoomEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonRoomEdit.BackColor = Color.Transparent;
            vButtonRoomEdit.Image = ((Image)(resources.GetObject("vButtonRoomEdit.Image")));
            vButtonRoomEdit.Location = new Point(3, 57);
            vButtonRoomEdit.Name = "vButtonRoomEdit";
            vButtonRoomEdit.RoundedCornersMask = 15;
            vButtonRoomEdit.Size = new Size(32, 25);
            vButtonRoomEdit.TabIndex = 2;
            vButtonRoomEdit.UseVisualStyleBackColor = false;
            vButtonRoomEdit.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonRoomEdit.Click += vButtonSubPremisesEdit_Click;
            // 
            // vButtonRoomDelete
            // 
            vButtonRoomDelete.AllowAnimations = true;
            vButtonRoomDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonRoomDelete.BackColor = Color.Transparent;
            vButtonRoomDelete.Image = ((Image)(resources.GetObject("vButtonRoomDelete.Image")));
            vButtonRoomDelete.Location = new Point(3, 30);
            vButtonRoomDelete.Name = "vButtonRoomDelete";
            vButtonRoomDelete.RoundedCornersMask = 15;
            vButtonRoomDelete.Size = new Size(32, 25);
            vButtonRoomDelete.TabIndex = 1;
            vButtonRoomDelete.UseVisualStyleBackColor = false;
            vButtonRoomDelete.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonRoomDelete.Click += vButtonSubPremisesDelete_Click;
            // 
            // vButtonRoomAdd
            // 
            vButtonRoomAdd.AllowAnimations = true;
            vButtonRoomAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonRoomAdd.BackColor = Color.Transparent;
            vButtonRoomAdd.Image = ((Image)(resources.GetObject("vButtonRoomAdd.Image")));
            vButtonRoomAdd.Location = new Point(3, 3);
            vButtonRoomAdd.Name = "vButtonRoomAdd";
            vButtonRoomAdd.RoundedCornersMask = 15;
            vButtonRoomAdd.Size = new Size(32, 25);
            vButtonRoomAdd.TabIndex = 0;
            vButtonRoomAdd.UseVisualStyleBackColor = false;
            vButtonRoomAdd.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonRoomAdd.Click += vButtonSubPremisesAdd_Click;
            // 
            // dataGridViewRooms
            // 
            dataGridViewRooms.AllowUserToAddRows = false;
            dataGridViewRooms.AllowUserToDeleteRows = false;
            dataGridViewRooms.AllowUserToResizeRows = false;
            dataGridViewRooms.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) 
                                        | AnchorStyles.Left) 
                                       | AnchorStyles.Right;
            dataGridViewRooms.BackgroundColor = Color.White;
            dataGridViewRooms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewRooms.Columns.AddRange(sub_premises_num, sub_premises_total_area, sub_premises_id_state, current_fund);
            dataGridViewRooms.Location = new Point(3, 17);
            dataGridViewRooms.Name = "dataGridViewRooms";
            dataGridViewRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewRooms.Size = new Size(408, 197);
            dataGridViewRooms.TabIndex = 0;
            dataGridViewRooms.CellDoubleClick += dataGridViewRooms_CellDoubleClick;
            dataGridViewRooms.Resize += dataGridViewRooms_Resize;
            // 
            // sub_premises_num
            // 
            sub_premises_num.HeaderText = "Номер";
            sub_premises_num.MinimumWidth = 100;
            sub_premises_num.Name = "sub_premises_num";
            sub_premises_num.ReadOnly = true;
            // 
            // sub_premises_total_area
            // 
            dataGridViewCellStyle1.Format = "#0.0## м²";
            sub_premises_total_area.DefaultCellStyle = dataGridViewCellStyle1;
            sub_premises_total_area.HeaderText = "Общая площадь";
            sub_premises_total_area.MinimumWidth = 125;
            sub_premises_total_area.Name = "sub_premises_total_area";
            sub_premises_total_area.ReadOnly = true;
            sub_premises_total_area.Width = 125;
            // 
            // sub_premises_id_state
            // 
            sub_premises_id_state.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            sub_premises_id_state.HeaderText = "Текущее состояние";
            sub_premises_id_state.MinimumWidth = 150;
            sub_premises_id_state.Name = "sub_premises_id_state";
            sub_premises_id_state.ReadOnly = true;
            sub_premises_id_state.Resizable = DataGridViewTriState.True;
            sub_premises_id_state.SortMode = DataGridViewColumnSortMode.Automatic;
            sub_premises_id_state.Width = 150;
            // 
            // current_fund
            // 
            current_fund.HeaderText = "Текущий фонд";
            current_fund.MinimumWidth = 150;
            current_fund.Name = "current_fund";
            current_fund.Width = 150;
            // 
            // PremisesViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(665, 580);
            BackColor = Color.White;
            ClientSize = new Size(924, 671);
            Controls.Add(tableLayoutPanel3);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "PremisesViewport";
            Padding = new Padding(3);
            Text = "Помещение";
            tableLayoutPanel3.ResumeLayout(false);
            groupBox13.ResumeLayout(false);
            groupBox13.PerformLayout();
            groupBox9.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewRestrictions)).EndInit();
            groupBox10.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewOwnerships)).EndInit();
            groupBox8.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((ISupportInitialize)(numericUpDownNumRooms)).EndInit();
            ((ISupportInitialize)(numericUpDownNumBeds)).EndInit();
            ((ISupportInitialize)(numericUpDownFloor)).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ((ISupportInitialize)(numericUpDownBalanceCost)).EndInit();
            ((ISupportInitialize)(numericUpDownCadastralCost)).EndInit();
            tableLayoutPanel5.ResumeLayout(false);
            groupBox11.ResumeLayout(false);
            groupBox11.PerformLayout();
            ((ISupportInitialize)(numericUpDownHeight)).EndInit();
            ((ISupportInitialize)(numericUpDownMunicipalArea)).EndInit();
            ((ISupportInitialize)(numericUpDownLivingArea)).EndInit();
            ((ISupportInitialize)(numericUpDownTotalArea)).EndInit();
            groupBoxRooms.ResumeLayout(false);
            panel5.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewRooms)).EndInit();
            ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
