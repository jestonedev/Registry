using System.Collections.Generic;
using Registry.Reporting;
using Registry.SearchForms;

namespace Registry.Viewport
{
    public interface IMenuController
    {
        bool Selected { get; set; }
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
        int GetRecordCount();

        bool CanFilterSocialFund();
        bool CanFilterCommercialFundFund();
        bool CanFilterSpecialFund();
        bool CanFilterOtherFunds();

        void ShowAssocViewport(ViewportType viewportType);
        bool HasAssocViewport(ViewportType viewportType);
        void GenerateReport(ReporterType reporterType);
        bool HasReport(ReporterType reporterType);
    }
}
