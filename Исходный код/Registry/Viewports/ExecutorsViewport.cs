using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

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
        ExecutorsDataModel executors;
        DataTable snapshot_executors = new DataTable("snapshot_executors");
        #endregion Models

        #region Views
        BindingSource v_executors;
        BindingSource v_snapshot_executors;
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

        public ExecutorsViewport(Viewport executorsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = executorsViewport.DynamicFilter;
            StaticFilter = executorsViewport.StaticFilter;
            ParentRow = executorsViewport.ParentRow;
            ParentType = executorsViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = ExecutorsFromView();
            var listFromViewport = ExecutorsFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            foreach (var executorView in listFromView)
            {
                var founded = false;
                foreach (var executorViewport in listFromViewport)
                    if (executorView == executorViewport)
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_executor"], 
                dataRowView["executor_name"],
                dataRowView["executor_login"],
                dataRowView["phone"],
                ViewportHelper.ValueOrNull<bool>(dataRowView,"is_inactive") == true
            };
        }

        private static bool ValidateViewportData(List<Executor> list)
        {
            foreach (var executor in list)
            {
                if (executor.ExecutorName == null)
                {
                    MessageBox.Show(@"ФИО исполнителя не может быть пустым", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if ((executor.ExecutorLogin != null) && (RegistrySettings.UseLDAP) &&
                    (UserDomain.GetUserDomain(executor.ExecutorLogin) == null))
                {
                    MessageBox.Show(@"Пользователя с указанным логином не существует", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static Executor RowToExecutor(DataRow row)
        {
            var executor = new Executor
            {
                IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor"),
                ExecutorName = ViewportHelper.ValueOrNull(row, "executor_name"),
                ExecutorLogin = ViewportHelper.ValueOrNull(row, "executor_login"),
                Phone = ViewportHelper.ValueOrNull(row, "phone"),
                IsInactive = ViewportHelper.ValueOrNull<bool>(row, "is_inactive")
            };
            return executor;
        }

        private List<Executor> ExecutorsFromViewport()
        {
            var list = new List<Executor>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var row = dataGridView.Rows[i];
                var e = new Executor
                {
                    IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor"),
                    ExecutorName = ViewportHelper.ValueOrNull(row, "executor_name"),
                    ExecutorLogin = ViewportHelper.ValueOrNull(row, "executor_login"),
                    Phone = ViewportHelper.ValueOrNull(row, "phone"),
                    IsInactive = ViewportHelper.ValueOrNull<bool>(row, "is_inactive") == true
                };
                list.Add(e);
            }
            return list;
        }

        private List<Executor> ExecutorsFromView()
        {
            var list = new List<Executor>();
            foreach (var executor in v_executors)
            {
                var row = ((DataRowView)executor);
                var e = new Executor
                {
                    IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor"),
                    ExecutorName = ViewportHelper.ValueOrNull(row, "executor_name"),
                    ExecutorLogin = ViewportHelper.ValueOrNull(row, "executor_login"),
                    Phone = ViewportHelper.ValueOrNull(row, "phone"),
                    IsInactive = ViewportHelper.ValueOrNull<bool>(row, "is_inactive") == true
                };
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
            DockAreas = DockAreas.Document;
            executors = ExecutorsDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            executors.Select();

            v_executors = new BindingSource
            {
                DataMember = "executors",
                DataSource = DataSetManager.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < executors.Select().Columns.Count; i++)
                snapshot_executors.Columns.Add(new DataColumn(
                    executors.Select().Columns[i].ColumnName, executors.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            foreach (var executor in v_executors)
                snapshot_executors.Rows.Add(DataRowViewToArray(((DataRowView)executor)));
            v_snapshot_executors = new BindingSource {DataSource = snapshot_executors};
            v_snapshot_executors.CurrentItemChanged += v_snapshot_executors_CurrentItemChanged;

            dataGridView.DataSource = v_snapshot_executors;
            id_executor.DataPropertyName = "id_executor";
            executor_name.DataPropertyName = "executor_name";
            executor_login.DataPropertyName = "executor_login";
            phone.DataPropertyName = "phone";
            is_inactive.DataPropertyName = "is_inactive";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            executors.Select().RowChanged += ExecutorsViewport_RowChanged;
            executors.Select().RowDeleting += ExecutorsViewport_RowDeleting;
            executors.Select().RowDeleted += ExecutorsViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)v_snapshot_executors.AddNew();
            if (row != null) row.EndEdit();
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
            foreach (var executor in v_executors)
                snapshot_executors.Rows.Add(DataRowViewToArray(((DataRowView)executor)));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            executors.EditingNewRecord = true;
            var list = ExecutorsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true; 
                executors.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = executors.Select().Rows.Find(list[i].IdExecutor);
                if (row == null)
                {
                    var idExecutor = ExecutorsDataModel.Insert(list[i]);
                    if (idExecutor == -1)
                    {
                        sync_views = true;
                        executors.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_executors[i])["id_executor"] = idExecutor;
                    executors.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_executors[i]));
                }
                else
                {

                    if (RowToExecutor(row) == list[i])
                        continue;
                    if (ExecutorsDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        executors.EditingNewRecord = false;
                        return;
                    }
                    row["executor_name"] = list[i].ExecutorName == null ? DBNull.Value : (object)list[i].ExecutorName;
                    row["executor_login"] = list[i].ExecutorLogin == null ? DBNull.Value : (object)list[i].ExecutorLogin;
                    row["phone"] = list[i].Phone == null ? DBNull.Value : (object)list[i].Phone;
                    row["is_inactive"] = list[i].IsInactive == null ? DBNull.Value : (object)list[i].IsInactive;
                }
            }
            list = ExecutorsFromView();
            foreach (var executor in list)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_executor"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_executor"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_executor"].Value == executor.IdExecutor))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (executor.IdExecutor != null && ExecutorsDataModel.Delete(executor.IdExecutor.Value) == -1)
                    {
                        sync_views = true;
                        executors.EditingNewRecord = false;
                        return;
                    }
                    executors.Select().Rows.Find(executor.IdExecutor).Delete();
                }
            }
            sync_views = true;
            executors.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new ExecutorsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
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
            executors.Select().RowChanged -= ExecutorsViewport_RowChanged;
            executors.Select().RowDeleting -= ExecutorsViewport_RowDeleting;
            executors.Select().RowDeleted -= ExecutorsViewport_RowDeleted;
            base.OnClosing(e);
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "executor_name":
                    cell.ErrorText = string.IsNullOrEmpty(cell.Value.ToString().Trim()) ? "ФИО исполнителя не может быть пустым" : "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void ExecutorsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void ExecutorsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = v_snapshot_executors.Find("id_executor", e.Row["id_executor"]);
                if (rowIndex != -1)
                    ((DataRowView)v_snapshot_executors[rowIndex]).Delete();
            }
        }

        void ExecutorsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = v_snapshot_executors.Find("id_executor", e.Row["id_executor"]);
            if (rowIndex == -1 && v_executors.Find("id_executor", e.Row["id_executor"]) != -1)
            {
                snapshot_executors.Rows.Add(e.Row["id_executor"], e.Row["executor_name"], e.Row["executor_login"], e.Row["phone"], e.Row["is_inactive"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)v_snapshot_executors[rowIndex]);
                    row["executor_name"] = e.Row["executor_name"];
                    row["executor_login"] = e.Row["executor_login"];
                    row["phone"] = e.Row["phone"];
                    row["is_inactive"] = e.Row["is_inactive"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshot_executors_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(ExecutorsViewport));
            dataGridView = new DataGridView();
            id_executor = new DataGridViewTextBoxColumn();
            executor_name = new DataGridViewTextBoxColumn();
            executor_login = new DataGridViewTextBoxColumn();
            phone = new DataGridViewTextBoxColumn();
            is_inactive = new DataGridViewCheckBoxColumn();
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
            dataGridView.Columns.AddRange(id_executor, executor_name, executor_login, phone, is_inactive);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(648, 281);
            dataGridView.TabIndex = 8;
            dataGridView.CurrentCellDirtyStateChanged += dataGridView_CurrentCellDirtyStateChanged;
            // 
            // id_executor
            // 
            id_executor.Frozen = true;
            id_executor.HeaderText = @"Идентификатор исполнителя";
            id_executor.Name = "id_executor";
            id_executor.ReadOnly = true;
            id_executor.Visible = false;
            // 
            // executor_name
            // 
            executor_name.HeaderText = @"ФИО исполнителя";
            executor_name.MaxInputLength = 255;
            executor_name.MinimumWidth = 150;
            executor_name.Name = "executor_name";
            // 
            // executor_login
            // 
            executor_login.HeaderText = @"Логин исполнителя";
            executor_login.MaxInputLength = 255;
            executor_login.MinimumWidth = 150;
            executor_login.Name = "executor_login";
            // 
            // phone
            // 
            phone.HeaderText = @"Телефон";
            phone.MaxInputLength = 255;
            phone.MinimumWidth = 150;
            phone.Name = "phone";
            // 
            // is_inactive
            // 
            is_inactive.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            is_inactive.HeaderText = @"Неактивный";
            is_inactive.Name = "is_inactive";
            is_inactive.Resizable = DataGridViewTriState.True;
            is_inactive.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // ExecutorsViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(654, 287);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ExecutorsViewport";
            Padding = new Padding(3);
            Text = @"Исполнители";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
