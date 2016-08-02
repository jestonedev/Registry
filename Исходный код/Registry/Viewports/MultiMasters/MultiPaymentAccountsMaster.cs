using System;
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
using Security;
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
            var claimsDataModel = DataModel.GetInstance<EntityDataModel<Claim>>();
            var claimStatesDataModel = DataModel.GetInstance<EntityDataModel<ClaimState>>();
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
                select new
                {
                    id_claim = stateRow.Field<int>("id_claim"),
                    id_state_type = stateRow.Field<int>("id_state_type")
                };
            // Check duplicates
            for (var i = 0; i < _paymentAccount.Count; i++)
            {
                var row = (DataRowView)_paymentAccount[i];
                if (row["id_account"] == DBNull.Value) continue;
                var isDuplicate = (from lastStateTypeRow in lastStateTypes
                    join claimsRow in claimsDataModel.FilterDeletedRows()
                        on lastStateTypeRow.id_claim equals claimsRow.Field<int>("id_claim")
                    where claimsRow.Field<int?>("id_account") == (int?) row["id_account"] &&
                        ClaimsService.ClaimStateTypeIdsByPrevStateType(lastStateTypeRow.id_state_type).Any()
                    select claimsRow).Any();
                if (isDuplicate && MessageBox.Show(string.Format(
                    @"По лицевому счету {0} уже заведена незавершенная претензионно-исковая работа. Все равно продолжить?",
                    row["account"]), @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) !=
                    DialogResult.Yes)
                    return;
            }
            var atDateForm = new MultiPaymentAccountsAtDateForm();
            if (atDateForm.ShowDialog() != DialogResult.OK)
                return;
            toolStripProgressBarMultiOperations.Visible = true;
            toolStripProgressBarMultiOperations.Value = 0;
            toolStripProgressBarMultiOperations.Maximum = _paymentAccount.Count - 1;
            for (var i = 0; i < _paymentAccount.Count; i++)
            {
                toolStripProgressBarMultiOperations.Value = i;
                var row = (DataRowView) _paymentAccount[i];
                if (row["id_account"] == DBNull.Value) continue;
                var claim = new Claim
                {
                    IdAccount = (int) row["id_account"],
                    AmountTenancy = ViewportHelper.ValueOrNull<decimal>(row, "balance_output_tenancy"),
                    AmountDgi = ViewportHelper.ValueOrNull<decimal>(row, "balance_output_dgi"),
                    AmountPenalties = ViewportHelper.ValueOrNull<decimal>(row, "balance_output_penalties"),
                    AtDate = atDateForm.DateAt
                };
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
                claimRow["amount_penalties"] = ViewportHelper.ValueOrDbNull(claim.AmountPenalties);
                claimRow["at_date"] = ViewportHelper.ValueOrDbNull(claim.AtDate);
                claimRow.EndEdit();
                // Add first state automaticaly
                var firstStateTypes = ClaimsService.ClaimStartStateTypeIds().ToList();
                if (!firstStateTypes.Any()) continue;
                var firstStateType = firstStateTypes.First();
                var claimStatesBindingSource = new BindingSource
                {
                    DataSource = claimStatesDataModel.Select()
                };
                var claimState = new ClaimState
                {
                    IdClaim = claim.IdClaim,
                    IdStateType = firstStateType,
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
                claimsStateRow["transfer_to_legal_department_who"] = ViewportHelper.ValueOrDbNull(claimState.TransferToLegalDepartmentWho);
                claimsStateRow["accepted_by_legal_department_who"] = ViewportHelper.ValueOrDbNull(claimState.AcceptedByLegalDepartmentWho);
                claimsStateRow["date_start_state"] = ViewportHelper.ValueOrDbNull(claimState.DateStartState);
                claimsStateRow.EndEdit();
                Application.DoEvents();
            }
            toolStripProgressBarMultiOperations.Visible = false;
            if (toolStripProgressBarMultiOperations.Value == toolStripProgressBarMultiOperations.Maximum)
                MessageBox.Show(@"Претензионно-исковые работы успешно созданы", @"Сообщение", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            else
                MessageBox.Show(@"Во время добавления претензионно-исковых работ возникли ошибки", @"Внимание", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
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
            // select all claims with stage 1 and with existed next stage
            var claimStatesDataModel = DataModel.GetInstance<EntityDataModel<ClaimState>>();
            for (var i = 0; i < _paymentAccount.Count; i++)
            {
                var row = ((DataRowView)_paymentAccount[i]);

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
                                      select new
                                      {
                                          id_claim = stateRow.Field<int>("id_claim"),
                                          id_state_type = stateRow.Field<int>("id_state_type")
                                      };
                var notCompletedClaimWithFirstState =
                    from claimRow in DataModel.GetInstance<EntityDataModel<Claim>>().FilterDeletedRows()
                    join claimStateRow in claimStatesDataModel.FilterDeletedRows()
                        on claimRow.Field<int?>("id_claim") equals claimStateRow.Field<int?>("id_claim")
                    join lstRow in lastStateTypes.Where(x => ClaimsService.ClaimStateTypeIdsByPrevStateType(x.id_state_type).Any())
                        on claimRow.Field<int?>("id_claim") equals lstRow.id_claim into j
                    from jRow in j.DefaultIfEmpty()
                    where jRow != null &&
                        claimStateRow.Field<int?>("id_state_type") == 1 &&
                        claimRow.Field<int?>("id_account") == (int?) row["id_account"]
                    select new {claimRow, jRow};
                if (notCompletedClaimWithFirstState.Any()) continue;
                MessageBox.Show(
                    string.Format(
                        "Не удалось найти подходящую исковую работу по лицевому счету №{0} для формирования запроса в БКС. Возможно по данному лицевому счету отсутствуют незавершенные исковые работы",
                        row["account"]), @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            var reporter = ReporterFactory.CreateReporter(ReporterType.RequestToBksReporter);
            var arguments = new Dictionary<string, string>();
            var filter = "";
            for (var i = 0; i < _paymentAccount.Count; i++)
            {
                var row = ((DataRowView)_paymentAccount[i]);
                if (row["id_account"] != DBNull.Value)
                    filter += row["id_account"] + ",";
            }
            filter = filter.TrimEnd(',');
            arguments.Add("filter", filter);
            reporter.Run(arguments);
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
    }
}
