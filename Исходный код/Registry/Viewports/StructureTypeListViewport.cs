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
    internal sealed class StructureTypeListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_structure_type;
        private DataGridViewTextBoxColumn structure_type;
        #endregion Components

        #region Models
        StructureTypesDataModel structure_types = null;
        DataTable snapshot_structure_types = new DataTable("snapshot_structure_types");
        #endregion Models

        #region Views
        BindingSource v_structure_types = null;
        BindingSource v_snapshot_structure_types = null;
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
            snapshot_structure_types.Locale = CultureInfo.CurrentCulture;
        }

        public StructureTypeListViewport(StructureTypeListViewport structureTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
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

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_structure_type"], 
                dataRowView["structure_type"]
            };
        }

        private static bool ValidateViewportData(List<StructureType> list)
        {
            foreach (StructureType structureType in list)
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
            StructureType structureType = new StructureType();
            structureType.IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
            structureType.StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type");
            return structureType;
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
                    st.IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type");
                    st.StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type");
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            structure_types = StructureTypesDataModel.GetInstance();
            //Ожиданем дозагрузки данных, если это необходимо
            structure_types.Select();

            v_structure_types = new BindingSource();
            v_structure_types.DataMember = "structure_types";
            v_structure_types.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < structure_types.Select().Columns.Count; i++)
                snapshot_structure_types.Columns.Add(new DataColumn(
                    structure_types.Select().Columns[i].ColumnName, structure_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_structure_types.Count; i++)
                snapshot_structure_types.Rows.Add(DataRowViewToArray(((DataRowView)v_structure_types[i])));
            v_snapshot_structure_types = new BindingSource();
            v_snapshot_structure_types.DataSource = snapshot_structure_types;
            v_snapshot_structure_types.CurrentItemChanged += new EventHandler(v_snapshot_structure_types_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_structure_types;
            id_structure_type.DataPropertyName = "id_structure_type";
            structure_type.DataPropertyName = "structure_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            structure_types.Select().RowChanged += new DataRowChangeEventHandler(StructureTypeListViewport_RowChanged);
            structure_types.Select().RowDeleting += new DataRowChangeEventHandler(StructureTypeListViewport_RowDeleting);
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_structure_types.AddNew();
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
            for (int i = 0; i < v_structure_types.Count; i++)
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
            List<StructureType> list = StructureTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = structure_types.Select().Rows.Find(((StructureType)list[i]).IdStructureType);
                if (row == null)
                {
                    int id_structure_type = StructureTypesDataModel.Insert(list[i]);
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
                    if (RowToStructureType(row) == list[i])
                        continue;
                    if (StructureTypesDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["structure_type"] = list[i].StructureTypeName == null ? DBNull.Value : (object)list[i].StructureTypeName;
                }
            }
            list = StructureTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_structure_type"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_structure_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_structure_type"].Value == list[i].IdStructureType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (StructureTypesDataModel.Delete(list[i].IdStructureType.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    structure_types.Select().Rows.Find(((StructureType)list[i]).IdStructureType).Delete();
                }
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            StructureTypeListViewport viewport = new StructureTypeListViewport(this, MenuCallback);
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
                DialogResult result = MessageBox.Show("Сохранить изменения о структуре зданий в базу данных?", "Внимание",
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
            structure_types.Select().RowChanged -= new DataRowChangeEventHandler(StructureTypeListViewport_RowChanged);
            structure_types.Select().RowDeleting -= new DataRowChangeEventHandler(StructureTypeListViewport_RowDeleting);
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

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "structure_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина названия структуры здания не может превышать 255 символов";
                    else
                        if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название структуры здания не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        void v_snapshot_structure_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                MenuCallback.NavigationStateUpdate();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StructureTypeListViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_structure_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.structure_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
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
            this.id_structure_type,
            this.structure_type});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(320, 255);
            this.dataGridView.TabIndex = 3;
            // 
            // id_structure_type
            // 
            this.id_structure_type.HeaderText = "Идентификатор типа материала";
            this.id_structure_type.Name = "id_structure_type";
            this.id_structure_type.Visible = false;
            // 
            // structure_type
            // 
            this.structure_type.HeaderText = "Наименование";
            this.structure_type.Name = "structure_type";
            // 
            // StructureTypeListViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(326, 261);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StructureTypeListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Типы материалов";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
