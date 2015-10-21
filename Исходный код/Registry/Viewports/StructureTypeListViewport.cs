using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class StructureTypeListViewport : EditableDataGridViewport
    {

        private StructureTypeListViewport()
            : this(null)
        {
        }

        public StructureTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_structure_types") {Locale = CultureInfo.InvariantCulture};
        }

        public StructureTypeListViewport(StructureTypeListViewport structureTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = StructureTypesFromView();
            var list_from_viewport = StructureTypesFromViewport();
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
                dataRowView["id_structure_type"], 
                dataRowView["structure_type"]
            };
        }

        private static bool ValidateViewportData(List<StructureType> list)
        {
            foreach (var structureType in list)
            {
                if (structureType.StructureTypeName == null)
                {
                    MessageBox.Show("Не заполнено наименование структуры здания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (structureType.StructureTypeName != null && structureType.StructureTypeName.Length > 255)
                {
                    MessageBox.Show("Длина названия структуры здания не может превышать 255 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static StructureType RowToStructureType(DataRow row)
        {
            var structureType = new StructureType();
            structureType.IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
            structureType.StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type");
            return structureType;
        }

        private List<StructureType> StructureTypesFromViewport()
        {
            var list = new List<StructureType>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var st = new StructureType();
                    var row = dataGridView.Rows[i];
                    st.IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
                    st.StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type");
                    list.Add(st);
                }
            }
            return list;
        }

        private List<StructureType> StructureTypesFromView()
        {
            var list = new List<StructureType>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var st = new StructureType();
                var row = ((DataRowView)GeneralBindingSource[i]);
                st.IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
                st.StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type");
                list.Add(st);
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.StructureTypesDataModel);
            //Ожиданем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "structure_types";
            GeneralBindingSource.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(
                    GeneralDataModel.Select().Columns[i].ColumnName, GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource();
            GeneralSnapshotBindingSource.DataSource = GeneralSnapshot;
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_structure_types_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;
            id_structure_type.DataPropertyName = "id_structure_type";
            structure_type.DataPropertyName = "structure_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += StructureTypeListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += StructureTypeListViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += StructureTypeListViewport_RowDeleted;
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
            var list = StructureTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = GeneralDataModel.Select().Rows.Find(list[i].IdStructureType);
                if (row == null)
                {
                    var id_structure_type = GeneralDataModel.Insert(list[i]);
                    if (id_structure_type == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_structure_type"] = id_structure_type;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToStructureType(row) == list[i])
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["structure_type"] = list[i].StructureTypeName == null ? DBNull.Value : (object)list[i].StructureTypeName;
                }
            }
            list = StructureTypesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_structure_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_structure_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_structure_type"].Value == list[i].IdStructureType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (GeneralDataModel.Delete(list[i].IdStructureType.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(list[i].IdStructureType).Delete();
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
            var viewport = new StructureTypeListViewport(this, MenuCallback);
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
                var result = MessageBox.Show("Сохранить изменения о структуре зданий в базу данных?", "Внимание",
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
            GeneralDataModel.Select().RowChanged -= StructureTypeListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= StructureTypeListViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= StructureTypeListViewport_RowDeleted;
            base.OnClosing(e);
        }

        private void StructureTypeListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void StructureTypeListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = GeneralSnapshotBindingSource.Find("id_structure_type", e.Row["id_structure_type"]);
                if (row_index != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[row_index]).Delete();
            }
        }

        void StructureTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = GeneralSnapshotBindingSource.Find("id_structure_type", e.Row["id_structure_type"]);
            if (row_index == -1 && GeneralBindingSource.Find("id_structure_type", e.Row["id_structure_type"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_structure_type"], e.Row["structure_type"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[row_index]);
                    row["structure_type"] = e.Row["structure_type"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
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
                case "structure_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина названия структуры здания не может превышать 255 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название структуры здания не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        void v_snapshot_structure_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }
    }
}
