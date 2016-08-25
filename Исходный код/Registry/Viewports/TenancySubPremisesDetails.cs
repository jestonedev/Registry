using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;

namespace Registry.Viewport
{
    internal sealed class TenancySubPremisesDetails : UserControl
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        public DataTable sub_premises { get; set; }
        private DataModel tenancy_sub_premises;
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
        private BindingSource v_tenancy_sub_premises;
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
            snapshot_tenancy_sub_premises = new DataTable("selected_sub_premises")
            {
                Locale = CultureInfo.InvariantCulture
            };
            snapshot_tenancy_sub_premises.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_tenancy_sub_premises.Columns.Add("id_sub_premises").DataType = typeof(int);
            snapshot_tenancy_sub_premises.Columns.Add("is_checked").DataType = typeof(bool);
            snapshot_tenancy_sub_premises.Columns.Add("rent_total_area").DataType = typeof(double);

            tenancy_sub_premises = EntityDataModel<TenancySubPremisesAssoc>.GetInstance();
            tenancy_sub_premises.Select();

            var ds = DataStorage.DataSet;

            v_tenancy_sub_premises = new BindingSource
            {
                DataMember = "tenancy_sub_premises_assoc",
                Filter = StaticFilter,
                DataSource = ds
            };

            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_tenancy_sub_premises.Count; i++)
                snapshot_tenancy_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_sub_premises[i])));
            v_snapshot_tenancy_sub_premises = new BindingSource {DataSource = snapshot_tenancy_sub_premises};

            v_sub_premises.CurrentChanged += dataSource_CurrentChanged;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.CellValuePushed += dataGridView_CellValuePushed;
            dataGridView.RowCount = v_sub_premises.Count;
            sub_premises.RowDeleted += sub_premises_RowDeleted;
            sub_premises.RowChanged += sub_premises_RowChanged;
            tenancy_sub_premises.Select().RowChanged += SubPremisesDetailsControl_RowChanged;
            tenancy_sub_premises.Select().RowDeleting += SubPremisesDetailsControl_RowDeleting;
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public TenancySubPremisesDetails()
        {
            ParentType = ParentTypeEnum.None;
            InitializeComponent();
            sync_views = true;
        }

        protected override void Dispose(bool disposing)
        {
            sub_premises.RowDeleted -= sub_premises_RowDeleted;
            sub_premises.RowChanged -= sub_premises_RowChanged;
            tenancy_sub_premises.Select().RowChanged -= SubPremisesDetailsControl_RowChanged;
            tenancy_sub_premises.Select().RowDeleting -= SubPremisesDetailsControl_RowDeleting;
            base.Dispose(disposing);
        }

        private static TenancySubPremisesAssoc RowToTenancySubPremises(DataRow row)
        {
            var to = new TenancySubPremisesAssoc
            {
                IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises"),
                RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area")
            };
            return to;
        }

        public List<TenancySubPremisesAssoc> TenancySubPremisesFromViewport()
        {
            var list = new List<TenancySubPremisesAssoc>();
            for (var i = 0; i < snapshot_tenancy_sub_premises.Rows.Count; i++)
            {
                var row = snapshot_tenancy_sub_premises.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var to = new TenancySubPremisesAssoc
                {
                    IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                    IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                    IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises"),
                    RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area")
                };
                list.Add(to);
            }
            return list;
        }

        public List<TenancySubPremisesAssoc> TenancySubPremisesFromView()
        {
            var list = new List<TenancySubPremisesAssoc>();
            for (var i = 0; i < v_tenancy_sub_premises.Count; i++)
            {
                var row = (DataRowView)v_tenancy_sub_premises[i];
                var to = new TenancySubPremisesAssoc
                {
                    IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                    IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                    IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises"),
                    RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area")
                };
                list.Add(to);
            }
            return list;
        }

        public bool ValidateTenancySubPremises(List<TenancySubPremisesAssoc> tenancySubPremises)
        {
            foreach (var subPremises in tenancySubPremises)
            {
                if (subPremises.IdSubPremises != null && OtherService.SubPremiseFundAndRentMatch(subPremises.IdSubPremises.Value,
                    (int) ParentRow["id_rent_type"])) continue;
                var idPremises = (int)EntityDataModel<SubPremise>.GetInstance().Select().Rows.Find(subPremises.IdSubPremises.Value)["id_premises"];
                if (OtherService.PremiseFundAndRentMatch(idPremises, (int)ParentRow["id_rent_type"])) continue;
                var idBuilding = (int)EntityDataModel<Premise>.GetInstance().Select().Rows.Find(idPremises)["id_building"];
                return OtherService.BuildingFundAndRentMatch(idBuilding, (int)ParentRow["id_rent_type"]) || 
                    MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемой комнаты. Все равно продолжить сохранение?",
                        @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
            }
            return true;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
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
            snapshot_tenancy_sub_premises.Clear();
            for (var i = 0; i < v_tenancy_sub_premises.Count; i++)
                snapshot_tenancy_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_sub_premises[i])));
            dataGridView.Refresh();
        }

        public void SaveRecord()
        {
            sync_views = false;
            tenancy_sub_premises.EditingNewRecord = true;
            var list = TenancySubPremisesFromViewport();
            for (var i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (list[i].IdAssoc != null)
                    row = tenancy_sub_premises.Select().Rows.Find(list[i].IdAssoc);
                if (row == null)
                {
                    var id_assoc = tenancy_sub_premises.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        tenancy_sub_premises.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_sub_premises[
                        v_snapshot_tenancy_sub_premises.Find("id_sub_premises", list[i].IdSubPremises)])["id_assoc"] = id_assoc;
                    tenancy_sub_premises.Select().Rows.Add(id_assoc, list[i].IdSubPremises, list[i].IdProcess, list[i].RentTotalArea, 0);
                }
                else
                {
                    if (RowToTenancySubPremises(row) == list[i])
                        continue;
                    if (tenancy_sub_premises.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        tenancy_sub_premises.EditingNewRecord = false;
                        return;
                    }
                    row["rent_total_area"] = list[i].RentTotalArea == null ? DBNull.Value : (object)list[i].RentTotalArea;
                }
            }
            list = TenancySubPremisesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < v_snapshot_tenancy_sub_premises.Count; j++)
                {
                    var row = (DataRowView)v_snapshot_tenancy_sub_premises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        !string.IsNullOrEmpty(row["id_assoc"].ToString()) &&
                        ((int)row["id_assoc"] == list[i].IdAssoc) &&
                        (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == true))
                        rowIndex = j;
                }
                if (rowIndex == -1)
                {
                    if (tenancy_sub_premises.Delete(list[i].IdAssoc.Value) == -1)
                    {
                        sync_views = true;
                        tenancy_sub_premises.EditingNewRecord = false;
                        return;
                    }
                    var snapshotRowIndex = -1;
                    for (var j = 0; j < v_snapshot_tenancy_sub_premises.Count; j++)
                        if (((DataRowView)v_snapshot_tenancy_sub_premises[j])["id_assoc"] != DBNull.Value &&
                            Convert.ToInt32(((DataRowView)v_snapshot_tenancy_sub_premises[j])["id_assoc"], CultureInfo.InvariantCulture) == list[i].IdAssoc)
                            snapshotRowIndex = j;
                    if (snapshotRowIndex != -1)
                    {
                        var subPremisesRowIndex = v_sub_premises.Find("id_sub_premises", list[i].IdSubPremises);
                        ((DataRowView)v_snapshot_tenancy_sub_premises[snapshotRowIndex]).Delete();
                        if (subPremisesRowIndex != -1)
                            dataGridView.InvalidateRow(subPremisesRowIndex);
                    }
                    tenancy_sub_premises.Select().Rows.Find(list[i].IdAssoc).Delete();
                }
            }
            sync_views = true;
            tenancy_sub_premises.EditingNewRecord = false;
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
            var row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", id_sub_premises);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (row_index == -1)
                        snapshot_tenancy_sub_premises.Rows.Add(null, id_sub_premises, e.Value, null);
                    else
                        ((DataRowView)v_snapshot_tenancy_sub_premises[row_index])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                    double value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (row_index == -1)
                        snapshot_tenancy_sub_premises.Rows.Add(null, id_sub_premises, false, value == 0 ? DBNull.Value : (object)value);
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
            var id_sub_premises = Convert.ToInt32(((DataRowView)v_sub_premises[e.RowIndex])["id_sub_premises"], CultureInfo.InvariantCulture);
            var row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", id_sub_premises);
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
                var row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
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
            var row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
            if (row_index == -1 && v_tenancy_sub_premises.Find("id_assoc", e.Row["id_assoc"]) != -1)
            {
                snapshot_tenancy_sub_premises.Rows.Add(e.Row["id_assoc"], e.Row["id_sub_premises"], true, e.Row["rent_total_area"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)v_snapshot_tenancy_sub_premises[row_index]);
                    row["rent_total_area"] = e.Row["rent_total_area"];
                }
            dataGridView.Invalidate();
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name == "rent_total_area")
            {
                dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
                dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
                if (string.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
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
                        e.Handled = ((TextBox)dataGridView.EditingControl).Text.IndexOf(',') != -1;
                    }
                    else
                        e.Handled = true;
            }
        }

        void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView.EndEdit();
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle4 = new DataGridViewCellStyle();
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var dataGridViewCellStyle3 = new DataGridViewCellStyle();
            dataGridView = new DataGridView();
            is_checked = new DataGridViewCheckBoxColumn();
            rent_total_area = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(is_checked, rent_total_area, id_sub_premises, id_premises, sub_premises_num, total_area, description);
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.LightGray;
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dataGridView.DefaultCellStyle = dataGridViewCellStyle4;
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
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
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
            // rent_total_area
            // 
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            rent_total_area.DefaultCellStyle = dataGridViewCellStyle2;
            rent_total_area.HeaderText = "Площадь койко-места";
            rent_total_area.MinimumWidth = 160;
            rent_total_area.Name = "rent_total_area";
            rent_total_area.SortMode = DataGridViewColumnSortMode.NotSortable;
            rent_total_area.Width = 160;
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
            dataGridViewCellStyle3.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle3;
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
            // TenancySubPremisesDetails
            // 
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Name = "TenancySubPremisesDetails";
            Size = new Size(772, 192);
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }

        internal string GetDefaultDynamicFilter()
        {
            var filter = "";
            if (v_snapshot_tenancy_sub_premises.Count <= 0) return filter;
            filter += "id_premises IN (0";
            foreach (DataRowView row in v_snapshot_tenancy_sub_premises)
            {
                var subPremisesRow = sub_premises.Rows.Find(row["id_sub_premises"]);
                if (subPremisesRow != null)
                    filter += "," + subPremisesRow["id_premises"];
            }
            filter += ")";
            return filter;
        }
    }
}
