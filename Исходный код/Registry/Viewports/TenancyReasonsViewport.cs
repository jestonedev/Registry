using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;

namespace Registry.Viewport
{
    internal sealed partial class TenancyReasonsViewport: EditableDataGridViewport
    {
        private DataModel tenancyReasonTypesDataModel;

        private BindingSource v_tenancyReasonTypesDataModel;

        private TenancyReasonsViewport()
            : this(null)
        {
        }

        public TenancyReasonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_tenancy_reasons");
            GeneralSnapshot.Locale = CultureInfo.InvariantCulture;
        }

        public TenancyReasonsViewport(TenancyReasonsViewport tenancyReasonsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = tenancyReasonsViewport.DynamicFilter;
            StaticFilter = tenancyReasonsViewport.StaticFilter;
            ParentRow = tenancyReasonsViewport.ParentRow;
            ParentType = tenancyReasonsViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = TenancyReasonsFromView();
            var list_from_viewport = TenancyReasonsFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            var founded = false;
            for (var i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (var j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_reason"], 
                dataRowView["id_process"], 
                dataRowView["id_reason_type"], 
                dataRowView["reason_number"], 
                dataRowView["reason_date"], 
                dataRowView["reason_prepared"]
            };
        }

        private static bool ValidateViewportData(List<TenancyReason> list)
        {
            foreach (var tenancyReason in list)
            {
                if (tenancyReason.IdReasonType == null)
                {
                    MessageBox.Show("Не выбран вид основания", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (tenancyReason.ReasonNumber != null && tenancyReason.ReasonNumber.Length > 50)
                {
                    MessageBox.Show("Длина номера основания не может превышать 50 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (tenancyReason.ReasonDate == null)
                {
                    MessageBox.Show("Не заполнена дата основания", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static TenancyReason RowToTenancyReason(DataRow row)
        {
            var tenancyReason = new TenancyReason();
            tenancyReason.IdReason = ViewportHelper.ValueOrNull<int>(row, "id_reason");
            tenancyReason.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            tenancyReason.IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type");
            tenancyReason.ReasonNumber = ViewportHelper.ValueOrNull(row, "reason_number");
            tenancyReason.ReasonDate = ViewportHelper.ValueOrNull<DateTime>(row, "reason_date");
            tenancyReason.ReasonPrepared = ViewportHelper.ValueOrNull(row, "reason_prepared");
            return tenancyReason;
        }

        private List<TenancyReason> TenancyReasonsFromViewport()
        {
            var list = new List<TenancyReason>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var cr = new TenancyReason();
                    var row = dataGridView.Rows[i];
                    cr.IdReason = ViewportHelper.ValueOrNull<int>(row, "id_reason");
                    cr.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                    cr.IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type");
                    cr.ReasonNumber = ViewportHelper.ValueOrNull(row, "reason_number");
                    cr.ReasonDate = ViewportHelper.ValueOrNull<DateTime>(row, "reason_date");
                    cr.ReasonPrepared = ViewportHelper.ValueOrNull(row, "reason_prepared");
                    list.Add(cr);
                }
            }
            return list;
        }

        private List<TenancyReason> TenancyReasonsFromView()
        {
            var list = new List<TenancyReason>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var cr = new TenancyReason();
                var row = ((DataRowView)GeneralBindingSource[i]);
                cr.IdReason = ViewportHelper.ValueOrNull<int>(row, "id_reason");
                cr.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                cr.IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type");
                cr.ReasonNumber = ViewportHelper.ValueOrNull(row, "reason_number");
                cr.ReasonDate = ViewportHelper.ValueOrNull<DateTime>(row, "reason_date");
                cr.ReasonPrepared = ViewportHelper.ValueOrNull(row, "reason_prepared");
                list.Add(cr);
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.TenancyReasonsDataModel);
            tenancyReasonTypesDataModel = DataModel.GetInstance(DataModelType.TenancyReasonTypesDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            tenancyReasonTypesDataModel.Select();

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "tenancy_reasons";
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = DataModel.DataSet;

            v_tenancyReasonTypesDataModel = new BindingSource();
            v_tenancyReasonTypesDataModel.DataMember = "tenancy_reason_types";
            v_tenancyReasonTypesDataModel.DataSource = DataModel.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                Text = string.Format(CultureInfo.InvariantCulture, "Основания найма №{0}", ParentRow["id_process"]);

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName,
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource();
            GeneralSnapshotBindingSource.DataSource = GeneralSnapshot;
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_tenancy_reasons_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;

            id_process.DataPropertyName = "id_process";
            id_reason.DataPropertyName = "id_reason";
            id_reason_type.DataSource = v_tenancyReasonTypesDataModel;
            id_reason_type.ValueMember = "id_reason_type";
            id_reason_type.DisplayMember = "reason_name";
            id_reason_type.DataPropertyName = "id_reason_type";
            reason_number.DataPropertyName = "reason_number";
            reason_date.DataPropertyName = "reason_date";
            reason_prepared.DataPropertyName = "reason_prepared";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;

            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += TenancyReasonsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += TenancyReasonsViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += TenancyReasonsViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            row["id_process"] = ParentRow["id_process"];
            row["reason_date"] = DateTime.Now.Date;
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
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            GeneralDataModel.EditingNewRecord = true;
            var list = TenancyReasonsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true; 
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = GeneralDataModel.Select().Rows.Find(list[i].IdReason);
                if (row == null)
                {
                    var id_reason = GeneralDataModel.Insert(list[i]);
                    if (id_reason == -1)
                    {
                        sync_views = true; 
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_reason"] = id_reason;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToTenancyReason(row) == list[i])
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["id_process"] = list[i].IdProcess == null ? DBNull.Value : (object)list[i].IdProcess;
                    row["id_reason_type"] = list[i].IdReasonType == null ? DBNull.Value : (object)list[i].IdReasonType;
                    row["reason_number"] = list[i].ReasonNumber == null ? DBNull.Value : (object)list[i].ReasonNumber;
                    row["reason_date"] = list[i].ReasonDate == null ? DBNull.Value : (object)list[i].ReasonDate;
                    row["reason_prepared"] = list[i].ReasonPrepared == null ? DBNull.Value : (object)list[i].ReasonPrepared;
                }
            }
            list = TenancyReasonsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_reason"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_reason"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_reason"].Value == list[i].IdReason))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (GeneralDataModel.Delete(list[i].IdReason.Value) == -1)
                {
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                GeneralDataModel.Select().Rows.Find(list[i].IdReason).Delete();
            }
            sync_views = true;
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show("Сохранить изменения об основаниях на найм жилья в базу данных?", "Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
            }
            GeneralDataModel.Select().RowChanged -= TenancyReasonsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= TenancyReasonsViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= TenancyReasonsViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= TenancyReasonsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= TenancyReasonsViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= TenancyReasonsViewport_RowDeleted;
            Close();
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var reason_type_index = v_tenancyReasonTypesDataModel.Find("id_reason_type", dataGridView.Rows[e.RowIndex].Cells["id_reason_type"].Value);
            var reason_template = "";
            if (reason_type_index != -1)
                reason_template = ((DataRowView)v_tenancyReasonTypesDataModel[reason_type_index])["reason_template"].ToString();
            var reason_number = dataGridView.Rows[e.RowIndex].Cells["reason_number"].Value.ToString();
            DateTime? reason_date = null;
            if (dataGridView.Rows[e.RowIndex].Cells["reason_date"].Value != DBNull.Value)
                reason_date = Convert.ToDateTime(dataGridView.Rows[e.RowIndex].Cells["reason_date"].Value, CultureInfo.InvariantCulture);
            reason_template = reason_template.Replace("@reason_date@", reason_date == null ? "" :
                    reason_date.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            if (string.IsNullOrEmpty(reason_number))
                reason_template = reason_template.Replace("№@reason_number@", reason_number).Replace("№ @reason_number@", reason_number);
            else
                reason_template = reason_template.Replace("@reason_number@", reason_number);
            dataGridView.Rows[e.RowIndex].Cells["reason_prepared"].Value = reason_template;
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "reason_number":
                    if (cell.Value.ToString().Trim().Length > 50)
                        cell.ErrorText = "Длина номера основания не может превышать 50 символов";
                    else
                        cell.ErrorText = "";
                    break;
                case "reason_date":
                    if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "Не заполнена дата основания";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void v_snapshot_tenancy_reasons_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void TenancyReasonsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void TenancyReasonsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = GeneralSnapshotBindingSource.Find("id_reason", e.Row["id_reason"]);
                if (row_index != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[row_index]).Delete();
            }
        }

        void TenancyReasonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = GeneralSnapshotBindingSource.Find("id_reason", e.Row["id_reason"]);
            if (row_index == -1 && GeneralBindingSource.Find("id_reason", e.Row["id_reason"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_reason"], e.Row["id_process"], e.Row["id_reason_type"], e.Row["reason_number"], e.Row["reason_date"], e.Row["reason_prepared"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[row_index]);
                    row["id_process"] = e.Row["id_process"];
                    row["id_reason_type"] = e.Row["id_reason_type"];
                    row["reason_number"] = e.Row["reason_number"];
                    row["reason_date"] = e.Row["reason_date"];
                    row["reason_prepared"] = e.Row["reason_prepared"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name == "id_reason_type")
            {
                var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                editingControl.DropDownClosed -= editingControl_DropDownClosed;
                editingControl.DropDownClosed += editingControl_DropDownClosed;
            }
        }

        void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }
    }
}
