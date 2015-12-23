using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;

namespace Registry.Viewport.SearchForms
{
    public partial class SimpleSearchClaimsForm : SearchForm
    {

        public SimpleSearchClaimsForm()
        {
            InitializeComponent();

            DataModel.GetInstance(DataModelType.ClaimStatesDataModel).Select();
            comboBoxLastState.DataSource = new BindingSource
            {
                DataSource = DataModel.DataSet,
                DataMember = "claim_state_types"
            };
            comboBoxLastState.DisplayMember = "state_type";
            comboBoxLastState.ValueMember = "id_state_type";

            foreach (Control control in Controls)
            {
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
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedClaims = null;
            IEnumerable<int> includedAccounts = null;

            if (checkBoxLastStateChecked.Checked && comboBoxLastState.SelectedValue != null)
            {
                var claimStatesDataModel = DataModel.GetInstance(DataModelType.ClaimStatesDataModel);
                var lastStates = from stateRow in claimStatesDataModel.FilterDeletedRows()
                                 group stateRow.Field<int?>("id_state") by stateRow.Field<int>("id_claim") into gs
                                 select new
                                 {
                                     id_claim = gs.Key,
                                     id_state = gs.Max()
                                 };
                var lastStateTypes = from lastStateRow in lastStates
                    join stateRow in claimStatesDataModel.FilterDeletedRows()
                        on lastStateRow.id_state equals stateRow.Field<int?>("id_state")
                    where stateRow.Field<int?>("id_state_type") == (int?) comboBoxLastState.SelectedValue
                    select stateRow.Field<int>("id_claim");
                includedClaims = DataModelHelper.Intersect(null, lastStateTypes);
            }
            if (checkBoxAccountChecked.Checked && !string.IsNullOrEmpty(textBoxAccount.Text.Trim()))
            {
                var accounts =
                    from accountRow in DataModel.GetInstance(DataModelType.PaymentsAccountsDataModel).FilterDeletedRows()
                    where accountRow.Field<string>("account").Contains(textBoxAccount.Text)
                    select accountRow.Field<int>("id_account");
                includedAccounts = DataModelHelper.Intersect(null, accounts);
            }
            if (checkBoxAtDateChecked.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format("at_date = '{0}'", dateTimePickerAtDate.Value.Date.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture));
            }

            if (includedClaims != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_claim IN (0";
                foreach (var id in includedClaims)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            if (includedAccounts != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_account IN (0";
                foreach (var id in includedAccounts)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            return filter;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (checkBoxAccountChecked.Checked && string.IsNullOrEmpty(textBoxAccount.Text.Trim()))
            {
                MessageBox.Show(@"Выберите лицевой счет или уберите галочку фильтрации по лицевому счету",@"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (checkBoxLastStateChecked.Checked && comboBoxLastState.SelectedValue == null)
            {
                MessageBox.Show(@"Выберите состояние исковой работы или уберите галочку фильтрации по последнему состоянию исковой работы", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void checkBoxLastStateChecked_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxLastState.Enabled = checkBoxLastStateChecked.Checked;
        }

        private void checkBoxAccountChecked_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAccount.Enabled = checkBoxAccountChecked.Checked;
        }

        private void checkBoxAtDateChecked_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerAtDate.Enabled = checkBoxAtDateChecked.Checked;
        }
    }
}
