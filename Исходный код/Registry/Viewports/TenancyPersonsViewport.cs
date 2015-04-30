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
using Security;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class TenancyPersonsViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel11;
        private GroupBox groupBox23;
        private GroupBox groupBox26;
        private GroupBox groupBox27;
        private GroupBox groupBox28;
        private DataGridView dataGridViewTenancyPersons;
        private Label label53;
        private Label label54;
        private Label label55;
        private Label label56;
        private Label label57;
        private Label label58;
        private Label label59;
        private Label label60;
        private Label label61;
        private Label label62;
        private Label label63;
        private Label label64;
        private Label label65;
        private Label label66;
        private Label label67;
        private Label label68;
        private Label label69;
        private Label label70;
        private Label label81;
        private TextBox textBoxSurname;
        private TextBox textBoxName;
        private TextBox textBoxPatronymic;
        private TextBox textBoxDocumentSeria;
        private TextBox textBoxDocumentNumber;
        private TextBox textBoxRegistrationHouse;
        private TextBox textBoxRegistrationFlat;
        private TextBox textBoxRegistrationRoom;
        private TextBox textBoxResidenceRoom;
        private TextBox textBoxResidenceFlat;
        private TextBox textBoxResidenceHouse;
        private TextBox textBoxPersonalAccount;
        private ComboBox comboBoxKinship;
        private ComboBox comboBoxDocumentType;
        private ComboBox comboBoxIssuedBy;
        private ComboBox comboBoxRegistrationStreet;
        private ComboBox comboBoxResidenceStreet;
        private DateTimePicker dateTimePickerDateOfBirth;
        private DateTimePicker dateTimePickerDateOfDocumentIssue;
        #endregion Components

        #region Models
        TenancyPersonsDataModel tenancy_persons = null;
        KinshipsDataModel kinships = null;
        DocumentTypesDataModel document_types = null;
        DocumentsIssuedByDataModel document_issued_by = null;
        KladrStreetsDataModel kladr = null;
        #endregion Models

        #region Views
        BindingSource v_tenancy_persons = null;
        BindingSource v_kinships = null;
        BindingSource v_document_types = null;
        BindingSource v_document_issued_by = null;
        BindingSource v_registration_street = null;
        BindingSource v_residence_street = null;
        #endregion Views

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        private DataGridViewTextBoxColumn date_of_birth;
        private DataGridViewComboBoxColumn id_kinship;
        private DateTimePicker dateTimePickerExcludeDate;
        private Label label2;
        private DateTimePicker dateTimePickerIncludeDate;
        private Label label1;
        private bool is_editable = false;

        private TenancyPersonsViewport()
            : this(null)
        {
        }

        public TenancyPersonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public TenancyPersonsViewport(TenancyPersonsViewport tenancyPersonsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = tenancyPersonsViewport.DynamicFilter;
            this.StaticFilter = tenancyPersonsViewport.StaticFilter;
            this.ParentRow = tenancyPersonsViewport.ParentRow;
            this.ParentType = tenancyPersonsViewport.ParentType;
        }

        private void RedrawDataGridRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (int i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
                if (((DataRowView)v_tenancy_persons[i])["id_kinship"] != DBNull.Value &&
                    Convert.ToInt32(((DataRowView)v_tenancy_persons[i])["id_kinship"], CultureInfo.InvariantCulture) == 1 &&
                    ((DataRowView)v_tenancy_persons[i])["exclude_date"] == DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else
                if (((DataRowView)v_tenancy_persons[i])["exclude_date"] != DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                else
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
        }

        private void DataBind()
        {
            comboBoxKinship.DataSource = v_kinships;
            comboBoxKinship.ValueMember = "id_kinship";
            comboBoxKinship.DisplayMember = "kinship";
            comboBoxKinship.DataBindings.Clear();
            comboBoxKinship.DataBindings.Add("SelectedValue", v_tenancy_persons, "id_kinship", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxDocumentType.DataSource = v_document_types;
            comboBoxDocumentType.ValueMember = "id_document_type";
            comboBoxDocumentType.DisplayMember = "document_type";
            comboBoxDocumentType.DataBindings.Clear();
            comboBoxDocumentType.DataBindings.Add("SelectedValue", v_tenancy_persons, "id_document_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxIssuedBy.DataSource = v_document_issued_by;
            comboBoxIssuedBy.ValueMember = "id_document_issued_by";
            comboBoxIssuedBy.DisplayMember = "document_issued_by";
            comboBoxIssuedBy.DataBindings.Clear();
            comboBoxIssuedBy.DataBindings.Add("SelectedValue", v_tenancy_persons, "id_document_issued_by", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxRegistrationStreet.DataSource = v_registration_street;
            comboBoxRegistrationStreet.ValueMember = "id_street";
            comboBoxRegistrationStreet.DisplayMember = "street_name";
            comboBoxRegistrationStreet.DataBindings.Clear();
            comboBoxRegistrationStreet.DataBindings.Add("SelectedValue", v_tenancy_persons, "registration_id_street", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxResidenceStreet.DataSource = v_residence_street;
            comboBoxResidenceStreet.ValueMember = "id_street";
            comboBoxResidenceStreet.DisplayMember = "street_name";
            comboBoxResidenceStreet.DataBindings.Clear();
            comboBoxResidenceStreet.DataBindings.Add("SelectedValue", v_tenancy_persons, "residence_id_street", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxSurname.DataBindings.Clear();
            textBoxSurname.DataBindings.Add("Text", v_tenancy_persons, "surname", true, DataSourceUpdateMode.Never, "");
            textBoxName.DataBindings.Clear();
            textBoxName.DataBindings.Add("Text", v_tenancy_persons, "name", true, DataSourceUpdateMode.Never, "");
            textBoxPatronymic.DataBindings.Clear();
            textBoxPatronymic.DataBindings.Add("Text", v_tenancy_persons, "patronymic", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDateOfBirth.DataBindings.Clear();
            dateTimePickerDateOfBirth.DataBindings.Add("Value", v_tenancy_persons, "date_of_birth", true, DataSourceUpdateMode.Never, null);
            textBoxPersonalAccount.DataBindings.Clear();
            textBoxPersonalAccount.DataBindings.Add("Text", v_tenancy_persons, "personal_account", true, DataSourceUpdateMode.Never, "");
            textBoxDocumentSeria.DataBindings.Clear();
            textBoxDocumentSeria.DataBindings.Add("Text", v_tenancy_persons, "document_seria", true, DataSourceUpdateMode.Never, "");
            textBoxDocumentNumber.DataBindings.Clear();
            textBoxDocumentNumber.DataBindings.Add("Text", v_tenancy_persons, "document_num", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDateOfDocumentIssue.DataBindings.Clear();
            dateTimePickerDateOfDocumentIssue.DataBindings.Add("Value", v_tenancy_persons, "date_of_document_issue", true, DataSourceUpdateMode.Never, null);
            textBoxRegistrationHouse.DataBindings.Clear();
            textBoxRegistrationHouse.DataBindings.Add("Text", v_tenancy_persons, "registration_house", true, DataSourceUpdateMode.Never, "");
            textBoxRegistrationFlat.DataBindings.Clear();
            textBoxRegistrationFlat.DataBindings.Add("Text", v_tenancy_persons, "registration_flat", true, DataSourceUpdateMode.Never, "");
            textBoxRegistrationRoom.DataBindings.Clear();
            textBoxRegistrationRoom.DataBindings.Add("Text", v_tenancy_persons, "registration_room", true, DataSourceUpdateMode.Never, "");
            textBoxResidenceHouse.DataBindings.Clear();
            textBoxResidenceHouse.DataBindings.Add("Text", v_tenancy_persons, "residence_house", true, DataSourceUpdateMode.Never, "");
            textBoxResidenceFlat.DataBindings.Clear();
            textBoxResidenceFlat.DataBindings.Add("Text", v_tenancy_persons, "residence_flat", true, DataSourceUpdateMode.Never, "");
            textBoxResidenceRoom.DataBindings.Clear();
            textBoxResidenceRoom.DataBindings.Add("Text", v_tenancy_persons, "residence_room", true, DataSourceUpdateMode.Never, "");
            dateTimePickerIncludeDate.DataBindings.Clear();
            dateTimePickerIncludeDate.DataBindings.Add("Value", v_tenancy_persons, "include_date", true, DataSourceUpdateMode.Never, null);
            dateTimePickerExcludeDate.DataBindings.Clear();
            dateTimePickerExcludeDate.DataBindings.Add("Value", v_tenancy_persons, "exclude_date", true, DataSourceUpdateMode.Never, null);
            dataGridViewTenancyPersons.DataSource = v_tenancy_persons;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            date_of_birth.DataPropertyName = "date_of_birth";
            id_kinship.DataSource = v_kinships;
            id_kinship.DisplayMember = "kinship";
            id_kinship.ValueMember = "id_kinship";
            id_kinship.DataPropertyName = "id_kinship";
        }

        private void UnbindedCheckBoxesUpdate()
        {
            DataRowView row = (v_tenancy_persons.Position >= 0) ? (DataRowView)v_tenancy_persons[v_tenancy_persons.Position] : null;
            if ((v_tenancy_persons.Position >= 0) && (row["date_of_birth"] != DBNull.Value))
                dateTimePickerDateOfBirth.Checked = true;
            else
            {
                dateTimePickerDateOfBirth.Value = DateTime.Now.Date;
                dateTimePickerDateOfBirth.Checked = false;
            }
            if ((v_tenancy_persons.Position >= 0) && (row["date_of_document_issue"] != DBNull.Value))
                dateTimePickerDateOfDocumentIssue.Checked = true;
            else
            {
                dateTimePickerDateOfDocumentIssue.Value = DateTime.Now.Date;
                dateTimePickerDateOfDocumentIssue.Checked = false;
            }
            if ((v_tenancy_persons.Position >= 0) && (row["include_date"] != DBNull.Value))
                dateTimePickerIncludeDate.Checked = true;
            else
            {
                dateTimePickerIncludeDate.Value = DateTime.Now.Date;
                dateTimePickerIncludeDate.Checked = false;
            }
            if ((v_tenancy_persons.Position >= 0) && (row["exclude_date"] != DBNull.Value))
                dateTimePickerExcludeDate.Checked = true;
            else
            {
                dateTimePickerExcludeDate.Value = DateTime.Now.Date;
                dateTimePickerExcludeDate.Checked = false;
            }
            if ((comboBoxRegistrationStreet.DataSource != null) && (row != null))
                comboBoxRegistrationStreet.SelectedValue = row["registration_id_street"];
            if ((comboBoxResidenceStreet.DataSource != null) && (row != null))
                comboBoxResidenceStreet.SelectedValue = row["residence_id_street"];
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!this.ContainsFocus) || (dataGridViewTenancyPersons.Focused))
                return;
            if ((v_tenancy_persons.Position != -1) && (TenancyPersonFromView() != TenancyPersonFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    MenuCallback.EditingStateUpdate();
                    dataGridViewTenancyPersons.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    MenuCallback.EditingStateUpdate();
                    dataGridViewTenancyPersons.Enabled = true;
                }
            }
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
                            if (tenancy_persons.EditingNewRecord)
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

        public void LocatePersonBy(int id)
        {
            int Position = v_tenancy_persons.Find("id_person", id);
            is_editable = false;
            if (Position > 0)
               v_tenancy_persons.Position = Position;
            is_editable = true;
        }

        private void ViewportFromTenancyPerson(TenancyPerson tenancyPerson)
        {
            comboBoxKinship.SelectedValue = ViewportHelper.ValueOrDBNull(tenancyPerson.IdKinship);
            comboBoxDocumentType.SelectedValue = ViewportHelper.ValueOrDBNull(tenancyPerson.IdDocumentType);
            comboBoxIssuedBy.SelectedValue = ViewportHelper.ValueOrDBNull(tenancyPerson.IdDocumentIssuedBy);
            comboBoxRegistrationStreet.SelectedValue = ViewportHelper.ValueOrDBNull(tenancyPerson.RegistrationIdStreet);
            comboBoxResidenceStreet.SelectedValue = ViewportHelper.ValueOrDBNull(tenancyPerson.ResidenceIdStreet);
            textBoxSurname.Text = tenancyPerson.Surname;
            textBoxName.Text = tenancyPerson.Name;
            textBoxPatronymic.Text = tenancyPerson.Patronymic;
            textBoxPersonalAccount.Text = tenancyPerson.PersonalAccount;
            textBoxDocumentNumber.Text = tenancyPerson.DocumentNum;
            textBoxDocumentSeria.Text = tenancyPerson.DocumentSeria;
            textBoxRegistrationHouse.Text = tenancyPerson.RegistrationHouse;
            textBoxRegistrationFlat.Text = tenancyPerson.RegistrationFlat;
            textBoxRegistrationRoom.Text = tenancyPerson.RegistrationRoom;
            textBoxResidenceHouse.Text = tenancyPerson.ResidenceHouse;
            textBoxResidenceFlat.Text = tenancyPerson.ResidenceFlat;
            textBoxResidenceRoom.Text = tenancyPerson.ResidenceRoom;
            dateTimePickerDateOfBirth.Value = ViewportHelper.ValueOrDefault(tenancyPerson.DateOfBirth);
            dateTimePickerDateOfBirth.Checked = (tenancyPerson.DateOfBirth != null);
            dateTimePickerDateOfDocumentIssue.Value = ViewportHelper.ValueOrDefault(tenancyPerson.DateOfDocumentIssue);
            dateTimePickerDateOfDocumentIssue.Checked = (tenancyPerson.DateOfDocumentIssue != null);
            dateTimePickerIncludeDate.Value = ViewportHelper.ValueOrDefault(tenancyPerson.IncludeDate);
            dateTimePickerIncludeDate.Checked = (tenancyPerson.IncludeDate != null);
            dateTimePickerExcludeDate.Value = ViewportHelper.ValueOrDefault(tenancyPerson.ExcludeDate);
            dateTimePickerExcludeDate.Checked = (tenancyPerson.ExcludeDate != null);
        }

        private TenancyPerson TenancyPersonFromViewport()
        {
            TenancyPerson tenancyPerson = new TenancyPerson();
            if (v_tenancy_persons.Position == -1)
                tenancyPerson.IdPerson = null;
            else
                tenancyPerson.IdPerson = ViewportHelper.ValueOrNull<int>((DataRowView)v_tenancy_persons[v_tenancy_persons.Position], "id_person");
            if (ParentType == ParentTypeEnum.Tenancy && ParentRow != null)
                tenancyPerson.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
            else
                tenancyPerson.IdProcess = null;
            tenancyPerson.IdKinship = ViewportHelper.ValueOrNull<int>(comboBoxKinship);
            tenancyPerson.IdDocumentType = ViewportHelper.ValueOrNull<int>(comboBoxDocumentType);
            tenancyPerson.IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(comboBoxIssuedBy);
            tenancyPerson.RegistrationIdStreet = ViewportHelper.ValueOrNull(comboBoxRegistrationStreet);
            tenancyPerson.ResidenceIdStreet = ViewportHelper.ValueOrNull(comboBoxResidenceStreet);           
            tenancyPerson.Surname = ViewportHelper.ValueOrNull(textBoxSurname);
            if (tenancyPerson.Surname != null)
                tenancyPerson.Surname = tenancyPerson.Surname[0].ToString().ToUpper(CultureInfo.CurrentCulture) + tenancyPerson.Surname.Substring(1);
            tenancyPerson.Name = ViewportHelper.ValueOrNull(textBoxName);
            if (tenancyPerson.Name != null)
                tenancyPerson.Name = tenancyPerson.Name[0].ToString().ToUpper(CultureInfo.CurrentCulture) + tenancyPerson.Name.Substring(1);
            tenancyPerson.Patronymic = ViewportHelper.ValueOrNull(textBoxPatronymic);
            if (tenancyPerson.Patronymic != null)
                tenancyPerson.Patronymic = tenancyPerson.Patronymic[0].ToString().ToUpper(CultureInfo.CurrentCulture) + tenancyPerson.Patronymic.Substring(1);
            tenancyPerson.PersonalAccount = ViewportHelper.ValueOrNull(textBoxPersonalAccount);
            tenancyPerson.DocumentSeria = ViewportHelper.ValueOrNull(textBoxDocumentSeria);
            tenancyPerson.DocumentNum = ViewportHelper.ValueOrNull(textBoxDocumentNumber);
            tenancyPerson.RegistrationHouse = ViewportHelper.ValueOrNull(textBoxRegistrationHouse);
            tenancyPerson.RegistrationFlat = ViewportHelper.ValueOrNull(textBoxRegistrationFlat);
            tenancyPerson.RegistrationRoom = ViewportHelper.ValueOrNull(textBoxRegistrationRoom);
            tenancyPerson.ResidenceHouse = ViewportHelper.ValueOrNull(textBoxResidenceHouse);
            tenancyPerson.ResidenceFlat = ViewportHelper.ValueOrNull(textBoxResidenceFlat);
            tenancyPerson.ResidenceRoom = ViewportHelper.ValueOrNull(textBoxResidenceRoom);
            tenancyPerson.DateOfBirth = ViewportHelper.ValueOrNull(dateTimePickerDateOfBirth);
            tenancyPerson.DateOfDocumentIssue = ViewportHelper.ValueOrNull(dateTimePickerDateOfDocumentIssue);
            tenancyPerson.IncludeDate = ViewportHelper.ValueOrNull(dateTimePickerIncludeDate);
            tenancyPerson.ExcludeDate = ViewportHelper.ValueOrNull(dateTimePickerExcludeDate);
            return tenancyPerson;
        }

        private TenancyPerson TenancyPersonFromView()
        {
            TenancyPerson tenancyPerson = new TenancyPerson();
            DataRowView row = (DataRowView)v_tenancy_persons[v_tenancy_persons.Position];
            tenancyPerson.IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person");
            tenancyPerson.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            tenancyPerson.IdKinship = ViewportHelper.ValueOrNull<int>(row, "id_kinship");
            tenancyPerson.IdDocumentType = ViewportHelper.ValueOrNull<int>(row, "id_document_type");
            tenancyPerson.IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by");
            tenancyPerson.Surname = ViewportHelper.ValueOrNull(row, "surname");
            tenancyPerson.Name = ViewportHelper.ValueOrNull(row, "name");
            tenancyPerson.Patronymic = ViewportHelper.ValueOrNull(row, "patronymic");
            tenancyPerson.DateOfBirth = ViewportHelper.ValueOrNull<DateTime>(row, "date_of_birth");
            tenancyPerson.DateOfDocumentIssue = ViewportHelper.ValueOrNull<DateTime>(row, "date_of_document_issue");
            tenancyPerson.DocumentNum = ViewportHelper.ValueOrNull(row, "document_num");
            tenancyPerson.DocumentSeria = ViewportHelper.ValueOrNull(row, "document_seria");
            tenancyPerson.RegistrationIdStreet = ViewportHelper.ValueOrNull(row, "registration_id_street");
            tenancyPerson.RegistrationHouse = ViewportHelper.ValueOrNull(row, "registration_house");
            tenancyPerson.RegistrationFlat = ViewportHelper.ValueOrNull(row, "registration_flat");
            tenancyPerson.RegistrationRoom = ViewportHelper.ValueOrNull(row, "registration_room");
            tenancyPerson.ResidenceIdStreet = ViewportHelper.ValueOrNull(row, "residence_id_street");
            tenancyPerson.ResidenceHouse = ViewportHelper.ValueOrNull(row, "residence_house");
            tenancyPerson.ResidenceFlat = ViewportHelper.ValueOrNull(row, "residence_flat");
            tenancyPerson.ResidenceRoom = ViewportHelper.ValueOrNull(row, "residence_room");
            tenancyPerson.PersonalAccount = ViewportHelper.ValueOrNull(row, "personal_account");
            tenancyPerson.IncludeDate = ViewportHelper.ValueOrNull<DateTime>(row, "include_date");
            tenancyPerson.ExcludeDate = ViewportHelper.ValueOrNull<DateTime>(row, "exclude_date");
            return tenancyPerson;
        }

        private static void FillRowFromTenancyPerson(TenancyPerson tenancyPerson, DataRowView row)
        {
            row.BeginEdit();
            row["id_person"] = ViewportHelper.ValueOrDBNull(tenancyPerson.IdPerson);
            row["id_process"] = ViewportHelper.ValueOrDBNull(tenancyPerson.IdProcess);
            row["id_kinship"] = ViewportHelper.ValueOrDBNull(tenancyPerson.IdKinship);
            row["surname"] = ViewportHelper.ValueOrDBNull(tenancyPerson.Surname);
            row["name"] = ViewportHelper.ValueOrDBNull(tenancyPerson.Name);
            row["patronymic"] = ViewportHelper.ValueOrDBNull(tenancyPerson.Patronymic);
            row["date_of_birth"] = ViewportHelper.ValueOrDBNull(tenancyPerson.DateOfBirth);
            row["id_document_type"] = ViewportHelper.ValueOrDBNull(tenancyPerson.IdDocumentType);
            row["date_of_document_issue"] = ViewportHelper.ValueOrDBNull(tenancyPerson.DateOfDocumentIssue);
            row["document_num"] = ViewportHelper.ValueOrDBNull(tenancyPerson.DocumentNum);
            row["document_seria"] = ViewportHelper.ValueOrDBNull(tenancyPerson.DocumentSeria);
            row["id_document_issued_by"] = ViewportHelper.ValueOrDBNull(tenancyPerson.IdDocumentIssuedBy);
            row["registration_id_street"] = ViewportHelper.ValueOrDBNull(tenancyPerson.RegistrationIdStreet);
            row["registration_house"] = ViewportHelper.ValueOrDBNull(tenancyPerson.RegistrationHouse);
            row["registration_flat"] = ViewportHelper.ValueOrDBNull(tenancyPerson.RegistrationFlat);
            row["registration_room"] = ViewportHelper.ValueOrDBNull(tenancyPerson.RegistrationRoom);
            row["residence_id_street"] = ViewportHelper.ValueOrDBNull(tenancyPerson.ResidenceIdStreet);
            row["residence_house"] = ViewportHelper.ValueOrDBNull(tenancyPerson.ResidenceHouse);
            row["residence_flat"] = ViewportHelper.ValueOrDBNull(tenancyPerson.ResidenceFlat);
            row["residence_room"] = ViewportHelper.ValueOrDBNull(tenancyPerson.ResidenceRoom);
            row["personal_account"] = ViewportHelper.ValueOrDBNull(tenancyPerson.PersonalAccount);
            row["include_date"] = ViewportHelper.ValueOrDBNull(tenancyPerson.IncludeDate);
            row["exclude_date"] = ViewportHelper.ValueOrDBNull(tenancyPerson.ExcludeDate);
            row.EndEdit();
        }

        private bool ValidateTenancyPerson(TenancyPerson tenancyPerson)
        {
            if (tenancyPerson.Surname == null)
            {
                MessageBox.Show("Необходимо указать фамилию участника найма", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxSurname.Focus();
                return false;
            }
            if (tenancyPerson.Name == null)
            {
                MessageBox.Show("Необходимо указать имя участника найма", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxName.Focus();
                return false;
            }
            if (tenancyPerson.IdKinship == null)
            {
                MessageBox.Show("Необходимо выбрать родственную связь", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxKinship.Focus();
                return false;
            }
            if (tenancyPerson.IdKinship == 1)
            {
                for (int i = 0; i < v_tenancy_persons.Count; i++)
                {
                    if (((DataRowView)v_tenancy_persons[i])["id_kinship"] != DBNull.Value &&
                        (Convert.ToInt32(((DataRowView)v_tenancy_persons[i])["id_kinship"], CultureInfo.InvariantCulture) == 1) &&
                        (((DataRowView)v_tenancy_persons[i])["exclude_date"] == DBNull.Value) &&
                        (Convert.ToInt32(((DataRowView)v_tenancy_persons[i])["id_person"], CultureInfo.InvariantCulture) != tenancyPerson.IdPerson))
                    {
                        MessageBox.Show("В процессе найма может быть только один наниматель", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        comboBoxKinship.Focus();
                        return false;
                    }
                }
            }
            if (tenancyPerson.IdDocumentType == null)
            {
                MessageBox.Show("Необходимо выбрать вид документа, удостоверяющего личность", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDocumentType.Focus();
                return false;
            }
            return true;
        }

        public override int GetRecordCount()
        {
            return v_tenancy_persons.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancy_persons.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancy_persons.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancy_persons.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancy_persons.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_tenancy_persons.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_tenancy_persons.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_tenancy_persons.Position > -1) && (v_tenancy_persons.Position < (v_tenancy_persons.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_tenancy_persons.Position > -1) && (v_tenancy_persons.Position < (v_tenancy_persons.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridViewTenancyPersons.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            tenancy_persons = TenancyPersonsDataModel.GetInstance();
            kinships = KinshipsDataModel.GetInstance();
            document_types = DocumentTypesDataModel.GetInstance();
            document_issued_by = DocumentsIssuedByDataModel.GetInstance();
            kladr = KladrStreetsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            tenancy_persons.Select();
            kinships.Select();
            document_types.Select();
            document_issued_by.Select();
            kladr.Select();

            DataSet ds = DataSetManager.DataSet;

            if ((ParentType == ParentTypeEnum.Tenancy) && (ParentRow != null))
                this.Text = String.Format(CultureInfo.InvariantCulture, "Участники найма №{0}", ParentRow["id_process"].ToString());
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

            v_tenancy_persons = new BindingSource();
            v_tenancy_persons.CurrentItemChanged += new EventHandler(v_tenancy_persons_CurrentItemChanged);
            v_tenancy_persons.DataMember = "tenancy_persons";
            v_tenancy_persons.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_tenancy_persons.Filter += " AND ";
            v_tenancy_persons.Filter += DynamicFilter;
            v_tenancy_persons.DataSource = ds;

            DataBind();

            tenancy_persons.Select().RowDeleted += new DataRowChangeEventHandler(TenancyPersonsViewport_RowDeleted);
            tenancy_persons.Select().RowChanged += new DataRowChangeEventHandler(TenancyPersonsViewport_RowChanged);
            is_editable = true;
            if (v_tenancy_persons.Count == 0)
                InsertRecord();
        }

        public override bool CanInsertRecord()
        {
            return (!tenancy_persons.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            v_tenancy_persons.AddNew();
            dataGridViewTenancyPersons.Enabled = false;
            is_editable = true;
            tenancy_persons.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (v_tenancy_persons.Position != -1) && (!tenancy_persons.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            TenancyPerson tenancyPerson = TenancyPersonFromView();
            v_tenancy_persons.AddNew();
            dataGridViewTenancyPersons.Enabled = false;
            tenancy_persons.EditingNewRecord = true;
            ViewportFromTenancyPerson(tenancyPerson);
            dateTimePickerDateOfBirth.Checked = (tenancyPerson.DateOfBirth != null);
            dateTimePickerDateOfDocumentIssue.Checked = (tenancyPerson.DateOfDocumentIssue != null);
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (v_tenancy_persons.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите этого участника договора?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (TenancyPersonsDataModel.Delete((int)((DataRowView)v_tenancy_persons.Current)["id_person"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_tenancy_persons[v_tenancy_persons.Position]).Delete();
                is_editable = true;
                RedrawDataGridRows();
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports(); 
                if (ParentType == ParentTypeEnum.Tenancy)
                    CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.TenancyProcess,
                        (int)ParentRow["id_process"], true);
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            TenancyPersonsViewport viewport = new TenancyPersonsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancy_persons.Count > 0)
                viewport.LocatePersonBy((((DataRowView)v_tenancy_persons[v_tenancy_persons.Position])["id_person"] as Int32?) ?? -1);
            return viewport;
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
                    tenancy_persons.EditingNewRecord = false;
                    if (v_tenancy_persons.Position != -1)
                    {
                        is_editable = false; 
                        dataGridViewTenancyPersons.Enabled = true;
                        ((DataRowView)v_tenancy_persons[v_tenancy_persons.Position]).Delete();
                        RedrawDataGridRows();
                        if (v_tenancy_persons.Position != -1)
                            dataGridViewTenancyPersons.Rows[v_tenancy_persons.Position].Selected = true;
                    }
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridViewTenancyPersons.Enabled = true;
                    is_editable = false;
                    DataBind();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            TenancyPerson tenancyPerson = TenancyPersonFromViewport();
            if (!ValidateTenancyPerson(tenancyPerson))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                case ViewportState.NewRowState:
                    int id_person = TenancyPersonsDataModel.Insert(tenancyPerson);
                    if (id_person == -1)
                        return;
                    DataRowView newRow;
                    tenancyPerson.IdPerson = id_person;
                    is_editable = false;
                    if (v_tenancy_persons.Position == -1)
                        newRow = (DataRowView)v_tenancy_persons.AddNew();
                    else
                        newRow = ((DataRowView)v_tenancy_persons[v_tenancy_persons.Position]);
                    FillRowFromTenancyPerson(tenancyPerson, newRow);
                    tenancy_persons.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancyPerson.IdPerson == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись об участнике договора без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (TenancyPersonsDataModel.Update(tenancyPerson) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_tenancy_persons[v_tenancy_persons.Position]);
                    is_editable = false;
                    FillRowFromTenancyPerson(tenancyPerson, row);
                    break;
            }
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            viewportState = ViewportState.ReadState;
            dataGridViewTenancyPersons.Enabled = true;
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            if (ParentType == ParentTypeEnum.Tenancy)
                CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.TenancyProcess, (int)ParentRow["id_process"], false);
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
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                tenancy_persons.EditingNewRecord = false;
            tenancy_persons.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyPersonsViewport_RowDeleted);
            tenancy_persons.Select().RowChanged -= new DataRowChangeEventHandler(TenancyPersonsViewport_RowChanged);
            base.Close();
        }

        void v_tenancy_persons_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_tenancy_persons.Position == -1 || dataGridViewTenancyPersons.RowCount == 0)
                dataGridViewTenancyPersons.ClearSelection();
            else
            if (v_tenancy_persons.Position >= dataGridViewTenancyPersons.RowCount)
                dataGridViewTenancyPersons.Rows[dataGridViewTenancyPersons.RowCount - 1].Selected = true;
            else
            if (dataGridViewTenancyPersons.Rows[v_tenancy_persons.Position].Selected != true)
                dataGridViewTenancyPersons.Rows[v_tenancy_persons.Position].Selected = true;
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            v_registration_street.Filter = "";
            v_residence_street.Filter = "";
            v_document_issued_by.Filter = "";
            UnbindedCheckBoxesUpdate();
            if (v_tenancy_persons.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridViewTenancyPersons.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        void TenancyPersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            RedrawDataGridRows();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void TenancyPersonsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                UnbindedCheckBoxesUpdate();
                RedrawDataGridRows();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            base.OnVisibleChanged(e);
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
            if (String.IsNullOrEmpty(comboBoxIssuedBy.Text))
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
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
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
            if (String.IsNullOrEmpty(comboBoxResidenceStreet.Text))
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
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
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
            if (String.IsNullOrEmpty(comboBoxRegistrationStreet.Text))
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

        void dataGridViewTenancyPersons_DataError(object sender, DataGridViewDataErrorEventArgs e)
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

        private void dateTimePickerIncludeDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void dateTimePickerExcludeDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyPersonsViewport));
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.textBoxPersonalAccount = new System.Windows.Forms.TextBox();
            this.label81 = new System.Windows.Forms.Label();
            this.dateTimePickerDateOfBirth = new System.Windows.Forms.DateTimePicker();
            this.comboBoxKinship = new System.Windows.Forms.ComboBox();
            this.label57 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.textBoxPatronymic = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label54 = new System.Windows.Forms.Label();
            this.textBoxSurname = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.groupBox27 = new System.Windows.Forms.GroupBox();
            this.label66 = new System.Windows.Forms.Label();
            this.textBoxRegistrationRoom = new System.Windows.Forms.TextBox();
            this.label65 = new System.Windows.Forms.Label();
            this.textBoxRegistrationFlat = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.comboBoxRegistrationStreet = new System.Windows.Forms.ComboBox();
            this.textBoxRegistrationHouse = new System.Windows.Forms.TextBox();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.dateTimePickerExcludeDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerIncludeDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxIssuedBy = new System.Windows.Forms.ComboBox();
            this.label62 = new System.Windows.Forms.Label();
            this.dateTimePickerDateOfDocumentIssue = new System.Windows.Forms.DateTimePicker();
            this.label61 = new System.Windows.Forms.Label();
            this.textBoxDocumentNumber = new System.Windows.Forms.TextBox();
            this.label60 = new System.Windows.Forms.Label();
            this.textBoxDocumentSeria = new System.Windows.Forms.TextBox();
            this.label59 = new System.Windows.Forms.Label();
            this.comboBoxDocumentType = new System.Windows.Forms.ComboBox();
            this.label58 = new System.Windows.Forms.Label();
            this.groupBox28 = new System.Windows.Forms.GroupBox();
            this.label67 = new System.Windows.Forms.Label();
            this.textBoxResidenceRoom = new System.Windows.Forms.TextBox();
            this.label68 = new System.Windows.Forms.Label();
            this.textBoxResidenceFlat = new System.Windows.Forms.TextBox();
            this.label69 = new System.Windows.Forms.Label();
            this.label70 = new System.Windows.Forms.Label();
            this.comboBoxResidenceStreet = new System.Windows.Forms.ComboBox();
            this.textBoxResidenceHouse = new System.Windows.Forms.TextBox();
            this.dataGridViewTenancyPersons = new System.Windows.Forms.DataGridView();
            this.surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_of_birth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_kinship = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tableLayoutPanel11.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox27.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.groupBox28.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).BeginInit();
            this.SuspendLayout();
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
            this.tableLayoutPanel11.Controls.Add(this.dataGridViewTenancyPersons, 0, 2);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 3;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(813, 564);
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
            this.groupBox23.Size = new System.Drawing.Size(400, 214);
            this.groupBox23.TabIndex = 1;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Личные данные";
            // 
            // textBoxPersonalAccount
            // 
            this.textBoxPersonalAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPersonalAccount.Location = new System.Drawing.Point(164, 157);
            this.textBoxPersonalAccount.MaxLength = 255;
            this.textBoxPersonalAccount.Name = "textBoxPersonalAccount";
            this.textBoxPersonalAccount.Size = new System.Drawing.Size(230, 21);
            this.textBoxPersonalAccount.TabIndex = 29;
            this.textBoxPersonalAccount.TextChanged += new System.EventHandler(this.textBoxPersonalAccount_TextChanged);
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Location = new System.Drawing.Point(17, 160);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(80, 15);
            this.label81.TabIndex = 30;
            this.label81.Text = "Личный счет";
            // 
            // dateTimePickerDateOfBirth
            // 
            this.dateTimePickerDateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfBirth.Location = new System.Drawing.Point(164, 100);
            this.dateTimePickerDateOfBirth.Name = "dateTimePickerDateOfBirth";
            this.dateTimePickerDateOfBirth.ShowCheckBox = true;
            this.dateTimePickerDateOfBirth.Size = new System.Drawing.Size(230, 21);
            this.dateTimePickerDateOfBirth.TabIndex = 3;
            this.dateTimePickerDateOfBirth.ValueChanged += new System.EventHandler(this.dateTimePickerDateOfBirth_ValueChanged);
            // 
            // comboBoxKinship
            // 
            this.comboBoxKinship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxKinship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKinship.FormattingEnabled = true;
            this.comboBoxKinship.Location = new System.Drawing.Point(164, 127);
            this.comboBoxKinship.Name = "comboBoxKinship";
            this.comboBoxKinship.Size = new System.Drawing.Size(230, 23);
            this.comboBoxKinship.TabIndex = 4;
            this.comboBoxKinship.SelectedValueChanged += new System.EventHandler(this.comboBoxKinship_SelectedValueChanged);
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(17, 131);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(110, 15);
            this.label57.TabIndex = 28;
            this.label57.Text = "Отношение/связь";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(17, 103);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(98, 15);
            this.label56.TabIndex = 26;
            this.label56.Text = "Дата рождения";
            // 
            // textBoxPatronymic
            // 
            this.textBoxPatronymic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPatronymic.Location = new System.Drawing.Point(164, 73);
            this.textBoxPatronymic.MaxLength = 255;
            this.textBoxPatronymic.Name = "textBoxPatronymic";
            this.textBoxPatronymic.Size = new System.Drawing.Size(230, 21);
            this.textBoxPatronymic.TabIndex = 2;
            this.textBoxPatronymic.TextChanged += new System.EventHandler(this.textBoxPatronymic_TextChanged);
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(17, 76);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(63, 15);
            this.label55.TabIndex = 24;
            this.label55.Text = "Отчество";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(164, 46);
            this.textBoxName.MaxLength = 50;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(230, 21);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(17, 49);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(32, 15);
            this.label54.TabIndex = 22;
            this.label54.Text = "Имя";
            // 
            // textBoxSurname
            // 
            this.textBoxSurname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSurname.Location = new System.Drawing.Point(164, 19);
            this.textBoxSurname.MaxLength = 50;
            this.textBoxSurname.Name = "textBoxSurname";
            this.textBoxSurname.Size = new System.Drawing.Size(230, 21);
            this.textBoxSurname.TabIndex = 0;
            this.textBoxSurname.TextChanged += new System.EventHandler(this.textBoxSurname_TextChanged);
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(17, 22);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(62, 15);
            this.label53.TabIndex = 20;
            this.label53.Text = "Фамилия";
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
            this.groupBox27.Location = new System.Drawing.Point(3, 223);
            this.groupBox27.Name = "groupBox27";
            this.groupBox27.Size = new System.Drawing.Size(400, 134);
            this.groupBox27.TabIndex = 3;
            this.groupBox27.TabStop = false;
            this.groupBox27.Text = "Адрес регистрации";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(17, 109);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(101, 15);
            this.label66.TabIndex = 18;
            this.label66.Text = "Номер комнаты";
            // 
            // textBoxRegistrationRoom
            // 
            this.textBoxRegistrationRoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationRoom.Location = new System.Drawing.Point(164, 106);
            this.textBoxRegistrationRoom.MaxLength = 15;
            this.textBoxRegistrationRoom.Name = "textBoxRegistrationRoom";
            this.textBoxRegistrationRoom.Size = new System.Drawing.Size(230, 21);
            this.textBoxRegistrationRoom.TabIndex = 3;
            this.textBoxRegistrationRoom.TextChanged += new System.EventHandler(this.textBoxRegistrationRoom_TextChanged);
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(17, 81);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(106, 15);
            this.label65.TabIndex = 16;
            this.label65.Text = "Номер квартиры";
            // 
            // textBoxRegistrationFlat
            // 
            this.textBoxRegistrationFlat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationFlat.Location = new System.Drawing.Point(164, 78);
            this.textBoxRegistrationFlat.MaxLength = 15;
            this.textBoxRegistrationFlat.Name = "textBoxRegistrationFlat";
            this.textBoxRegistrationFlat.Size = new System.Drawing.Size(230, 21);
            this.textBoxRegistrationFlat.TabIndex = 2;
            this.textBoxRegistrationFlat.TextChanged += new System.EventHandler(this.textBoxRegistrationFlat_TextChanged);
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(17, 24);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(43, 15);
            this.label63.TabIndex = 12;
            this.label63.Text = "Улица";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(17, 53);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(79, 15);
            this.label64.TabIndex = 13;
            this.label64.Text = "Номер дома";
            // 
            // comboBoxRegistrationStreet
            // 
            this.comboBoxRegistrationStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRegistrationStreet.FormattingEnabled = true;
            this.comboBoxRegistrationStreet.Location = new System.Drawing.Point(164, 20);
            this.comboBoxRegistrationStreet.Name = "comboBoxRegistrationStreet";
            this.comboBoxRegistrationStreet.Size = new System.Drawing.Size(230, 23);
            this.comboBoxRegistrationStreet.TabIndex = 0;
            this.comboBoxRegistrationStreet.DropDownClosed += new System.EventHandler(this.comboBoxRegistrationStreet_DropDownClosed);
            this.comboBoxRegistrationStreet.SelectedValueChanged += new System.EventHandler(this.comboBoxRegistrationStreet_SelectedValueChanged);
            this.comboBoxRegistrationStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxRegistrationStreet_KeyUp);
            this.comboBoxRegistrationStreet.Leave += new System.EventHandler(this.comboBoxRegistrationStreet_Leave);
            // 
            // textBoxRegistrationHouse
            // 
            this.textBoxRegistrationHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationHouse.Location = new System.Drawing.Point(164, 50);
            this.textBoxRegistrationHouse.MaxLength = 10;
            this.textBoxRegistrationHouse.Name = "textBoxRegistrationHouse";
            this.textBoxRegistrationHouse.Size = new System.Drawing.Size(230, 21);
            this.textBoxRegistrationHouse.TabIndex = 1;
            this.textBoxRegistrationHouse.TextChanged += new System.EventHandler(this.textBoxRegistrationHouse_TextChanged);
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.dateTimePickerExcludeDate);
            this.groupBox26.Controls.Add(this.label2);
            this.groupBox26.Controls.Add(this.dateTimePickerIncludeDate);
            this.groupBox26.Controls.Add(this.label1);
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
            this.groupBox26.Location = new System.Drawing.Point(409, 3);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(401, 214);
            this.groupBox26.TabIndex = 2;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Документ, удостоверяющий личность";
            // 
            // dateTimePickerExcludeDate
            // 
            this.dateTimePickerExcludeDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerExcludeDate.Location = new System.Drawing.Point(164, 184);
            this.dateTimePickerExcludeDate.Name = "dateTimePickerExcludeDate";
            this.dateTimePickerExcludeDate.ShowCheckBox = true;
            this.dateTimePickerExcludeDate.Size = new System.Drawing.Size(231, 21);
            this.dateTimePickerExcludeDate.TabIndex = 41;
            this.dateTimePickerExcludeDate.ValueChanged += new System.EventHandler(this.dateTimePickerExcludeDate_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 15);
            this.label2.TabIndex = 42;
            this.label2.Text = "Дата исключения";
            // 
            // dateTimePickerIncludeDate
            // 
            this.dateTimePickerIncludeDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIncludeDate.Location = new System.Drawing.Point(164, 157);
            this.dateTimePickerIncludeDate.Name = "dateTimePickerIncludeDate";
            this.dateTimePickerIncludeDate.ShowCheckBox = true;
            this.dateTimePickerIncludeDate.Size = new System.Drawing.Size(231, 21);
            this.dateTimePickerIncludeDate.TabIndex = 39;
            this.dateTimePickerIncludeDate.ValueChanged += new System.EventHandler(this.dateTimePickerIncludeDate_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 15);
            this.label1.TabIndex = 40;
            this.label1.Text = "Дата включения";
            // 
            // comboBoxIssuedBy
            // 
            this.comboBoxIssuedBy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIssuedBy.FormattingEnabled = true;
            this.comboBoxIssuedBy.Location = new System.Drawing.Point(164, 128);
            this.comboBoxIssuedBy.Name = "comboBoxIssuedBy";
            this.comboBoxIssuedBy.Size = new System.Drawing.Size(231, 23);
            this.comboBoxIssuedBy.TabIndex = 4;
            this.comboBoxIssuedBy.DropDownClosed += new System.EventHandler(this.comboBoxIssuedBy_DropDownClosed);
            this.comboBoxIssuedBy.SelectedValueChanged += new System.EventHandler(this.comboBoxIssuedBy_SelectedValueChanged);
            this.comboBoxIssuedBy.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxIssuedBy_KeyUp);
            this.comboBoxIssuedBy.Leave += new System.EventHandler(this.comboBoxIssuedBy_Leave);
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(17, 132);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(71, 15);
            this.label62.TabIndex = 38;
            this.label62.Text = "Кем выдан";
            // 
            // dateTimePickerDateOfDocumentIssue
            // 
            this.dateTimePickerDateOfDocumentIssue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfDocumentIssue.Location = new System.Drawing.Point(164, 101);
            this.dateTimePickerDateOfDocumentIssue.Name = "dateTimePickerDateOfDocumentIssue";
            this.dateTimePickerDateOfDocumentIssue.ShowCheckBox = true;
            this.dateTimePickerDateOfDocumentIssue.Size = new System.Drawing.Size(231, 21);
            this.dateTimePickerDateOfDocumentIssue.TabIndex = 3;
            this.dateTimePickerDateOfDocumentIssue.ValueChanged += new System.EventHandler(this.dateTimePickerDateOfDocumentIssue_ValueChanged);
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(17, 104);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(83, 15);
            this.label61.TabIndex = 36;
            this.label61.Text = "Дата выдачи";
            // 
            // textBoxDocumentNumber
            // 
            this.textBoxDocumentNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocumentNumber.Location = new System.Drawing.Point(164, 74);
            this.textBoxDocumentNumber.MaxLength = 8;
            this.textBoxDocumentNumber.Name = "textBoxDocumentNumber";
            this.textBoxDocumentNumber.Size = new System.Drawing.Size(231, 21);
            this.textBoxDocumentNumber.TabIndex = 2;
            this.textBoxDocumentNumber.TextChanged += new System.EventHandler(this.textBoxDocumentNumber_TextChanged);
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(17, 77);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(46, 15);
            this.label60.TabIndex = 34;
            this.label60.Text = "Номер";
            // 
            // textBoxDocumentSeria
            // 
            this.textBoxDocumentSeria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocumentSeria.Location = new System.Drawing.Point(164, 47);
            this.textBoxDocumentSeria.MaxLength = 8;
            this.textBoxDocumentSeria.Name = "textBoxDocumentSeria";
            this.textBoxDocumentSeria.Size = new System.Drawing.Size(231, 21);
            this.textBoxDocumentSeria.TabIndex = 1;
            this.textBoxDocumentSeria.TextChanged += new System.EventHandler(this.textBoxDocumentSeria_TextChanged);
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(17, 50);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(43, 15);
            this.label59.TabIndex = 32;
            this.label59.Text = "Серия";
            // 
            // comboBoxDocumentType
            // 
            this.comboBoxDocumentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDocumentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDocumentType.FormattingEnabled = true;
            this.comboBoxDocumentType.Location = new System.Drawing.Point(164, 18);
            this.comboBoxDocumentType.Name = "comboBoxDocumentType";
            this.comboBoxDocumentType.Size = new System.Drawing.Size(231, 23);
            this.comboBoxDocumentType.TabIndex = 0;
            this.comboBoxDocumentType.SelectedValueChanged += new System.EventHandler(this.comboBoxDocumentType_SelectedValueChanged);
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(17, 22);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(94, 15);
            this.label58.TabIndex = 30;
            this.label58.Text = "Вид документа";
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
            this.groupBox28.Location = new System.Drawing.Point(409, 223);
            this.groupBox28.Name = "groupBox28";
            this.groupBox28.Size = new System.Drawing.Size(401, 134);
            this.groupBox28.TabIndex = 4;
            this.groupBox28.TabStop = false;
            this.groupBox28.Text = "Адрес проживания";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(17, 110);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(101, 15);
            this.label67.TabIndex = 26;
            this.label67.Text = "Номер комнаты";
            // 
            // textBoxResidenceRoom
            // 
            this.textBoxResidenceRoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceRoom.Location = new System.Drawing.Point(164, 106);
            this.textBoxResidenceRoom.MaxLength = 15;
            this.textBoxResidenceRoom.Name = "textBoxResidenceRoom";
            this.textBoxResidenceRoom.Size = new System.Drawing.Size(231, 21);
            this.textBoxResidenceRoom.TabIndex = 3;
            this.textBoxResidenceRoom.TextChanged += new System.EventHandler(this.textBoxResidenceRoom_TextChanged);
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(17, 81);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(106, 15);
            this.label68.TabIndex = 24;
            this.label68.Text = "Номер квартиры";
            // 
            // textBoxResidenceFlat
            // 
            this.textBoxResidenceFlat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceFlat.Location = new System.Drawing.Point(164, 78);
            this.textBoxResidenceFlat.MaxLength = 15;
            this.textBoxResidenceFlat.Name = "textBoxResidenceFlat";
            this.textBoxResidenceFlat.Size = new System.Drawing.Size(231, 21);
            this.textBoxResidenceFlat.TabIndex = 2;
            this.textBoxResidenceFlat.TextChanged += new System.EventHandler(this.textBoxResidenceFlat_TextChanged);
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(17, 24);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(43, 15);
            this.label69.TabIndex = 20;
            this.label69.Text = "Улица";
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Location = new System.Drawing.Point(17, 53);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(79, 15);
            this.label70.TabIndex = 21;
            this.label70.Text = "Номер дома";
            // 
            // comboBoxResidenceStreet
            // 
            this.comboBoxResidenceStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxResidenceStreet.FormattingEnabled = true;
            this.comboBoxResidenceStreet.Location = new System.Drawing.Point(164, 20);
            this.comboBoxResidenceStreet.Name = "comboBoxResidenceStreet";
            this.comboBoxResidenceStreet.Size = new System.Drawing.Size(231, 23);
            this.comboBoxResidenceStreet.TabIndex = 0;
            this.comboBoxResidenceStreet.DropDownClosed += new System.EventHandler(this.comboBoxResidenceStreet_DropDownClosed);
            this.comboBoxResidenceStreet.SelectedValueChanged += new System.EventHandler(this.comboBoxResidenceStreet_SelectedValueChanged);
            this.comboBoxResidenceStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxResidenceStreet_KeyUp);
            this.comboBoxResidenceStreet.Leave += new System.EventHandler(this.comboBoxResidenceStreet_Leave);
            // 
            // textBoxResidenceHouse
            // 
            this.textBoxResidenceHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceHouse.Location = new System.Drawing.Point(164, 50);
            this.textBoxResidenceHouse.MaxLength = 10;
            this.textBoxResidenceHouse.Name = "textBoxResidenceHouse";
            this.textBoxResidenceHouse.Size = new System.Drawing.Size(231, 21);
            this.textBoxResidenceHouse.TabIndex = 1;
            this.textBoxResidenceHouse.TextChanged += new System.EventHandler(this.textBoxResidenceHouse_TextChanged);
            // 
            // dataGridViewTenancyPersons
            // 
            this.dataGridViewTenancyPersons.AllowUserToAddRows = false;
            this.dataGridViewTenancyPersons.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyPersons.AllowUserToResizeRows = false;
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
            this.date_of_birth,
            this.id_kinship});
            this.tableLayoutPanel11.SetColumnSpan(this.dataGridViewTenancyPersons, 2);
            this.dataGridViewTenancyPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyPersons.Location = new System.Drawing.Point(3, 363);
            this.dataGridViewTenancyPersons.MultiSelect = false;
            this.dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            this.dataGridViewTenancyPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyPersons.Size = new System.Drawing.Size(807, 198);
            this.dataGridViewTenancyPersons.TabIndex = 0;
            this.dataGridViewTenancyPersons.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewTenancyPersons_DataError);
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
            this.date_of_birth.MinimumWidth = 130;
            this.date_of_birth.Name = "date_of_birth";
            this.date_of_birth.ReadOnly = true;
            // 
            // id_kinship
            // 
            this.id_kinship.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_kinship.HeaderText = "Отношение/связь";
            this.id_kinship.MinimumWidth = 100;
            this.id_kinship.Name = "id_kinship";
            // 
            // TenancyPersonsViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(660, 420);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(819, 570);
            this.Controls.Add(this.tableLayoutPanel11);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyPersonsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Участники найма №{0}";
            this.tableLayoutPanel11.ResumeLayout(false);
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox27.ResumeLayout(false);
            this.groupBox27.PerformLayout();
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            this.groupBox28.ResumeLayout(false);
            this.groupBox28.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).EndInit();
            this.ResumeLayout(false);

        }

    }
}
