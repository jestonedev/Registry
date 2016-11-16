using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;

namespace Registry.Viewport.SearchForms
{
    internal partial class SimpleSearchPaymentAccounts : SearchForm
    {
        public SimpleSearchPaymentAccounts()
        {
            InitializeComponent();
            comboBoxCriteriaType.SelectedIndex = 0;
            HandleHotKeys(Controls, vButtonSearch_Click);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            textBoxCriteria.Focus();
            textBoxCriteria.SelectAll();
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedAccounts = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("account LIKE '%{0}%'", textBoxCriteria.Text.Trim().Replace("'", ""));
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("crn LIKE '%{0}%'", textBoxCriteria.Text.Trim().Replace("'", ""));
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                includedAccounts =
                        PaymentService.GetAccountIdsByPaymentFilter(string.Format("tenant LIKE '%{0}%'", textBoxCriteria.Text.Trim().Replace("'", "")));
            }
            if (comboBoxCriteriaType.SelectedIndex == 3)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                var addressParts = textBoxCriteria.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var addressFilter = "";
                foreach (var part in addressParts)
                {
                    if (!string.IsNullOrEmpty(addressFilter))
                        addressFilter += " AND ";
                    addressFilter += addressFilter + string.Format("raw_address LIKE '%{0}%'", part.Replace("'", ""));
                }
                filter += string.Format("({0})", addressFilter);
            }
            if (includedAccounts != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_account IN (0";
                filter = includedAccounts.Aggregate(filter, (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
                filter = filter.TrimEnd(',') + ")";
            }
            return filter;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCriteria.Text.Trim()))
            {
                MessageBox.Show(@"Не ввиден критерий поиска", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void comboBoxCriteriaType_DropDownClosed(object sender, EventArgs e)
        {
            textBoxCriteria.Focus();
            textBoxCriteria.SelectAll();
        }

        private void textBoxCriteria_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
