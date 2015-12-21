using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    public sealed partial class MultiPaymentAccountsMaster : DockContent, IMultiMaster
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
            DataModel.GetInstance(DataModelType.PremisesDataModel).Select();
            _paymentAccount.DataSource = DataModel.DataSet;
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
                case "crn":
                    e.Value = row["crn"];
                    break;
                case "raw_address":
                    e.Value = row["raw_address"];
                    break;
                case "parsed_address":
                    e.Value = row["parsed_address"];
                    break;
                case "account":
                    e.Value = row["account"];
                    break;
                case "tenant":
                    e.Value = row["tenant"];
                    break;
                case "total_area":
                    e.Value = row["total_area"];
                    break;
                case "living_area":
                    e.Value = row["living_area"];
                    break;
                case "prescribed":
                    e.Value = row["prescribed"];
                    break;
                case "date":
                    e.Value = ((DateTime)row["date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "balance_input":
                    e.Value = row["balance_input"];
                    break;
                case "balance_tenancy":
                    e.Value = row["balance_tenancy"];
                    break;
                case "balance_dgi":
                    e.Value = row["balance_dgi"];
                    break;
                case "charging_tenancy":
                    e.Value = row["charging_tenancy"];
                    break;
                case "charging_dgi":
                    e.Value = row["charging_dgi"];
                    break;
                case "charging_total":
                    e.Value = row["charging_total"];
                    break;
                case "recalc_tenancy":
                    e.Value = row["recalc_tenancy"];
                    break;
                case "recalc_dgi":
                    e.Value = row["recalc_dgi"];
                    break;
                case "payment_tenancy":
                    e.Value = row["payment_tenancy"];
                    break;
                case "payment_dgi":
                    e.Value = row["payment_dgi"];
                    break;
                case "transfer_balance":
                    e.Value = row["transfer_balance"];
                    break;
                case "balance_output_total":
                    e.Value = row["balance_output_total"];
                    break;
                case "balance_output_tenancy":
                    e.Value = row["balance_output_tenancy"];
                    break;
                case "balance_output_dgi":
                    e.Value = row["balance_output_dgi"];
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
            var idAccount = -1;
            var paymentsAccountsViewport = viewport as PaymentsAccountsViewport;
            if (paymentsAccountsViewport != null)
                idAccount = paymentsAccountsViewport.GetCurrentId();
            if (idAccount == -1) return;
            _paymentAccount.Filter = string.Format("({0}) OR (id_account = {1})", _paymentAccount.Filter, idAccount);
            dataGridView.RowCount = _paymentAccount.Count;
        }

        private void toolStripButtonAccountsByFilter_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            var filter = "";
            var premisesAccountViewport = viewport as PaymentsAccountsViewport;
            if (premisesAccountViewport != null)
                filter = premisesAccountViewport.GetFilter();
            if (filter == "") return;
            _paymentAccount.Filter = string.Format("({0}) OR ({1})", _paymentAccount.Filter, filter);
            dataGridView.RowCount = _paymentAccount.Count;
        }

        private void toolStripButtonCreateClaims_Click(object sender, EventArgs e)
        {
            if (_paymentAccount.Count == 0)
                return;
            if (DataModel.GetInstance(DataModelType.ClaimsDataModel).EditingNewRecord)
            {
                MessageBox.Show(@"Невозможно провести массовую операцию вставки претензионно-исковых работ пока форма исковых работ находится в состоянии добавления новой записи. Отмените добавление новой записи или сохраните ее.",
                    @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            if (DataModel.GetInstance(DataModelType.ClaimStatesDataModel).EditingNewRecord)
            {
                MessageBox.Show(@"Невозможно провести массовую операцию вставки претензионно-исковых работ пока форма состояний исковых работ находится в состоянии добавления новой записи. Отмените добавление новой записи или сохраните ее.",
                    @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            var claimsDataModel = DataModel.GetInstance(DataModelType.ClaimsDataModel);
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
                        DataModelHelper.ClaimStateTypeIdsByPrevStateType(lastStateTypeRow.id_state_type).Any()
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
                claimRow["id_claim"] = ViewportHelper.ValueOrDBNull(claim.IdClaim);
                claimRow["id_account"] = ViewportHelper.ValueOrDBNull(claim.IdAccount);
                claimRow["amount_tenancy"] = ViewportHelper.ValueOrDBNull(claim.AmountTenancy);
                claimRow["amount_dgi"] = ViewportHelper.ValueOrDBNull(claim.AmountDgi);
                claimRow["at_date"] = ViewportHelper.ValueOrDBNull(claim.AtDate);
                claimRow.EndEdit();
                // Add first state automaticaly
                var firstStateTypes = DataModelHelper.ClaimStartStateTypeIds().ToList();
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
                if (claimsStateRow == null) continue;
                claimsStateRow.BeginEdit();
                claimsStateRow["id_state"] = ViewportHelper.ValueOrDBNull(claimState.IdState);
                claimsStateRow["id_claim"] = ViewportHelper.ValueOrDBNull(claimState.IdClaim);
                claimsStateRow["id_state_type"] = ViewportHelper.ValueOrDBNull(claimState.IdStateType);
                claimsStateRow["transfer_to_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.TransferToLegalDepartmentWho);
                claimsStateRow["accepted_by_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.AcceptedByLegalDepartmentWho);
                claimsStateRow["date_start_state"] = ViewportHelper.ValueOrDBNull(claimState.DateStartState);
                claimsStateRow.EndEdit();
                Application.DoEvents();
            }
            toolStripProgressBarMultiOperations.Visible = false;
            MessageBox.Show(@"Претензионно-исковые работы успешно созданы", @"Сообщение", MessageBoxButtons.OK,
                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
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
    }
}
