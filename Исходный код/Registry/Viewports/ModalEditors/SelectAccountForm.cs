﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using System.Drawing;

namespace Registry.Viewport.ModalEditors
{
    internal partial class SelectAccountForm : Form
    {
        private readonly BindingSource _viewModel;
        private readonly int? _idAccountCurrent;
        public int? IdAccount
        {
            get
            {
                if (_viewModel.Position != -1 && ((DataRowView)_viewModel[_viewModel.Position])["id_account"] != DBNull.Value)
                    return (int?) ((DataRowView) _viewModel[_viewModel.Position])["id_account"];
                return null;
            }
        }

        public SelectAccountForm(IEnumerable<int> idAccounts, int? idAccountCurrent)
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
            var model = DataModel.GetInstance<PaymentsAccountsDataModel>();
            model.Select();
            _viewModel = new BindingSource
            {
                DataSource = DataStorage.DataSet,
                DataMember = "payments_accounts",
                Filter =
                    string.Format("id_account IN (0{0})",
                        idAccounts.Select(v => v.ToString()).Aggregate((acc, v) => acc + "," + v).Trim(','))
            };
            dataGridView.DataSource = _viewModel;
            date.DataPropertyName = "date";
            charging_date.DataPropertyName = "charging_date";
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
            _idAccountCurrent = idAccountCurrent;
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

        private void dataGridView_VisibleChanged(object sender, EventArgs e)
        {
            if (_idAccountCurrent == null)
            {
                return;
            }
            for (var i = 0; i < _viewModel.Count; i++)
            {
                if (((DataRowView)_viewModel[i])["id_account"] != DBNull.Value &&
                    (int)((DataRowView)_viewModel[i])["id_account"] == _idAccountCurrent.Value)
                {
                    dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    dataGridView.Rows[i].DefaultCellStyle.SelectionBackColor = Color.Green;
                }
                else
                {
                    dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dataGridView.Rows[i].DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                }
            }
        }
    }
}
