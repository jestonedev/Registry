using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels;
using Registry.Entities;

namespace Registry.Viewport
{
    internal class SubPremisesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_sub_premises = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_id_premises = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_sub_premises_num = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_total_area = new DataGridViewTextBoxColumn();
        #endregion Components

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        //Modeles
        SubPremisesDataModel sub_premises = null;
        DataTable snapshot_sub_premises = new DataTable("snapshot_sub_premises");

        //Views
        BindingSource v_sub_premises = null;
        BindingSource v_snapshot_sub_premises = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public SubPremisesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageBuildings";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень комнат";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public SubPremisesViewport(SubPremisesViewport subPremisesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = subPremisesViewport.DynamicFilter;
            this.StaticFilter = subPremisesViewport.StaticFilter;
            this.ParentRow = subPremisesViewport.ParentRow;
            this.ParentType = subPremisesViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            sub_premises = SubPremisesDataModel.GetInstance();
            v_sub_premises = new BindingSource();
            v_sub_premises.DataMember = "sub_premises";
            v_sub_premises.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_sub_premises.Filter += " AND ";
            v_sub_premises.Filter += DynamicFilter;
            v_sub_premises.DataSource = DataSetManager.GetDataSet();

            if (ParentRow != null && ParentType == ParentTypeEnum.Premises)
                this.Text = String.Format("Комнаты помещения №{0}", ParentRow["id_premises"]);
            
            //Инициируем колонки snapshot-модели
            for (int i = 0; i < sub_premises.Select().Columns.Count; i++)
                snapshot_sub_premises.Columns.Add(new DataColumn(sub_premises.Select().Columns[i].ColumnName, sub_premises.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_sub_premises.Count; i++)
                snapshot_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_sub_premises[i])));
            v_snapshot_sub_premises = new BindingSource();
            v_snapshot_sub_premises.DataSource = snapshot_sub_premises;

            dataGridView.DataSource = v_snapshot_sub_premises;
            field_id_sub_premises.DataPropertyName = "id_sub_premises";
            field_id_premises.DataPropertyName = "id_premises";
            field_sub_premises_num.DataPropertyName = "sub_premises_num";
            field_sub_premises_num.DataPropertyName = "sub_premises_num";
            field_total_area.DataPropertyName = "total_area";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView_EditingControlShowing);
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            sub_premises.Select().RowChanged += new DataRowChangeEventHandler(SubPremisesViewport_RowChanged);
            sub_premises.Select().RowDeleting += new DataRowChangeEventHandler(SubPremisesViewport_RowDeleting);
        }

        public override void MoveFirst()
        {
            v_snapshot_sub_premises.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_sub_premises.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_sub_premises.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_sub_premises.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_sub_premises.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_sub_premises.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_sub_premises.Position > -1) && (v_snapshot_sub_premises.Position < (v_snapshot_sub_premises.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_sub_premises.Position > -1) && (v_snapshot_sub_premises.Position < (v_snapshot_sub_premises.Count - 1));
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "total_area") &&
                (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == ""))
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
            if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "sub_premises_num") &&
                (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 5))
            {
                MessageBox.Show("Длина номера комнаты не может превышать 5 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 5);
            }
        }

        void SubPremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_sub_premises[row_index]).Delete();
            }
        }

        void SubPremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_sub_premises[row_index]);
                    row["id_premises"] = e.Row["id_premises"];
                    row["sub_premises_num"] = e.Row["sub_premises_num"];
                    row["total_area"] = e.Row["total_area"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_sub_premises.Rows.Add(new object[] { 
                        e.Row["id_sub_premises"], 
                        e.Row["id_premises"],                    
                        e.Row["sub_premises_num"],
                        e.Row["total_area"]
                    });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<SubPremise> list_from_view = SubPremisesFromView();
            List<SubPremise> list_from_viewport = SubPremisesFromViewport();
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

        private List<SubPremise> SubPremisesFromViewport()
        {
            List<SubPremise> list = new List<SubPremise>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    SubPremise sp = new SubPremise();
                    DataGridViewRow row = dataGridView.Rows[i];
                    sp.id_sub_premises = row.Cells["id_sub_premises"].Value == DBNull.Value ? null : (int?)Convert.ToInt32(row.Cells["id_sub_premises"].Value);
                    sp.id_premises = row.Cells["id_premises"].Value == DBNull.Value ? null : (int?)Convert.ToDouble(row.Cells["id_premises"].Value);
                    sp.sub_premises_num = row.Cells["sub_premises_num"].Value == DBNull.Value ? null : row.Cells["sub_premises_num"].Value.ToString();
                    sp.total_area = row.Cells["total_area"].Value == DBNull.Value ? null : (double?)Convert.ToDouble(row.Cells["total_area"].Value);
                    list.Add(sp);
                }
            }
            return list;
        }

        private List<SubPremise> SubPremisesFromView()
        {
            List<SubPremise> list = new List<SubPremise>();
            for (int i = 0; i < v_sub_premises.Count; i++)
            {
                SubPremise sp = new SubPremise();
                DataRowView row = ((DataRowView)v_sub_premises[i]);
                sp.id_sub_premises = row["id_sub_premises"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_sub_premises"]);
                sp.id_premises = row["id_premises"] == DBNull.Value ? null : (int?)Convert.ToDouble(row["id_premises"]);
                sp.sub_premises_num = row["sub_premises_num"] == DBNull.Value ? null : row["sub_premises_num"].ToString();
                sp.total_area = row["total_area"] == DBNull.Value ? null : (double?)row["total_area"];
                list.Add(sp);
            }
            return list;
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            menuCallback.EditingStateUpdate();
        }

        void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.SelectedCells[0].OwningColumn.Name == "total_area")
            {
                dataGridView.EditingControl.KeyPress -= new KeyPressEventHandler(EditingControl_KeyPress);
                dataGridView.EditingControl.KeyPress += new KeyPressEventHandler(EditingControl_KeyPress);
                if (((TextBox)e.Control).Text.Trim() == "")
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text = "0";
                else
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text.Substring(0, ((TextBox)e.Control).Text.Length - 3);
            }
        }

        void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView.SelectedCells[0].OwningColumn.Name == "total_area")
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == (char)8))
                    e.Handled = false;
                else
                    if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    {
                        e.KeyChar = ',';
                        if (((TextBox)dataGridView.EditingControl).Text.IndexOf(',') != -1)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                    else
                        e.Handled = true;
            }
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_sub_premises"], 
                dataRowView["id_premises"], 
                dataRowView["sub_premises_num"], 
                dataRowView["total_area"],
                dataRowView["deleted"]
            };
        }

        public override bool CanInsertRecord()
        {
            return (ParentType == ParentTypeEnum.Premises) && (ParentRow != null);
        }

        public override void InsertRecord()
        {
            if ((ParentRow == null) || (ParentType != ParentTypeEnum.Premises))
                return;
            DataRowView row = (DataRowView)v_snapshot_sub_premises.AddNew();
            row["id_premises"] = ParentRow["id_premises"];
            row["total_area"] = 0;
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения о комнатах в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            } 
            sub_premises.Select().RowChanged -= new DataRowChangeEventHandler(SubPremisesViewport_RowChanged);
            sub_premises.Select().RowDeleting -= new DataRowChangeEventHandler(SubPremisesViewport_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_sub_premises.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_sub_premises[v_snapshot_sub_premises.Position]).Row.Delete();
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_sub_premises.Clear();
            for (int i = 0; i < v_sub_premises.Count; i++)
                snapshot_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_sub_premises[i])));
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<SubPremise> list = SubPremisesFromViewport();
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = sub_premises.Select().Rows.Find(((SubPremise)list[i]).id_sub_premises);
                if (row == null)
                {
                    int id_sub_premises = sub_premises.Insert(list[i]);
                    if (id_sub_premises == -1)
                        return;
                    ((DataRowView)v_snapshot_sub_premises[i])["id_sub_premises"] = id_sub_premises;
                    sub_premises.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_sub_premises[i]));
                }
                else
                {
                    if (sub_premises.Update(list[i]) == -1)
                        return;
                    row["id_premises"] = list[i].id_premises == null ? DBNull.Value : (object)list[i].id_premises;
                    row["sub_premises_num"] = list[i].sub_premises_num == null ? DBNull.Value : (object)list[i].sub_premises_num;
                    row["total_area"] = list[i].total_area == null ? DBNull.Value : (object)list[i].total_area;
                }
            }
            list = SubPremisesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_sub_premises"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_sub_premises"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_sub_premises"].Value == list[i].id_sub_premises))
                        row_index = j;
                if (row_index == -1)
                {
                    if (sub_premises.Delete(list[i].id_sub_premises.Value) == -1)
                        return;
                    sub_premises.Select().Rows.Find(((SubPremise)list[i]).id_sub_premises).Delete();
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
            SubPremisesViewport viewport = new SubPremisesViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        public override void ForceClose()
        {
            base.Close();
        }

        public override bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        private void ConstructViewport()
        {
            this.SuspendLayout();
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).BeginInit();
            this.Controls.Add(dataGridView);
            // 
            // dataGridView
            // 
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, 
                System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                field_id_sub_premises,
                field_id_premises,
                field_sub_premises_num,
                field_total_area });
            dataGridView.Name = "dataGridView";
            dataGridView.TabIndex = 0;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.MultiSelect = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            // 
            // field_id_sub_premises
            // 
            field_id_sub_premises.HeaderText = "Внутренний номер комнаты";
            field_id_sub_premises.Name = "id_sub_premises";
            field_id_sub_premises.Visible = false;
            // 
            // field_id_premises
            // 
            field_id_premises.HeaderText = "Внутренний номер помещения";
            field_id_premises.Name = "id_premises";
            field_id_premises.Visible = false;
            // 
            // field_sub_premises_num
            // 
            field_sub_premises_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_sub_premises_num.HeaderText = "Номер комнаты";
            field_sub_premises_num.MinimumWidth = 150;
            field_sub_premises_num.Name = "sub_premises_num";
            // 
            // field_sub_premises_num
            // 
            field_sub_premises_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_sub_premises_num.HeaderText = "Номер комнаты";
            field_sub_premises_num.MinimumWidth = 150;
            field_sub_premises_num.Name = "sub_premises_num";
            // 
            // field_total_area
            // 
            field_total_area.HeaderText = "Общая площадь";
            field_total_area.MinimumWidth = 150;
            field_total_area.Name = "total_area";
            field_total_area.DefaultCellStyle.Format = "#0.0## м²";
            field_total_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
