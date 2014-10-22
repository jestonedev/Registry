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
            List<int> included_premises = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                //по адресу
                List<int> contract_ids = new List<int>();
                string[] addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                DataTable kladr_street = KladrStreetsDataModel.GetInstance().Select();
                DataTable buildings = BuildingsDataModel.GetInstance().Select();
                DataTable premises = PremisesDataModel.GetInstance().Select();
                contract_ids = (from premises_row in premises.AsEnumerable()
                                join building_row in buildings.AsEnumerable()
                                on premises_row.Field<int>("id_building") equals building_row.Field<int>("id_building")
                                join kladr_row in kladr_street.AsEnumerable()
                                on building_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                where (addressParts.Count() == 1) ? kladr_row.Field<string>("street_name").Contains(addressParts[0]) :
                                      (addressParts.Count() == 2) ? kladr_row.Field<string>("street_name").Contains(addressParts[0]) &&
                                      building_row.Field<string>("house") == addressParts[1] :
                                      (addressParts.Count() == 3) ? kladr_row.Field<string>("street_name").Contains(addressParts[0]) &&
                                      building_row.Field<string>("house") == addressParts[1] && 
                                      premises_row.Field<string>("premises_num") == addressParts[2] : false
                                select premises_row.Field<int>("id_premises")).ToList();

                if (included_premises != null)
                    included_premises = included_premises.Intersect(contract_ids).ToList();
                else
                    included_premises = contract_ids;
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по ФИО нанимателя
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> contract_ids = ContractsIDBySNP(snp, (row) => { return row.Field<int>("id_kinship") == 1; });
                if (included_premises != null)
                    included_premises = included_premises.Intersect(contract_ids).ToList();
                else
                    included_premises = contract_ids;
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                // по ФИО участника
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> contract_ids = ContractsIDBySNP(snp, (row) => { return true; });
                if (included_premises != null)
                    included_premises = included_premises.Intersect(contract_ids).ToList();
                else
                    included_premises = contract_ids;
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
                DataTable tenancy_premises_assoc = TenancyPremisesAssocDataModel.GetInstance().Select();
                DataTable tenancies = TenancyContractsDataModel.GetInstance().Select();
                contract_ids = (from tenancies_row in tenancies.AsEnumerable()
                                join tenancy_premises_assoc_row in tenancy_premises_assoc.AsEnumerable()
                                on tenancies_row.Field<int>("id_contract") equals tenancy_premises_assoc_row.Field<int>("id_contract")
                                where tenancies_row.Field<string>("registration_num") == textBoxCriteria.Text.Trim().Replace("'", "")
                                select tenancy_premises_assoc_row.Field<int>("id_premises")).ToList();
                if (included_premises != null)
                    included_premises = included_premises.Intersect(contract_ids).ToList();
                else
                    included_premises = contract_ids;
            }
            if (included_premises != null)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "id_premises IN (0";
                for (int i = 0; i < included_premises.Count; i++)
                    filter += included_premises[i].ToString() + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        private List<int> ContractsIDBySNP(string[] snp, Func<DataRow, bool> condition)
        {
            DataTable tenancy_premises_assoc = TenancyPremisesAssocDataModel.GetInstance().Select();
            DataTable persons = PersonsDataModel.GetInstance().Select();
            return
            (from tenancy_premises_assoc_row in tenancy_premises_assoc.AsEnumerable()
             join persons_row in persons.AsEnumerable()
             on tenancy_premises_assoc_row.Field<int>("id_contract") equals persons_row.Field<int>("id_contract")
             where ((snp.Count() == 1) ? persons_row.Field<string>("surname") == snp[0] :
                    (snp.Count() == 2) ? persons_row.Field<string>("surname") == snp[0] && persons_row.Field<string>("name") == snp[1] :
                    (snp.Count() == 3) ? persons_row.Field<string>("surname") == snp[0] && persons_row.Field<string>("name") == snp[1] &&
                    persons_row.Field<string>("patronymic") == snp[2] : false) && condition(persons_row)
             select tenancy_premises_assoc_row.Field<int>("id_premises")).ToList();
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
