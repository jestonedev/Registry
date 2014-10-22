using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using Registry.DataModels;
using System.Drawing;
using Registry.CalcDataModels;

namespace Registry.Viewport
{
    internal sealed class PersonsViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel11 = new TableLayoutPanel();
        private GroupBox groupBox23 = new GroupBox();
        private GroupBox groupBox26 = new GroupBox();
        private GroupBox groupBox27 = new GroupBox();
        private GroupBox groupBox28 = new GroupBox();
        private DataGridView dataGridViewPersons = new DataGridView();
        private DataGridViewTextBoxColumn field_surname = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_name = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_patronymic = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_date_of_birth = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_kinship = new DataGridViewComboBoxColumn();
        private Label label53 = new Label();
        private Label label54 = new Label();
        private Label label55 = new Label();
        private Label label56 = new Label();
        private Label label57 = new Label();
        private Label label58 = new Label();
        private Label label59 = new Label();
        private Label label60 = new Label();
        private Label label61 = new Label();
        private Label label62 = new Label();
        private Label label63 = new Label();
        private Label label64 = new Label();
        private Label label65 = new Label();
        private Label label66 = new Label();
        private Label label67 = new Label();
        private Label label68 = new Label();
        private Label label69 = new Label();
        private Label label70 = new Label();
        private Label label81 = new Label();
        private TextBox textBoxSurname = new TextBox();
        private TextBox textBoxName = new TextBox();
        private TextBox textBoxPatronymic = new TextBox();
        private TextBox textBoxDocumentSeria = new TextBox();
        private TextBox textBoxDocumentNumber = new TextBox();
        private TextBox textBoxRegistrationHouse = new TextBox();
        private TextBox textBoxRegistrationFlat = new TextBox();
        private TextBox textBoxRegistrationRoom = new TextBox();
        private TextBox textBoxResidenceRoom = new TextBox();
        private TextBox textBoxResidenceFlat = new TextBox();
        private TextBox textBoxResidenceHouse = new TextBox();
        private TextBox textBoxPersonalAccount = new TextBox();
        private ComboBox comboBoxKinship = new ComboBox();
        private ComboBox comboBoxDocumentType = new ComboBox();
        private ComboBox comboBoxIssuedBy = new ComboBox();
        private ComboBox comboBoxRegistrationStreet = new ComboBox();
        private ComboBox comboBoxResidenceStreet = new ComboBox();
        private DateTimePicker dateTimePickerDateOfBirth = new DateTimePicker();
        private DateTimePicker dateTimePickerDateOfDocumentIssue = new DateTimePicker();
        #endregion Components

        //Modeles
        PersonsDataModel persons = null;
        KinshipsDataModel kinships = null;
        DocumentTypesDataModel document_types = null;
        DocumentsIssuedByDataModel document_issued_by = null;
        KladrStreetsDataModel kladr = null;

        //Views
        BindingSource v_persons = null;
        BindingSource v_kinships = null;
        BindingSource v_document_types = null;
        BindingSource v_document_issued_by = null;
        BindingSource v_registration_street = null;
        BindingSource v_residence_street = null;

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;

        public PersonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPagePersons";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Участники найма №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public PersonsViewport(PersonsViewport personsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = personsViewport.DynamicFilter;
            this.StaticFilter = personsViewport.StaticFilter;
            this.ParentRow = personsViewport.ParentRow;
            this.ParentType = personsViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            persons = PersonsDataModel.GetInstance();
            kinships = KinshipsDataModel.GetInstance();
            document_types = DocumentTypesDataModel.GetInstance();
            document_issued_by = DocumentsIssuedByDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            persons.Select();
            kinships.Select();
            document_types.Select();
            document_issued_by.Select();
            kladr.Select();

            DataSet ds = DataSetManager.GetDataSet();

            if ((ParentType == ParentTypeEnum.Tenancy) && (ParentRow != null))
                this.Text = String.Format("Участники найма №{0}", ParentRow["id_contract"].ToString());
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_kinships = new BindingSource();
            v_kinships.DataMember = "kinships";
            v_kinships.DataSource = ds;

            v_registration_street = new BindingSource();
            v_registration_street.DataMember = "kladr";
            v_registration_street.DataSource = ds;

            v_residence_street = new BindingSource();
            v_residence_street.DataMember = "kladr";
            v_residence_street.DataSource = ds;

            v_document_types = new BindingSource();
            v_document_types.DataMember = "document_types";
            v_document_types.DataSource = ds;

            v_document_issued_by = new BindingSource();
            v_document_issued_by.DataMember = "documents_issued_by";
            v_document_issued_by.DataSource = ds;

            v_persons = new BindingSource();
            v_persons.CurrentItemChanged += new EventHandler(v_persons_CurrentItemChanged);
            v_persons.DataMember = "persons";
            v_persons.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_persons.Filter += " AND ";
            v_persons.Filter += DynamicFilter;
            v_persons.DataSource = ds;

            DataBind();

