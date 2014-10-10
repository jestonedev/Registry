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
    internal sealed class OwnershipTypeListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_ownership_type = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_ownership_type = new DataGridViewTextBoxColumn();
        #endregion Components

        //Modeles
        OwnershipRightTypesDataModel ownership_right_types = null;
        DataTable snapshot_ownership_right_types = new DataTable("snapshot_ownership_right_types");

        //Views
        BindingSource v_ownership_right_types = null;
        BindingSource v_snapshot_ownership_right_types = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public OwnershipTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageOwnershipTypes";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Типы оснований";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public OwnershipTypeListViewport(OwnershipTypeListViewport ownershipTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            ownership_right_types = OwnershipRightTypesDataModel.GetInstance();
            v_ownership_right_types = new BindingSource();
            v_ownership_right_types.DataMember = "ownership_right_types";
            v_ownership_right_types.DataSource = DataSetManager.GetDataSet();

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < ownership_right_types.Select().Columns.Count; i++)
                snapshot_ownership_right_types.Columns.Add(new DataColumn(ownership_right_types.Select().Columns[i].ColumnName, 
                    ownership_right_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_ownership_right_types.Count; i++)
                snapshot_ownership_right_types.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_right_types[i])));
            v_snapshot_ownership_right_types = new BindingSource();
            v_snapshot_ownership_right_types.DataSource = snapshot_ownership_right_types;

            dataGridView.DataSource = v_snapshot_ownership_right_types;
            field_id_ownership_type.DataPropertyName = "id_ownership_right_type";
            field_ownership_type.DataPropertyName = "ownership_right_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            ownership_right_types.Select().RowChanged += new DataRowChangeEventHandler(OwnershipTypeListViewport_RowChanged);
            ownership_right_types.Select().RowDeleting += new DataRowChangeEventHandler(OwnershipTypeListViewport_RowDeleting);
        }

        public override void MoveFirst()
        {
            v_snapshot_ownership_right_types.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_ownership_right_types.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_ownership_right_types.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_ownership_right_types.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_ownership_right_types.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_ownership_right_types.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_ownership_right_types.Position > -1) && (v_snapshot_ownership_right_types.Position < (v_snapshot_ownership_right_types.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_ownership_right_types.Position > -1) && (v_snapshot_ownership_right_types.Position < (v_snapshot_ownership_right_types.Count - 1));
        }

        void OwnershipTypeListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_ownership_right_types.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_ownership_right_types[row_index]).Delete();
            }
        }

        void OwnershipTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_ownership_right_types.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_ownership_right_types[row_index]);
                    row["ownership_right_type"] = e.Row["ownership_right_type"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_ownership_right_types.Rows.Add(new object[] { 
                        e.Row["id_ownership_right_type"], 
                        e.Row["ownership_right_type"]
                    });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<OwnershipRightType> list_from_view = OwnershipRightTypesFromView();
            List<OwnershipRightType> list_from_viewport = OwnershipRightTypesFromViewport();
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

        private List<OwnershipRightType> OwnershipRightTypesFromViewport()
        {
            List<OwnershipRightType> list = new List<OwnershipRightType>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    OwnershipRightType ort = new OwnershipRightType();
                    DataGridViewRow row = dataGridView.Rows[i];
                    ort.id_ownership_right_type = row.Cells["id_ownership_right_type"].Value == DBNull.Value ? null :
                        (int?)Convert.ToInt32(row.Cells["id_ownership_right_type"].Value);
                    ort.ownership_right_type = row.Cells["ownership_right_type"].Value == DBNull.Value ? null : row.Cells["ownership_right_type"].Value.ToString();
                    list.Add(ort);
                }
            }
            return list;
        }

        private List<OwnershipRightType> OwnershipRightTypesFromView()
        {
            List<OwnershipRightType> list = new List<OwnershipRightType>();
            for (int i = 0; i < v_ownership_right_types.Count; i++)
            {
                OwnershipRightType ort = new OwnershipRightType();
                DataRowView row = ((DataRowView)v_ownership_right_types[i]);
                ort.id_ownership_right_type = row["id_ownership_right_type"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_ownership_right_type"]);
                ort.ownership_right_type = row["ownership_right_type"] == DBNull.Value ? null : row["ownership_right_type"].ToString();
                list.Add(ort);
            }
            return list;
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_ownership_right_type"], 
                dataRowView["ownership_right_type"]
            };
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            menuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "ownership_right_type") &&
                (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 255))
            {
                MessageBox.Show("Длина названия типа ограничения не может превышать 255 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 255);
            }
        }

        public override bool CanInsertRecord()
        {
            return true;
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_ownership_right_types.AddNew();
            row.EndEdit();
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
            menuCallback.StatusBarStateUpdate();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения о типах ограничений в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            ownership_right_types.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipTypeListViewport_RowChanged);
            ownership_right_types.Select().RowDeleting -= new DataRowChangeEventHandler(OwnershipTypeListViewport_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_ownership_right_types.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_ownership_right_types[v_snapshot_ownership_right_types.Position]).Row.Delete();
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
            menuCallback.StatusBarStateUpdate();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_ownership_right_types.Clear();
            for (int i = 0; i < v_ownership_right_types.Count; i++)
                snapshot_ownership_right_types.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_right_types[i])));
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
            menuCallback.StatusBarStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        private bool ValidateViewportData(List<OwnershipRightType> list)
        {
            foreach (OwnershipRightType ownershipRightType in list)
                if (ownershipRightType.ownership_right_type == null)
                {
                    MessageBox.Show("Не заполнено наименование типа ограничения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<OwnershipRightType> list = OwnershipRightTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = ownership_right_types.Select().Rows.Find(((OwnershipRightType)list[i]).id_ownership_right_type);
                if (row == null)
                {
                    int id_ownership_right_type = ownership_right_types.Insert(list[i]);
                    if (id_ownership_right_type == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_ownership_right_types[i])["id_ownership_right_type"] = id_ownership_right_type;
                    ownership_right_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_ownership_right_types[i]));
                }
                else
                {
                    if (ownership_right_types.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["ownership_right_type"] = list[i].ownership_right_type == null ? DBNull.Value : (object)list[i].ownership_right_type;
                }
            }
            list = OwnershipRightTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_ownership_right_type"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_ownership_right_type"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_ownership_right_type"].Value == list[i].id_ownership_right_type))
                        row_index = j;
                if (row_index == -1)
                {
                    if (ownership_right_types.Delete(list[i].id_ownership_right_type.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ownership_right_types.Select().Rows.Find(((OwnershipRightType)list[i]).id_ownership_right_type).Delete();
                }
            }
            menuCallback.NavigationStateUpdate();
            menuCallback.EditingStateUpdate();
            sync_views = true;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            OwnershipTypeListViewport viewport = new OwnershipTypeListViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_ownership_right_types.Count;
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
            this.field_id_ownership_type,
            this.field_ownership_type});
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
            // field_id_ownership_type
            // 
            this.field_id_ownership_type.HeaderText = "Идентификатор реквизита";
            this.field_id_ownership_type.Name = "id_ownership_right_type";
            this.field_id_ownership_type.Visible = false;
            // 
            // field_ownership_type
            // 
            this.field_ownership_type.HeaderText = "Наименование";
            this.field_ownership_type.Name = "ownership_right_type";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
