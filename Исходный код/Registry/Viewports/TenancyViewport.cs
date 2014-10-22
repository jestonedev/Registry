using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.SearchForms;
using System.Drawing;

namespace Registry.Viewport
{
    internal sealed class TenancyViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel9 = new TableLayoutPanel();
        private TableLayoutPanel tableLayoutPanel10 = new TableLayoutPanel();
        private TableLayoutPanel tableLayoutPanel13 = new TableLayoutPanel();
        private GroupBox groupBoxTenancyContract = new GroupBox();
        private GroupBox groupBoxResidenceWarrant = new GroupBox();
        private GroupBox groupBoxKumiOrder = new GroupBox();
        private GroupBox groupBox21 = new GroupBox();
        private GroupBox groupBox22 = new GroupBox();
        private GroupBox groupBox24 = new GroupBox();
        private GroupBox groupBox25 = new GroupBox();
        private GroupBox groupBox31 = new GroupBox();
        private Panel panel5 = new Panel();
        private Panel panel6 = new Panel();
        private DataGridView dataGridViewPersons = new DataGridView();
        private DataGridViewTextBoxColumn field_surname = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_name = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_patronymic = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_date_of_birth = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_kinship = new DataGridViewComboBoxColumn();
        private DataGridView dataGridViewContractReasons = new DataGridView();
        private DataGridViewTextBoxColumn field_contract_reason_prepared = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_reason_number = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_reason_date = new DataGridViewTextBoxColumn();
        private DataGridView dataGridViewAgreements = new DataGridView();
        private DataGridViewTextBoxColumn field_agreement_date = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_agreement_content = new DataGridViewTextBoxColumn();
        private CheckBox checkBoxContractEnable = new CheckBox();
        private CheckBox checkBoxResidenceWarrantEnable = new CheckBox();
        private CheckBox checkBoxKumiOrderEnable = new CheckBox();
        private Label label41 = new Label();
        private Label label42 = new Label();
        private Label label43 = new Label();
        private Label label44 = new Label();
        private Label label45 = new Label();
        private Label label46 = new Label();
        private Label label47 = new Label();
        private Label label48 = new Label();
        private Label label49 = new Label();
        private Label label50 = new Label();
        private Label label51 = new Label();
        private Label label52 = new Label();
        private Label label82 = new Label();
        private TextBox textBoxResidenceWarrantNumber = new TextBox();
        private TextBox textBoxKumiOrderNumber = new TextBox();
        private TextBox textBoxDescription = new TextBox();
        private TextBox textBoxRegistrationNumber = new TextBox();
        private TextBox textBoxSelectedWarrant = new System.Windows.Forms.TextBox();
        private DateTimePicker dateTimePickerResidenceWarrantDate = new DateTimePicker();
        private DateTimePicker dateTimePickerKumiOrderDate = new DateTimePicker();
        private DateTimePicker dateTimePickerRegistrationDate = new DateTimePicker();
        private DateTimePicker dateTimePickerIssueDate = new DateTimePicker();
        private DateTimePicker dateTimePickerBeginDate = new DateTimePicker();
        private DateTimePicker dateTimePickerEndDate = new DateTimePicker();
        private ComboBox comboBoxExecutor = new ComboBox();
        private ComboBox comboBoxRentType = new ComboBox();
        private VIBlend.WinForms.Controls.vButton vButtonWarrant = new VIBlend.WinForms.Controls.vButton();
        #endregion Components

        //Models
        private TenancyContractsDataModel tenancies = null;
        private ExecutorsDataModel executors = null;
        private RentTypesDataModel rent_types = null;
        private AgreementsDataModel agreements = null;
        private WarrantsDataModel warrants = null;
        private PersonsDataModel persons = null;
        private ContractReasonsDataModel contract_reasons = null;
        private KinshipsDataModel kinships = null;

        //Views
        private BindingSource v_tenancies = null;
        private BindingSource v_executors = null;
        private BindingSource v_rent_types = null;
        private BindingSource v_agreements = null;
        private BindingSource v_warrants = null;
        private BindingSource v_persons = null;
        private BindingSource v_contract_reasons = null;
        private BindingSource v_kinships = null;

        //Forms
        private SearchForm stExtendedSearchForm = null;
        private SearchForm stSimpleSearchForm = null;
        private SelectWarrantForm swForm = null;

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;
        private int? warrant_id = null;

        public TenancyViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageTenancy";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Процесс найма №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public TenancyViewport(TenancyViewport tenancyViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = tenancyViewport.DynamicFilter;
            this.StaticFilter = tenancyViewport.StaticFilter;
            this.ParentRow = tenancyViewport.ParentRow;
            this.ParentType = tenancyViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            tenancies = TenancyContractsDataModel.GetInstance();
            executors = ExecutorsDataModel.GetInstance();
            rent_types = RentTypesDataModel.GetInstance();
            agreements = AgreementsDataModel.GetInstance();
            warrants = WarrantsDataModel.GetInstance();
            persons = PersonsDataModel.GetInstance();
            contract_reasons = ContractReasonsDataModel.GetInstance();
            kinships = KinshipsDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            tenancies.Select();
            executors.Select();
            rent_types.Select();
            agreements.Select();
            warrants.Select();
            persons.Select();
            contract_reasons.Select();
            kinships.Select();

            DataSet ds = DataSetManager.GetDataSet();

            v_executors = new BindingSource();
            v_executors.DataMember = "executors";
            v_executors.DataSource = ds;

            v_rent_types = new BindingSource();
            v_rent_types.DataMember = "rent_types";
            v_rent_types.DataSource = ds;

            v_kinships = new BindingSource();
            v_kinships.DataMember = "kinships";
            v_kinships.DataSource = ds;

            v_warrants = new BindingSource();
            v_warrants.DataMember = "warrants";
            v_warrants.DataSource = ds;

            v_tenancies = new BindingSource();
            v_tenancies.CurrentItemChanged += new EventHandler(v_tenancies_CurrentItemChanged);
            v_tenancies.DataMember = "tenancy_contracts";
            v_tenancies.DataSource = ds;
            v_tenancies.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_tenancies.Filter += " AND ";
            v_tenancies.Filter += DynamicFilter;

            v_persons = new BindingSource();
            v_persons.DataMember = "tenancy_contracts_persons";
            v_persons.DataSource = v_tenancies;

            v_agreements = new BindingSource();
            v_agreements.DataMember = "tenancy_contracts_agreements";
            v_agreements.DataSource = v_tenancies;

            v_contract_reasons = new BindingSource();
            v_contract_reasons.DataMember = "tenancy_contracts_contract_reasons";
            v_contract_reasons.DataSource = v_tenancies;

            DataBind();

