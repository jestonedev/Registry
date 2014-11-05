using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Reporting
{
    public partial class ClaimsFilterForm : Form
    {
        public ClaimsFilterForm()
        {
            InitializeComponent();
        }

        internal string GetFilter()
        {
            string filter = "";
            if (checkBoxIDClaimEnable.Checked)
            {
                if (filter != "")
                    filter += " AND ";
                filter += String.Format("c.id_claim = {0}", numericUpDownIDClaim.Value.ToString());
            }
            if (checkBoxIDProcessEnable.Checked)
            {
                if (filter != "")
                    filter += " AND ";
                filter += String.Format("c.id_process = {0}", numericUpDownIDProcess.Value.ToString());
            }
            if (dateTimePickerTransferFrom.Checked || dateTimePickerTransferTo.Checked)
            {
                if (filter != "")
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerTransferFrom, dateTimePickerTransferTo, "date_of_transfer");
            }
            if (dateTimePickerStartDeptFrom.Checked || dateTimePickerStartDeptTo.Checked)
            {
                if (filter != "")
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerStartDeptFrom, dateTimePickerStartDeptTo, "start_dept_period");
            }
            if (dateTimePickerEndDeptFrom.Checked || dateTimePickerEndDeptTo.Checked)
            {
                if (filter != "")
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerEndDeptFrom, dateTimePickerEndDeptTo, "end_dept_period");
            }
            return filter;
        }

        private string FilterByDateRange(DateTimePicker dateFrom, DateTimePicker dateTo, string name)
        {
            if (dateFrom.Checked && dateTo.Checked)
            {
                return String.Format("c.{0} BETWEEN STR_TO_DATE('{1}','%d.%m.%Y') AND STR_TO_DATE('{2}','%d.%m.%Y')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy"), dateTo.Value.ToString("dd.MM.yyyy"));
            }
            else
                if (dateFrom.Checked)
                {
                    return String.Format("c.{0} >= STR_TO_DATE('{1}','%d.%m.%Y')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy"));
                }
                else
                    if (dateTo.Checked)
                    {
                        return String.Format("c.{0} <= STR_TO_DATE('{1}','%d.%m.%Y')",
                        name, dateTo.Value.ToString("dd.MM.yyyy"));
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
