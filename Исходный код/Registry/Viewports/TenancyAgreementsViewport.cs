using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Declensions.Unicode;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyAgreementsViewport : FormWithGridViewport
    {
        #region Modeles
        DataModel tenancy_persons;
        DataModel executors;
        DataModel warrants;
        DataModel kinships;
        #endregion Modeles

        #region Views
        BindingSource v_tenancy_persons;
        BindingSource v_executors;
        BindingSource v_warrants;
        BindingSource v_kinships;
        #endregion Views

        //Forms
        private SelectWarrantForm swForm;
        
        private int? id_warrant;
        private bool is_first_visible = true;   // первое отображение формы
        private List<TenancyPerson> excludePersons = new List<TenancyPerson>(); 
        private List<TenancyPerson> includePersons = new List<TenancyPerson>(); 

        private TenancyAgreementsViewport()
            : this(null, null)
        {
        }

        public TenancyAgreementsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private void RedrawDataGridTenancyPersonsRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (var i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
                if (((DataRowView)v_tenancy_persons[i])["id_kinship"] != DBNull.Value &&
                    Convert.ToInt32(((DataRowView)v_tenancy_persons[i])["id_kinship"], CultureInfo.InvariantCulture) == 1)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
        }

        private void DataBind()
        {
            comboBoxExecutor.DataSource = v_executors;
            comboBoxExecutor.ValueMember = "id_executor";
            comboBoxExecutor.DisplayMember = "executor_name";
            comboBoxExecutor.DataBindings.Clear();
            comboBoxExecutor.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_executor", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxIncludeKinship.DataSource = v_kinships;
            comboBoxIncludeKinship.ValueMember = "id_kinship";
            comboBoxIncludeKinship.DisplayMember = "kinship";

            textBoxAgreementContent.DataBindings.Clear();
            textBoxAgreementContent.DataBindings.Add("Text", GeneralBindingSource, "agreement_content", true, DataSourceUpdateMode.Never, "");

            dateTimePickerAgreementDate.DataBindings.Clear();
            dateTimePickerAgreementDate.DataBindings.Add("Value", GeneralBindingSource, "agreement_date", true, DataSourceUpdateMode.Never, DateTime.Now);

            dataGridViewTenancyPersons.DataSource = v_tenancy_persons;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            date_of_birth.DataPropertyName = "date_of_birth";

            dataGridView.DataSource = GeneralBindingSource;
            id_agreement.DataPropertyName = "id_agreement";
            agreement_date.DataPropertyName = "agreement_date";
            agreement_content.DataPropertyName = "agreement_content";
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.TenancyWrite)) return base.ChangeViewportStateTo(state);
            viewportState = ViewportState.ReadState;
            return true;
        }

        private string WarrantStringById(int idWarrant)
        {
            if (v_warrants.Position == -1)
                return null;
            else
            {
                var row_index = v_warrants.Find("id_warrant", idWarrant);
                if (row_index == -1)
                    return null;
                var registration_date = Convert.ToDateTime(((DataRowView)v_warrants[row_index])["registration_date"], CultureInfo.InvariantCulture);
                var registration_num = ((DataRowView)v_warrants[row_index])["registration_num"].ToString();
                return string.Format(CultureInfo.InvariantCulture, "№ {0} от {1}",
                    registration_num, registration_date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            }
        }

        private void BindWarrantId()
        {
            if ((GeneralBindingSource.Position > -1) && ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_warrant"] != DBNull.Value)
            {
                id_warrant = Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_warrant"], CultureInfo.InvariantCulture);
                textBoxAgreementWarrant.Text =
                    WarrantStringById(id_warrant.Value);
                vButtonSelectWarrant.Text = "x";
            }
            else
            {
                id_warrant = null;
                textBoxAgreementWarrant.Text = "";
                vButtonSelectWarrant.Text = "...";
            }
        }

        private void ViewportFromTenancyAgreement(TenancyAgreement tenancyAgreement)
        {
            comboBoxExecutor.SelectedValue = ViewportHelper.ValueOrDBNull(tenancyAgreement.IdExecutor);
            dateTimePickerAgreementDate.Value = ViewportHelper.ValueOrDefault(tenancyAgreement.AgreementDate);
            textBoxAgreementContent.Text = tenancyAgreement.AgreementContent;
            if (tenancyAgreement.IdWarrant != null)
            {
                textBoxAgreementWarrant.Text = WarrantStringById(tenancyAgreement.IdWarrant.Value);
                id_warrant = tenancyAgreement.IdWarrant;
            }
            else
            {
                textBoxAgreementWarrant.Text = "";
                id_warrant = null;
            }
        }

        protected override Entity EntityFromViewport()
        {
            var tenancyAgreement = new TenancyAgreement();
            if (GeneralBindingSource.Position == -1)
                tenancyAgreement.IdAgreement = null;
            else
                tenancyAgreement.IdAgreement = ViewportHelper.ValueOrNull<int>((DataRowView)GeneralBindingSource[GeneralBindingSource.Position], "id_agreement");
            if (ParentType == ParentTypeEnum.Tenancy && ParentRow != null)
                tenancyAgreement.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
            else
                tenancyAgreement.IdProcess = null;
            tenancyAgreement.IdExecutor = ViewportHelper.ValueOrNull<int>(comboBoxExecutor);
            tenancyAgreement.IdWarrant = id_warrant;
            tenancyAgreement.AgreementContent = textBoxAgreementContent.Text;
            tenancyAgreement.AgreementDate = ViewportHelper.ValueOrNull(dateTimePickerAgreementDate);
            return tenancyAgreement;
        }

        protected override Entity EntityFromView()
        {
            var tenancyAgreement = new TenancyAgreement();
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            tenancyAgreement.IdAgreement = ViewportHelper.ValueOrNull<int>(row, "id_agreement");
            tenancyAgreement.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            tenancyAgreement.IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor");
            tenancyAgreement.IdWarrant = ViewportHelper.ValueOrNull<int>(row, "id_warrant");
            tenancyAgreement.AgreementDate = ViewportHelper.ValueOrNull<DateTime>(row, "agreement_date");
            tenancyAgreement.AgreementContent = ViewportHelper.ValueOrNull(row, "agreement_content");
            return tenancyAgreement;
        }

        private static void FillRowFromAgreement(TenancyAgreement tenancyAgreement, DataRowView row)
        {
            row.BeginEdit();
            row["id_agreement"] = ViewportHelper.ValueOrDBNull(tenancyAgreement.IdAgreement);
            row["id_process"] = ViewportHelper.ValueOrDBNull(tenancyAgreement.IdProcess);
            row["agreement_date"] = ViewportHelper.ValueOrDBNull(tenancyAgreement.AgreementDate);
            row["agreement_content"] = ViewportHelper.ValueOrDBNull(tenancyAgreement.AgreementContent);
            row["id_executor"] = ViewportHelper.ValueOrDBNull(tenancyAgreement.IdExecutor);
            row["id_warrant"] = ViewportHelper.ValueOrDBNull(tenancyAgreement.IdWarrant);
            row.EndEdit();
        }

        private bool ValidateAgreement(TenancyAgreement tenancyAgreement)
        {
            if (tenancyAgreement.IdExecutor == null)
            {
                MessageBox.Show("Необходимо выбрать исполнителя", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxExecutor.Focus();
                return false;
            }
            return true;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            dataGridViewTenancyPersons.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.TenancyAgreementsDataModel);
            tenancy_persons = DataModel.GetInstance(DataModelType.TenancyPersonsDataModel);
            executors = DataModel.GetInstance(DataModelType.ExecutorsDataModel);
            warrants = DataModel.GetInstance(DataModelType.WarrantsDataModel);
            kinships = DataModel.GetInstance(DataModelType.KinshipsDataModel);

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            tenancy_persons.Select();
            executors.Select();
            warrants.Select();
            kinships.Select();

            var ds = DataModel.DataSet;

            if ((ParentType == ParentTypeEnum.Tenancy) && (ParentRow != null))
                Text = string.Format(CultureInfo.InvariantCulture, "Соглашения найма №{0}", ParentRow["id_process"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_tenancy_persons = new BindingSource();
            v_tenancy_persons.DataMember = "tenancy_persons";
            v_tenancy_persons.Filter = StaticFilter;
            v_tenancy_persons.DataSource = ds;

            v_executors = new BindingSource();
            v_executors.DataMember = "executors";
            v_executors.DataSource = ds;
            v_executors.Filter = "is_inactive = 0";

            v_warrants = new BindingSource();
            v_warrants.DataMember = "warrants";
            v_warrants.DataSource = ds;

            v_kinships = new BindingSource();
            v_kinships.DataMember = "kinships";
            v_kinships.DataSource = ds;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataMember = "tenancy_agreements";
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = ds;
            GeneralDataModel.Select().RowDeleted += TenancyAgreementsViewport_RowDeleted;
            GeneralDataModel.Select().RowChanged += TenancyAgreementsViewport_RowChanged;

            DataBind();
            tenancy_persons.Select().RowDeleted += TenancyPersonsViewport_RowDeleted;
            tenancy_persons.Select().RowChanged += TenancyPersonsViewport_RowChanged;
            is_editable = true;
            DataChangeHandlersInit();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите это соглашение?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_agreement"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
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
                        dataGridView.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        if (GeneralBindingSource.Position != -1)
                            dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                    }
                    else
                        Text = "Соглашения отсутствуют";
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    BindWarrantId();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            is_editable = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                 && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            var tenancyAgreement = (TenancyAgreement) EntityFromViewport();
            if (!ValidateAgreement(tenancyAgreement))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                case ViewportState.NewRowState:
                    var id_agreement = GeneralDataModel.Insert(tenancyAgreement);
                    if (id_agreement == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    tenancyAgreement.IdAgreement = id_agreement;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    FillRowFromAgreement(tenancyAgreement, newRow);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancyAgreement.IdAgreement == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить соглашение без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(tenancyAgreement) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    is_editable = false;
                    FillRowFromAgreement(tenancyAgreement, row);
                    break;
            }
            viewportState = ViewportState.ReadState;
            dataGridView.Enabled = true;
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            if (includePersons.Count > 0)
            {
                new TenancyAgreementOnSavePersonManager(includePersons,
                    TenancyAgreementOnSavePersonManager.PersonsOperationType.IncludePersons).ShowDialog();
                includePersons.Clear();
            }
            if (excludePersons.Count > 0)
            {
                new TenancyAgreementOnSavePersonManager(excludePersons,
                    TenancyAgreementOnSavePersonManager.PersonsOperationType.ExcludePersons).ShowDialog();
                excludePersons.Clear();
            }
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            var index = v_executors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)v_executors[index])["id_executor"];
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                textBoxAgreementContent.Text = string.Format(CultureInfo.InvariantCulture, "1.1 По настоящему Соглашению Стороны по договору № {0} от {1} договорились:",
                    ParentRow["registration_num"],
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");
            is_editable = true;
            GeneralDataModel.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var tenancyAgreement = (TenancyAgreement) EntityFromView();
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromTenancyAgreement(tenancyAgreement);
            is_editable = true;
        }

        public override bool HasReport(ReporterType reporterType)
        {
            return reporterType == ReporterType.TenancyAgreementReporter && 
                (GeneralBindingSource.Position > -1);
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано соглашение для печати",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            ReporterFactory.CreateReporter(ReporterType.TenancyAgreementReporter).Run(
                new Dictionary<string, string> { { "id_agreement", row["id_agreement"].ToString() } });
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (ParentType != ParentTypeEnum.Tenancy)
            {
                MessageBox.Show("Некорректный родительский объект",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (!DataModelHelper.TenancyProcessHasTenant(Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture)))
            {
                MessageBox.Show("Для формирования отчетной документации необходимо указать нанимателя процесса найма", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(ParentRow, "registration_date") == null || ViewportHelper.ValueOrNull(ParentRow, "registration_num") == null)
            {
                MessageBox.Show("Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                GeneralBindingSource.CurrentItemChanged -= GeneralBindingSource_CurrentItemChanged;
                tenancy_persons.Select().RowDeleted -= TenancyPersonsViewport_RowDeleted;
                tenancy_persons.Select().RowChanged -= TenancyPersonsViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= TenancyAgreementsViewport_RowDeleted;
                GeneralDataModel.Select().RowChanged -= TenancyAgreementsViewport_RowChanged;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            Close();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
            if (is_first_visible)
            {
                is_first_visible = false;
                if (GeneralBindingSource.Count == 0)
                    InsertRecord();
            }
            vButtonSelectWarrant.Focus();
            base.OnVisibleChanged(e);
        }

        void vButtonSelectWarrant_Click(object sender, EventArgs e)
        {
            if (id_warrant != null)
            {
                id_warrant = null;
                textBoxAgreementWarrant.Text = "";
                vButtonSelectWarrant.Text = "...";
                return;
            }
            if (swForm == null)
                swForm = new SelectWarrantForm();
            if (swForm.ShowDialog() == DialogResult.OK)
            {
                if (swForm.WarrantId != null)
                {
                    id_warrant = swForm.WarrantId.Value;
                    textBoxAgreementWarrant.Text = WarrantStringById(swForm.WarrantId.Value);
                    vButtonSelectWarrant.Text = "x";
                }
            }
        }

        void TenancyAgreementsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        void TenancyAgreementsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {

            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }
        void TenancyPersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
        }

        void TenancyPersonsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
        }

        void vButtonTerminatePaste_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxTerminateAgreement.Text.Trim()))
            {
                MessageBox.Show("Не указана причина расторжения договора", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainPoint.Focus();
                return;
            }
            textBoxAgreementContent.Clear();
            textBoxAgreementContent.Text =
                string.Format(CultureInfo.InvariantCulture,
                    "1.1. По настоящему Соглашению Стороны договорились расторгнуть  с {3} договор № {0} от {1} {4} найма (далее - договор) жилого помещения по {2}.\r\n" +
                    "1.2.Обязательства, возникшие из указанного договора до момента расторжения, подлежат исполнению в соответствии с указанным договором. Стороны не имеют взаимных претензий по исполнению условий договора № {0} от {1}.",
                    ParentRow["registration_num"],
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "",
                        textBoxTerminateAgreement.Text.StartsWith("по ") ? textBoxTerminateAgreement.Text.Substring(3).Trim() : textBoxTerminateAgreement.Text.Trim(),
                    dateTimePickerTerminateDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    DataModel.GetInstance(DataModelType.RentTypesDataModel).Select().Rows.Find(ParentRow["id_rent_type"])["rent_type_genetive"]);
        }

        void vButtonExplainPaste_Click(object sender, EventArgs e)
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
            var contentList = textBoxAgreementContent.Lines.ToList();
            var headersCount = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B"))
                    headersCount++;
            }
            var headerIndex = -1;
            var lastPointIndex = -1;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B.*изложить"))
                {
                    headerIndex = i;
                }
                else
                    if (headerIndex != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ исключить|\u200B.*пункт .+ дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        lastPointIndex = i;
                        break;
                    }
            }
            var point = "";
            if (!string.IsNullOrEmpty(textBoxExplainPoint.Text.Trim()) &&
                !string.IsNullOrEmpty(textBoxExplainGeneralPoint.Text.Trim()))
            {
                point = string.Format("подпункт {0} пункта {1}", textBoxExplainPoint.Text.Trim(), textBoxExplainGeneralPoint.Text.Trim());
            } else if (!string.IsNullOrEmpty(textBoxExplainPoint.Text.Trim()))
            {
                point = string.Format("подпункт {0}", textBoxExplainPoint.Text.Trim());
            }
            else
            {
                point = string.Format("пункт {0}", textBoxExplainGeneralPoint.Text.Trim());
            }
            var element = string.Format(CultureInfo.InvariantCulture, "{0}. {1}", point, textBoxExplainContent.Text.Trim());
            if (headerIndex == -1)
            {
                contentList.Add(string.Format("\u200B{0}) изложить в новой редакции:", ++headersCount));
            }
            if (lastPointIndex == -1)
                contentList.Add(element);
            else
                contentList.Insert(lastPointIndex, element);
            textBoxAgreementContent.Lines = contentList.ToArray();
        }

        void vButtonIncludePaste_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxIncludeSNP.Text.Trim()))
            {
                MessageBox.Show("Поле ФИО не может быть пустым", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxIncludeSNP.Focus();
                return;
            }
            if (comboBoxIncludeKinship.SelectedValue == null)
            {
                MessageBox.Show("Не выбрана родственная связь", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxIncludeKinship.Focus();
                return;
            }
            var contentList = textBoxAgreementContent.Lines.ToList();
            var headers_count = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B"))
                    headers_count++;
            }
            var header_index = -1;
            var last_point_index = -1;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], string.Format("^\u200B.*пункт {0} договора дополнить", 
                    textBoxGeneralIncludePoint.Text)))
                {
                    header_index = i;
                }
                else
                    if (header_index != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ договора исключить|\u200B.*пункт .+ договора дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        last_point_index = i;
                        break;
                    }
            }

            var kinship = ((DataRowView)comboBoxIncludeKinship.SelectedItem)["kinship"].ToString();
            var element = string.Format(CultureInfo.InvariantCulture, "«{0}. {1}, {2} - {3} г.р.»;", textBoxIncludePoint.Text,
                textBoxIncludeSNP.Text.Trim(),
                kinship,
                dateTimePickerIncludeDateOfBirth.Value.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)); 
            var snp = textBoxIncludeSNP.Text.Trim();
            string sSurname, sName, sPatronymic;
            Declension.GetSNM(snp, out sSurname, out sName, out sPatronymic);
            if (kinship == "наниматель")
            {
                var gender = Declension.GetGender(sPatronymic);
                contentList.Add(string.Format("\u200B{4}) считать по договору № {0} от {1} нанимателем - «{2} - {3} г.р.»;",
                    ParentRow["registration_num"],
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "",
                        gender == Gender.NotDefind ? snp :
                            Declension.GetSNPDeclension(snp, gender, DeclensionCase.Vinit),
                        dateTimePickerIncludeDateOfBirth.Value.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                        ++headers_count));
            }
            else
            {
                if (header_index == -1)
                {
                    contentList.Add(string.Format("\u200B{2}) пункт {0} договора дополнить подпунктом {1} следующего содержания:",
                        textBoxGeneralIncludePoint.Text, textBoxIncludePoint.Text, ++headers_count));
                }
                if (last_point_index == -1)
                    contentList.Add(element);
                else
                    contentList.Insert(last_point_index, element);
            }
            textBoxAgreementContent.Lines = contentList.ToArray();
            includePersons.Add(new TenancyPerson
            {
                IdProcess = (int?)ParentRow["id_process"],
                Surname = sSurname,
                Name = sName,
                Patronymic = sPatronymic,
                DateOfBirth = dateTimePickerIncludeDateOfBirth.Value.Date,
                IdKinship = (int?)comboBoxIncludeKinship.SelectedValue
            });
        }

        void vButtonExcludePaste_Click(object sender, EventArgs e)
        {
            if (v_tenancy_persons.Position == -1)
            {
                MessageBox.Show("Не выбран участник найма", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var contentList = textBoxAgreementContent.Lines.ToList();
            var header_index = -1;
            var last_point_index = -1;
            var headers_count = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B"))
                    headers_count++;
            }
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], string.Format("^\u200B.*из пункта {0} договора исключить",
                    textBoxGeneralExcludePoint.Text)))
                {
                    header_index = i;
                }
                else
                    if (header_index != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ договора исключить|\u200B.*пункт .+ договора дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        last_point_index = i;
                        break;
                    }
            }
            var tenancyPerson = ((DataRowView)v_tenancy_persons[v_tenancy_persons.Position]);

            var kinship = tenancyPerson["id_kinship"] != DBNull.Value ?
                ((DataRowView)v_kinships[v_kinships.Find("id_kinship", tenancyPerson["id_kinship"])])["kinship"].ToString() : "";
            var element = string.Format(CultureInfo.InvariantCulture, "«{0}. {1} {2} {3} - {4}, {5} г.р.»;", textBoxExcludePoint.Text,
                tenancyPerson["surname"],
                tenancyPerson["name"],
                tenancyPerson["patronymic"],
                kinship,
                tenancyPerson["date_of_birth"] != DBNull.Value ?
                    Convert.ToDateTime(tenancyPerson["date_of_birth"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");
            if (header_index == -1)
            {
                contentList.Add(string.Format("\u200B{2}) из пункта {0} договора исключить подпункт {1} следующего содержания:",
                    textBoxGeneralExcludePoint.Text, textBoxExcludePoint.Text, ++headers_count));
            }
            if (last_point_index == -1)
                contentList.Add(element);
            else
                contentList.Insert(last_point_index, element);
            textBoxAgreementContent.Lines = contentList.ToArray();
            excludePersons.Add(new TenancyPerson
            {
                IdProcess = (int?)ParentRow["id_process"],
                IdPerson = (int?)tenancyPerson["id_person"],
                Surname = tenancyPerson["surname"].ToString(),
                Name = tenancyPerson["name"].ToString(),
                Patronymic = tenancyPerson["patronymic"].ToString(),
                DateOfBirth = (DateTime?)tenancyPerson["date_of_birth"],
                IdKinship = (int?)tenancyPerson["id_kinship"]
            });
        }

        void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (GeneralBindingSource.Position >= dataGridView.RowCount)
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                else
                    if (dataGridView.Rows[GeneralBindingSource.Position].Selected != true)
                        dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            BindWarrantId();
            if (GeneralBindingSource.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
                return false;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        private void vButtonPaymentInsert_Click(object sender, EventArgs e)
        {
            var add = "";
            switch ((int)ParentRow["id_rent_type"]) 
            {
                case 3 :
                     add = @"2) изложить пункт ___ в следующей редакции:
«вносить плату:
• за пользование жилым помещением (плата за наем) на расчетный счет Наймодателя в размере, установленном муниципальным правовым актом города Братска в соответствии с Жилищным кодексом Российской Федерации; 
• за содержание и ремонт жилого помещения лицам, осуществляющим соответствующий вид деятельности, в размере установленном муниципальным правовым актом города Братска в соответствии с Жилищным кодексом Российской Федерации;
• за коммунальные услуги лицам, осуществляющим соответствующий вид деятельности, в соответствии с тарифами, установленными в соответствии законодательством Российской Федерации.
Плата за жилое помещение и коммунальные услуги вносится ежемесячно до десятого числа месяца, следующего за истекшим месяцем, если иной срок не установлен договором управления многоквартирным домом либо решением общего собрания членов товарищества собственников жилья, жилищного кооператива или иного специализированного потребительского кооператива, созданного в целях удовлетворения потребностей граждан в жилье в соответствии с федеральным законом о таком кооперативе.
Плата за жилое помещение и коммунальные услуги вносится Нанимателем на основании платежных документов.
В случае невнесения в установленный срок платы за жилое помещение и (или) коммунальные услуги Наниматель уплачивает Наймодателю пени в размере, установленном Жилищным кодексом Российской Федерации, что не освобождает Нанимателя от уплаты причитающихся платежей».";                     
                    break;
                case 1:
                case 2:
                    add = @"2) главу ___ изложить в следующей редакции:
Наниматель вносит плату:
• за пользование жилым помещением (плата за наем) на расчетный счет Наймодателя в размере, установленном решением органа местного самоуправления в соответствии с Жилищным кодексом Российской Федерации;
• за содержание и ремонт жилого помещения лицам, осуществляющим соответствующий вид деятельности, в размере установленном решением органа местного самоуправления, в соответствии с Жилищным кодексом Российской Федерации;
• за коммунальные услуги лицам, осуществляющим соответствующий вид деятельности, в соответствии с тарифами, установленными в соответствии законодательством Российской Федерации.
Плата за жилое помещение и коммунальные услуги вносится ежемесячно до десятого числа месяца, следующего за истекшим месяцем, если иной срок не установлен договором управления многоквартирным домом либо решением общего собрания членов товарищества собственников жилья, жилищного кооператива или иного специализированного потребительского кооператива, созданного в целях удовлетворения потребностей граждан в жилье в соответствии с федеральным законом о таком кооперативе.
Плата за жилое помещение и коммунальные услуги вносится Нанимателем на основании платежных документов.
Наймодатель вправе в одностороннем порядке изменить размер платы за пользование жилым помещением (плата за наем) и за содержание и ремонт жилого помещения в случае внесения изменения в муниципальный правовой акт города Братска, устанавливающего размер платы за жилое помещение.";
                    break;
            }
            textBoxAgreementContent.Text += textBoxAgreementContent.Text.EndsWith("\n") ? add : Environment.NewLine + add;                                       
        }      
    }
}
