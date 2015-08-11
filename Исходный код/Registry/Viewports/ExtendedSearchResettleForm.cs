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
using System.Globalization;
using Registry.Entities;

namespace Registry.SearchForms
{
    internal partial class ExtendedSearchResettleForm : SearchForm
    {
        KladrRegionsDataModel regions = null;
        BindingSource v_kladr_from = null;
        BindingSource v_regions_from = null;
        BindingSource v_kladr_to = null;
        BindingSource v_regions_to = null;

        public ExtendedSearchResettleForm()
        {
            InitializeComponent();
            KladrStreetsDataModel.GetInstance().Select();
            regions = KladrRegionsDataModel.GetInstance();

            DataSet ds = DataSetManager.DataSet;

            v_kladr_from = new BindingSource();
            v_kladr_from.DataSource = ds;
            v_kladr_from.DataMember = "kladr";

            v_kladr_to = new BindingSource();
            v_kladr_to.DataSource = ds;
            v_kladr_to.DataMember = "kladr";

            v_regions_from = new BindingSource();
            v_regions_from.DataSource = regions.Select();

            v_regions_to = new BindingSource();
            v_regions_to.DataSource = regions.Select();

            comboBoxStreetFrom.DataSource = v_kladr_from;
            comboBoxStreetFrom.ValueMember = "id_street";
            comboBoxStreetFrom.DisplayMember = "street_name";

            comboBoxStreetTo.DataSource = v_kladr_to;
            comboBoxStreetTo.ValueMember = "id_street";
            comboBoxStreetTo.DisplayMember = "street_name";

            comboBoxRegionFrom.DataSource = v_regions_from;
            comboBoxRegionFrom.ValueMember = "id_region";
            comboBoxRegionFrom.DisplayMember = "region";

            comboBoxRegionTo.DataSource = v_regions_to;
            comboBoxRegionTo.ValueMember = "id_region";
            comboBoxRegionTo.DisplayMember = "region";

            comboBoxResettleDateExpr.SelectedIndex = 2;
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

        internal override string GetFilter()
        {
            string filter = "";
            IEnumerable<int> included_processes = null;
            if (checkBoxResettleDateEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "resettle_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxResettleDateExpr.SelectedItem.ToString()),
                        dateTimePickerResettleDate.Value.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            }
            if (checkBoxIDResettleEnable.Checked)
                included_processes = DataModelHelper.Intersect(included_processes, new List<int>() { Convert.ToInt32(numericUpDownIDResettle.Value) });
            if (checkBoxPersonSNPEnable.Checked)
            {
                string[] snp = textBoxPersonSNP.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsBySNP(snp);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxRegionFromEnable.Checked && (comboBoxRegionFrom.SelectedValue != null))
            {
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByCondition(
                    (row) => { return row.Field<string>("id_street").StartsWith(comboBoxRegionFrom.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase); }, 
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.From);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxStreetFromEnable.Checked && (comboBoxStreetFrom.SelectedValue != null))
            {
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByCondition(
                    (row) => { return row.Field<string>("id_street") == comboBoxStreetFrom.SelectedValue.ToString(); },
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.From);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxHouseFromEnable.Checked)
            {
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByCondition(
                    (row) => { return row.Field<string>("house") == textBoxHouseFrom.Text.Trim().Replace("'", ""); },
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.From);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxPremisesNumFromEnable.Checked)
            {
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByCondition(
                    (row) => { return row.Field<string>("premises_num") == textBoxPremisesNumFrom.Text.Trim().Replace("'", ""); },
                    DataModelHelper.ConditionType.PremisesCondition, ResettleEstateObjectWay.From);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxRegionToEnable.Checked && (comboBoxRegionTo.SelectedValue != null))
            {
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByCondition(
                    (row) => { return row.Field<string>("id_street").StartsWith(comboBoxRegionTo.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase); },
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.To);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxStreetToEnable.Checked && (comboBoxStreetTo.SelectedValue != null))
            {
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByCondition(
                    (row) => { return row.Field<string>("id_street") == comboBoxStreetTo.SelectedValue.ToString(); },
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.To);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxHouseToEnable.Checked)
            {
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByCondition(
                    (row) => { return row.Field<string>("house") == textBoxHouseTo.Text.Trim().Replace("'", ""); },
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.To);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (checkBoxPremisesNumToEnable.Checked)
            {
                IEnumerable<int> processes_ids = DataModelHelper.ResettleProcessIDsByCondition(
                    (row) => { return row.Field<string>("premises_num") == textBoxPremisesNumTo.Text.Trim().Replace("'", ""); },
                    DataModelHelper.ConditionType.PremisesCondition, ResettleEstateObjectWay.To);
                included_processes = DataModelHelper.Intersect(included_processes, processes_ids);
            }
            if (included_processes != null)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_process IN (0";
                foreach (int id in included_processes)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        private static string ConvertDisplayEqExprToSql(string expr)
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

        private void checkBoxIDTenancyEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDResettle.Enabled = checkBoxIDResettleEnable.Checked;
        }

        private void checkBoxPersonSNPEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPersonSNP.Enabled = checkBoxPersonSNPEnable.Checked;
        }

        private void checkBoxRegionEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegionFrom.Enabled = checkBoxRegionFromEnable.Checked;
        }

        private void checkBoxStreetEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStreetFrom.Enabled = checkBoxStreetFromEnable.Checked;
        }

