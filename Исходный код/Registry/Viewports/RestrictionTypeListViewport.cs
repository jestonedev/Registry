using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport
{
    internal sealed class RestrictionTypeListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_restriction_type = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_restriction_type = new DataGridViewTextBoxColumn();
        #endregion Components

        //Modeles
        RestrictionTypesDataModel restriction_types = null;
        DataTable snapshot_restriction_types = new DataTable("snapshot_restriction_types");

        //Views
        BindingSource v_restriction_types = null;
        BindingSource v_snapshot_restriction_types = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public RestrictionTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageRestrictionTypes";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Типы реквизитов";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public RestrictionTypeListViewport(RestrictionTypeListViewport restrictionTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            restriction_types = RestrictionTypesDataModel.GetInstance();
            restriction_types.Select();

            v_restriction_types = new BindingSource();
            v_restriction_types.DataMember = "restriction_types";
            v_restriction_types.DataSource = DataSetManager.GetDataSet();

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < restriction_types.Select().Columns.Count; i++)
                snapshot_restriction_types.Columns.Add(new DataColumn(restriction_types.Select().Columns[i].ColumnName, 
                    restriction_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_restriction_types.Count; i++)
                snapshot_restriction_types.Rows.Add(DataRowViewToArray(((DataRowView)v_restriction_types[i])));
            v_snapshot_restriction_types = new BindingSource();
            v_snapshot_restriction_types.DataSource = snapshot_restriction_types;
            v_snapshot_restriction_types.CurrentItemChanged += new EventHandler(v_snapshot_restriction_types_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_restriction_types;
            field_id_restriction_type.DataPropertyName = "id_restriction_type";
            field_restriction_type.DataPropertyName = "restriction_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            restriction_types.Select().RowChanged += new DataRowChangeEventHandler(RestrictionTypeListViewport_RowChanged);
            restriction_types.Select().RowDeleting += new DataRowChangeEventHandler(RestrictionTypeListViewport_RowDeleting);
        }

        void v_snapshot_restriction_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        public override void MoveFirst()
        {
            v_snapshot_restriction_types.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_restriction_types.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_restriction_types.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_restriction_types.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_restriction_types.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_restriction_types.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_restriction_types.Position > -1) && (v_snapshot_restriction_types.Position < (v_snapshot_restriction_types.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_restriction_types.Position > -1) && (v_snapshot_restriction_types.Position < (v_snapshot_restriction_types.Count - 1));
        }

        void RestrictionTypeListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_restriction_types.Find("id_restriction_type", e.Row["id_restriction_type"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_restriction_types[row_index]).Delete();
            }
        }

        void RestrictionTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_restriction_types.Find("id_restriction_type", e.Row["id_restriction_type"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_restriction_types[row_index]);
                    row["restriction_type"] = e.Row["restriction_type"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_restriction_types.Rows.Add(new object[] { 
                        e.Row["id_restriction_type"], 
                        e.Row["restriction_type"]
                    });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<RestrictionType> list_from_view = RestrictionTypesFromView();
            List<RestrictionType> list_from_viewport = RestrictionTypesFromViewport();
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
            return false;
        }

        private List<RestrictionType> RestrictionTypesFromViewport()
        {
            List<RestrictionType> list = new List<RestrictionType>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    RestrictionType rt = new RestrictionType();
                    DataGridViewRow row = dataGridView.Rows[i];
                    rt.id_restriction_type = row.Cells["id_restriction_type"].Value == DBNull.Value ? null : 
                        (int?)Convert.ToInt32(row.Cells["id_restriction_type"].Value);
                    rt.restriction_type = row.Cells["restriction_type"].Value == DBNull.Value ? null : row.Cells["restriction_type"].Value.ToString();
                    list.Add(rt);
                }
            }
            return list;
        }

        private List<RestrictionType> RestrictionTypesFromView()
        {
            List<RestrictionType> list = new List<RestrictionType>();
            for (int i = 0; i < v_restriction_types.Count; i++)
            {
                RestrictionType rt = new RestrictionType();
                DataRowView row = ((DataRowView)v_restriction_types[i]);
                rt.id_restriction_type = row["id_restriction_type"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_restriction_type"]);
                rt.restriction_type = row["restriction_type"] == DBNull.Value ? null : row["restriction_type"].ToString();
                list.Add(rt);
            }
            return list;
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_restriction_type"], 
                dataRowView["restriction_type"]
            };
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            menuCallback.EditingStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return true;
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_restriction_types.AddNew();
            row.EndEdit();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения о типах реквизитов в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            restriction_types.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionTypeListViewport_RowChanged);
            restriction_types.Select().RowDeleting -= new DataRowChangeEventHandler(RestrictionTypeListViewport_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_restriction_types.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_restriction_types[v_snapshot_restriction_types.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_restriction_types.Clear();
            for (int i = 0; i < v_restriction_types.Count; i++)
                snapshot_restriction_types.Rows.Add(DataRowViewToArray(((DataRowView)v_restriction_types[i])));
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        private bool ValidateViewportData(List<RestrictionType> list)
        {
            foreach (RestrictionType restrictionType in list)
                if (restrictionType.restriction_type == null)
                {
                    MessageBox.Show("Не заполнено наименование типа реквизита", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<RestrictionType> list = RestrictionTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = restriction_types.Select().Rows.Find(((RestrictionType)list[i]).id_restriction_type);
                if (row == null)
                {
                    int id_restriction_type = restriction_types.Insert(list[i]);
                    if (id_restriction_type == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_restriction_types[i])["id_restriction_type"] = id_restriction_type;
                    restriction_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_restriction_types[i]));
                }
                else
                {
                    if (RowToRestrictionType(row) == list[i])
                        continue;
                    if (restriction_types.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["restriction_type"] = list[i].restriction_type == null ? DBNull.Value : (object)list[i].restriction_type;
                }
            }
            list = RestrictionTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_restriction_type"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_restriction_type"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_restriction_type"].Value == list[i].id_restriction_type))
                        row_index = j;
                if (row_index == -1)
                {
                    if (restriction_types.Delete(list[i].id_restriction_type.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    restriction_types.Select().Rows.Find(((RestrictionType)list[i]).id_restriction_type).Delete();
                }
            }
            sync_views = true;
        }

        private RestrictionType RowToRestrictionType(DataRow row)
        {
            RestrictionType restrictionType = new RestrictionType();
            restrictionType.id_restriction_type = row["id_restriction_type"] == DBNull.Value ? null :
                (int?)Convert.ToInt32(row["id_restriction_type"]);
            restrictionType.restriction_type = row["restriction_type"] == DBNull.Value ? null : row["restriction_type"].ToString();
            return restrictionType;
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "restriction_type") &&
                (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 255))
            {
                MessageBox.Show("Длина названия типа реквизита не может превышать 255 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 255);
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            RestrictionTypeListViewport viewport = new RestrictionTypeListViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_restriction_types.Count;
        }

        private void ConstructViewport()
        {
            this.SuspendLayout();
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.Controls.Add(this.dataGridView);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_restriction_type,
            this.field_restriction_type});
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.TabIndex = 4;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.AllowUserToResizeRows = false;
            // 
            // field_id_restriction_type
            // 
            this.field_id_restriction_type.HeaderText = "Идентификатор реквизита";
            this.field_id_restriction_type.Name = "id_restriction_type";
            this.field_id_restriction_type.Visible = false;
            // 
            // field_description
            // 
            this.field_restriction_type.HeaderText = "Наименование";
            this.field_restriction_type.Name = "restriction_type";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
