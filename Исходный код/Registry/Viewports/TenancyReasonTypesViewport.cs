using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.Presenters;
using Registry.Viewport.ViewModels;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyReasonTypesViewport: EditableDataGridViewport
    {
        private TenancyReasonTypesViewport()
            : this(null, null)
        {
        }

        public TenancyReasonTypesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new TenancyReasonTypesPresenter())
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            return ((TenancyReasonTypesPresenter)Presenter).EntitiesListFromSnapshot();
        }

        protected override List<Entity> EntitiesListFromView()
        {
            return ((TenancyReasonTypesPresenter)Presenter).EntitiesListFromView();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;

            ((SnapshotedViewModel)Presenter.ViewModel).InitializeSnapshot();

            GeneralSnapshot = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotDataSource;
            GeneralSnapshotBindingSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;

            DataBind();

            AddEventHandler<EventArgs>(((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource, 
                "CurrentItemChanged", v_snapshot_reason_types_CurrentItemChanged);

            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", ReasonTypesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleting", ReasonTypesViewport_RowDeleting);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", ReasonTypesViewport_RowDeleted);
        }

        private void DataBind()
        {
            dataGridView.DataSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;
            id_reason_type.DataPropertyName = "id_reason_type";
            reason_name.DataPropertyName = "reason_name";
            reason_template.DataPropertyName = "reason_template";
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            ((TenancyReasonTypesPresenter)Presenter).InsertRecordIntoSnapshot();
        }

        public override bool CanDeleteRecord()
        {
            return (((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource.Position != -1) && 
                AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((TenancyReasonTypesPresenter)Presenter).DeleteCurrentRecordFromSnapshot();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            ((SnapshotedViewModel)Presenter.ViewModel).LoadSnapshot();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {

            SyncViews = false;
            dataGridView.EndEdit();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            if (((TenancyReasonTypesPresenter)Presenter).ValidateReasonTypesInSnapshot())
            {
                ((TenancyReasonTypesPresenter)Presenter).SaveRecords();
                MenuCallback.EditingStateUpdate();
            }
            Presenter.ViewModel["general"].Model.EditingNewRecord = false;
            SyncViews = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new TenancyReasonTypesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.Value == null)
            {
                return;
            }
            switch (cell.OwningColumn.Name)
            {
                case "reason_name":
                    if (cell.Value.ToString().Trim().Length > 150)
                        cell.ErrorText = "Длина названия вида основания не может превышать 150 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название вида основания не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
                case "reason_template":
                    if (cell.Value.ToString().Length > 4000)
                        cell.ErrorText = "Длина шаблона вида основания не может превышать 4000 символов";
                    else
                    if (!(Regex.IsMatch(cell.Value.ToString(), "@reason_number@") &&
                         Regex.IsMatch(cell.Value.ToString(), "@reason_date@")))
                        cell.ErrorText = "Шаблон основания имеет неверный формат. В шаблоне должны быть указаны номер (в виде шаблона @reason_number@) и" +
                            " дата (в виде шаблона @reason_date@) основания";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.EditingStateUpdate();
        }

        private void ReasonTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.EditingStateUpdate();
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
        }

        private void ReasonTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            ((TenancyReasonTypesPresenter)Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
        }

        private void ReasonTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            ((TenancyReasonTypesPresenter)Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void v_snapshot_reason_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
        }
    }
}
