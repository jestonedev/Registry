using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;
using Settings;

namespace Registry.Viewport.Presenters
{
    internal class ClaimListPresenter: Presenter
    {
        public ClaimListPresenter()
            : base(new ClaimListViewModel(), new SimpleSearchClaimsForm(), new ExtendedSearchClaimsForm())
        {
            
        }

        public bool DeleteRecord()
        {
            var row = ViewModel["general"].CurrentRow;
            var columnName = ViewModel["general"].PrimaryKeyFirst;
            if (ViewModel["general"].Model.Delete((int)row[columnName]) == -1)
                return false;
            row.Delete();
            return true;
        }

        public string AccountById(int? idAccount)
        {
            if (idAccount == null) return null;
            return (from row in ViewModel["payments_accounts"].Model.FilterDeletedRows()
                    where row.Field<int>("id_account") == idAccount.Value
                    select row.Field<string>("account")).FirstOrDefault();
        }

        public List<int> AccountIdsByAccount(string account)
        {
            return (from row in ViewModel["payments_accounts"].Model.FilterDeletedRows()
                where row.Field<string>("account") == account
                select row.Field<int>("id_account")).ToList();
        }

        public List<int> AccountIdsByRawAddress(string rawAddress)
        {
            return (from row in ViewModel["payments_accounts"].Model.FilterDeletedRows()
                    where row.Field<string>("raw_address") == rawAddress
                    select row.Field<int>("id_account")).ToList();
        }

        public List<int> AccountIdsByParsedAddress(string parsedAddress)
        {
            return (from row in ViewModel["payments_accounts"].Model.FilterDeletedRows()
                    where row.Field<string>("parsed_address") == parsedAddress
                    select row.Field<int>("id_account")).ToList();
        }

        public List<int> SameAccountIdsByAccountId(int? idAccount)
        {
            if (idAccount == null)
            {
                return new List<int>();
            }
            var account = (from row in ViewModel["payments_accounts"].Model.FilterDeletedRows()
                where row.Field<int>("id_account") == idAccount
                select row).FirstOrDefault();
            if (account == null)
            {
                return new List<int>();
            }
            var accountNumber = account["account"] != DBNull.Value ? (string)account["account"] : null;
            var accounts = AccountIdsByAccount(accountNumber);
            var rawAddress = account["raw_address"] != DBNull.Value ? (string)account["raw_address"] : null;
            accounts = accounts.Concat(AccountIdsByRawAddress(rawAddress)).ToList();
            var parsedAddress = account["parsed_address"] != DBNull.Value ? (string)account["parsed_address"] : null; 
            accounts = accounts.Concat(AccountIdsByParsedAddress(parsedAddress)).ToList();
            return accounts.Concat(new List<int> { idAccount.Value }).Distinct().ToList();
        }

        public List<DataRow> AccountRowsById(int? idAccount)
        {
            return (from paymentAccountRow in ViewModel["payments_accounts"].Model.FilterDeletedRows()
                    where paymentAccountRow.Field<int?>("id_account") == idAccount
                    select paymentAccountRow).ToList();
        }

        internal bool UpdateRecord(Claim claim)
        {
            if (claim.IdClaim == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить запись о претензионно-исковой работе без внутреннего номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(claim) == -1)
                return false;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, claim.IdClaim);
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<Claim>.FillRow(claim, row);
            return true;
        }

        internal bool InsertRecord(Claim claim)
        {
            var idClaim = ViewModel["general"].Model.Insert(claim);
            if (idClaim == -1)
            {
                return false;
            }
            claim.IdClaim = idClaim;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, claim.IdClaim);
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<Claim>.FillRow(claim, row);
            InsertFirstClaimState(claim.IdClaim);
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        private void InsertFirstClaimState(int? idClaim)
        {
            var claimStatesDataModel = ViewModel["claim_states"].Model;
            if (claimStatesDataModel.EditingNewRecord)
            {
                MessageBox.Show(@"Не удалось автоматически вставить первый этап претензионно-исковой работы, т.к. форма состояний исковых работ находится в состоянии добавления новой записи.",
                    @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            var firstStateTypes = ClaimsService.ClaimStartStateTypeIds().ToList();
            if (!firstStateTypes.Any()) return;
            var firstStateType = firstStateTypes.First();
            var claimStatesBindingSource = new BindingSource
            {
                DataSource = claimStatesDataModel.Select()
            };
            var claimState = new ClaimState
            {
                IdClaim = idClaim,
                IdStateType = firstStateType,
                BksRequester = UserDomain.Current.DisplayName,
                TransferToLegalDepartmentWho = UserDomain.Current.DisplayName,
                AcceptedByLegalDepartmentWho = UserDomain.Current.DisplayName,
                DateStartState = DateTime.Now.Date
            };
            var idState = claimStatesDataModel.Insert(claimState);
            if (idState == -1) return;
            claimState.IdState = idState;
            var claimsStateRow = (DataRowView)claimStatesBindingSource.AddNew();
            if (claimsStateRow != null)
            {
                EntityConverter<ClaimState>.FillRow(claimState, claimsStateRow);
            }
        }

        private static void RebuildFilterAfterSave(IBindingListView bindingSource, int? idClaim)
        {
            var filter = "";
            if (!string.IsNullOrEmpty(bindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            filter += string.Format(CultureInfo.CurrentCulture, "(id_claim = {0})", idClaim);
            bindingSource.Filter += filter;
        }

        public void UpdateBalance(Claim claim, int? idAccountFrom)
        {
            if (idAccountFrom == null) return;
            if (claim.EndDeptPeriod == null) return;
            var balanceInfoTable = PaymentService.GetBalanceInfoOnDate(
                new List<int> { idAccountFrom.Value }, claim.EndDeptPeriod.Value.Year,
                claim.EndDeptPeriod.Value.Month);
            var balanceInfoList = (from row in balanceInfoTable.Select()
                                   where row.Field<int>("id_account") == idAccountFrom.Value
                                   select new
                                   {
                                       BalanceOutputTenancy = row.Field<decimal>("balance_output_tenancy"),
                                       BalanceOutputDgi = row.Field<decimal>("balance_output_dgi"),
                                       BalanceOutputPenalties = row.Field<decimal>("balance_output_penalties")
                                   }).ToList();
            if (!balanceInfoList.Any())
            {
                MessageBox.Show(@"На конец указанного периода отсутствуют данные по задолженности", @"Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
            else
            {
                var balanceInfo = balanceInfoList.First();
                claim.AmountTenancy = balanceInfo.BalanceOutputTenancy;
                claim.AmountDgi = balanceInfo.BalanceOutputDgi;
                claim.AmountPenalties = balanceInfo.BalanceOutputPenalties;
            }
        }
    }
}
