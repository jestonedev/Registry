using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;

namespace Registry.Viewport.SearchForms
{
    internal partial class SimpleSearchClaimsForm : SearchForm
    {

        public SimpleSearchClaimsForm()
        {
            InitializeComponent();

            DataModel.GetInstance<EntityDataModel<ClaimState>>().Select();
            comboBoxState.DataSource = new BindingSource
            {
                DataSource = DataStorage.DataSet,
                DataMember = "claim_state_types"
            };
            comboBoxState.DisplayMember = "state_type";
            comboBoxState.ValueMember = "id_state_type";
            comboBoxDateStartStateExpr.SelectedIndex = 2;
            comboBoxBalanceOutputDgiExpr.SelectedIndex = 2;
            comboBoxBalanceOutputTenancyExpr.SelectedIndex = 2;
            comboBoxBalanceOutputPenaltiesExpr.SelectedIndex = 2;

            HandleHotKeys(Controls, vButtonSearch_Click);
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedClaims = null;
            IEnumerable<int> includedAccounts = null;
            if (checkBoxLastState.Checked)
            {
                if (checkBoxStateEnable.Checked && comboBoxState.SelectedValue != null)
                {
                    var claimStatesDataModel = DataModel.GetInstance<EntityDataModel<ClaimState>>();
                    var lastStates = from stateRow in claimStatesDataModel.FilterDeletedRows()
                        group stateRow.Field<int?>("id_state") by stateRow.Field<int>("id_claim")
                        into gs
                        select new
                        {
                            id_claim = gs.Key,
                            id_state = gs.Max()
                        };
                    var lastStateTypes = from lastStateRow in lastStates
                        join stateRow in claimStatesDataModel.FilterDeletedRows()
                            on lastStateRow.id_state equals stateRow.Field<int?>("id_state")
                        where stateRow.Field<int?>("id_state_type") == (int?) comboBoxState.SelectedValue
                        select stateRow.Field<int>("id_claim");
                    includedClaims = DataModelHelper.Intersect(null, lastStateTypes);
                }
                if (checkBoxDateStartStateEnable.Checked)
                {
                    var lastStateBindingSource = new BindingSource
                    {
                        DataSource = CalcDataModel.GetInstance<CalcDataModelLastClaimStates>().Select(),
                        Filter = BuildFilter("date_start_state", comboBoxDateStartStateExpr.Text,
                            dateTimePickerDateStartStateFrom.Value.Date,
                            dateTimePickerDateStartStateTo.Value.Date)
                    };
                    var claimsByDateStartState = new List<int>();
                    for (var i = 0; i < lastStateBindingSource.Count; i++)
                    {
                        var idClaim = (int?) ((DataRowView) lastStateBindingSource[i])["id_claim"];
                        if (idClaim != null)
                            claimsByDateStartState.Add(idClaim.Value);
                    }
                    includedClaims = DataModelHelper.Intersect(includedClaims, claimsByDateStartState);
                }
            } else if (checkBoxStateEnable.Checked || checkBoxDateStartStateEnable.Checked)
            {

                var claims = from row in DataModel.GetInstance<EntityDataModel<ClaimState>>().FilterDeletedRows()
                    where
                        (!checkBoxStateEnable.Checked ||
                         row.Field<int?>("id_state_type") == (int?) comboBoxState.SelectedValue) &&
                        (!checkBoxDateStartStateEnable.Checked ||
                         DateSatisfiesExpression(
                             row["date_start_state"] == DBNull.Value ? null : (DateTime?) row["date_start_state"],
                             comboBoxDateStartStateExpr.Text,
                             dateTimePickerDateStartStateFrom.Value.Date,
                             dateTimePickerDateStartStateTo.Value.Date))
                    select row.Field<int>("id_claim");
                includedClaims = DataModelHelper.Intersect(null, claims);
            }
            if (checkBoxAccountChecked.Checked && !string.IsNullOrEmpty(textBoxAccount.Text.Trim()))
            {
                var accounts = from accountRow in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                               where accountRow.Field<string>("account").Contains(textBoxAccount.Text.Trim())
                               select accountRow.Field<int>("id_account");
                includedAccounts = DataModelHelper.Intersect(null, accounts);
            }
            if (checkBoxSRNEnable.Checked && !string.IsNullOrEmpty(textBoxSRN.Text.Trim()))
            {
                var accounts = from accountRow in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                               where accountRow.Field<string>("crn").Contains(textBoxSRN.Text.Trim())
                               select accountRow.Field<int>("id_account");
                includedAccounts = DataModelHelper.Intersect(includedAccounts, accounts);
            }
            if (checkBoxRawAddressEnabled.Checked && !string.IsNullOrEmpty(textBoxRawAddress.Text.Trim()))
            {
                var addressParts = textBoxRawAddress.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var accounts = from accountRow in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                               where addressParts.All(a => accountRow.Field<string>("raw_address").ToUpper().Contains(a.ToUpper()))
                               select accountRow.Field<int>("id_account");
                includedAccounts = DataModelHelper.Intersect(includedAccounts, accounts);
            }
            if (checkBoxAtDateChecked.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format("at_date = '{0}'", dateTimePickerAtDate.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxCourtOrderNumEnable.Checked)
            {
                var claims = ClaimsService.ClaimIdsByStateCondition(r =>
                    r.Field<string>("court_order_num") != null && 
                    r.Field<string>("court_order_num").Contains(textBoxCourtOrderNum.Text.Trim()) && r.Field<int>("id_state_type") == 4);
                includedClaims = DataModelHelper.Intersect(includedClaims, claims);
            }
            if (checkBoxBalanceOutputTenancyChecked.Checked)
            {
                var accountIds =
                    AccountIdsByFilter(BuildFilter("balance_output_tenancy", comboBoxBalanceOutputTenancyExpr.Text,
                        numericUpDownBalanceOutputTenancyFrom.Value, numericUpDownBalanceOutputTenancyTo.Value));
                includedAccounts = DataModelHelper.Intersect(includedAccounts, accountIds);
            }
            if (checkBoxBalanceOutputDgiChecked.Checked)
            {
                var accountIds =
                    AccountIdsByFilter(BuildFilter("balance_output_dgi", comboBoxBalanceOutputDgiExpr.Text,
                            numericUpDownBalanceOutputDgiFrom.Value, numericUpDownBalanceOutputDgiTo.Value));
                includedAccounts = DataModelHelper.Intersect(includedAccounts, accountIds);
            }
            if (checkBoxBalanceOutputPenaltiesChecked.Checked)
            {
                var accountIds =
                    AccountIdsByFilter(BuildFilter("balance_output_penalties", comboBoxBalanceOutputPenaltiesExpr.Text,
                            numericUpDownBalanceOutputPenaltiesFrom.Value, numericUpDownBalanceOutputPenaltiesTo.Value));
                includedAccounts = DataModelHelper.Intersect(includedAccounts, accountIds);
            }
            if (includedClaims != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_claim IN (0";
                foreach (var id in includedClaims)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            if (includedAccounts != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_account IN (0";
                foreach (var id in includedAccounts)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            return filter;
        }

        private static IEnumerable<int> AccountIdsByFilter(string filter)
        {
            var accounts = new BindingSource
            {
                DataSource = DataModel.GetInstance<PaymentsAccountsDataModel>().Select(),
                Filter = filter
            };
            var accountIds = new List<int>();
            for (var i = 0; i < accounts.Count; i++)
            {
                var idAccount = (int?)((DataRowView)accounts[i])["id_account"];
                if (idAccount != null)
                    accountIds.Add(idAccount.Value);
            }
            return accountIds;
        }

        private static bool DateSatisfiesExpression(DateTime? dateStartState, string rawOperator, DateTime dateStartStateFrom, DateTime dateStartStateTo)
        {
            if (dateStartState == null) return false;
            switch (rawOperator)
            {
                case "≥":
                    return dateStartState.Value.Date >= dateStartStateFrom;
                case "≤":
                    return dateStartState.Value.Date <= dateStartStateFrom;
                case "=":
                    return dateStartState.Value.Date == dateStartStateFrom;
                case "между":
                    return dateStartState.Value.Date >= dateStartStateFrom &&
                           dateStartState.Value.Date <= dateStartStateTo;
            }
            return false;
        }

        private string BuildFilter(string field, string rawOperator, DateTime from, DateTime to)
        {
            var op = ConvertDisplayEqExprToSql(rawOperator);
            var format = "{0} {1} '{2}'";
            if (op != "BETWEEN")
                return string.Format(format, field, op,
                    from.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture),
                    to.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            format = "{0} >= '{1}' AND {0} <= '{2}'";
            return string.Format(format, field,
                from.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture),
                to.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
        }

        private string BuildFilter(string field, string rawOperator, decimal from, decimal to)
        {
            var op = ConvertDisplayEqExprToSql(rawOperator);
            var format = "{0} {1} {2}";
            if (op != "BETWEEN")
                return string.Format(format, field, op,
                    from.ToString(CultureInfo.InvariantCulture),
                    to.ToString(CultureInfo.InvariantCulture));
            format = "{0} >= {1} AND {0} <= {2}";
            return string.Format(format, field,
                from.ToString(CultureInfo.InvariantCulture),
                to.ToString(CultureInfo.InvariantCulture));
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
            if (checkBoxAccountChecked.Checked && string.IsNullOrEmpty(textBoxAccount.Text.Trim()))
            {
                MessageBox.Show(@"Выберите лицевой счет или уберите галочку фильтрации по лицевому счету",@"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxAccount.Focus();
                return;
            }
            if (checkBoxSRNEnable.Checked && string.IsNullOrEmpty(textBoxSRN.Text.Trim()))
            {
                MessageBox.Show(@"Укажите СРН или уберите галочку фильтрации по СРН", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxSRN.Focus();
                return;
            }
            if (checkBoxRawAddressEnabled.Checked && string.IsNullOrEmpty(textBoxRawAddress.Text.Trim()))
            {
                MessageBox.Show(@"Укажите адрес по БКС или уберите галочку фильтрации по адресу БКС", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxRawAddress.Focus();
                return;
            }
            if (checkBoxStateEnable.Checked && comboBoxState.SelectedValue == null)
            {
                MessageBox.Show(@"Выберите состояние исковой работы или уберите галочку фильтрации по последнему состоянию исковой работы", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxState.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void checkBoxLastStateChecked_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxState.Enabled = checkBoxStateEnable.Checked;
        }

        private void checkBoxAccountChecked_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAccount.Enabled = checkBoxAccountChecked.Checked;
        }

        private void checkBoxAtDateChecked_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerAtDate.Enabled = checkBoxAtDateChecked.Checked;
        }

        private void checkBoxDateStartStateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerDateStartStateFrom.Enabled = checkBoxDateStartStateEnable.Checked;
            dateTimePickerDateStartStateTo.Enabled = checkBoxDateStartStateEnable.Checked;
            comboBoxDateStartStateExpr.Enabled = checkBoxDateStartStateEnable.Checked;
        }

        private void checkBoxSRNEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSRN.Enabled = checkBoxSRNEnable.Checked;
        }

        private void checkBoxCourtOrderNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCourtOrderNum.Enabled = checkBoxCourtOrderNumEnable.Checked;
        }

        private void checkBoxBalanceOutputTenancyChecked_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBalanceOutputTenancyExpr.Enabled = checkBoxBalanceOutputTenancyChecked.Checked;
            numericUpDownBalanceOutputTenancyFrom.Enabled = checkBoxBalanceOutputTenancyChecked.Checked;
            numericUpDownBalanceOutputTenancyTo.Enabled = checkBoxBalanceOutputTenancyChecked.Checked;
        }

        private void checkBoxBalanceOutputDgiChecked_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBalanceOutputDgiExpr.Enabled = checkBoxBalanceOutputDgiChecked.Checked;
            numericUpDownBalanceOutputDgiFrom.Enabled = checkBoxBalanceOutputDgiChecked.Checked;
            numericUpDownBalanceOutputDgiTo.Enabled = checkBoxBalanceOutputDgiChecked.Checked;
        }

        private void checkBoxBalanceOutputPenaltiesChecked_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBalanceOutputPenaltiesExpr.Enabled = checkBoxBalanceOutputPenaltiesChecked.Checked;
            numericUpDownBalanceOutputPenaltiesFrom.Enabled = checkBoxBalanceOutputPenaltiesChecked.Checked;
            numericUpDownBalanceOutputPenaltiesTo.Enabled = checkBoxBalanceOutputPenaltiesChecked.Checked;
        }

        private void checkBoxRawAddressEnabled_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRawAddress.Enabled = checkBoxRawAddressEnabled.Checked;
        }
    }
}
