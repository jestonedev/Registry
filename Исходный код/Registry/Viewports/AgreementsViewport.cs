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

namespace Registry.Viewport
{
    internal sealed class AgreementsViewport: Viewport
    {
        #region Components  
        private TableLayoutPanel tableLayoutPanel12 = new TableLayoutPanel();
        private GroupBox groupBox29 = new GroupBox();
        private GroupBox groupBox30 = new GroupBox();
        private TabControl tabControl1 = new TabControl();
        private Panel panel7 = new System.Windows.Forms.Panel();
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_agreement = new DataGridViewTextBoxColumn();
        private DateGridViewDateTimeColumn field_agreement_date = new DateGridViewDateTimeColumn();
        private DataGridViewTextBoxColumn field_agreement_content = new DataGridViewTextBoxColumn();
        private DataGridView dataGridViewPersons = new DataGridView();
        private DataGridViewTextBoxColumn field_surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_date_of_birth = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private TextBox textBoxAgreementContent = new TextBox();
        private TextBox textBoxAgreementWarrant = new TextBox();
        private TextBox textBoxExcludePoint = new TextBox();
        private TextBox textBoxIncludePoint = new TextBox();
        private TextBox textBoxIncludeSNP = new TextBox();
        private TextBox textBoxExplainPoint = new TextBox();
        private TextBox textBoxExplainContent = new TextBox();
        private TextBox textBoxTerminateAgreement = new TextBox();
        private TabPage tabPageExclude = new TabPage();
        private TabPage tabPageInclude = new TabPage();
        private TabPage tabPageExplain = new TabPage();
        private TabPage tabPageTerminate = new TabPage();
        private Label label71 = new Label();
        private Label label72 = new Label();
        private Label label73 = new Label();
        private Label label74 = new Label();
        private Label label75 = new Label();
        private Label label76 = new Label();
        private Label label77 = new Label();
        private Label label78 = new Label();
        private Label label79 = new Label();
        private Label label80 = new Label();
        private ComboBox comboBoxExecutor = new ComboBox();
        private ComboBox comboBoxIncludeKinship = new ComboBox();
        private VIBlend.WinForms.Controls.vButton vButtonSelectWarrant = new VIBlend.WinForms.Controls.vButton();
        private VIBlend.WinForms.Controls.vButton vButtonExcludePaste = new VIBlend.WinForms.Controls.vButton();
        private VIBlend.WinForms.Controls.vButton vButtonIncludePaste = new VIBlend.WinForms.Controls.vButton();
        private VIBlend.WinForms.Controls.vButton vButtonExplainPaste = new VIBlend.WinForms.Controls.vButton();
        private VIBlend.WinForms.Controls.vButton vButtonTerminatePaste = new VIBlend.WinForms.Controls.vButton();
        private DateTimePicker dateTimePickerAgreementDate = new DateTimePicker();
        private DateTimePicker dateTimePickerIncludeDateOfBirth = new DateTimePicker();
        #endregion Components

        //Modeles
        PersonsDataModel persons = null;
        AgreementsDataModel agreements = null;
        ExecutorsDataModel executors = null;
        WarrantsDataModel warrants = null;
        KinshipsDataModel kinships = null;

        //Views
        BindingSource v_persons = null;
        BindingSource v_agreements = null;
        BindingSource v_executors = null;
        BindingSource v_warrants = null;
        BindingSource v_kinships = null;

        //Forms
        private SelectWarrantForm swForm = null;


        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;
        private int? warrant_id = null;

        public AgreementsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageAgreements";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Соглашения найма №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public AgreementsViewport(AgreementsViewport agreementsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = agreementsViewport.DynamicFilter;
            this.StaticFilter = agreementsViewport.StaticFilter;
            this.ParentRow = agreementsViewport.ParentRow;
            this.ParentType = agreementsViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            agreements = AgreementsDataModel.GetInstance();
            persons = PersonsDataModel.GetInstance();
            executors = ExecutorsDataModel.GetInstance();
            warrants = WarrantsDataModel.GetInstance();
            kinships = KinshipsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            agreements.Select();
            persons.Select();
            executors.Select();
            warrants.Select();
            kinships.Select();

            DataSet ds = DataSetManager.GetDataSet();

            if ((ParentType == ParentTypeEnum.Tenancy) && (ParentRow != null))
                this.Text = String.Format("Соглашения найма №{0}", ParentRow["id_contract"].ToString());
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_persons = new BindingSource();
            v_persons.DataMember = "persons";
            v_persons.Filter = StaticFilter;
            v_persons.DataSource = ds;

            v_executors = new BindingSource();
            v_executors.DataMember = "executors";
            v_executors.DataSource = ds;

            v_warrants = new BindingSource();
            v_warrants.DataMember = "warrants";
            v_warrants.DataSource = ds;

            v_kinships = new BindingSource();
            v_kinships.DataMember = "kinships";
            v_kinships.DataSource = ds;

