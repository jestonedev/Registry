using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ModalEditors;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ClaimListViewport : FormWithGridViewport
    {
        private BindingSource _vAccounts;


        private SearchForm _spExtendedSearchForm;
        private SearchForm _spSimpleSearchForm;

        private CalcDataModel _lastClaimStates;

        private int? _idAccount;

        private ClaimListViewport()
            : this(null, null)
        {
        }

        public ClaimListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridViewClaims;
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
            }
            else
                if (GeneralBindingSource.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.PaymentAccount))
                        Text = string.Format(CultureInfo.InvariantCulture, "Исковая работа №{0} по ЛС №{1}",
                            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"], ParentRow["account"]);
                    else
                        Text = string.Format(CultureInfo.InvariantCulture, "Исковая работа №{0}", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"]);
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
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerAtDate.DataBindings.Clear();
            dateTimePickerAtDate.DataBindings.Add("Value", GeneralBindingSource, "at_date", true, DataSourceUpdateMode.Never, null);
            dateTimePickerStartDeptPeriod.DataBindings.Clear();
            dateTimePickerStartDeptPeriod.DataBindings.Add("Value", GeneralBindingSource, "start_dept_period", true, DataSourceUpdateMode.Never, null);
            dateTimePickerEndDeptPeriod.DataBindings.Clear();
            dateTimePickerEndDeptPeriod.DataBindings.Add("Value", GeneralBindingSource, "end_dept_period", true, DataSourceUpdateMode.Never, null);
            numericUpDownAmountDGI.DataBindings.Clear();
            numericUpDownAmountDGI.DataBindings.Add("Value", GeneralBindingSource, "amount_dgi", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountTenancy.DataBindings.Clear();
            numericUpDownAmountTenancy.DataBindings.Add("Value", GeneralBindingSource, "amount_tenancy", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountPenalties.DataBindings.Clear();
            numericUpDownAmountPenalties.DataBindings.Add("Value", GeneralBindingSource, "amount_penalties", true, DataSourceUpdateMode.Never, 0);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            if (GeneralBindingSource.Count == 0) return;
            var row = GeneralBindingSource.Position >= 0 ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["at_date"] != DBNull.Value)))
                dateTimePickerAtDate.Checked = true;
            else
            {
                dateTimePickerAtDate.Value = DateTime.Now.Date;
                dateTimePickerAtDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["start_dept_period"] != DBNull.Value)))
                dateTimePickerStartDeptPeriod.Checked = true;
            else
            {
                dateTimePickerStartDeptPeriod.Value = DateTime.Now.Date;
                dateTimePickerStartDeptPeriod.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["end_dept_period"] != DBNull.Value)))
                dateTimePickerEndDeptPeriod.Checked = true;
            else
            {
                dateTimePickerEndDeptPeriod.Value = DateTime.Now.Date;
                dateTimePickerEndDeptPeriod.Checked = false;
            }
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
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return EntityConverter<Claim>.FromRow(row);
        }

        protected override Entity EntityFromViewport()
        {
            var claim = new Claim();
            if ((GeneralBindingSource.Position == -1) || ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"] is DBNull)
                claim.IdClaim = null;
            else
                claim.IdClaim = Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"], CultureInfo.InvariantCulture);
            claim.IdAccount = _idAccount;
            claim.AmountTenancy = numericUpDownAmountTenancy.Value;
            claim.AmountDgi = numericUpDownAmountDGI.Value;
            claim.AmountPenalties = numericUpDownAmountPenalties.Value;
            claim.AtDate = ViewportHelper.ValueOrNull(dateTimePickerAtDate);
            claim.StartDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerStartDeptPeriod);
            claim.EndDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerEndDeptPeriod);
            claim.Description = ViewportHelper.ValueOrNull(textBoxDescription);
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
            DockAreas = DockAreas.Document;
            dataGridViewClaims.AutoGenerateColumns = false;
            GeneralDataModel = DataModel.GetInstance<EntityDataModel<Claim>>();

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            DataModel.GetInstance<PaymentsAccountsDataModel>().Select();

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);
            GeneralBindingSource.DataMember = "claims";
            GeneralBindingSource.DataSource = DataStorage.DataSet;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;

            if (ParentRow != null && ParentType == ParentTypeEnum.PaymentAccount)
            {
                _idAccount = (int?) ParentRow["id_account"];
                textBoxAccount.Enabled = false;
            }
            BindAccount(_idAccount);

            _vAccounts = new BindingSource
            {
                DataSource = DataStorage.DataSet,
                DataMember = "payments_accounts"
            };

            DataBind();

            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", ClaimListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", ClaimListViewport_RowDeleted);

            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridViewClaims);
            IsEditable = true;
            DataChangeHandlersInit();
            _lastClaimStates = CalcDataModel.GetInstance<CalcDataModelLastClaimStates>();
            AddEventHandler<EventArgs>(_lastClaimStates, "RefreshEvent", lastClaimStates_RefreshEvent);
        }

        private void lastClaimStates_RefreshEvent(object sender, EventArgs e)
        {
            dataGridViewClaims.Refresh();
        }

        private void BindAccount(int? idAccount)
        {
            if (ParentRow != null && ParentType == ParentTypeEnum.PaymentAccount)
                textBoxAccount.Text = ParentRow["account"].ToString();
            else
            {
                if (idAccount == null) return;
                textBoxAccount.Text =
                    (from row in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                        where row.Field<int>("id_account") == idAccount.Value
                        select row.Field<string>("account")).FirstOrDefault();
            }
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            GeneralBindingSource.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.PaymentAccount)
            {
                numericUpDownAmountTenancy.Value = ViewportHelper.ValueOrDefault((decimal?)ParentRow["balance_output_tenancy"]);
                numericUpDownAmountDGI.Value = ViewportHelper.ValueOrDefault((decimal?)ParentRow["balance_output_dgi"]);
                numericUpDownAmountPenalties.Value = ViewportHelper.ValueOrDefault((decimal?)ParentRow["balance_output_penalties"]);
            }
            IsEditable = true;
            dataGridViewClaims.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord) 
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var claim = (Claim)EntityFromView();
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            GeneralBindingSource.AddNew();
            dataGridViewClaims.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromClaim(claim);
            dateTimePickerAtDate.Checked = (claim.AtDate != null);
            dateTimePickerStartDeptPeriod.Checked = claim.StartDeptPeriod != null;
            dateTimePickerEndDeptPeriod.Checked = claim.EndDeptPeriod != null;
            IsEditable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_claim"]) == -1)
                return;
            IsEditable = false;
            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
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
                var accounts = (from row in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                                where row.Field<string>("account") == textBoxAccount.Text.Trim()
                                select row.Field<int?>("id_account")).ToList();
                if (accounts.Count == 1)
                    _idAccount = accounts.First();
                else
                if (accounts.Count > 1)
                {
                    using (var form = new SelectAccountForm(accounts))
                    {
                        if (form.ShowDialog() != DialogResult.OK) return;
                        _idAccount = form.IdAccount;
                    }
                }
            }
            var claim = (Claim) EntityFromViewport();
            if (!ValidateClaim(claim))
                return;
            if (((ViewportState == ViewportState.ModifyRowState && ((Claim)EntityFromView()).EndDeptPeriod != claim.EndDeptPeriod) ||
                ViewportState == ViewportState.NewRowState) && claim.EndDeptPeriod != null && claim.IdAccount != null &&
                MessageBox.Show(@"Вы хотите обновить суммы взыскания на предъявленный период?",@"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                var balanceInfoTable = PaymentsAccountsDataModel.GetBalanceInfoOnDate(
                    new List<int> {claim.IdAccount.Value}, claim.EndDeptPeriod.Value.Year,
                    claim.EndDeptPeriod.Value.Month);
                var balanceInfoList = (from row in balanceInfoTable.Select()
                                  where row.Field<int>("id_account") == claim.IdAccount.Value
                    select new
                    {
                        BalanceOutputTenancy = row.Field<decimal>("balance_output_tenancy"),
                        BalanceOutputDgi = row.Field<decimal>("balance_output_dgi"),
                        BalanceOutputPenalties = row.Field<decimal>("balance_output_penalties")
                    }).ToList();
                if (!balanceInfoList.Any())
                {
                    MessageBox.Show(@"На конец указанного периода отсутствуют данные по задолженности", @"Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    var balanceInfo = balanceInfoList.First();
                    claim.AmountTenancy = balanceInfo.BalanceOutputTenancy;
                    claim.AmountDgi = balanceInfo.BalanceOutputDgi;
                    claim.AmountPenalties = balanceInfo.BalanceOutputPenalties;
                }
            }
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    InsertRecord(claim);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    UpdateRecord(claim);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
            dataGridViewClaims.Enabled = true;
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        private void UpdateRecord(Claim claim)
        {
            if (claim.IdClaim == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить запись о претензионно-исковой работе без внутреннего номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (GeneralDataModel.Update(claim) == -1)
                return;
            RebuildFilterAfterSave(GeneralBindingSource, claim.IdClaim);
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            EntityConverter<Claim>.FillRow(claim, row);
        }

        private void InsertRecord(Claim claim)
        {
            var idClaim = GeneralDataModel.Insert(claim);
            if (idClaim == -1)
            {
                return;
            }
            DataRowView newRow;
            claim.IdClaim = idClaim;
            RebuildFilterAfterSave(GeneralBindingSource, claim.IdClaim);
            if (GeneralBindingSource.Position == -1)
                newRow = (DataRowView)GeneralBindingSource.AddNew();
            else
                newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
            EntityConverter<Claim>.FillRow(claim, newRow);

            InsertFirstClaimState(claim.IdClaim);
        }

        private void InsertFirstClaimState(int? idClaim)
        {
            var claimStatesDataModel = DataModel.GetInstance<EntityDataModel<ClaimState>>();
            if (claimStatesDataModel.EditingNewRecord)
            {
                MessageBox.Show(@"Не удалось автоматически вставить первый этап претензионно-исковой работы, т.к. форма состояний исковых работ находится в состоянии добавления новой записи.",
                    @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            var firstStateTypes = ClaimsService.ClaimStartStateTypeIds().ToList();
            if (!firstStateTypes.Any()) return;
            var firstStateType = firstStateTypes.First();
            var claimStatesBindingSource = new BindingSource
            {
                DataSource = claimStatesDataModel.Select()
            };
            var claimState = new ClaimState
            {
                IdClaim = idClaim,
                IdStateType = firstStateType,
                TransferToLegalDepartmentWho = UserDomain.Current.DisplayName,
                AcceptedByLegalDepartmentWho = UserDomain.Current.DisplayName,
                DateStartState = DateTime.Now.Date
            };
            var idState = claimStatesDataModel.Insert(claimState);
            if (idState == -1) return;
            claimState.IdState = idState;
            var claimsStateRow = (DataRowView)claimStatesBindingSource.AddNew();
            if (claimsStateRow != null)
            {
                EntityConverter<ClaimState>.FillRow(claimState, claimsStateRow);
            }
        }

        private static void RebuildFilterAfterSave(IBindingListView bindingSource, int? idClaim)
        {
            var filter = "";
            if (!string.IsNullOrEmpty(bindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            filter += string.Format(CultureInfo.CurrentCulture, "(id_claim = {0})", idClaim);
            bindingSource.Filter += filter;
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
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        IsEditable = false;
                        dataGridViewClaims.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        dataGridViewClaims.RowCount = dataGridViewClaims.RowCount - 1;
                        if (GeneralBindingSource.Position != -1)
                            dataGridViewClaims.Rows[GeneralBindingSource.Position].Selected = true;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    dataGridViewClaims.Enabled = true;
                    IsEditable = false;
                    DataBind();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            UpdateIdAccount();
            BindAccount(_idAccount);
            IsEditable = true;
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
                    if (_spSimpleSearchForm == null)
                        _spSimpleSearchForm = new SimpleSearchClaimsForm();
                    if (_spSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _spSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (_spExtendedSearchForm == null)
                        _spExtendedSearchForm = new ExtendedSearchClaimsForm();
                    if (_spExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _spExtendedSearchForm.GetFilter();
                    break;
            }
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            dataGridViewClaims.RowCount = 0;
            GeneralBindingSource.Filter = filter;
            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
        }

        public override void ClearSearch()
        {
            GeneralBindingSource.Filter = StaticFilter;
            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
            DynamicFilter = "";
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.ClaimStatesViewport,
                ViewportType.PaymentsAccountsViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport<T>()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрана протензионно-исковая работа для отображения ее состояний", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, 
                "id_claim = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row, ParentTypeEnum.Claim);
        }

        private void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (_vAccounts != null) _vAccounts.Filter = "";
            if (GeneralBindingSource.Position == -1 || dataGridViewClaims.RowCount == 0)
                dataGridViewClaims.ClearSelection();
            else
                if (GeneralBindingSource.Position >= dataGridViewClaims.RowCount)
				{
                    dataGridViewClaims.Rows[dataGridViewClaims.RowCount - 1].Selected = true;
					dataGridViewClaims.CurrentCell = dataGridViewClaims.Rows[dataGridViewClaims.RowCount - 1].
						Cells[1];
				}
            else
                    if (dataGridViewClaims.Rows[GeneralBindingSource.Position].Selected != true)
					{
                        dataGridViewClaims.Rows[GeneralBindingSource.Position].Selected = true;
						dataGridViewClaims.CurrentCell = dataGridViewClaims.Rows[GeneralBindingSource.Position].
							Cells[1];
					}
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            UpdateIdAccount();
            BindAccount(_idAccount);
            if (GeneralBindingSource.Position == -1)
                return;
            if (ViewportState == ViewportState.NewRowState)
                return;
            dataGridViewClaims.Enabled = true;
            ViewportState = ViewportState.ReadState;
            IsEditable = true;
        }

        private void UpdateIdAccount()
        {
            if (GeneralBindingSource.Position > -1 && ParentRow == null &&
                ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_account"] != DBNull.Value)
            {
                _idAccount = (int?) ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_account"];
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
            var reporter = ReporterFactory.CreateReporter(reporterType);
            var arguments = new Dictionary<string, string>();
            if (reporterType == ReporterType.ExportReporter)
            {
                arguments = ExportReportArguments();
            }
            reporter.Run(arguments);
        }

        private Dictionary<string, string> ExportReportArguments()
        {
            var columnHeaders = dataGridViewClaims.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnHeader\":\"" + column.HeaderText + "\"}");
            var columnPatterns = dataGridViewClaims.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnPattern\":\"$column" + column.DisplayIndex + "$\"}");
            var arguments = new Dictionary<string, string>
            {
                {"type", "4"},
                {"filter", GeneralBindingSource.Filter.Trim() == "" ? "(1=1)" : GeneralBindingSource.Filter},
                {"columnHeaders", "["+columnHeaders+",{\"columnHeader\":\"Примечание\"}]"},
                {"columnPatterns", "["+columnPatterns+",{\"columnPattern\":\"$description$\"}]"}
            };
            return arguments;
        }

        private void dataGridViewClaims_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewClaims.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridViewClaims.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridViewClaims.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridViewClaims.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridViewClaims.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection ==
                             SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridViewClaims.Refresh();
        }

        private void dataGridViewClaims_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewClaims.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridViewClaims.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        private int _idClaim = int.MinValue;
        private IEnumerable<DataRow> _accountList;

        private void dataGridViewClaims_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            var row = ((DataRowView)GeneralBindingSource[e.RowIndex]);
            switch (dataGridViewClaims.Columns[e.ColumnIndex].Name)
            {
                case "id_claim":
                    e.Value = row["id_claim"];
                    break;
                case "account":
                case "raw_address":
                case "tenant":
                    if (row["id_account"] == DBNull.Value) return;
                    if ((int)row["id_claim"] != _idClaim || _accountList.Any(entry => entry.RowState == DataRowState.Deleted || entry.RowState == DataRowState.Detached))
                    {   
                        _accountList =
                            (from paymentAccountRow in
                                DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                                where paymentAccountRow.Field<int?>("id_account") == (int?) row["id_account"]
                                select paymentAccountRow).ToList();
                        _idClaim = (int)row["id_claim"];
                    }
                    if (_accountList != null && _accountList.Any())
                    {
                        e.Value = _accountList.First().Field<string>(dataGridViewClaims.Columns[e.ColumnIndex].Name);
                    }
                    break;
                case "start_dept_period":
                    e.Value = row["start_dept_period"] == DBNull.Value ? "" :
                        ((DateTime)row["start_dept_period"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "end_dept_period":
                    e.Value = row["end_dept_period"] == DBNull.Value ? "" :
                        ((DateTime)row["end_dept_period"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "amount_tenancy":
                case "amount_dgi":
                case "amount_penalties":
                    e.Value = row[dataGridViewClaims.Columns[e.ColumnIndex].Name];
                    break;
                case "at_date":
                    e.Value = row["at_date"] == DBNull.Value ? "" :
                        ((DateTime)row["at_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "state_type":
                    if (row["id_claim"] == DBNull.Value || row["id_claim"] == null) return;
                    var idClaim = (int?) row["id_claim"];
                    var lastClaimState = _lastClaimStates.Select().Rows.Find(idClaim);
                    if (lastClaimState != null)
                        e.Value = lastClaimState.Field<string>("state_type");
                    break;
                case "date_start_state":
                    if (row["id_claim"] == DBNull.Value || row["id_claim"] == null) return;
                    idClaim = (int?) row["id_claim"];
                    lastClaimState = _lastClaimStates.Select().Rows.Find(idClaim);
                    if (lastClaimState != null && lastClaimState.Field<DateTime?>("date_start_state") != null)
                        e.Value = lastClaimState.Field<DateTime>("date_start_state").ToString("dd.MM.yyyy");
                    break;
            }
        }

        private void ClaimListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
            dataGridViewClaims.Refresh();
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
                dataGridViewClaims.Refresh();
            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
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
            if (GeneralBindingSource.Position < 0) return ids;
            for (var i = 0; i < dataGridViewClaims.SelectedRows.Count; i++)
                if (((DataRowView)GeneralBindingSource[dataGridViewClaims.SelectedRows[i].Index])["id_claim"] != DBNull.Value)
                    ids.Add((int)((DataRowView)GeneralBindingSource[dataGridViewClaims.SelectedRows[i].Index])["id_claim"]);
            return ids;
        }

        internal string GetFilter()
        {
            return GeneralBindingSource.Filter;
        }

        private void textBoxAccount_Leave(object sender, EventArgs e)
        {
            _idAccount = (from row in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                where row.Field<string>("account") == textBoxAccount.Text.Trim()
                select row.Field<int?>("id_account")).LastOrDefault();
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
            numericUpDownAmountTotal.Value =
                numericUpDownAmountTotal.Minimum =
                    numericUpDownAmountTotal.Maximum = numericUpDownAmountDGI.Value + numericUpDownAmountTenancy.Value +
                    numericUpDownAmountPenalties.Value;
        }
    }
}
