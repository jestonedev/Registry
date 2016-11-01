using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.Services;

namespace Registry.Viewport.SearchForms
{
    internal partial class SimpleSearchTenancyForm : SearchForm
    {

        public SimpleSearchTenancyForm()
        {
            InitializeComponent();
            comboBoxCriteriaType.SelectedIndex = 0;
            HandleHotKeys(Controls, vButtonSearch_Click);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            textBoxCriteria.Focus();
            textBoxCriteria.SelectAll();
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedProcesses = null;
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
                var processesIds = TenancyService.TenancyProcessIdsBySnp(snp, row => row.Field<int?>("id_kinship") == 1);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                //по ФИО участника
                var snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processesIds = TenancyService.TenancyProcessIdsBySnp(snp, row => true);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (comboBoxCriteriaType.SelectedIndex == 3)
            {
                //по адресу
                var addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processesIds = TenancyService.TenancyProcessIDsByAddress(addressParts);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }

            if (includedProcesses != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_process IN (0";
                foreach (var id in includedProcesses)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            return filter;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCriteria.Text.Trim()))
            {
                MessageBox.Show(@"Не ввиден критерий поиска",@"Ошибка",
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
