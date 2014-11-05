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

namespace Registry.Viewport
{
    public partial class SimpleSearchBuildingForm : SearchForm
    {
        public SimpleSearchBuildingForm()
        {
            InitializeComponent();
            comboBoxCriteriaType.SelectedIndex = 0;
            foreach (Control control in this.Controls)
            {
                if (control.Name != "comboBoxCriteriaType")
                    control.KeyDown += (sender, e) =>
                    {
                        if (sender is ComboBox && ((ComboBox)sender).DroppedDown)
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
            List<int> included_buildings = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                //по адресу
                string[] addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                List<int> building_ids = DataModelHelper.BuildingIDsByAddress(addressParts);
                included_buildings = DataModelHelper.Intersect(included_buildings, building_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по ФИО нанимателя
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> building_ids = DataModelHelper.BuildingIDsBySNP(snp, (row) => { return row.Field<int>("id_kinship") == 1; });
                included_buildings = DataModelHelper.Intersect(included_buildings, building_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                // по ФИО участника
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> building_ids = DataModelHelper.BuildingIDsBySNP(snp, (row) => { return true; });
                included_buildings = DataModelHelper.Intersect(included_buildings, building_ids);
            }
            if (comboBoxCriteriaType.SelectedIndex == 3)
            {
                //по кадастровому номеру
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("cadastral_num = '{0}'", textBoxCriteria.Text.Trim().Replace("'", ""));
            }
            if (comboBoxCriteriaType.SelectedIndex == 4)
            {
                // по номеру договора
                List<int> building_ids = DataModelHelper.BuildingIDsByRegistrationNumber(textBoxCriteria.Text.Trim().Replace("'", ""));
                included_buildings = DataModelHelper.Intersect(included_buildings, building_ids);
            }
            if (included_buildings != null)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "id_building IN (0";
                for (int i = 0; i < included_buildings.Count; i++)
                    filter += included_buildings[i].ToString() + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (textBoxCriteria.Text.Trim() == "")
            {
                MessageBox.Show("Не ввиден критерий поиска", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
