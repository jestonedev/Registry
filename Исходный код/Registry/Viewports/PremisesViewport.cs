using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using Registry.Viewport.EntityConverters;
using Registry.Viewport.SearchForms;

namespace Registry.Viewport
{
    internal sealed partial class PremisesViewport : FormViewport
    {
        #region Models
        private DataModel _buildings;
        private DataModel _kladr;
        private DataModel _premisesTypes;
        private DataModel _premisesKinds;
        private DataModel _subPremises;
        private DataModel _restrictions;
        private DataModel _restrictionTypes;
        private DataModel _restrictionPremisesAssoc;
        private DataModel _restrictionBuildingsAssoc;
        private DataModel _ownershipRights;
        private DataModel _ownershipRightTypes;
        private DataModel _ownershipPremisesAssoc;
        private DataModel _ownershipBuildingsAssoc;
        private DataModel _fundTypes;
        private DataModel _objectStates;
        private CalcDataModel _premisesCurrentFund;
        private CalcDataModel _premiseSubPremisesSumArea;
        private CalcDataModel _subPremisesCurrentFund;

        #endregion Models

        #region Views
        private BindingSource _vPremisesCurrentFund;
        private BindingSource _vBuildings;
        private BindingSource _vKladr;
        private BindingSource _vPremisesTypes;
        private BindingSource _vPremisesKinds;
        private BindingSource _vSubPremises;
        private BindingSource _vRestrictions;
        private BindingSource _vRestrictonTypes;
        private BindingSource _vRestrictionPremisesAssoc;
        private BindingSource _vRestrictionBuildingsAssoc;
        private BindingSource _vOwnershipRights;
        private BindingSource _vOwnershipRightTypes;
        private BindingSource _vOwnershipPremisesAssoc;
        private BindingSource _vOwnershipBuildingsAssoc;
        private BindingSource _vFundType;
        private BindingSource _vObjectStates;
        private BindingSource _vSubPremisesObjectStates;
        private BindingSource _vPremisesSubPremisesSumArea;
        private BindingSource _vSubPremisesCurrentFund;
        #endregion Views

        //Forms
        private SearchForm _spExtendedSearchForm;
        private SearchForm _spSimpleSearchForm;

