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
    internal sealed class TenancyReasonsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_reason;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewComboBoxColumn id_reason_type;
        private DataGridViewTextBoxColumn reason_number;
        private DataGridViewDateTimeColumn reason_date;
        private DataGridViewTextBoxColumn reason_prepared;
        #endregion Components

        #region Models
        DataModel tenancy_reasons;
        DataModel tenancy_reason_types;
        DataTable snapshot_tenancy_reasons = new DataTable("snapshot_tenancy_reasons");
        #endregion Models

        #region Views
        BindingSource v_tenancy_reasons;
        BindingSource v_tenancy_reason_types;
        BindingSource v_snapshot_tenancy_reasons;
        #endregion Views

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private TenancyReasonsViewport()
            : this(null)
        {
        }

        public TenancyReasonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshot_tenancy_reasons.Locale = CultureInfo.InvariantCulture;
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
            for (var i = 0; i < v_tenancy_reasons.Count; i++)
            {
                var cr = new TenancyReason();
                var row = ((DataRowView)v_tenancy_reasons[i]);
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

        public override int GetRecordCount()
        {
            return v_snapshot_tenancy_reasons.Count;
        }

        public override void MoveFirst()
        {
            v_snapshot_tenancy_reasons.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_tenancy_reasons.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_tenancy_reasons.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_tenancy_reasons.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_tenancy_reasons.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_tenancy_reasons.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_tenancy_reasons.Position > -1) && (v_snapshot_tenancy_reasons.Position < (v_snapshot_tenancy_reasons.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_tenancy_reasons.Position > -1) && (v_snapshot_tenancy_reasons.Position < (v_snapshot_tenancy_reasons.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            tenancy_reasons = DataModel.GetInstance(DataModelType.TenancyReasonsDataModel);
            tenancy_reason_types = DataModel.GetInstance(DataModelType.TenancyReasonTypesDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            tenancy_reasons.Select();
            tenancy_reason_types.Select();

            v_tenancy_reasons = new BindingSource();
            v_tenancy_reasons.DataMember = "tenancy_reasons";
            v_tenancy_reasons.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_tenancy_reasons.Filter += " AND ";
            v_tenancy_reasons.Filter += DynamicFilter;
            v_tenancy_reasons.DataSource = DataModel.DataSet;

            v_tenancy_reason_types = new BindingSource();
            v_tenancy_reason_types.DataMember = "tenancy_reason_types";
            v_tenancy_reason_types.DataSource = DataModel.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                Text = string.Format(CultureInfo.InvariantCulture, "Основания найма №{0}", ParentRow["id_process"]);

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < tenancy_reasons.Select().Columns.Count; i++)
                snapshot_tenancy_reasons.Columns.Add(new DataColumn(tenancy_reasons.Select().Columns[i].ColumnName,
                    tenancy_reasons.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_tenancy_reasons.Count; i++)
                snapshot_tenancy_reasons.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_reasons[i])));
            v_snapshot_tenancy_reasons = new BindingSource();
            v_snapshot_tenancy_reasons.DataSource = snapshot_tenancy_reasons;
            v_snapshot_tenancy_reasons.CurrentItemChanged += v_snapshot_tenancy_reasons_CurrentItemChanged;

            dataGridView.DataSource = v_snapshot_tenancy_reasons;

            id_process.DataPropertyName = "id_process";
            id_reason.DataPropertyName = "id_reason";
            id_reason_type.DataSource = v_tenancy_reason_types;
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
            tenancy_reasons.Select().RowChanged += TenancyReasonsViewport_RowChanged;
            tenancy_reasons.Select().RowDeleting += TenancyReasonsViewport_RowDeleting;
            tenancy_reasons.Select().RowDeleted += TenancyReasonsViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)v_snapshot_tenancy_reasons.AddNew();
            row["id_process"] = ParentRow["id_process"];
            row["reason_date"] = DateTime.Now.Date;
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_tenancy_reasons.Position != -1) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_tenancy_reasons[v_snapshot_tenancy_reasons.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_tenancy_reasons.Clear();
            for (var i = 0; i < v_tenancy_reasons.Count; i++)
                snapshot_tenancy_reasons.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_reasons[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            tenancy_reasons.EditingNewRecord = true;
            var list = TenancyReasonsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true; 
                tenancy_reasons.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = tenancy_reasons.Select().Rows.Find(list[i].IdReason);
                if (row == null)
                {
                    var id_reason = tenancy_reasons.Insert(list[i]);
                    if (id_reason == -1)
                    {
                        sync_views = true; 
                        tenancy_reasons.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_reasons[i])["id_reason"] = id_reason;
                    tenancy_reasons.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_tenancy_reasons[i]));
                }
                else
                {
                    if (RowToTenancyReason(row) == list[i])
                        continue;
                    if (tenancy_reasons.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        tenancy_reasons.EditingNewRecord = false;
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
                if (tenancy_reasons.Delete(list[i].IdReason.Value) == -1)
                {
                    sync_views = true;
                    tenancy_reasons.EditingNewRecord = false;
                    return;
                }
                tenancy_reasons.Select().Rows.Find(list[i].IdReason).Delete();
            }
            sync_views = true;
            tenancy_reasons.EditingNewRecord = false;
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
            tenancy_reasons.Select().RowChanged -= TenancyReasonsViewport_RowChanged;
            tenancy_reasons.Select().RowDeleting -= TenancyReasonsViewport_RowDeleting;
            tenancy_reasons.Select().RowDeleted -= TenancyReasonsViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            tenancy_reasons.Select().RowChanged -= TenancyReasonsViewport_RowChanged;
            tenancy_reasons.Select().RowDeleting -= TenancyReasonsViewport_RowDeleting;
            tenancy_reasons.Select().RowDeleted -= TenancyReasonsViewport_RowDeleted;
            Close();
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var reason_type_index = v_tenancy_reason_types.Find("id_reason_type", dataGridView.Rows[e.RowIndex].Cells["id_reason_type"].Value);
            var reason_template = "";
            if (reason_type_index != -1)
                reason_template = ((DataRowView)v_tenancy_reason_types[reason_type_index])["reason_template"].ToString();
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
                var row_index = v_snapshot_tenancy_reasons.Find("id_reason", e.Row["id_reason"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_tenancy_reasons[row_index]).Delete();
            }
        }

        void TenancyReasonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = v_snapshot_tenancy_reasons.Find("id_reason", e.Row["id_reason"]);
            if (row_index == -1 && v_tenancy_reasons.Find("id_reason", e.Row["id_reason"]) != -1)
            {
                snapshot_tenancy_reasons.Rows.Add(e.Row["id_reason"], e.Row["id_process"], e.Row["id_reason_type"], e.Row["reason_number"], e.Row["reason_date"], e.Row["reason_prepared"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)v_snapshot_tenancy_reasons[row_index]);
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

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyReasonsViewport));
            dataGridView = new DataGridView();
            id_reason = new DataGridViewTextBoxColumn();
            id_process = new DataGridViewTextBoxColumn();
            id_reason_type = new DataGridViewComboBoxColumn();
            reason_number = new DataGridViewTextBoxColumn();
            reason_date = new DataGridViewDateTimeColumn();
            reason_prepared = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_reason, id_process, id_reason_type, reason_number, reason_date, reason_prepared);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(843, 255);
            dataGridView.TabIndex = 5;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            // 
            // id_reason
            // 
            id_reason.HeaderText = "Идентификатор основания";
            id_reason.Name = "id_reason";
            id_reason.ReadOnly = true;
            id_reason.Visible = false;
            // 
            // id_process
            // 
            id_process.HeaderText = "Идентификатор процесса найма";
            id_process.Name = "id_process";
            id_process.Visible = false;
            // 
            // id_reason_type
            // 
            id_reason_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            id_reason_type.HeaderText = "Вид основания";
            id_reason_type.MinimumWidth = 150;
            id_reason_type.Name = "id_reason_type";
            id_reason_type.Width = 150;
            // 
            // reason_number
            // 
            reason_number.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            reason_number.HeaderText = "Номер основания";
            reason_number.MinimumWidth = 150;
            reason_number.Name = "reason_number";
            reason_number.Width = 150;
            // 
            // reason_date
            // 
            reason_date.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            reason_date.HeaderText = "Дата основания";
            reason_date.MinimumWidth = 150;
            reason_date.Name = "reason_date";
            reason_date.Width = 150;
            // 
            // reason_prepared
            // 
            reason_prepared.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.BackColor = Color.LightGray;
            reason_prepared.DefaultCellStyle = dataGridViewCellStyle2;
            reason_prepared.FillWeight = 59.64467F;
            reason_prepared.HeaderText = "Результирующее основание";
            reason_prepared.MinimumWidth = 300;
            reason_prepared.Name = "reason_prepared";
            reason_prepared.ReadOnly = true;
            // 
            // TenancyReasonsViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(849, 261);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyReasonsViewport";
            Padding = new Padding(3);
            Text = "Основания найма №{0}";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
