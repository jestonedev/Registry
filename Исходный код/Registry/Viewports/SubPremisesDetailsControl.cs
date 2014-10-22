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

namespace Registry.Viewport
{
    internal sealed class SubPremisesDetailsControl : System.Windows.Forms.UserControl
    {
        #region Components
        private DataGridView dataGridView = new DataGridView();
        DataGridViewCheckBoxColumn field_checked = new DataGridViewCheckBoxColumn();
        DataGridViewTextBoxColumn field_beds = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_id_sub_premises = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_id_premises = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_sub_premises_num = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_total_area = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_description = new DataGridViewTextBoxColumn();

        #endregion Components

        //Models
        public DataTable sub_premises { get; set; }
        private TenancySubPremisesAssocDataModel tenancy_sub_premises = null;

        //Views
        public BindingSource v_sub_premises { get; set; }
        private BindingSource v_tenancy_sub_premises = null;

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
            if (v_sub_premises == null || sub_premises == null)
                throw new ApplicationException("Не заданна ссылка на таблицу и представление комнат");
            if (ParentRow == null)
                throw new ApplicationException("Не заданна ссылка на родительский объект");

            // Инициализируем snapshot-модель
            snapshot_tenancy_sub_premises = new DataTable("selected_sub_premises");
            snapshot_tenancy_sub_premises.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_tenancy_sub_premises.Columns.Add("id_sub_premises").DataType = typeof(int);
            snapshot_tenancy_sub_premises.Columns.Add("checked").DataType = typeof(bool);
            snapshot_tenancy_sub_premises.Columns.Add("beds").DataType = typeof(string);

            tenancy_sub_premises = TenancySubPremisesAssocDataModel.GetInstance();
            tenancy_sub_premises.Select();

            DataSet ds = DataSetManager.GetDataSet();

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
        }

