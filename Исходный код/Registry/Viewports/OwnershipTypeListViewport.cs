using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

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
        OwnershipRightTypesDataModel ownership_right_types;
        DataTable snapshot_ownership_right_types = new DataTable("snapshot_ownership_right_types");
        #endregion Models

        #region Views
        BindingSource v_ownership_right_types;
        BindingSource v_snapshot_ownership_right_types;
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
            var list_from_view = OwnershipRightTypesFromView();
            var list_from_viewport = OwnershipRightTypesFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            var founded = false;
            for (var i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (var j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_ownership_right_type"], 
                dataRowView["ownership_right_type"]
            };
        }

        private static bool ValidateViewportData(List<OwnershipRightType> list)
        {
            foreach (var ownershipRightType in list)
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
            var ownershipRightType = new OwnershipRightType();
            ownershipRightType.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
            ownershipRightType.OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type");
            return ownershipRightType;
        }

        private List<OwnershipRightType> OwnershipRightTypesFromViewport()
        {
            var list = new List<OwnershipRightType>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var ort = new OwnershipRightType();
                    var row = dataGridView.Rows[i];
                    ort.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                    ort.OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type");
                    list.Add(ort);
                }
            }
            return list;
        }

        private List<OwnershipRightType> OwnershipRightTypesFromView()
        {
            var list = new List<OwnershipRightType>();
            for (var i = 0; i < v_ownership_right_types.Count; i++)
            {
                var ort = new OwnershipRightType();
                var row = ((DataRowView)v_ownership_right_types[i]);
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
            DockAreas = DockAreas.Document;
            ownership_right_types = OwnershipRightTypesDataModel.GetInstance();
            ownership_right_types.Select();

            v_ownership_right_types = new BindingSource();
            v_ownership_right_types.DataMember = "ownership_right_types";
            v_ownership_right_types.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < ownership_right_types.Select().Columns.Count; i++)
                snapshot_ownership_right_types.Columns.Add(new DataColumn(ownership_right_types.Select().Columns[i].ColumnName,
                    ownership_right_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_ownership_right_types.Count; i++)
                snapshot_ownership_right_types.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_right_types[i])));
            v_snapshot_ownership_right_types = new BindingSource();
            v_snapshot_ownership_right_types.DataSource = snapshot_ownership_right_types;
            v_snapshot_ownership_right_types.CurrentItemChanged += v_snapshot_ownership_right_types_CurrentItemChanged;

            dataGridView.DataSource = v_snapshot_ownership_right_types;
            id_ownership_right_type.DataPropertyName = "id_ownership_right_type";
            ownership_right_type.DataPropertyName = "ownership_right_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            ownership_right_types.Select().RowChanged += OwnershipTypeListViewport_RowChanged;
            ownership_right_types.Select().RowDeleting += OwnershipTypeListViewport_RowDeleting;
            ownership_right_types.Select().RowDeleted += OwnershipTypeListViewport_RowDeleted;
        }
        
        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)v_snapshot_ownership_right_types.AddNew();
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
            for (var i = 0; i < v_ownership_right_types.Count; i++)
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
            var list = OwnershipRightTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                ownership_right_types.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = ownership_right_types.Select().Rows.Find(list[i].IdOwnershipRightType);
                if (row == null)
                {
                    var id_ownership_right_type = OwnershipRightTypesDataModel.Insert(list[i]);
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
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_ownership_right_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_ownership_right_type"].Value.ToString()) &&
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
                    ownership_right_types.Select().Rows.Find(list[i].IdOwnershipRightType).Delete();
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
            var viewport = new OwnershipTypeListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show("Сохранить изменения о типах ограничений в базу данных?", "Внимание",
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
            ownership_right_types.Select().RowChanged -= OwnershipTypeListViewport_RowChanged;
            ownership_right_types.Select().RowDeleting -= OwnershipTypeListViewport_RowDeleting;
            ownership_right_types.Select().RowDeleted -= OwnershipTypeListViewport_RowDeleted;
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
                var row_index = v_snapshot_ownership_right_types.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_ownership_right_types[row_index]).Delete();
            }
        }

        void OwnershipTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = v_snapshot_ownership_right_types.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]);
            if (row_index == -1 && v_ownership_right_types.Find("id_ownership_right_type", e.Row["id_ownership_right_type"]) != -1)
            {
                snapshot_ownership_right_types.Rows.Add(e.Row["id_ownership_right_type"], e.Row["ownership_right_type"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)v_snapshot_ownership_right_types[row_index]);
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
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "ownership_right_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина названия типа ограничения не может превышать 255 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название типа ограничения не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(OwnershipTypeListViewport));
            dataGridView = new DataGridView();
            id_ownership_right_type = new DataGridViewTextBoxColumn();
            ownership_right_type = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_ownership_right_type, ownership_right_type);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(455, 255);
            dataGridView.TabIndex = 4;
            // 
            // id_ownership_right_type
            // 
            id_ownership_right_type.HeaderText = "Идентификатор реквизита";
            id_ownership_right_type.Name = "id_ownership_right_type";
            id_ownership_right_type.Visible = false;
            // 
            // ownership_right_type
            // 
            ownership_right_type.HeaderText = "Наименование";
            ownership_right_type.Name = "ownership_right_type";
            // 
            // OwnershipTypeListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(461, 261);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "OwnershipTypeListViewport";
            Padding = new Padding(3);
            Text = "Типы оснований";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
