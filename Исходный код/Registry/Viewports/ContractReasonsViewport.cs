using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;

namespace Registry.Viewport
{
    internal sealed class ContractReasonsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_contract_reason = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_id_contract = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_reason_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_reason_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DateGridViewDateTimeColumn field_reason_date = new CustomControls.DateGridViewDateTimeColumn();
        private DataGridViewTextBoxColumn field_contract_reason_prepared = new DataGridViewTextBoxColumn();
        #endregion Components

        //Modeles
        ContractReasonsDataModel contract_reasons = null;
        ReasonTypesDataModel reason_types = null;
        DataTable snapshot_contract_reasons = new DataTable("snapshot_contract_reasons");

        //Views
        BindingSource v_contract_reasons = null;
        BindingSource v_reason_types = null;
        BindingSource v_snapshot_contract_reasons = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public ContractReasonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageContractReasons";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Основания найма №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public ContractReasonsViewport(ContractReasonsViewport contractReasonsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = contractReasonsViewport.DynamicFilter;
            this.StaticFilter = contractReasonsViewport.StaticFilter;
            this.ParentRow = contractReasonsViewport.ParentRow;
            this.ParentType = contractReasonsViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            contract_reasons = ContractReasonsDataModel.GetInstance();
            reason_types = ReasonTypesDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            contract_reasons.Select();
            reason_types.Select();

            v_contract_reasons = new BindingSource();
            v_contract_reasons.DataMember = "contract_reasons";
            v_contract_reasons.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_contract_reasons.Filter += " AND ";
            v_contract_reasons.Filter += DynamicFilter;
            v_contract_reasons.DataSource = DataSetManager.GetDataSet();

