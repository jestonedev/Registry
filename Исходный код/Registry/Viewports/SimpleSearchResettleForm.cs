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
    public partial class SimpleSearchResettleForm : SearchForm
    {
        private enum ConditionType { BuildingCondition, PremisesCondition, KladrCondition };

        public SimpleSearchResettleForm()
        {
            InitializeComponent();
            comboBoxCriteriaType.SelectedIndex = 0;
            foreach (Control control in this.Controls)
            {
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
        }

        internal override string GetFilter()
        {
            string filter = "";
            IEnumerable<int> included_processes = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                //по ФИО участника переселения
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsBySNP(snp);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по адресу переселения (откуда)
                string[] addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByAddress(addressParts, Entities.ResettleEstateObjectWay.From);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                //по адресу переселения (куда)
                string[] addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByAddress(addressParts, Entities.ResettleEstateObjectWay.To);
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

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxCriteria.Text.Trim()))
            {
                MessageBox.Show("Не ввиден критерий поиска","Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
