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
    internal sealed class SubPremisesDetailsControl : System.Windows.Forms.UserControl
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        public DataTable sub_premises { get; set; }
        private TenancySubPremisesAssocDataModel tenancy_sub_premises = null;
        #endregion Models
        private DataGridViewCheckBoxColumn is_checked;
        private DataGridViewTextBoxColumn rent_total_area;
        private DataGridViewTextBoxColumn id_sub_premises;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn sub_premises_num;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn description;


        #region Views
        public BindingSource v_sub_premises { get; set; }
        private BindingSource v_tenancy_sub_premises = null;
        #endregion Views

        //Начальные настройки компонента. Задавать необходимо их все.
        public string StaticFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }
        public IMenuCallback menuCallback { get; set; }

        //Состояние snapshot'а модели данных
        public BindingSource v_snapshot_tenancy_sub_premises { get; set; }
        public DataTable snapshot_tenancy_sub_premises { get; set; }

        //Флаг разрешения синхронизации snapshot и original моделей
        public bool sync_views { get; set; }

        public void InitializeControl()
        {
            dataGridView.AutoGenerateColumns = false;
            if (v_sub_premises == null || sub_premises == null)
                throw new ViewportException("Не заданна ссылка на таблицу и представление комнат");
            if (ParentRow == null)
                throw new ViewportException("Не заданна ссылка на родительский объект");
            if (ParentType != ParentTypeEnum.Tenancy)
                throw new ViewportException("Неизвестный родительский объект");

            // Инициализируем snapshot-модель
            snapshot_tenancy_sub_premises = new DataTable("selected_sub_premises");
            snapshot_tenancy_sub_premises.Locale = CultureInfo.InvariantCulture;
            snapshot_tenancy_sub_premises.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_tenancy_sub_premises.Columns.Add("id_sub_premises").DataType = typeof(int);
            snapshot_tenancy_sub_premises.Columns.Add("is_checked").DataType = typeof(bool);
            snapshot_tenancy_sub_premises.Columns.Add("rent_total_area").DataType = typeof(double);

            tenancy_sub_premises = TenancySubPremisesAssocDataModel.GetInstance();
            tenancy_sub_premises.Select();

            DataSet ds = DataSetManager.DataSet;

            v_tenancy_sub_premises = new BindingSource();
            v_tenancy_sub_premises.DataMember = "tenancy_sub_premises_assoc";
            v_tenancy_sub_premises.Filter = StaticFilter;
            v_tenancy_sub_premises.DataSource = ds;

            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_tenancy_sub_premises.Count; i++)
                snapshot_tenancy_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_sub_premises[i])));
            v_snapshot_tenancy_sub_premises = new BindingSource();
            v_snapshot_tenancy_sub_premises.DataSource = snapshot_tenancy_sub_premises;

            v_sub_premises.CurrentChanged += dataSource_CurrentChanged;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.CellValuePushed += dataGridView_CellValuePushed;
            dataGridView.RowCount = v_sub_premises.Count;
            sub_premises.RowDeleted += sub_premises_RowDeleted;
            sub_premises.RowChanged += sub_premises_RowChanged;
            tenancy_sub_premises.Select().RowChanged += new DataRowChangeEventHandler(SubPremisesDetailsControl_RowChanged);
            tenancy_sub_premises.Select().RowDeleting += new DataRowChangeEventHandler(SubPremisesDetailsControl_RowDeleting);
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public SubPremisesDetailsControl()
        {
            this.ParentType = ParentTypeEnum.None;
            InitializeComponent();
            sync_views = true;
        }

        protected override void Dispose(bool disposing)
        {
            sub_premises.RowDeleted -= sub_premises_RowDeleted;
            sub_premises.RowChanged -= sub_premises_RowChanged;
            tenancy_sub_premises.Select().RowChanged -= new DataRowChangeEventHandler(SubPremisesDetailsControl_RowChanged);
            tenancy_sub_premises.Select().RowDeleting -= new DataRowChangeEventHandler(SubPremisesDetailsControl_RowDeleting);
            base.Dispose(disposing);
        }

        private static TenancyObject RowToTenancySubPremises(DataRow row)
        {
            TenancyObject to = new TenancyObject();
            to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
            to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
            to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
            return to;
        }

        public List<TenancyObject> TenancySubPremisesFromViewport()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < snapshot_tenancy_sub_premises.Rows.Count; i++)
            {
                DataRow row = snapshot_tenancy_sub_premises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                TenancyObject to = new TenancyObject();
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
                to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
                list.Add(to);
            }
            return list;
        }

        public List<TenancyObject> TenancySubPremisesFromView()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < v_tenancy_sub_premises.Count; i++)
            {
                TenancyObject to = new TenancyObject();
                DataRowView row = ((DataRowView)v_tenancy_sub_premises[i]);
                to.IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc");
                to.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                to.IdObject = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
                to.RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area");
                list.Add(to);
            }
            return list;
        }

        public static bool ValidateTenancySubPremises(List<TenancyObject> tenancySubPremises)
        {
            return true;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_assoc"],
                dataRowView["id_sub_premises"], 
                true, 
                dataRowView["rent_total_area"]
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
            snapshot_tenancy_sub_premises.Clear();
            for (int i = 0; i < v_tenancy_sub_premises.Count; i++)
                snapshot_tenancy_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_sub_premises[i])));
            dataGridView.Refresh();
        }

        public void SaveRecord()
        {
            sync_views = false;
            List<TenancyObject> list = TenancySubPremisesFromViewport();
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (((TenancyObject)list[i]).IdAssoc != null)
                    row = tenancy_sub_premises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    int id_assoc = TenancySubPremisesAssocDataModel.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_sub_premises[
                        v_snapshot_tenancy_sub_premises.Find("id_sub_premises", list[i].IdObject)])["id_assoc"] = id_assoc;
                    tenancy_sub_premises.Select().Rows.Add(new object[] { 
                        id_assoc, list[i].IdObject, list[i].IdProcess, list[i].RentTotalArea, 0
                    });
                }
                else
                {
                    if (RowToTenancySubPremises(row) == list[i])
                        continue;
                    if (TenancySubPremisesAssocDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["rent_total_area"] = list[i].RentTotalArea == null ? DBNull.Value : (object)list[i].RentTotalArea;
                }
            }
            list = TenancySubPremisesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < v_snapshot_tenancy_sub_premises.Count; j++)
                {
                    DataRowView row = (DataRowView)v_snapshot_tenancy_sub_premises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !String.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    if (TenancySubPremisesAssocDataModel.Delete(list[i].IdAssoc.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    int snapshot_row_index = -1;
                    for (int j = 0; j < v_snapshot_tenancy_sub_premises.Count; j++)
                        if (((DataRowView)v_snapshot_tenancy_sub_premises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_tenancy_sub_premises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshot_row_index = j;
                    if (snapshot_row_index != -1)
                    {
                        int sub_premises_row_index = v_sub_premises.Find("id_sub_premises", list[i].IdObject);
                        ((DataRowView)v_snapshot_tenancy_sub_premises[snapshot_row_index]).Delete();
                        if (sub_premises_row_index != -1)
                            dataGridView.InvalidateRow(sub_premises_row_index);
                    }
                    tenancy_sub_premises.Select().Rows.Find(((TenancyObject)list[i]).IdAssoc).Delete();
                }
            }
            sync_views = true;
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
            int row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", id_sub_premises);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_tenancy_sub_premises.Rows.Add(new object[] { null, id_sub_premises, e.Value, null });
                    else
                        ((DataRowView)v_snapshot_tenancy_sub_premises[row_index])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                    double value = 0;
                    if (e.Value != null)
                        Double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_sub_premises.Rows.Add(new object[] { null, id_sub_premises, false, value == 0 ? DBNull.Value : (object)value });
                    else
                        ((DataRowView)v_snapshot_tenancy_sub_premises[row_index])["rent_total_area"] = value == 0 ? DBNull.Value : (object)value;
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
            int row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", id_sub_premises);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_sub_premises[row_index])["is_checked"];
                    break;
                case "rent_total_area":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_sub_premises[row_index])["rent_total_area"];
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
                int row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_tenancy_sub_premises[row_index]).Delete();
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
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_tenancy_sub_premises[row_index]);
                    row["rent_total_area"] = e.Row["rent_total_area"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    int row_index = v_tenancy_sub_premises.Find("id_assoc", e.Row["id_assoc"]);
                    if (row_index != -1)
                        snapshot_tenancy_sub_premises.Rows.Add(new object[] { 
                            e.Row["id_assoc"],
                            e.Row["id_sub_premises"], 
                            true,   
                            e.Row["rent_total_area"]
                        });
                }
            dataGridView.Refresh();
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name == "rent_total_area")
            {
                dataGridView.EditingControl.KeyPress -= new KeyPressEventHandler(EditingControl_KeyPress);
                dataGridView.EditingControl.KeyPress += new KeyPressEventHandler(EditingControl_KeyPress);
                if (String.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text = "0";
                else
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text.Substring(0, ((TextBox)e.Control).Text.Length - 3);
            }
        }

        void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name == "rent_total_area")
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == (char)8))
                    e.Handled = false;
                else
                    if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    {
                        e.KeyChar = ',';
                        if (((TextBox)dataGridView.EditingControl).Text.IndexOf(',') != -1)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                    else
                        e.Handled = true;
            }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.is_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.rent_total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.rent_total_area,
            this.id_sub_premises,
            this.id_premises,
            this.sub_premises_num,
            this.total_area,
            this.description});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowEditingIcon = false;
            this.dataGridView.Size = new System.Drawing.Size(769, 192);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView_EditingControlShowing);
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
            // rent_total_area
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            this.rent_total_area.DefaultCellStyle = dataGridViewCellStyle2;
            this.rent_total_area.HeaderText = "Арендуемая S общ.";
            this.rent_total_area.MinimumWidth = 130;
            this.rent_total_area.Name = "rent_total_area";
            this.rent_total_area.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rent_total_area.Width = 130;
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
            dataGridViewCellStyle3.Format = "#0.0## м²";
            this.total_area.DefaultCellStyle = dataGridViewCellStyle3;
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
            // SubPremisesDetailsControl
            // 
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "SubPremisesDetailsControl";
            this.Size = new System.Drawing.Size(772, 192);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
