using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using Registry.Viewport;
using Registry.DataModels;
using WeifenLuo.WinFormsUI.Docking;
using Registry.Reporting;
using Security;
using System.Text.RegularExpressions;
using Registry.SearchForms;

namespace Registry
{
    public partial class MainForm : Form, IMenuCallback
    {
        private ReportLogForm reportLogForm = new ReportLogForm();
        private int reportCounter = 0;

        private void ChangeViewportsSelectProprty()
        {
            for (int i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                (dockPanel.Documents.ElementAt(i) as IMenuController).Selected = false;
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).Selected = true;
        }

        private void ChangeMainMenuState()
        {
            TabsStateUpdate();
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            TenancyRefsStateUpdate();
        }

        public void StatusBarStateUpdate()
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                toolStripLabelRecordCount.Text = "Всего записей: " + (dockPanel.ActiveDocument as IMenuController).GetRecordCount();
            else
                toolStripLabelRecordCount.Text = "";
        }

        public MainForm()
        {
            InitializeComponent();
            ChangeMainMenuState();
            StatusBarStateUpdate();
        }

        private void PreLoadData()
        {
            toolStripProgressBar.Maximum = 0;
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead) || AccessControl.HasPrivelege(Priveleges.TenancyRead)
                || AccessControl.HasPrivelege(Priveleges.ResettleRead))
                toolStripProgressBar.Maximum += 6;   
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead))
                toolStripProgressBar.Maximum += 16;
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead) || AccessControl.HasPrivelege(Priveleges.ClaimsRead))
                toolStripProgressBar.Maximum += 1;
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
                toolStripProgressBar.Maximum += 15;
            if (AccessControl.HasPrivelege(Priveleges.ClaimsRead))
                toolStripProgressBar.Maximum += 4;
            if (AccessControl.HasPrivelege(Priveleges.ResettleRead))
                toolStripProgressBar.Maximum += 8;
            //Общие таблицы для реестра жилого фонда, процессов найма и процессов переселения
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead) || AccessControl.HasPrivelege(Priveleges.TenancyRead)
                || AccessControl.HasPrivelege(Priveleges.ResettleRead))
            {
                BuildingsDataModel.GetInstance(toolStripProgressBar, 1);
                PremisesDataModel.GetInstance(toolStripProgressBar, 1);
                SubPremisesDataModel.GetInstance(toolStripProgressBar, 1);
                KladrStreetsDataModel.GetInstance(toolStripProgressBar, 1);
                KladrRegionsDataModel.GetInstance(toolStripProgressBar, 1);
                PremisesTypesDataModel.GetInstance(toolStripProgressBar, 1);
            }
            // Реестр жилого фонда
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead))
            {
                StructureTypesDataModel.GetInstance(toolStripProgressBar, 1);
                PremisesKindsDataModel.GetInstance(toolStripProgressBar, 1);
                FundTypesDataModel.GetInstance(toolStripProgressBar, 1);
                ObjectStatesDataModel.GetInstance(toolStripProgressBar, 1);
                FundsBuildingsAssocDataModel.GetInstance(toolStripProgressBar, 1);
                FundsPremisesAssocDataModel.GetInstance(toolStripProgressBar, 1);
                FundsSubPremisesAssocDataModel.GetInstance(toolStripProgressBar, 1);
                FundsHistoryDataModel.GetInstance(toolStripProgressBar, 1);
                OwnershipBuildingsAssocDataModel.GetInstance(toolStripProgressBar, 1);
                OwnershipPremisesAssocDataModel.GetInstance(toolStripProgressBar, 1);
                OwnershipsRightsDataModel.GetInstance(toolStripProgressBar, 1);
                OwnershipRightTypesDataModel.GetInstance(toolStripProgressBar, 1);
                RestrictionsBuildingsAssocDataModel.GetInstance(toolStripProgressBar, 1);
                RestrictionsPremisesAssocDataModel.GetInstance(toolStripProgressBar, 1);
                RestrictionsDataModel.GetInstance(toolStripProgressBar, 1);
                RestrictionTypesDataModel.GetInstance(toolStripProgressBar, 1);
            }
            //Общие таблицы для претензионно-исковой работы и процессов найма
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead) || AccessControl.HasPrivelege(Priveleges.ClaimsRead))
                TenancyProcessesDataModel.GetInstance(toolStripProgressBar, 1);
            // Процессы найма
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                TenancyPersonsDataModel.GetInstance(toolStripProgressBar, 1);
                KinshipsDataModel.GetInstance(toolStripProgressBar, 1);
                TenancyBuildingsAssocDataModel.GetInstance(toolStripProgressBar, 1);
                TenancyPremisesAssocDataModel.GetInstance(toolStripProgressBar, 1);
                TenancySubPremisesAssocDataModel.GetInstance(toolStripProgressBar, 1);
                TenancyReasonsDataModel.GetInstance(toolStripProgressBar, 1);
                TenancyReasonTypesDataModel.GetInstance(toolStripProgressBar, 1);
                RentTypesDataModel.GetInstance(toolStripProgressBar, 1);
                DocumentTypesDataModel.GetInstance(toolStripProgressBar, 1);
                ExecutorsDataModel.GetInstance(toolStripProgressBar, 1);
                TenancyAgreementsDataModel.GetInstance(toolStripProgressBar, 1);
                WarrantsDataModel.GetInstance(toolStripProgressBar, 1);
                WarrantDocTypesDataModel.GetInstance(toolStripProgressBar, 1);
                DocumentsIssuedByDataModel.GetInstance(toolStripProgressBar, 1);
                TenancyNotifiesDataModel.GetInstance(toolStripProgressBar, 1);
            }
            // Претензионно-исковая работа
            if (AccessControl.HasPrivelege(Priveleges.ClaimsRead))
            {
                ClaimsDataModel.GetInstance(toolStripProgressBar, 1);
                ClaimStatesDataModel.GetInstance(toolStripProgressBar, 1);
                ClaimStateTypesDataModel.GetInstance(toolStripProgressBar, 1);
                ClaimStateTypesRelationsDataModel.GetInstance(toolStripProgressBar, 1);
            }
            // Процессы переселения
            if (AccessControl.HasPrivelege(Priveleges.ResettleRead))
            {
                ResettleProcessesDataModel.GetInstance(toolStripProgressBar, 1);
                ResettlePersonsDataModel.GetInstance(toolStripProgressBar, 1);
                ResettleBuildingsFromAssocDataModel.GetInstance(toolStripProgressBar, 1);
                ResettleBuildingsToAssocDataModel.GetInstance(toolStripProgressBar, 1);
                ResettlePremisesFromAssocDataModel.GetInstance(toolStripProgressBar, 1);
                ResettlePremisesToAssocDataModel.GetInstance(toolStripProgressBar, 1);
                ResettleSubPremisesFromAssocDataModel.GetInstance(toolStripProgressBar, 1);
                ResettleSubPremisesToAssocDataModel.GetInstance(toolStripProgressBar, 1);
            }
        }

        private void ribbonButtonTabClose_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).Close();
        }

        private void ribbonButtonTabsClose_Click(object sender, EventArgs e)
        {
            for (int i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if (dockPanel.Documents.ElementAt(i) as IMenuController != null)
                    (dockPanel.Documents.ElementAt(i) as IMenuController).Close();
        }

        private void ribbonButtonTabCopy_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            Registry.Viewport.Viewport viewport = (dockPanel.ActiveDocument as IMenuController).Duplicate();
            viewport.Show(dockPanel, DockState.Document);
        }

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            ChangeMainMenuState();
            StatusBarStateUpdate();
            ChangeViewportsSelectProprty();
        }

        private void ribbonButtonFirst_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).MoveFirst();
            NavigationStateUpdate();
        }

        private void ribbonButtonLast_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).MoveLast();
            NavigationStateUpdate();
        }

        private void ribbonButtonPrev_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).MovePrev();
            NavigationStateUpdate();
        }

        private void ribbonButtonNext_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).MoveNext();
            NavigationStateUpdate();
        }

        private void ribbonButtonSearch_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            if (ribbonButtonSearch.Checked)
                (dockPanel.ActiveDocument as IMenuController).ClearSearch();
            else
                (dockPanel.ActiveDocument as IMenuController).SearchRecord(SearchFormType.SimpleSearchForm);
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            TenancyRefsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonExtendedSearch_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).SearchRecord(SearchFormType.ExtendedSearchForm);
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            TenancyRefsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonSimpleSearch_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).SearchRecord(SearchFormType.SimpleSearchForm);
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            TenancyRefsStateUpdate();
            StatusBarStateUpdate();
        }

        public void SearchButtonToggle(bool value)
        {
            ribbonButtonSearch.Checked = value;
        }

        public void AddViewport(Viewport.Viewport viewport)
        {
            if (viewport != null)
                viewport.Show(dockPanel, DockState.Document);
        }

        private void ribbonButtonOpen_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).OpenDetails();
        }

        public void TabsStateUpdate()
        {
            ribbonButtonTabCopy.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanDuplicate();
            ribbonButtonTabClose.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null);
            ribbonButtonTabsClose.Enabled = (dockPanel.Documents.Count() > 0);  
        }

        public void NavigationStateUpdate()
        {
            ribbonButtonFirst.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanMoveFirst();
            ribbonButtonPrev.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanMovePrev();
            ribbonButtonNext.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanMoveNext();
            ribbonButtonLast.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanMoveLast();
            ribbonButtonSearch.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanSearchRecord();
            ribbonButtonSearch.Checked = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).SearchedRecords();
            ribbonButtonOpen.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanOpenDetails();
        }

        public void EditingStateUpdate()
        {
            ribbonButtonDeleteRecord.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanDeleteRecord();
            ribbonButtonInsertRecord.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanInsertRecord();
            ribbonButtonCopyRecord.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanCopyRecord();
            ribbonButtonCancel.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanCancelRecord();
            ribbonButtonSave.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanSaveRecord();
        }

        public void RelationsStateUpdate()
        {
            ribbonPanelRelations.Items.Clear();
            bool hasActiveDocument = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null);
            ribbon1.SuspendUpdating();
            RegistryRelationsStateUpdate(hasActiveDocument);
            TenancyRelationsStateUpdate(hasActiveDocument);
            ClaimRelationsStateUpdate(hasActiveDocument);
            ResettleRelationsStateUpdate(hasActiveDocument);
            if (ribbonPanelRelations.Items.Count == 0)
                ribbonTabGeneral.Panels.Remove(ribbonPanelRelations);
            else
                if (!ribbonTabGeneral.Panels.Contains(ribbonPanelRelations))
                    ribbonTabGeneral.Panels.Insert(2, ribbonPanelRelations);
            ribbon1.ResumeUpdating(true);
        }

        private void ResettleRelationsStateUpdate(bool hasActiveDocument)
        {
            if (AccessControl.HasPrivelege(Priveleges.ResettleRead))
            {
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocResettlePersons())
                    ribbonPanelRelations.Items.Add(ribbonButtonResettlePersons);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocResettleFromObjects())
                    ribbonPanelRelations.Items.Add(ribbonButtonResettleFromObjects);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocResettleToObjects())
                    ribbonPanelRelations.Items.Add(ribbonButtonResettleToObjects);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocResettles())
                    ribbonPanelRelations.Items.Add(ribbonButtonAssocResettles);
            }
        }

        private void ClaimRelationsStateUpdate(bool hasActiveDocument)
        {
            if (AccessControl.HasPrivelege(Priveleges.ClaimsRead))
            {
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocClaims())
                    ribbonPanelRelations.Items.Add(ribbonButtonClaims);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocClaimStates())
                    ribbonPanelRelations.Items.Add(ribbonButtonClaimStates);
            }
        }

        private void TenancyRelationsStateUpdate(bool hasActiveDocument)
        {
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocTenancyObjects())
                    ribbonPanelRelations.Items.Add(ribbonButtonTenancyObjects);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocTenancyPersons())
                    ribbonPanelRelations.Items.Add(ribbonButtonTenancyPersons);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocTenancyReasons())
                    ribbonPanelRelations.Items.Add(ribbonButtonTenancyReasons);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocTenancyAgreements())
                    ribbonPanelRelations.Items.Add(ribbonButtonTenancyAgreements);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocTenancies())
                    ribbonPanelRelations.Items.Add(ribbonButtonAssocTenancies);
            }
        }

        private void RegistryRelationsStateUpdate(bool hasActiveDocument)
        {
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead))
            {
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocBuildings())
                    ribbonPanelRelations.Items.Add(ribbonButtonBuildings);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocSubPremises())
                    ribbonPanelRelations.Items.Add(ribbonButtonSubPremises);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocPremises())
                    ribbonPanelRelations.Items.Add(ribbonButtonPremises);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocOwnerships())
                    ribbonPanelRelations.Items.Add(ribbonButtonOwnerships);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocRestrictions())
                    ribbonPanelRelations.Items.Add(ribbonButtonRestrictions);
                if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocFundHistory())
                    ribbonPanelRelations.Items.Add(ribbonButtonFundsHistory);
            }
        }

        public void TenancyRefsStateUpdate()
        {
            ribbonPanelTenancyDocs.Items.Clear();
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && 
                (dockPanel.ActiveDocument as IMenuController).HasTenancyContract17xReport())
                ribbonPanelTenancyDocs.Items.Add(ribbonButtonTenancyContract17x);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && 
                (dockPanel.ActiveDocument as IMenuController).HasTenancyContractReport())
                ribbonPanelTenancyDocs.Items.Add(ribbonButtonTenancyContract);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && 
                (dockPanel.ActiveDocument as IMenuController).HasTenancyActReport())
                ribbonPanelTenancyDocs.Items.Add(ribbonButtonTenancyAct);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && 
                (dockPanel.ActiveDocument as IMenuController).HasTenancyAgreementReport())
                ribbonPanelTenancyDocs.Items.Add(ribbonButtonTenancyAgreement);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) &&
                (dockPanel.ActiveDocument as IMenuController).HasTenancyExcerptReport())
                ribbonPanelTenancyDocs.Items.Add(ribbonButtonTenancyExcerpt);
            ribbon1.SuspendUpdating();
            if (ribbonPanelTenancyDocs.Items.Count == 0)
                ribbonTabTenancyProcesses.Panels.Remove(ribbonPanelTenancyDocs);
            else
                if (!ribbonTabTenancyProcesses.Panels.Contains(ribbonPanelTenancyDocs))
                    ribbonTabTenancyProcesses.Panels.Insert(1, ribbonPanelTenancyDocs);
            ribbon1.ResumeUpdating(true);
        }

        private void RibbonTabsStateUpdate()
        {
            if (!AccessControl.HasPrivelege(Priveleges.RegistryRead))
                ribbon1.Tabs.Remove(ribbonTabHousing);
            if (!AccessControl.HasPrivelege(Priveleges.TenancyRead))
                ribbon1.Tabs.Remove(ribbonTabTenancyProcesses);
            if (!AccessControl.HasPrivelege(Priveleges.ClaimsRead))
                ribbon1.Tabs.Remove(ribbonTabClaims);
            if (!AccessControl.HasPrivelege(Priveleges.ResettleRead))
                ribbon1.Tabs.Remove(ribbonTabResettle);
        }

        private void MainMenuStateUpdate()
        {
            ribbonOrbMenuItemBuildings.Enabled = AccessControl.HasPrivelege(Priveleges.RegistryRead);
            ribbonOrbMenuItemPremises.Enabled = AccessControl.HasPrivelege(Priveleges.RegistryRead);
            ribbonOrbMenuItemTenancy.Enabled = AccessControl.HasPrivelege(Priveleges.TenancyRead);
            ribbonOrbMenuItemClaims.Enabled = AccessControl.HasPrivelege(Priveleges.ClaimsRead);
            ribbonOrbMenuItemResettles.Enabled = AccessControl.HasPrivelege(Priveleges.ResettleRead);
        }

        private void ribbonButtonSave_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).SaveRecord();
            NavigationStateUpdate();
            RelationsStateUpdate();
            TenancyRefsStateUpdate();
        }

        private void ribbonButtonDeleteRecord_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).DeleteRecord();
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            TenancyRefsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonInsertRecord_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).InsertRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            TenancyRefsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCancel_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).CancelRecord();
            NavigationStateUpdate();
            RelationsStateUpdate();
            TenancyRefsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCopyRecord_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).CopyRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            TenancyRefsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonOrbMenuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if (dockPanel.Documents.ElementAt(i) as IMenuController != null)
                    (dockPanel.Documents.ElementAt(i) as IMenuController).Close();
            if (dockPanel.Documents.Count() != 0)
                e.Cancel = true;
        }

        public void ForceCloseDetachedViewports()
        {
            for (int i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if ((dockPanel.Documents.ElementAt(i) as IMenuController != null) && (dockPanel.Documents.ElementAt(i) as IMenuController).ViewportDetached())
                    (dockPanel.Documents.ElementAt(i) as IMenuController).ForceClose();
        }

        private void ribbonButtonBuildings_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowBuildings();
        }

        private void ribbonButtonPremises_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowPremises();
        }

        private void ribbonButtonSubPremises_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowSubPremises();
        }

        private void ribbonButtonRestrictions_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowRestrictions();
        }

        private void ribbonButtonOwnership_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowOwnerships();
        }

        private void ribbonButtonFundsHistory_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowFundHistory();
        }

        private void ribbonButtonPersons_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowTenancyPersons();
        }

        private void ribbonButtonTenancyReasons_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowTenancyReasons();
        }

        private void ribbonButtonAgreements_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowTenancyAgreements();
        }

        private void ribbonButtonTenancyPremises_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowTenancyPremises();
        }

        private void ribbonButtonTenancyBuildings_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowTenancyBuildings();
        }

        private void ribbonButtonClaims_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowClaims();
        }

        private void ribbonButtonClaimStates_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowClaimStates();
        }

        private void ribbonButtonAssocTenancies_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowTenancies();
        }

        private void ribbonOrbMenuItemBuildings_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.BuildingListViewport);
        }

        private void ribbonOrbMenuItemPremises_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.PremisesListViewport);
        }

        private void ribbonOrbMenuItemSocNaim_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.TenancyListViewport);
        }

        private void ribbonButtonStructureTypes_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.StructureTypeListViewport);
        }

        private void ribbonButtonRestrictionTypes_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.RestrictionTypeListViewport);
        }

        private void ribbonButtonOwnershipTypes_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.OwnershipTypeListViewport);
        }

        private void ribbonButtonWarrants_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.WarrantsViewport);
        }

        private void ribbonButtonReasonTypes_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.TenancyReasonTypesViewport);
        }

        private void ribbonButtonExecutors_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.ExecutorsViewport);
        }

        private void ribbonButtonIssuedBy_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.DocumentIssuedByViewport);
        }

        private void ribbonOrbMenuItemClaims_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.ClaimListViewport);
        }

        private void ribbonButtonClaimStateTypes_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.ClaimStateTypesViewport);
        }

        private void CreateViewport(ViewportType viewportType)
        {
            Registry.Viewport.Viewport viewport = Registry.Viewport.ViewportFactory.CreateViewport(this, viewportType);
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            AddViewport(viewport);
            ChangeMainMenuState();
            StatusBarStateUpdate();
            ChangeViewportsSelectProprty();
        }

        public void SwitchToPreviousViewport()
        {
            if (dockPanel.ActiveDocument != null && dockPanel.ActiveDocument.DockHandler.PreviousActive != null)
                dockPanel.ActiveDocument.DockHandler.PreviousActive.DockHandler.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UserDomain user = null;
            if (RegistrySettings.UseLDAP)
                user = UserDomain.Current;
            if (user == null)
                toolStripLabelHelloUser.Text = "";
            else
                toolStripLabelHelloUser.Text = "Здравствуйте, " + user.DisplayName;
            //Загружаем права пользователя
            AccessControl.LoadPriveleges();
            if (AccessControl.HasNoPriveleges())
            {
                MessageBox.Show("У вас нет прав на использование данного приложения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Application.Exit();
                return;
            }
            //Инициируем начальные параметры CallbackUpdater
            DataModelsCallbackUpdater.GetInstance().Initialize();
            //Загружаем данные в асинхронном режиме
            PreLoadData();
            //Обновляем состояние главного меню и вкладок в соответствии с правами пользователя
            MainMenuStateUpdate();
            RibbonTabsStateUpdate();
        }

        private void RunReport(Reporting.ReporterType reporterType)
        {
            Reporter reporter = Reporting.ReporterFactory.CreateReporter(reporterType);
            reporter.ReportOutputStreamResponse += new EventHandler<ReportOutputStreamEventArgs>(reporter_ReportOutputStreamResponse);
            reporter.ReportComplete += new EventHandler<EventArgs>(reporter_ReportComplete);
            reporter.ReportCanceled += new EventHandler<EventArgs>(reporter_ReportCanceled);
            reportCounter++;
            reporter.Run();
        }

        void reporter_ReportCanceled(object sender, EventArgs e)
        {
            reportCounter--;
            if (reportCounter == 0)
                reportLogForm.Hide();
        }

        void reporter_ReportComplete(object sender, EventArgs e)
        {
            reportLogForm.Log("[" + ((Reporter)sender).ReportTitle + "]: Формирвоание отчета закончено");
            reportCounter--;
            if (reportCounter == 0)
                reportLogForm.Hide();
        }

        void reporter_ReportOutputStreamResponse(object sender, ReportOutputStreamEventArgs e)
        {
            if (reportLogForm.Visible == false)
                reportLogForm.Show(dockPanel, DockState.DockBottomAutoHide);
            if (!String.IsNullOrEmpty(e.Text.Trim()) && (!Regex.IsMatch(e.Text.Trim(), "styles.xml")))
                reportLogForm.Log("["+((Reporter)sender).ReportTitle+"]: "+e.Text.Trim());
        }

        private void ribbonButtonRegistryShortStatistic_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistryShortStatisticReporter);
        }

        private void ribbonButtonOwnershipReport_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistryOwnershipsReporter);
        }

        private void ribbonButtonCommercialFundReport_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistryCommercialFundReporter);
        }

        private void ribbonButtonSpecialFundReport_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistrySpecialFundReporter);
        }

        private void ribbonButtonSocialFundReport_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistrySocialFundReporter);
        }

        private void ribbonButtonPremisesForOrphansReport_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistryPremisesForOrphansReporter);
        }

        private void ribbonButtonPremisesByExchangeReport_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistryPremisesByExchangeReporter);
        }

        private void ribbonButtonPremisesByDonationReport_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistryPremisesByDonationReporter);
        }

        private void ribbonButtonMunicipalPremises_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistryMunicipalPremisesReporter);
        }

        private void ribbonButtonRegistryFullStatistic_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.RegistryFullStatisticReporter);
        }

        private void ribbonButtonClaimsStatistic_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.ClaimsStatisticReporter);
        }

        private void ribbonButtonStatistic_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.TenancyStatisticReporter);
        }

        private void ribbonButtonTenancyForCoMS_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.TenancyStatisticForCoMSReporter);
        }

        private void ribbonButtonTenancyOrder_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.TenancyOrderReporter);
        }

        private void ribbonButtonNotifies_Click(object sender, EventArgs e)
        {
            RunReport(Reporting.ReporterType.TenancyNotifiesReporter);
        }

        private void ribbonButton1711_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).TenancyContract17xReportGenerate(TenancyContractTypes.SpecialContract1711Form);
        }

        private void ribbonButton1712_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).TenancyContract17xReportGenerate(TenancyContractTypes.SpecialContract1712Form);
        }

        private void ribbonButtonTenancyAct_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).TenancyActReportGenerate();
        }

        private void ribbonButtonTenancyContract_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).TenancyContractReportGenerate();
        }

        private void ribbonButtonTenancyAgreement_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).TenancyAgreementReportGenerate();
        }

        private void ribbonButtonOrderByCurrentTenancy_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).TenancyOrderReportGenerate();
        }

        private void ribbonButtonTenancyExcerpt_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).TenancyExcerptReportGenerate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                if (ribbonButtonSave.Enabled)
                    ribbonButtonSave_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Z))
            {
                if (ribbonButtonCancel.Enabled)
                    ribbonButtonCancel_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.N))
            {
                if (ribbonButtonInsertRecord.Enabled)
                    ribbonButtonInsertRecord_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.O))
            {
                if (ribbonButtonOpen.Enabled)
                    ribbonButtonOpen_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Home))
            {
                if (ribbonButtonFirst.Enabled)
                    ribbonButtonFirst_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.End))
            {
                if (ribbonButtonLast.Enabled)
                    ribbonButtonLast_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Left))
            {
                if (ribbonButtonPrev.Enabled)
                    ribbonButtonPrev_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Right))
            {
                if (ribbonButtonNext.Enabled)
                    ribbonButtonNext_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.F))
            {
                if (ribbonButtonSimpleSearch.Enabled)
                    ribbonButtonSimpleSearch_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.F))
            {
                if (ribbonButtonExtendedSearch.Enabled)
                    ribbonButtonExtendedSearch_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Q))
            {
                if (ribbonButtonTabClose.Enabled)
                    ribbonButtonTabClose_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Q))
            {
                if (ribbonButtonTabsClose.Enabled)
                    ribbonButtonTabsClose_Click(this, new EventArgs());
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
