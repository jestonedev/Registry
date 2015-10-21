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
    internal sealed partial class OwnershipTypeListViewport : EditableDataGridViewport
    {
        private OwnershipTypeListViewport()
            : this(null)
        {
        }

        public OwnershipTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_ownership_right_types")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        public OwnershipTypeListViewport(OwnershipTypeListViewport ownershipTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = OwnershipRightTypesFromView();
            var list_from_viewport = OwnershipRightTypesFromViewport();
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
                dataRowView["id_ownership_right_type"], 
                dataRowView["ownership_right_type"]
            };
        }

        private static bool ValidateViewportData(List<OwnershipRightType> list)
        {
            foreach (var ownershipRightType in list)
            {
                if (ownershipRightType.OwnershipRightTypeName == null)
                {
                    MessageBox.Show("Не заполнено наименование типа ограничения", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRightType.OwnershipRightTypeName != null && ownershipRightType.OwnershipRightTypeName.Length > 255)
                {
                    MessageBox.Show("Длина названия типа ограничения не может превышать 255 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static OwnershipRightType RowToOwnershipRightType(DataRow row)
        {
            var ownershipRightType = new OwnershipRightType();
            ownershipRightType.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
            ownershipRightType.OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type");
            return ownershipRightType;
        }

        private List<OwnershipRightType> OwnershipRightTypesFromViewport()
        {
            var list = new List<OwnershipRightType>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var ort = new OwnershipRightType();
                    var row = dataGridView.Rows[i];
                    ort.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                    ort.OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type");
                    list.Add(ort);
                }
            }
            return list;
        }

        private List<OwnershipRightType> OwnershipRightTypesFromView()
        {
            var list = new List<OwnershipRightType>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var ort = new OwnershipRightType();
                var row = ((DataRowView)GeneralBindingSource[i]);
                ort.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                ort.OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type");
                list.Add(ort);
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.OwnershipRightTypesDataModel);
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "ownership_right_types";
            GeneralBindingSource.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName,
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource();
            GeneralSnapshotBindingSource.DataSource = GeneralSnapshot;
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_ownership_right_types_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;
            id_ownership_right_type.DataPropertyName = "id_ownership_right_type";
            ownership_right_type.DataPropertyName = "ownership_right_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += OwnershipTypeListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += OwnershipTypeListViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += OwnershipTypeListViewport_RowDeleted;
        }
        
        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralSnapshotBindingSource.Position != -1) && AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
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
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            GeneralDataModel.EditingNewRecord = true;
            var list = OwnershipRightTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = GeneralDataModel.Select().Rows.Find(list[i].IdOwnershipRightType);
                if (row == null)
                {
                    var id_ownership_right_type = GeneralDataModel.Insert(list[i]);
                    if (id_ownership_right_type == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_ownership_right_type"] = id_ownership_right_type;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToOwnershipRightType(row) == list[i])
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["ownership_right_type"] = list[i].OwnershipRightTypeName == null ? DBNull.Value : (object)list[i].OwnershipRightTypeName;
                }
            }
            list = OwnershipRightTypesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_ownership_right_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_ownership_right_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_ownership_right_type"].Value == list[i].IdOwnershipRightType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (GeneralDataModel.Delete(list[i].IdOwnershipRightType.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(list[i].IdOwnershipRightType).Delete();
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
            var viewport = new OwnershipTypeListViewport(this, MenuCallback);
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
                var result = MessageBox.Show("Сохранить изменения о типах ограничений в базу данных?", "Внимание",
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
            GeneralDataModel.Select().RowChanged -= OwnershipTypeListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= OwnershipTypeListViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= OwnershipTypeListViewport_RowDeleted;
            base.OnClosing(e);
        }

        private void OwnershipTypeListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void OwnershipTypeListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = GeneralSnapshotBindingSource.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]);
                if (row_index != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[row_index]).Delete();
            }
        }

        void OwnershipTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = GeneralSnapshotBindingSource.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]);
            if (row_index == -1 && GeneralBindingSource.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_ownership_right_type"], e.Row["ownership_right_type"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[row_index]);
                    row["ownership_right_type"] = e.Row["ownership_right_type"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshot_ownership_right_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "ownership_right_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина названия типа ограничения не может превышать 255 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название типа ограничения не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }
    }
}
