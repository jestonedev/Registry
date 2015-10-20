using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class StructureTypeListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_structure_type;
        private DataGridViewTextBoxColumn structure_type;
        #endregion Components

        #region Models
        DataModel structure_types;
        DataTable snapshot_structure_types = new DataTable("snapshot_structure_types");
        #endregion Models

        #region Views
        BindingSource v_structure_types;
        BindingSource v_snapshot_structure_types;
        #endregion Views

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private StructureTypeListViewport()
            : this(null)
        {
        }

        public StructureTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            snapshot_structure_types.Locale = CultureInfo.InvariantCulture;
        }

        public StructureTypeListViewport(StructureTypeListViewport structureTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = StructureTypesFromView();
            var list_from_viewport = StructureTypesFromViewport();
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
                dataRowView["id_structure_type"], 
                dataRowView["structure_type"]
            };
        }

        private static bool ValidateViewportData(List<StructureType> list)
        {
            foreach (var structureType in list)
            {
                if (structureType.StructureTypeName == null)
                {
                    MessageBox.Show("Не заполнено наименование структуры здания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (structureType.StructureTypeName != null && structureType.StructureTypeName.Length > 255)
                {
                    MessageBox.Show("Длина названия структуры здания не может превышать 255 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static StructureType RowToStructureType(DataRow row)
        {
            var structureType = new StructureType();
            structureType.IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
            structureType.StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type");
            return structureType;
        }

        private List<StructureType> StructureTypesFromViewport()
        {
            var list = new List<StructureType>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var st = new StructureType();
                    var row = dataGridView.Rows[i];
                    st.IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
                    st.StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type");
                    list.Add(st);
                }
            }
            return list;
        }

        private List<StructureType> StructureTypesFromView()
        {
            var list = new List<StructureType>();
            for (var i = 0; i < v_structure_types.Count; i++)
            {
                var st = new StructureType();
                var row = ((DataRowView)v_structure_types[i]);
                st.IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
                st.StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type");
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_structure_types.Count;
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            structure_types = DataModel.GetInstance(DataModelType.StructureTypesDataModel);
            //Ожиданем дозагрузки данных, если это необходимо
            structure_types.Select();

            v_structure_types = new BindingSource();
            v_structure_types.DataMember = "structure_types";
            v_structure_types.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < structure_types.Select().Columns.Count; i++)
                snapshot_structure_types.Columns.Add(new DataColumn(
                    structure_types.Select().Columns[i].ColumnName, structure_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_structure_types.Count; i++)
                snapshot_structure_types.Rows.Add(DataRowViewToArray(((DataRowView)v_structure_types[i])));
            v_snapshot_structure_types = new BindingSource();
            v_snapshot_structure_types.DataSource = snapshot_structure_types;
            v_snapshot_structure_types.CurrentItemChanged += v_snapshot_structure_types_CurrentItemChanged;

            dataGridView.DataSource = v_snapshot_structure_types;
            id_structure_type.DataPropertyName = "id_structure_type";
            structure_type.DataPropertyName = "structure_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            structure_types.Select().RowChanged += StructureTypeListViewport_RowChanged;
            structure_types.Select().RowDeleting += StructureTypeListViewport_RowDeleting;
            structure_types.Select().RowDeleted += StructureTypeListViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)v_snapshot_structure_types.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_structure_types.Position != -1) && AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_structure_types[v_snapshot_structure_types.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_structure_types.Clear();
            for (var i = 0; i < v_structure_types.Count; i++)
                snapshot_structure_types.Rows.Add(DataRowViewToArray(((DataRowView)v_structure_types[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            structure_types.EditingNewRecord = true;
            var list = StructureTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                structure_types.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = structure_types.Select().Rows.Find(list[i].IdStructureType);
                if (row == null)
                {
                    var id_structure_type = structure_types.Insert(list[i]);
                    if (id_structure_type == -1)
                    {
                        sync_views = true;
                        structure_types.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_structure_types[i])["id_structure_type"] = id_structure_type;
                    structure_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_structure_types[i]));
                }
                else
                {
                    if (RowToStructureType(row) == list[i])
                        continue;
                    if (structure_types.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        structure_types.EditingNewRecord = false;
                        return;
                    }
                    row["structure_type"] = list[i].StructureTypeName == null ? DBNull.Value : (object)list[i].StructureTypeName;
                }
            }
            list = StructureTypesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_structure_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_structure_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_structure_type"].Value == list[i].IdStructureType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (structure_types.Delete(list[i].IdStructureType.Value) == -1)
                    {
                        sync_views = true;
                        structure_types.EditingNewRecord = false;
                        return;
                    }
                    structure_types.Select().Rows.Find(list[i].IdStructureType).Delete();
                }
            }
            sync_views = true;
            structure_types.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new StructureTypeListViewport(this, MenuCallback);
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
                var result = MessageBox.Show("Сохранить изменения о структуре зданий в базу данных?", "Внимание",
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
            structure_types.Select().RowChanged -= StructureTypeListViewport_RowChanged;
            structure_types.Select().RowDeleting -= StructureTypeListViewport_RowDeleting;
            structure_types.Select().RowDeleted -= StructureTypeListViewport_RowDeleted;
            base.OnClosing(e);
        }

        private void StructureTypeListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void StructureTypeListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = v_snapshot_structure_types.Find("id_structure_type", e.Row["id_structure_type"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_structure_types[row_index]).Delete();
            }
        }

        void StructureTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = v_snapshot_structure_types.Find("id_structure_type", e.Row["id_structure_type"]);
            if (row_index == -1 && v_structure_types.Find("id_structure_type", e.Row["id_structure_type"]) != -1)
            {
                snapshot_structure_types.Rows.Add(e.Row["id_structure_type"], e.Row["structure_type"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)v_snapshot_structure_types[row_index]);
                    row["structure_type"] = e.Row["structure_type"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
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
                case "structure_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина названия структуры здания не может превышать 255 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название структуры здания не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        void v_snapshot_structure_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(StructureTypeListViewport));
            dataGridView = new DataGridView();
            id_structure_type = new DataGridViewTextBoxColumn();
            structure_type = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(id_structure_type, structure_type);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(320, 255);
            dataGridView.TabIndex = 3;
            // 
            // id_structure_type
            // 
            id_structure_type.HeaderText = "Идентификатор типа материала";
            id_structure_type.Name = "id_structure_type";
            id_structure_type.Visible = false;
            // 
            // structure_type
            // 
            structure_type.HeaderText = "Наименование";
            structure_type.Name = "structure_type";
            // 
            // StructureTypeListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(326, 261);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "StructureTypeListViewport";
            Padding = new Padding(3);
            Text = "Типы материалов";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
