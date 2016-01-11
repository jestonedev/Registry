using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels;
using Registry.Entities;
using System.Drawing;
using Security;
using System.Globalization;
using System.Text.RegularExpressions;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Reporting;
using Registry.Viewport.SearchForms;

namespace Registry.Viewport
{
    internal sealed partial class PremisesViewport : FormViewport
    {
        #region Models
        private DataModel buildings = null;
        private DataModel kladr = null;
        private DataModel premises_types = null;
        private DataModel premises_kinds = null;
        private DataModel sub_premises = null;
        private DataModel restrictions = null;
        private DataModel restrictionTypes = null;
        private DataModel restrictionPremisesAssoc = null;
        private DataModel restrictionBuildingsAssoc = null;
        private DataModel ownershipRights = null;
        private DataModel ownershipRightTypes = null;
        private DataModel ownershipPremisesAssoc = null;
        private DataModel ownershipBuildingsAssoc = null;
        private DataModel fundTypes = null;
        private DataModel object_states = null;
        private CalcDataModel premisesCurrentFund = null;
        private CalcDataModel premiseSubPremisesSumArea = null;
        private CalcDataModel subPremisesCurrentFund = null;
        #endregion Models

        #region Views
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
        private BindingSource v_subPremisesCurrentFund = null;
        #endregion Views

        //Forms
        private SearchForm spExtendedSearchForm = null;
        private SearchForm spSimpleSearchForm = null;

        private bool is_first_visibility = true;

        private PremisesViewport()
            : this(null, null)
        {
        }

        public PremisesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
        }

