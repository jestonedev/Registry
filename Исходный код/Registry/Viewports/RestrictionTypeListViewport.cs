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
    internal sealed class RestrictionTypeListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_restriction_type;
        private DataGridViewTextBoxColumn restriction_type;
        #endregion Components

        #region Models
        RestrictionTypesDataModel restriction_types = null;
        DataTable snapshot_restriction_types = new DataTable("snapshot_restriction_types");
        #endregion Models

        #region Views
        BindingSource v_restriction_types = null;
        BindingSource v_snapshot_restriction_types = null;
        #endregion Views


        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private RestrictionTypeListViewport()
            : this(null)
        {
        }

        public RestrictionTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            snapshot_restriction_types.Locale = CultureInfo.CurrentCulture;
        }

        public RestrictionTypeListViewport(RestrictionTypeListViewport restrictionTypeListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
        }

        private bool SnapshotHasChanges()
        {
            List<RestrictionType> list_from_view = RestrictionTypesFromView();
            List<RestrictionType> list_from_viewport = RestrictionTypesFromViewport();
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
                dataRowView["id_restriction_type"], 
                dataRowView["restriction_type"]
            };
        }

        private static bool ValidateViewportData(List<RestrictionType> list)
        {
            foreach (RestrictionType restrictionType in list)
            {
                if (restrictionType.RestrictionTypeName == null)
                {
                    MessageBox.Show("Не заполнено наименование типа реквизита", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restrictionType.RestrictionTypeName != null && restrictionType.RestrictionTypeName.Length > 255)
                {
                    MessageBox.Show("Длина названия типа реквизита не может превышать 255 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static RestrictionType RowToRestrictionType(DataRow row)
        {
            RestrictionType restrictionType = new RestrictionType();
            restrictionType.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
            restrictionType.RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type");
            return restrictionType;
        }

        private List<RestrictionType> RestrictionTypesFromViewport()
        {
            List<RestrictionType> list = new List<RestrictionType>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    RestrictionType rt = new RestrictionType();
                    DataGridViewRow row = dataGridView.Rows[i];
                    rt.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                    rt.RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type");
                    list.Add(rt);
                }
            }
            return list;
        }

        private List<RestrictionType> RestrictionTypesFromView()
        {
            List<RestrictionType> list = new List<RestrictionType>();
            for (int i = 0; i < v_restriction_types.Count; i++)
            {
                RestrictionType rt = new RestrictionType();
                DataRowView row = ((DataRowView)v_restriction_types[i]);
                rt.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                rt.RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type");
                list.Add(rt);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_restriction_types.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            restriction_types = RestrictionTypesDataModel.GetInstance();
            restriction_types.Select();

            v_restriction_types = new BindingSource();
            v_restriction_types.DataMember = "restriction_types";
            v_restriction_types.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < restriction_types.Select().Columns.Count; i++)
                snapshot_restriction_types.Columns.Add(new DataColumn(restriction_types.Select().Columns[i].ColumnName, 
                    restriction_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_restriction_types.Count; i++)
                snapshot_restriction_types.Rows.Add(DataRowViewToArray(((DataRowView)v_restriction_types[i])));
            v_snapshot_restriction_types = new BindingSource();
            v_snapshot_restriction_types.DataSource = snapshot_restriction_types;
            v_snapshot_restriction_types.CurrentItemChanged += new EventHandler(v_snapshot_restriction_types_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_restriction_types;
            id_restriction_type.DataPropertyName = "id_restriction_type";
            restriction_type.DataPropertyName = "restriction_type";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            restriction_types.Select().RowChanged += new DataRowChangeEventHandler(RestrictionTypeListViewport_RowChanged);
            restriction_types.Select().RowDeleting += new DataRowChangeEventHandler(RestrictionTypeListViewport_RowDeleting);
        }

        public override void MoveFirst()
        {
            v_snapshot_restriction_types.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_restriction_types.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_restriction_types.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_restriction_types.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_restriction_types.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_restriction_types.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_restriction_types.Position > -1) && (v_snapshot_restriction_types.Position < (v_snapshot_restriction_types.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_restriction_types.Position > -1) && (v_snapshot_restriction_types.Position < (v_snapshot_restriction_types.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_restriction_types.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_restriction_types.Position != -1) && AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_restriction_types[v_snapshot_restriction_types.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_restriction_types.Clear();
            for (int i = 0; i < v_restriction_types.Count; i++)
                snapshot_restriction_types.Rows.Add(DataRowViewToArray(((DataRowView)v_restriction_types[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.RegistryDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<RestrictionType> list = RestrictionTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = restriction_types.Select().Rows.Find(((RestrictionType)list[i]).IdRestrictionType);
                if (row == null)
                {
                    int id_restriction_type = RestrictionTypesDataModel.Insert(list[i]);
                    if (id_restriction_type == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_restriction_types[i])["id_restriction_type"] = id_restriction_type;
                    restriction_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_restriction_types[i]));
                }
                else
                {
                    if (RowToRestrictionType(row) == list[i])
                        continue;
                    if (RestrictionTypesDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["restriction_type"] = list[i].RestrictionTypeName == null ? DBNull.Value : (object)list[i].RestrictionTypeName;
                }
            }
            list = RestrictionTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_restriction_type"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_restriction_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_restriction_type"].Value == list[i].IdRestrictionType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (RestrictionTypesDataModel.Delete(list[i].IdRestrictionType.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    restriction_types.Select().Rows.Find(((RestrictionType)list[i]).IdRestrictionType).Delete();
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
            RestrictionTypeListViewport viewport = new RestrictionTypeListViewport(this, MenuCallback);
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
                DialogResult result = MessageBox.Show("Сохранить изменения о типах реквизитов в базу данных?", "Внимание",
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
            restriction_types.Select().RowChanged -= new DataRowChangeEventHandler(RestrictionTypeListViewport_RowChanged);
            restriction_types.Select().RowDeleting -= new DataRowChangeEventHandler(RestrictionTypeListViewport_RowDeleting);
        }

        void RestrictionTypeListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_restriction_types.Find("id_restriction_type", e.Row["id_restriction_type"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_restriction_types[row_index]).Delete();
            }
        }

        void RestrictionTypeListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_restriction_types.Find("id_restriction_type", e.Row["id_restriction_type"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_restriction_types[row_index]);
                    row["restriction_type"] = e.Row["restriction_type"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_restriction_types.Rows.Add(new object[] { 
                        e.Row["id_restriction_type"], 
                        e.Row["restriction_type"]
                    });
                }
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "restriction_type":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина названия типа реквизита не может превышать 255 символов";
                    else
                        if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название типа реквизита не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void v_snapshot_restriction_types_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                MenuCallback.NavigationStateUpdate();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RestrictionTypeListViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_restriction_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.id_restriction_type,
            this.restriction_type});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(316, 255);
            this.dataGridView.TabIndex = 4;
            // 
            // id_restriction_type
            // 
            this.id_restriction_type.HeaderText = "Идентификатор реквизита";
            this.id_restriction_type.Name = "id_restriction_type";
            this.id_restriction_type.Visible = false;
            // 
            // restriction_type
            // 
            this.restriction_type.HeaderText = "Наименование";
            this.restriction_type.Name = "restriction_type";
            // 
            // RestrictionTypeListViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(322, 261);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RestrictionTypeListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Типы реквизитов";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