        private bool _isFirstVisibility = true;

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
            var restrictionsFilter = "id_restriction IN (0";
            for (var i = 0; i < _vRestrictionPremisesAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)_vRestrictionPremisesAssoc[i])["id_restriction"] + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(',');
            restrictionsFilter += ")";
            if (GeneralBindingSource.Position > -1 && _vRestrictionBuildingsAssoc != null && ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] != DBNull.Value)
            {
                _vRestrictionBuildingsAssoc.Filter = "id_building = " + ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"];
                restrictionsFilter += " OR id_restriction IN (0";
                for (var i = 0; i < _vRestrictionBuildingsAssoc.Count; i++)
                    restrictionsFilter += ((DataRowView)_vRestrictionBuildingsAssoc[i])["id_restriction"] + ",";
                restrictionsFilter = restrictionsFilter.TrimEnd(',');
                restrictionsFilter += ")";
            }
            _vRestrictions.Filter = restrictionsFilter;
            if (dataGridViewRestrictions.Columns.Contains("id_restriction"))
                id_restriction.Visible = false;
            RedrawRestrictionDataGridRows();
        }

        private void OwnershipsFilterRebuild()
        {
            var ownershipFilter = "id_ownership_right IN (0";
            for (var i = 0; i < _vOwnershipPremisesAssoc.Count; i++)
                ownershipFilter += ((DataRowView)_vOwnershipPremisesAssoc[i])["id_ownership_right"] + ",";
            ownershipFilter = ownershipFilter.TrimEnd(',');
            ownershipFilter += ")";
            if (GeneralBindingSource.Position > -1 && _vOwnershipBuildingsAssoc != null && ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] != DBNull.Value)
            {
                _vOwnershipBuildingsAssoc.Filter = "id_building = " + ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"];
                ownershipFilter += " OR id_ownership_right IN (0";
                for (var i = 0; i < _vOwnershipBuildingsAssoc.Count; i++)
                    ownershipFilter += ((DataRowView)_vOwnershipBuildingsAssoc[i])["id_ownership_right"] + ",";
                ownershipFilter = ownershipFilter.TrimEnd(',');
                ownershipFilter += ")";
            }
            _vOwnershipRights.Filter = ownershipFilter;
            
            if (dataGridViewOwnerships.Columns.Contains("id_ownership_right"))
                id_ownership_right.Visible = false;
            RedrawOwnershipDataGridRows();
        }

        private void FiltersRebuild()
        {
            if (GeneralBindingSource.Position == -1)
            {
                ClearPremiseCalcInfo();
                return;
            }
            var row = (DataRowView) GeneralBindingSource[GeneralBindingSource.Position];
            if (row["id_premises"] == DBNull.Value)
            {
                ClearPremiseCalcInfo();
                return;
            }
            if (_vPremisesCurrentFund != null)
            {
                var position = _vPremisesCurrentFund.Find("id_premises", row["id_premises"]);
                comboBoxCurrentFundType.SelectedValue = position != -1 ? 
                    ((DataRowView)_vPremisesCurrentFund[position])["id_fund_type"] : DBNull.Value;
                ShowOrHideCurrentFund();
            }
            if (_vPremisesSubPremisesSumArea != null)
            {
                var position = _vPremisesSubPremisesSumArea.Find("id_premises", row["id_premises"]);
                if (position != -1)
                {
                    var value = Convert.ToDecimal((double)((DataRowView)_vPremisesSubPremisesSumArea[position])["sum_area"]);
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

        private void ClearPremiseCalcInfo()
        {
            comboBoxCurrentFundType.SelectedValue = DBNull.Value;
            numericUpDownMunicipalArea.Minimum = 0;
            numericUpDownMunicipalArea.Maximum = 0;
            numericUpDownMunicipalArea.Value = 0;
        }

        private void RedrawRestrictionDataGridRows()
        {
            for (var i = 0; i < dataGridViewRestrictions.Rows.Count; i++)
                if (_vRestrictionBuildingsAssoc.Find("id_restriction", dataGridViewRestrictions.Rows[i].Cells["id_restriction"].Value) != -1)
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
                if (_vOwnershipBuildingsAssoc.Find("id_ownership_right", dataGridViewOwnerships.Rows[i].Cells["id_ownership_right"].Value) != -1)
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
            if (_vSubPremises == null)
                return;
            if (_vSubPremises.Count != dataGridViewRooms.Rows.Count)
                return;
            for (var i = 0; i < _vSubPremises.Count; i++)
            {
                var idSubPremises = (int)((DataRowView)_vSubPremises[i])["id_sub_premises"];
                var id = _vSubPremisesCurrentFund.Find("id_sub_premises", idSubPremises);
                if (id == -1)
                    continue;
                var idFundType = (int)((DataRowView)_vSubPremisesCurrentFund[id])["id_fund_type"];
                var fundType = ((DataRowView)_vFundType[_vFundType.Find("id_fund_type", idFundType)])["fund_type"].ToString();
                if (DataModelHelper.MunicipalAndUnknownObjectStates().Contains((int)((DataRowView)_vSubPremises[i])["id_state"]))
                    dataGridViewRooms.Rows[i].Cells["current_fund"].Value = fundType;
                else
                    dataGridViewRooms.Rows[i].Cells["current_fund"].Value = "";
            }
        }

        private void SetViewportCaption()
        {
            if (ViewportState == ViewportState.NewRowState)
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
                DataModelHelper.MunicipalAndUnknownObjectStates().Contains((int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state"]))
            {
                label38.Visible = true;
                comboBoxCurrentFundType.Visible = true;
                checkBoxIsMemorial.Location = new Point(19, 181);
            }
            else
            {
                label38.Visible = false;
                comboBoxCurrentFundType.Visible = false;
                checkBoxIsMemorial.Location = new Point(19, 153);
            }
        }

        private void SelectCurrentBuilding()
        {
            if ((comboBoxHouse.DataSource != null) && (comboBoxStreet.DataSource != null))
            {
                int? idBuilding = null;
                if ((GeneralBindingSource.Position != -1) && (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] != DBNull.Value))
                    idBuilding = Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"], CultureInfo.InvariantCulture);
                else 
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                    idBuilding = Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture);
                string idStreet = null;
                if (idBuilding != null)
                {
                    var buildingRow = _buildings.Select().Rows.Find(idBuilding);
                    if (buildingRow != null)
                        idStreet = buildingRow["id_street"].ToString();
                }
                _vKladr.Filter = "";
                if (idStreet != null)
                    comboBoxStreet.SelectedValue = idStreet;
                else
                    comboBoxStreet.SelectedValue = DBNull.Value;
                if (idBuilding != null)
                    comboBoxHouse.SelectedValue = idBuilding;
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
            comboBoxStreet.DataSource = _vKladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";
            comboBoxHouse.DataSource = _vBuildings;
            _vBuildings.Sort = "id_building DESC";
            comboBoxHouse.ValueMember = "id_building";
            comboBoxHouse.DisplayMember = "house";
            comboBoxPremisesKind.DataSource = _vPremisesKinds;
            comboBoxPremisesKind.ValueMember = "id_premises_kind";
            comboBoxPremisesKind.DisplayMember = "premises_kind";
            comboBoxPremisesKind.DataBindings.Clear();
            comboBoxPremisesKind.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_premises_kind", true, DataSourceUpdateMode.Never, 1);
            comboBoxPremisesType.DataSource = _vPremisesTypes;
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

            comboBoxCurrentFundType.DataSource = _vFundType;
            comboBoxCurrentFundType.ValueMember = "id_fund_type";
            comboBoxCurrentFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = _vObjectStates;
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

            dataGridViewRestrictions.DataSource = _vRestrictions;
            id_restriction.DataPropertyName = "id_restriction";
            id_restriction_type.DataSource = _vRestrictonTypes;
            id_restriction_type.DataPropertyName = "id_restriction_type";
            id_restriction_type.ValueMember = "id_restriction_type";
            id_restriction_type.DisplayMember = "restriction_type";
            restriction_number.DataPropertyName = "number";
            restriction_date.DataPropertyName = "date";
            restriction_description.DataPropertyName = "description";

            dataGridViewOwnerships.DataSource = _vOwnershipRights;
            id_ownership_right.DataPropertyName = "id_ownership_right";
            id_ownership_type.DataSource = _vOwnershipRightTypes;
            id_ownership_type.DataPropertyName = "id_ownership_right_type";
            id_ownership_type.ValueMember = "id_ownership_right_type";
            id_ownership_type.DisplayMember = "ownership_right_type";
            ownership_number.DataPropertyName = "number";
            ownership_date.DataPropertyName = "date";
            ownership_description.DataPropertyName = "description";

            dataGridViewRooms.DataSource = _vSubPremises;
            sub_premises_num.DataPropertyName = "sub_premises_num";
            sub_premises_total_area.DataPropertyName = "total_area";
            sub_premises_id_state.DataSource = _vSubPremisesObjectStates;
            sub_premises_id_state.DataPropertyName = "id_state";
            sub_premises_id_state.ValueMember = "id_state";
            sub_premises_id_state.DisplayMember = "state_female";
            sub_premises_cadastral_num.DataPropertyName = "cadastral_num";
            sub_premises_cadastral_cost.DataPropertyName = "cadastral_cost";
            sub_premises_balance_cost.DataPropertyName = "balance_cost";
            sub_premises_account.DataPropertyName = "account";
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) ||
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)))
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidatePremise(Premise premise)
        {
            if (premise.IdBuilding == null)
            {
                MessageBox.Show(@"Необходимо выбрать здание",@"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxHouse.Focus();
                return false;
            }
            if (premise.PremisesNum == null || string.IsNullOrEmpty(premise.PremisesNum.Trim()))
            {
                MessageBox.Show(@"Необходимо указать номер помещения", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNumber.Focus();
                return false;
            }
            if (!Regex.IsMatch(premise.PremisesNum, @"^[0-9]+[а-я]{0,1}([/]([а-я]{0,1}|[0-9]+[а-я]{0,1}))?([,-][0-9]+[а-я]{0,1}([/]([а-я]{0,1}|[0-9]+[а-я]{0,1}))?)*$"))
            {
                MessageBox.Show(@"Некорректно задан номер помещения. Можно использовать только цифры и не более одной строчной буквы кирилицы, а также знак дроби /. Для объединенных квартир номера должны быть перечислены через запятую или тире. Например: ""1а,2а,3б/4""", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNumber.Focus();
                return false;
            }
            if (premise.IdState == null)
            {
                MessageBox.Show(@"Необходимо выбрать текущее состояние помещения", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxState.Focus();
                return false;
            }
            // Проверяем права на модификацию муниципального или не муниципального здания
            var premiseFromView = (Premise)EntityFromView();
            if (premiseFromView.IdPremises != null && DataModelHelper.HasMunicipal(premiseFromView.IdPremises.Value, EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"Вы не можете изменить информацию по данному помещению, т.к. оно является муниципальным или содержит в себе муниципальные комнаты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (premiseFromView.IdPremises != null && DataModelHelper.HasNotMunicipal(premiseFromView.IdPremises.Value, EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"Вы не можете изменить информацию по данному помещения, т.к. оно является немуниципальным или содержит в себе немуниципальные комнаты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.MunicipalObjectStates().Contains(premise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на добавление в базу муниципальных жилых помещений", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.NonMunicipalAndUnknownObjectStates().Contains(premise.IdState.Value) && 
                !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на добавление в базу немуниципальных жилых помещений", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            // Проверяем дубликаты квартир
            if ((premise.PremisesNum != premiseFromView.PremisesNum) || (premise.IdBuilding != premiseFromView.IdBuilding))
                if (DataModelHelper.PremisesDuplicateCount(premise) != 0 &&
                    MessageBox.Show(@"В указанном доме уже есть квартира с таким номером. Все равно продолжить сохранение?", @"Внимание", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
            return true;
        }

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return PremiseConverter.FromRow(row);
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
                var buildingRow = _buildings.Select().Rows.Find(premise.IdBuilding);
                if (buildingRow != null)
                {
                    _vKladr.Filter = "";
                    comboBoxStreet.SelectedValue = buildingRow["id_street"];
                    comboBoxHouse.SelectedValue = ViewportHelper.ValueOrDbNull(premise.IdBuilding);
                }
            }
            comboBoxState.SelectedValue = ViewportHelper.ValueOrDbNull(premise.IdState);
            comboBoxPremisesType.SelectedValue = ViewportHelper.ValueOrDbNull(premise.IdPremisesType);
            comboBoxPremisesKind.SelectedValue = ViewportHelper.ValueOrDbNull(premise.IdPremisesKind);
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

            GeneralDataModel = EntityDataModel<Premise>.GetInstance();
            _kladr = DataModel.GetInstance<KladrStreetsDataModel>();
            _buildings = EntityDataModel<Building>.GetInstance();
            _premisesTypes = DataModel.GetInstance<PremisesTypesDataModel>();
            _premisesKinds = DataModel.GetInstance<PremisesKindsDataModel>();
            _subPremises = EntityDataModel<SubPremise>.GetInstance();
            _restrictions = EntityDataModel<Restriction>.GetInstance();
            _restrictionTypes = EntityDataModel<RestrictionType>.GetInstance();
            _restrictionPremisesAssoc = DataModel.GetInstance<RestrictionsPremisesAssocDataModel>();
            _restrictionBuildingsAssoc = DataModel.GetInstance<RestrictionsBuildingsAssocDataModel>();
            _ownershipRights = EntityDataModel<OwnershipRight>.GetInstance();
            _ownershipRightTypes = EntityDataModel<OwnershipRightType>.GetInstance();
            _ownershipPremisesAssoc = DataModel.GetInstance<OwnershipPremisesAssocDataModel>();
            _ownershipBuildingsAssoc = DataModel.GetInstance<OwnershipBuildingsAssocDataModel>();
            _fundTypes = DataModel.GetInstance<FundTypesDataModel>();
            _objectStates = DataModel.GetInstance<ObjectStatesDataModel>(); 

            // Вычисляемые модели
            _premisesCurrentFund = CalcDataModel.GetInstance<CalcDataModelPremisesCurrentFunds>();
            _premiseSubPremisesSumArea = CalcDataModel.GetInstance<CalcDataModelPremiseSubPremisesSumArea>();
            _subPremisesCurrentFund = CalcDataModel.GetInstance<CalcDataModelSubPremisesCurrentFunds>();
            CalcDataModel.GetInstance<CalcDataModelMunicipalPremises>();

            // Ожидаем дозагрузки, если это необходмо
            GeneralDataModel.Select();
            _kladr.Select();
            _buildings.Select();
            _premisesTypes.Select();
            _premisesKinds.Select();
            _subPremises.Select();
            _restrictions.Select();
            _restrictionTypes.Select();
            _restrictionPremisesAssoc.Select();
            _restrictionBuildingsAssoc.Select();
            _ownershipRights.Select();
            _ownershipRightTypes.Select();
            _ownershipPremisesAssoc.Select();
            _ownershipBuildingsAssoc.Select();
            _fundTypes.Select();
            _objectStates.Select();

            var ds = DataModel.DataSet;

            _vKladr = new BindingSource
            {
                DataMember = "kladr",
                DataSource = ds
            };

            _vBuildings = new BindingSource
            {
                DataMember = "kladr_buildings",
                DataSource = _vKladr
            };

            _vPremisesCurrentFund = new BindingSource
            {
                DataMember = "premises_current_funds",
                DataSource = _premisesCurrentFund.Select()
            };

            _vPremisesSubPremisesSumArea = new BindingSource
            {
                DataMember = "premise_sub_premises_sum_area",
                DataSource = _premiseSubPremisesSumArea.Select()
            };

            _vSubPremisesCurrentFund = new BindingSource
            {
                DataMember = "sub_premises_current_funds",
                DataSource = _subPremisesCurrentFund.Select()
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

            _vSubPremisesObjectStates = new BindingSource
            {
                DataMember = "object_states",
                DataSource = ds
            };

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", v_premises_CurrentItemChanged);
            GeneralBindingSource.DataMember = "premises";
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = ds;
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", PremisesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", PremisesViewport_RowDeleted);

            _vPremisesTypes = new BindingSource
            {
                DataMember = "premises_types",
                DataSource = ds
            };

            _vPremisesKinds = new BindingSource
            {
                DataMember = "premises_kinds",
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
                DataSource = ds,
                DataMember = "restriction_types"
            };

            _vOwnershipRightTypes = new BindingSource
            {
                DataMember = "ownership_right_types",
                DataSource = ds
            };

            _vSubPremises = new BindingSource
            {
                DataMember = "premises_sub_premises",
                DataSource = GeneralBindingSource
            };
            AddEventHandler<EventArgs>(_vSubPremises, "CurrentItemChanged", v_sub_premises_CurrentItemChanged);

            _vRestrictionPremisesAssoc = new BindingSource { DataMember = "premises_restrictions_premises_assoc" };
            AddEventHandler<EventArgs>(_vRestrictionPremisesAssoc, "CurrentItemChanged", v_restrictionPremisesAssoc_CurrentItemChanged);
            _vRestrictionPremisesAssoc.DataSource = GeneralBindingSource;

            _vRestrictionBuildingsAssoc = new BindingSource { DataMember = "restrictions_buildings_assoc" };
            AddEventHandler<EventArgs>(_vRestrictionBuildingsAssoc, "CurrentItemChanged", v_restrictionBuildingsAssoc_CurrentItemChanged);
            _vRestrictionBuildingsAssoc.DataSource = ds;

            RestrictionsFilterRebuild();
            AddEventHandler<DataRowChangeEventArgs>(_restrictionPremisesAssoc.Select(), "RowChanged", RestrictionsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_restrictionPremisesAssoc.Select(), "RowDeleted", RestrictionsAssoc_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(_restrictionBuildingsAssoc.Select(), "RowChanged", restrictionBuildingsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_restrictionBuildingsAssoc.Select(), "RowDeleted", restrictionBuildingsAssoc_RowDeleted);

            _vOwnershipPremisesAssoc = new BindingSource { DataMember = "premises_ownership_premises_assoc" };
            AddEventHandler<EventArgs>(_vOwnershipPremisesAssoc, "CurrentItemChanged", v_ownershipPremisesAssoc_CurrentItemChanged);
            _vOwnershipPremisesAssoc.DataSource = GeneralBindingSource;

            _vOwnershipBuildingsAssoc = new BindingSource { DataMember = "ownership_buildings_assoc" };
            AddEventHandler<EventArgs>(_vOwnershipBuildingsAssoc, "CurrentItemChanged", v_ownershipBuildingsAssoc_CurrentItemChanged);
            _vOwnershipBuildingsAssoc.DataSource = ds;

            OwnershipsFilterRebuild();
            AddEventHandler<DataRowChangeEventArgs>(_ownershipPremisesAssoc.Select(), "RowChanged", OwnershipsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_ownershipPremisesAssoc.Select(), "RowDeleted", OwnershipsAssoc_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(_ownershipBuildingsAssoc.Select(), "RowChanged", ownershipBuildingsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_ownershipBuildingsAssoc.Select(), "RowDeleted", ownershipBuildingsAssoc_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(_subPremises.Select(), "RowChanged", SubPremises_RowChanged);

            DataBind();

            AddEventHandler<EventArgs>(_premisesCurrentFund, "RefreshEvent", premisesCurrentFund_RefreshEvent);
            AddEventHandler<EventArgs>(_premiseSubPremisesSumArea, "RefreshEvent", premiseSubPremisesSumArea_RefreshEvent);
            AddEventHandler<EventArgs>(_subPremisesCurrentFund, "RefreshEvent", subPremisesCurrentFund_RefreshEvent);

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
            IsEditable = false;
            var premise = (Premise) EntityFromView();
            GeneralBindingSource.AddNew();
            GeneralDataModel.EditingNewRecord = true;
            if (premise.IdBuilding != null)
            {
                comboBoxStreet.SelectedValue = premise.IdBuilding;
                comboBoxHouse.SelectedValue = premise.IdBuilding;
            }
            ViewportFromPremise(premise);
            IsEditable = true;
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
            IsEditable = false;
            GeneralBindingSource.AddNew();
            IsEditable = true;
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
                    if (_spSimpleSearchForm == null)
                        _spSimpleSearchForm = new SimpleSearchPremiseForm();
                    if (_spSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _spSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (_spExtendedSearchForm == null)
                        _spExtendedSearchForm = new ExtendedSearchPremisesForm();
                    if (_spExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _spExtendedSearchForm.GetFilter();
                    break;
            }
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            IsEditable = false;
            GeneralBindingSource.Filter = filter;
            IsEditable = true;
        }

        public override void ClearSearch()
        {
            IsEditable = false;
            GeneralBindingSource.Filter = StaticFilter;
            IsEditable = true;
            DynamicFilter = "";
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (ViewportState != ViewportState.NewRowState) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это помещение?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_premises"], EntityType.Premise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_premises"]) == -1)
                    return;
                IsEditable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                IsEditable = true;
                ViewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            var premise = (Premise)EntityFromViewport();
            if (!ValidatePremise(premise))
                return;
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    InsertRecord(premise);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    UpdateRecord(premise);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            ShowOrHideCurrentFund();
        }

        public void InsertRecord(Premise premise)
        {
            var idPremise = GeneralDataModel.Insert(premise);
            if (idPremise == -1)
            {
                return;
            }
            DataRowView newRow;
            premise.IdPremises = idPremise;
            RebuildFilterAfterSave(GeneralBindingSource, premise.IdPremises);
            if (GeneralBindingSource.Position == -1)
                newRow = (DataRowView)GeneralBindingSource.AddNew();
            else
                newRow = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            PremiseConverter.FillRow(premise, newRow);
        }

        public void UpdateRecord(Premise premise)
        {
            if (premise.IdPremises == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить помещение без внутренного номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (GeneralDataModel.Update(premise) == -1)
                return;
            RebuildFilterAfterSave(GeneralBindingSource, premise.IdPremises);
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            PremiseConverter.FillRow(premise, row);
        }

        private void RebuildFilterAfterSave(IBindingListView bindingSource, int? idPremise)
        {
            var filter = "";
            if (!string.IsNullOrEmpty(bindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            filter += string.Format(CultureInfo.CurrentCulture, "(id_premises = {0})", idPremise);
            bindingSource.Filter += filter;
        }

        public override bool CanCancelRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        IsEditable = false;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                    }
                    else
                        Text = @"Здания отсутствуют";
                    ViewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    SelectCurrentBuilding();
                    ViewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool HasAssocViewport<T>()
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
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport<T>()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, 
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
                    return (_vSubPremises.Position > -1);
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
                {"ids", ((DataRowView) _vSubPremises[_vSubPremises.Position])["id_sub_premises"].ToString()},
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

        private void premisesCurrentFund_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        private void premiseSubPremisesSumArea_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        private void subPremisesCurrentFund_RefreshEvent(object sender, EventArgs e)
        {
            RedrawSubPremiseDataGridRows();
        }

        private void SubPremises_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RedrawSubPremiseDataGridRows();
        }

        private void PremisesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                UnbindedCheckBoxesUpdate();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void PremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
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

        private void v_ownershipPremisesAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            OwnershipsFilterRebuild();
        }

        private void v_restrictionPremisesAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            RestrictionsFilterRebuild();
        }

        private void v_ownershipBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            OwnershipsFilterRebuild();
        }

        private void v_restrictionBuildingsAssoc_CurrentItemChanged(object sender, EventArgs e)
        {
            RestrictionsFilterRebuild();
        }

        private void comboBoxStreet_VisibleChanged(object sender, EventArgs e)
        {
            if (_isFirstVisibility)
            {
                SelectCurrentBuilding();
                _isFirstVisibility = false;
            }
        }

        private void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
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

        private void v_premises_CurrentItemChanged(object sender, EventArgs e)
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
            if (ViewportState == ViewportState.NewRowState)
                return;
            ViewportState = ViewportState.ReadState;
            IsEditable = true;
        }

        private void v_sub_premises_CurrentItemChanged(object sender, EventArgs e)
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
            if (dataGridViewOwnerships.Size.Width > 700)
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

        private void dataGridViewRooms_Resize(object sender, EventArgs e)
        {
            if (dataGridViewRooms.Size.Width > 900)
            {
                if (sub_premises_id_state.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    sub_premises_id_state.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (sub_premises_id_state.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    sub_premises_id_state.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
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

        private void dataGridViewRooms_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocViewport<SubPremisesViewport>())
                ShowAssocViewport<SubPremisesViewport>();
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
            if (_restrictions.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок реквизитов уже находится в режиме добавления новой записи. " +
                    @"Одновременно можно добавлять не более одного реквизита.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new RestrictionsEditor())
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
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vRestrictions.Position == -1)
            {
                MessageBox.Show(@"Не выбран реквизит для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = (DataRowView)_vRestrictions[_vRestrictions.Position];
            var restriction = new Restriction
            {
                IdRestriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction"),
                IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type"),
                Number = row["number"].ToString(),
                Date = ViewportHelper.ValueOrNull<DateTime>(row, "date"),
                Description = row["description"].ToString()
            };
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                if (_vRestrictionBuildingsAssoc.Find("id_restriction", restriction.IdRestriction) != -1)
                {
                    editor.ParentType = ParentTypeEnum.Building;
                    editor.ParentRow = _buildings.Select().Rows.Find(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
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
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new OwnershipsEditor())
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
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
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
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vOwnershipRights.Position == -1)
            {
                MessageBox.Show(@"Не выбрано ограничение для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = (DataRowView)_vOwnershipRights[_vOwnershipRights.Position];
            var ownershipRight = OwnershipRightConverter.FromRow(row);
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                if (_vOwnershipBuildingsAssoc.Find("id_ownership_right", ownershipRight.IdOwnershipRight) != -1)
                {
                    editor.ParentType = ParentTypeEnum.Building;
                    editor.ParentRow = _buildings.Select().Rows.Find(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"]);
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
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_subPremises.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок комнат уже находится в режиме добавления новых записей. " +
                    @"Одновременно можно добавлять не более одной комнаты.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new SubPremisesEditor())
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
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vSubPremises.Position == -1)
            {
                MessageBox.Show(@"Не выбрана комната для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить эту комнату?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var idSubPremise = (int)((DataRowView)_vSubPremises[_vSubPremises.Position])["id_sub_premises"];
            if (_subPremises.Delete(idSubPremise) == -1)
                return;
            _subPremises.Select().Rows.Find(idSubPremise).Delete();
        }

        private void vButtonSubPremisesEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vSubPremises.Position == -1)
            {
                MessageBox.Show(@"Не выбрана комната для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = (DataRowView)_vSubPremises[_vSubPremises.Position];
            var subPremise = new SubPremise
            {
                IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises"),
                IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                SubPremisesNum = row["sub_premises_num"].ToString(),
                TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area"),
                LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area"),
                Description = row["description"].ToString(),
                StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date"),
                CadastralNum = ViewportHelper.ValueOrNull(row, "cadastral_num"),
                CadastralCost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost"),
                BalanceCost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost"),
                Account = ViewportHelper.ValueOrNull(row, "account")
            };
            using (var editor = new SubPremisesEditor())
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

        private void comboBoxHouse_SelectedValueChanged(object sender, EventArgs e)
        {
            label20.Text = @"Номер дома";
            label20.ForeColor = SystemColors.ControlText;
            if (!(comboBoxHouse.SelectedValue is int)) return;
            var idBuilding = (int)comboBoxHouse.SelectedValue;
            var isDemolished = DataModelHelper.DemolishedBuildingIDs().Any(v => v == idBuilding);
            if (!isDemolished) return;
            label20.Text = @"Номер дома (снесен)";
            label20.ForeColor = Color.Red;
        }
    }
}
