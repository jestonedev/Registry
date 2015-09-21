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
using CustomControls;
using Declensions.Unicode;
using Registry.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Security;
using VIBlend.Utilities;
using VIBlend.WinForms.Controls;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class TenancyAgreementsViewport: Viewport
    {
        #region Components  
        private TableLayoutPanel tableLayoutPanel12;
        private GroupBox groupBox29;
        private GroupBox groupBox30;
        private TabControl tabControl1;
        private Panel panel7;
        private DataGridView dataGridView;
        private DataGridView dataGridViewTenancyPersons;
        private TextBox textBoxAgreementContent;
        private TextBox textBoxAgreementWarrant;
        private TextBox textBoxExcludePoint;
        private TextBox textBoxIncludePoint;
        private TextBox textBoxIncludeSNP;
        private TextBox textBoxExplainPoint;
        private TextBox textBoxExplainContent;
        private TextBox textBoxTerminateAgreement;
        private TabPage tabPageExclude;
        private TabPage tabPageInclude;
        private TabPage tabPageExplain;
        private TabPage tabPageTerminate;
        private Label label71;
        private Label label72;
        private Label label73;
        private Label label74;
        private Label label75;
        private Label label76;
        private Label label77;
        private Label label78;
        private Label label79;
        private Label label80;
        private ComboBox comboBoxExecutor;
        private ComboBox comboBoxIncludeKinship;
        private vButton vButtonSelectWarrant;
        private vButton vButtonExcludePaste;
        private vButton vButtonIncludePaste;
        private vButton vButtonExplainPaste;
        private vButton vButtonTerminatePaste;
        private DateTimePicker dateTimePickerAgreementDate;
        private DateTimePicker dateTimePickerIncludeDateOfBirth;
        private DataGridViewTextBoxColumn id_agreement;
        private DataGridViewDateTimeColumn agreement_date;
        private DataGridViewTextBoxColumn agreement_content;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        private DataGridViewTextBoxColumn date_of_birth;
        private DateTimePicker dateTimePickerTerminateDate;
        private Label label1;
        #endregion Components

        #region Modeles
        TenancyPersonsDataModel tenancy_persons;
        TenancyAgreementsDataModel tenancy_agreements;
        ExecutorsDataModel executors;
        WarrantsDataModel warrants;
        KinshipsDataModel kinships;
        #endregion Modeles

        #region Views
        BindingSource v_tenancy_persons;
        BindingSource v_tenancy_agreements;
        BindingSource v_executors;
        BindingSource v_warrants;
        BindingSource v_kinships;
        #endregion Views

        //Forms
        private SelectWarrantForm swForm;
        
        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable;
        private int? id_warrant;
        private TextBox textBoxGeneralIncludePoint;
        private Label label2;
        private TextBox textBoxGeneralExcludePoint;
        private Label label3;
        private bool is_first_visible = true;   // первое отображение формы

        private TenancyAgreementsViewport()
            : this(null)
        {
        }

        public TenancyAgreementsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public TenancyAgreementsViewport(TenancyAgreementsViewport tenancyAgreementsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = tenancyAgreementsViewport.DynamicFilter;
            StaticFilter = tenancyAgreementsViewport.StaticFilter;
            ParentRow = tenancyAgreementsViewport.ParentRow;
            ParentType = tenancyAgreementsViewport.ParentType;
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
            comboBoxExecutor.DataBindings.Add("SelectedValue", v_tenancy_agreements, "id_executor", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxIncludeKinship.DataSource = v_kinships;
            comboBoxIncludeKinship.ValueMember = "id_kinship";
            comboBoxIncludeKinship.DisplayMember = "kinship";

            textBoxAgreementContent.DataBindings.Clear();
            textBoxAgreementContent.DataBindings.Add("Text", v_tenancy_agreements, "agreement_content", true, DataSourceUpdateMode.Never, "");

            dateTimePickerAgreementDate.DataBindings.Clear();
            dateTimePickerAgreementDate.DataBindings.Add("Value", v_tenancy_agreements, "agreement_date", true, DataSourceUpdateMode.Never, DateTime.Now);

            dataGridViewTenancyPersons.DataSource = v_tenancy_persons;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            date_of_birth.DataPropertyName = "date_of_birth";

            dataGridView.DataSource = v_tenancy_agreements;
            id_agreement.DataPropertyName = "id_agreement";
            agreement_date.DataPropertyName = "agreement_date";
            agreement_content.DataPropertyName = "agreement_content";
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_tenancy_agreements.Position != -1) && (TenancyAgreementFromView() != TenancyAgreementFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    dataGridView.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    dataGridView.Enabled = true;
                }
            }
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }

        private bool ChangeViewportStateTo(ViewportState state)
        {
            if (!AccessControl.HasPrivelege(Priveleges.TenancyWrite))
            {
                viewportState = ViewportState.ReadState;
                return true;
            }
            switch (state)
            {
                case ViewportState.ReadState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            return true;
                        case ViewportState.NewRowState:
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else return false;
                            if (viewportState == ViewportState.ReadState)
                                return true;
                            else
                                return false;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            if (tenancy_agreements.EditingNewRecord)
                                return false;
                            else
                            {
                                viewportState = ViewportState.NewRowState;
                                return true;
                            }
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.NewRowState);
                            else
                                return false;
                    }
                    break;
                case ViewportState.ModifyRowState: ;
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.ModifyRowState);
                            else
                                return false;
                    }
                    break;
            }
            return false;
        }

        public void LocateAgreementBy(int id)
        {
            var Position = v_tenancy_agreements.Find("id_agreement", id);
            is_editable = false;
            if (Position > 0)
                v_tenancy_agreements.Position = Position;
            is_editable = true;
        }

        private string WarrantStringByID(int id_warrant)
        {
            if (v_warrants.Position == -1)
                return null;
            else
            {
                var row_index = v_warrants.Find("id_warrant", id_warrant);
                if (row_index == -1)
                    return null;
                var registration_date = Convert.ToDateTime(((DataRowView)v_warrants[row_index])["registration_date"], CultureInfo.InvariantCulture);
                var registration_num = ((DataRowView)v_warrants[row_index])["registration_num"].ToString();
                return string.Format(CultureInfo.InvariantCulture, "№ {0} от {1}",
                    registration_num, registration_date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            }
        }

        private void BindWarrantID()
        {
            if ((v_tenancy_agreements.Position > -1) && ((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position])["id_warrant"] != DBNull.Value)
            {
                id_warrant = Convert.ToInt32(((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position])["id_warrant"], CultureInfo.InvariantCulture);
                textBoxAgreementWarrant.Text =
                    WarrantStringByID(id_warrant.Value);
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
                textBoxAgreementWarrant.Text = WarrantStringByID(tenancyAgreement.IdWarrant.Value);
                id_warrant = tenancyAgreement.IdWarrant;
            }
            else
            {
                textBoxAgreementWarrant.Text = "";
                id_warrant = null;
            }
        }

        private TenancyAgreement TenancyAgreementFromViewport()
        {
            var tenancyAgreement = new TenancyAgreement();
            if (v_tenancy_agreements.Position == -1)
                tenancyAgreement.IdAgreement = null;
            else
                tenancyAgreement.IdAgreement = ViewportHelper.ValueOrNull<int>((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position], "id_agreement");
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

        private TenancyAgreement TenancyAgreementFromView()
        {
            var tenancyAgreement = new TenancyAgreement();
            var row = (DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position];
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

        public override int GetRecordCount()
        {
            return v_tenancy_agreements.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancy_agreements.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancy_agreements.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancy_agreements.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancy_agreements.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_tenancy_agreements.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_tenancy_agreements.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_tenancy_agreements.Position > -1) && (v_tenancy_agreements.Position < (v_tenancy_agreements.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_tenancy_agreements.Position > -1) && (v_tenancy_agreements.Position < (v_tenancy_agreements.Count - 1));
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
            tenancy_agreements = TenancyAgreementsDataModel.GetInstance();
            tenancy_persons = TenancyPersonsDataModel.GetInstance();
            executors = ExecutorsDataModel.GetInstance();
            warrants = WarrantsDataModel.GetInstance();
            kinships = KinshipsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            tenancy_agreements.Select();
            tenancy_persons.Select();
            executors.Select();
            warrants.Select();
            kinships.Select();

            var ds = DataSetManager.DataSet;

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

            v_tenancy_agreements = new BindingSource();
            v_tenancy_agreements.CurrentItemChanged += v_tenancy_agreements_CurrentItemChanged;
            v_tenancy_agreements.DataMember = "tenancy_agreements";
            v_tenancy_agreements.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_tenancy_agreements.Filter += " AND ";
            v_tenancy_agreements.Filter += DynamicFilter;
            v_tenancy_agreements.DataSource = ds;
            tenancy_agreements.Select().RowDeleted += TenancyAgreementsViewport_RowDeleted;
            tenancy_agreements.Select().RowChanged += TenancyAgreementsViewport_RowChanged;

            DataBind();
            tenancy_persons.Select().RowDeleted += TenancyPersonsViewport_RowDeleted;
            tenancy_persons.Select().RowChanged += TenancyPersonsViewport_RowChanged;
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (v_tenancy_agreements.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите это соглашение?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (TenancyAgreementsDataModel.Delete((int)((DataRowView)v_tenancy_agreements.Current)["id_agreement"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position]).Delete();
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
                    tenancy_agreements.EditingNewRecord = false;
                    if (v_tenancy_agreements.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position]).Delete();
                        if (v_tenancy_agreements.Position != -1)
                            dataGridView.Rows[v_tenancy_agreements.Position].Selected = true;
                    }
                    else
                        Text = "Соглашения отсутствуют";
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    BindWarrantID();
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
            var tenancyAgreement = TenancyAgreementFromViewport();
            if (!ValidateAgreement(tenancyAgreement))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                case ViewportState.NewRowState:
                    var id_agreement = TenancyAgreementsDataModel.Insert(tenancyAgreement);
                    if (id_agreement == -1)
                    {
                        tenancy_agreements.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    tenancyAgreement.IdAgreement = id_agreement;
                    is_editable = false;
                    if (v_tenancy_agreements.Position == -1)
                        newRow = (DataRowView)v_tenancy_agreements.AddNew();
                    else
                        newRow = ((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position]);
                    FillRowFromAgreement(tenancyAgreement, newRow);
                    tenancy_agreements.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancyAgreement.IdAgreement == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить соглашение без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (TenancyAgreementsDataModel.Update(tenancyAgreement) == -1)
                        return;
                    var row = ((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position]);
                    is_editable = false;
                    FillRowFromAgreement(tenancyAgreement, row);
                    break;
            }
            viewportState = ViewportState.ReadState;
            dataGridView.Enabled = true;
            is_editable = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return (!tenancy_agreements.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            v_tenancy_agreements.AddNew();
            dataGridView.Enabled = false;
            var index = v_executors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)v_executors[index])["id_executor"];
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                textBoxAgreementContent.Text = string.Format(CultureInfo.InvariantCulture, "1.1 По настоящему Соглашению Стороны по договору № {0} от {1} договорились",
                    ParentRow["registration_num"],
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");
            is_editable = true;
            tenancy_agreements.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (v_tenancy_agreements.Position != -1) && (!tenancy_agreements.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var tenancyAgreement = TenancyAgreementFromView();
            v_tenancy_agreements.AddNew();
            dataGridView.Enabled = false;
            tenancy_agreements.EditingNewRecord = true;
            ViewportFromTenancyAgreement(tenancyAgreement);
            is_editable = true;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new TenancyAgreementsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancy_agreements.Count > 0)
                viewport.LocateAgreementBy((((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position])["id_agreement"] as int?) ?? -1);
            return viewport;
        }

        public override bool HasTenancyAgreementReport()
        {
            return (v_tenancy_agreements.Position > -1);
        }

        public override void TenancyAgreementReportGenerate()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            if (v_tenancy_agreements.Position == -1)
            {
                MessageBox.Show("Не выбрано соглашение для печати",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = (DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position];
            ReporterFactory.CreateReporter(ReporterType.TenancyAgreementReporter).Run(
                new Dictionary<string, string>() { { "id_agreement", row["id_agreement"].ToString() } });
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
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                tenancy_persons.Select().RowDeleted -= TenancyPersonsViewport_RowDeleted;
                tenancy_persons.Select().RowChanged -= TenancyPersonsViewport_RowChanged;
                tenancy_agreements.Select().RowDeleted -= TenancyAgreementsViewport_RowDeleted;
                tenancy_agreements.Select().RowChanged -= TenancyAgreementsViewport_RowChanged;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                tenancy_agreements.EditingNewRecord = false;
            tenancy_persons.Select().RowDeleted -= TenancyPersonsViewport_RowDeleted;
            tenancy_persons.Select().RowChanged -= TenancyPersonsViewport_RowChanged;
            tenancy_agreements.Select().RowDeleted -= TenancyAgreementsViewport_RowDeleted;
            tenancy_agreements.Select().RowChanged -= TenancyAgreementsViewport_RowChanged;
            Close();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
            if (is_first_visible)
            {
                is_first_visible = false;
                if (v_tenancy_agreements.Count == 0)
                    InsertRecord();
            }
            vButtonSelectWarrant.Focus();
            base.OnVisibleChanged(e);
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
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
                    textBoxAgreementWarrant.Text = WarrantStringByID(swForm.WarrantId.Value);
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

        void textBoxAgreementContent_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxExecutor_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxAgreementWarrant_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerAgreementDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
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
                    RentTypesDataModel.GetInstance().Select().Rows.Find(ParentRow["id_rent_type"])["rent_type_genetive"]);
        }

        void vButtonExplainPaste_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxExplainPoint.Text.Trim()))
            {
                MessageBox.Show("Не указан номер подпункта", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainPoint.Focus();
                return;
            }
            if (string.IsNullOrEmpty(textBoxExplainContent.Text.Trim()))
            {
                MessageBox.Show("Содержание изложения не может быть пустым", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainContent.Focus();
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
                if (Regex.IsMatch(contentList[i], "^\u200B.*изложить"))
                {
                    header_index = i;
                }
                else
                    if (header_index != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ исключить|\u200B.*пункт .+ дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        last_point_index = i;
                        break;
                    }
            }

            var element = string.Format(CultureInfo.InvariantCulture, "подпункт {0}. {1}", textBoxExplainPoint.Text, textBoxExplainContent.Text.Trim());
            if (header_index == -1)
            {
                contentList.Add(string.Format("\u200B{0}) изложить в новой редакции:", ++headers_count));
            }
            if (last_point_index == -1)
                contentList.Add(element);
            else
                contentList.Insert(last_point_index, element);
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
            if (kinship == "наниматель")
            {
                var snp = textBoxIncludeSNP.Text.Trim();
                string sSurname, sName, sPatronymic;
                Declension.GetSNM(snp, out sSurname, out sName, out sPatronymic);
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
        }

        void v_tenancy_agreements_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_tenancy_agreements.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (v_tenancy_agreements.Position >= dataGridView.RowCount)
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                else
                    if (dataGridView.Rows[v_tenancy_agreements.Position].Selected != true)
                        dataGridView.Rows[v_tenancy_agreements.Position].Selected = true;
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            BindWarrantID();
            if (v_tenancy_agreements.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyAgreementsViewport));
            tableLayoutPanel12 = new TableLayoutPanel();
            panel7 = new Panel();
            groupBox29 = new GroupBox();
            comboBoxExecutor = new ComboBox();
            label73 = new Label();
            vButtonSelectWarrant = new vButton();
            textBoxAgreementWarrant = new TextBox();
            label72 = new Label();
            dateTimePickerAgreementDate = new DateTimePicker();
            label71 = new Label();
            groupBox30 = new GroupBox();
            textBoxAgreementContent = new TextBox();
            tabControl1 = new TabControl();
            tabPageExclude = new TabPage();
            textBoxGeneralExcludePoint = new TextBox();
            label3 = new Label();
            dataGridViewTenancyPersons = new DataGridView();
            surname = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            patronymic = new DataGridViewTextBoxColumn();
            date_of_birth = new DataGridViewTextBoxColumn();
            vButtonExcludePaste = new vButton();
            textBoxExcludePoint = new TextBox();
            label74 = new Label();
            tabPageInclude = new TabPage();
            textBoxGeneralIncludePoint = new TextBox();
            label2 = new Label();
            dateTimePickerIncludeDateOfBirth = new DateTimePicker();
            comboBoxIncludeKinship = new ComboBox();
            label76 = new Label();
            label77 = new Label();
            textBoxIncludeSNP = new TextBox();
            textBoxIncludePoint = new TextBox();
            label78 = new Label();
            vButtonIncludePaste = new vButton();
            label75 = new Label();
            tabPageExplain = new TabPage();
            textBoxExplainContent = new TextBox();
            textBoxExplainPoint = new TextBox();
            vButtonExplainPaste = new vButton();
            label79 = new Label();
            tabPageTerminate = new TabPage();
            dateTimePickerTerminateDate = new DateTimePicker();
            label1 = new Label();
            vButtonTerminatePaste = new vButton();
            textBoxTerminateAgreement = new TextBox();
            label80 = new Label();
            dataGridView = new DataGridView();
            id_agreement = new DataGridViewTextBoxColumn();
            agreement_date = new DataGridViewDateTimeColumn();
            agreement_content = new DataGridViewTextBoxColumn();
            tableLayoutPanel12.SuspendLayout();
            panel7.SuspendLayout();
            groupBox29.SuspendLayout();
            groupBox30.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPageExclude.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).BeginInit();
            tabPageInclude.SuspendLayout();
            tabPageExplain.SuspendLayout();
            tabPageTerminate.SuspendLayout();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.ColumnCount = 2;
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel12.Controls.Add(panel7, 0, 0);
            tableLayoutPanel12.Controls.Add(groupBox30, 1, 0);
            tableLayoutPanel12.Controls.Add(tabControl1, 0, 1);
            tableLayoutPanel12.Controls.Add(dataGridView, 0, 2);
            tableLayoutPanel12.Dock = DockStyle.Fill;
            tableLayoutPanel12.Location = new Point(3, 3);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.RowCount = 3;
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Absolute, 179F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel12.Size = new Size(888, 518);
            tableLayoutPanel12.TabIndex = 0;
            // 
            // panel7
            // 
            panel7.Controls.Add(groupBox29);
            panel7.Dock = DockStyle.Fill;
            panel7.Location = new Point(3, 3);
            panel7.Name = "panel7";
            panel7.Size = new Size(438, 104);
            panel7.TabIndex = 0;
            // 
            // groupBox29
            // 
            groupBox29.Controls.Add(comboBoxExecutor);
            groupBox29.Controls.Add(label73);
            groupBox29.Controls.Add(vButtonSelectWarrant);
            groupBox29.Controls.Add(textBoxAgreementWarrant);
            groupBox29.Controls.Add(label72);
            groupBox29.Controls.Add(dateTimePickerAgreementDate);
            groupBox29.Controls.Add(label71);
            groupBox29.Dock = DockStyle.Fill;
            groupBox29.Location = new Point(0, 0);
            groupBox29.Name = "groupBox29";
            groupBox29.Size = new Size(438, 104);
            groupBox29.TabIndex = 0;
            groupBox29.TabStop = false;
            groupBox29.Text = "Общие сведения";
            // 
            // comboBoxExecutor
            // 
            comboBoxExecutor.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                      | AnchorStyles.Right;
            comboBoxExecutor.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxExecutor.FormattingEnabled = true;
            comboBoxExecutor.Location = new Point(164, 77);
            comboBoxExecutor.Name = "comboBoxExecutor";
            comboBoxExecutor.Size = new Size(268, 23);
            comboBoxExecutor.TabIndex = 3;
            comboBoxExecutor.TextChanged += comboBoxExecutor_TextChanged;
            // 
            // label73
            // 
            label73.AutoSize = true;
            label73.Location = new Point(17, 80);
            label73.Name = "label73";
            label73.Size = new Size(85, 15);
            label73.TabIndex = 38;
            label73.Text = "Исполнитель";
            // 
            // vButtonSelectWarrant
            // 
            vButtonSelectWarrant.AllowAnimations = true;
            vButtonSelectWarrant.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonSelectWarrant.BackColor = Color.Transparent;
            vButtonSelectWarrant.Location = new Point(405, 48);
            vButtonSelectWarrant.Name = "vButtonSelectWarrant";
            vButtonSelectWarrant.RoundedCornersMask = 15;
            vButtonSelectWarrant.Size = new Size(27, 20);
            vButtonSelectWarrant.TabIndex = 2;
            vButtonSelectWarrant.Text = "...";
            vButtonSelectWarrant.UseVisualStyleBackColor = false;
            vButtonSelectWarrant.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonSelectWarrant.Click += vButtonSelectWarrant_Click;
            // 
            // textBoxAgreementWarrant
            // 
            textBoxAgreementWarrant.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            textBoxAgreementWarrant.Location = new Point(164, 48);
            textBoxAgreementWarrant.Name = "textBoxAgreementWarrant";
            textBoxAgreementWarrant.ReadOnly = true;
            textBoxAgreementWarrant.Size = new Size(235, 21);
            textBoxAgreementWarrant.TabIndex = 1;
            textBoxAgreementWarrant.TextChanged += textBoxAgreementWarrant_TextChanged;
            // 
            // label72
            // 
            label72.AutoSize = true;
            label72.Location = new Point(17, 51);
            label72.Name = "label72";
            label72.Size = new Size(109, 15);
            label72.TabIndex = 35;
            label72.Text = "По доверенности";
            // 
            // dateTimePickerAgreementDate
            // 
            dateTimePickerAgreementDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                 | AnchorStyles.Right;
            dateTimePickerAgreementDate.Location = new Point(164, 19);
            dateTimePickerAgreementDate.Name = "dateTimePickerAgreementDate";
            dateTimePickerAgreementDate.Size = new Size(268, 21);
            dateTimePickerAgreementDate.TabIndex = 0;
            dateTimePickerAgreementDate.ValueChanged += dateTimePickerAgreementDate_ValueChanged;
            // 
            // label71
            // 
            label71.AutoSize = true;
            label71.Location = new Point(17, 23);
            label71.Name = "label71";
            label71.Size = new Size(109, 15);
            label71.TabIndex = 33;
            label71.Text = "Дата соглашения";
            // 
            // groupBox30
            // 
            groupBox30.Controls.Add(textBoxAgreementContent);
            groupBox30.Dock = DockStyle.Fill;
            groupBox30.Location = new Point(447, 3);
            groupBox30.Name = "groupBox30";
            tableLayoutPanel12.SetRowSpan(groupBox30, 2);
            groupBox30.Size = new Size(438, 283);
            groupBox30.TabIndex = 1;
            groupBox30.TabStop = false;
            groupBox30.Text = "Содержание";
            // 
            // textBoxAgreementContent
            // 
            textBoxAgreementContent.Dock = DockStyle.Fill;
            textBoxAgreementContent.Location = new Point(3, 17);
            textBoxAgreementContent.MaxLength = 4000;
            textBoxAgreementContent.Multiline = true;
            textBoxAgreementContent.Name = "textBoxAgreementContent";
            textBoxAgreementContent.Size = new Size(432, 263);
            textBoxAgreementContent.TabIndex = 1;
            textBoxAgreementContent.TextChanged += textBoxAgreementContent_TextChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageExclude);
            tabControl1.Controls.Add(tabPageInclude);
            tabControl1.Controls.Add(tabPageExplain);
            tabControl1.Controls.Add(tabPageTerminate);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 110);
            tabControl1.Margin = new Padding(0);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new Point(0, 0);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(444, 179);
            tabControl1.TabIndex = 0;
            // 
            // tabPageExclude
            // 
            tabPageExclude.BackColor = Color.White;
            tabPageExclude.Controls.Add(textBoxGeneralExcludePoint);
            tabPageExclude.Controls.Add(label3);
            tabPageExclude.Controls.Add(dataGridViewTenancyPersons);
            tabPageExclude.Controls.Add(vButtonExcludePaste);
            tabPageExclude.Controls.Add(textBoxExcludePoint);
            tabPageExclude.Controls.Add(label74);
            tabPageExclude.Location = new Point(4, 24);
            tabPageExclude.Name = "tabPageExclude";
            tabPageExclude.Padding = new Padding(3);
            tabPageExclude.Size = new Size(436, 151);
            tabPageExclude.TabIndex = 0;
            tabPageExclude.Text = "Исключить";
            // 
            // textBoxGeneralExcludePoint
            // 
            textBoxGeneralExcludePoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            textBoxGeneralExcludePoint.Location = new Point(164, 6);
            textBoxGeneralExcludePoint.Name = "textBoxGeneralExcludePoint";
            textBoxGeneralExcludePoint.Size = new Size(234, 21);
            textBoxGeneralExcludePoint.TabIndex = 0;
            textBoxGeneralExcludePoint.Enter += selectAll_Enter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 9);
            label3.Name = "label3";
            label3.Size = new Size(41, 15);
            label3.TabIndex = 50;
            label3.Text = "Пункт";
            // 
            // dataGridViewTenancyPersons
            // 
            dataGridViewTenancyPersons.AllowUserToAddRows = false;
            dataGridViewTenancyPersons.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) 
                                                 | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            dataGridViewTenancyPersons.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTenancyPersons.BackgroundColor = Color.White;
            dataGridViewTenancyPersons.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridViewTenancyPersons.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewTenancyPersons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyPersons.Columns.AddRange(surname, name, patronymic, date_of_birth);
            dataGridViewTenancyPersons.Location = new Point(3, 61);
            dataGridViewTenancyPersons.MultiSelect = false;
            dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            dataGridViewTenancyPersons.ReadOnly = true;
            dataGridViewTenancyPersons.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyPersons.Size = new Size(433, 83);
            dataGridViewTenancyPersons.TabIndex = 3;
            // 
            // surname
            // 
            surname.HeaderText = "Фамилия";
            surname.MinimumWidth = 100;
            surname.Name = "surname";
            surname.ReadOnly = true;
            // 
            // name
            // 
            name.HeaderText = "Имя";
            name.MinimumWidth = 100;
            name.Name = "name";
            name.ReadOnly = true;
            // 
            // patronymic
            // 
            patronymic.HeaderText = "Отчество";
            patronymic.MinimumWidth = 100;
            patronymic.Name = "patronymic";
            patronymic.ReadOnly = true;
            // 
            // date_of_birth
            // 
            date_of_birth.HeaderText = "Дата рождения";
            date_of_birth.MinimumWidth = 140;
            date_of_birth.Name = "date_of_birth";
            date_of_birth.ReadOnly = true;
            // 
            // vButtonExcludePaste
            // 
            vButtonExcludePaste.AllowAnimations = true;
            vButtonExcludePaste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonExcludePaste.BackColor = Color.Transparent;
            vButtonExcludePaste.Location = new Point(404, 6);
            vButtonExcludePaste.Name = "vButtonExcludePaste";
            vButtonExcludePaste.RoundedCornersMask = 15;
            vButtonExcludePaste.Size = new Size(27, 20);
            vButtonExcludePaste.TabIndex = 2;
            vButtonExcludePaste.Text = "→";
            vButtonExcludePaste.UseVisualStyleBackColor = false;
            vButtonExcludePaste.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonExcludePaste.Click += vButtonExcludePaste_Click;
            // 
            // textBoxExcludePoint
            // 
            textBoxExcludePoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                         | AnchorStyles.Right;
            textBoxExcludePoint.Location = new Point(164, 34);
            textBoxExcludePoint.Name = "textBoxExcludePoint";
            textBoxExcludePoint.Size = new Size(235, 21);
            textBoxExcludePoint.TabIndex = 1;
            textBoxExcludePoint.Enter += selectAll_Enter;
            // 
            // label74
            // 
            label74.AutoSize = true;
            label74.Location = new Point(13, 37);
            label74.Name = "label74";
            label74.Size = new Size(62, 15);
            label74.TabIndex = 37;
            label74.Text = "Подпункт";
            // 
            // tabPageInclude
            // 
            tabPageInclude.BackColor = Color.White;
            tabPageInclude.Controls.Add(textBoxGeneralIncludePoint);
            tabPageInclude.Controls.Add(label2);
            tabPageInclude.Controls.Add(dateTimePickerIncludeDateOfBirth);
            tabPageInclude.Controls.Add(comboBoxIncludeKinship);
            tabPageInclude.Controls.Add(label76);
            tabPageInclude.Controls.Add(label77);
            tabPageInclude.Controls.Add(textBoxIncludeSNP);
            tabPageInclude.Controls.Add(textBoxIncludePoint);
            tabPageInclude.Controls.Add(label78);
            tabPageInclude.Controls.Add(vButtonIncludePaste);
            tabPageInclude.Controls.Add(label75);
            tabPageInclude.Location = new Point(4, 24);
            tabPageInclude.Name = "tabPageInclude";
            tabPageInclude.Padding = new Padding(3);
            tabPageInclude.Size = new Size(436, 151);
            tabPageInclude.TabIndex = 1;
            tabPageInclude.Text = "Включить";
            // 
            // textBoxGeneralIncludePoint
            // 
            textBoxGeneralIncludePoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            textBoxGeneralIncludePoint.Location = new Point(164, 6);
            textBoxGeneralIncludePoint.Name = "textBoxGeneralIncludePoint";
            textBoxGeneralIncludePoint.Size = new Size(234, 21);
            textBoxGeneralIncludePoint.TabIndex = 0;
            textBoxGeneralIncludePoint.Enter += selectAll_Enter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 9);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 48;
            label2.Text = "Пункт";
            // 
            // dateTimePickerIncludeDateOfBirth
            // 
            dateTimePickerIncludeDateOfBirth.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                      | AnchorStyles.Right;
            dateTimePickerIncludeDateOfBirth.Location = new Point(164, 90);
            dateTimePickerIncludeDateOfBirth.Name = "dateTimePickerIncludeDateOfBirth";
            dateTimePickerIncludeDateOfBirth.Size = new Size(234, 21);
            dateTimePickerIncludeDateOfBirth.TabIndex = 3;
            // 
            // comboBoxIncludeKinship
            // 
            comboBoxIncludeKinship.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            comboBoxIncludeKinship.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxIncludeKinship.FormattingEnabled = true;
            comboBoxIncludeKinship.Location = new Point(164, 118);
            comboBoxIncludeKinship.Name = "comboBoxIncludeKinship";
            comboBoxIncludeKinship.Size = new Size(234, 23);
            comboBoxIncludeKinship.TabIndex = 4;
            // 
            // label76
            // 
            label76.AutoSize = true;
            label76.Location = new Point(13, 122);
            label76.Name = "label76";
            label76.Size = new Size(110, 15);
            label76.TabIndex = 46;
            label76.Text = "Отношение/связь";
            // 
            // label77
            // 
            label77.AutoSize = true;
            label77.Location = new Point(13, 93);
            label77.Name = "label77";
            label77.Size = new Size(98, 15);
            label77.TabIndex = 45;
            label77.Text = "Дата рождения";
            // 
            // textBoxIncludeSNP
            // 
            textBoxIncludeSNP.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                       | AnchorStyles.Right;
            textBoxIncludeSNP.Location = new Point(164, 62);
            textBoxIncludeSNP.Name = "textBoxIncludeSNP";
            textBoxIncludeSNP.Size = new Size(234, 21);
            textBoxIncludeSNP.TabIndex = 2;
            textBoxIncludeSNP.Enter += selectAll_Enter;
            // 
            // textBoxIncludePoint
            // 
            textBoxIncludePoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                         | AnchorStyles.Right;
            textBoxIncludePoint.Location = new Point(164, 34);
            textBoxIncludePoint.Name = "textBoxIncludePoint";
            textBoxIncludePoint.Size = new Size(234, 21);
            textBoxIncludePoint.TabIndex = 1;
            textBoxIncludePoint.Enter += selectAll_Enter;
            // 
            // label78
            // 
            label78.AutoSize = true;
            label78.Location = new Point(13, 65);
            label78.Name = "label78";
            label78.Size = new Size(36, 15);
            label78.TabIndex = 43;
            label78.Text = "ФИО";
            // 
            // vButtonIncludePaste
            // 
            vButtonIncludePaste.AllowAnimations = true;
            vButtonIncludePaste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonIncludePaste.BackColor = Color.Transparent;
            vButtonIncludePaste.Location = new Point(404, 6);
            vButtonIncludePaste.Name = "vButtonIncludePaste";
            vButtonIncludePaste.RoundedCornersMask = 15;
            vButtonIncludePaste.Size = new Size(27, 20);
            vButtonIncludePaste.TabIndex = 4;
            vButtonIncludePaste.Text = "→";
            vButtonIncludePaste.UseVisualStyleBackColor = false;
            vButtonIncludePaste.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonIncludePaste.Click += vButtonIncludePaste_Click;
            // 
            // label75
            // 
            label75.AutoSize = true;
            label75.Location = new Point(13, 37);
            label75.Name = "label75";
            label75.Size = new Size(62, 15);
            label75.TabIndex = 40;
            label75.Text = "Подпункт";
            // 
            // tabPageExplain
            // 
            tabPageExplain.BackColor = Color.White;
            tabPageExplain.Controls.Add(textBoxExplainContent);
            tabPageExplain.Controls.Add(textBoxExplainPoint);
            tabPageExplain.Controls.Add(vButtonExplainPaste);
            tabPageExplain.Controls.Add(label79);
            tabPageExplain.Location = new Point(4, 24);
            tabPageExplain.Name = "tabPageExplain";
            tabPageExplain.Size = new Size(436, 151);
            tabPageExplain.TabIndex = 2;
            tabPageExplain.Text = "Изложить";
            // 
            // textBoxExplainContent
            // 
            textBoxExplainContent.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) 
                                            | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            textBoxExplainContent.Location = new Point(7, 32);
            textBoxExplainContent.Multiline = true;
            textBoxExplainContent.Name = "textBoxExplainContent";
            textBoxExplainContent.Size = new Size(424, 103);
            textBoxExplainContent.TabIndex = 1;
            // 
            // textBoxExplainPoint
            // 
            textBoxExplainPoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                         | AnchorStyles.Right;
            textBoxExplainPoint.Location = new Point(163, 6);
            textBoxExplainPoint.Name = "textBoxExplainPoint";
            textBoxExplainPoint.Size = new Size(234, 21);
            textBoxExplainPoint.TabIndex = 0;
            textBoxExplainPoint.Enter += selectAll_Enter;
            // 
            // vButtonExplainPaste
            // 
            vButtonExplainPaste.AllowAnimations = true;
            vButtonExplainPaste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonExplainPaste.BackColor = Color.Transparent;
            vButtonExplainPaste.Location = new Point(404, 6);
            vButtonExplainPaste.Name = "vButtonExplainPaste";
            vButtonExplainPaste.RoundedCornersMask = 15;
            vButtonExplainPaste.Size = new Size(27, 20);
            vButtonExplainPaste.TabIndex = 2;
            vButtonExplainPaste.Text = "→";
            vButtonExplainPaste.UseVisualStyleBackColor = false;
            vButtonExplainPaste.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonExplainPaste.Click += vButtonExplainPaste_Click;
            // 
            // label79
            // 
            label79.AutoSize = true;
            label79.Location = new Point(12, 9);
            label79.Name = "label79";
            label79.Size = new Size(62, 15);
            label79.TabIndex = 40;
            label79.Text = "Подпункт";
            // 
            // tabPageTerminate
            // 
            tabPageTerminate.BackColor = Color.White;
            tabPageTerminate.Controls.Add(dateTimePickerTerminateDate);
            tabPageTerminate.Controls.Add(label1);
            tabPageTerminate.Controls.Add(vButtonTerminatePaste);
            tabPageTerminate.Controls.Add(textBoxTerminateAgreement);
            tabPageTerminate.Controls.Add(label80);
            tabPageTerminate.Location = new Point(4, 24);
            tabPageTerminate.Name = "tabPageTerminate";
            tabPageTerminate.Size = new Size(436, 151);
            tabPageTerminate.TabIndex = 3;
            tabPageTerminate.Text = "Расторгнуть";
            // 
            // dateTimePickerTerminateDate
            // 
            dateTimePickerTerminateDate.Location = new Point(163, 34);
            dateTimePickerTerminateDate.Name = "dateTimePickerTerminateDate";
            dateTimePickerTerminateDate.Size = new Size(234, 21);
            dateTimePickerTerminateDate.TabIndex = 45;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 38);
            label1.Name = "label1";
            label1.Size = new Size(118, 15);
            label1.TabIndex = 44;
            label1.Text = "Дата расторжения";
            // 
            // vButtonTerminatePaste
            // 
            vButtonTerminatePaste.AllowAnimations = true;
            vButtonTerminatePaste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonTerminatePaste.BackColor = Color.Transparent;
            vButtonTerminatePaste.Location = new Point(404, 6);
            vButtonTerminatePaste.Name = "vButtonTerminatePaste";
            vButtonTerminatePaste.RoundedCornersMask = 15;
            vButtonTerminatePaste.Size = new Size(27, 20);
            vButtonTerminatePaste.TabIndex = 1;
            vButtonTerminatePaste.Text = "→";
            vButtonTerminatePaste.UseVisualStyleBackColor = false;
            vButtonTerminatePaste.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonTerminatePaste.Click += vButtonTerminatePaste_Click;
            // 
            // textBoxTerminateAgreement
            // 
            textBoxTerminateAgreement.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                               | AnchorStyles.Right;
            textBoxTerminateAgreement.Location = new Point(163, 6);
            textBoxTerminateAgreement.Name = "textBoxTerminateAgreement";
            textBoxTerminateAgreement.Size = new Size(234, 21);
            textBoxTerminateAgreement.TabIndex = 0;
            textBoxTerminateAgreement.Enter += selectAll_Enter;
            // 
            // label80
            // 
            label80.AutoSize = true;
            label80.Location = new Point(12, 10);
            label80.Name = "label80";
            label80.Size = new Size(110, 15);
            label80.TabIndex = 43;
            label80.Text = "По какой причине";
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) 
                                   | AnchorStyles.Left) 
                                  | AnchorStyles.Right;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_agreement, agreement_date, agreement_content);
            tableLayoutPanel12.SetColumnSpan(dataGridView, 2);
            dataGridView.Location = new Point(3, 292);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(882, 223);
            dataGridView.TabIndex = 2;
            dataGridView.DataError += dataGridView_DataError;
            // 
            // id_agreement
            // 
            id_agreement.HeaderText = "Номер соглашения";
            id_agreement.MinimumWidth = 100;
            id_agreement.Name = "id_agreement";
            id_agreement.ReadOnly = true;
            id_agreement.Visible = false;
            // 
            // agreement_date
            // 
            agreement_date.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            agreement_date.HeaderText = "Дата соглашения";
            agreement_date.MinimumWidth = 150;
            agreement_date.Name = "agreement_date";
            agreement_date.ReadOnly = true;
            agreement_date.Width = 150;
            // 
            // agreement_content
            // 
            agreement_content.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            agreement_content.FillWeight = 500F;
            agreement_content.HeaderText = "Содержание";
            agreement_content.MinimumWidth = 100;
            agreement_content.Name = "agreement_content";
            agreement_content.ReadOnly = true;
            // 
            // TenancyAgreementsViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(660, 360);
            BackColor = Color.White;
            ClientSize = new Size(894, 524);
            Controls.Add(tableLayoutPanel12);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyAgreementsViewport";
            Padding = new Padding(3);
            Text = "Соглашения найма №{0}";
            tableLayoutPanel12.ResumeLayout(false);
            panel7.ResumeLayout(false);
            groupBox29.ResumeLayout(false);
            groupBox29.PerformLayout();
            groupBox30.ResumeLayout(false);
            groupBox30.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPageExclude.ResumeLayout(false);
            tabPageExclude.PerformLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).EndInit();
            tabPageInclude.ResumeLayout(false);
            tabPageInclude.PerformLayout();
            tabPageExplain.ResumeLayout(false);
            tabPageExplain.PerformLayout();
            tabPageTerminate.ResumeLayout(false);
            tabPageTerminate.PerformLayout();
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

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
    }
}
