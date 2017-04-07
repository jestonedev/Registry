using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Registry.DataModels.Services;
using Registry.DataModels.Services.ServiceModels;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.Presenters;
using Registry.Viewport.SearchForms;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal partial class PaymentsAccountsViewport : FormWithGridViewport
    {

        private PaymentsAccountsViewport()
            : this(null, null)
        {
        }

        public PaymentsAccountsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new PaymentsAccountsPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            ViewportHelper.SetDoubleBuffered(DataGridView);
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;
            Presenter.ParentRow = ParentRow;
            Presenter.ParentType = ParentType;

            StaticFilter = ((PaymentsAccountsPresenter)Presenter).GetStaticFilter(StaticFilter);

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            var title = "Лицевые счета";
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Premises:
                        title = string.Format("Лицевые счета помещение №{0}", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        title = string.Format("Лицевые счета комнаты {0} помещения №{1}", ParentRow["sub_premises_num"], ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.Claim:
                        title = string.Format("Лицевые счета исковой работы №{0}", ParentRow["id_claim"]);
                        break;
                }
            }
            Text = title;

            if (ParentType == ParentTypeEnum.Claim)
            {
                Presenter.ViewModel["general"].BindingSource.Sort = "date";
                date.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            }

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            DataBind();
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());

            if (DataGridView.RowCount > 0)
                DataGridView.Rows[0].Selected = true;
            CanUpdateNotCompletedClaims = true;
            UpdateNotCompletedClaims();

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["claims"].DataSource, "RowDeleted",
                (s, e) => UpdateNotCompletedClaims());
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["claims"].DataSource, "RowChanged",
                (s, e) => UpdateNotCompletedClaims());
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["claim_states"].DataSource, "RowDeleted",
               (s, e) => UpdateNotCompletedClaims());
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["claim_states"].DataSource, "RowChanged",
                (s, e) => UpdateNotCompletedClaims());
        }

        private IEnumerable<ClaimPaymentAccountInfo> _notCompletedClaims;
        public bool CanUpdateNotCompletedClaims { get; set; }

        public void UpdateNotCompletedClaims()
        {
            if (!CanUpdateNotCompletedClaims)
            {
                return;
            }
            _notCompletedClaims = ClaimsService.NotCompletedClaimsPaymentAccountsInfo().ToList();
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindProperty(textBoxCRN, "Text", bindingSource, "crn", "");
            ViewportHelper.BindProperty(textBoxRawAddress, "Text", bindingSource, "raw_address", "");
            ViewportHelper.BindProperty(textBoxAddress, "Text", bindingSource, "parsed_address", "");
            ViewportHelper.BindProperty(textBoxAccount, "Text", bindingSource, "account", "");
            ViewportHelper.BindProperty(textBoxTenant, "Text", bindingSource, "tenant", "");

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
                keyValuePair.Value.DataBindings.Add("Value", bindingSource, keyValuePair.Key, true, DataSourceUpdateMode.Never, 0m);
            }

            ViewportHelper.BindProperty(dateTimePickerAtDate, "Value", bindingSource, "date", DateTime.Now.Date);
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
                    if (Presenter.SimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.SimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (Presenter.ExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.ExtendedSearchForm.GetFilter();
                    break;
            }
            DataGridView.RowCount = 0;
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
        }

        public override void ClearSearch()
        {
            Presenter.ViewModel["general"].BindingSource.Filter = StaticFilter;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DynamicFilter = "";
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (Presenter.ViewModel["general"].BindingSource.Count <= e.RowIndex) return;
            var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[e.RowIndex];
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "date":
                    var hasDuplicate = _notCompletedClaims.Any(r => row["account"] != DBNull.Value && r.Account == (string)row["account"]) ||
                        _notCompletedClaims.Any(r => row["raw_address"] != DBNull.Value && r.RawAddress == (string)row["raw_address"]) ||
                        _notCompletedClaims.Any(r => row["parsed_address"] != DBNull.Value && r.ParsedAddress == (string)row["parsed_address"]);
                    if (hasDuplicate)
                    {
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(255, 117, 234, 232);
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.FromArgb(255, 22, 145, 143);
                    }
                    else
                    {
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = SystemColors.Highlight;
                    }

                    DateTime dateValue;
                    if (DateTime.TryParse(row[DataGridView.Columns[e.ColumnIndex].Name].ToString(), out dateValue))
                    {
                        e.Value = dateValue.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    }
                    break;
                case "charging_date":
                    DateTime chargingDateValue;
                    if (DateTime.TryParse(row[DataGridView.Columns[e.ColumnIndex].Name].ToString(), out chargingDateValue))
                    {
                        e.Value = chargingDateValue.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    }
                    break;
                default:
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
            }
            if (ParentType == ParentTypeEnum.Claim && (int)ParentRow["id_account"] == (int)row["id_account"])
            {
                DataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                DataGridView.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.Green;
            }
            else
            {
                DataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                DataGridView.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView_SelectionChanged();
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView_ColumnHeaderMouseClick(sender, e);
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.PaymentsAccountHistoryViewport,
                ViewportType.PaymentsPremiseHistoryViewport,
                ViewportType.ClaimListViewport,
                ViewportType.PremisesListViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (Presenter.ViewModel["general"].CurrentRow != null);
        }

        public override void ShowAssocViewport<T>()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран лицевой счет для отображения истории", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            
            var filter = "id_account = " + Convert.ToInt32(row["id_account"], CultureInfo.InvariantCulture);
            if (typeof(T) == typeof(PremisesListViewport))
            {
                var ids = PaymentService.GetPremisesIdsByAccountFilter(filter).ToList();
                if (!ids.Any())
                {
                    MessageBox.Show(@"К данному лицевому счету не привязано ни одного объекта недвижимости", @"Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                filter = string.Format("id_premises IN (0{0})", ids.Select(id => id.ToString()).Aggregate((x,y) => x + "," + y));
                ShowAssocViewport<PremisesViewport>(MenuCallback, filter, row.Row, ParentTypeEnum.PaymentAccount);
            }
            else
            {
                ShowAssocViewport<T>(MenuCallback, filter, row.Row, ParentTypeEnum.PaymentAccount);
            }
            
        }

        internal IEnumerable<int> GetCurrentIds()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            var ids = new List<int>();
            if (bindingSource.Position < 0) return ids;
            for (var i = 0; i < DataGridView.SelectedRows.Count; i++)
                if (((DataRowView)bindingSource[DataGridView.SelectedRows[i].Index])["id_account"] != DBNull.Value)
                    ids.Add((int)((DataRowView)bindingSource[DataGridView.SelectedRows[i].Index])["id_account"]);
            return ids;
        }

        internal string GetFilter()
        {
            return Presenter.ViewModel["general"].BindingSource.Filter;
        }

        public override bool HasReport(ReporterType reporterType)
        {
            var reports = new List<ReporterType>
            {
                ReporterType.ExportReporter,
                ReporterType.RequestToBksReporter
            };
            return reports.Contains(reporterType);
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            var arguments = new Dictionary<string, string>();
            if (reporterType == ReporterType.ExportReporter)
            {
                arguments = ExportReportArguments();
            }
            if (reporterType == ReporterType.RequestToBksReporter)
            {
                var ids = GetCurrentIds().ToList();
                var filter = ids.Select(r => r.ToString()).Aggregate((acc, v) => acc + "," + v);
                arguments.Add("filter", filter);
            }
            MenuCallback.RunReport(reporterType, arguments);
        }

        private Dictionary<string, string> ExportReportArguments()
        {
            var columnHeaders = DataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnHeader\":\"" + column.HeaderText + "\"}");
            var columnPatterns = DataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnPattern\":\"$column" + column.DisplayIndex + "$\"}");
            var filter = Presenter.ViewModel["general"].BindingSource.Filter ?? "";
            var arguments = new Dictionary<string, string>
            {
                {"type", "5"},
                {"filter", filter.Trim() == "" ? "(1=1)" : filter },
                {"columnHeaders", "["+columnHeaders+"]"},
                {"columnPatterns", "["+columnPatterns+"]"}
            };
            return arguments;
        }
    }
}
