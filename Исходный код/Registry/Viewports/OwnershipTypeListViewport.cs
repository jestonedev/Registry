using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class OwnershipTypeListViewport : EditableDataGridViewport
    {
        private OwnershipTypeListViewport()
            : this(null, null)
        {
        }

        public OwnershipTypeListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent(); 
            GeneralSnapshot = new DataTable("snapshot_ownership_right_types") { Locale = CultureInfo.InvariantCulture };
        }

        private static bool ValidateViewportData(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var ownershipRightType = (OwnershipRightType) entity;
                if (ownershipRightType.OwnershipRightTypeName == null)
                {
                    MessageBox.Show(@"Не заполнено наименование типа ограничения", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRightType.OwnershipRightTypeName != null && ownershipRightType.OwnershipRightTypeName.Length > 255)
                {
                    MessageBox.Show(@"Длина названия типа ограничения не может превышать 255 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var ort = new OwnershipRightType();
                var row = dataGridView.Rows[i];
                ort.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                ort.OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type");
                list.Add(ort);
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
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
            GeneralDataModel = DataModel.GetInstance<OwnershipRightTypesDataModel>(); 
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource
            {
                DataMember = "ownership_right_types",
                DataSource = DataModel.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName,
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(OwnershipRightTypeConverter.ToArray((DataRowView)GeneralBindingSource[i]));
            GeneralSnapshotBindingSource = new BindingSource { DataSource = GeneralSnapshot };
            AddEventHandler<EventArgs>(GeneralSnapshotBindingSource, "CurrentItemChanged", v_snapshot_ownership_right_types_CurrentItemChanged);

            dataGridView.DataSource = GeneralSnapshotBindingSource;
            id_ownership_right_type.DataPropertyName = "id_ownership_right_type";
            ownership_right_type.DataPropertyName = "ownership_right_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            AddEventHandler<DataGridViewCellEventArgs>(dataGridView, "CellValidated", dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            AddEventHandler<DataGridViewCellEventArgs>(dataGridView, "CellValueChanged", dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", OwnershipTypeListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleting", OwnershipTypeListViewport_RowDeleting);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", OwnershipTypeListViewport_RowDeleted);
        }
        
        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
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
                GeneralSnapshot.Rows.Add(OwnershipRightTypeConverter.ToArray(((DataRowView)GeneralBindingSource[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            SyncViews = false;
            dataGridView.EndEdit();
            GeneralDataModel.EditingNewRecord = true;
            var list = EntitiesListFromViewport();
            if (!ValidateViewportData(list))
            {
                SyncViews = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var ownershipType = (OwnershipRightType) list[i];
                var row = GeneralDataModel.Select().Rows.Find(ownershipType.IdOwnershipRightType);
                if (row == null)
                {
                    var idOwnershipRightType = GeneralDataModel.Insert(ownershipType);
                    if (idOwnershipRightType == -1)
                    {
                        SyncViews = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_ownership_right_type"] = idOwnershipRightType;
                    GeneralDataModel.Select().Rows.Add(OwnershipRightTypeConverter.ToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (OwnershipRightTypeConverter.FromRow(row) == ownershipType)
                        continue;
                    if (GeneralDataModel.Update(ownershipType) == -1)
                    {
                        SyncViews = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    OwnershipRightTypeConverter.FillRow(ownershipType, row);
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var ownershipType = (OwnershipRightType) entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_ownership_right_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_ownership_right_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_ownership_right_type"].Value == ownershipType.IdOwnershipRightType))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (ownershipType.IdOwnershipRightType != null && 
                        GeneralDataModel.Delete(ownershipType.IdOwnershipRightType.Value) == -1)
                    {
                        SyncViews = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(ownershipType.IdOwnershipRightType).Delete();
                }
            }
            SyncViews = true;
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
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения о типах ограничений в базу данных?", @"Внимание",
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

        private void OwnershipTypeListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]);
            if (rowIndex != -1)
                ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
        }

        private void OwnershipTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]) != -1)
            {
                GeneralSnapshot.Rows.Add(
                    OwnershipRightTypeConverter.ToArray(e.Row));
            }
            else
                if (rowIndex != -1)
                {
                    var row = (DataRowView)GeneralSnapshotBindingSource[rowIndex];
                    row["ownership_right_type"] = e.Row["ownership_right_type"];
                }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void v_snapshot_ownership_right_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
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
