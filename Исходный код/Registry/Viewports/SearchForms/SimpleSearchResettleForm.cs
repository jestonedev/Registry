using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;

namespace Registry.Viewport.SearchForms
{
    internal partial class SimpleSearchResettleForm : SearchForm
    {

        public SimpleSearchResettleForm()
        {
            InitializeComponent();
            comboBoxCriteriaType.SelectedIndex = 0;
            HandleHotKeys(Controls, vButtonSearch_Click);
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedProcesses = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                //по ФИО участника переселения
                var snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processesIds = ResettleService.ResettleProcessIdsBySnp(snp);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по адресу переселения (откуда)
                var addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processesIds = ResettleService.ResettleProcessIDsByAddress(addressParts, ResettleEstateObjectWay.From);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                //по адресу переселения (куда)
                var addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processesIds = ResettleService.ResettleProcessIDsByAddress(addressParts, ResettleEstateObjectWay.To);
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

        private void textBoxCriteria_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
