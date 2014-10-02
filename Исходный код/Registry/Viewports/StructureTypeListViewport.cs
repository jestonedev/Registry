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
    internal class StructureTypeListViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_structure_type = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_structure_type = new DataGridViewTextBoxColumn();
        #endregion Components

        //Modeles
        StructureTypesDataModel structure_types = null;
        DataTable snapshot_structure_types = new DataTable("snapshot_structure_types");

        //Views
        BindingSource v_structure_types = null;
        BindingSource v_snapshot_structure_types = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public StructureTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageStructureTypes";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Типы материалов";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public StructureTypeListViewport(StructureTypeListViewport structureTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            structure_types = StructureTypesDataModel.GetInstance();
            v_structure_types = new BindingSource();
            v_structure_types.DataMember = "structure_types";
            v_structure_types.DataSource = DataSetManager.GetDataSet();

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < structure_types.Select().Columns.Count; i++)
                snapshot_structure_types.Columns.Add(new DataColumn(structure_types.Select().Columns[i].ColumnName, structure_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_structure_types.Count; i++)
                snapshot_structure_types.Rows.Add(DataRowViewToArray(((DataRowView)v_structure_types[i])));
            v_snapshot_structure_types = new BindingSource();
            v_snapshot_structure_types.DataSource = snapshot_structure_types;

            dataGridView.DataSource = v_snapshot_structure_types;
            field_id_structure_type.DataPropertyName = "id_structure_type";
            field_structure_type.DataPropertyName = "structure_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            structure_types.Select().RowChanged += new DataRowChangeEventHandler(StructureTypeListViewport_RowChanged);
            structure_types.Select().RowDeleting += new DataRowChangeEventHandler(StructureTypeListViewport_RowDeleting);
        }

        public override void MoveFirst()
        {
            v_snapshot_structure_types.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_structure_types.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_structure_types.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_structure_types.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_structure_types.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_structure_types.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_structure_types.Position > -1) && (v_snapshot_structure_types.Position < (v_snapshot_structure_types.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_structure_types.Position > -1) && (v_snapshot_structure_types.Position < (v_snapshot_structure_types.Count - 1));
        }

        void StructureTypeListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_structure_types.Find("id_structure_type", e.Row["id_structure_type"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_structure_types[row_index]).Delete();
            }
        }

        void StructureTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_structure_types.Find("id_structure_type", e.Row["id_structure_type"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_structure_types[row_index]);
                    row["structure_type"] = e.Row["structure_type"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_structure_types.Rows.Add(new object[] { 
                        e.Row["id_structure_type"], 
                        e.Row["structure_type"]
                    });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<StructureType> list_from_view = StructureTypesFromView();
            List<StructureType> list_from_viewport = StructureTypesFromViewport();
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

        private List<StructureType> StructureTypesFromViewport()
        {
            List<StructureType> list = new List<StructureType>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    StructureType st = new StructureType();
                    DataGridViewRow row = dataGridView.Rows[i];
                    st.id_structure_type = row.Cells["id_structure_type"].Value == DBNull.Value ? null : (int?)Convert.ToInt32(row.Cells["id_structure_type"].Value);
                    st.structure_type = row.Cells["structure_type"].Value == DBNull.Value ? null : row.Cells["structure_type"].Value.ToString();
                    list.Add(st);
                }
            }
            return list;
        }

        private List<StructureType> StructureTypesFromView()
        {
            List<StructureType> list = new List<StructureType>();
            for (int i = 0; i < v_structure_types.Count; i++)
            {
                StructureType st = new StructureType();
                DataRowView row = ((DataRowView)v_structure_types[i]);
                st.id_structure_type = row["id_structure_type"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_structure_type"]);
                st.structure_type = row["structure_type"] == DBNull.Value ? null : row["structure_type"].ToString();
                list.Add(st);
            }
            return list;
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_structure_type"], 
                dataRowView["structure_type"]
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
            DataRowView row = (DataRowView)v_snapshot_structure_types.AddNew();
            row.EndEdit();
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения о структуре зданий в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            structure_types.Select().RowChanged -= new DataRowChangeEventHandler(StructureTypeListViewport_RowChanged);
            structure_types.Select().RowDeleting -= new DataRowChangeEventHandler(StructureTypeListViewport_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_structure_types.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_structure_types[v_snapshot_structure_types.Position]).Row.Delete();
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_structure_types.Clear();
            for (int i = 0; i < v_structure_types.Count; i++)
                snapshot_structure_types.Rows.Add(DataRowViewToArray(((DataRowView)v_structure_types[i])));
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        private bool ValidateViewportData(List<StructureType> list)
        {
            foreach (StructureType structureType in list)
                if (structureType.structure_type == null)
                {
                    MessageBox.Show("Не заполнено наименование структуры здания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<StructureType> list = StructureTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = structure_types.Select().Rows.Find(((StructureType)list[i]).id_structure_type);
                if (row == null)
                {
                    int id_structure_type = structure_types.Insert(list[i]);
                    if (id_structure_type == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_structure_types[i])["id_structure_type"] = id_structure_type;
                    structure_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_structure_types[i]));
                }
                else
                {
                    if (structure_types.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["structure_type"] = list[i].structure_type == null ? DBNull.Value : (object)list[i].structure_type;
                }
            }
            list = StructureTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_structure_type"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_structure_type"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_structure_type"].Value == list[i].id_structure_type))
                        row_index = j;
                if (row_index == -1)
                {
                    if (structure_types.Delete(list[i].id_structure_type.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    structure_types.Select().Rows.Find(((StructureType)list[i]).id_structure_type).Delete();
                }
            }
            menuCallback.NavigationStateUpdate();
            menuCallback.EditingStateUpdate();
            sync_views = true;
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "structure_type") &&
                (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 255))
            {
                MessageBox.Show("Длина названия типа структуры здания не может превышать 255 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 255);
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            StructureTypeListViewport viewport = new StructureTypeListViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
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
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, 
                System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_structure_type,
            this.field_structure_type});
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.TabIndex = 3;
            // 
            // field_id_structure_type
            // 
            this.field_id_structure_type.HeaderText = "Идентификатор типа материала";
            this.field_id_structure_type.Name = "id_structure_type";
            this.field_id_structure_type.Visible = false;
            // 
            // field_structure_type
            // 
            this.field_structure_type.HeaderText = "Наименование";
            this.field_structure_type.Name = "structure_type";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
