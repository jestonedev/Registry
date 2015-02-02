using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using WeifenLuo.WinFormsUI.Docking;
using Registry.Reporting;
using Registry.SearchForms;

namespace Registry.Viewport
{
    public class Viewport: DockContent, IMenuController
    {
        private IMenuCallback menuCallback;

        protected IMenuCallback MenuCallback { get { return menuCallback; } set { menuCallback = value; } }
        private bool selected_ = false;

        
        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        protected Viewport(): this(null)
        {
        }

        protected Viewport(IMenuCallback menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.MenuCallback = menuCallback;
        }

        public new virtual void Close()
        {
            MenuCallback.SwitchToPreviousViewport();
            base.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            MenuCallback.SwitchToPreviousViewport();
            base.OnClosing(e);
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

        public virtual void SearchRecord(SearchFormType searchFormType)
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

        public virtual void ShowTenancyPersons()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowTenancyReasons()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowTenancyAgreements()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowTenancyBuildings()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowTenancyPremises()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowClaims()
        {
           throw new ViewportException("Не реализовано");
        }

        public virtual void ShowClaimStates()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowTenancies()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowResettlePersons()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowResettleFromBuildings()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowResettleFromPremises()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowResettleToBuildings()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowResettleToPremises()
        {
            throw new ViewportException("Не реализовано");
        }

        protected virtual Viewport ShowAssocViewport(IMenuCallback menuCallback, ViewportType viewportType, 
            string staticFilter, DataRow parentRow, ParentTypeEnum parentType)
        {
            if (menuCallback == null)
                throw new ViewportException("Не заданна ссылка на интерфейс menuCallback");
            Viewport viewport = ViewportFactory.CreateViewport(menuCallback, viewportType);
            viewport.StaticFilter = staticFilter;
            viewport.ParentRow = parentRow;
            viewport.ParentType = parentType;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            return viewport;
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

        public virtual bool CanSearchRecord()
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

        public virtual bool HasAssocFundHistory()
        {
            return false;
        }

        public virtual bool HasAssocTenancyPersons()
        {
            return false;
        }

        public virtual bool HasAssocTenancyReasons()
        {
            return false;
        }

        public virtual bool HasAssocTenancyAgreements()
        {
            return false;
        }

        public virtual bool HasAssocTenancyObjects()
        {
            return false;
        }

        public virtual bool HasAssocClaims()
        {
            return false;
        }

        public virtual bool HasAssocClaimStates()
        {
            return false;
        }

        public virtual bool HasAssocResettlePersons()
        {
            return false;
        }

        public virtual bool HasAssocResettleFromObjects()
        {
            return false;
        }

        public virtual bool HasAssocResettleToObjects()
        {
            return false;
        }

        public virtual bool HasAssocTenancies()
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
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
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

        public virtual bool HasTenancyContract17xReport()
        {
            return false;
        }

        public virtual bool HasTenancyContractReport()
        {
            return false;
        }

        public virtual bool HasTenancyActReport()
        {
            return false;
        }

        public virtual bool HasTenancyAgreementReport()
        {
            return false;
        }

        public virtual bool HasTenancyOrderReport()
        {
            return false;
        }

        public virtual bool HasRegistryExcerptSubPremisesReport()
        {
            return false;
        }

        public virtual bool HasRegistryExcerptSubPremiseReport()
        {
            return false;
        }

        public virtual bool HasRegistryExcerptPremiseReport()
        {
            return false;
        }

        public virtual void TenancyContract17xReportGenerate(TenancyContractTypes tenancyContractType)
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void TenancyContractReportGenerate()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void TenancyActReportGenerate()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void TenancyAgreementReportGenerate()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void TenancyOrderReportGenerate()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void RegistryExcerptPremiseReportGenerate()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void RegistryExcerptSubPremiseReportGenerate()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void RegistryExcerptSubPremisesReportGenerate()
        {
            throw new ViewportException("Не реализовано");
        }
    }
}
