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
using Registry.Entities;

namespace Registry
{
    public partial class MainForm : Form, IMenuCallback
    {
        private void ChangeViewportsSelectProprty()
        {
            for (int i = tabControl.TabCount - 1; i >= 0; i--)
                (tabControl.Controls[i] as IMenuController).Selected = false;
            if (tabControl.SelectedIndex == -1)
                return;
            (tabControl.SelectedTab as IMenuController).Selected = true;
        }

        private void ChangeMainMenuState()
        {
            TabsStateUpdate();
            RegistryStateUpdate();
            NavigationStateUpdate();
            EditingStateUpdate();
            RibbonTabsStateUpdate();
            RelationsStateUpdate();
            HousingRefBooksStateUpdate();
        }

        public void StatusBarStateUpdate()
        {
            if (tabControl.SelectedTab != null)
                toolStripLabelRecordCount.Text = "Всего записей: " + (tabControl.SelectedTab as IMenuController).GetRecordCount();
            else
                toolStripLabelRecordCount.Text = "";
        }

        public MainForm()
        {
            InitializeComponent();
            tabControl.Controls.Clear();
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
            StatesDataModel.GetInstance(toolStripProgressBar, 3);
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
            TenancyContractsDataModel.GetInstance(toolStripProgressBar, 3);
            PersonsDataModel.GetInstance(toolStripProgressBar, 3);
            KinshipsDataModel.GetInstance(toolStripProgressBar, 2);
            TenancyBuildingsAssocDataModel.GetInstance(toolStripProgressBar, 2);
            TenancyPremisesAssocDataModel.GetInstance(toolStripProgressBar, 2);
            TenancySubPremisesAssocDataModel.GetInstance(toolStripProgressBar, 2);
            ContractReasonsDataModel.GetInstance(toolStripProgressBar, 2);
            ReasonTypesDataModel.GetInstance(toolStripProgressBar, 2);
            RentTypesDataModel.GetInstance(toolStripProgressBar, 2);
            DocumentTypesDataModel.GetInstance(toolStripProgressBar, 2);
            ExecutorsDataModel.GetInstance(toolStripProgressBar, 2);
            AgreementsDataModel.GetInstance(toolStripProgressBar, 2);
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
            if (tabControl.SelectedIndex >= 0)
                (tabControl.SelectedTab as IMenuController).Close();
        }

        private void ribbonButtonTabsClose_Click(object sender, EventArgs e)
        {
            for (int i = tabControl.TabCount - 1; i >= 0; i--)
                (tabControl.Controls[i] as IMenuController).Close();
        }

