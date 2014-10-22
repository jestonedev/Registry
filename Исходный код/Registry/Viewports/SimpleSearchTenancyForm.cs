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

namespace Registry.SearchForms
{
    public partial class SimpleSearchTenancyForm : SearchForm
    {
        private enum ConditionType { BuildingCondition, PremisesCondition, KladrCondition };

        public SimpleSearchTenancyForm()
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
            List<int> included_contracts = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                //по номеру договора
                filter += "registration_num = '"+textBoxCriteria.Text.Trim()+"'";
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по ФИО нанимателя
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> contract_ids = ContractsIDBySNP(snp, (row) => { return row.Field<int>("id_kinship") == 1; });
                contract_ids = contract_ids.Distinct().ToList();
                if (included_contracts != null)
                    included_contracts = included_contracts.Intersect(contract_ids).ToList();
                else
                    included_contracts = contract_ids;
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                //по ФИО участника
                string[] snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> contract_ids = ContractsIDBySNP(snp, (row) => { return true; });
                contract_ids = contract_ids.Distinct().ToList();
                if (included_contracts != null)
                    included_contracts = included_contracts.Intersect(contract_ids).ToList();
                else
                    included_contracts = contract_ids;
            }
            if (comboBoxCriteriaType.SelectedIndex == 3)
            {
                //по адресу
                string[] addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> contract_ids = ContractsIDByAddress(addressParts);
                if (included_contracts != null)
                    included_contracts = included_contracts.Intersect(contract_ids).ToList();
                else
                    included_contracts = contract_ids;
            }

            if (included_contracts != null)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "id_contract IN (0";
                for (int i = 0; i < included_contracts.Count; i++)
                    filter += included_contracts[i].ToString() + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        private List<int> ContractsIDBySNP(string[] snp, Func<DataRow, bool> condition)
        {
            DataTable persons = PersonsDataModel.GetInstance().Select();
            return
            (from persons_row in persons.AsEnumerable()
             where ((snp.Count() == 1) ? persons_row.Field<string>("surname") == snp[0] :
                    (snp.Count() == 2) ? persons_row.Field<string>("surname") == snp[0] && persons_row.Field<string>("name") == snp[1] :
                    (snp.Count() == 3) ? persons_row.Field<string>("surname") == snp[0] && persons_row.Field<string>("name") == snp[1] &&
                    persons_row.Field<string>("patronymic") == snp[2] : false) && condition(persons_row)
             select persons_row.Field<int>("id_contract")).ToList();
        }

        private List<int> ContractsIDByAddress(string[] address)
        {
            DataTable kladr_street = KladrStreetsDataModel.GetInstance().Select();
            DataTable buildings = BuildingsDataModel.GetInstance().Select();
            DataTable premises = PremisesDataModel.GetInstance().Select();
            DataTable sub_premises = SubPremisesDataModel.GetInstance().Select();
            DataTable tenancy_buildings_assoc = TenancyBuildingsAssocDataModel.GetInstance().Select();
            DataTable tenancy_premises_assoc = TenancyPremisesAssocDataModel.GetInstance().Select();
            DataTable tenancy_sub_premises_assoc = TenancySubPremisesAssocDataModel.GetInstance().Select();
            var tenancy_buildings = from tenancy_buildings_row in tenancy_buildings_assoc.AsEnumerable()
                                    join buildings_row in buildings.AsEnumerable()
                                    on tenancy_buildings_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                    join kladr_row in kladr_street.AsEnumerable()
                                    on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                    where (address.Count() == 1) ? kladr_row.Field<string>("street_name").Contains(address[0]) : 
                                          (address.Count() >= 2) ? (kladr_row.Field<string>("street_name").Contains(address[0])) &&
                                          (buildings_row.Field<string>("house") == address[1]) : false
                                    select tenancy_buildings_row.Field<int>("id_contract");
            var tenancy_premises = from tenancy_premises_row in tenancy_premises_assoc.AsEnumerable()
                                   join premises_row in premises.AsEnumerable()
                                   on tenancy_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                   join buildings_row in buildings.AsEnumerable()
                                   on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                   join kladr_row in kladr_street.AsEnumerable()
                                    on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                   where (address.Count() == 1) ? (kladr_row.Field<string>("street_name").Contains(address[0])) :
                                         (address.Count() == 2) ? (kladr_row.Field<string>("street_name").Contains(address[0])) &&
                                         (buildings_row.Field<string>("house") == address[1]) :
                                         (address.Count() == 3) ? (kladr_row.Field<string>("street_name").Contains(address[0])) &&
                                         (buildings_row.Field<string>("house") == address[1]) && 
                                         (premises_row.Field<string>("premises_num") == address[2]): false
                                   select tenancy_premises_row.Field<int>("id_contract");
            var tenancy_sub_premises = from tenancy_sub_premises_row in tenancy_sub_premises_assoc.AsEnumerable()
                                       join sub_premises_row in sub_premises.AsEnumerable()
                                       on tenancy_sub_premises_row.Field<int>("id_sub_premises") equals sub_premises_row.Field<int>("id_sub_premises")
                                       join premises_row in premises.AsEnumerable()
                                       on sub_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                       join buildings_row in buildings.AsEnumerable()
                                       on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                       join kladr_row in kladr_street.AsEnumerable()
                                       on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                       where (address.Count() == 1) ? (kladr_row.Field<string>("street_name").Contains(address[0])) :
                                         (address.Count() == 2) ? (kladr_row.Field<string>("street_name").Contains(address[0])) &&
                                         (buildings_row.Field<string>("house") == address[1]) :
                                         (address.Count() == 3) ? (kladr_row.Field<string>("street_name").Contains(address[0])) &&
                                         (buildings_row.Field<string>("house") == address[1]) &&
                                         (premises_row.Field<string>("premises_num") == address[2]) : false
                                       select tenancy_sub_premises_row.Field<int>("id_contract");
            return tenancy_buildings.Union(tenancy_premises).Union(tenancy_sub_premises).ToList();
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (textBoxCriteria.Text.Trim() == "")
            {
                MessageBox.Show("Не ввиден критерий поиска","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
