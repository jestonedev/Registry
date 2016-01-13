using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ClaimListViewport : FormWithGridViewport
    {
        private BindingSource v_accounts;


        private SearchForm spExtendedSearchForm;
        private SearchForm spSimpleSearchForm;

        private CalcDataModel lastClaimStates;

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
            if (viewportState == ViewportState.NewRowState)
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
            viewportState = ViewportState.ReadState;
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


            var claim = new Claim
            {
                IdClaim = ViewportHelper.ValueOrNull<int>(row, "id_claim"),
                IdAccount = ViewportHelper.ValueOrNull<int>(row, "id_account"),
                AmountTenancy = ViewportHelper.ValueOrNull<decimal>(row, "amount_tenancy"),
                AmountDgi = ViewportHelper.ValueOrNull<decimal>(row, "amount_dgi"),
                AtDate = ViewportHelper.ValueOrNull<DateTime>(row, "at_date"),
                StartDeptPeriod = ViewportHelper.ValueOrNull<DateTime>(row, "start_dept_period"),
                EndDeptPeriod = ViewportHelper.ValueOrNull<DateTime>(row, "end_dept_period"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
            return claim;
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
            claim.AtDate = ViewportHelper.ValueOrNull(dateTimePickerAtDate);
            claim.StartDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerStartDeptPeriod);
            claim.EndDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerEndDeptPeriod);
            claim.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            return claim;
        }

        private void ViewportFromClaim(Claim claim)
        {
            numericUpDownAmountDGI.Value = ViewportHelper.ValueOrDefault(claim.AmountDgi);
            numericUpDownAmountTenancy.Value = ViewportHelper.ValueOrDefault(claim.AmountTenancy);
            dateTimePickerAtDate.Value = ViewportHelper.ValueOrDefault(claim.AtDate);
            dateTimePickerStartDeptPeriod.Value = ViewportHelper.ValueOrDefault(claim.StartDeptPeriod);
            dateTimePickerEndDeptPeriod.Value = ViewportHelper.ValueOrDefault(claim.EndDeptPeriod);
            textBoxDescription.Text = claim.Description;
            _idAccount = claim.IdAccount;
            BindAccount(claim.IdAccount);
        }

        private static void FillRowFromClaim(Claim claim, DataRowView row)
        {
            row.BeginEdit();
            row["id_claim"] = ViewportHelper.ValueOrDBNull(claim.IdClaim);
            row["id_account"] = ViewportHelper.ValueOrDBNull(claim.IdAccount);
            row["at_date"] = ViewportHelper.ValueOrDBNull(claim.AtDate);
            row["start_dept_period"] = ViewportHelper.ValueOrDBNull(claim.StartDeptPeriod);
            row["end_dept_period"] = ViewportHelper.ValueOrDBNull(claim.EndDeptPeriod);
            row["amount_tenancy"] = ViewportHelper.ValueOrDBNull(claim.AmountTenancy);
            row["amount_dgi"] = ViewportHelper.ValueOrDBNull(claim.AmountDgi);
            row["description"] = ViewportHelper.ValueOrDBNull(claim.Description);
            row.EndEdit();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            dataGridViewClaims.AutoGenerateColumns = false;
            GeneralDataModel = DataModel.GetInstance(DataModelType.ClaimsDataModel);

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            DataModel.GetInstance(DataModelType.PaymentsAccountsDataModel).Select();

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataMember = "claims";
            GeneralBindingSource.DataSource = DataModel.DataSet;
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

            v_accounts = new BindingSource
            {
                DataSource = DataModel.DataSet,
                DataMember = "payments_accounts"
            };

            DataBind();

            GeneralDataModel.Select().RowChanged += ClaimListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += ClaimListViewport_RowDeleted;

            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridViewClaims);
            is_editable = true;
            DataChangeHandlersInit();
            lastClaimStates = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelLastClaimStates);
            lastClaimStates.RefreshEvent += lastClaimStates_RefreshEvent;
        }

        void lastClaimStates_RefreshEvent(object sender, EventArgs e)
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
                    (from row in DataModel.GetInstance(DataModelType.PaymentsAccountsDataModel).FilterDeletedRows()
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
            is_editable = false;
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            GeneralBindingSource.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.PaymentAccount)
            {
                numericUpDownAmountTenancy.Value = ViewportHelper.ValueOrDefault((decimal?)ParentRow["balance_output_tenancy"]);
                numericUpDownAmountDGI.Value = ViewportHelper.ValueOrDefault((decimal?)ParentRow["balance_output_dgi"]);
            }
            is_editable = true;
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
            is_editable = false;
            var claim = (Claim)EntityFromView();
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            GeneralBindingSource.AddNew();
            dataGridViewClaims.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromClaim(claim);
            dateTimePickerAtDate.Checked = (claim.AtDate != null);
            dateTimePickerStartDeptPeriod.Checked = claim.StartDeptPeriod != null;
            dateTimePickerEndDeptPeriod.Checked = claim.EndDeptPeriod != null;
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_claim"]) == -1)
                return;
            is_editable = false;
            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void SaveRecord()
        {
            _idAccount = (from row in DataModel.GetInstance(DataModelType.PaymentsAccountsDataModel).FilterDeletedRows()
                          where row.Field<string>("account") == textBoxAccount.Text.Trim()
                          select row.Field<int?>("id_account")).FirstOrDefault();

            var claim = (Claim) EntityFromViewport();
            if (!ValidateClaim(claim))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idClaim = GeneralDataModel.Insert(claim);
                    if (idClaim == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    claim.IdClaim = idClaim;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    FillRowFromClaim(claim, newRow);
                    // Add first state automaticaly
                    if (DataModel.GetInstance(DataModelType.ClaimStatesDataModel).EditingNewRecord)
                    {
                        MessageBox.Show(@"Не удалось автоматически вставить первый этап претензионно-исковой работы, т.к. форма состояний исковых работ находится в состоянии добавления новой записи.",
                            @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        GeneralDataModel.EditingNewRecord = false;
                        break;
                    }
                    var firstStateTypes = DataModelHelper.ClaimStartStateTypeIds().ToList();
                    if (firstStateTypes.Any())
                    {
                        var firstStateType = firstStateTypes.First();
                        var claimStatesDataModel = DataModel.GetInstance(DataModelType.ClaimStatesDataModel);
                        var claimStatesBindingSource = new BindingSource
                        {
                            DataSource = claimStatesDataModel.Select()
                        };
                        var claimState = new ClaimState
                        {
                            IdClaim = claim.IdClaim,
                            IdStateType = firstStateType,
                            TransferToLegalDepartmentWho = UserDomain.Current.DisplayName,
                            AcceptedByLegalDepartmentWho = UserDomain.Current.DisplayName,
                            DateStartState = DateTime.Now.Date
                        };
                        var idState = claimStatesDataModel.Insert(claimState);
                        if (idState != -1)
                        {
                            claimState.IdState = idState;
                            var claimsStateRow = (DataRowView)claimStatesBindingSource.AddNew();
                            if (claimsStateRow != null)
                            {
                                claimsStateRow.BeginEdit();
                                claimsStateRow["id_state"] = ViewportHelper.ValueOrDBNull(claimState.IdState);
                                claimsStateRow["id_claim"] = ViewportHelper.ValueOrDBNull(claimState.IdClaim);
                                claimsStateRow["id_state_type"] = ViewportHelper.ValueOrDBNull(claimState.IdStateType);
                                claimsStateRow["transfer_to_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.TransferToLegalDepartmentWho);
                                claimsStateRow["accepted_by_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.AcceptedByLegalDepartmentWho);
                                claimsStateRow["date_start_state"] = ViewportHelper.ValueOrDBNull(claimState.DateStartState);
                                claimsStateRow.EndEdit();
                            }
                        }                     
                    }
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (claim.IdClaim == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о претензионно-исковой работе без внутреннего номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(claim) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    is_editable = false;
                    FillRowFromClaim(claim, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            dataGridViewClaims.Enabled = true;
            is_editable = true;
            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        is_editable = false;
                        dataGridViewClaims.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        dataGridViewClaims.RowCount = dataGridViewClaims.RowCount - 1;
                        if (GeneralBindingSource.Position != -1)
                            dataGridViewClaims.Rows[GeneralBindingSource.Position].Selected = true;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    dataGridViewClaims.Enabled = true;
                    is_editable = false;
                    DataBind();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            UpdateIdAccount();
            BindAccount(_idAccount);
            is_editable = true;
            viewportState = ViewportState.ReadState;
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
                    if (spSimpleSearchForm == null)
                        spSimpleSearchForm = new SimpleSearchClaimsForm();
                    if (spSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = spSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (spExtendedSearchForm == null)
                        spExtendedSearchForm = new ExtendedSearchClaimsForm();
                    if (spExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = spExtendedSearchForm.GetFilter();
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                GeneralBindingSource.CurrentItemChanged -= GeneralBindingSource_CurrentItemChanged;
                GeneralDataModel.Select().RowChanged -= ClaimListViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= ClaimListViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            Close();
        }

        public override bool HasAssocViewport(ViewportType viewportType)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.ClaimStatesViewport,
                ViewportType.PaymentsAccountsViewport
            };
            return reports.Contains(viewportType) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрана протензионно-исковая работа для отображения ее состояний", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_claim = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row, ParentTypeEnum.Claim);
        }

        void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (v_accounts != null) v_accounts.Filter = "";
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
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridViewClaims.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
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

        void dataGridViewClaims_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
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

        void dataGridViewClaims_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewClaims.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridViewClaims.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        void dataGridViewClaims_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            var row = ((DataRowView)GeneralBindingSource[e.RowIndex]);
            switch (dataGridViewClaims.Columns[e.ColumnIndex].Name)
            {
                case "id_claim":
                    e.Value = row["id_claim"];
                    break;
                case "id_account":
                    if (row["id_account"] == DBNull.Value) return;
                    var accountList = (from paymentAccountRow in DataModel.GetInstance(DataModelType.PaymentsAccountsDataModel).FilterDeletedRows()
                                       where paymentAccountRow.Field<int?>("id_account") == (int?)row["id_account"]
                                       select paymentAccountRow).ToList();
                    if (accountList.Any())
                        e.Value = accountList.First().Field<string>("account");
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
                    e.Value = row["amount_tenancy"];
                    break;
                case "amount_dgi":
                    e.Value = row["amount_dgi"];
                    break;
                case "at_date":
                    e.Value = row["at_date"] == DBNull.Value ? "" :
                        ((DateTime)row["at_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "description":
                    e.Value = row["description"];
                    break;
                case "state_type":
                    if (row["id_claim"] == DBNull.Value || row["id_claim"] == null) return;
                    var idClaim = (int?) row["id_claim"];
                    var lastClaimState = lastClaimStates.Select().Rows.Find(idClaim);
                    if (lastClaimState != null)
                        e.Value = lastClaimState.Field<string>("state_type");
                    break;
                case "date_start_state":
                    if (row["id_claim"] == DBNull.Value || row["id_claim"] == null) return;
                    idClaim = (int?) row["id_claim"];
                    lastClaimState = lastClaimStates.Select().Rows.Find(idClaim);
                    if (lastClaimState != null && lastClaimState.Field<DateTime?>("date_start_state") != null)
                        e.Value = lastClaimState.Field<DateTime>("date_start_state").ToString("dd.MM.yyyy");
                    break;
            }
        }

        void ClaimListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridViewClaims.RowCount = GeneralBindingSource.Count;
                dataGridViewClaims.Refresh();
                UnbindedCheckBoxesUpdate();
                UpdateIdAccount();
                BindAccount(_idAccount);
                MenuCallback.ForceCloseDetachedViewports();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        void ClaimListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
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
            if (keyData == Keys.Enter)
                return false;
            return base.ProcessCmdKey(ref msg, keyData);
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
            _idAccount = (from row in DataModel.GetInstance(DataModelType.PaymentsAccountsDataModel).FilterDeletedRows()
                where row.Field<string>("account") == textBoxAccount.Text.Trim()
                select row.Field<int?>("id_account")).FirstOrDefault();
            BindAccount(_idAccount);
            CheckViewportModifications();
        }
    }
}