            comboBoxRentType.SelectedValueChanged += new EventHandler(comboBoxRentType_SelectedValueChanged);
            textBoxRegistrationNumber.TextChanged += new EventHandler(textBoxRegistrationNumber_TextChanged);
            dateTimePickerRegistrationDate.ValueChanged += new EventHandler(dateTimePickerRegistrationDate_ValueChanged);
            dateTimePickerIssueDate.ValueChanged += new EventHandler(dateTimePickerIssueDate_ValueChanged);
            dateTimePickerBeginDate.ValueChanged += new EventHandler(dateTimePickerBeginDate_ValueChanged);
            dateTimePickerEndDate.ValueChanged += new EventHandler(dateTimePickerEndDate_ValueChanged);
            textBoxResidenceWarrantNumber.TextChanged += new EventHandler(textBoxResidenceWarrantNumber_TextChanged);
            dateTimePickerResidenceWarrantDate.ValueChanged += new EventHandler(dateTimePickerResidenceWarrantDate_ValueChanged);
            textBoxKumiOrderNumber.TextChanged += new EventHandler(textBoxKumiOrderNumber_TextChanged);
            dateTimePickerKumiOrderDate.ValueChanged += new EventHandler(dateTimePickerKumiOrderDate_ValueChanged);
            comboBoxExecutor.SelectedValueChanged += new EventHandler(comboBoxExecutor_SelectedValueChanged);
            textBoxDescription.TextChanged += new EventHandler(textBoxDescription_TextChanged);
            vButtonWarrant.Click += new EventHandler(vButtonWarrant_Click);
            textBoxSelectedWarrant.TextChanged += new EventHandler(textBoxSelectedWarrant_TextChanged);
            persons.Select().RowChanged += new DataRowChangeEventHandler(Persons_RowChanged);
            persons.Select().RowDeleted += new DataRowChangeEventHandler(Persons_RowDeleted);
            tenancies.Select().RowChanged += new DataRowChangeEventHandler(TenancyViewport_RowChanged);
            tenancies.Select().RowDeleted += new DataRowChangeEventHandler(TenancyViewport_RowDeleted);
            v_persons.ListChanged += new System.ComponentModel.ListChangedEventHandler(v_persons_ListChanged);
        }

        void TenancyViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                UnbindedCheckBoxesUpdate();
        }

        void TenancyViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            base.OnVisibleChanged(e);
        }

        void v_persons_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            RedrawDataGridRows();
        }

        void Persons_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                RedrawDataGridRows();
            }
        }

        void Persons_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridRows();
        }

        private void RedrawDataGridRows()
        {
            if (dataGridViewPersons.Rows.Count == 0)
                return;
            for (int i = 0; i < dataGridViewPersons.Rows.Count; i++)
                if (((DataRowView)v_persons[i])["id_kinship"] != DBNull.Value && Convert.ToInt32(((DataRowView)v_persons[i])["id_kinship"]) == 1)
                    dataGridViewPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    dataGridViewPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
        }

        void textBoxSelectedWarrant_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void vButtonWarrant_Click(object sender, EventArgs e)
        {
            if (warrant_id != null)
            {
                warrant_id = null;
                textBoxSelectedWarrant.Text = "";
                vButtonWarrant.Text = "...";
                return;
            }
            if (swForm == null)
                swForm = new SelectWarrantForm();
            if (swForm.ShowDialog() == DialogResult.OK)
            {
                if (swForm.WarrantID != null)
                {
                    warrant_id = swForm.WarrantID.Value;
                    textBoxSelectedWarrant.Text = WarrantStringByID(swForm.WarrantID.Value);
                    vButtonWarrant.Text = "x";
                }
            }
        }

        void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxExecutor_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerKumiOrderDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxKumiOrderNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerResidenceWarrantDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxResidenceWarrantNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerEndDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerBeginDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerIssueDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerRegistrationDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxRegistrationNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxRentType_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void checkBoxResidenceWarrantEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxResidenceWarrant.Controls)
                if (control != checkBoxResidenceWarrantEnable)
                    control.Enabled = checkBoxResidenceWarrantEnable.Checked;
            CheckViewportModifications();
        }

        void checkBoxKumiOrderEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxKumiOrder.Controls)
                if (control != checkBoxKumiOrderEnable)
                    control.Enabled = checkBoxKumiOrderEnable.Checked;
            CheckViewportModifications();
        }

        void checkBoxContractEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxTenancyContract.Controls)
                if (control != checkBoxContractEnable)
                    control.Enabled = checkBoxContractEnable.Checked;
            CheckViewportModifications();
        }

        void v_tenancies_CurrentItemChanged(object sender, EventArgs e)
        {
            if (viewportState == ViewportState.NewRowState)
                this.Text = "Новый процесс";
            else
                if (v_tenancies.Position != -1)
                    this.Text = String.Format("Процесс найма №{0}", ((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
                else
                    this.Text = "Процессы отсутствуют";
            if (Selected)
                menuCallback.NavigationStateUpdate();
            UnbindedCheckBoxesUpdate();
            BindWarrantID();
            if (v_tenancies.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        private void BindWarrantID()
        {
            if ((v_tenancies.Position > -1) && ((DataRowView)v_tenancies[v_tenancies.Position])["id_warrant"] != DBNull.Value)
            {
                warrant_id = Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_warrant"]);
                textBoxSelectedWarrant.Text =
                    WarrantStringByID(warrant_id.Value);
                vButtonWarrant.Text = "x";
            }
            else
            {
                warrant_id = null;
                textBoxSelectedWarrant.Text = "";
                vButtonWarrant.Text = "...";
            }
        }

        private void UnbindedCheckBoxesUpdate()
        {
            checkBoxContractEnable.Checked = (v_tenancies.Position >= 0) &&
                ((((DataRowView)v_tenancies[v_tenancies.Position])["registration_date"] != DBNull.Value) ||
                (((DataRowView)v_tenancies[v_tenancies.Position])["registration_num"] != DBNull.Value));
            checkBoxResidenceWarrantEnable.Checked = (v_tenancies.Position >= 0) &&
                (((DataRowView)v_tenancies[v_tenancies.Position])["residence_warrant_date"] != DBNull.Value);
            checkBoxKumiOrderEnable.Checked = (v_tenancies.Position >= 0) &&
                (((DataRowView)v_tenancies[v_tenancies.Position])["kumi_order_date"] != DBNull.Value);
            dateTimePickerIssueDate.Checked = (v_tenancies.Position >= 0) &&
                (((DataRowView)v_tenancies[v_tenancies.Position])["issue_date"] != DBNull.Value);
            dateTimePickerBeginDate.Checked = (v_tenancies.Position >= 0) &&
                (((DataRowView)v_tenancies[v_tenancies.Position])["begin_date"] != DBNull.Value);
            dateTimePickerEndDate.Checked = (v_tenancies.Position >= 0) &&
                (((DataRowView)v_tenancies[v_tenancies.Position])["end_date"] != DBNull.Value);
        }

        private void DataBind()
        {
            comboBoxRentType.DataSource = v_rent_types;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";
            comboBoxRentType.DataBindings.Clear();
            comboBoxRentType.DataBindings.Add("SelectedValue", v_tenancies, "id_rent_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxRegistrationNumber.DataBindings.Clear();
            textBoxRegistrationNumber.DataBindings.Add("Text", v_tenancies, "registration_num", true, DataSourceUpdateMode.Never, "");

            dateTimePickerRegistrationDate.DataBindings.Clear();
            dateTimePickerRegistrationDate.DataBindings.Add("Value", v_tenancies, "registration_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            dateTimePickerIssueDate.DataBindings.Clear();
            dateTimePickerIssueDate.DataBindings.Add("Value", v_tenancies, "issue_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            dateTimePickerBeginDate.DataBindings.Clear();
            dateTimePickerBeginDate.DataBindings.Add("Value", v_tenancies, "begin_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            dateTimePickerEndDate.DataBindings.Clear();
            dateTimePickerEndDate.DataBindings.Add("Value", v_tenancies, "end_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            dataGridViewPersons.DataSource = v_persons;
            field_surname.DataPropertyName = "surname";
            field_name.DataPropertyName = "name";
            field_patronymic.DataPropertyName = "patronymic";
            field_id_kinship.DataSource = v_kinships;
            field_id_kinship.ValueMember = "id_kinship";
            field_id_kinship.DisplayMember = "kinship";
            field_id_kinship.DataPropertyName = "id_kinship";

            dataGridViewAgreements.DataSource = v_agreements;
            field_agreement_date.DataPropertyName = "agreement_date";
            field_agreement_content.DataPropertyName = "agreement_content";

            dataGridViewContractReasons.DataSource = v_contract_reasons;
            field_reason_date.DataPropertyName = "reason_date";
            field_reason_number.DataPropertyName = "reason_number";
            field_contract_reason_prepared.DataPropertyName = "contract_reason_prepared";

            textBoxResidenceWarrantNumber.DataBindings.Clear();
            textBoxResidenceWarrantNumber.DataBindings.Add("Text", v_tenancies, "residence_warrant_num", true, DataSourceUpdateMode.Never, "");

            dateTimePickerResidenceWarrantDate.DataBindings.Clear();
            dateTimePickerResidenceWarrantDate.DataBindings.Add("Value", v_tenancies, "residence_warrant_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            textBoxKumiOrderNumber.DataBindings.Clear();
            textBoxKumiOrderNumber.DataBindings.Add("Text", v_tenancies, "kumi_order_num", true, DataSourceUpdateMode.Never, "");

            dateTimePickerKumiOrderDate.DataBindings.Clear();
            dateTimePickerKumiOrderDate.DataBindings.Add("Value", v_tenancies, "kumi_order_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_tenancies, "description", true, DataSourceUpdateMode.Never, "");

            comboBoxExecutor.DataSource = v_executors;
            comboBoxExecutor.ValueMember = "id_executor";
            comboBoxExecutor.DisplayMember = "executor_name";
            comboBoxExecutor.DataBindings.Clear();
            comboBoxExecutor.DataBindings.Add("SelectedValue", v_tenancies, "id_executor", true, DataSourceUpdateMode.Never, DBNull.Value);
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if (!this.ContainsFocus)
                return;
            if ((v_tenancies.Position != -1) && (TenancyFromView() != TenancyFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                    viewportState = ViewportState.ModifyRowState;
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                    viewportState = ViewportState.ReadState;
            }
            menuCallback.EditingStateUpdate();
        }

        private Tenancy TenancyFromView()
        {
            Tenancy tenancy = new Tenancy();
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
            if (row["id_contract"] is DBNull)
                tenancy.id_contract = null;
            else
                tenancy.id_contract = Convert.ToInt32(row["id_contract"]);
            if (row["id_rent_type"] is DBNull)
                tenancy.id_rent_type = null;
            else
                tenancy.id_rent_type = Convert.ToInt32(row["id_rent_type"]);
            if (row["id_warrant"] is DBNull)
                tenancy.id_warrant = null;
            else
                tenancy.id_warrant = Convert.ToInt32(row["id_warrant"]);
            if (row["registration_num"] is DBNull)
                tenancy.registration_num = null;
            else
                tenancy.registration_num = row["registration_num"].ToString();
            if (row["registration_date"] is DBNull)
                tenancy.registration_date = null;
            else
                tenancy.registration_date = Convert.ToDateTime(row["registration_date"]);
            if (row["issue_date"] is DBNull)
                tenancy.issue_date = null;
            else
                tenancy.issue_date = Convert.ToDateTime(row["issue_date"]);
            if (row["begin_date"] is DBNull)
                tenancy.begin_date = null;
            else
                tenancy.begin_date = Convert.ToDateTime(row["begin_date"]);
            if (row["end_date"] is DBNull)
                tenancy.end_date = null;
            else
                tenancy.end_date = Convert.ToDateTime(row["end_date"]);
            if (row["residence_warrant_num"] is DBNull)
                tenancy.residence_warrant_num = null;
            else
                tenancy.residence_warrant_num = row["residence_warrant_num"].ToString();
            if (row["residence_warrant_date"] is DBNull)
                tenancy.residence_warrant_date = null;
            else
                tenancy.residence_warrant_date = Convert.ToDateTime(row["residence_warrant_date"]);
            if (row["kumi_order_num"] is DBNull)
                tenancy.kumi_order_num = null;
            else
                tenancy.kumi_order_num = row["kumi_order_num"].ToString();
            if (row["kumi_order_date"] is DBNull)
                tenancy.kumi_order_date = null;
            else
                tenancy.kumi_order_date = Convert.ToDateTime(row["kumi_order_date"]);
            if (row["id_executor"] is DBNull)
                tenancy.id_executor = null;
            else
                tenancy.id_executor = Convert.ToInt32(row["id_executor"]);
            if (row["description"] is DBNull)
                tenancy.description = null;
            else
                tenancy.description = row["description"].ToString();
            return tenancy;
        }

        private Tenancy TenancyFromViewport()
        {
            Tenancy tenancy = new Tenancy();
            if ((v_tenancies.Position == -1) || ((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"] is DBNull)
                tenancy.id_contract = null;
            else
                tenancy.id_contract = Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            if (comboBoxRentType.SelectedValue != null)
                tenancy.id_rent_type = Convert.ToInt32(comboBoxRentType.SelectedValue);
            else
                tenancy.id_rent_type = null;
            if (checkBoxContractEnable.Checked)
            {
                tenancy.id_warrant = warrant_id;
                if (textBoxRegistrationNumber.Text.Trim() == "")
                    tenancy.registration_num = null;
                else
                    tenancy.registration_num = textBoxRegistrationNumber.Text.Trim();
                tenancy.registration_date = dateTimePickerRegistrationDate.Value.Date;
                if (!dateTimePickerEndDate.Checked)
                    tenancy.issue_date = null;
                else
                    tenancy.issue_date = dateTimePickerIssueDate.Value.Date;
                if (!dateTimePickerBeginDate.Checked)
                    tenancy.begin_date = null;
                else
                    tenancy.begin_date = dateTimePickerBeginDate.Value.Date;
                if (!dateTimePickerEndDate.Checked)
                    tenancy.end_date = null;
                else
                    tenancy.end_date = dateTimePickerEndDate.Value.Date;
            }
            else
            {
                tenancy.id_rent_type = null;
                tenancy.registration_num = null;
                tenancy.registration_date = null;
                tenancy.issue_date = null;
                tenancy.begin_date = null;
                tenancy.end_date = null;
            }
            if (checkBoxResidenceWarrantEnable.Checked)
            {
                if (textBoxResidenceWarrantNumber.Text.Trim() == "")
                    tenancy.residence_warrant_num = null;
                else
                    tenancy.residence_warrant_num = textBoxResidenceWarrantNumber.Text.Trim();
                tenancy.residence_warrant_date = dateTimePickerResidenceWarrantDate.Value.Date;
            }
            else
            {
                tenancy.residence_warrant_num = null;
                tenancy.residence_warrant_date = null;
            }
            if (checkBoxKumiOrderEnable.Checked)
            {
                if (textBoxKumiOrderNumber.Text.Trim() == "")
                    tenancy.kumi_order_num = null;
                else
                    tenancy.kumi_order_num = textBoxKumiOrderNumber.Text.Trim();
                tenancy.kumi_order_date = dateTimePickerKumiOrderDate.Value.Date;
            }
            else
            {
                tenancy.kumi_order_num = null;
                tenancy.kumi_order_date = null;
            }
            if (comboBoxExecutor.SelectedValue != null)
                tenancy.id_executor = Convert.ToInt32(comboBoxExecutor.SelectedValue);
            else
                tenancy.id_executor = null;
            if (textBoxDescription.Text.Trim() == "")
                tenancy.description = null;
            else
                tenancy.description = textBoxDescription.Text.Trim();
            return tenancy;
        }

        private void ViewportFromTenancy(Tenancy tenancy)
        {
            if (tenancy.id_rent_type != null)
                comboBoxRentType.SelectedValue = tenancy.id_rent_type;
            else
                comboBoxRentType.SelectedValue = DBNull.Value;
            if (tenancy.id_warrant != null)
            {
                textBoxSelectedWarrant.Text = WarrantStringByID(tenancy.id_warrant.Value);
                warrant_id = tenancy.id_warrant;
            }
            else
            {
                textBoxSelectedWarrant.Text = "";
                warrant_id = null;
            }
            if (tenancy.registration_num != null)
                textBoxRegistrationNumber.Text = tenancy.registration_num;
            else
                textBoxRegistrationNumber.Text = "";
            if (tenancy.registration_date != null)
                dateTimePickerRegistrationDate.Value = tenancy.registration_date.Value;
            else
                dateTimePickerRegistrationDate.Value = DateTime.Now;
            if (tenancy.issue_date != null)
            {
                dateTimePickerIssueDate.Value = tenancy.issue_date.Value;
                dateTimePickerIssueDate.Checked = true;
            }
            else
            {
                dateTimePickerIssueDate.Value = DateTime.Now;
                dateTimePickerIssueDate.Checked = false;
            }
            if (tenancy.begin_date != null)
            {
                dateTimePickerBeginDate.Value = tenancy.begin_date.Value;
                dateTimePickerBeginDate.Checked = true;
            }
            else
            {
                dateTimePickerBeginDate.Value = DateTime.Now;
                dateTimePickerBeginDate.Checked = false;

            }
            if (tenancy.end_date != null)
            {
                dateTimePickerEndDate.Value = tenancy.end_date.Value;
                dateTimePickerEndDate.Checked = true;
            }
            else
            {
                dateTimePickerEndDate.Value = DateTime.Now;
                dateTimePickerEndDate.Checked = false;
            }
            if (tenancy.residence_warrant_num != null)
                textBoxResidenceWarrantNumber.Text = tenancy.residence_warrant_num;
            else
                textBoxResidenceWarrantNumber.Text = "";
            if (tenancy.residence_warrant_date != null)
                dateTimePickerResidenceWarrantDate.Value = tenancy.residence_warrant_date.Value;
            else
                dateTimePickerResidenceWarrantDate.Value = DateTime.Now;
            if (tenancy.kumi_order_num != null)
                textBoxKumiOrderNumber.Text = tenancy.kumi_order_num;
            else
                textBoxKumiOrderNumber.Text = "";
            if (tenancy.kumi_order_date != null)
                dateTimePickerKumiOrderDate.Value = tenancy.kumi_order_date.Value;
            else
                dateTimePickerKumiOrderDate.Value = DateTime.Now;
            if (tenancy.description != null)
                textBoxDescription.Text = tenancy.description;
            else
                textBoxDescription.Text = "";
            if (tenancy.id_executor != null)
                comboBoxExecutor.SelectedValue = tenancy.id_executor;
            else
                comboBoxExecutor.SelectedValue = DBNull.Value;
        }

        public override void Close()
        {
            if (ChangeViewportStateTo(ViewportState.ReadState))
                base.Close();
            persons.Select().RowChanged -= new DataRowChangeEventHandler(Persons_RowChanged);
            persons.Select().RowDeleted -= new DataRowChangeEventHandler(Persons_RowDeleted);
            tenancies.Select().RowChanged -= new DataRowChangeEventHandler(TenancyViewport_RowChanged);
            tenancies.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyViewport_RowDeleted);
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        internal void LocateTenancyBy(int id)
        {
            v_tenancies.Position = v_tenancies.Find("id_contract", id);
        }

        public override bool CanCopyRecord()
        {
            return (v_tenancies.Position != -1) && (viewportState == ViewportState.ReadState || viewportState == ViewportState.ModifyRowState)
                    && !tenancies.EditingNewRecord;
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            Tenancy tenancy = TenancyFromView();
            DataRowView row = (DataRowView)v_tenancies.AddNew();
            tenancies.EditingNewRecord = true;
            ViewportFromTenancy(tenancy);
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        private bool ValidateTenancy(Tenancy tenancy)
        {
            if (checkBoxContractEnable.Checked)
            {
                if (tenancy.registration_num == null)
                {
                    MessageBox.Show("Не указан номер договора найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxRegistrationNumber.Focus();
                    return false;
                }
            }
            if (tenancy.id_executor == null)
            {
                MessageBox.Show("Необходимо выбрать составителя договора", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxExecutor.Focus();
                return false;
            }
            if (tenancy.id_rent_type == null)
            {
                MessageBox.Show("Необходимо выбрать тип найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxRentType.Focus();
                return false;
            }
            return true;
        }

        public override void SaveRecord()
        {
            Tenancy tenancy = TenancyFromViewport();
            if (!ValidateTenancy(tenancy))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ViewportState.NewRowState:
                    int id_contract = tenancies.Insert(tenancy);
                    if (id_contract == -1)
                        return;
                    DataRowView newRow;
                    if (v_tenancies.Position == -1)
                        newRow = (DataRowView)v_tenancies.AddNew();
                    else
                        newRow = ((DataRowView)v_tenancies[v_tenancies.Position]);
                    tenancy.id_contract = id_contract;
                    FillRowFromTenancy(tenancy, newRow);
                    tenancies.EditingNewRecord = false;
                    this.Text = String.Format("Процесс найма №{0}", id_contract.ToString());
                    viewportState = ViewportState.ReadState;
                    is_editable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancy.id_contract == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить процесс найма без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (tenancies.Update(tenancy) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_tenancies[v_tenancies.Position]);
                    FillRowFromTenancy(tenancy, row);
                    viewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
        }

        private void FillRowFromTenancy(Tenancy tenancy, DataRowView row)
        {
            row.BeginEdit();
            row["id_contract"] = tenancy.id_contract;
            row["id_rent_type"] = tenancy.id_rent_type == null ? DBNull.Value : (object)tenancy.id_rent_type;
            row["id_warrant"] = tenancy.id_warrant == null ? DBNull.Value : (object)tenancy.id_warrant;
            row["registration_num"] = tenancy.registration_num == null ? DBNull.Value : (object)tenancy.registration_num;
            row["registration_date"] = tenancy.registration_date == null ? DBNull.Value : (object)tenancy.registration_date;
            row["issue_date"] = tenancy.issue_date == null ? DBNull.Value : (object)tenancy.issue_date;
            row["begin_date"] = tenancy.begin_date == null ? DBNull.Value : (object)tenancy.begin_date;
            row["end_date"] = tenancy.end_date == null ? DBNull.Value : (object)tenancy.end_date;
            row["residence_warrant_num"] = tenancy.residence_warrant_num == null ? DBNull.Value : (object)tenancy.residence_warrant_num;
            row["residence_warrant_date"] = tenancy.residence_warrant_date == null ? DBNull.Value : (object)tenancy.residence_warrant_date;
            row["kumi_order_num"] = tenancy.kumi_order_num == null ? DBNull.Value : (object)tenancy.kumi_order_num;
            row["kumi_order_date"] = tenancy.kumi_order_date == null ? DBNull.Value : (object)tenancy.kumi_order_date;
            row["id_executor"] = tenancy.id_executor == null ? DBNull.Value : (object)tenancy.id_executor;
            row["description"] = tenancy.description == null ? DBNull.Value : (object)tenancy.description;
            row.EndEdit();
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    viewportState = ViewportState.ReadState;
                    tenancies.EditingNewRecord = false;
                    if (v_tenancies.Position != -1)
                    {
                        ((DataRowView)v_tenancies[v_tenancies.Position]).Delete();
                    }
                    else
                        this.Text = "Процессы отсутствуют";
                    break;
                case ViewportState.ModifyRowState:
                    is_editable = false;
                    DataBind();
                    BindWarrantID();
                    UnbindedCheckBoxesUpdate();
                    is_editable = true;
                    viewportState = ViewportState.ReadState;
                    break;
            }
        }

        bool ChangeViewportStateTo(ViewportState state)
        {
            switch (state)
            {
                case ViewportState.ReadState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            return true;
                        case ViewportState.NewRowState:
                        case ViewportState.ModifyRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения о процессе найма в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                            if (tenancies.EditingNewRecord)
                                return false;
                            else
                            {
                                viewportState = ViewportState.NewRowState;
                                return true;
                            }
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения о процессе найма в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                            DialogResult result = MessageBox.Show("Сохранить изменения о помещениях в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancies.MoveFirst();
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancies.MoveLast();
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancies.MoveNext();
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancies.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_tenancies.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_tenancies.Position > 0;
        }

        public override void ClearSearch()
        {
            v_tenancies.Filter = StaticFilter;
            DynamicFilter = "";
        }

        public override bool CanMoveNext()
        {
            return (v_tenancies.Position > -1) && (v_tenancies.Position < (v_tenancies.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_tenancies.Position > -1) && (v_tenancies.Position < (v_tenancies.Count - 1));
        }

        public override bool CanSearchRecord()
        {
            return (viewportState == ViewportState.ReadState);
        }

        public override bool CanInsertRecord()
        {
            if ((viewportState == ViewportState.ReadState || viewportState == ViewportState.ModifyRowState)
                    && !tenancies.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            DataRowView row = (DataRowView)v_tenancies.AddNew();
            int index = v_executors.Find("executor_login", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)v_executors[index])["id_executor"];
            tenancies.EditingNewRecord = true;
        }

        public override bool SearchedRecords()
        {
            if (DynamicFilter != "")
                return true;
            else
                return false;
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (stSimpleSearchForm == null)
                        stSimpleSearchForm = new SimpleSearchTenancyForm();
                    if (stSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = stSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (stExtendedSearchForm == null)
                        stExtendedSearchForm = new ExtendedSearchTenancyForm();
                    if (stExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = stExtendedSearchForm.GetFilter();
                    break;
            }
            string Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                Filter += " AND ";
            Filter += DynamicFilter;
            v_tenancies.Filter += Filter;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот процесс найма?", 
                "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (tenancies.Delete((int)((DataRowView)v_tenancies.Current)["id_contract"]) == -1)
                    return;
                ((DataRowView)v_tenancies[v_tenancies.Position]).Delete();
                menuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanDeleteRecord()
        {
            if ((v_tenancies.Position == -1) || (viewportState == ViewportState.NewRowState))
                return false;
            else
                return true;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            TenancyViewport viewport = new TenancyViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"] as Int32?) ?? -1);
            return viewport;
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                tenancies.EditingNewRecord = false;
            persons.Select().RowChanged -= new DataRowChangeEventHandler(Persons_RowChanged);
            persons.Select().RowDeleted -= new DataRowChangeEventHandler(Persons_RowDeleted);
            tenancies.Select().RowChanged -= new DataRowChangeEventHandler(TenancyViewport_RowChanged);
            tenancies.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyViewport_RowDeleted);
            base.Close();
        }

        public override bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        public override int GetRecordCount()
        {
            return v_tenancies.Count;
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
                DateTime registration_date = Convert.ToDateTime(((DataRowView)v_warrants[row_index])["registration_date"]);
                string registration_num = ((DataRowView)v_warrants[row_index])["registration_num"].ToString();
                return String.Format("№ {0} от {1}", registration_num, registration_date.ToString("dd.MM.yyyy"));
            }
        }

        public override bool HasAssocPersons()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocContractReasons()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocTenancyObjects()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocAgreements()
        {
            return (v_tenancies.Position > -1);
        }

        public override void ShowPersons()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения участников", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PersonsViewport viewport = new PersonsViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowContractReasons()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения оснований найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ContractReasonsViewport viewport = new ContractReasonsViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowAgreements()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения соглашений по найму", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AgreementsViewport viewport = new AgreementsViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowTenancyBuildings()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения нанимаемых зданий", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            TenancyBuildingsViewport viewport = new TenancyBuildingsViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowTenancyPremises()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма для отображения нанимаемых помещений", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            TenancyPremisesViewport viewport = new TenancyPremisesViewport(menuCallback);
            viewport.StaticFilter = "id_contract = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_contract"]);
            viewport.ParentRow = ((DataRowView)v_tenancies[v_tenancies.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Tenancy;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        private void ConstructViewport()
        {
            this.Controls.Add(this.tableLayoutPanel9);
            this.tableLayoutPanel9.SuspendLayout();
            this.groupBoxTenancyContract.SuspendLayout();
            this.groupBoxResidenceWarrant.SuspendLayout();
            this.groupBoxKumiOrder.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox31.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.groupBox25.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContractReasons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAgreements)).BeginInit();
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Controls.Add(this.groupBoxTenancyContract, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.groupBox25, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.groupBox24, 1, 1);
            this.tableLayoutPanel9.Controls.Add(this.groupBoxResidenceWarrant, 0, 2);
            this.tableLayoutPanel9.Controls.Add(this.groupBoxKumiOrder, 1, 2);
            this.tableLayoutPanel9.Controls.Add(this.groupBox21, 0, 3);
            this.tableLayoutPanel9.Controls.Add(this.tableLayoutPanel13, 1, 3);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 4;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 113F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(990, 537);
            this.tableLayoutPanel9.TabIndex = 0;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.panel6, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(978, 88);
            this.tableLayoutPanel10.TabIndex = 1;
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.ColumnCount = 1;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel13.Controls.Add(this.groupBox22, 0, 0);
            this.tableLayoutPanel13.Controls.Add(this.groupBox31, 0, 1);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(498, 358);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 2;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(489, 176);
            this.tableLayoutPanel13.TabIndex = 6;
            // 
            // groupBox18
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.groupBoxTenancyContract, 2);
            this.groupBoxTenancyContract.Controls.Add(this.tableLayoutPanel10);
            this.groupBoxTenancyContract.Controls.Add(this.checkBoxContractEnable);
            this.groupBoxTenancyContract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxTenancyContract.Location = new System.Drawing.Point(3, 3);
            this.groupBoxTenancyContract.Name = "groupBox18";
            this.groupBoxTenancyContract.Size = new System.Drawing.Size(984, 343);
            this.groupBoxTenancyContract.TabIndex = 0;
            this.groupBoxTenancyContract.TabStop = false;
            this.groupBoxTenancyContract.Text = "      Договор найма";
            // 
            // groupBoxResidenceWarrant
            // 
            this.groupBoxResidenceWarrant.Controls.Add(this.label44);
            this.groupBoxResidenceWarrant.Controls.Add(this.label43);
            this.groupBoxResidenceWarrant.Controls.Add(this.textBoxResidenceWarrantNumber);
            this.groupBoxResidenceWarrant.Controls.Add(this.dateTimePickerResidenceWarrantDate);
            this.groupBoxResidenceWarrant.Controls.Add(this.checkBoxResidenceWarrantEnable);
            this.groupBoxResidenceWarrant.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxResidenceWarrant.Location = new System.Drawing.Point(3, 352);
            this.groupBoxResidenceWarrant.Name = "groupBox19";
            this.groupBoxResidenceWarrant.Size = new System.Drawing.Size(489, 77);
            this.groupBoxResidenceWarrant.TabIndex = 3;
            this.groupBoxResidenceWarrant.TabStop = false;
            this.groupBoxResidenceWarrant.Text = "      Ордер на проживание";
            // 
            // groupBoxKumiOrder
            // 
            this.groupBoxKumiOrder.Controls.Add(this.label45);
            this.groupBoxKumiOrder.Controls.Add(this.dateTimePickerKumiOrderDate);
            this.groupBoxKumiOrder.Controls.Add(this.label42);
            this.groupBoxKumiOrder.Controls.Add(this.textBoxKumiOrderNumber);
            this.groupBoxKumiOrder.Controls.Add(this.checkBoxKumiOrderEnable);
            this.groupBoxKumiOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxKumiOrder.Location = new System.Drawing.Point(498, 352);
            this.groupBoxKumiOrder.Name = "groupBox20";
            this.groupBoxKumiOrder.Size = new System.Drawing.Size(489, 77);
            this.groupBoxKumiOrder.TabIndex = 4;
            this.groupBoxKumiOrder.TabStop = false;
            this.groupBoxKumiOrder.Text = "      Распоряжение КУМИ";
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.dataGridViewPersons);
            this.groupBox21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox21.Location = new System.Drawing.Point(3, 435);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(489, 227);
            this.groupBox21.TabIndex = 5;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Участники найма";
            // 
            // groupBox31
            // 
            this.groupBox31.Controls.Add(this.textBoxDescription);
            this.groupBox31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox31.Location = new System.Drawing.Point(3, 88);
            this.groupBox31.Name = "groupBox31";
            this.groupBox31.Size = new System.Drawing.Size(489, 85);
            this.groupBox31.TabIndex = 10;
            this.groupBox31.TabStop = false;
            this.groupBox31.Text = "Дополнительные сведения";
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.comboBoxExecutor);
            this.groupBox22.Controls.Add(this.label41);
            this.groupBox22.Controls.Add(this.comboBoxRentType);
            this.groupBox22.Controls.Add(this.label46);
            this.groupBox22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox22.Location = new System.Drawing.Point(3, 3);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(489, 79);
            this.groupBox22.TabIndex = 9;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Общие сведения";
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.dataGridViewContractReasons);
            this.groupBox24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox24.Location = new System.Drawing.Point(492, 93);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(483, 228);
            this.groupBox24.TabIndex = 2;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "Основания найма";
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.dataGridViewAgreements);
            this.groupBox25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox25.Location = new System.Drawing.Point(3, 93);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(483, 228);
            this.groupBox25.TabIndex = 1;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "Соглашения найма";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.vButtonWarrant);
            this.panel5.Controls.Add(this.textBoxSelectedWarrant);
            this.panel5.Controls.Add(this.label82);
            this.panel5.Controls.Add(this.label48);
            this.panel5.Controls.Add(this.dateTimePickerRegistrationDate);
            this.panel5.Controls.Add(this.textBoxRegistrationNumber);
            this.panel5.Controls.Add(this.label47);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(489, 90);
            this.panel5.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label52);
            this.panel6.Controls.Add(this.label51);
            this.panel6.Controls.Add(this.dateTimePickerEndDate);
            this.panel6.Controls.Add(this.label50);
            this.panel6.Controls.Add(this.dateTimePickerBeginDate);
            this.panel6.Controls.Add(this.label49);
            this.panel6.Controls.Add(this.dateTimePickerIssueDate);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(489, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(489, 90);
            this.panel6.TabIndex = 1;
            // 
            // dataGridViewPersons
            // 
            this.dataGridViewPersons.AllowUserToAddRows = false;
            this.dataGridViewPersons.AllowUserToDeleteRows = false;
            this.dataGridViewPersons.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_surname,
            this.field_name,
            this.field_patronymic,
            this.field_date_of_birth,
            this.field_id_kinship});
            this.dataGridViewPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPersons.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewPersons.Name = "dataGridView12";
            this.dataGridViewPersons.ReadOnly = true;
            this.dataGridViewPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPersons.Size = new System.Drawing.Size(483, 208);
            this.dataGridViewPersons.TabIndex = 0;
            this.dataGridViewPersons.MultiSelect = false;
            this.dataGridViewPersons.AutoGenerateColumns = false;
            // 
            // field_surname
            // 
            this.field_surname.HeaderText = "Фамилия";
            this.field_surname.Name = "surname";
            // 
            // field_name
            // 
            this.field_name.HeaderText = "Имя";
            this.field_name.Name = "name";
            // 
            // field_patronymic
            // 
            this.field_patronymic.HeaderText = "Отчество";
            this.field_patronymic.Name = "patronymic";
            // 
            // field_date_of_birth
            // 
            this.field_date_of_birth.HeaderText = "Дата рождения";
            this.field_date_of_birth.MinimumWidth = 120;
            this.field_date_of_birth.Name = "date_of_birth";
            // 
            // field_id_kinship
            // 
            this.field_id_kinship.HeaderText = "Отношение/связь";
            this.field_id_kinship.Name = "id_kinship";
            this.field_id_kinship.MinimumWidth = 120;
            this.field_id_kinship.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // dataGridViewContractReasons
            // 
            this.dataGridViewContractReasons.AllowUserToAddRows = false;
            this.dataGridViewContractReasons.AllowUserToDeleteRows = false;
            this.dataGridViewContractReasons.Dock = DockStyle.Fill;
            this.dataGridViewContractReasons.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewContractReasons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewContractReasons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_contract_reason_prepared,
            this.field_reason_number,
            this.field_reason_date});
            this.dataGridViewContractReasons.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewContractReasons.Name = "dataGridView14";
            this.dataGridViewContractReasons.ReadOnly = true;
            this.dataGridViewContractReasons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewContractReasons.Size = new System.Drawing.Size(477, 209);
            this.dataGridViewContractReasons.TabIndex = 0;
            this.dataGridViewContractReasons.MultiSelect = false;
            this.dataGridViewContractReasons.AutoGenerateColumns = false;
            // 
            // field_contract_reason_prepared
            // 
            this.field_contract_reason_prepared.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_contract_reason_prepared.HeaderText = "Основание";
            this.field_contract_reason_prepared.Name = "contract_reason_prepared";
            // 
            // field_reason_number
            // 
            this.field_reason_number.HeaderText = "№";
            this.field_reason_number.Name = "reason_number";
            // 
            // field_reason_date
            // 
            this.field_reason_date.HeaderText = "Дата";
            this.field_reason_date.Name = "reason_date";
            // 
            // dataGridViewAgreements
            // 
            this.dataGridViewAgreements.AllowUserToAddRows = false;
            this.dataGridViewAgreements.AllowUserToDeleteRows = false;
            this.dataGridViewAgreements.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewAgreements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAgreements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_agreement_date,
            this.field_agreement_content});
            this.dataGridViewAgreements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAgreements.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewAgreements.Name = "dataGridView15";
            this.dataGridViewAgreements.ReadOnly = true;
            this.dataGridViewAgreements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAgreements.Size = new System.Drawing.Size(477, 209);
            this.dataGridViewAgreements.TabIndex = 0;
            this.dataGridViewAgreements.MultiSelect = false;
            this.dataGridViewAgreements.AutoGenerateColumns = false;
            // 
            // field_agreement_date
            // 
            this.field_agreement_date.HeaderText = "Дата";
            this.field_agreement_date.Name = "agreement_date";
            // 
            // field_agreement_content
            // 
            this.field_agreement_content.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_agreement_content.HeaderText = "Содержание";
            this.field_agreement_content.Name = "agreement_content";
            // 
            // checkBoxContractEnable
            // 
            this.checkBoxContractEnable.AutoSize = true;
            this.checkBoxContractEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxContractEnable.Name = "checkBoxContractEnable";
            this.checkBoxContractEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxContractEnable.TabIndex = 0;
            this.checkBoxContractEnable.UseVisualStyleBackColor = true;
            this.checkBoxContractEnable.Checked = true;
            this.checkBoxContractEnable.CheckedChanged += new EventHandler(checkBoxContractEnable_CheckedChanged);
            // 
            // checkBoxResidenceWarrantEnable
            // 
            this.checkBoxResidenceWarrantEnable.AutoSize = true;
            this.checkBoxResidenceWarrantEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxResidenceWarrantEnable.Name = "checkBoxResidenceWarrantEnable";
            this.checkBoxResidenceWarrantEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxResidenceWarrantEnable.TabIndex = 0;
            this.checkBoxResidenceWarrantEnable.UseVisualStyleBackColor = true;
            this.checkBoxResidenceWarrantEnable.Checked = true;
            this.checkBoxResidenceWarrantEnable.CheckedChanged += new EventHandler(checkBoxResidenceWarrantEnable_CheckedChanged);
            // 
            // checkBoxKumiOrderEnable
            // 
            this.checkBoxKumiOrderEnable.AutoSize = true;
            this.checkBoxKumiOrderEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxKumiOrderEnable.Name = "checkBoxKumiOrderEnable";
            this.checkBoxKumiOrderEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxKumiOrderEnable.TabIndex = 0;
            this.checkBoxKumiOrderEnable.UseVisualStyleBackColor = true;
            this.checkBoxKumiOrderEnable.Checked = true;
            this.checkBoxKumiOrderEnable.CheckedChanged += new EventHandler(checkBoxKumiOrderEnable_CheckedChanged);
            // 
            // label41
            // 
            this.label41.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(12, 51);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(122, 13);
            this.label41.TabIndex = 1;
            this.label41.Text = "Составитель договора";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(12, 22);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(118, 13);
            this.label42.TabIndex = 12;
            this.label42.Text = "Номер распоряжения";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(17, 22);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(80, 13);
            this.label43.TabIndex = 14;
            this.label43.Text = "Номер ордера";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(17, 54);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(72, 13);
            this.label44.TabIndex = 16;
            this.label44.Text = "Дата ордера";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(12, 54);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(110, 13);
            this.label45.TabIndex = 18;
            this.label45.Text = "Дата распоряжения";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(12, 22);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(96, 13);
            this.label46.TabIndex = 16;
            this.label46.Text = "Тип найма жилья";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(14, 7);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(133, 13);
            this.label47.TabIndex = 18;
            this.label47.Text = "Регистрационный номер";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(14, 36);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(100, 13);
            this.label48.TabIndex = 21;
            this.label48.Text = "Дата регистрации";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(15, 7);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(73, 13);
            this.label49.TabIndex = 23;
            this.label49.Text = "Дата выдачи";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(15, 36);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(82, 13);
            this.label50.TabIndex = 25;
            this.label50.Text = "Срок действия";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(143, 36);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(13, 13);
            this.label51.TabIndex = 27;
            this.label51.Text = "с";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(137, 65);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(19, 13);
            this.label52.TabIndex = 28;
            this.label52.Text = "по";
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Location = new System.Drawing.Point(14, 65);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(81, 13);
            this.label82.TabIndex = 23;
            this.label82.Text = "Доверенность";
            // 
            // textBoxSelectedWarrant
            // 
            this.textBoxSelectedWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSelectedWarrant.Location = new System.Drawing.Point(161, 62);
            this.textBoxSelectedWarrant.Name = "textBoxSelectedWarrant";
            this.textBoxSelectedWarrant.ReadOnly = true;
            this.textBoxSelectedWarrant.Size = new System.Drawing.Size(286, 20);
            this.textBoxSelectedWarrant.TabIndex = 22;
            // 
            // textBoxResidenceWarrantNumber
            // 
            this.textBoxResidenceWarrantNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceWarrantNumber.Location = new System.Drawing.Point(164, 19);
            this.textBoxResidenceWarrantNumber.Name = "textBoxResidenceWarrantNumber";
            this.textBoxResidenceWarrantNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxResidenceWarrantNumber.TabIndex = 1;
            this.textBoxResidenceWarrantNumber.MaxLength = 50;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 16);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription3";
            this.textBoxDescription.Size = new System.Drawing.Size(483, 66);
            this.textBoxDescription.TabIndex = 18;
            this.textBoxDescription.MaxLength = 4000;
            // 
            // textBoxKumiOrderNumber
            // 
            this.textBoxKumiOrderNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxKumiOrderNumber.Location = new System.Drawing.Point(159, 19);
            this.textBoxKumiOrderNumber.Name = "textBoxKumiOrderNumber";
            this.textBoxKumiOrderNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxKumiOrderNumber.TabIndex = 1;
            this.textBoxKumiOrderNumber.MaxLength = 50;
            // 
            // textBoxRegistrationNumber
            // 
            this.textBoxRegistrationNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationNumber.Location = new System.Drawing.Point(161, 4);
            this.textBoxRegistrationNumber.Name = "textBoxRegistrationNumber";
            this.textBoxRegistrationNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxRegistrationNumber.TabIndex = 1;
            this.textBoxRegistrationNumber.MaxLength = 16;
            // 
            // dateTimePickerResidenceWarrantDate
            // 
            this.dateTimePickerResidenceWarrantDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerResidenceWarrantDate.Location = new System.Drawing.Point(164, 48);
            this.dateTimePickerResidenceWarrantDate.Name = "dateTimePickerResidenceWarrantDate";
            this.dateTimePickerResidenceWarrantDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerResidenceWarrantDate.TabIndex = 2;
            // 
            // dateTimePickerKumiOrderDate
            // 
            this.dateTimePickerKumiOrderDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerKumiOrderDate.Location = new System.Drawing.Point(159, 48);
            this.dateTimePickerKumiOrderDate.Name = "dateTimePickerKumiOrderDate";
            this.dateTimePickerKumiOrderDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerKumiOrderDate.TabIndex = 2;
            // 
            // dateTimePickerRegistrationDate
            // 
            this.dateTimePickerRegistrationDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerRegistrationDate.Location = new System.Drawing.Point(161, 33);
            this.dateTimePickerRegistrationDate.Name = "dateTimePickerRegistrationDate";
            this.dateTimePickerRegistrationDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerRegistrationDate.TabIndex = 2;
            // 
            // dateTimePickerIssueDate
            // 
            this.dateTimePickerIssueDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIssueDate.Location = new System.Drawing.Point(162, 4);
            this.dateTimePickerIssueDate.Name = "dateTimePickerIssueDate";
            this.dateTimePickerIssueDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerIssueDate.TabIndex = 0;
            this.dateTimePickerIssueDate.ShowCheckBox = true;
            // 
            // dateTimePickerBeginDate
            // 
            this.dateTimePickerBeginDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerBeginDate.Location = new System.Drawing.Point(162, 33);
            this.dateTimePickerBeginDate.Name = "dateTimePickerBeginDate";
            this.dateTimePickerBeginDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerBeginDate.TabIndex = 1;
            this.dateTimePickerBeginDate.ShowCheckBox = true;
            // 
            // dateTimePickerEndDate
            // 
            this.dateTimePickerEndDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDate.Location = new System.Drawing.Point(162, 62);
            this.dateTimePickerEndDate.Name = "dateTimePickerEndDate";
            this.dateTimePickerEndDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerEndDate.TabIndex = 3;
            this.dateTimePickerEndDate.ShowCheckBox = true;
            // 
            // comboBoxExecutor
            // 
            this.comboBoxExecutor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxExecutor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecutor.FormattingEnabled = true;
            this.comboBoxExecutor.Location = new System.Drawing.Point(159, 48);
            this.comboBoxExecutor.Name = "comboBoxExecutor";
            this.comboBoxExecutor.Size = new System.Drawing.Size(319, 21);
            this.comboBoxExecutor.TabIndex = 1;
            // 
            // comboBoxRentType
            // 
            this.comboBoxRentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRentType.FormattingEnabled = true;
            this.comboBoxRentType.Location = new System.Drawing.Point(159, 19);
            this.comboBoxRentType.Name = "comboBoxRentType";
            this.comboBoxRentType.Size = new System.Drawing.Size(319, 21);
            this.comboBoxRentType.TabIndex = 0;
            // 
            // vButtonWarrant
            // 
            this.vButtonWarrant.AllowAnimations = true;
            this.vButtonWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonWarrant.BackColor = System.Drawing.Color.Transparent;
            this.vButtonWarrant.Location = new System.Drawing.Point(453, 62);
            this.vButtonWarrant.Name = "vButtonWarrant";
            this.vButtonWarrant.RoundedCornersMask = ((byte)(15));
            this.vButtonWarrant.Size = new System.Drawing.Size(27, 20);
            this.vButtonWarrant.TabIndex = 24;
            this.vButtonWarrant.Text = "...";
            this.vButtonWarrant.UseVisualStyleBackColor = false;
            this.vButtonWarrant.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;

            this.tableLayoutPanel9.ResumeLayout(false);
            this.groupBoxTenancyContract.ResumeLayout(false);
            this.groupBoxTenancyContract.PerformLayout();
            this.groupBoxResidenceWarrant.ResumeLayout(false);
            this.groupBoxResidenceWarrant.PerformLayout();
            this.groupBoxKumiOrder.ResumeLayout(false);
            this.groupBoxKumiOrder.PerformLayout();
            this.groupBox21.ResumeLayout(false);
            this.groupBox21.PerformLayout();
            this.tableLayoutPanel13.ResumeLayout(false);
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.groupBox31.ResumeLayout(false);
            this.groupBox31.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.groupBox24.ResumeLayout(false);
            this.groupBox25.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContractReasons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAgreements)).EndInit();
        }
    }
}
