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
    internal sealed partial class RestrictionTypeListViewport : EditableDataGridViewport
    {
        #region Models

        readonly DataTable _snapshotRestrictionTypes = new DataTable("snapshot_restriction_types");
        #endregion Models

        private RestrictionTypeListViewport()
            : this(null)
        {
        }

        public RestrictionTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            _snapshotRestrictionTypes.Locale = CultureInfo.InvariantCulture;
        }

        public RestrictionTypeListViewport(RestrictionTypeListViewport restrictionTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = restrictionTypeListViewport.DynamicFilter;
            StaticFilter = restrictionTypeListViewport.StaticFilter;
            ParentRow = restrictionTypeListViewport.ParentRow;
            ParentType = restrictionTypeListViewport.ParentType;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_restriction_type"], 
                dataRowView["restriction_type"]
            };
        }

        private static bool ValidateViewportData(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var restrictionType = (RestrictionType) entity;
                if (restrictionType.RestrictionTypeName == null)
                {
                    MessageBox.Show(@"Не заполнено наименование типа реквизита", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restrictionType.RestrictionTypeName == null || restrictionType.RestrictionTypeName.Length <= 255)
                    continue;
                MessageBox.Show(@"Длина названия типа реквизита не может превышать 255 символов", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private static RestrictionType RowToRestrictionType(DataRow row)
        {
            var restrictionType = new RestrictionType
            {
                IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type"),
                RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type")
            };
            return restrictionType;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var rt = new RestrictionType();
                var row = dataGridView.Rows[i];
                rt.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                rt.RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type");
                list.Add(rt);
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
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

            GeneralBindingSource = new BindingSource
            {
                DataMember = "restriction_types",
                DataSource = DataModel.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                _snapshotRestrictionTypes.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName, 
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                _snapshotRestrictionTypes.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource {DataSource = _snapshotRestrictionTypes};
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
            _snapshotRestrictionTypes.Clear();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                _snapshotRestrictionTypes.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
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
            var list = EntitiesListFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var restrictionType = (RestrictionType) list[i];
                var row = GeneralDataModel.Select().Rows.Find(restrictionType.IdRestrictionType);
                if (row == null)
                {
                    var idRestrictionType = GeneralDataModel.Insert(restrictionType);
                    if (idRestrictionType == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_restriction_type"] = idRestrictionType;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToRestrictionType(row) == restrictionType)
                        continue;
                    if (GeneralDataModel.Update(restrictionType) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["restriction_type"] = restrictionType.RestrictionTypeName == null ? 
                        DBNull.Value : (object)restrictionType.RestrictionTypeName;
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var restrictionType = (RestrictionType) entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_restriction_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_restriction_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_restriction_type"].Value == restrictionType.IdRestrictionType))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (restrictionType.IdRestrictionType != null && 
                        GeneralDataModel.Delete(restrictionType.IdRestrictionType.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(restrictionType.IdRestrictionType).Delete();
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
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения о типах реквизитов в базу данных?", @"Внимание",
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
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_restriction_type", e.Row["id_restriction_type"]);
            if (rowIndex != -1)
                ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
        }

        void RestrictionTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_restriction_type", e.Row["id_restriction_type"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_restriction_type", e.Row["id_restriction_type"]) != -1)
            {
                _snapshotRestrictionTypes.Rows.Add(e.Row["id_restriction_type"], e.Row["restriction_type"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
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
