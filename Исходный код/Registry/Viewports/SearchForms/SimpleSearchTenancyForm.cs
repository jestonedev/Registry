using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;

namespace Registry.Viewport.SearchForms
{
    public partial class SimpleSearchTenancyForm : SearchForm
    {

        public SimpleSearchTenancyForm()
        {
            InitializeComponent();
            comboBoxCriteriaType.SelectedIndex = 0;
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

        protected override void OnVisibleChanged(EventArgs e)
        {
            textBoxCriteria.Focus();
            textBoxCriteria.SelectAll();
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> included_processes = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                //по номеру договора
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "registration_num LIKE '%"+textBoxCriteria.Text.Trim()+"%'";
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по ФИО нанимателя
                var snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processes_ids = DataModelHelper.TenancyProcessIdsBySnp(snp, (row) => { return row.Field<int?>("id_kinship") == 1; });
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                //по ФИО участника
                var snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processes_ids = DataModelHelper.TenancyProcessIdsBySnp(snp, (row) => { return true; });
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 3)
            {
                //по адресу
                var addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processes_ids = DataModelHelper.TenancyProcessIDsByAddress(addressParts);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }

            if (included_processes != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_process IN (0";
                foreach (var id in included_processes)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            return filter;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCriteria.Text.Trim()))
            {
                MessageBox.Show("Не ввиден критерий поиска","Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void comboBoxCriteriaType_DropDownClosed(object sender, EventArgs e)
        {
            textBoxCriteria.Focus();
            textBoxCriteria.SelectAll();
        }

        private void SimpleSearchTenancyForm_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        private void textBoxCriteria_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
