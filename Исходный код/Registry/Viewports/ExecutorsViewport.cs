using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport
{
    internal sealed class ExecutorsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_executor = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_executor_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_executor_login = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        //Modeles
        ExecutorsDataModel executors = null;
        DataTable snapshot_executors = new DataTable("snapshot_executors");

        //Views
        BindingSource v_executors = null;
        BindingSource v_snapshot_executors = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public ExecutorsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageExecutors";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Исполнители";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public ExecutorsViewport(ExecutorsViewport executorsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = executorsViewport.DynamicFilter;
            this.StaticFilter = executorsViewport.StaticFilter;
            this.ParentRow = executorsViewport.ParentRow;
            this.ParentType = executorsViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            executors = ExecutorsDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            executors.Select();

            v_executors = new BindingSource();
            v_executors.DataMember = "executors";
            v_executors.DataSource = DataSetManager.GetDataSet();

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < executors.Select().Columns.Count; i++)
                snapshot_executors.Columns.Add(new DataColumn(
                    executors.Select().Columns[i].ColumnName, executors.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_executors.Count; i++)
                snapshot_executors.Rows.Add(DataRowViewToArray(((DataRowView)v_executors[i])));
            v_snapshot_executors = new BindingSource();
            v_snapshot_executors.DataSource = snapshot_executors;
            v_snapshot_executors.CurrentItemChanged += new EventHandler(v_snapshot_executors_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_executors;
            field_id_executor.DataPropertyName = "id_executor";
            field_executor_name.DataPropertyName = "executor_name";
            field_executor_login.DataPropertyName = "executor_login";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            executors.Select().RowChanged += new DataRowChangeEventHandler(ExecutorsViewport_RowChanged);
            executors.Select().RowDeleting += new DataRowChangeEventHandler(ExecutorsViewport_RowDeleting);
        }

        public override void MoveFirst()
        {
            v_snapshot_executors.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_executors.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_executors.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_executors.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_executors.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_executors.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_executors.Position > -1) && (v_snapshot_executors.Position < (v_snapshot_executors.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_executors.Position > -1) && (v_snapshot_executors.Position < (v_snapshot_executors.Count - 1));
        }

        void ExecutorsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_executors.Find("id_executor", e.Row["id_executor"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_executors[row_index]).Delete();
            }
        }

        void ExecutorsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_executors.Find("id_executor", e.Row["id_executor"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_executors[row_index]);
                    row["executor_name"] = e.Row["executor_name"];
                    row["executor_login"] = e.Row["executor_login"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_executors.Rows.Add(new object[] { 
                        e.Row["id_executor"], 
                        e.Row["executor_name"], 
                        e.Row["executor_login"]
                    });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<Executor> list_from_view = ExecutorsFromView();
            List<Executor> list_from_viewport = ExecutorsFromViewport();
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

        private List<Executor> ExecutorsFromViewport()
        {
            List<Executor> list = new List<Executor>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    Executor e = new Executor();
                    DataGridViewRow row = dataGridView.Rows[i];
                    e.id_executor = row.Cells["id_executor"].Value == DBNull.Value ? null : (int?)Convert.ToInt32(row.Cells["id_executor"].Value);
                    e.executor_name = row.Cells["executor_name"].Value == DBNull.Value ? null : row.Cells["executor_name"].Value.ToString();
                    e.executor_login = row.Cells["executor_login"].Value == DBNull.Value ? null : row.Cells["executor_login"].Value.ToString();
                    list.Add(e);
                }
            }
            return list;
        }

        private List<Executor> ExecutorsFromView()
        {
            List<Executor> list = new List<Executor>();
            for (int i = 0; i < v_executors.Count; i++)
            {
                Executor e = new Executor();
                DataRowView row = ((DataRowView)v_executors[i]);
                e.id_executor = row["id_executor"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_executor"]);
                e.executor_name = row["executor_name"] == DBNull.Value ? null : row["executor_name"].ToString();
                e.executor_login = row["executor_login"] == DBNull.Value ? null : row["executor_login"].ToString();
                list.Add(e);
            }
            return list;
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            menuCallback.EditingStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return true;
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_executors.AddNew();
            row.EndEdit();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения о виде основания в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            executors.Select().RowChanged -= new DataRowChangeEventHandler(ExecutorsViewport_RowChanged);
            executors.Select().RowDeleting -= new DataRowChangeEventHandler(ExecutorsViewport_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_executors.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_executors[v_snapshot_executors.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_executors.Clear();
            for (int i = 0; i < v_executors.Count; i++)
                snapshot_executors.Rows.Add(DataRowViewToArray(((DataRowView)v_executors[i])));
        }

        private bool ValidateViewportData(List<Executor> list)
        {
            foreach (Executor executor in list)
            {
                if (executor.executor_name == null)
                {
                    MessageBox.Show("ФИО исполнителя не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if ((executor.executor_login != null) && 
                    (executor.executor_login.Trim() != "") &&
                    (UserDomain.GetUserDomain(executor.executor_login) == null))
                {
                    MessageBox.Show("Не удалось найти пользователя с указанным логином", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<Executor> list = ExecutorsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = executors.Select().Rows.Find(((Executor)list[i]).id_executor);
                if (row == null)
                {
                    int id_executor = executors.Insert(list[i]);
                    if (id_executor == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_executors[i])["id_executor"] = id_executor;
                    executors.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_executors[i]));
                }
                else
                {

                    if (RowToExecutor(row) == list[i])
                        continue;
                    if (executors.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["executor_name"] = list[i].executor_name == null ? DBNull.Value : (object)list[i].executor_name;
                    row["executor_login"] = list[i].executor_login == null ? DBNull.Value : (object)list[i].executor_login;
                }
            }
            list = ExecutorsFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_executor"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_executor"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_executor"].Value == list[i].id_executor))
                        row_index = j;
                if (row_index == -1)
                {
                    if (executors.Delete(list[i].id_executor.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    executors.Select().Rows.Find(((Executor)list[i]).id_executor).Delete();
                }
            }
            sync_views = true;
        }

        private Executor RowToExecutor(DataRow row)
        {
            Executor executor = new Executor();
            executor.id_executor = row["id_executor"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_executor"]);
            executor.executor_name = row["executor_name"] == DBNull.Value ? null : row["executor_name"].ToString();
            executor.executor_login = row["executor_login"] == DBNull.Value ? null : row["executor_login"].ToString();
            return executor;
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.OwningColumn.Name == "executor_name")
            {
                if (cell.Value.ToString().Length > 255)
                {
                    MessageBox.Show("Длина ФИО исполнителя не может превышать 255 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cell.Value = cell.Value.ToString().Substring(0, 255);
                }
                if (cell.Value.ToString().Trim().Length == 0)
                {
                    cell.ErrorText = "ФИО исполнителя не может быть пустым";
                }
                else
                    cell.ErrorText = "";
            }
            if (cell.OwningColumn.Name == "executor_login")
            {
                if (cell.Value.ToString().Length > 255)
                {
                    MessageBox.Show("Длина логина исполнителя не может превышать 255 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cell.Value = cell.Value.ToString().Substring(0, 255);
                }
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            ExecutorsViewport viewport = new ExecutorsViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_executors.Count;
        }

        void v_snapshot_executors_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_executor"], 
                dataRowView["executor_name"],
                dataRowView["executor_login"]
            };
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
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_executor,
            this.field_executor_name,
            this.field_executor_login});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(990, 537);
            this.dataGridView.TabIndex = 8;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.AutoGenerateColumns = false;
            // 
            // field_id_executor
            // 
            this.field_id_executor.Frozen = true;
            this.field_id_executor.HeaderText = "Идентификатор исполнителя";
            this.field_id_executor.Name = "id_executor";
            this.field_id_executor.ReadOnly = true;
            this.field_id_executor.Visible = false;
            // 
            // field_executor_name
            // 
            this.field_executor_name.HeaderText = "ФИО исполнителя";
            this.field_executor_name.MinimumWidth = 100;
            this.field_executor_name.Name = "executor_name";
            // 
            // field_executor_login
            // 
            this.field_executor_login.HeaderText = "Логин исполнителя";
            this.field_executor_login.MinimumWidth = 100;
            this.field_executor_login.Name = "executor_login";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