            textBoxSurname.TextChanged += new EventHandler(textBoxSurname_TextChanged);
            textBoxName.TextChanged += new EventHandler(textBoxName_TextChanged);
            textBoxPatronymic.TextChanged += new EventHandler(textBoxPatronymic_TextChanged);
            dateTimePickerDateOfBirth.ValueChanged += new EventHandler(dateTimePickerDateOfBirth_ValueChanged);
            comboBoxKinship.SelectedValueChanged += new EventHandler(comboBoxKinship_SelectedValueChanged);
            textBoxPersonalAccount.TextChanged += new EventHandler(textBoxPersonalAccount_TextChanged);
            comboBoxDocumentType.SelectedValueChanged += new EventHandler(comboBoxDocumentType_SelectedValueChanged);
            textBoxDocumentSeria.TextChanged += new EventHandler(textBoxDocumentSeria_TextChanged);
            textBoxDocumentNumber.TextChanged += new EventHandler(textBoxDocumentNumber_TextChanged);
            dateTimePickerDateOfDocumentIssue.ValueChanged += new EventHandler(dateTimePickerDateOfDocumentIssue_ValueChanged);
            comboBoxIssuedBy.SelectedValueChanged += new EventHandler(comboBoxIssuedBy_SelectedValueChanged);
            comboBoxRegistrationStreet.SelectedValueChanged += new EventHandler(comboBoxRegistrationStreet_SelectedValueChanged);
            textBoxRegistrationHouse.TextChanged += new EventHandler(textBoxRegistrationHouse_TextChanged);
            textBoxRegistrationFlat.TextChanged += new EventHandler(textBoxRegistrationFlat_TextChanged);
            textBoxRegistrationRoom.TextChanged += new EventHandler(textBoxRegistrationRoom_TextChanged);
            comboBoxResidenceStreet.SelectedValueChanged += new EventHandler(comboBoxResidenceStreet_SelectedValueChanged);
            textBoxResidenceHouse.TextChanged += new EventHandler(textBoxResidenceHouse_TextChanged);
            textBoxResidenceFlat.TextChanged += new EventHandler(textBoxResidenceFlat_TextChanged);
            textBoxResidenceRoom.TextChanged += new EventHandler(textBoxResidenceRoom_TextChanged);
            dataGridViewPersons.DataError += new DataGridViewDataErrorEventHandler(dataGridViewPersons_DataError);
            comboBoxRegistrationStreet.Leave += new EventHandler(comboBoxRegistrationStreet_Leave);
            comboBoxRegistrationStreet.KeyUp += new KeyEventHandler(comboBoxRegistrationStreet_KeyUp);
            comboBoxRegistrationStreet.DropDownClosed += new EventHandler(comboBoxRegistrationStreet_DropDownClosed);
            comboBoxResidenceStreet.Leave += new EventHandler(comboBoxResidenceStreet_Leave);
            comboBoxResidenceStreet.KeyUp += new KeyEventHandler(comboBoxResidenceStreet_KeyUp);
            comboBoxResidenceStreet.DropDownClosed += new EventHandler(comboBoxResidenceStreet_DropDownClosed);
            comboBoxIssuedBy.Leave += new EventHandler(comboBoxIssuedBy_Leave);
            comboBoxIssuedBy.KeyUp += new KeyEventHandler(comboBoxIssuedBy_KeyUp);
            comboBoxIssuedBy.DropDownClosed += new EventHandler(comboBoxIssuedBy_DropDownClosed);
            persons.Select().RowDeleted += new DataRowChangeEventHandler(PersonsViewport_RowDeleted);
            persons.Select().RowChanged += new DataRowChangeEventHandler(PersonsViewport_RowChanged);
        }

        void PersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            RedrawDataGridRows();
        }

        void PersonsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                UnbindedCheckBoxesUpdate();
                RedrawDataGridRows();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            base.OnVisibleChanged(e);
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

        void comboBoxIssuedBy_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxIssuedBy.Items.Count == 0)
                comboBoxIssuedBy.SelectedIndex = -1;
        }

        void comboBoxIssuedBy_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back))
            {
                string text = comboBoxIssuedBy.Text;
                int selectionStart = comboBoxIssuedBy.SelectionStart;
                int selectionLength = comboBoxIssuedBy.SelectionLength;
                v_document_issued_by.Filter = "document_issued_by like '%" + comboBoxIssuedBy.Text + "%'";
                comboBoxIssuedBy.Text = text;
                comboBoxIssuedBy.SelectionStart = selectionStart;
                comboBoxIssuedBy.SelectionLength = selectionLength;
            }
        }

        void comboBoxIssuedBy_Leave(object sender, EventArgs e)
        {
            if (comboBoxIssuedBy.Text == "")
            {
                comboBoxIssuedBy.SelectedValue = DBNull.Value;
                return;
            }
            if (comboBoxIssuedBy.Items.Count > 0)
            {
                if (comboBoxIssuedBy.SelectedValue == null)
                    comboBoxIssuedBy.SelectedValue = v_document_issued_by[v_document_issued_by.Position];
                comboBoxIssuedBy.Text = ((DataRowView)v_document_issued_by[v_document_issued_by.Position])["document_issued_by"].ToString();
            }
            if (comboBoxIssuedBy.SelectedValue == null)
                comboBoxIssuedBy.Text = "";
        }

        void comboBoxResidenceStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxResidenceStreet.Items.Count == 0)
                comboBoxResidenceStreet.SelectedIndex = -1;
        }

        void comboBoxResidenceStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back))
            {
                string text = comboBoxResidenceStreet.Text;
                int selectionStart = comboBoxResidenceStreet.SelectionStart;
                int selectionLength = comboBoxResidenceStreet.SelectionLength;
                v_residence_street.Filter = "street_name like '%" + comboBoxResidenceStreet.Text + "%'";
                comboBoxResidenceStreet.Text = text;
                comboBoxResidenceStreet.SelectionStart = selectionStart;
                comboBoxResidenceStreet.SelectionLength = selectionLength;
            }
        }

        void comboBoxResidenceStreet_Leave(object sender, EventArgs e)
        {
            if (comboBoxResidenceStreet.Text == "")
            {
                comboBoxResidenceStreet.SelectedValue = DBNull.Value;
                return;
            }
            if (comboBoxResidenceStreet.Items.Count > 0)
            {
                if (comboBoxResidenceStreet.SelectedValue == null)
                    comboBoxResidenceStreet.SelectedValue = v_residence_street[v_residence_street.Position];
                comboBoxResidenceStreet.Text = ((DataRowView)v_residence_street[v_residence_street.Position])["street_name"].ToString();
            }
            if (comboBoxResidenceStreet.SelectedValue == null)
                comboBoxResidenceStreet.Text = "";
        }

        void comboBoxRegistrationStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxRegistrationStreet.Items.Count == 0)
                comboBoxRegistrationStreet.SelectedIndex = -1;
        }

        void comboBoxRegistrationStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back))
            {
                string text = comboBoxRegistrationStreet.Text;
                int selectionStart = comboBoxRegistrationStreet.SelectionStart;
                int selectionLength = comboBoxRegistrationStreet.SelectionLength;
                v_registration_street.Filter = "street_name like '%" + comboBoxRegistrationStreet.Text + "%'";
                comboBoxRegistrationStreet.Text = text;
                comboBoxRegistrationStreet.SelectionStart = selectionStart;
                comboBoxRegistrationStreet.SelectionLength = selectionLength;
            }
        }

        void comboBoxRegistrationStreet_Leave(object sender, EventArgs e)
        {
            if (comboBoxRegistrationStreet.Text == "")
            {
                comboBoxRegistrationStreet.SelectedValue = DBNull.Value;
                return;
            }
            if (comboBoxRegistrationStreet.Items.Count > 0)
            {
                if (comboBoxRegistrationStreet.SelectedValue == null)
                    comboBoxRegistrationStreet.SelectedValue = v_registration_street[v_registration_street.Position];
                comboBoxRegistrationStreet.Text = ((DataRowView)v_registration_street[v_registration_street.Position])["street_name"].ToString();
            }
            if (comboBoxRegistrationStreet.SelectedValue == null)
                comboBoxRegistrationStreet.Text = "";
        }

        void dataGridViewPersons_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        void textBoxResidenceRoom_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxResidenceFlat_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxResidenceHouse_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxResidenceStreet_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxRegistrationRoom_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxRegistrationFlat_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxRegistrationHouse_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxRegistrationStreet_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxIssuedBy_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerDateOfDocumentIssue_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxDocumentNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxDocumentSeria_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxDocumentType_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxPersonalAccount_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxKinship_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerDateOfBirth_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxPatronymic_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxName_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxSurname_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void DataBind()
        {
            comboBoxKinship.DataSource = v_kinships;
            comboBoxKinship.ValueMember = "id_kinship";
            comboBoxKinship.DisplayMember = "kinship";
            comboBoxKinship.DataBindings.Clear();
            comboBoxKinship.DataBindings.Add("SelectedValue", v_persons, "id_kinship", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxDocumentType.DataSource = v_document_types;
            comboBoxDocumentType.ValueMember = "id_document_type";
            comboBoxDocumentType.DisplayMember = "document_type";
            comboBoxDocumentType.DataBindings.Clear();
            comboBoxDocumentType.DataBindings.Add("SelectedValue", v_persons, "id_document_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxIssuedBy.DataSource = v_document_issued_by;
            comboBoxIssuedBy.ValueMember = "id_document_issued_by";
            comboBoxIssuedBy.DisplayMember = "document_issued_by";
            comboBoxIssuedBy.DataBindings.Clear();
            comboBoxIssuedBy.DataBindings.Add("SelectedValue", v_persons, "id_document_issued_by", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxRegistrationStreet.DataSource = v_registration_street;
            comboBoxRegistrationStreet.ValueMember = "id_street";
            comboBoxRegistrationStreet.DisplayMember = "street_name";
            comboBoxRegistrationStreet.DataBindings.Clear();
            comboBoxRegistrationStreet.DataBindings.Add("SelectedValue", v_persons, "registration_id_street", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxResidenceStreet.DataSource = v_residence_street;
            comboBoxResidenceStreet.ValueMember = "id_street";
            comboBoxResidenceStreet.DisplayMember = "street_name";
            comboBoxResidenceStreet.DataBindings.Clear();
            comboBoxResidenceStreet.DataBindings.Add("SelectedValue", v_persons, "residence_id_street", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxSurname.DataBindings.Clear();
            textBoxSurname.DataBindings.Add("Text", v_persons, "surname", true, DataSourceUpdateMode.Never, "");
            textBoxName.DataBindings.Clear();
            textBoxName.DataBindings.Add("Text", v_persons, "name", true, DataSourceUpdateMode.Never, "");
            textBoxPatronymic.DataBindings.Clear();
            textBoxPatronymic.DataBindings.Add("Text", v_persons, "patronymic", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDateOfBirth.DataBindings.Clear();
            dateTimePickerDateOfBirth.DataBindings.Add("Value", v_persons, "date_of_birth", true, DataSourceUpdateMode.Never, DateTime.Now);
            textBoxPersonalAccount.DataBindings.Clear();
            textBoxPersonalAccount.DataBindings.Add("Text", v_persons, "personal_account", true, DataSourceUpdateMode.Never, "");
            textBoxDocumentSeria.DataBindings.Clear();
            textBoxDocumentSeria.DataBindings.Add("Text", v_persons, "document_seria", true, DataSourceUpdateMode.Never, "");
            textBoxDocumentNumber.DataBindings.Clear();
            textBoxDocumentNumber.DataBindings.Add("Text", v_persons, "document_num", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDateOfDocumentIssue.DataBindings.Clear();
            dateTimePickerDateOfDocumentIssue.DataBindings.Add("Value", v_persons, "date_of_document_issue", true, DataSourceUpdateMode.Never, DateTime.Now);
            textBoxRegistrationHouse.DataBindings.Clear();
            textBoxRegistrationHouse.DataBindings.Add("Text", v_persons, "registration_house", true, DataSourceUpdateMode.Never, "");
            textBoxRegistrationFlat.DataBindings.Clear();
            textBoxRegistrationFlat.DataBindings.Add("Text", v_persons, "registration_flat", true, DataSourceUpdateMode.Never, "");
            textBoxRegistrationRoom.DataBindings.Clear();
            textBoxRegistrationRoom.DataBindings.Add("Text", v_persons, "registration_room", true, DataSourceUpdateMode.Never, "");
            textBoxResidenceHouse.DataBindings.Clear();
            textBoxResidenceHouse.DataBindings.Add("Text", v_persons, "residence_house", true, DataSourceUpdateMode.Never, "");
            textBoxResidenceFlat.DataBindings.Clear();
            textBoxResidenceFlat.DataBindings.Add("Text", v_persons, "residence_flat", true, DataSourceUpdateMode.Never, "");
            textBoxResidenceRoom.DataBindings.Clear();
            textBoxResidenceRoom.DataBindings.Add("Text", v_persons, "residence_room", true, DataSourceUpdateMode.Never, "");

            dataGridViewPersons.DataSource = v_persons;
            field_surname.DataPropertyName = "surname";
            field_name.DataPropertyName = "name";
            field_patronymic.DataPropertyName = "patronymic";
            field_date_of_birth.DataPropertyName = "date_of_birth";
            field_id_kinship.DataSource = v_kinships;
            field_id_kinship.DisplayMember = "kinship";
            field_id_kinship.ValueMember = "id_kinship";
            field_id_kinship.DataPropertyName = "id_kinship";
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!this.ContainsFocus) || (dataGridViewPersons.Focused))
                return;
            if ((v_persons.Position != -1) && (PersonFromView() != PersonFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    dataGridViewPersons.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    dataGridViewPersons.Enabled = true;
                }
            }
            menuCallback.EditingStateUpdate();
        }

        public override void Close()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            persons.Select().RowDeleted -= new DataRowChangeEventHandler(PersonsViewport_RowDeleted);
            persons.Select().RowChanged -= new DataRowChangeEventHandler(PersonsViewport_RowChanged);
            base.Close();
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanCopyRecord()
        {
            return ((v_persons.Position != -1) && (!persons.EditingNewRecord));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            Person person = PersonFromView();
            DataRowView row = (DataRowView)v_persons.AddNew();
            dataGridViewPersons.Enabled = false;
            persons.EditingNewRecord = true;
            ViewportFromPerson(person);
        }

        private void ViewportFromPerson(Person person)
        {
            if (person.id_kinship != null)
                comboBoxKinship.SelectedValue = person.id_kinship;
            else
                comboBoxKinship.SelectedValue = DBNull.Value;
            if (person.id_document_type != null)
                comboBoxDocumentType.SelectedValue = person.id_document_type;
            else
                comboBoxDocumentType.SelectedValue = DBNull.Value;
            if (person.id_document_issued_by != null)
                comboBoxIssuedBy.SelectedValue = person.id_document_issued_by;
            else
                comboBoxIssuedBy.SelectedValue = DBNull.Value;
            if (person.registration_id_street != null)
                comboBoxRegistrationStreet.SelectedValue = person.registration_id_street;
            else
                comboBoxRegistrationStreet.SelectedValue = DBNull.Value;
            if (person.residence_id_street != null)
                comboBoxResidenceStreet.SelectedValue = person.residence_id_street;
            else
                comboBoxResidenceStreet.SelectedValue = DBNull.Value;
            textBoxSurname.Text = person.surname;
            textBoxName.Text = person.name;
            textBoxPatronymic.Text = person.patronymic;
            textBoxPersonalAccount.Text = person.personal_account;
            textBoxDocumentNumber.Text = person.document_num;
            textBoxDocumentSeria.Text = person.document_seria;
            textBoxRegistrationHouse.Text = person.registration_house;
            textBoxRegistrationFlat.Text = person.registration_flat;
            textBoxRegistrationRoom.Text = person.registration_room;
            textBoxResidenceHouse.Text = person.residence_house;
            textBoxResidenceFlat.Text = person.residence_flat;
            textBoxResidenceRoom.Text = person.residence_room;

            if (person.date_of_birth != null)
            {
                dateTimePickerDateOfBirth.Value = person.date_of_birth.Value;
                dateTimePickerDateOfBirth.Checked = true;
            }
            else
            {
                dateTimePickerDateOfBirth.Value = DateTime.Now;
                dateTimePickerDateOfBirth.Checked = false;
            }
            if (person.date_of_document_issue != null)
            {
                dateTimePickerDateOfDocumentIssue.Value = person.date_of_document_issue.Value;
                dateTimePickerDateOfDocumentIssue.Checked = true;
            }
            else
            {
                dateTimePickerDateOfDocumentIssue.Value = DateTime.Now;
                dateTimePickerDateOfDocumentIssue.Checked = false;
            }
        }

        private Person PersonFromViewport()
        {
            Person person = new Person();
            if ((v_persons.Position == -1) || ((DataRowView)v_persons[v_persons.Position])["id_person"] is DBNull)
                person.id_person = null;
            else
                person.id_person = Convert.ToInt32(((DataRowView)v_persons[v_persons.Position])["id_person"]);
            if (ParentType == ParentTypeEnum.Tenancy && ParentRow != null)
                person.id_contract = Convert.ToInt32(ParentRow["id_contract"]);
            else
                person.id_contract = null;
            if (comboBoxKinship.SelectedValue == null)
                person.id_kinship = null;
            else
                person.id_kinship = Convert.ToInt32(comboBoxKinship.SelectedValue);
            if (comboBoxDocumentType.SelectedValue == null)
                person.id_document_type = null;
            else
                person.id_document_type = Convert.ToInt32(comboBoxDocumentType.SelectedValue);
            if (comboBoxIssuedBy.SelectedValue == null)
                person.id_document_issued_by = null;
            else
                person.id_document_issued_by = Convert.ToInt32(comboBoxIssuedBy.SelectedValue);
            if (comboBoxRegistrationStreet.SelectedValue == null)
                person.registration_id_street = null;
            else
                person.registration_id_street = comboBoxRegistrationStreet.SelectedValue.ToString();
            if (comboBoxResidenceStreet.SelectedValue == null)
                person.residence_id_street = null;
            else
                person.residence_id_street = comboBoxResidenceStreet.SelectedValue.ToString();
            if (textBoxSurname.Text.Trim() != "")
                person.surname = textBoxSurname.Text.Trim();
            else
                person.surname = null;
            if (textBoxName.Text.Trim() != "")
                person.name = textBoxName.Text.Trim();
            else
                person.name = null;
            if (textBoxPatronymic.Text.Trim() != "")
                person.patronymic = textBoxPatronymic.Text.Trim();
            else
                person.patronymic = null;
            if (textBoxPersonalAccount.Text.Trim() != "")
                person.personal_account = textBoxPersonalAccount.Text.Trim();
            else
                person.personal_account = null;
            if (textBoxDocumentSeria.Text.Trim() != "")
                person.document_seria = textBoxDocumentSeria.Text.Trim();
            else
                person.document_seria = null;
            if (textBoxDocumentNumber.Text.Trim() != "")
                person.document_num = textBoxDocumentNumber.Text.Trim();
            else
                person.document_num = null;
            if (textBoxRegistrationHouse.Text.Trim() != "")
                person.registration_house = textBoxRegistrationHouse.Text.Trim();
            else
                person.registration_house = null;
            if (textBoxRegistrationFlat.Text.Trim() != "")
                person.registration_flat = textBoxRegistrationFlat.Text.Trim();
            else
                person.registration_flat = null;
            if (textBoxRegistrationRoom.Text.Trim() != "")
                person.registration_room = textBoxRegistrationRoom.Text.Trim();
            else
                person.registration_room = null;
            if (textBoxResidenceHouse.Text.Trim() != "")
                person.residence_house = textBoxResidenceHouse.Text.Trim();
            else
                person.residence_house = null;
            if (textBoxResidenceFlat.Text.Trim() != "")
                person.residence_flat = textBoxResidenceFlat.Text.Trim();
            else
                person.residence_flat = null;
            if (textBoxResidenceRoom.Text.Trim() != "")
                person.residence_room = textBoxResidenceRoom.Text.Trim();
            else
                person.residence_room = null;
            if (dateTimePickerDateOfBirth.Checked)
                person.date_of_birth = dateTimePickerDateOfBirth.Value.Date;
            else
                person.date_of_birth = null;
            if (dateTimePickerDateOfDocumentIssue.Checked)
                person.date_of_document_issue = dateTimePickerDateOfDocumentIssue.Value.Date;
            else
                person.date_of_document_issue = null;
            return person;
        }

        private Person PersonFromView()
        {
            Person person = new Person();
            DataRowView row = (DataRowView)v_persons[v_persons.Position];
            if (row["id_person"] is DBNull)
                person.id_person = null;
            else
                person.id_person = Convert.ToInt32(row["id_person"]);
            if (row["id_contract"] is DBNull)
                person.id_contract = null;
            else
                person.id_contract = Convert.ToInt32(row["id_contract"]);
            if (row["id_kinship"] is DBNull)
                person.id_kinship = null;
            else
                person.id_kinship = Convert.ToInt32(row["id_kinship"]);
            if (row["surname"] is DBNull)
                person.surname = null;
            else
                person.surname = row["surname"].ToString();
            if (row["name"] is DBNull)
                person.name = null;
            else
                person.name = row["name"].ToString();
            if (row["patronymic"] is DBNull)
                person.patronymic = null;
            else
                person.patronymic = row["patronymic"].ToString();
            if (row["date_of_birth"] is DBNull)
                person.date_of_birth = null;
            else
                person.date_of_birth = Convert.ToDateTime(row["date_of_birth"]);
            if (row["id_document_type"] is DBNull)
                person.id_document_type = null;
            else
                person.id_document_type = Convert.ToInt32(row["id_document_type"]);
            if (row["date_of_document_issue"] is DBNull)
                person.date_of_document_issue = null;
            else
                person.date_of_document_issue = Convert.ToDateTime(row["date_of_document_issue"]);
            if (row["document_num"] is DBNull)
                person.document_num = null;
            else
                person.document_num = row["document_num"].ToString();
            if (row["document_seria"] is DBNull)
                person.document_seria = null;
            else
                person.document_seria = row["document_seria"].ToString();
            if (row["id_document_issued_by"] is DBNull)
                person.id_document_issued_by = null;
            else
                person.id_document_issued_by = Convert.ToInt32(row["id_document_issued_by"]);
            if (row["registration_id_street"] is DBNull)
                person.registration_id_street = null;
            else
                person.registration_id_street = row["registration_id_street"].ToString();
            if (row["registration_house"] is DBNull)
                person.registration_house = null;
            else
                person.registration_house = row["registration_house"].ToString();
            if (row["registration_flat"] is DBNull)
                person.registration_flat = null;
            else
                person.registration_flat = row["registration_flat"].ToString();
            if (row["registration_room"] is DBNull)
                person.registration_room = null;
            else
                person.registration_room = row["registration_room"].ToString();
            if (row["residence_id_street"] is DBNull)
                person.residence_id_street = null;
            else
                person.residence_id_street = row["residence_id_street"].ToString();
            if (row["residence_house"] is DBNull)
                person.residence_house = null;
            else
                person.residence_house = row["residence_house"].ToString();
            if (row["residence_flat"] is DBNull)
                person.residence_flat = null;
            else
                person.residence_flat = row["residence_flat"].ToString();
            if (row["residence_room"] is DBNull)
                person.residence_room = null;
            else
                person.residence_room = row["residence_room"].ToString();
            if (row["personal_account"] is DBNull)
                person.personal_account = null;
            else
                person.personal_account = row["personal_account"].ToString();
            return person;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_persons.MoveFirst();
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_persons.MoveLast();
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_persons.MoveNext();
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_persons.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_persons.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_persons.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_persons.Position > -1) && (v_persons.Position < (v_persons.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_persons.Position > -1) && (v_persons.Position < (v_persons.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            if ((viewportState == ViewportState.ReadState || viewportState == ViewportState.ModifyRowState) && !persons.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            DataRowView row = (DataRowView)v_persons.AddNew();
            dataGridViewPersons.Enabled = false;
            persons.EditingNewRecord = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите этого участника договора?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (persons.Delete((int)((DataRowView)v_persons.Current)["id_person"]) == -1)
                    return;
                ((DataRowView)v_persons[v_persons.Position]).Delete();
                RedrawDataGridRows();
                menuCallback.ForceCloseDetachedViewports(); 
                if (ParentType == ParentTypeEnum.Tenancy)
                    CalcDataModeTenancyAggregated.GetInstance().Refresh(CalcDataModelFilterEnity.Tenancy, (int)ParentRow["id_contract"]);
            }
        }

        public override bool CanDeleteRecord()
        {
            if ((v_persons.Position == -1) || (viewportState == ViewportState.NewRowState))
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
            PersonsViewport viewport = new PersonsViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_persons.Count > 0)
                viewport.LocatePersonBy((((DataRowView)v_persons[v_persons.Position])["id_person"] as Int32?) ?? -1);
            return viewport;
        }

        private void LocatePersonBy(int id)
        {
            int Position = v_persons.Find("id_person", id);
            if (Position > 0)
                v_persons.Position = Position;
        }

        void v_persons_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
            dataGridViewPersons.Enabled = true;
            UnbindedCheckBoxesUpdate();
            if (v_persons.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        public override int GetRecordCount()
        {
            return v_persons.Count;
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        private bool ValidatePerson(Person person)
        {
            if (person.surname == null)
            {
                MessageBox.Show("Необходимо указать фамилию участника найма", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxSurname.Focus();
                return false;
            }
            if (person.name == null)
            {
                MessageBox.Show("Необходимо указать имя участника найма", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return false;
            }
            if (person.id_kinship == null)
            {
                MessageBox.Show("Необходимо выбрать родственную связь", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxKinship.Focus();
                return false;
            }
            if (person.id_kinship == 1)
            {
                for (int i = 0; i < v_persons.Count; i++)
                {
                    if (((DataRowView)v_persons[i])["id_kinship"] != DBNull.Value &&
                        (Convert.ToInt32(((DataRowView)v_persons[i])["id_kinship"]) == 1) &&
                        (Convert.ToInt32(((DataRowView)v_persons[i])["id_person"]) != person.id_person))
                    {
                        MessageBox.Show("В процессе найма может быть только один наниматель", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        comboBoxKinship.Focus();
                        return false;
                    }
                }
            }
            if (person.id_document_type == null)
            {
                MessageBox.Show("Необходимо выбрать вид документа, удостоверяющего личность", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxDocumentType.Focus();
                return false;
            }
            return true;
        }

        public override void SaveRecord()
        {
            Person person = PersonFromViewport();
            if (!ValidatePerson(person))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                case ViewportState.NewRowState:
                    int id_person = persons.Insert(person);
                    if (id_person == -1)
                        return;
                    DataRowView newRow;
                    if (v_persons.Position == -1)
                        newRow = (DataRowView)v_persons.AddNew();
                    else
                        newRow = ((DataRowView)v_persons[v_persons.Position]);
                    person.id_person = id_person;
                    FillRowFromPerson(person, newRow);
                    persons.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (person.id_person == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись об участнике договора без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (persons.Update(person) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_persons[v_persons.Position]);
                    FillRowFromPerson(person, row);
                    break;
            }
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            viewportState = ViewportState.ReadState;
            dataGridViewPersons.Enabled = true;
            is_editable = true;
            if (ParentType == ParentTypeEnum.Tenancy)
                CalcDataModeTenancyAggregated.GetInstance().Refresh(CalcDataModelFilterEnity.Tenancy, (int)ParentRow["id_contract"]);
        }

        private void FillRowFromPerson(Person person, DataRowView row)
        {
            row.BeginEdit();
            row["id_person"] = person.id_person == null ? DBNull.Value : (object)person.id_person;
            row["id_contract"] = person.id_contract == null ? DBNull.Value : (object)person.id_contract;
            row["id_kinship"] = person.id_kinship == null ? DBNull.Value : (object)person.id_kinship;
            row["surname"] = person.surname == null ? DBNull.Value : (object)person.surname;
            row["name"] = person.name == null ? DBNull.Value : (object)person.name;
            row["patronymic"] = person.patronymic == null ? DBNull.Value : (object)person.patronymic;
            row["date_of_birth"] = person.date_of_birth == null ? DBNull.Value : (object)person.date_of_birth;
            row["id_document_type"] = person.id_document_type == null ? DBNull.Value : (object)person.id_document_type;
            row["date_of_document_issue"] = person.date_of_document_issue == null ? DBNull.Value : (object)person.date_of_document_issue;
            row["document_num"] = person.document_num == null ? DBNull.Value : (object)person.document_num;
            row["document_seria"] = person.document_seria == null ? DBNull.Value : (object)person.document_seria;
            row["id_document_issued_by"] = person.id_document_issued_by == null ? DBNull.Value : (object)person.id_document_issued_by;
            row["registration_id_street"] = person.registration_id_street == null ? DBNull.Value : (object)person.registration_id_street;
            row["registration_house"] = person.registration_house == null ? DBNull.Value : (object)person.registration_house;
            row["registration_flat"] = person.registration_flat == null ? DBNull.Value : (object)person.registration_flat;
            row["registration_room"] = person.registration_room == null ? DBNull.Value : (object)person.registration_room;
            row["residence_id_street"] = person.residence_id_street == null ? DBNull.Value : (object)person.residence_id_street;
            row["residence_house"] = person.residence_house == null ? DBNull.Value : (object)person.residence_house;
            row["residence_flat"] = person.residence_flat == null ? DBNull.Value : (object)person.residence_flat;
            row["residence_room"] = person.residence_room == null ? DBNull.Value : (object)person.residence_room;
            row["personal_account"] = person.personal_account == null ? DBNull.Value : (object)person.personal_account;
            row.EndEdit();
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    viewportState = ViewportState.ReadState;
                    persons.EditingNewRecord = false;
                    if (v_persons.Position != -1)
                    {
                        dataGridViewPersons.Enabled = true;
                        ((DataRowView)v_persons[v_persons.Position]).Delete();
                        RedrawDataGridRows();
                    }
                    break;
                case ViewportState.ModifyRowState:
                    dataGridViewPersons.Enabled = true;
                    is_editable = false;
                    DataBind();
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
                            if (persons.EditingNewRecord)
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

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                persons.EditingNewRecord = false;
            persons.Select().RowDeleted -= new DataRowChangeEventHandler(PersonsViewport_RowDeleted);
            persons.Select().RowChanged -= new DataRowChangeEventHandler(PersonsViewport_RowChanged);
            base.Close();
        }

        public override bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        private void UnbindedCheckBoxesUpdate()
        {
            dateTimePickerDateOfBirth.Checked = (v_persons.Position >= 0) &&
                (((DataRowView)v_persons[v_persons.Position])["date_of_birth"] != DBNull.Value);
            dateTimePickerDateOfDocumentIssue.Checked = (v_persons.Position >= 0) &&
                (((DataRowView)v_persons[v_persons.Position])["date_of_document_issue"] != DBNull.Value);
        }

        private void ConstructViewport()
        {
            this.Controls.Add(tableLayoutPanel11);
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            this.tableLayoutPanel11.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.groupBox27.SuspendLayout();
            this.groupBox28.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).BeginInit();
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel11.Controls.Add(this.groupBox23, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.groupBox27, 0, 1);
            this.tableLayoutPanel11.Controls.Add(this.groupBox26, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.groupBox28, 1, 1);
            this.tableLayoutPanel11.Controls.Add(this.dataGridViewPersons, 0, 2);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 3;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.Size = new System.Drawing.Size(990, 665);
            this.tableLayoutPanel11.TabIndex = 0;
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.textBoxPersonalAccount);
            this.groupBox23.Controls.Add(this.label81);
            this.groupBox23.Controls.Add(this.dateTimePickerDateOfBirth);
            this.groupBox23.Controls.Add(this.comboBoxKinship);
            this.groupBox23.Controls.Add(this.label57);
            this.groupBox23.Controls.Add(this.label56);
            this.groupBox23.Controls.Add(this.textBoxPatronymic);
            this.groupBox23.Controls.Add(this.label55);
            this.groupBox23.Controls.Add(this.textBoxName);
            this.groupBox23.Controls.Add(this.label54);
            this.groupBox23.Controls.Add(this.textBoxSurname);
            this.groupBox23.Controls.Add(this.label53);
            this.groupBox23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox23.Location = new System.Drawing.Point(3, 3);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Size = new System.Drawing.Size(489, 194);
            this.groupBox23.TabIndex = 0;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Личные данные";
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.comboBoxIssuedBy);
            this.groupBox26.Controls.Add(this.label62);
            this.groupBox26.Controls.Add(this.dateTimePickerDateOfDocumentIssue);
            this.groupBox26.Controls.Add(this.label61);
            this.groupBox26.Controls.Add(this.textBoxDocumentNumber);
            this.groupBox26.Controls.Add(this.label60);
            this.groupBox26.Controls.Add(this.textBoxDocumentSeria);
            this.groupBox26.Controls.Add(this.label59);
            this.groupBox26.Controls.Add(this.comboBoxDocumentType);
            this.groupBox26.Controls.Add(this.label58);
            this.groupBox26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox26.Location = new System.Drawing.Point(498, 3);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(489, 194);
            this.groupBox26.TabIndex = 2;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Документ, удостоверяющий личность";
            // 
            // groupBox27
            // 
            this.groupBox27.Controls.Add(this.label66);
            this.groupBox27.Controls.Add(this.textBoxRegistrationRoom);
            this.groupBox27.Controls.Add(this.label65);
            this.groupBox27.Controls.Add(this.textBoxRegistrationFlat);
            this.groupBox27.Controls.Add(this.label63);
            this.groupBox27.Controls.Add(this.label64);
            this.groupBox27.Controls.Add(this.comboBoxRegistrationStreet);
            this.groupBox27.Controls.Add(this.textBoxRegistrationHouse);
            this.groupBox27.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox27.Location = new System.Drawing.Point(3, 173);
            this.groupBox27.Name = "groupBox27";
            this.groupBox27.Size = new System.Drawing.Size(489, 134);
            this.groupBox27.TabIndex = 1;
            this.groupBox27.TabStop = false;
            this.groupBox27.Text = "Адрес регистрации";
            // 
            // groupBox28
            // 
            this.groupBox28.Controls.Add(this.label67);
            this.groupBox28.Controls.Add(this.textBoxResidenceRoom);
            this.groupBox28.Controls.Add(this.label68);
            this.groupBox28.Controls.Add(this.textBoxResidenceFlat);
            this.groupBox28.Controls.Add(this.label69);
            this.groupBox28.Controls.Add(this.label70);
            this.groupBox28.Controls.Add(this.comboBoxResidenceStreet);
            this.groupBox28.Controls.Add(this.textBoxResidenceHouse);
            this.groupBox28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox28.Location = new System.Drawing.Point(498, 173);
            this.groupBox28.Name = "groupBox28";
            this.groupBox28.Size = new System.Drawing.Size(489, 134);
            this.groupBox28.TabIndex = 3;
            this.groupBox28.TabStop = false;
            this.groupBox28.Text = "Адрес проживания";
            // 
            // dataGridView13
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
            this.field_date_of_birth,
            this.field_id_kinship});
            this.tableLayoutPanel11.SetColumnSpan(this.dataGridViewPersons, 2);
            this.dataGridViewPersons.Name = "dataGridView13";
            this.dataGridViewPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPersons.TabIndex = 4;
            this.dataGridViewPersons.MultiSelect = false;
            this.dataGridViewPersons.AutoGenerateColumns = false;
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
            this.field_date_of_birth.MinimumWidth = 100;
            this.field_date_of_birth.Name = "date_of_birth";
            this.field_date_of_birth.ReadOnly = true;
            // 
            // field_id_kinship
            // 
            this.field_id_kinship.HeaderText = "Отношение/связь";
            this.field_id_kinship.MinimumWidth = 100;
            this.field_id_kinship.Name = "id_kinship";
            this.field_id_kinship.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(17, 22);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(56, 13);
            this.label53.TabIndex = 20;
            this.label53.Text = "Фамилия";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(17, 51);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(29, 13);
            this.label54.TabIndex = 22;
            this.label54.Text = "Имя";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(17, 80);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(54, 13);
            this.label55.TabIndex = 24;
            this.label55.Text = "Отчество";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(17, 110);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(86, 13);
            this.label56.TabIndex = 26;
            this.label56.Text = "Дата рождения";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(17, 138);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(99, 13);
            this.label57.TabIndex = 28;
            this.label57.Text = "Отношение/связь";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(17, 22);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(83, 13);
            this.label58.TabIndex = 30;
            this.label58.Text = "Вид документа";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(17, 51);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(38, 13);
            this.label59.TabIndex = 32;
            this.label59.Text = "Серия";
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(17, 80);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(41, 13);
            this.label60.TabIndex = 34;
            this.label60.Text = "Номер";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(17, 110);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(73, 13);
            this.label61.TabIndex = 36;
            this.label61.Text = "Дата выдачи";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(17, 138);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(63, 13);
            this.label62.TabIndex = 38;
            this.label62.Text = "Кем выдан";
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(17, 23);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(39, 13);
            this.label63.TabIndex = 12;
            this.label63.Text = "Улица";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(17, 52);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(70, 13);
            this.label64.TabIndex = 13;
            this.label64.Text = "Номер дома";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(17, 81);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(93, 13);
            this.label65.TabIndex = 16;
            this.label65.Text = "Номер квартиры";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(17, 110);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(93, 13);
            this.label66.TabIndex = 18;
            this.label66.Text = "Номер комнаты";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(17, 110);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(89, 13);
            this.label67.TabIndex = 26;
            this.label67.Text = "Номер комнаты";
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(17, 81);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(93, 13);
            this.label68.TabIndex = 24;
            this.label68.Text = "Номер квартиры";
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(17, 23);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(39, 13);
            this.label69.TabIndex = 20;
            this.label69.Text = "Улица";
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Location = new System.Drawing.Point(17, 52);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(70, 13);
            this.label70.TabIndex = 21;
            this.label70.Text = "Номер дома";
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Location = new System.Drawing.Point(17, 167);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(71, 13);
            this.label81.TabIndex = 30;
            this.label81.Text = "Личный счет";
            // 
            // textBoxSurname
            // 
            this.textBoxSurname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSurname.Location = new System.Drawing.Point(164, 19);
            this.textBoxSurname.Name = "textBoxSurname";
            this.textBoxSurname.Size = new System.Drawing.Size(319, 20);
            this.textBoxSurname.TabIndex = 0;
            this.textBoxSurname.MaxLength = 50;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(164, 48);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(319, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.MaxLength = 50;
            // 
            // textBoxPatronymic
            // 
            this.textBoxPatronymic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPatronymic.Location = new System.Drawing.Point(164, 77);
            this.textBoxPatronymic.Name = "textBoxPatronymic";
            this.textBoxPatronymic.Size = new System.Drawing.Size(319, 20);
            this.textBoxPatronymic.TabIndex = 2;
            this.textBoxPatronymic.MaxLength = 255;
            // 
            // textBoxDocumentSeria
            // 
            this.textBoxDocumentSeria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocumentSeria.Location = new System.Drawing.Point(164, 48);
            this.textBoxDocumentSeria.Name = "textBoxDocumentSeria";
            this.textBoxDocumentSeria.Size = new System.Drawing.Size(319, 20);
            this.textBoxDocumentSeria.TabIndex = 1;
            this.textBoxDocumentSeria.MaxLength = 8;
            // 
            // textBoxDocumentNumber
            // 
            this.textBoxDocumentNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocumentNumber.Location = new System.Drawing.Point(164, 77);
            this.textBoxDocumentNumber.Name = "textBoxDocumentNumber";
            this.textBoxDocumentNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxDocumentNumber.TabIndex = 2;
            this.textBoxDocumentNumber.MaxLength = 8;
            // 
            // textBoxRegistrationHouse
            // 
            this.textBoxRegistrationHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationHouse.Location = new System.Drawing.Point(164, 49);
            this.textBoxRegistrationHouse.MaxLength = 10;
            this.textBoxRegistrationHouse.Name = "textBoxRegistrationHouse";
            this.textBoxRegistrationHouse.Size = new System.Drawing.Size(319, 20);
            this.textBoxRegistrationHouse.TabIndex = 1;
            this.textBoxRegistrationHouse.MaxLength = 10;
            // 
            // textBoxRegistrationFlat
            // 
            this.textBoxRegistrationFlat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationFlat.Location = new System.Drawing.Point(164, 78);
            this.textBoxRegistrationFlat.MaxLength = 15;
            this.textBoxRegistrationFlat.Name = "textBoxRegistrationFlat";
            this.textBoxRegistrationFlat.Size = new System.Drawing.Size(319, 20);
            this.textBoxRegistrationFlat.TabIndex = 2;
            this.textBoxRegistrationFlat.MaxLength = 15;
            // 
            // textBoxRegistrationRoom
            // 
            this.textBoxRegistrationRoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationRoom.Location = new System.Drawing.Point(164, 107);
            this.textBoxRegistrationRoom.MaxLength = 15;
            this.textBoxRegistrationRoom.Name = "textBoxRegistrationRoom";
            this.textBoxRegistrationRoom.Size = new System.Drawing.Size(319, 20);
            this.textBoxRegistrationRoom.TabIndex = 3;
            this.textBoxRegistrationRoom.MaxLength = 15;
            // 
            // textBoxResidenceRoom
            // 
            this.textBoxResidenceRoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceRoom.Location = new System.Drawing.Point(164, 107);
            this.textBoxResidenceRoom.MaxLength = 15;
            this.textBoxResidenceRoom.Name = "textBoxResidenceRoom";
            this.textBoxResidenceRoom.Size = new System.Drawing.Size(319, 20);
            this.textBoxResidenceRoom.TabIndex = 3;
            this.textBoxResidenceRoom.MaxLength = 15;
            // 
            // textBoxResidenceFlat
            // 
            this.textBoxResidenceFlat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceFlat.Location = new System.Drawing.Point(164, 78);
            this.textBoxResidenceFlat.MaxLength = 15;
            this.textBoxResidenceFlat.Name = "textBoxResidenceFlat";
            this.textBoxResidenceFlat.Size = new System.Drawing.Size(319, 20);
            this.textBoxResidenceFlat.TabIndex = 2;
            this.textBoxResidenceFlat.MaxLength = 15;
            // 
            // textBoxResidenceHouse
            // 
            this.textBoxResidenceHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceHouse.Location = new System.Drawing.Point(164, 49);
            this.textBoxResidenceHouse.MaxLength = 10;
            this.textBoxResidenceHouse.Name = "textBoxResidenceHouse";
            this.textBoxResidenceHouse.Size = new System.Drawing.Size(319, 20);
            this.textBoxResidenceHouse.TabIndex = 1;
            this.textBoxResidenceHouse.MaxLength = 10;
            // 
            // textBoxPersonalAccount
            // 
            this.textBoxPersonalAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPersonalAccount.Location = new System.Drawing.Point(164, 164);
            this.textBoxPersonalAccount.Name = "textBoxPersonalAccount";
            this.textBoxPersonalAccount.Size = new System.Drawing.Size(319, 20);
            this.textBoxPersonalAccount.TabIndex = 29;
            this.textBoxPersonalAccount.MaxLength = 255;
            // 
            // comboBoxKinship
            // 
            this.comboBoxKinship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxKinship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKinship.FormattingEnabled = true;
            this.comboBoxKinship.Location = new System.Drawing.Point(164, 135);
            this.comboBoxKinship.Name = "comboBoxKinship";
            this.comboBoxKinship.Size = new System.Drawing.Size(319, 21);
            this.comboBoxKinship.TabIndex = 4;
            // 
            // comboBoxDocumentType
            // 
            this.comboBoxDocumentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDocumentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDocumentType.FormattingEnabled = true;
            this.comboBoxDocumentType.Location = new System.Drawing.Point(164, 19);
            this.comboBoxDocumentType.Name = "comboBoxDocumentType";
            this.comboBoxDocumentType.Size = new System.Drawing.Size(319, 21);
            this.comboBoxDocumentType.TabIndex = 0;
            // 
            // comboBoxIssuedBy
            // 
            this.comboBoxIssuedBy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIssuedBy.FormattingEnabled = true;
            this.comboBoxIssuedBy.Location = new System.Drawing.Point(164, 135);
            this.comboBoxIssuedBy.Name = "comboBoxIssuedBy";
            this.comboBoxIssuedBy.Size = new System.Drawing.Size(319, 21);
            this.comboBoxIssuedBy.TabIndex = 4;
            // 
            // comboBoxRegistrationStreet
            // 
            this.comboBoxRegistrationStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRegistrationStreet.FormattingEnabled = true;
            this.comboBoxRegistrationStreet.Location = new System.Drawing.Point(164, 20);
            this.comboBoxRegistrationStreet.Name = "comboBoxRegistrationStreet";
            this.comboBoxRegistrationStreet.Size = new System.Drawing.Size(319, 21);
            this.comboBoxRegistrationStreet.TabIndex = 0;
            // 
            // comboBoxResidenceStreet
            // 
            this.comboBoxResidenceStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxResidenceStreet.FormattingEnabled = true;
            this.comboBoxResidenceStreet.Location = new System.Drawing.Point(164, 20);
            this.comboBoxResidenceStreet.Name = "comboBoxResidenceStreet";
            this.comboBoxResidenceStreet.Size = new System.Drawing.Size(319, 21);
            this.comboBoxResidenceStreet.TabIndex = 0;
            // 
            // dateTimePickerDateOfBirth
            // 
            this.dateTimePickerDateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfBirth.Location = new System.Drawing.Point(164, 106);
            this.dateTimePickerDateOfBirth.Name = "dateTimePickerDateOfBirth";
            this.dateTimePickerDateOfBirth.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerDateOfBirth.TabIndex = 3;
            this.dateTimePickerDateOfBirth.ShowCheckBox = true;
            // 
            // dateTimePickerDateOfDocumentIssue
            // 
            this.dateTimePickerDateOfDocumentIssue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfDocumentIssue.Location = new System.Drawing.Point(164, 106);
            this.dateTimePickerDateOfDocumentIssue.Name = "dateTimePickerDateOfDocumentIssue";
            this.dateTimePickerDateOfDocumentIssue.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerDateOfDocumentIssue.TabIndex = 3;
            this.dateTimePickerDateOfDocumentIssue.ShowCheckBox = true;

            this.tableLayoutPanel11.ResumeLayout(false);
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            this.groupBox27.ResumeLayout(false);
            this.groupBox27.PerformLayout();
            this.groupBox28.ResumeLayout(false);
            this.groupBox28.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).EndInit();
        }
    }
}