        private void checkBoxHouseEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxHouseFrom.Enabled = checkBoxHouseFromEnable.Checked;
        }

        private void checkBoxPremisesNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPremisesNumFrom.Enabled = checkBoxPremisesNumFromEnable.Checked;
        }

        private void checkBoxRegionToEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegionTo.Enabled = checkBoxRegionToEnable.Checked;
        }

        private void checkBoxStreetToEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStreetTo.Enabled = checkBoxStreetToEnable.Checked;
        }

        private void checkBoxHouseToEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxHouseTo.Enabled = checkBoxHouseToEnable.Checked;
        }

        private void checkBoxPremisesNumToEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPremisesNumTo.Enabled = checkBoxPremisesNumToEnable.Checked;
        }

        private void checkBoxRegDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxResettleDateExpr.Enabled = checkBoxResettleDateEnable.Checked;
            dateTimePickerResettleDate.Enabled = checkBoxResettleDateEnable.Checked;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if ((checkBoxPersonSNPEnable.Checked) && String.IsNullOrEmpty(textBoxPersonSNP.Text.Trim()))
            {
                MessageBox.Show("Введите ФИО переселенца или уберите галочку поиска по ФИО переселенца", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPersonSNP.Focus();
                return;
            }
            if ((checkBoxStreetFromEnable.Checked) && (comboBoxStreetFrom.SelectedValue == null))
            {
                MessageBox.Show("Выберите улицу, с которой производится переселение, или уберите галочку поиска по улице", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreetFrom.Focus();
                return;
            }
            if ((checkBoxHouseFromEnable.Checked) && String.IsNullOrEmpty(textBoxHouseFrom.Text.Trim()))
            {
                MessageBox.Show("Введите номер дома, из которого произовдится переселение, или уберите галочку поиска по номеру дома", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouseFrom.Focus();
                return;
            }
            if ((checkBoxPremisesNumFromEnable.Checked) && String.IsNullOrEmpty(textBoxPremisesNumFrom.Text.Trim()))
            {
                MessageBox.Show("Введите номер помещения, из которого произовдится переселение, или уберите галочку поиска по номеру помещения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNumFrom.Focus();
                return;
            }
            if ((checkBoxStreetToEnable.Checked) && (comboBoxStreetTo.SelectedValue == null))
            {
                MessageBox.Show("Выберите улицу, на которую производится переселение, или уберите галочку поиска по улице", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreetTo.Focus();
                return;
            }
            if ((checkBoxHouseToEnable.Checked) && String.IsNullOrEmpty(textBoxHouseTo.Text.Trim()))
            {
                MessageBox.Show("Введите номер дома, с которого произовдится переселение, или уберите галочку поиска по номеру дома", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouseTo.Focus();
                return;
            }
            if ((checkBoxPremisesNumToEnable.Checked) && String.IsNullOrEmpty(textBoxPremisesNumTo.Text.Trim()))
            {
                MessageBox.Show("Введите номер помещения, с которого произовдится переселение, или уберите галочку поиска по номеру помещения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNumTo.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreetFrom.Items.Count == 0)
                comboBoxStreetFrom.SelectedIndex = -1;
        }

        private void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                string text = comboBoxStreetFrom.Text;
                int selectionStart = comboBoxStreetFrom.SelectionStart;
                int selectionLength = comboBoxStreetFrom.SelectionLength;
                v_kladr_from.Filter = "street_name like '%" + comboBoxStreetFrom.Text + "%'";
                comboBoxStreetFrom.Text = text;
                comboBoxStreetFrom.SelectionStart = selectionStart;
                comboBoxStreetFrom.SelectionLength = selectionLength;
            }
        }

        private void comboBoxStreet_Leave(object sender, EventArgs e)
        {
            if (comboBoxStreetFrom.Items.Count > 0)
            {
                if (comboBoxStreetFrom.SelectedValue == null)
                    comboBoxStreetFrom.SelectedValue = v_kladr_from[v_kladr_from.Position];
                comboBoxStreetFrom.Text = ((DataRowView)v_kladr_from[v_kladr_from.Position])["street_name"].ToString();
            }
            if (comboBoxStreetFrom.SelectedValue == null)
                comboBoxStreetFrom.Text = "";
        }

        private void comboBoxStreetTo_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreetTo.Items.Count == 0)
                comboBoxStreetTo.SelectedIndex = -1;
        }

        private void comboBoxStreetTo_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back))
            {
                string text = comboBoxStreetTo.Text;
                int selectionStart = comboBoxStreetTo.SelectionStart;
                int selectionLength = comboBoxStreetTo.SelectionLength;
                v_kladr_to.Filter = "street_name like '%" + comboBoxStreetTo.Text + "%'";
                comboBoxStreetTo.Text = text;
                comboBoxStreetTo.SelectionStart = selectionStart;
                comboBoxStreetTo.SelectionLength = selectionLength;
            }
        }

        private void comboBoxStreetTo_Leave(object sender, EventArgs e)
        {
            if (comboBoxStreetTo.Items.Count > 0)
            {
                if (comboBoxStreetTo.SelectedValue == null)
                    comboBoxStreetTo.SelectedValue = v_kladr_to[v_kladr_to.Position];
                comboBoxStreetTo.Text = ((DataRowView)v_kladr_to[v_kladr_to.Position])["street_name"].ToString();
            }
            if (comboBoxStreetTo.SelectedValue == null)
                comboBoxStreetTo.Text = "";
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