        private void RestrictionsFilterRebuild()
        {
            string restrictionsFilter = "id_restriction IN (0";
            for (int i = 0; i < v_restrictionPremisesAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)v_restrictionPremisesAssoc[i])["id_restriction"].ToString() + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(new char[] { ',' });
            restrictionsFilter += ")";
            if (GeneralBindingSource.Position > -1 && v_restrictionBuildingsAssoc != null && ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] != DBNull.Value)
            {
                v_restrictionBuildingsAssoc.Filter = "id_building = " + ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"].ToString();
                restrictionsFilter += " OR id_restriction IN (0";
                for (int i = 0; i < v_restrictionBuildingsAssoc.Count; i++)
                    restrictionsFilter += ((DataRowView)v_restrictionBuildingsAssoc[i])["id_restriction"].ToString() + ",";
                restrictionsFilter = restrictionsFilter.TrimEnd(new char[] { ',' });
                restrictionsFilter += ")";
            }
            v_restrictions.Filter = restrictionsFilter;
            if (dataGridViewRestrictions.Columns.Contains("id_restriction"))
                dataGridViewRestrictions.Columns["id_restriction"].Visible = false;
            RedrawRestrictionDataGridRows();
        }

        private void OwnershipsFilterRebuild()
        {
            string ownershipFilter = "id_ownership_right IN (0";
            for (int i = 0; i < v_ownershipPremisesAssoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownershipPremisesAssoc[i])["id_ownership_right"].ToString() + ",";
            ownershipFilter = ownershipFilter.TrimEnd(new char[] { ',' });
            ownershipFilter += ")";
            if (GeneralBindingSource.Position > -1 && v_ownershipBuildingsAssoc != null && ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] != DBNull.Value)
            {
                v_ownershipBuildingsAssoc.Filter = "id_building = " + ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"].ToString();
                ownershipFilter += " OR id_ownership_right IN (0";
                for (int i = 0; i < v_ownershipBuildingsAssoc.Count; i++)
                    ownershipFilter += ((DataRowView)v_ownershipBuildingsAssoc[i])["id_ownership_right"].ToString() + ",";
                ownershipFilter = ownershipFilter.TrimEnd(new char[] { ',' });
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
                int position = -1;
                if ((GeneralBindingSource.Position != -1) && !(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] is DBNull))
                    position = 
                        v_premisesCurrentFund.Find("id_premises", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"]);
                if (position != -1)
                    comboBoxCurrentFundType.SelectedValue = ((DataRowView)v_premisesCurrentFund[position])["id_fund_type"];
                else
                    comboBoxCurrentFundType.SelectedValue = DBNull.Value;
                ShowOrHideCurrentFund();
            }
            if (v_premisesSubPremisesSumArea != null)
            {
                int position = -1;
                if ((GeneralBindingSource.Position != -1) && !(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] is DBNull))
                    position = v_premisesSubPremisesSumArea.Find("id_premises", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"]);
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

        private void RedrawRestrictionDataGridRows()
        {
            for (int i = 0; i < dataGridViewRestrictions.Rows.Count; i++)
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
            for (int i = 0; i < dataGridViewOwnerships.Rows.Count; i++)
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
            for (int i = 0; i < v_sub_premises.Count; i++)
            {
                int id_sub_premises = (int)((DataRowView)v_sub_premises[i])["id_sub_premises"];
                int id = v_subPremisesCurrentFund.Find("id_sub_premises", id_sub_premises);
                if (id == -1)
                    continue;
                int id_fund_type = (int)((DataRowView)v_subPremisesCurrentFund[id])["id_fund_type"];
                string fundType = ((DataRowView)v_fundType[v_fundType.Find("id_fund_type", id_fund_type)])["fund_type"].ToString();
                if ((new int[] {1, 4, 5, 9}).Contains((int)((DataRowView)v_sub_premises[i])["id_state"]))
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
                    Text = @"Новое помещение";
            }
            else
                if (GeneralBindingSource.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                    {
                        Text = string.Format(CultureInfo.InvariantCulture, "Помещение №{0} здания №{1}",
                            ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_premises"],
                            ParentRow["id_building"]);
                    } else
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.PaymentAccount))
                    {
                        Text = string.Format("Помещения по лицевому счету №{0}", ParentRow["account"]);
                    }
                    else
                        Text = string.Format(CultureInfo.InvariantCulture, "Помещение №{0}", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"]);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                    {
                        Text = string.Format(CultureInfo.InvariantCulture, "Помещения в здании №{0} отсутствуют",
                            ParentRow["id_building"]);
                    } else
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.PaymentAccount))
                    {
                        Text = string.Format("Помещения по лицевому счету №{0} отсутствуют", ParentRow["account"]);
                    }
                    else
                        Text = @"Помещения отсутствуют";
                }
        }

        private void ShowOrHideCurrentFund()
        {
            if (comboBoxCurrentFundType.SelectedValue != null && GeneralBindingSource.Position != -1 &&
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state"] != DBNull.Value &&
                (new int[] { 1, 4, 5, 9 }).Contains((int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state"]))
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
                if ((GeneralBindingSource.Position != -1) && (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] != DBNull.Value))
                    id_building = Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"], CultureInfo.InvariantCulture);
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
                CheckViewportModifications();
            }
        }

        private void UnbindedCheckBoxesUpdate()
        {
            if (GeneralBindingSource.Count == 0) return;
            var row = GeneralBindingSource.Position >= 0 ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["state_date"] != DBNull.Value)))
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
            comboBoxPremisesKind.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_premises_kind", true, DataSourceUpdateMode.Never, 1);
            comboBoxPremisesType.DataSource = v_premises_types;
            comboBoxPremisesType.ValueMember = "id_premises_type";
            comboBoxPremisesType.DisplayMember = "premises_type_as_num";
            comboBoxPremisesType.DataBindings.Clear();
            comboBoxPremisesType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_premises_type", true, DataSourceUpdateMode.Never, 1);
            textBoxPremisesNumber.DataBindings.Clear();
            textBoxPremisesNumber.DataBindings.Add("Text", GeneralBindingSource, "premises_num", true, DataSourceUpdateMode.Never, "");
            numericUpDownFloor.DataBindings.Clear();
            numericUpDownFloor.DataBindings.Add("Value", GeneralBindingSource, "floor", true, DataSourceUpdateMode.Never, 0);
            textBoxCadastralNum.DataBindings.Clear();
            textBoxCadastralNum.DataBindings.Add("Text", GeneralBindingSource, "cadastral_num", true, DataSourceUpdateMode.Never, "");
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            textBoxAccount.DataBindings.Clear();
            textBoxAccount.DataBindings.Add("Text", GeneralBindingSource, "account", true, DataSourceUpdateMode.Never, "");
            numericUpDownCadastralCost.DataBindings.Clear();
            numericUpDownCadastralCost.DataBindings.Add("Value", GeneralBindingSource, "cadastral_cost", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceCost.DataBindings.Clear();
            numericUpDownBalanceCost.DataBindings.Add("Value", GeneralBindingSource, "balance_cost", true, DataSourceUpdateMode.Never, 0);
            numericUpDownNumRooms.DataBindings.Clear();
            numericUpDownNumRooms.DataBindings.Add("Value", GeneralBindingSource, "num_rooms", true, DataSourceUpdateMode.Never, 0);
            numericUpDownNumBeds.DataBindings.Clear();
            numericUpDownNumBeds.DataBindings.Add("Value", GeneralBindingSource, "num_beds", true, DataSourceUpdateMode.Never, 0);
            numericUpDownTotalArea.DataBindings.Clear();
            numericUpDownTotalArea.DataBindings.Add("Value", GeneralBindingSource, "total_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownLivingArea.DataBindings.Clear();
            numericUpDownLivingArea.DataBindings.Add("Value", GeneralBindingSource, "living_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownHeight.DataBindings.Clear();
            numericUpDownHeight.DataBindings.Add("Value", GeneralBindingSource, "height", true, DataSourceUpdateMode.Never, 0);

            comboBoxCurrentFundType.DataSource = v_fundType;
            comboBoxCurrentFundType.ValueMember = "id_fund_type";
            comboBoxCurrentFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = v_object_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";
            comboBoxState.DataBindings.Clear();
            comboBoxState.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_state", true, DataSourceUpdateMode.Never, DBNull.Value);

            checkBoxIsMemorial.DataBindings.Clear();
            checkBoxIsMemorial.DataBindings.Add("Checked", GeneralBindingSource, "is_memorial", true, DataSourceUpdateMode.Never, true);

            dateTimePickerRegDate.DataBindings.Clear();
            dateTimePickerRegDate.DataBindings.Add("Value", GeneralBindingSource, "reg_date", true, DataSourceUpdateMode.Never, null);

            dateTimePickerStateDate.DataBindings.Clear();
            dateTimePickerStateDate.DataBindings.Add("Value", GeneralBindingSource, "state_date", true, DataSourceUpdateMode.Never, null);

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

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) ||
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)))
                return base.ChangeViewportStateTo(state);
            viewportState = ViewportState.ReadState;
            return true;
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
            if (!Regex.IsMatch(premise.PremisesNum, @"^[0-9]+[а-я]{0,1}([/]([а-я]{0,1}|[0-9]+[а-я]{0,1}))?([,-][0-9]+[а-я]{0,1}([/]([а-я]{0,1}|[0-9]+[а-я]{0,1}))?)*$"))
            {
                MessageBox.Show("Некорректно задан номер помещения. Можно использовать только цифры и не более одной строчной буквы кирилицы, а также знак дроби /. Для объединенных квартир номера должны быть перечислены через запятую или тире. Например: \"1а,2а,3б/4\"", "Ошибка",
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
            var premiseFromView = (Premise)EntityFromView();
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
            if (new int[] { 4, 5, 9 }.Contains(premise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу муниципальных жилых помещений", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new int[] { 1, 3, 6, 7, 8 }.Contains(premise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
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

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
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

        protected override Entity EntityFromViewport()
        {
            var premise = new Premise
            {
                IdPremises = GeneralBindingSource.Position == -1 ? null : ViewportHelper.ValueOrNull<int>(
                    (DataRowView) GeneralBindingSource[GeneralBindingSource.Position], "id_premises"),
                IdBuilding = ViewportHelper.ValueOrNull<int>(comboBoxHouse),
                IdState = ViewportHelper.ValueOrNull<int>(comboBoxState),
                PremisesNum = ViewportHelper.ValueOrNull(textBoxPremisesNumber),
                TotalArea = Convert.ToDouble(numericUpDownTotalArea.Value),
                LivingArea = Convert.ToDouble(numericUpDownLivingArea.Value),
                Height = Convert.ToDouble(numericUpDownHeight.Value),
                NumRooms = Convert.ToInt16(numericUpDownNumRooms.Value),
                NumBeds = Convert.ToInt16(numericUpDownNumBeds.Value),
                IdPremisesType = ViewportHelper.ValueOrNull<int>(comboBoxPremisesType),
                IdPremisesKind = ViewportHelper.ValueOrNull<int>(comboBoxPremisesKind)
            };
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridViewOwnerships.AutoGenerateColumns = false;
            dataGridViewRestrictions.AutoGenerateColumns = false;
            dataGridViewRooms.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;

            GeneralDataModel = DataModel.GetInstance(DataModelType.PremisesDataModel);
            kladr = DataModel.GetInstance(DataModelType.KladrStreetsDataModel);
            buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel);
            premises_types = DataModel.GetInstance(DataModelType.PremisesTypesDataModel);
            premises_kinds = DataModel.GetInstance(DataModelType.PremisesKindsDataModel);
            sub_premises = DataModel.GetInstance(DataModelType.SubPremisesDataModel);
            restrictions = DataModel.GetInstance(DataModelType.RestrictionsDataModel);
            restrictionTypes = DataModel.GetInstance(DataModelType.RestrictionTypesDataModel);
            restrictionPremisesAssoc = DataModel.GetInstance(DataModelType.RestrictionsPremisesAssocDataModel);
            restrictionBuildingsAssoc = DataModel.GetInstance(DataModelType.RestrictionsBuildingsAssocDataModel);
            ownershipRights = DataModel.GetInstance(DataModelType.OwnershipsRightsDataModel);
            ownershipRightTypes = DataModel.GetInstance(DataModelType.OwnershipRightTypesDataModel);
            ownershipPremisesAssoc = DataModel.GetInstance(DataModelType.OwnershipPremisesAssocDataModel);
            ownershipBuildingsAssoc = DataModel.GetInstance(DataModelType.OwnershipBuildingsAssocDataModel);
            fundTypes = DataModel.GetInstance(DataModelType.FundTypesDataModel);
            object_states = DataModel.GetInstance(DataModelType.ObjectStatesDataModel); 

            // Вычисляемые модели
            premisesCurrentFund = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelPremisesCurrentFunds);
            premiseSubPremisesSumArea = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelPremiseSubPremisesSumArea);
            subPremisesCurrentFund = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelSubPremisesCurrentFunds);

            // Ожидаем дозагрузки, если это необходмо
            GeneralDataModel.Select();
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

            var ds = DataModel.DataSet;

            v_kladr = new BindingSource
            {
                DataMember = "kladr",
                DataSource = ds
            };

            v_buildings = new BindingSource
            {
                DataMember = "kladr_buildings",
                DataSource = v_kladr
            };

            v_premisesCurrentFund = new BindingSource
            {
                DataMember = "premises_current_funds",
                DataSource = premisesCurrentFund.Select()
            };

            v_premisesSubPremisesSumArea = new BindingSource
            {
                DataMember = "premise_sub_premises_sum_area",
                DataSource = premiseSubPremisesSumArea.Select()
            };

            v_subPremisesCurrentFund = new BindingSource
            {
                DataMember = "sub_premises_current_funds",
                DataSource = subPremisesCurrentFund.Select()
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

            v_sub_premises_object_states = new BindingSource();
            v_sub_premises_object_states.DataMember = "object_states";
            v_sub_premises_object_states.DataSource = ds;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += v_premises_CurrentItemChanged;
            GeneralBindingSource.DataMember = "premises";
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = ds;
            GeneralDataModel.Select().RowChanged += PremisesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += PremisesViewport_RowDeleted;

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
            v_sub_premises.DataSource = GeneralBindingSource;
            v_sub_premises.CurrentItemChanged += v_sub_premises_CurrentItemChanged;

            v_restrictionPremisesAssoc = new BindingSource();
            v_restrictionPremisesAssoc.DataMember = "premises_restrictions_premises_assoc";
            v_restrictionPremisesAssoc.CurrentItemChanged += v_restrictionPremisesAssoc_CurrentItemChanged;
            v_restrictionPremisesAssoc.DataSource = GeneralBindingSource;

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
            v_ownershipPremisesAssoc.DataSource = GeneralBindingSource;

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
            DataChangeHandlersInit();
        }
        
        public override bool CanCopyRecord()
        {
            return ((GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var premise = (Premise) EntityFromView();
            GeneralBindingSource.AddNew();
            GeneralDataModel.EditingNewRecord = true;
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

        public override bool CanSearchRecord()
        {
            return true;
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
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            is_editable = false;
            GeneralBindingSource.Filter = Filter;
            is_editable = true;
        }

        public override void ClearSearch()
        {
            is_editable = false;
            GeneralBindingSource.Filter = StaticFilter;
            is_editable = true;
            DynamicFilter = "";
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (viewportState != ViewportState.NewRowState) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это помещение?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show("У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_premises"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            var premise = (Premise)EntityFromViewport();
            if (!ValidatePremise(premise))
                return;
            string Filter = "";
            if (!string.IsNullOrEmpty(GeneralBindingSource.Filter))
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
                    int id_premise = GeneralDataModel.Insert(premise);
                    if (id_premise == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    premise.IdPremises = id_premise;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_premises = {0})", premise.IdPremises);
                    GeneralBindingSource.Filter += Filter;
                    FillRowFromPremise(premise, newRow);
                    viewportState = ViewportState.ReadState;
                    is_editable = true;
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (premise.IdPremises == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить помещение без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(premise) == -1)
                        return;
                    DataRowView row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    is_editable = false;
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_premises = {0})", premise.IdPremises);
                    GeneralBindingSource.Filter += Filter;
                    FillRowFromPremise(premise, row);
                    viewportState = ViewportState.ReadState;
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
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        is_editable = false;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
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
                v_sub_premises.CurrentItemChanged -= v_sub_premises_CurrentItemChanged;
                v_restrictionPremisesAssoc.CurrentItemChanged -= v_restrictionPremisesAssoc_CurrentItemChanged;
                v_restrictionBuildingsAssoc.CurrentItemChanged -= v_restrictionBuildingsAssoc_CurrentItemChanged;
                v_ownershipPremisesAssoc.CurrentItemChanged -= v_ownershipPremisesAssoc_CurrentItemChanged;
                v_ownershipBuildingsAssoc.CurrentItemChanged -= v_ownershipBuildingsAssoc_CurrentItemChanged;
                GeneralDataModel.Select().RowChanged -= PremisesViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= PremisesViewport_RowDeleted;
                GeneralBindingSource.CurrentItemChanged -= v_premises_CurrentItemChanged;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            Close();
        }

        public override bool HasAssocViewport(ViewportType viewportType)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.SubPremisesViewport,
                ViewportType.OwnershipListViewport,
                ViewportType.RestrictionListViewport,
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport,
                ViewportType.PaymentsAccountsViewport
            };
            return reports.Contains(viewportType) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_premises = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Premises);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            switch (reporterType)
            {
                case ReporterType.RegistryExcerptReporterPremise:
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    return (GeneralBindingSource.Position > -1);
                case ReporterType.RegistryExcerptReporterSubPremise:
                    return (v_sub_premises.Position > -1);
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var reporter = ReporterFactory.CreateReporter(reporterType);
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.RegistryExcerptReporterPremise:
                    arguments = RegistryExcerptPremiseReportArguments();
                    break;
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    arguments = RegistryExcerptReporterAllMunSubPremisesArguments();
                    break;
                case ReporterType.RegistryExcerptReporterSubPremise:
                    arguments = RegistryExcerptReporterSubPremiseArguments();
                    break;
            }
            reporter.Run(arguments);
        }

        private Dictionary<string, string> RegistryExcerptPremiseReportArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_premises"].ToString()},
                {"excerpt_type", "1"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterSubPremiseArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", ((DataRowView) v_sub_premises[v_sub_premises.Position])["id_sub_premises"].ToString()},
                {"excerpt_type", "2"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterAllMunSubPremisesArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_premises"].ToString()},
                {"excerpt_type", "3"}
            };
            return arguments;
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
            if (GeneralBindingSource.Position == -1)
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
            if (HasAssocViewport(ViewportType.RestrictionListViewport))
                ShowAssocViewport(ViewportType.RestrictionListViewport);
        }

        private void dataGridViewOwnerships_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocViewport(ViewportType.OwnershipListViewport))
                ShowAssocViewport(ViewportType.OwnershipListViewport);
        }

        private void dataGridViewRooms_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocViewport(ViewportType.SubPremisesViewport))
                ShowAssocViewport(ViewportType.SubPremisesViewport);
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
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (RestrictionsEditor editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
                editor.ShowDialog();
            }
        }

        private void vButtonRestrictionEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
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
            Restriction restriction = new Restriction();
            DataRowView row = (DataRowView)v_restrictions[v_restrictions.Position];
            restriction.IdRestriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction");
            restriction.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
            restriction.Number = row["number"].ToString();
            restriction.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
            restriction.Description = row["description"].ToString();
            using (RestrictionsEditor editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                if (v_restrictionBuildingsAssoc.Find("id_restriction", restriction.IdRestriction) != -1)
                {
                    editor.ParentType = ParentTypeEnum.Building;
                    editor.ParentRow = buildings.Select().Rows.Find(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                }
                else
                {
                    editor.ParentType = ParentTypeEnum.Premises;
                    editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
                }
                editor.RestrictionValue = restriction;
                editor.ShowDialog();
            }
        }

        private void vButtonRestrictionDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
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

        private void vButtonOwnershipAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (OwnershipsEditor editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Premises;
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
            int idOwnershipRight = (int)((DataRowView)v_ownershipRights[v_ownershipRights.Position])["id_ownership_right"];
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
            OwnershipRight ownershipRight = new OwnershipRight();
            DataRowView row = (DataRowView)v_ownershipRights[v_ownershipRights.Position];
            ownershipRight.IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right");
            ownershipRight.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
            ownershipRight.Number = row["number"].ToString();
            ownershipRight.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
            ownershipRight.Description = row["description"].ToString();
            using (OwnershipsEditor editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                if (v_ownershipBuildingsAssoc.Find("id_ownership_right", ownershipRight.IdOwnershipRight) != -1)
                {
                    editor.ParentType = ParentTypeEnum.Building;
                    editor.ParentRow = buildings.Select().Rows.Find(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
                }
                else
                {
                    editor.ParentType = ParentTypeEnum.Premises;
                    editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
                }
                editor.OwnershipRightValue = ownershipRight;
                editor.ShowDialog();
            }
        }

        private void vButtonSubPremisesAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (SubPremisesEditor editor = new SubPremisesEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
                editor.ShowDialog();
            }
        }

        private void vButtonSubPremisesDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
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
            int idSubPremise = (int)((DataRowView)v_sub_premises[v_sub_premises.Position])["id_sub_premises"];
            if (sub_premises.Delete(idSubPremise) == -1)
                return;
            sub_premises.Select().Rows.Find(idSubPremise).Delete();
        }

        private void vButtonSubPremisesEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
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
            SubPremise subPremise = new SubPremise();
            DataRowView row = (DataRowView)v_sub_premises[v_sub_premises.Position];
            subPremise.IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
            subPremise.IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises");
            subPremise.IdState = ViewportHelper.ValueOrNull<int>(row, "id_state");
            subPremise.SubPremisesNum = row["sub_premises_num"].ToString();
            subPremise.TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area");
            subPremise.LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area");
            subPremise.Description = row["description"].ToString();
            subPremise.StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date");
            using (SubPremisesEditor editor = new SubPremisesEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;             
                editor.SubPremiseValue = subPremise;
                editor.ShowDialog();
            }
        }

        internal int GetCurrentId()
        {
            if (GeneralBindingSource.Position < 0) return -1;
            if (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"] != DBNull.Value)
                return (int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_premises"];
            return -1;
        }

        internal string GetFilter()
        {
            return GeneralBindingSource.Filter;
        }
    }
}
