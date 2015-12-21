using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.SearchForms;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal partial class PaymentsAccountsViewport : FormWithGridViewport
    {
        private SearchForm spExtendedSearchForm;

        private PaymentsAccountsViewport()
            : this(null, null)
        {
        }

        public PaymentsAccountsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            dataGridView.AutoGenerateColumns = false;
            GeneralDataModel = DataModel.GetInstance(DataModelType.PaymentsAccountsDataModel);
            GeneralDataModel.Select();
            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataSource = DataModel.DataSet;
            GeneralBindingSource.DataMember = "payments_accounts";
            var filter = "";
            var title = "Лицевые счета";
            if (ParentRow != null)
            {
                IEnumerable<int> ids = new List<int>();
                switch (ParentType)
                {
                    case ParentTypeEnum.Premises:
                        ids = PaymentsAccountsDataModel.GetAccountIdsByPremiseFilter(StaticFilter);
                        title = string.Format("Лицевой счет помещение №{0}", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        ids = PaymentsAccountsDataModel.GetAccountIdsBySubPremiseFilter(StaticFilter);
                        title = string.Format("Лицевой счет комнаты {0} помещения №{1}", ParentRow["sub_premises_num"], ParentRow["id_premises"]);
                        break;
                }
                if (ids.Any())
                    filter = "id_account IN (" + ids.Select(id => id.ToString()).Aggregate((acc, v) => acc + "," + v) +
                             ")";
                else
                    filter = "id_account IN (0)";
            }
            StaticFilter = filter;
            if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            GeneralBindingSource.Filter = filter;

            Text = title;

            DataBind();
            DataGridView.RowCount = GeneralBindingSource.Count;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            if (DataGridView.RowCount > 0)
                DataGridView.Rows[0].Selected = true;
        }

        private void DataBind()
        {
            textBoxCRN.DataBindings.Clear();
            textBoxCRN.DataBindings.Add("Text", GeneralBindingSource, "crn", true, DataSourceUpdateMode.Never, "");
            textBoxRawAddress.DataBindings.Clear();
            textBoxRawAddress.DataBindings.Add("Text", GeneralBindingSource, "raw_address", true, DataSourceUpdateMode.Never, "");
            textBoxAddress.DataBindings.Clear();
            textBoxAddress.DataBindings.Add("Text", GeneralBindingSource, "parsed_address", true, DataSourceUpdateMode.Never, "");
            textBoxAccount.DataBindings.Clear();
            textBoxAccount.DataBindings.Add("Text", GeneralBindingSource, "account", true, DataSourceUpdateMode.Never, "");
            textBoxTenant.DataBindings.Clear();
            textBoxTenant.DataBindings.Add("Text", GeneralBindingSource, "tenant", true, DataSourceUpdateMode.Never, "");

            numericUpDownTotalArea.DataBindings.Clear();
            numericUpDownTotalArea.DataBindings.Add("Value", GeneralBindingSource, "total_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownTotalArea.DataBindings.Add("Minimum", GeneralBindingSource, "total_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownTotalArea.DataBindings.Add("Maximum", GeneralBindingSource, "total_area", true, DataSourceUpdateMode.Never, 0);

            numericUpDownLivingArea.DataBindings.Clear();
            numericUpDownLivingArea.DataBindings.Add("Value", GeneralBindingSource, "living_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownLivingArea.DataBindings.Add("Minimum", GeneralBindingSource, "living_area", true, DataSourceUpdateMode.Never, 0);
            numericUpDownLivingArea.DataBindings.Add("Maximum", GeneralBindingSource, "living_area", true, DataSourceUpdateMode.Never, 0);

            numericUpDownPrescribed.DataBindings.Clear();
            numericUpDownPrescribed.DataBindings.Add("Value", GeneralBindingSource, "prescribed", true, DataSourceUpdateMode.Never, 0);
            numericUpDownPrescribed.DataBindings.Add("Minimum", GeneralBindingSource, "prescribed", true, DataSourceUpdateMode.Never, 0);
            numericUpDownPrescribed.DataBindings.Add("Maximum", GeneralBindingSource, "prescribed", true, DataSourceUpdateMode.Never, 0);

            numericUpDownBalanceTotalInput.DataBindings.Clear();
            numericUpDownBalanceTotalInput.DataBindings.Add("Value", GeneralBindingSource, "balance_input", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceTotalInput.DataBindings.Add("Minimum", GeneralBindingSource, "balance_input", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceTotalInput.DataBindings.Add("Maximum", GeneralBindingSource, "balance_input", true, DataSourceUpdateMode.Never, 0);

            numericUpDownBalanceTenancyInput.DataBindings.Clear();
            numericUpDownBalanceTenancyInput.DataBindings.Add("Value", GeneralBindingSource, "balance_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceTenancyInput.DataBindings.Add("Minimum", GeneralBindingSource, "balance_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceTenancyInput.DataBindings.Add("Maximum", GeneralBindingSource, "balance_tenancy", true, DataSourceUpdateMode.Never, 0);

            numericUpDownBalanceDGIInput.DataBindings.Clear();
            numericUpDownBalanceDGIInput.DataBindings.Add("Value", GeneralBindingSource, "balance_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceDGIInput.DataBindings.Add("Minimum", GeneralBindingSource, "balance_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceDGIInput.DataBindings.Add("Maximum", GeneralBindingSource, "balance_dgi", true, DataSourceUpdateMode.Never, 0);

            numericUpDownChargingTotal.DataBindings.Clear();
            numericUpDownChargingTotal.DataBindings.Add("Value", GeneralBindingSource, "charging_total", true, DataSourceUpdateMode.Never, 0);
            numericUpDownChargingTotal.DataBindings.Add("Minimum", GeneralBindingSource, "charging_total", true, DataSourceUpdateMode.Never, 0);
            numericUpDownChargingTotal.DataBindings.Add("Maximum", GeneralBindingSource, "charging_total", true, DataSourceUpdateMode.Never, 0);

            numericUpDownChargingTenancy.DataBindings.Clear();
            numericUpDownChargingTenancy.DataBindings.Add("Value", GeneralBindingSource, "charging_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownChargingTenancy.DataBindings.Add("Minimum", GeneralBindingSource, "charging_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownChargingTenancy.DataBindings.Add("Maximum", GeneralBindingSource, "charging_tenancy", true, DataSourceUpdateMode.Never, 0);

            numericUpDownChargingDGI.DataBindings.Clear();
            numericUpDownChargingDGI.DataBindings.Add("Value", GeneralBindingSource, "charging_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownChargingDGI.DataBindings.Add("Minimum", GeneralBindingSource, "charging_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownChargingDGI.DataBindings.Add("Maximum", GeneralBindingSource, "charging_dgi", true, DataSourceUpdateMode.Never, 0);

            numericUpDownRecalcTenancy.DataBindings.Clear();
            numericUpDownRecalcTenancy.DataBindings.Add("Value", GeneralBindingSource, "recalc_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownRecalcTenancy.DataBindings.Add("Minimum", GeneralBindingSource, "recalc_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownRecalcTenancy.DataBindings.Add("Maximum", GeneralBindingSource, "recalc_tenancy", true, DataSourceUpdateMode.Never, 0);

            numericUpDownRecalcDGI.DataBindings.Clear();
            numericUpDownRecalcDGI.DataBindings.Add("Value", GeneralBindingSource, "recalc_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownRecalcDGI.DataBindings.Add("Minimum", GeneralBindingSource, "recalc_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownRecalcDGI.DataBindings.Add("Maximum", GeneralBindingSource, "recalc_dgi", true, DataSourceUpdateMode.Never, 0);

            numericUpDownPaymentTenancy.DataBindings.Clear();
            numericUpDownPaymentTenancy.DataBindings.Add("Value", GeneralBindingSource, "payment_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownPaymentTenancy.DataBindings.Add("Minimum", GeneralBindingSource, "payment_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownPaymentTenancy.DataBindings.Add("Maximum", GeneralBindingSource, "payment_tenancy", true, DataSourceUpdateMode.Never, 0);

            numericUpDownPaymentDGI.DataBindings.Clear();
            numericUpDownPaymentDGI.DataBindings.Add("Value", GeneralBindingSource, "payment_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownPaymentDGI.DataBindings.Add("Minimum", GeneralBindingSource, "payment_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownPaymentDGI.DataBindings.Add("Maximum", GeneralBindingSource, "payment_dgi", true, DataSourceUpdateMode.Never, 0);

            numericUpDownTransferBalance.DataBindings.Clear();
            numericUpDownTransferBalance.DataBindings.Add("Value", GeneralBindingSource, "transfer_balance", true, DataSourceUpdateMode.Never, 0);
            numericUpDownTransferBalance.DataBindings.Add("Minimum", GeneralBindingSource, "transfer_balance", true, DataSourceUpdateMode.Never, 0);
            numericUpDownTransferBalance.DataBindings.Add("Maximum", GeneralBindingSource, "transfer_balance", true, DataSourceUpdateMode.Never, 0);

            numericUpDownBalanceTotalOutput.DataBindings.Clear();
            numericUpDownBalanceTotalOutput.DataBindings.Add("Value", GeneralBindingSource, "balance_output_total", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceTotalOutput.DataBindings.Add("Minimum", GeneralBindingSource, "balance_output_total", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceTotalOutput.DataBindings.Add("Maximum", GeneralBindingSource, "balance_output_total", true, DataSourceUpdateMode.Never, 0);

            numericUpDownBalanceTenancyOutput.DataBindings.Clear();
            numericUpDownBalanceTenancyOutput.DataBindings.Add("Value", GeneralBindingSource, "balance_output_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceTenancyOutput.DataBindings.Add("Minimum", GeneralBindingSource, "balance_output_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceTenancyOutput.DataBindings.Add("Maximum", GeneralBindingSource, "balance_output_tenancy", true, DataSourceUpdateMode.Never, 0);

            numericUpDownBalanceDGIOutput.DataBindings.Clear();
            numericUpDownBalanceDGIOutput.DataBindings.Add("Value", GeneralBindingSource, "balance_output_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceDGIOutput.DataBindings.Add("Minimum", GeneralBindingSource, "balance_output_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownBalanceDGIOutput.DataBindings.Add("Maximum", GeneralBindingSource, "balance_output_dgi", true, DataSourceUpdateMode.Never, 0);

            dateTimePickerAtDate.DataBindings.Clear();
            dateTimePickerAtDate.DataBindings.Add("Value", GeneralBindingSource, "date", true, DataSourceUpdateMode.Never, 0);

        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            return !string.IsNullOrEmpty(DynamicFilter);
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                case SearchFormType.ExtendedSearchForm:
                    if (spExtendedSearchForm == null)
                        spExtendedSearchForm = new ExtendedSearchPaymentAccounts();
                    if (spExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = spExtendedSearchForm.GetFilter();
                    break;
            }
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            dataGridView.RowCount = 0;
            GeneralBindingSource.Filter = filter;
            dataGridView.RowCount = GeneralBindingSource.Count;
        }

        public override void ClearSearch()
        {
            GeneralBindingSource.Filter = StaticFilter;
            dataGridView.RowCount = GeneralBindingSource.Count;
            DynamicFilter = "";
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "crn":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["crn"];
                    break;
                case "raw_address":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["raw_address"];
                    break;
                case "parsed_address":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["parsed_address"];
                    break;
                case "account":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["account"];
                    break;
                case "tenant":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["tenant"];
                    break;
                case "total_area":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["total_area"];
                    break;
                case "living_area":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["living_area"];
                    break;
                case "prescribed":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["prescribed"];
                    break;
                case "date":
                    e.Value = ((DateTime)((DataRowView)GeneralBindingSource[e.RowIndex])["date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "balance_input":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["balance_input"];
                    break;
                case "balance_tenancy":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["balance_tenancy"];
                    break;
                case "balance_dgi":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["balance_dgi"];
                    break;
                case "charging_tenancy":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["charging_tenancy"];
                    break;
                case "charging_dgi":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["charging_dgi"];
                    break;
                case "charging_total":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["charging_total"];
                    break;
                case "recalc_tenancy":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["recalc_tenancy"];
                    break;
                case "recalc_dgi":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["recalc_dgi"];
                    break;
                case "payment_tenancy":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["payment_tenancy"];
                    break;
                case "payment_dgi":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["payment_dgi"];
                    break;
                case "transfer_balance":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["transfer_balance"];
                    break;
                case "balance_output_total":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["balance_output_total"];
                    break;
                case "balance_output_tenancy":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["balance_output_tenancy"];
                    break;
                case "balance_output_dgi":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["balance_output_dgi"];
                    break;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection ==
                             SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        public override bool HasAssocViewport(ViewportType viewportType)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.PaymentsViewport,
                ViewportType.ClaimListViewport
            };
            return reports.Contains(viewportType) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport(ViewportType viewportType)
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран лицевой счет для отображения истории", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_account = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_account"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.PaymentAccount);
        }

        void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || DataGridView.RowCount == 0)
                DataGridView.ClearSelection();
            else
            if (GeneralBindingSource.Position >= DataGridView.RowCount)
            {
                DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                DataGridView.CurrentCell = DataGridView.CurrentCell != null ?
                    DataGridView.Rows[DataGridView.RowCount - 1].Cells[DataGridView.CurrentCell.ColumnIndex] :
                    DataGridView.Rows[DataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                DataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                DataGridView.CurrentCell = DataGridView.CurrentCell != null ?
                    DataGridView.Rows[GeneralBindingSource.Position].Cells[DataGridView.CurrentCell.ColumnIndex] :
                    DataGridView.Rows[GeneralBindingSource.Position].Cells[0];
            }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.EditingStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (GeneralBindingSource != null)
                GeneralBindingSource.CurrentItemChanged -= GeneralBindingSource_CurrentItemChanged;
            base.OnClosing(e);
        }

        internal int GetCurrentId()
        {
            if (GeneralBindingSource.Position < 0) return -1;
            if (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_account"] != DBNull.Value)
                return (int)((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_account"];
            return -1;
        }

        internal string GetFilter()
        {
            return GeneralBindingSource.Filter;
        }
    }
}
