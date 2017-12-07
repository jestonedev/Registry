using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
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
    internal sealed partial class TenancyViewport: FormViewport
    {
        private SelectWarrantForm _swForm;

        private int? _idWarrant;
        private int? _idCopyProcess;

        private TenancyViewport()
            : this(null, null)
        {
        }

        public TenancyViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new TenancyPresenter())
        {
            InitializeComponent();
            dataGridViewTenancyAgreements.AutoGenerateColumns = false;
            dataGridViewTenancyAddress.AutoGenerateColumns = false;
            dataGridViewTenancyPersons.AutoGenerateColumns = false;
            dataGridViewTenancyReasons.AutoGenerateColumns = false;
            dataGridViewRentPeriods.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        private void SetViewportCaption()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (ParentRow == null)
            {
                if (ViewportState == ViewportState.NewRowState)
                    Text = @"Новый найм";
                else
                {
                    Text = row != null ? string.Format(CultureInfo.InvariantCulture, "Процесс найма №{0}", row["id_process"]) : @"Процессы отсутствуют";
                }
                return;
            }
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    if (ViewportState == ViewportState.NewRowState)
                        Text = string.Format(CultureInfo.InvariantCulture, "Новый найм здания №{0}",
                            ParentRow["id_building"]);
                    else
                    {
                        Text = row != null ? 
                            string.Format(CultureInfo.InvariantCulture, "Найм №{0} здания №{1}", row["id_process"], ParentRow["id_building"]) : 
                            string.Format(CultureInfo.InvariantCulture, "Наймы здания №{0} отсутствуют", ParentRow["id_building"]);
                    }
                    break;
                case ParentTypeEnum.Premises:
                    if (ViewportState == ViewportState.NewRowState)
                        Text = string.Format(CultureInfo.InvariantCulture, "Новый найм помещения №{0}",
                            ParentRow["id_premises"]);
                    else
                    {
                        Text = row != null ?
                            string.Format(CultureInfo.InvariantCulture, "Найм №{0} помещения №{1}", row["id_process"], ParentRow["id_premises"]) : 
                            string.Format(CultureInfo.InvariantCulture, "Наймы помещения №{0} отсутствуют", ParentRow["id_premises"]);
                    }
                    break;
                case ParentTypeEnum.SubPremises:
                    if (ViewportState == ViewportState.NewRowState)
                        Text = string.Format(CultureInfo.InvariantCulture, "Новый найм комнаты №{0}",
                            ParentRow["id_sub_premises"]);
                    else
                    {
                        Text = row != null ? 
                            string.Format(CultureInfo.InvariantCulture, "Найм №{0} комнаты №{1}", row["id_process"], ParentRow["sub_premises_num"]) : 
                            string.Format(CultureInfo.InvariantCulture, "Наймы комнаты №{0} отсутствуют", ParentRow["sub_premises_num"]);
                    }
                    break;
                default: throw new ViewportException("Неизвестный тип родительского объекта");
            }
        }

        private void RedrawDataGridRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (var i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
            {
                var row = (DataRowView)Presenter.ViewModel["tenancy_processes_tenancy_persons"].BindingSource[i];
                if (ViewportHelper.ValueOrNull<int>(row, "id_kinship") == 1 && row["exclude_date"] == DBNull.Value)
                {
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 187, 254, 232);
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 72, 215, 143);
                }
                else if (row["exclude_date"] != DBNull.Value)
                {
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 254, 220, 220);
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 215, 72, 72);
                }
                else
                {
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                }
            }
        }

        private void BindWarrantId()
        {
            IsEditable = false;
            var row = Presenter.ViewModel["general"].CurrentRow;
            if ((row != null) && (row["id_warrant"] != DBNull.Value))
            {
                _idWarrant = Convert.ToInt32(row["id_warrant"], CultureInfo.InvariantCulture);
                textBoxSelectedWarrant.Text = ((TenancyPresenter)Presenter).WarrantStringById(_idWarrant.Value);
                vButtonWarrant.Text = @"x";
            }
            else
            {
                _idWarrant = null;
                textBoxSelectedWarrant.Text = "";
                vButtonWarrant.Text = @"...";
            }
            IsEditable = true;
        }

        private void UnbindedCheckBoxesUpdate()
        {
            IsEditable = false;
            var row = Presenter.ViewModel["general"].CurrentRow;
            checkBoxContractEnable.Checked = (row != null) &&
                (row["registration_date"] != DBNull.Value) && (row["registration_num"] != DBNull.Value);
            checkBoxProtocolEnable.Checked = (row != null) && (row["protocol_date"] != DBNull.Value) && (row["protocol_num"] != DBNull.Value);
            checkBoxSubTenancyEnable.Checked = (row != null) && (row["sub_tenancy_date"] != DBNull.Value) && (row["sub_tenancy_num"] != DBNull.Value);
            if ((row != null) && (row["issue_date"] != DBNull.Value))
                dateTimePickerIssueDate.Checked = true;
            else
            {
                dateTimePickerIssueDate.Value = DateTime.Now.Date;
                dateTimePickerIssueDate.Checked = false;
            }
            if ((row != null) && (row["begin_date"] != DBNull.Value))
                dateTimePickerBeginDate.Checked = true;
            else
            {
                dateTimePickerBeginDate.Value = DateTime.Now.Date;
                dateTimePickerBeginDate.Checked = false;
            }
            if ((row != null) && (row["end_date"] != DBNull.Value))
                dateTimePickerEndDate.Checked = true;
            else
            {
                dateTimePickerEndDate.Value = DateTime.Now.Date;
                dateTimePickerEndDate.Checked = false;
            }
            IsEditable = true;
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindSource(comboBoxRentType, Presenter.ViewModel["rent_types"].BindingSource, "rent_type",
                Presenter.ViewModel["rent_types"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxRentType, "SelectedValue", bindingSource,
                Presenter.ViewModel["rent_types"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindProperty(textBoxRegistrationNumber, "Text", bindingSource, "registration_num", "");
            ViewportHelper.BindProperty(dateTimePickerRegistrationDate, "Value", bindingSource, "registration_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(dateTimePickerIssueDate, "Value", bindingSource, "issue_date", null);
            ViewportHelper.BindProperty(dateTimePickerBeginDate, "Value", bindingSource, "begin_date", null);
            ViewportHelper.BindProperty(dateTimePickerEndDate, "Value", bindingSource, "end_date", null);
            ViewportHelper.BindProperty(textBoxProtocolNumber, "Text", bindingSource, "protocol_num", "");
            ViewportHelper.BindProperty(dateTimePickerProtocolDate, "Value", bindingSource, "protocol_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxDescription, "Text", bindingSource, "description", "");
            ViewportHelper.BindProperty(dateTimePickerSubTenancyDate, "Value", bindingSource, "sub_tenancy_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxSubTenancyNumber, "Text", bindingSource, "sub_tenancy_num", "");

            ViewportHelper.BindSource(comboBoxExecutor, Presenter.ViewModel["executors"].BindingSource, "executor_name",
                Presenter.ViewModel["executors"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxExecutor, "SelectedValue", bindingSource,
                Presenter.ViewModel["executors"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindProperty(checkBoxUntilDismissal, "Checked", bindingSource, "until_dismissal", false);

            dataGridViewTenancyPersons.DataSource = Presenter.ViewModel["tenancy_processes_tenancy_persons"].BindingSource;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            date_of_birth.DataPropertyName = "date_of_birth";
            ViewportHelper.BindSource(id_kinship, Presenter.ViewModel["kinships"].BindingSource, "kinship",
                Presenter.ViewModel["kinships"].PrimaryKeyFirst);

            dataGridViewTenancyAgreements.DataSource = Presenter.ViewModel["tenancy_processes_tenancy_agreements"].BindingSource;
            agreement_date.DataPropertyName = "agreement_date";
            agreement_content.DataPropertyName = "agreement_content";

            dataGridViewTenancyReasons.DataSource = Presenter.ViewModel["tenancy_processes_tenancy_reasons"].BindingSource;
            reason_date.DataPropertyName = "reason_date";
            reason_number.DataPropertyName = "reason_number";
            reason_prepared.DataPropertyName = "reason_prepared";

            dataGridViewTenancyAddress.DataSource = Presenter.ViewModel["tenancy_premises_info"].BindingSource;
            address.DataPropertyName = "address";
            total_area.DataPropertyName = "total_area";
            living_area.DataPropertyName = "living_area";
            rent_area.DataPropertyName = "rent_area";

            dataGridViewRentPeriods.DataSource = Presenter.ViewModel["tenancy_processes_tenancy_rent_periods_history"].BindingSource;
            rent_periods_begin_date.DataPropertyName = "begin_date";
            rent_periods_end_date.DataPropertyName = "end_date";
            rent_periods_until_dismissal.DataPropertyName = "until_dismissal";
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.TenancyWrite)) 
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidateTenancy(TenancyProcess tenancy)
        {
            if (tenancy.IdRentType == null)
            {
                MessageBox.Show(@"Необходимо выбрать тип найма", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxRentType.Focus();
                return false;
            }
            if (checkBoxContractEnable.Checked && tenancy.RegistrationNum == null)
            {
                MessageBox.Show(@"Не указан номер договора найма", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                textBoxRegistrationNumber.Focus();
                return false;
            }
            if (checkBoxProtocolEnable.Checked && tenancy.ProtocolNum == null)
            {
                MessageBox.Show(@"Не указан номер протокола жилищной комиссии", @"Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxProtocolNumber.Focus();
                return false;
            }
            if (checkBoxSubTenancyEnable.Checked && tenancy.SubTenancyNum == null)
            {
                MessageBox.Show(@"Не указан номер реквизита на сдачу в поднаем", @"Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxSubTenancyNumber.Focus();
                return false;
            }
            var tenancyFromView = (TenancyProcess) EntityFromView();
            if (tenancy.RegistrationNum != null && tenancy.RegistrationNum != tenancyFromView.RegistrationNum)
                if (TenancyService.TenancyProcessesDuplicateCount(tenancy) != 0 &&
                    MessageBox.Show(@"В базе уже имеется договор с таким номером. Все равно продолжить сохранение?", @"Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
            // Проверить соответствие вида найма
            if (ParentRow == null) return true;
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    if (!OtherService.BuildingFundAndRentMatch((int)ParentRow["id_building"], tenancy.IdRentType.Value) &&
                        MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемого здания. Все равно продолжить сохранение?",
                            @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                        return false;
                    break;
                case ParentTypeEnum.Premises:
                    if (!OtherService.PremiseFundAndRentMatch((int)ParentRow["id_premises"], tenancy.IdRentType.Value))
                    {
                        if (!OtherService.BuildingFundAndRentMatch((int)ParentRow["id_building"], tenancy.IdRentType.Value) &&
                            MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемого помещения. Все равно продолжить сохранение?",
                                @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                            return false;
                    }
                    break;
                case ParentTypeEnum.SubPremises:
                    if (!OtherService.SubPremiseFundAndRentMatch((int)ParentRow["id_sub_premises"], tenancy.IdRentType.Value))
                    {
                        if (!OtherService.PremiseFundAndRentMatch((int)ParentRow["id_premises"], tenancy.IdRentType.Value))
                        {
                            var idBuilding = (int)EntityDataModel<Premise>.GetInstance().Select().Rows.Find((int)ParentRow["id_premises"])["id_building"];
                            if (!OtherService.BuildingFundAndRentMatch(idBuilding, tenancy.IdRentType.Value) &&
                                MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемой комнаты. Все равно продолжить сохранение?",
                                    @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                                return false;
                        }
                    }
                    break;
            }
            return true;
        }

        protected override Entity EntityFromView()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null) return new TenancyProcess();
            var process = EntityConverter<TenancyProcess>.FromRow(row);
            if (Presenter.ViewModel["executors"].Model.FilterDeletedRows().Where(v => v.Field<short?>("is_inactive") == 0).
                        All(v => v.Field<int?>("id_executor") != process.IdExecutor))
            {
                process.IdExecutor = null;
            }
            return process;
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var tenancy = new TenancyProcess
            {
                IdProcess = row == null ? null : ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdRentType = ViewportHelper.ValueOrNull<int>(comboBoxRentType),
                IdExecutor = ViewportHelper.ValueOrNull<int>(comboBoxExecutor),
                Description = ViewportHelper.ValueOrNull(textBoxDescription)
            };
            if (checkBoxContractEnable.Checked)
            {
                tenancy.IdWarrant = _idWarrant;
                tenancy.RegistrationNum = ViewportHelper.ValueOrNull(textBoxRegistrationNumber);
                tenancy.RegistrationDate = dateTimePickerRegistrationDate.Value.Date;
                tenancy.IssueDate = ViewportHelper.ValueOrNull(dateTimePickerIssueDate);
                tenancy.BeginDate = ViewportHelper.ValueOrNull(dateTimePickerBeginDate);
                if (checkBoxUntilDismissal.Checked)
                {
                    tenancy.EndDate = null;
                    tenancy.UntilDismissal = true;
                }
                else
                {
                    tenancy.EndDate = ViewportHelper.ValueOrNull(dateTimePickerEndDate);
                    tenancy.UntilDismissal = false;
                }
            }
            else
            {
                tenancy.IdWarrant = null;
                tenancy.RegistrationNum = null;
                tenancy.RegistrationDate = null;
                tenancy.IssueDate = null;
                tenancy.BeginDate = null;
                tenancy.EndDate = null;
                tenancy.UntilDismissal = false;
            }
            if (checkBoxSubTenancyEnable.Checked)
            {
                tenancy.SubTenancyDate = ViewportHelper.ValueOrNull(dateTimePickerSubTenancyDate);
                tenancy.SubTenancyNum = ViewportHelper.ValueOrNull(textBoxSubTenancyNumber);
            }
            else
            {
                tenancy.SubTenancyDate = null;
                tenancy.SubTenancyNum = null;
            }
            // Отклики из прошлого, раньше была возможность менять ордер на вкладке процесса найма, убрано из-за плохой согласованности с основаниями найма
            if (ViewportState != ViewportState.NewRowState)
            {
                var reasons =
                    (from reasonRow in EntityDataModel<TenancyReason>.GetInstance().FilterDeletedRows() 
                    where row != null && 
                        reasonRow.Field<int>("id_process") == (int) row["id_process"] 
                    select new
                    {
                        number = reasonRow.Field<string>("reason_number"),
                        date = reasonRow.Field<DateTime?>("reason_date")
                    }).ToList();

                if (reasons.Any())
                {

                    var reason = reasons.Last();
                    tenancy.ResidenceWarrantNum = reason.number;
                    tenancy.ResidenceWarrantDate = reason.date;
                }
            }
            //
            if (checkBoxProtocolEnable.Checked)
            {
                tenancy.ProtocolNum = ViewportHelper.ValueOrNull(textBoxProtocolNumber);
                tenancy.ProtocolDate = dateTimePickerProtocolDate.Value.Date;
            }
            else
            {
                tenancy.ProtocolNum = null;
                tenancy.ProtocolDate = null;
            }
            return tenancy;
        }

        private void ViewportFromTenancy(TenancyProcess tenancy)
        {
            comboBoxRentType.SelectedValue = ViewportHelper.ValueOrDbNull(tenancy.IdRentType);
            comboBoxExecutor.SelectedValue = ViewportHelper.ValueOrDbNull(tenancy.IdExecutor);
            textBoxRegistrationNumber.Text = tenancy.RegistrationNum;
            dateTimePickerRegistrationDate.Value = ViewportHelper.ValueOrDefault(tenancy.RegistrationDate);
            dateTimePickerIssueDate.Value = ViewportHelper.ValueOrDefault(tenancy.IssueDate);
            dateTimePickerIssueDate.Checked = tenancy.IssueDate != null;
            dateTimePickerBeginDate.Value = ViewportHelper.ValueOrDefault(tenancy.BeginDate);
            dateTimePickerBeginDate.Checked = tenancy.BeginDate != null;
            dateTimePickerEndDate.Value = ViewportHelper.ValueOrDefault(tenancy.EndDate);
            dateTimePickerEndDate.Checked = (tenancy.EndDate != null) && (tenancy.UntilDismissal != true);
            checkBoxUntilDismissal.Checked = tenancy.UntilDismissal != null && tenancy.UntilDismissal.Value;
            textBoxProtocolNumber.Text = tenancy.ProtocolNum;
            dateTimePickerProtocolDate.Value = ViewportHelper.ValueOrDefault(tenancy.ProtocolDate);
            textBoxDescription.Text = tenancy.Description;
            checkBoxSubTenancyEnable.Checked = tenancy.SubTenancyDate != null && tenancy.SubTenancyNum != null;
            dateTimePickerSubTenancyDate.Value = ViewportHelper.ValueOrDefault(tenancy.SubTenancyDate);
            textBoxSubTenancyNumber.Text = tenancy.SubTenancyNum;
            if (tenancy.IdWarrant != null)
            {
                textBoxSelectedWarrant.Text = ((TenancyPresenter)Presenter).WarrantStringById(tenancy.IdWarrant.Value);
                _idWarrant = tenancy.IdWarrant;
            }
            else
            {
                textBoxSelectedWarrant.Text = "";
                _idWarrant = null;
            }
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

            ((TenancyPresenter)Presenter).AddAssocViewModelItem();
            StaticFilter = ((TenancyPresenter)Presenter).GetStaticFilter();
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            DataBind();

            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", v_tenancies_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["tenancy_processes_tenancy_persons"].DataSource, 
                "RowChanged", TenancyPersons_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["tenancy_processes_tenancy_persons"].DataSource, 
                "RowDeleted", TenancyPersons_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", TenancyViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", TenancyViewport_RowDeleted);

            if (ParentRow != null)
            {
                AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowChanged", TenancyAssocViewport_RowChanged);
                AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowDeleted", TenancyAssocViewport_RowDeleted);
            }

            AddEventHandler<ListChangedEventArgs>(Presenter.ViewModel["tenancy_processes_tenancy_persons"].BindingSource, 
                "ListChanged", v_persons_ListChanged);

            AddEventHandler<EventArgs>(Presenter.ViewModel["tenancy_premises_info"].Model, "RefreshEvent", tenancy_premises_info_RefreshEvent);
            AddEventHandler<EventArgs>(Presenter.ViewModel["tenancy_payments_info"].Model, "RefreshEvent", tenancy_payments_info_RefreshEvent);
            
            FiltersRebuild();
            v_tenancies_CurrentItemChanged(null, new EventArgs());
            DataChangeHandlersInit();
            IsEditable = true;
        }

        private void FiltersRebuild()
        {
            ((TenancyPresenter)Presenter).FiltersRebuild();
            var currentRow = Presenter.ViewModel["general"].CurrentRow;
            if (currentRow == null || currentRow["id_process"] == DBNull.Value)
            {
                numericUpDownPayment.Value = 0;
                return;
            }
            var payments =
                from row in Presenter.ViewModel["tenancy_payments_info"].Model.FilterDeletedRows()
                where (int)row["id_process"] == (int)currentRow["id_process"]
                select (decimal) row["payment"];
            var payment = payments.Sum(v => v);
            numericUpDownPayment.Value = payment;
        }

        private void tenancy_premises_info_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        private void tenancy_payments_info_RefreshEvent(object sender, EventArgs e)
        {
            FiltersRebuild();
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            var executors = Presenter.ViewModel["executors"].BindingSource;
            var index = executors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)executors[index])["id_executor"];
            _idCopyProcess = null;
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
        }

        public override bool CanCopyRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) && 
                !Presenter.ViewModel["general"].Model.EditingNewRecord && 
                AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            var tenancy = (TenancyProcess)EntityFromView();
            Presenter.ViewModel["general"].BindingSource.AddNew();
            ViewportFromTenancy(tenancy);

            checkBoxContractEnable.Checked = (tenancy.RegistrationDate != null) || (tenancy.RegistrationNum != null);
            checkBoxProtocolEnable.Checked = tenancy.ProtocolDate != null;
            dateTimePickerIssueDate.Checked = tenancy.IssueDate != null;
            dateTimePickerBeginDate.Checked = tenancy.BeginDate != null;
            dateTimePickerEndDate.Checked = tenancy.EndDate != null;
            
            var executors = Presenter.ViewModel["executors"].BindingSource;
            var index = executors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)executors[index])["id_executor"];
            _idCopyProcess = tenancy.IdProcess;
            IsEditable = true;
        }

        public override bool SearchedRecords()
        {
            return !string.IsNullOrEmpty(DynamicFilter);
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
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
            IsEditable = false;
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            IsEditable = true;
        }

        public override void ClearSearch()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].BindingSource.Filter = StaticFilter;
            IsEditable = true;
            DynamicFilter = "";
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null)
                && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить этот процесс найма?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            IsEditable = false;
            if (!((TenancyPresenter)Presenter).DeleteRecord())
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
                    GeneralDataModel.EditingNewRecord = false;
                    if (Presenter.ViewModel["general"].CurrentRow != null)
                    {
                        IsEditable = false;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    BindWarrantId();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            _idCopyProcess = null;
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            var tenancy = (TenancyProcess) EntityFromViewport();
            if (!ValidateTenancy(tenancy))
                return;
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((TenancyPresenter)Presenter).InsertRecord(tenancy, _idCopyProcess))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((TenancyPresenter)Presenter).UpdateRecord(tenancy))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.TenancyPersonsViewport,
                ViewportType.TenancyReasonsViewport,
                ViewportType.TenancyBuildingsViewport,
                ViewportType.TenancyPremisesViewport,
                ViewportType.TenancyAgreementsViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (Presenter.ViewModel["general"].CurrentRow != null);
        }

        public override void ShowAssocViewport<T>()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var row = Presenter.ViewModel["general"].CurrentRow;
            var columnName = Presenter.ViewModel["general"].PrimaryKeyFirst;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback,
                columnName + " = " + Convert.ToInt32(row[columnName], CultureInfo.InvariantCulture), row.Row,
                ParentTypeEnum.Tenancy);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
                return false;
            var idProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            var idRentType = ViewportHelper.ValueOrNull<int>(row, "id_rent_type");
            switch (reporterType)
            {
                case ReporterType.TenancyContractCommercialReporter:
                    return idProcess != null && idRentType == 1;
                case ReporterType.TenancyContractSocialReporter:
                    return idProcess != null && idRentType == 3;
                case ReporterType.TenancyContractSpecial1711Reporter:
                case ReporterType.TenancyContractSpecial1712Reporter:
                    return idProcess != null && idRentType == 2;
                case ReporterType.TenancyActToEmploymentReporter:
                case ReporterType.TenancyActFromEmploymentReporter:
                case ReporterType.TenancyNotifyDocumentsPrepared:
                case ReporterType.TenancyNotifyContractViolation:
                case ReporterType.TenancyNotifyIllegalResident:
                case ReporterType.TenancyNotifyNoProlongTrouble:
                case ReporterType.TenancyNotifyNoProlongCategory:
                case ReporterType.RequestToMvdReporter:
                case ReporterType.RequestToMvdNewReporter:
                case ReporterType.DistrictCommitteePreContractReporter:
                case ReporterType.ContractDksrReporter:
                case ReporterType.ExportReasonsForGisZkhReporter:
                    return idProcess != null;
                case ReporterType.TenancyAgreementReporter:
                    return idProcess != null && (TenancyService.TenancyAgreementsCountForProcess(idProcess.Value) > 0);
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate(reporterType))
                return;
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.TenancyContractCommercialReporter:
                case ReporterType.TenancyContractSocialReporter:
                case ReporterType.TenancyContractSpecial1711Reporter:
                case ReporterType.TenancyContractSpecial1712Reporter:
                case ReporterType.RequestToMvdReporter:
                case ReporterType.RequestToMvdNewReporter:
                case ReporterType.DistrictCommitteePreContractReporter:
                case ReporterType.ContractDksrReporter:
                case ReporterType.ExportReasonsForGisZkhReporter:
                    arguments = TenancyContractReporterArguments();
                    break;
                case ReporterType.TenancyActToEmploymentReporter:
                case ReporterType.TenancyActFromEmploymentReporter:
                    arguments = TenancyActReporterArguments();
                    break;
                case ReporterType.TenancyNotifyDocumentsPrepared:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "1");
                    break;
                case ReporterType.TenancyNotifyContractViolation:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "3");  // Report type 2 used into TenancyAgreementsViewport
                    break;
                case ReporterType.TenancyNotifyIllegalResident:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "4");
                    break;
                case ReporterType.TenancyNotifyNoProlongTrouble:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "5");
                    break;
                case ReporterType.TenancyNotifyNoProlongCategory:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "6");
                    break;
                case ReporterType.TenancyAgreementReporter:
                    if (Presenter.ViewModel["tenancy_processes_tenancy_agreements"].CurrentRow == null)
                    {
                        MessageBox.Show(@"Не выбрано соглашение для печати",
                            @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    arguments = TenancyAgreementReporterArguments();
                    break;
            }
            MenuCallback.RunReport(reporterType, arguments);
        }
        
        private Dictionary<string, string> TenancyContractReporterArguments()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return new Dictionary<string, string> { { "id_process", row["id_process"].ToString() } };
        }

        private Dictionary<string, string> TenancyActReporterArguments()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return new Dictionary<string, string> {{"id_process", row["id_process"].ToString()}};
        }

        private Dictionary<string, string> TenancyAgreementReporterArguments()
        {
            var row = Presenter.ViewModel["tenancy_processes_tenancy_agreements"].CurrentRow;
            return new Dictionary<string, string> {{"id_agreement", row["id_agreement"].ToString()}};
        }

        private bool TenancyValidForReportGenerate(ReporterType reporterType)
        {
            if (reporterType == ReporterType.ExportReporter)
            {
                return true;
            }
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
                return false;
            if (reporterType == ReporterType.RequestToMvdReporter ||
                reporterType == ReporterType.TenancyNotifyIllegalResident ||
                reporterType == ReporterType.DistrictCommitteePreContractReporter ||
                reporterType == ReporterType.ContractDksrReporter)
            {
                return true;
            }
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (!TenancyService.TenancyProcessHasTenant(Convert.ToInt32(row["id_process"], CultureInfo.InvariantCulture)))
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо указать нанимателя процесса найма", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(row, "registration_date") == null || ViewportHelper.ValueOrNull(row, "registration_num") == null)
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            checkBoxProtocolEnable.Focus();
            base.OnVisibleChanged(e);
        }

        private void v_persons_ListChanged(object sender, ListChangedEventArgs e)
        {
            RedrawDataGridRows();
        }

        private void v_tenancies_CurrentItemChanged(object sender, EventArgs e)
        {
            var isEditable = IsEditable;
            SetViewportCaption();
            UnbindedCheckBoxesUpdate();
            BindWarrantId();
            UpdateDuplicateContractInfo();
            FiltersRebuild();
            IsEditable = isEditable;
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.DocumentsStateUpdate();
        }

        private void TenancyViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                UpdateDuplicateContractInfo();
                UnbindedCheckBoxesUpdate();
            }
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void TenancyViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UpdateDuplicateContractInfo();
            UnbindedCheckBoxesUpdate();
            CheckViewportModifications();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void TenancyPersons_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            RedrawDataGridRows();
            UpdateDuplicateContractInfo();
        }

        private void TenancyPersons_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridRows();
            UpdateDuplicateContractInfo();
        }

        private void UpdateDuplicateContractInfo()
        {
            var hasDuplicate = ((TenancyPresenter)Presenter).HasContractDuplicates();
            labelDuplicateContract.Visible = hasDuplicate;
            if (hasDuplicate)
            {
                textBoxDescription.Height = 47;
                labelPayment.Top = 72;
                numericUpDownPayment.Top = 70;
                buttonShowAttachments.Top = 96;
            }
            else
            {
                textBoxDescription.Height = 62;
                labelPayment.Top = 87;
                numericUpDownPayment.Top = 85;
                buttonShowAttachments.Top = 111;
            }
            
        }

        private void TenancyAssocViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            StaticFilter = ((TenancyPresenter)Presenter).GetStaticFilter();
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
        }

        private void TenancyAssocViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            StaticFilter = ((TenancyPresenter)Presenter).GetStaticFilter();
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
        }

        private void vButtonWarrant_Click(object sender, EventArgs e)
        {
            if (_idWarrant != null)
            {
                _idWarrant = null;
                textBoxSelectedWarrant.Text = "";
                vButtonWarrant.Text = @"...";
                return;
            }
            if (_swForm == null)
                _swForm = new SelectWarrantForm();
            if (_swForm.ShowDialog() != DialogResult.OK) return;
            if (_swForm.WarrantId == null) return;
            _idWarrant = _swForm.WarrantId.Value;
            textBoxSelectedWarrant.Text = ((TenancyPresenter)Presenter).WarrantStringById(_swForm.WarrantId.Value);
            vButtonWarrant.Text = @"x";
        }

        private void checkBoxProtocolEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxProtocol.Controls)
                if (control != checkBoxProtocolEnable)
                    control.Enabled = checkBoxProtocolEnable.Checked;
        }

        private void checkBoxProcessEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxTenancyContract.Controls)
                if (control != checkBoxContractEnable)
                    control.Enabled = checkBoxContractEnable.Checked;
        }

        private void dataGridViewTenancyAgreements_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!HasAssocViewport<TenancyAgreementsViewport>())
                return;
            if (e.RowIndex == -1)
                return;
            ShowAssocViewport<TenancyAgreementsViewport>();
        }

        private void dataGridViewTenancyPersons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!HasAssocViewport<TenancyPersonsViewport>())
                return;
            if (e.RowIndex == -1)
                return;
            ShowAssocViewport<TenancyPersonsViewport>();
        }

        private void dataGridViewTenancyAddress_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocViewport<TenancyPremisesViewport>())
                ShowAssocViewport<TenancyPremisesViewport>();
        }

        private void dataGridViewTenancyReasons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocViewport<TenancyReasonsViewport>())
                ShowAssocViewport<TenancyReasonsViewport>();
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        private void checkBoxUntilDismissal_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerEndDate.Enabled = !checkBoxUntilDismissal.Checked;
        }

        private void vButtonRentPeriodAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            using (var form = new RentPeriodsEditor())
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                ((TenancyPresenter)Presenter).AddRentPeriod(form.BeginDate, form.EndDate, form.UntilDismissal);
            }
        }

        private void vButtonRentPeriodDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            ((TenancyPresenter)Presenter).DeleteCurrentRentPeriod();
        }

        private void vButtonSwapRentPeriod_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            ((TenancyPresenter)Presenter).SwapRentPeriod();
            dateTimePickerBeginDate.Value = dateTimePickerEndDate.Value;
            dateTimePickerBeginDate.Checked = dateTimePickerEndDate.Checked;
            dateTimePickerEndDate.Checked = false;
            dateTimePickerEndDate.Focus();
        }

        private void checkBoxSubTenancyEnable_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxSubTenancy.Enabled = checkBoxSubTenancyEnable.Checked;
        }

        private void buttonShowAttachments_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (Presenter.ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбран процесс найма для отображения прикрепленных файлов", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var form = new TenancyFiles())
            {
                form.Initialize((int)Presenter.ViewModel["general"].CurrentRow["id_process"]);
                form.ShowDialog();
            }
        }

        internal int GetCurrentId()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var columnName = Presenter.ViewModel["general"].PrimaryKeyFirst;
            if (row == null) return -1;
            if (row[columnName] != DBNull.Value)
                return (int)row[columnName];
            return -1;
        }

        internal IEnumerable<int> GetFilteredIds()
        {
            var ids = new List<int>();
            if (Presenter.ViewModel["general"].BindingSource.Position < 0) return ids;
            for (var i = 0; i < Presenter.ViewModel["general"].BindingSource.Count; i++)
            {
                var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[i];
                if (row["id_premises"] != DBNull.Value)
                    ids.Add((int)row["id_premises"]);
            }
            return ids;
        }
    }
}
