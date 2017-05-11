using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal class ClaimCourtOrdersPresenter : Presenter
    {
        public ClaimCourtOrdersPresenter()
            : base(new ClaimCourtOrdersViewModel(), null, null)
        {
            
        }

        internal bool InsertRecord(ClaimCourtOrder claimCourtOrder)
        {
            var id = ViewModel["general"].Model.Insert(claimCourtOrder);
            if (id == -1)
            {
                return false;
            }
            claimCourtOrder.IdOrder = id;
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<ClaimCourtOrder>.FillRow(claimCourtOrder, row);
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        internal bool UpdateRecord(ClaimCourtOrder claimCourtOrder)
        {
            if (claimCourtOrder.IdOrder == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить судебный приказ без внутренного номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(claimCourtOrder) == -1)
                return false;
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<ClaimCourtOrder>.FillRow(claimCourtOrder, row);
            return true;
        }

        internal bool InsertClaimPersonRecord(ClaimPerson claimPerson)
        {
            var id = ViewModel["claim_persons"].Model.Insert(claimPerson);
            if (id == -1)
            {
                return false;
            }
            claimPerson.IdPerson = id;
            var row = (DataRowView)ViewModel["claim_persons"].BindingSource.AddNew();
            if (row == null) return false;
            EntityConverter<ClaimPerson>.FillRow(claimPerson, row);
            ViewModel["claim_persons"].Model.EditingNewRecord = false;
            return true;
        }
    }
}
