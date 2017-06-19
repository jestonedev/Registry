using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Reporting;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ModalEditors;
using Registry.Viewport.Presenters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ClaimCourtOrdersViewport : FormWithGridViewport
    {
        public ClaimCourtOrdersViewport()
            : this(null, null)
        {
        }

        public ClaimCourtOrdersViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new ClaimCourtOrdersPresenter())
        {
            InitializeComponent();
            dataGridViewClaimPersons.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.ClaimsWrite))
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        protected override Entity EntityFromView()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return row == null ? new ClaimCourtOrder() : EntityConverter<ClaimCourtOrder>.FromRow(row);
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var claimCourtOrder = new ClaimCourtOrder
            {
                IdOrder = row == null ? null : ViewportHelper.ValueOrNull<int>(row, "id_order"),
                IdClaim = (int)ParentRow["id_claim"],
                IdSigner = ViewportHelper.ValueOrNull<int>(comboBoxSigner),
                IdExecutor = ViewportHelper.ValueOrNull<int>(comboBoxExecutor),
                IdJudge = ViewportHelper.ValueOrNull<int>(comboBoxJudge),
                OrderDate = ViewportHelper.ValueOrNull(dateTimePickerOrderDate),
                CreateDate = ViewportHelper.ValueOrNull(dateTimePickerCreateDate),
                OpenAccountDate = ViewportHelper.ValueOrNull(dateTimePickerPaymentAccountOpenDate),
                AmountTenancy = numericUpDownAmountTenancy.Value,
                AmountDgi = numericUpDownAmountDGI.Value,
                AmountPadun = numericUpDownAmountPadun.Value,
                AmountPkk = numericUpDownAmountPkk.Value,
                AmountPenalties = numericUpDownAmountPenalties.Value,
                StartDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerStartDeptPeriod),
                EndDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerEndDeptPeriod),
            };
            return claimCourtOrder;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;
            DataGridView = dataGridViewClaimCourtOrderVersions;
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            DataBind();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", GeneralBindingSource_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", GeneralBindingSource_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["payments_accounts"].DataSource, "RowChanged", PaymentsAccounts_RowChanged);

            DataChangeHandlersInit();

            UpdateExtInfo();
            GeneralBindingSource_CurrentItemChanged(null, EventArgs.Empty);
            IsEditable = true;
        }

        private void PaymentsAccounts_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UpdateExtInfo();
        }

        private void UpdateExtInfo()
        {
            var account = Presenter.ViewModel["payments_accounts"].DataSource.Rows.Find(ParentRow["id_account"]);
            if (account == null)
            {
                return;
            }
            textBoxRawAddress.Text = account["raw_address"].ToString();
            textBoxParsedAddress.Text = account["parsed_address"].ToString();
        }

        private bool _firstShowing = true;

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (_firstShowing && Presenter.ViewModel["general"].BindingSource.Count == 0)
            {
                _firstShowing = false;
                if (GeneralBindingSource.Count == 0)
                {
                    InsertRecord();
                    MenuCallback.EditingStateUpdate();
                }
            }
            base.OnVisibleChanged(e);
        }

        protected override void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            if (Presenter.ViewModel["general"].CurrentRow == null || DataGridView.RowCount == 0)
                DataGridView.ClearSelection();
            else
                if (bindingSource.Position >= DataGridView.RowCount)
                    DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                else if (DataGridView.Rows[bindingSource.Position].Selected != true)
                    DataGridView.Rows[bindingSource.Position].Selected = true;

            var isEditable = IsEditable;
            UpdateExtInfo();
            IsEditable = isEditable;

            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void GeneralBindingSource_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            dataGridViewClaimCourtOrderVersions.Refresh();
        }

        private void GeneralBindingSource_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
            dataGridViewClaimCourtOrderVersions.Refresh();
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindProperty(dateTimePickerOrderDate, "Value", bindingSource, "order_date", DateTime.Now.Date);
            ViewportHelper.BindSource(comboBoxJudge, Presenter.ViewModel["judge_info"].BindingSource, "judge",
                 Presenter.ViewModel["judge_info"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxJudge, "SelectedValue", bindingSource,
                Presenter.ViewModel["judge_info"].PrimaryKeyFirst, 1);

            ViewportHelper.BindSource(comboBoxSigner, Presenter.ViewModel["selectable_signers"].BindingSource, "snp",
                 Presenter.ViewModel["selectable_signers"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxSigner, "SelectedValue", bindingSource,
                "id_signer", 1);

            ViewportHelper.BindSource(comboBoxExecutor, Presenter.ViewModel["executors"].BindingSource, "executor_name",
                 Presenter.ViewModel["executors"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxExecutor, "SelectedValue", bindingSource,
                "id_executor", 1);

            ViewportHelper.BindProperty(dateTimePickerPaymentAccountOpenDate, "Value", bindingSource, "open_account_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(dateTimePickerCreateDate, "Value", bindingSource, "create_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(dateTimePickerStartDeptPeriod, "Value", bindingSource, "start_dept_period", DateTime.Now.Date);
            ViewportHelper.BindProperty(dateTimePickerEndDeptPeriod, "Value", bindingSource, "end_dept_period", DateTime.Now.Date);
            ViewportHelper.BindProperty(numericUpDownAmountTenancy, "Value", bindingSource, "amount_tenancy", DateTime.Now.Date);
            ViewportHelper.BindProperty(numericUpDownAmountDGI, "Value", bindingSource, "amount_dgi", DateTime.Now.Date);
            ViewportHelper.BindProperty(numericUpDownAmountPadun, "Value", bindingSource, "amount_padun", DateTime.Now.Date);
            ViewportHelper.BindProperty(numericUpDownAmountPkk, "Value", bindingSource, "amount_pkk", DateTime.Now.Date);
            ViewportHelper.BindProperty(numericUpDownAmountPenalties, "Value", bindingSource, "amount_penalties", DateTime.Now.Date);

            dataGridViewClaimPersons.AutoGenerateColumns = false;
            dataGridViewClaimPersons.DataSource = Presenter.ViewModel["claim_persons"].BindingSource;
            Presenter.ViewModel["claim_persons"].BindingSource.Filter = StaticFilter;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            is_claimer.DataPropertyName = "is_claimer";
            date_of_birth.DataPropertyName = "date_of_birth";

            dataGridViewClaimCourtOrderVersions.AutoGenerateColumns = false;
            dataGridViewClaimCourtOrderVersions.DataSource = Presenter.ViewModel["general"].BindingSource;
            id_order.DataPropertyName = "id_order";
            create_date.DataPropertyName = "create_date";
            order_date.DataPropertyName = "order_date";
            id_executor.DataPropertyName = "id_executor";
            id_executor.DataSource = Presenter.ViewModel["executors"].BindingSource;
            id_executor.DisplayMember = "executor_name";
            id_executor.ValueMember = "id_executor";
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void SaveRecord()
        {
            var claimCourtOrder = (ClaimCourtOrder)EntityFromViewport();
            if (!ValidateClaimCourtOrder(claimCourtOrder))
                return;
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((ClaimCourtOrdersPresenter)Presenter).InsertRecord(claimCourtOrder))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((ClaimCourtOrdersPresenter)Presenter).UpdateRecord(claimCourtOrder))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            PaymentService.UpdateJudgeInfoByIdAccount((int)ParentRow["id_account"], claimCourtOrder.IdJudge);
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            dataGridViewClaimCourtOrderVersions.Refresh();
            SetPaymentFieldsState(true);
        }

        private bool ValidateClaimCourtOrder(ClaimCourtOrder claimCourtOrder)
        {
            if (claimCourtOrder.IdJudge == null)
            {
                MessageBox.Show(@"Необходимо выбрать мирового судью", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxJudge.Focus();
                return false;
            }
            if (claimCourtOrder.IdSigner == null)
            {
                MessageBox.Show(@"Необходимо выбрать подписывающего", @"Ошибка",
                   MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSigner.Focus();
                return false;
            }
            if (claimCourtOrder.IdExecutor == null)
            {
                MessageBox.Show(@"Не указан исполнитель. В связи с отсутствием возможности редактирования поля ""Исполнитель"" обратитесь к системному администратору для добавления Вас в справочник исполнителей", @"Ошибка",
                   MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        public override bool CanCopyRecord()
        {
            return false;
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

            DataRowView lastRow = null;
            if (Presenter.ViewModel["general"].BindingSource.Count > 0)
            {
                lastRow =
                    (DataRowView)
                        Presenter.ViewModel["general"].BindingSource[
                            Presenter.ViewModel["general"].BindingSource.Position];
            }

            Presenter.ViewModel["general"].BindingSource.AddNew();

            if (lastRow != null)
            {
                dateTimePickerOrderDate.Value = lastRow["order_date"] != DBNull.Value
                    ? (DateTime) lastRow["order_date"]
                    : DateTime.Now.Date;
                dateTimePickerPaymentAccountOpenDate.Value = lastRow["open_account_date"] != DBNull.Value
                    ? (DateTime) lastRow["open_account_date"]
                    : DateTime.Now.Date;
                dateTimePickerStartDeptPeriod.Value = lastRow["start_dept_period"] != DBNull.Value
                    ? (DateTime)lastRow["start_dept_period"]
                    : DateTime.Now.Date;
                dateTimePickerEndDeptPeriod.Value = lastRow["end_dept_period"] != DBNull.Value
                    ? (DateTime)lastRow["end_dept_period"]
                    : DateTime.Now.Date;
                comboBoxSigner.SelectedValue = lastRow["id_signer"] != DBNull.Value ? (int?) lastRow["id_signer"] : null;
                comboBoxJudge.SelectedValue = lastRow["id_judge"] != DBNull.Value ? (int?)lastRow["id_judge"] : null;
                numericUpDownAmountTenancy.Value = lastRow["amount_tenancy"] != DBNull.Value ? (decimal)lastRow["amount_tenancy"] : 0;
                numericUpDownAmountDGI.Value = lastRow["amount_dgi"] != DBNull.Value ? (decimal)lastRow["amount_dgi"] : 0;
                numericUpDownAmountPadun.Value = lastRow["amount_padun"] != DBNull.Value ? (decimal)lastRow["amount_padun"] : 0;
                numericUpDownAmountPkk.Value = lastRow["amount_pkk"] != DBNull.Value ? (decimal)lastRow["amount_pkk"] : 0;
                numericUpDownAmountPenalties.Value = lastRow["amount_penalties"] != DBNull.Value ? (decimal)lastRow["amount_penalties"] : 0;
            }
            else
            {

                var idJudge = PaymentService.GetJudgeByIdAccount((int) ParentRow["id_account"]);
                if (idJudge != null)
                {
                    comboBoxJudge.SelectedValue = (int) idJudge;
                }
                else
                {
                    comboBoxJudge.SelectedValue = DBNull.Value;
                }

                if (ParentRow["start_dept_period"] != DBNull.Value)
                {
                    dateTimePickerStartDeptPeriod.Checked = true;
                    dateTimePickerStartDeptPeriod.Value = (DateTime)ParentRow["start_dept_period"];
                }
                else
                {
                    dateTimePickerStartDeptPeriod.Checked = false;
                    dateTimePickerStartDeptPeriod.Value = DateTime.Now.Date;
                }
                if (ParentRow["end_dept_period"] != DBNull.Value)
                {
                    dateTimePickerEndDeptPeriod.Checked = true;
                    dateTimePickerEndDeptPeriod.Value = (DateTime)ParentRow["end_dept_period"];
                }
                else
                {
                    dateTimePickerEndDeptPeriod.Checked = false;
                    dateTimePickerEndDeptPeriod.Value = DateTime.Now.Date;
                }
                numericUpDownAmountTenancy.Value = (decimal)ParentRow["amount_tenancy"];
                numericUpDownAmountDGI.Value = (decimal)ParentRow["amount_dgi"];
                numericUpDownAmountPadun.Value = (decimal)ParentRow["amount_padun"];
                numericUpDownAmountPkk.Value = (decimal)ParentRow["amount_pkk"];
                numericUpDownAmountPenalties.Value = (decimal)ParentRow["amount_penalties"];
            }

            if (Presenter.ViewModel["claim_persons"].BindingSource.Count == 0)
            {
                var claimPersons = PaymentService.GetClaimPersonsByIdAccount((int) ParentRow["id_account"]).ToList();
                if (!claimPersons.Any())
                {
                    MessageBox.Show(
                        @"Участники найма по даннмоу адресу ЖФ отсутствуют, либо отсутствует привязка адреса",
                        @"Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    foreach (var claimPerson in claimPersons)
                    {
                        claimPerson.IdClaim = (int) ParentRow["id_claim"];
                        if (!((ClaimCourtOrdersPresenter) Presenter).InsertClaimPersonRecord(claimPerson))
                        {
                            IsEditable = true;
                            return;
                        }
                    }
                }
            }
            dateTimePickerCreateDate.Value = DateTime.Now.Date;
            var login = WindowsIdentity.GetCurrent().Name;
            var index = Presenter.ViewModel["executors"].BindingSource.Find("executor_login", login);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)Presenter.ViewModel["executors"].BindingSource[index])["id_executor"];

            numericUpDownAmountTotal.Value = numericUpDownAmountTenancy.Value + numericUpDownAmountDGI.Value +
                + numericUpDownAmountPadun.Value + numericUpDownAmountPkk.Value +
                + numericUpDownAmountPenalties.Value;

            SetPaymentFieldsState(false);

            IsEditable = true;
        }

        public void SetPaymentFieldsState(bool readOnly)
        {
            numericUpDownAmountTenancy.ReadOnly = readOnly;
            numericUpDownAmountDGI.ReadOnly = readOnly;
            numericUpDownAmountPadun.ReadOnly = readOnly;
            numericUpDownAmountPkk.ReadOnly = readOnly;
            numericUpDownAmountPenalties.ReadOnly = readOnly;
            dateTimePickerStartDeptPeriod.Enabled = !readOnly;
            dateTimePickerEndDeptPeriod.Enabled = !readOnly;
            dateTimePickerOrderDate.Enabled = !readOnly;
            dateTimePickerPaymentAccountOpenDate.Enabled = !readOnly;
            comboBoxSigner.Enabled = !readOnly;
            comboBoxJudge.Enabled = !readOnly;
        }

        public override bool CanCancelRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            textBoxRawAddress.Focus();
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    Presenter.ViewModel["general"].Model.EditingNewRecord = false;
                    if (Presenter.ViewModel["general"].CurrentRow != null)
                    {
                        IsEditable = false;
                        Presenter.ViewModel["general"].CurrentRow.Delete();
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    break;
            }
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetPaymentFieldsState(true);
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null)
                && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить выбранную версию судебного приказа?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            IsEditable = false;
            var selectedIdOrders = new List<int>();
            foreach (DataGridViewRow row in DataGridView.SelectedRows)
            {
                if (row.Cells["id_order"].Value != null)
                {
                    selectedIdOrders.Add((int)row.Cells["id_order"].Value);
                }
            }

            if (!((ClaimCourtOrdersPresenter)Presenter).DeleteRecords(selectedIdOrders))
            {
                IsEditable = true;
                return;
            }
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
        }

        private void vButtonPersonAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран судебный приказ", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (Presenter.ViewModel["claim_persons"].Model.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок участников найма уже находится в режиме добавления новых записей. " +
                    @"Одновременно можно добавлять не более одного участника.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var editor = new ClaimPersonEditor())
            {
                editor.State = ViewportState.NewRowState;
                editor.ParentRow = ParentRow;
                editor.ShowDialog();
            }
        }

        private void vButtonPersonDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран судебный приказ", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var claimPersonRow = Presenter.ViewModel["claim_persons"].CurrentRow;
            if (claimPersonRow == null)
            {
                MessageBox.Show(@"Не выбран участник для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var id = (int)claimPersonRow["id_person"];
            if (MessageBox.Show(@"Вы действительно хотите удалить этого участника?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            Presenter.ViewModel["claim_persons"].Delete(id);
        }

        private void vButtonPersonEdit_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var claimCourtOrderRow = Presenter.ViewModel["general"].CurrentRow;
            if (claimCourtOrderRow == null)
            {
                MessageBox.Show(@"Не выбран судебный приказ", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (Presenter.ViewModel["claim_persons"].Model.EditingNewRecord)
            {
                MessageBox.Show(@"Одна из вкладок участников найма уже находится в режиме добавления новых записей. " +
                                @"Одновременно можно добавлять не более одного участника.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = Presenter.ViewModel["claim_persons"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрана комната для редактирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var claimPerson = EntityConverter<ClaimPerson>.FromRow(row);
            using (var editor = new ClaimPersonEditor())
            {
                editor.State = ViewportState.ModifyRowState;
                editor.ParentRow = ParentRow;
                editor.ClaimPerson = claimPerson;
                editor.ShowDialog();
            }
        }

        public override bool HasReport(ReporterType reporterType)
        {
            var reports = new List<ReporterType>
            {
                ReporterType.JudicialOrderReporter
            };
            return reports.Contains(reporterType);
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position < 0)
            {
                MessageBox.Show(@"Не выбран судебный приказ для формирования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.JudicialOrderReporter:
                    if (ValidForJudicialOrderReporter())
                    {
                        arguments.Add("id_claim", ParentRow["id_claim"].ToString());
                        arguments.Add("id_order", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_order"].ToString());
                        MenuCallback.RunReport(reporterType, arguments);
                    }
                    break;
                default:
                    throw new ReporterException("Неподдерживаемый тип отчета");
            }
        }

        private bool ValidForJudicialOrderReporter()
        {
            if (GeneralBindingSource.Count == 0)
            {
                MessageBox.Show(@"Отсутствует необходимая информация по судебному приказу. Невозможно сформировать заявление", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }
    }
}