            v_reason_types = new BindingSource();
            v_reason_types.DataMember = "reason_types";
            v_reason_types.DataSource = DataSetManager.GetDataSet();

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                this.Text = String.Format("Основания найма №{0}", ParentRow["id_contract"]);

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < contract_reasons.Select().Columns.Count; i++)
                snapshot_contract_reasons.Columns.Add(new DataColumn(contract_reasons.Select().Columns[i].ColumnName,
                    contract_reasons.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_contract_reasons.Count; i++)
                snapshot_contract_reasons.Rows.Add(DataRowViewToArray(((DataRowView)v_contract_reasons[i])));
            v_snapshot_contract_reasons = new BindingSource();
            v_snapshot_contract_reasons.DataSource = snapshot_contract_reasons;
            v_snapshot_contract_reasons.CurrentItemChanged += new EventHandler(v_snapshot_contract_reasons_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_contract_reasons;

            field_id_contract_reason.DataPropertyName = "id_contract_reason";
            field_id_contract.DataPropertyName = "id_contract";
            field_id_reason_type.DataPropertyName = "id_reason_type";
            field_id_reason_type.DataSource = v_reason_types;
            field_id_reason_type.ValueMember = "id_reason_type";
            field_id_reason_type.DisplayMember = "reason_name";
            field_reason_number.DataPropertyName = "reason_number";
            field_reason_date.DataPropertyName = "reason_date";
            field_contract_reason_prepared.DataPropertyName = "contract_reason_prepared";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);

            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            contract_reasons.Select().RowChanged += new DataRowChangeEventHandler(ContractReasonsViewport_RowChanged);
            contract_reasons.Select().RowDeleting += new DataRowChangeEventHandler(ContractReasonsViewport_RowDeleting);
        }

        void ContractReasonsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_contract_reasons.Find("id_contract", e.Row["id_contract"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_contract_reasons[row_index]).Delete();
            }
        }

        void ContractReasonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_contract_reasons.Find("id_contract_reason", e.Row["id_contract_reason"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_contract_reasons[row_index]);
                    row["id_contract"] = e.Row["id_contract"];
                    row["id_reason_type"] = e.Row["id_reason_type"];
                    row["reason_number"] = e.Row["reason_number"];
                    row["reason_date"] = e.Row["reason_date"];
                    row["contract_reason_prepared"] = e.Row["contract_reason_prepared"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    int row_index = v_contract_reasons.Find("id_contract_reason", e.Row["id_contract_reason"]);
                    if (row_index != -1)
                        snapshot_contract_reasons.Rows.Add(new object[] { 
                            e.Row["id_contract"], 
                            e.Row["id_reason_type"],   
                            e.Row["reason_number"],                 
                            e.Row["reason_date"],
                            e.Row["contract_reason_prepared"]
                        });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<ContractReason> list_from_view = ContractReasonsFromView();
            List<ContractReason> list_from_viewport = ContractReasonsFromViewport();
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

        private List<ContractReason> ContractReasonsFromViewport()
        {
            List<ContractReason> list = new List<ContractReason>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    ContractReason cr = new ContractReason();
                    DataGridViewRow row = dataGridView.Rows[i];
                    cr.id_contract_reason = row.Cells["id_contract_reason"].Value == DBNull.Value ? null :
                        (int?)Convert.ToInt32(row.Cells["id_contract_reason"].Value);
                    cr.id_contract = row.Cells["id_contract"].Value == DBNull.Value ? null :
                        (int?)Convert.ToInt32(row.Cells["id_contract"].Value);
                    cr.id_reason_type = row.Cells["id_reason_type"].Value == DBNull.Value ? null :
                        (int?)Convert.ToInt32(row.Cells["id_reason_type"].Value);
                    cr.reason_number = row.Cells["reason_number"].Value == DBNull.Value ? null : 
                        row.Cells["reason_number"].Value.ToString();
                    cr.reason_date = row.Cells["reason_date"].Value == DBNull.Value ? null : 
                        (DateTime?)Convert.ToDateTime(row.Cells["reason_date"].Value);
                    cr.contract_reason_prepared = row.Cells["contract_reason_prepared"].Value == DBNull.Value ? null : 
                        row.Cells["contract_reason_prepared"].Value.ToString();
                    list.Add(cr);
                }
            }
            return list;
        }

        private List<ContractReason> ContractReasonsFromView()
        {
            List<ContractReason> list = new List<ContractReason>();
            for (int i = 0; i < v_contract_reasons.Count; i++)
            {
                ContractReason cr = new ContractReason();
                DataRowView row = ((DataRowView)v_contract_reasons[i]);
                cr.id_contract_reason = row["id_contract_reason"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_contract_reason"]);
                cr.id_contract = row["id_contract"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_contract"]);
                cr.id_reason_type = row["id_reason_type"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_reason_type"]);
                cr.reason_number = row["reason_number"] == DBNull.Value ? null : row["reason_number"].ToString();
                cr.reason_date = row["reason_date"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["reason_date"]);
                cr.contract_reason_prepared = row["contract_reason_prepared"] == DBNull.Value ? null : row["contract_reason_prepared"].ToString();
                list.Add(cr);
            }
            return list;
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int reason_type_index = v_reason_types.Find("id_reason_type", dataGridView.Rows[e.RowIndex].Cells["id_reason_type"].Value);
            string reason_template = "";
            if (reason_type_index != -1)
                reason_template = ((DataRowView)v_reason_types[reason_type_index])["reason_template"].ToString();
            string reason_number = dataGridView.Rows[e.RowIndex].Cells["reason_number"].Value.ToString();
            DateTime? reason_date = null;
            if (dataGridView.Rows[e.RowIndex].Cells["reason_date"].Value != DBNull.Value)
                reason_date = Convert.ToDateTime(dataGridView.Rows[e.RowIndex].Cells["reason_date"].Value);
            dataGridView.Rows[e.RowIndex].Cells["contract_reason_prepared"].Value = 
                reason_template.Replace("@reason_date@", reason_date == null ? "" : reason_date.Value.ToString("dd.MM.yyyy"))
                               .Replace("@reason_number@", reason_number);
            menuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "reason_number") &&
                (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 50))
            {
                MessageBox.Show("Длина номера основания не может превышать 50 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 50);
            }
        }

        void v_snapshot_contract_reasons_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        void v_snapshot_restrictions_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        public override void MoveFirst()
        {
            v_snapshot_contract_reasons.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_contract_reasons.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_contract_reasons.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_contract_reasons.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_contract_reasons.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_contract_reasons.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_contract_reasons.Position > -1) && (v_snapshot_contract_reasons.Position < (v_snapshot_contract_reasons.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_contract_reasons.Position > -1) && (v_snapshot_contract_reasons.Position < (v_snapshot_contract_reasons.Count - 1));
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_contract_reason"], 
                dataRowView["id_contract"], 
                dataRowView["id_reason_type"], 
                dataRowView["reason_number"], 
                dataRowView["reason_date"], 
                dataRowView["contract_reason_prepared"]
            };
        }

        public override bool CanInsertRecord()
        {
            return true;
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_contract_reasons.AddNew();
            row["id_contract"] = ParentRow["id_contract"];
            row["reason_date"] = DateTime.Now.Date;
            row.EndEdit();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения об основаниях на найм жилья в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            contract_reasons.Select().RowChanged -= new DataRowChangeEventHandler(ContractReasonsViewport_RowChanged);
            contract_reasons.Select().RowDeleting -= new DataRowChangeEventHandler(ContractReasonsViewport_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_contract_reasons.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_contract_reasons[v_snapshot_contract_reasons.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_contract_reasons.Clear();
            for (int i = 0; i < v_contract_reasons.Count; i++)
                snapshot_contract_reasons.Rows.Add(DataRowViewToArray(((DataRowView)v_contract_reasons[i])));
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        private bool ValidateViewportData(List<ContractReason> list)
        {
            foreach (ContractReason contractReason in list)
            {
                if (contractReason.id_reason_type == null)
                {
                    MessageBox.Show("Не выбран вид основания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (contractReason.reason_number == null)
                {
                    MessageBox.Show("Не заполнен номер основания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (contractReason.reason_date == null)
                {
                    MessageBox.Show("Не заполнена дата основания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<ContractReason> list = ContractReasonsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = contract_reasons.Select().Rows.Find(((ContractReason)list[i]).id_contract_reason);
                if (row == null)
                {
                    int id_contract_reason = contract_reasons.Insert(list[i]);
                    if (id_contract_reason == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_contract_reasons[i])["id_contract_reason"] = id_contract_reason;
                    contract_reasons.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_contract_reasons[i]));
                }
                else
                {
                    if (RowToContractReason(row) == list[i])
                        continue;
                    if (contract_reasons.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["id_contract"] = list[i].id_contract == null ? DBNull.Value : (object)list[i].id_contract;
                    row["id_reason_type"] = list[i].id_reason_type == null ? DBNull.Value : (object)list[i].id_reason_type;
                    row["reason_number"] = list[i].reason_number == null ? DBNull.Value : (object)list[i].reason_number;
                    row["reason_date"] = list[i].reason_date == null ? DBNull.Value : (object)list[i].reason_date;
                    row["contract_reason_prepared"] = list[i].contract_reason_prepared == null ? DBNull.Value : (object)list[i].contract_reason_prepared;
                }
            }
            list = ContractReasonsFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_contract_reason"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_contract_reason"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_contract_reason"].Value == list[i].id_contract_reason))
                        row_index = j;
                if (row_index == -1)
                {
                    if (contract_reasons.Delete(list[i].id_contract_reason.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    contract_reasons.Select().Rows.Find(((ContractReason)list[i]).id_contract_reason).Delete();
                }
            }
            sync_views = true;
        }

        private ContractReason RowToContractReason(DataRow row)
        {
            ContractReason contractReason = new ContractReason();
            contractReason.id_contract_reason = row["id_contract_reason"] == DBNull.Value ? null :
                (int?)Convert.ToInt32(row["id_contract_reason"]);
            contractReason.id_contract = row["id_contract"] == DBNull.Value ? null :
                (int?)Convert.ToInt32(row["id_contract"]);
            contractReason.id_reason_type = row["id_reason_type"] == DBNull.Value ? null :
                (int?)Convert.ToInt32(row["id_reason_type"]);
            contractReason.reason_number = row["reason_number"] == DBNull.Value ? null : row["reason_number"].ToString();
            contractReason.reason_date = row["reason_date"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["reason_date"]);
            contractReason.contract_reason_prepared = row["contract_reason_prepared"] == DBNull.Value ? null : row["contract_reason_prepared"].ToString();
            return contractReason;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            ContractReasonsViewport viewport = new ContractReasonsViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        public override void ForceClose()
        {
            contract_reasons.Select().RowChanged -= new DataRowChangeEventHandler(ContractReasonsViewport_RowChanged);
            contract_reasons.Select().RowDeleting -= new DataRowChangeEventHandler(ContractReasonsViewport_RowDeleting);
            base.Close();
        }

        public override bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        public override int GetRecordCount()
        {
            return v_snapshot_contract_reasons.Count;
        }

        private void ConstructViewport()
        {
            this.Controls.Add(dataGridView);
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = 
                new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_contract_reason,
            this.field_id_contract,
            this.field_id_reason_type,
            this.field_reason_number,
            this.field_reason_date,
            this.field_contract_reason_prepared});         
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Size = new System.Drawing.Size(990, 537);
            this.dataGridView.TabIndex = 5;
            this.dataGridView.AutoGenerateColumns = false;
            // 
            // field_id_contract_reason
            // 
            this.field_id_contract_reason.HeaderText = "Идентификатор основания";
            this.field_id_contract_reason.Name = "id_contract_reason";
            this.field_id_contract_reason.ReadOnly = true;
            this.field_id_contract_reason.Visible = false;
            // 
            // field_id_contract
            // 
            this.field_id_contract.HeaderText = "Идентификатор процесса найма";
            this.field_id_contract.Name = "id_contract";
            this.field_id_contract.Visible = false;
            // 
            // field_id_reason_type
            // 
            this.field_id_reason_type.HeaderText = "Вид основания";
            this.field_id_reason_type.Name = "id_reason_type";
            // 
            // field_reason_number
            // 
            this.field_reason_number.HeaderText = "Номер основания";
            this.field_reason_number.Name = "reason_number";
            // 
            // field_reason_date
            // 
            this.field_reason_date.HeaderText = "Дата основания";
            this.field_reason_date.Name = "reason_date";
            // 
            // field_contract_reason_prepared
            // 
            this.field_contract_reason_prepared.HeaderText = "Результирующее основание";
            this.field_contract_reason_prepared.Name = "contract_reason_prepared";
            this.field_contract_reason_prepared.ReadOnly = true;
            this.field_contract_reason_prepared.FillWeight = 200;
            this.field_contract_reason_prepared.DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLight;

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
