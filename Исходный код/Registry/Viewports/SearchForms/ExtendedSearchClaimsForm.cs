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
using Settings;

namespace Registry.Viewport.SearchForms
{
    internal partial class ExtendedSearchClaimsForm : SearchForm
    {

        public ExtendedSearchClaimsForm()
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
            comboBoxAtDateExpr.SelectedIndex = 2;
            comboBoxStartDeptPeriodExpr.SelectedIndex = 2;
            comboBoxEndDeptPeriodExpr.SelectedIndex = 2;
            comboBoxAmmountDGIExpr.SelectedIndex = 2;
            comboBoxAmmountTenancyExpr.SelectedIndex = 2;
            comboBoxDateStartStateExpr.SelectedIndex = 2;
            comboBoxClaimDirectionDateExpr.SelectedIndex = 2;
            comboBoxCourtOrderDateExpr.SelectedIndex = 2;
            comboBoxObtainingCourtOrderDateExpr.SelectedIndex = 2;

            textBoxBksRequester.Text = UserDomain.Current.DisplayName;
            textBoxTransferedToLegalDepartmentWho.Text = UserDomain.Current.DisplayName;
            textBoxAcceptedByLegalDepartmentWho.Text = UserDomain.Current.DisplayName;

            HandleHotKeys(Controls, vButtonSearch_Click);
        }

