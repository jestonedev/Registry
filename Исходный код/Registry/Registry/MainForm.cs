using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Registry.SearchForms;
using Registry.Viewport;
using Security;
using WeifenLuo.WinFormsUI.Docking;

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
            toolStripProgressBar.Maximum = 0;
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead) || AccessControl.HasPrivelege(Priveleges.TenancyRead)
                || AccessControl.HasPrivelege(Priveleges.ResettleRead))
                toolStripProgressBar.Maximum += 9;   
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead))
                toolStripProgressBar.Maximum += 13;
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
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.BuildingsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.PremisesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.SubPremisesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.KladrStreetsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.KladrRegionsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.PremisesTypesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.FundTypesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ObjectStatesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.OwnershipRightTypesDataModel);
            }
            // Реестр жилого фонда
            if (AccessControl.HasPrivelege(Priveleges.RegistryRead))
            {
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.StructureTypesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.PremisesKindsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.FundsBuildingsAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.FundsPremisesAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.FundsSubPremisesAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.FundsHistoryDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.OwnershipBuildingsAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.OwnershipPremisesAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.OwnershipsRightsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.RestrictionsBuildingsAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.RestrictionsPremisesAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.RestrictionsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.RestrictionTypesDataModel);
            }
            //Общие таблицы для претензионно-исковой работы и процессов найма
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead) || AccessControl.HasPrivelege(Priveleges.ClaimsRead))
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.TenancyProcessesDataModel);
            // Процессы найма
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                // При поиске здания или помещения по нанимателю предварительная подгрузка не производится 
                // Данные подгружаются по необходимости
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.TenancyPersonsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.KinshipsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.TenancyBuildingsAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.TenancyPremisesAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.TenancySubPremisesAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.TenancyReasonsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.TenancyReasonTypesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.RentTypesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.DocumentTypesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ExecutorsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.TenancyAgreementsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.WarrantsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.WarrantDocTypesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.DocumentsIssuedByDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.TenancyNotifiesDataModel);
            }
            // Претензионно-исковая работа
            if (AccessControl.HasPrivelege(Priveleges.ClaimsRead))
            {
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ClaimsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ClaimStatesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ClaimStateTypesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ClaimStateTypesRelationsDataModel);
            }
            // Процессы переселения
            if (AccessControl.HasPrivelege(Priveleges.ResettleRead))
            {
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ResettleProcessesDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ResettlePersonsDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ResettleBuildingsFromAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ResettleBuildingsToAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ResettlePremisesFromAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ResettlePremisesToAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ResettleSubPremisesFromAssocDataModel);
                DataModel.GetInstance(toolStripProgressBar, 1, DataModelType.ResettleSubPremisesToAssocDataModel);
            }
        }

        private void ribbonButtonTabClose_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.Close();
        }

        private void ribbonButtonTabsClose_Click(object sender, EventArgs e)
        {
            for (var i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if (dockPanel.Documents.ElementAt(i) is IMenuController)
                {
                    var document = dockPanel.Documents.ElementAt(i) as IMenuController;
                    if (document != null)
                        document.Close();
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
                viewport.Show(dockPanel, DockState.Document);
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
            if (document.HasAssocResettlePersons())
                ribbonPanelRelations.Items.Add(ribbonButtonResettlePersons);
            if (document.HasAssocResettleFromObjects())
                ribbonPanelRelations.Items.Add(ribbonButtonResettleFromObjects);
            if (document.HasAssocResettleToObjects())
                ribbonPanelRelations.Items.Add(ribbonButtonResettleToObjects);
        }

        private void ClaimRelationsStateUpdate()
        {
            if (!AccessControl.HasPrivelege(Priveleges.ClaimsRead)) return;
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document == null) return;
            if (document.HasAssocClaims())
                ribbonPanelRelations.Items.Add(ribbonButtonClaims);
            if (document.HasAssocClaimStates())
                ribbonPanelRelations.Items.Add(ribbonButtonClaimStates);
        }

        private void TenancyRelationsStateUpdate()
        {
            if (!AccessControl.HasPrivelege(Priveleges.TenancyRead)) return;
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document == null) return;
            if (document.HasAssocTenancyObjects())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyObjects);
            if (document.HasAssocTenancyPersons())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyPersons);
            if (document.HasAssocTenancyReasons())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyReasons);
            if (document.HasAssocTenancyAgreements())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyAgreements);
            if (document.HasAssocTenancies())
                ribbonPanelRelations.Items.Add(ribbonButtonAssocTenancies);
        }

        private void RegistryRelationsStateUpdate()
        {
            if (!AccessControl.HasPrivelege(Priveleges.RegistryRead)) return;
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document == null) return;
            if (document.HasAssocBuildings())
                ribbonPanelRelations.Items.Add(ribbonButtonBuildings);
            if (document.HasAssocSubPremises())
                ribbonPanelRelations.Items.Add(ribbonButtonSubPremises);
            if (document.HasAssocPremises())
                ribbonPanelRelations.Items.Add(ribbonButtonPremises);
            if (document.HasAssocOwnerships())
                ribbonPanelRelations.Items.Add(ribbonButtonOwnerships);
            if (document.HasAssocRestrictions())
                ribbonPanelRelations.Items.Add(ribbonButtonRestrictions);
            if (document.HasAssocFundHistory())
                ribbonPanelRelations.Items.Add(ribbonButtonFundsHistory);
        }

        public void DocumentsStateUpdate()
        {
            ribbon1.OrbDropDown.RecentItems.Clear();
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document == null)
                return;
            if (document.HasTenancyContract17xReport())
            {
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyContract1711);
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyContract1712);
            }
            if (document.HasTenancyContractReport())
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyContract);
            if (document.HasTenancyActReport())
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyAct);
            if (document.HasTenancyAgreementReport())
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbTenancyAgreement);
            if (document.HasRegistryExcerptPremiseReport())
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbRegistryExcerptPremise);
            if (document.HasRegistryExcerptSubPremiseReport())
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbRegistryExcerptSubPremise);
            if (document.HasRegistryExcerptSubPremisesReport())
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonOrbRegistryExcerptSubPremises);
            if (document.HasExportToOds())
                ribbon1.OrbDropDown.RecentItems.Add(ribbonButtonExportOds);
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
                document.ShowBuildings();
        }

        private void ribbonButtonPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowPremises();
        }

        private void ribbonButtonSubPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowSubPremises();
        }

        private void ribbonButtonRestrictions_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowRestrictions();
        }

        private void ribbonButtonOwnership_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowOwnerships();
        }

        private void ribbonButtonFundsHistory_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowFundHistory();
        }

        private void ribbonButtonPersons_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowTenancyPersons();
        }

        private void ribbonButtonTenancyReasons_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowTenancyReasons();
        }

        private void ribbonButtonAgreements_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowTenancyAgreements();
        }

        private void ribbonButtonTenancyPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowTenancyPremises();
        }

        private void ribbonButtonTenancyBuildings_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowTenancyBuildings();
        }

        private void ribbonButtonClaims_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowClaims();
        }

        private void ribbonButtonClaimStates_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowClaimStates();
        }

        private void ribbonButtonAssocTenancies_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowTenancies();
        }

        private void ribbonButtonResettlePersons_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowResettlePersons();
        }

        private void ribbonButtonResettleFromPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowResettleFromPremises();
        }

        private void ribbonButtonResettleFromBuildings_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowResettleFromBuildings();
        }

        private void ribbonButtonResettleToPremises_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowResettleToPremises();
        }

        private void ribbonButtonResettleToBuildings_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowResettleToBuildings();
        }

        private void ribbonOrbMenuItemBuildings_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.BuildingListViewport);
        }

        private void ribbonOrbMenuItemPremises_Click(object sender, EventArgs e)
        {
            var filter = "";
            var municipalIds = DataModelHelper.ObjectIdsByStates(EntityType.Premise, new[] { 4, 5, 9 });
            var ids = municipalIds.Aggregate("", (current, id) => current + (id.ToString(CultureInfo.InvariantCulture) + ","));
            ids = ids.TrimEnd(',');
            filter += "(id_state IN (4, 5, 9) OR (id_state = 1 AND id_premises IN (0" + ids + ")))";
            var viewport = ViewportFactory.CreateViewport(this, ViewportType.PremisesListViewport);
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
            CreateViewport(ViewportType.TenancyListViewport);
        }

        private void ribbonOrbMenuItemResettles_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.ResettleProcessListViewport);
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

        private void ribbonButtonDocumentResidence_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.DocumentsResidenceViewport);
        }

        private void CreateViewport(ViewportType viewportType)
        {
            var viewport = ViewportFactory.CreateViewport(this, viewportType);
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
            if (RegistrySettings.UseLDAP)
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
        }

        private void RunReport(ReporterType reporterType)
        {
            Reporter reporter = ReporterFactory.CreateReporter(reporterType);
            reporter.ReportOutputStreamResponse += reporter_ReportOutputStreamResponse;
            reporter.ReportComplete += reporter_ReportComplete;
            reporter.ReportCanceled += reporter_ReportCanceled;
            _reportCounter++;
            reporter.Run();
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
            if (!String.IsNullOrEmpty(e.Text.Trim()) && (!Regex.IsMatch(e.Text.Trim(), "styles.xml")))
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

        private void ribbonButtonMunicipalPremises_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryMunicipalPremisesReporter);
        }

        private void ribbonButtonAllPremises_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.RegistryAllPremisesReporter);
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

        private void ribbonButton1711_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).TenancyContract17xReportGenerate(TenancyContractTypes.SpecialContract1711Form);
        }

        private void ribbonButton1712_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController)dockPanel.ActiveDocument).TenancyContract17xReportGenerate(TenancyContractTypes.SpecialContract1712Form);
        }

        private void ribbonButtonTenancyAct_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController)dockPanel.ActiveDocument).TenancyActReportGenerate();
        }

        private void ribbonButtonTenancyContract_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController)dockPanel.ActiveDocument).TenancyContractReportGenerate();
        }

        private void ribbonButtonTenancyAgreement_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController)dockPanel.ActiveDocument).TenancyAgreementReportGenerate();
        }

        private void ribbonButtonOrderByCurrentTenancy_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController)dockPanel.ActiveDocument).TenancyOrderReportGenerate();
        }

        private void ribbonButtonOrbRegistryExcerptPremise_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController)dockPanel.ActiveDocument).RegistryExcerptPremiseReportGenerate();
        }

        private void ribbonButtonOrbRegistryExcerptSubPremise_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController)dockPanel.ActiveDocument).RegistryExcerptSubPremiseReportGenerate();
        }

        private void ribbonButtonOrbRegistryExcerptSubPremises_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController)dockPanel.ActiveDocument).RegistryExcerptSubPremisesReportGenerate();
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
                if (ribbonButtonSearch.Enabled)
                    ribbonButtonSimpleSearch_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.F))
            {
                if (ribbonButtonSearch.Enabled)
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
            if (keyData == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ribbonButtonExportOds_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).ExportToOds();
        }
    }
}
