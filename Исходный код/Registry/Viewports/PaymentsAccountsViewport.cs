using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.SearchForms;
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
            GeneralDataModel = DataModel.GetInstance<PaymentsAccountsDataModel>();
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
                    case ParentTypeEnum.Claim:
                        ids = new List<int>{(int)ParentRow["id_account"]};
                        title = string.Format("Лицевой счет для исковой работы №{0}", ParentRow["id_claim"]);
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

            foreach (var keyValuePair in new Dictionary<string, NumericUpDown>
            {
                {"total_area",numericUpDownTotalArea},
                {"living_area",numericUpDownLivingArea},
                {"prescribed",numericUpDownPrescribed},
                {"balance_input",numericUpDownBalanceTotalInput},
                {"balance_tenancy",numericUpDownBalanceTenancyInput},
                {"balance_dgi",numericUpDownBalanceDGIInput},
                {"balance_input_penalties",numericUpDownPenaltiesInput},
                {"charging_total",numericUpDownChargingTotal},
                {"charging_tenancy",numericUpDownChargingTenancy},
                {"charging_dgi",numericUpDownChargingDGI},
                {"charging_penalties",numericUpDownChargingPenalties},
                {"recalc_tenancy",numericUpDownRecalcTenancy},
                {"recalc_dgi",numericUpDownRecalcDGI},
                {"recalc_penalties",numericUpDownRecalcPenalties},
                {"payment_tenancy",numericUpDownPaymentTenancy},
                {"payment_dgi",numericUpDownPaymentDGI},
                {"payment_penalties",numericUpDownPaymentPenalties},
                {"transfer_balance",numericUpDownTransferBalance},
                {"balance_output_total",numericUpDownBalanceTotalOutput},
                {"balance_output_tenancy",numericUpDownBalanceTenancyOutput},
                {"balance_output_dgi",numericUpDownBalanceDGIOutput},
                {"balance_output_penalties",numericUpDownPenaltiesOutput}
            })
            {
                keyValuePair.Value.DataBindings.Clear();
                keyValuePair.Value.DataBindings.Add("Minimum", GeneralBindingSource, keyValuePair.Key, true, DataSourceUpdateMode.Never, 0);
                keyValuePair.Value.DataBindings.Add("Maximum", GeneralBindingSource, keyValuePair.Key, true, DataSourceUpdateMode.Never, 0);
                keyValuePair.Value.DataBindings.Add("Value", GeneralBindingSource, keyValuePair.Key, true, DataSourceUpdateMode.Never, 0);
            }

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
                case "date":
                    e.Value = ((DateTime)((DataRowView)GeneralBindingSource[e.RowIndex])["date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                default:
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])[dataGridView.Columns[e.ColumnIndex].Name];
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

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.PaymentsViewport,
                ViewportType.ClaimListViewport,
                ViewportType.PremisesListViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport<T>()
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран лицевой счет для отображения истории", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            
            var filter = "id_account = " +
                         Convert.ToInt32(
                             ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_account"],
                             CultureInfo.InvariantCulture);
            if (typeof(T) == typeof(PremisesListViewport))
            {
                var ids = PaymentsAccountsDataModel.GetPremisesIdsByAccountFilter(filter).ToList();
                if (!ids.Any())
                {
                    MessageBox.Show(@"К данному лицевому счету не привязано ни одного объекта недвижимости", @"Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                filter = string.Format("id_premises IN (0{0})", ids.Select(id => id.ToString()).Aggregate((x,y) => x + "," + y));
                ShowAssocViewport<PremisesViewport>(MenuCallback, 
                filter,
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.PaymentAccount);
            }
            else
            {
                ShowAssocViewport<T>(MenuCallback,
                filter,
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.PaymentAccount);
            }
            
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

        internal IEnumerable<int> GetCurrentIds()
        {
            var ids = new List<int>();
            if (GeneralBindingSource.Position < 0) return ids;
            for (var i = 0; i < dataGridView.SelectedRows.Count; i++)
                if (((DataRowView)GeneralBindingSource[dataGridView.SelectedRows[i].Index])["id_account"] != DBNull.Value)
                ids.Add((int)((DataRowView)GeneralBindingSource[dataGridView.SelectedRows[i].Index])["id_account"]);
            return ids;
        }

        internal string GetFilter()
        {
            return GeneralBindingSource.Filter;
        }
    }
}
