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
    internal sealed partial class TenancyReasonsViewport: EditableDataGridViewport
    {
        private TenancyReasonsViewport()
            : this(null, null)
        {
        }

        public TenancyReasonsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new TenancyReasonsPresenter())
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            return ((TenancyReasonsPresenter)Presenter).EntitiesListFromSnapshot();
        }

        protected override List<Entity> EntitiesListFromView()
        {
            return ((TenancyReasonsPresenter)Presenter).EntitiesListFromView();
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

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            ((SnapshotedViewModel)Presenter.ViewModel).InitializeSnapshot();

            GeneralSnapshot = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotDataSource;
            GeneralSnapshotBindingSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                Text = string.Format(CultureInfo.InvariantCulture, "Основания найма №{0}", ParentRow["id_process"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            DataBind();

            AddEventHandler<EventArgs>(((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource, 
                "CurrentItemChanged", v_snapshot_tenancy_reasons_CurrentItemChanged);

            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", TenancyReasonsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleting", TenancyReasonsViewport_RowDeleting);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", TenancyReasonsViewport_RowDeleted);

            v_snapshot_tenancy_reasons_CurrentItemChanged(null, new EventArgs());
        }

        private void DataBind()
        {
            dataGridView.DataSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;

            id_process.DataPropertyName = "id_process";
            id_reason.DataPropertyName = "id_reason";
            reason_number.DataPropertyName = "reason_number";
            reason_date.DataPropertyName = "reason_date";
            reason_prepared.DataPropertyName = "reason_prepared";
            ViewportHelper.BindSource(id_reason_type, Presenter.ViewModel["reason_types"].BindingSource, "reason_name",
                Presenter.ViewModel["reason_types"].PrimaryKeyFirst);
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            ((TenancyReasonsPresenter)Presenter).InsertRecordIntoSnapshot(); 
            dataGridView.Rows[dataGridView.Rows.Count - 1].Cells["id_reason_type"].Value = 1;
            dataGridView.Rows[dataGridView.Rows.Count - 1].Cells["reason_number"].Selected = true;
        }

        public override bool CanDeleteRecord()
        {
            return (((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource.Position != -1) && 
                AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            ((TenancyReasonsPresenter)Presenter).DeleteCurrentRecordFromSnapshot();
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
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            SyncViews = false;
            dataGridView.EndEdit();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            if (((TenancyReasonsPresenter)Presenter).ValidateTenancyReasonsInSnapshot())
            {
                ((TenancyReasonsPresenter)Presenter).SaveRecords();
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
            var viewport = new TenancyReasonsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Selected || dataGridView.Columns[e.ColumnIndex].Name == "reason_prepared") return;
            var reasonTypeIndex = Presenter.ViewModel["reason_types"].BindingSource.Find("id_reason_type",
                dataGridView.Rows[e.RowIndex].Cells["id_reason_type"].Value);
            var reasonTemplate = "";
            if (reasonTypeIndex != -1)
                reasonTemplate = ((DataRowView)Presenter.ViewModel["reason_types"].BindingSource[reasonTypeIndex])["reason_template"].ToString();
            var reasonNumber = dataGridView.Rows[e.RowIndex].Cells["reason_number"].Value.ToString();
            DateTime? reasonDate = null;
            if (dataGridView.Rows[e.RowIndex].Cells["reason_date"].Value != DBNull.Value)
                reasonDate = Convert.ToDateTime(dataGridView.Rows[e.RowIndex].Cells["reason_date"].Value, CultureInfo.InvariantCulture);
            reasonTemplate = reasonTemplate.Replace("@reason_date@", reasonDate == null ? "" :
                reasonDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            reasonTemplate = string.IsNullOrEmpty(reasonNumber) ?
                reasonTemplate.Replace("№@reason_number@", reasonNumber).Replace("№ @reason_number@", reasonNumber) :
                reasonTemplate.Replace("@reason_number@", reasonNumber);
            dataGridView.Rows[e.RowIndex].Cells["reason_prepared"].Value = reasonTemplate;
            MenuCallback.EditingStateUpdate();
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
                case "reason_number":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 50 ? 
                        "Длина номера основания не может превышать 50 символов" : "";
                    break;
                case "reason_date":
                    cell.ErrorText = string.IsNullOrEmpty(cell.Value.ToString().Trim()) ? 
                        "Не заполнена дата основания" : "";
                    break;
            }
        }

        private void v_snapshot_tenancy_reasons_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
        }

        private void TenancyReasonsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void TenancyReasonsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            ((TenancyReasonsPresenter)Presenter).DeleteRowByIdFromSnapshot((int)e.Row["id_reason"]);
        }

        private void TenancyReasonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            ((TenancyReasonsPresenter)Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name != "id_reason_type") return;
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