            v_agreements = new BindingSource();
            v_agreements.CurrentItemChanged += new EventHandler(v_agreements_CurrentItemChanged);
            v_agreements.DataMember = "agreements";
            v_agreements.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_agreements.Filter += " AND ";
            v_agreements.Filter += DynamicFilter;
            v_agreements.DataSource = ds;

            DataBind();

            dateTimePickerAgreementDate.ValueChanged += new EventHandler(dateTimePickerAgreementDate_ValueChanged);
            textBoxAgreementWarrant.TextChanged += new EventHandler(textBoxAgreementWarrant_TextChanged);
            comboBoxExecutor.TextChanged += new EventHandler(comboBoxExecutor_TextChanged);
            textBoxAgreementContent.TextChanged += new EventHandler(textBoxAgreementContent_TextChanged);
            persons.Select().RowDeleted += new DataRowChangeEventHandler(AgreementsViewport_RowDeleted);
            persons.Select().RowChanged += new DataRowChangeEventHandler(AgreementsViewport_RowChanged);
            vButtonSelectWarrant.Click += new EventHandler(vButtonSelectWarrant_Click);
            dataGridView.DataError += new DataGridViewDataErrorEventHandler(dataGridView_DataError);
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        void vButtonSelectWarrant_Click(object sender, EventArgs e)
        {
            if (warrant_id != null)
            {
                warrant_id = null;
                textBoxAgreementWarrant.Text = "";
                vButtonSelectWarrant.Text = "...";
                return;
            }
            if (swForm == null)
                swForm = new SelectWarrantForm();
            if (swForm.ShowDialog() == DialogResult.OK)
            {
                if (swForm.WarrantID != null)
                {
                    warrant_id = swForm.WarrantID.Value;
                    textBoxAgreementWarrant.Text = WarrantStringByID(swForm.WarrantID.Value);
                    vButtonSelectWarrant.Text = "x";
                }
            }
        }

        private void RedrawDataGridPersonsRows()
        {
            if (dataGridViewPersons.Rows.Count == 0)
                return;
            for (int i = 0; i < dataGridViewPersons.Rows.Count; i++)
                if (((DataRowView)v_persons[i])["id_kinship"] != DBNull.Value && Convert.ToInt32(((DataRowView)v_persons[i])["id_kinship"]) == 1)
                    dataGridViewPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    dataGridViewPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridPersonsRows();
            base.OnVisibleChanged(e);
        }

        void AgreementsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridPersonsRows();
        }

        void AgreementsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridPersonsRows();
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

