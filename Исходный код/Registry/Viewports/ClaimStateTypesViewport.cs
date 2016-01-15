using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ClaimStateTypesViewport: EditableDataGridViewport
    {
        #region Models
        DataModel claim_state_types_relations;
        DataTable snapshot_claim_state_types_relations = new DataTable("snapshot_claim_state_types_relations");
        #endregion Models

        #region Views
        BindingSource v_claim_state_types_from;
        BindingSource v_claim_state_types_relations;
        BindingSource v_snapshot_claim_state_types_relations;
        #endregion Views

        int temp_id_state_type = int.MaxValue;

        private ClaimStateTypesViewport()
            : this(null, null)
        {
        }

        public ClaimStateTypesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_claim_state_types")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        protected override bool SnapshotHasChanges()
        {
            var listFromView = ClaimStateTypesFromView();
            var listFromViewport = ClaimStateTypesFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            bool founded;
            foreach (var viewClaimType in listFromView)
            {
                founded = false;
                foreach (var viewportClaimType in listFromViewport)
                    if (viewClaimType == viewportClaimType)
                        founded = true;
                if (!founded)
                    return true;
            }
            var listFromViewRel = ClaimStateTypesRelationsFromView();
            var listFromViewportRel = ClaimStateTypesRelationsFromViewport();
            if (listFromViewRel.Count != listFromViewportRel.Count)
                return true;
            foreach (var viewRel in listFromViewRel)
            {
                founded = false;
                foreach (var viewportRel in listFromViewportRel)
                    if (viewRel == viewportRel)
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_state_type"], 
                dataRowView["state_type"], 
                ViewportHelper.ValueOrNull<bool>(dataRowView,"is_start_state_type") == true
            };
        }

        private static object[] DataRowViewRelationToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_relation"], 
                dataRowView["id_state_from"],
                dataRowView["id_state_to"],
                true
            };
        }

        private static bool ValidateViewportData(List<ClaimStateType> list)
        {
            foreach (var claimStateType in list)
            {
                if (claimStateType.StateType == null)
                {
                    MessageBox.Show(@"Название вида состояния претензионно-исковой работы не может быть пустым", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (claimStateType.StateType != null && claimStateType.StateType.Length > 255)
                {
                    MessageBox.Show(@"Длина наименования вида состояния претензионно-исковой работы не может превышать 255 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static ClaimStateType RowToClaimStateType(DataRow row)
        {
            var сlaimStateType = new ClaimStateType
            {
                IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type"),
                StateType = ViewportHelper.ValueOrNull(row, "state_type"),
                IsStartStateType = ViewportHelper.ValueOrNull<bool>(row, "is_start_state_type")
            };
            return сlaimStateType;
        }

        private List<ClaimStateType> ClaimStateTypesFromViewport()
        {
            var list = new List<ClaimStateType>();
            for (var i = 0; i < dataGridViewClaimStateTypes.Rows.Count; i++)
            {
                if (dataGridViewClaimStateTypes.Rows[i].IsNewRow) continue;
                var cst = new ClaimStateType();
                var row = dataGridViewClaimStateTypes.Rows[i];
                cst.IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type");
                cst.StateType = ViewportHelper.ValueOrNull(row, "state_type");
                cst.IsStartStateType = (ViewportHelper.ValueOrNull<bool>(row, "is_start_state_type") == true);
                list.Add(cst);
            }
            return list;
        }

        private List<ClaimStateType> ClaimStateTypesFromView()
        {
            var list = new List<ClaimStateType>();
            foreach (var claimStateType in GeneralBindingSource)
            {
                var cst = new ClaimStateType();
                var row = ((DataRowView)claimStateType);
                cst.IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type");
                cst.StateType = ViewportHelper.ValueOrNull(row, "state_type");
                cst.IsStartStateType = (ViewportHelper.ValueOrNull<bool>(row, "is_start_state_type") == true);
                list.Add(cst);
            }
            return list;
        }

        private List<ClaimStateTypeRelation> ClaimStateTypesRelationsFromViewport()
        {
            var list = new List<ClaimStateTypeRelation>();
            for (var i = 0; i < snapshot_claim_state_types_relations.Rows.Count; i++)
            {
                var row = snapshot_claim_state_types_relations.Rows[i];
                if (row["checked"] == DBNull.Value || 
                    (Convert.ToBoolean(row["checked"], CultureInfo.InvariantCulture) != true))
                    continue;
                var cstr = new ClaimStateTypeRelation
                {
                    IdRelation = ViewportHelper.ValueOrNull<int>(row, "id_relation"),
                    IdStateFrom = ViewportHelper.ValueOrNull<int>(row, "id_state_from"),
                    IdStateTo = ViewportHelper.ValueOrNull<int>(row, "id_state_to")
                };
                list.Add(cstr);
            }
            return list;
        }

        private List<ClaimStateTypeRelation> ClaimStateTypesRelationsFromView()
        {
            var list = new List<ClaimStateTypeRelation>();
            foreach (var claimStateRel in v_claim_state_types_relations)
            {
                var row = ((DataRowView)claimStateRel);
                var cstr = new ClaimStateTypeRelation
                {
                    IdRelation = ViewportHelper.ValueOrNull<int>(row, "id_relation"),
                    IdStateFrom = ViewportHelper.ValueOrNull<int>(row, "id_state_from"),
                    IdStateTo = ViewportHelper.ValueOrNull<int>(row, "id_state_to")
                };
                list.Add(cstr);
            }
            return list;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridViewClaimStateTypes.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.ClaimStateTypesDataModel);
            claim_state_types_relations = DataModel.GetInstance(DataModelType.ClaimStateTypesRelationsDataModel);
            //Ожиданем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            claim_state_types_relations.Select();

            // Инициализируем snapshot-модель relations
            snapshot_claim_state_types_relations = new DataTable("snapshot_claim_state_types_relations")
            {
                Locale = CultureInfo.InvariantCulture
            };
            snapshot_claim_state_types_relations.Columns.Add("id_relation").DataType = typeof(int);
            snapshot_claim_state_types_relations.Columns.Add("id_state_from").DataType = typeof(int);
            snapshot_claim_state_types_relations.Columns.Add("id_state_to").DataType = typeof(int);
            snapshot_claim_state_types_relations.Columns.Add("checked").DataType = typeof(bool);

            GeneralBindingSource = new BindingSource
            {
                DataMember = "claim_state_types",
                DataSource = DataModel.DataSet
            };

            v_claim_state_types_from = new BindingSource
            {
                DataMember = "claim_state_types",
                DataSource = DataModel.DataSet
            };
            v_claim_state_types_from.CurrentItemChanged += v_claim_state_types_from_CurrentItemChanged;

            v_claim_state_types_relations = new BindingSource
            {
                DataMember = "claim_state_types_relations",
                DataSource = DataModel.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(
                    GeneralDataModel.Select().Columns[i].ColumnName, GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            foreach (var claimStateType in GeneralBindingSource)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)claimStateType)));
            GeneralSnapshotBindingSource = new BindingSource { DataSource = GeneralSnapshot };
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_claim_state_types_CurrentItemChanged;

            //Загружаем данные snapshot-модели из original-view relations
            foreach (object claimStateRel in v_claim_state_types_relations)
                snapshot_claim_state_types_relations.Rows.Add(DataRowViewRelationToArray((DataRowView)claimStateRel));
            v_snapshot_claim_state_types_relations = new BindingSource
            {
                DataSource = snapshot_claim_state_types_relations
            };

            dataGridViewClaimStateTypes.DataSource = GeneralSnapshotBindingSource;
            id_state_type.DataPropertyName = "id_state_type";
            state_type.DataPropertyName = "state_type";
            is_start_state_type.DataPropertyName = "is_start_state_type";
            dataGridViewClaimStateTypes.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridViewClaimStateTypesFrom.RowCount = GeneralSnapshotBindingSource.Count;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridViewClaimStateTypes.CellValueChanged += dataGridViewClaimStateTypes_CellValueChanged;
            dataGridViewClaimStateTypesFrom.CellValueChanged += dataGridViewClaimStateTypesFrom_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += ClaimStateTypesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += ClaimStateTypesViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += ClaimStateTypesViewport_RowDeleted;
            claim_state_types_relations.Select().RowChanged += ClaimStateTypesRelationsViewport_RowChanged;
            claim_state_types_relations.Select().RowDeleting += ClaimStateTypesRelationsViewport_RowDeleting;
            claim_state_types_relations.Select().RowDeleted += ClaimStateTypesRelationsViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return AccessControl.HasPrivelege(Priveleges.ClaimsDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            if (row == null) return;
            row["id_state_type"] = temp_id_state_type--;
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralSnapshotBindingSource.Position != -1) && AccessControl.HasPrivelege(Priveleges.ClaimsDirectoriesReadWrite);
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
            foreach (var claimStateType in GeneralBindingSource)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)claimStateType)));
            temp_id_state_type = int.MaxValue;
            snapshot_claim_state_types_relations.Clear();
            foreach (var claimStateRel in v_claim_state_types_relations)
                snapshot_claim_state_types_relations.Rows.Add(DataRowViewRelationToArray(((DataRowView)claimStateRel)));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.ClaimsDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            dataGridViewClaimStateTypes.EndEdit();
            GeneralDataModel.EditingNewRecord = true;
            // Сохраняем общую информацию о видах состояний
            var list = ClaimStateTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = GeneralDataModel.Select().Rows.Find(list[i].IdStateType);
                if (row == null)
                {
                    var idStateType = GeneralDataModel.Insert(list[i]);
                    if (idStateType == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_state_type"] = idStateType;
                    foreach (var claimStateRel in v_snapshot_claim_state_types_relations)
                        if ((int)((DataRowView)claimStateRel)["id_state_to"] == list[i].IdStateType)
                            ((DataRowView)claimStateRel)["id_state_to"] = idStateType;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToClaimStateType(row) == list[i])
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["state_type"] = list[i].StateType == null ? DBNull.Value : (object)list[i].StateType;
                    row["is_start_state_type"] = list[i].IsStartStateType == null ? DBNull.Value : (object)list[i].IsStartStateType;
                }
            }
            //Удаляем виды состояний из модели и базы
            list = ClaimStateTypesFromView();
            foreach (var claimStateType in list)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridViewClaimStateTypes.Rows.Count; j++)
                    if ((dataGridViewClaimStateTypes.Rows[j].Cells["id_state_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridViewClaimStateTypes.Rows[j].Cells["id_state_type"].Value.ToString()) &&
                        ((int)dataGridViewClaimStateTypes.Rows[j].Cells["id_state_type"].Value == claimStateType.IdStateType))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (claimStateType.IdStateType != null && GeneralDataModel.Delete(claimStateType.IdStateType.Value) == -1)
                {
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                GeneralDataModel.Select().Rows.Find(claimStateType.IdStateType).Delete();
                //Рекурсивно удаляем зависимости из снапшот-модели
                for (var j = snapshot_claim_state_types_relations.Rows.Count - 1; j >= 0; j--)
                    if (Convert.ToInt32(snapshot_claim_state_types_relations.Rows[j]["id_state_from"], CultureInfo.InvariantCulture) == 
                        claimStateType.IdStateType ||
                        Convert.ToInt32(snapshot_claim_state_types_relations.Rows[j]["id_state_to"], CultureInfo.InvariantCulture) == 
                        claimStateType.IdStateType)
                        snapshot_claim_state_types_relations.Rows[j].Delete();
            }
            // Сохраняем возможные переходы
            var listRelations = ClaimStateTypesRelationsFromViewport();
            foreach (var claimStateRel in listRelations)
            {
                var row = claim_state_types_relations.Select().Rows.Find(claimStateRel.IdRelation);
                if (row != null) continue;
                var idRelation = claim_state_types_relations.Insert(claimStateRel);
                if (idRelation == -1)
                {
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                var relRow = (from snapshotRow in snapshot_claim_state_types_relations.AsEnumerable()
                    where (snapshotRow.RowState != DataRowState.Deleted) &&
                          (snapshotRow.RowState != DataRowState.Detached) &&
                          snapshotRow.Field<int>("id_state_from") == claimStateRel.IdStateFrom &&
                          snapshotRow.Field<int>("id_state_to") == claimStateRel.IdStateTo
                    select snapshotRow).First();
                relRow["id_relation"] = idRelation;
                claim_state_types_relations.Select().Rows.Add(
                    idRelation,
                    claimStateRel.IdStateFrom,
                    claimStateRel.IdStateTo);
            }
            listRelations = ClaimStateTypesRelationsFromView();
            foreach (var claimStateRel in listRelations)
            {
                var rowIndex = -1;
                for (var j = 0; j < v_snapshot_claim_state_types_relations.Count; j++)
                {
                    var row = (DataRowView)v_snapshot_claim_state_types_relations[j];
                    if ((row["id_relation"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_relation"].ToString()) &&
                        ((int)row["id_relation"] == claimStateRel.IdRelation) &&
                        Convert.ToBoolean(row["checked"], CultureInfo.InvariantCulture))
                        rowIndex = j;
                }
                if (rowIndex == -1)
                {
                    if (claimStateRel.IdRelation != null && claim_state_types_relations.Delete(claimStateRel.IdRelation.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    claim_state_types_relations.Select().Rows.Find(claimStateRel.IdRelation).Delete();
                }
            }
            sync_views = true;
            GeneralDataModel.EditingNewRecord = false;
            dataGridViewClaimStateTypesFrom.RowCount = v_claim_state_types_from.Count;
            dataGridViewClaimStateTypesFrom.Refresh();          
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new ClaimStateTypesViewport(this, MenuCallback);
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
            v_claim_state_types_from.CurrentItemChanged -= v_claim_state_types_from_CurrentItemChanged;
            GeneralSnapshotBindingSource.CurrentItemChanged -= v_snapshot_claim_state_types_CurrentItemChanged;
            dataGridViewClaimStateTypes.CellValueChanged -= dataGridViewClaimStateTypes_CellValueChanged;
            dataGridViewClaimStateTypesFrom.CellValueChanged -= dataGridViewClaimStateTypesFrom_CellValueChanged;
            GeneralDataModel.Select().RowChanged -= ClaimStateTypesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= ClaimStateTypesViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= ClaimStateTypesViewport_RowDeleted;
            claim_state_types_relations.Select().RowChanged -= ClaimStateTypesRelationsViewport_RowChanged;
            claim_state_types_relations.Select().RowDeleting -= ClaimStateTypesRelationsViewport_RowDeleting;
            claim_state_types_relations.Select().RowDeleted -= ClaimStateTypesRelationsViewport_RowDeleted;
            base.OnClosing(e);
        }

        void v_claim_state_types_from_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_claim_state_types_from.Position == -1 || dataGridViewClaimStateTypesFrom.RowCount == 0)
            {
                dataGridViewClaimStateTypesFrom.ClearSelection();
                return;
            }
            if (v_claim_state_types_from.Position >= dataGridViewClaimStateTypesFrom.RowCount)
            {
                dataGridViewClaimStateTypesFrom.Rows[dataGridViewClaimStateTypesFrom.RowCount - 1].Selected = true;
                dataGridViewClaimStateTypesFrom.CurrentCell = dataGridViewClaimStateTypesFrom.Rows[dataGridViewClaimStateTypesFrom.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridViewClaimStateTypesFrom.Rows[v_claim_state_types_from.Position].Selected = true;
                dataGridViewClaimStateTypesFrom.CurrentCell = dataGridViewClaimStateTypesFrom.Rows[v_claim_state_types_from.Position].Cells[0];
            }
        }

        void v_snapshot_claim_state_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
            var stateTypeCheckedColumn = dataGridViewClaimStateTypesFrom.Columns["state_type_checked"];
            if (stateTypeCheckedColumn != null)
                dataGridViewClaimStateTypesFrom.InvalidateColumn(stateTypeCheckedColumn.Index);
        }

        void dataGridViewClaimStateTypesFrom_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridViewClaimStateTypesFrom.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                v_claim_state_types_from.Sort = dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].Name + " " + 
                    ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection ==
                             SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridViewClaimStateTypesFrom.Refresh();
        }

        void dataGridViewClaimStateTypesFrom_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewClaimStateTypesFrom.SelectedRows.Count > 0)
                v_claim_state_types_from.Position = dataGridViewClaimStateTypesFrom.SelectedRows[0].Index;
            else
                v_claim_state_types_from.Position = -1;
        }

        void dataGridViewClaimStateTypesFrom_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_claim_state_types_from.Count <= e.RowIndex || v_claim_state_types_from.Count == 0 ||
                GeneralSnapshotBindingSource.Position == -1) return;
            var idStateType = Convert.ToInt32(
                ((DataRowView)GeneralSnapshotBindingSource[GeneralSnapshotBindingSource.Position])["id_state_type"], CultureInfo.InvariantCulture);
            var row = ((DataRowView)v_claim_state_types_from[e.RowIndex]);
            var rowCount = (from relRow in snapshot_claim_state_types_relations.AsEnumerable()
                             where relRow.Field<int?>("id_state_from") == (int)row["id_state_type"]  &&
                                relRow.Field<int?>("id_state_to") == idStateType &&
                                relRow.Field<bool>("checked")
                             select relRow).Count();
                v_snapshot_claim_state_types_relations.Find("id_state_to", idStateType);
            switch (dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].Name)
            {
                case "state_type_checked":
                    e.Value = rowCount > 0;
                    break;
                case "id_state_type_from":
                    e.Value = row["id_state_type"];
                    break;
                case "state_type_from":
                    e.Value = row["state_type"];
                    break;
            }
        }

        void dataGridViewClaimStateTypesFrom_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_claim_state_types_from.Count <= e.RowIndex || v_claim_state_types_from.Count == 0) return;
            var idStateType = Convert.ToInt32(
                ((DataRowView)GeneralSnapshotBindingSource[GeneralSnapshotBindingSource.Position])["id_state_type"], CultureInfo.InvariantCulture);
            var row = ((DataRowView)v_claim_state_types_from[e.RowIndex]);
            var rows = from relRow in snapshot_claim_state_types_relations.AsEnumerable()
                       where relRow.Field<int>("id_state_from") == (int)row["id_state_type"] &&
                        relRow.Field<int>("id_state_to") == idStateType
                       select relRow;
            sync_views = false;
            switch (dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].Name)
            {
                case "state_type_checked":  
                    if (!rows.Any())
                    {
                        snapshot_claim_state_types_relations.Rows.Add(null, row["id_state_type"], idStateType, e.Value);
                    }
                    else
                        foreach (var relRow in rows)
                            relRow["checked"] = e.Value;
                    break;
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        private void ClaimStateTypesRelationsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }

        void ClaimStateTypesRelationsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action != DataRowAction.Delete) return;
            var rows = from relRow in snapshot_claim_state_types_relations.AsEnumerable()
                where relRow.Field<int>("id_state_from") == (int)e.Row["id_state_from"] &&
                      relRow.Field<int>("id_state_to") == (int)e.Row["id_state_to"]
                select relRow;
            for (var i = (rows.Count() - 1); i >= 0; i--)
                rows.ElementAt(i).Delete();
            var stateTypeCheckedColumn = dataGridViewClaimStateTypesFrom.Columns["state_type_checked"];
            if (stateTypeCheckedColumn != null)
                dataGridViewClaimStateTypesFrom.InvalidateColumn(stateTypeCheckedColumn.Index);
        }

        void ClaimStateTypesRelationsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = v_snapshot_claim_state_types_relations.Find("id_relation", e.Row["id_relation"]);
            if (rowIndex == -1 && v_claim_state_types_relations.Find("id_relation", e.Row["id_relation"]) != -1)
            {
                snapshot_claim_state_types_relations.Rows.Add(e.Row["id_relation"], e.Row["id_state_from"], e.Row["id_state_to"], true);
                dataGridViewClaimStateTypesFrom.RowCount = v_claim_state_types_from.Count;
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)v_snapshot_claim_state_types_relations[rowIndex]);
                    row["id_state_from"] = e.Row["id_state_from"];
                    row["id_state_to"] = e.Row["id_state_to"];
                }
            var stateTypeCheckedColumn = dataGridViewClaimStateTypesFrom.Columns["state_type_checked"];
            if (stateTypeCheckedColumn != null)
                dataGridViewClaimStateTypesFrom.InvalidateColumn(stateTypeCheckedColumn.Index);
        }

        void ClaimStateTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void ClaimStateTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = GeneralSnapshotBindingSource.Find("id_state_type", e.Row["id_state_type"]);
                if (rowIndex != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
                dataGridViewClaimStateTypesFrom.RowCount = dataGridViewClaimStateTypesFrom.RowCount - 1;
                dataGridViewClaimStateTypesFrom.Invalidate();
            }
        }

        void ClaimStateTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_state_type", e.Row["id_state_type"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_state_type", e.Row["id_state_type"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_state_type"], e.Row["state_type"], e.Row["is_start_state_type"]);
                dataGridViewClaimStateTypesFrom.RowCount = dataGridViewClaimStateTypesFrom.RowCount + 1;
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
                    row["state_type"] = e.Row["state_type"];
                    row["is_start_state_type"] = e.Row["is_start_state_type"];
                }
            dataGridViewClaimStateTypesFrom.Invalidate();
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridViewClaimStateTypes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void dataGridViewClaimStateTypesFrom_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void dataGridViewClaimStateTypes_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridViewClaimStateTypes.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "state_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования вида состояния претензионно-исковой работы не может превышать 255 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название вида состояния претензионно-исковой работы не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }
    }
}
