using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Viewport
{
    public class Viewport: System.Windows.Forms.TabPage, IMenuController
    {
        protected readonly IMenuCallback menuCallback;
        private bool selected_ = false;

        protected Viewport(IMenuCallback menuCallback)
        {
            this.menuCallback = menuCallback;
        }

        public virtual void Close()
        {
            menuCallback.SwitchToPreviousViewport();
            Dispose();
        }

        public virtual int GetRecordCount()
        {
            return 0;
        }

        public virtual Viewport Duplicate()
        { 
            return this;
        }

        public virtual bool CanDuplicate()
        {
            return false;
        }

        public virtual bool CanLoadData()
        {
            return false;
        }

        public virtual void LoadData()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MoveFirst()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MovePrev()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MoveNext()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MoveLast()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual bool CanMoveFirst()
        {
            return false;
        }

        public virtual bool CanMovePrev()
        {
            return false;
        }

        public virtual bool CanMoveNext()
        {
            return false;
        }

        public virtual bool CanMoveLast()
        {
            return false;
        }

        public virtual void SaveRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void CancelRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void CopyRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void InsertRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void DeleteRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void OpenDetails()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void DataRefresh()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void SearchRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowBuildings()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowPremises()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowRestrictions()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowOwnerships()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowFundHistory()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowSubPremises()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ClearSearch()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual bool CanSaveRecord()
        {
            return false;
        }

        public virtual bool CanCancelRecord()
        {
            return false;
        }

        public virtual bool CanCopyRecord()
        {
            return false;
        }

        public virtual bool CanInsertRecord()
        {
            return false;
        }

        public virtual bool CanDeleteRecord()
        {
            return false;
        }

        public virtual bool CanOpenDetails()
        {
            return false;
        }

        public virtual bool CanDataRefresh()
        {
            return false;
        }

        public virtual bool CanSearchRecord()
        {
            return false;
        }

        public virtual bool CanShowPremises()
        {
            return false;
        }

        public virtual bool CanShowSubPremises()
        {
            return false;
        }

        public virtual bool CanShowRestrictions()
        {
            return false;
        }

        public virtual bool CanShowOwnerships()
        {
            return false;
        }

        public virtual bool AllowHousingMenuTab()
        {
            return false;
        }

        public virtual bool AllowRelatedWorkMenuTab()
        {
            return false;
        }

        public virtual bool AllowSocialRecruitmentMenuTab()
        {
            return false;
        }

        public virtual bool SearchedRecords()
        {
            return false;
        }

        public virtual bool HasAssocSubPremises()
        {
            return false;
        }

        public virtual bool HasAssocBuildings()
        {
            return false;
        }

        public virtual bool HasAssocPremises()
        {
            return false;
        }

        public virtual bool HasAssocOwnerships()
        {
            return false;
        }

        public virtual bool HasAssocRestrictions()
        {
            return false;
        }

        public virtual bool HasFundHistory()
        {
            return false;
        }

        public virtual bool CanFilterSocialFund()
        {
            return false;
        }

        public virtual bool CanFilterCommercialFundFund()
        {
            return false;
        }

        public virtual bool CanFilterSpecialFund()
        {
            return false;
        }

        public virtual bool CanFilterOtherFunds()
        {
            return false;
        }

        public virtual void ForceClose()
        {
            Dispose();
        }

        public virtual bool ViewportDetached()
        {
            return false;
        }

        public bool Selected
        {
            get
            {
                return selected_;
            }
            set
            {
                selected_ = value;
            }
        }
    }
}
