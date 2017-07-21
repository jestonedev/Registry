using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Registry.Viewport;
using Registry.Viewport.MultiMasters;
using Registry.Viewport.SearchForms;
using Security;
using Settings;
using WeifenLuo.WinFormsUI.Docking;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;

namespace Registry
{
    public partial class MainForm : Form, IMenuCallback
    {
        private readonly ReportLogForm _reportLogForm = new ReportLogForm();
        private int _reportCounter;

        private void ChangeViewportsSelectProprty()
        {
            for (var i = dockPanel.Documents.Count() - 1; i >= 0; i--)
            {
                var document = dockPanel.Documents.ElementAt(i) as IMenuController;
                if (document != null)
                    document.Selected = false;
            }
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).Selected = true;
        }

        private void ChangeMainMenuState()
        {
            TabsStateUpdate();
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            DocumentsStateUpdate();
        }

        public void StatusBarStateUpdate()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                toolStripLabelRecordCount.Text = @"Всего записей: " + document.GetRecordCount();
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
            Action afterLoadHandler = () =>
            {
                toolStripProgressBar.Value += 1;
                if (toolStripProgressBar.Value != toolStripProgressBar.Maximum) return;
                toolStripProgressBar.Visible = false;
                //Если мы загрузили все данные, то запускаем CallbackUpdater
                DataModelsCallbackUpdater.GetInstance().Run();
                CalcDataModel.RunRefreshWalker();
            };

