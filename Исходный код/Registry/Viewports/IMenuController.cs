using Registry.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.SearchForms;

namespace Registry.Viewport
{
    public interface IMenuController
    {
        Viewport Duplicate();
        void LoadData();
        void MoveFirst();
        void MovePrev();
        void MoveNext();
        void MoveLast();
        void SaveRecord();
        void CancelRecord();
        void CopyRecord();
        void InsertRecord();
        void DeleteRecord();
        void OpenDetails();
        void SearchRecord(SearchFormType searchFormType);
        void ClearSearch();
        void ShowBuildings();
        void ShowPremises();
        void ShowSubPremises();
        void ShowRestrictions();
        void ShowOwnerships();
        void ShowFundHistory();
        void ShowTenancyPersons();
        void ShowTenancyReasons();
        void ShowTenancyAgreements();
        void ShowTenancyBuildings();
        void ShowTenancyPremises();
        void ShowClaims();
        void ShowClaimStates();
        void ShowTenancies();
        void ShowResettlePersons();
        void ShowResettleFromBuildings();
        void ShowResettleFromPremises();
        void ShowResettleToBuildings();
        void ShowResettleToPremises();
        void Close();
        void ForceClose();

        bool CanDuplicate();
        bool CanLoadData();
        bool CanMoveFirst();
        bool CanMovePrev();
        bool CanMoveNext();
        bool CanMoveLast();
        bool CanSaveRecord();
        bool CanCancelRecord();
        bool CanCopyRecord();
        bool CanInsertRecord();
        bool CanDeleteRecord();
        bool CanOpenDetails();
        bool CanSearchRecord();
        bool SearchedRecords();
        bool ViewportDetached();
        
        bool AllowHousingMenuTab();
        bool AllowRelatedWorkMenuTab();
        bool AllowSocialRecruitmentMenuTab();

        bool HasAssocBuildings();
        bool HasAssocSubPremises();
        bool HasAssocPremises();
        bool HasAssocOwnerships();
        bool HasAssocRestrictions();
        bool HasAssocFundHistory();
        bool HasAssocTenancyPersons();
        bool HasAssocTenancyReasons();
        bool HasAssocTenancyAgreements();
        bool HasAssocTenancyObjects();
        bool HasAssocClaims();
        bool HasAssocClaimStates();
        bool HasAssocTenancies();
        bool HasAssocResettlePersons();
        bool HasAssocResettleFromObjects();
        bool HasAssocResettleToObjects();

        bool CanFilterSocialFund();
        bool CanFilterCommercialFundFund();
        bool CanFilterSpecialFund();
        bool CanFilterOtherFunds();

        int GetRecordCount();

        bool Selected { get; set; }

        bool HasTenancyContract17xReport();
        bool HasTenancyContractReport();
        bool HasTenancyActReport();
        bool HasTenancyAgreementReport();
        bool HasTenancyOrderReport();     

        void TenancyContract17xReportGenerate(TenancyContractTypes tenancyContractType);
        void TenancyContractReportGenerate();
        void TenancyActReportGenerate();
        void TenancyAgreementReportGenerate();
        void TenancyOrderReportGenerate();

        void RegistryExcerptPremiseReportGenerate();

        void RegistryExcerptSubPremiseReportGenerate();

        void RegistryExcerptSubPremisesReportGenerate();

        bool HasRegistryExcerptSubPremisesReport();

        bool HasRegistryExcerptSubPremiseReport();

        bool HasRegistryExcerptPremiseReport();

        bool HasAttach1Form2();

        bool HasAttach1Form3();
        void Attach1Form2();
        void Attach1Form3();
    }
}
