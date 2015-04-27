using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;
using Security;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class TenancyReasonsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        TenancyReasonsDataModel tenancy_reasons = null;
        TenancyReasonTypesDataModel tenancy_reason_types = null;
        DataTable snapshot_tenancy_reasons = new DataTable("snapshot_tenancy_reasons");
        #endregion Models

        #region Views
        BindingSource v_tenancy_reasons = null;
        BindingSource v_tenancy_reason_types = null;
        BindingSource v_snapshot_tenancy_reasons = null;
        #endregion Views
        private DataGridViewTextBoxColumn id_reason;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewComboBoxColumn id_reason_type;
        private DataGridViewTextBoxColumn reason_number;
        private DataGridViewDateTimeColumn reason_date;
        private DataGridViewTextBoxColumn reason_prepared;

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
            this.DynamicFilter = tenancyReasonsViewport.DynamicFilter;
            this.StaticFilter = tenancyReasonsViewport.StaticFilter;
            this.ParentRow = tenancyReasonsViewport.ParentRow;
            this.ParentType = tenancyReasonsViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<TenancyReason> list_from_view = TenancyReasonsFromView();
            List<TenancyReason> list_from_viewport = TenancyReasonsFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            bool founded = false;
            for (int i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (int j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
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
            foreach (TenancyReason tenancyReason in list)
            {
                if (tenancyReason.IdReasonType == null)
                {
                    MessageBox.Show("Не выбран вид основания", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (tenancyReason.ReasonNumber == null)
                {
                    MessageBox.Show("Номер основания не может быть пустым", "Ошибка", 
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
            TenancyReason tenancyReason = new TenancyReason();
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
            List<TenancyReason> list = new List<TenancyReason>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    TenancyReason cr = new TenancyReason();
                    DataGridViewRow row = dataGridView.Rows[i];
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
            List<TenancyReason> list = new List<TenancyReason>();
            for (int i = 0; i < v_tenancy_reasons.Count; i++)
            {
                TenancyReason cr = new TenancyReason();
                DataRowView row = ((DataRowView)v_tenancy_reasons[i]);
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            tenancy_reasons = TenancyReasonsDataModel.GetInstance();
            tenancy_reason_types = TenancyReasonTypesDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            tenancy_reasons.Select();
            tenancy_reason_types.Select();

            v_tenancy_reasons = new BindingSource();
            v_tenancy_reasons.DataMember = "tenancy_reasons";
            v_tenancy_reasons.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_tenancy_reasons.Filter += " AND ";
            v_tenancy_reasons.Filter += DynamicFilter;
            v_tenancy_reasons.DataSource = DataSetManager.DataSet;

            v_tenancy_reason_types = new BindingSource();
            v_tenancy_reason_types.DataMember = "tenancy_reason_types";
            v_tenancy_reason_types.DataSource = DataSetManager.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                this.Text = String.Format(CultureInfo.InvariantCulture, "Основания найма №{0}", ParentRow["id_process"]);

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < tenancy_reasons.Select().Columns.Count; i++)
                snapshot_tenancy_reasons.Columns.Add(new DataColumn(tenancy_reasons.Select().Columns[i].ColumnName,
                    tenancy_reasons.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_tenancy_reasons.Count; i++)
                snapshot_tenancy_reasons.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_reasons[i])));
            v_snapshot_tenancy_reasons = new BindingSource();
            v_snapshot_tenancy_reasons.DataSource = snapshot_tenancy_reasons;
            v_snapshot_tenancy_reasons.CurrentItemChanged += new EventHandler(v_snapshot_tenancy_reasons_CurrentItemChanged);

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
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);

            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            tenancy_reasons.Select().RowChanged += new DataRowChangeEventHandler(TenancyReasonsViewport_RowChanged);
            tenancy_reasons.Select().RowDeleting += new DataRowChangeEventHandler(TenancyReasonsViewport_RowDeleting);
            tenancy_reasons.Select().RowDeleted += TenancyReasonsViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_tenancy_reasons.AddNew();
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
            for (int i = 0; i < v_tenancy_reasons.Count; i++)
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
            List<TenancyReason> list = TenancyReasonsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = tenancy_reasons.Select().Rows.Find(((TenancyReason)list[i]).IdReason);
                if (row == null)
                {
                    int id_reason = TenancyReasonsDataModel.Insert(list[i]);
                    if (id_reason == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_reasons[i])["id_reason"] = id_reason;
                    tenancy_reasons.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_tenancy_reasons[i]));
                }
                else
                {
                    if (RowToTenancyReason(row) == list[i])
                        continue;
                    if (TenancyReasonsDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
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
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_reason"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_reason"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_reason"].Value == list[i].IdReason))
                        row_index = j;
                if (row_index == -1)
                {
                    if (TenancyReasonsDataModel.Delete(list[i].IdReason.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    tenancy_reasons.Select().Rows.Find(((TenancyReason)list[i]).IdReason).Delete();
                }
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            TenancyReasonsViewport viewport = new TenancyReasonsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения об основаниях на найм жилья в базу данных?", "Внимание",
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
            tenancy_reasons.Select().RowChanged -= new DataRowChangeEventHandler(TenancyReasonsViewport_RowChanged);
            tenancy_reasons.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyReasonsViewport_RowDeleting);
            tenancy_reasons.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyReasonsViewport_RowDeleted);
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            tenancy_reasons.Select().RowChanged -= new DataRowChangeEventHandler(TenancyReasonsViewport_RowChanged);
            tenancy_reasons.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyReasonsViewport_RowDeleting);
            tenancy_reasons.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyReasonsViewport_RowDeleted);
            base.Close();
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int reason_type_index = v_tenancy_reason_types.Find("id_reason_type", dataGridView.Rows[e.RowIndex].Cells["id_reason_type"].Value);
            string reason_template = "";
            if (reason_type_index != -1)
                reason_template = ((DataRowView)v_tenancy_reason_types[reason_type_index])["reason_template"].ToString();
            string reason_number = dataGridView.Rows[e.RowIndex].Cells["reason_number"].Value.ToString();
            DateTime? reason_date = null;
            if (dataGridView.Rows[e.RowIndex].Cells["reason_date"].Value != DBNull.Value)
                reason_date = Convert.ToDateTime(dataGridView.Rows[e.RowIndex].Cells["reason_date"].Value, CultureInfo.InvariantCulture);
            dataGridView.Rows[e.RowIndex].Cells["reason_prepared"].Value =
                reason_template.Replace("@reason_date@", reason_date == null ? "" : 
                    reason_date.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture))
                               .Replace("@reason_number@", reason_number);
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "reason_number":
                    if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "Номер основания не может быть пустым";
                    else
                        if (cell.Value.ToString().Trim().Length > 50)
                            cell.ErrorText = "Длина номера основания не может превышать 50 символов";
                        else
                            cell.ErrorText = "";
                    break;
                case "reason_date":
                    if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
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
                int row_index = v_snapshot_tenancy_reasons.Find("id_reason", e.Row["id_reason"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_tenancy_reasons[row_index]).Delete();
            }
        }

        void TenancyReasonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshot_tenancy_reasons.Find("id_reason", e.Row["id_reason"]);
            if (row_index == -1 && v_tenancy_reasons.Find("id_reason", e.Row["id_reason"]) != -1)
            {
                snapshot_tenancy_reasons.Rows.Add(new object[] { 
                        e.Row["id_reason"], 
                        e.Row["id_process"],   
                        e.Row["id_reason_type"],                 
                        e.Row["reason_number"],
                        e.Row["reason_date"],
                        e.Row["reason_prepared"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_tenancy_reasons[row_index]);
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
                DataGridViewComboBoxEditingControl editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                editingControl.DropDownClosed -= editingControl_DropDownClosed;
                editingControl.DropDownClosed += editingControl_DropDownClosed;
            }
        }

        void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            DataGridViewComboBoxEditingControl editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyReasonsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_reason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_reason_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.reason_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reason_date = new CustomControls.DataGridViewDateTimeColumn();
            this.reason_prepared = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_reason,
            this.id_process,
            this.id_reason_type,
            this.reason_number,
            this.reason_date,
            this.reason_prepared});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(843, 255);
            this.dataGridView.TabIndex = 5;
            this.dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView_EditingControlShowing);
            // 
            // id_reason
            // 
            this.id_reason.HeaderText = "Идентификатор основания";
            this.id_reason.Name = "id_reason";
            this.id_reason.ReadOnly = true;
            this.id_reason.Visible = false;
            // 
            // id_process
            // 
            this.id_process.HeaderText = "Идентификатор процесса найма";
            this.id_process.Name = "id_process";
            this.id_process.Visible = false;
            // 
            // id_reason_type
            // 
            this.id_reason_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.id_reason_type.HeaderText = "Вид основания";
            this.id_reason_type.MinimumWidth = 150;
            this.id_reason_type.Name = "id_reason_type";
            this.id_reason_type.Width = 150;
            // 
            // reason_number
            // 
            this.reason_number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.reason_number.HeaderText = "Номер основания";
            this.reason_number.MinimumWidth = 150;
            this.reason_number.Name = "reason_number";
            this.reason_number.Width = 150;
            // 
            // reason_date
            // 
            this.reason_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.reason_date.HeaderText = "Дата основания";
            this.reason_date.MinimumWidth = 150;
            this.reason_date.Name = "reason_date";
            this.reason_date.Width = 150;
            // 
            // reason_prepared
            // 
            this.reason_prepared.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGray;
            this.reason_prepared.DefaultCellStyle = dataGridViewCellStyle2;
            this.reason_prepared.FillWeight = 59.64467F;
            this.reason_prepared.HeaderText = "Результирующее основание";
            this.reason_prepared.MinimumWidth = 300;
            this.reason_prepared.Name = "reason_prepared";
            this.reason_prepared.ReadOnly = true;
            // 
            // TenancyReasonsViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(849, 261);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyReasonsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Основания найма №{0}";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
