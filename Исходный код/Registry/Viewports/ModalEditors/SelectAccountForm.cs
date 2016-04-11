using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.Viewport.ModalEditors
{
    internal partial class SelectAccountForm : Form
    {
        private readonly BindingSource _viewModel;
        public int? IdAccount
        {
            get
            {
                if (_viewModel.Position != -1 && ((DataRowView)_viewModel[_viewModel.Position])["id_account"] != DBNull.Value)
                    return (int?) ((DataRowView) _viewModel[_viewModel.Position])["id_account"];
                return null;
            }
        }

        public SelectAccountForm(IEnumerable<int?> idAccounts)
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
            var model = DataModel.GetInstance<PaymentsAccountsDataModel>();
            model.Select();
            _viewModel = new BindingSource
            {
                DataSource = DataModel.DataSet,
                DataMember = "payments_accounts",
                Filter =
                    string.Format("id_account IN (0{0})",
                        idAccounts.Select(v => v.ToString()).Aggregate((acc, v) => acc + "," + v).Trim(','))
            };
            dataGridView.DataSource = _viewModel;
            date.DataPropertyName = "date";
            id_account.DataPropertyName = "id_account";
            crn.DataPropertyName = "crn";
            raw_address.DataPropertyName = "raw_address";
            parsed_address.DataPropertyName = "parsed_address";
            account.DataPropertyName = "account";
            tenant.DataPropertyName = "tenant";
            total_area.DataPropertyName = "total_area";
            living_area.DataPropertyName = "living_area";
            prescribed.DataPropertyName = "prescribed";
            balance_input.DataPropertyName = "balance_input";
            balance_tenancy.DataPropertyName = "balance_tenancy";
            balance_dgi.DataPropertyName = "balance_dgi";
            charging_tenancy.DataPropertyName = "charging_tenancy";
            charging_dgi.DataPropertyName = "charging_dgi";
            charging_total.DataPropertyName = "charging_total";
            recalc_tenancy.DataPropertyName = "recalc_tenancy";
            recalc_dgi.DataPropertyName = "recalc_dgi";
            payment_tenancy.DataPropertyName = "payment_tenancy";
            payment_dgi.DataPropertyName = "payment_dgi";
            transfer_balance.DataPropertyName = "transfer_balance";
            balance_output_total.DataPropertyName = "balance_output_total";
            balance_output_tenancy.DataPropertyName = "balance_output_tenancy";
            balance_output_dgi.DataPropertyName = "balance_output_dgi";
            if (_viewModel.Count > 0)
                _viewModel.Position = 0;
        }

        public SelectAccountForm()
        {
            InitializeComponent();
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            if (_viewModel.Position == -1)
            {
                MessageBox.Show(@"Выберите лицевой счет для сохранения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
