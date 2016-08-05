using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.Presenters;
using Registry.Viewport.ViewModels;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class OwnershipListViewport : EditableDataGridViewport
    {
        private OwnershipListViewport()
            : this(null, null)
        {
        }

        public OwnershipListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new OwnershipListPresenter())
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            return ((OwnershipListPresenter)Presenter).EntitiesListFromSnapshot();
        }

        protected override List<Entity> EntitiesListFromView()
        {
            return ((OwnershipListPresenter)Presenter).EntitiesListFromView();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;
            Presenter.ParentRow = ParentRow;
            Presenter.ParentType = ParentType;

            ((OwnershipListPresenter)Presenter).AddAssocViewModelItem();

            //Перестраиваем фильтр v_ownerships_rights.Filter
            ((OwnershipListPresenter)Presenter).RebuildFilter();

            ((SnapshotedViewModel)Presenter.ViewModel).InitializeSnapshot();

            GeneralSnapshot = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotDataSource;
            GeneralSnapshotBindingSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;


            if (ParentRow == null)
            {
                throw new ViewportException("Не указан родительский объект");
            }

            switch (ParentType)
            {
                case ParentTypeEnum.Premises:
                    Text = string.Format(CultureInfo.InvariantCulture, "Ограничения помещения №{0}", ParentRow["id_premises"]);
                    break;
                case ParentTypeEnum.Building:
                    Text = string.Format(CultureInfo.InvariantCulture, "Ограничения здания №{0}", ParentRow["id_building"]);
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }

            DataBind();


            AddEventHandler<EventArgs>(((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource,
                "CurrentItemChanged", v_snapshot_ownerships_rights_CurrentItemChanged);

            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", OwnershipListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleting", OwnershipListViewport_RowDeleting);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", OwnershipListViewport_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowChanged", OwnershipAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowDeleted", OwnershipAssoc_RowDeleted);

            v_snapshot_ownerships_rights_CurrentItemChanged(null, new EventArgs());
        }

        private void DataBind()
        {
            dataGridView.DataSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;

            id_ownership_right.DataPropertyName = "id_ownership_right";
            number.DataPropertyName = "number";
            date.DataPropertyName = "date";
            description.DataPropertyName = "description";
            ViewportHelper.BindSource(id_ownership_right_type, Presenter.ViewModel["ownership_right_types"].BindingSource, "ownership_right_type",
                Presenter.ViewModel["ownership_right_types"].PrimaryKeyFirst);
        }

        public override bool CanInsertRecord()
        {
            return AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal);
        }

        public override void InsertRecord()
        {
            ((OwnershipListPresenter)Presenter).InsertRecordIntoSnapshot();
        }

        public override bool CanDeleteRecord()
        {
            return (((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource.Position != -1) && 
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            ((OwnershipListPresenter)Presenter).DeleteCurrentRecordFromSnapshot();
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
            return SnapshotHasChanges() && 
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void SaveRecord()
        {
            SyncViews = false;
            dataGridView.EndEdit();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            if (((OwnershipListPresenter)Presenter).ValidateOwnershipsInSnapshot())
            {
                ((OwnershipListPresenter)Presenter).SaveRecords();
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
            var viewport = new OwnershipListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
            }
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
                case "number":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 20 ? 
                        "Длина номера основания не может превышать 20 символов" : "";
                    break;
                case "date":
                    cell.ErrorText = string.IsNullOrEmpty(cell.Value.ToString().Trim()) ? 
                        "Не заполнена дата начала действия ограничения, установленного в отношении муниципальной собственности" : "";
                    break;
                case "description":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 255 ? 
                        "Длина наименования ограничения не может превышать 255 символов" : "";
                    break;
            }
        }

        private void OwnershipAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            ((OwnershipListPresenter)Presenter).InsertOrUpdateAssocRowIntoSnapshot(e.Row);
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void OwnershipAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            //Если удалена ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            if (e.Action == DataRowAction.Delete)
                ((OwnershipListPresenter)Presenter).RebuildFilter();
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void OwnershipListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void OwnershipListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            ((OwnershipListPresenter)Presenter).DeleteRowByIdFromSnapshot((int)e.Row["id_ownership_right"]);
        }

        private void OwnershipListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            ((OwnershipListPresenter)Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void v_snapshot_ownerships_rights_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name != "id_ownership_right_type") return;
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            if (editingControl == null) return;
            editingControl.DropDownClosed -= editingControl_DropDownClosed;
            editingControl.DropDownClosed += editingControl_DropDownClosed;
        }

        private void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            if (editingControl != null) dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
