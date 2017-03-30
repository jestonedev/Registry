using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;

namespace Registry.Viewport.SearchForms
{
    internal partial class ExtendedSearchPaymentAccounts : SearchForm
    {
        readonly BindingSource _vKladr;

        public ExtendedSearchPaymentAccounts()
        {
            InitializeComponent();
            _vKladr = new BindingSource
            {
                DataSource = DataModel.GetInstance<KladrStreetsDataModel>().Select()
            };

            var regions = DataModel.GetInstance<KladrRegionsDataModel>();
            var vRegions = new BindingSource { DataSource = regions.Select() };

            comboBoxStreet.DataSource = _vKladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxRegion.DataSource = vRegions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxDateExpr.SelectedIndex = 2;
            comboBoxBalanceInputExpr.SelectedIndex = 2;
            comboBoxBalanceInputDGIExpr.SelectedIndex = 2;
            comboBoxBalanceInputTenancyExpr.SelectedIndex = 2;
            comboBoxChargingDGIExpr.SelectedIndex = 2;
            comboBoxChargingExpr.SelectedIndex = 2;
            comboBoxChargingTenancyExpr.SelectedIndex = 2;
            comboBoxPaymentDGIExpr.SelectedIndex = 2;
            comboBoxPaymentTenancyExpr.SelectedIndex = 2;
            comboBoxTransferBalanceExpr.SelectedIndex = 2;
            comboBoxBalanceOutputExpr.SelectedIndex = 2;
            comboBoxBalanceOutputTenancyExpr.SelectedIndex = 2;
            comboBoxBalanceOutputDGIExpr.SelectedIndex = 2;
            comboBoxRecalcDGIExpr.SelectedIndex = 2;
            comboBoxRecalcTenancyExpr.SelectedIndex = 2;

            HandleHotKeys(Controls, vButtonSearch_Click);
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedAccounts = null;
            if (checkBoxCRNEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("crn LIKE '%{0}%'", textBoxCRN.Text.Trim().Replace("'", ""));
            }
            if (checkBoxAccountEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("account LIKE '%{0}%'", textBoxAccount.Text.Trim().Replace("'", ""));
            }
            if (checkBoxRawAddressEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                var addressParts = textBoxRawAddress.Text.Trim().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var addressFilter = "";
                foreach (var part in addressParts)
                {
                    if (!string.IsNullOrEmpty(addressFilter))
                        addressFilter += " AND ";
                    addressFilter += addressFilter + string.Format("raw_address LIKE '%{0}%'", part.Replace("'", ""));
                }
                filter += string.Format("({0})", addressFilter);
            }
            if (checkBoxStreetEnable.Checked)
                includedAccounts = DataModelHelper.Intersect(null, PaymentService.GetAccountIdsByStreet(comboBoxStreet.SelectedValue.ToString()));
            if (checkBoxRegionEnable.Checked)
                includedAccounts = DataModelHelper.Intersect(null, PaymentService.GetAccountIdsByRegion(comboBoxRegion.SelectedValue.ToString()));
            if (checkBoxHouseEnable.Checked)
                includedAccounts = DataModelHelper.Intersect(includedAccounts, PaymentService.GetAccountIdsByHouse(textBoxHouse.Text));
            if (checkBoxPremisesNumEnable.Checked)
                includedAccounts = DataModelHelper.Intersect(includedAccounts, PaymentService.GetAccountIdsByPremiseNumber(textBoxPremisesNum.Text));         
            if (checkBoxDateEnable.Checked && !checkBoxBalanceInputEnable.Checked &&
                !checkBoxBalanceInputTenancyEnable.Checked
                && !checkBoxBalanceInputDGIEnable.Checked && !checkBoxChargingEnable.Checked &&
                !checkBoxChargingTenancyEnable.Checked &&
                !checkBoxChargingDGIEnable.Checked && !checkBoxRecalcTenancyEnable.Checked &&
                !checkBoxRecalcDGIEnable.Checked &&
                !checkBoxPaymentTenancyEnable.Checked && !checkBoxPaymentDGIEnable.Checked &&
                !checkBoxTransferBalanceEnable.Checked && !checkBoxBalanceOutputEnable.Checked &&
                !checkBoxBalanceOutputTenancyEnable.Checked
                && !checkBoxBalanceOutputDGIEnable.Checked && !checkBoxTenantSNPEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("date {0} '{1}'", ConvertDisplayEqExprToSql(comboBoxDateExpr.Text), dateTimePickerDate.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                if (checkBoxTenantSNPEnable.Checked)
                    includedAccounts =
                        PaymentService.GetAccountIdsByPaymentFilter(string.Format("tenant LIKE '%{0}%'", textBoxTenantSNP.Text.Trim().Replace("'", "")));
                if (checkBoxBalanceInputEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "balance_input",
                        comboBoxBalanceInputExpr.Text, numericUpDownBalanceInputFrom.Value, numericUpDownBalanceInputTo.Value);
                if (checkBoxBalanceInputTenancyEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "balance_tenancy",
                        comboBoxBalanceInputTenancyExpr.Text, numericUpDownBalanceInputTenancyFrom.Value, numericUpDownBalanceInputTenancyTo.Value);
                if (checkBoxBalanceInputDGIEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "balance_dgi",
                        comboBoxBalanceInputDGIExpr.Text, numericUpDownBalanceInputDGIFrom.Value, numericUpDownBalanceInputDGITo.Value);
                if (checkBoxChargingEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "charging_total",
                        comboBoxChargingExpr.Text, numericUpDownChargingFrom.Value, numericUpDownChargingTo.Value);
                if (checkBoxChargingTenancyEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "charging_tenancy",
                        comboBoxChargingTenancyExpr.Text, numericUpDownChargingTenancyFrom.Value, numericUpDownChargingTenancyTo.Value);
                if (checkBoxChargingDGIEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "charging_dgi",
                        comboBoxChargingDGIExpr.Text, numericUpDownChargingDGIFrom.Value, numericUpDownChargingDGITo.Value);
                if (checkBoxRecalcTenancyEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "recalc_tenancy",
                        comboBoxRecalcTenancyExpr.Text, numericUpDownRecalcTenancyFrom.Value, numericUpDownRecalcTenancyTo.Value);
                if (checkBoxRecalcDGIEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "recalc_dgi",
                        comboBoxRecalcDGIExpr.Text, numericUpDownRecalcDGIFrom.Value, numericUpDownRecalcDGITo.Value);
                if (checkBoxPaymentTenancyEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "payment_tenancy",
                        comboBoxPaymentTenancyExpr.Text, numericUpDownPaymentTenancyFrom.Value, numericUpDownPaymentTenancyTo.Value);
                if (checkBoxPaymentDGIEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "payment_dgi",
                        comboBoxPaymentDGIExpr.Text, numericUpDownPaymentDGIFrom.Value, numericUpDownPaymentDGITo.Value);
                if (checkBoxTransferBalanceEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "transfer_balance",
                        comboBoxTransferBalanceExpr.Text, numericUpDownTransferBalanceFrom.Value, numericUpDownTransferBalanceTo.Value);
                if (checkBoxBalanceOutputEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "balance_output_total",
                        comboBoxBalanceOutputExpr.Text, numericUpDownBalanceOutputFrom.Value, numericUpDownBalanceOutputTo.Value);
                if (checkBoxBalanceOutputTenancyEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "balance_output_tenancy",
                        comboBoxBalanceOutputTenancyExpr.Text, numericUpDownBalanceOutputTenancyFrom.Value, numericUpDownBalanceOutputTenancyTo.Value);
                if (checkBoxBalanceOutputDGIEnable.Checked)
                    includedAccounts = AccountIdsByPaymentInfo(includedAccounts, "balance_output_dgi",
                        comboBoxBalanceOutputDGIExpr.Text, numericUpDownBalanceOutputDGIFrom.Value, numericUpDownBalanceOutputDGITo.Value);
            }
            if (checkBoxByClaimsChecked.Checked)
            {
                if (radioButtonWithClaims.Checked || radioButtonWithoutClaims.Checked)
                {
                    var idsByClaims = (from row in DataModel.GetInstance<EntityDataModel<Claim>>().FilterDeletedRows()
                        select row.Field<int>("id_account")).Distinct();
                    if (radioButtonWithClaims.Checked)
                    {
                        includedAccounts = DataModelHelper.Intersect(includedAccounts, idsByClaims);
                    }
                    else
                    {
                        var ids = from row in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                            select row.Field<int>("id_account");
                        includedAccounts = DataModelHelper.Intersect(includedAccounts, ids.Except(idsByClaims));
                    }
                }
                else
                if (radioButtonWithUncompletedClaims.Checked || radioButtonWithoutUncompletedClaims.Checked)
                {
                    var uncompletedClaimsPremisesInfo = ClaimsService.NotCompletedClaimsPaymentAccountsInfo().ToList();
                    var paymentsAccounts = DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows().ToList();
                    var paymentsAccountAccountDuplicate = from paymentAccountRow in paymentsAccounts
                        join claimsRow in uncompletedClaimsPremisesInfo
                            on paymentAccountRow.Field<string>("account") equals claimsRow.Account
                                                          where claimsRow.Account != null
                        select paymentAccountRow.Field<int>("id_account");
                    var paymentsAccountRawAddressDuplicate = from paymentAccountRow in paymentsAccounts
                                                          join claimsRow in uncompletedClaimsPremisesInfo
                                                              on paymentAccountRow.Field<string>("raw_address") equals claimsRow.RawAddress
                                                             where claimsRow.RawAddress != null
                                                             select paymentAccountRow.Field<int>("id_account");
                    var paymentsAccountParsedAddressDuplicate = from paymentAccountRow in paymentsAccounts
                                                             join claimsRow in uncompletedClaimsPremisesInfo
                                                                 on paymentAccountRow.Field<string>("parsed_address") equals claimsRow.ParsedAddress
                                                                where claimsRow.ParsedAddress != null
                                                             select paymentAccountRow.Field<int>("id_account");
                    var withUncomplitedClaims = paymentsAccountAccountDuplicate.Union(paymentsAccountRawAddressDuplicate)
                        .Union(paymentsAccountParsedAddressDuplicate).Distinct();
                    if (radioButtonWithUncompletedClaims.Checked)
                    {
                        includedAccounts = DataModelHelper.Intersect(includedAccounts, withUncomplitedClaims);
                    }
                    else
                    {
                        var ids = from row in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                                  select row.Field<int>("id_account");
                        includedAccounts = DataModelHelper.Intersect(includedAccounts, ids.Except(withUncomplitedClaims));
                    }
                }   
            }
            if (includedAccounts == null) return filter == "" ? "0 = 1" : filter;
            if (!string.IsNullOrEmpty(filter.Trim()))
                filter += " AND ";
            var processesFilter = BuildFilter(includedAccounts, "id_account");
            if (!string.IsNullOrEmpty(processesFilter))
                filter += "(" + processesFilter + ")";
            return filter == "" ? "0 = 1" : filter;
        }

        private IEnumerable<int> AccountIdsByPaymentInfo(IEnumerable<int> includedAccounts, string field, string rawOperator, decimal from, decimal to)
        {
            var op = ConvertDisplayEqExprToSql(rawOperator);
            var format = "{0} {1} {2}";
            if (op == "BETWEEN")
            {
                format = "{0} {1} {2} AND {3}";
            }
            if (checkBoxDateEnable.Checked)
                format += string.Format(" AND date {0} STR_TO_DATE('{1}','%d.%m.%Y')", ConvertDisplayEqExprToSql(comboBoxDateExpr.Text), dateTimePickerDate.Value.ToString("dd.MM.yyyy"));
            includedAccounts = DataModelHelper.Intersect(includedAccounts,
                PaymentService.GetAccountIdsByPaymentFilter(
                    string.Format(format, field, op, from.ToString(CultureInfo.InvariantCulture), to.ToString(CultureInfo.InvariantCulture))));
            return includedAccounts;
        }

        private static string BuildFilter(IEnumerable<int> ids, string fieldName)
        {
            var startId = -1;
            var count = 0;
            var filter = "";
            var entropicPremisesIds = new List<int>();
            foreach (var id in ids.Union(new List<int> { -1 }))
            {
                if (id != startId + count)
                {
                    if (count < 5)
                    {
                        if (startId != -1)
                            for (var i = 0; i < count; i++)
                                entropicPremisesIds.Add(startId + i);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(filter))
                            filter += " OR ";
                        filter += string.Format("({1} <= {0} AND {0} <= {2})", fieldName, startId, startId + count - 1);
                    }
                    startId = id;
                    count = 1;
                }
                else
                    count++;
            }
            string entropicPIdsStr;
            if (entropicPremisesIds.Count > 1000)
            {
                entropicPIdsStr = ids.Aggregate("", (current, premisesId) => current + (premisesId + ","));
                filter = "";
            }
            else
                entropicPIdsStr = entropicPremisesIds.Aggregate("",
                    (current, premisesId) => current + (premisesId + ","));
            entropicPIdsStr = entropicPIdsStr.Trim(',');
            if (string.IsNullOrEmpty(entropicPIdsStr))
                return string.IsNullOrEmpty(filter) ? string.Format("{0} IN (0)", fieldName) : filter;
            if (!string.IsNullOrEmpty(filter))
                filter += " OR ";
            filter += string.Format("{0} IN ({1})", fieldName, entropicPIdsStr);
            return filter;
        }

