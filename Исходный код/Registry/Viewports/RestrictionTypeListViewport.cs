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
    internal sealed partial class RestrictionTypeListViewport : EditableDataGridViewport
    {
        #region Models
        DataTable snapshot_restriction_types = new DataTable("snapshot_restriction_types");
        #endregion Models

        private RestrictionTypeListViewport()
            : this(null)
        {
        }

        public RestrictionTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            snapshot_restriction_types.Locale = CultureInfo.InvariantCulture;
        }

        public RestrictionTypeListViewport(RestrictionTypeListViewport restrictionTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = RestrictionTypesFromView();
            var list_from_viewport = RestrictionTypesFromViewport();
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
                dataRowView["id_restriction_type"], 
                dataRowView["restriction_type"]
            };
        }

        private static bool ValidateViewportData(List<RestrictionType> list)
        {
            foreach (var restrictionType in list)
            {
                if (restrictionType.RestrictionTypeName == null)
                {
                    MessageBox.Show("Не заполнено наименование типа реквизита", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restrictionType.RestrictionTypeName != null && restrictionType.RestrictionTypeName.Length > 255)
                {
                    MessageBox.Show("Длина названия типа реквизита не может превышать 255 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static RestrictionType RowToRestrictionType(DataRow row)
        {
            var restrictionType = new RestrictionType();
            restrictionType.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
            restrictionType.RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type");
            return restrictionType;
        }

        private List<RestrictionType> RestrictionTypesFromViewport()
        {
            var list = new List<RestrictionType>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var rt = new RestrictionType();
                    var row = dataGridView.Rows[i];
                    rt.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                    rt.RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type");
                    list.Add(rt);
                }
            }
            return list;
        }

        private List<RestrictionType> RestrictionTypesFromView()
        {
            var list = new List<RestrictionType>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var rt = new RestrictionType();
                var row = ((DataRowView)GeneralBindingSource[i]);
                rt.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                rt.RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type");
                list.Add(rt);
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.RestrictionTypesDataModel);
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "restriction_types";
            GeneralBindingSource.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                snapshot_restriction_types.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName, 
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                snapshot_restriction_types.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource();
            GeneralSnapshotBindingSource.DataSource = snapshot_restriction_types;
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_restriction_types_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;
            id_restriction_type.DataPropertyName = "id_restriction_type";
            restriction_type.DataPropertyName = "restriction_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += RestrictionTypeListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += RestrictionTypeListViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += RestrictionTypeListViewport_RowDeleted;
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
            snapshot_restriction_types.Clear();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                snapshot_restriction_types.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
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
            var list = RestrictionTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = GeneralDataModel.Select().Rows.Find(list[i].IdRestrictionType);
                if (row == null)
                {
                    var id_restriction_type = GeneralDataModel.Insert(list[i]);
                    if (id_restriction_type == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_restriction_type"] = id_restriction_type;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToRestrictionType(row) == list[i])
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["restriction_type"] = list[i].RestrictionTypeName == null ? DBNull.Value : (object)list[i].RestrictionTypeName;
                }
            }
            list = RestrictionTypesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_restriction_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_restriction_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_restriction_type"].Value == list[i].IdRestrictionType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (GeneralDataModel.Delete(list[i].IdRestrictionType.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(list[i].IdRestrictionType).Delete();
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
            var viewport = new RestrictionTypeListViewport(this, MenuCallback);
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
                var result = MessageBox.Show("Сохранить изменения о типах реквизитов в базу данных?", "Внимание",
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
            GeneralDataModel.Select().RowChanged -= RestrictionTypeListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= RestrictionTypeListViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= RestrictionTypeListViewport_RowDeleted;
            base.OnClosing(e);
        }

        private void RestrictionTypeListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void RestrictionTypeListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = GeneralSnapshotBindingSource.Find("id_restriction_type", e.Row["id_restriction_type"]);
                if (row_index != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[row_index]).Delete();
            }
        }

        void RestrictionTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = GeneralSnapshotBindingSource.Find("id_restriction_type", e.Row["id_restriction_type"]);
            if (row_index == -1 && GeneralBindingSource.Find("id_restriction_type", e.Row["id_restriction_type"]) != -1)
            {
                snapshot_restriction_types.Rows.Add(e.Row["id_restriction_type"], e.Row["restriction_type"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[row_index]);
                    row["restriction_type"] = e.Row["restriction_type"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "restriction_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина названия типа реквизита не может превышать 255 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название типа реквизита не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void v_snapshot_restriction_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }
    }
}
