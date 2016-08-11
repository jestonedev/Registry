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
    internal sealed partial class RestrictionListViewport : EditableDataGridViewport
    {
        private RestrictionListViewport()
            : this(null, null)
        {
        }

        public RestrictionListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new RestrictionListPresenter())
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            return ((RestrictionListPresenter)Presenter).EntitiesListFromSnapshot();
        }

        protected override List<Entity> EntitiesListFromView()
        {
            return ((RestrictionListPresenter) Presenter).EntitiesListFromView();
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

            ((RestrictionListPresenter)Presenter).AddAssocViewModelItem();

            //Перестраиваем фильтр
            ((RestrictionListPresenter)Presenter).RebuildFilter();
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
                    Text = string.Format(CultureInfo.InvariantCulture, "Реквизиты помещения №{0}", ParentRow["id_premises"]);
                    break;
                case ParentTypeEnum.Building:
                    Text = string.Format(CultureInfo.InvariantCulture, "Реквизиты здания №{0}", ParentRow["id_building"]);
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }

            DataBind();

            AddEventHandler<EventArgs>(((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource,
                "CurrentItemChanged", v_snapshot_restrictions_CurrentItemChanged);

            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", RestrictionListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleting", RestrictionListViewport_RowDeleting);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", RestrictionListViewport_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowChanged", RestrictionAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowDeleted", RestrictionAssoc_RowDeleted);

            v_snapshot_restrictions_CurrentItemChanged(null, new EventArgs());
        }

        private void DataBind()
        {
            dataGridView.DataSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;

            id_restriction.DataPropertyName = "id_restriction";
            number.DataPropertyName = "number";
            date.DataPropertyName = "date";
            description.DataPropertyName = "description";
            ViewportHelper.BindSource(id_restriction_type, Presenter.ViewModel["restriction_types"].BindingSource, "restriction_type",
                Presenter.ViewModel["restriction_types"].PrimaryKeyFirst);
        }

        public override bool CanInsertRecord()
        {
            return AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal);
        }

        public override void InsertRecord()
        {
            ((RestrictionListPresenter)Presenter).InsertRecordIntoSnapshot();
        }

        public override bool CanDeleteRecord()
        {
            return (((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource.Position != -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            ((RestrictionListPresenter)Presenter).DeleteCurrentRecordFromSnapshot();
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
            if (((RestrictionListPresenter)Presenter).ValidateRestrictionsInSnapshot())
            {
                ((RestrictionListPresenter)Presenter).SaveRecords();
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
            var viewport = new RestrictionListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        private void RestrictionAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            //Если удалена ассоциативная связь, то перестраиваем фильтр
            if (e.Action == DataRowAction.Delete)
                ((RestrictionListPresenter)Presenter).RebuildFilter();
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void RestrictionAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            ((RestrictionListPresenter)Presenter).InsertOrUpdateAssocRowIntoSnapshot(e.Row);
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void RestrictionListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            ((RestrictionListPresenter)Presenter).DeleteRowByIdFromSnapshot((int)e.Row["id_restriction"]);
        }

        private void RestrictionListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            ((RestrictionListPresenter)Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void RestrictionListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
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
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 10 ? 
                        "Длина номера реквизита не может превышать 10 символов" : "";
                    break;
                case "date":
                    cell.ErrorText = string.IsNullOrEmpty(cell.Value.ToString().Trim()) ? 
                        "Не заполнена дата" : "";
                    break;
                case "description":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 255 ? 
                        "Длина наименования реквизита не может превышать 255 символов" : "";
                    break;
            }
        }

        private void v_snapshot_restrictions_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name != "id_restriction_type") return;
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
    }
}