            toolStripProgressBar.Maximum = 0;
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead) || AccessControl.HasPrivelege(Priveleges.TenancyRead)
                || AccessControl.HasPrivelege(Priveleges.ResettleRead))
                toolStripProgressBar.Maximum += 9;   
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead))
                toolStripProgressBar.Maximum += 14;
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
                toolStripProgressBar.Maximum += 18;
            //Общие таблицы для реестра жилого фонда, процессов найма и процессов переселения
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead) || AccessControl.HasPrivelege(Priveleges.TenancyRead)
                || AccessControl.HasPrivelege(Priveleges.ResettleRead))
            {
                EntityDataModel<Building>.GetInstance(afterLoadHandler);
                EntityDataModel<Premise>.GetInstance(afterLoadHandler);
                EntityDataModel<SubPremise>.GetInstance(afterLoadHandler);
                DataModel.GetInstance<KladrStreetsDataModel>(afterLoadHandler);
                DataModel.GetInstance<KladrRegionsDataModel>(afterLoadHandler);
                DataModel.GetInstance<PremisesTypesDataModel>(afterLoadHandler);
                DataModel.GetInstance<FundTypesDataModel>(afterLoadHandler);
                DataModel.GetInstance<ObjectStatesDataModel>(afterLoadHandler);
                EntityDataModel<OwnershipRightType>.GetInstance(afterLoadHandler);
            }
            // Реестр жилого фонда
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead))
            {
                EntityDataModel<StructureType>.GetInstance(afterLoadHandler);
                EntityDataModel<HeatingType>.GetInstance(afterLoadHandler);
                DataModel.GetInstance<PremisesKindsDataModel>(afterLoadHandler);
                EntityDataModel<FundBuildingAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<FundPremisesAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<FundSubPremisesAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<FundHistory>.GetInstance(afterLoadHandler);
                EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<OwnershipRight>.GetInstance(afterLoadHandler);
                EntityDataModel<RestrictionBuildingAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<RestrictionPremisesAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<Restriction>.GetInstance(afterLoadHandler);
                EntityDataModel<RestrictionType>.GetInstance(afterLoadHandler);
            }
            // Процессы найма
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                // При поиске здания или помещения по нанимателю предварительная подгрузка не производится 
                // Данные подгружаются по необходимости
                EntityDataModel<TenancyProcess>.GetInstance(afterLoadHandler);
                EntityDataModel<TenancyPerson>.GetInstance(afterLoadHandler);
                EntityDataModel<TenancyBuildingAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<TenancyPremisesAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<TenancySubPremisesAssoc>.GetInstance(afterLoadHandler);
                EntityDataModel<TenancyReason>.GetInstance(afterLoadHandler);
                EntityDataModel<ReasonType>.GetInstance(afterLoadHandler);
                EntityDataModel<TenancyProlongReasonType>.GetInstance(afterLoadHandler);
                EntityDataModel<Executor>.GetInstance(afterLoadHandler);
                EntityDataModel<TenancyAgreement>.GetInstance(afterLoadHandler);
                EntityDataModel<Warrant>.GetInstance(afterLoadHandler);
                EntityDataModel<DocumentIssuedBy>.GetInstance(afterLoadHandler);
                EntityDataModel<TenancyRentPeriod>.GetInstance(afterLoadHandler);
                DataModel.GetInstance<KinshipsDataModel>(afterLoadHandler);
                DataModel.GetInstance<RentTypesDataModel>(afterLoadHandler);
                DataModel.GetInstance<DocumentTypesDataModel>(afterLoadHandler);
                DataModel.GetInstance<WarrantDocTypesDataModel>(afterLoadHandler);
                DataModel.GetInstance<TenancyNotifiesDataModel>(afterLoadHandler);
            }
            if (toolStripProgressBar.Maximum == 0)
                toolStripProgressBar.Visible = false;
        }

        private void ribbonButtonTabClose_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
            {
                document.Close();
            }
        }

        private void ribbonButtonTabsClose_Click(object sender, EventArgs e)
        {
            for (var i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if (dockPanel.Documents.ElementAt(i) is IMenuController)
                {
                    var document = dockPanel.Documents.ElementAt(i) as IMenuController;
                    if (document != null)
                    {
                        document.Close();
                    }
                }
        }

        private void ribbonButtonTabCopy_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            var viewport = ((IMenuController) dockPanel.ActiveDocument).Duplicate();
            viewport.Show(dockPanel, DockState.Document);
        }

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            ChangeMainMenuState();
            StatusBarStateUpdate();
            ChangeViewportsSelectProprty();
            foreach (var doc in dockPanel.Contents)
            {
                if (doc is IMultiMaster)
                    (doc as IMultiMaster).UpdateToolbar();
            }
        }

        private void ribbonButtonFirst_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).MoveFirst();
            NavigationStateUpdate();
        }

        private void ribbonButtonLast_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).MoveLast();
            NavigationStateUpdate();
        }

        private void ribbonButtonPrev_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).MovePrev();
            NavigationStateUpdate();
        }

        private void ribbonButtonNext_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).MoveNext();
            NavigationStateUpdate();
        }

        private void ribbonButtonSearch_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            if (ribbonButtonSearch.Checked)
                ((IMenuController) dockPanel.ActiveDocument).ClearSearch();
            else
                ((IMenuController) dockPanel.ActiveDocument).SearchRecord(SearchFormType.SimpleSearchForm);
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            DocumentsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonExtendedSearch_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).SearchRecord(SearchFormType.ExtendedSearchForm);
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            DocumentsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonSimpleSearch_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).SearchRecord(SearchFormType.SimpleSearchForm);
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            DocumentsStateUpdate();
            StatusBarStateUpdate();
        }

        public void SearchButtonToggle(bool value)
        {
            ribbonButtonSearch.Checked = value;
        }

        public void AddViewport(Viewport.Viewport viewport)
        {
            if (viewport != null)
            {
                viewport.Show(dockPanel, DockState.Document);
                viewport.Focus();
            }
        }

        private void ribbonButtonOpen_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.OpenDetails();
        }

        public void TabsStateUpdate()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            ribbonButtonTabCopy.Enabled = document != null && document.CanDuplicate();
            ribbonButtonTabClose.Enabled = document != null;
            ribbonButtonTabsClose.Enabled = dockPanel.Documents.Any();  
        }

        public void NavigationStateUpdate()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            ribbonButtonFirst.Enabled = document != null && document.CanMoveFirst();
            ribbonButtonPrev.Enabled = document != null && document.CanMovePrev();
            ribbonButtonNext.Enabled = document != null && document.CanMoveNext();
            ribbonButtonLast.Enabled = document != null && document.CanMoveLast();
            ribbonButtonSearch.Enabled = document != null && document.CanSearchRecord();
            ribbonButtonSearch.Checked = document != null && document.SearchedRecords();
            ribbonButtonOpen.Enabled = document != null && document.CanOpenDetails();
        }

        public void EditingStateUpdate()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            ribbonButtonDeleteRecord.Enabled = document != null && document.CanDeleteRecord();
            ribbonButtonInsertRecord.Enabled = document != null && document.CanInsertRecord();
            ribbonButtonCopyRecord.Enabled = document != null && document.CanCopyRecord();
            ribbonButtonCancel.Enabled = document != null && document.CanCancelRecord();
            ribbonButtonSave.Enabled = document != null && document.CanSaveRecord();
        }

        public void RelationsStateUpdate()
        {
            ribbonPanelRelations.Items.Clear();
            ribbon1.SuspendUpdating();
            RegistryRelationsStateUpdate();
            TenancyRelationsStateUpdate();
            ClaimRelationsStateUpdate();
            ResettleRelationsStateUpdate();
            if (ribbonPanelRelations.Items.Count == 0)
                ribbonTabGeneral.Panels.Remove(ribbonPanelRelations);
            else
                if (!ribbonTabGeneral.Panels.Contains(ribbonPanelRelations))
                    ribbonTabGeneral.Panels.Insert(2, ribbonPanelRelations);
            ribbon1.ResumeUpdating(true);
        }

        private void ResettleRelationsStateUpdate()
        {
            if (!AccessControl.HasPrivelege(Priveleges.ResettleRead)) return;
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document == null) return;
            if (document.HasAssocViewport<ResettlePersonsViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonResettlePersons);
            if (document.HasAssocViewport<ResettleBuildingsViewport>(ResettleEstateObjectWay.From) ||
                document.HasAssocViewport<ResettlePremisesViewport>(ResettleEstateObjectWay.From))
                ribbonPanelRelations.Items.Add(ribbonButtonResettleFromObjects);
            if (document.HasAssocViewport<ResettleBuildingsViewport>(ResettleEstateObjectWay.To) ||
               document.HasAssocViewport<ResettlePremisesViewport>(ResettleEstateObjectWay.To))
                ribbonPanelRelations.Items.Add(ribbonButtonResettleToObjects);
        }

        private void ClaimRelationsStateUpdate()
        {
            if (!AccessControl.HasPrivelege(Priveleges.ClaimsRead)) return;
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document == null) return;
            if (document.HasAssocViewport<ClaimListViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonClaims);
            if (document.HasAssocViewport<ClaimStatesViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonClaimStates);
            if (document.HasAssocViewport<ClaimCourtOrdersViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonClaimCourtOrders);
            if (document.HasAssocViewport<PaymentsAccountsViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonAccounts);
            if (document.HasAssocViewport<PaymentsAccountHistoryViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonPaymentsAccountHistory);
            if (document.HasAssocViewport<PaymentsPremiseHistoryViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonPaymentsPremiseHistory);
        }

        private void TenancyRelationsStateUpdate()
        {
            if (!AccessControl.HasPrivelege(Priveleges.TenancyRead)) return;
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document == null) return;
            if (document.HasAssocViewport<TenancyBuildingsViewport>() ||
                document.HasAssocViewport<TenancyPremisesViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyObjects);
            if (document.HasAssocViewport<TenancyPersonsViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyPersons);
            if (document.HasAssocViewport<TenancyReasonsViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyReasons);
            if (document.HasAssocViewport<TenancyAgreementsViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyAgreements);
            if (document.HasAssocViewport<TenancyListViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonAssocTenancies);
        }

        private void RegistryRelationsStateUpdate()
        {
            if (!AccessControl.HasPrivelege(Priveleges.RegistryRead)) return;
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document == null) return;
            if (document.HasAssocViewport<BuildingListViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonBuildings);
            if (document.HasAssocViewport<SubPremisesViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonSubPremises);
            if (document.HasAssocViewport<PremisesListViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonPremises);
            if (document.HasAssocViewport<OwnershipListViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonOwnerships);
            if (document.HasAssocViewport<RestrictionListViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonRestrictions);
            if (document.HasAssocViewport<FundsHistoryViewport>())
                ribbonPanelRelations.Items.Add(ribbonButtonFundsHistory);
        }

        public void DocumentsStateUpdate()
        {
            ribbon1.OrbDropDown.RecentItems.Clear();
            var document = dockPanel.ActiveDocument as IMenuController;
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead))
            {
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbRegistryMultiPremises);
            }
            if (AccessControl.HasPrivelege(Priveleges.ClaimsRead))
            {
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbMultiPaymentAccount);
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbMultiClaims);
            }

            var mastersCount = ribbon1.OrbDropDown.RecentItems.Count;
            var hasMasters = mastersCount > 0;

            if (document == null)
            {
                return;
            }

            if (document.HasReport(ReporterType.TenancyContractSpecial1711Reporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyContract1711);
            if (document.HasReport(ReporterType.TenancyContractSpecial1712Reporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyContract1712);
            if (document.HasReport(ReporterType.TenancyContractCommercialReporter) ||
                document.HasReport(ReporterType.TenancyContractSocialReporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyContract);
            if (document.HasReport(ReporterType.TenancyActToEmploymentReporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyActToEmployment);
            if (document.HasReport(ReporterType.TenancyActFromEmploymentReporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyActFromEmployment);
            if (document.HasReport(ReporterType.TenancyAgreementReporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyAgreement);
            if (document.HasReport(ReporterType.TenancyNotifyDocumentsPrepared))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonNotifyContractAgreement);
            if (document.HasReport(ReporterType.TenancyNotifyIllegalResident))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonNotifyIllegalResident);
            if (document.HasReport(ReporterType.TenancyNotifyNoProlongTrouble))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonNotifyNoProlongTrouble);
            if (document.HasReport(ReporterType.TenancyNotifyNoProlongCategory))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonNotifyNoProlongCategory);
            if (document.HasReport(ReporterType.TenancyNotifyEvictionTrouble))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonNotifyEvictionTrouble);
            if (document.HasReport(ReporterType.TenancyNotifyContractViolation))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonNotifyCotractViolation);
            if (document.HasReport(ReporterType.RegistryExcerptReporterBuilding))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbRegistryExcerptBuild);
            if (document.HasReport(ReporterType.RegistryExcerptReporterPremise))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbRegistryExcerptPremise);
            if (document.HasReport(ReporterType.RegistryExcerptReporterSubPremise))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbRegistryExcerptSubPremise);
            if (document.HasReport(ReporterType.RegistryExcerptReporterAllMunSubPremises))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbRegistryExcerptSubPremises);
            if (document.HasReport(ReporterType.RequestToMvdReporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonRequestToMvd);

            if (document.HasReport(ReporterType.JudicialOrderReporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonJudicialOrder);
            if (document.HasReport(ReporterType.RequestToBksReporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonRequestToBks);
            if (document.HasReport(ReporterType.TransferToLegalDepartmentReporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonTransfertToLegalDepartment);

            var reportsCount = ribbon1.OrbDropDown.RecentItems.Count - mastersCount;
            var hasReports = reportsCount > 0;

            if (document.HasReport(ReporterType.ExportReporter))
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonExportOds);

            var hasExport = ribbon1.OrbDropDown.RecentItems.Count - reportsCount - mastersCount > 0;

            if (hasMasters && (hasReports || hasExport))
            {
                ribbon1.OrbDropDown.RecentItems.Insert(mastersCount, ribbonSeparatorMasters);
            }
            if (hasReports && hasExport)
            {
                ribbon1.OrbDropDown.RecentItems.Insert(mastersCount + reportsCount + 1, ribbonSeparatorExport);
            }
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
            ribbonOrbMenuItemPayments.Enabled = AccessControl.HasPrivelege(Priveleges.ClaimsRead);
            ribbonOrbMenuItemResettles.Enabled = AccessControl.HasPrivelege(Priveleges.ResettleRead);
        }

        private void ribbonButtonSave_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.SaveRecord();
            NavigationStateUpdate();
            RelationsStateUpdate();
            DocumentsStateUpdate();
        }

        private void ribbonButtonDeleteRecord_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.DeleteRecord();
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            DocumentsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonInsertRecord_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.InsertRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            DocumentsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCancel_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.CancelRecord();
            NavigationStateUpdate();
            RelationsStateUpdate();
            DocumentsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCopyRecord_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.CopyRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            DocumentsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonOrbMenuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (var i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if (dockPanel.Documents.ElementAt(i) is IMenuController)
                {
                    var document = dockPanel.Documents.ElementAt(i) as IMenuController;
                    if (document != null)
                        document.Close();
                }
            if (dockPanel.Documents.Count() != 0)
                e.Cancel = true;
        }

        public void ForceCloseDetachedViewports()
        {
            for (var i = dockPanel.Documents.Count() - 1; i >= 0; i--)
            {
                var document = dockPanel.Documents.ElementAt(i) as IMenuController;
                if (document != null && document.ViewportDetached())
                    document.ForceClose();
            }
        }

        private void ribbonButtonBuildings_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<BuildingListViewport>();
        }

        private void ribbonButtonPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<PremisesListViewport>();
        }

        private void ribbonButtonSubPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<SubPremisesViewport>();
        }

        private void ribbonButtonRestrictions_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<RestrictionListViewport>();
        }

        private void ribbonButtonOwnership_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<OwnershipListViewport>();
        }

        private void ribbonButtonFundsHistory_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<FundsHistoryViewport>();
        }

        private void ribbonButtonPersons_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<TenancyPersonsViewport>();
        }

        private void ribbonButtonTenancyReasons_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<TenancyReasonsViewport>();
        }

        private void ribbonButtonAgreements_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<TenancyAgreementsViewport>();
        }

        private void ribbonButtonTenancyPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<TenancyPremisesViewport>();
        }

        private void ribbonButtonTenancyBuildings_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<TenancyBuildingsViewport>();
        }

        private void ribbonButtonClaims_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<ClaimListViewport>();
        }

        private void ribbonButtonClaimStates_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<ClaimStatesViewport>();
        }

        private void ribbonButtonAccounts_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<PaymentsAccountsViewport>();
        }

        private void ribbonButtonPaymentsAccountHistory_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<PaymentsAccountHistoryViewport>();
        }

        private void ribbonButtonPaymentsPremiseHistory_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<PaymentsPremiseHistoryViewport>();
        }

        private void ribbonButtonAssocTenancies_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<TenancyListViewport>();
        }

        private void ribbonButtonResettlePersons_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<ResettlePersonsViewport>();
        }

        private void ribbonButtonResettleFromPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<ResettlePremisesViewport>(ResettleEstateObjectWay.From);
        }

        private void ribbonButtonResettleFromBuildings_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<ResettleBuildingsViewport>(ResettleEstateObjectWay.From);
        }

        private void ribbonButtonResettleToPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<ResettlePremisesViewport>(ResettleEstateObjectWay.To);
        }

        private void ribbonButtonResettleToBuildings_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<ResettleBuildingsViewport>(ResettleEstateObjectWay.To);
        }

        private void ribbonOrbMenuItemBuildings_Click(object sender, EventArgs e)
        {
            CreateViewport<BuildingListViewport>();
        }

        private void ribbonOrbMenuItemPremises_Click(object sender, EventArgs e)
        {
            var filter = "";
            var municipalIds = OtherService.ObjectIdsByStates(EntityType.Premise, DataModelHelper.MunicipalObjectStates().ToArray());
            var ids = municipalIds.Aggregate("", (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
            var municipalStateIds = DataModelHelper.MunicipalObjectStates()
                .Aggregate("", (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
            ids = ids.TrimEnd(',');
            municipalStateIds = municipalStateIds.Trim(',');
            filter += string.Format("(id_state IN ({0}) OR (id_state = 1 AND id_premises IN (0{1})))", municipalStateIds, ids);
            var viewport = ViewportFactory.CreateViewport<PremisesListViewport>(this);
            viewport.DynamicFilter = filter;
            if (((IMenuController) viewport).CanLoadData())
                ((IMenuController) viewport).LoadData();
            AddViewport(viewport);
            ChangeMainMenuState();
            StatusBarStateUpdate();
            ChangeViewportsSelectProprty();
        }

        private void ribbonOrbMenuItemSocNaim_Click(object sender, EventArgs e)
        {
            CreateViewport<TenancyListViewport>();
        }

        private void ribbonOrbMenuItemResettles_Click(object sender, EventArgs e)
        {
            CreateViewport<ResettleProcessListViewport>();
        }

        private void ribbonButtonStructureTypes_Click(object sender, EventArgs e)
        {
            CreateViewport<StructureTypeListViewport>();
        }

        private void ribbonButtonRestrictionTypes_Click(object sender, EventArgs e)
        {
            CreateViewport<RestrictionTypeListViewport>();
        }

        private void ribbonButtonOwnershipTypes_Click(object sender, EventArgs e)
        {
            CreateViewport<OwnershipTypeListViewport>();
        }

        private void ribbonButtonWarrants_Click(object sender, EventArgs e)
        {
            CreateViewport<WarrantsViewport>();
        }

        private void ribbonButtonReasonTypes_Click(object sender, EventArgs e)
        {
            CreateViewport<TenancyReasonTypesViewport>();
        }

        private void ribbonButtonExecutors_Click(object sender, EventArgs e)
        {
            CreateViewport<ExecutorsViewport>();
        }

        private void ribbonButtonIssuedBy_Click(object sender, EventArgs e)
        {
            CreateViewport<DocumentIssuedByViewport>();
        }

        private void ribbonOrbMenuItemClaims_Click(object sender, EventArgs e)
        {
            CreateViewport<ClaimListViewport>();
        }

        private void ribbonButtonClaimStateTypes_Click(object sender, EventArgs e)
        {
            CreateViewport<ClaimStateTypesViewport>();
        }

        private void ribbonOrbMenuItemPayments_Click(object sender, EventArgs e)
        {
            CreateViewport<PaymentsAccountsViewport>();
        }

        private void ribbonButtonDocumentResidence_Click(object sender, EventArgs e)
        {
            CreateViewport<DocumentsResidenceViewport>();
        }

        private void CreateViewport<T>() where T : Viewport.Viewport
        {
            var viewport = ViewportFactory.CreateViewport<T>(this);
            if (((IMenuController) viewport).CanLoadData())
                ((IMenuController) viewport).LoadData();
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
            if (RegistrySettings.UseLdap)
                user = UserDomain.Current;
            if (user == null)
                toolStripLabelHelloUser.Text = "";
            else
                toolStripLabelHelloUser.Text = @"Здравствуйте, " + user.DisplayName;
            //Загружаем права пользователя
            AccessControl.LoadPriveleges();
            if (AccessControl.HasNoPriveleges())
            {
                MessageBox.Show(@"У вас нет прав на использование данного приложения", @"Ошибка",
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
            DocumentsStateUpdate();
        }

        public void RunReport(ReporterType reporterType, Dictionary<string, string> arguments = null)
        {
            var reporter = ReporterFactory.CreateReporter(reporterType);
            reporter.ReportOutputStreamResponse += reporter_ReportOutputStreamResponse;
            reporter.ReportComplete += reporter_ReportComplete;
            reporter.ReportCanceled += reporter_ReportCanceled;
            _reportCounter++;
            if (arguments == null)
                reporter.Run();
            else
                reporter.Run(arguments);
        }

        void reporter_ReportCanceled(object sender, EventArgs e)
        {
            _reportCounter--;
            if (_reportCounter == 0)
                _reportLogForm.Hide();
        }

        void reporter_ReportComplete(object sender, EventArgs e)
        {
            _reportLogForm.Log("[" + ((Reporter)sender).ReportTitle + "]: Формирвоание отчета закончено");
            _reportCounter--;
            if (_reportCounter == 0)
                _reportLogForm.Hide();
        }

        void reporter_ReportOutputStreamResponse(object sender, ReportOutputStreamEventArgs e)
        {
            if (_reportLogForm.Visible == false)
                _reportLogForm.Show(dockPanel, DockState.DockBottomAutoHide);
            if (!string.IsNullOrEmpty(e.Text.Trim()) && (!Regex.IsMatch(e.Text.Trim(), "styles.xml")))
                _reportLogForm.Log("["+((Reporter)sender).ReportTitle+"]: "+e.Text.Trim());
        }

        private void ribbonButtonRegistryShortStatistic_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryShortStatisticReporter);
        }

        private void ribbonButtonOwnershipReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryOwnershipsReporter);
        }

        private void ribbonButtonCommercialFundReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryCommercialFundReporter);
        }

        private void ribbonButtonSpecialFundReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistrySpecialFundReporter);
        }

        private void ribbonButtonSocialFundReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistrySocialFundReporter);
        }

        private void ribbonButtonPremisesForOrphansReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryPremisesForOrphansReporter);
        }

        private void ribbonButtonPremisesByExchangeReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryPremisesByExchangeReporter);
        }

        private void ribbonButtonPremisesByDonationReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryPremisesByDonationReporter);
        }

        private void ribbonButtonMunicipalPremisesCurrentFunds_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryMunicipalPremisesCurrentFundsReporter);
        }

        private void ribbonButtonRegistryFullStatistic_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryFullStatisticReporter);
        }

        private void ribbonButtonClaimsStatistic_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.ClaimsStatisticReporter);
        }

        private void ribbonButtonStatistic_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.TenancyStatisticReporter);
        }

        private void ribbonButtonTenancyForCoMS_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.TenancyStatisticForCoMSReporter);
        }

        private void ribbonButtonTenancyOrder_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.TenancyOrderReporter);
        }

        private void ribbonButtonNotifies_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.TenancyNotifiesReporter);
        }

        private void ribbonButtonResettleTotalStatistic_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.ResettleTotalStatisticReporter);
        }

        private void ribbonButtonResettleBuildingDemolishing_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.ResettleBuildingDemolishingReporter);
        }

        private void ribbonButtonEmergencyBuildings_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.ResettleEmergencyBuildingsReporter);
        }

        private void ribbonButtonResettleShortProcessing_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.ResettleShortProcessingReporter);
        }

        private void ribbonButtonResettleFullProcessing_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.ResettleFullProcessingReporter);
        }

        private void ribbonButtonClaimStatesReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.ClaimsStatesReporter);
        }

        private void ribbonButtonMunicipalBuildings_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryMunicipalBuildingsReporter);
        }

        private void ribbonButtonMunicipalPremises_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryMunicipalPremisesReporter);
        }

        private void ribbonButtonAccountsDuplicateStatistic_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.AccountsDuplicateStatistic);
        }

        private void ribbonButton1711_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyContractSpecial1711Reporter); 
        }

        private void ribbonButton1712_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyContractSpecial1712Reporter); 
        }

        private void ribbonButtonTenancyActToEmployment_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyActToEmploymentReporter);
        }

        private void ribbonButtonOrbTenancyActFromEmployment_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyActFromEmploymentReporter); 
        }

        private void ribbonButtonTenancyContract_Click(object sender, EventArgs e)
        {

            var document = dockPanel.ActiveDocument as IMenuController;
            if (document == null) return;
            if (document.HasReport(ReporterType.TenancyContractCommercialReporter))
                document.GenerateReport(ReporterType.TenancyContractCommercialReporter);
            if (document.HasReport(ReporterType.TenancyContractSocialReporter))
                document.GenerateReport(ReporterType.TenancyContractSocialReporter);
        }

        private void ribbonButtonTenancyAgreement_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyAgreementReporter); 
        }

        private void ribbonButtonOrderByCurrentTenancy_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyOrderReporter); 
        }

        private void ribbonButtonOrbRegistryExcerptBuild_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.RegistryExcerptReporterBuilding);
        }
        private void ribbonButtonOrbRegistryExcerptPremise_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.RegistryExcerptReporterPremise); 
        }

        private void ribbonButtonOrbRegistryExcerptSubPremise_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.RegistryExcerptReporterSubPremise); 
        }

        private void ribbonButtonOrbRegistryExcerptSubPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.RegistryExcerptReporterAllMunSubPremises); 
        }

        private void ribbonButtonExportOds_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.ExportReporter); 
        }

        private void ribbonButtonNotifyContractAgreement_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyNotifyDocumentsPrepared);
        }

        private void ribbonButtonNotifyIllegalResident_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyNotifyIllegalResident);
        }

        private void ribbonButtonNotifyCotractViolation_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyNotifyContractViolation);
        }



        private void ribbonButtonNotifyNoProlongTrouble_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyNotifyNoProlongTrouble);
        }

        private void ribbonButtonNotifyNoProlongCategory_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyNotifyNoProlongCategory);
        }

        private void ribbonButtonNotifyEvictionTrouble_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TenancyNotifyEvictionTrouble);
        }
        private void ribbonButtonOrbMultiPaymentAccount_Click(object sender, EventArgs e)
        {
            var multiPaymentAccountsMaster = new MultiPaymentAccountsMaster(this);
            multiPaymentAccountsMaster.Show(dockPanel, DockState.DockBottom);
            multiPaymentAccountsMaster.UpdateToolbar();
        }

        private void ribbonButtonOrbMultiPremises_Click(object sender, EventArgs e)
        {
            var multiExcerptsMasterForm = new MultiPremisesMaster(this);
            multiExcerptsMasterForm.Show(dockPanel, DockState.DockBottom);
            multiExcerptsMasterForm.UpdateToolbar();
        }

        private void ribbonButtonOrbMultiClaims_Click(object sender, EventArgs e)
        {
            var multiClaimsMaster = new MultiClaimsMaster(this);
            multiClaimsMaster.Show(dockPanel, DockState.DockBottom);
            multiClaimsMaster.UpdateToolbar();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.S):
                    if (ribbonButtonSave.Enabled)
                        ribbonButtonSave_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.Alt | Keys.Z):
                    if (ribbonButtonCancel.Enabled)
                        ribbonButtonCancel_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.N):
                    if (ribbonButtonInsertRecord.Enabled)
                        ribbonButtonInsertRecord_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.O):
                    if (ribbonButtonOpen.Enabled)
                        ribbonButtonOpen_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.Alt | Keys.Home):
                    if (ribbonButtonFirst.Enabled)
                        ribbonButtonFirst_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.Alt | Keys.End):
                    if (ribbonButtonLast.Enabled)
                        ribbonButtonLast_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.Alt | Keys.Left):
                    if (ribbonButtonPrev.Enabled)
                        ribbonButtonPrev_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.Alt | Keys.Right):
                    if (ribbonButtonNext.Enabled)
                        ribbonButtonNext_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.F):
                    if (ribbonButtonSearch.Enabled)
                        ribbonButtonSimpleSearch_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.Alt | Keys.F):
                    if (ribbonButtonSearch.Enabled)
                        ribbonButtonExtendedSearch_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.Q):
                    if (ribbonButtonTabClose.Enabled)
                        ribbonButtonTabClose_Click(this, new EventArgs());
                    return true;
                case (Keys.Control | Keys.Alt | Keys.Q):
                    if (ribbonButtonTabsClose.Enabled)
                        ribbonButtonTabsClose_Click(this, new EventArgs());
                    return true;
                case Keys.Enter:
                    SendKeys.Send("{TAB}");
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        public Viewport.Viewport GetCurrentViewport()
        {
            return dockPanel.ActiveDocument as Viewport.Viewport;
        }

        public IEnumerable<Viewport.Viewport> GetAllViewports()
        {
            return dockPanel.Documents.Select(d => d as Viewport.Viewport).Where(d => d != null);
        }

        private void ribbonOrbMenuItemHelp_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"Руководство пользователя.doc");
            if (!File.Exists(fileName))
            {
                MessageBox.Show(
                    string.Format(@"Не удалось найти руководство пользователя по пути {0}", fileName),
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var process = new Process())
            {
                var psi = new ProcessStartInfo(fileName);
                process.StartInfo = psi;
                process.Start();
            }
        }

        private void ribbonButtonTransfertToLegalDepartment_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.TransferToLegalDepartmentReporter);
        }

        private void ribbonButtonRequestToBks_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.RequestToBksReporter);
        }

        private void ribbonButtonJudicialOrder_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.JudicialOrderReporter);
        }

        private void ribbonButtonClaimCourtOrders_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocViewport<ClaimCourtOrdersViewport>();
        }

        private void ribbonButtonCourtOrdersPrepareReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.ClaimsCourtOrderPrepareReporter);
        }

        private void ribbonButtonGisZkhExport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.GisZkhReporter);
        }

        private void ribbonButtonRequestToMvd_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.GenerateReport(ReporterType.RequestToMvdReporter);
        }
    }
}
