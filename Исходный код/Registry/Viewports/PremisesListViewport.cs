using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels;
using System.Collections;
using Registry.Entities;
using System.Threading;

namespace Registry.Viewport
{
    internal sealed class PremisesListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_street = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_house = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_premises_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        //Models
        private PremisesDataModel premises = null;
        private BuildingsDataModel buildings = null;
        private KladrStreetsDataModel kladr = null;
        private PremisesTypesDataModel premises_types = null;

        //Views
        private BindingSource v_premises = null;
        private BindingSource v_buildings = null;
        private BindingSource v_premises_types = null;

        public PremisesListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPagePremises";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень помещений";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public PremisesListViewport(PremisesListViewport premisesListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = premisesListViewport.DynamicFilter;
            this.StaticFilter = premisesListViewport.StaticFilter;
            this.ParentRow = premisesListViewport.ParentRow;
            this.ParentType = premisesListViewport.ParentType;
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

            // Ожидаем дозагрузки данных, если это необходимо
            premises.Select();
            kladr.Select();
            buildings.Select();
            premises_types.Select();

            DataSet ds = DataSetManager.GetDataSet();

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += new EventHandler(v_premises_CurrentItemChanged);
            v_premises.DataMember = "premises";
            v_premises.DataSource = ds;
            v_premises.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_premises.Filter += " AND ";
            v_premises.Filter += DynamicFilter;
            if (ParentRow != null)
            {
                if (ParentType == ParentTypeEnum.Building)
                    Text = "Помещения здания №" + ParentRow["id_building"].ToString();
            }

            v_buildings = new BindingSource();
            v_buildings.DataMember = "buildings";
            v_buildings.DataSource = ds;

            v_premises_types = new BindingSource();
            v_premises_types.DataMember = "premises_types";
            v_premises_types.DataSource = ds;

            field_id_premises_type.DataSource = v_premises_types;
            field_id_premises_type.ValueMember = "id_premises_type";
            field_id_premises_type.DisplayMember = "premises_type";

            dataGridView.CellValueNeeded += new DataGridViewCellValueEventHandler(dataGridView_CellValueNeeded);
            dataGridView.SelectionChanged += new EventHandler(dataGridView_SelectionChanged);
            dataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dataGridView_ColumnHeaderMouseClick);
            v_premises.PositionChanged += new EventHandler(v_premises_PositionChanged);
            premises.Select().RowChanged += new DataRowChangeEventHandler(PremisesListViewport_RowChanged);
            premises.Select().RowDeleting += new DataRowChangeEventHandler(PremisesListViewport_RowDeleting);
            dataGridView.RowCount = v_premises.Count;

            dataGridView.CellDoubleClick += new DataGridViewCellEventHandler(dataGridView_CellDoubleClick);
        }

        void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (CanOpenDetails())
                OpenDetails();
        }

        void PremisesListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.RowCount = v_premises.Count;
            dataGridView.Refresh();
        }

        void PremisesListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
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
            if (dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
            {
                v_premises.Sort = dataGridView.Columns[e.ColumnIndex].Name + " DESC";
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
            }
            else
            {
                v_premises.Sort = dataGridView.Columns[e.ColumnIndex].Name + " ASC";
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            }
            dataGridView.Refresh();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                v_premises.Position = dataGridView.SelectedRows[0].Index;
            else
                v_premises.Position = -1;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_premises.Count <= e.RowIndex) return;
            DataRowView row = ((DataRowView)v_premises[e.RowIndex]);
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
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
                if (premises.Delete((int)((DataRowView)v_premises.Current)["id_premises"]) == -1)
                    return;
                ((DataRowView)v_premises[v_premises.Position]).Delete();
                menuCallback.ForceCloseDetachedViewports();
                BuildingsAggregatedDataModel.GetInstance().Refresh();
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

        public override void SearchRecord()
        {
            SearchPremisesForm spForm = new SearchPremisesForm();
            if (spForm.ShowDialog() == DialogResult.OK)
            {
                DynamicFilter = spForm.GetFilter();
                string Filter = StaticFilter;
                if (StaticFilter != "" && DynamicFilter != "")
                    Filter += " AND ";
                Filter += DynamicFilter;
                dataGridView.RowCount = 0;
                v_premises.Filter = Filter;
                dataGridView.RowCount = v_premises.Count;
            }
        }

        public override void ClearSearch()
        {
            v_premises.Filter = StaticFilter;
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
            viewport.StaticFilter = StaticFilter;
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
            viewport.StaticFilter = StaticFilter;
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
            viewport.StaticFilter = StaticFilter;
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
            PremisesListViewport viewport = new PremisesListViewport(this, menuCallback);
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

        public override bool HasFundHistory()
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
            this.dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.ReadOnly = true;
            this.dataGridView.VirtualMode = true;
            // 
            // field_id_premises
            // 
            this.field_id_premises.HeaderText = "№";
            this.field_id_premises.Name = "id_premises";
            // 
            // field_street
            // 
            this.field_street.HeaderText = "Адрес";
            this.field_street.MinimumWidth = 300;
            this.field_street.Name = "id_street";
            // 
            // field_street
            // 
            this.field_house.HeaderText = "Дом";
            this.field_house.Name = "house";
            // 
            // field_premises_num
            // 
            this.field_premises_num.HeaderText = "Помещение";
            this.field_premises_num.Name = "premises_num";
            // 
            // field_premises_type
            // 
            this.field_id_premises_type.HeaderText = "Тип помещения";
            this.field_id_premises_type.MinimumWidth = 150;
            this.field_id_premises_type.Name = "id_premises_type";
            this.field_id_premises_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            this.field_id_premises_type.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // field_cadastral_num
            // 
            this.field_cadastral_num.HeaderText = "Кадастровый номер";
            this.field_cadastral_num.MinimumWidth = 150;
            this.field_cadastral_num.Name = "cadastral_num";
            // 
            // field_total_area
            // 
            this.field_total_area.HeaderText = "Общая площадь";
            this.field_total_area.MinimumWidth = 120;
            this.field_total_area.Name = "total_area";
            this.field_total_area.DefaultCellStyle.Format = "#0.0## м²";
            // 
            // field_living_area
            // 
            this.field_living_area.HeaderText = "Жилая площадь";
            this.field_living_area.MinimumWidth = 150;
            this.field_living_area.Name = "living_area";
            this.field_living_area.DefaultCellStyle.Format = "#0.0## м²";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
