﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Reporting;
using Registry.Viewport.ModalEditors;
using Security;
using Settings;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport.MultiMasters
{
    internal sealed partial class MultiPaymentAccountsMaster : DockContent, IMultiMaster
    {
        private readonly BindingSource _paymentAccount = new BindingSource();
        private readonly IMenuCallback _menuCallback;

        public MultiPaymentAccountsMaster(IMenuCallback menuCallback)
        {
            InitializeComponent();
            DockAreas = DockAreas.DockLeft | DockAreas.DockRight
                        | DockAreas.DockTop
                        | DockAreas.DockBottom;
            _menuCallback = menuCallback;
            DataModel.GetInstance<PaymentsAccountsDataModel>().Select();
            _paymentAccount.DataSource = DataStorage.DataSet;
            _paymentAccount.DataMember = "payments_accounts";
            _paymentAccount.Filter = "0 = 1";
            dataGridView.RowCount = 0;
        }

        void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                _paymentAccount.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + (way == SortOrder.Ascending ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                _paymentAccount.Position = dataGridView.SelectedRows[0].Index;
            else
                _paymentAccount.Position = -1;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_paymentAccount.Count <= e.RowIndex) return;
            var row = (DataRowView)_paymentAccount[e.RowIndex];
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "date":
                    e.Value = ((DateTime)row["date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                default:
                    e.Value = row[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
            }
        }

        private void toolStripButtonAccountDeleteAll_Click(object sender, EventArgs e)
        {
            _paymentAccount.Filter = "0 = 1";
            dataGridView.RowCount = _paymentAccount.Count;
        }

        private void toolStripButtonAccountDelete_Click(object sender, EventArgs e)
        {
            if (_paymentAccount.Position < 0) return;
            if (((DataRowView) _paymentAccount[_paymentAccount.Position])["id_account"] == DBNull.Value) return;
            var idAccount = (int)((DataRowView)_paymentAccount[_paymentAccount.Position])["id_account"];
            _paymentAccount.Filter = string.Format("({0}) AND (id_account <> {1})", _paymentAccount.Filter, idAccount);
            dataGridView.RowCount = _paymentAccount.Count;
            dataGridView.Refresh();
        }

        private void toolStripButtonAccountCurrent_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            IEnumerable<int> idAccount = new List<int>();
            var paymentsAccountsViewport = viewport as PaymentsAccountsViewport;
            if (paymentsAccountsViewport != null)
                idAccount = paymentsAccountsViewport.GetCurrentIds();
            if (!idAccount.Any()) return;
            _paymentAccount.Filter = string.Format("({0}) OR (id_account IN ({1}))", _paymentAccount.Filter, 
                idAccount.Select(x => x.ToString()).Aggregate((x,y) => x +","+ y));
            dataGridView.RowCount = _paymentAccount.Count;
        }

        private void toolStripButtonAccountsByFilter_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            var filter = "";
            var paymentsAccountViewport = viewport as PaymentsAccountsViewport;
            if (paymentsAccountViewport != null)
                filter = paymentsAccountViewport.GetFilter();
            if (filter == "") filter = "1=1";
            _paymentAccount.Filter = string.Format("({0}) OR ({1})", _paymentAccount.Filter, filter);
            dataGridView.RowCount = _paymentAccount.Count;
        }

        private void toolStripButtonCreateClaims_Click(object sender, EventArgs e)
        {
            if (_paymentAccount.Count == 0)
                return;
            if (DataModel.GetInstance<EntityDataModel<Claim>>().EditingNewRecord)
            {
                MessageBox.Show(@"Невозможно провести массовую операцию вставки претензионно-исковых работ пока форма исковых работ находится в состоянии добавления новой записи. Отмените добавление новой записи или сохраните ее.",
                    @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            if (DataModel.GetInstance<EntityDataModel<ClaimState>>().EditingNewRecord)
            {
                MessageBox.Show(@"Невозможно провести массовую операцию вставки претензионно-исковых работ пока форма состояний исковых работ находится в состоянии добавления новой записи. Отмените добавление новой записи или сохраните ее.",
                    @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            List<int> excludeIdAccounts;
            if (!ValidateDuplicates(out excludeIdAccounts)) return;
            var atDateForm = new MultiPaymentAccountsAtDateForm();
            if (atDateForm.ShowDialog() != DialogResult.OK)
                return;

            foreach (var viewport in _menuCallback.GetAllViewports())
            {
                var paymentAccountViewport = viewport as PaymentsAccountsViewport;
                if (paymentAccountViewport != null)
                {
                    paymentAccountViewport.CanUpdateNotCompletedClaims = false;
                }
            }

            toolStripProgressBarMultiOperations.Visible = true;
            toolStripProgressBarMultiOperations.Value = 0;
            toolStripProgressBarMultiOperations.Maximum = _paymentAccount.Count - 1;

            for (var i = 0; i < _paymentAccount.Count; i++)
            {
                toolStripProgressBarMultiOperations.Value = i;
                var row = (DataRowView) _paymentAccount[i];
                if (row["id_account"] == DBNull.Value || excludeIdAccounts.Contains((int)row["id_account"])) continue;
                var claim = new Claim
                {
                    IdAccount = (int) row["id_account"],
                    AmountTenancy = ViewportHelper.ValueOrNull<decimal>(row, "balance_output_tenancy"),
                    AmountDgi = ViewportHelper.ValueOrNull<decimal>(row, "balance_output_dgi"),
                    AmountPadun = ViewportHelper.ValueOrNull<decimal>(row, "balance_output_padun"),
                    AmountPkk = ViewportHelper.ValueOrNull<decimal>(row, "balance_output_pkk"),
                    AmountPenalties = ViewportHelper.ValueOrNull<decimal>(row, "balance_output_penalties"),
                    AtDate = atDateForm.DateAt
                };
                var claimsDataModel = DataModel.GetInstance<EntityDataModel<Claim>>();
                var id = claimsDataModel.Insert(claim);
                if (id == -1) break;
                claim.IdClaim = id;
                var claimBindingSource = new BindingSource
                {
                    DataSource = claimsDataModel.Select()
                };
                var claimRow = (DataRowView)claimBindingSource.AddNew();
                if (claimRow == null)
                {
                    MessageBox.Show(@"Произошла неизвестная ошибка во время создания исковых работ", 
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                }
                claimRow.BeginEdit();
                claimRow["id_claim"] = ViewportHelper.ValueOrDbNull(claim.IdClaim);
                claimRow["id_account"] = ViewportHelper.ValueOrDbNull(claim.IdAccount);
                claimRow["amount_tenancy"] = ViewportHelper.ValueOrDbNull(claim.AmountTenancy);
                claimRow["amount_dgi"] = ViewportHelper.ValueOrDbNull(claim.AmountDgi);
                claimRow["amount_padun"] = ViewportHelper.ValueOrDbNull(claim.AmountPadun);
                claimRow["amount_pkk"] = ViewportHelper.ValueOrDbNull(claim.AmountPkk);
                claimRow["amount_penalties"] = ViewportHelper.ValueOrDbNull(claim.AmountPenalties);
                claimRow["at_date"] = ViewportHelper.ValueOrDbNull(claim.AtDate);
                claimRow.EndEdit();
                // Add first state automaticaly
                var firstStateTypes = ClaimsService.ClaimStartStateTypeIds().ToList();
                if (!firstStateTypes.Any()) continue;
                var firstStateType = firstStateTypes.First();
                var claimStatesDataModel = DataModel.GetInstance<EntityDataModel<ClaimState>>();
                var claimStatesBindingSource = new BindingSource
                {
                    DataSource = claimStatesDataModel.Select()
                };
                var claimState = new ClaimState
                {
                    IdClaim = claim.IdClaim,
                    IdStateType = firstStateType,
                    BksRequester = UserDomain.Current.DisplayName,
                    TransferToLegalDepartmentWho = UserDomain.Current.DisplayName,
                    AcceptedByLegalDepartmentWho = UserDomain.Current.DisplayName,
                    DateStartState = DateTime.Now.Date
                };
                var idState = claimStatesDataModel.Insert(claimState);
                if (idState == -1)
                {
                    MessageBox.Show(@"Произошла неизвестная ошибка во время создания стадии исковых работ",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                }
                claimState.IdState = idState;
                var claimsStateRow = (DataRowView)claimStatesBindingSource.AddNew();
                if (claimsStateRow == null) break;
                claimsStateRow.BeginEdit();
                claimsStateRow["id_state"] = ViewportHelper.ValueOrDbNull(claimState.IdState);
                claimsStateRow["id_claim"] = ViewportHelper.ValueOrDbNull(claimState.IdClaim);
                claimsStateRow["id_state_type"] = ViewportHelper.ValueOrDbNull(claimState.IdStateType);
                claimsStateRow["bks_requester"] = ViewportHelper.ValueOrDbNull(claimState.BksRequester);
                claimsStateRow["transfer_to_legal_department_who"] = ViewportHelper.ValueOrDbNull(claimState.TransferToLegalDepartmentWho);
                claimsStateRow["accepted_by_legal_department_who"] = ViewportHelper.ValueOrDbNull(claimState.AcceptedByLegalDepartmentWho);
                claimsStateRow["date_start_state"] = ViewportHelper.ValueOrDbNull(claimState.DateStartState);
                claimsStateRow.EndEdit();
                Application.DoEvents();
            }
            toolStripProgressBarMultiOperations.Visible = false;

            foreach (var viewport in _menuCallback.GetAllViewports())
            {
                var paymentAccountViewport = viewport as PaymentsAccountsViewport;
                if (paymentAccountViewport != null)
                {
                    paymentAccountViewport.CanUpdateNotCompletedClaims = true;
                    paymentAccountViewport.UpdateNotCompletedClaims();
                }
            }

            if (toolStripProgressBarMultiOperations.Value == toolStripProgressBarMultiOperations.Maximum)
                MessageBox.Show(@"Претензионно-исковые работы успешно созданы", @"Сообщение", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            else
                MessageBox.Show(@"Во время добавления претензионно-исковых работ возникли ошибки", @"Внимание", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }

        private bool ValidateDuplicates(out List<int> excludeIdAccounts)
        {
            var notCompletedClaims = ClaimsService.NotCompletedClaimsPaymentAccountsInfo().ToList();
            excludeIdAccounts = new List<int>();
            // Check duplicates
            for (var i = 0; i < _paymentAccount.Count; i++)
            {
                var row = (DataRowView)_paymentAccount[i];
                if (row["id_account"] == DBNull.Value) continue;
                var accountDuplicate = notCompletedClaims.LastOrDefault(r => 
                    row["account"] != DBNull.Value &&  r.Account == (string)row["account"]);
                if (accountDuplicate != null)
                {
                    var dialogResult = MessageBox.Show(string.Format(
                        @"По лицевому счету {0} уже заведена незавершенная претензионно-исковая работа. " +
                        @"Наниматель: {1}. Текущее состояние: {2}. Предьявляемый период: {3}-{4}. Сумма к взысканию: {5}. Все равно продолжить?",
                        row["account"], accountDuplicate.Tenant, accountDuplicate.StateType,
                        accountDuplicate.StartDeptPeriod.HasValue
                            ? accountDuplicate.StartDeptPeriod.Value.ToString("dd.MM.yyyy")
                            : "н/а",
                        accountDuplicate.EndDeptPeriod.HasValue
                            ? accountDuplicate.EndDeptPeriod.Value.ToString("dd.MM.yyyy")
                            : "н/а",
                        accountDuplicate.Amount),
                        @"Внимание", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);

                    switch (dialogResult)
                    {
                        case DialogResult.Cancel:
                            return false;
                        case DialogResult.No:
                            excludeIdAccounts.Add((int)row["id_account"]);
                            break;
                    }
                }
                if (accountDuplicate != null) continue;

                accountDuplicate = notCompletedClaims.LastOrDefault(
                    r => row["raw_address"] != DBNull.Value && 
                    r.RawAddress == (string)row["raw_address"]);
                if (accountDuplicate != null)
                {
                    var dialogResult = MessageBox.Show(string.Format(
                        @"По адресу БКС ""{0}"" уже заведена незавершенная претензионно-исковая работа. " +
                        @"Наниматель: {1}. Текущее состояние: {2}. Предьявляемый период: {3}-{4}. Сумма к взысканию: {5}. Все равно продолжить?",
                        row["raw_address"], accountDuplicate.Tenant, accountDuplicate.StateType,
                        accountDuplicate.StartDeptPeriod.HasValue
                            ? accountDuplicate.StartDeptPeriod.Value.ToString("dd.MM.yyyy")
                            : "н/а",
                        accountDuplicate.EndDeptPeriod.HasValue
                            ? accountDuplicate.EndDeptPeriod.Value.ToString("dd.MM.yyyy")
                            : "н/а",
                        accountDuplicate.Amount),
                        @"Внимание", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);
                    switch (dialogResult)
                    {
                        case DialogResult.Cancel:
                            return false;
                        case DialogResult.No:
                            excludeIdAccounts.Add((int)row["id_account"]);
                            break;
                    }
                }
                if (accountDuplicate != null) continue;
                accountDuplicate = notCompletedClaims.LastOrDefault(
                    r => row["parsed_address"] != DBNull.Value && 
                    r.ParsedAddress == (string)row["parsed_address"]);
                if (accountDuplicate != null)
                {
                    var dialogResult = MessageBox.Show(string.Format(
                        @"По адресу ЖФ ""{0}"" уже заведена незавершенная претензионно-исковая работа. " +
                        @"Наниматель: {1}. Текущее состояние: {2}. Предьявляемый период: {3}-{4}. Сумма к взысканию: {5}. Все равно продолжить?",
                        row["parsed_address"], accountDuplicate.Tenant, accountDuplicate.StateType,
                        accountDuplicate.StartDeptPeriod.HasValue
                            ? accountDuplicate.StartDeptPeriod.Value.ToString("dd.MM.yyyy")
                            : "н/а",
                        accountDuplicate.EndDeptPeriod.HasValue
                            ? accountDuplicate.EndDeptPeriod.Value.ToString("dd.MM.yyyy")
                            : "н/а",
                        accountDuplicate.Amount),
                        @"Внимание", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);
                    switch (dialogResult)
                    {
                        case DialogResult.Cancel:
                            return false;
                        case DialogResult.No:
                            excludeIdAccounts.Add((int)row["id_account"]);
                            break;
                    }
                }
            }
            return true;
        }

        public void UpdateToolbar()
        {
            var viewport = _menuCallback.GetCurrentViewport();
            toolStripButtonAccountCurrent.Visible = false;
            toolStripButtonAccountsByFilter.Visible = false;
            if (!(viewport is PaymentsAccountsViewport)) return;
            toolStripButtonAccountCurrent.Visible = true;
            toolStripButtonAccountsByFilter.Visible = true;
            toolStripButtonCreateClaims.Enabled = AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        private void toolStripButtonRequestToBks_Click(object sender, EventArgs e)
        {
            if (_paymentAccount.Count == 0) return;
            var arguments = new Dictionary<string, string>();
            var filter = "";
            for (var i = 0; i < _paymentAccount.Count; i++)
            {
                var row = (DataRowView)_paymentAccount[i];
                if (row["id_account"] != DBNull.Value)
                    filter += row["id_account"] + ",";
            }
            filter = filter.TrimEnd(',');
            arguments.Add("filter", filter);
            _menuCallback.RunReport(ReporterType.RequestToBksReporter, arguments);
        }

        private void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            RowCountChanged();
        }

        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            RowCountChanged();
        }

        private void RowCountChanged()
        {
            toolStripLabelRowCount.Text = string.Format("Всего записей в мастере: {0}", dataGridView.RowCount);
        }

        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
            var reporter = ReporterFactory.CreateReporter(ReporterType.ExportReporter);
            reporter.Run(ExportReportArguments());
        }

        private Dictionary<string, string> ExportReportArguments()
        {
            var columnHeaders = dataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnHeader\":\"" + column.HeaderText + "\"}");
            var columnPatterns = dataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnPattern\":\"$column" + column.DisplayIndex + "$\"}");
            var filter = _paymentAccount.Filter ?? "";
            var arguments = new Dictionary<string, string>
            {
                {"type", "5"},
                {"filter", filter.Trim() == "" ? "(1=1)" : filter },
                {"columnHeaders", "["+columnHeaders+"]"},
                {"columnPatterns", "["+columnPatterns+"]"},
                {"orderColumn", string.IsNullOrEmpty(_paymentAccount.Sort) ? "id_account" : _paymentAccount.Sort}
            };
            return arguments;
        }
    }
}
