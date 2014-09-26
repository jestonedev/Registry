using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;

namespace Registry
{
    public partial class SearchBuildingForm : Form
    {
        KladrDataModel kladr = null;
        FundTypesDataModel fundTypes = null;

        BindingSource v_kladr = null;
        BindingSource v_fundTypes = null;

        public string GetFilter()
        {
            string filter = "";
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue != null))
                filter += "id_street = '" + comboBoxStreet.SelectedValue.ToString()+"'";
            if (checkBoxHouseEnable.Checked)
            {
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "house = '" + textBoxHouse.Text.Trim()+"'";
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
                    filter += "cadastral_num = '" + textBoxCadastralNum.Text.Trim() + "'";
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
                DataTable table = BuildingsCurrentFundsDataModel.GetInstance().Select();
                BindingSource v_table = new BindingSource();
                v_table.DataSource = table;
                v_table.Filter = "id_fund = " + comboBoxFundType.SelectedValue.ToString();
                if (filter.Trim() != "")
                    filter += " AND ";
                filter += "id_building IN (0";
                for (int i = 0; i < v_table.Count; i++)
                    filter += ((DataRowView)v_table[i])["id_building"].ToString() + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        public SearchBuildingForm()
        {
            InitializeComponent();
            kladr = KladrDataModel.GetInstance();
            fundTypes = FundTypesDataModel.GetInstance();

            v_kladr = new BindingSource();
            v_kladr.DataSource = kladr.Select();

            v_fundTypes = new BindingSource();
            v_fundTypes.DataSource = fundTypes.Select();

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxFundType.DataSource = v_fundTypes;
            comboBoxFundType.ValueMember = "id_fund_type";
            comboBoxFundType.DisplayMember = "fund_type";

            numericUpDownStartupYear.Maximum = DateTime.Now.Year;
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
                MessageBox.Show("Введите номер здания или уберите галочку поиска по номеру дома", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxHouse.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
