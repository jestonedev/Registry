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
                List<int> contract_ids = new List<int>();
                string[] addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                DataTable kladr_street = KladrStreetsDataModel.GetInstance().Select();
                DataTable buildings = BuildingsDataModel.GetInstance().Select();
                contract_ids = (from building_row in buildings.AsEnumerable()
                               join kladr_row in kladr_street.AsEnumerable()
                               on building_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                               where (addressParts.Count() == 1) ? kladr_row.Field<string>("street_name").Contains(addressParts[0]) :
                                     (addressParts.Count() == 2) ? kladr_row.Field<string>("street_name").Contains(addressParts[0]) &&
                                     building_row.Field<string>("house") == addressParts[1] : false
                               select building_row.Field<int>("id_building")).ToList();

                if (included_buildings != null)
                    included_buildings = included_buildings.Intersect(contract_ids).ToList();
                else
                    included_buildings = contract_ids;
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по ФИО нанимателя
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> contract_ids = ContractsIDBySNP(snp, (row) => { return row.Field<int>("id_kinship") == 1; });
                if (included_buildings != null)
                    included_buildings = included_buildings.Intersect(contract_ids).ToList();
                else
                    included_buildings = contract_ids;
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                // по ФИО участника
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> contract_ids = ContractsIDBySNP(snp, (row) => { return true; });
                if (included_buildings != null)
                    included_buildings = included_buildings.Intersect(contract_ids).ToList();
                else
                    included_buildings = contract_ids;
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
                List<int> contract_ids = new List<int>();
                DataTable tenancy_buildings_assoc = TenancyBuildingsAssocDataModel.GetInstance().Select();
                DataTable tenancies = TenancyContractsDataModel.GetInstance().Select();
                contract_ids = (from tenancies_row in tenancies.AsEnumerable()
                                join tenancy_buildings_assoc_row in tenancy_buildings_assoc.AsEnumerable()
                                on tenancies_row.Field<int>("id_contract") equals tenancy_buildings_assoc_row.Field<int>("id_contract")
                                where tenancies_row.Field<string>("registration_num") == textBoxCriteria.Text.Trim().Replace("'", "")
                                select tenancy_buildings_assoc_row.Field<int>("id_building")).ToList();
                if (included_buildings != null)
                    included_buildings = included_buildings.Intersect(contract_ids).ToList();
                else
                    included_buildings = contract_ids;
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

        private List<int> ContractsIDBySNP(string[] snp, Func<DataRow,bool> condition)
        {
            DataTable tenancy_buildings_assoc = TenancyBuildingsAssocDataModel.GetInstance().Select();
            DataTable persons = PersonsDataModel.GetInstance().Select();
            return 
            (from tenancy_buildings_assoc_row in tenancy_buildings_assoc.AsEnumerable()
             join persons_row in persons.AsEnumerable()
             on tenancy_buildings_assoc_row.Field<int>("id_contract") equals persons_row.Field<int>("id_contract")
             where ((snp.Count() == 1) ? persons_row.Field<string>("surname") == snp[0] :
                    (snp.Count() == 2) ? persons_row.Field<string>("surname") == snp[0] && persons_row.Field<string>("name") == snp[1] :
                    (snp.Count() == 3) ? persons_row.Field<string>("surname") == snp[0] && persons_row.Field<string>("name") == snp[1] &&
                    persons_row.Field<string>("patronymic") == snp[2] : false) && condition(persons_row)
             select tenancy_buildings_assoc_row.Field<int>("id_building")).ToList();
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
