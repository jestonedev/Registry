using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using Registry.DataModels;
using System.Text.RegularExpressions;

namespace Registry.Viewport
{
    internal sealed class ReasonTypesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_reason_type = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_reason_name = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_reason_template = new DataGridViewTextBoxColumn();
        #endregion Components

        //Modeles
        ReasonTypesDataModel reason_types = null;
        DataTable snapshot_reason_types = new DataTable("snapshot_reason_types");

        //Views
        BindingSource v_reason_types = null;
        BindingSource v_snapshot_reason_types = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public ReasonTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageReasonTypes";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Виды оснований";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public ReasonTypesViewport(ReasonTypesViewport reasonTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = reasonTypesViewport.DynamicFilter;
            this.StaticFilter = reasonTypesViewport.StaticFilter;
            this.ParentRow = reasonTypesViewport.ParentRow;
            this.ParentType = reasonTypesViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            reason_types = ReasonTypesDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            reason_types.Select();

            v_reason_types = new BindingSource();
            v_reason_types.DataMember = "reason_types";
            v_reason_types.DataSource = DataSetManager.GetDataSet();

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < reason_types.Select().Columns.Count; i++)
                snapshot_reason_types.Columns.Add(new DataColumn(
                    reason_types.Select().Columns[i].ColumnName, reason_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_reason_types.Count; i++)
                snapshot_reason_types.Rows.Add(DataRowViewToArray(((DataRowView)v_reason_types[i])));
            v_snapshot_reason_types = new BindingSource();
            v_snapshot_reason_types.DataSource = snapshot_reason_types;
            v_snapshot_reason_types.CurrentItemChanged += new EventHandler(v_snapshot_reason_types_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_reason_types;
            field_id_reason_type.DataPropertyName = "id_reason_type";
            field_reason_name.DataPropertyName = "reason_name";
            field_reason_template.DataPropertyName = "reason_template";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            reason_types.Select().RowChanged += new DataRowChangeEventHandler(ReasonTypesViewport_RowChanged);
            reason_types.Select().RowDeleting += new DataRowChangeEventHandler(ReasonTypesViewport_RowDeleting);
        }

        public override void MoveFirst()
        {
            v_snapshot_reason_types.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_reason_types.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_reason_types.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_reason_types.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_reason_types.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_reason_types.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_reason_types.Position > -1) && (v_snapshot_reason_types.Position < (v_snapshot_reason_types.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_reason_types.Position > -1) && (v_snapshot_reason_types.Position < (v_snapshot_reason_types.Count - 1));
        }

        void ReasonTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_reason_types.Find("id_reason_type", e.Row["id_reason_type"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_reason_types[row_index]).Delete();
            }
        }

        void ReasonTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_reason_types.Find("id_reason_type", e.Row["id_reason_type"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_reason_types[row_index]);
                    row["reason_name"] = e.Row["reason_name"];
                    row["reason_template"] = e.Row["reason_template"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_reason_types.Rows.Add(new object[] { 
                        e.Row["id_reason_type"], 
                        e.Row["reason_name"], 
                        e.Row["reason_template"]
                    });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<ReasonType> list_from_view = ReasonTypesFromView();
            List<ReasonType> list_from_viewport = ReasonTypesFromViewport();
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

        private List<ReasonType> ReasonTypesFromViewport()
        {
            List<ReasonType> list = new List<ReasonType>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    ReasonType rt = new ReasonType();
                    DataGridViewRow row = dataGridView.Rows[i];
                    rt.id_reason_type = row.Cells["id_reason_type"].Value == DBNull.Value ? null : (int?)Convert.ToInt32(row.Cells["id_reason_type"].Value);
                    rt.reason_name = row.Cells["reason_name"].Value == DBNull.Value ? null : row.Cells["reason_name"].Value.ToString();
                    rt.reason_template = row.Cells["reason_template"].Value == DBNull.Value ? null : row.Cells["reason_template"].Value.ToString();
                    list.Add(rt);
                }
            }
            return list;
        }

