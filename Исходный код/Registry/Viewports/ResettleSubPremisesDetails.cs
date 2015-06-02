using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Registry.DataModels;
using Registry.Entities;
using System.Drawing;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class ResettleSubPremisesDetails : System.Windows.Forms.UserControl
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
        private DataModel resettle_sub_premises = null;
        #endregion Models


        #region Views
        public BindingSource v_sub_premises { get; set; }
        private BindingSource v_resettle_sub_premises = null;
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
                resettle_sub_premises = ResettleSubPremisesFromAssocDataModel.GetInstance();
            else
                resettle_sub_premises = ResettleSubPremisesToAssocDataModel.GetInstance();
            resettle_sub_premises.Select();

            DataSet ds = DataSetManager.DataSet;

            v_resettle_sub_premises = new BindingSource();
            if (way == ResettleEstateObjectWay.From)
                v_resettle_sub_premises.DataMember = "resettle_sub_premises_from_assoc";
            else
                v_resettle_sub_premises.DataMember = "resettle_sub_premises_to_assoc";
            v_resettle_sub_premises.Filter = StaticFilter;
            v_resettle_sub_premises.DataSource = ds;

            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_resettle_sub_premises.Count; i++)
                snapshot_resettle_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_sub_premises[i])));
            v_snapshot_resettle_sub_premises = new BindingSource();
            v_snapshot_resettle_sub_premises.DataSource = snapshot_resettle_sub_premises;

            v_sub_premises.CurrentChanged += dataSource_CurrentChanged;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.CellValuePushed += dataGridView_CellValuePushed;
            dataGridView.RowCount = v_sub_premises.Count;
            sub_premises.RowDeleted += sub_premises_RowDeleted;
            sub_premises.RowChanged += sub_premises_RowChanged;
            resettle_sub_premises.Select().RowChanged += new DataRowChangeEventHandler(SubPremisesDetailsControl_RowChanged);
            resettle_sub_premises.Select().RowDeleting += new DataRowChangeEventHandler(SubPremisesDetailsControl_RowDeleting);
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public ResettleSubPremisesDetails()
        {
            this.ParentType = ParentTypeEnum.None;
            InitializeComponent();
            sync_views = true;
        }

        protected override void Dispose(bool disposing)
        {
            sub_premises.RowDeleted -= sub_premises_RowDeleted;
            sub_premises.RowChanged -= sub_premises_RowChanged;
            resettle_sub_premises.Select().RowChanged -= new DataRowChangeEventHandler(SubPremisesDetailsControl_RowChanged);
            resettle_sub_premises.Select().RowDeleting -= new DataRowChangeEventHandler(SubPremisesDetailsControl_RowDeleting);
            base.Dispose(disposing);
        }

        private static ResettleObject RowToResettleSubPremises(DataRow row)
        {
            ResettleObject ro = new ResettleObject();
            ro.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
            ro.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            ro.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
            return ro;
        }

        public List<ResettleObject> ResettleSubPremisesFromViewport()
        {
            List<ResettleObject> list = new List<ResettleObject>();
            for (int i = 0; i < snapshot_resettle_sub_premises.Rows.Count; i++)
            {
                DataRow row = snapshot_resettle_sub_premises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                ResettleObject ro = new ResettleObject();
                ro.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                ro.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
                ro.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
                list.Add(ro);
            }
            return list;
        }

        public List<ResettleObject> ResettleSubPremisesFromView()
        {
            List<ResettleObject> list = new List<ResettleObject>();
            for (int i = 0; i < v_resettle_sub_premises.Count; i++)
            {
                ResettleObject ro = new ResettleObject();
                DataRowView row = ((DataRowView)v_resettle_sub_premises[i]);
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
            return new object[] { 
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
            int height = 0;
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                height += dataGridView.Rows[i].Height;
            }
            height += dataGridView.ColumnHeadersHeight;
            dataGridView.Height = height+5;
        }

        public void CancelRecord()
        {
            snapshot_resettle_sub_premises.Clear();
            for (int i = 0; i < v_resettle_sub_premises.Count; i++)
                snapshot_resettle_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_sub_premises[i])));
            dataGridView.Refresh();
        }

        public void SaveRecord()
        {
            sync_views = false;
            ResettleSubPremisesFromAssocDataModel.GetInstance().EditingNewRecord = true;
            ResettleSubPremisesToAssocDataModel.GetInstance().EditingNewRecord = true;
            List<ResettleObject> list = ResettleSubPremisesFromViewport();
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (((ResettleObject)list[i]).IdAssoc != null)
                    row = resettle_sub_premises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    int id_assoc = -1;
                    if (way == ResettleEstateObjectWay.From)
                        id_assoc = ResettleSubPremisesFromAssocDataModel.Insert(list[i]);
                    else
                        id_assoc = ResettleSubPremisesToAssocDataModel.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        ResettleSubPremisesFromAssocDataModel.GetInstance().EditingNewRecord = false;
                        ResettleSubPremisesToAssocDataModel.GetInstance().EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_resettle_sub_premises[
                        v_snapshot_resettle_sub_premises.Find("id_sub_premises", list[i].IdObject)])["id_assoc"] = id_assoc;
                    resettle_sub_premises.Select().Rows.Add(new object[] { 
                        id_assoc, list[i].IdObject, list[i].IdProcess, 0
                    });
                }
            }
            list = ResettleSubPremisesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < v_snapshot_resettle_sub_premises.Count; j++)
                {
                    DataRowView row = (DataRowView)v_snapshot_resettle_sub_premises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !String.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    int affected = -1;
                    if (way == ResettleEstateObjectWay.From)
                        affected = ResettleSubPremisesFromAssocDataModel.Delete(list[i].IdAssoc.Value);
                    else
                        affected = ResettleSubPremisesToAssocDataModel.Delete(list[i].IdAssoc.Value);

                    if (affected == -1)
                    {
                        sync_views = true;
                        ResettleSubPremisesFromAssocDataModel.GetInstance().EditingNewRecord = false;
                        ResettleSubPremisesToAssocDataModel.GetInstance().EditingNewRecord = false;
                        return;
                    }
                    int snapshot_row_index = -1;
                    for (int j = 0; j < v_snapshot_resettle_sub_premises.Count; j++)
                        if (((DataRowView)v_snapshot_resettle_sub_premises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_resettle_sub_premises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        int sub_premises_row_index = v_sub_premises.Find("id_sub_premises", list[i].IdObject);
                        ((DataRowView)v_snapshot_resettle_sub_premises[snapshot_row_index]).Delete();
                        if (sub_premises_row_index != -1)
                            dataGridView.InvalidateRow(sub_premises_row_index);
                    }
                    resettle_sub_premises.Select().Rows.Find(((ResettleObject)list[i]).IdAssoc).Delete();
                }
            }
            sync_views = true;
            ResettleSubPremisesFromAssocDataModel.GetInstance().EditingNewRecord = false;
            ResettleSubPremisesToAssocDataModel.GetInstance().EditingNewRecord = false;
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
            int id_sub_premises = Convert.ToInt32(((DataRowView)v_sub_premises[e.RowIndex])["id_sub_premises"], CultureInfo.InvariantCulture);
            int row_index = v_snapshot_resettle_sub_premises.Find("id_sub_premises", id_sub_premises);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_resettle_sub_premises.Rows.Add(new object[] { null, id_sub_premises, e.Value });
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
            int id_sub_premises = Convert.ToInt32(((DataRowView)v_sub_premises[e.RowIndex])["id_sub_premises"], CultureInfo.InvariantCulture);
            int row_index = v_snapshot_resettle_sub_premises.Find("id_sub_premises", id_sub_premises);
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
                int row_index = v_snapshot_resettle_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
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
            int row_index = v_snapshot_resettle_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
            if (row_index == -1 && v_resettle_sub_premises.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_resettle_sub_premises.Rows.Add(new object[] { 
                        e.Row["id_assoc"],
                        e.Row["id_sub_premises"], 
                        true
                    });
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.is_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.id_sub_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.is_checked,
            this.id_sub_premises,
            this.id_premises,
            this.sub_premises_num,
            this.total_area,
            this.description});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowEditingIcon = false;
            this.dataGridView.Size = new System.Drawing.Size(769, 192);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView_CurrentCellDirtyStateChanged);
            // 
            // is_checked
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.NullValue = false;
            this.is_checked.DefaultCellStyle = dataGridViewCellStyle1;
            this.is_checked.HeaderText = "";
            this.is_checked.Name = "is_checked";
            this.is_checked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.is_checked.Width = 30;
            // 
            // id_sub_premises
            // 
            this.id_sub_premises.HeaderText = "Внутренний номер комнаты";
            this.id_sub_premises.Name = "id_sub_premises";
            this.id_sub_premises.ReadOnly = true;
            this.id_sub_premises.Visible = false;
            // 
            // id_premises
            // 
            this.id_premises.HeaderText = "Внутренний номер помещения";
            this.id_premises.Name = "id_premises";
            this.id_premises.ReadOnly = true;
            this.id_premises.Visible = false;
            // 
            // sub_premises_num
            // 
            this.sub_premises_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.sub_premises_num.HeaderText = "Номер комнаты";
            this.sub_premises_num.MinimumWidth = 150;
            this.sub_premises_num.Name = "sub_premises_num";
            this.sub_premises_num.ReadOnly = true;
            this.sub_premises_num.Width = 150;
            // 
            // total_area
            // 
            this.total_area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            this.total_area.DefaultCellStyle = dataGridViewCellStyle2;
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.MinimumWidth = 150;
            this.total_area.Name = "total_area";
            this.total_area.ReadOnly = true;
            this.total_area.Width = 150;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 300;
            this.description.Name = "description";
            this.description.ReadOnly = true;
            // 
            // ResettleSubPremisesDetails
            // 
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "ResettleSubPremisesDetails";
            this.Size = new System.Drawing.Size(772, 192);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
