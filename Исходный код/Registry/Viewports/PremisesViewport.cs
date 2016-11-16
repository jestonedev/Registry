using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels;
using Registry.Entities;
using System.Drawing;
using Security;
using System.Globalization;
using System.Text.RegularExpressions;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ModalEditors;
using Registry.Viewport.Presenters;
using Registry.Viewport.SearchForms;

namespace Registry.Viewport
{
    internal sealed partial class PremisesViewport : FormViewport
    {
        private bool _isFirstVisibility = true;

        private PremisesViewport()
            : this(null, null)
        {
        }

        public PremisesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new PremisesPresenter())
        {
            InitializeComponent();
            dataGridViewOwnerships.AutoGenerateColumns = false;
            dataGridViewRestrictions.AutoGenerateColumns = false;
            dataGridViewRooms.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
        }

        private void RestrictionsFilterRebuild()
        {
            ((PremisesPresenter)Presenter).RestrictionsFilterRebuild();
            if (dataGridViewRestrictions.Columns.Contains("id_restriction"))
                id_restriction.Visible = false;
            RedrawRestrictionDataGridRows();
        }

        private void OwnershipsFilterRebuild()
        {
            ((PremisesPresenter)Presenter).OwnershipsFilterRebuild();
            if (dataGridViewOwnerships.Columns.Contains("id_ownership_right"))
                id_ownership_right.Visible = false;
            RedrawOwnershipDataGridRows();
        }

