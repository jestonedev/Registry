using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.SearchForms;
using System.Data;
using Registry.Entities;
using Microsoft.TeamFoundation.Client;
using System.Text.RegularExpressions;
using Registry.CalcDataModels;

namespace Registry.Viewport
{
    internal sealed class TenancyPremisesViewport: Viewport
    {
        #region Components
        private DataGridViewWithDetails dataGridView = new DataGridViewWithDetails();
        private DataGridViewImageColumn field_image = new DataGridViewImageColumn();
        private DataGridViewCheckBoxColumn field_checked = new DataGridViewCheckBoxColumn();
        private DataGridViewTextBoxColumn field_beds = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_street = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_house = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_premises_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        //Models
        private PremisesDataModel premises = null;
        private BuildingsDataModel buildings = null;
        private KladrStreetsDataModel kladr = null;
        private PremisesTypesDataModel premises_types = null;
        private SubPremisesDataModel sub_premises = null;
        private TenancyPremisesAssocDataModel tenancy_premises = null;
        private DataTable snapshot_tenancy_premises = null;

        //Views
        private BindingSource v_premises = null;
        private BindingSource v_buildings = null;
        private BindingSource v_premises_types = null;
        private BindingSource v_sub_premises = null;
        private BindingSource v_tenancy_premises = null;
        private BindingSource v_snapshot_tenancy_premises = null;

        //Forms
        private SearchForm spExtendedSearchForm = null;
        private SearchForm spSimpleSearchForm = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        //Идентификатор развернутого помещения
        private int expanded_id = -1;

