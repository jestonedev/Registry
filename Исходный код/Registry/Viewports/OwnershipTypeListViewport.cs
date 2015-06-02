using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;
using Security;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class OwnershipTypeListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_ownership_right_type;
        private DataGridViewTextBoxColumn ownership_right_type;
        #endregion Components

        #region Models
        OwnershipRightTypesDataModel ownership_right_types = null;
        DataTable snapshot_ownership_right_types = new DataTable("snapshot_ownership_right_types");
        #endregion Models

        #region Views
        BindingSource v_ownership_right_types = null;
        BindingSource v_snapshot_ownership_right_types = null;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private OwnershipTypeListViewport()
            : this(null)
        {
        }

        public OwnershipTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            snapshot_ownership_right_types.Locale = CultureInfo.InvariantCulture;
        }

        public OwnershipTypeListViewport(OwnershipTypeListViewport ownershipTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
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

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_ownership_right_type"], 
                dataRowView["ownership_right_type"]
            };
        }

        private static bool ValidateViewportData(List<OwnershipRightType> list)
        {
            foreach (OwnershipRightType ownershipRightType in list)
            {
                if (ownershipRightType.OwnershipRightTypeName == null)
                {
                    MessageBox.Show("Не заполнено наименование типа ограничения", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRightType.OwnershipRightTypeName != null && ownershipRightType.OwnershipRightTypeName.Length > 255)
                {
                    MessageBox.Show("Длина названия типа ограничения не может превышать 255 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static OwnershipRightType RowToOwnershipRightType(DataRow row)
        {
            OwnershipRightType ownershipRightType = new OwnershipRightType();
            ownershipRightType.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
            ownershipRightType.OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type");
            return ownershipRightType;
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
                    ort.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                    ort.OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type");
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
                ort.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                ort.OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type");
                list.Add(ort);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_ownership_right_types.Count;
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            ownership_right_types = OwnershipRightTypesDataModel.GetInstance();
            ownership_right_types.Select();

            v_ownership_right_types = new BindingSource();
            v_ownership_right_types.DataMember = "ownership_right_types";
            v_ownership_right_types.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < ownership_right_types.Select().Columns.Count; i++)
                snapshot_ownership_right_types.Columns.Add(new DataColumn(ownership_right_types.Select().Columns[i].ColumnName,
                    ownership_right_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_ownership_right_types.Count; i++)
                snapshot_ownership_right_types.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_right_types[i])));
            v_snapshot_ownership_right_types = new BindingSource();
            v_snapshot_ownership_right_types.DataSource = snapshot_ownership_right_types;
            v_snapshot_ownership_right_types.CurrentItemChanged += new EventHandler(v_snapshot_ownership_right_types_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_ownership_right_types;
            id_ownership_right_type.DataPropertyName = "id_ownership_right_type";
            ownership_right_type.DataPropertyName = "ownership_right_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            ownership_right_types.Select().RowChanged += new DataRowChangeEventHandler(OwnershipTypeListViewport_RowChanged);
            ownership_right_types.Select().RowDeleting += new DataRowChangeEventHandler(OwnershipTypeListViewport_RowDeleting);
            ownership_right_types.Select().RowDeleted += new DataRowChangeEventHandler(OwnershipTypeListViewport_RowDeleted);
        }
        
        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_ownership_right_types.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_ownership_right_types.Position != -1) && AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_ownership_right_types[v_snapshot_ownership_right_types.Position]).Row.Delete();
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
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            ownership_right_types.EditingNewRecord = true;
            List<OwnershipRightType> list = OwnershipRightTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                ownership_right_types.EditingNewRecord = false;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = ownership_right_types.Select().Rows.Find(((OwnershipRightType)list[i]).IdOwnershipRightType);
                if (row == null)
                {
                    int id_ownership_right_type = OwnershipRightTypesDataModel.Insert(list[i]);
                    if (id_ownership_right_type == -1)
                    {
                        sync_views = true;
                        ownership_right_types.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_ownership_right_types[i])["id_ownership_right_type"] = id_ownership_right_type;
                    ownership_right_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_ownership_right_types[i]));
                }
                else
                {
                    if (RowToOwnershipRightType(row) == list[i])
                        continue;
                    if (OwnershipRightTypesDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        ownership_right_types.EditingNewRecord = false;
                        return;
                    }
                    row["ownership_right_type"] = list[i].OwnershipRightTypeName == null ? DBNull.Value : (object)list[i].OwnershipRightTypeName;
                }
            }
            list = OwnershipRightTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_ownership_right_type"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_ownership_right_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_ownership_right_type"].Value == list[i].IdOwnershipRightType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (OwnershipRightTypesDataModel.Delete(list[i].IdOwnershipRightType.Value) == -1)
                    {
                        sync_views = true;
                        ownership_right_types.EditingNewRecord = false;
                        return;
                    }
                    ownership_right_types.Select().Rows.Find(((OwnershipRightType)list[i]).IdOwnershipRightType).Delete();
                }
            }
            sync_views = true;
            ownership_right_types.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            OwnershipTypeListViewport viewport = new OwnershipTypeListViewport(this, MenuCallback);
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
                DialogResult result = MessageBox.Show("Сохранить изменения о типах ограничений в базу данных?", "Внимание",
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
            ownership_right_types.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipTypeListViewport_RowChanged);
            ownership_right_types.Select().RowDeleting -= new DataRowChangeEventHandler(OwnershipTypeListViewport_RowDeleting);
            ownership_right_types.Select().RowDeleted -= new DataRowChangeEventHandler(OwnershipTypeListViewport_RowDeleted);
            base.OnClosing(e);
        }

        private void OwnershipTypeListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
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
            int row_index = v_snapshot_ownership_right_types.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]);
            if (row_index == -1 && v_ownership_right_types.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]) != -1)
            {
                snapshot_ownership_right_types.Rows.Add(new object[] { 
                        e.Row["id_ownership_right_type"], 
                        e.Row["ownership_right_type"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_ownership_right_types[row_index]);
                    row["ownership_right_type"] = e.Row["ownership_right_type"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshot_ownership_right_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "ownership_right_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина названия типа ограничения не может превышать 255 символов";
                    else
                        if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название типа ограничения не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OwnershipTypeListViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_ownership_right_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_right_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_ownership_right_type,
            this.ownership_right_type});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(455, 255);
            this.dataGridView.TabIndex = 4;
            // 
            // id_ownership_right_type
            // 
            this.id_ownership_right_type.HeaderText = "Идентификатор реквизита";
            this.id_ownership_right_type.Name = "id_ownership_right_type";
            this.id_ownership_right_type.Visible = false;
            // 
            // ownership_right_type
            // 
            this.ownership_right_type.HeaderText = "Наименование";
            this.ownership_right_type.Name = "ownership_right_type";
            // 
            // OwnershipTypeListViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(461, 261);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OwnershipTypeListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Типы оснований";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
