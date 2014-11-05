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

namespace Registry
{
    public partial class MainForm : Form, IMenuCallback
    {
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
            RegistryStateUpdate();
            NavigationStateUpdate();
            EditingStateUpdate();
            RibbonTabsStateUpdate();
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
            // Инстрации подгружаются в асинхронном режиме
            // Тут будет реализован планировщик загрузки с учетом прав пользователей и частоты использования данных
            // Реестр жилого фонда
            BuildingsDataModel.GetInstance(toolStripProgressBar, 3);
            StructureTypesDataModel.GetInstance(toolStripProgressBar, 3);
            KladrStreetsDataModel.GetInstance(toolStripProgressBar, 3);
            PremisesDataModel.GetInstance(toolStripProgressBar, 3);
            PremisesTypesDataModel.GetInstance(toolStripProgressBar, 3);
            PremisesKindsDataModel.GetInstance(toolStripProgressBar, 3);
            SubPremisesDataModel.GetInstance(toolStripProgressBar, 3);
            FundTypesDataModel.GetInstance(toolStripProgressBar, 3);
            ObjectStatesDataModel.GetInstance(toolStripProgressBar, 3);
            FundsBuildingsAssocDataModel.GetInstance(toolStripProgressBar, 3);
            FundsPremisesAssocDataModel.GetInstance(toolStripProgressBar, 3);
            FundsSubPremisesAssocDataModel.GetInstance(toolStripProgressBar,3);
            FundsHistoryDataModel.GetInstance(toolStripProgressBar,3);
            OwnershipBuildingsAssocDataModel.GetInstance(toolStripProgressBar, 3);
            OwnershipPremisesAssocDataModel.GetInstance(toolStripProgressBar, 3);
            OwnershipsRightsDataModel.GetInstance(toolStripProgressBar, 3);
            OwnershipRightTypesDataModel.GetInstance(toolStripProgressBar, 2);
            RestrictionsBuildingsAssocDataModel.GetInstance(toolStripProgressBar, 2);
            RestrictionsPremisesAssocDataModel.GetInstance(toolStripProgressBar, 2);
            RestrictionsDataModel.GetInstance(toolStripProgressBar, 2);
            RestrictionTypesDataModel.GetInstance(toolStripProgressBar, 2);
            KladrRegionsDataModel.GetInstance(toolStripProgressBar, 2);
            // Процесс найма
            TenancyProcessesDataModel.GetInstance(toolStripProgressBar, 3);
            TenancyPersonsDataModel.GetInstance(toolStripProgressBar, 3);
            KinshipsDataModel.GetInstance(toolStripProgressBar, 2);
            TenancyBuildingsAssocDataModel.GetInstance(toolStripProgressBar, 2);
            TenancyPremisesAssocDataModel.GetInstance(toolStripProgressBar, 2);
            TenancySubPremisesAssocDataModel.GetInstance(toolStripProgressBar, 2);
            TenancyReasonsDataModel.GetInstance(toolStripProgressBar, 2);
            TenancyReasonTypesDataModel.GetInstance(toolStripProgressBar, 2);
            RentTypesDataModel.GetInstance(toolStripProgressBar, 2);
            DocumentTypesDataModel.GetInstance(toolStripProgressBar, 2);
            ExecutorsDataModel.GetInstance(toolStripProgressBar, 2);
            TenancyAgreementsDataModel.GetInstance(toolStripProgressBar, 2);
            WarrantsDataModel.GetInstance(toolStripProgressBar, 2);
            WarrantDocTypesDataModel.GetInstance(toolStripProgressBar, 2);
            DocumentsIssuedByDataModel.GetInstance(toolStripProgressBar, 2);
            // Претензионно-исковая работа
            ClaimsDataModel.GetInstance(toolStripProgressBar, 2);
            ClaimStatesDataModel.GetInstance(toolStripProgressBar, 2);
            ClaimStateTypesDataModel.GetInstance(toolStripProgressBar, 2);
            ClaimStateTypesRelationsDataModel.GetInstance(toolStripProgressBar, 2);
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
            viewport.Show(dockPanel, DockState.Document);
        }

