using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Registry.Reporting
{
    public partial class ClaimsFilterForm : Form
    {
        public ClaimsFilterForm()
        {
            InitializeComponent();
            foreach (Control control in this.Controls)
                control.KeyDown += (sender, e) =>
                {
                    ComboBox comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    else
                        if (e.KeyCode == Keys.Escape)
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                };
        }

        internal string GetFilter()
        {
            string filter = "";
            if (checkBoxIDClaimEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.CurrentCulture, "c.id_claim = {0}", numericUpDownIDClaim.Value.ToString(CultureInfo.CurrentCulture));
            }
            if (checkBoxIDProcessEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.CurrentCulture, "c.id_process = {0}", numericUpDownIDProcess.Value.ToString(CultureInfo.CurrentCulture));
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
                return String.Format(CultureInfo.CurrentCulture, "c.{0} BETWEEN STR_TO_DATE('{1}','%d.%m.%Y') AND STR_TO_DATE('{2}','%d.%m.%Y')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture), 
                    dateTo.Value.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
            }
            else
                if (dateFrom.Checked)
                {
                    return String.Format(CultureInfo.CurrentCulture, "c.{0} >= STR_TO_DATE('{1}','%d.%m.%Y')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                }
                else
                    if (dateTo.Checked)
                    {
                        return String.Format(CultureInfo.CurrentCulture, "c.{0} <= STR_TO_DATE('{1}','%d.%m.%Y')",
                        name, dateTo.Value.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
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
