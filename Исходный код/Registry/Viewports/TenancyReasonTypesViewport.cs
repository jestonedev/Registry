using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class TenancyReasonTypesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_reason_type;
        private DataGridViewTextBoxColumn reason_name;
        private DataGridViewTextBoxColumn reason_template;
        #endregion Components

        #region Models
        DataModel tenancy_reason_types;
        DataTable snapshot_reason_types = new DataTable("snapshot_reason_types");
        #endregion Models

        #region Views
        BindingSource v_tenancy_reason_types;
        BindingSource v_snapshot_tenancy_reason_types;
        #endregion Views

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private TenancyReasonTypesViewport()
            : this(null)
        {
        }

        public TenancyReasonTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshot_reason_types.Locale = CultureInfo.InvariantCulture;
        }

        public TenancyReasonTypesViewport(TenancyReasonTypesViewport reasonTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = reasonTypesViewport.DynamicFilter;
            StaticFilter = reasonTypesViewport.StaticFilter;
            ParentRow = reasonTypesViewport.ParentRow;
            ParentType = reasonTypesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = ReasonTypesFromView();
            var list_from_viewport = ReasonTypesFromViewport();
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
                dataRowView["id_reason_type"], 
                dataRowView["reason_name"],
                dataRowView["reason_template"]
            };
        }

        private static bool ValidateViewportData(List<ReasonType> list)
        {
            foreach (var reasonType in list)
            {
                if (reasonType.ReasonName == null)
                {
                    MessageBox.Show("Имя вида основания не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (reasonType.ReasonName != null && reasonType.ReasonName.Length > 150)
                {
                    MessageBox.Show("Длина имени типа основания не может превышать 150 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (reasonType.ReasonTemplate == null)
                {
                    MessageBox.Show("Шаблон основания не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (reasonType.ReasonTemplate != null && reasonType.ReasonTemplate.Length > 4000)
                {
                    MessageBox.Show("Длина шаблона вида основания не может превышать 4000 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (!(Regex.IsMatch(reasonType.ReasonTemplate, "@reason_number@") &&
                     (Regex.IsMatch(reasonType.ReasonTemplate, "@reason_date@"))))
                {
                    MessageBox.Show("Шаблон основания имеет неверный формат. В шаблоне должны быть указаны номер (в виде шаблона @reason_number@) и" +
                        " дата (в виде шаблона @reason_date@) основания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static ReasonType RowToReasonType(DataRow row)
        {
            var reasonType = new ReasonType();
            reasonType.IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type");
            reasonType.ReasonName = ViewportHelper.ValueOrNull(row, "reason_name");
            reasonType.ReasonTemplate = ViewportHelper.ValueOrNull(row, "reason_template"); 
            return reasonType;
        }

        private List<ReasonType> ReasonTypesFromViewport()
        {
            var list = new List<ReasonType>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var rt = new ReasonType();
                    var row = dataGridView.Rows[i];
                    rt.IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type");
                    rt.ReasonName = ViewportHelper.ValueOrNull(row, "reason_name");
                    rt.ReasonTemplate = ViewportHelper.ValueOrNull(row, "reason_template"); 
                    list.Add(rt);
                }
            }
            return list;
        }

        private List<ReasonType> ReasonTypesFromView()
        {
            var list = new List<ReasonType>();
            for (var i = 0; i < v_tenancy_reason_types.Count; i++)
            {
                var rt = new ReasonType();
                var row = ((DataRowView)v_tenancy_reason_types[i]);
                rt.IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type");
                rt.ReasonName = ViewportHelper.ValueOrNull(row, "reason_name");
                rt.ReasonTemplate = ViewportHelper.ValueOrNull(row, "reason_template"); 
                list.Add(rt);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_tenancy_reason_types.Count;
        }

        public override void MoveFirst()
        {
            v_snapshot_tenancy_reason_types.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_tenancy_reason_types.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_tenancy_reason_types.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_tenancy_reason_types.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_tenancy_reason_types.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_tenancy_reason_types.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_tenancy_reason_types.Position > -1) && (v_snapshot_tenancy_reason_types.Position < (v_snapshot_tenancy_reason_types.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_tenancy_reason_types.Position > -1) && (v_snapshot_tenancy_reason_types.Position < (v_snapshot_tenancy_reason_types.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            tenancy_reason_types = DataModel.GetInstance(DataModelType.TenancyReasonTypesDataModel);

            //Ожидаем дозагрузки данных, если это необходимо
            tenancy_reason_types.Select();

            v_tenancy_reason_types = new BindingSource();
            v_tenancy_reason_types.DataMember = "tenancy_reason_types";
            v_tenancy_reason_types.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            snapshot_reason_types.Locale = CultureInfo.InvariantCulture;
            for (var i = 0; i < tenancy_reason_types.Select().Columns.Count; i++)
                snapshot_reason_types.Columns.Add(new DataColumn(
                    tenancy_reason_types.Select().Columns[i].ColumnName, tenancy_reason_types.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_tenancy_reason_types.Count; i++)
                snapshot_reason_types.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_reason_types[i])));
            v_snapshot_tenancy_reason_types = new BindingSource();
            v_snapshot_tenancy_reason_types.DataSource = snapshot_reason_types;
            v_snapshot_tenancy_reason_types.CurrentItemChanged += v_snapshot_reason_types_CurrentItemChanged;

            dataGridView.DataSource = v_snapshot_tenancy_reason_types;
            id_reason_type.DataPropertyName = "id_reason_type";
            reason_name.DataPropertyName = "reason_name";
            reason_template.DataPropertyName = "reason_template";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            tenancy_reason_types.Select().RowChanged += ReasonTypesViewport_RowChanged;
            tenancy_reason_types.Select().RowDeleting += ReasonTypesViewport_RowDeleting;
            tenancy_reason_types.Select().RowDeleted += ReasonTypesViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)v_snapshot_tenancy_reason_types.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_tenancy_reason_types.Position != -1) && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_tenancy_reason_types[v_snapshot_tenancy_reason_types.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_reason_types.Clear();
            for (var i = 0; i < v_tenancy_reason_types.Count; i++)
                snapshot_reason_types.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_reason_types[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            tenancy_reason_types.EditingNewRecord = true;
            var list = ReasonTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                tenancy_reason_types.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = tenancy_reason_types.Select().Rows.Find(list[i].IdReasonType);
                if (row == null)
                {
                    var id_reason_type = tenancy_reason_types.Insert(list[i]);
                    if (id_reason_type == -1)
                    {
                        sync_views = true;
                        tenancy_reason_types.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_reason_types[i])["id_reason_type"] = id_reason_type;
                    tenancy_reason_types.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_tenancy_reason_types[i]));
                }
                else
                {

                    if (RowToReasonType(row) == list[i])
                        continue;
                    if (tenancy_reason_types.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        tenancy_reason_types.EditingNewRecord = false;
                        return;
                    }
                    row["reason_name"] = list[i].ReasonName == null ? DBNull.Value : (object)list[i].ReasonName;
                    row["reason_template"] = list[i].ReasonTemplate == null ? DBNull.Value : (object)list[i].ReasonTemplate;
                }
            }
            list = ReasonTypesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_reason_type"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_reason_type"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_reason_type"].Value == list[i].IdReasonType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (tenancy_reason_types.Delete(list[i].IdReasonType.Value) == -1)
                    {
                        sync_views = true;
                        tenancy_reason_types.EditingNewRecord = false;
                        return;
                    }
                    tenancy_reason_types.Select().Rows.Find(list[i].IdReasonType).Delete();
                }
            }
            sync_views = true;
            tenancy_reason_types.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new TenancyReasonTypesViewport(this, MenuCallback);
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
                var result = MessageBox.Show("Сохранить изменения о виде основания в базу данных?", "Внимание",
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
            tenancy_reason_types.Select().RowChanged -= ReasonTypesViewport_RowChanged;
            tenancy_reason_types.Select().RowDeleting -= ReasonTypesViewport_RowDeleting;
            tenancy_reason_types.Select().RowDeleted -= ReasonTypesViewport_RowDeleted;
            base.OnClosing(e);
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "reason_name":
                    if (cell.Value.ToString().Trim().Length > 150)
                        cell.ErrorText = "Длина названия вида основания не может превышать 150 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Название вида основания не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
                case "reason_template":
                    if (cell.Value.ToString().Length > 4000)
                        cell.ErrorText = "Длина шаблона вида основания не может превышать 4000 символов";
                    else
                    if (!(Regex.IsMatch(cell.Value.ToString(), "@reason_number@") &&
                         (Regex.IsMatch(cell.Value.ToString(), "@reason_date@"))))
                        cell.ErrorText = "Шаблон основания имеет неверный формат. В шаблоне должны быть указаны номер (в виде шаблона @reason_number@) и" +
                            " дата (в виде шаблона @reason_date@) основания";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void ReasonTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void ReasonTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = v_snapshot_tenancy_reason_types.Find("id_reason_type", e.Row["id_reason_type"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_tenancy_reason_types[row_index]).Delete();
            }
        }

        void ReasonTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = v_snapshot_tenancy_reason_types.Find("id_reason_type", e.Row["id_reason_type"]);
            if (row_index == -1 && v_tenancy_reason_types.Find("id_reason_type", e.Row["id_reason_type"]) != -1)
            {
                snapshot_reason_types.Rows.Add(e.Row["id_reason_type"], e.Row["reason_name"], e.Row["reason_template"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)v_snapshot_tenancy_reason_types[row_index]);
                    row["reason_name"] = e.Row["reason_name"];
                    row["reason_template"] = e.Row["reason_template"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshot_reason_types_CurrentItemChanged(object sender, EventArgs e)
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
            var resources = new ComponentResourceManager(typeof(TenancyReasonTypesViewport));
            dataGridView = new DataGridView();
            id_reason_type = new DataGridViewTextBoxColumn();
            reason_name = new DataGridViewTextBoxColumn();
            reason_template = new DataGridViewTextBoxColumn();
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
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
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
            dataGridView.Columns.AddRange(id_reason_type, reason_name, reason_template);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(706, 255);
            dataGridView.TabIndex = 7;
            // 
            // id_reason_type
            // 
            id_reason_type.Frozen = true;
            id_reason_type.HeaderText = "Идентификатор вида основания";
            id_reason_type.Name = "id_reason_type";
            id_reason_type.ReadOnly = true;
            id_reason_type.Visible = false;
            // 
            // reason_name
            // 
            reason_name.FillWeight = 200F;
            reason_name.HeaderText = "Имя вида основания";
            reason_name.MinimumWidth = 100;
            reason_name.Name = "reason_name";
            // 
            // reason_template
            // 
            reason_template.FillWeight = 500F;
            reason_template.HeaderText = "Шаблон вида основания";
            reason_template.MinimumWidth = 100;
            reason_template.Name = "reason_template";
            // 
            // TenancyReasonTypesViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(712, 261);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyReasonTypesViewport";
            Padding = new Padding(3);
            Text = "Виды оснований";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
