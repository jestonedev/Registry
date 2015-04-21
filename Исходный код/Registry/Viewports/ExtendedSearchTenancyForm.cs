using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.Viewport;
using System.Globalization;

namespace Registry.SearchForms
{
    internal partial class ExtendedSearchTenancyForm : SearchForm
    {
        KladrRegionsDataModel regions = null;

        BindingSource v_kladr = null;
        BindingSource v_regions = null;
        BindingSource v_rentTypes = null;

        public ExtendedSearchTenancyForm()
        {
            InitializeComponent();
            KladrStreetsDataModel.GetInstance().Select();
            RentTypesDataModel.GetInstance().Select();
            regions = KladrRegionsDataModel.GetInstance();

            DataSet ds = DataSetManager.DataSet;

            v_kladr = new BindingSource();
            v_kladr.DataSource = ds;
            v_kladr.DataMember = "kladr";

            v_regions = new BindingSource();
            v_regions.DataSource = regions.Select();

            v_rentTypes = new BindingSource();
            v_rentTypes.DataSource = ds;
            v_rentTypes.DataMember = "rent_types";

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxRentType.DataSource = v_rentTypes;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";

            comboBoxRegion.DataSource = v_regions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxRegDateExpr.SelectedIndex = 2;
            comboBoxIssueDateExpr.SelectedIndex = 2;
            comboBoxBeginDateExpr.SelectedIndex = 2;
            comboBoxEndDateExpr.SelectedIndex = 2;
            comboBoxResidenceWarrDateExpr.SelectedIndex = 2;
            comboBoxProtocolDateExpr.SelectedIndex = 2;
            foreach (Control control in this.Controls)
                control.KeyDown += (sender, e) =>
                {
                    ComboBox comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButtonSearch_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                };
        }

