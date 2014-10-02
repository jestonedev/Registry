using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using CustomControls;
using Registry.DataModels;

namespace Registry.Viewport
{
    internal class RestrictionListViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_restriction = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_number = new DataGridViewTextBoxColumn();
        private DateGridViewDateTimeColumn field_date = new DateGridViewDateTimeColumn();
        private DataGridViewTextBoxColumn field_description = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_restriction_type = new DataGridViewComboBoxColumn();
        #endregion Components

        //Modeles
        RestrictionsDataModel restrictions = null;
        RestrictionTypesDataModel restriction_types = null;
        DataModel restriction_assoc = null;
        DataTable snapshot_restrictions = new DataTable("snapshot_restrictions");

        //Views
        BindingSource v_restrictions = null;
        BindingSource v_restriction_types = null;
        BindingSource v_restriction_assoc = null;
        BindingSource v_snapshot_restrictions = null;

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public RestrictionListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageRestrictions";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Реквизиты";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public RestrictionListViewport(RestrictionListViewport restrictionListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = restrictionListViewport.DynamicFilter;
            this.StaticFilter = restrictionListViewport.StaticFilter;
            this.ParentRow = restrictionListViewport.ParentRow;
            this.ParentType = restrictionListViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            restrictions = RestrictionsDataModel.GetInstance();
            restriction_types = RestrictionTypesDataModel.GetInstance();
            if (ParentType == ParentTypeEnum.Premises)
                restriction_assoc = RestrictionsPremisesAssocDataModel.GetInstance();
            else
                if (ParentType == ParentTypeEnum.Building)
                    restriction_assoc = RestrictionsBuildingsAssocDataModel.GetInstance();
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");

            v_restriction_assoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
            {
                v_restriction_assoc.DataMember = "restrictions_premises_assoc";
                v_restriction_assoc.Filter = "id_premises = " + ParentRow["id_premises"].ToString();
                this.Text = String.Format("Реквизиты помещения № {0}", ParentRow["id_premises"].ToString());
            }
            else
                if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                {
                    v_restriction_assoc.DataMember = "restrictions_buildings_assoc";
                    v_restriction_assoc.Filter = "id_building = " + ParentRow["id_building"].ToString();
                    this.Text = String.Format("Реквизиты здания № {0}", ParentRow["id_building"].ToString());
                }
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            v_restriction_assoc.DataSource = DataSetManager.GetDataSet();

            v_restrictions = new BindingSource();
            v_restrictions.DataMember = "restrictions";
            v_restrictions.DataSource = DataSetManager.GetDataSet();
            //Перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();

            v_restriction_types = new BindingSource();
            v_restriction_types.DataMember = "restriction_types";
            v_restriction_types.DataSource = DataSetManager.GetDataSet();

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < restrictions.Select().Columns.Count; i++)
                snapshot_restrictions.Columns.Add(new DataColumn(restrictions.Select().Columns[i].ColumnName,
                    restrictions.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_restrictions.Count; i++)
                snapshot_restrictions.Rows.Add(DataRowViewToArray(((DataRowView)v_restrictions[i])));
            v_snapshot_restrictions = new BindingSource();
            v_snapshot_restrictions.DataSource = snapshot_restrictions;

            dataGridView.DataSource = v_snapshot_restrictions;

            field_id_restriction.DataPropertyName = "id_restriction";
            field_id_restriction_type.DataPropertyName = "id_restriction_type";
            field_id_restriction_type.DataSource = v_restriction_types;
            field_id_restriction_type.ValueMember = "id_restriction_type";
            field_id_restriction_type.DisplayMember = "restriction_type";
            field_number.DataPropertyName = "number";
            field_date.DataPropertyName = "date";
            field_description.DataPropertyName = "description";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);

            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            restrictions.Select().RowChanged += new DataRowChangeEventHandler(RestrictionListViewport_RowChanged);
            restrictions.Select().RowDeleting += new DataRowChangeEventHandler(RestrictionListViewport_RowDeleting);
            restriction_assoc.Select().RowChanged += new DataRowChangeEventHandler(RestrictionAssoc_RowChanged);
            restriction_assoc.Select().RowDeleting += new DataRowChangeEventHandler(RestrictionAssoc_RowDeleting);
        }

        public override void MoveFirst()
        {
            v_snapshot_restrictions.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_restrictions.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_restrictions.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_restrictions.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_restrictions.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_restrictions.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_restrictions.Position > -1) && (v_snapshot_restrictions.Position < (v_snapshot_restrictions.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_restrictions.Position > -1) && (v_snapshot_restrictions.Position < (v_snapshot_restrictions.Count - 1));
        }

        private void RebuildFilter()
        {
            string restrictionFilter = "id_restriction IN (0";
            for (int i = 0; i < v_restriction_assoc.Count; i++)
                restrictionFilter += ((DataRowView)v_restriction_assoc[i])["id_restriction"].ToString() + ",";
            restrictionFilter = restrictionFilter.TrimEnd(new char[] { ',' });
            restrictionFilter += ")";
            v_restrictions.Filter = restrictionFilter;
        }

        void RestrictionAssoc_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если удалена ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            if (e.Action == DataRowAction.Delete)
                RebuildFilter();
        }

        void RestrictionAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если добавлена новая ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            if (e.Action == DataRowAction.Add)
                RebuildFilter();
        }

        void RestrictionListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_restrictions.Find("id_restriction", e.Row["id_restriction"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_restrictions[row_index]).Delete();
            }
        }

        void RestrictionListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_restrictions.Find("id_restriction", e.Row["id_restriction"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_restrictions[row_index]);
                    row["id_restriction_type"] = e.Row["id_restriction_type"];
                    row["number"] = e.Row["number"];
                    row["date"] = e.Row["date"];
                    row["description"] = e.Row["description"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_restrictions.Rows.Add(new object[] { 
                        e.Row["id_restriction"], 
                        e.Row["id_restriction_type"],
                        e.Row["number"],
                        e.Row["date"],
                        e.Row["description"]
                    });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<Restriction> list_from_view = RestrictionsFromView();
            List<Restriction> list_from_viewport = RestrictionsFromViewport();
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

        private List<Restriction> RestrictionsFromViewport()
        {
            List<Restriction> list = new List<Restriction>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    Restriction r = new Restriction();
                    DataGridViewRow row = dataGridView.Rows[i];
                    r.id_restriction = row.Cells["id_restriction"].Value == DBNull.Value ? null :
                        (int?)Convert.ToInt32(row.Cells["id_restriction"].Value);
                    r.id_restriction_type = row.Cells["id_restriction_type"].Value == DBNull.Value ? null :
                        (int?)Convert.ToInt32(row.Cells["id_restriction_type"].Value);
                    r.number = row.Cells["number"].Value == DBNull.Value ? null : row.Cells["number"].Value.ToString();
                    r.date = row.Cells["date"].Value == DBNull.Value ? null :
                       (DateTime?)Convert.ToDateTime(row.Cells["date"].Value);
                    r.description = row.Cells["description"].Value == DBNull.Value ? null : row.Cells["description"].Value.ToString();
                    list.Add(r);
                }
            }
            return list;
        }

        private List<Restriction> RestrictionsFromView()
        {
            List<Restriction> list = new List<Restriction>();
            for (int i = 0; i < v_restrictions.Count; i++)
            {
                Restriction r = new Restriction();
                DataRowView row = ((DataRowView)v_restrictions[i]);
                r.id_restriction = row["id_restriction"] == DBNull.Value ? null :
                       (int?)Convert.ToInt32(row["id_restriction"]);
                r.id_restriction_type = row["id_restriction_type"] == DBNull.Value ? null :
                    (int?)Convert.ToInt32(row["id_restriction_type"]);
                r.number = row["number"] == DBNull.Value ? null : row["number"].ToString();
                r.date = row["date"] == DBNull.Value ? null :
                   (DateTime?)Convert.ToDateTime(row["date"]);
                r.description = row["description"] == DBNull.Value ? null : row["description"].ToString();
                list.Add(r);
            }
            return list;
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_restriction"], 
                dataRowView["id_restriction_type"], 
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
            DataRowView row = (DataRowView)v_snapshot_restrictions.AddNew();
            row.EndEdit();
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения о реквизитах в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            restrictions.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionListViewport_RowChanged);
            restrictions.Select().RowDeleting -= new DataRowChangeEventHandler(RestrictionListViewport_RowDeleting);
            restriction_assoc.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionAssoc_RowChanged);
            restriction_assoc.Select().RowDeleting -= new DataRowChangeEventHandler(RestrictionAssoc_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_restrictions.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_restrictions[v_snapshot_restrictions.Position]).Row.Delete();
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_restrictions.Clear();
            for (int i = 0; i < v_restrictions.Count; i++)
                snapshot_restrictions.Rows.Add(DataRowViewToArray(((DataRowView)v_restrictions[i])));
            menuCallback.EditingStateUpdate();
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        private bool ValidateViewportData(List<Restriction> list)
        {
            foreach (Restriction restriction in list)
            {
                if (restriction.date == null)
                {
                    MessageBox.Show("Не заполнена дата", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (restriction.id_restriction_type == null)
                {
                    MessageBox.Show("Не выбран тип реквизита", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<Restriction> list = RestrictionsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = restrictions.Select().Rows.Find(((Restriction)list[i]).id_restriction);
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
                    int id_restriction = restrictions.Insert(list[i], ParentType, id_parent);
                    if (id_restriction == -1)
                    {
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    ((DataRowView)v_snapshot_restrictions[i])["id_restriction"] = id_restriction;
                    restrictions.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_restrictions[i]));
                    restriction_assoc.Select().Rows.Add(new object[] { id_parent, id_restriction });
                }
                else
                {
                    if (restrictions.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    row["id_restriction_type"] = list[i].id_restriction_type == null ? DBNull.Value : (object)list[i].id_restriction_type;
                    row["number"] = list[i].number == null ? DBNull.Value : (object)list[i].number;
                    row["date"] = list[i].date == null ? DBNull.Value : (object)list[i].date;
                    row["description"] = list[i].description == null ? DBNull.Value : (object)list[i].description;
                }
            }
            list = RestrictionsFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_restriction"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_restriction"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_restriction"].Value == list[i].id_restriction))
                        row_index = j;
                if (row_index == -1)
                {
                    if (restrictions.Delete(list[i].id_restriction.Value) == -1)
                    {
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    restrictions.Select().Rows.Find(((Restriction)list[i]).id_restriction).Delete();
                }
            }
            RebuildFilter();
            sync_views = true;
            menuCallback.NavigationStateUpdate();
            menuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "number") &&
                (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 10))
            {
                MessageBox.Show("Длина номера реквизита не может превышать 10 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 10);
            }
            else
                if ((dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == "description") &&
                                (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 255))
                {
                    MessageBox.Show("Длина наименования реквизита не может превышать 255 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 255);
                }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            RestrictionListViewport viewport = new RestrictionListViewport(this, menuCallback);
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
            this.field_id_restriction,
            this.field_number,
            this.field_date,
            this.field_description,
            this.field_id_restriction_type});
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.TabIndex = 1;
            // 
            // field_id_restriction
            // 
            this.field_id_restriction.HeaderText = "Идентификатор реквизита";
            this.field_id_restriction.Name = "id_restriction";
            this.field_id_restriction.Visible = false;
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
            // field_restriction_type
            // 
            this.field_id_restriction_type.HeaderText = "Тип реквизита";
            this.field_id_restriction_type.Name = "id_restriction_type";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }

    }
}