        void SubPremisesDetailsControl_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_contract"]) != Convert.ToInt32(ParentRow["id_contract"]))
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
            if (Convert.ToInt32(e.Row["id_contract"]) != Convert.ToInt32(ParentRow["id_contract"]))
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_tenancy_sub_premises[row_index]);
                    row["beds"] = e.Row["beds"];
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
                            e.Row["beds"]
                        });
                }
            dataGridView.Refresh();
        }

        protected override void Dispose(bool disposing)
        {
            sub_premises.RowDeleted -= sub_premises_RowDeleted;
            sub_premises.RowChanged -= sub_premises_RowChanged;
            tenancy_sub_premises.Select().RowChanged -= new DataRowChangeEventHandler(SubPremisesDetailsControl_RowChanged);
            tenancy_sub_premises.Select().RowDeleting -= new DataRowChangeEventHandler(SubPremisesDetailsControl_RowDeleting);
            base.Dispose(disposing);
        }

        public SubPremisesDetailsControl()
        {
            this.ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            this.Controls.Add(dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();

            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.MultiSelect = false;
            dataGridView.Columns.AddRange(new DataGridViewColumn[]
                {
                    field_checked,
                    field_beds,
                    field_id_sub_premises,
                    field_id_premises,
                    field_sub_premises_num,
                    field_total_area,
                    field_description
                });
            dataGridView.VirtualMode = true;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.ShowEditingIcon = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLight;
            //
            // field_checked
            //
            field_checked.HeaderText = "";
            field_checked.Name = "checked";
            field_checked.Width = 30;
            field_checked.Resizable = DataGridViewTriState.False;
            field_checked.DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLightLight;
            field_checked.ReadOnly = false;
            field_checked.SortMode = DataGridViewColumnSortMode.NotSortable;
            //
            // field_beds
            //
            field_beds.HeaderText = "№ койко-мест";
            field_beds.Name = "beds";
            field_beds.MinimumWidth = 130;
            field_beds.DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLightLight;
            field_beds.ReadOnly = false;
            field_beds.SortMode = DataGridViewColumnSortMode.NotSortable;       
            // 
            // field_id_sub_premises
            // 
            field_id_sub_premises.HeaderText = "Внутренний номер комнаты";
            field_id_sub_premises.Name = "id_sub_premises";
            field_id_sub_premises.Visible = false;
            field_id_sub_premises.ReadOnly = true;
            // 
            // field_id_premises
            // 
            field_id_premises.HeaderText = "Внутренний номер помещения";
            field_id_premises.Name = "id_premises";
            field_id_premises.Visible = false;
            field_id_premises.ReadOnly = true;
            // 
            // field_sub_premises_num
            // 
            field_sub_premises_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            field_sub_premises_num.HeaderText = "Номер комнаты";
            field_sub_premises_num.MinimumWidth = 150;
            field_sub_premises_num.Name = "sub_premises_num";
            field_sub_premises_num.ReadOnly = true;
            // 
            // field_total_area
            // 
            field_total_area.HeaderText = "Общая площадь";
            field_total_area.MinimumWidth = 150;
            field_total_area.Name = "total_area";
            field_total_area.DefaultCellStyle.Format = "#0.0## м²";
            field_total_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            field_total_area.ReadOnly = true;
            // 
            // field_description
            // 
            field_description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            field_description.HeaderText = "Примечание";
            field_description.MinimumWidth = 300;
            field_description.Name = "description";
            field_description.ReadOnly = true;
            sync_views = true;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }

        public List<TenancyObject> TenancySubPremisesFromViewport()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < snapshot_tenancy_sub_premises.Rows.Count; i++)
            {
                DataRow row = snapshot_tenancy_sub_premises.Rows[i];
                if (Convert.ToBoolean(row["checked"]) == false)
                    continue;
                TenancyObject to = new TenancyObject();
                to.id_assoc = row["id_assoc"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_assoc"]);
                to.id_contract = (ParentType == ParentTypeEnum.Tenancy) ? (int?)Convert.ToInt32(ParentRow["id_contract"]) : null;
                to.id_object = row["id_sub_premises"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_sub_premises"]);
                to.beds = row["beds"] == DBNull.Value ? null : row["beds"].ToString();
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
                to.id_assoc = row["id_assoc"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_assoc"]);
                to.id_contract = row["id_contract"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_contract"]);
                to.id_object = row["id_sub_premises"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_sub_premises"]);
                to.beds = row["beds"] == DBNull.Value ? null : row["beds"].ToString();
                list.Add(to);
            }
            return list;
        }
        
        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_assoc"],
                dataRowView["id_sub_premises"], 
                true, 
                dataRowView["beds"]
            };
        }

        public void SetControlWidth(int width)
        {
            dataGridView.Width = width;
        }

        public void SetRowHeaderWidth(int width)
        {
            dataGridView.RowHeadersWidth = width;
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

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_sub_premises.Count <= e.RowIndex || v_sub_premises.Count == 0) return;
            int id_sub_premises = Convert.ToInt32(((DataRowView)v_sub_premises[e.RowIndex])["id_sub_premises"]);
            int row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", id_sub_premises);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "checked":
                    if (row_index == -1)
                        snapshot_tenancy_sub_premises.Rows.Add(new object[] { null, id_sub_premises, e.Value, null });
                    else
                        ((DataRowView)v_snapshot_tenancy_sub_premises[row_index])["checked"] = e.Value;
                    break;
                case "beds":
                    if (row_index == -1)
                        snapshot_tenancy_sub_premises.Rows.Add(new object[] { null, id_sub_premises, false, e.Value == null ? null : e.Value.ToString().ToLower() });
                    else
                        ((DataRowView)v_snapshot_tenancy_sub_premises[row_index])["beds"] = e.Value == null ? null : e.Value.ToString().ToLower();
                    break;
            }
            sync_views = true;
            if (menuCallback != null)
                menuCallback.EditingStateUpdate();
        }

        void dataSource_CurrentChanged(object sender, EventArgs e)
        {
            dataGridView.RowCount = v_sub_premises.Count;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_sub_premises.Count <= e.RowIndex || v_sub_premises.Count == 0) return;
            int id_sub_premises = Convert.ToInt32(((DataRowView)v_sub_premises[e.RowIndex])["id_sub_premises"]);
            int row_index = v_snapshot_tenancy_sub_premises.Find("id_sub_premises", id_sub_premises);
            DataRowView row = ((DataRowView)v_sub_premises[e.RowIndex]);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "checked":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_sub_premises[row_index])["checked"];
                    break;
                case "beds":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_sub_premises[row_index])["beds"];
                    if (e.Value != null && e.Value.ToString() != "" && !Regex.IsMatch(e.Value.ToString().ToLower(), "^[0-9а-яА-Я]+([,][0-9а-яА-Я]*)*$"))
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Некорректное значение для номеров койко-мест";
                    else
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
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

        public void CancelRecord()
        {
            snapshot_tenancy_sub_premises.Clear();
            for (int i = 0; i < v_tenancy_sub_premises.Count; i++)
                snapshot_tenancy_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_sub_premises[i])));
            dataGridView.Refresh();
        }

        public bool ValidateTenancySubPremises(List<TenancyObject> tenancySubPremises)
        {
            foreach (TenancyObject tenancySubPremise in tenancySubPremises)
                if ((tenancySubPremise.beds != null) && !Regex.IsMatch(tenancySubPremise.beds, "^[0-9а-яА-Я]+([,][0-9а-яА-Я]*)*$"))
                {
                    MessageBox.Show("Некорректное значение для номеров койко-мест", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            return true;
        }

        public void SaveRecord()
        {
            sync_views = false;
            List<TenancyObject> list = TenancySubPremisesFromViewport();
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (((TenancyObject)list[i]).id_assoc != null)
                    row = tenancy_sub_premises.Select().Rows.Find(list[i].id_assoc);
                if (row == null)
                {
                    int id_assoc = tenancy_sub_premises.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_sub_premises[
                        v_snapshot_tenancy_sub_premises.Find("id_sub_premises", list[i].id_object)])["id_assoc"] = id_assoc;
                    tenancy_sub_premises.Select().Rows.Add(new object[] { 
                        id_assoc, list[i].id_object, list[i].id_contract, list[i].beds, 0
                    });
                }
                else
                {
                    if (RowToTenancySubPremises(row) == list[i])
                        continue;
                    if (tenancy_sub_premises.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["beds"] = list[i].beds == null ? DBNull.Value : (object)list[i].beds;
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
                        (row["id_assoc"].ToString() != "") &&
                        ((int)row["id_assoc"] == list[i].id_assoc) &&
                        (Convert.ToBoolean(row["checked"]) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    if (tenancy_sub_premises.Delete(list[i].id_assoc.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    int snapshot_row_index = v_snapshot_tenancy_sub_premises.Find("id_assoc", list[i].id_assoc);
                    if (snapshot_row_index != -1)
                    {
                        int premises_row_index = v_sub_premises.Find("id_sub_premises", list[i].id_object);
                        ((DataRowView)v_snapshot_tenancy_sub_premises[snapshot_row_index]).Delete();
                        dataGridView.InvalidateRow(premises_row_index);
                    }
                    tenancy_sub_premises.Select().Rows.Find(((TenancyObject)list[i]).id_assoc).Delete();
                }
            }
            sync_views = true;
        }

        private TenancyObject RowToTenancySubPremises(DataRow row)
        {
            TenancyObject to = new TenancyObject();
            to.id_assoc = row["id_assoc"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_assoc"]);
            to.id_contract = row["id_contract"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_contract"]);
            to.id_object = row["id_sub_premises"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_sub_premises"]);
            to.beds = row["beds"] == DBNull.Value ? null : row["beds"].ToString();
            return to;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SubPremisesDetailsControl
            // 
            this.Name = "SubPremisesDetailsControl";
            this.Size = new System.Drawing.Size(77, 19);
            this.ResumeLayout(false);

        }
    }
}
