using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.CalcDataModels;

namespace Registry.SearchForms
{
    internal partial class ExtendedSearchBuildingForm : SearchForm
    {
        KladrStreetsDataModel kladr = null;
        KladrRegionsDataModel regions = null;
        FundTypesDataModel fundTypes = null;
        StatesDataModel states = null;

        BindingSource v_kladr = null;
        BindingSource v_regions = null;
        BindingSource v_fundTypes = null;
        BindingSource v_states = null;

        internal override string GetFilter()
        {
            string filter = "";
            List<int> included_buildings = null;
            if (checkBoxIDBuildingEnable.Checked)
            {
                if (included_buildings == null)
                    included_buildings = new List<int>();
                included_buildings.Add(Convert.ToInt32(numericUpDownIDBuilding.Value));
            }
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue != null))
                filter += "id_street = '" + comboBoxStreet.SelectedValue.ToString()+"'";
            if (checkBoxRegionEnable.Checked && comboBoxRegion.SelectedValue != null)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "id_street LIKE '"+comboBoxRegion.SelectedValue.ToString()+"%'";
            }
            if (checkBoxHouseEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += String.Format("house = '{0}'", textBoxHouse.Text.Trim().Replace("'", ""));
            }
            if (checkBoxFloorsEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "floors = " + numericUpDownFloors.Value.ToString();
            }
            if (checkBoxCadastralNumEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                if (textBoxCadastralNum.Text.Trim() != "")
                    filter += String.Format("cadastral_num = '{0}'", textBoxCadastralNum.Text.Trim().Replace("'", ""));
                else
                    filter += "cadastral_num is null";
            }
            if (checkBoxStartupYearEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "startup_year = " + numericUpDownStartupYear.Value;
            }
            if (checkBoxImprovementEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "improvement = " + (checkBoxImprovement.Checked ? 1 : 0).ToString();
            }
            if (checkBoxElevatorEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "elevator = " + (checkBoxElevator.Checked ? 1 : 0).ToString();
            }
            if ((checkBoxFundTypeEnable.Checked) && (comboBoxStreet.SelectedValue != null))
            {
                DataTable table = CalcDataModelBuildingsCurrentFunds.GetInstance().Select();
                List<int> buildings_ids = (from funds_row in table.AsEnumerable()
                                         where funds_row.Field<int>("id_fund_type") == Convert.ToInt32(comboBoxFundType.SelectedValue)
                                         select funds_row.Field<int>("id_building")).ToList();
                if (included_buildings != null)
                    included_buildings = included_buildings.Intersect(buildings_ids).ToList();
                else
                    included_buildings = buildings_ids;
            }
            if ((checkBoxStateEnable.Checked) && (comboBoxState.SelectedValue != null))
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "id_state = " + comboBoxState.SelectedValue.ToString();
            }
            if (checkBoxContractNumberEnable.Checked)
            {
                List<int> contract_ids = new List<int>();
                DataTable tenancy_buildings_assoc = TenancyBuildingsAssocDataModel.GetInstance().Select();
                DataTable tenancy_contracts = TenancyContractsDataModel.GetInstance().Select();

                contract_ids = (from tenancy_buildings_assoc_row in tenancy_buildings_assoc.AsEnumerable()
                                join tenancy_contract_row in tenancy_contracts.AsEnumerable()
                                on tenancy_buildings_assoc_row.Field<int>("id_contract") equals tenancy_contract_row.Field<int>("id_contract")
                                where tenancy_contract_row.Field<string>("registration_num") == textBoxContractNumber.Text.Trim().Replace("'", "")
                                select tenancy_buildings_assoc_row.Field<int>("id_building")).ToList();
                if (included_buildings != null)
                    included_buildings = included_buildings.Intersect(contract_ids).ToList();
                else
                    included_buildings = contract_ids;
            }
            if (checkBoxTenantSNPEnable.Checked)
            {
                List<int> contract_ids = new List<int>();
                string[] snp = textBoxTenantSNP.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                DataTable tenancy_buildings_assoc = TenancyBuildingsAssocDataModel.GetInstance().Select();
                DataTable persons = PersonsDataModel.GetInstance().Select();

                contract_ids = (from tenancy_buildings_assoc_row in tenancy_buildings_assoc.AsEnumerable()
                                join persons_row in persons.AsEnumerable()
                                on tenancy_buildings_assoc_row.Field<int>("id_contract") equals persons_row.Field<int>("id_contract")
                                where (persons_row.Field<int>("id_kinship") == 1) && 
                                      ((snp.Count() == 1) ? persons_row.Field<string>("surname") == snp[0] :
                                       (snp.Count() == 2) ? persons_row.Field<string>("surname") == snp[0] && persons_row.Field<string>("name") == snp[1] :
                                       (snp.Count() == 3) ? persons_row.Field<string>("surname") == snp[0] && persons_row.Field<string>("name") == snp[1] &&
                                       persons_row.Field<string>("patronymic") == snp[2] : false)
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

        public ExtendedSearchBuildingForm()
        {
            InitializeComponent();
            kladr = KladrStreetsDataModel.GetInstance();
            fundTypes = FundTypesDataModel.GetInstance();
            states = StatesDataModel.GetInstance();
            regions = KladrRegionsDataModel.GetInstance();

            v_kladr = new BindingSource();
            v_kladr.DataSource = kladr.Select();

            v_regions = new BindingSource();
            v_regions.DataSource = regions.Select();

            v_fundTypes = new BindingSource();
            v_fundTypes.DataSource = fundTypes.Select();

            v_states = new BindingSource();
            v_states.DataSource = states.Select();

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxFundType.DataSource = v_fundTypes;
            comboBoxFundType.ValueMember = "id_fund_type";
            comboBoxFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = v_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";

            comboBoxRegion.DataSource = v_regions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            numericUpDownStartupYear.Maximum = DateTime.Now.Year;
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

        private void checkBoxStreetEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStreet.Enabled = checkBoxStreetEnable.Checked;
        }

        private void checkBoxHouseEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxHouse.Enabled = checkBoxHouseEnable.Checked;
        }

        private void checkBoxFloorsEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownFloors.Enabled = checkBoxFloorsEnable.Checked;
        }

        private void checkBoxCadastralNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCadastralNum.Enabled = checkBoxCadastralNumEnable.Checked;
        }

        private void checkBoxStartupYearEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownStartupYear.Enabled = checkBoxStartupYearEnable.Checked;
        }

        private void checkBoxFundTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxFundType.Enabled = checkBoxFundTypeEnable.Checked;
        }

        private void checkBoxImprovementEnable_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxImprovement.Enabled = checkBoxImprovementEnable.Checked;
        }

        private void checkBoxElevatorEnable_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxElevator.Enabled = checkBoxElevatorEnable.Checked;
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

        private void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
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
                MessageBox.Show("Введите номер здания или уберите галочку поиска по номеру дома", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxHouse.Focus();
                return;
            }
            if ((checkBoxContractNumberEnable.Checked) && (textBoxContractNumber.Text.Trim() == ""))
            {
                MessageBox.Show("Введите номер договора найма или уберите галочку поиска по номеру договора найма", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxContractNumber.Focus();
                return;
            }
            if ((checkBoxTenantSNPEnable.Checked) && (textBoxTenantSNP.Text.Trim() == ""))
            {
                MessageBox.Show("Введите ФИО нанимателя или уберите галочку поиска по ФИО нанимателя",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxTenantSNP.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void checkBoxStateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxState.Enabled = checkBoxStateEnable.Checked;
        }

        private void checkBoxIDBuildingEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDBuilding.Enabled = checkBoxIDBuildingEnable.Checked;
        }

        private void checkBoxRegionEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegion.Enabled = checkBoxRegionEnable.Checked;
        }

        private void checkBoxContractNumberEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxContractNumber.Enabled = checkBoxContractNumberEnable.Checked;
        }

        private void checkBoxTenantSNPEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTenantSNP.Enabled = checkBoxTenantSNPEnable.Checked;
        }
    }
}