        private void ribbonButtonTabCopy_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == -1)
                return;
            Registry.Viewport.Viewport viewport = (tabControl.SelectedTab as IMenuController).Duplicate();
            tabControl.Controls.Add(viewport);
            tabControl.SelectedTab = viewport; 
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeMainMenuState();
            StatusBarStateUpdate();
            ChangeViewportsSelectProprty();
        }

        private void ribbonButtonFirst_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == -1)
                return;
            (tabControl.SelectedTab as IMenuController).MoveFirst();
            NavigationStateUpdate();
        }

        private void ribbonButtonLast_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).MoveLast();
            NavigationStateUpdate();
        }

        private void ribbonButtonPrev_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).MovePrev();
            NavigationStateUpdate();
        }

        private void ribbonButtonNext_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).MoveNext();
            NavigationStateUpdate();
        }

        private void ribbonButtonSearch_Click(object sender, EventArgs e)
        {
            if (ribbonButtonSearch.Checked)
                (tabControl.SelectedTab as IMenuController).ClearSearch();
            else
                (tabControl.SelectedTab as IMenuController).SearchRecord(SearchFormType.SimpleSearchForm);
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonExtendedSearch_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).SearchRecord(SearchFormType.ExtendedSearchForm);
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonSimpleSearch_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).SearchRecord(SearchFormType.SimpleSearchForm);
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        public void SearchButtonToggle(bool value)
        {
            ribbonButtonSearch.Checked = value;
        }

        public void AddViewport(Viewport.Viewport viewport)
        {
            tabControl.TabPages.Add(viewport);
        }

        public void SwitchToViewport(Viewport.Viewport viewport)
        {
            tabControl.SelectedTab = viewport;
        }

        private void ribbonButtonOpen_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).OpenDetails();
        }

        public void TabsStateUpdate()
        {
            ribbonButtonTabCopy.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanDuplicate();
            ribbonButtonTabClose.Enabled = (tabControl.SelectedTab != null);
            ribbonButtonTabsClose.Enabled = (tabControl.TabCount > 0);  
        }

        public void RegistryStateUpdate()
        {
            //Always enable, maybe will change in future
        }

        public void NavigationStateUpdate()
        {
            ribbonButtonFirst.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanMoveFirst();
            ribbonButtonPrev.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanMovePrev();
            ribbonButtonNext.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanMoveNext();
            ribbonButtonLast.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanMoveLast();
            ribbonButtonSearch.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanSearchRecord();
            ribbonButtonSearch.Checked = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).SearchedRecords();
            ribbonButtonOpen.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanOpenDetails();
        }

        public void EditingStateUpdate()
        {
            ribbonButtonDeleteRecord.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanDeleteRecord();
            ribbonButtonInsertRecord.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanInsertRecord();
            ribbonButtonCopyRecord.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanCopyRecord();
            ribbonButtonCancel.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanCancelRecord();
            ribbonButtonSave.Enabled = (tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).CanSaveRecord();
        }

        public void RibbonTabsStateUpdate()
        {
            //Always enable, maybe will change in future
        }

        public void RelationsStateUpdate()
        {
            ribbonPanelRelations.Items.Clear();
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocBuildings())
                ribbonPanelRelations.Items.Add(ribbonButtonBuildings);
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocSubPremises())
                ribbonPanelRelations.Items.Add(ribbonButtonSubPremises);
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocPremises())
                ribbonPanelRelations.Items.Add(ribbonButtonPremises);
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocOwnerships())
                ribbonPanelRelations.Items.Add(ribbonButtonOwnerships);
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocRestrictions())
                ribbonPanelRelations.Items.Add(ribbonButtonRestrictions);
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocFundHistory())
                ribbonPanelRelations.Items.Add(ribbonButtonFundsHistory);
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocTenancyObjects())
                ribbonPanelRelations.Items.Add(ribbonButtonTenancyObjects);
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocPersons())
                ribbonPanelRelations.Items.Add(ribbonButtonPersons);
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocContractReasons())
                ribbonPanelRelations.Items.Add(ribbonButtonContractReasons);
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasAssocAgreements())
                ribbonPanelRelations.Items.Add(ribbonButtonAgreements);

            ribbon1.SuspendUpdating();
            if (ribbonPanelRelations.Items.Count == 0)
                ribbonTabGeneral.Panels.Remove(ribbonPanelRelations);
            else
                if (!ribbonTabGeneral.Panels.Contains(ribbonPanelRelations))
                    ribbonTabGeneral.Panels.Insert(2, ribbonPanelRelations);
            ribbon1.ResumeUpdating(true);
        }

        public void HousingRefBooksStateUpdate()
        {
            //Always enable, maybe will change in future
        }

        private void ribbonButtonSave_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).SaveRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
        }

        private void ribbonButtonDeleteRecord_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).DeleteRecord();
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonInsertRecord_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).InsertRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCancel_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).CancelRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCopyRecord_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).CopyRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonOrbMenuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = tabControl.TabCount - 1; i >= 0; i--)
                (tabControl.Controls[i] as IMenuController).Close();
            if (tabControl.TabCount != 0)
                e.Cancel = true;
        }

        public void ForceCloseDetachedViewports()
        {
            for (int i = tabControl.TabCount - 1; i >= 0; i--)
                if ((tabControl.Controls[i] as IMenuController).ViewportDetached())
                    (tabControl.Controls[i] as IMenuController).ForceClose();
        }

        private void ribbonButtonBuildings_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowBuildings();
        }

        private void ribbonButtonPremises_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowPremises();
        }

        private void ribbonButtonSubPremises_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowSubPremises();
        }

        private void ribbonButtonRestrictions_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowRestrictions();
        }

        private void ribbonButtonOwnership_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowOwnerships();
        }

        private void ribbonButtonFundsHistory_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowFundHistory();
        }

        private void ribbonButtonPersons_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowPersons();
        }

        private void ribbonButtonContractReasons_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowContractReasons();
        }

        private void ribbonButtonAgreements_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowAgreements();
        }

        private void ribbonButtonTenancyPremises_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowTenancyPremises();
        }

        private void ribbonButtonTenancyBuildings_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowTenancyBuildings();
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
            CreateViewport(ViewportType.ReasonTypesViewport);
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
            CreateViewport(ViewportType.Claims);
        }

        private void ribbonButtonClaimStateTypes_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.ClaimStateTypes);
        }

        private void CreateViewport(ViewportType viewportType)
        {
            Registry.Viewport.Viewport viewport = Registry.Viewport.ViewportFactory.CreateViewport(this, viewportType);
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            tabControl.Controls.Add(viewport);
            tabControl.SelectedTab = viewport;
            ChangeMainMenuState();
            StatusBarStateUpdate();
            ChangeViewportsSelectProprty();
        }

        public void SwitchToPreviousViewport()
        {
            if (tabControl.TabCount > 0)
            {
                if (tabControl.SelectedIndex != 0)
                    tabControl.SelectedIndex = tabControl.SelectedIndex - 1;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UserDomain user = UserDomain.Current;
            if (user == null)
            {
                MessageBox.Show("Пользователь не распознан или учетная запись не включена в службу каталогов Active Directory","Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            toolStripLabelHelloUser.Text = "Здравствуйте, " + user.DisplayName;
            PreLoadData();
        }

    }
}
