using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.Presenters;
using Registry.Viewport.ViewModels;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ExecutorsViewport : EditableDataGridViewport
    {
        private ExecutorsViewport()
            : this(null, null)
        {
        }

        public ExecutorsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new ExecutorsPresenter())
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }


        protected override List<Entity> EntitiesListFromViewport()
        {
            return ((ExecutorsPresenter)Presenter).EntitiesListFromSnapshot();
        }

        protected override List<Entity> EntitiesListFromView()
        {
            return ((ExecutorsPresenter)Presenter).EntitiesListFromView();
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
                "CurrentItemChanged", v_snapshot_executors_CurrentItemChanged);

            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", ExecutorsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleting", ExecutorsViewport_RowDeleting);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", ExecutorsViewport_RowDeleted);

            v_snapshot_executors_CurrentItemChanged(null, new EventArgs());
        }

        private void DataBind()
        {
            dataGridView.DataSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;
            id_executor.DataPropertyName = "id_executor";
            executor_name.DataPropertyName = "executor_name";
            executor_login.DataPropertyName = "executor_login";
            phone.DataPropertyName = "phone";
            is_inactive.DataPropertyName = "is_inactive";
            is_inactive.FalseValue = false;
        }

        public override bool CanInsertRecord()
        {
            return AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            ((ExecutorsPresenter)Presenter).InsertRecordIntoSnapshot();
        }

        public override bool CanDeleteRecord()
        {
            return (((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource.Position != -1) && 
                AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((ExecutorsPresenter)Presenter).DeleteCurrentRecordFromSnapshot();
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
            if (((ExecutorsPresenter)Presenter).ValidateExecutorsInSnapshot())
            {
                ((ExecutorsPresenter)Presenter).SaveRecords();
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
            var viewport = new ExecutorsViewport(this, MenuCallback);
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
                case "executor_name":
                    cell.ErrorText = string.IsNullOrEmpty(cell.Value.ToString().Trim()) ? "ФИО исполнителя не может быть пустым" : "";
                    break;
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.EditingStateUpdate();
        }

        private void ExecutorsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.EditingStateUpdate();
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
        }

        private void ExecutorsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            ((ExecutorsPresenter)Presenter).DeleteRowByIdFromSnapshot((int)e.Row["id_executor"]);
        }

        private void ExecutorsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            ((ExecutorsPresenter)Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void v_snapshot_executors_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
        }

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }
    }
}
