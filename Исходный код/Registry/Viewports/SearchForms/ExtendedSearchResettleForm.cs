using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;

namespace Registry.Viewport.SearchForms
{
    internal partial class ExtendedSearchResettleForm : SearchForm
    {
        private readonly BindingSource _vKladrFrom;
        private readonly BindingSource _vKladrTo;

        public ExtendedSearchResettleForm()
        {
            InitializeComponent();

            DataModel.GetInstance<KladrStreetsDataModel>().Select();
            var regions = DataModel.GetInstance<KladrRegionsDataModel>();

            var ds = DataStorage.DataSet;

            _vKladrFrom = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            _vKladrTo = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            var vRegionsFrom = new BindingSource { DataSource = regions.Select() };

            var vRegionsTo = new BindingSource { DataSource = regions.Select() };

            comboBoxStreetFrom.DataSource = _vKladrFrom;
            comboBoxStreetFrom.ValueMember = "id_street";
            comboBoxStreetFrom.DisplayMember = "street_name";

            comboBoxStreetTo.DataSource = _vKladrTo;
            comboBoxStreetTo.ValueMember = "id_street";
            comboBoxStreetTo.DisplayMember = "street_name";

            comboBoxRegionFrom.DataSource = vRegionsFrom;
            comboBoxRegionFrom.ValueMember = "id_region";
            comboBoxRegionFrom.DisplayMember = "region";

            comboBoxRegionTo.DataSource = vRegionsTo;
            comboBoxRegionTo.ValueMember = "id_region";
            comboBoxRegionTo.DisplayMember = "region";

            comboBoxResettleDateExpr.SelectedIndex = 2;

            HandleHotKeys(Controls, vButtonSearch_Click);
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
                        dateTimePickerResettleDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxIDResettleEnable.Checked)
                includedProcesses = DataModelHelper.Intersect(null, new List<int> { Convert.ToInt32(numericUpDownIDResettle.Value) });
            if (checkBoxPersonSNPEnable.Checked)
            {
                var snp = textBoxPersonSNP.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var processesIds = ResettleService.ResettleProcessIdsBySnp(snp);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxRegionFromEnable.Checked && (comboBoxRegionFrom.SelectedValue != null))
            {
                var processesIds = ResettleService.ResettleProcessIDsByCondition(
                    row => 
                        row.Field<string>("id_street") != null &&
                        row.Field<string>("id_street").StartsWith(comboBoxRegionFrom.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase), 
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.From);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxStreetFromEnable.Checked && (comboBoxStreetFrom.SelectedValue != null))
            {
                var processesIds = ResettleService.ResettleProcessIDsByCondition(
                    row => row.Field<string>("id_street") == comboBoxStreetFrom.SelectedValue.ToString(),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.From);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxHouseFromEnable.Checked)
            {
                var processesIds = ResettleService.ResettleProcessIDsByCondition(
                    row => row.Field<string>("house") == textBoxHouseFrom.Text.Trim().Replace("'", ""),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.From);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxPremisesNumFromEnable.Checked)
            {
                var processesIds = ResettleService.ResettleProcessIDsByCondition(
                    row => row.Field<string>("premises_num") == textBoxPremisesNumFrom.Text.Trim().Replace("'", ""),
                    DataModelHelper.ConditionType.PremisesCondition, ResettleEstateObjectWay.From);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxRegionToEnable.Checked && (comboBoxRegionTo.SelectedValue != null))
            {
                var processesIds = ResettleService.ResettleProcessIDsByCondition(
                    row => row.Field<string>("id_street") != null && row.Field<string>("id_street").StartsWith(
                        comboBoxRegionTo.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.To);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxStreetToEnable.Checked && (comboBoxStreetTo.SelectedValue != null))
            {
                var processesIds = ResettleService.ResettleProcessIDsByCondition(
                    row => row.Field<string>("id_street") == comboBoxStreetTo.SelectedValue.ToString(),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.To);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxHouseToEnable.Checked)
            {
                var processesIds = ResettleService.ResettleProcessIDsByCondition(
                    row => row.Field<string>("house") == textBoxHouseTo.Text.Trim().Replace("'", ""),
                    DataModelHelper.ConditionType.BuildingCondition, ResettleEstateObjectWay.To);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (checkBoxPremisesNumToEnable.Checked)
            {
                var processesIds = ResettleService.ResettleProcessIDsByCondition(
                    row => row.Field<string>("premises_num") == textBoxPremisesNumTo.Text.Trim().Replace("'", ""),
                    DataModelHelper.ConditionType.PremisesCondition, ResettleEstateObjectWay.To);
                includedProcesses = DataModelHelper.Intersect(includedProcesses, processesIds);
            }
            if (includedProcesses == null) return filter == "" ? "0 = 1" : filter;
            if (!string.IsNullOrEmpty(filter.Trim()))
                filter += " AND ";
            filter += "id_process IN (0";
            foreach (var id in includedProcesses)
                filter += id.ToString(CultureInfo.InvariantCulture) + ",";
            filter = filter.TrimEnd(',') + ")";
            return filter == "" ? "0 = 1" : filter;
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
                _vKladrFrom.Filter = "street_name like '%" + comboBoxStreetFrom.Text + "%'";
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
                    comboBoxStreetFrom.SelectedValue = _vKladrFrom[_vKladrFrom.Position];
                comboBoxStreetFrom.Text = ((DataRowView)_vKladrFrom[_vKladrFrom.Position])["street_name"].ToString();
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
                _vKladrTo.Filter = "street_name like '%" + comboBoxStreetTo.Text + "%'";
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
                    comboBoxStreetTo.SelectedValue = _vKladrTo[_vKladrTo.Position];
                comboBoxStreetTo.Text = ((DataRowView)_vKladrTo[_vKladrTo.Position])["street_name"].ToString();
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
