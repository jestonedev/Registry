using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.SearchForms;
using Registry.CalcDataModels;

namespace Registry.Viewport
{
    internal sealed class TenancyListViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_contract = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_registration_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_residence_warrant_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_kumi_order_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_tenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_rent_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_address = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        //Models
        private TenancyContractsDataModel tenancies = null;
        private CalcDataModeTenancyAggregated tenancies_aggregate = null;
        private RentTypesDataModel rent_types = null;

        //Views
        private BindingSource v_tenancies = null;
        private BindingSource v_tenancies_aggregate = null;
        private BindingSource v_rent_types = null;

        //Forms
        private SearchForm stExtendedSearchForm = null;
        private SearchForm stSimpleSearchForm = null;

        public TenancyListViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageTenancies";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Процессы найма жилья";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public TenancyListViewport(TenancyListViewport tenancyListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = tenancyListViewport.DynamicFilter;
            this.StaticFilter = tenancyListViewport.StaticFilter;
            this.ParentRow = tenancyListViewport.ParentRow;
            this.ParentType = tenancyListViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            tenancies = TenancyContractsDataModel.GetInstance();
            rent_types = RentTypesDataModel.GetInstance();
            tenancies_aggregate = CalcDataModeTenancyAggregated.GetInstance();

            //Ожидаем загрузки данных, если это необходимо
            tenancies.Select();
            rent_types.Select();

            DataSet ds = DataSetManager.GetDataSet();

            v_tenancies = new BindingSource();
            v_tenancies.DataMember = "tenancy_contracts";
            v_tenancies.CurrentItemChanged += new EventHandler(v_tenancies_CurrentItemChanged);
            v_tenancies.DataSource = ds;
            v_tenancies.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_tenancies.Filter += " AND ";
            v_tenancies.Filter += DynamicFilter;
            v_tenancies.PositionChanged += new EventHandler(v_tenancies_PositionChanged);

            v_tenancies_aggregate = new BindingSource();
            v_tenancies_aggregate.DataSource = tenancies_aggregate.Select();

            v_rent_types = new BindingSource();
            v_rent_types.DataMember = "rent_types";
            v_rent_types.DataSource = ds;

            tenancies.Select().RowChanged += new DataRowChangeEventHandler(TenancyListViewport_RowChanged);
            tenancies.Select().RowDeleted += new DataRowChangeEventHandler(TenancyListViewport_RowDeleted);
            dataGridView.CellValueNeeded += new DataGridViewCellValueEventHandler(dataGridView_CellValueNeeded);
            dataGridView.SelectionChanged += new EventHandler(dataGridView_SelectionChanged);
            dataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dataGridView_ColumnHeaderMouseClick);
            dataGridView.RowCount = v_tenancies.Count;
            dataGridView.CellDoubleClick += new DataGridViewCellEventHandler(dataGridView_CellDoubleClick);
            tenancies_aggregate.RefreshEvent += new EventHandler<EventArgs>(tenancies_aggregate_RefreshEvent);
        }

        void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (CanOpenDetails())
                OpenDetails();
        }

        void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                v_tenancies.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
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
                v_tenancies.Position = dataGridView.SelectedRows[0].Index;
            else
                v_tenancies.Position = -1;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_tenancies.Count <= e.RowIndex) return;
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_contract":
                    e.Value = ((DataRowView)v_tenancies[e.RowIndex])["id_contract"];
                    break;
                case "registration_num":
                    e.Value = ((DataRowView)v_tenancies[e.RowIndex])["registration_num"];
                    break;
                case "residence_warrant_num":
                    e.Value = ((DataRowView)v_tenancies[e.RowIndex])["residence_warrant_num"];
                    break;
                case "kumi_order_num":
                    e.Value = ((DataRowView)v_tenancies[e.RowIndex])["kumi_order_num"];
                    break;
                case "tenant":
                    int row_index = v_tenancies_aggregate.Find("id_contract", ((DataRowView)v_tenancies[e.RowIndex])["id_contract"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_tenancies_aggregate[row_index])["tenant"];
                    break;
                case "rent_type":
                    row_index = v_rent_types.Find("id_rent_type", ((DataRowView)v_tenancies[e.RowIndex])["id_rent_type"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_rent_types[row_index])["rent_type"];
                    break;
                case "address":
                    row_index = v_tenancies_aggregate.Find("id_contract", ((DataRowView)v_tenancies[e.RowIndex])["id_contract"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_tenancies_aggregate[row_index])["address"];
                    break;
            }
        }

        public void LocateTenancyBy(int id)
        {
            v_tenancies.Position = v_tenancies.Find("id_contract", id);
        }

        void tenancies_aggregate_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        void TenancyListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.RowCount = v_tenancies.Count;
            dataGridView.Refresh();
        }

        void TenancyListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_tenancies.Count;
        }

        void v_tenancies_PositionChanged(object sender, EventArgs e)
        {
            if (v_tenancies.Position == -1 || dataGridView.Rows.Count == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            dataGridView.Rows[v_tenancies.Position].Selected = true;
            dataGridView.CurrentCell = dataGridView.Rows[v_tenancies.Position].Cells[0];
        }

        void v_tenancies_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();

        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            TenancyListViewport viewport = new TenancyListViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanMoveFirst()
        {
            return v_tenancies.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_tenancies.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_tenancies.Position > -1) && (v_tenancies.Position < (v_tenancies.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_tenancies.Position > -1) && (v_tenancies.Position < (v_tenancies.Count - 1));
        }

        public override void MoveFirst()
        {
            v_tenancies.MoveFirst();
        }

        public override void MovePrev()
        {
            v_tenancies.MovePrevious();
        }

        public override void MoveNext()
        {
            v_tenancies.MoveNext();
        }

        public override void MoveLast()
        {
            v_tenancies.MoveLast();
        }

        public override bool CanDeleteRecord()
        {
            if (v_tenancies.Position == -1)
                return false;
            else
                return true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот процесс найма жилья?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (tenancies.Delete((int)((DataRowView)v_tenancies.Current)["id_contract"]) == -1)
                    return;
                ((DataRowView)v_tenancies[v_tenancies.Position]).Delete();
                menuCallback.ForceCloseDetachedViewports();
            }
        }

        public override int GetRecordCount()
        {
            return v_tenancies.Count;
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
                    if (stSimpleSearchForm == null)
                        stSimpleSearchForm = new SimpleSearchTenancyForm();
                    if (stSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = stSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (stExtendedSearchForm == null)
                        stExtendedSearchForm = new ExtendedSearchTenancyForm();
                    if (stExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = stExtendedSearchForm.GetFilter();
                    break;
            }
            string Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                Filter += " AND ";
            Filter += DynamicFilter;
            dataGridView.RowCount = 0;
            v_tenancies.Filter = Filter;
            dataGridView.RowCount = v_tenancies.Count;
        }

        public override void ClearSearch()
        {
            v_tenancies.Filter = StaticFilter;
            dataGridView.RowCount = v_tenancies.Count;
            DynamicFilter = "";
        }

        public override bool CanOpenDetails()
        {
            if (v_tenancies.Position == -1)
                return false;
            else
                return true;
        }

        public override void OpenDetails()
        {
            TenancyViewport viewport = new TenancyViewport(menuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            if (!tenancies.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            TenancyViewport viewport = new TenancyViewport(menuCallback);
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
            if ((v_tenancies.Position != -1) && (!tenancies.EditingNewRecord))
                return true;
            else
                return false;
        }

        public override void CopyRecord()
        {
            TenancyViewport viewport = new TenancyViewport(menuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
            viewport.CopyRecord();
        }

        public override void Close()
        {
            tenancies.Select().RowChanged -= new DataRowChangeEventHandler(TenancyListViewport_RowChanged);
            tenancies.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyListViewport_RowDeleted);
            tenancies_aggregate.RefreshEvent -= new EventHandler<EventArgs>(tenancies_aggregate_RefreshEvent);
            base.Close();
        }

        public override bool HasAssocPersons()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocContractReasons()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocAgreements()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocTenancyObjects()
        {
            return (v_tenancies.Position > -1);
        }

        public override void ShowPersons()
        {
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения участников", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PersonsViewport viewport = new PersonsViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowContractReasons()
        {
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения оснований найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ContractReasonsViewport viewport = new ContractReasonsViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowAgreements()
        {
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения соглашений по найму", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AgreementsViewport viewport = new AgreementsViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowTenancyBuildings()
        {
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения нанимаемых зданий", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            TenancyBuildingsViewport viewport = new TenancyBuildingsViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowTenancyPremises()
        {
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения нанимаемых помещений", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            TenancyPremisesViewport viewport = new TenancyPremisesViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        private void ConstructViewport()
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.Controls.Add(dataGridView);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font =
                new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_contract,
            this.field_registration_num,
            this.field_residence_warrant_num,
            this.field_kumi_order_num,
            this.field_tenant,
            this.field_rent_type,
            this.field_address});
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.TabIndex = 0;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.MultiSelect = false;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.ReadOnly = true;
            this.dataGridView.VirtualMode = true;
            dataGridView.ScrollBars = ScrollBars.Both;
            // 
            // field_id_contract
            // 
            this.field_id_contract.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_id_contract.HeaderText = "№";
            this.field_id_contract.MinimumWidth = 100;
            this.field_id_contract.Name = "id_contract";
            // 
            // field_registration_num
            // 
            this.field_registration_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_registration_num.HeaderText = "№ договора";
            this.field_registration_num.MinimumWidth = 120;
            this.field_registration_num.Name = "registration_num";
            // 
            // field_residence_warrant_num
            // 
            this.field_residence_warrant_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_residence_warrant_num.HeaderText = "№ ордера";
            this.field_residence_warrant_num.MinimumWidth = 120;
            this.field_residence_warrant_num.Name = "residence_warrant_num";
            // 
            // field_kumi_order_num
            // 
            this.field_kumi_order_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_kumi_order_num.HeaderText = "№ распоряжения";
            this.field_kumi_order_num.MinimumWidth = 120;
            this.field_kumi_order_num.Name = "kumi_order_num";
            // 
            // field_tenant
            // 
            this.field_tenant.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_tenant.HeaderText = "Наниматель";
            this.field_tenant.MinimumWidth = 200;
            this.field_tenant.Name = "tenant";
            this.field_tenant.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // field_rent_type
            // 
            this.field_rent_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_rent_type.HeaderText = "Тип найма";
            this.field_rent_type.MinimumWidth = 150;
            this.field_rent_type.Name = "rent_type";
            // 
            // field_address
            // 
            this.field_address.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_address.HeaderText = "Нанимаемое жилье";
            this.field_address.MinimumWidth = 300;
            this.field_address.Name = "address";
            this.field_address.SortMode = DataGridViewColumnSortMode.NotSortable;

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