        internal override string GetFilter()
        {
            var filter = "";
            var includedClaims = ClaimIdsByClaimStateInfo();
            IEnumerable<int> includedAccounts = null;
            if (checkBoxBksRequesterEnable.Checked && !string.IsNullOrEmpty(textBoxBksRequester.Text.Trim()))
            {
                var claims =
                    ClaimsService.ClaimIdsByStateCondition(
                        r => (r.Field<string>("bks_requester") ?? "").ToUpperInvariant()
                                .Contains(textBoxBksRequester.Text.Trim().ToUpperInvariant()) && r.Field<int>("id_state_type") == 1);
                includedClaims = DataModelHelper.Intersect(includedClaims, claims);
            }
            if (checkBoxAcceptedByLegalDepartmentWhoEnable.Checked && !string.IsNullOrEmpty(textBoxAcceptedByLegalDepartmentWho.Text.Trim()))
            {
                var claims =
                    ClaimsService.ClaimIdsByStateCondition(
                        r => (r.Field<string>("accepted_by_legal_department_who") ?? "").ToUpperInvariant()
                                .Contains(textBoxAcceptedByLegalDepartmentWho.Text.Trim().ToUpperInvariant()) && r.Field<int>("id_state_type") == 3);
                includedClaims = DataModelHelper.Intersect(includedClaims, claims);
            }
            if (checkBoxTransferedToLegalDepartmentWhoEnable.Checked && !string.IsNullOrEmpty(textBoxTransferedToLegalDepartmentWho.Text.Trim()))
            {
                var claims =
                    ClaimsService.ClaimIdsByStateCondition(
                        r => (r.Field<string>("transfer_to_legal_department_who") ?? "").ToUpperInvariant()
                                .Contains(textBoxTransferedToLegalDepartmentWho.Text.Trim().ToUpperInvariant()) && r.Field<int>("id_state_type") == 2);
                includedClaims = DataModelHelper.Intersect(includedClaims, claims);
            }
            if (checkBoxAccountEnable.Checked && !string.IsNullOrEmpty(textBoxAccount.Text.Trim()))
            {
                var accounts =
                    (from accountRow in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                     where accountRow.Field<string>("account") != null && accountRow.Field<string>("account").Contains(textBoxAccount.Text.Trim())
                     select accountRow).ToList();
                if (accounts.Any())
                {
                    accounts =
                        accounts.Concat(
                            from accountRow in
                                DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                            join selAccountRow in accounts
                                on accountRow.Field<string>("raw_address") equals
                                selAccountRow.Field<string>("raw_address")
                            where selAccountRow.Field<string>("raw_address") != null
                            select accountRow).ToList();
                    accounts =
                        accounts.Concat(
                            from accountRow in
                                DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                            join selAccountRow in accounts
                                on accountRow.Field<string>("parsed_address") equals
                                selAccountRow.Field<string>("parsed_address")
                            where selAccountRow.Field<string>("parsed_address") != null
                            select accountRow).ToList();
                }
                includedAccounts = DataModelHelper.Intersect(null, accounts.Select(r => r.Field<int>("id_account")));
            }
            if (checkBoxSRNEnable.Checked && !string.IsNullOrEmpty(textBoxSRN.Text.Trim()))
            {
                var accounts = from accountRow in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                               where accountRow.Field<string>("crn") != null && accountRow.Field<string>("crn").Contains(textBoxSRN.Text.Trim())
                               select accountRow.Field<int>("id_account");
                includedAccounts = DataModelHelper.Intersect(includedAccounts, accounts);
            }
            if (checkBoxClaimIdChecked.Checked)
            {
                includedClaims = DataModelHelper.Intersect(includedClaims, new List<int> { (int)numericUpDownClaimId.Value });
            }
            if (checkBoxAtDateChecked.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += BuildFilter("at_date", comboBoxAtDateExpr.Text, dateTimePickerAtDateFrom.Value.Date,
                    dateTimePickerAtDateTo.Value.Date);
            }
            if (checkBoxStartDeptPeriodChecked.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += BuildFilter("start_dept_period", comboBoxStartDeptPeriodExpr.Text, dateTimePickerStartDeptPeriodFrom.Value.Date,
                    dateTimePickerStartDeptPeriodTo.Value.Date);
            }
            if (checkBoxEndDeptPeriodChecked.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += BuildFilter("end_dept_period", comboBoxEndDeptPeriodExpr.Text, dateTimePickerEndDeptPeriodFrom.Value.Date,
                    dateTimePickerEndDeptPeriodTo.Value.Date);
            }
            if (checkBoxAmmountTenancyChecked.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += BuildFilter("amount_tenancy", comboBoxAmmountTenancyExpr.Text, numericUpDownAmmountTenancyFrom.Value,
                    numericUpDownAmmountTenancyTo.Value);
            }
            if (checkBoxAmmountDGIChecked.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += BuildFilter("amount_dgi", comboBoxAmmountDGIExpr.Text, numericUpDownAmmountDGIFrom.Value,
                    numericUpDownAmmountDGITo.Value);
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

        private IEnumerable<int> ClaimIdsByClaimStateInfo()
        {
            IEnumerable<int> includedClaims = null;
            if (!checkBoxStateEnable.Checked && !checkBoxDateStartStateEnable.Checked &&
                !checkBoxClaimDirectionDateEnable.Checked && !checkBoxCourtOrderDateEnable.Checked &&
                !checkBoxObtainingCourtOrderDateEnable.Checked && !checkBoxCourtOrderNumEnable.Checked)
            {
                return null;
            }
            if (checkBoxLastState.Checked)
            {
                if (checkBoxStateEnable.Checked && comboBoxState.SelectedValue != null)
                {
                    var lastStates = CalcDataModel.GetInstance<CalcDataModelLastClaimStates>();
                    var lastStateTypes = from lastStateRow in lastStates.FilterDeletedRows()
                        where lastStateRow.Field<int?>("id_state_type") == (int?) comboBoxState.SelectedValue
                        select lastStateRow.Field<int>("id_claim");
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
            }
            else
            {
                var claims = from row in DataModel.GetInstance<EntityDataModel<ClaimState>>().FilterDeletedRows()
                    where
                        (!checkBoxStateEnable.Checked || row.Field<int?>("id_state_type") == (int?) comboBoxState.SelectedValue) &&
                        (!checkBoxDateStartStateEnable.Checked ||
                         DateSatisfiesExpression(
                             row["date_start_state"] == DBNull.Value ? null : (DateTime?) row["date_start_state"],
                             comboBoxDateStartStateExpr.Text,
                             dateTimePickerDateStartStateFrom.Value.Date,
                             dateTimePickerDateStartStateTo.Value.Date))
                    select row.Field<int>("id_claim");
                includedClaims = DataModelHelper.Intersect(null, claims);
            }
            int idStateType;
            if (comboBoxState.SelectedValue == null ||
                !int.TryParse(comboBoxState.SelectedValue.ToString(), out idStateType) || idStateType != 4)
                return includedClaims;
            if (checkBoxStateEnable.Checked && (int?)comboBoxState.SelectedValue == 4)
            {
                if (checkBoxClaimDirectionDateEnable.Checked)
                {
                    var claims = ClaimsService.ClaimIdsByStateCondition(r =>
                        DateSatisfiesExpression(r.Field<DateTime?>("claim_direction_date"),
                            comboBoxClaimDirectionDateExpr.Text,
                            dateTimePickerClaimDirectionDateFrom.Value.Date,
                            dateTimePickerClaimDirectionDateTo.Value.Date) && r.Field<int>("id_state_type") == 4);
                    includedClaims = DataModelHelper.Intersect(includedClaims, claims);
                }
                if (checkBoxCourtOrderDateEnable.Checked)
                {
                    var claims = ClaimsService.ClaimIdsByStateCondition(r =>
                        DateSatisfiesExpression(r.Field<DateTime?>("court_order_date"),
                            comboBoxCourtOrderDateExpr.Text,
                            dateTimePickerCourtOrderDateFrom.Value.Date,
                            dateTimePickerCourtOrderDateTo.Value.Date) && r.Field<int>("id_state_type") == 4);
                    includedClaims = DataModelHelper.Intersect(includedClaims, claims);
                }
                if (checkBoxObtainingCourtOrderDateEnable.Checked)
                {
                    var claims = ClaimsService.ClaimIdsByStateCondition(r =>
                        DateSatisfiesExpression(r.Field<DateTime?>("obtaining_court_order_date"),
                            comboBoxObtainingCourtOrderDateExpr.Text,
                            dateTimePickerObtainingCourtOrderDateFrom.Value.Date,
                            dateTimePickerObtainingCourtOrderDateTo.Value.Date) && r.Field<int>("id_state_type") == 4);
                    includedClaims = DataModelHelper.Intersect(includedClaims, claims);
                }
                if (checkBoxCourtOrderNumEnable.Checked)
                {
                    var claims = ClaimsService.ClaimIdsByStateCondition(r =>
                        r.Field<string>("court_order_num") != null &&
                        r.Field<string>("court_order_num").Contains(textBoxCourtOrderNum.Text.Trim()) && r.Field<int>("id_state_type") == 4);
                    includedClaims = DataModelHelper.Intersect(includedClaims, claims);
                }
            }
            return includedClaims;
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

        private string BuildFilter(string field, string rawOperator, decimal from, decimal to)
        {
            var op = ConvertDisplayEqExprToSql(rawOperator);
            var format = "{0} {1} {2}";
            if (op != "BETWEEN")
                return string.Format(format, field, op, from.ToString(CultureInfo.InvariantCulture),
                    to.ToString(CultureInfo.InvariantCulture));
            format = "{0} >= {1} AND {0} <= {2}";
            return string.Format(format, field, from.ToString(CultureInfo.InvariantCulture), to.ToString(CultureInfo.InvariantCulture));
        }

        private string BuildFilter(string field, string rawOperator, DateTime from, DateTime to)
        {
            var op = ConvertDisplayEqExprToSql(rawOperator);
            var format = "{0} {1} '{2}'";
            if (op != "BETWEEN")
                return string.Format(format, field, op,
                    from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            format = "{0} >= '{1}' AND {0} <= '{2}'";
            return string.Format(format, field,
                from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
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
            if (checkBoxAccountEnable.Checked && string.IsNullOrEmpty(textBoxAccount.Text.Trim()))
            {
                MessageBox.Show(@"Укажите лицевой счет или уберите галочку фильтрации по лицевому счету",@"Ошибка",
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
            if (checkBoxCourtOrderNumEnable.Checked && string.IsNullOrEmpty(textBoxCourtOrderNum.Text.Trim()))
            {
                MessageBox.Show(@"Укажите номер судебного приказа или уберите галочку фильтрации по номеру судебного приказа", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxCourtOrderNum.Focus();
                return;
            }
            if (checkBoxBksRequesterEnable.Checked &&
                string.IsNullOrEmpty(textBoxBksRequester.Text.Trim()))
            {
                MessageBox.Show(@"Введите часть ФИО сотрудника, сделавшего запрос в БКС, или уберите галочку фильтрации по данному полю", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxBksRequester.Focus();
                return;
            }
            if (checkBoxTransferedToLegalDepartmentWhoEnable.Checked &&
                string.IsNullOrEmpty(textBoxTransferedToLegalDepartmentWho.Text.Trim()))
            {
                MessageBox.Show(@"Введите часть ФИО сотрудника, передавшего работу в юридический отдел, или уберите галочку фильтрации по данному полю", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxTransferedToLegalDepartmentWho.Focus();
                return;
            }
            if (checkBoxAcceptedByLegalDepartmentWhoEnable.Checked &&
                string.IsNullOrEmpty(textBoxAcceptedByLegalDepartmentWho.Text.Trim()))
            {
                MessageBox.Show(@"Введите часть ФИО сотрудника, принявшего работу в юридический отдел, или уберите галочку фильтрации по данному полю", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxAcceptedByLegalDepartmentWho.Focus();
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
            if (!checkBoxStateEnable.Checked)
            {
                SetExtStatePropertiesVisibility(false);
            }
            else
            {
                comboBoxState_SelectedValueChanged(comboBoxState, new EventArgs());
            }
        }

        private void checkBoxAccountChecked_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAccount.Enabled = checkBoxAccountEnable.Checked;
        }

        private void checkBoxAtDateChecked_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerAtDateFrom.Enabled = checkBoxAtDateChecked.Checked;
            dateTimePickerAtDateTo.Enabled = checkBoxAtDateChecked.Checked;
            comboBoxAtDateExpr.Enabled = checkBoxAtDateChecked.Checked;
        }

        private void checkBoxStartDeptPeriodChecked_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerStartDeptPeriodFrom.Enabled = checkBoxStartDeptPeriodChecked.Checked;
            dateTimePickerStartDeptPeriodTo.Enabled = checkBoxStartDeptPeriodChecked.Checked;
            comboBoxStartDeptPeriodExpr.Enabled = checkBoxStartDeptPeriodChecked.Checked;
        }

        private void checkBoxEndDeptPeriodChecked_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerEndDeptPeriodFrom.Enabled = checkBoxEndDeptPeriodChecked.Checked;
            dateTimePickerEndDeptPeriodTo.Enabled = checkBoxEndDeptPeriodChecked.Checked;
            comboBoxEndDeptPeriodExpr.Enabled = checkBoxEndDeptPeriodChecked.Checked;
        }

        private void checkBoxAmmountTenancyChecked_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownAmmountTenancyFrom.Enabled = checkBoxAmmountTenancyChecked.Checked;
            numericUpDownAmmountTenancyTo.Enabled = checkBoxAmmountTenancyChecked.Checked;
            comboBoxAmmountTenancyExpr.Enabled = checkBoxAmmountTenancyChecked.Checked;
        }

        private void checkBoxAmmountDGIChecked_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownAmmountDGIFrom.Enabled = checkBoxAmmountDGIChecked.Checked;
            numericUpDownAmmountDGITo.Enabled = checkBoxAmmountDGIChecked.Checked;
            comboBoxAmmountDGIExpr.Enabled = checkBoxAmmountDGIChecked.Checked;
        }

        private void checkBoxClaimIdChecked_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownClaimId.Enabled = checkBoxClaimIdChecked.Checked;
        }

        private void checkBoxDateStartStateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerDateStartStateFrom.Enabled = checkBoxDateStartStateEnable.Checked;
            dateTimePickerDateStartStateTo.Enabled = checkBoxDateStartStateEnable.Checked;
            comboBoxDateStartStateExpr.Enabled = checkBoxDateStartStateEnable.Checked;
        }

        private void checkBoxSRNChecked_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSRN.Enabled = checkBoxSRNEnable.Checked;
        }

        private void checkBoxClaimDirectionDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerClaimDirectionDateFrom.Enabled = checkBoxClaimDirectionDateEnable.Checked;
            dateTimePickerClaimDirectionDateTo.Enabled = checkBoxClaimDirectionDateEnable.Checked;
            comboBoxClaimDirectionDateExpr.Enabled = checkBoxClaimDirectionDateEnable.Checked;
        }

        private void checkBoxAcceptedByLegalDepartmentWhoEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAcceptedByLegalDepartmentWho.Enabled = checkBoxAcceptedByLegalDepartmentWhoEnable.Checked;
        }

        private void checkBoxCourtOrderDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerCourtOrderDateFrom.Enabled = checkBoxCourtOrderDateEnable.Checked;
            dateTimePickerCourtOrderDateTo.Enabled = checkBoxCourtOrderDateEnable.Checked;
            comboBoxCourtOrderDateExpr.Enabled = checkBoxCourtOrderDateEnable.Checked;
        }

        private void checkBoxObtainingCourtOrderDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerObtainingCourtOrderDateFrom.Enabled = checkBoxObtainingCourtOrderDateEnable.Checked;
            dateTimePickerObtainingCourtOrderDateTo.Enabled = checkBoxObtainingCourtOrderDateEnable.Checked;
            comboBoxObtainingCourtOrderDateExpr.Enabled = checkBoxObtainingCourtOrderDateEnable.Checked;
        }

