using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
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

        private static TenancyReason RowToTenancyReason(DataRow row)
        {
            var tenancyReason = new TenancyReason
            {
                IdReason = ViewportHelper.ValueOrNull<int>(row, "id_reason"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type"),
                ReasonNumber = ViewportHelper.ValueOrNull(row, "reason_number"),
                ReasonDate = ViewportHelper.ValueOrNull<DateTime>(row, "reason_date"),
                ReasonPrepared = ViewportHelper.ValueOrNull(row, "reason_prepared")
            };
            return tenancyReason;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
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

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
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
            _tenancyReasonTypesDataModel = DataModel.GetInstance(DataModelType.TenancyReasonTypesDataModel);
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
                DataSource = DataModel.DataSet
            };
            _vTenancyReasonTypesDataModel.Sort = "reason_name";

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                Text = string.Format(CultureInfo.InvariantCulture, "Основания найма №{0}", ParentRow["id_process"]);

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName,
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource {DataSource = GeneralSnapshot};
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_tenancy_reasons_CurrentItemChanged;

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
            var list = EntitiesListFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true; 
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
                        sync_views = true; 
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_reason"] = idReason;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToTenancyReason(row) == tenancyReason)
                        continue;
                    if (GeneralDataModel.Update(tenancyReason) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["id_process"] = tenancyReason.IdProcess == null ? DBNull.Value : (object)tenancyReason.IdProcess;
                    row["id_reason_type"] = tenancyReason.IdReasonType == null ? DBNull.Value : (object)tenancyReason.IdReasonType;
                    row["reason_number"] = tenancyReason.ReasonNumber == null ? DBNull.Value : (object)tenancyReason.ReasonNumber;
                    row["reason_date"] = tenancyReason.ReasonDate == null ? DBNull.Value : (object)tenancyReason.ReasonDate;
                    row["reason_prepared"] = tenancyReason.ReasonPrepared == null ? DBNull.Value : (object)tenancyReason.ReasonPrepared;
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
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                GeneralDataModel.Select().Rows.Find(tenancyReason.IdReason).Delete();
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
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения об основаниях на найм жилья в базу данных?", 
                    @"Внимание", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveRecord();
                        break;
                    case DialogResult.No:
                        CancelRecord();
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }
            GeneralSnapshotBindingSource.CurrentItemChanged -= v_snapshot_tenancy_reasons_CurrentItemChanged;
            dataGridView.CellValidated -= dataGridView_CellValidated;
            dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
            GeneralDataModel.Select().RowChanged -= TenancyReasonsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= TenancyReasonsViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= TenancyReasonsViewport_RowDeleted;
            base.OnClosing(e);
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
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

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
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
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_reason", e.Row["id_reason"]);
            if (rowIndex != -1)
                ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
        }

        void TenancyReasonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_reason", e.Row["id_reason"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_reason", e.Row["id_reason"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_reason"], e.Row["id_process"], e.Row["id_reason_type"], e.Row["reason_number"], e.Row["reason_date"], e.Row["reason_prepared"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
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
            if (dataGridView.CurrentCell.OwningColumn.Name != "id_reason_type") return;
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            if (editingControl == null) return;
            editingControl.DropDownClosed -= editingControl_DropDownClosed;
            editingControl.DropDownClosed += editingControl_DropDownClosed;
        }

        void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            if (editingControl != null) dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }
    }
}
