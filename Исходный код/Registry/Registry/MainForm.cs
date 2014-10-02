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

namespace Registry
{
    public partial class MainForm : Form, IMenuCallback
    {

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

        public MainForm()
        {
            InitializeComponent();
            tabControl.Controls.Clear();
            ChangeMainMenuState(); 
        }

        private void ribbonButtonTabClose_Click(object sender, EventArgs e)
        {
            int index = tabControl.SelectedIndex;
            if (tabControl.SelectedIndex >= 0)
                (tabControl.SelectedTab as IMenuController).Close();
            ChangeMainMenuState();
        }

        private void ribbonButtonTabsClose_Click(object sender, EventArgs e)
        {
            for (int i = tabControl.TabCount - 1; i >= 0; i--)
                (tabControl.Controls[i] as IMenuController).Close();
            ChangeMainMenuState();
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
        }

        public void ViewportStateChanged()
        {
            ChangeMainMenuState();
        }

        private void ribbonButtonFirst_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == -1)
                return;
            (tabControl.SelectedTab as IMenuController).MoveFirst();
            ChangeMainMenuState();
        }

        private void ribbonButtonLast_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).MoveLast();
            ChangeMainMenuState();
        }

        private void ribbonButtonPrev_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).MovePrev();
            ChangeMainMenuState();
        }

        private void ribbonButtonNext_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).MoveNext();
            ChangeMainMenuState();
        }

        private void ribbonButtonDeleteRecord_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).DeleteRecord();
            ChangeMainMenuState();
        }

        private void ribbonButtonSearch_Click(object sender, EventArgs e)
        {
            if (ribbonButtonSearch.Checked)
                (tabControl.SelectedTab as IMenuController).ClearSearch();
            else
                (tabControl.SelectedTab as IMenuController).SearchRecord();
            ChangeMainMenuState();
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

        private void ribbonButtonInsertRecord_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).InsertRecord();
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
            if ((tabControl.SelectedTab != null) && (tabControl.SelectedTab as IMenuController).HasFundHistory())
                ribbonPanelRelations.Items.Add(ribbonButtonFundsHistory);
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
        }

        private void ribbonButtonCancel_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).CancelRecord();
        }

        private void ribbonButtonCopyRecord_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).CopyRecord();
        }

        private void ribbonButtonPremisesAssoc_Click(object sender, EventArgs e)
        {
            (tabControl.SelectedTab as IMenuController).ShowPremises();
        }

        private void ribbonOrbMenuItemBuildings_Click(object sender, EventArgs e)
        {
            Registry.Viewport.Viewport viewport = Registry.Viewport.ViewportFactory.CreateViewport(this, ViewportType.BuildingListViewport);
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            tabControl.Controls.Add(viewport);
            tabControl.SelectedTab = viewport;
            ChangeMainMenuState();
        }

        private void ribbonOrbMenuItemPremises_Click(object sender, EventArgs e)
        {
            Registry.Viewport.Viewport viewport = Registry.Viewport.ViewportFactory.CreateViewport(this, ViewportType.PremisesListViewport);
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            tabControl.Controls.Add(viewport);
            tabControl.SelectedTab = viewport;
            ChangeMainMenuState();
        }

        private void ribbonOrbMenuItemSocNaim_Click(object sender, EventArgs e)
        {

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

        public void ForceCloseDetachedViewports()
        {
            for (int i = tabControl.TabCount - 1; i >= 0; i--)
                if ((tabControl.Controls[i] as IMenuController).ViewportDetached())
                    (tabControl.Controls[i] as IMenuController).ForceClose();
        }

        private void ribbonButtonStructureTypes_Click(object sender, EventArgs e)
        {
            Registry.Viewport.Viewport viewport = Registry.Viewport.ViewportFactory.CreateViewport(this, ViewportType.StructureTypeListViewport);
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            tabControl.Controls.Add(viewport);
            tabControl.SelectedTab = viewport;
            ChangeMainMenuState();
        }

        private void ribbonButtonRestrictionTypes_Click(object sender, EventArgs e)
        {
            Registry.Viewport.Viewport viewport = Registry.Viewport.ViewportFactory.CreateViewport(this, ViewportType.RestrictionTypeListViewport);
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            tabControl.Controls.Add(viewport);
            tabControl.SelectedTab = viewport;
            ChangeMainMenuState();
        }

        private void ribbonButtonOwnershipTypes_Click(object sender, EventArgs e)
        {
            Registry.Viewport.Viewport viewport = Registry.Viewport.ViewportFactory.CreateViewport(this, ViewportType.OwnershipTypeListViewport);
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            tabControl.Controls.Add(viewport);
            tabControl.SelectedTab = viewport;
            ChangeMainMenuState();
        }

        public void SwitchToPreviousViewport()
        {
            if (tabControl.TabCount > 0)
            {
                if (tabControl.SelectedIndex != 0)
                    tabControl.SelectedIndex = tabControl.SelectedIndex - 1;
            }
        }
    }
}
