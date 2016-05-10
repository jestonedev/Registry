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
    internal sealed partial class StructureTypeListViewport : EditableDataGridViewport
    {
        private StructureTypeListViewport()
            : this(null, null)
        {
        }

        public StructureTypeListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_structure_types") { Locale = CultureInfo.InvariantCulture };
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_structure_type"], 
                dataRowView["structure_type"]
            };
        }

        private static bool ValidateViewportData(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var structureType = (StructureType)entity;
                if (structureType.StructureTypeName == null)
                {
                    MessageBox.Show(@"Не заполнено наименование структуры здания", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (structureType.StructureTypeName == null || structureType.StructureTypeName.Length <= 255)
                    continue;
                MessageBox.Show(@"Длина названия структуры здания не может превышать 255 символов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private static StructureType RowToStructureType(DataRow row)
        {
            var structureType = new StructureType
            {
                IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type"),
                StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type")
            };
            return structureType;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var st = new StructureType();
                var row = dataGridView.Rows[i];
                st.IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
                st.StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type");
                list.Add(st);
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
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
            GeneralDataModel = DataModel.GetInstance<StructureTypesDataModel>();
            //Ожиданем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource
            {
                DataMember = "structure_types",
                DataSource = DataModel.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(
                    GeneralDataModel.Select().Columns[i].ColumnName, GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource { DataSource = GeneralSnapshot };
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
            return AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            if (row != null) row.EndEdit();
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
            dataGridView.EndEdit();
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
                var structureType = (StructureType)list[i];
                var row = GeneralDataModel.Select().Rows.Find(structureType.IdStructureType);
                if (row == null)
                {
                    var idStructureType = GeneralDataModel.Insert(structureType);
                    if (idStructureType == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_structure_type"] = idStructureType;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToStructureType(row) == structureType)
                        continue;
                    if (GeneralDataModel.Update(structureType) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["structure_type"] = structureType.StructureTypeName == null ? DBNull.Value :
                        (object)structureType.StructureTypeName;
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var structureType = (StructureType)entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_structure_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_structure_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_structure_type"].Value == structureType.IdStructureType))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (structureType.IdStructureType != null &&
                    GeneralDataModel.Delete(structureType.IdStructureType.Value) == -1)
                {
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                GeneralDataModel.Select().Rows.Find(structureType.IdStructureType).Delete();
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
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения о структуре зданий в базу данных?", @"Внимание",
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
            } GeneralSnapshotBindingSource.CurrentItemChanged -= v_snapshot_structure_types_CurrentItemChanged;
            dataGridView.CellValidated -= dataGridView_CellValidated;
            dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
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
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_structure_type", e.Row["id_structure_type"]);
            if (rowIndex != -1)
                ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
        }

        void StructureTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_structure_type", e.Row["id_structure_type"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_structure_type", e.Row["id_structure_type"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_structure_type"], e.Row["structure_type"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
                    row["structure_type"] = e.Row["structure_type"];
                }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
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
