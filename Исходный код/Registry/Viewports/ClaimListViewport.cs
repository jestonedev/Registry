using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ModalEditors;
using Registry.Viewport.Presenters;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ClaimListViewport : FormWithGridViewport
    {
        private int? _idAccount;

        private ClaimListViewport()
            : this(null, null)
        {
        }

        public ClaimListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new ClaimListPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridViewClaims;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            ViewportHelper.SetDoubleBuffered(DataGridView);          
        }

        private void SetViewportCaption()
        {
            if (ViewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.PaymentAccount))
                {
                    Text = string.Format(CultureInfo.InvariantCulture, "Новая исковая работа по ЛС №{0}", ParentRow["account"]);
                }
                else
                    Text = @"Новая исковая работа";
                return;
            }
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row != null)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.PaymentAccount))
                    Text = string.Format(CultureInfo.InvariantCulture, "Исковая работа №{0} по ЛС №{1}",
                        row["id_claim"], ParentRow["account"]);
                else
                    Text = string.Format(CultureInfo.InvariantCulture, "Исковая работа №{0}", row["id_claim"]);
            }
            else
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.PaymentAccount))
                    Text = string.Format(CultureInfo.InvariantCulture, "Исковые работы по ЛС №{0} отсутствуют", ParentRow["account"]);
                else
                    Text = @"Исковые работы отсутствуют";
            }
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindProperty(textBoxDescription, "Text", bindingSource, "description", "");
            ViewportHelper.BindProperty(dateTimePickerAtDate, "Value", bindingSource, "at_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(dateTimePickerStartDeptPeriod, "Value", bindingSource, "start_dept_period", DateTime.Now.Date);
            ViewportHelper.BindProperty(dateTimePickerEndDeptPeriod, "Value", bindingSource, "end_dept_period", DateTime.Now.Date);
            ViewportHelper.BindProperty(numericUpDownAmountDGI, "Value", bindingSource, "amount_dgi", 0m);
            ViewportHelper.BindProperty(numericUpDownAmountTenancy, "Value", bindingSource, "amount_tenancy", 0m);
            ViewportHelper.BindProperty(numericUpDownAmountPenalties, "Value", bindingSource, "amount_penalties", 0m);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            IsEditable = false;
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row != null && (row["at_date"] != DBNull.Value))
                dateTimePickerAtDate.Checked = true;
            else
            {
                dateTimePickerAtDate.Value = DateTime.Now.Date;
                dateTimePickerAtDate.Checked = false;
            }
            if (row != null && (row["start_dept_period"] != DBNull.Value))
                dateTimePickerStartDeptPeriod.Checked = true;
            else
            {
                dateTimePickerStartDeptPeriod.Value = DateTime.Now.Date;
                dateTimePickerStartDeptPeriod.Checked = false;
            }
            if (row != null && (row["end_dept_period"] != DBNull.Value))
                dateTimePickerEndDeptPeriod.Checked = true;
            else
            {
                dateTimePickerEndDeptPeriod.Value = DateTime.Now.Date;
                dateTimePickerEndDeptPeriod.Checked = false;
            }
            IsEditable = true;
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.ClaimsWrite))
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidateClaim(Claim claim)
        {
            if (claim.IdAccount == null)
            {
                MessageBox.Show(@"Лицевого счета с указанным номером не существует",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxAccount.Focus();
                return false;
            }
            return true;
        }

        protected override Entity EntityFromView()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return row == null ? new Claim() : EntityConverter<Claim>.FromRow(row);
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var claim = new Claim
            {
                IdClaim = row == null || row["id_claim"] is DBNull
                    ? (int?) null
                    : Convert.ToInt32(row["id_claim"], CultureInfo.InvariantCulture),
                IdAccount = _idAccount,
                AmountTenancy = numericUpDownAmountTenancy.Value,
                AmountDgi = numericUpDownAmountDGI.Value,
                AmountPenalties = numericUpDownAmountPenalties.Value,
                AtDate = ViewportHelper.ValueOrNull(dateTimePickerAtDate),
                StartDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerStartDeptPeriod),
                EndDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerEndDeptPeriod),
                Description = ViewportHelper.ValueOrNull(textBoxDescription)
            };
            return claim;
        }

        private void ViewportFromClaim(Claim claim)
        {
            numericUpDownAmountDGI.Value = ViewportHelper.ValueOrDefault(claim.AmountDgi);
            numericUpDownAmountPenalties.Value = ViewportHelper.ValueOrDefault(claim.AmountPenalties);
            numericUpDownAmountTenancy.Value = ViewportHelper.ValueOrDefault(claim.AmountTenancy);
            dateTimePickerAtDate.Value = ViewportHelper.ValueOrDefault(claim.AtDate);
            dateTimePickerStartDeptPeriod.Value = ViewportHelper.ValueOrDefault(claim.StartDeptPeriod);
            dateTimePickerEndDeptPeriod.Value = ViewportHelper.ValueOrDefault(claim.EndDeptPeriod);
            textBoxDescription.Text = claim.Description;
            _idAccount = claim.IdAccount;
            BindAccount(claim.IdAccount);
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
            StaticFilter = ((ClaimListPresenter)Presenter).GetStaticFilter();

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            SetViewportCaption();
            
            if (ParentRow != null && ParentType == ParentTypeEnum.PaymentAccount)
            {
                _idAccount = (int?) ParentRow["id_account"];
                textBoxAccount.Enabled = false;
            }
            BindAccount(_idAccount);

            DataBind();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", 
                GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, 
                "RowChanged", ClaimListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, 
                "RowDeleted", ClaimListViewport_RowDeleted);

            AddEventHandler<EventArgs>(Presenter.ViewModel["last_claim_states"].Model, 
                "RefreshEvent", lastClaimStates_RefreshEvent);

            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());

            DataChangeHandlersInit();
            IsEditable = true;
        }

        private void lastClaimStates_RefreshEvent(object sender, EventArgs e)
        {
            DataGridView.Refresh();
        }

        private void BindAccount(int? idAccount)
        {
            IsEditable = false;
            if (ParentRow != null && ParentType == ParentTypeEnum.PaymentAccount)
                textBoxAccount.Text = ParentRow["account"].ToString();
            else
            {
                textBoxAccount.Text = ((ClaimListPresenter)Presenter).AccountById(idAccount);
            }
            IsEditable = true;
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            DataGridView.RowCount = DataGridView.RowCount + 1;
            DataGridView.Enabled = false;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.PaymentAccount)
            {
                numericUpDownAmountTenancy.Value = ViewportHelper.ValueOrDefault((decimal?)ParentRow["balance_output_tenancy"]);
                numericUpDownAmountDGI.Value = ViewportHelper.ValueOrDefault((decimal?)ParentRow["balance_output_dgi"]);
                numericUpDownAmountPenalties.Value = ViewportHelper.ValueOrDefault((decimal?)ParentRow["balance_output_penalties"]);
            }
            IsEditable = true;
        }

        public override bool CanCopyRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) 
                && !Presenter.ViewModel["general"].Model.EditingNewRecord 
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var claim = (Claim)EntityFromView();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            DataGridView.RowCount = DataGridView.RowCount + 1;
            DataGridView.Enabled = false;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            ViewportFromClaim(claim);
            dateTimePickerAtDate.Checked = claim.AtDate != null;
            dateTimePickerStartDeptPeriod.Checked = claim.StartDeptPeriod != null;
            dateTimePickerEndDeptPeriod.Checked = claim.EndDeptPeriod != null;
            IsEditable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) 
                && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            IsEditable = false;
            var selectedIdClaims  = new List<int>();
            foreach (DataGridViewRow row in dataGridViewClaims.SelectedRows)
            {
                if (row.Cells["id_claim"].Value != null)
                {
                    selectedIdClaims.Add((int) row.Cells["id_claim"].Value);
                }
            }

            if (!((ClaimListPresenter)Presenter).DeleteRecords(selectedIdClaims))
            {
                IsEditable = true;
                return;
            }
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void SaveRecord()
        {
            if (textBoxAccount.Enabled)
            {
                var accounts = ((ClaimListPresenter) Presenter).AccountIdsByAccount(textBoxAccount.Text.Trim());
                if (accounts.Count == 1)
                    _idAccount = accounts.First();
                else
                if (accounts.Count > 1)
                {
                    using (var form = new SelectAccountForm(accounts, _idAccount))
                    {
                        if (form.ShowDialog() != DialogResult.OK) return;
                        _idAccount = form.IdAccount;
                    }
                }
            }
            var claim = (Claim) EntityFromViewport();
            if (!ValidateClaim(claim))
                return;
            if (!UpdateBalance(claim))
                return;
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((ClaimListPresenter)Presenter).InsertRecord(claim))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((ClaimListPresenter)Presenter).UpdateRecord(claim))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DataGridView.Enabled = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        private bool UpdateBalance(Claim claim)
        {
            if ((ViewportState != ViewportState.ModifyRowState ||
                 ((Claim) EntityFromView()).EndDeptPeriod == claim.EndDeptPeriod) &&
                ViewportState != ViewportState.NewRowState) return true;
            if (claim.EndDeptPeriod == null) return true;
            if (claim.IdAccount == null) return true;
            if (MessageBox.Show(@"Вы хотите обновить суммы взыскания на предъявленный период?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) !=
                DialogResult.Yes) return true;

            var accounts = ((ClaimListPresenter)Presenter).SameAccountIdsByAccountId(_idAccount);
            var idAccountFrom = (int?) null;
            if (accounts.Count == 1)
                idAccountFrom = accounts.First();
            else if (accounts.Count > 1)
            {
                using (var form = new SelectAccountForm(accounts, claim.IdAccount))
                {
                    if (form.ShowDialog() != DialogResult.OK) return false;
                    idAccountFrom = form.IdAccount;
                }
            }
            ((ClaimListPresenter)Presenter).UpdateBalance(claim, idAccountFrom);
            return true;
        }

        public override bool CanCancelRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    Presenter.ViewModel["general"].Model.EditingNewRecord = false;
                    var row = Presenter.ViewModel["general"].CurrentRow;
                    if (row != null)
                    {
                        IsEditable = false;
                        row.Delete();
                        DataGridView.RowCount = DataGridView.RowCount - 1;
                        if (Presenter.ViewModel["general"].CurrentRow != null)
                            DataGridView.Rows[Presenter.ViewModel["general"].BindingSource.Position].Selected = true;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            UpdateIdAccount();
            BindAccount(_idAccount);
            IsEditable = true;
            DataGridView.Enabled = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
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
            DataGridView.RowCount = 0;
            Presenter.ViewModel["general"].BindingSource.Filter = StaticFilter;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DynamicFilter = "";
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.ClaimStatesViewport,
                ViewportType.PaymentsAccountsViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (Presenter.ViewModel["general"].CurrentRow != null);
        }

        public override void ShowAssocViewport<T>()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var viewModel = Presenter.ViewModel["general"];
            if (viewModel.CurrentRow == null)
            {
                MessageBox.Show(@"ННе выбрана протензионно-исковая работа для отображения ее состояний", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, viewModel.PrimaryKeyFirst + " = " +
                Convert.ToInt32(viewModel.CurrentRow[viewModel.PrimaryKeyFirst], CultureInfo.InvariantCulture),
                viewModel.CurrentRow.Row, ParentTypeEnum.Claim);
        }

        public override bool CanOpenDetails()
        {
            return Presenter.ViewModel["general"].CurrentRow != null;
        }

        public override void OpenDetails()
        {
            ShowAssocViewport<ClaimStatesViewport>();
        }

        protected override void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (Presenter.ViewModel["payments_accounts"].BindingSource != null) 
                Presenter.ViewModel["payments_accounts"].BindingSource.Filter = "";
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            if (Presenter.ViewModel["general"].CurrentRow == null || DataGridView.RowCount == 0)
                DataGridView.ClearSelection();
            else if (bindingSource.Position >= DataGridView.RowCount)
            {
                DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                DataGridView.CurrentCell = DataGridView.Rows[DataGridView.RowCount - 1].Cells[1];
            }
            else if (DataGridView.Rows[bindingSource.Position].Selected != true)
            {
                DataGridView.Rows[bindingSource.Position].Selected = true;
                DataGridView.CurrentCell = DataGridView.Rows[bindingSource.Position].Cells[1];
            }

            var isEditable = IsEditable;
            UnbindedCheckBoxesUpdate();
            UpdateIdAccount();
            BindAccount(_idAccount);
            IsEditable = isEditable;

            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void UpdateIdAccount()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row != null && ParentRow == null && row["id_account"] != DBNull.Value)
            {
                _idAccount = (int?) row["id_account"];
            }
            else
            {
                if (ParentRow != null && ParentType == ParentTypeEnum.PaymentAccount)
                    _idAccount = (int?)ParentRow["id_account"];
                else
                    _idAccount = null;
            }
        }

        public override bool HasReport(ReporterType reporterType)
        {
            var reports = new List<ReporterType>
            {
                ReporterType.ExportReporter
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
                {"type", "4"},
                {"filter", filter.Trim() == "" ? "(1=1)" : filter },
                {"columnHeaders", "["+columnHeaders+",{\"columnHeader\":\"Примечание\"}]"},
                {"columnPatterns", "["+columnPatterns+",{\"columnPattern\":\"$description$\"}]"}
            };
            return arguments;
        }

        private void dataGridViewClaims_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in DataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                Presenter.ViewModel["general"].BindingSource.Sort = 
                    DataGridView.Columns[e.ColumnIndex].Name + " " + (way == SortOrder.Ascending ? "ASC" : "DESC");
                DataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(DataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection ==
                             SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            DataGridView.Refresh();
        }

        private void dataGridViewClaims_SelectionChanged(object sender, EventArgs e)
        {
            if (DataGridView.SelectedRows.Count > 0)
                Presenter.ViewModel["general"].BindingSource.Position = DataGridView.SelectedRows[0].Index;
            else
                Presenter.ViewModel["general"].BindingSource.Position = -1;
        }

        private int _idClaim = int.MinValue;
        private IEnumerable<DataRow> _accountList;

        private void dataGridViewClaims_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (Presenter.ViewModel["general"].BindingSource.Count <= e.RowIndex) return;
            var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[e.RowIndex];
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_claim":
                case "amount_tenancy":
                case "amount_dgi":
                case "amount_penalties":
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "start_dept_period":
                case "end_dept_period":
                case "at_date":
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name] == DBNull.Value ? "" :
                        ((DateTime)row[DataGridView.Columns[e.ColumnIndex].Name]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "account":
                case "raw_address":
                case "tenant":
                case "balance_output_tenancy":
                case "balance_output_dgi":
                case "balance_output_penalties":
                    if (row["id_account"] == DBNull.Value) return;
                    if ((int)row["id_claim"] != _idClaim || 
                        _accountList.Any(entry => entry.RowState == DataRowState.Deleted || entry.RowState == DataRowState.Detached))
                    {
                        _accountList = ((ClaimListPresenter) Presenter).AccountRowsById((int?) row["id_account"]);
                        _idClaim = (int)row["id_claim"];
                    }
                    if (_accountList != null && _accountList.Any())
                    {
                        e.Value = _accountList.First().Field<object>(DataGridView.Columns[e.ColumnIndex].Name);
                    }
                    break;
                case "state_type":
                    if (row["id_claim"] == DBNull.Value || row["id_claim"] == null) return;
                    var idClaim = (int?) row["id_claim"];
                    var lastClaimState = Presenter.ViewModel["last_claim_states"].DataSource.Rows.Find(idClaim);
                    if (lastClaimState != null)
                        e.Value = lastClaimState.Field<string>("state_type");
                    break;
                case "date_start_state":
                    if (row["id_claim"] == DBNull.Value || row["id_claim"] == null) return;
                    idClaim = (int?) row["id_claim"];
                    lastClaimState = Presenter.ViewModel["last_claim_states"].DataSource.Rows.Find(idClaim);
                    if (lastClaimState != null && lastClaimState.Field<DateTime?>("date_start_state") != null)
                        e.Value = lastClaimState.Field<DateTime>("date_start_state").ToString("dd.MM.yyyy");
                    break;
            }
            if (ParentType == ParentTypeEnum.PaymentAccount && row["id_account"] != DBNull.Value && 
                (int)ParentRow["id_account"] == (int)row["id_account"])
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

        private void ClaimListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DataGridView.Invalidate();
            UnbindedCheckBoxesUpdate();
            UpdateIdAccount();
            BindAccount(_idAccount);
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void ClaimListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                DataGridView.Invalidate();
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            UnbindedCheckBoxesUpdate();
            UpdateIdAccount();
            BindAccount(_idAccount);
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            base.OnVisibleChanged(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return keyData != Keys.Enter && base.ProcessCmdKey(ref msg, keyData);
        }

        internal IEnumerable<int> GetCurrentIds()
        {
            var ids = new List<int>();
            if (Presenter.ViewModel["general"].BindingSource.Position < 0) return ids;
            for (var i = 0; i < DataGridView.SelectedRows.Count; i++)
            {
                var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[DataGridView.SelectedRows[i].Index];
                if (row["id_claim"] != DBNull.Value)
                    ids.Add((int) row["id_claim"]);
            }
            return ids;
        }

        internal string GetFilter()
        {
            return Presenter.ViewModel["general"].BindingSource.Filter;
        }

        private void textBoxAccount_Leave(object sender, EventArgs e)
        {
            _idAccount = ((ClaimListPresenter) Presenter).AccountIdsByAccount(textBoxAccount.Text.Trim()).LastOrDefault();
            BindAccount(_idAccount);
            CheckViewportModifications();
        }

        private void buttonShowAttachments_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var ids = GetCurrentIds().ToList();
            if (!ids.Any())
            {
                MessageBox.Show(@"Не выбрана исковая работа для отображения прикрепленных файлов", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var form = new ClaimFiles())
            {
                form.Initialize(ids.First());
                form.ShowDialog();
            }
        }

        private void numericUpDownAmountTenancy_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDownAmountTotalChange();
        }

        private void numericUpDownAmountDGI_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDownAmountTotalChange();
        }

        private void numericUpDownAmountPenalties_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDownAmountTotalChange();
        }

        private void NumericUpDownAmountTotalChange()
        {
            numericUpDownAmountTotal.Minimum = decimal.MinValue;
            numericUpDownAmountTotal.Maximum = decimal.MaxValue;
            numericUpDownAmountTotal.Value = numericUpDownAmountDGI.Value + numericUpDownAmountTenancy.Value +
                    numericUpDownAmountPenalties.Value;
        }

        private void dataGridViewClaims_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowAssocViewport<ClaimStatesViewport>();
        }
    }
}
