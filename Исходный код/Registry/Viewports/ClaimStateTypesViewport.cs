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
    internal sealed class ClaimStateTypesViewport: Viewport
    {
        #region Components
        TableLayoutPanel tableLayoutPanel19;
        GroupBox groupBox36;
        GroupBox groupBox37;
        DataGridView dataGridViewClaimStateTypes;
        DataGridViewTextBoxColumn id_state_type;
        DataGridViewCheckBoxColumn is_start_state_type;
        DataGridViewTextBoxColumn state_type;
        DataGridView dataGridViewClaimStateTypesFrom;
        private DataGridViewCheckBoxColumn state_type_checked;
        private DataGridViewTextBoxColumn id_relation;
        private DataGridViewTextBoxColumn id_state_type_from;
        private DataGridViewTextBoxColumn state_type_from;
        #endregion Components

        #region Models
        DataModel claim_state_types;
        DataModel claim_state_types_relations;
        DataTable snapshot_claim_state_types = new DataTable("snapshot_claim_state_types");
        DataTable snapshot_claim_state_types_relations = new DataTable("snapshot_claim_state_types_relations");
        #endregion Models

        #region Views
        BindingSource v_claim_state_types;
        BindingSource v_claim_state_types_from;
        BindingSource v_claim_state_types_relations;
        BindingSource v_snapshot_claim_state_types;
        BindingSource v_snapshot_claim_state_types_relations;
        #endregion Views

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;
        int temp_id_state_type = int.MaxValue;

        private ClaimStateTypesViewport()
            : this(null)
        {
        }

        public ClaimStateTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshot_claim_state_types.Locale = CultureInfo.InvariantCulture;
            snapshot_claim_state_types_relations.Locale = CultureInfo.InvariantCulture;
        }

        public ClaimStateTypesViewport(ClaimStateTypesViewport claimStateTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        private bool SnapshotHasChanges()
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
            foreach (var claimStateType in v_claim_state_types)
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

        public override int GetRecordCount()
        {
            return v_snapshot_claim_state_types.Count;
        }

        public override void MoveFirst()
        {
            v_snapshot_claim_state_types.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_claim_state_types.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_claim_state_types.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_claim_state_types.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_claim_state_types.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_claim_state_types.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_claim_state_types.Position > -1) && (v_snapshot_claim_state_types.Position < (v_snapshot_claim_state_types.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_claim_state_types.Position > -1) && (v_snapshot_claim_state_types.Position < (v_snapshot_claim_state_types.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridViewClaimStateTypes.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            claim_state_types = DataModel.GetInstance(DataModelType.ClaimStateTypesDataModel);
            claim_state_types_relations = DataModel.GetInstance(DataModelType.ClaimStateTypesRelationsDataModel);
            //Ожиданем дозагрузки данных, если это необходимо
            claim_state_types.Select();
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

            v_claim_state_types = new BindingSource
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
            for (var i = 0; i < claim_state_types.Select().Columns.Count; i++)
                snapshot_claim_state_types.Columns.Add(new DataColumn(
                    claim_state_types.Select().Columns[i].ColumnName, claim_state_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            foreach (var claimStateType in v_claim_state_types)
                snapshot_claim_state_types.Rows.Add(DataRowViewToArray(((DataRowView)claimStateType)));
            v_snapshot_claim_state_types = new BindingSource {DataSource = snapshot_claim_state_types};
            v_snapshot_claim_state_types.CurrentItemChanged += v_snapshot_claim_state_types_CurrentItemChanged;

            //Загружаем данные snapshot-модели из original-view relations
            foreach (object claimStateRel in v_claim_state_types_relations)
                snapshot_claim_state_types_relations.Rows.Add(DataRowViewRelationToArray((DataRowView)claimStateRel));
            v_snapshot_claim_state_types_relations = new BindingSource
            {
                DataSource = snapshot_claim_state_types_relations
            };

            dataGridViewClaimStateTypes.DataSource = v_snapshot_claim_state_types;
            id_state_type.DataPropertyName = "id_state_type";
            state_type.DataPropertyName = "state_type";
            is_start_state_type.DataPropertyName = "is_start_state_type";
            dataGridViewClaimStateTypes.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridViewClaimStateTypesFrom.RowCount = v_snapshot_claim_state_types.Count;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridViewClaimStateTypes.CellValueChanged += dataGridViewClaimStateTypes_CellValueChanged;
            dataGridViewClaimStateTypesFrom.CellValueChanged += dataGridViewClaimStateTypesFrom_CellValueChanged;
            //Синхронизация данных исходные->текущие
            claim_state_types.Select().RowChanged += ClaimStateTypesViewport_RowChanged;
            claim_state_types.Select().RowDeleting += ClaimStateTypesViewport_RowDeleting;
            claim_state_types.Select().RowDeleted += ClaimStateTypesViewport_RowDeleted;
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
            var row = (DataRowView)v_snapshot_claim_state_types.AddNew();
            if (row == null) return;
            row["id_state_type"] = temp_id_state_type--;
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_claim_state_types.Position != -1) && AccessControl.HasPrivelege(Priveleges.ClaimsDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_claim_state_types[v_snapshot_claim_state_types.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_claim_state_types.Clear();
            foreach (var claimStateType in v_claim_state_types)
                snapshot_claim_state_types.Rows.Add(DataRowViewToArray(((DataRowView)claimStateType)));
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
            claim_state_types.EditingNewRecord = true;
            // Сохраняем общую информацию о видах состояний
            var list = ClaimStateTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                claim_state_types.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = claim_state_types.Select().Rows.Find(list[i].IdStateType);
                if (row == null)
                {
                    var idStateType = claim_state_types.Insert(list[i]);
                    if (idStateType == -1)
                    {
                        sync_views = true;
                        claim_state_types.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_claim_state_types[i])["id_state_type"] = idStateType;
                    foreach (var claimStateRel in v_snapshot_claim_state_types_relations)
                        if ((int)((DataRowView)claimStateRel)["id_state_to"] == list[i].IdStateType)
                            ((DataRowView)claimStateRel)["id_state_to"] = idStateType;
                    claim_state_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_claim_state_types[i]));
                }
                else
                {
                    if (RowToClaimStateType(row) == list[i])
                        continue;
                    if (claim_state_types.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        claim_state_types.EditingNewRecord = false;
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
                if (claimStateType.IdStateType != null && claim_state_types.Delete(claimStateType.IdStateType.Value) == -1)
                {
                    sync_views = true;
                    claim_state_types.EditingNewRecord = false;
                    return;
                }
                claim_state_types.Select().Rows.Find(claimStateType.IdStateType).Delete();
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
                    claim_state_types.EditingNewRecord = false;
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
                        claim_state_types.EditingNewRecord = false;
                        return;
                    }
                    claim_state_types_relations.Select().Rows.Find(claimStateRel.IdRelation).Delete();
                }
            }
            sync_views = true;
            claim_state_types.EditingNewRecord = false;
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
            claim_state_types.Select().RowChanged -= ClaimStateTypesViewport_RowChanged;
            claim_state_types.Select().RowDeleting -= ClaimStateTypesViewport_RowDeleting;
            claim_state_types.Select().RowDeleted -= ClaimStateTypesViewport_RowDeleted;
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
                v_snapshot_claim_state_types.Position == -1) return;
            var idStateType = Convert.ToInt32(
                ((DataRowView)v_snapshot_claim_state_types[v_snapshot_claim_state_types.Position])["id_state_type"], CultureInfo.InvariantCulture);
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
                ((DataRowView)v_snapshot_claim_state_types[v_snapshot_claim_state_types.Position])["id_state_type"], CultureInfo.InvariantCulture);
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
                var rowIndex = v_snapshot_claim_state_types.Find("id_state_type", e.Row["id_state_type"]);
                if (rowIndex != -1)
                    ((DataRowView)v_snapshot_claim_state_types[rowIndex]).Delete();
                dataGridViewClaimStateTypesFrom.RowCount = dataGridViewClaimStateTypesFrom.RowCount - 1;
                dataGridViewClaimStateTypesFrom.Invalidate();
            }
        }

        void ClaimStateTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = v_snapshot_claim_state_types.Find("id_state_type", e.Row["id_state_type"]);
            if (rowIndex == -1 && v_claim_state_types.Find("id_state_type", e.Row["id_state_type"]) != -1)
            {
                snapshot_claim_state_types.Rows.Add(e.Row["id_state_type"], e.Row["state_type"], e.Row["is_start_state_type"]);
                dataGridViewClaimStateTypesFrom.RowCount = dataGridViewClaimStateTypesFrom.RowCount + 1;
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)v_snapshot_claim_state_types[rowIndex]);
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

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(ClaimStateTypesViewport));
            tableLayoutPanel19 = new TableLayoutPanel();
            groupBox36 = new GroupBox();
            dataGridViewClaimStateTypes = new DataGridView();
            id_state_type = new DataGridViewTextBoxColumn();
            is_start_state_type = new DataGridViewCheckBoxColumn();
            state_type = new DataGridViewTextBoxColumn();
            groupBox37 = new GroupBox();
            dataGridViewClaimStateTypesFrom = new DataGridView();
            state_type_checked = new DataGridViewCheckBoxColumn();
            id_relation = new DataGridViewTextBoxColumn();
            id_state_type_from = new DataGridViewTextBoxColumn();
            state_type_from = new DataGridViewTextBoxColumn();
            tableLayoutPanel19.SuspendLayout();
            groupBox36.SuspendLayout();
            ((ISupportInitialize)(dataGridViewClaimStateTypes)).BeginInit();
            groupBox37.SuspendLayout();
            ((ISupportInitialize)(dataGridViewClaimStateTypesFrom)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel19
            // 
            tableLayoutPanel19.ColumnCount = 2;
            tableLayoutPanel19.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel19.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel19.Controls.Add(groupBox36, 0, 0);
            tableLayoutPanel19.Controls.Add(groupBox37, 1, 0);
            tableLayoutPanel19.Dock = DockStyle.Fill;
            tableLayoutPanel19.Location = new Point(3, 3);
            tableLayoutPanel19.Name = "tableLayoutPanel19";
            tableLayoutPanel19.RowCount = 1;
            tableLayoutPanel19.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel19.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel19.Size = new Size(707, 427);
            tableLayoutPanel19.TabIndex = 0;
            // 
            // groupBox36
            // 
            groupBox36.Controls.Add(dataGridViewClaimStateTypes);
            groupBox36.Dock = DockStyle.Fill;
            groupBox36.Location = new Point(3, 3);
            groupBox36.Name = "groupBox36";
            groupBox36.Size = new Size(347, 421);
            groupBox36.TabIndex = 0;
            groupBox36.TabStop = false;
            groupBox36.Text = @"Состояния";
            // 
            // dataGridViewClaimStateTypes
            // 
            dataGridViewClaimStateTypes.AllowUserToAddRows = false;
            dataGridViewClaimStateTypes.AllowUserToDeleteRows = false;
            dataGridViewClaimStateTypes.AllowUserToResizeRows = false;
            dataGridViewClaimStateTypes.BackgroundColor = Color.White;
            dataGridViewClaimStateTypes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewClaimStateTypes.Columns.AddRange(id_state_type, is_start_state_type, state_type);
            dataGridViewClaimStateTypes.Dock = DockStyle.Fill;
            dataGridViewClaimStateTypes.Location = new Point(3, 17);
            dataGridViewClaimStateTypes.Name = "dataGridViewClaimStateTypes";
            dataGridViewClaimStateTypes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewClaimStateTypes.Size = new Size(341, 401);
            dataGridViewClaimStateTypes.TabIndex = 0;
            dataGridViewClaimStateTypes.CellValidated += dataGridViewClaimStateTypes_CellValidated;
            // 
            // id_state_type
            // 
            id_state_type.HeaderText = @"Идентификатор состояния";
            id_state_type.Name = "id_state_type";
            id_state_type.Visible = false;
            // 
            // is_start_state_type
            // 
            is_start_state_type.HeaderText = @"Начальное";
            is_start_state_type.MinimumWidth = 70;
            is_start_state_type.Name = "is_start_state_type";
            is_start_state_type.Resizable = DataGridViewTriState.True;
            is_start_state_type.SortMode = DataGridViewColumnSortMode.Automatic;
            is_start_state_type.Width = 70;
            // 
            // state_type
            // 
            state_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            state_type.HeaderText = @"Наименование вида состояния";
            state_type.Name = "state_type";
            state_type.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // groupBox37
            // 
            groupBox37.Controls.Add(dataGridViewClaimStateTypesFrom);
            groupBox37.Dock = DockStyle.Fill;
            groupBox37.Location = new Point(356, 3);
            groupBox37.Name = "groupBox37";
            groupBox37.Size = new Size(348, 421);
            groupBox37.TabIndex = 1;
            groupBox37.TabStop = false;
            groupBox37.Text = @"Разрешены переходы из";
            // 
            // dataGridViewClaimStateTypesFrom
            // 
            dataGridViewClaimStateTypesFrom.AllowUserToAddRows = false;
            dataGridViewClaimStateTypesFrom.AllowUserToDeleteRows = false;
            dataGridViewClaimStateTypesFrom.AllowUserToResizeRows = false;
            dataGridViewClaimStateTypesFrom.BackgroundColor = Color.White;
            dataGridViewClaimStateTypesFrom.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewClaimStateTypesFrom.Columns.AddRange(state_type_checked, id_relation, id_state_type_from, state_type_from);
            dataGridViewClaimStateTypesFrom.Dock = DockStyle.Fill;
            dataGridViewClaimStateTypesFrom.Location = new Point(3, 17);
            dataGridViewClaimStateTypesFrom.Name = "dataGridViewClaimStateTypesFrom";
            dataGridViewClaimStateTypesFrom.Size = new Size(342, 401);
            dataGridViewClaimStateTypesFrom.TabIndex = 0;
            dataGridViewClaimStateTypesFrom.VirtualMode = true;
            dataGridViewClaimStateTypesFrom.CellValueNeeded += dataGridViewClaimStateTypesFrom_CellValueNeeded;
            dataGridViewClaimStateTypesFrom.CellValuePushed += dataGridViewClaimStateTypesFrom_CellValuePushed;
            dataGridViewClaimStateTypesFrom.ColumnHeaderMouseClick += dataGridViewClaimStateTypesFrom_ColumnHeaderMouseClick;
            dataGridViewClaimStateTypesFrom.SelectionChanged += dataGridViewClaimStateTypesFrom_SelectionChanged;
            // 
            // state_type_checked
            // 
            state_type_checked.HeaderText = "";
            state_type_checked.MinimumWidth = 70;
            state_type_checked.Name = "state_type_checked";
            state_type_checked.Resizable = DataGridViewTriState.False;
            state_type_checked.Width = 70;
            // 
            // id_relation
            // 
            id_relation.HeaderText = @"Идентификатор отношения";
            id_relation.Name = "id_relation";
            id_relation.Visible = false;
            // 
            // id_state_type_from
            // 
            id_state_type_from.HeaderText = @"Идентификатор состояния";
            id_state_type_from.Name = "id_state_type_from";
            id_state_type_from.Visible = false;
            // 
            // state_type_from
            // 
            state_type_from.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(224, 224, 224);
            state_type_from.DefaultCellStyle = dataGridViewCellStyle1;
            state_type_from.HeaderText = @"Наименование вида состояния";
            state_type_from.Name = "state_type_from";
            state_type_from.ReadOnly = true;
            // 
            // ClaimStateTypesViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(713, 433);
            Controls.Add(tableLayoutPanel19);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ClaimStateTypesViewport";
            Padding = new Padding(3);
            Text = @"Виды состояний иск. работы";
            tableLayoutPanel19.ResumeLayout(false);
            groupBox36.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewClaimStateTypes)).EndInit();
            groupBox37.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewClaimStateTypesFrom)).EndInit();
            ResumeLayout(false);

        }
    }
}
