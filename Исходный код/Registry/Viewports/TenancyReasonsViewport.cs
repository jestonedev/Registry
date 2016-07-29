using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyReasonsViewport: EditableDataGridViewport
    {
        private DataModel _tenancyReasonTypesDataModel;

        private BindingSource _vTenancyReasonTypesDataModel;

        private TenancyReasonsViewport()
            : this(null, null)
        {
        }

        public TenancyReasonsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_tenancy_reasons")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        private static bool ValidateViewportData(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var tenancyReason = (TenancyReason) entity;
                if (tenancyReason.IdReasonType == null)
                {
                    MessageBox.Show(@"Не выбран вид основания", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (tenancyReason.ReasonNumber != null && tenancyReason.ReasonNumber.Length > 50)
                {
                    MessageBox.Show(@"Длина номера основания не может превышать 50 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (tenancyReason.ReasonDate == null)
                {
                    MessageBox.Show(@"Не заполнена дата основания", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var row = dataGridView.Rows[i];
                list.Add(EntityConverter<TenancyReason>.FromRow(row));
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var row = (DataRowView)GeneralBindingSource[i];
                list.Add(EntityConverter<TenancyReason>.FromRow(row));
            }
            return list;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = EntityDataModel<TenancyReason>.GetInstance();
            _tenancyReasonTypesDataModel = EntityDataModel<ReasonType>.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _tenancyReasonTypesDataModel.Select();

            GeneralBindingSource = new BindingSource
            {
                DataMember = "tenancy_reasons",
                Filter = StaticFilter
            };
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = DataModel.DataSet;

            _vTenancyReasonTypesDataModel = new BindingSource
            {
                DataMember = "tenancy_reason_types",
                DataSource = DataModel.DataSet,
                Sort = "reason_name"
            };

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                Text = string.Format(CultureInfo.InvariantCulture, "Основания найма №{0}", ParentRow["id_process"]);

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName,
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(EntityConverter<TenancyReason>.ToArray((DataRowView)GeneralBindingSource[i]));
            GeneralSnapshotBindingSource = new BindingSource { DataSource = GeneralSnapshot };
            AddEventHandler<EventArgs>(GeneralSnapshotBindingSource, "CurrentItemChanged", v_snapshot_tenancy_reasons_CurrentItemChanged);

            dataGridView.DataSource = GeneralSnapshotBindingSource;

            id_process.DataPropertyName = "id_process";
            id_reason.DataPropertyName = "id_reason";
            id_reason_type.DataSource = _vTenancyReasonTypesDataModel;
            id_reason_type.ValueMember = "id_reason_type";
            id_reason_type.DisplayMember = "reason_name";
            id_reason_type.DataPropertyName = "id_reason_type";
            reason_number.DataPropertyName = "reason_number";
            reason_date.DataPropertyName = "reason_date";
            reason_prepared.DataPropertyName = "reason_prepared";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            AddEventHandler<DataGridViewCellEventArgs>(dataGridView, "CellValidated", dataGridView_CellValidated);

            //События изменения данных для проверки соответствия реальным данным в модели
            AddEventHandler<DataGridViewCellEventArgs>(dataGridView, "CellValueChanged", dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", TenancyReasonsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleting", TenancyReasonsViewport_RowDeleting);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", TenancyReasonsViewport_RowDeleted);
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            if (row == null) return;
            row["id_process"] = ParentRow["id_process"];
            row["reason_date"] = DateTime.Now.Date;
            dataGridView.Rows[dataGridView.Rows.Count - 1].Cells["id_reason_type"].Value = 1;
            dataGridView.Rows[dataGridView.Rows.Count - 1].Cells["reason_number"].Selected = true;
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralSnapshotBindingSource.Position != -1) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)GeneralSnapshotBindingSource[GeneralSnapshotBindingSource.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            GeneralSnapshot.Clear();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(EntityConverter<TenancyReason>.ToArray((DataRowView)GeneralBindingSource[i]));
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
            GeneralDataModel.EditingNewRecord = true;
            var list = EntitiesListFromViewport();
            if (!ValidateViewportData(list))
            {
                SyncViews = true; 
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var tenancyReason = (TenancyReason) list[i];
                var row = GeneralDataModel.Select().Rows.Find(tenancyReason.IdReason);
                if (row == null)
                {
                    var idReason = GeneralDataModel.Insert(tenancyReason);
                    if (idReason == -1)
                    {
                        SyncViews = true; 
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_reason"] = idReason;
                    GeneralDataModel.Select().Rows.Add(EntityConverter<TenancyReason>.ToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (EntityConverter<TenancyReason>.FromRow(row) == tenancyReason)
                        continue;
                    if (GeneralDataModel.Update(tenancyReason) == -1)
                    {
                        SyncViews = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    EntityConverter<TenancyReason>.FillRow(tenancyReason, row);
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var tenancyReason = (TenancyReason)entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_reason"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_reason"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_reason"].Value == tenancyReason.IdReason))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (tenancyReason.IdReason != null && 
                    GeneralDataModel.Delete(tenancyReason.IdReason.Value) == -1)
                {
                    SyncViews = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                GeneralDataModel.Select().Rows.Find(tenancyReason.IdReason).Delete();
            }
            SyncViews = true;
            GeneralDataModel.EditingNewRecord = false;
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
            var reasonTypeIndex = _vTenancyReasonTypesDataModel.Find("id_reason_type", dataGridView.Rows[e.RowIndex].Cells["id_reason_type"].Value);
            var reasonTemplate = "";
            if (reasonTypeIndex != -1)
                reasonTemplate = ((DataRowView)_vTenancyReasonTypesDataModel[reasonTypeIndex])["reason_template"].ToString();
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
            MenuCallback.EditingStateUpdate();
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
            var rowIndex = GeneralSnapshotBindingSource.Find("id_reason", e.Row["id_reason"]);
            if (rowIndex != -1)
                ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
        }

        private void TenancyReasonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_reason", e.Row["id_reason"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_reason", e.Row["id_reason"]) != -1)
            {
                GeneralSnapshot.Rows.Add(EntityConverter<TenancyReason>.ToArray(e.Row));
            }
            else
                if (rowIndex != -1)
                {
                    var row = (DataRowView)GeneralSnapshotBindingSource[rowIndex];
                    row["id_process"] = e.Row["id_process"];
                    row["id_reason_type"] = e.Row["id_reason_type"];
                    row["reason_number"] = e.Row["reason_number"];
                    row["reason_date"] = e.Row["reason_date"];
                    row["reason_prepared"] = e.Row["reason_prepared"];
                }
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
