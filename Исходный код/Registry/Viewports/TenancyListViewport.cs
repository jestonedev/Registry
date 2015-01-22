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
using Registry.Reporting;
using Security;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class TenancyListViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        private TenancyProcessesDataModel tenancies = null;
        private TenancyBuildingsAssocDataModel tenancy_building_assoc = null;
        private TenancyPremisesAssocDataModel tenancy_premises_assoc = null;
        private TenancySubPremisesAssocDataModel tenancy_sub_premises_assoc = null;
        private CalcDataModelTenancyAggregated tenancies_aggregate = null;
        private RentTypesDataModel rent_types = null;
        #endregion Models

        #region Views
        private BindingSource v_tenancies = null;
        private BindingSource v_tenancies_aggregate = null;
        private BindingSource v_rent_types = null;
        #endregion Views

        //Forms
        private SearchForm stExtendedSearchForm = null;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn registration_num;
        private DataGridViewTextBoxColumn residence_warrant_num;
        private DataGridViewTextBoxColumn kumi_order_num;
        private DataGridViewTextBoxColumn tenant;
        private DataGridViewTextBoxColumn rent_type;
        private DataGridViewTextBoxColumn address;
        private SearchForm stSimpleSearchForm = null;

        private TenancyListViewport()
            : this(null)
        {
        }

        public TenancyListViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }
        
        public TenancyListViewport(TenancyListViewport tenancyListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = tenancyListViewport.DynamicFilter;
            this.StaticFilter = tenancyListViewport.StaticFilter;
            this.ParentRow = tenancyListViewport.ParentRow;
            this.ParentType = tenancyListViewport.ParentType;
        }

        private void RebuildStaticFilter()
        {
            IEnumerable<int> ids = null;
            if (ParentRow == null)
                return;
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    ids = DataModelHelper.TenancyProcessIDsByBuildingID(Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.Premises:
                    ids = DataModelHelper.TenancyProcessIDsByPremisesID(Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.SubPremises:
                    ids = DataModelHelper.TenancyProcessIDsBySubPremisesID(Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture));
                    break;
                default: 
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
            if (ids != null)
            {
                StaticFilter = "id_process IN (0";
                foreach (int id in ids)
                    StaticFilter += id.ToString(CultureInfo.InvariantCulture) + ",";
                StaticFilter = StaticFilter.TrimEnd(new char[] { ',' }) + ")";
            }
            string Filter = StaticFilter;
            v_tenancies.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(Filter) && !String.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            v_tenancies.Filter = Filter + DynamicFilter;
        }

        private void SetViewportCaption()
        {
            if (ParentRow == null)
                this.Text = "Процессы найма жилья";
            else
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Найм здания №{0}", ParentRow["id_building"]);
                        break;
                    case ParentTypeEnum.Premises:
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Найм помещения №{0}", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Найм комнаты №{0}", ParentRow["id_sub_premises"]);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
        }

        public void LocateTenancyBy(int id)
        {
            int Position = v_tenancies.Find("id_process", id);
            if (Position > 0)
                v_tenancies.Position = Position;
        }

        public override int GetRecordCount()
        {
            return v_tenancies.Count;
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            tenancies = TenancyProcessesDataModel.GetInstance();
            rent_types = RentTypesDataModel.GetInstance();
            tenancies_aggregate = CalcDataModelTenancyAggregated.GetInstance();

            //Ожидаем загрузки данных, если это необходимо
            tenancies.Select();
            rent_types.Select();

            SetViewportCaption();

            DataSet ds = DataSetManager.DataSet;

            v_tenancies = new BindingSource();
            v_tenancies.DataMember = "tenancy_processes";
            v_tenancies.CurrentItemChanged += new EventHandler(v_tenancies_CurrentItemChanged);
            v_tenancies.DataSource = ds;
            RebuildStaticFilter();
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_tenancies.Filter += " AND ";
            v_tenancies.Filter += DynamicFilter;

            v_tenancies_aggregate = new BindingSource();
            v_tenancies_aggregate.DataSource = tenancies_aggregate.Select();

            v_rent_types = new BindingSource();
            v_rent_types.DataMember = "rent_types";
            v_rent_types.DataSource = ds;

            tenancies.Select().RowChanged += new DataRowChangeEventHandler(TenancyListViewport_RowChanged);
            tenancies.Select().RowDeleted += new DataRowChangeEventHandler(TenancyListViewport_RowDeleted);
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        tenancy_building_assoc = TenancyBuildingsAssocDataModel.GetInstance();
                        tenancy_building_assoc.Select().RowChanged += new DataRowChangeEventHandler(TenancyAssocViewport_RowChanged);
                        tenancy_building_assoc.Select().RowDeleted += new DataRowChangeEventHandler(TenancyAssocViewport_RowDeleted);
                        break;
                    case ParentTypeEnum.Premises:
                        tenancy_premises_assoc = TenancyPremisesAssocDataModel.GetInstance();
                        tenancy_premises_assoc.Select().RowChanged += new DataRowChangeEventHandler(TenancyAssocViewport_RowChanged);
                        tenancy_premises_assoc.Select().RowDeleted += new DataRowChangeEventHandler(TenancyAssocViewport_RowDeleted);
                        break;
                    case ParentTypeEnum.SubPremises:
                        tenancy_sub_premises_assoc = TenancySubPremisesAssocDataModel.GetInstance();
                        tenancy_sub_premises_assoc.Select().RowChanged += new DataRowChangeEventHandler(TenancyAssocViewport_RowChanged);
                        tenancy_sub_premises_assoc.Select().RowDeleted += new DataRowChangeEventHandler(TenancyAssocViewport_RowDeleted);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
            dataGridView.RowCount = v_tenancies.Count;
            tenancies_aggregate.RefreshEvent += new EventHandler<EventArgs>(tenancies_aggregate_RefreshEvent);
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public override bool CanDeleteRecord()
        {
            return (v_tenancies.Position > -1) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот процесс найма жилья?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (TenancyProcessesDataModel.Delete((int)((DataRowView)v_tenancies.Current)["id_process"]) == -1)
                    return;
                ((DataRowView)v_tenancies[v_tenancies.Position]).Delete();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            if (!String.IsNullOrEmpty(DynamicFilter))
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
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
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
            TenancyViewport viewport = new TenancyViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] as Int32?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return (!tenancies.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            TenancyViewport viewport = new TenancyViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            viewport.InsertRecord();
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanCopyRecord()
        {
            return (v_tenancies.Position != -1) && (!tenancies.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            TenancyViewport viewport = new TenancyViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] as Int32?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            TenancyListViewport viewport = new TenancyListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool HasAssocTenancyPersons()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocTenancyReasons()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocTenancyAgreements()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocTenancyObjects()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocClaims()
        {
            return (v_tenancies.Position > -1);
        }

        public override void ShowTenancyPersons()
        {
            ShowAssocViewport(ViewportType.TenancyPersonsViewport);
        }

        public override void ShowTenancyReasons()
        {
            ShowAssocViewport(ViewportType.TenancyReasonsViewport);
        }

        public override void ShowTenancyAgreements()
        {
            ShowAssocViewport(ViewportType.TenancyAgreementsViewport);
        }

        public override void ShowTenancyBuildings()
        {
            ShowAssocViewport(ViewportType.TenancyBuildingsViewport);
        }

        public override void ShowTenancyPremises()
        {
            ShowAssocViewport(ViewportType.TenancyPremisesViewport);
        }

        public override void ShowClaims()
        {
            ShowAssocViewport(ViewportType.ClaimListViewport);
        }

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_process = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_process"], CultureInfo.InvariantCulture),
                ((DataRowView)v_tenancies[v_tenancies.Position]).Row,
                ParentTypeEnum.Tenancy);
        }

        public override bool HasTenancyContract17xReport()
        {
            return (v_tenancies.Position > -1) &&
                Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_rent_type"], CultureInfo.InvariantCulture) == 2;
        }

        public override bool HasTenancyContractReport()
        {
            return (v_tenancies.Position > -1) &&
                Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_rent_type"], CultureInfo.InvariantCulture) != 2;
        }

        public override bool HasTenancyActReport()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasTenancyExcerptReport()
        {
            return (v_tenancies.Position > -1);
        }

        public override void TenancyContract17xReportGenerate(Reporting.TenancyContractTypes tenancyContractType)
        {
            if (!TenancyValidForReportGenerate())
                return;
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
            if (ViewportHelper.ValueOrNull<int>(row, "id_rent_type") != 2)
            {
                MessageBox.Show("Для формирования договора по формам 1711 и 1712 необходимо, чтобы тип найма был - специализированный", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (tenancyContractType == TenancyContractTypes.SpecialContract1711Form)
                ReporterFactory.CreateReporter(ReporterType.TenancyContractSpecial1711Reporter).
                    Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
            else
            if (tenancyContractType == TenancyContractTypes.SpecialContract1712Form)
                ReporterFactory.CreateReporter(ReporterType.TenancyContractSpecial1712Reporter).
                    Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
        }

        public override void TenancyContractReportGenerate()
        {
            if (!TenancyValidForReportGenerate())
                return;
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
            if (ViewportHelper.ValueOrNull<int>(row, "id_rent_type") == 2)
                MessageBox.Show("Для формирования договора специализированного найма необходимо выбрать форму договора: 1711 или 1712", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            else
            if (ViewportHelper.ValueOrNull<int>(row, "id_rent_type") == 1)
                ReporterFactory.CreateReporter(ReporterType.TenancyContractCommercialReporter).
                    Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } }); 
            else
            if (ViewportHelper.ValueOrNull<int>(row, "id_rent_type") == 3)
                ReporterFactory.CreateReporter(ReporterType.TenancyContractSocialReporter).
                    Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
        }

        public override void TenancyActReportGenerate()
        {
            if (!TenancyValidForReportGenerate())
                return;
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
            ReporterFactory.CreateReporter(ReporterType.TenancyActReporter).
                Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
        }

        public override void TenancyExcerptReportGenerate()
        {
            if (!TenancyValidForReportGenerate())
                return;
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
            ReporterFactory.CreateReporter(ReporterType.TenancyExcerptReporter).
                Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (v_tenancies.Position == -1)
                return false;
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
            if (!DataModelHelper.TenancyProcessHasTenant(Convert.ToInt32(row["id_process"], CultureInfo.InvariantCulture)))
            {
                MessageBox.Show("Для формирования отчетной документации необходимо указать нанимателя процесса найма","Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(row, "registration_date") == null || ViewportHelper.ValueOrNull(row, "registration_num") == null)
            {
                MessageBox.Show("Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации","Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            tenancies.Select().RowChanged -= new DataRowChangeEventHandler(TenancyListViewport_RowChanged);
            tenancies.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyListViewport_RowDeleted);
            tenancies_aggregate.RefreshEvent -= new EventHandler<EventArgs>(tenancies_aggregate_RefreshEvent);
        }

        public override void ForceClose()
        {
            tenancies.Select().RowChanged -= new DataRowChangeEventHandler(TenancyListViewport_RowChanged);
            tenancies.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyListViewport_RowDeleted);
            tenancies_aggregate.RefreshEvent -= new EventHandler<EventArgs>(tenancies_aggregate_RefreshEvent);
            base.ForceClose();
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
                case "id_process":
                    e.Value = ((DataRowView)v_tenancies[e.RowIndex])["id_process"];
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
                    int row_index = v_tenancies_aggregate.Find("id_process", ((DataRowView)v_tenancies[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_tenancies_aggregate[row_index])["tenant"];
                    break;
                case "rent_type":
                    row_index = v_rent_types.Find("id_rent_type", ((DataRowView)v_tenancies[e.RowIndex])["id_rent_type"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_rent_types[row_index])["rent_type"];
                    break;
                case "address":
                    row_index = v_tenancies_aggregate.Find("id_process", ((DataRowView)v_tenancies[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_tenancies_aggregate[row_index])["address"];
                    break;
            }
        }

        void tenancies_aggregate_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        void TenancyListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.RowCount = v_tenancies.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void TenancyListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_tenancies.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void TenancyAssocViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RebuildStaticFilter();
            dataGridView.RowCount = v_tenancies.Count;
        }

        private void TenancyAssocViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RebuildStaticFilter();
            dataGridView.RowCount = v_tenancies.Count;
        }

        void v_tenancies_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_tenancies.Position == -1 || dataGridView.RowCount == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            if (v_tenancies.Position >= dataGridView.RowCount)
            {
                dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridView.Rows[v_tenancies.Position].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[v_tenancies.Position].Cells[0];
            }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
                MenuCallback.TenancyRefsStateUpdate();
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1430)
            {
                if (dataGridView.Columns["address"].AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    dataGridView.Columns["address"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (dataGridView.Columns["address"].AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    dataGridView.Columns["address"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyListViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.residence_warrant_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kumi_order_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rent_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.id_process,
            this.registration_num,
            this.residence_warrant_num,
            this.kumi_order_num,
            this.tenant,
            this.rent_type,
            this.address});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(1166, 255);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // id_process
            // 
            this.id_process.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_process.HeaderText = "№";
            this.id_process.MinimumWidth = 100;
            this.id_process.Name = "id_process";
            this.id_process.ReadOnly = true;
            // 
            // registration_num
            // 
            this.registration_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.registration_num.HeaderText = "№ договора";
            this.registration_num.MinimumWidth = 130;
            this.registration_num.Name = "registration_num";
            this.registration_num.ReadOnly = true;
            this.registration_num.Width = 130;
            // 
            // residence_warrant_num
            // 
            this.residence_warrant_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.residence_warrant_num.HeaderText = "№ ордера";
            this.residence_warrant_num.MinimumWidth = 130;
            this.residence_warrant_num.Name = "residence_warrant_num";
            this.residence_warrant_num.ReadOnly = true;
            this.residence_warrant_num.Width = 130;
            // 
            // kumi_order_num
            // 
            this.kumi_order_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.kumi_order_num.HeaderText = "№ распоряжения";
            this.kumi_order_num.MinimumWidth = 130;
            this.kumi_order_num.Name = "kumi_order_num";
            this.kumi_order_num.ReadOnly = true;
            this.kumi_order_num.Width = 130;
            // 
            // tenant
            // 
            this.tenant.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.tenant.HeaderText = "Наниматель";
            this.tenant.MinimumWidth = 250;
            this.tenant.Name = "tenant";
            this.tenant.ReadOnly = true;
            this.tenant.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.tenant.Width = 250;
            // 
            // rent_type
            // 
            this.rent_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.rent_type.HeaderText = "Тип найма";
            this.rent_type.MinimumWidth = 150;
            this.rent_type.Name = "rent_type";
            this.rent_type.ReadOnly = true;
            this.rent_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rent_type.Width = 150;
            // 
            // address
            // 
            this.address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.address.HeaderText = "Нанимаемое жилье";
            this.address.MinimumWidth = 500;
            this.address.Name = "address";
            this.address.ReadOnly = true;
            this.address.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.address.Width = 500;
            // 
            // TenancyListViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1172, 261);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Процессы найма жилья";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

    }
}
