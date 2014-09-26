﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        void DataRefresh();
        void SearchRecord();
        void ClearSearch();
        void ShowBuildings();
        void ShowPremises();
        void ShowSubPremises();
        void ShowRestrictions();
        void ShowOwnerships();
        void ShowFundHistory();
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
        bool CanDataRefresh();
        bool CanSearchRecord();
        bool CanShowPremises();
        bool CanShowSubPremises();
        bool CanShowRestrictions();
        bool CanShowOwnerships();
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
        bool HasFundHistory();

        bool CanFilterSocialFund();
        bool CanFilterCommercialFundFund();
        bool CanFilterSpecialFund();
        bool CanFilterOtherFunds();
    }
}
