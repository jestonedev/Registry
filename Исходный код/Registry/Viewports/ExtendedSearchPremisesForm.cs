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
using System.Globalization;

namespace Registry.SearchForms
{
    internal partial class ExtendedSearchPremisesForm : SearchForm
    {
        KladrRegionsDataModel regions = null;

        BindingSource v_kladr = null;
        BindingSource v_regions = null;
        BindingSource v_fundTypes = null;
        BindingSource v_object_states = null;

        internal override string GetFilter()
        {
            string filter = "";
            IEnumerable<int> included_premises = null;
            IEnumerable<int> included_buildings = null;
            if (checkBoxPremisesNumEnable.Checked && !String.IsNullOrEmpty(textBoxPremisesNum.Text.Trim()))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.CurrentCulture, "premises_num = '{0}'", textBoxPremisesNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxFloorEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "floor = " + numericUpDownFloor.Value.ToString(CultureInfo.CurrentCulture);
            }
            if (checkBoxCadastralNumEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                if (!String.IsNullOrEmpty(textBoxCadastralNum.Text.Trim()))
                    filter += String.Format(CultureInfo.CurrentCulture, "cadastral_num = '{0}'", textBoxCadastralNum.Text.Trim().Replace("'", ""));
                else
                    filter += "cadastral_num is null";
            }
            if ((checkBoxStateEnable.Checked) && (comboBoxState.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_state = " + comboBoxState.SelectedValue.ToString();
            }
            if (checkBoxIDPremisesEnable.Checked)
                included_premises = DataModelHelper.Intersect(included_premises, new List<int>() { Convert.ToInt32(numericUpDownIDPremises.Value) });
            if ((checkBoxRegionEnable.Checked) && (comboBoxRegion.SelectedValue != null))
            {
                IEnumerable<int> buildings_ids = DataModelHelper.BuildingIDsByRegion(comboBoxRegion.SelectedValue.ToString());
                included_buildings = DataModelHelper.Intersect(included_buildings, buildings_ids);
            }
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue != null))
            {
                IEnumerable<int> buildings_ids = DataModelHelper.BuildingIDsByStreet(comboBoxStreet.SelectedValue.ToString());
                included_buildings = DataModelHelper.Intersect(included_buildings, buildings_ids);
            }
            if (checkBoxHouseEnable.Checked)
            {
                IEnumerable<int> buildings_ids = DataModelHelper.BuildingIDsByHouse(textBoxHouse.Text.Trim().Replace("'", ""));
                included_buildings = DataModelHelper.Intersect(included_buildings, buildings_ids);
            }
            if ((checkBoxFundTypeEnable.Checked) && (comboBoxStreet.SelectedValue != null))
            {
                IEnumerable<int> premises_ids = DataModelHelper.PremiseIDsByCurrentFund(
                    Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.CurrentCulture));
                included_premises = DataModelHelper.Intersect(included_premises, premises_ids);        
            }
            if (checkBoxContractNumberEnable.Checked)
            {
                IEnumerable<int> premises_ids = DataModelHelper.PremiseIDsByRegistrationNumber(textBoxContractNumber.Text.Trim().Replace("'", ""));
                included_premises = DataModelHelper.Intersect(included_premises, premises_ids);    
            }
            if (checkBoxTenantSNPEnable.Checked)
            {
                string[] snp = textBoxTenantSNP.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> premises_ids = DataModelHelper.PremisesIDsBySNP(snp, (row) => { return row.Field<int>("id_kinship") == 1; });
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
            if (included_buildings != null)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_building IN (0";
                foreach (int id in included_buildings)
                    filter += id.ToString(CultureInfo.CurrentCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        public ExtendedSearchPremisesForm()
        {
            InitializeComponent();
            KladrStreetsDataModel.GetInstance().Select();
            FundTypesDataModel.GetInstance().Select();
            ObjectStatesDataModel.GetInstance().Select();
            regions = KladrRegionsDataModel.GetInstance();

            DataSet ds = DataSetManager.DataSet;

            v_kladr = new BindingSource();
            v_kladr.DataSource = ds;
            v_kladr.DataMember = "kladr";

            v_regions = new BindingSource();
            v_regions.DataSource = regions.Select();

            v_fundTypes = new BindingSource();
            v_fundTypes.DataSource = ds;
            v_fundTypes.DataMember = "fund_types";

            v_object_states = new BindingSource();
            v_object_states.DataSource = ds;
            v_object_states.DataMember = "object_states";

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxFundType.DataSource = v_fundTypes;
            comboBoxFundType.ValueMember = "id_fund_type";
            comboBoxFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = v_object_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";

            comboBoxRegion.DataSource = v_regions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";
            foreach (Control control in this.Controls)
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

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue == null))
            {
                MessageBox.Show("Выберите улицу или уберите галочку поиска по улице", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return;
            }
            if ((checkBoxHouseEnable.Checked) && String.IsNullOrEmpty(textBoxHouse.Text.Trim()))
            {
                MessageBox.Show("Введите номер дома или уберите галочку поиска по номеру дома", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if ((checkBoxPremisesNumEnable.Checked) && String.IsNullOrEmpty(textBoxPremisesNum.Text.Trim()))
            {
                MessageBox.Show("Введите номер помещения или уберите галочку поиска по номеру помещения", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNum.Focus();
                return;
            }
            if ((checkBoxCadastralNumEnable.Checked) && String.IsNullOrEmpty(textBoxCadastralNum.Text.Trim()))
            {
                MessageBox.Show("Введите кадастровый номер или уберите галочку поиска по кадастровому номеру", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxCadastralNum.Focus();
                return;
            }
            if ((checkBoxContractNumberEnable.Checked) && String.IsNullOrEmpty(textBoxContractNumber.Text.Trim()))
            {
                MessageBox.Show("Введите номер договора найма или уберите галочку поиска по номеру договора найма",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxContractNumber.Focus();
                return;
            }
            if ((checkBoxTenantSNPEnable.Checked) && String.IsNullOrEmpty(textBoxTenantSNP.Text.Trim()))
            {
                MessageBox.Show("Введите ФИО нанимателя или уберите галочку поиска по ФИО нанимателя",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxTenantSNP.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
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

        private void checkBoxFloorEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownFloor.Enabled = checkBoxFloorEnable.Checked;
        }

        private void checkBoxCadastralNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCadastralNum.Enabled = checkBoxCadastralNumEnable.Checked;
        }

        private void checkBoxFundTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxFundType.Enabled = checkBoxFundTypeEnable.Checked;
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

        private void checkBoxStateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxState.Enabled = checkBoxStateEnable.Checked;
        }

        private void checkBoxIDBuildingEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDPremises.Enabled = checkBoxIDPremisesEnable.Checked;
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
