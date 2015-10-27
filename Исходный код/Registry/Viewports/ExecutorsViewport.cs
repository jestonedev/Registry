using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using Settings;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ExecutorsViewport : EditableDataGridViewport
    {
        private ExecutorsViewport()
            : this(null)
        {
        }

        public ExecutorsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_executors")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        public ExecutorsViewport(Viewport executorsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = executorsViewport.DynamicFilter;
            StaticFilter = executorsViewport.StaticFilter;
            ParentRow = executorsViewport.ParentRow;
            ParentType = executorsViewport.ParentType;
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

        private static bool ValidateViewportData(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var executor = (Executor) entity;
                if (executor.ExecutorName == null)
                {
                    MessageBox.Show(@"ФИО исполнителя не может быть пустым", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if ((executor.ExecutorLogin != null) && (RegistrySettings.UseLdap) &&
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

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
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

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var executor in GeneralBindingSource)
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.ExecutorsDataModel);

            //Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource
            {
                DataMember = "executors",
                DataSource = DataModel.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(
                    GeneralDataModel.Select().Columns[i].ColumnName, GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            foreach (var executor in GeneralBindingSource)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)executor)));
            GeneralSnapshotBindingSource = new BindingSource { DataSource = GeneralSnapshot };
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_executors_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;
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
            GeneralDataModel.Select().RowChanged += ExecutorsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += ExecutorsViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += ExecutorsViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            if (row != null) row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralSnapshotBindingSource.Position != -1) && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
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
            foreach (var executor in GeneralBindingSource)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)executor)));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
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
                var executor = (Executor)list[i];
                var row = GeneralDataModel.Select().Rows.Find(executor.IdExecutor);
                if (row == null)
                {
                    var idExecutor = GeneralDataModel.Insert(list[i]);
                    if (idExecutor == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_executor"] = idExecutor;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {

                    if (RowToExecutor(row) == executor)
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["executor_name"] = executor.ExecutorName == null ? DBNull.Value : (object)executor.ExecutorName;
                    row["executor_login"] = executor.ExecutorLogin == null ? DBNull.Value : (object)executor.ExecutorLogin;
                    row["phone"] = executor.Phone == null ? DBNull.Value : (object)executor.Phone;
                    row["is_inactive"] = executor.IsInactive == null ? DBNull.Value : (object)executor.IsInactive;
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var executor = (Executor) entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_executor"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_executor"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_executor"].Value == executor.IdExecutor))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (executor.IdExecutor != null && GeneralDataModel.Delete(executor.IdExecutor.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(executor.IdExecutor).Delete();
                }
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
            GeneralDataModel.Select().RowChanged -= ExecutorsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= ExecutorsViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= ExecutorsViewport_RowDeleted;
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
                var rowIndex = GeneralSnapshotBindingSource.Find("id_executor", e.Row["id_executor"]);
                if (rowIndex != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
            }
        }

        void ExecutorsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_executor", e.Row["id_executor"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_executor", e.Row["id_executor"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_executor"], e.Row["executor_name"], e.Row["executor_login"], e.Row["phone"], e.Row["is_inactive"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
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
    }
}
