using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;

namespace Registry.Viewport.SearchForms
{
    public partial class ExtendedSearchClaimsForm : SearchForm
    {

        public ExtendedSearchClaimsForm()
        {
            InitializeComponent();

            DataModel.GetInstance(DataModelType.ClaimStatesDataModel).Select();
            comboBoxLastState.DataSource = new BindingSource
            {
                DataSource = DataModel.DataSet,
                DataMember = "claim_state_types"
            };
            comboBoxLastState.DisplayMember = "state_type";
            comboBoxLastState.ValueMember = "id_state_type";
            comboBoxAtDateExpr.SelectedIndex = 2;
            comboBoxStartDeptPeriodExpr.SelectedIndex = 2;
            comboBoxEndDeptPeriodExpr.SelectedIndex = 2;
            comboBoxAmmountDGIExpr.SelectedIndex = 2;
            comboBoxAmmountTenancyExpr.SelectedIndex = 2;

            foreach (Control control in Controls)
            {
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButtonSearch_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            DialogResult = DialogResult.Cancel;
                };
            }
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedClaims = null;
            IEnumerable<int> includedAccounts = null;

            if (checkBoxLastStateChecked.Checked && comboBoxLastState.SelectedValue != null)
            {
                var claimStatesDataModel = DataModel.GetInstance(DataModelType.ClaimStatesDataModel);
                var lastStates = from stateRow in claimStatesDataModel.FilterDeletedRows()
                                 group stateRow.Field<int?>("id_state") by stateRow.Field<int>("id_claim") into gs
                                 select new
                                 {
                                     id_claim = gs.Key,
                                     id_state = gs.Max()
                                 };
                var lastStateTypes = from lastStateRow in lastStates
                    join stateRow in claimStatesDataModel.FilterDeletedRows()
                        on lastStateRow.id_state equals stateRow.Field<int?>("id_state")
                    where stateRow.Field<int?>("id_state_type") == (int?) comboBoxLastState.SelectedValue
                    select stateRow.Field<int>("id_claim");
                includedClaims = DataModelHelper.Intersect(null, lastStateTypes);
            }
            if (checkBoxAccountChecked.Checked && !string.IsNullOrEmpty(textBoxAccount.Text.Trim()))
            {
                var accounts =
                    from accountRow in DataModel.GetInstance(DataModelType.PaymentsAccountsDataModel).FilterDeletedRows()
                    where accountRow.Field<string>("account").Contains(textBoxAccount.Text)
                    select accountRow.Field<int>("id_account");
                includedAccounts = DataModelHelper.Intersect(null, accounts);
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

        private string BuildFilter(string field, string rawOperator, decimal from, decimal to)
        {
            var op = ConvertDisplayEqExprToSql(rawOperator);
            var format = "{0} {1} {2}";
            if (op != "BETWEEN")
                return string.Format(format, field, op, @from.ToString(CultureInfo.InvariantCulture),
                    to.ToString(CultureInfo.InvariantCulture));
            format = "{0} >= {1} AND {0} <= {2}";
            return string.Format(format, field, @from.ToString(CultureInfo.InvariantCulture), to.ToString(CultureInfo.InvariantCulture));
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
                return;
            }
            if (checkBoxLastStateChecked.Checked && comboBoxLastState.SelectedValue == null)
            {
                MessageBox.Show(@"Выберите состояние исковой работы или уберите галочку фильтрации по последнему состоянию исковой работы", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void checkBoxLastStateChecked_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxLastState.Enabled = checkBoxLastStateChecked.Checked;
        }

        private void checkBoxAccountChecked_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAccount.Enabled = checkBoxAccountChecked.Checked;
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
    }
}