        private List<ReasonType> ReasonTypesFromView()
        {
            List<ReasonType> list = new List<ReasonType>();
            for (int i = 0; i < v_reason_types.Count; i++)
            {
                ReasonType rt = new ReasonType();
                DataRowView row = ((DataRowView)v_reason_types[i]);
                rt.id_reason_type = row["id_reason_type"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_reason_type"]);
                rt.reason_name = row["reason_name"] == DBNull.Value ? null : row["reason_name"].ToString();
                rt.reason_template = row["reason_template"] == DBNull.Value ? null : row["reason_template"].ToString();
                list.Add(rt);
            }
            return list;
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
            DataRowView row = (DataRowView)v_snapshot_reason_types.AddNew();
            row.EndEdit();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения о виде основания в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            reason_types.Select().RowChanged -= new DataRowChangeEventHandler(ReasonTypesViewport_RowChanged);
            reason_types.Select().RowDeleting -= new DataRowChangeEventHandler(ReasonTypesViewport_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_reason_types.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_reason_types[v_snapshot_reason_types.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_reason_types.Clear();
            for (int i = 0; i < v_reason_types.Count; i++)
                snapshot_reason_types.Rows.Add(DataRowViewToArray(((DataRowView)v_reason_types[i])));
        }

        private bool ValidateViewportData(List<ReasonType> list)
        {
            foreach (ReasonType reasonType in list)
            {
                if (reasonType.reason_name == null)
                {
                    MessageBox.Show("Имя вида основания не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (reasonType.reason_template == null)
                {
                    MessageBox.Show("Шаблон основания не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!(Regex.IsMatch(reasonType.reason_template,"@reason_number@") &&
                     (Regex.IsMatch(reasonType.reason_template, "@reason_date@"))))
                {
                    MessageBox.Show("Шаблон основания имеет неверный формат. В шаблоне должны быть указаны номер (в виде шаблона @reason_number@) и"+
                        " дата (в виде шаблона @reason_date@) основания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<ReasonType> list = ReasonTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = reason_types.Select().Rows.Find(((ReasonType)list[i]).id_reason_type);
                if (row == null)
                {
                    int id_reason_type = reason_types.Insert(list[i]);
                    if (id_reason_type == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_reason_types[i])["id_reason_type"] = id_reason_type;
                    reason_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_reason_types[i]));
                }
                else
                {

                    if (RowToReasonType(row) == list[i])
                        continue;
                    if (reason_types.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["reason_name"] = list[i].reason_name == null ? DBNull.Value : (object)list[i].reason_name;
                    row["reason_template"] = list[i].reason_template == null ? DBNull.Value : (object)list[i].reason_template;
                }
            }
            list = ReasonTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_reason_type"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_reason_type"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_reason_type"].Value == list[i].id_reason_type))
                        row_index = j;
                if (row_index == -1)
                {
                    if (reason_types.Delete(list[i].id_reason_type.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    reason_types.Select().Rows.Find(((ReasonType)list[i]).id_reason_type).Delete();
                }
            }
            sync_views = true;
        }

        private ReasonType RowToReasonType(DataRow row)
        {
            ReasonType reasonType = new ReasonType();
            reasonType.id_reason_type = row["id_reason_type"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_reason_type"]);
            reasonType.reason_name = row["reason_name"] == DBNull.Value ? null : row["reason_name"].ToString();
            reasonType.reason_template = row["reason_template"] == DBNull.Value ? null : row["reason_template"].ToString();
            return reasonType;
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.OwningColumn.Name == "reason_name")
            {
                if (cell.Value.ToString().Length > 150)
                {
                    MessageBox.Show("Длина названия вида основания не может превышать 150 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cell.Value = cell.Value.ToString().Substring(0, 150);
                }
                if (cell.Value.ToString().Trim().Length == 0)
                {
                    cell.ErrorText = "Название вида основания не может быть пустым";
                }
                else
                    cell.ErrorText = "";
            }
            if (cell.OwningColumn.Name == "reason_template")
            {
                if (cell.Value.ToString().Length > 4000)
                {
                    MessageBox.Show("Длина шаблона вида основания не может превышать 4000 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cell.Value = cell.Value.ToString().Substring(0, 4000);
                }
                if (!(Regex.IsMatch(cell.Value.ToString(), "@reason_number@") && 
                     (Regex.IsMatch(cell.Value.ToString(), "@reason_date@"))))
                {
                    cell.ErrorText = "Шаблон основания имеет неверный формат. В шаблоне должны быть указаны номер (в виде шаблона @reason_number@) и" +
                        " дата (в виде шаблона @reason_date@) основания";
                }
                else
                    cell.ErrorText = "";
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            ReasonTypesViewport viewport = new ReasonTypesViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_reason_types.Count;
        }

        void v_snapshot_reason_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_reason_type"], 
                dataRowView["reason_name"],
                dataRowView["reason_template"]
            };
        }

        private void ConstructViewport()
        {
            this.Controls.Add(dataGridView);
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font =
                new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_reason_type,
            this.field_reason_name,
            this.field_reason_template});          
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(990, 537);
            this.dataGridView.TabIndex = 7;
            this.dataGridView.MultiSelect = false;
            // 
            // field_id_reason_type
            // 
            this.field_id_reason_type.Frozen = true;
            this.field_id_reason_type.HeaderText = "Идентификатор вида основания";
            this.field_id_reason_type.Name = "id_reason_type";
            this.field_id_reason_type.ReadOnly = true;
            this.field_id_reason_type.Visible = false;
            // 
            // field_reason_name
            // 
            this.field_reason_name.FillWeight = 200F;
            this.field_reason_name.HeaderText = "Имя вида основания";
            this.field_reason_name.MinimumWidth = 100;
            this.field_reason_name.Name = "reason_name";
            // 
            // field_reason_template
            // 
            this.field_reason_template.FillWeight = 500F;
            this.field_reason_template.HeaderText = "Шаблон вида основания";
            this.field_reason_template.MinimumWidth = 100;
            this.field_reason_template.Name = "reason_template";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
