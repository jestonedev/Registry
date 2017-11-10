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
    internal partial class ExtendedSearchTenancyForm : SearchForm
    {
        private readonly BindingSource vKladr;

        public ExtendedSearchTenancyForm()
        {
            InitializeComponent();

            DataModel.GetInstance<KladrStreetsDataModel>().Select();
            DataModel.GetInstance<RentTypesDataModel>().Select();
            EntityDataModel<ReasonType>.GetInstance().Select();
            var regions = DataModel.GetInstance<KladrRegionsDataModel>();

            var ds = DataStorage.DataSet;

            vKladr = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            var vRegions = new BindingSource {DataSource = regions.Select()};

            var vRentTypes = new BindingSource
            {
                DataSource = ds,
                DataMember = "rent_types"
            };

            var vTenancyReasonTypes = new BindingSource
            {
                DataSource = ds,
                DataMember = "tenancy_reason_types",
                Sort = "reason_name ASC"
            };

            comboBoxStreet.DataSource = vKladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxRentType.DataSource = vRentTypes;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";

            comboBoxRegion.DataSource = vRegions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxReasonType.DataSource = vTenancyReasonTypes;
            comboBoxReasonType.ValueMember = "id_reason_type";
            comboBoxReasonType.DisplayMember = "reason_name";

            comboBoxRegDateExpr.SelectedIndex = 2;
            comboBoxIssueDateExpr.SelectedIndex = 2;
            comboBoxBeginDateExpr.SelectedIndex = 2;
            comboBoxEndDateExpr.SelectedIndex = 2;
            comboBoxResidenceWarrDateExpr.SelectedIndex = 2;
            comboBoxProtocolDateExpr.SelectedIndex = 2;

            HandleHotKeys(Controls, vButtonSearch_Click);
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedProcesses = null;
            if (checkBoxContractNumEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "registration_num like '%{0}%'", textBoxRegistrationNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxReasonTypeEnable.Checked)
            {
                var processesIds =
                    from reason in EntityDataModel<TenancyReason>.GetInstance().FilterDeletedRows()
                    where reason.Field<int?>("id_reason_type") == (int?)comboBoxReasonType.SelectedValue
                    select reason.Field<int>("id_process");
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxReasonNumEnable.Checked)
            {
                var processesIds =
                    from reason in EntityDataModel<TenancyReason>.GetInstance().FilterDeletedRows()
                    where reason.Field<string>("reason_number") == textBoxReasonNum.Text
                    select reason.Field<int>("id_process");
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxProtocolEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "protocol_num = '{0}'", textBoxProtocolNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxRentTypeEnable.Checked && (comboBoxRentType.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "id_rent_type = {0}", comboBoxRentType.SelectedValue);
            }
            if (checkBoxRegDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "registration_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxRegDateExpr.SelectedItem.ToString()),
                        dateTimePickerRegDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxIssueDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "issue_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxIssueDateExpr.SelectedItem.ToString()),
                        dateTimePickerIssueDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxBeginDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "begin_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxBeginDateExpr.SelectedItem.ToString()),
                        dateTimePickerBeginDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxEndDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "end_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                    comboBoxEndDateExpr.SelectedItem.ToString()),
                    dateTimePickerEndDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxResidenceWarrDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "residence_warrant_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxResidenceWarrDateExpr.SelectedItem.ToString()),
                        dateTimePickerResidenceWarrDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxProtocolDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "protocol_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                    comboBoxProtocolDateExpr.SelectedItem.ToString()),
                    dateTimePickerProtocolDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxIDTenancyEnable.Checked)
                includedProcesses = DataModelHelper.Intersect(includedProcesses, new List<int> { Convert.ToInt32(numericUpDownIDTenancy.Value) });
            if (checkBoxTenantSNPEnable.Checked)
            {
                var snp = textBoxTenantSNP.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processesIds = TenancyService.TenancyProcessIdsBySnp(snp, row => row.Field<int?>("id_kinship") == 1);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxPersonSNPEnable.Checked)
            {
                var snp = textBoxPersonSNP.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processesIds = TenancyService.TenancyProcessIdsBySnp(snp, row => true);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxRegionEnable.Checked && (comboBoxRegion.SelectedValue != null))
            {
                var processesIds = TenancyService.TenancyProcessIDsByCondition(
                    row => row.Field<string>("id_street") != null && row.Field<string>("id_street").StartsWith(
                        comboBoxRegion.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase), 
                    DataModelHelper.ConditionType.BuildingCondition);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxStreetEnable.Checked && (comboBoxStreet.SelectedValue != null))
            {
                var processesIds = TenancyService.TenancyProcessIDsByCondition(
                    row => row.Field<string>("id_street") == comboBoxStreet.SelectedValue.ToString(),
                    DataModelHelper.ConditionType.BuildingCondition);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxHouseEnable.Checked)
            {
                var processesIds = TenancyService.TenancyProcessIDsByCondition(
                    row => row.Field<string>("house") == textBoxHouse.Text.Trim().Replace("'", ""), 
                    DataModelHelper.ConditionType.BuildingCondition);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxPremisesNumEnable.Checked)
            {
                var processesIds = TenancyService.TenancyProcessIDsByCondition(
                    row => row.Field<string>("premises_num") == textBoxPremisesNum.Text.Trim().Replace("'", ""), 
                    DataModelHelper.ConditionType.PremisesCondition);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (includedProcesses == null) return filter == "" ? "0 = 1" : filter;
            if (!string.IsNullOrEmpty(filter.Trim()))
                filter += " AND ";
            var processesFilter = BuildFilter(includedProcesses, "id_process");
            if (!string.IsNullOrEmpty(processesFilter))
                filter += "(" + processesFilter + ")";
            return filter == "" ? "0 = 1" : filter;
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
                default:
                    throw new ViewportException("Неизвестный знак сравнения дат");
            }
        }

        private void checkBoxIDTenancyEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDTenancy.Enabled = checkBoxIDTenancyEnable.Checked;
        }

        private void checkBoxContractNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRegistrationNum.Enabled = checkBoxContractNumEnable.Checked;
        }

        private void checkBoxReasonNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxReasonNum.Enabled = checkBoxReasonNumEnable.Checked;
        }

        private void checkBoxProtocolEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxProtocolNum.Enabled = checkBoxProtocolEnable.Checked;
        }

        private void checkBoxTenantSNPEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTenantSNP.Enabled = checkBoxTenantSNPEnable.Checked;
        }

        private void checkBoxPersonSNPEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPersonSNP.Enabled = checkBoxPersonSNPEnable.Checked;
        }

        private void checkBoxRentTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRentType.Enabled = checkBoxRentTypeEnable.Checked;
        }

        private void checkBoxRegionEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegion.Enabled = checkBoxRegionEnable.Checked;
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

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if ((checkBoxContractNumEnable.Checked) && string.IsNullOrEmpty(textBoxRegistrationNum.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер договора или уберите галочку поиска по номеру договора", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxRegistrationNum.Focus();
                return;
            }
            if ((checkBoxReasonNumEnable.Checked) && string.IsNullOrEmpty(textBoxReasonNum.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер документа-основания или уберите галочку поиска по номеру ордера на проживание", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxReasonNum.Focus();
                return;
            }
            if ((checkBoxProtocolEnable.Checked) && string.IsNullOrEmpty(textBoxProtocolNum.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер распоряжения КУМИ или уберите галочку поиска по номеру распоряжения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxProtocolNum.Focus();
                return;
            }
            if ((checkBoxTenantSNPEnable.Checked) && string.IsNullOrEmpty(textBoxTenantSNP.Text.Trim()))
            {
                MessageBox.Show(@"Введите ФИО нанимателя или уберите галочку поиска по ФИО нанимателя", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxTenantSNP.Focus();
                return;
            }
            if ((checkBoxPersonSNPEnable.Checked) && string.IsNullOrEmpty(textBoxPersonSNP.Text.Trim()))
            {
                MessageBox.Show(@"Введите ФИО участника найма или уберите галочку поиска по ФИО участника найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPersonSNP.Focus();
                return;
            }
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите улицу или уберите галочку поиска по улице", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return;
            }
            if ((checkBoxHouseEnable.Checked) && string.IsNullOrEmpty(textBoxHouse.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер дома или уберите галочку поиска по номеру дома", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if ((checkBoxPremisesNumEnable.Checked) && string.IsNullOrEmpty(textBoxPremisesNum.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер помещения или уберите галочку поиска по номеру помещения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNum.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
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
                vKladr.Filter = "street_name like '%" + comboBoxStreet.Text + "%'";
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
                    comboBoxStreet.SelectedValue = vKladr[vKladr.Position];
                comboBoxStreet.Text = ((DataRowView)vKladr[vKladr.Position])["street_name"].ToString();
            }
            if (comboBoxStreet.SelectedValue == null)
                comboBoxStreet.Text = "";
        }

        private void checkBoxRegDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegDateExpr.Enabled = checkBoxRegDateEnable.Checked;
            dateTimePickerRegDate.Enabled = checkBoxRegDateEnable.Checked;
        }

        private void checkBoxIssueDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxIssueDateExpr.Enabled = checkBoxIssueDateEnable.Checked;
            dateTimePickerIssueDate.Enabled = checkBoxIssueDateEnable.Checked;
        }

        private void checkBoxBeginDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBeginDateExpr.Enabled = checkBoxBeginDateEnable.Checked;
            dateTimePickerBeginDate.Enabled = checkBoxBeginDateEnable.Checked;
        }

        private void checkBoxEndDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxEndDateExpr.Enabled = checkBoxEndDateEnable.Checked;
            dateTimePickerEndDate.Enabled = checkBoxEndDateEnable.Checked;
        }

        private void checkBoxResidenceWarrDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxResidenceWarrDateExpr.Enabled = checkBoxResidenceWarrDateEnable.Checked;
            dateTimePickerResidenceWarrDate.Enabled = checkBoxResidenceWarrDateEnable.Checked;
        }

        private void checkBoxProtocolDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxProtocolDateExpr.Enabled = checkBoxProtocolDateEnable.Checked;
            dateTimePickerProtocolDate.Enabled = checkBoxProtocolDateEnable.Checked;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        private void checkBoxReasonTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxReasonType.Enabled = checkBoxReasonTypeEnable.Checked;
        }
    }
}