        private void comboBoxState_SelectedValueChanged(object sender, EventArgs e)
        {
            int idStateType;
            if (comboBoxState.SelectedValue != null &&
                int.TryParse(comboBoxState.SelectedValue.ToString(), out idStateType) && idStateType == 4)
            {
                SetExtStatePropertiesVisibility(true);
            }
            else
            {
                SetExtStatePropertiesVisibility(false);
            }
        }

        private void SetExtStatePropertiesVisibility(bool visibility)
        {
            dateTimePickerClaimDirectionDateFrom.Visible = dateTimePickerClaimDirectionDateTo.Visible =
            comboBoxClaimDirectionDateExpr.Visible = dateTimePickerCourtOrderDateFrom.Visible =
            dateTimePickerCourtOrderDateTo.Visible = comboBoxCourtOrderDateExpr.Visible =
            dateTimePickerObtainingCourtOrderDateFrom.Visible = dateTimePickerObtainingCourtOrderDateTo.Visible =
            comboBoxObtainingCourtOrderDateExpr.Visible = label8.Visible = label9.Visible = label10.Visible =
            checkBoxObtainingCourtOrderDateEnable.Visible = checkBoxCourtOrderDateEnable.Visible = checkBoxClaimDirectionDateEnable.Visible = 
            checkBoxCourtOrderNumEnable.Visible = textBoxCourtOrderNum.Visible = visibility;
            Height = visibility ? 495 : 436;
            groupBox1.Height = visibility ? 313 : 141;
        }

        private void checkBoxTransferedToLegalDepartmentWhoEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTransferedToLegalDepartmentWho.Enabled = checkBoxTransferedToLegalDepartmentWhoEnable.Checked;
        }

        private void checkBoxBksRequesterEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBksRequester.Enabled = checkBoxBksRequesterEnable.Checked;
        }

        private void checkBoxCourtOrderNumEnable_CheckStateChanged(object sender, EventArgs e)
        {
            textBoxCourtOrderNum.Enabled = checkBoxCourtOrderNumEnable.Checked;
        }
    }
}
