using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Registry.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class TenancyListViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        private TenancyProcessesDataModel tenancies;
        private TenancyBuildingsAssocDataModel tenancy_building_assoc;
        private TenancyPremisesAssocDataModel tenancy_premises_assoc;
        private TenancySubPremisesAssocDataModel tenancy_sub_premises_assoc;
        private CalcDataModelTenancyAggregated tenancies_aggregate;
        private RentTypesDataModel rent_types;
        #endregion Models

        #region Views
        private BindingSource v_tenancies;
        private BindingSource v_tenancies_aggregate;
        private BindingSource v_rent_types;
        #endregion Views

        //Forms
        private SearchForm stExtendedSearchForm;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn registration_num;
        private DataGridViewTextBoxColumn residence_warrant_num;
        private DataGridViewTextBoxColumn tenant;
        private DataGridViewTextBoxColumn rent_type;
        private DataGridViewTextBoxColumn address;
        private SearchForm stSimpleSearchForm;

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
            DynamicFilter = tenancyListViewport.DynamicFilter;
            StaticFilter = tenancyListViewport.StaticFilter;
            ParentRow = tenancyListViewport.ParentRow;
            ParentType = tenancyListViewport.ParentType;
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
                foreach (var id in ids)
                    StaticFilter += id.ToString(CultureInfo.InvariantCulture) + ",";
                StaticFilter = StaticFilter.TrimEnd(',') + ")";
            }
            var Filter = StaticFilter;
            v_tenancies.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(Filter) && !string.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            v_tenancies.Filter = Filter + DynamicFilter;
        }

        private void SetViewportCaption()
        {
            if (ParentRow == null)
                Text = "Процессы найма жилья";
            else
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        Text = string.Format(CultureInfo.InvariantCulture, "Найм здания №{0}", ParentRow["id_building"]);
                        break;
                    case ParentTypeEnum.Premises:
                        Text = string.Format(CultureInfo.InvariantCulture, "Найм помещения №{0}", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        Text = string.Format(CultureInfo.InvariantCulture, "Найм комнаты №{0}", ParentRow["id_sub_premises"]);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
        }

        public void LocateTenancyBy(int id)
        {
            var Position = v_tenancies.Find("id_process", id);
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
            DockAreas = DockAreas.Document;
            tenancies = TenancyProcessesDataModel.GetInstance();
            rent_types = RentTypesDataModel.GetInstance();
            tenancies_aggregate = CalcDataModelTenancyAggregated.GetInstance();

            //Ожидаем загрузки данных, если это необходимо
            tenancies.Select();
            rent_types.Select();

            SetViewportCaption();

            var ds = DataSetManager.DataSet;

            v_tenancies = new BindingSource();
            v_tenancies.DataMember = "tenancy_processes";
            v_tenancies.CurrentItemChanged += v_tenancies_CurrentItemChanged;
            v_tenancies.DataSource = ds;
            RebuildStaticFilter();
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_tenancies.Filter += " AND ";
            v_tenancies.Filter += DynamicFilter;

            v_tenancies_aggregate = new BindingSource();
            v_tenancies_aggregate.DataSource = tenancies_aggregate.Select();

            v_rent_types = new BindingSource();
            v_rent_types.DataMember = "rent_types";
            v_rent_types.DataSource = ds;

            tenancies.Select().RowChanged += TenancyListViewport_RowChanged;
            tenancies.Select().RowDeleted += TenancyListViewport_RowDeleted;
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        tenancy_building_assoc = TenancyBuildingsAssocDataModel.GetInstance();
                        tenancy_building_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_building_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    case ParentTypeEnum.Premises:
                        tenancy_premises_assoc = TenancyPremisesAssocDataModel.GetInstance();
                        tenancy_premises_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_premises_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    case ParentTypeEnum.SubPremises:
                        tenancy_sub_premises_assoc = TenancySubPremisesAssocDataModel.GetInstance();
                        tenancy_sub_premises_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_sub_premises_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
            dataGridView.RowCount = v_tenancies.Count;
            tenancies_aggregate.RefreshEvent += tenancies_aggregate_RefreshEvent;
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
            if (!string.IsNullOrEmpty(DynamicFilter))
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
            var Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
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
            var viewport = new TenancyViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return (!tenancies.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            var viewport = new TenancyViewport(MenuCallback);
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
            var viewport = new TenancyViewport(MenuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new TenancyListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] as int?) ?? -1);
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

        public override void TenancyContract17xReportGenerate(TenancyContractTypes tenancyContractType)
        {
            if (!TenancyValidForReportGenerate())
                return;
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
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
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
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
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
            ReporterFactory.CreateReporter(ReporterType.TenancyActReporter).
                Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (v_tenancies.Position == -1)
                return false;
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
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

        protected override void OnClosing(CancelEventArgs e)
        {
            tenancies.Select().RowChanged -= TenancyListViewport_RowChanged;
            tenancies.Select().RowDeleted -= TenancyListViewport_RowDeleted;
            tenancies_aggregate.RefreshEvent -= tenancies_aggregate_RefreshEvent;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            tenancies.Select().RowChanged -= TenancyListViewport_RowChanged;
            tenancies.Select().RowDeleted -= TenancyListViewport_RowDeleted;
            tenancies_aggregate.RefreshEvent -= tenancies_aggregate_RefreshEvent;
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
            dataGridView.Refresh();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_tenancies.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
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
                case "tenant":
                    var row_index = v_tenancies_aggregate.Find("id_process", ((DataRowView)v_tenancies[e.RowIndex])["id_process"]);
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
                MenuCallback.DocumentsStateUpdate();
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1260)
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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyListViewport));
            dataGridView = new DataGridView();
            id_process = new DataGridViewTextBoxColumn();
            registration_num = new DataGridViewTextBoxColumn();
            residence_warrant_num = new DataGridViewTextBoxColumn();
            tenant = new DataGridViewTextBoxColumn();
            rent_type = new DataGridViewTextBoxColumn();
            address = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(id_process, registration_num, residence_warrant_num, tenant, rent_type, address);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(1166, 255);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.CellDoubleClick += dataGridView_CellDoubleClick;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            dataGridView.Resize += dataGridView_Resize;
            // 
            // id_process
            // 
            id_process.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            id_process.HeaderText = "№";
            id_process.MinimumWidth = 100;
            id_process.Name = "id_process";
            id_process.ReadOnly = true;
            // 
            // registration_num
            // 
            registration_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            registration_num.HeaderText = "№ договора";
            registration_num.MinimumWidth = 130;
            registration_num.Name = "registration_num";
            registration_num.ReadOnly = true;
            registration_num.Width = 130;
            // 
            // residence_warrant_num
            // 
            residence_warrant_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            residence_warrant_num.HeaderText = "№ ордера";
            residence_warrant_num.MinimumWidth = 130;
            residence_warrant_num.Name = "residence_warrant_num";
            residence_warrant_num.ReadOnly = true;
            residence_warrant_num.Width = 130;
            // 
            // tenant
            // 
            tenant.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tenant.HeaderText = "Наниматель";
            tenant.MinimumWidth = 250;
            tenant.Name = "tenant";
            tenant.ReadOnly = true;
            tenant.SortMode = DataGridViewColumnSortMode.NotSortable;
            tenant.Width = 250;
            // 
            // rent_type
            // 
            rent_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            rent_type.HeaderText = "Тип найма";
            rent_type.MinimumWidth = 150;
            rent_type.Name = "rent_type";
            rent_type.ReadOnly = true;
            rent_type.SortMode = DataGridViewColumnSortMode.NotSortable;
            rent_type.Width = 150;
            // 
            // address
            // 
            address.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            address.HeaderText = "Нанимаемое жилье";
            address.MinimumWidth = 500;
            address.Name = "address";
            address.ReadOnly = true;
            address.SortMode = DataGridViewColumnSortMode.NotSortable;
            address.Width = 500;
            // 
            // TenancyListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(1172, 261);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyListViewport";
            Padding = new Padding(3);
            Text = "Процессы найма жилья";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }

    }
}
