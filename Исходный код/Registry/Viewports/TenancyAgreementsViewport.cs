using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;
using System.Drawing;
using System.Text.RegularExpressions;
using Registry.Reporting;
using Security;
using System.Globalization;

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
        private VIBlend.WinForms.Controls.vButton vButtonSelectWarrant;
        private VIBlend.WinForms.Controls.vButton vButtonExcludePaste;
        private VIBlend.WinForms.Controls.vButton vButtonIncludePaste;
        private VIBlend.WinForms.Controls.vButton vButtonExplainPaste;
        private VIBlend.WinForms.Controls.vButton vButtonTerminatePaste;
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
        TenancyPersonsDataModel tenancy_persons = null;
        TenancyAgreementsDataModel tenancy_agreements = null;
        ExecutorsDataModel executors = null;
        WarrantsDataModel warrants = null;
        KinshipsDataModel kinships = null;
        #endregion Modeles

        #region Views
        BindingSource v_tenancy_persons = null;
        BindingSource v_tenancy_agreements = null;
        BindingSource v_executors = null;
        BindingSource v_warrants = null;
        BindingSource v_kinships = null;
        #endregion Views

        //Forms
        private SelectWarrantForm swForm = null;
        
        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;
        private int? id_warrant = null;
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
            this.DynamicFilter = tenancyAgreementsViewport.DynamicFilter;
            this.StaticFilter = tenancyAgreementsViewport.StaticFilter;
            this.ParentRow = tenancyAgreementsViewport.ParentRow;
            this.ParentType = tenancyAgreementsViewport.ParentType;
        }

        private void RedrawDataGridTenancyPersonsRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (int i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
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
            if ((!this.ContainsFocus) || (dataGridView.Focused))
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
            int Position = v_tenancy_agreements.Find("id_agreement", id);
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
                int row_index = v_warrants.Find("id_warrant", id_warrant);
                if (row_index == -1)
                    return null;
                DateTime registration_date = Convert.ToDateTime(((DataRowView)v_warrants[row_index])["registration_date"], CultureInfo.InvariantCulture);
                string registration_num = ((DataRowView)v_warrants[row_index])["registration_num"].ToString();
                return String.Format(CultureInfo.InvariantCulture, "№ {0} от {1}",
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
            TenancyAgreement tenancyAgreement = new TenancyAgreement();
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
            TenancyAgreement tenancyAgreement = new TenancyAgreement();
            DataRowView row = (DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position];
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
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

            DataSet ds = DataSetManager.DataSet;

            if ((ParentType == ParentTypeEnum.Tenancy) && (ParentRow != null))
                this.Text = String.Format(CultureInfo.InvariantCulture, "Соглашения найма №{0}", ParentRow["id_process"].ToString());
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
            v_tenancy_agreements.CurrentItemChanged += new EventHandler(v_tenancy_agreements_CurrentItemChanged);
            v_tenancy_agreements.DataMember = "tenancy_agreements";
            v_tenancy_agreements.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_tenancy_agreements.Filter += " AND ";
            v_tenancy_agreements.Filter += DynamicFilter;
            v_tenancy_agreements.DataSource = ds;
            tenancy_agreements.Select().RowDeleted += TenancyAgreementsViewport_RowDeleted;
            tenancy_agreements.Select().RowChanged += TenancyAgreementsViewport_RowChanged;

            DataBind();
            tenancy_persons.Select().RowDeleted += new DataRowChangeEventHandler(TenancyPersonsViewport_RowDeleted);
            tenancy_persons.Select().RowChanged += new DataRowChangeEventHandler(TenancyPersonsViewport_RowChanged);
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
                        this.Text = "Соглашения отсутствуют";
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
            TenancyAgreement tenancyAgreement = TenancyAgreementFromViewport();
            if (!ValidateAgreement(tenancyAgreement))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                case ViewportState.NewRowState:
                    int id_agreement = TenancyAgreementsDataModel.Insert(tenancyAgreement);
                    if (id_agreement == -1)
                        return;
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
                    DataRowView row = ((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position]);
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
            int index = v_executors.Find("executor_login", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)v_executors[index])["id_executor"];
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                textBoxAgreementContent.Text = String.Format(CultureInfo.InvariantCulture, "1.1 По настоящему Соглашению Стороны по договору № {0} от {1} договорились",
                    ParentRow["registration_num"].ToString(),
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
            TenancyAgreement tenancyAgreement = TenancyAgreementFromView();
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
            TenancyAgreementsViewport viewport = new TenancyAgreementsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancy_agreements.Count > 0)
                viewport.LocateAgreementBy((((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position])["id_agreement"] as Int32?) ?? -1);
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
            DataRowView row = (DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position];
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                tenancy_persons.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyPersonsViewport_RowDeleted);
                tenancy_persons.Select().RowChanged -= new DataRowChangeEventHandler(TenancyPersonsViewport_RowChanged);
                tenancy_agreements.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyAgreementsViewport_RowDeleted);
                tenancy_agreements.Select().RowChanged -= new DataRowChangeEventHandler(TenancyAgreementsViewport_RowChanged);
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                tenancy_agreements.EditingNewRecord = false;
            tenancy_persons.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyPersonsViewport_RowDeleted);
            tenancy_persons.Select().RowChanged -= new DataRowChangeEventHandler(TenancyPersonsViewport_RowChanged);
            tenancy_agreements.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyAgreementsViewport_RowDeleted);
            tenancy_agreements.Select().RowChanged -= new DataRowChangeEventHandler(TenancyAgreementsViewport_RowChanged);
            base.Close();
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
            if (String.IsNullOrEmpty(textBoxTerminateAgreement.Text.Trim()))
            {
                MessageBox.Show("Не указана причина расторжения договора", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainPoint.Focus();
                return;
            }
            textBoxAgreementContent.Clear();
            textBoxAgreementContent.Text =
                String.Format(CultureInfo.InvariantCulture,
                    "1.1. По настоящему Соглашению Стороны договорились расторгнуть  с {3} договор № {0} от {1} {4} найма (далее - Договор) жилого помещения по {2}.\r\n" +
                    "1.2.Обязательства, возникшие из указанного Договора до момента расторжения, подлежат исполнению в соответствии с указанным Договором. Стороны не имеют взаимных претензий по исполнению условий договора № {0} от {1}.",
                    ParentRow["registration_num"].ToString(),
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "",
                    textBoxTerminateAgreement.Text,
                    dateTimePickerTerminateDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    RentTypesDataModel.GetInstance().Select().Rows.Find(ParentRow["id_rent_type"])["rent_type_genetive"]);
        }

        void vButtonExplainPaste_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxExplainPoint.Text.Trim()))
            {
                MessageBox.Show("Не указан номер подпункта", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainPoint.Focus();
                return;
            }
            if (String.IsNullOrEmpty(textBoxExplainContent.Text.Trim()))
            {
                MessageBox.Show("Содержание изложения не может быть пустым", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainContent.Focus();
                return;
            }
            List<string> contentList = textBoxAgreementContent.Lines.ToList();
            int header_index = -1;
            int last_point_index = -1;
            for (int i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200Bизложить"))
                {
                    header_index = i;
                }
                else
                    if (header_index != -1 && Regex.IsMatch(contentList[i], "^(\u200Bисключить|\u200Bвключить|\u200Bизложить|\u200Bрасторгнуть)"))
                    {
                        last_point_index = i;
                        break;
                    }
            }

            string element = String.Format(CultureInfo.InvariantCulture, "подпункт {0}. {1}", textBoxExplainPoint.Text, textBoxExplainContent.Text.Trim());
            if (header_index == -1)
            {
                contentList.Add("\u200Bизложить в новой редакции:");
            }
            if (last_point_index == -1)
                contentList.Add(element);
            else
                contentList.Insert(last_point_index, element);
            textBoxAgreementContent.Lines = contentList.ToArray();
        }

        void vButtonIncludePaste_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxIncludePoint.Text.Trim()))
            {
                MessageBox.Show("Не указан номер подпункта", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxIncludePoint.Focus();
                return;
            }
            if (String.IsNullOrEmpty(textBoxIncludeSNP.Text.Trim()))
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
            List<string> contentList = textBoxAgreementContent.Lines.ToList();
            int header_index = -1;
            int last_point_index = -1;
            for (int i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200Bвключить"))
                {
                    header_index = i;
                }
                else
                    if (header_index != -1 && Regex.IsMatch(contentList[i], "^(\u200Bисключить|\u200Bвключить|\u200Bизложить|\u200Bрасторгнуть)"))
                    {
                        last_point_index = i;
                        break;
                    }
            }

            string kinship = ((DataRowView)comboBoxIncludeKinship.SelectedItem)["kinship"].ToString();
            string element = String.Format(CultureInfo.InvariantCulture, "подпункт {0}. {1} - {2}, {3} г.р.", textBoxIncludePoint.Text,
                textBoxIncludeSNP.Text.Trim(),
                kinship,
                dateTimePickerIncludeDateOfBirth.Value.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            if (header_index == -1)
            {
                contentList.Add("\u200Bвключить:");
            }
            if (last_point_index == -1)
                contentList.Add(element);
            else
                contentList.Insert(last_point_index, element);
            textBoxAgreementContent.Lines = contentList.ToArray();
        }

        void vButtonExcludePaste_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxExcludePoint.Text.Trim()))
            {
                MessageBox.Show("Не указан номер подпункта", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (v_tenancy_persons.Position == -1)
            {
                MessageBox.Show("Не выбран участник найма", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            List<string> contentList = textBoxAgreementContent.Lines.ToList();
            int header_index = -1;
            int last_point_index = -1;
            for (int i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200Bисключить"))
                {
                    header_index = i;
                }
                else
                    if (header_index != -1 && Regex.IsMatch(contentList[i], "^(\u200Bисключить|\u200Bвключить|\u200Bизложить|\u200Bрасторгнуть)"))
                    {
                        last_point_index = i;
                        break;
                    }
            }
            DataRowView tenancyPerson = ((DataRowView)v_tenancy_persons[v_tenancy_persons.Position]);

            string kinship = tenancyPerson["id_kinship"] != DBNull.Value ?
                ((DataRowView)v_kinships[v_kinships.Find("id_kinship", tenancyPerson["id_kinship"])])["kinship"].ToString() : "";
            string element = String.Format(CultureInfo.InvariantCulture, "подпункт {0}. {1} {2} {3} - {4}, {5} г.р.", textBoxExcludePoint.Text,
                tenancyPerson["surname"].ToString(),
                tenancyPerson["name"].ToString(),
                tenancyPerson["patronymic"].ToString(),
                kinship,
                tenancyPerson["date_of_birth"] != DBNull.Value ?
                    Convert.ToDateTime(tenancyPerson["date_of_birth"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");
            if (header_index == -1)
            {
                contentList.Add("\u200Bисключить:");
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyAgreementsViewport));
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.groupBox29 = new System.Windows.Forms.GroupBox();
            this.comboBoxExecutor = new System.Windows.Forms.ComboBox();
            this.label73 = new System.Windows.Forms.Label();
            this.vButtonSelectWarrant = new VIBlend.WinForms.Controls.vButton();
            this.textBoxAgreementWarrant = new System.Windows.Forms.TextBox();
            this.label72 = new System.Windows.Forms.Label();
            this.dateTimePickerAgreementDate = new System.Windows.Forms.DateTimePicker();
            this.label71 = new System.Windows.Forms.Label();
            this.groupBox30 = new System.Windows.Forms.GroupBox();
            this.textBoxAgreementContent = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageExclude = new System.Windows.Forms.TabPage();
            this.dataGridViewTenancyPersons = new System.Windows.Forms.DataGridView();
            this.surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_of_birth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vButtonExcludePaste = new VIBlend.WinForms.Controls.vButton();
            this.textBoxExcludePoint = new System.Windows.Forms.TextBox();
            this.label74 = new System.Windows.Forms.Label();
            this.tabPageInclude = new System.Windows.Forms.TabPage();
            this.dateTimePickerIncludeDateOfBirth = new System.Windows.Forms.DateTimePicker();
            this.comboBoxIncludeKinship = new System.Windows.Forms.ComboBox();
            this.label76 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.textBoxIncludeSNP = new System.Windows.Forms.TextBox();
            this.textBoxIncludePoint = new System.Windows.Forms.TextBox();
            this.label78 = new System.Windows.Forms.Label();
            this.vButtonIncludePaste = new VIBlend.WinForms.Controls.vButton();
            this.label75 = new System.Windows.Forms.Label();
            this.tabPageExplain = new System.Windows.Forms.TabPage();
            this.textBoxExplainContent = new System.Windows.Forms.TextBox();
            this.textBoxExplainPoint = new System.Windows.Forms.TextBox();
            this.vButtonExplainPaste = new VIBlend.WinForms.Controls.vButton();
            this.label79 = new System.Windows.Forms.Label();
            this.tabPageTerminate = new System.Windows.Forms.TabPage();
            this.vButtonTerminatePaste = new VIBlend.WinForms.Controls.vButton();
            this.textBoxTerminateAgreement = new System.Windows.Forms.TextBox();
            this.label80 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_agreement = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agreement_date = new CustomControls.DataGridViewDateTimeColumn();
            this.agreement_content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerTerminateDate = new System.Windows.Forms.DateTimePicker();
            this.tableLayoutPanel12.SuspendLayout();
            this.panel7.SuspendLayout();
            this.groupBox29.SuspendLayout();
            this.groupBox30.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageExclude.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).BeginInit();
            this.tabPageInclude.SuspendLayout();
            this.tabPageExplain.SuspendLayout();
            this.tabPageTerminate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 2;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Controls.Add(this.panel7, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.groupBox30, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel12.Controls.Add(this.dataGridView, 0, 2);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 3;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(888, 392);
            this.tableLayoutPanel12.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.groupBox29);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(3, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(438, 104);
            this.panel7.TabIndex = 0;
            // 
            // groupBox29
            // 
            this.groupBox29.Controls.Add(this.comboBoxExecutor);
            this.groupBox29.Controls.Add(this.label73);
            this.groupBox29.Controls.Add(this.vButtonSelectWarrant);
            this.groupBox29.Controls.Add(this.textBoxAgreementWarrant);
            this.groupBox29.Controls.Add(this.label72);
            this.groupBox29.Controls.Add(this.dateTimePickerAgreementDate);
            this.groupBox29.Controls.Add(this.label71);
            this.groupBox29.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox29.Location = new System.Drawing.Point(0, 0);
            this.groupBox29.Name = "groupBox29";
            this.groupBox29.Size = new System.Drawing.Size(438, 104);
            this.groupBox29.TabIndex = 0;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "Общие сведения";
            // 
            // comboBoxExecutor
            // 
            this.comboBoxExecutor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxExecutor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecutor.FormattingEnabled = true;
            this.comboBoxExecutor.Location = new System.Drawing.Point(164, 77);
            this.comboBoxExecutor.Name = "comboBoxExecutor";
            this.comboBoxExecutor.Size = new System.Drawing.Size(268, 23);
            this.comboBoxExecutor.TabIndex = 3;
            this.comboBoxExecutor.TextChanged += new System.EventHandler(this.comboBoxExecutor_TextChanged);
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(17, 80);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(85, 15);
            this.label73.TabIndex = 38;
            this.label73.Text = "Исполнитель";
            // 
            // vButtonSelectWarrant
            // 
            this.vButtonSelectWarrant.AllowAnimations = true;
            this.vButtonSelectWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonSelectWarrant.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSelectWarrant.Location = new System.Drawing.Point(405, 48);
            this.vButtonSelectWarrant.Name = "vButtonSelectWarrant";
            this.vButtonSelectWarrant.RoundedCornersMask = ((byte)(15));
            this.vButtonSelectWarrant.Size = new System.Drawing.Size(27, 20);
            this.vButtonSelectWarrant.TabIndex = 2;
            this.vButtonSelectWarrant.Text = "...";
            this.vButtonSelectWarrant.UseVisualStyleBackColor = false;
            this.vButtonSelectWarrant.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSelectWarrant.Click += new System.EventHandler(this.vButtonSelectWarrant_Click);
            // 
            // textBoxAgreementWarrant
            // 
            this.textBoxAgreementWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAgreementWarrant.Location = new System.Drawing.Point(164, 48);
            this.textBoxAgreementWarrant.Name = "textBoxAgreementWarrant";
            this.textBoxAgreementWarrant.ReadOnly = true;
            this.textBoxAgreementWarrant.Size = new System.Drawing.Size(235, 21);
            this.textBoxAgreementWarrant.TabIndex = 1;
            this.textBoxAgreementWarrant.TextChanged += new System.EventHandler(this.textBoxAgreementWarrant_TextChanged);
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Location = new System.Drawing.Point(17, 51);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(109, 15);
            this.label72.TabIndex = 35;
            this.label72.Text = "По доверенности";
            // 
            // dateTimePickerAgreementDate
            // 
            this.dateTimePickerAgreementDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAgreementDate.Location = new System.Drawing.Point(164, 19);
            this.dateTimePickerAgreementDate.Name = "dateTimePickerAgreementDate";
            this.dateTimePickerAgreementDate.Size = new System.Drawing.Size(268, 21);
            this.dateTimePickerAgreementDate.TabIndex = 0;
            this.dateTimePickerAgreementDate.ValueChanged += new System.EventHandler(this.dateTimePickerAgreementDate_ValueChanged);
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Location = new System.Drawing.Point(17, 23);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(109, 15);
            this.label71.TabIndex = 33;
            this.label71.Text = "Дата соглашения";
            // 
            // groupBox30
            // 
            this.groupBox30.Controls.Add(this.textBoxAgreementContent);
            this.groupBox30.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox30.Location = new System.Drawing.Point(447, 3);
            this.groupBox30.Name = "groupBox30";
            this.tableLayoutPanel12.SetRowSpan(this.groupBox30, 2);
            this.groupBox30.Size = new System.Drawing.Size(438, 259);
            this.groupBox30.TabIndex = 1;
            this.groupBox30.TabStop = false;
            this.groupBox30.Text = "Содержание";
            // 
            // textBoxAgreementContent
            // 
            this.textBoxAgreementContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAgreementContent.Location = new System.Drawing.Point(3, 17);
            this.textBoxAgreementContent.MaxLength = 4000;
            this.textBoxAgreementContent.Multiline = true;
            this.textBoxAgreementContent.Name = "textBoxAgreementContent";
            this.textBoxAgreementContent.Size = new System.Drawing.Size(432, 239);
            this.textBoxAgreementContent.TabIndex = 1;
            this.textBoxAgreementContent.TextChanged += new System.EventHandler(this.textBoxAgreementContent_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageExclude);
            this.tabControl1.Controls.Add(this.tabPageInclude);
            this.tabControl1.Controls.Add(this.tabPageExplain);
            this.tabControl1.Controls.Add(this.tabPageTerminate);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 110);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(444, 155);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageExclude
            // 
            this.tabPageExclude.BackColor = System.Drawing.Color.White;
            this.tabPageExclude.Controls.Add(this.dataGridViewTenancyPersons);
            this.tabPageExclude.Controls.Add(this.vButtonExcludePaste);
            this.tabPageExclude.Controls.Add(this.textBoxExcludePoint);
            this.tabPageExclude.Controls.Add(this.label74);
            this.tabPageExclude.Location = new System.Drawing.Point(4, 24);
            this.tabPageExclude.Name = "tabPageExclude";
            this.tabPageExclude.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExclude.Size = new System.Drawing.Size(436, 127);
            this.tabPageExclude.TabIndex = 0;
            this.tabPageExclude.Text = "Исключить";
            // 
            // dataGridViewTenancyPersons
            // 
            this.dataGridViewTenancyPersons.AllowUserToAddRows = false;
            this.dataGridViewTenancyPersons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTenancyPersons.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTenancyPersons.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyPersons.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTenancyPersons.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTenancyPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.surname,
            this.name,
            this.patronymic,
            this.date_of_birth});
            this.dataGridViewTenancyPersons.Location = new System.Drawing.Point(3, 32);
            this.dataGridViewTenancyPersons.MultiSelect = false;
            this.dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            this.dataGridViewTenancyPersons.ReadOnly = true;
            this.dataGridViewTenancyPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyPersons.Size = new System.Drawing.Size(430, 84);
            this.dataGridViewTenancyPersons.TabIndex = 2;
            // 
            // surname
            // 
            this.surname.HeaderText = "Фамилия";
            this.surname.MinimumWidth = 100;
            this.surname.Name = "surname";
            this.surname.ReadOnly = true;
            // 
            // name
            // 
            this.name.HeaderText = "Имя";
            this.name.MinimumWidth = 100;
            this.name.Name = "name";
            this.name.ReadOnly = true;
            // 
            // patronymic
            // 
            this.patronymic.HeaderText = "Отчество";
            this.patronymic.MinimumWidth = 100;
            this.patronymic.Name = "patronymic";
            this.patronymic.ReadOnly = true;
            // 
            // date_of_birth
            // 
            this.date_of_birth.HeaderText = "Дата рождения";
            this.date_of_birth.MinimumWidth = 140;
            this.date_of_birth.Name = "date_of_birth";
            this.date_of_birth.ReadOnly = true;
            // 
            // vButtonExcludePaste
            // 
            this.vButtonExcludePaste.AllowAnimations = true;
            this.vButtonExcludePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonExcludePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonExcludePaste.Location = new System.Drawing.Point(404, 6);
            this.vButtonExcludePaste.Name = "vButtonExcludePaste";
            this.vButtonExcludePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonExcludePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonExcludePaste.TabIndex = 1;
            this.vButtonExcludePaste.Text = "→";
            this.vButtonExcludePaste.UseVisualStyleBackColor = false;
            this.vButtonExcludePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonExcludePaste.Click += new System.EventHandler(this.vButtonExcludePaste_Click);
            // 
            // textBoxExcludePoint
            // 
            this.textBoxExcludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcludePoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxExcludePoint.Name = "textBoxExcludePoint";
            this.textBoxExcludePoint.Size = new System.Drawing.Size(235, 21);
            this.textBoxExcludePoint.TabIndex = 0;
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(12, 9);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(62, 15);
            this.label74.TabIndex = 37;
            this.label74.Text = "Подпункт";
            // 
            // tabPageInclude
            // 
            this.tabPageInclude.BackColor = System.Drawing.Color.White;
            this.tabPageInclude.Controls.Add(this.dateTimePickerIncludeDateOfBirth);
            this.tabPageInclude.Controls.Add(this.comboBoxIncludeKinship);
            this.tabPageInclude.Controls.Add(this.label76);
            this.tabPageInclude.Controls.Add(this.label77);
            this.tabPageInclude.Controls.Add(this.textBoxIncludeSNP);
            this.tabPageInclude.Controls.Add(this.textBoxIncludePoint);
            this.tabPageInclude.Controls.Add(this.label78);
            this.tabPageInclude.Controls.Add(this.vButtonIncludePaste);
            this.tabPageInclude.Controls.Add(this.label75);
            this.tabPageInclude.Location = new System.Drawing.Point(4, 24);
            this.tabPageInclude.Name = "tabPageInclude";
            this.tabPageInclude.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInclude.Size = new System.Drawing.Size(436, 127);
            this.tabPageInclude.TabIndex = 1;
            this.tabPageInclude.Text = "Включить";
            // 
            // dateTimePickerIncludeDateOfBirth
            // 
            this.dateTimePickerIncludeDateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIncludeDateOfBirth.Location = new System.Drawing.Point(163, 62);
            this.dateTimePickerIncludeDateOfBirth.Name = "dateTimePickerIncludeDateOfBirth";
            this.dateTimePickerIncludeDateOfBirth.Size = new System.Drawing.Size(234, 21);
            this.dateTimePickerIncludeDateOfBirth.TabIndex = 2;
            // 
            // comboBoxIncludeKinship
            // 
            this.comboBoxIncludeKinship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIncludeKinship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIncludeKinship.FormattingEnabled = true;
            this.comboBoxIncludeKinship.Location = new System.Drawing.Point(163, 90);
            this.comboBoxIncludeKinship.Name = "comboBoxIncludeKinship";
            this.comboBoxIncludeKinship.Size = new System.Drawing.Size(234, 23);
            this.comboBoxIncludeKinship.TabIndex = 3;
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(12, 94);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(110, 15);
            this.label76.TabIndex = 46;
            this.label76.Text = "Отношение/связь";
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Location = new System.Drawing.Point(12, 65);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(98, 15);
            this.label77.TabIndex = 45;
            this.label77.Text = "Дата рождения";
            // 
            // textBoxIncludeSNP
            // 
            this.textBoxIncludeSNP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludeSNP.Location = new System.Drawing.Point(163, 34);
            this.textBoxIncludeSNP.Name = "textBoxIncludeSNP";
            this.textBoxIncludeSNP.Size = new System.Drawing.Size(234, 21);
            this.textBoxIncludeSNP.TabIndex = 1;
            // 
            // textBoxIncludePoint
            // 
            this.textBoxIncludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludePoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxIncludePoint.Name = "textBoxIncludePoint";
            this.textBoxIncludePoint.Size = new System.Drawing.Size(234, 21);
            this.textBoxIncludePoint.TabIndex = 0;
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(12, 37);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(36, 15);
            this.label78.TabIndex = 43;
            this.label78.Text = "ФИО";
            // 
            // vButtonIncludePaste
            // 
            this.vButtonIncludePaste.AllowAnimations = true;
            this.vButtonIncludePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonIncludePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonIncludePaste.Location = new System.Drawing.Point(404, 6);
            this.vButtonIncludePaste.Name = "vButtonIncludePaste";
            this.vButtonIncludePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonIncludePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonIncludePaste.TabIndex = 4;
            this.vButtonIncludePaste.Text = "→";
            this.vButtonIncludePaste.UseVisualStyleBackColor = false;
            this.vButtonIncludePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonIncludePaste.Click += new System.EventHandler(this.vButtonIncludePaste_Click);
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(12, 9);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(62, 15);
            this.label75.TabIndex = 40;
            this.label75.Text = "Подпункт";
            // 
            // tabPageExplain
            // 
            this.tabPageExplain.BackColor = System.Drawing.Color.White;
            this.tabPageExplain.Controls.Add(this.textBoxExplainContent);
            this.tabPageExplain.Controls.Add(this.textBoxExplainPoint);
            this.tabPageExplain.Controls.Add(this.vButtonExplainPaste);
            this.tabPageExplain.Controls.Add(this.label79);
            this.tabPageExplain.Location = new System.Drawing.Point(4, 24);
            this.tabPageExplain.Name = "tabPageExplain";
            this.tabPageExplain.Size = new System.Drawing.Size(436, 127);
            this.tabPageExplain.TabIndex = 2;
            this.tabPageExplain.Text = "Изложить";
            // 
            // textBoxExplainContent
            // 
            this.textBoxExplainContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExplainContent.Location = new System.Drawing.Point(7, 32);
            this.textBoxExplainContent.Multiline = true;
            this.textBoxExplainContent.Name = "textBoxExplainContent";
            this.textBoxExplainContent.Size = new System.Drawing.Size(424, 89);
            this.textBoxExplainContent.TabIndex = 1;
            // 
            // textBoxExplainPoint
            // 
            this.textBoxExplainPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExplainPoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxExplainPoint.Name = "textBoxExplainPoint";
            this.textBoxExplainPoint.Size = new System.Drawing.Size(234, 21);
            this.textBoxExplainPoint.TabIndex = 0;
            // 
            // vButtonExplainPaste
            // 
            this.vButtonExplainPaste.AllowAnimations = true;
            this.vButtonExplainPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonExplainPaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonExplainPaste.Location = new System.Drawing.Point(404, 6);
            this.vButtonExplainPaste.Name = "vButtonExplainPaste";
            this.vButtonExplainPaste.RoundedCornersMask = ((byte)(15));
            this.vButtonExplainPaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonExplainPaste.TabIndex = 2;
            this.vButtonExplainPaste.Text = "→";
            this.vButtonExplainPaste.UseVisualStyleBackColor = false;
            this.vButtonExplainPaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonExplainPaste.Click += new System.EventHandler(this.vButtonExplainPaste_Click);
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Location = new System.Drawing.Point(12, 9);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(62, 15);
            this.label79.TabIndex = 40;
            this.label79.Text = "Подпункт";
            // 
            // tabPageTerminate
            // 
            this.tabPageTerminate.BackColor = System.Drawing.Color.White;
            this.tabPageTerminate.Controls.Add(this.dateTimePickerTerminateDate);
            this.tabPageTerminate.Controls.Add(this.label1);
            this.tabPageTerminate.Controls.Add(this.vButtonTerminatePaste);
            this.tabPageTerminate.Controls.Add(this.textBoxTerminateAgreement);
            this.tabPageTerminate.Controls.Add(this.label80);
            this.tabPageTerminate.Location = new System.Drawing.Point(4, 24);
            this.tabPageTerminate.Name = "tabPageTerminate";
            this.tabPageTerminate.Size = new System.Drawing.Size(436, 127);
            this.tabPageTerminate.TabIndex = 3;
            this.tabPageTerminate.Text = "Расторгнуть";
            // 
            // vButtonTerminatePaste
            // 
            this.vButtonTerminatePaste.AllowAnimations = true;
            this.vButtonTerminatePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonTerminatePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonTerminatePaste.Location = new System.Drawing.Point(404, 6);
            this.vButtonTerminatePaste.Name = "vButtonTerminatePaste";
            this.vButtonTerminatePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonTerminatePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonTerminatePaste.TabIndex = 1;
            this.vButtonTerminatePaste.Text = "→";
            this.vButtonTerminatePaste.UseVisualStyleBackColor = false;
            this.vButtonTerminatePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonTerminatePaste.Click += new System.EventHandler(this.vButtonTerminatePaste_Click);
            // 
            // textBoxTerminateAgreement
            // 
            this.textBoxTerminateAgreement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTerminateAgreement.Location = new System.Drawing.Point(163, 6);
            this.textBoxTerminateAgreement.Name = "textBoxTerminateAgreement";
            this.textBoxTerminateAgreement.Size = new System.Drawing.Size(234, 21);
            this.textBoxTerminateAgreement.TabIndex = 0;
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Location = new System.Drawing.Point(12, 10);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(110, 15);
            this.label80.TabIndex = 43;
            this.label80.Text = "По какой причине";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_agreement,
            this.agreement_date,
            this.agreement_content});
            this.tableLayoutPanel12.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Location = new System.Drawing.Point(3, 268);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(882, 121);
            this.dataGridView.TabIndex = 2;
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // id_agreement
            // 
            this.id_agreement.HeaderText = "Номер соглашения";
            this.id_agreement.MinimumWidth = 100;
            this.id_agreement.Name = "id_agreement";
            this.id_agreement.ReadOnly = true;
            this.id_agreement.Visible = false;
            // 
            // agreement_date
            // 
            this.agreement_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.agreement_date.HeaderText = "Дата соглашения";
            this.agreement_date.MinimumWidth = 150;
            this.agreement_date.Name = "agreement_date";
            this.agreement_date.ReadOnly = true;
            this.agreement_date.Width = 150;
            // 
            // agreement_content
            // 
            this.agreement_content.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.agreement_content.FillWeight = 500F;
            this.agreement_content.HeaderText = "Содержание";
            this.agreement_content.MinimumWidth = 100;
            this.agreement_content.Name = "agreement_content";
            this.agreement_content.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 15);
            this.label1.TabIndex = 44;
            this.label1.Text = "Дата расторжения";
            // 
            // dateTimePickerTerminateDate
            // 
            this.dateTimePickerTerminateDate.Location = new System.Drawing.Point(163, 34);
            this.dateTimePickerTerminateDate.Name = "dateTimePickerTerminateDate";
            this.dateTimePickerTerminateDate.Size = new System.Drawing.Size(234, 21);
            this.dateTimePickerTerminateDate.TabIndex = 45;
            // 
            // TenancyAgreementsViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(660, 360);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(894, 398);
            this.Controls.Add(this.tableLayoutPanel12);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyAgreementsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Соглашения найма №{0}";
            this.tableLayoutPanel12.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.groupBox29.ResumeLayout(false);
            this.groupBox29.PerformLayout();
            this.groupBox30.ResumeLayout(false);
            this.groupBox30.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageExclude.ResumeLayout(false);
            this.tabPageExclude.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).EndInit();
            this.tabPageInclude.ResumeLayout(false);
            this.tabPageInclude.PerformLayout();
            this.tabPageExplain.ResumeLayout(false);
            this.tabPageExplain.PerformLayout();
            this.tabPageTerminate.ResumeLayout(false);
            this.tabPageTerminate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
                return false;
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
