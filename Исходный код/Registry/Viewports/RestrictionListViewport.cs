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
    internal sealed class RestrictionListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        RestrictionsDataModel restrictions = null;
        RestrictionTypesDataModel restriction_types = null;
        DataModel restriction_assoc = null;
        DataTable snapshot_restrictions = new DataTable("snapshot_restrictions");
        #endregion Models

        #region Views
        BindingSource v_restrictions = null;
        BindingSource v_restriction_types = null;
        BindingSource v_restriction_assoc = null;
        BindingSource v_snapshot_restrictions = null;
        #endregion Views
        private DataGridViewTextBoxColumn id_restriction;
        private DataGridViewTextBoxColumn number;
        private DataGridViewDateTimeColumn date;
        private DataGridViewTextBoxColumn description;
        private DataGridViewComboBoxColumn id_restriction_type;


        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private RestrictionListViewport()
            : this(null)
        {
        }

        public RestrictionListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
        }

        public RestrictionListViewport(RestrictionListViewport restrictionListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = restrictionListViewport.DynamicFilter;
            this.StaticFilter = restrictionListViewport.StaticFilter;
            this.ParentRow = restrictionListViewport.ParentRow;
            this.ParentType = restrictionListViewport.ParentType;
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

        private bool ValidateViewportData(List<Restriction> list)
        {
            foreach (Restriction restriction in list)
            {
                if (restriction.number != null && restriction.number.Length > 10)
                {
                    MessageBox.Show("Номер реквизита не может привышать 10 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (restriction.date == null)
                {
                    MessageBox.Show("Не заполнена дата", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (restriction.description != null && restriction.description.Length > 255)
                {
                    MessageBox.Show("Длина наименования реквизита не может превышать 255 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private Restriction RowToRestriction(DataRow row)
        {
            Restriction restriction = new Restriction();
            restriction.id_restriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction");
            restriction.id_restriction_type = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
            restriction.number = ViewportHelper.ValueOrNull(row, "number");
            restriction.date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
            restriction.description = ViewportHelper.ValueOrNull(row, "description");
            return restriction;
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
                    r.id_restriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction");
                    r.id_restriction_type = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                    r.number = ViewportHelper.ValueOrNull(row, "number");
                    r.date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                    r.description = ViewportHelper.ValueOrNull(row, "description");
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
                r.id_restriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction");
                r.id_restriction_type = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                r.number = ViewportHelper.ValueOrNull(row, "number");
                r.date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                r.description = ViewportHelper.ValueOrNull(row, "description");
                list.Add(r);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_restrictions.Count;
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            restrictions = RestrictionsDataModel.GetInstance();
            restriction_types = RestrictionTypesDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            restrictions.Select();
            restriction_types.Select();

            if (ParentType == ParentTypeEnum.Premises)
                restriction_assoc = RestrictionsPremisesAssocDataModel.GetInstance();
            else
                if (ParentType == ParentTypeEnum.Building)
                    restriction_assoc = RestrictionsBuildingsAssocDataModel.GetInstance();
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            restriction_assoc.Select();

            v_restriction_assoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
            {
                v_restriction_assoc.DataMember = "restrictions_premises_assoc";
                v_restriction_assoc.Filter = "id_premises = " + ParentRow["id_premises"].ToString();
                this.Text = String.Format("Реквизиты помещения №{0}", ParentRow["id_premises"].ToString());
            }
            else
                if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                {
                    v_restriction_assoc.DataMember = "restrictions_buildings_assoc";
                    v_restriction_assoc.Filter = "id_building = " + ParentRow["id_building"].ToString();
                    this.Text = String.Format("Реквизиты здания №{0}", ParentRow["id_building"].ToString());
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
            v_snapshot_restrictions.CurrentItemChanged += new EventHandler(v_snapshot_restrictions_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_restrictions;

            id_restriction.DataPropertyName = "id_restriction";
            id_restriction_type.DataSource = v_restriction_types;
            id_restriction_type.ValueMember = "id_restriction_type";
            id_restriction_type.DisplayMember = "restriction_type";
            id_restriction_type.DataPropertyName = "id_restriction_type";
            number.DataPropertyName = "number";
            date.DataPropertyName = "date";
            description.DataPropertyName = "description";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);

            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            restrictions.Select().RowChanged += new DataRowChangeEventHandler(RestrictionListViewport_RowChanged);
            restrictions.Select().RowDeleting += new DataRowChangeEventHandler(RestrictionListViewport_RowDeleting);
            restriction_assoc.Select().RowChanged += new DataRowChangeEventHandler(RestrictionAssoc_RowChanged);
            restriction_assoc.Select().RowDeleted += new DataRowChangeEventHandler(RestrictionAssoc_RowDeleted);
        }
        
        public override bool CanInsertRecord()
        {
            return true;
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_restrictions.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_restrictions.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_restrictions[v_snapshot_restrictions.Position]).Row.Delete();
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
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
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
                    if (RowToRestriction(row) == list[i])
                        continue;
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
            menuCallback.EditingStateUpdate();
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
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
                    {
                        e.Cancel = true;
                        return;
                    }
            }
            restrictions.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionListViewport_RowChanged);
            restrictions.Select().RowDeleting -= new DataRowChangeEventHandler(RestrictionListViewport_RowDeleting);
            restriction_assoc.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionAssoc_RowChanged);
            restriction_assoc.Select().RowDeleted -= new DataRowChangeEventHandler(RestrictionAssoc_RowDeleted);
        }

        public override void ForceClose()
        {
            restrictions.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionListViewport_RowChanged);
            restrictions.Select().RowDeleting -= new DataRowChangeEventHandler(RestrictionListViewport_RowDeleting);
            restriction_assoc.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionAssoc_RowChanged);
            restriction_assoc.Select().RowDeleted -= new DataRowChangeEventHandler(RestrictionAssoc_RowDeleted);
            base.Close();
        }

        void RestrictionAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
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
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    int row_index = v_restrictions.Find("id_restriction", e.Row["id_restriction"]);
                    if (row_index != -1)
                        snapshot_restrictions.Rows.Add(new object[] { 
                            e.Row["id_restriction"], 
                            e.Row["id_restriction_type"],
                            e.Row["number"],
                            e.Row["date"],
                            e.Row["description"]
                        });
                }
        }
        
        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            menuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "number":
                    if (cell.Value.ToString().Trim().Length > 10)
                        cell.ErrorText = "Длина номера реквизита не может превышать 10 символов";
                    else
                        cell.ErrorText = "";
                    break;
                case "date":
                    if (cell.Value.ToString().Trim().Trim() == "")
                        cell.ErrorText = "Не заполнена дата";
                    else
                        cell.ErrorText = "";
                    break;
                case "description":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования реквизита не может превышать 255 символов";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void v_snapshot_restrictions_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_restriction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new CustomControls.DataGridViewDateTimeColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_restriction_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_restriction,
            this.number,
            this.date,
            this.description,
            this.id_restriction_type});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(849, 385);
            this.dataGridView.TabIndex = 1;
            // 
            // id_restriction
            // 
            this.id_restriction.HeaderText = "Идентификатор реквизита";
            this.id_restriction.Name = "id_restriction";
            this.id_restriction.Visible = false;
            // 
            // number
            // 
            this.number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.number.HeaderText = "Номер";
            this.number.MinimumWidth = 150;
            this.number.Name = "number";
            this.number.Width = 150;
            // 
            // date
            // 
            this.date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.date.HeaderText = "Дата";
            this.date.MinimumWidth = 150;
            this.date.Name = "date";
            this.date.Width = 150;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Наименование";
            this.description.MinimumWidth = 300;
            this.description.Name = "description";
            // 
            // id_restriction_type
            // 
            this.id_restriction_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.id_restriction_type.FillWeight = 200F;
            this.id_restriction_type.HeaderText = "Тип реквизита";
            this.id_restriction_type.MinimumWidth = 200;
            this.id_restriction_type.Name = "id_restriction_type";
            this.id_restriction_type.Width = 200;
            // 
            // RestrictionListViewport
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(855, 391);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "RestrictionListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Реквизиты";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
