using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels;
using Registry.Entities;
using System.Drawing;
using Registry.SearchForms;
using Registry.CalcDataModels;
using Security;
using System.Globalization;
using System.Text.RegularExpressions;
using Registry.Reporting;

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
        #endregion Components

        #region Models
        private PremisesDataModel premises = null;
        private CalcDataModelPremisesCurrentFunds premisesCurrentFund = null;
        private BuildingsDataModel buildings = null;
        private KladrStreetsDataModel kladr = null;
        private PremisesTypesDataModel premises_types = null;
        private PremisesKindsDataModel premises_kinds = null;
        private SubPremisesDataModel sub_premises = null;
        private RestrictionsDataModel restrictions = null;
        private RestrictionTypesDataModel restrictionTypes = null;
        private RestrictionsPremisesAssocDataModel restrictionPremisesAssoc = null;
        private RestrictionsBuildingsAssocDataModel restrictionBuildingsAssoc = null;
        private OwnershipsRightsDataModel ownershipRights = null;
        private OwnershipRightTypesDataModel ownershipRightTypes = null;
        private OwnershipPremisesAssocDataModel ownershipPremisesAssoc = null;
        private OwnershipBuildingsAssocDataModel ownershipBuildingsAssoc = null;
        private FundTypesDataModel fundTypes = null;
        private ObjectStatesDataModel object_states = null;
        private CalcDataModelPremiseSubPremisesSumArea premiseSubPremisesSumArea = null;
        #endregion Models

        #region Views
        private BindingSource v_premises = null;
        private BindingSource v_premisesCurrentFund = null;
        private BindingSource v_buildings = null;
        private BindingSource v_kladr = null;
        private BindingSource v_premises_types = null;
        private BindingSource v_premises_kinds = null;
        private BindingSource v_sub_premises = null;
        private BindingSource v_restrictions = null;
        private BindingSource v_restrictonTypes = null;
        private BindingSource v_restrictionPremisesAssoc = null;
        private BindingSource v_restrictionBuildingsAssoc = null;
        private BindingSource v_ownershipRights = null;
        private BindingSource v_ownershipRightTypes = null;
        private BindingSource v_ownershipPremisesAssoc = null;
        private BindingSource v_ownershipBuildingsAssoc = null;
        private BindingSource v_fundType = null;
        private BindingSource v_object_states = null;
        private BindingSource v_sub_premises_object_states = null;
        private BindingSource v_premisesSubPremisesSumArea = null;
        #endregion Views

        //Forms
        private SearchForm spExtendedSearchForm = null;
        private SearchForm spSimpleSearchForm = null;

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;
        private Label label1;
        private NumericUpDown numericUpDownNumRooms;
        private NumericUpDown numericUpDownMunicipalArea;
        private Label label2;
        private DataGridViewTextBoxColumn sub_premises_num;
        private DataGridViewTextBoxColumn sub_premises_total_area;
        private DataGridViewComboBoxColumn sub_premises_id_state;
        private NumericUpDown numericUpDownHeight;
        private Label label3;
        private DataGridViewTextBoxColumn id_restriction;
        private DataGridViewTextBoxColumn restriction_number;
        private DataGridViewTextBoxColumn restriction_date;
        private DataGridViewTextBoxColumn restriction_description;
        private DataGridViewComboBoxColumn id_restriction_type;
        private DataGridViewTextBoxColumn id_ownership_right;
        private DataGridViewTextBoxColumn ownership_number;
        private DataGridViewTextBoxColumn ownership_date;
        private DataGridViewTextBoxColumn ownership_description;
        private DataGridViewComboBoxColumn id_ownership_type;
        private DateTimePicker dateTimePickerRegDate;
        private Label label4;
        private CheckBox checkBoxIsMemorial;
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
            this.DynamicFilter = premsiesListViewport.DynamicFilter;
            this.StaticFilter = premsiesListViewport.StaticFilter;
            this.ParentRow = premsiesListViewport.ParentRow;
            this.ParentType = premsiesListViewport.ParentType;
        }

        private void RestrictionsFilterRebuild()
        {
            string restrictionsFilter = "id_restriction IN (0";
            for (int i = 0; i < v_restrictionPremisesAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)v_restrictionPremisesAssoc[i])["id_restriction"].ToString() + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(new char[] { ',' });
            restrictionsFilter += ")";
            if (v_premises.Position > -1 && v_restrictionBuildingsAssoc != null && ((DataRowView)v_premises[v_premises.Position])["id_building"] != DBNull.Value)
            {
                v_restrictionBuildingsAssoc.Filter = "id_building = " + ((DataRowView)v_premises[v_premises.Position])["id_building"].ToString();
                restrictionsFilter += " OR id_restriction IN (0";
                for (int i = 0; i < v_restrictionBuildingsAssoc.Count; i++)
                    restrictionsFilter += ((DataRowView)v_restrictionBuildingsAssoc[i])["id_restriction"].ToString() + ",";
                restrictionsFilter = restrictionsFilter.TrimEnd(new char[] { ',' });
                restrictionsFilter += ")";
            }
            v_restrictions.Filter = restrictionsFilter;
            for (int i = 0; i < dataGridViewRestrictions.Rows.Count; i++)
                if (v_restrictionBuildingsAssoc.Find("id_restriction", dataGridViewRestrictions.Rows[i].Cells["id_restriction"].Value) != -1)
                    dataGridViewRestrictions.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    dataGridViewRestrictions.Rows[i].DefaultCellStyle.BackColor = Color.White;
            if (dataGridViewRestrictions.Columns.Contains("id_restriction"))
                dataGridViewRestrictions.Columns["id_restriction"].Visible = false;
        }

        private void OwnershipsFilterRebuild()
        {
            string ownershipFilter = "id_ownership_right IN (0";
            for (int i = 0; i < v_ownershipPremisesAssoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownershipPremisesAssoc[i])["id_ownership_right"].ToString() + ",";
            ownershipFilter = ownershipFilter.TrimEnd(new char[] { ',' });
            ownershipFilter += ")";
            if (v_premises.Position > -1 && v_ownershipBuildingsAssoc != null && ((DataRowView)v_premises[v_premises.Position])["id_building"] != DBNull.Value)
            {
                v_ownershipBuildingsAssoc.Filter = "id_building = " + ((DataRowView)v_premises[v_premises.Position])["id_building"].ToString();
                ownershipFilter += " OR id_ownership_right IN (0";
                for (int i = 0; i < v_ownershipBuildingsAssoc.Count; i++)
                    ownershipFilter += ((DataRowView)v_ownershipBuildingsAssoc[i])["id_ownership_right"].ToString() + ",";
                ownershipFilter = ownershipFilter.TrimEnd(new char[] { ',' });
                ownershipFilter += ")";
            }
            v_ownershipRights.Filter = ownershipFilter;
            for (int i = 0; i < dataGridViewOwnerships.Rows.Count; i++)
                if (v_ownershipBuildingsAssoc.Find("id_ownership_right", dataGridViewOwnerships.Rows[i].Cells["id_ownership_right"].Value) != -1)
                    dataGridViewOwnerships.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    dataGridViewOwnerships.Rows[i].DefaultCellStyle.BackColor = Color.White;
            if (dataGridViewOwnerships.Columns.Contains("id_ownership_right"))
                dataGridViewOwnerships.Columns["id_ownership_right"].Visible = false;
        }

        private void FiltersRebuild()
        {
            if (v_premisesCurrentFund != null)
            {
                int position = -1;
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
                int position = -1;
                if ((v_premises.Position != -1) && !(((DataRowView)v_premises[v_premises.Position])["id_premises"] is DBNull))
                    position = v_premisesSubPremisesSumArea.Find("id_premises", ((DataRowView)v_premises[v_premises.Position])["id_premises"]);
                if (position != -1)
                {
                    decimal value = Convert.ToDecimal((double)((DataRowView)v_premisesSubPremisesSumArea[position])["sum_area"]);
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

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                {
                    this.Text = String.Format(CultureInfo.InvariantCulture, "Новое помещение здания №{0}", ParentRow["id_building"]);
                }
                else
                    this.Text = "Новое помещение";
            }
            else
                if (v_premises.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Помещение №{0} здания №{1}",
                            ((DataRowView)v_premises[v_premises.Position])["id_premises"], ParentRow["id_building"]);
                    else
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Помещение №{0}", ((DataRowView)v_premises[v_premises.Position])["id_premises"]);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Помещения в здании №{0} отсутствуют", ParentRow["id_building"]);
                    else
                        this.Text = "Помещения отсутствуют";
                }
        }

        private void ShowOrHideCurrentFund()
        {
            if (comboBoxCurrentFundType.SelectedValue != null && v_premises.Position != -1 &&
                ((DataRowView)v_premises[v_premises.Position])["id_state"] != DBNull.Value &&
                (int)((DataRowView)v_premises[v_premises.Position])["id_state"] != 3)
            {
                label38.Visible = true;
                comboBoxCurrentFundType.Visible = true;
                checkBoxIsMemorial.Location = new System.Drawing.Point(19, 181);
            }
            else
            {
                label38.Visible = false;
                comboBoxCurrentFundType.Visible = false;
                checkBoxIsMemorial.Location = new System.Drawing.Point(19, 153);
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
                    DataRow building_row = buildings.Select().Rows.Find(id_building);
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
                            DialogResult result = MessageBox.Show("Сохранить изменения о помещениях в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения о помещениях в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения о помещениях в базу данных?", "Внимание",
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
            MenuCallback.EditingStateUpdate();
        }

        public void LocatePremisesBy(int id)
        {
            int Position = v_premises.Find("id_premises", id);
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
            if (premise.PremisesNum == null || String.IsNullOrEmpty(premise.PremisesNum.Trim()))
            {
                MessageBox.Show("Необходимо указать номер помещения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNumber.Focus();
                return false;
            }
            if (!Regex.IsMatch(premise.PremisesNum, @"^[0-9]+[а-я]{0,1}([,][0-9]+[а-я]{0,1})*$"))
            {
                MessageBox.Show("Некорректно задан номер помещения. Можно использовать только цифры и не более одной строчной буквы кирилицы. Для объединенных квартир номера должны быть перечислены через запятую. Например: \"1а,2а,3\"", "Ошибка",
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
            Premise premiseFromView = PremiseFromView();
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
            if (new int[] { 4, 5 }.Contains(premise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу муниципальных жилых помещений", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new int[] { 1, 3 }.Contains(premise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу немуниципальных жилых помещений", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            // Проверяем дубликаты квартир
            if ((premise.PremisesNum != premiseFromView.PremisesNum) || (premise.IdBuilding != premiseFromView.IdBuilding))
                if (DataModelHelper.PremisesDuplicateCount(premise) != 0 &&
                    MessageBox.Show("В указанном доме уже есть квартира с таким номером. Все равно продолжить сохранение?", "Внимание", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                    return false;
            return true;
        }

        private Premise PremiseFromView()
        {
            Premise premise = new Premise();
            DataRowView row = (DataRowView)v_premises[v_premises.Position];
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
            premise.RegDate = ViewportHelper.ValueOrNull<DateTime>(row, "reg_date");
            return premise;
        }

        private Premise PremiseFromViewport()
        {
            Premise premise = new Premise();
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
            premise.Floor = Convert.ToInt16(numericUpDownFloor.Value);
            premise.CadastralNum = ViewportHelper.ValueOrNull(textBoxCadastralNum);
            premise.CadastralCost = numericUpDownCadastralCost.Value;
            premise.BalanceCost = numericUpDownBalanceCost.Value;
            premise.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            premise.IsMemorial = checkBoxIsMemorial.Checked;
            premise.RegDate = ViewportHelper.ValueOrNull(dateTimePickerRegDate);
            return premise;
        }

        private void ViewportFromPremise(Premise premise)
        {
            if (premise.IdBuilding != null)
            {
                DataRow building_row = buildings.Select().Rows.Find(premise.IdBuilding);
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
            checkBoxIsMemorial.Checked = ViewportHelper.ValueOrDefault(premise.IsMemorial);
            textBoxPremisesNumber.Text = premise.PremisesNum;
            textBoxCadastralNum.Text = premise.CadastralNum;
            textBoxDescription.Text = premise.Description;
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;

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

            DataSet ds = DataSetManager.DataSet;

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
            v_premises.CurrentItemChanged += new EventHandler(v_premises_CurrentItemChanged);
            v_premises.DataMember = "premises";
            v_premises.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
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
            v_restrictionPremisesAssoc.CurrentItemChanged += new EventHandler(v_restrictionPremisesAssoc_CurrentItemChanged);
            v_restrictionPremisesAssoc.DataSource = v_premises;

            v_restrictionBuildingsAssoc = new BindingSource();
            v_restrictionBuildingsAssoc.DataMember = "restrictions_buildings_assoc";
            v_restrictionBuildingsAssoc.CurrentItemChanged += v_restrictionBuildingsAssoc_CurrentItemChanged;
            v_restrictionBuildingsAssoc.DataSource = ds;

            RestrictionsFilterRebuild();
            restrictionPremisesAssoc.Select().RowChanged += new DataRowChangeEventHandler(RestrictionsAssoc_RowChanged);
            restrictionPremisesAssoc.Select().RowDeleted += new DataRowChangeEventHandler(RestrictionsAssoc_RowDeleted);
            restrictionBuildingsAssoc.Select().RowChanged += restrictionBuildingsAssoc_RowChanged;
            restrictionBuildingsAssoc.Select().RowDeleted += restrictionBuildingsAssoc_RowDeleted;

            v_ownershipPremisesAssoc = new BindingSource();
            v_ownershipPremisesAssoc.DataMember = "premises_ownership_premises_assoc";
            v_ownershipPremisesAssoc.CurrentItemChanged += new EventHandler(v_ownershipPremisesAssoc_CurrentItemChanged);
            v_ownershipPremisesAssoc.DataSource = v_premises;

            v_ownershipBuildingsAssoc = new BindingSource();
            v_ownershipBuildingsAssoc.DataMember = "ownership_buildings_assoc";
            v_ownershipBuildingsAssoc.CurrentItemChanged += v_ownershipBuildingsAssoc_CurrentItemChanged;
            v_ownershipBuildingsAssoc.DataSource = ds;

            OwnershipsFilterRebuild();
            ownershipPremisesAssoc.Select().RowChanged += new DataRowChangeEventHandler(OwnershipsAssoc_RowChanged);
            ownershipPremisesAssoc.Select().RowDeleted += new DataRowChangeEventHandler(OwnershipsAssoc_RowDeleted);
            ownershipBuildingsAssoc.Select().RowChanged += new DataRowChangeEventHandler(ownershipBuildingsAssoc_RowChanged);
            ownershipBuildingsAssoc.Select().RowDeleted += new DataRowChangeEventHandler(ownershipBuildingsAssoc_RowDeleted);

            DataBind();

            premisesCurrentFund.RefreshEvent += new EventHandler<EventArgs>(premisesCurrentFund_RefreshEvent);
            premiseSubPremisesSumArea.RefreshEvent += premiseSubPremisesSumArea_RefreshEvent;
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
            Premise premise = PremiseFromView();
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
        }

        public override bool CanSearchRecord()
        {
            return true;
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
            string Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
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
                int id_building = (int)((DataRowView)v_premises[v_premises.Position])["id_building"];
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
            PremisesViewport viewport = new PremisesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            Premise premise = PremiseFromViewport();
            bool updateSubPremisesState = false;
            if (!ValidatePremise(premise))
                return;
            if ((viewportState == ViewportState.ModifyRowState) && (premise.IdState != PremiseFromView().IdState) && (premise.IdState != 1))
            {
                if (MessageBox.Show("Вы пытаетесь изменить состояние помещения. В результате всем комнатам данного помещения будет назначено то же состояние. " +
                    "Вы уверены, что хотите сохранить данные?", "Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return;
                updateSubPremisesState = true;
            }
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    int id_premise = PremisesDataModel.Insert(premise);
                    if (id_premise == -1)
                        return;
                    DataRowView newRow;
                    premise.IdPremises = id_premise;
                    is_editable = false;
                    if (v_premises.Position == -1)
                        newRow = (DataRowView)v_premises.AddNew();
                    else
                        newRow = ((DataRowView)v_premises[v_premises.Position]);
                    FillRowFromPremise(premise, newRow);
                    premises.EditingNewRecord = false;
                    viewportState = ViewportState.ReadState;
                    is_editable = true;
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
                    DataRowView row = ((DataRowView)v_premises[v_premises.Position]);
                    is_editable = false;
                    FillRowFromPremise(premise, row);
                    if (updateSubPremisesState)
                    {
                        for (int i = 0; i < v_sub_premises.Count; i++)
                        {
                            DataRowView subPremiseRow = (DataRowView)v_sub_premises[i];
                            subPremiseRow["id_state"] = premise.IdState;
                            subPremiseRow.EndEdit();
                        }
                    }
                    viewportState = ViewportState.ReadState;
                    CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
                    CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
                    break;
            }
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
                        this.Text = "Здания отсутствуют";
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    is_editable = false;
                    DataBind();
                    SelectCurrentBuilding();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                ownershipPremisesAssoc.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipsAssoc_RowChanged);
                ownershipPremisesAssoc.Select().RowDeleted -= new DataRowChangeEventHandler(OwnershipsAssoc_RowDeleted);
                ownershipBuildingsAssoc.Select().RowChanged -= new DataRowChangeEventHandler(ownershipBuildingsAssoc_RowChanged);
                ownershipBuildingsAssoc.Select().RowDeleted -= new DataRowChangeEventHandler(ownershipBuildingsAssoc_RowDeleted);
                restrictionPremisesAssoc.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionsAssoc_RowChanged);
                restrictionPremisesAssoc.Select().RowDeleted -= new DataRowChangeEventHandler(RestrictionsAssoc_RowDeleted);
                restrictionBuildingsAssoc.Select().RowChanged -= restrictionBuildingsAssoc_RowChanged;
                restrictionBuildingsAssoc.Select().RowDeleted -= restrictionBuildingsAssoc_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                premises.EditingNewRecord = false;
            ownershipPremisesAssoc.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipsAssoc_RowChanged);
            ownershipPremisesAssoc.Select().RowDeleted -= new DataRowChangeEventHandler(OwnershipsAssoc_RowDeleted);
            ownershipBuildingsAssoc.Select().RowChanged -= new DataRowChangeEventHandler(ownershipBuildingsAssoc_RowChanged);
            ownershipBuildingsAssoc.Select().RowDeleted -= new DataRowChangeEventHandler(ownershipBuildingsAssoc_RowDeleted);
            restrictionPremisesAssoc.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionsAssoc_RowChanged);
            restrictionPremisesAssoc.Select().RowDeleted -= new DataRowChangeEventHandler(RestrictionsAssoc_RowDeleted);
            restrictionBuildingsAssoc.Select().RowChanged -= restrictionBuildingsAssoc_RowChanged;
            restrictionBuildingsAssoc.Select().RowDeleted -= restrictionBuildingsAssoc_RowDeleted;
            base.Close();
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
            Reporter reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)v_premises[v_premises.Position])["id_premises"].ToString());
            arguments.Add("excerpt_type", "1");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremiseReportGenerate()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            Reporter reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)v_sub_premises[v_sub_premises.Position])["id_sub_premises"].ToString());
            arguments.Add("excerpt_type", "2");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremisesReportGenerate()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            Reporter reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            Dictionary<string, string> arguments = new Dictionary<string, string>();
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

        void premisesCurrentFund_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        void premiseSubPremisesSumArea_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void PremisesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void PremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            ShowOrHideCurrentFund();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
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

        void textBoxPremisesNumber_TextChanged(object sender, EventArgs e)
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
                MenuCallback.DocumentsStateUpdate();
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

        private void dataGridViewRooms_Resize(object sender, EventArgs e)
        {
            if (dataGridViewRooms.Size.Width > 280)
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

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PremisesViewport));
            this.label29 = new System.Windows.Forms.Label();
            this.textBoxSubPremisesNumber = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.dataGridViewRestrictions = new System.Windows.Forms.DataGridView();
            this.id_restriction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_restriction_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.dataGridViewOwnerships = new System.Windows.Forms.DataGridView();
            this.id_ownership_right = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_ownership_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dateTimePickerRegDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownNumRooms = new System.Windows.Forms.NumericUpDown();
            this.comboBoxPremisesType = new System.Windows.Forms.ComboBox();
            this.textBoxPremisesNumber = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.numericUpDownNumBeds = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownFloor = new System.Windows.Forms.NumericUpDown();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.comboBoxHouse = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.checkBoxIsMemorial = new System.Windows.Forms.CheckBox();
            this.comboBoxPremisesKind = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.comboBoxState = new System.Windows.Forms.ComboBox();
            this.label38 = new System.Windows.Forms.Label();
            this.comboBoxCurrentFundType = new System.Windows.Forms.ComboBox();
            this.numericUpDownBalanceCost = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.textBoxCadastralNum = new System.Windows.Forms.TextBox();
            this.numericUpDownCadastralCost = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownMunicipalArea = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownLivingArea = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTotalArea = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBoxRooms = new System.Windows.Forms.GroupBox();
            this.dataGridViewRooms = new System.Windows.Forms.DataGridView();
            this.sub_premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_id_state = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).BeginInit();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumRooms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumBeds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunicipalArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).BeginInit();
            this.groupBoxRooms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRooms)).BeginInit();
            this.SuspendLayout();
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(10, 97);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(53, 13);
            this.label29.TabIndex = 0;
            this.label29.Text = "Комнаты";
            // 
            // textBoxSubPremisesNumber
            // 
            this.textBoxSubPremisesNumber.Location = new System.Drawing.Point(0, 0);
            this.textBoxSubPremisesNumber.Name = "textBoxSubPremisesNumber";
            this.textBoxSubPremisesNumber.Size = new System.Drawing.Size(100, 20);
            this.textBoxSubPremisesNumber.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox13, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.groupBox9, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.groupBox10, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.groupBox8, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupBoxRooms, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(858, 581);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.textBoxDescription);
            this.groupBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox13.Location = new System.Drawing.Point(3, 383);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(423, 74);
            this.groupBox13.TabIndex = 4;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 17);
            this.textBoxDescription.MaxLength = 65535;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(417, 54);
            this.textBoxDescription.TabIndex = 0;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.dataGridViewRestrictions);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(3, 463);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(423, 115);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Реквизиты";
            // 
            // dataGridViewRestrictions
            // 
            this.dataGridViewRestrictions.AllowUserToAddRows = false;
            this.dataGridViewRestrictions.AllowUserToDeleteRows = false;
            this.dataGridViewRestrictions.AllowUserToResizeRows = false;
            this.dataGridViewRestrictions.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewRestrictions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRestrictions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_restriction,
            this.restriction_number,
            this.restriction_date,
            this.restriction_description,
            this.id_restriction_type});
            this.dataGridViewRestrictions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRestrictions.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewRestrictions.Name = "dataGridViewRestrictions";
            this.dataGridViewRestrictions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRestrictions.Size = new System.Drawing.Size(417, 95);
            this.dataGridViewRestrictions.TabIndex = 0;
            this.dataGridViewRestrictions.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRestrictions_CellDoubleClick);
            this.dataGridViewRestrictions.Resize += new System.EventHandler(this.dataGridViewRestrictions_Resize);
            // 
            // id_restriction
            // 
            this.id_restriction.HeaderText = "Идентификатор";
            this.id_restriction.Name = "id_restriction";
            this.id_restriction.Visible = false;
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
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.dataGridViewOwnerships);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Location = new System.Drawing.Point(432, 463);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(423, 115);
            this.groupBox10.TabIndex = 6;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Ограничения";
            // 
            // dataGridViewOwnerships
            // 
            this.dataGridViewOwnerships.AllowUserToAddRows = false;
            this.dataGridViewOwnerships.AllowUserToDeleteRows = false;
            this.dataGridViewOwnerships.AllowUserToResizeRows = false;
            this.dataGridViewOwnerships.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewOwnerships.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOwnerships.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_ownership_right,
            this.ownership_number,
            this.ownership_date,
            this.ownership_description,
            this.id_ownership_type});
            this.dataGridViewOwnerships.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewOwnerships.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewOwnerships.Name = "dataGridViewOwnerships";
            this.dataGridViewOwnerships.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewOwnerships.Size = new System.Drawing.Size(417, 95);
            this.dataGridViewOwnerships.TabIndex = 0;
            this.dataGridViewOwnerships.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewOwnerships_CellDoubleClick);
            this.dataGridViewOwnerships.Resize += new System.EventHandler(this.dataGridViewOwnerships_Resize);
            // 
            // id_ownership_right
            // 
            this.id_ownership_right.HeaderText = "Идентификатор";
            this.id_ownership_right.Name = "id_ownership_right";
            this.id_ownership_right.Visible = false;
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
            // groupBox8
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox8, 2);
            this.groupBox8.Controls.Add(this.tableLayoutPanel4);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(852, 234);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Общие сведения";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.panel4, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(846, 214);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dateTimePickerRegDate);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.numericUpDownNumRooms);
            this.panel3.Controls.Add(this.comboBoxPremisesType);
            this.panel3.Controls.Add(this.textBoxPremisesNumber);
            this.panel3.Controls.Add(this.label27);
            this.panel3.Controls.Add(this.label21);
            this.panel3.Controls.Add(this.numericUpDownNumBeds);
            this.panel3.Controls.Add(this.numericUpDownFloor);
            this.panel3.Controls.Add(this.comboBoxStreet);
            this.panel3.Controls.Add(this.label20);
            this.panel3.Controls.Add(this.comboBoxHouse);
            this.panel3.Controls.Add(this.label19);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(417, 208);
            this.panel3.TabIndex = 1;
            // 
            // dateTimePickerRegDate
            // 
            this.dateTimePickerRegDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerRegDate.Location = new System.Drawing.Point(169, 180);
            this.dateTimePickerRegDate.Name = "dateTimePickerRegDate";
            this.dateTimePickerRegDate.Size = new System.Drawing.Size(242, 21);
            this.dateTimePickerRegDate.TabIndex = 7;
            this.dateTimePickerRegDate.ValueChanged += new System.EventHandler(this.dateTimePickerRegDate_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 183);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Дата регистрации";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Количество комнат";
            // 
            // numericUpDownNumRooms
            // 
            this.numericUpDownNumRooms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownNumRooms.Location = new System.Drawing.Point(169, 123);
            this.numericUpDownNumRooms.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownNumRooms.Name = "numericUpDownNumRooms";
            this.numericUpDownNumRooms.Size = new System.Drawing.Size(242, 21);
            this.numericUpDownNumRooms.TabIndex = 5;
            this.numericUpDownNumRooms.ValueChanged += new System.EventHandler(this.numericUpDownNumRooms_ValueChanged);
            // 
            // comboBoxPremisesType
            // 
            this.comboBoxPremisesType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPremisesType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxPremisesType.FormattingEnabled = true;
            this.comboBoxPremisesType.Items.AddRange(new object[] {
            "Номер квартиры"});
            this.comboBoxPremisesType.Location = new System.Drawing.Point(9, 66);
            this.comboBoxPremisesType.Name = "comboBoxPremisesType";
            this.comboBoxPremisesType.Size = new System.Drawing.Size(154, 23);
            this.comboBoxPremisesType.TabIndex = 2;
            this.comboBoxPremisesType.SelectedIndexChanged += new System.EventHandler(this.comboBoxPremisesType_SelectedIndexChanged);
            // 
            // textBoxPremisesNumber
            // 
            this.textBoxPremisesNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPremisesNumber.Location = new System.Drawing.Point(169, 67);
            this.textBoxPremisesNumber.MaxLength = 25;
            this.textBoxPremisesNumber.Name = "textBoxPremisesNumber";
            this.textBoxPremisesNumber.Size = new System.Drawing.Size(242, 21);
            this.textBoxPremisesNumber.TabIndex = 3;
            this.textBoxPremisesNumber.TextChanged += new System.EventHandler(this.textBoxPremisesNumber_TextChanged);
            this.textBoxPremisesNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPremisesNumber_KeyPress);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(10, 154);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(145, 15);
            this.label27.TabIndex = 4;
            this.label27.Text = "Количество койко-мест";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 98);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(38, 15);
            this.label21.TabIndex = 6;
            this.label21.Text = "Этаж";
            // 
            // numericUpDownNumBeds
            // 
            this.numericUpDownNumBeds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownNumBeds.Location = new System.Drawing.Point(169, 151);
            this.numericUpDownNumBeds.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownNumBeds.Name = "numericUpDownNumBeds";
            this.numericUpDownNumBeds.Size = new System.Drawing.Size(242, 21);
            this.numericUpDownNumBeds.TabIndex = 6;
            this.numericUpDownNumBeds.ValueChanged += new System.EventHandler(this.numericUpDownNumBeds_ValueChanged);
            // 
            // numericUpDownFloor
            // 
            this.numericUpDownFloor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownFloor.Location = new System.Drawing.Point(169, 95);
            this.numericUpDownFloor.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownFloor.Name = "numericUpDownFloor";
            this.numericUpDownFloor.Size = new System.Drawing.Size(242, 21);
            this.numericUpDownFloor.TabIndex = 4;
            this.numericUpDownFloor.ValueChanged += new System.EventHandler(this.numericUpDownFloor_ValueChanged);
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStreet.Location = new System.Drawing.Point(169, 7);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(242, 23);
            this.comboBoxStreet.TabIndex = 0;
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.SelectedValueChanged += new System.EventHandler(this.comboBoxStreet_SelectedValueChanged);
            this.comboBoxStreet.VisibleChanged += new System.EventHandler(this.comboBoxStreet_VisibleChanged);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 41);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(79, 15);
            this.label20.TabIndex = 7;
            this.label20.Text = "Номер дома";
            // 
            // comboBoxHouse
            // 
            this.comboBoxHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxHouse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHouse.Location = new System.Drawing.Point(169, 37);
            this.comboBoxHouse.Name = "comboBoxHouse";
            this.comboBoxHouse.Size = new System.Drawing.Size(242, 23);
            this.comboBoxHouse.TabIndex = 1;
            this.comboBoxHouse.SelectedIndexChanged += new System.EventHandler(this.comboBoxHouse_SelectedIndexChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 11);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(43, 15);
            this.label19.TabIndex = 8;
            this.label19.Text = "Улица";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.checkBoxIsMemorial);
            this.panel4.Controls.Add(this.comboBoxPremisesKind);
            this.panel4.Controls.Add(this.label28);
            this.panel4.Controls.Add(this.label39);
            this.panel4.Controls.Add(this.comboBoxState);
            this.panel4.Controls.Add(this.label38);
            this.panel4.Controls.Add(this.comboBoxCurrentFundType);
            this.panel4.Controls.Add(this.numericUpDownBalanceCost);
            this.panel4.Controls.Add(this.label22);
            this.panel4.Controls.Add(this.textBoxCadastralNum);
            this.panel4.Controls.Add(this.numericUpDownCadastralCost);
            this.panel4.Controls.Add(this.label23);
            this.panel4.Controls.Add(this.label24);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(426, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(417, 208);
            this.panel4.TabIndex = 2;
            // 
            // checkBoxIsMemorial
            // 
            this.checkBoxIsMemorial.AutoSize = true;
            this.checkBoxIsMemorial.Location = new System.Drawing.Point(19, 181);
            this.checkBoxIsMemorial.Name = "checkBoxIsMemorial";
            this.checkBoxIsMemorial.Size = new System.Drawing.Size(141, 19);
            this.checkBoxIsMemorial.TabIndex = 6;
            this.checkBoxIsMemorial.Text = "Памятник культуры";
            this.checkBoxIsMemorial.UseVisualStyleBackColor = true;
            this.checkBoxIsMemorial.CheckedChanged += new System.EventHandler(this.checkBoxIsMemorial_CheckedChanged);
            // 
            // comboBoxPremisesKind
            // 
            this.comboBoxPremisesKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPremisesKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPremisesKind.FormattingEnabled = true;
            this.comboBoxPremisesKind.Location = new System.Drawing.Point(170, 94);
            this.comboBoxPremisesKind.Name = "comboBoxPremisesKind";
            this.comboBoxPremisesKind.Size = new System.Drawing.Size(242, 23);
            this.comboBoxPremisesKind.TabIndex = 3;
            this.comboBoxPremisesKind.SelectedIndexChanged += new System.EventHandler(this.comboBoxPremisesKind_SelectedIndexChanged);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(16, 98);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(99, 15);
            this.label28.TabIndex = 5;
            this.label28.Text = "Вид помещения";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(16, 126);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(119, 15);
            this.label39.TabIndex = 0;
            this.label39.Text = "Текущее состояние";
            // 
            // comboBoxState
            // 
            this.comboBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxState.FormattingEnabled = true;
            this.comboBoxState.Location = new System.Drawing.Point(170, 122);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new System.Drawing.Size(242, 23);
            this.comboBoxState.TabIndex = 4;
            this.comboBoxState.SelectedIndexChanged += new System.EventHandler(this.comboBoxState_SelectedIndexChanged);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(16, 154);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(90, 15);
            this.label38.TabIndex = 2;
            this.label38.Text = "Текущий фонд";
            // 
            // comboBoxCurrentFundType
            // 
            this.comboBoxCurrentFundType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCurrentFundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCurrentFundType.Enabled = false;
            this.comboBoxCurrentFundType.ForeColor = System.Drawing.Color.Black;
            this.comboBoxCurrentFundType.FormattingEnabled = true;
            this.comboBoxCurrentFundType.Location = new System.Drawing.Point(170, 150);
            this.comboBoxCurrentFundType.Name = "comboBoxCurrentFundType";
            this.comboBoxCurrentFundType.Size = new System.Drawing.Size(242, 23);
            this.comboBoxCurrentFundType.TabIndex = 5;
            // 
            // numericUpDownBalanceCost
            // 
            this.numericUpDownBalanceCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownBalanceCost.DecimalPlaces = 2;
            this.numericUpDownBalanceCost.Location = new System.Drawing.Point(170, 67);
            this.numericUpDownBalanceCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownBalanceCost.Name = "numericUpDownBalanceCost";
            this.numericUpDownBalanceCost.Size = new System.Drawing.Size(242, 21);
            this.numericUpDownBalanceCost.TabIndex = 2;
            this.numericUpDownBalanceCost.ThousandsSeparator = true;
            this.numericUpDownBalanceCost.ValueChanged += new System.EventHandler(this.numericUpDownBalanceCost_ValueChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(16, 70);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(143, 15);
            this.label22.TabIndex = 9;
            this.label22.Text = "Балансовая стоимость";
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCadastralNum.Location = new System.Drawing.Point(170, 8);
            this.textBoxCadastralNum.MaxLength = 20;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new System.Drawing.Size(242, 21);
            this.textBoxCadastralNum.TabIndex = 0;
            this.textBoxCadastralNum.TextChanged += new System.EventHandler(this.textBoxCadastralNum_TextChanged);
            // 
            // numericUpDownCadastralCost
            // 
            this.numericUpDownCadastralCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownCadastralCost.DecimalPlaces = 2;
            this.numericUpDownCadastralCost.Location = new System.Drawing.Point(170, 38);
            this.numericUpDownCadastralCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownCadastralCost.Name = "numericUpDownCadastralCost";
            this.numericUpDownCadastralCost.Size = new System.Drawing.Size(242, 21);
            this.numericUpDownCadastralCost.TabIndex = 1;
            this.numericUpDownCadastralCost.ThousandsSeparator = true;
            this.numericUpDownCadastralCost.ValueChanged += new System.EventHandler(this.numericUpDownCadastralCost_ValueChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(16, 11);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(126, 15);
            this.label23.TabIndex = 10;
            this.label23.Text = "Кадастровый номер";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(16, 41);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(150, 15);
            this.label24.TabIndex = 11;
            this.label24.Text = "Кадастровая стоимость";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Controls.Add(this.groupBox11, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 243);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(423, 134);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.numericUpDownHeight);
            this.groupBox11.Controls.Add(this.label3);
            this.groupBox11.Controls.Add(this.numericUpDownMunicipalArea);
            this.groupBox11.Controls.Add(this.label2);
            this.groupBox11.Controls.Add(this.numericUpDownLivingArea);
            this.groupBox11.Controls.Add(this.numericUpDownTotalArea);
            this.groupBox11.Controls.Add(this.label25);
            this.groupBox11.Controls.Add(this.label26);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(0, 0);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(423, 134);
            this.groupBox11.TabIndex = 1;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Геометрия помещения";
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownHeight.DecimalPlaces = 3;
            this.numericUpDownHeight.Location = new System.Drawing.Point(175, 105);
            this.numericUpDownHeight.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(242, 21);
            this.numericUpDownHeight.TabIndex = 3;
            this.numericUpDownHeight.ThousandsSeparator = true;
            this.numericUpDownHeight.ValueChanged += new System.EventHandler(this.numericUpDownHeight_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 15);
            this.label3.TabIndex = 17;
            this.label3.Text = "Высота помещения";
            // 
            // numericUpDownMunicipalArea
            // 
            this.numericUpDownMunicipalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownMunicipalArea.DecimalPlaces = 3;
            this.numericUpDownMunicipalArea.Location = new System.Drawing.Point(175, 76);
            this.numericUpDownMunicipalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownMunicipalArea.Name = "numericUpDownMunicipalArea";
            this.numericUpDownMunicipalArea.ReadOnly = true;
            this.numericUpDownMunicipalArea.Size = new System.Drawing.Size(242, 21);
            this.numericUpDownMunicipalArea.TabIndex = 2;
            this.numericUpDownMunicipalArea.ThousandsSeparator = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 15);
            this.label2.TabIndex = 15;
            this.label2.Text = "Площадь мун. комнат";
            // 
            // numericUpDownLivingArea
            // 
            this.numericUpDownLivingArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownLivingArea.DecimalPlaces = 3;
            this.numericUpDownLivingArea.Location = new System.Drawing.Point(175, 47);
            this.numericUpDownLivingArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            this.numericUpDownLivingArea.Size = new System.Drawing.Size(242, 21);
            this.numericUpDownLivingArea.TabIndex = 1;
            this.numericUpDownLivingArea.ThousandsSeparator = true;
            this.numericUpDownLivingArea.ValueChanged += new System.EventHandler(this.numericUpDownLivingArea_ValueChanged);
            // 
            // numericUpDownTotalArea
            // 
            this.numericUpDownTotalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownTotalArea.DecimalPlaces = 3;
            this.numericUpDownTotalArea.Location = new System.Drawing.Point(175, 18);
            this.numericUpDownTotalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            this.numericUpDownTotalArea.Size = new System.Drawing.Size(242, 21);
            this.numericUpDownTotalArea.TabIndex = 0;
            this.numericUpDownTotalArea.ThousandsSeparator = true;
            this.numericUpDownTotalArea.ValueChanged += new System.EventHandler(this.numericUpDownTotalArea_ValueChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(16, 50);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(100, 15);
            this.label25.TabIndex = 12;
            this.label25.Text = "Жилая площадь";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(16, 21);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(100, 15);
            this.label26.TabIndex = 13;
            this.label26.Text = "Общая площадь";
            // 
            // groupBoxRooms
            // 
            this.groupBoxRooms.Controls.Add(this.dataGridViewRooms);
            this.groupBoxRooms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRooms.Location = new System.Drawing.Point(432, 243);
            this.groupBoxRooms.Name = "groupBoxRooms";
            this.tableLayoutPanel3.SetRowSpan(this.groupBoxRooms, 2);
            this.groupBoxRooms.Size = new System.Drawing.Size(423, 214);
            this.groupBoxRooms.TabIndex = 3;
            this.groupBoxRooms.TabStop = false;
            this.groupBoxRooms.Text = "Комнаты";
            // 
            // dataGridViewRooms
            // 
            this.dataGridViewRooms.AllowUserToAddRows = false;
            this.dataGridViewRooms.AllowUserToDeleteRows = false;
            this.dataGridViewRooms.AllowUserToResizeRows = false;
            this.dataGridViewRooms.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewRooms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRooms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sub_premises_num,
            this.sub_premises_total_area,
            this.sub_premises_id_state});
            this.dataGridViewRooms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRooms.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewRooms.Name = "dataGridViewRooms";
            this.dataGridViewRooms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRooms.Size = new System.Drawing.Size(417, 194);
            this.dataGridViewRooms.TabIndex = 0;
            this.dataGridViewRooms.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRooms_CellDoubleClick);
            this.dataGridViewRooms.Resize += new System.EventHandler(this.dataGridViewRooms_Resize);
            // 
            // sub_premises_num
            // 
            this.sub_premises_num.HeaderText = "Номер";
            this.sub_premises_num.MinimumWidth = 100;
            this.sub_premises_num.Name = "sub_premises_num";
            this.sub_premises_num.ReadOnly = true;
            // 
            // sub_premises_total_area
            // 
            dataGridViewCellStyle1.Format = "#0.0## м²";
            this.sub_premises_total_area.DefaultCellStyle = dataGridViewCellStyle1;
            this.sub_premises_total_area.HeaderText = "Общая площадь";
            this.sub_premises_total_area.MinimumWidth = 125;
            this.sub_premises_total_area.Name = "sub_premises_total_area";
            this.sub_premises_total_area.ReadOnly = true;
            this.sub_premises_total_area.Width = 125;
            // 
            // sub_premises_id_state
            // 
            this.sub_premises_id_state.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.sub_premises_id_state.HeaderText = "Текущее состояние";
            this.sub_premises_id_state.MinimumWidth = 150;
            this.sub_premises_id_state.Name = "sub_premises_id_state";
            this.sub_premises_id_state.ReadOnly = true;
            this.sub_premises_id_state.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.sub_premises_id_state.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.sub_premises_id_state.Width = 150;
            // 
            // PremisesViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(665, 580);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(864, 587);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PremisesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Помещение";
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).EndInit();
            this.groupBox10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumRooms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumBeds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunicipalArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).EndInit();
            this.groupBoxRooms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRooms)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