        private void DataBind()
        {
            comboBoxExecutor.DataSource = v_executors;
            comboBoxExecutor.ValueMember = "id_executor";
            comboBoxExecutor.DisplayMember = "executor_name";
            comboBoxExecutor.DataBindings.Clear();
            comboBoxExecutor.DataBindings.Add("SelectedValue", v_agreements, "id_executor", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxIncludeKinship.DataSource = v_kinships;
            comboBoxIncludeKinship.ValueMember = "id_kinship";
            comboBoxIncludeKinship.DisplayMember = "kinship";

            textBoxAgreementContent.DataBindings.Clear();
            textBoxAgreementContent.DataBindings.Add("Text", v_agreements, "agreement_content", true, DataSourceUpdateMode.Never, "");

            dateTimePickerAgreementDate.DataBindings.Clear();
            dateTimePickerAgreementDate.DataBindings.Add("Value", v_agreements, "agreement_date", true, DataSourceUpdateMode.Never, DateTime.Now);

            dataGridViewPersons.DataSource = v_persons;
            field_surname.DataPropertyName = "surname";
            field_name.DataPropertyName = "name";
            field_patronymic.DataPropertyName = "patronymic";
            field_date_of_birth.DataPropertyName = "date_of_birth";

            dataGridView.DataSource = v_agreements;
            field_id_agreement.DataPropertyName = "id_agreement";
            field_agreement_date.DataPropertyName = "agreement_date";
            field_agreement_content.DataPropertyName = "agreement_content";
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!this.ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_agreements.Position != -1) && (AgreementFromView() != AgreementFromViewport()))
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
            menuCallback.EditingStateUpdate();
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            Agreement agreement = AgreementFromView();
            DataRowView row = (DataRowView)v_agreements.AddNew();
            dataGridView.Enabled = false;
            agreements.EditingNewRecord = true;
            ViewportFromAgreement(agreement);
        }

        private void ViewportFromAgreement(Agreement agreement)
        {
            if (agreement.id_executor != null)
                comboBoxExecutor.SelectedValue = agreement.id_executor;
            else
                comboBoxExecutor.SelectedValue = DBNull.Value;
            if (agreement.id_warrant != null)
            {
                textBoxAgreementWarrant.Text = WarrantStringByID(agreement.id_warrant.Value);
                warrant_id = agreement.id_warrant;
            }
            else
            {
                textBoxAgreementWarrant.Text = "";
                warrant_id = null;
            }
            textBoxAgreementContent.Text = agreement.agreement_content;
            if (agreement.agreement_date != null)
                dateTimePickerAgreementDate.Value = agreement.agreement_date.Value;
            else
                dateTimePickerAgreementDate.Value = DateTime.Now;
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
                DateTime registration_date = Convert.ToDateTime(((DataRowView)v_warrants[row_index])["registation_date"]);
                string registration_num = ((DataRowView)v_warrants[row_index])["registation_num"].ToString();
                return String.Format("№ {0} от {1}", registration_num, registration_date.ToString("dd.MM.yyyy"));
            }
        }

        private Agreement AgreementFromViewport()
        {
            Agreement agreement = new Agreement();
            if ((v_agreements.Position == -1) || ((DataRowView)v_agreements[v_agreements.Position])["id_agreement"] is DBNull)
                agreement.id_agreement = null;
            else
                agreement.id_agreement = Convert.ToInt32(((DataRowView)v_agreements[v_agreements.Position])["id_agreement"]);
            if (ParentType == ParentTypeEnum.Tenancy && ParentRow != null)
                agreement.id_contract = Convert.ToInt32(ParentRow["id_contract"]);
            else
                agreement.id_contract = null;
            if (comboBoxExecutor.SelectedValue == null)
                agreement.id_executor = null;
            else
                agreement.id_executor = Convert.ToInt32(comboBoxExecutor.SelectedValue);
            agreement.id_warrant = warrant_id;     
            if (textBoxAgreementContent.Text.Trim() != "")
                agreement.agreement_content = textBoxAgreementContent.Text.Trim();
            else
                agreement.agreement_content = null;
            if (dateTimePickerAgreementDate.Checked)
                agreement.agreement_date = dateTimePickerAgreementDate.Value.Date;
            else
                agreement.agreement_date = null;
            return agreement;
        }

        private Agreement AgreementFromView()
        {
            Agreement agreement = new Agreement();
            DataRowView row = (DataRowView)v_agreements[v_agreements.Position];
            if (row["id_agreement"] is DBNull)
                agreement.id_agreement = null;
            else
                agreement.id_agreement = Convert.ToInt32(row["id_agreement"]);
            if (row["id_contract"] is DBNull)
                agreement.id_contract = null;
            else
                agreement.id_contract = Convert.ToInt32(row["id_contract"]);
            if (row["id_executor"] is DBNull)
                agreement.id_executor = null;
            else
                agreement.id_executor = Convert.ToInt32(row["id_executor"]);
            if (row["id_warrant"] is DBNull)
                agreement.id_warrant = null;
            else
                agreement.id_warrant = Convert.ToInt32(row["id_warrant"]);
            if (row["agreement_date"] is DBNull)
                agreement.agreement_date = null;
            else
                agreement.agreement_date = Convert.ToDateTime(row["agreement_date"]);
            if (row["agreement_content"] is DBNull)
                agreement.agreement_content = null;
            else
                agreement.agreement_content = row["agreement_content"].ToString();
            return agreement;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_agreements.MoveFirst();
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_agreements.MoveLast();
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_agreements.MoveNext();
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_agreements.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_agreements.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_agreements.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_agreements.Position > -1) && (v_agreements.Position < (v_agreements.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_agreements.Position > -1) && (v_agreements.Position < (v_agreements.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            if ((viewportState == ViewportState.ReadState || viewportState == ViewportState.ModifyRowState) && !agreements.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            DataRowView row = (DataRowView)v_agreements.AddNew();
            dataGridView.Enabled = false;
            int index = v_executors.Find("executor_login", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)v_executors[index])["id_executor"];
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                textBoxAgreementContent.Text = String.Format("1.1 По настоящему Соглашению Стороны по договору № {0} от {1} договорились",
                    ParentRow["registration_num"].ToString(), 
                    ParentRow["registration_date"] != DBNull.Value ? Convert.ToDateTime(ParentRow["registration_date"]).ToString("dd.MM.yyyy") : "");
            agreements.EditingNewRecord = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите это соглашение?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (agreements.Delete((int)((DataRowView)v_agreements.Current)["id_agreement"]) == -1)
                    return;
                ((DataRowView)v_agreements[v_agreements.Position]).Delete();
                menuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanDeleteRecord()
        {
            if ((v_agreements.Position == -1) || (viewportState == ViewportState.NewRowState))
                return false;
            else
                return true;
        }

        private void FillRowFromAgreement(Agreement agreement, DataRowView row)
        {
            row.BeginEdit();
            row["id_agreement"] = agreement.id_agreement == null ? DBNull.Value : (object)agreement.id_agreement;
            row["id_contract"] = agreement.id_contract == null ? DBNull.Value : (object)agreement.id_contract;
            row["agreement_date"] = agreement.agreement_date == null ? DBNull.Value : (object)agreement.agreement_date;
            row["agreement_content"] = agreement.agreement_content == null ? DBNull.Value : (object)agreement.agreement_content;
            row["id_executor"] = agreement.id_executor == null ? DBNull.Value : (object)agreement.id_executor;
            row["id_warrant"] = agreement.id_warrant == null ? DBNull.Value : (object)agreement.id_warrant;
            row.EndEdit();
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    viewportState = ViewportState.ReadState;
                    agreements.EditingNewRecord = false;
                    if (v_agreements.Position != -1)
                    {
                        dataGridView.Enabled = true;
                        ((DataRowView)v_agreements[v_agreements.Position]).Delete();
                    }
                    else
                        this.Text = "Соглашения отсутствуют";
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    BindWarrantID();
                    is_editable = true;
                    viewportState = ViewportState.ReadState;
                    break;
            }
        }

        private void BindWarrantID()
        {
            if ((v_agreements.Position > -1) && ((DataRowView)v_agreements[v_agreements.Position])["id_warrant"] != DBNull.Value)
            {
                warrant_id = Convert.ToInt32(((DataRowView)v_agreements[v_agreements.Position])["id_warrant"]);
                textBoxAgreementWarrant.Text =
                    WarrantStringByID(warrant_id.Value);
                vButtonSelectWarrant.Text = "x";
            }
            else
            {
                warrant_id = null;
                textBoxAgreementWarrant.Text = "";
                vButtonSelectWarrant.Text = "...";
            }
        }

        public override void Close()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            persons.Select().RowDeleted -= new DataRowChangeEventHandler(AgreementsViewport_RowDeleted);
            persons.Select().RowChanged -= new DataRowChangeEventHandler(AgreementsViewport_RowChanged);
            base.Close();
        }

        private bool ChangeViewportStateTo(ViewportState state)
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            if (agreements.EditingNewRecord)
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                agreements.EditingNewRecord = false;
            persons.Select().RowDeleted -= new DataRowChangeEventHandler(AgreementsViewport_RowDeleted);
            persons.Select().RowChanged -= new DataRowChangeEventHandler(AgreementsViewport_RowChanged);
            base.Close();
        }

        public override bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void SaveRecord()
        {
            Agreement agreement = AgreementFromViewport();
            if (!ValidateAgreement(agreement))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                case ViewportState.NewRowState:
                    int id_agreement = agreements.Insert(agreement);
                    if (id_agreement == -1)
                        return;
                    DataRowView newRow;
                    if (v_agreements.Position == -1)
                        newRow = (DataRowView)v_agreements.AddNew();
                    else
                        newRow = ((DataRowView)v_agreements[v_agreements.Position]);
                    agreement.id_agreement = id_agreement;
                    FillRowFromAgreement(agreement, newRow);
                    agreements.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (agreement.id_agreement == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить соглашение без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (agreements.Update(agreement) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_agreements[v_agreements.Position]);
                    FillRowFromAgreement(agreement, row);
                    break;
            }
            viewportState = ViewportState.ReadState;
            dataGridView.Enabled = true;
            is_editable = true;
        }

        private bool ValidateAgreement(Agreement agreement)
        {
            if (agreement.id_executor == null)
            {
                MessageBox.Show("Необходимо выбрать исполнителя", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxExecutor.Focus();
                return false;
            }
            return true;
        }

        public override bool CanCopyRecord()
        {
            return ((v_agreements.Position != -1) && (!agreements.EditingNewRecord));
        }

        void v_agreements_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
            dataGridView.Enabled = true;
            BindWarrantID();
            if (v_agreements.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            AgreementsViewport viewport = new AgreementsViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_agreements.Count > 0)
                viewport.LocateAgreementBy((((DataRowView)v_agreements[v_agreements.Position])["id_agreement"] as Int32?) ?? -1);
            return viewport;
        }

        private void LocateAgreementBy(int id)
        {
            int Position = v_agreements.Find("id_agreement", id);
            if (Position > 0)
                v_agreements.Position = Position;
        }

        private void ConstructViewport()
        {
            this.Controls.Add(tableLayoutPanel12);
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            this.tableLayoutPanel12.SuspendLayout();
            this.groupBox30.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.groupBox29.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tabPageExclude.SuspendLayout();
            this.tabPageInclude.SuspendLayout();
            this.tabPageExplain.SuspendLayout();
            this.tabPageTerminate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).BeginInit();
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
            this.tableLayoutPanel12.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 3;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.72365F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.27634F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(990, 537);
            this.tableLayoutPanel12.TabIndex = 0;
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
            this.groupBox29.Size = new System.Drawing.Size(489, 104);
            this.groupBox29.TabIndex = 0;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "Общие сведения";
            // 
            // groupBox30
            // 
            this.groupBox30.Controls.Add(this.textBoxAgreementContent);
            this.groupBox30.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox30.Location = new System.Drawing.Point(498, 3);
            this.groupBox30.Name = "groupBox30";
            this.tableLayoutPanel12.SetRowSpan(this.groupBox30, 2);
            this.groupBox30.Size = new System.Drawing.Size(489, 232);
            this.groupBox30.TabIndex = 1;
            this.groupBox30.TabStop = false;
            this.groupBox30.Text = "Содержание";
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
            this.tabControl1.Size = new System.Drawing.Size(495, 128);
            this.tabControl1.TabIndex = 1;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.groupBox29);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(3, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(489, 104);
            this.panel7.TabIndex = 0;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = 
                new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_agreement,
            this.field_agreement_date,
            this.field_agreement_content});
            this.tableLayoutPanel12.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Location = new System.Drawing.Point(3, 256);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(984, 278);
            this.dataGridView.TabIndex = 2;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.ReadOnly = true;
            this.dataGridView.AutoGenerateColumns = false;
            // 
            // field_id_agreement
            // 
            this.field_id_agreement.HeaderText = "Номер соглашения";
            this.field_id_agreement.MinimumWidth = 100;
            this.field_id_agreement.Name = "id_agreement";
            this.field_id_agreement.Visible = false;
            // 
            // field_agreement_date
            // 
            this.field_agreement_date.HeaderText = "Дата соглашения";
            this.field_agreement_date.MinimumWidth = 100;
            this.field_agreement_date.Name = "agreement_date";
            // 
            // field_agreement_content
            // 
            this.field_agreement_content.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_agreement_content.FillWeight = 500F;
            this.field_agreement_content.HeaderText = "Содержание";
            this.field_agreement_content.MinimumWidth = 100;
            this.field_agreement_content.Name = "agreement_content";
            // 
            // dataGridViewPersons
            // 
            this.dataGridViewPersons.AllowUserToAddRows = false;
            this.dataGridViewPersons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewPersons.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewPersons.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewPersons.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = 
                new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPersons.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridViewPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_surname,
            this.field_name,
            this.field_patronymic,
            this.field_date_of_birth});
            this.dataGridViewPersons.Location = new System.Drawing.Point(3, 32);
            this.dataGridViewPersons.Name = "dataGridView18";
            this.dataGridViewPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPersons.Size = new System.Drawing.Size(481, 82);
            this.dataGridViewPersons.TabIndex = 2;
            this.dataGridViewPersons.MultiSelect = false;
            this.dataGridViewPersons.AutoGenerateColumns = false;
            this.dataGridViewPersons.ReadOnly = true;
            // 
            // field_surname
            // 
            this.field_surname.HeaderText = "Фамилия";
            this.field_surname.MinimumWidth = 100;
            this.field_surname.Name = "surname";
            this.field_surname.ReadOnly = true;
            // 
            // field_name
            // 
            this.field_name.HeaderText = "Имя";
            this.field_name.MinimumWidth = 100;
            this.field_name.Name = "name";
            this.field_name.ReadOnly = true;
            // 
            // field_patronymic
            // 
            this.field_patronymic.HeaderText = "Отчество";
            this.field_patronymic.MinimumWidth = 100;
            this.field_patronymic.Name = "patronymic";
            this.field_patronymic.ReadOnly = true;
            // 
            // field_date_of_birth
            // 
            this.field_date_of_birth.HeaderText = "Дата рождения";
            this.field_date_of_birth.MinimumWidth = 120;
            this.field_date_of_birth.Name = "date_of_birth";
            this.field_date_of_birth.ReadOnly = true;
            // 
            // textBoxAgreementContent
            // 
            this.textBoxAgreementContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAgreementContent.Location = new System.Drawing.Point(3, 16);
            this.textBoxAgreementContent.Multiline = true;
            this.textBoxAgreementContent.Name = "textBoxAgreementContent";
            this.textBoxAgreementContent.Size = new System.Drawing.Size(483, 228);
            this.textBoxAgreementContent.TabIndex = 1;
            this.textBoxAgreementContent.MaxLength = 4000;
            // 
            // textBoxAgreementWarrant
            // 
            this.textBoxAgreementWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAgreementWarrant.Location = new System.Drawing.Point(164, 48);
            this.textBoxAgreementWarrant.Name = "textBoxAgreementWarrant";
            this.textBoxAgreementWarrant.Size = new System.Drawing.Size(286, 20);
            this.textBoxAgreementWarrant.TabIndex = 1;
            this.textBoxAgreementWarrant.ReadOnly = true;
            // 
            // textBoxIncludePoint
            // 
            this.textBoxExcludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcludePoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxExcludePoint.Name = "textBoxIncludePoint";
            this.textBoxExcludePoint.Size = new System.Drawing.Size(286, 20);
            this.textBoxExcludePoint.TabIndex = 0;
            // 
            // textBoxExcludePoint
            // 
            this.textBoxIncludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludePoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxIncludePoint.Name = "textBoxExcludePoint";
            this.textBoxIncludePoint.Size = new System.Drawing.Size(285, 20);
            this.textBoxIncludePoint.TabIndex = 0;
            // 
            // textBoxExcludeSNP
            // 
            this.textBoxIncludeSNP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludeSNP.Location = new System.Drawing.Point(163, 32);
            this.textBoxIncludeSNP.Name = "textBoxExcludeSNP";
            this.textBoxIncludeSNP.Size = new System.Drawing.Size(285, 20);
            this.textBoxIncludeSNP.TabIndex = 1;
            // 
            // textBoxExplainPoint
            // 
            this.textBoxExplainPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExplainPoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxExplainPoint.Name = "textBoxExplainPoint";
            this.textBoxExplainPoint.Size = new System.Drawing.Size(285, 20);
            this.textBoxExplainPoint.TabIndex = 0;
            // 
            // textBoxExplainContent
            // 
            this.textBoxExplainContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExplainContent.Location = new System.Drawing.Point(7, 32);
            this.textBoxExplainContent.Multiline = true;
            this.textBoxExplainContent.Name = "textBoxExplainContent";
            this.textBoxExplainContent.Size = new System.Drawing.Size(475, 83);
            this.textBoxExplainContent.TabIndex = 1;
            // 
            // textBoxTerminatePoint
            // 
            this.textBoxTerminateAgreement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTerminateAgreement.Location = new System.Drawing.Point(163, 6);
            this.textBoxTerminateAgreement.Name = "textBoxTerminatePoint";
            this.textBoxTerminateAgreement.Size = new System.Drawing.Size(285, 20);
            this.textBoxTerminateAgreement.TabIndex = 0;
            // 
            // tabPageExclude
            // 
            this.tabPageExclude.Controls.Add(this.dataGridViewPersons);
            this.tabPageExclude.Controls.Add(this.vButtonExcludePaste);
            this.tabPageExclude.Controls.Add(this.textBoxExcludePoint);
            this.tabPageExclude.Controls.Add(this.label74);
            this.tabPageExclude.Location = new System.Drawing.Point(4, 22);
            this.tabPageExclude.Name = "tabPageExclude";
            this.tabPageExclude.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExclude.Size = new System.Drawing.Size(487, 117);
            this.tabPageExclude.TabIndex = 0;
            this.tabPageExclude.Text = "Исключить";
            this.tabPageExclude.UseVisualStyleBackColor = true;
            // 
            // tabPageInclude
            // 
            this.tabPageInclude.Controls.Add(this.dateTimePickerIncludeDateOfBirth);
            this.tabPageInclude.Controls.Add(this.comboBoxIncludeKinship);
            this.tabPageInclude.Controls.Add(this.label76);
            this.tabPageInclude.Controls.Add(this.label77);
            this.tabPageInclude.Controls.Add(this.textBoxIncludeSNP);
            this.tabPageInclude.Controls.Add(this.textBoxIncludePoint);
            this.tabPageInclude.Controls.Add(this.label78);
            this.tabPageInclude.Controls.Add(this.vButtonIncludePaste);
            this.tabPageInclude.Controls.Add(this.label75);
            this.tabPageInclude.Location = new System.Drawing.Point(4, 22);
            this.tabPageInclude.Name = "tabPageInclude";
            this.tabPageInclude.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInclude.Size = new System.Drawing.Size(487, 117);
            this.tabPageInclude.TabIndex = 1;
            this.tabPageInclude.Text = "Включить";
            this.tabPageInclude.UseVisualStyleBackColor = true;
            // 
            // tabPageExplain
            // 
            this.tabPageExplain.Controls.Add(this.textBoxExplainContent);
            this.tabPageExplain.Controls.Add(this.textBoxExplainPoint);
            this.tabPageExplain.Controls.Add(this.vButtonExplainPaste);
            this.tabPageExplain.Controls.Add(this.label79);
            this.tabPageExplain.Location = new System.Drawing.Point(4, 22);
            this.tabPageExplain.Name = "tabPageExplain";
            this.tabPageExplain.Size = new System.Drawing.Size(487, 117);
            this.tabPageExplain.TabIndex = 2;
            this.tabPageExplain.Text = "Изложить";
            this.tabPageExplain.UseVisualStyleBackColor = true;
            // 
            // tabPageTerminate
            // 
            this.tabPageTerminate.Controls.Add(this.vButtonTerminatePaste);
            this.tabPageTerminate.Controls.Add(this.textBoxTerminateAgreement);
            this.tabPageTerminate.Controls.Add(this.label80);
            this.tabPageTerminate.Location = new System.Drawing.Point(4, 22);
            this.tabPageTerminate.Name = "tabPageTerminate";
            this.tabPageTerminate.Size = new System.Drawing.Size(487, 117);
            this.tabPageTerminate.TabIndex = 3;
            this.tabPageTerminate.Text = "Расторгнуть";
            this.tabPageTerminate.UseVisualStyleBackColor = true;
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Location = new System.Drawing.Point(17, 23);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(97, 13);
            this.label71.TabIndex = 33;
            this.label71.Text = "Дата соглашения";
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Location = new System.Drawing.Point(17, 51);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(95, 13);
            this.label72.TabIndex = 35;
            this.label72.Text = "По доверенности";
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(17, 80);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(74, 13);
            this.label73.TabIndex = 38;
            this.label73.Text = "Исполнитель";
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(12, 9);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(55, 13);
            this.label74.TabIndex = 37;
            this.label74.Text = "Подпункт";
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(12, 9);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(55, 13);
            this.label75.TabIndex = 40;
            this.label75.Text = "Подпункт";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(12, 93);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(99, 13);
            this.label76.TabIndex = 46;
            this.label76.Text = "Отношение/связь";
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Location = new System.Drawing.Point(12, 65);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(86, 13);
            this.label77.TabIndex = 45;
            this.label77.Text = "Дата рождения";
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(12, 35);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(34, 13);
            this.label78.TabIndex = 43;
            this.label78.Text = "ФИО";
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Location = new System.Drawing.Point(12, 9);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(55, 13);
            this.label79.TabIndex = 40;
            this.label79.Text = "Подпункт";
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Location = new System.Drawing.Point(12, 9);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(55, 13);
            this.label80.TabIndex = 43;
            this.label80.Text = "№ соглашения";
            // 
            // comboBoxExecutor
            // 
            this.comboBoxExecutor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxExecutor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecutor.FormattingEnabled = true;
            this.comboBoxExecutor.Location = new System.Drawing.Point(164, 77);
            this.comboBoxExecutor.Name = "comboBoxExecutor";
            this.comboBoxExecutor.Size = new System.Drawing.Size(319, 21);
            this.comboBoxExecutor.TabIndex = 3;
            // 
            // comboBoxIncludeKinship
            // 
            this.comboBoxIncludeKinship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIncludeKinship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIncludeKinship.FormattingEnabled = true;
            this.comboBoxIncludeKinship.Location = new System.Drawing.Point(163, 90);
            this.comboBoxIncludeKinship.Name = "comboBoxIncludeKinship";
            this.comboBoxIncludeKinship.Size = new System.Drawing.Size(285, 21);
            this.comboBoxIncludeKinship.TabIndex = 3;
            // 
            // vButtonSelectWarrant
            // 
            this.vButtonSelectWarrant.AllowAnimations = true;
            this.vButtonSelectWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonSelectWarrant.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSelectWarrant.Location = new System.Drawing.Point(456, 48);
            this.vButtonSelectWarrant.Name = "vButtonSelectWarrant";
            this.vButtonSelectWarrant.RoundedCornersMask = ((byte)(15));
            this.vButtonSelectWarrant.Size = new System.Drawing.Size(27, 20);
            this.vButtonSelectWarrant.TabIndex = 2;
            this.vButtonSelectWarrant.Text = "...";
            this.vButtonSelectWarrant.UseVisualStyleBackColor = false;
            this.vButtonSelectWarrant.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // vButtonExcludePaste
            // 
            this.vButtonExcludePaste.AllowAnimations = true;
            this.vButtonExcludePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonExcludePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonExcludePaste.Location = new System.Drawing.Point(455, 6);
            this.vButtonExcludePaste.Name = "vButton2";
            this.vButtonExcludePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonExcludePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonExcludePaste.TabIndex = 1;
            this.vButtonExcludePaste.Text = "→";
            this.vButtonExcludePaste.UseVisualStyleBackColor = false;
            this.vButtonExcludePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            this.vButtonExcludePaste.Click += new EventHandler(vButtonExcludePaste_Click);
            // 
            // vButtonIncludePaste
            // 
            this.vButtonIncludePaste.AllowAnimations = true;
            this.vButtonIncludePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonIncludePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonIncludePaste.Location = new System.Drawing.Point(455, 6);
            this.vButtonIncludePaste.Name = "vButton3";
            this.vButtonIncludePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonIncludePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonIncludePaste.TabIndex = 4;
            this.vButtonIncludePaste.Text = "→";
            this.vButtonIncludePaste.UseVisualStyleBackColor = false;
            this.vButtonIncludePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            this.vButtonIncludePaste.Click += new EventHandler(vButtonIncludePaste_Click);
            // 
            // vButtonExplainPaste
            // 
            this.vButtonExplainPaste.AllowAnimations = true;
            this.vButtonExplainPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonExplainPaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonExplainPaste.Location = new System.Drawing.Point(455, 6);
            this.vButtonExplainPaste.Name = "vButton4";
            this.vButtonExplainPaste.RoundedCornersMask = ((byte)(15));
            this.vButtonExplainPaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonExplainPaste.TabIndex = 2;
            this.vButtonExplainPaste.Text = "→";
            this.vButtonExplainPaste.UseVisualStyleBackColor = false;
            this.vButtonExplainPaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            this.vButtonExplainPaste.Click += new EventHandler(vButtonExplainPaste_Click);
            // 
            // vButtonTerminatePaste
            // 
            this.vButtonTerminatePaste.AllowAnimations = true;
            this.vButtonTerminatePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonTerminatePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonTerminatePaste.Location = new System.Drawing.Point(455, 6);
            this.vButtonTerminatePaste.Name = "vButton5";
            this.vButtonTerminatePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonTerminatePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonTerminatePaste.TabIndex = 1;
            this.vButtonTerminatePaste.Text = "→";
            this.vButtonTerminatePaste.UseVisualStyleBackColor = false;
            this.vButtonTerminatePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            this.vButtonTerminatePaste.Click += new EventHandler(vButtonTerminatePaste_Click);
            // 
            // dateTimePickerAgreementDate
            // 
            this.dateTimePickerAgreementDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAgreementDate.Location = new System.Drawing.Point(164, 19);
            this.dateTimePickerAgreementDate.Name = "dateTimePickerAgreementDate";
            this.dateTimePickerAgreementDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerAgreementDate.TabIndex = 0;
            // 
            // dateTimePickerIncludeDateOfBirth
            // 
            this.dateTimePickerIncludeDateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIncludeDateOfBirth.Location = new System.Drawing.Point(163, 61);
            this.dateTimePickerIncludeDateOfBirth.Name = "dateTimePickerIncludeDateOfBirth";
            this.dateTimePickerIncludeDateOfBirth.Size = new System.Drawing.Size(285, 20);
            this.dateTimePickerIncludeDateOfBirth.TabIndex = 2;

            this.tableLayoutPanel12.ResumeLayout(false);
            this.groupBox30.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.groupBox29.ResumeLayout(false);
            this.groupBox29.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tabPageExclude.ResumeLayout(false);
            this.tabPageExclude.PerformLayout();
            this.tabPageInclude.ResumeLayout(false);
            this.tabPageInclude.PerformLayout();
            this.tabPageExplain.ResumeLayout(false);
            this.tabPageExplain.PerformLayout();
            this.tabPageTerminate.ResumeLayout(false);
            this.tabPageTerminate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).EndInit();
        }

        void vButtonTerminatePaste_Click(object sender, EventArgs e)
        {
            if (textBoxTerminateAgreement.Text.Trim() == "")
            {
                MessageBox.Show("Не указан номер соглашения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxExplainPoint.Focus();
                return;
            }
            List<string> contentList = textBoxAgreementContent.Lines.ToList();
            int header_index = -1;
            int last_point_index = -1;
            for (int i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200Bрасторгнуть"))
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

            string element = String.Format("№ соглашения {0}.", textBoxTerminateAgreement.Text);
            if (header_index == -1)
            {
                contentList.Add("\u200Bрасторгнуть:");
            }
            if (last_point_index == -1)
                contentList.Add(element);
            else
                contentList.Insert(last_point_index, element);
            textBoxAgreementContent.Lines = contentList.ToArray();
        }

        void vButtonExplainPaste_Click(object sender, EventArgs e)
        {
            if (textBoxExplainPoint.Text.Trim() == "")
            {
                MessageBox.Show("Не указан номер подпункта", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxExplainPoint.Focus();
                return;
            }
            if (textBoxExplainContent.Text.Trim() == "")
            {
                MessageBox.Show("Содержание изложения не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            string element = String.Format("подпункт {0}. {1}", textBoxExplainPoint.Text, textBoxExplainContent.Text.Trim());
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
            if (textBoxIncludePoint.Text.Trim() == "")
            {
                MessageBox.Show("Не указан номер подпункта", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxIncludePoint.Focus();
                return;
            }
            if (textBoxIncludeSNP.Text.Trim() == "")
            {
                MessageBox.Show("Поле ФИО не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxIncludeSNP.Focus();
                return;
            }
            if (comboBoxIncludeKinship.SelectedValue == null)
            {
                MessageBox.Show("Не выбрана родственная связь", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            string element = String.Format("подпункт {0}. {1} - {2}, {3} г.р.", textBoxIncludePoint.Text,
                textBoxIncludeSNP.Text.Trim(),
                kinship,
                dateTimePickerIncludeDateOfBirth.Value);
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
            if (textBoxExcludePoint.Text.Trim() == "")
            {
                MessageBox.Show("Не указан номер подпункта", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (v_persons.Position == -1)
            {
                MessageBox.Show("Не выбран участник найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                } else
                    if (header_index != -1 && Regex.IsMatch(contentList[i], "^(\u200Bисключить|\u200Bвключить|\u200Bизложить|\u200Bрасторгнуть)"))
                {
                    last_point_index = i;
                    break;
                }
            }
            DataRowView person = ((DataRowView)v_persons[v_persons.Position]);

            string kinship = person["id_kinship"] != DBNull.Value ?
                ((DataRowView)v_kinships[v_kinships.Find("id_kinship", person["id_kinship"])])["kinship"].ToString() : "";
            string element = String.Format("подпункт {0}. {1} {2} {3} - {4}, {5} г.р.", textBoxExcludePoint.Text,
                person["surname"].ToString(),
                person["name"].ToString(),
                person["patronymic"].ToString(),
                kinship,
                person["date_of_birth"] != DBNull.Value ? Convert.ToDateTime(person["date_of_birth"]).ToString("dd.MM.yyyy") : "");
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
    }
}
