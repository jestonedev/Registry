using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyReasonTypesViewport: EditableDataGridViewport
    {
        private TenancyReasonTypesViewport()
            : this(null, null)
        {
        }

        public TenancyReasonTypesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_reason_types")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_reason_type"], 
                dataRowView["reason_name"],
                dataRowView["reason_template"]
            };
        }

        private static bool ValidateViewportData(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var reasonType = (ReasonType) entity;
                if (reasonType.ReasonName == null)
                {
                    MessageBox.Show(@"Имя вида основания не может быть пустым", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (reasonType.ReasonName != null && reasonType.ReasonName.Length > 150)
                {
                    MessageBox.Show(@"Длина имени типа основания не может превышать 150 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (reasonType.ReasonTemplate == null)
                {
                    MessageBox.Show(@"Шаблон основания не может быть пустым", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (reasonType.ReasonTemplate != null && reasonType.ReasonTemplate.Length > 4000)
                {
                    MessageBox.Show(@"Длина шаблона вида основания не может превышать 4000 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (Regex.IsMatch(reasonType.ReasonTemplate, "@reason_number@") &&
                    (Regex.IsMatch(reasonType.ReasonTemplate, "@reason_date@"))) continue;
                MessageBox.Show(@"Шаблон основания имеет неверный формат. В шаблоне должны быть указаны номер (в виде шаблона @reason_number@) и" +
                                @" дата (в виде шаблона @reason_date@) основания", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private static ReasonType RowToReasonType(DataRow row)
        {
            var reasonType = new ReasonType
            {
                IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type"),
                ReasonName = ViewportHelper.ValueOrNull(row, "reason_name"),
                ReasonTemplate = ViewportHelper.ValueOrNull(row, "reason_template")
            };

            return reasonType;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var rt = new ReasonType();
                var row = dataGridView.Rows[i];
                rt.IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type");
                rt.ReasonName = ViewportHelper.ValueOrNull(row, "reason_name");
                rt.ReasonTemplate = ViewportHelper.ValueOrNull(row, "reason_template"); 
                list.Add(rt);
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var rt = new ReasonType();
                var row = ((DataRowView)GeneralBindingSource[i]);
                rt.IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type");
                rt.ReasonName = ViewportHelper.ValueOrNull(row, "reason_name");
                rt.ReasonTemplate = ViewportHelper.ValueOrNull(row, "reason_template"); 
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.TenancyReasonTypesDataModel);

            //Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "tenancy_reason_types";
            GeneralBindingSource.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            GeneralSnapshot.Locale = CultureInfo.InvariantCulture;
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(
                    GeneralDataModel.Select().Columns[i].ColumnName, GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource();
            GeneralSnapshotBindingSource.DataSource = GeneralSnapshot;
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_reason_types_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;
            id_reason_type.DataPropertyName = "id_reason_type";
            reason_name.DataPropertyName = "reason_name";
            reason_template.DataPropertyName = "reason_template";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += ReasonTypesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += ReasonTypesViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += ReasonTypesViewport_RowDeleted;
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
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
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
                var reasonType = (ReasonType) list[i];
                var row = GeneralDataModel.Select().Rows.Find(reasonType.IdReasonType);
                if (row == null)
                {
                    var idReasonType = GeneralDataModel.Insert(reasonType);
                    if (idReasonType == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_reason_type"] = idReasonType;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {

                    if (RowToReasonType(row) == reasonType)
                        continue;
                    if (GeneralDataModel.Update(reasonType) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["reason_name"] = reasonType.ReasonName == null ? DBNull.Value : (object)reasonType.ReasonName;
                    row["reason_template"] = reasonType.ReasonTemplate == null ? DBNull.Value : (object)reasonType.ReasonTemplate;
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var reasonType = (ReasonType) entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_reason_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_reason_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_reason_type"].Value == reasonType.IdReasonType))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (reasonType.IdReasonType != null && 
                        GeneralDataModel.Delete(reasonType.IdReasonType.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(reasonType.IdReasonType).Delete();
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
            var viewport = new TenancyReasonTypesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения о виде основания в базу данных?", @"Внимание",
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
            GeneralDataModel.Select().RowChanged -= ReasonTypesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= ReasonTypesViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= ReasonTypesViewport_RowDeleted;
            base.OnClosing(e);
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "reason_name":
                    if (cell.Value.ToString().Trim().Length > 150)
                        cell.ErrorText = "Длина названия вида основания не может превышать 150 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название вида основания не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
                case "reason_template":
                    if (cell.Value.ToString().Length > 4000)
                        cell.ErrorText = "Длина шаблона вида основания не может превышать 4000 символов";
                    else
                    if (!(Regex.IsMatch(cell.Value.ToString(), "@reason_number@") &&
                         (Regex.IsMatch(cell.Value.ToString(), "@reason_date@"))))
                        cell.ErrorText = "Шаблон основания имеет неверный формат. В шаблоне должны быть указаны номер (в виде шаблона @reason_number@) и" +
                            " дата (в виде шаблона @reason_date@) основания";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void ReasonTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void ReasonTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = GeneralSnapshotBindingSource.Find("id_reason_type", e.Row["id_reason_type"]);
                if (rowIndex != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
            }
        }

        void ReasonTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_reason_type", e.Row["id_reason_type"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_reason_type", e.Row["id_reason_type"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_reason_type"], e.Row["reason_name"], e.Row["reason_template"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
                    row["reason_name"] = e.Row["reason_name"];
                    row["reason_template"] = e.Row["reason_template"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshot_reason_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }
    }
}