        public TenancyPremisesViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPagePremises";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень помещений";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public TenancyPremisesViewport(TenancyPremisesViewport tenancyPremisesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = tenancyPremisesViewport.DynamicFilter;
            this.StaticFilter = tenancyPremisesViewport.StaticFilter;
            this.ParentRow = tenancyPremisesViewport.ParentRow;
            this.ParentType = tenancyPremisesViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            premises = PremisesDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();
            buildings  = BuildingsDataModel.GetInstance();
            premises_types = PremisesTypesDataModel.GetInstance();
            sub_premises = SubPremisesDataModel.GetInstance();
            tenancy_premises = TenancyPremisesAssocDataModel.GetInstance();

            // Ожидаем дозагрузки данных, если это необходимо
            premises.Select();
            kladr.Select();
            buildings.Select();
            premises_types.Select();
            sub_premises.Select();
            tenancy_premises.Select();

            // Инициализируем snapshot-модель
            snapshot_tenancy_premises = new DataTable("selected_premises");
            snapshot_tenancy_premises.Columns.Add("id_assoc").DataType = typeof(int);
            snapshot_tenancy_premises.Columns.Add("id_premises").DataType = typeof(int);
            snapshot_tenancy_premises.Columns.Add("checked").DataType = typeof(bool);
            snapshot_tenancy_premises.Columns.Add("beds").DataType = typeof(string);

            DataSet ds = DataSetManager.GetDataSet();

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += new EventHandler(v_premises_CurrentItemChanged);
            v_premises.DataMember = "premises";
            v_premises.DataSource = ds;
            v_premises.Filter += DynamicFilter;

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = "Помещения найма №" + ParentRow["id_contract"].ToString();
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_tenancy_premises = new BindingSource();
            v_tenancy_premises.DataMember = "tenancy_premises_assoc";
            v_tenancy_premises.Filter = StaticFilter;
            v_tenancy_premises.DataSource = ds;

            v_buildings = new BindingSource();
            v_buildings.DataMember = "buildings";
            v_buildings.DataSource = ds;

            v_premises_types = new BindingSource();
            v_premises_types.DataMember = "premises_types";
            v_premises_types.DataSource = ds;

            v_sub_premises = new BindingSource();
            v_sub_premises.DataSource = v_premises;
            v_sub_premises.DataMember = "premises_sub_premises";

            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_tenancy_premises.Count; i++)
                snapshot_tenancy_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_premises[i])));
            v_snapshot_tenancy_premises = new BindingSource();
            v_snapshot_tenancy_premises.DataSource = snapshot_tenancy_premises;

            field_id_premises_type.DataSource = v_premises_types;
            field_id_premises_type.ValueMember = "id_premises_type";
            field_id_premises_type.DisplayMember = "premises_type";

            // Настраивем компонент отображения комнат
            SubPremisesDetailsControl details = new SubPremisesDetailsControl();
            details.v_sub_premises = v_sub_premises;
            details.sub_premises = sub_premises.Select();
            details.StaticFilter = StaticFilter;
            details.ParentRow = ParentRow;
            details.ParentType = ParentType;
            details.menuCallback = menuCallback;
            details.InitializeControl();
            dataGridView.DetailsControl = details;

            v_premises.PositionChanged += new EventHandler(v_premises_PositionChanged);
            premises.Select().RowChanged += new DataRowChangeEventHandler(PremisesListViewport_RowChanged);
            premises.Select().RowDeleted += new DataRowChangeEventHandler(PremisesListViewport_RowDeleted);
            tenancy_premises.Select().RowChanged += new DataRowChangeEventHandler(TenancyPremisesViewport_RowChanged);
            tenancy_premises.Select().RowDeleting += new DataRowChangeEventHandler(TenancyPremisesViewport_RowDeleting);
            dataGridView.CellValueNeeded += new DataGridViewCellValueEventHandler(dataGridView_CellValueNeeded);
            dataGridView.CellValuePushed += new DataGridViewCellValueEventHandler(dataGridView_CellValuePushed);
            dataGridView.SelectionChanged += new EventHandler(dataGridView_SelectionChanged);
            dataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dataGridView_ColumnHeaderMouseClick);
            dataGridView.CellContentClick += new DataGridViewCellEventHandler(dataGridView_CellContentClick);
            dataGridView.Resize += new EventHandler(dataGridView_Resize);
            dataGridView.BeforeExpandDetails += new EventHandler<DataGridViewDetailsEventArgs>(dataGridView_BeforeExpandDetails);
            dataGridView.BeforeCollapseDetails += new EventHandler<DataGridViewDetailsEventArgs>(dataGridView_BeforeCollapseDetails);
            dataGridView.RowCount = v_premises.Count;
        }

        void dataGridView_BeforeCollapseDetails(object sender, DataGridViewDetailsEventArgs e)
        {
            dataGridView.Rows[e.RowIndex].Cells["checked"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        void dataGridView_BeforeExpandDetails(object sender, DataGridViewDetailsEventArgs e)
        {
            dataGridView.Rows[e.RowIndex].Cells["checked"].Style.Alignment = DataGridViewContentAlignment.TopCenter;
            ((SubPremisesDetailsControl)dataGridView.DetailsControl).CalcControlHeight();
            int width = 0;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((SubPremisesDetailsControl)dataGridView.DetailsControl).SetControlWidth(width);
        }

        void dataGridView_Resize(object sender, EventArgs e)
        {
            int width = 0;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
                width += dataGridView.Columns[i].Width;
            width += dataGridView.RowHeadersWidth;
            ((SubPremisesDetailsControl)dataGridView.DetailsControl).SetControlWidth(width);
        }

        void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].Name == "image")
            {
                if (expanded_id != Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"]))
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Registry.Viewport.Properties.Resource.minus;
                    expanded_id = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"]);
                    dataGridView.ExpandDetails(e.RowIndex);
                }
                else
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Registry.Viewport.Properties.Resource.plus;
                    expanded_id = -1;
                    dataGridView.CollapseDetails();
                }
            }
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else return;
            }
            premises.Select().RowChanged -= new DataRowChangeEventHandler(PremisesListViewport_RowChanged);
            premises.Select().RowDeleted -= new DataRowChangeEventHandler(PremisesListViewport_RowDeleted);
            tenancy_premises.Select().RowChanged -= new DataRowChangeEventHandler(TenancyPremisesViewport_RowChanged);
            tenancy_premises.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyPremisesViewport_RowDeleting);
            base.Close();
        }

        public override void ForceClose()
        {
            premises.Select().RowChanged -= new DataRowChangeEventHandler(PremisesListViewport_RowChanged);
            premises.Select().RowDeleted -= new DataRowChangeEventHandler(PremisesListViewport_RowDeleted);
            tenancy_premises.Select().RowChanged -= new DataRowChangeEventHandler(TenancyPremisesViewport_RowChanged);
            tenancy_premises.Select().RowDeleting -= new DataRowChangeEventHandler(TenancyPremisesViewport_RowDeleting);
            base.ForceClose();
        }

        private bool SnapshotHasChanges()
        {
            //Проверяем помещения
            List<TenancyObject> list_from_view = TenancyPremisesFromView();
            List<TenancyObject> list_from_viewport = TenancyPremisesFromViewport();
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
            //Проверяем комнаты
            list_from_view = ((SubPremisesDetailsControl)dataGridView.DetailsControl).TenancySubPremisesFromView();
            list_from_viewport = ((SubPremisesDetailsControl)dataGridView.DetailsControl).TenancySubPremisesFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            founded = false;
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

        private List<TenancyObject> TenancyPremisesFromViewport()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < snapshot_tenancy_premises.Rows.Count; i++)
            {
                DataRow row = snapshot_tenancy_premises.Rows[i];
                if (Convert.ToBoolean(row["checked"]) == false)
                    continue;
                TenancyObject to = new TenancyObject();
                to.id_assoc = row["id_assoc"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_assoc"]);
                to.id_contract = (ParentType == ParentTypeEnum.Tenancy) ? (int?)Convert.ToInt32(ParentRow["id_contract"]) : null;
                to.id_object = row["id_premises"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_premises"]);
                to.beds = row["beds"] == DBNull.Value ? null : row["beds"].ToString();
                list.Add(to);
            }
            return list;
        }

        private List<TenancyObject> TenancyPremisesFromView()
        {
            List<TenancyObject> list = new List<TenancyObject>();
            for (int i = 0; i < v_tenancy_premises.Count; i++)
            {
                TenancyObject to = new TenancyObject();
                DataRowView row = ((DataRowView)v_tenancy_premises[i]);
                to.id_assoc = row["id_assoc"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_assoc"]);
                to.id_contract = row["id_contract"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_contract"]);
                to.id_object = row["id_premises"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_premises"]);
                to.beds = row["beds"] == DBNull.Value ? null : row["beds"].ToString();
                list.Add(to);
            }
            return list;
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_tenancy_premises.Clear();
            for (int i = 0; i < v_tenancy_premises.Count; i++)
                snapshot_tenancy_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_tenancy_premises[i])));
            dataGridView.Refresh();
            ((SubPremisesDetailsControl)dataGridView.DetailsControl).CancelRecord();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        private bool ValidateTenancyPremises(List<TenancyObject> tenancyPremises)
        {
            foreach (TenancyObject tenancyPremise in tenancyPremises)
                if ((tenancyPremise.beds != null) && !Regex.IsMatch(tenancyPremise.beds, "^[0-9а-яА-Я]+([,][0-9а-яА-Я]*)*$"))
                {
                    MessageBox.Show("Некорректное значение для номеров койко-мест", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<TenancyObject> list = TenancyPremisesFromViewport();
            // Проверяем данные о помещениях
            if (!ValidateTenancyPremises(list))
            {
                sync_views = true;
                return;
            }
            // Проверяем данные о комнатах
            if (!((SubPremisesDetailsControl)dataGridView.DetailsControl).ValidateTenancySubPremises(
                ((SubPremisesDetailsControl)dataGridView.DetailsControl).TenancySubPremisesFromViewport()))
            {
                sync_views = true;
                return;
            }
            // Сохраняем помещения в базу данных
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = null;
                if (((TenancyObject)list[i]).id_assoc != null)
                    row = tenancy_premises.Select().Rows.Find(list[i].id_assoc);
                if (row == null)
                {
                    int id_assoc = tenancy_premises.Insert(list[i]);
                    if (id_assoc == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_tenancy_premises[
                        v_snapshot_tenancy_premises.Find("id_premises", list[i].id_object)])["id_assoc"] = id_assoc;
                    tenancy_premises.Select().Rows.Add(new object[] { 
                        id_assoc, list[i].id_object, list[i].id_contract, list[i].beds, 0
                    });
                }
                else
                {
                    if (RowToTenancyPremises(row) == list[i])
                        continue;
                    if (tenancy_premises.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["beds"] = list[i].beds == null ? DBNull.Value : (object)list[i].beds;
                }
            }
            list = TenancyPremisesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < v_snapshot_tenancy_premises.Count; j++)
                {
                    DataRowView row = (DataRowView)v_snapshot_tenancy_premises[j];
                    if ((row["id_assoc"] != DBNull.Value) &&
                        (row["id_assoc"].ToString() != "") &&
                        ((int)row["id_assoc"] == list[i].id_assoc) &&
                        (Convert.ToBoolean(row["checked"]) == true))
                        row_index = j;
                }
                if (row_index == -1)
                {
                    if (tenancy_premises.Delete(list[i].id_assoc.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    int snapshot_row_index = v_snapshot_tenancy_premises.Find("id_assoc", list[i].id_assoc);
                    if (snapshot_row_index != -1)
                    {
                        int premises_row_index = v_premises.Find("id_premises", list[i].id_object);
                        ((DataRowView)v_snapshot_tenancy_premises[snapshot_row_index]).Delete();
                        dataGridView.InvalidateRow(premises_row_index);
                    }
                    tenancy_premises.Select().Rows.Find(((TenancyObject)list[i]).id_assoc).Delete();
                }
            }
            sync_views = true;
            // Сохраняем комнаты в базу данных
            ((SubPremisesDetailsControl)dataGridView.DetailsControl).SaveRecord();
            // Обновляем зависимую агрегационную модель
            if (ParentType == ParentTypeEnum.Tenancy)
                CalcDataModeTenancyAggregated.GetInstance().Refresh(CalcDataModelFilterEnity.Tenancy, (int)ParentRow["id_contract"]);
        }

        private TenancyObject RowToTenancyPremises(DataRow row)
        {
            TenancyObject to = new TenancyObject();
            to.id_assoc = row["id_assoc"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_assoc"]);
            to.id_contract = row["id_contract"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_contract"]);
            to.id_object = row["id_premises"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_premises"]);
            to.beds = row["beds"] == DBNull.Value ? null : row["beds"].ToString();
            return to;
        }

        void TenancyPremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_contract"]) != Convert.ToInt32(ParentRow["id_contract"]))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_tenancy_premises.Find("id_premises", e.Row["id_premises"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_tenancy_premises[row_index]).Delete();
            }
            dataGridView.Refresh();
        }

        void TenancyPremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (Convert.ToInt32(e.Row["id_contract"]) != Convert.ToInt32(ParentRow["id_contract"]))
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_tenancy_premises.Find("id_premises", e.Row["id_premises"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_tenancy_premises[row_index]);
                    row["beds"] = e.Row["beds"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    int row_index = v_tenancy_premises.Find("id_assoc", e.Row["id_assoc"]);
                    if (row_index != -1)
                        snapshot_tenancy_premises.Rows.Add(new object[] { 
                            e.Row["id_assoc"],
                            e.Row["id_premises"], 
                            true,   
                            e.Row["beds"]
                        });
                }
            dataGridView.Refresh();
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_assoc"],
                dataRowView["id_premises"], 
                true, 
                dataRowView["beds"]
            };
        }

        void PremisesListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            dataGridView.RowCount = v_premises.Count;
            dataGridView.Refresh();
        }

        void PremisesListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_premises.Count;
        }

        void v_premises_PositionChanged(object sender, EventArgs e)
        {
            if (v_premises.Position == -1 || dataGridView.Rows.Count == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            dataGridView.Rows[v_premises.Position].Selected = true;
            dataGridView.CurrentCell = dataGridView.Rows[v_premises.Position].Cells[0];
        }

        void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            dataGridView.CollapseDetails();
            expanded_id = -1;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                v_premises.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            if (dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
            dataGridView.Refresh();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                v_premises.Position = dataGridView.SelectedRows[0].Index;
            else
                v_premises.Position = -1;
            expanded_id = -1;
            dataGridView.CollapseDetails();
        }

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_premises.Count <= e.RowIndex || v_premises.Count == 0) return;
            int id_premises = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"]);
            int row_index = v_snapshot_tenancy_premises.Find("id_premises", id_premises);
            sync_views = false;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "checked":
                    if (row_index == -1)
                        snapshot_tenancy_premises.Rows.Add(new object[] { null, id_premises, e.Value, null });
                    else
                        ((DataRowView)v_snapshot_tenancy_premises[row_index])["checked"] = e.Value;
                    break;
                case "beds":
                    if (row_index == -1)
                        snapshot_tenancy_premises.Rows.Add(new object[] { null, id_premises, false, e.Value == null ? null : e.Value.ToString().ToLower() });
                    else
                        ((DataRowView)v_snapshot_tenancy_premises[row_index])["beds"] = e.Value == null ? null : e.Value.ToString().ToLower();
                    break;
            }
            sync_views = true;
            menuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_premises.Count <= e.RowIndex || v_premises.Count == 0) return;
            int id_premises = Convert.ToInt32(((DataRowView)v_premises[e.RowIndex])["id_premises"]);
            int row_index = v_snapshot_tenancy_premises.Find("id_premises", id_premises);
            DataRowView row = ((DataRowView)v_premises[e.RowIndex]);
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "image":
                    if (expanded_id == id_premises)
                        e.Value = Registry.Viewport.Properties.Resource.minus;
                    else
                        e.Value = Registry.Viewport.Properties.Resource.plus;
                    break;
                case "checked":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_premises[row_index])["checked"];
                    break;
                case "beds":
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_snapshot_tenancy_premises[row_index])["beds"];
                    if (e.Value != null && e.Value.ToString() != "" && !Regex.IsMatch(e.Value.ToString().ToLower(), "^[0-9а-яА-Я]+([,][0-9а-яА-Я]*)*$"))
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Некорректное значение для номеров койко-мест";
                    else
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
                    break;
                case "id_premises":
                    e.Value = row["id_premises"];
                    break;
                case "id_street":
                    DataRow building_row = buildings.Select().Rows.Find(row["id_building"]);
                    DataRow kladr_row = kladr.Select().Rows.Find(building_row["id_street"]);
                    string street_name = null;
                    if (kladr_row != null)
                    street_name = kladr_row["street_name"].ToString();
                    e.Value = street_name;
                    break;
                case "house":
                    e.Value = buildings.Select().Rows.Find(row["id_building"])["house"];
                    break;
                case "premises_num":
                    e.Value = row["premises_num"];
                    break;
                case "id_premises_type":
                    e.Value = row["id_premises_type"];
                    break;
                case "cadastral_num":
                    e.Value = row["cadastral_num"];
                    break;
                case "total_area":
                    e.Value = row["total_area"];
                    break;
                case "living_area":
                    e.Value = row["living_area"];
                    break;
            }
        }

        public void LocatePremisesBy(int id)
        {
            v_premises.Position = v_premises.Find("id_premises", id);
        }

        void v_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        public override bool CanMoveFirst()
        {
            return v_premises.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_premises.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override void MoveFirst()
        {
            v_premises.MoveFirst();
        }

        public override void MovePrev()
        {
            v_premises.MovePrevious();
        }

        public override void MoveNext()
        {
            v_premises.MoveNext();
        }

        public override void MoveLast()
        {
            v_premises.MoveLast();
        }

        public override bool CanDeleteRecord()
        {
            if (v_premises.Position == -1)
                return false;
            else
                return true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это помещение?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id_building = (int)((DataRowView)v_premises[v_premises.Position])["id_building"];
                if (premises.Delete((int)((DataRowView)v_premises.Current)["id_premises"]) == -1)
                    return;
                ((DataRowView)v_premises[v_premises.Position]).Delete();
                menuCallback.ForceCloseDetachedViewports();
                CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(CalcDataModelFilterEnity.Building, id_building);
            }
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            if (DynamicFilter != "")
                return true;
            else
                return false;
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (spSimpleSearchForm == null)
                        spSimpleSearchForm = new SimpleSearchPremiseForm();
                    if (spSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = spSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (spExtendedSearchForm == null)
                        spExtendedSearchForm = new ExtendedSearchPremisesForm();
                    if (spExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = spExtendedSearchForm.GetFilter();
                    break;
            }
            dataGridView.RowCount = 0;
            v_premises.Filter = DynamicFilter;
            dataGridView.RowCount = v_premises.Count;
        }

        public override void ClearSearch()
        {
            v_premises.Filter = "";
            dataGridView.RowCount = v_premises.Count;
            DynamicFilter = "";
        }

        public override bool CanOpenDetails()
        {
            if (v_premises.Position == -1)
                return false;
            else
                return true;
        }

        public override void OpenDetails()
        {
            PremisesViewport viewport = new PremisesViewport(menuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            if (!premises.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            PremisesViewport viewport = new PremisesViewport(menuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            viewport.InsertRecord();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override bool CanCopyRecord()
        {
            return (v_premises.Position != -1) && !premises.EditingNewRecord;
        }

        public override void CopyRecord()
        {
            PremisesViewport viewport = new PremisesViewport(menuCallback);
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        public override Viewport Duplicate()
        {
            TenancyPremisesViewport viewport = new TenancyPremisesViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override int GetRecordCount()
        {
            return v_premises.Count;
        }

        public override bool HasAssocOwnerships()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocRestrictions()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocSubPremises()
        {
            return (v_premises.Position > -1);
        }

        public override bool HasAssocFundHistory()
        {
            return (v_premises.Position > -1);
        }

        public override void ShowOwnerships()
        {
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение для отображения ограничений", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            OwnershipListViewport viewport = new OwnershipListViewport(menuCallback);
            viewport.StaticFilter = "id_premises = " + Convert.ToInt32(((DataRowView)v_premises[v_premises.Position])["id_premises"]);
            viewport.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Premises;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowRestrictions()
        {
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение для отображения реквизитов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RestrictionListViewport viewport = new RestrictionListViewport(menuCallback);
            viewport.StaticFilter = "id_premises = " + Convert.ToInt32(((DataRowView)v_premises[v_premises.Position])["id_premises"]);
            viewport.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Premises;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowSubPremises()
        {
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение для отображения перечня комнат", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SubPremisesViewport viewport = new SubPremisesViewport(menuCallback);
            viewport.StaticFilter = "id_premises = " + Convert.ToInt32(((DataRowView)v_premises[v_premises.Position])["id_premises"]);
            viewport.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Premises;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowFundHistory()
        {
            if (v_premises.Position == -1)
            {
                MessageBox.Show("Не выбрано помещение для отображения истории найма", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FundsHistoryViewport viewport = new FundsHistoryViewport(menuCallback);
            viewport.StaticFilter = "id_premises = " + Convert.ToInt32(((DataRowView)v_premises[v_premises.Position])["id_premises"]);
            viewport.ParentRow = ((DataRowView)v_premises[v_premises.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Premises;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        private void ConstructViewport()
        {
            this.SuspendLayout();
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            this.Controls.Add(this.dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_image,
            this.field_checked,
            this.field_beds,
            this.field_id_premises,
            this.field_street,
            this.field_house,
            this.field_premises_num,
            this.field_id_premises_type,
            this.field_total_area,
            this.field_living_area,
            this.field_cadastral_num});
            this.dataGridView.Location = new System.Drawing.Point(6, 6);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.TabIndex = 0;
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.MultiSelect = false;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLight;
            //
            // field_image
            //
            field_image.Name = "image";
            field_image.ReadOnly = true;
            field_image.DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLight;
            field_image.Width = 23;
            field_image.Resizable = DataGridViewTriState.False;
            field_image.HeaderText = "";
            field_image.SortMode = DataGridViewColumnSortMode.NotSortable;
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
            // field_id_premises
            // 
            this.field_id_premises.HeaderText = "№";
            this.field_id_premises.Name = "id_premises";
            this.field_id_premises.ReadOnly = true;
            // 
            // field_street
            // 
            this.field_street.HeaderText = "Адрес";
            this.field_street.MinimumWidth = 300;
            this.field_street.Name = "id_street";
            this.field_street.ReadOnly = true;
            // 
            // field_street
            // 
            this.field_house.HeaderText = "Дом";
            this.field_house.Name = "house";
            this.field_house.ReadOnly = true;
            // 
            // field_premises_num
            // 
            this.field_premises_num.HeaderText = "Помещение";
            this.field_premises_num.Name = "premises_num";
            this.field_premises_num.ReadOnly = true;
            // 
            // field_premises_type
            // 
            this.field_id_premises_type.HeaderText = "Тип помещения";
            this.field_id_premises_type.MinimumWidth = 150;
            this.field_id_premises_type.Name = "id_premises_type";
            this.field_id_premises_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            this.field_id_premises_type.SortMode = DataGridViewColumnSortMode.Automatic;
            this.field_id_premises_type.ReadOnly = true;
            // 
            // field_cadastral_num
            // 
            this.field_cadastral_num.HeaderText = "Кадастровый номер";
            this.field_cadastral_num.MinimumWidth = 150;
            this.field_cadastral_num.Name = "cadastral_num";
            this.field_cadastral_num.ReadOnly = true;
            // 
            // field_total_area
            // 
            this.field_total_area.HeaderText = "Общая площадь";
            this.field_total_area.MinimumWidth = 120;
            this.field_total_area.Name = "total_area";
            this.field_total_area.DefaultCellStyle.Format = "#0.0## м²";
            this.field_total_area.ReadOnly = true;
            // 
            // field_living_area
            // 
            this.field_living_area.HeaderText = "Жилая площадь";
            this.field_living_area.MinimumWidth = 150;
            this.field_living_area.Name = "living_area";
            this.field_living_area.DefaultCellStyle.Format = "#0.0## м²";
            this.field_living_area.ReadOnly = true;

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