        internal override string GetFilter()
        {
            string filter = "";
            IEnumerable<int> included_processes = null;
            if (checkBoxContractNumEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "registration_num = '{0}'", textBoxRegistrationNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxResidenceWarrantNumEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "residence_warrant_num = '{0}'", textBoxResidenceWarrantNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxProtocolEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "protocol_num = '{0}'", textBoxProtocolNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxRentTypeEnable.Checked && (comboBoxRentType.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "id_rent_type = {0}", comboBoxRentType.SelectedValue.ToString());
            }
            if (checkBoxRegDateEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "registration_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxRegDateExpr.SelectedItem.ToString()),
                        dateTimePickerRegDate.Value.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            }
            if (checkBoxIssueDateEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "issue_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxIssueDateExpr.SelectedItem.ToString()),
                        dateTimePickerIssueDate.Value.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            }
            if (checkBoxBeginDateEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "begin_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxBeginDateExpr.SelectedItem.ToString()),
                        dateTimePickerBeginDate.Value.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            }
            if (checkBoxEndDateEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "end_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                    comboBoxEndDateExpr.SelectedItem.ToString()),
                    dateTimePickerEndDate.Value.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            }
            if (checkBoxResidenceWarrDateEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "residence_warrant_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxResidenceWarrDateExpr.SelectedItem.ToString()),
                        dateTimePickerResidenceWarrDate.Value.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            }
            if (checkBoxProtocolDateEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "protocol_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                    comboBoxProtocolDateExpr.SelectedItem.ToString()),
                    dateTimePickerProtocolDate.Value.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            }
            if (checkBoxIDTenancyEnable.Checked)
                included_processes = DataModelHelper.Intersect(included_processes, new List<int>() { Convert.ToInt32(numericUpDownIDTenancy.Value) });
            if (checkBoxTenantSNPEnable.Checked)
            {
                string[] snp = textBoxTenantSNP.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> processes_ids = DataModelHelper.TenancyProcessIDsBySNP(snp, (row) => { return row.Field<int?>("id_kinship") == 1; });
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxPersonSNPEnable.Checked)
            {
                string[] snp = textBoxPersonSNP.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> processes_ids = DataModelHelper.TenancyProcessIDsBySNP(snp, (row) => { return true; });
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxRegionEnable.Checked && (comboBoxRegion.SelectedValue != null))
            {
                IEnumerable<int> processes_ids = DataModelHelper.TenancyProcessIDsByCondition(
                    (row) => { return row.Field<string>("id_street").StartsWith(comboBoxRegion.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase); }, 
                    DataModelHelper.ConditionType.BuildingCondition);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxStreetEnable.Checked && (comboBoxStreet.SelectedValue != null))
            {
                IEnumerable<int> processes_ids = DataModelHelper.TenancyProcessIDsByCondition(
                    (row) => { return row.Field<string>("id_street") == comboBoxStreet.SelectedValue.ToString(); },
                    DataModelHelper.ConditionType.BuildingCondition);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxHouseEnable.Checked)
            {
                IEnumerable<int> processes_ids = DataModelHelper.TenancyProcessIDsByCondition(
                    (row) => { return row.Field<string>("house") == textBoxHouse.Text.Trim().Replace("'", ""); }, 
                    DataModelHelper.ConditionType.BuildingCondition);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxPremisesNumEnable.Checked)
            {
                IEnumerable<int> processes_ids = DataModelHelper.TenancyProcessIDsByCondition(
                    (row) => { return row.Field<string>("premises_num") == textBoxPremisesNum.Text.Trim().Replace("'", ""); }, 
                    DataModelHelper.ConditionType.PremisesCondition);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (included_processes != null)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_process IN (0";
                foreach (int id in included_processes)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
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

        private void checkBoxResidenceWarrantNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxResidenceWarrantNum.Enabled = checkBoxResidenceWarrantNumEnable.Checked;
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
            if ((checkBoxContractNumEnable.Checked) && String.IsNullOrEmpty(textBoxRegistrationNum.Text.Trim()))
            {
                MessageBox.Show("Введите номер договора или уберите галочку поиска по номеру договора", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxRegistrationNum.Focus();
                return;
            }
            if ((checkBoxResidenceWarrantNumEnable.Checked) && String.IsNullOrEmpty(textBoxResidenceWarrantNum.Text.Trim()))
            {
                MessageBox.Show("Введите номер ордера на проживание или уберите галочку поиска по номеру ордера на проживание", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxResidenceWarrantNum.Focus();
                return;
            }
            if ((checkBoxProtocolEnable.Checked) && String.IsNullOrEmpty(textBoxProtocolNum.Text.Trim()))
            {
                MessageBox.Show("Введите номер распоряжения КУМИ или уберите галочку поиска по номеру распоряжения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxProtocolNum.Focus();
                return;
            }
            if ((checkBoxTenantSNPEnable.Checked) && String.IsNullOrEmpty(textBoxTenantSNP.Text.Trim()))
            {
                MessageBox.Show("Введите ФИО нанимателя или уберите галочку поиска по ФИО нанимателя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxTenantSNP.Focus();
                return;
            }
            if ((checkBoxPersonSNPEnable.Checked) && String.IsNullOrEmpty(textBoxPersonSNP.Text.Trim()))
            {
                MessageBox.Show("Введите ФИО участника найма или уберите галочку поиска по ФИО участника найма", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPersonSNP.Focus();
                return;
            }
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue == null))
            {
                MessageBox.Show("Выберите улицу или уберите галочку поиска по улице", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return;
            }
            if ((checkBoxHouseEnable.Checked) && String.IsNullOrEmpty(textBoxHouse.Text.Trim()))
            {
                MessageBox.Show("Введите номер дома или уберите галочку поиска по номеру дома", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if ((checkBoxPremisesNumEnable.Checked) && String.IsNullOrEmpty(textBoxPremisesNum.Text.Trim()))
            {
                MessageBox.Show("Введите номер помещения или уберите галочку поиска по номеру помещения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNum.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
        }

        private void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back))
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

        private void comboBoxStreet_Leave(object sender, EventArgs e)
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
    }
}
