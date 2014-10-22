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
    internal partial class ExtendedSearchTenancyForm : SearchForm
    {
        private enum ConditionType { BuildingCondition, PremisesCondition };

        KladrStreetsDataModel kladr = null;
        KladrRegionsDataModel regions = null;
        RentTypesDataModel rentTypes = null;

        BindingSource v_kladr = null;
        BindingSource v_regions = null;
        BindingSource v_rentTypes = null;

        public ExtendedSearchTenancyForm()
        {
            InitializeComponent();
            kladr = KladrStreetsDataModel.GetInstance();
            rentTypes = RentTypesDataModel.GetInstance();
            regions = KladrRegionsDataModel.GetInstance();

            DataSet ds = DataSetManager.GetDataSet();

            v_kladr = new BindingSource();
            v_kladr.DataSource = ds;
            v_kladr.DataMember = "kladr";

            v_regions = new BindingSource();
            v_regions.DataSource = regions.Select();

            v_rentTypes = new BindingSource();
            v_rentTypes.DataSource = ds;
            v_rentTypes.DataMember = "rent_types";

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxRentType.DataSource = v_rentTypes;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";

            comboBoxRegion.DataSource = v_regions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxRegDateExpr.SelectedIndex = 2;
            comboBoxIssueDateExpr.SelectedIndex = 2;
            comboBoxBeginDateExpr.SelectedIndex = 2;
            comboBoxEndDateExpr.SelectedIndex = 2;
            comboBoxResidenceWarrDateExpr.SelectedIndex = 2;
            comboBoxKumiOrderDateExpr.SelectedIndex = 2;
            foreach (Control control in this.Controls)
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

        internal override string GetFilter()
        {
            string filter = "";
            List<int> included_contracts = null;
            if (checkBoxIDTenancyEnable.Checked)
            {
                if (included_contracts == null)
                    included_contracts = new List<int>();
                included_contracts.Add(Convert.ToInt32(numericUpDownIDTenancy.Value));
            }
            if (checkBoxContractNumEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("registration_num = '{0}'", textBoxRegistrationNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxResidenceWarrantNumEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("residence_warrant_num = '{0}'", textBoxResidenceWarrantNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxKumiOrderEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("kumi_order_num = '{0}'", textBoxKumiOrderNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxRentTypeEnable.Checked && (comboBoxRentType.SelectedValue != null))
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("id_rent_type = {0}", comboBoxRentType.SelectedValue.ToString());
            }
            if (checkBoxTenantSNPEnable.Checked)
            {
                string[] snp = textBoxTenantSNP.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> contract_ids = ContractsIDBySNP(snp, (row) => { return row.Field<int>("id_kinship") == 1; });
                contract_ids = contract_ids.Distinct().ToList();
                if (included_contracts != null)
                    included_contracts = included_contracts.Intersect(contract_ids).ToList();
                else
                    included_contracts = contract_ids;
            }
            if (checkBoxPersonSNPEnable.Checked)
            {
                string[] snp = textBoxTenantSNP.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                List<int> contract_ids = ContractsIDBySNP(snp, (row) => { return true; });
                contract_ids = contract_ids.Distinct().ToList();
                if (included_contracts != null)
                    included_contracts = included_contracts.Intersect(contract_ids).ToList();
                else
                    included_contracts = contract_ids;
            }
            if (checkBoxRegionEnable.Checked && (comboBoxRegion.SelectedValue != null))
            {
                List<int> contract_ids = SearchContractsByCondition((row) =>
                    { return row.Field<string>("id_street").StartsWith(comboBoxRegion.SelectedValue.ToString()); }, ConditionType.BuildingCondition);
                if (included_contracts != null)
                    included_contracts = included_contracts.Intersect(contract_ids).ToList();
                else
                    included_contracts = contract_ids;
            }
            if (checkBoxStreetEnable.Checked && (comboBoxStreet.SelectedValue != null))
            {
                List<int> contract_ids = SearchContractsByCondition((row) => { return row.Field<string>("id_street") == comboBoxStreet.SelectedValue.ToString(); },
                    ConditionType.BuildingCondition);
                if (included_contracts != null)
                    included_contracts = included_contracts.Intersect(contract_ids).ToList();
                else
                    included_contracts = contract_ids;
            }
            if (checkBoxHouseEnable.Checked)
            {
                List<int> contract_ids = SearchContractsByCondition(
                    (row) => { return row.Field<string>("house") == textBoxHouse.Text.Trim().Replace("'", ""); }, ConditionType.BuildingCondition);
                if (included_contracts != null)
                    included_contracts = included_contracts.Intersect(contract_ids).ToList();
                else
                    included_contracts = contract_ids;
            }
            if (checkBoxPremisesNumEnable.Checked)
            {
                List<int> contract_ids = SearchContractsByCondition(
                    (row) => { return row.Field<string>("premises_num") == textBoxPremisesNum.Text.Trim().Replace("'", ""); }, ConditionType.PremisesCondition);
                if (included_contracts != null)
                    included_contracts = included_contracts.Intersect(contract_ids).ToList();
                else
                    included_contracts = contract_ids;
            }
            if (checkBoxRegDateEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("registration_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(comboBoxRegDateExpr.SelectedItem.ToString()), dateTimePickerRegDate.Value.ToString("dd.MM.yyyy"));
            }
            if (checkBoxIssueDateEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("issue_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(comboBoxIssueDateExpr.SelectedItem.ToString()), dateTimePickerIssueDate.Value.ToString("dd.MM.yyyy"));
            }
            if (checkBoxBeginDateEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("begin_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(comboBoxBeginDateExpr.SelectedItem.ToString()), dateTimePickerBeginDate.Value.ToString("dd.MM.yyyy"));
            }
            if (checkBoxEndDateEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("end_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(comboBoxEndDateExpr.SelectedItem.ToString()), dateTimePickerEndDate.Value.ToString("dd.MM.yyyy"));
            }
            if (checkBoxResidenceWarrDateEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("residence_warrant_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(comboBoxResidenceWarrDateExpr.SelectedItem.ToString()), 
                    dateTimePickerResidenceWarrDate.Value.ToString("dd.MM.yyyy"));
            }
            if (checkBoxKumiOrderDateEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("kumi_order_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(comboBoxKumiOrderDateExpr.SelectedItem.ToString()),
                    dateTimePickerKumiOrderDate.Value.ToString("dd.MM.yyyy"));
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

        private string SNPArrayToFilter(string[] snp)
        {
            switch (snp.Count())
            {
                case 0: return "surname = '' AND name = '' AND patronymic = ''";
                case 1: return String.Format("surname LIKE '{0}%'", snp[0]);
                case 2: return String.Format("surname LIKE '{0}%' AND name LIKE '{1}%'", snp[0], snp[1]);
                case 3: return String.Format("surname LIKE '{0}%' AND name LIKE '{1}%' AND patronymic LIKE '{2}%'", snp[0], snp[1], snp[2]);
                default:
                    throw new ViewportException("Ошибка при фильтрации по имени участника найма");
            }
        }

        private string ConvertDisplayEqExprToSql(string expr)
        {
            switch (expr)
            {
                case "=": return "=";
                case "≥": return ">=";
                case "≤": return "<=";
                default:
                    throw new ViewportException("Неизвестный знак сравнения дат");
            }
        }

        private List<int> SearchContractsByCondition(Func<DataRow, bool> condition, ConditionType conditionType)
        {
            DataTable buildings = BuildingsDataModel.GetInstance().Select();
            DataTable premises = PremisesDataModel.GetInstance().Select();
            DataTable sub_premises = SubPremisesDataModel.GetInstance().Select();
            DataTable tenancy_buildings_assoc = TenancyBuildingsAssocDataModel.GetInstance().Select();
            DataTable tenancy_premises_assoc = TenancyPremisesAssocDataModel.GetInstance().Select();
            DataTable tenancy_sub_premises_assoc = TenancySubPremisesAssocDataModel.GetInstance().Select();
            var tenancy_buildings = from tenancy_buildings_row in tenancy_buildings_assoc.AsEnumerable()
                                    join buildings_row in buildings.AsEnumerable()
                                    on tenancy_buildings_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                    where
                                    (conditionType == ConditionType.PremisesCondition) ? false : condition(buildings_row)
                                    select tenancy_buildings_row.Field<int>("id_contract");
            var tenancy_premises = from tenancy_premises_row in tenancy_premises_assoc.AsEnumerable()
                                   join premises_row in premises.AsEnumerable()
                                   on tenancy_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                   join buildings_row in buildings.AsEnumerable()
                                   on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                   where (conditionType == ConditionType.PremisesCondition) ? condition(premises_row) : condition(buildings_row)
                                   select tenancy_premises_row.Field<int>("id_contract");
            var tenancy_sub_premises = from tenancy_sub_premises_row in tenancy_sub_premises_assoc.AsEnumerable()
                                       join sub_premises_row in sub_premises.AsEnumerable()
                                       on tenancy_sub_premises_row.Field<int>("id_sub_premises") equals sub_premises_row.Field<int>("id_sub_premises")
                                       join premises_row in premises.AsEnumerable()
                                       on sub_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                       join buildings_row in buildings.AsEnumerable()
                                       on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                       where (conditionType == ConditionType.PremisesCondition) ? condition(premises_row) : condition(buildings_row)
                                       select tenancy_sub_premises_row.Field<int>("id_contract");
            return tenancy_buildings.Union(tenancy_premises).Union(tenancy_sub_premises).ToList();
        }

        private void checkBoxIDTenancyEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDTenancy.Enabled = checkBoxIDTenancyEnable.Checked;
        }

        private void checkBoxContractNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRegistrationNum.Enabled = checkBoxContractNumEnable.Checked;
        }

        private void checkBoxResidenceWarrantNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxResidenceWarrantNum.Enabled = checkBoxResidenceWarrantNumEnable.Checked;
        }

        private void checkBoxKumiOrderEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxKumiOrderNum.Enabled = checkBoxKumiOrderEnable.Checked;
        }

        private void checkBoxTenantSNPEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTenantSNP.Enabled = checkBoxTenantSNPEnable.Checked;
        }

        private void checkBoxPersonSNPEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPersonSNP.Enabled = checkBoxPersonSNPEnable.Checked;
        }

        private void checkBoxRentTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRentType.Enabled = checkBoxRentTypeEnable.Checked;
        }

        private void checkBoxRegionEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegion.Enabled = checkBoxRegionEnable.Checked;
        }

        private void checkBoxStreetEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStreet.Enabled = checkBoxStreetEnable.Checked;
        }

        private void checkBoxHouseEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxHouse.Enabled = checkBoxHouseEnable.Checked;
        }

        private void checkBoxPremisesNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPremisesNum.Enabled = checkBoxPremisesNumEnable.Checked;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue == null))
            {
                MessageBox.Show("Выберите улицу или уберите галочку поиска по улице", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxStreet.Focus();
                return;
            }
            if ((checkBoxHouseEnable.Checked) && (textBoxHouse.Text.Trim() == ""))
            {
                MessageBox.Show("Введите номер дома или уберите галочку поиска по номеру дома", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxHouse.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
        }

        private void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back))
            {
                string text = comboBoxStreet.Text;
                int selectionStart = comboBoxStreet.SelectionStart;
                int selectionLength = comboBoxStreet.SelectionLength;
                v_kladr.Filter = "street_name like '%" + comboBoxStreet.Text + "%'";
                comboBoxStreet.Text = text;
                comboBoxStreet.SelectionStart = selectionStart;
                comboBoxStreet.SelectionLength = selectionLength;
            }
        }

        private void comboBoxStreet_Leave(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count > 0)
            {
                if (comboBoxStreet.SelectedValue == null)
                    comboBoxStreet.SelectedValue = v_kladr[v_kladr.Position];
                comboBoxStreet.Text = ((DataRowView)v_kladr[v_kladr.Position])["street_name"].ToString();
            }
            if (comboBoxStreet.SelectedValue == null)
                comboBoxStreet.Text = "";
        }

        private void checkBoxRegDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegDateExpr.Enabled = checkBoxRegDateEnable.Checked;
            dateTimePickerRegDate.Enabled = checkBoxRegDateEnable.Checked;
        }

        private void checkBoxIssueDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxIssueDateExpr.Enabled = checkBoxIssueDateEnable.Checked;
            dateTimePickerIssueDate.Enabled = checkBoxIssueDateEnable.Checked;
        }

        private void checkBoxBeginDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBeginDateExpr.Enabled = checkBoxBeginDateEnable.Checked;
            dateTimePickerBeginDate.Enabled = checkBoxBeginDateEnable.Checked;
        }

        private void checkBoxEndDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxEndDateExpr.Enabled = checkBoxEndDateEnable.Checked;
            dateTimePickerEndDate.Enabled = checkBoxEndDateEnable.Checked;
        }

        private void checkBoxResidenceWarrDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxResidenceWarrDateExpr.Enabled = checkBoxResidenceWarrDateEnable.Checked;
            dateTimePickerResidenceWarrDate.Enabled = checkBoxResidenceWarrDateEnable.Checked;
        }

        private void checkBoxKumiOrderDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxKumiOrderDateExpr.Enabled = checkBoxKumiOrderDateEnable.Checked;
            dateTimePickerKumiOrderDate.Enabled = checkBoxKumiOrderDateEnable.Checked;
        }
    }
}
