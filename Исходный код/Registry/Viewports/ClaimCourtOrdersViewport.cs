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
    internal sealed partial class ClaimCourtOrdersViewport : FormViewport
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
                IdExecutor = ViewportHelper.ValueOrNull<int>(comboBoxExecutor),
                IdJudge = ViewportHelper.ValueOrNull<int>(comboBoxJudge),
                OrderDate = ViewportHelper.ValueOrNull(dateTimePickerOrderDate),
                OpenAccountDate = ViewportHelper.ValueOrNull(dateTimePickerPaymentAccountOpenDate)
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
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            Presenter.ViewModel["judges"].BindingSource.Filter = "is_inactive = 0";
            Presenter.ViewModel["executors"].BindingSource.Filter = "is_inactive = 0";

            DataBind();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", GeneralBindingSource_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", GeneralBindingSource_RowDeleted);

            DataChangeHandlersInit();

            IsEditable = true;
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

        private void GeneralBindingSource_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void GeneralBindingSource_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        private void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindProperty(dateTimePickerOrderDate, "Value", bindingSource, "order_date", DateTime.Now.Date);
            ViewportHelper.BindSource(comboBoxJudge, Presenter.ViewModel["judges"].BindingSource, "snp",
                 Presenter.ViewModel["judges"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxJudge, "SelectedValue", bindingSource,
                Presenter.ViewModel["judges"].PrimaryKeyFirst, 1);

            ViewportHelper.BindSource(comboBoxExecutor, Presenter.ViewModel["executors"].BindingSource, "executor_name",
                 Presenter.ViewModel["executors"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxExecutor, "SelectedValue", bindingSource,
                Presenter.ViewModel["executors"].PrimaryKeyFirst, 1);

            ViewportHelper.BindProperty(dateTimePickerPaymentAccountOpenDate, "Value", bindingSource, "open_account_date", DateTime.Now.Date);

            dataGridViewClaimPersons.AutoGenerateColumns = false;
            dataGridViewClaimPersons.DataSource = Presenter.ViewModel["claim_persons"].BindingSource;
            Presenter.ViewModel["claim_persons"].BindingSource.Filter = StaticFilter;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            is_claimer.DataPropertyName = "is_claimer";
            date_of_birth.DataPropertyName = "date_of_birth";
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
            if (claimCourtOrder.IdExecutor == null)
            {
                MessageBox.Show(@"Необходимо выбрать исполнителя", @"Ошибка",
                   MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxExecutor.Focus();
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
            return GeneralBindingSource.Count == 0;
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();

            var login = WindowsIdentity.GetCurrent().Name;
            var index = Presenter.ViewModel["executors"].BindingSource.Find("executor_login", login);
            if (index != -1)
            {
                comboBoxExecutor.SelectedValue = ((DataRowView)Presenter.ViewModel["executors"].BindingSource[index])["id_executor"];   
            }
            var idJudge = PaymentService.GetJudgeByIdAccount((int)ParentRow["id_account"]);
            if (idJudge != null)
            {
                comboBoxJudge.SelectedValue = (int) idJudge;
            }
            else
            {
                comboBoxJudge.SelectedValue = DBNull.Value;
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

            IsEditable = true;
        }

        public override bool CanCancelRecord()
        {
            return false;
        }

        public override void CancelRecord()
        {
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
        }

        public override bool CanDeleteRecord()
        {
            return false;
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
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.JudicialOrderReporter:
                    if (ValidForJudicialOrderReporter())
                    {
                        arguments.Add("id_claim", ParentRow["id_claim"].ToString());
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
