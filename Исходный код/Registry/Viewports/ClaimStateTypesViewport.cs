using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;
using System.Drawing;
using Security;
using System.Globalization;

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
        #endregion Components

        #region Models
        ClaimStateTypesDataModel claim_state_types = null;
        ClaimStateTypesRelationsDataModel claim_state_types_relations = null;
        DataTable snapshot_claim_state_types = new DataTable("snapshot_claim_state_types");
        DataTable snapshot_claim_state_types_relations = new DataTable("snapshot_claim_state_types_relations");
        #endregion Models

        #region Views
        BindingSource v_claim_state_types = null;
        BindingSource v_claim_state_types_from = null;
        BindingSource v_claim_state_types_relations = null;
        BindingSource v_snapshot_claim_state_types = null;
        BindingSource v_snapshot_claim_state_types_relations = null;
        #endregion Views

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;
        private DataGridViewCheckBoxColumn state_type_checked;
        private DataGridViewTextBoxColumn id_relation;
        private DataGridViewTextBoxColumn id_state_type_from;
        private DataGridViewTextBoxColumn state_type_from;
        int temp_id_state_type = Int32.MaxValue;

        private ClaimStateTypesViewport()
            : this(null)
        {
        }

        public ClaimStateTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshot_claim_state_types.Locale = CultureInfo.CurrentCulture;
            snapshot_claim_state_types_relations.Locale = CultureInfo.CurrentCulture;
        }

        public ClaimStateTypesViewport(ClaimStateTypesViewport claimStateTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        private bool SnapshotHasChanges()
        {
            List<ClaimStateType> list_from_view = ClaimStateTypesFromView();
            List<ClaimStateType> list_from_viewport = ClaimStateTypesFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            bool founded = false;
            for (int i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (int j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            List<ClaimStateTypeRelation> list_from_view_rel = ClaimStateTypesRelationsFromView();
            List<ClaimStateTypeRelation> list_from_viewport_rel = ClaimStateTypesRelationsFromViewport();
            if (list_from_view_rel.Count != list_from_viewport_rel.Count)
                return true;
            founded = false;
            for (int i = 0; i < list_from_view_rel.Count; i++)
            {
                founded = false;
                for (int j = 0; j < list_from_viewport_rel.Count; j++)
                    if (list_from_view_rel[i] == list_from_viewport_rel[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_state_type"], 
                dataRowView["state_type"], 
                ViewportHelper.ValueOrNull<bool>(dataRowView,"is_start_state_type") == true
            };
        }

        private static object[] DataRowViewRelationToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_relation"], 
                dataRowView["id_state_from"],
                dataRowView["id_state_to"],
                true
            };
        }

        private static bool ValidateViewportData(List<ClaimStateType> list)
        {
            foreach (ClaimStateType claimStateType in list)
            {
                if (claimStateType.StateType == null)
                {
                    MessageBox.Show("Название вида состояния претензионно-исковой работы не может быть пустым", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (claimStateType.StateType != null && claimStateType.StateType.Length > 255)
                {
                    MessageBox.Show("Длина наименования вида состояния претензионно-исковой работы не может превышать 255 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static ClaimStateType RowToClaimStateType(DataRow row)
        {
            ClaimStateType сlaimStateType = new ClaimStateType();
            сlaimStateType.IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type");
            сlaimStateType.StateType = ViewportHelper.ValueOrNull(row, "state_type");
            сlaimStateType.IsStartStateType = ViewportHelper.ValueOrNull<bool>(row, "is_start_state_type");
            return сlaimStateType;
        }

        private List<ClaimStateType> ClaimStateTypesFromViewport()
        {
            List<ClaimStateType> list = new List<ClaimStateType>();
            for (int i = 0; i < dataGridViewClaimStateTypes.Rows.Count; i++)
            {
                if (!dataGridViewClaimStateTypes.Rows[i].IsNewRow)
                {
                    ClaimStateType cst = new ClaimStateType();
                    DataGridViewRow row = dataGridViewClaimStateTypes.Rows[i];
                    cst.IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type");
                    cst.StateType = ViewportHelper.ValueOrNull(row, "state_type");
                    cst.IsStartStateType = (ViewportHelper.ValueOrNull<bool>(row, "is_start_state_type") == true);
                    list.Add(cst);
                }
            }
            return list;
        }

        private List<ClaimStateType> ClaimStateTypesFromView()
        {
            List<ClaimStateType> list = new List<ClaimStateType>();
            for (int i = 0; i < v_claim_state_types.Count; i++)
            {
                ClaimStateType cst = new ClaimStateType();
                DataRowView row = ((DataRowView)v_claim_state_types[i]);
                cst.IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type");
                cst.StateType = ViewportHelper.ValueOrNull(row, "state_type");
                cst.IsStartStateType = (ViewportHelper.ValueOrNull<bool>(row, "is_start_state_type") == true);
                list.Add(cst);
            }
            return list;
        }

        private List<ClaimStateTypeRelation> ClaimStateTypesRelationsFromViewport()
        {
            List<ClaimStateTypeRelation> list = new List<ClaimStateTypeRelation>();
            for (int i = 0; i < snapshot_claim_state_types_relations.Rows.Count; i++)
            {
                DataRow row = snapshot_claim_state_types_relations.Rows[i];
                if (Convert.ToBoolean(row["checked"], CultureInfo.CurrentCulture) == false)
                    continue;
                ClaimStateTypeRelation cstr = new ClaimStateTypeRelation();
                cstr.IdRelation = ViewportHelper.ValueOrNull<int>(row, "id_relation");
                cstr.IdStateFrom = ViewportHelper.ValueOrNull<int>(row, "id_state_from");
                cstr.IdStateTo = ViewportHelper.ValueOrNull<int>(row, "id_state_to");
                list.Add(cstr);
            }
            return list;
        }

        private List<ClaimStateTypeRelation> ClaimStateTypesRelationsFromView()
        {
            List<ClaimStateTypeRelation> list = new List<ClaimStateTypeRelation>();
            for (int i = 0; i < v_claim_state_types_relations.Count; i++)
            {
                DataRowView row = ((DataRowView)v_claim_state_types_relations[i]);
                ClaimStateTypeRelation cstr = new ClaimStateTypeRelation();
                cstr.IdRelation = ViewportHelper.ValueOrNull<int>(row, "id_relation");
                cstr.IdStateFrom = ViewportHelper.ValueOrNull<int>(row, "id_state_from");
                cstr.IdStateTo = ViewportHelper.ValueOrNull<int>(row, "id_state_to");
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            claim_state_types = ClaimStateTypesDataModel.GetInstance();
            claim_state_types_relations = ClaimStateTypesRelationsDataModel.GetInstance();
            //Ожиданем дозагрузки данных, если это необходимо
            claim_state_types.Select();
            claim_state_types_relations.Select();

            // Инициализируем snapshot-модель relations
            snapshot_claim_state_types_relations = new DataTable("snapshot_claim_state_types_relations");
            snapshot_claim_state_types_relations.Locale = CultureInfo.CurrentCulture;
            snapshot_claim_state_types_relations.Columns.Add("id_relation").DataType = typeof(int);
            snapshot_claim_state_types_relations.Columns.Add("id_state_from").DataType = typeof(int);
            snapshot_claim_state_types_relations.Columns.Add("id_state_to").DataType = typeof(int);
            snapshot_claim_state_types_relations.Columns.Add("checked").DataType = typeof(bool);

            v_claim_state_types = new BindingSource();
            v_claim_state_types.DataMember = "claim_state_types";
            v_claim_state_types.DataSource = DataSetManager.DataSet;

            v_claim_state_types_from = new BindingSource();
            v_claim_state_types_from.DataMember = "claim_state_types";
            v_claim_state_types_from.DataSource = DataSetManager.DataSet;
            v_claim_state_types_from.CurrentItemChanged += new EventHandler(v_claim_state_types_from_CurrentItemChanged);

            v_claim_state_types_relations = new BindingSource();
            v_claim_state_types_relations.DataMember = "claim_state_types_relations";
            v_claim_state_types_relations.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < claim_state_types.Select().Columns.Count; i++)
                snapshot_claim_state_types.Columns.Add(new DataColumn(
                    claim_state_types.Select().Columns[i].ColumnName, claim_state_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_claim_state_types.Count; i++)
                snapshot_claim_state_types.Rows.Add(DataRowViewToArray(((DataRowView)v_claim_state_types[i])));
            v_snapshot_claim_state_types = new BindingSource();
            v_snapshot_claim_state_types.DataSource = snapshot_claim_state_types;
            v_snapshot_claim_state_types.CurrentItemChanged += new EventHandler(v_snapshot_claim_state_types_CurrentItemChanged);

            //Загружаем данные snapshot-модели из original-view relations
            for (int i = 0; i < v_claim_state_types_relations.Count; i++)
                snapshot_claim_state_types_relations.Rows.Add(DataRowViewRelationToArray((DataRowView)v_claim_state_types_relations[i]));
            v_snapshot_claim_state_types_relations = new BindingSource();
            v_snapshot_claim_state_types_relations.DataSource = snapshot_claim_state_types_relations;

            dataGridViewClaimStateTypes.DataSource = v_snapshot_claim_state_types;
            id_state_type.DataPropertyName = "id_state_type";
            state_type.DataPropertyName = "state_type";
            is_start_state_type.DataPropertyName = "is_start_state_type";
            dataGridViewClaimStateTypes.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridViewClaimStateTypesFrom.RowCount = v_snapshot_claim_state_types.Count;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridViewClaimStateTypes.CellValueChanged += new DataGridViewCellEventHandler(dataGridViewClaimStateTypes_CellValueChanged);
            dataGridViewClaimStateTypesFrom.CellValueChanged += new DataGridViewCellEventHandler(dataGridViewClaimStateTypesFrom_CellValueChanged);
            //Синхронизация данных исходные->текущие
            claim_state_types.Select().RowChanged += new DataRowChangeEventHandler(ClaimStateTypesViewport_RowChanged);
            claim_state_types.Select().RowDeleting += new DataRowChangeEventHandler(ClaimStateTypesViewport_RowDeleting);
            claim_state_types_relations.Select().RowChanged += new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowChanged);
            claim_state_types_relations.Select().RowDeleting += new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowDeleting);
        }

        public override bool CanInsertRecord()
        {
            return AccessControl.HasPrivelege(Priveleges.ClaimsDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_claim_state_types.AddNew();
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
            for (int i = 0; i < v_claim_state_types.Count; i++)
                snapshot_claim_state_types.Rows.Add(DataRowViewToArray(((DataRowView)v_claim_state_types[i])));
            temp_id_state_type = Int32.MaxValue;
            snapshot_claim_state_types_relations.Clear();
            for (int i = 0; i < v_claim_state_types_relations.Count; i++)
                snapshot_claim_state_types_relations.Rows.Add(DataRowViewRelationToArray(((DataRowView)v_claim_state_types_relations[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.ClaimsDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            // Сохраняем общую информацию о видах состояний
            List<ClaimStateType> list = ClaimStateTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = claim_state_types.Select().Rows.Find(((ClaimStateType)list[i]).IdStateType);
                if (row == null)
                {
                    int id_state_type = ClaimStateTypesDataModel.Insert(list[i]);
                    if (id_state_type == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_claim_state_types[i])["id_state_type"] = id_state_type;
                    for (int j = 0; j < v_snapshot_claim_state_types_relations.Count; j++)
                        if ((int)((DataRowView)v_snapshot_claim_state_types_relations[j])["id_state_to"] == list[i].IdStateType)
                            ((DataRowView)v_snapshot_claim_state_types_relations[j])["id_state_to"] = id_state_type;
                    claim_state_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_claim_state_types[i]));
                }
                else
                {
                    if (RowToClaimStateType(row) == list[i])
                        continue;
                    if (ClaimStateTypesDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["state_type"] = list[i].StateType == null ? DBNull.Value : (object)list[i].StateType;
                    row["is_start_state_type"] = list[i].IsStartStateType == null ? DBNull.Value : (object)list[i].IsStartStateType;
                }
            }
            //Удаляем виды состояний из модели и базы
            list = ClaimStateTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridViewClaimStateTypes.Rows.Count; j++)
                    if ((dataGridViewClaimStateTypes.Rows[j].Cells["id_state_type"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridViewClaimStateTypes.Rows[j].Cells["id_state_type"].Value.ToString()) &&
                        ((int)dataGridViewClaimStateTypes.Rows[j].Cells["id_state_type"].Value == list[i].IdStateType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (ClaimStateTypesDataModel.Delete(list[i].IdStateType.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    claim_state_types.Select().Rows.Find(((ClaimStateType)list[i]).IdStateType).Delete();
                    //Рекурсивно удаляем зависимости из снапшот-модели
                    for (int j = snapshot_claim_state_types_relations.Rows.Count - 1; j >= 0; j--)
                        if (Convert.ToInt32(snapshot_claim_state_types_relations.Rows[j]["id_state_from"], CultureInfo.CurrentCulture) == 
                                ((ClaimStateType)list[i]).IdStateType ||
                            Convert.ToInt32(snapshot_claim_state_types_relations.Rows[j]["id_state_to"], CultureInfo.CurrentCulture) == 
                                ((ClaimStateType)list[i]).IdStateType)
                            snapshot_claim_state_types_relations.Rows[j].Delete();
                }
            }
            // Сохраняем возможные переходы
            List<ClaimStateTypeRelation> list_relations = ClaimStateTypesRelationsFromViewport();
            for (int i = 0; i < list_relations.Count; i++)
            {
                DataRow row = claim_state_types_relations.Select().Rows.Find(((ClaimStateTypeRelation)list_relations[i]).IdRelation);
                if (row == null)
                {
                    int id_relation = ClaimStateTypesRelationsDataModel.Insert(list_relations[i]);
                    if (id_relation == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    var rel_row = (from snapshot_row in DataModelHelper.FilterRows(snapshot_claim_state_types_relations)
                                   where snapshot_row.Field<int>("id_state_from") == list_relations[i].IdStateFrom &&
                                         snapshot_row.Field<int>("id_state_to") == list_relations[i].IdStateTo
                                   select snapshot_row).First();
                    rel_row["id_relation"] = id_relation;
                    claim_state_types_relations.Select().Rows.Add(
                        id_relation,
                        list_relations[i].IdStateFrom,
                        list_relations[i].IdStateTo);
                }
            }
            list_relations = ClaimStateTypesRelationsFromView();
            for (int i = 0; i < list_relations.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < v_snapshot_claim_state_types_relations.Count; j++)
                {
                    DataRowView row = (DataRowView)v_snapshot_claim_state_types_relations[j];
                    if ((row["id_relation"] != DBNull.Value) &&
                        !String.IsNullOrEmpty(row["id_relation"].ToString()) &&
                        ((int)row["id_relation"] == list_relations[i].IdRelation) &&
                        (Convert.ToBoolean(row["checked"], CultureInfo.CurrentCulture) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    if (ClaimStateTypesRelationsDataModel.Delete(list_relations[i].IdRelation.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    claim_state_types_relations.Select().Rows.Find(((ClaimStateTypeRelation)list_relations[i]).IdRelation).Delete();
                }
            }
            sync_views = true;
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
            ClaimStateTypesViewport viewport = new ClaimStateTypesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения о структуре зданий в базу данных?", "Внимание",
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
            claim_state_types.Select().RowChanged -= new DataRowChangeEventHandler(ClaimStateTypesViewport_RowChanged);
            claim_state_types.Select().RowDeleting -= new DataRowChangeEventHandler(ClaimStateTypesViewport_RowDeleting);
            claim_state_types_relations.Select().RowChanged -= new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowChanged);
            claim_state_types_relations.Select().RowDeleting -= new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowDeleting);
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
                MenuCallback.NavigationStateUpdate();
            dataGridViewClaimStateTypesFrom.InvalidateColumn(dataGridViewClaimStateTypesFrom.Columns["state_type_checked"].Index);
        }

        void dataGridViewClaimStateTypesFrom_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridViewClaimStateTypesFrom.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                v_claim_state_types_from.Sort = dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            if (dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
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
            int id_state_type = Convert.ToInt32(
                ((DataRowView)v_snapshot_claim_state_types[v_snapshot_claim_state_types.Position])["id_state_type"], CultureInfo.CurrentCulture);
            DataRowView row = ((DataRowView)v_claim_state_types_from[e.RowIndex]);
            int row_count = (from rel_row in snapshot_claim_state_types_relations.AsEnumerable()
                             where rel_row.Field<int>("id_state_from") == (int)row["id_state_type"]  &&
                                rel_row.Field<int>("id_state_to") == id_state_type &&
                                rel_row.Field<bool>("checked") == true
                             select rel_row).Count();
                v_snapshot_claim_state_types_relations.Find("id_state_to", id_state_type);
            switch (this.dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].Name)
            {
                case "state_type_checked":
                    if (row_count > 0)
                        e.Value = true;
                    else
                        e.Value = false;
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
            int id_state_type = Convert.ToInt32(
                ((DataRowView)v_snapshot_claim_state_types[v_snapshot_claim_state_types.Position])["id_state_type"], CultureInfo.CurrentCulture);
            DataRowView row = ((DataRowView)v_claim_state_types_from[e.RowIndex]);
            var rows = from rel_row in snapshot_claim_state_types_relations.AsEnumerable()
                       where rel_row.Field<int>("id_state_from") == (int)row["id_state_type"] &&
                        rel_row.Field<int>("id_state_to") == id_state_type
                       select rel_row;
            sync_views = false;
            switch (dataGridViewClaimStateTypesFrom.Columns[e.ColumnIndex].Name)
            {
                case "state_type_checked":  
                    if (rows.Count() == 0)
                    {
                        snapshot_claim_state_types_relations.Rows.Add(null, row["id_state_type"], id_state_type, e.Value);
                    }
                    else
                        foreach (DataRow rel_row in rows)
                            rel_row["checked"] = e.Value;
                    break;
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        void ClaimStateTypesRelationsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int id_state_to = Convert.ToInt32(
                    ((DataRowView)v_snapshot_claim_state_types[v_snapshot_claim_state_types.Position])["id_state_type"], CultureInfo.CurrentCulture);
                var rows = from rel_row in snapshot_claim_state_types_relations.AsEnumerable()
                           where rel_row.Field<int>("id_state_from") == (int)e.Row["id_state_from"] &&
                            rel_row.Field<int>("id_state_to") == (int)e.Row["id_state_to"]
                           select rel_row;
                foreach (DataRow row in rows)
                    row.Delete();
                dataGridViewClaimStateTypesFrom.InvalidateColumn(dataGridViewClaimStateTypesFrom.Columns["state_type_checked"].Index);
            }
        }

        void ClaimStateTypesRelationsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Add)
            {
                //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                //иначе - объект не принадлежит текущему родителю
                snapshot_claim_state_types_relations.Rows.Add(new object[] { 
                    e.Row["id_state_from"],
                    e.Row["id_state_to"], 
                    true
                });
                dataGridViewClaimStateTypesFrom.InvalidateColumn(dataGridViewClaimStateTypesFrom.Columns["state_type_checked"].Index);
            }
        }

        void ClaimStateTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_claim_state_types.Find("id_state_type", e.Row["id_state_type"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_claim_state_types[row_index]).Delete();
            }
        }

        void ClaimStateTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_claim_state_types.Find("id_state_type", e.Row["id_state_type"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_claim_state_types[row_index]);
                    row["state_type"] = e.Row["state_type"];
                    row["is_start_state_type"] = e.Row["is_start_state_type"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_claim_state_types.Rows.Add(new object[] { 
                        e.Row["id_state_type"], 
                        e.Row["state_type"], 
                        e.Row["is_start_state_type"]
                    });
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
            DataGridViewCell cell = dataGridViewClaimStateTypes.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "state_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования вида состояния претензионно-исковой работы не может превышать 255 символов";
                    else
                        if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название вида состояния претензионно-исковой работы не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimStateTypesViewport));
            this.tableLayoutPanel19 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox36 = new System.Windows.Forms.GroupBox();
            this.dataGridViewClaimStateTypes = new System.Windows.Forms.DataGridView();
            this.id_state_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.is_start_state_type = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.state_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox37 = new System.Windows.Forms.GroupBox();
            this.dataGridViewClaimStateTypesFrom = new System.Windows.Forms.DataGridView();
            this.state_type_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.id_relation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_state_type_from = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.state_type_from = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel19.SuspendLayout();
            this.groupBox36.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypes)).BeginInit();
            this.groupBox37.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypesFrom)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel19
            // 
            this.tableLayoutPanel19.ColumnCount = 2;
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel19.Controls.Add(this.groupBox36, 0, 0);
            this.tableLayoutPanel19.Controls.Add(this.groupBox37, 1, 0);
            this.tableLayoutPanel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel19.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel19.Name = "tableLayoutPanel19";
            this.tableLayoutPanel19.RowCount = 1;
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel19.Size = new System.Drawing.Size(707, 427);
            this.tableLayoutPanel19.TabIndex = 0;
            // 
            // groupBox36
            // 
            this.groupBox36.Controls.Add(this.dataGridViewClaimStateTypes);
            this.groupBox36.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox36.Location = new System.Drawing.Point(3, 3);
            this.groupBox36.Name = "groupBox36";
            this.groupBox36.Size = new System.Drawing.Size(347, 421);
            this.groupBox36.TabIndex = 0;
            this.groupBox36.TabStop = false;
            this.groupBox36.Text = "Состояния";
            // 
            // dataGridViewClaimStateTypes
            // 
            this.dataGridViewClaimStateTypes.AllowUserToAddRows = false;
            this.dataGridViewClaimStateTypes.AllowUserToDeleteRows = false;
            this.dataGridViewClaimStateTypes.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewClaimStateTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaimStateTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_state_type,
            this.is_start_state_type,
            this.state_type});
            this.dataGridViewClaimStateTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaimStateTypes.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewClaimStateTypes.Name = "dataGridViewClaimStateTypes";
            this.dataGridViewClaimStateTypes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClaimStateTypes.Size = new System.Drawing.Size(341, 401);
            this.dataGridViewClaimStateTypes.TabIndex = 0;
            this.dataGridViewClaimStateTypes.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewClaimStateTypes_CellValidated);
            // 
            // id_state_type
            // 
            this.id_state_type.HeaderText = "Идентификатор состояния";
            this.id_state_type.Name = "id_state_type";
            this.id_state_type.Visible = false;
            // 
            // is_start_state_type
            // 
            this.is_start_state_type.HeaderText = "Начальное";
            this.is_start_state_type.MinimumWidth = 70;
            this.is_start_state_type.Name = "is_start_state_type";
            this.is_start_state_type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.is_start_state_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.is_start_state_type.Width = 70;
            // 
            // state_type
            // 
            this.state_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.state_type.HeaderText = "Наименование вида состояния";
            this.state_type.Name = "state_type";
            this.state_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // groupBox37
            // 
            this.groupBox37.Controls.Add(this.dataGridViewClaimStateTypesFrom);
            this.groupBox37.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox37.Location = new System.Drawing.Point(356, 3);
            this.groupBox37.Name = "groupBox37";
            this.groupBox37.Size = new System.Drawing.Size(348, 421);
            this.groupBox37.TabIndex = 1;
            this.groupBox37.TabStop = false;
            this.groupBox37.Text = "Разрешены переходы из";
            // 
            // dataGridViewClaimStateTypesFrom
            // 
            this.dataGridViewClaimStateTypesFrom.AllowUserToAddRows = false;
            this.dataGridViewClaimStateTypesFrom.AllowUserToDeleteRows = false;
            this.dataGridViewClaimStateTypesFrom.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewClaimStateTypesFrom.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaimStateTypesFrom.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.state_type_checked,
            this.id_relation,
            this.id_state_type_from,
            this.state_type_from});
            this.dataGridViewClaimStateTypesFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaimStateTypesFrom.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewClaimStateTypesFrom.Name = "dataGridViewClaimStateTypesFrom";
            this.dataGridViewClaimStateTypesFrom.Size = new System.Drawing.Size(342, 401);
            this.dataGridViewClaimStateTypesFrom.TabIndex = 0;
            this.dataGridViewClaimStateTypesFrom.VirtualMode = true;
            this.dataGridViewClaimStateTypesFrom.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridViewClaimStateTypesFrom_CellValueNeeded);
            this.dataGridViewClaimStateTypesFrom.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridViewClaimStateTypesFrom_CellValuePushed);
            this.dataGridViewClaimStateTypesFrom.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewClaimStateTypesFrom_ColumnHeaderMouseClick);
            this.dataGridViewClaimStateTypesFrom.SelectionChanged += new System.EventHandler(this.dataGridViewClaimStateTypesFrom_SelectionChanged);
            // 
            // state_type_checked
            // 
            this.state_type_checked.HeaderText = "";
            this.state_type_checked.MinimumWidth = 70;
            this.state_type_checked.Name = "state_type_checked";
            this.state_type_checked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.state_type_checked.Width = 70;
            // 
            // id_relation
            // 
            this.id_relation.HeaderText = "Идентификатор отношения";
            this.id_relation.Name = "id_relation";
            this.id_relation.Visible = false;
            // 
            // id_state_type_from
            // 
            this.id_state_type_from.HeaderText = "Идентификатор состояния";
            this.id_state_type_from.Name = "id_state_type_from";
            this.id_state_type_from.Visible = false;
            // 
            // state_type_from
            // 
            this.state_type_from.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.state_type_from.DefaultCellStyle = dataGridViewCellStyle1;
            this.state_type_from.HeaderText = "Наименование вида состояния";
            this.state_type_from.Name = "state_type_from";
            this.state_type_from.ReadOnly = true;
            // 
            // ClaimStateTypesViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(713, 433);
            this.Controls.Add(this.tableLayoutPanel19);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClaimStateTypesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Виды состояний иск. работы";
            this.tableLayoutPanel19.ResumeLayout(false);
            this.groupBox36.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypes)).EndInit();
            this.groupBox37.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypesFrom)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