        private static string ConvertDisplayEqExprToSql(string expr)
        {
            switch (expr)
            {
                case "=": return "=";
                case "≥": return ">=";
                case "≤": return "<=";
                case "между":
                    return "BETWEEN";
                default:
                    throw new ViewportException("Неизвестный знак сравнения дат");
            }
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (checkBoxCRNEnable.Checked && string.IsNullOrEmpty(textBoxCRN.Text.Trim()))
            {
                MessageBox.Show(@"Укажите СРН или уберите галочку фильтрации по СРН", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxCRN.Focus();
                return;
            }
            if (checkBoxAccountEnable.Checked && string.IsNullOrEmpty(textBoxAccount.Text.Trim()))
            {
                MessageBox.Show(@"Укажите лицевой счет или уберите галочку фильтрации по лицевому счету", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxAccount.Focus();
                return;
            }
            if (checkBoxTenantSNPEnable.Checked && string.IsNullOrEmpty(textBoxTenantSNP.Text.Trim()))
            {
                MessageBox.Show(@"Укажите ФИО нанимателя или уберите галочку фильтрации по ФИО нанемателя", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxTenantSNP.Focus();
                return;
            }
            if (checkBoxRawAddressEnable.Checked && string.IsNullOrEmpty(textBoxRawAddress.Text.Trim()))
            {
                MessageBox.Show(@"Укажите адрес по БКС или уберите галочку фильтрации по адресу БКС", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxRawAddress.Focus();
                return;
            }
            if (checkBoxStreetEnable.Checked && comboBoxStreet.SelectedValue == null)
            {
                MessageBox.Show(@"Укажите улицу или уберите галочку фильтрации по улице", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return;
            }
            if (checkBoxHouseEnable.Checked && string.IsNullOrEmpty(textBoxHouse.Text.Trim()))
            {
                MessageBox.Show(@"Укажите номер дома или уберите галочку фильтрации по номеру дома", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if (checkBoxPremisesNumEnable.Checked && string.IsNullOrEmpty(textBoxPremisesNum.Text.Trim()))
            {
                MessageBox.Show(@"Укажите номер помещения или уберите галочку фильтрации по номеру помещения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNum.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        private void checkBoxCRNEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCRN.Enabled = checkBoxCRNEnable.Checked;
        }

        private void checkBoxAccountEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAccount.Enabled = checkBoxAccountEnable.Checked;
        }

        private void checkBoxTenantSNPEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTenantSNP.Enabled = checkBoxTenantSNPEnable.Checked;
        }

        private void checkBoxRawAddressEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRawAddress.Enabled = checkBoxRawAddressEnable.Checked;
        }

        private void checkBoxStreetEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStreet.Enabled = checkBoxStreetEnable.Checked;
        }

        private void checkBoxHouseEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxHouse.Enabled = checkBoxHouseEnable.Checked;
        }

        private void checkBoxPremisesNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPremisesNum.Enabled = checkBoxPremisesNumEnable.Checked;
        }

        private void checkBoxDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxDateExpr.Enabled = dateTimePickerDate.Enabled = checkBoxDateEnable.Checked;
        }

        private void checkBoxBalanceInputEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBalanceInputExpr.Enabled =
                numericUpDownBalanceInputFrom.Enabled =
                    numericUpDownBalanceInputTo.Enabled = checkBoxBalanceInputEnable.Checked;
        }

        private void checkBoxBalanceInputTenancyEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBalanceInputTenancyExpr.Enabled =
                numericUpDownBalanceInputTenancyFrom.Enabled =
                    numericUpDownBalanceInputTenancyTo.Enabled = checkBoxBalanceInputTenancyEnable.Checked;
        }

        private void checkBoxBalanceInputDGIEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBalanceInputDGIExpr.Enabled =
                numericUpDownBalanceInputDGIFrom.Enabled =
                    numericUpDownBalanceInputDGITo.Enabled = checkBoxBalanceInputDGIEnable.Checked;
        }

        private void checkBoxChargingEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxChargingExpr.Enabled =
                numericUpDownChargingFrom.Enabled =
                    numericUpDownChargingTo.Enabled = checkBoxChargingEnable.Checked;
        }

        private void checkBoxChargingTenancyEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxChargingTenancyExpr.Enabled =
                numericUpDownChargingTenancyFrom.Enabled =
                    numericUpDownChargingTenancyTo.Enabled = checkBoxChargingTenancyEnable.Checked;
        }

        private void checkBoxChargingDGIEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxChargingDGIExpr.Enabled =
                numericUpDownChargingDGIFrom.Enabled =
                    numericUpDownChargingDGITo.Enabled = checkBoxChargingDGIEnable.Checked;
        }

        private void checkBoxRecalcTenancyEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRecalcTenancyExpr.Enabled =
                numericUpDownRecalcTenancyFrom.Enabled =
                    numericUpDownRecalcTenancyTo.Enabled = checkBoxRecalcTenancyEnable.Checked;
        }

        private void checkBoxRecalcDGIEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRecalcDGIExpr.Enabled =
                numericUpDownRecalcDGIFrom.Enabled =
                    numericUpDownRecalcDGITo.Enabled = checkBoxRecalcDGIEnable.Checked;
        }

        private void checkBoxTransferBalanceEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxTransferBalanceExpr.Enabled =
                numericUpDownTransferBalanceFrom.Enabled =
                    numericUpDownTransferBalanceTo.Enabled = checkBoxTransferBalanceEnable.Checked;
        }

        private void checkBoxPaymentTenancyEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxPaymentTenancyExpr.Enabled =
                numericUpDownPaymentTenancyFrom.Enabled =
                    numericUpDownPaymentTenancyTo.Enabled = checkBoxPaymentTenancyEnable.Checked;
        }

        private void checkBoxPaymentDGIEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxPaymentDGIExpr.Enabled =
                numericUpDownPaymentDGIFrom.Enabled =
                    numericUpDownPaymentDGITo.Enabled = checkBoxPaymentDGIEnable.Checked;
        }

        private void checkBoxBalanceOutputEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBalanceOutputExpr.Enabled =
                numericUpDownBalanceOutputFrom.Enabled =
                    numericUpDownBalanceOutputTo.Enabled = checkBoxBalanceOutputEnable.Checked;
        }

        private void checkBoxBalanceTenancyOutputEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBalanceOutputTenancyExpr.Enabled =
                numericUpDownBalanceOutputTenancyFrom.Enabled =
                    numericUpDownBalanceOutputTenancyTo.Enabled = checkBoxBalanceOutputTenancyEnable.Checked;
        }

        private void checkBoxBalanceDGIOutputEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBalanceOutputDGIExpr.Enabled =
                numericUpDownBalanceOutputDGIFrom.Enabled =
                    numericUpDownBalanceOutputDGITo.Enabled = checkBoxBalanceOutputDGIEnable.Checked;
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

        private void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
        }

        private void checkBoxByClaimsChecked_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = checkBoxByClaimsChecked.Checked;
        }

        private void checkBoxRegionEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegion.Enabled = checkBoxRegionEnable.Checked;
        }
    }
}
