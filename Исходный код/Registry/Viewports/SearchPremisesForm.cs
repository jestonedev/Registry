using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;

namespace Registry.Viewport
{
    public partial class SearchPremisesForm : Form
    {
        KladrDataModel kladr = null;
        FundTypesDataModel fundTypes = null;

        BindingSource v_kladr = null;
        BindingSource v_fundTypes = null;

        public string GetFilter()
        {
            string filter = "";
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue != null))
            {
                DataTable table = BuildingsDataModel.GetInstance().Select();
                BindingSource v_table = new BindingSource();
                v_table.DataSource = table;
                v_table.Filter = "id_street = " + comboBoxStreet.SelectedValue.ToString();
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "id_building IN (0";
                for (int i = 0; i < v_table.Count; i++)
                    filter += ((DataRowView)v_table[i])["id_building"].ToString() + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            if (checkBoxHouseEnable.Checked)
            {
                DataTable table = BuildingsDataModel.GetInstance().Select();
                BindingSource v_table = new BindingSource();
                v_table.DataSource = table;
                v_table.Filter = "house = " + textBoxHouse.Text.Trim();
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "id_building IN (0";
                for (int i = 0; i < v_table.Count; i++)
                    filter += ((DataRowView)v_table[i])["id_building"].ToString() + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            if (checkBoxPremisesNumEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                if (textBoxPremisesNum.Text.Trim() != "")
                    filter += "premises_num = '" + textBoxPremisesNum.Text.Trim() + "'";
                else
                    filter += "premises_num is null";
            }
            if (checkBoxFloorEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "floor = " + numericUpDownFloor.Value.ToString();
            }
            if (checkBoxCadastralNumEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                if (textBoxCadastralNum.Text.Trim() != "")
                    filter += "cadastral_num = '" + textBoxCadastralNum.Text.Trim() + "'";
                else
                    filter += "cadastral_num is null";
            }
            if (checkBoxForOrpahnsEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "for_orphans = " + (checkBoxForOrpahns.Checked ? 1 : 0).ToString();
            }
            if (checkBoxAcceptedByDonationEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "accepted_by_donation = " + (checkBoxAcceptedByDonation.Checked ? 1 : 0).ToString();
            }
            if (checkBoxAcceptedByExchangeEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "accepted_by_exchange = " + (checkBoxAcceptedByExchange.Checked ? 1 : 0).ToString();
            }
            if (checkBoxAcceptedByOtherEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "accepted_by_other = " + (checkBoxAcceptedByOther.Checked ? 1 : 0).ToString();
            }
            if ((checkBoxFundTypeEnable.Checked) && (comboBoxStreet.SelectedValue != null))
            {
                DataTable table = PremisesCurrentFundsDataModel.GetInstance().Select();
                BindingSource v_table = new BindingSource();
                v_table.DataSource = table;
                v_table.Filter = "id_fund = " + comboBoxFundType.SelectedValue.ToString();
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "id_premises IN (0";
                for (int i = 0; i < v_table.Count; i++)
                    filter += ((DataRowView)v_table[i])["id_premises"].ToString() + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        public SearchPremisesForm()
        {
            InitializeComponent();
            kladr = KladrDataModel.GetInstance();
            fundTypes = FundTypesDataModel.GetInstance();

            DataSet ds = DataSetManager.GetDataSet();

            v_kladr = new BindingSource();
            v_kladr.DataSource = ds;
            v_kladr.DataMember = "kladr";

            v_fundTypes = new BindingSource();
            v_fundTypes.DataSource = ds;
            v_fundTypes.DataMember = "fund_types";

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxFundType.DataSource = v_fundTypes;
            comboBoxFundType.ValueMember = "id_fund_type";
            comboBoxFundType.DisplayMember = "fund_type";
        }

        private void vButton1_Click(object sender, EventArgs e)
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

        private void checkBoxForOrpahnsEnable_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxForOrpahns.Enabled = checkBoxForOrpahnsEnable.Checked;
        }

        private void checkBoxAcceptedByExchangeEnable_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAcceptedByExchange.Enabled = checkBoxAcceptedByExchangeEnable.Checked;
        }

        private void checkBoxAcceptedByDonationEnable_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAcceptedByDonation.Enabled = checkBoxAcceptedByDonationEnable.Checked;
        }

        private void checkBoxAcceptedByOtherEnbale_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAcceptedByOther.Enabled = checkBoxAcceptedByOtherEnable.Checked;
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
    }
}