        private void FiltersRebuild()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                ClearPremiseCalcInfo();
                return;
            }
            if (row["id_premises"] == DBNull.Value)
            {
                ClearPremiseCalcInfo();
                return;
            }
            IsEditable = false;
            var currentFunds = Presenter.ViewModel["current_funds"].BindingSource;
            if (currentFunds != null)
            {
                var position = currentFunds.Find("id_premises", row["id_premises"]);
                comboBoxCurrentFundType.SelectedValue = position != -1 ?
                    ((DataRowView)currentFunds[position])["id_fund_type"] : DBNull.Value;
                ShowOrHideCurrentFund();
            }
            var subPremisesSumArea = Presenter.ViewModel["sub_premises_sum_area"].BindingSource;
            if (subPremisesSumArea != null)
            {
                var position = subPremisesSumArea.Find("id_premises", row["id_premises"]);
                if (position != -1)
                {
                    var value = Convert.ToDecimal((double)((DataRowView)subPremisesSumArea[position])["sum_area"]);
                    numericUpDownMunicipalArea.Value = value;
                }
                else
                {
                    numericUpDownMunicipalArea.Value = 0;
                }
            }
            IsEditable = true;
        }

        private void ClearPremiseCalcInfo()
        {
            comboBoxCurrentFundType.SelectedValue = DBNull.Value;
            numericUpDownMunicipalArea.Value = 0;
        }

        private void RedrawRestrictionDataGridRows()
        {
            for (var i = 0; i < dataGridViewRestrictions.Rows.Count; i++)
            {
                var assoc = Presenter.ViewModel["restrictions_buildings_assoc"].BindingSource;
                var style = dataGridViewRestrictions.Rows[i].DefaultCellStyle;
                if (
                    assoc.Find("id_restriction",
                        dataGridViewRestrictions.Rows[i].Cells["id_restriction"].Value) != -1)
                {
                    style.BackColor = Color.LightGreen;
                    style.SelectionBackColor = Color.Green;
                    dataGridViewRestrictions.Rows[i].Cells["restriction_relation"].Value = "Здание";
                }
                else
                {
                    style.BackColor = Color.White;
                    style.SelectionBackColor = SystemColors.Highlight;
                    dataGridViewRestrictions.Rows[i].Cells["restriction_relation"].Value = "Помещение";
                }
            }
        }

        private void RedrawOwnershipDataGridRows()
        {
            for (var i = 0; i < dataGridViewOwnerships.Rows.Count; i++)
            {
                var assoc = Presenter.ViewModel["ownership_buildings_assoc"].BindingSource;
                var style = dataGridViewOwnerships.Rows[i].DefaultCellStyle;
                if (
                    assoc.Find("id_ownership_right",
                        dataGridViewOwnerships.Rows[i].Cells["id_ownership_right"].Value) != -1)
                {
                    style.BackColor = Color.LightGreen;
                    style.SelectionBackColor = Color.Green;
                    dataGridViewOwnerships.Rows[i].Cells["ownership_relation"].Value = "Здание";
                }
                else
                {
                    style.BackColor = Color.White;
                    style.SelectionBackColor = SystemColors.Highlight;
                    dataGridViewOwnerships.Rows[i].Cells["ownership_relation"].Value = "Помещение";
                }
            }
        }

        private void RedrawSubPremiseDataGridRows()
        {
            var subPremises = Presenter.ViewModel["premises_sub_premises"].BindingSource;
            var subPremisesKey = Presenter.ViewModel["premises_sub_premises"].PrimaryKeyFirst;
            if (subPremises == null)
                return;
            if (subPremises.Count != dataGridViewRooms.Rows.Count)
                return;
            for (var i = 0; i < subPremises.Count; i++)
            {
                var idSubPremises = (int)((DataRowView)subPremises[i])[subPremisesKey];
                var subPremisesCurrentFund = Presenter.ViewModel["sub_premises_current_funds"].BindingSource;
                var id = subPremisesCurrentFund.Find(subPremisesKey, idSubPremises);
                if (id == -1)
                    continue;
                var idFundType = (int)((DataRowView)subPremisesCurrentFund[id])["id_fund_type"];
                var fundTypes = Presenter.ViewModel["fund_types"].DataSource;
                var fundTypeRow = fundTypes.Rows.Find(idFundType);
                var fundType = fundTypeRow == null ? "" : fundTypeRow["fund_type"];

                dataGridViewRooms.Rows[i].Cells["current_fund"].Value = 
                    DataModelHelper.MunicipalAndUnknownObjectStates().Contains((int)((DataRowView)subPremises[i])["id_state"]) ? 
                    fundType : "";
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
                return;
            }
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row != null)
            {
                if (ParentRow == null)
                {
                    Text = string.Format(CultureInfo.InvariantCulture, "Помещение №{0}", row["id_premises"]);
                    return;
                }
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        Text = string.Format(CultureInfo.InvariantCulture, "Помещение №{0} здания №{1}", row["id_premises"], ParentRow["id_building"]);
                        return;
                    case ParentTypeEnum.PaymentAccount:
                        Text = string.Format("Помещения по лицевому счету №{0}", ParentRow["account"]);
                        break;
                }
                return;
            }
            if (ParentRow == null)
            {
                Text = @"Помещения отсутствуют";
                return;
            }
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    Text = string.Format(CultureInfo.InvariantCulture, "Помещения в здании №{0} отсутствуют", ParentRow["id_building"]);
                    break;
                case ParentTypeEnum.PaymentAccount:
                    Text = string.Format("Помещения по лицевому счету №{0} отсутствуют", ParentRow["account"]);
                    break;
            }
        }

        private void ShowOrHideCurrentFund()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (comboBoxCurrentFundType.SelectedValue != null && row != null && row["id_state"] != DBNull.Value &&
                DataModelHelper.MunicipalAndUnknownObjectStates().Contains((int)row["id_state"]))
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
            if ((comboBoxHouse.DataSource == null) || (comboBoxStreet.DataSource == null)) return;
            int? idBuilding = null;
            var row = Presenter.ViewModel["general"].CurrentRow;
            if ((row != null) && (row["id_building"] != DBNull.Value))
                idBuilding = Convert.ToInt32(row["id_building"], CultureInfo.InvariantCulture);
            else 
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Building))
                    idBuilding = Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture);
            string idStreet = null;
            if (idBuilding != null)
            {
                var buildingRow = Presenter.ViewModel["buildings"].DataSource.Rows.Find(idBuilding);
                if (buildingRow != null)
                    idStreet = buildingRow["id_street"].ToString();
            }
            Presenter.ViewModel["kladr"].BindingSource.Filter = "";
            IsEditable = false;
            if (idStreet != null)
                comboBoxStreet.SelectedValue = idStreet;
            else
                comboBoxStreet.SelectedValue = DBNull.Value;
            if (idBuilding != null)
                comboBoxHouse.SelectedValue = idBuilding;
            else
                comboBoxHouse.SelectedValue = DBNull.Value;
            IsEditable = true;
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            IsEditable = false;
            if (row != null && (row["state_date"] != DBNull.Value))
                dateTimePickerStateDate.Checked = true;
            else
            {
                dateTimePickerStateDate.Value = DateTime.Now.Date;
                dateTimePickerStateDate.Checked = false;
            }
            IsEditable = true;
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindSource(comboBoxStreet, Presenter.ViewModel["kladr"].BindingSource, "street_name", 
                Presenter.ViewModel["kladr"].PrimaryKeyFirst);
            ViewportHelper.BindSource(comboBoxHouse, Presenter.ViewModel["kladr_buildings"].BindingSource, "house", "id_building");
            ViewportHelper.BindSource(comboBoxPremisesKind, Presenter.ViewModel["premises_kinds"].BindingSource, "premises_kind",
                Presenter.ViewModel["premises_kinds"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxPremisesKind, "SelectedValue", bindingSource,
                Presenter.ViewModel["premises_kinds"].PrimaryKeyFirst, 1);
            ViewportHelper.BindSource(comboBoxPremisesType, Presenter.ViewModel["premises_types"].BindingSource, "premises_type_as_num",
                Presenter.ViewModel["premises_types"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxPremisesType, "SelectedValue", bindingSource,
                Presenter.ViewModel["premises_types"].PrimaryKeyFirst, 1);
            ViewportHelper.BindProperty(textBoxPremisesNumber, "Text", bindingSource, "premises_num", "");
            ViewportHelper.BindProperty(numericUpDownFloor, "Value", bindingSource, "floor", 0m);
            ViewportHelper.BindProperty(textBoxCadastralNum, "Text", bindingSource, "cadastral_num", "");
            ViewportHelper.BindProperty(textBoxDescription, "Text", bindingSource, "description", "");
            ViewportHelper.BindProperty(textBoxAccount, "Text", bindingSource, "account", "");
            ViewportHelper.BindProperty(numericUpDownCadastralCost, "Value", bindingSource, "cadastral_cost", 0m);
            ViewportHelper.BindProperty(numericUpDownBalanceCost, "Value", bindingSource, "balance_cost", 0m);
            ViewportHelper.BindProperty(numericUpDownNumRooms, "Value", bindingSource, "num_rooms", 0m);
            ViewportHelper.BindProperty(numericUpDownNumBeds, "Value", bindingSource, "num_beds", 0m);
            ViewportHelper.BindProperty(numericUpDownTotalArea, "Value", bindingSource, "total_area", 0m);
            ViewportHelper.BindProperty(numericUpDownLivingArea, "Value", bindingSource, "living_area", 0m);
            ViewportHelper.BindProperty(numericUpDownHeight, "Value", bindingSource, "height", 0m);
            ViewportHelper.BindSource(comboBoxCurrentFundType, Presenter.ViewModel["fund_types"].BindingSource, "fund_type",
                Presenter.ViewModel["fund_types"].PrimaryKeyFirst);
            ViewportHelper.BindSource(comboBoxState, Presenter.ViewModel["object_states"].BindingSource, "state_neutral",
                Presenter.ViewModel["object_states"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxState, "SelectedValue", bindingSource,
                Presenter.ViewModel["object_states"].PrimaryKeyFirst, DBNull.Value);
            ViewportHelper.BindProperty(checkBoxIsMemorial, "Checked", bindingSource, "is_memorial", true);
            ViewportHelper.BindProperty(dateTimePickerRegDate, "Value", bindingSource, "reg_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(dateTimePickerStateDate, "Value", bindingSource, "state_date", DateTime.Now.Date);

            dataGridViewRestrictions.DataSource = Presenter.ViewModel["restrictions"].BindingSource;
            id_restriction.DataPropertyName = Presenter.ViewModel["restrictions"].PrimaryKeyFirst;
            restriction_number.DataPropertyName = "number";
            restriction_date.DataPropertyName = "date";
            restriction_date_state_reg.DataPropertyName = "date_state_reg";
            restriction_description.DataPropertyName = "description";
            ViewportHelper.BindSource(id_restriction_type, Presenter.ViewModel["restriction_types"].BindingSource, "restriction_type",
                Presenter.ViewModel["restriction_types"].PrimaryKeyFirst);

            dataGridViewOwnerships.DataSource = Presenter.ViewModel["ownership_rights"].BindingSource;
            id_ownership_right.DataPropertyName = Presenter.ViewModel["ownership_rights"].PrimaryKeyFirst;
            ownership_number.DataPropertyName = "number";
            ownership_date.DataPropertyName = "date";
            ownership_description.DataPropertyName = "description";
            ViewportHelper.BindSource(id_ownership_type, Presenter.ViewModel["ownership_right_types"].BindingSource, "ownership_right_type",
                Presenter.ViewModel["ownership_right_types"].PrimaryKeyFirst);

            dataGridViewRooms.DataSource = Presenter.ViewModel["premises_sub_premises"].BindingSource;
            sub_premises_num.DataPropertyName = "sub_premises_num";
            sub_premises_total_area.DataPropertyName = "total_area";
            sub_premises_cadastral_num.DataPropertyName = "cadastral_num";
            sub_premises_cadastral_cost.DataPropertyName = "cadastral_cost";
            sub_premises_balance_cost.DataPropertyName = "balance_cost";
            sub_premises_account.DataPropertyName = "account";
            ViewportHelper.BindSource(sub_premises_id_state, Presenter.ViewModel["sub_premises_object_states"].BindingSource, "state_female",
                Presenter.ViewModel["sub_premises_object_states"].PrimaryKeyFirst);
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) ||
                AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidatePremise(Premise premise)
        {
            var premiseFromView = (Premise)EntityFromView();
            if (premise.IdBuilding == null)
            {
                MessageBox.Show(@"Необходимо выбрать здание",@"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxHouse.Focus();
                return false;
            }
            if (premise.IdPremises != null && ((premise.IdBuilding != premiseFromView.IdBuilding) || (premise.PremisesNum != premiseFromView.PremisesNum)))
            {
                if (MessageBox.Show(@"Вы хотите поменять адрес квартиры. Продолжить?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
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
            if (premiseFromView.IdPremises != null && OtherService.HasMunicipal(premiseFromView.IdPremises.Value, EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"Вы не можете изменить информацию по данному помещению, т.к. оно является муниципальным или содержит в себе муниципальные комнаты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (premiseFromView.IdPremises != null && OtherService.HasNotMunicipal(premiseFromView.IdPremises.Value, EntityType.Premise)
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
                if (PremisesService.PremisesDuplicateCount(premise) != 0 &&
                    MessageBox.Show(@"В указанном доме уже есть квартира с таким номером. Все равно продолжить сохранение?", @"Внимание", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
            return true;
        }

        protected override Entity EntityFromView()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return row == null ? new Premise() : EntityConverter<Premise>.FromRow(row);
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var premise = new Premise
            {
                IdPremises = row == null ? null : ViewportHelper.ValueOrNull<int>(row, "id_premises"),
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
                var buildings = Presenter.ViewModel["buildings"].DataSource;
                var buildingRow = buildings.Rows.Find(premise.IdBuilding);
                if (buildingRow != null)
                {
                    Presenter.ViewModel["kladr"].BindingSource.Filter = "";
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
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", PremisesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", PremisesViewport_RowDeleted);

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", v_premises_CurrentItemChanged);
            AddEventHandler<EventArgs>(Presenter.ViewModel["premises_sub_premises"].BindingSource, 
                "CurrentItemChanged", v_sub_premises_CurrentItemChanged);
            AddEventHandler<EventArgs>(Presenter.ViewModel["premises_restrictions_premises_assoc"].BindingSource, 
                "CurrentItemChanged", v_restrictionPremisesAssoc_CurrentItemChanged);
            AddEventHandler<EventArgs>(Presenter.ViewModel["restrictions_buildings_assoc"].BindingSource,
                "CurrentItemChanged", v_restrictionBuildingsAssoc_CurrentItemChanged);
            AddEventHandler<EventArgs>(Presenter.ViewModel["premises_ownership_premises_assoc"].BindingSource, 
                "CurrentItemChanged", v_ownershipPremisesAssoc_CurrentItemChanged);
            AddEventHandler<EventArgs>(Presenter.ViewModel["ownership_buildings_assoc"].BindingSource, 
                "CurrentItemChanged", v_ownershipBuildingsAssoc_CurrentItemChanged);

            RestrictionsFilterRebuild();
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["restrictions_premises_assoc"].DataSource, "RowChanged", RestrictionsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["restrictions_premises_assoc"].DataSource, "RowDeleted", RestrictionsAssoc_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["restrictions_buildings_assoc"].DataSource, "RowChanged", restrictionBuildingsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["restrictions_buildings_assoc"].DataSource, "RowDeleted", restrictionBuildingsAssoc_RowDeleted);

            OwnershipsFilterRebuild();
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_premises_assoc"].DataSource, "RowChanged", OwnershipsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_premises_assoc"].DataSource, "RowDeleted", OwnershipsAssoc_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_buildings_assoc"].DataSource, "RowChanged", ownershipBuildingsAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_buildings_assoc"].DataSource, "RowDeleted", ownershipBuildingsAssoc_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["sub_premises"].DataSource, "RowChanged", SubPremises_RowChanged);

            AddEventHandler<EventArgs>(Presenter.ViewModel["current_funds"].Model, "RefreshEvent", premisesCurrentFund_RefreshEvent);
            AddEventHandler<EventArgs>(Presenter.ViewModel["sub_premises_sum_area"].Model, "RefreshEvent", premiseSubPremisesSumArea_RefreshEvent);
            AddEventHandler<EventArgs>(Presenter.ViewModel["sub_premises_current_funds"].Model, "RefreshEvent", subPremisesCurrentFund_RefreshEvent);

            DataBind();

            v_premises_CurrentItemChanged(null, new EventArgs());
            v_sub_premises_CurrentItemChanged(null, new EventArgs());
            v_restrictionPremisesAssoc_CurrentItemChanged(null, new EventArgs());
            v_restrictionBuildingsAssoc_CurrentItemChanged(null, new EventArgs());
            v_ownershipPremisesAssoc_CurrentItemChanged(null, new EventArgs());
            v_ownershipBuildingsAssoc_CurrentItemChanged(null, new EventArgs());

            DataChangeHandlersInit();
            IsEditable = true;
        }
        
        public override bool CanCopyRecord()
        {
            return Presenter.ViewModel["general"].CurrentRow != null && !Presenter.ViewModel["general"].Model.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            var premise = (Premise)EntityFromView();
            Presenter.ViewModel["general"].BindingSource.AddNew();
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
            return !Presenter.ViewModel["general"].Model.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
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
                    if (Presenter.SimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.SimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (Presenter.ExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.ExtendedSearchForm.GetFilter();
                    break;
            }
            IsEditable = false;
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            IsEditable = true;
        }

        public override void ClearSearch()
        {
            IsEditable = false;
            Presenter.ViewModel["general"].BindingSource.Filter = StaticFilter;
            IsEditable = true;
            DynamicFilter = "";
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null)
                && (ViewportState != ViewportState.NewRowState) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это помещение?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            IsEditable = false;
            if (!((PremisesPresenter)Presenter).DeleteRecord())
            {
                IsEditable = true;
                return;
            }
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
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
                    if (!((PremisesPresenter)Presenter).InsertRecord(premise))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((PremisesPresenter) Presenter).UpdateRecord(premise))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            ShowOrHideCurrentFund();
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
                    Presenter.ViewModel["general"].Model.EditingNewRecord = false;
                    if (Presenter.ViewModel["general"].CurrentRow != null)
                    {
                        IsEditable = false;
                        Presenter.ViewModel["general"].CurrentRow.Delete();
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    SelectCurrentBuilding();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
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
            return reports.Any(v => v.ToString() == typeof(T).Name) && Presenter.ViewModel["general"].CurrentRow != null;
        }

        public override void ShowAssocViewport<T>()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var row = Presenter.ViewModel["general"].CurrentRow;
            var columnName = Presenter.ViewModel["general"].PrimaryKeyFirst;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback,
                columnName+" = " + Convert.ToInt32(row[columnName], CultureInfo.InvariantCulture), row.Row,
                ParentTypeEnum.Premises);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            switch (reporterType)
            {
                case ReporterType.RegistryExcerptReporterPremise:
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    return Presenter.ViewModel["general"].CurrentRow != null;
                case ReporterType.RegistryExcerptReporterSubPremise:
                    return Presenter.ViewModel["premises_sub_premises"].CurrentRow != null;
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
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
            MenuCallback.RunReport(reporterType, arguments);
        }

        private Dictionary<string, string> RegistryExcerptPremiseReportArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", Presenter.ViewModel["general"].CurrentRow["id_premises"].ToString()},
                {"excerpt_type", "1"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterSubPremiseArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", Presenter.ViewModel["premises_sub_premises"].CurrentRow["id_sub_premises"].ToString()},
                {"excerpt_type", "2"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterAllMunSubPremisesArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", Presenter.ViewModel["general"].CurrentRow["id_premises"].ToString()},
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
            dataGridViewRestrictions.Focus();
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
            if (!_isFirstVisibility) return;
            SelectCurrentBuilding();
            _isFirstVisibility = false;
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
                Presenter.ViewModel["kladr"].BindingSource.Filter = "street_name like '%" + comboBoxStreet.Text + "%'";
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
                    comboBoxStreet.SelectedValue = Presenter.ViewModel["kladr"].CurrentRow;
                comboBoxStreet.Text = Presenter.ViewModel["kladr"].CurrentRow["street_name"].ToString();
            }
            if (comboBoxStreet.SelectedValue == null)
                comboBoxStreet.Text = "";
        }

        private void v_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            var isEditable = IsEditable;
            SetViewportCaption();
            FiltersRebuild();
            SelectCurrentBuilding();
            UnbindedCheckBoxesUpdate();
            IsEditable = isEditable;

            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void v_sub_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.DocumentsStateUpdate();
            RedrawSubPremiseDataGridRows();
        }

        private void dataGridViewRestrictions_Resize(object sender, EventArgs e)
        {
            if (dataGridViewRestrictions.Size.Width > 800)
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
            if (Presenter.ViewModel["restrictions"].Model.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок реквизитов уже находится в режиме добавления новой записи. " +
                    @"Одновременно можно добавлять не более одного реквизита.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = row.Row;
                editor.ShowDialog();
            }
        }

        private void vButtonRestrictionEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["restrictions"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран реквизит для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var restriction = EntityConverter<Restriction>.FromRow(row);
            using (var editor = new RestrictionsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                var assoc = Presenter.ViewModel["restrictions_buildings_assoc"].BindingSource;
                if (assoc.Find("id_restriction", restriction.IdRestriction) != -1)
                {
                    editor.ParentType = ParentTypeEnum.Building;
                    editor.ParentRow = Presenter.ViewModel["buildings"].DataSource.Rows.Find(Presenter.ViewModel["general"].CurrentRow["id_building"]);
                }
                else
                {
                    editor.ParentType = ParentTypeEnum.Premises;
                    editor.ParentRow = Presenter.ViewModel["general"].CurrentRow.Row;
                }
                editor.RestrictionValue = restriction;
                editor.ShowDialog();
            }
        }

        private void vButtonRestrictionDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["restrictions"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран реквизит для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить этот реквизит?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var id = (int)row["id_restriction"];
            Presenter.ViewModel["restrictions"].Delete(id);
        }

        private void vButtonOwnershipAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["ownership_rights"].Model.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок ограничений уже находится в режиме добавления новой записи. " +
                    @"Одновременно можно добавлять не более одного ограничения.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = row.Row;
                editor.ShowDialog();
            }
        }

        private void vButtonOwnershipDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["ownership_rights"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано ограничение для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить это ограничение?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var id = (int)row["id_ownership_right"];
            Presenter.ViewModel["ownership_rights"].Delete(id);
        }

        private void vButtonOwnershipEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["ownership_rights"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано ограничение для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var ownershipRight = EntityConverter<OwnershipRight>.FromRow(row);
            using (var editor = new OwnershipsEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                var assoc = Presenter.ViewModel["ownership_buildings_assoc"].BindingSource;
                if (assoc.Find("id_ownership_right", ownershipRight.IdOwnershipRight) != -1)
                {
                    editor.ParentType = ParentTypeEnum.Building;
                    editor.ParentRow = Presenter.ViewModel["buildings"].DataSource.Rows.Find(
                        Presenter.ViewModel["general"].CurrentRow["id_building"]);
                }
                else
                {
                    editor.ParentType = ParentTypeEnum.Premises;
                    editor.ParentRow = Presenter.ViewModel["general"].CurrentRow.Row;
                }
                editor.OwnershipRightValue = ownershipRight;
                editor.ShowDialog();
            }
        }

        private void vButtonSubPremisesAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (Presenter.ViewModel["premises_sub_premises"].Model.EditingNewRecord)
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
                editor.ParentRow = row.Row;
                editor.ShowDialog();
            }
        }

        private void vButtonSubPremisesDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var subPremiseRow = Presenter.ViewModel["premises_sub_premises"].CurrentRow;
            if (subPremiseRow == null)
            {
                MessageBox.Show(@"Не выбрана комната для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить эту комнату?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var id = (int)subPremiseRow["id_sub_premises"];
            Presenter.ViewModel["premises_sub_premises"].Delete(id);
        }

        private void vButtonSubPremisesEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var premiseRow = Presenter.ViewModel["general"].CurrentRow;
            if (premiseRow == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["premises_sub_premises"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрана комната для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var subPremise = EntityConverter<SubPremise>.FromRow(row);
            using (var editor = new SubPremisesEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentType = ParentTypeEnum.Premises;
                editor.ParentRow = premiseRow.Row;             
                editor.SubPremiseValue = subPremise;
                editor.ShowDialog();
            }
        }

        internal int GetCurrentId()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var columnName = Presenter.ViewModel["general"].PrimaryKeyFirst;
            if (row == null) return -1;
            if (row[columnName] != DBNull.Value)
                return (int)row[columnName];
            return -1;
        }

        internal string GetFilter()
        {
            return Presenter.ViewModel["general"].BindingSource.Filter;
        }

        private void comboBoxHouse_SelectedValueChanged(object sender, EventArgs e)
        {
            label20.Text = @"Номер дома";
            label20.ForeColor = SystemColors.ControlText;
            if (!(comboBoxHouse.SelectedValue is int)) return;
            var idBuilding = (int)comboBoxHouse.SelectedValue;
            var isDemolished = BuildingService.DemolishedBuildingIDs().Any(v => v == idBuilding);
            if (!isDemolished) return;
            label20.Text = @"Номер дома (снесен)";
            label20.ForeColor = Color.Red;
        }
    }
}