using System;
using System.Globalization;
using System.Windows.Forms;

namespace Registry.Reporting.SettingForms
{
    public partial class ClaimsFilterForm : Form
    {
        public ClaimsFilterForm()
        {
            InitializeComponent();
            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        DialogResult = DialogResult.OK;
                    else
                        if (e.KeyCode == Keys.Escape)
                            DialogResult = DialogResult.Cancel;
                };
        }

        internal string GetFilter()
        {
            var filter = "";
            if (checkBoxIDClaimEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "c.id_claim = {0}", numericUpDownIDClaim.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (checkBoxIDProcessEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "c.id_process = {0}", numericUpDownIDProcess.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (dateTimePickerTransferFrom.Checked || dateTimePickerTransferTo.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerTransferFrom, dateTimePickerTransferTo, "date_of_transfer");
            }
            if (dateTimePickerStartDeptFrom.Checked || dateTimePickerStartDeptTo.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerStartDeptFrom, dateTimePickerStartDeptTo, "start_dept_period");
            }
            if (dateTimePickerEndDeptFrom.Checked || dateTimePickerEndDeptTo.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerEndDeptFrom, dateTimePickerEndDeptTo, "end_dept_period");
            }
            return filter;
        }

        private static string FilterByDateRange(DateTimePicker dateFrom, DateTimePicker dateTo, string name)
        {
            if (dateFrom.Checked && dateTo.Checked)
            {
                return String.Format(CultureInfo.InvariantCulture, "c.{0} BETWEEN STR_TO_DATE('{1}','%d.%m.%Y') AND STR_TO_DATE('{2}','%d.%m.%Y')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), 
                    dateTo.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            }
            else
                if (dateFrom.Checked)
                {
                    return String.Format(CultureInfo.InvariantCulture, "c.{0} >= STR_TO_DATE('{1}','%d.%m.%Y')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
                else
                    if (dateTo.Checked)
                    {
                        return String.Format(CultureInfo.InvariantCulture, "c.{0} <= STR_TO_DATE('{1}','%d.%m.%Y')",
                        name, dateTo.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    }
            throw new ReporterException("Невозможно построить фильтр для поиска претензионно-исковых работ");
        }

        private void checkBoxIDClaimEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDClaim.Enabled = checkBoxIDClaimEnable.Checked;
        }

        private void checkBoxIDProcessEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDProcess.Enabled = checkBoxIDProcessEnable.Checked;
        }
    }
}
