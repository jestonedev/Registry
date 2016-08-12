using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Declensions.Unicode;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ModalEditors;
using Registry.Viewport.Presenters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyAgreementsViewport : FormWithGridViewport
    {
        //Forms
        private SelectWarrantForm _swForm;
        private int? _idWarrant;

        // Auto modify properties
        private readonly List<TenancyPerson> _excludePersons = new List<TenancyPerson>();
        private readonly List<TenancyPerson> _includePersons = new List<TenancyPerson>();

        private enum ExtModificationTypes
        {
            CommercialProlong,
            SpecialProlong,
            ChangeTenant
        }

        private sealed class ExtModificationParameter
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        private readonly List<Dictionary<ExtModificationTypes, List<ExtModificationParameter>>> _modifications = 
            new List<Dictionary<ExtModificationTypes, List<ExtModificationParameter>>>();

        private TenancyAgreementsViewport()
            : this(null, null)
        {
        }

        public TenancyAgreementsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new TenancyAgreementsPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            dataGridViewTenancyPersons.AutoGenerateColumns = false;
            dataGridViewChangeTenant.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        private void RedrawDataGridTenancyPersonsRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (var i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
            {
                if (Presenter.ViewModel["persons_exclude"].BindingSource.Count <= i) break;
                var row = (DataRowView)Presenter.ViewModel["persons_exclude"].BindingSource[i];
                if (ViewportHelper.ValueOrNull<int>(row, "id_kinship") == 1 && row["exclude_date"] == DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else if (row["exclude_date"] != DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                else
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }

            if (dataGridViewChangeTenant.Rows.Count == 0)
                return;
            for (var i = 0; i < dataGridViewChangeTenant.Rows.Count; i++)
            {
                if (Presenter.ViewModel["persons_change_tenant"].BindingSource.Count <= i) break;
                var row = (DataRowView)Presenter.ViewModel["persons_change_tenant"].BindingSource[i];
                if (ViewportHelper.ValueOrNull<int>(row, "id_kinship") == 1 && row["exclude_date"] == DBNull.Value)
                    dataGridViewChangeTenant.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else if (row["exclude_date"] != DBNull.Value)
                    dataGridViewChangeTenant.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                else
                    dataGridViewChangeTenant.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindSource(comboBoxExecutor, Presenter.ViewModel["executors"].BindingSource, "executor_name",
                 Presenter.ViewModel["executors"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxExecutor, "SelectedValue", bindingSource,
                Presenter.ViewModel["executors"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindSource(comboBoxIncludeKinship, Presenter.ViewModel["kinships"].BindingSource, "kinship",
                Presenter.ViewModel["kinships"].PrimaryKeyFirst);

            ViewportHelper.BindProperty(textBoxAgreementContent, "Text", bindingSource, "agreement_content", "");
            ViewportHelper.BindProperty(dateTimePickerAgreementDate, "Value", bindingSource, "agreement_date", DateTime.Now.Date);

            ViewportHelper.BindSource(comboboxTenantChangeKinship, Presenter.ViewModel["kinships"].BindingSource,
                "kinship", Presenter.ViewModel["kinships"].PrimaryKeyFirst);

            UpdateTenantChangeTab();

            dataGridView.DataSource = bindingSource;
            id_agreement.DataPropertyName = "id_agreement";
            agreement_date.DataPropertyName = "agreement_date";
            agreement_content.DataPropertyName = "agreement_content";

            dataGridViewTenancyPersons.DataSource = Presenter.ViewModel["persons_exclude"].BindingSource;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            date_of_birth.DataPropertyName = "date_of_birth";

            dataGridViewChangeTenant.DataSource = Presenter.ViewModel["persons_change_tenant"].BindingSource;
            surnameChangeTenant.DataPropertyName = "surname";
            nameChangeTenant.DataPropertyName = "name";
            patronymicChangeTenant.DataPropertyName = "patronymic";
            dateofbirthChangeTenant.DataPropertyName = "date_of_birth";
        }

        private void UpdateTenantChangeTab()
        {
            if (Presenter.ViewModel["tenant_change_tenant"].BindingSource.Count <= 0) return;
            var row = (DataRowView)Presenter.ViewModel["tenant_change_tenant"].BindingSource[0];
            textBoxChangeTenantChangeFIO.Text = row["surname"] + @" " + row["name"] + @" " + row["patronymic"];
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.TenancyWrite)) return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private void BindWarrantId()
        {
            IsEditable = false;
            var row = Presenter.ViewModel["general"].CurrentRow;
            if ((row != null) && row["id_warrant"] != DBNull.Value)
            {
                _idWarrant = Convert.ToInt32(row["id_warrant"], CultureInfo.InvariantCulture);
                textBoxAgreementWarrant.Text = ((TenancyAgreementsPresenter)Presenter).WarrantStringById(_idWarrant.Value);
                vButtonSelectWarrant.Text = @"x";
            }
            else
            {
                _idWarrant = null;
                textBoxAgreementWarrant.Text = "";
                vButtonSelectWarrant.Text = @"...";
            }
            IsEditable = true;
        }

        private void ViewportFromTenancyAgreement(TenancyAgreement tenancyAgreement)
        {
            comboBoxExecutor.SelectedValue = ViewportHelper.ValueOrDbNull(tenancyAgreement.IdExecutor);
            dateTimePickerAgreementDate.Value = ViewportHelper.ValueOrDefault(tenancyAgreement.AgreementDate);
            textBoxAgreementContent.Text = tenancyAgreement.AgreementContent;
            if (tenancyAgreement.IdWarrant != null)
            {
                textBoxAgreementWarrant.Text = ((TenancyAgreementsPresenter)Presenter).WarrantStringById(tenancyAgreement.IdWarrant.Value);
                _idWarrant = tenancyAgreement.IdWarrant;
            }
            else
            {
                textBoxAgreementWarrant.Text = "";
                _idWarrant = null;
            }
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var tenancyAgreement = new TenancyAgreement
            {
                IdAgreement = row == null ? null : ViewportHelper.ValueOrNull<int>(row, "id_agreement"),
                IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                IdExecutor = ViewportHelper.ValueOrNull<int>(comboBoxExecutor),
                IdWarrant = _idWarrant,
                AgreementContent = textBoxAgreementContent.Text,
                AgreementDate = ViewportHelper.ValueOrNull(dateTimePickerAgreementDate)
            };
            return tenancyAgreement;
        }

        protected override Entity EntityFromView()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return EntityConverter<TenancyAgreement>.FromRow(row);
        }

        private bool ValidateAgreement(TenancyAgreement tenancyAgreement)
        {
            if (tenancyAgreement.IdExecutor != null) return true;
            MessageBox.Show(@"Необходимо выбрать исполнителя", @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            comboBoxExecutor.Focus();
            return false;
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

            if (ParentRow == null)
            {
                throw new ViewportException("Не указан родительский объект");
            }

            if (ParentType == ParentTypeEnum.Tenancy)
                Text = string.Format(CultureInfo.InvariantCulture, "Соглашения найма №{0}", ParentRow["id_process"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            ((TenancyAgreementsPresenter)Presenter).InitExtendedViewModelItems(StaticFilter);

            DataBind();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", TenancyAgreementsViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", TenancyAgreementsViewport_RowChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["persons_exclude"].DataSource, "RowDeleted", TenancyPersonsViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["persons_exclude"].DataSource, "RowChanged", TenancyPersonsViewport_RowChanged);

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());

            DataChangeHandlersInit();

            IsEditable = true;
            if (Presenter.ViewModel["general"].BindingSource.Count == 0)
                InsertRecord();
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null)
                && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите это соглашение?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;

            IsEditable = false;
            if (!((TenancyAgreementsPresenter)Presenter).DeleteRecord())
            {
                IsEditable = true;
                return;
            }
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
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
                        if (Presenter.ViewModel["general"].CurrentRow != null)
                            DataGridView.Rows[Presenter.ViewModel["general"].BindingSource.Position].Selected = true;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    BindWarrantId();
                    break;
            }
            IsEditable = true;
            DataGridView.Enabled = true;
            ClearModifyState();
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState))
                 && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            var tenancyAgreement = (TenancyAgreement) EntityFromViewport();
            if (!ValidateAgreement(tenancyAgreement))
                return;

            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((TenancyAgreementsPresenter)Presenter).InsertRecord(tenancyAgreement))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((TenancyAgreementsPresenter)Presenter).UpdateRecord(tenancyAgreement))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            IsEditable = true;
            DataGridView.Enabled = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            
            // Обновление участников найма после сохранения соглашения
            if (_includePersons.Count > 0)
            {
                new TenancyAgreementOnSavePersonManager(_includePersons,
                    TenancyAgreementOnSavePersonManager.PersonsOperationType.IncludePersons).ShowDialog();
            }
            if (_excludePersons.Count > 0)
            {
                new TenancyAgreementOnSavePersonManager(_excludePersons,
                    TenancyAgreementOnSavePersonManager.PersonsOperationType.ExcludePersons).ShowDialog();
            }
            // Дополнительные обновления
            ExtModificationsExecute(_modifications);
            ClearModifyState();
        }

        private void ExtModificationsExecute(IEnumerable<Dictionary<ExtModificationTypes, List<ExtModificationParameter>>> modifications)
        {
            foreach (var modification in modifications)
            {
                foreach (var modificationPair in modification)
                {
                    switch (modificationPair.Key)
                    {
                            case ExtModificationTypes.ChangeTenant:
                                ChangeTenant(modificationPair.Value);
                                break;
                            case ExtModificationTypes.CommercialProlong:
                            case ExtModificationTypes.SpecialProlong:
                                Prolong(modificationPair.Value);
                                break;
                    }
                }
            }
        }

        private void Prolong(List<ExtModificationParameter> parameters)
        {
            var beginDate = parameters.Where(v => v.Name == "ProlongFrom").Select(v => (DateTime?)v.Value).FirstOrDefault();
            var endDate = parameters.Where(v => v.Name == "ProlongTo").Select(v => (DateTime?)v.Value).FirstOrDefault();
            var untilDismissal = parameters.Where(v => v.Name == "UntilDismissal").Select(v => (bool?)v.Value).FirstOrDefault() ?? false;

            var rentPeriod = new TenancyRentPeriod
            {
                IdProcess = (int)ParentRow["id_process"],
                BeginDate = ViewportHelper.ValueOrNull<DateTime>(ParentRow, "begin_date"),
                EndDate = ViewportHelper.ValueOrNull<DateTime>(ParentRow, "end_date"),
                UntilDismissal = ViewportHelper.ValueOrNull<bool>(ParentRow, "until_dismissal"),
            };
            var rentPeriods = EntityDataModel<TenancyRentPeriod>.GetInstance();
            rentPeriods.EditingNewRecord = true;
            var idRentPeriod = rentPeriods.Insert(rentPeriod);
            if (idRentPeriod == -1) return;
            rentPeriod.IdRentPeriod = idRentPeriod;
            rentPeriods.Select().Rows.Add(idRentPeriod, rentPeriod.IdProcess, rentPeriod.BeginDate, rentPeriod.EndDate, rentPeriod.UntilDismissal);
            rentPeriods.EditingNewRecord = false;

            var tenancyProcess = EntityConverter<TenancyProcess>.FromRow(ParentRow);
            tenancyProcess.BeginDate = beginDate;
            tenancyProcess.EndDate = endDate;
            tenancyProcess.UntilDismissal = untilDismissal;
            var tenancyProcesses = EntityDataModel<TenancyProcess>.GetInstance();
            tenancyProcesses.EditingNewRecord = true;
            if (tenancyProcesses.Update(tenancyProcess) == -1)
            {
                return;
            }
            ParentRow["begin_date"] = ViewportHelper.ValueOrDbNull(beginDate);
            ParentRow["end_date"] = ViewportHelper.ValueOrDbNull(endDate);
            ParentRow["until_dismissal"] = untilDismissal;
            tenancyProcesses.EditingNewRecord = false;
        }

        private static void ChangeTenant(List<ExtModificationParameter> parameters)
        {
            var idOldTenant = parameters.Where(v => v.Name == "IdOldTenant").Select(v => (int?)v.Value).FirstOrDefault();
            var idKinshipOldTenant = parameters.Where(v => v.Name == "IdKinshipOldTenant").Select(v => (int?)v.Value).FirstOrDefault();
            var idNewTenant = parameters.Where(v => v.Name == "IdNewTenant").Select(v => (int?)v.Value).FirstOrDefault();
            var excludeTenant = parameters.Where(v => v.Name == "ExcludeTenant").Select(v => (bool?)v.Value).FirstOrDefault();
            if (idOldTenant == null || idNewTenant == null) return;
            var oldTenantRow =
                EntityDataModel<TenancyPerson>.GetInstance()
                    .FilterDeletedRows().FirstOrDefault(v => v.Field<int>("id_person") == idOldTenant.Value);
            var newTenantRow =
                EntityDataModel<TenancyPerson>.GetInstance()
                    .FilterDeletedRows().FirstOrDefault(v => v.Field<int>("id_person") == idNewTenant.Value);
            var oldTenant = oldTenantRow != null ? EntityConverter<TenancyPerson>.FromRow(oldTenantRow) : null;
            var newTenant = newTenantRow != null ? EntityConverter<TenancyPerson>.FromRow(newTenantRow) : null;

            if (oldTenant == null || newTenant == null)
            {
                MessageBox.Show(@"Произошла непредвиденная ошибка при изменении данных об участниках найма",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            if (excludeTenant == true)
            {
                using (var form = new PersonExcludeDateForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        oldTenant.ExcludeDate = form.ExcludeDate;
                    }
                }
            }
            else
            {
                oldTenant.IdKinship = idKinshipOldTenant;
            }
            var affected = EntityDataModel<TenancyPerson>.GetInstance().Update(oldTenant);
            if (affected == -1)
            {
                return;
            }
            oldTenantRow.BeginEdit();
            oldTenantRow["id_kinship"] = ViewportHelper.ValueOrDbNull(oldTenant.IdKinship);
            oldTenantRow["exclude_date"] = ViewportHelper.ValueOrDbNull(oldTenant.ExcludeDate);
            oldTenantRow.EndEdit();

            newTenant.IdKinship = 1;
            newTenant.ExcludeDate = null;
            affected = EntityDataModel<TenancyPerson>.GetInstance().Update(newTenant);
            if (affected == -1)
            {
                return;
            }
            newTenantRow.BeginEdit();
            newTenantRow["id_kinship"] = ViewportHelper.ValueOrDbNull(newTenant.IdKinship);
            newTenantRow["exclude_date"] = ViewportHelper.ValueOrDbNull(newTenant.ExcludeDate);
            newTenantRow.EndEdit();
        }

        public override bool CanInsertRecord()
        {
            return (!Presenter.ViewModel["general"].Model.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            DataGridView.Enabled = false;
            var index = Presenter.ViewModel["executors"].BindingSource.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)Presenter.ViewModel["executors"].BindingSource[index])["id_executor"];
            textBoxAgreementContent.Text = string.Format(CultureInfo.InvariantCulture,
                "1.1. По настоящему Соглашению Стороны по договору № {0} от {1} договорились:",
                ParentRow["registration_num"],
                ParentRow["registration_date"] != DBNull.Value
                    ? Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture)
                        .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
                    : "");
            IsEditable = true;
        }

        public override bool CanCopyRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) && (!Presenter.ViewModel["general"].Model.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var tenancyAgreement = (TenancyAgreement)EntityFromView();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            DataGridView.Enabled = false;
            ViewportFromTenancyAgreement(tenancyAgreement);
            IsEditable = true;
        }

        public override bool HasReport(ReporterType reporterType)
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
                return false;
            var idAgreement = ViewportHelper.ValueOrNull(row, "id_agreement");
            switch (reporterType)
            {
                case ReporterType.TenancyAgreementReporter:
                case ReporterType.TenancyNotifyContractAgreement:
                    return idAgreement != null;
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбрано соглашение для печати",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            switch (reporterType)
            {
                case ReporterType.TenancyAgreementReporter:
                    ReporterFactory.CreateReporter(reporterType).Run(new Dictionary<string, string> { { "id_agreement", row["id_agreement"].ToString() } });
                    break;
                case ReporterType.TenancyNotifyContractAgreement:
                    var arguments = new Dictionary<string, string>
                    {
                        {"id_process", row["id_process"].ToString()},
                        {"report_type", "2"}
                    };
                    ReporterFactory.CreateReporter(reporterType).Run(arguments);
                    break;
            }
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (ParentType != ParentTypeEnum.Tenancy)
            {
                MessageBox.Show(@"Некорректный родительский объект",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (!TenancyService.TenancyProcessHasTenant(Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture)))
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо указать нанимателя процесса найма", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(ParentRow, "registration_date") == null || 
                ViewportHelper.ValueOrNull(ParentRow, "registration_num") == null)
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
            vButtonSelectWarrant.Focus();
            base.OnVisibleChanged(e);
        }

        private void vButtonSelectWarrant_Click(object sender, EventArgs e)
        {
            if (_idWarrant != null)
            {
                _idWarrant = null;
                textBoxAgreementWarrant.Text = "";
                vButtonSelectWarrant.Text = @"...";
                return;
            }
            if (_swForm == null)
                _swForm = new SelectWarrantForm();
            if (_swForm.ShowDialog() != DialogResult.OK) return;
            if (_swForm.WarrantId == null) return;
            _idWarrant = _swForm.WarrantId.Value;
            textBoxAgreementWarrant.Text = ((TenancyAgreementsPresenter)Presenter).WarrantStringById(_swForm.WarrantId.Value);
            vButtonSelectWarrant.Text = @"x";
        }

        private void TenancyAgreementsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        private void TenancyAgreementsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void TenancyPersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
            UpdateTenantChangeTab();
        }

        private void TenancyPersonsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
            UpdateTenantChangeTab();
        }

        private void vButtonTerminatePaste_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxTerminateAgreement.Text.Trim()))
            {
                MessageBox.Show(@"Не указана причина расторжения договора", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainPoint.Focus();
                return;
            }
            ClearModifyState();
            textBoxAgreementContent.Text =
                string.Format(CultureInfo.InvariantCulture,
                    "1.1. По настоящему Соглашению Стороны договорились расторгнуть с {3} договор № {0} от {1} {4} найма жилого помещения (далее - договор) по {2}.\r\n" +
                    "1.2. Обязательства, возникшие из указанного договора до момента расторжения, подлежат исполнению в соответствии с указанным договором. Стороны не имеют взаимных претензий по исполнению условий договора № {0} от {1}.",
                    ParentRow["registration_num"],
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "",
                        textBoxTerminateAgreement.Text.StartsWith("по ") ? textBoxTerminateAgreement.Text.Substring(3).Trim() : textBoxTerminateAgreement.Text.Trim(),
                    dateTimePickerTerminateDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    DataModel.GetInstance<RentTypesDataModel>().Select().Rows.Find(ParentRow["id_rent_type"])["rent_type_genetive"]);
        }

        private void vButtonExplainPaste_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxExplainPoint.Text.Trim()) && string.IsNullOrEmpty(textBoxExplainGeneralPoint.Text.Trim()))
            {
                MessageBox.Show(@"Не указан ни номер пункта, ни номер подпункта", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainPoint.Focus();
                return;
            }
            if (string.IsNullOrEmpty(textBoxExplainContent.Text.Trim()))
            {
                MessageBox.Show(@"Содержание изложения не может быть пустым", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainContent.Focus();
                return;
            }
            textBoxAgreementContent.Text = ((TenancyAgreementsPresenter)Presenter).ExplainContentModifier(textBoxAgreementContent.Text,
                textBoxExplainGeneralPoint.Text,
                textBoxExplainPoint.Text, textBoxExplainContent.Text);

        }

        private void vButtonIncludePaste_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxIncludeSNP.Text.Trim()))
            {
                MessageBox.Show(@"Поле ФИО не может быть пустым", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxIncludeSNP.Focus();
                return;
            }
            if (comboBoxIncludeKinship.SelectedValue == null)
            {
                MessageBox.Show(@"Не выбрана родственная связь", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxIncludeKinship.Focus();
                return;
            }
            var snp = textBoxIncludeSNP.Text.Trim();
            string sSurname, sName, sPatronymic;
            Declension.GetSNM(snp, out sSurname, out sName, out sPatronymic);

            textBoxAgreementContent.Text = ((TenancyAgreementsPresenter)Presenter).IncludePersonsContentModifier(textBoxAgreementContent.Text,
                textBoxGeneralIncludePoint.Text, textBoxIncludePoint.Text, sSurname, sName, sPatronymic,
                ((DataRowView)comboBoxIncludeKinship.SelectedItem)["kinship"].ToString(), dateTimePickerIncludeDateOfBirth.Value);

            _includePersons.Add(new TenancyPerson
            {
                IdProcess = (int?)ParentRow["id_process"],
                Surname = sSurname,
                Name = sName,
                Patronymic = sPatronymic,
                DateOfBirth = dateTimePickerIncludeDateOfBirth.Value.Date,
                IdKinship = (int?)comboBoxIncludeKinship.SelectedValue
            });
        }

        private void vButtonExcludePaste_Click(object sender, EventArgs e)
        {
            if (Presenter.ViewModel["persons_exclude"].BindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран участник найма", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            textBoxAgreementContent.Text = ((TenancyAgreementsPresenter)Presenter).ExcludePersonsContentModifier(textBoxAgreementContent.Text,
                textBoxGeneralExcludePoint.Text,
                textBoxExcludePoint.Text);

            var tenancyPerson = Presenter.ViewModel["persons_exclude"].CurrentRow;
            _excludePersons.Add(new TenancyPerson
            {
                IdProcess = (int?)ParentRow["id_process"],
                IdPerson = (int?)tenancyPerson["id_person"],
                Surname = tenancyPerson["surname"].ToString(),
                Name = tenancyPerson["name"].ToString(),
                Patronymic = tenancyPerson["patronymic"].ToString(),
                DateOfBirth = (DateTime?)(tenancyPerson["date_of_birth"] == DBNull.Value ? null : tenancyPerson["date_of_birth"]),
                IdKinship = (int?)tenancyPerson["id_kinship"]
            });
        }

        private void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
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
            BindWarrantId();
            IsEditable = isEditable;

            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void vButtonChangeTenancy_Click(object sender, EventArgs e)
        {
            var tenantChangeTenant = Presenter.ViewModel["tenant_change_tenant"].BindingSource;
            var personsChangeTenant = Presenter.ViewModel["persons_change_tenant"].BindingSource;
            if (tenantChangeTenant.Position == -1)
            {
                MessageBox.Show(@"Не выбран текущий наниматель", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (personsChangeTenant.Position == -1)
            {
                MessageBox.Show(@"Не выбран новый наниматель", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (comboboxTenantChangeKinship.SelectedValue == null && !checkBoxExcludeTenant.Checked)
            {
                MessageBox.Show(@"Не выбрано новое родственное отношение текущего нанимателя или его исключение из найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (comboboxTenantChangeKinship.SelectedValue != null && (int)comboboxTenantChangeKinship.SelectedValue == 1 && !checkBoxExcludeTenant.Checked)
            {
                MessageBox.Show(@"Нельзя исключаемому нанимателю указать новую родственную связь «наниматель». Выберите другую родственную связь", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            ClearModifyState();

            textBoxAgreementContent.Text = ((TenancyAgreementsPresenter)Presenter).ChangeTenancyStringBuilder();

            var oldTenantRow = (DataRowView)tenantChangeTenant[0];
            var newTenantRow = (DataRowView)personsChangeTenant[personsChangeTenant.Position];
            _modifications.Add(new Dictionary<ExtModificationTypes, List<ExtModificationParameter>>
            {
                { ExtModificationTypes.ChangeTenant, new List<ExtModificationParameter>
                    {
                        new ExtModificationParameter
                        {
                            Name = "ExcludeTenant",
                            Value = checkBoxExcludeTenant.Checked
                        },
                        new ExtModificationParameter
                        {
                            Name = "IdNewTenant",
                            Value = (int)newTenantRow["id_person"]
                        },
                        new ExtModificationParameter
                        {
                            Name = "IdOldTenant",
                            Value = (int?)oldTenantRow["id_person"]
                        },
                        new ExtModificationParameter
                        {
                            Name = "IdKinshipOldTenant",
                            Value = !checkBoxExcludeTenant.Checked && comboboxTenantChangeKinship.SelectedValue != null ? 
                                    (int?)comboboxTenantChangeKinship.SelectedValue : null
                        }
                    } 
                }
            });
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return keyData != Keys.Enter && base.ProcessCmdKey(ref msg, keyData);
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        private void vButtonPaymentInsert_Click(object sender, EventArgs e)
        {
            var pointCount = textBoxAgreementContent.Lines.Count(v => Regex.IsMatch(v, "^\u200B?[0-9]+\\)"));
            var text = ((TenancyAgreementsPresenter)Presenter).PaymentStringBuilder(pointCount + 1);
            textBoxAgreementContent.Text += textBoxAgreementContent.Text.EndsWith("\n") ? text : Environment.NewLine + text;                                       
        }

        private void checkBoxExcludeTenant_CheckStateChanged(object sender, EventArgs e)
        {
            comboboxTenantChangeKinship.Enabled = !checkBoxExcludeTenant.Checked;
        }

        private void vButtonProlongCommercial_Click(object sender, EventArgs e)
        {
            ClearModifyState();
            var text = ((TenancyAgreementsPresenter)Presenter).ProlongCommercialStringBuilder(
                dateTimePickerCommercialProlongFrom.Checked ? (DateTime?)dateTimePickerCommercialProlongFrom.Value : null,
                dateTimePickerCommercialProlongTo.Checked ? (DateTime?)dateTimePickerCommercialProlongTo.Value : null,
                checkBoxCommercialProlongUntilDismissal.Checked,
                dateTimePickerCommercialProlongRequest.Value,
                textBoxCommercialProlongGeneralPoint.Text
            );

            textBoxAgreementContent.Text = text;

            _modifications.Add(new Dictionary<ExtModificationTypes, List<ExtModificationParameter>>
            {
                { ExtModificationTypes.CommercialProlong, new List<ExtModificationParameter>
                    {
                        new ExtModificationParameter
                        {
                            Name = "ProlongFrom",
                            Value = ViewportHelper.ValueOrNull(dateTimePickerCommercialProlongFrom)
                        },
                        new ExtModificationParameter
                        {
                            Name = "ProlongTo",
                            Value = ViewportHelper.ValueOrNull(dateTimePickerCommercialProlongTo)
                        },
                        new ExtModificationParameter
                        {
                            Name = "UntilDismissal",
                            Value = checkBoxCommercialProlongUntilDismissal.Checked
                        }
                    } 
                }
            });
        }

        private void vButtonProlongSpecial_Click(object sender, EventArgs e)
        {
            ClearModifyState();
            var text = ((TenancyAgreementsPresenter)Presenter).ProlongSpecialStringBuilder(
                dateTimePickerSpecialProlongFrom.Checked ? (DateTime?)dateTimePickerSpecialProlongFrom.Value : null,
                dateTimePickerSpecialProlongTo.Checked ? (DateTime?)dateTimePickerSpecialProlongTo.Value : null,
                checkBoxSpecialProlongUntilDismissal.Checked,
                textBoxSpecialProlongPoint.Text,
                textBoxSpecialProlongGeneralPoint.Text
            );
            textBoxAgreementContent.Text = text;

            _modifications.Add(new Dictionary<ExtModificationTypes, List<ExtModificationParameter>>
            {
                { ExtModificationTypes.SpecialProlong, new List<ExtModificationParameter>
                    {
                        new ExtModificationParameter
                        {
                            Name = "ProlongFrom",
                            Value = ViewportHelper.ValueOrNull(dateTimePickerSpecialProlongFrom)
                        },
                        new ExtModificationParameter
                        {
                            Name = "ProlongTo",
                            Value = ViewportHelper.ValueOrNull(dateTimePickerSpecialProlongTo)
                        },
                        new ExtModificationParameter
                        {
                            Name = "UntilDismissal",
                            Value = checkBoxSpecialProlongUntilDismissal.Checked
                        }
                    } 
                }
            });
        }

        private void ClearModifyState()
        {
            _includePersons.Clear();
            _excludePersons.Clear();
            _modifications.Clear();
        }

        private void checkBoxSpecialProlongUntilDismissal_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerSpecialProlongTo.Enabled = !checkBoxSpecialProlongUntilDismissal.Checked;
        }

        private void checkBoxCommercialProlongUntilDismissal_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerCommercialProlongTo.Enabled = !checkBoxCommercialProlongUntilDismissal.Checked;
        }
    }
}
