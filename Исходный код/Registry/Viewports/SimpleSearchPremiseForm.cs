using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.SearchForms;
using Registry.DataModels;
using System.Globalization;

namespace Registry.SearchForms
{
    public partial class SimpleSearchPremiseForm : SearchForm
    {
        public SimpleSearchPremiseForm()
        {
            InitializeComponent();
            comboBoxCriteriaType.SelectedIndex = 0;
            foreach (Control control in this.Controls)
            {
                if (control.Name != "comboBoxCriteriaType")
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
            IEnumerable<int> included_premises = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                //по адресу
                string[] addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> premises_ids = DataModelHelper.PremiseIDsByAddress(addressParts);
                included_premises = DataModelHelper.Intersect(included_premises, premises_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по ФИО нанимателя
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> premises_ids = DataModelHelper.PremisesIDsBySNP(snp, (row) => { return row.Field<int>("id_kinship") == 1; });
                included_premises = DataModelHelper.Intersect(included_premises, premises_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                // по ФИО участника
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> premises_ids = DataModelHelper.PremisesIDsBySNP(snp, (row) => { return true; });
                included_premises = DataModelHelper.Intersect(included_premises, premises_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 3)
            {
                //по кадастровому номеру
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.CurrentCulture, "cadastral_num = '{0}'", textBoxCriteria.Text.Trim().Replace("'", ""));
            }
            if (comboBoxCriteriaType.SelectedIndex == 4)
            {
                // по номеру договора
                IEnumerable<int> premises_ids = DataModelHelper.PremiseIDsByRegistrationNumber(textBoxCriteria.Text.Trim().Replace("'", ""));
                included_premises = DataModelHelper.Intersect(included_premises, premises_ids);
            }
            if (included_premises != null)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_premises IN (0";
                foreach (int id in included_premises)
                    filter += id.ToString(CultureInfo.CurrentCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxCriteria.Text.Trim()))
            {
                MessageBox.Show("Не ввиден критерий поиска", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
