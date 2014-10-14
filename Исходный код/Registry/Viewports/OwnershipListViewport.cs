using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CustomControls;
using Registry.DataModels;
using Registry.Entities;

namespace Registry.Viewport
{
    internal sealed class OwnershipListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_ownership_right = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_number = new DataGridViewTextBoxColumn();
        private DateGridViewDateTimeColumn field_date = new DateGridViewDateTimeColumn();
        private DataGridViewTextBoxColumn field_description = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_ownership_right_type = new DataGridViewComboBoxColumn();
        #endregion Components

        //Modeles
        OwnershipsRightsDataModel ownership_rights = null;
        OwnershipRightTypesDataModel ownerships_rights_types = null;
        DataModel ownership_assoc = null;
        DataTable snapshot_ownerships_rights = new DataTable("snapshot_ownerships_rights");

        //Views
        BindingSource v_ownership_rights = null;
        BindingSource v_ownership_right_types = null;
        BindingSource v_ownership_assoc = null;
        BindingSource v_snapshot_ownerships_rights = null;

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public OwnershipListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageOwnerships";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Ограничения";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public OwnershipListViewport(OwnershipListViewport ownershipListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = ownershipListViewport.DynamicFilter;
            this.StaticFilter = ownershipListViewport.StaticFilter;
            this.ParentRow = ownershipListViewport.ParentRow;
            this.ParentType = ownershipListViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            ownership_rights = OwnershipsRightsDataModel.GetInstance();
            ownerships_rights_types = OwnershipRightTypesDataModel.GetInstance();
            if (ParentType == ParentTypeEnum.Premises)
                ownership_assoc = OwnershipPremisesAssocDataModel.GetInstance();
            else
                if (ParentType == ParentTypeEnum.Building)
                    ownership_assoc = OwnershipBuildingsAssocDataModel.GetInstance();
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");

            v_ownership_assoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
            {
                v_ownership_assoc.DataMember = "ownership_premises_assoc";
                v_ownership_assoc.Filter = "id_premises = " + ParentRow["id_premises"].ToString();
                this.Text = String.Format("Ограничения помещения № {0}", ParentRow["id_premises"].ToString());
            }
            else
                if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                {
                    v_ownership_assoc.DataMember = "ownership_buildings_assoc";
                    v_ownership_assoc.Filter = "id_building = " + ParentRow["id_building"].ToString();
                    this.Text = String.Format("Ограничения здания № {0}", ParentRow["id_building"].ToString());
                }
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            v_ownership_assoc.DataSource = DataSetManager.GetDataSet();

            v_ownership_rights = new BindingSource();
            v_ownership_rights.DataMember = "ownership_rights";
            v_ownership_rights.DataSource = DataSetManager.GetDataSet();
            //Перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();

            v_ownership_right_types = new BindingSource();
            v_ownership_right_types.DataMember = "ownership_right_types";
            v_ownership_right_types.DataSource = DataSetManager.GetDataSet();

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < ownership_rights.Select().Columns.Count; i++)
                snapshot_ownerships_rights.Columns.Add(new DataColumn(ownership_rights.Select().Columns[i].ColumnName, 
                    ownership_rights.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_ownership_rights.Count; i++)
                snapshot_ownerships_rights.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_rights[i])));
            v_snapshot_ownerships_rights = new BindingSource();
            v_snapshot_ownerships_rights.DataSource = snapshot_ownerships_rights;
            v_snapshot_ownerships_rights.CurrentItemChanged += new EventHandler(v_snapshot_ownerships_rights_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_ownerships_rights;

            field_id_ownership_right.DataPropertyName = "id_ownership_right";
            field_id_ownership_right_type.DataPropertyName = "id_ownership_right_type";
            field_id_ownership_right_type.DataSource = v_ownership_right_types;
            field_id_ownership_right_type.ValueMember = "id_ownership_right_type";
            field_id_ownership_right_type.DisplayMember = "ownership_right_type";
            field_number.DataPropertyName = "number";
            field_date.DataPropertyName = "date";
            field_description.DataPropertyName = "description";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);

            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            ownership_rights.Select().RowChanged += new DataRowChangeEventHandler(OwnershipListViewport_RowChanged);
            ownership_rights.Select().RowDeleting += new DataRowChangeEventHandler(OwnershipListViewport_RowDeleting);
            ownership_assoc.Select().RowChanged += new DataRowChangeEventHandler(OwnershipAssoc_RowChanged);
            ownership_assoc.Select().RowDeleting += new DataRowChangeEventHandler(OwnershipAssoc_RowDeleting);
        }

        void v_snapshot_ownerships_rights_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        public override void MoveFirst()
        {
            v_snapshot_ownerships_rights.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_ownerships_rights.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_ownerships_rights.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_ownerships_rights.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_ownerships_rights.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_ownerships_rights.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_ownerships_rights.Position > -1) && (v_snapshot_ownerships_rights.Position < (v_snapshot_ownerships_rights.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_ownerships_rights.Position > -1) && (v_snapshot_ownerships_rights.Position < (v_snapshot_ownerships_rights.Count - 1));
        }

        private void RebuildFilter()
        {
            string ownershipFilter = "id_ownership_right IN (0";
            for (int i = 0; i < v_ownership_assoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownership_assoc[i])["id_ownership_right"].ToString() + ",";
            ownershipFilter = ownershipFilter.TrimEnd(new char[] { ',' });
            ownershipFilter += ")";
            v_ownership_rights.Filter = ownershipFilter;
        }

        void OwnershipAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если добавлена новая ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            if (e.Action == DataRowAction.Add)
                RebuildFilter();
        }

        void OwnershipAssoc_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если удалена ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            if (e.Action == DataRowAction.Delete)
                RebuildFilter();
        }

        void OwnershipListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_ownerships_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_ownerships_rights[row_index]).Delete();
            }
        }

        void OwnershipListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_ownerships_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_ownerships_rights[row_index]);
                    row["id_ownership_right_type"] = e.Row["id_ownership_right_type"];
                    row["number"] = e.Row["number"];
                    row["date"] = e.Row["date"];
                    row["description"] = e.Row["description"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    int row_index = v_ownership_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
                    if (row_index != -1)
                        snapshot_ownerships_rights.Rows.Add(new object[] { 
                            e.Row["id_ownership_right"], 
                            e.Row["id_ownership_right_type"],
                            e.Row["number"],
                            e.Row["date"],
                            e.Row["description"]
                        });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<OwnershipRight> list_from_view = OwnershipRightsFromView();
            List<OwnershipRight> list_from_viewport = OwnershipRightsFromViewport();
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

        private List<OwnershipRight> OwnershipRightsFromViewport()
        {
            List<OwnershipRight> list = new List<OwnershipRight>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    OwnershipRight or = new OwnershipRight();
                    DataGridViewRow row = dataGridView.Rows[i];
                    or.id_ownership_right = row.Cells["id_ownership_right"].Value == DBNull.Value ? null : 
                        (int?)Convert.ToInt32(row.Cells["id_ownership_right"].Value);
                    or.id_ownership_right_type = row.Cells["id_ownership_right_type"].Value == DBNull.Value ? null :
                        (int?)Convert.ToInt32(row.Cells["id_ownership_right_type"].Value);
                    or.number = row.Cells["number"].Value == DBNull.Value ? null : row.Cells["number"].Value.ToString();
                    or.date = row.Cells["date"].Value == DBNull.Value ? null :
                       (DateTime?)Convert.ToDateTime(row.Cells["date"].Value);
                    or.description = row.Cells["description"].Value == DBNull.Value ? null : row.Cells["description"].Value.ToString();
                    list.Add(or);
                }
            }
            return list;
        }

        private List<OwnershipRight> OwnershipRightsFromView()
        {
            List<OwnershipRight> list = new List<OwnershipRight>();
            for (int i = 0; i < v_ownership_rights.Count; i++)
            {
                OwnershipRight or = new OwnershipRight();
                DataRowView row = ((DataRowView)v_ownership_rights[i]);
                or.id_ownership_right = row["id_ownership_right"] == DBNull.Value ? null :
                       (int?)Convert.ToInt32(row["id_ownership_right"]);
                or.id_ownership_right_type = row["id_ownership_right_type"] == DBNull.Value ? null :
                    (int?)Convert.ToInt32(row["id_ownership_right_type"]);
                or.number = row["number"] == DBNull.Value ? null : row["number"].ToString();
                or.date = row["date"] == DBNull.Value ? null :
                   (DateTime?)Convert.ToDateTime(row["date"]);
                or.description = row["description"] == DBNull.Value ? null : row["description"].ToString();
                list.Add(or);
            }
            return list;
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_ownership_right"], 
                dataRowView["id_ownership_right_type"], 
                dataRowView["number"], 
                dataRowView["date"], 
                dataRowView["description"]
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
            DataRowView row = (DataRowView)v_snapshot_ownerships_rights.AddNew();
            row.EndEdit();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения об ограничениях в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            ownership_rights.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipListViewport_RowChanged);
            ownership_rights.Select().RowDeleting -= new DataRowChangeEventHandler(OwnershipListViewport_RowDeleting);
            ownership_assoc.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipAssoc_RowChanged);
            ownership_assoc.Select().RowDeleting -= new DataRowChangeEventHandler(OwnershipAssoc_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_ownerships_rights.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_ownerships_rights[v_snapshot_ownerships_rights.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_ownerships_rights.Clear();
            for (int i = 0; i < v_ownership_rights.Count; i++)
                snapshot_ownerships_rights.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_rights[i])));
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        private bool ValidateViewportData(List<OwnershipRight> list)
        {
            foreach (OwnershipRight ownershipRight in list)
            {
                if (ownershipRight.date == null)
                {
                    MessageBox.Show("Не заполнена дата начала действия ограничения, установленного в отношении муниципальной собственности",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (ownershipRight.id_ownership_right_type == null)
                {
                    MessageBox.Show("Не выбран тип ограничения, установленного в отношении муниципальной собственности",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<OwnershipRight> list = OwnershipRightsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = ownership_rights.Select().Rows.Find(((OwnershipRight)list[i]).id_ownership_right);
                if (row == null)
                {
                    int id_parent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] :
                        ((ParentType == ParentTypeEnum.Building) && ParentRow != null) ? (int)ParentRow["id_building"] :
                        -1;
                    if (id_parent == -1)
                    {
                        MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору", 
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    int id_ownership_right = ownership_rights.Insert(list[i], ParentType, id_parent);
                    if (id_ownership_right == -1)
                    {
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    ((DataRowView)v_snapshot_ownerships_rights[i])["id_ownership_right"] = id_ownership_right;
                    ownership_rights.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_ownerships_rights[i]));
                    ownership_assoc.Select().Rows.Add(new object[] { id_parent, id_ownership_right });
                }
                else
                {
                    if (ownership_rights.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    row["id_ownership_right_type"] = list[i].id_ownership_right_type == null ? DBNull.Value : (object)list[i].id_ownership_right_type;
                    row["number"] = list[i].number == null ? DBNull.Value : (object)list[i].number;
                    row["date"] = list[i].date == null ? DBNull.Value : (object)list[i].date;
                    row["description"] = list[i].description == null ? DBNull.Value : (object)list[i].description;
                }
            }
            list = OwnershipRightsFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_ownership_right"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_ownership_right"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_ownership_right"].Value == list[i].id_ownership_right))
                        row_index = j;
                if (row_index == -1)
                {
                    if (ownership_rights.Delete(list[i].id_ownership_right.Value) == -1)
                    {
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    ownership_rights.Select().Rows.Find(((OwnershipRight)list[i]).id_ownership_right).Delete();
                    RebuildFilter();
                }
            }
            RebuildFilter();
            sync_views = true;
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "number") &&
                (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 10))
            {
                MessageBox.Show("Длина номера ограничения не может превышать 10 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 10);
            } else
            if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "description") &&
                            (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 255))
            {
                MessageBox.Show("Длина наименования ограничения не может превышать 255 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 255);
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            OwnershipListViewport viewport = new OwnershipListViewport(this, menuCallback);
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

        public override int GetRecordCount()
        {
            return v_snapshot_ownerships_rights.Count;
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
            this.field_id_ownership_right,
            this.field_number,
            this.field_date,
            this.field_description,
            this.field_id_ownership_right_type});
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.TabIndex = 2;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.AllowUserToResizeRows = false;
            // 
            // field_id_ownership
            // 
            this.field_id_ownership_right.HeaderText = "Идентификатор основания";
            this.field_id_ownership_right.Name = "id_ownership_right";
            this.field_id_ownership_right.Visible = false;
            // 
            // field_number
            // 
            this.field_number.HeaderText = "Номер";
            this.field_number.Name = "number";
            // 
            // field_date
            // 
            this.field_date.HeaderText = "Дата";
            this.field_date.Name = "date";
            // 
            // field_description
            // 
            this.field_description.HeaderText = "Наименование";
            this.field_description.Name = "description";
            // 
            // field_ownership_right_type
            // 
            this.field_id_ownership_right_type.HeaderText = "Тип ограничения";
            this.field_id_ownership_right_type.Name = "id_ownership_right_type";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
