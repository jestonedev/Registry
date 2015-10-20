using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport;

namespace Registry.SearchForms
{
    internal partial class ExtendedSearchResettleForm : SearchForm
    {
        DataModel regions;
        BindingSource v_kladr_from;
        BindingSource v_regions_from;
        BindingSource v_kladr_to;
        BindingSource v_regions_to;

        public ExtendedSearchResettleForm()
        {
            InitializeComponent();
            DataModel.GetInstance(DataModelType.KladrStreetsDataModel).Select();
            regions = DataModel.GetInstance(DataModelType.KladrRegionsDataModel);

            var ds = DataModel.DataSet;

            v_kladr_from = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            v_kladr_to = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            v_regions_from = new BindingSource {DataSource = regions.Select()};

            v_regions_to = new BindingSource {DataSource = regions.Select()};

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
            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButtonSearch_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            DialogResult = DialogResult.Cancel;
                };
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedProcesses = null;
            if (checkBoxResettleDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "resettle_date {0} '{1}'",
                    ConvertDisplayEqExprToSql(
                        comboBoxResettleDateExpr.SelectedItem.ToString()),
                        dateTimePickerResettleDate.Value.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            }
            if (checkBoxIDResettleEnable.Checked)
                includedProcesses = DataModelHelper.Intersect(null, new List<int>() { Convert.ToInt32(numericUpDownIDResettle.Value) });
            if (checkBoxPersonSNPEnable.Checked)
            {
                var snp = textBoxPersonSNP.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processesIds = DataModelHelper.ResettleProcessIdsBySnp(snp);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxRegionFromEnable.Checked && (comboBoxRegionFrom.SelectedValue != null))
            {
                var processesIds = DataModelHelper.ResettleProcessIDsByCondition(
                    row => row.Field<string>("id_street").StartsWith(comboBoxRegionFrom.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase), 
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.From);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxStreetFromEnable.Checked && (comboBoxStreetFrom.SelectedValue != null))
            {
                var processesIds = DataModelHelper.ResettleProcessIDsByCondition(
                    row => row.Field<string>("id_street") == comboBoxStreetFrom.SelectedValue.ToString(),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.From);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxHouseFromEnable.Checked)
            {
                var processesIds = DataModelHelper.ResettleProcessIDsByCondition(
                    row => row.Field<string>("house") == textBoxHouseFrom.Text.Trim().Replace("'", ""),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.From);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxPremisesNumFromEnable.Checked)
            {
                var processesIds = DataModelHelper.ResettleProcessIDsByCondition(
                    row => row.Field<string>("premises_num") == textBoxPremisesNumFrom.Text.Trim().Replace("'", ""),
                    DataModelHelper.ConditionType.PremisesCondition, ResettleEstateObjectWay.From);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxRegionToEnable.Checked && (comboBoxRegionTo.SelectedValue != null))
            {
                var processesIds = DataModelHelper.ResettleProcessIDsByCondition(
                    row => row.Field<string>("id_street").StartsWith(
                        comboBoxRegionTo.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.To);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxStreetToEnable.Checked && (comboBoxStreetTo.SelectedValue != null))
            {
                var processesIds = DataModelHelper.ResettleProcessIDsByCondition(
                    row => row.Field<string>("id_street") == comboBoxStreetTo.SelectedValue.ToString(),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.To);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxHouseToEnable.Checked)
            {
                var processesIds = DataModelHelper.ResettleProcessIDsByCondition(
                    row => row.Field<string>("house") == textBoxHouseTo.Text.Trim().Replace("'", ""),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.To);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxPremisesNumToEnable.Checked)
            {
                var processesIds = DataModelHelper.ResettleProcessIDsByCondition(
                    row => row.Field<string>("premises_num") == textBoxPremisesNumTo.Text.Trim().Replace("'", ""),
                    DataModelHelper.ConditionType.PremisesCondition, ResettleEstateObjectWay.To);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (includedProcesses != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_process IN (0";
                foreach (var id in includedProcesses)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
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
            if ((checkBoxPersonSNPEnable.Checked) && string.IsNullOrEmpty(textBoxPersonSNP.Text.Trim()))
            {
                MessageBox.Show(@"Введите ФИО переселенца или уберите галочку поиска по ФИО переселенца", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPersonSNP.Focus();
                return;
            }
            if ((checkBoxStreetFromEnable.Checked) && (comboBoxStreetFrom.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите улицу, с которой производится переселение, или уберите галочку поиска по улице", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreetFrom.Focus();
                return;
            }
            if ((checkBoxHouseFromEnable.Checked) && string.IsNullOrEmpty(textBoxHouseFrom.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер дома, из которого произовдится переселение, или уберите галочку поиска по номеру дома", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouseFrom.Focus();
                return;
            }
            if ((checkBoxPremisesNumFromEnable.Checked) && string.IsNullOrEmpty(textBoxPremisesNumFrom.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер помещения, из которого произовдится переселение, или уберите галочку поиска по номеру помещения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNumFrom.Focus();
                return;
            }
            if ((checkBoxStreetToEnable.Checked) && (comboBoxStreetTo.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите улицу, на которую производится переселение, или уберите галочку поиска по улице", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreetTo.Focus();
                return;
            }
            if ((checkBoxHouseToEnable.Checked) && string.IsNullOrEmpty(textBoxHouseTo.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер дома, с которого произовдится переселение, или уберите галочку поиска по номеру дома", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouseTo.Focus();
                return;
            }
            if ((checkBoxPremisesNumToEnable.Checked) && string.IsNullOrEmpty(textBoxPremisesNumTo.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер помещения, с которого произовдится переселение, или уберите галочку поиска по номеру помещения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNumTo.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
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
                var text = comboBoxStreetFrom.Text;
                var selectionStart = comboBoxStreetFrom.SelectionStart;
                var selectionLength = comboBoxStreetFrom.SelectionLength;
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
                var text = comboBoxStreetTo.Text;
                var selectionStart = comboBoxStreetTo.SelectionStart;
                var selectionLength = comboBoxStreetTo.SelectionLength;
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
