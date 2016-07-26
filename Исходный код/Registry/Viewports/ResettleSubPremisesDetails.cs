using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport
{
    internal sealed class ResettleSubPremisesDetails : UserControl
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewCheckBoxColumn is_checked;
        private DataGridViewTextBoxColumn id_sub_premises;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn sub_premises_num;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn description;
        #endregion Components

        #region Models
        public DataTable sub_premises { get; set; }
        private DataModel resettle_sub_premises;
        #endregion Models


        #region Views
        public BindingSource v_sub_premises { get; set; }
        private BindingSource v_resettle_sub_premises;
        #endregion Views
        //Начальные настройки компонента. Задавать необходимо их все.
        public string StaticFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }
        public IMenuCallback menuCallback { get; set; }

        //Состояние snapshot'а модели данных
        public BindingSource v_snapshot_resettle_sub_premises { get; set; }
        public DataTable snapshot_resettle_sub_premises { get; set; }

        //Флаг разрешения синхронизации snapshot и original моделей
        public bool sync_views { get; set; }

        private ResettleEstateObjectWay way = ResettleEstateObjectWay.From;

        public ResettleEstateObjectWay Way { get { return way; } set { way = value; } }

        public void InitializeControl()
        {
            dataGridView.AutoGenerateColumns = false;
            if (v_sub_premises == null || sub_premises == null)
                throw new ViewportException("Не заданна ссылка на таблицу и представление комнат");
            if (ParentRow == null)
                throw new ViewportException("Не заданна ссылка на родительский объект");
            if (ParentType != ParentTypeEnum.ResettleProcess)
                throw new ViewportException("Неизвестный родительский объект");

            // Инициализируем snapshot-модель
            snapshot_resettle_sub_premises = new DataTable("selected_sub_premises");
            snapshot_resettle_sub_premises.Locale = CultureInfo.InvariantCulture;
            snapshot_resettle_sub_premises.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_resettle_sub_premises.Columns.Add("id_sub_premises").DataType = typeof(int);
            snapshot_resettle_sub_premises.Columns.Add("is_checked").DataType = typeof(bool);

            if (way == ResettleEstateObjectWay.From)
                resettle_sub_premises = DataModel.GetInstance<ResettleSubPremisesFromAssocDataModel>();
            else
                resettle_sub_premises = DataModel.GetInstance<ResettleSubPremisesToAssocDataModel>();
            resettle_sub_premises.Select();

            var ds = DataModel.DataSet;

            v_resettle_sub_premises = new BindingSource();
            if (way == ResettleEstateObjectWay.From)
                v_resettle_sub_premises.DataMember = "resettle_sub_premises_from_assoc";
            else
                v_resettle_sub_premises.DataMember = "resettle_sub_premises_to_assoc";
            v_resettle_sub_premises.Filter = StaticFilter;
            v_resettle_sub_premises.DataSource = ds;

            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_resettle_sub_premises.Count; i++)
                snapshot_resettle_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_sub_premises[i])));
            v_snapshot_resettle_sub_premises = new BindingSource();
            v_snapshot_resettle_sub_premises.DataSource = snapshot_resettle_sub_premises;

            v_sub_premises.CurrentChanged += dataSource_CurrentChanged;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.CellValuePushed += dataGridView_CellValuePushed;
            dataGridView.RowCount = v_sub_premises.Count;
            sub_premises.RowDeleted += sub_premises_RowDeleted;
            sub_premises.RowChanged += sub_premises_RowChanged;
            resettle_sub_premises.Select().RowChanged += SubPremisesDetailsControl_RowChanged;
            resettle_sub_premises.Select().RowDeleting += SubPremisesDetailsControl_RowDeleting;
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public ResettleSubPremisesDetails()
        {
            ParentType = ParentTypeEnum.None;
            InitializeComponent();
            sync_views = true;
        }

        protected override void Dispose(bool disposing)
        {
            sub_premises.RowDeleted -= sub_premises_RowDeleted;
            sub_premises.RowChanged -= sub_premises_RowChanged;
            resettle_sub_premises.Select().RowChanged -= SubPremisesDetailsControl_RowChanged;
            resettle_sub_premises.Select().RowDeleting -= SubPremisesDetailsControl_RowDeleting;
            base.Dispose(disposing);
        }

        private static ResettleObject RowToResettleSubPremises(DataRow row)
        {
            var ro = new ResettleObject();
            ro.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
            ro.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            ro.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
            return ro;
        }

        public List<ResettleObject> ResettleSubPremisesFromViewport()
        {
            var list = new List<ResettleObject>();
            for (var i = 0; i < snapshot_resettle_sub_premises.Rows.Count; i++)
            {
                var row = snapshot_resettle_sub_premises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var ro = new ResettleObject();
                ro.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                ro.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
                ro.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
                list.Add(ro);
            }
            return list;
        }

        public List<ResettleObject> ResettleSubPremisesFromView()
        {
            var list = new List<ResettleObject>();
            for (var i = 0; i < v_resettle_sub_premises.Count; i++)
            {
                var ro = new ResettleObject();
                var row = ((DataRowView)v_resettle_sub_premises[i]);
                ro.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                ro.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                ro.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
                list.Add(ro);
            }
            return list;
        }

        public static bool ValidateResettleSubPremises(List<ResettleObject> resettleSubPremises)
        {
            return true;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_assoc"],
                dataRowView["id_sub_premises"], 
                true
            };
        }

        public void SetControlWidth(int width)
        {
            dataGridView.Width = width;
        }

        public void CalcControlHeight()
        {
            var height = 0;
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                height += dataGridView.Rows[i].Height;
            }
            height += dataGridView.ColumnHeadersHeight;
            dataGridView.Height = height+5;
        }

        public void CancelRecord()
        {
            snapshot_resettle_sub_premises.Clear();
            for (var i = 0; i < v_resettle_sub_premises.Count; i++)
                snapshot_resettle_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_sub_premises[i])));
            dataGridView.Refresh();
        }

        public void SaveRecord()
        {
            sync_views = false;
            var resettleSubPremisesFromAssoc = DataModel.GetInstance<ResettleSubPremisesFromAssocDataModel>();
            var resettleSubPremisesToAssoc = DataModel.GetInstance<ResettleSubPremisesToAssocDataModel>();
            resettleSubPremisesFromAssoc.EditingNewRecord = true;
            resettleSubPremisesToAssoc.EditingNewRecord = true;
            var list = ResettleSubPremisesFromViewport();
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = resettle_sub_premises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    var id_assoc = -1;
                    if (way == ResettleEstateObjectWay.From)
                        id_assoc = resettleSubPremisesFromAssoc.Insert(list[i]);
                    else
                        id_assoc = resettleSubPremisesToAssoc.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        resettleSubPremisesFromAssoc.EditingNewRecord = false;
                        resettleSubPremisesToAssoc.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_resettle_sub_premises[
                        v_snapshot_resettle_sub_premises.Find("id_sub_premises", list[i].IdObject)])["id_assoc"] = id_assoc;
                    resettle_sub_premises.Select().Rows.Add(id_assoc, list[i].IdObject, list[i].IdProcess, 0);
                }
            }
            list = ResettleSubPremisesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < v_snapshot_resettle_sub_premises.Count; j++)
                {
                    var row = (DataRowView)v_snapshot_resettle_sub_premises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    var affected = -1;
                    if (way == ResettleEstateObjectWay.From)
                        affected = resettleSubPremisesFromAssoc.Delete(list[i].IdAssoc.Value);
                    else
                        affected = resettleSubPremisesToAssoc.Delete(list[i].IdAssoc.Value);

                    if (affected == -1)
                    {
                        sync_views = true;
                        resettleSubPremisesFromAssoc.EditingNewRecord = false;
                        resettleSubPremisesToAssoc.EditingNewRecord = false;
                        return;
                    }
                    var snapshot_row_index = -1;
                    for (var j = 0; j < v_snapshot_resettle_sub_premises.Count; j++)
                        if (((DataRowView)v_snapshot_resettle_sub_premises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_resettle_sub_premises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        var sub_premises_row_index = v_sub_premises.Find("id_sub_premises", list[i].IdObject);
                        ((DataRowView)v_snapshot_resettle_sub_premises[snapshot_row_index]).Delete();
                        if (sub_premises_row_index != -1)
                            dataGridView.InvalidateRow(sub_premises_row_index);
                    }
                    resettle_sub_premises.Select().Rows.Find(list[i].IdAssoc).Delete();
                }
            }
            sync_views = true;
            resettleSubPremisesFromAssoc.EditingNewRecord = false;
            resettleSubPremisesToAssoc.EditingNewRecord = false;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            dataGridView.ClearSelection();
            if (dataGridView.Rows.Count > 0)
                dataGridView.Rows[0].Selected = true;
        }

        void sub_premises_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Add)
            {
                dataGridView.RowCount = v_sub_premises.Count;
                CalcControlHeight();
            }
            dataGridView.Invalidate();
        }

        void sub_premises_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            dataGridView.RowCount = v_sub_premises.Count;
            dataGridView.Invalidate();
            CalcControlHeight();
        }

        void dataSource_CurrentChanged(object sender, EventArgs e)
        {
            dataGridView.RowCount = v_sub_premises.Count;
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_sub_premises.Count <= e.RowIndex || v_sub_premises.Count == 0) return;
            var id_sub_premises = Convert.ToInt32(((DataRowView)v_sub_premises[e.RowIndex])["id_sub_premises"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_resettle_sub_premises.Find("id_sub_premises", id_sub_premises);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_resettle_sub_premises.Rows.Add(null, id_sub_premises, e.Value);
                    else
                        ((DataRowView)v_snapshot_resettle_sub_premises[row_index])["is_checked"] = e.Value;
                    break;
            }
            sync_views = true;
            if (menuCallback != null)
                menuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_sub_premises.Count <= e.RowIndex || v_sub_premises.Count == 0) return;
            var id_sub_premises = Convert.ToInt32(((DataRowView)v_sub_premises[e.RowIndex])["id_sub_premises"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_resettle_sub_premises.Find("id_sub_premises", id_sub_premises);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_resettle_sub_premises[row_index])["is_checked"];
                    break;
                case "id_sub_premises": e.Value = ((DataRowView)v_sub_premises[e.RowIndex])["id_sub_premises"];
                    break;
                case "id_premises": e.Value = ((DataRowView)v_sub_premises[e.RowIndex])["id_premises"];
                    break;
                case "sub_premises_num": e.Value = ((DataRowView)v_sub_premises[e.RowIndex])["sub_premises_num"];
                    break;
                case "total_area": e.Value = ((DataRowView)v_sub_premises[e.RowIndex])["total_area"];
                    break;
                case "description": e.Value = ((DataRowView)v_sub_premises[e.RowIndex])["description"];
                    break;
                case "id_state": e.Value = ((DataRowView)v_sub_premises[e.RowIndex])["id_state"];
                    break;
            }
        }

        void SubPremisesDetailsControl_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = v_snapshot_resettle_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_resettle_sub_premises[row_index]).Delete();
            }
            dataGridView.Refresh();
        }

        void SubPremisesDetailsControl_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            var row_index = v_snapshot_resettle_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
            if (row_index == -1 && v_resettle_sub_premises.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_resettle_sub_premises.Rows.Add(e.Row["id_assoc"], e.Row["id_sub_premises"], true);
            }
            dataGridView.Invalidate();
        }

        void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle3 = new DataGridViewCellStyle();
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            dataGridView = new DataGridView();
            is_checked = new DataGridViewCheckBoxColumn();
            id_sub_premises = new DataGridViewTextBoxColumn();
            id_premises = new DataGridViewTextBoxColumn();
            sub_premises_num = new DataGridViewTextBoxColumn();
            total_area = new DataGridViewTextBoxColumn();
            description = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.Columns.AddRange(is_checked, id_sub_premises, id_premises, sub_premises_num, total_area, description);
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.LightGray;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            dataGridView.Location = new Point(0, 0);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowEditingIcon = false;
            dataGridView.Size = new Size(769, 192);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.CurrentCellDirtyStateChanged += dataGridView_CurrentCellDirtyStateChanged;
            // 
            // is_checked
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = Color.White;
            dataGridViewCellStyle1.NullValue = false;
            is_checked.DefaultCellStyle = dataGridViewCellStyle1;
            is_checked.HeaderText = "";
            is_checked.Name = "is_checked";
            is_checked.Resizable = DataGridViewTriState.False;
            is_checked.Width = 30;
            // 
            // id_sub_premises
            // 
            id_sub_premises.HeaderText = "Внутренний номер комнаты";
            id_sub_premises.Name = "id_sub_premises";
            id_sub_premises.ReadOnly = true;
            id_sub_premises.Visible = false;
            // 
            // id_premises
            // 
            id_premises.HeaderText = "Внутренний номер помещения";
            id_premises.Name = "id_premises";
            id_premises.ReadOnly = true;
            id_premises.Visible = false;
            // 
            // sub_premises_num
            // 
            sub_premises_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            sub_premises_num.HeaderText = "Номер комнаты";
            sub_premises_num.MinimumWidth = 150;
            sub_premises_num.Name = "sub_premises_num";
            sub_premises_num.ReadOnly = true;
            sub_premises_num.Width = 150;
            // 
            // total_area
            // 
            total_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle2;
            total_area.HeaderText = "Общая площадь";
            total_area.MinimumWidth = 150;
            total_area.Name = "total_area";
            total_area.ReadOnly = true;
            total_area.Width = 150;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            description.HeaderText = "Примечание";
            description.MinimumWidth = 300;
            description.Name = "description";
            description.ReadOnly = true;
            // 
            // ResettleSubPremisesDetails
            // 
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Name = "ResettleSubPremisesDetails";
            Size = new Size(772, 192);
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