        private void ribbonButtonOpen_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).OpenDetails();
        }

        public void TabsStateUpdate()
        {
            ribbonButtonTabCopy.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanDuplicate();
            ribbonButtonTabClose.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null);
            ribbonButtonTabsClose.Enabled = (dockPanel.Documents.Count() > 0);  
        }

        public void RegistryStateUpdate()
        {
            //Always enable, maybe will change in future
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
            ribbonButtonDeleteRecord.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanDeleteRecord();
            ribbonButtonInsertRecord.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanInsertRecord();
            ribbonButtonCopyRecord.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanCopyRecord();
            ribbonButtonCancel.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanCancelRecord();
            ribbonButtonSave.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanSaveRecord();
        }

        public void RibbonTabsStateUpdate()
        {
            //Always enable, maybe will change in future
        }

        public void RelationsStateUpdate()
        {
            ribbonPanelRelations.Items.Clear();
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocBuildings())
                ribbonPanelRelations.Items.Add(ribbonButtonBuildings);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocSubPremises())
                ribbonPanelRelations.Items.Add(ribbonButtonSubPremises);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocPremises())
                ribbonPanelRelations.Items.Add(ribbonButtonPremises);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocOwnerships())
                ribbonPanelRelations.Items.Add(ribbonButtonOwnerships);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocRestrictions())
                ribbonPanelRelations.Items.Add(ribbonButtonRestrictions);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocFundHistory())
                ribbonPanelRelations.Items.Add(ribbonButtonFundsHistory);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocTenancyObjects())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyObjects);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocTenancyPersons())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyPersons);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocTenancyReasons())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyReasons);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocTenancyAgreements())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyAgreements);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocClaims())
                ribbonPanelRelations.Items.Add(ribbonButtonClaims);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasAssocClaimStates())
                ribbonPanelRelations.Items.Add(ribbonButtonClaimStates);
            ribbon1.SuspendUpdating();
            if (ribbonPanelRelations.Items.Count == 0)
                ribbonTabGeneral.Panels.Remove(ribbonPanelRelations);
            else
                if (!ribbonTabGeneral.Panels.Contains(ribbonPanelRelations))
                    ribbonTabGeneral.Panels.Insert(2, ribbonPanelRelations);
            ribbon1.ResumeUpdating(true);
        }

        public void TenancyRefsStateUpdate()
        {
            ribbonPanelTenancyDocs.Items.Clear();
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasTenancyContract17xReport())
                ribbonPanelTenancyDocs.Items.Add(ribbonButtonTenancyContract17x);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasTenancyContractReport())
                ribbonPanelTenancyDocs.Items.Add(ribbonButtonTenancyContract);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasTenancyActReport())
                ribbonPanelTenancyDocs.Items.Add(ribbonButtonTenancyAct);
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).HasTenancyAgreementReport())
                ribbonPanelTenancyDocs.Items.Add(ribbonButtonTenancyAgreement);
            ribbon1.SuspendUpdating();
            if (ribbonPanelTenancyDocs.Items.Count == 0)
                ribbonTabTenancyProcesses.Panels.Remove(ribbonPanelTenancyDocs);
            else
                if (!ribbonTabTenancyProcesses.Panels.Contains(ribbonPanelTenancyDocs))
                    ribbonTabTenancyProcesses.Panels.Insert(1, ribbonPanelTenancyDocs);
            ribbon1.ResumeUpdating(true);
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
            /*if (dockPanel.Documents.Count() > 1)
            {
                int index = -1;
                foreach (var document in dockPanel.Documents)
                {
                    index++;
                    if (document == dockPanel.ActiveDocument)
                    {
                        break;
                    }
                }
                ((DockContent)dockPanel.Documents.ElementAt(index - 1)).Activate();
            }*/
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            /*UserDomain user = UserDomain.Current;
            if (user == null)
            {
                MessageBox.Show("Пользователь не распознан или учетная запись не включена в службу каталогов Active Directory","Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            toolStripLabelHelloUser.Text = "Здравствуйте, " + user.DisplayName;*/
            PreLoadData();
        }

        private void ribbonButtonRegistryShortStatistic_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistryShortStatisticReporter).Run();
        }

        private void ribbonButtonOwnershipReport_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistryOwnershipsReporter).Run();
        }

        private void ribbonButtonCommercialFundReport_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistryCommercialFundReporter).Run();
        }

        private void ribbonButtonSpecialFundReport_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistrySpecialFundReporter).Run();
        }

        private void ribbonButtonSocialFundReport_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistrySocialFundReporter).Run();
        }

        private void ribbonButtonPremisesForOrphansReport_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistryPremisesForOrphansReporter).Run();
        }

        private void ribbonButtonPremisesByExchangeReport_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistryPremisesByExchangeReporter).Run();
        }

        private void ribbonButtonPremisesByDonationReport_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistryPremisesByDonationReporter).Run();
        }

        private void ribbonButtonMunicipalPremises_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistryMunicipalPremisesReporter).Run();
        }

        private void ribbonButtonRegistryFullStatistic_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.RegistryFullStatisticReporter).Run();
        }

        private void ribbonButtonClaimsStatistic_Click(object sender, EventArgs e)
        {
            Reporting.ReporterFactory.CreateReporter(Reporting.ReporterType.ClaimsStatisticReporter).Run();
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
    }
}
