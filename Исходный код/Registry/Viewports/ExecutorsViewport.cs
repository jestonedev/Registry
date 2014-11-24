using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;
using Security;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class ExecutorsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_executor;
        private DataGridViewTextBoxColumn executor_name;
        private DataGridViewTextBoxColumn executor_login;
        private DataGridViewTextBoxColumn phone;
        private DataGridViewCheckBoxColumn is_inactive;
        #endregion Components

        #region Models
        ExecutorsDataModel executors = null;
        DataTable snapshot_executors = new DataTable("snapshot_executors");
        #endregion Models

        #region Views
        BindingSource v_executors = null;
        BindingSource v_snapshot_executors = null;
        #endregion Views



        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private ExecutorsViewport()
            : this(null)
        {
        }

        public ExecutorsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshot_executors.Locale = CultureInfo.InvariantCulture;
        }

        public ExecutorsViewport(ExecutorsViewport executorsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = executorsViewport.DynamicFilter;
            this.StaticFilter = executorsViewport.StaticFilter;
            this.ParentRow = executorsViewport.ParentRow;
            this.ParentType = executorsViewport.ParentType;
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

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_executor"], 
                dataRowView["executor_name"],
                dataRowView["executor_login"],
                dataRowView["phone"],
                ViewportHelper.ValueOrNull<bool>(dataRowView,"is_inactive") == true
            };
        }

        private static bool ValidateViewportData(List<Executor> list)
        {
            foreach (Executor executor in list)
            {
                if (executor.ExecutorName == null)
                {
                    MessageBox.Show("ФИО исполнителя не может быть пустым", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if ((executor.ExecutorLogin != null) &&
                    (UserDomain.GetUserDomain(executor.ExecutorLogin) == null))
                {
                    MessageBox.Show("Пользователя с указанным логином не существует", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static Executor RowToExecutor(DataRow row)
        {
            Executor executor = new Executor();
            executor.IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor");
            executor.ExecutorName = ViewportHelper.ValueOrNull(row, "executor_name");
            executor.ExecutorLogin = ViewportHelper.ValueOrNull(row, "executor_login");
            executor.Phone = ViewportHelper.ValueOrNull(row, "phone");
            executor.IsInactive = ViewportHelper.ValueOrNull<bool>(row, "is_inactive");
            return executor;
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
                    e.IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor");
                    e.ExecutorName = ViewportHelper.ValueOrNull(row, "executor_name");
                    e.ExecutorLogin = ViewportHelper.ValueOrNull(row, "executor_login");
                    e.Phone = ViewportHelper.ValueOrNull(row, "phone");
                    e.IsInactive = ViewportHelper.ValueOrNull<bool>(row, "is_inactive") == true;
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
                e.IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor");
                e.ExecutorName = ViewportHelper.ValueOrNull(row, "executor_name");
                e.ExecutorLogin = ViewportHelper.ValueOrNull(row, "executor_login");
                e.Phone = ViewportHelper.ValueOrNull(row, "phone");
                e.IsInactive = ViewportHelper.ValueOrNull<bool>(row, "is_inactive") == true;
                list.Add(e);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_executors.Count;
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            executors = ExecutorsDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            executors.Select();

            v_executors = new BindingSource();
            v_executors.DataMember = "executors";
            v_executors.DataSource = DataSetManager.DataSet;

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
            id_executor.DataPropertyName = "id_executor";
            executor_name.DataPropertyName = "executor_name";
            executor_login.DataPropertyName = "executor_login";
            phone.DataPropertyName = "phone";
            is_inactive.DataPropertyName = "is_inactive";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            executors.Select().RowChanged += new DataRowChangeEventHandler(ExecutorsViewport_RowChanged);
            executors.Select().RowDeleting += new DataRowChangeEventHandler(ExecutorsViewport_RowDeleting);
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_executors.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_executors.Position != -1) && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
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
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
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
                DataRow row = executors.Select().Rows.Find(((Executor)list[i]).IdExecutor);
                if (row == null)
                {
                    int id_executor = ExecutorsDataModel.Insert(list[i]);
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
                    if (ExecutorsDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["executor_name"] = list[i].ExecutorName == null ? DBNull.Value : (object)list[i].ExecutorName;
                    row["executor_login"] = list[i].ExecutorLogin == null ? DBNull.Value : (object)list[i].ExecutorLogin;
                    row["phone"] = list[i].Phone == null ? DBNull.Value : (object)list[i].Phone;
                    row["is_inactive"] = list[i].IsInactive == null ? DBNull.Value : (object)list[i].IsInactive;
                }
            }
            list = ExecutorsFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_executor"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_executor"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_executor"].Value == list[i].IdExecutor))
                        row_index = j;
                if (row_index == -1)
                {
                    if (ExecutorsDataModel.Delete(list[i].IdExecutor.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    executors.Select().Rows.Find(((Executor)list[i]).IdExecutor).Delete();
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
            ExecutorsViewport viewport = new ExecutorsViewport(this, MenuCallback);
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
                DialogResult result = MessageBox.Show("Сохранить изменения о виде основания в базу данных?", "Внимание",
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
            executors.Select().RowChanged -= new DataRowChangeEventHandler(ExecutorsViewport_RowChanged);
            executors.Select().RowDeleting -= new DataRowChangeEventHandler(ExecutorsViewport_RowDeleting);
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "executor_name":
                    if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "ФИО исполнителя не может быть пустым";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
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
                    row["phone"] = e.Row["phone"];
                    row["is_inactive"] = e.Row["is_inactive"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_executors.Rows.Add(new object[] { 
                        e.Row["id_executor"], 
                        e.Row["executor_name"], 
                        e.Row["executor_login"], 
                        e.Row["phone"], 
                        e.Row["is_inactive"]
                    });
                }
        }

        void v_snapshot_executors_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExecutorsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_executor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.executor_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.executor_login = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.phone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.is_inactive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
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
            this.id_executor,
            this.executor_name,
            this.executor_login,
            this.phone,
            this.is_inactive});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(648, 281);
            this.dataGridView.TabIndex = 8;
            // 
            // id_executor
            // 
            this.id_executor.Frozen = true;
            this.id_executor.HeaderText = "Идентификатор исполнителя";
            this.id_executor.Name = "id_executor";
            this.id_executor.ReadOnly = true;
            this.id_executor.Visible = false;
            // 
            // executor_name
            // 
            this.executor_name.HeaderText = "ФИО исполнителя";
            this.executor_name.MaxInputLength = 255;
            this.executor_name.MinimumWidth = 150;
            this.executor_name.Name = "executor_name";
            // 
            // executor_login
            // 
            this.executor_login.HeaderText = "Логин исполнителя";
            this.executor_login.MaxInputLength = 255;
            this.executor_login.MinimumWidth = 150;
            this.executor_login.Name = "executor_login";
            // 
            // phone
            // 
            this.phone.HeaderText = "Телефон";
            this.phone.MaxInputLength = 255;
            this.phone.MinimumWidth = 150;
            this.phone.Name = "phone";
            // 
            // is_inactive
            // 
            this.is_inactive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.is_inactive.HeaderText = "Неактивный";
            this.is_inactive.Name = "is_inactive";
            this.is_inactive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.is_inactive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ExecutorsViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(654, 287);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExecutorsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Исполнители";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
