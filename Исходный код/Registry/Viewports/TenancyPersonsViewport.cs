using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

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
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        private DataGridViewTextBoxColumn date_of_birth;
        private DataGridViewComboBoxColumn id_kinship;
        private DateTimePicker dateTimePickerExcludeDate;
        private Label label2;
        private DateTimePicker dateTimePickerIncludeDate;
        private Label label1;
        #endregion Components

        #region Models
        TenancyPersonsDataModel tenancy_persons;
        KinshipsDataModel kinships;
        DocumentTypesDataModel document_types;
        DocumentsIssuedByDataModel document_issued_by;
        KladrStreetsDataModel kladr;
        #endregion Models

        #region Views
        BindingSource v_tenancy_persons;
        BindingSource v_kinships;
        BindingSource v_document_types;
        BindingSource v_document_issued_by;
        BindingSource v_registration_street;
        BindingSource v_residence_street;
        #endregion Views

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable;

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
            DynamicFilter = tenancyPersonsViewport.DynamicFilter;
            StaticFilter = tenancyPersonsViewport.StaticFilter;
            ParentRow = tenancyPersonsViewport.ParentRow;
            ParentType = tenancyPersonsViewport.ParentType;
        }

        private void RedrawDataGridRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (var i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
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
            var row = (v_tenancy_persons.Position >= 0) ? (DataRowView)v_tenancy_persons[v_tenancy_persons.Position] : null;
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
            if ((!ContainsFocus) || (dataGridViewTenancyPersons.Focused))
                return;
            if ((v_tenancy_persons.Position != -1) && (TenancyPersonFromView() != TenancyPersonFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    dataGridViewTenancyPersons.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    dataGridViewTenancyPersons.Enabled = true;
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

        public void LocatePersonBy(int id)
        {
            var Position = v_tenancy_persons.Find("id_person", id);
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
            var tenancyPerson = new TenancyPerson();
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
            var tenancyPerson = new TenancyPerson();
            var row = (DataRowView)v_tenancy_persons[v_tenancy_persons.Position];
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
                for (var i = 0; i < v_tenancy_persons.Count; i++)
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
            DockAreas = DockAreas.Document;
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

            var ds = DataSetManager.DataSet;

            if ((ParentType == ParentTypeEnum.Tenancy) && (ParentRow != null))
                Text = string.Format(CultureInfo.InvariantCulture, "Участники найма №{0}", ParentRow["id_process"]);
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
            v_document_issued_by.Sort = "document_issued_by";

            v_tenancy_persons = new BindingSource();
            v_tenancy_persons.CurrentItemChanged += v_tenancy_persons_CurrentItemChanged;
            v_tenancy_persons.DataMember = "tenancy_persons";
            v_tenancy_persons.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_tenancy_persons.Filter += " AND ";
            v_tenancy_persons.Filter += DynamicFilter;
            v_tenancy_persons.DataSource = ds;

            DataBind();

            tenancy_persons.Select().RowDeleted += TenancyPersonsViewport_RowDeleted;
            tenancy_persons.Select().RowChanged += TenancyPersonsViewport_RowChanged;
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
            textBoxSurname.Focus();
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
            var tenancyPerson = TenancyPersonFromView();
            v_tenancy_persons.AddNew();
            dataGridViewTenancyPersons.Enabled = false;
            tenancy_persons.EditingNewRecord = true;
            ViewportFromTenancyPerson(tenancyPerson);
            dateTimePickerDateOfBirth.Checked = (tenancyPerson.DateOfBirth != null);
            dateTimePickerDateOfDocumentIssue.Checked = (tenancyPerson.DateOfDocumentIssue != null);
            textBoxSurname.Focus();
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
                if (CalcDataModelPremisesTenanciesInfo.HasInstance())
                    CalcDataModelPremisesTenanciesInfo.GetInstance().Refresh(EntityType.Unknown, null, true);
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new TenancyPersonsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancy_persons.Count > 0)
                viewport.LocatePersonBy((((DataRowView)v_tenancy_persons[v_tenancy_persons.Position])["id_person"] as int?) ?? -1);
            return viewport;
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            v_registration_street.Filter = "";
            v_residence_street.Filter = "";
            v_document_issued_by.Filter = "";
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
            if (comboBoxIssuedBy.SelectedValue == null && !string.IsNullOrEmpty(comboBoxIssuedBy.Text))
            {
                var document = new DocumentIssuedBy {DocumentIssuedByName = comboBoxIssuedBy.Text};
                var idDocument = DocumentsIssuedByDataModel.Insert(document);
                if (idDocument == -1) return;
                document.IdDocumentIssuedBy = idDocument;
                DocumentsIssuedByDataModel.GetInstance().Select().Rows.
                    Add(document.IdDocumentIssuedBy, document.DocumentIssuedByName);
                comboBoxIssuedBy.SelectedValue = document.IdDocumentIssuedBy;
            }
            var tenancyPerson = TenancyPersonFromViewport();
            if (!ValidateTenancyPerson(tenancyPerson))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                case ViewportState.NewRowState:
                    var id_person = TenancyPersonsDataModel.Insert(tenancyPerson);
                    if (id_person == -1)
                    {
                        tenancy_persons.EditingNewRecord = false;
                        return;
                    }
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
                    var row = ((DataRowView)v_tenancy_persons[v_tenancy_persons.Position]);
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
            if (CalcDataModelPremisesTenanciesInfo.HasInstance())
                CalcDataModelPremisesTenanciesInfo.GetInstance().Refresh(EntityType.Unknown, null, true);
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
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                tenancy_persons.EditingNewRecord = false;
            tenancy_persons.Select().RowDeleted -= TenancyPersonsViewport_RowDeleted;
            tenancy_persons.Select().RowChanged -= TenancyPersonsViewport_RowChanged;
            Close();
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
            CheckViewportModifications();
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
                var text = comboBoxIssuedBy.Text;
                var selectionStart = comboBoxIssuedBy.SelectionStart;
                var selectionLength = comboBoxIssuedBy.SelectionLength;
                v_document_issued_by.Filter = "document_issued_by like '%" + comboBoxIssuedBy.Text + "%'";
                comboBoxIssuedBy.Text = text;
                comboBoxIssuedBy.SelectionStart = selectionStart;
                comboBoxIssuedBy.SelectionLength = selectionLength;
            }
        }

        void comboBoxIssuedBy_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxIssuedBy.Text))
                comboBoxIssuedBy.SelectedValue = DBNull.Value;
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
                var text = comboBoxResidenceStreet.Text;
                var selectionStart = comboBoxResidenceStreet.SelectionStart;
                var selectionLength = comboBoxResidenceStreet.SelectionLength;
                v_residence_street.Filter = "street_name like '%" + comboBoxResidenceStreet.Text + "%'";
                comboBoxResidenceStreet.Text = text;
                comboBoxResidenceStreet.SelectionStart = selectionStart;
                comboBoxResidenceStreet.SelectionLength = selectionLength;
            }
        }

        void comboBoxResidenceStreet_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxResidenceStreet.Text))
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
                var text = comboBoxRegistrationStreet.Text;
                var selectionStart = comboBoxRegistrationStreet.SelectionStart;
                var selectionLength = comboBoxRegistrationStreet.SelectionLength;
                v_registration_street.Filter = "street_name like '%" + comboBoxRegistrationStreet.Text + "%'";
                comboBoxRegistrationStreet.Text = text;
                comboBoxRegistrationStreet.SelectionStart = selectionStart;
                comboBoxRegistrationStreet.SelectionLength = selectionLength;
            }
        }

        void comboBoxRegistrationStreet_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxRegistrationStreet.Text))
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

        private void textBoxSNP_Leave(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;
            if (!string.IsNullOrEmpty(text))
                textBox.Text = text[0].ToString().ToUpper(CultureInfo.CurrentCulture) + text.Substring(1);
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyPersonsViewport));
            tableLayoutPanel11 = new TableLayoutPanel();
            groupBox23 = new GroupBox();
            textBoxPersonalAccount = new TextBox();
            label81 = new Label();
            dateTimePickerDateOfBirth = new DateTimePicker();
            comboBoxKinship = new ComboBox();
            label57 = new Label();
            label56 = new Label();
            textBoxPatronymic = new TextBox();
            label55 = new Label();
            textBoxName = new TextBox();
            label54 = new Label();
            textBoxSurname = new TextBox();
            label53 = new Label();
            groupBox27 = new GroupBox();
            label66 = new Label();
            textBoxRegistrationRoom = new TextBox();
            label65 = new Label();
            textBoxRegistrationFlat = new TextBox();
            label63 = new Label();
            label64 = new Label();
            comboBoxRegistrationStreet = new ComboBox();
            textBoxRegistrationHouse = new TextBox();
            groupBox26 = new GroupBox();
            dateTimePickerExcludeDate = new DateTimePicker();
            label2 = new Label();
            dateTimePickerIncludeDate = new DateTimePicker();
            label1 = new Label();
            comboBoxIssuedBy = new ComboBox();
            label62 = new Label();
            dateTimePickerDateOfDocumentIssue = new DateTimePicker();
            label61 = new Label();
            textBoxDocumentNumber = new TextBox();
            label60 = new Label();
            textBoxDocumentSeria = new TextBox();
            label59 = new Label();
            comboBoxDocumentType = new ComboBox();
            label58 = new Label();
            groupBox28 = new GroupBox();
            label67 = new Label();
            textBoxResidenceRoom = new TextBox();
            label68 = new Label();
            textBoxResidenceFlat = new TextBox();
            label69 = new Label();
            label70 = new Label();
            comboBoxResidenceStreet = new ComboBox();
            textBoxResidenceHouse = new TextBox();
            dataGridViewTenancyPersons = new DataGridView();
            surname = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            patronymic = new DataGridViewTextBoxColumn();
            date_of_birth = new DataGridViewTextBoxColumn();
            id_kinship = new DataGridViewComboBoxColumn();
            tableLayoutPanel11.SuspendLayout();
            groupBox23.SuspendLayout();
            groupBox27.SuspendLayout();
            groupBox26.SuspendLayout();
            groupBox28.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel11
            // 
            tableLayoutPanel11.ColumnCount = 2;
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel11.Controls.Add(groupBox23, 0, 0);
            tableLayoutPanel11.Controls.Add(groupBox27, 0, 1);
            tableLayoutPanel11.Controls.Add(groupBox26, 1, 0);
            tableLayoutPanel11.Controls.Add(groupBox28, 1, 1);
            tableLayoutPanel11.Controls.Add(dataGridViewTenancyPersons, 0, 2);
            tableLayoutPanel11.Dock = DockStyle.Fill;
            tableLayoutPanel11.Location = new Point(3, 3);
            tableLayoutPanel11.Name = "tableLayoutPanel11";
            tableLayoutPanel11.RowCount = 3;
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Absolute, 220F));
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel11.Size = new Size(813, 564);
            tableLayoutPanel11.TabIndex = 0;
            // 
            // groupBox23
            // 
            groupBox23.Controls.Add(textBoxPersonalAccount);
            groupBox23.Controls.Add(label81);
            groupBox23.Controls.Add(dateTimePickerDateOfBirth);
            groupBox23.Controls.Add(comboBoxKinship);
            groupBox23.Controls.Add(label57);
            groupBox23.Controls.Add(label56);
            groupBox23.Controls.Add(textBoxPatronymic);
            groupBox23.Controls.Add(label55);
            groupBox23.Controls.Add(textBoxName);
            groupBox23.Controls.Add(label54);
            groupBox23.Controls.Add(textBoxSurname);
            groupBox23.Controls.Add(label53);
            groupBox23.Dock = DockStyle.Fill;
            groupBox23.Location = new Point(3, 3);
            groupBox23.Name = "groupBox23";
            groupBox23.Size = new Size(400, 214);
            groupBox23.TabIndex = 1;
            groupBox23.TabStop = false;
            groupBox23.Text = "Личные данные";
            // 
            // textBoxPersonalAccount
            // 
            textBoxPersonalAccount.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            textBoxPersonalAccount.Location = new Point(164, 157);
            textBoxPersonalAccount.MaxLength = 255;
            textBoxPersonalAccount.Name = "textBoxPersonalAccount";
            textBoxPersonalAccount.Size = new Size(230, 21);
            textBoxPersonalAccount.TabIndex = 29;
            textBoxPersonalAccount.TextChanged += textBoxPersonalAccount_TextChanged;
            textBoxPersonalAccount.Enter += selectAll_Enter;
            // 
            // label81
            // 
            label81.AutoSize = true;
            label81.Location = new Point(17, 160);
            label81.Name = "label81";
            label81.Size = new Size(86, 15);
            label81.TabIndex = 30;
            label81.Text = "Лицевой счет";
            // 
            // dateTimePickerDateOfBirth
            // 
            dateTimePickerDateOfBirth.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                               | AnchorStyles.Right;
            dateTimePickerDateOfBirth.Location = new Point(164, 100);
            dateTimePickerDateOfBirth.Name = "dateTimePickerDateOfBirth";
            dateTimePickerDateOfBirth.ShowCheckBox = true;
            dateTimePickerDateOfBirth.Size = new Size(230, 21);
            dateTimePickerDateOfBirth.TabIndex = 3;
            dateTimePickerDateOfBirth.ValueChanged += dateTimePickerDateOfBirth_ValueChanged;
            // 
            // comboBoxKinship
            // 
            comboBoxKinship.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                     | AnchorStyles.Right;
            comboBoxKinship.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxKinship.FormattingEnabled = true;
            comboBoxKinship.Location = new Point(164, 127);
            comboBoxKinship.Name = "comboBoxKinship";
            comboBoxKinship.Size = new Size(230, 23);
            comboBoxKinship.TabIndex = 4;
            comboBoxKinship.SelectedValueChanged += comboBoxKinship_SelectedValueChanged;
            // 
            // label57
            // 
            label57.AutoSize = true;
            label57.Location = new Point(17, 131);
            label57.Name = "label57";
            label57.Size = new Size(110, 15);
            label57.TabIndex = 28;
            label57.Text = "Отношение/связь";
            // 
            // label56
            // 
            label56.AutoSize = true;
            label56.Location = new Point(17, 103);
            label56.Name = "label56";
            label56.Size = new Size(98, 15);
            label56.TabIndex = 26;
            label56.Text = "Дата рождения";
            // 
            // textBoxPatronymic
            // 
            textBoxPatronymic.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                       | AnchorStyles.Right;
            textBoxPatronymic.Location = new Point(164, 73);
            textBoxPatronymic.MaxLength = 255;
            textBoxPatronymic.Name = "textBoxPatronymic";
            textBoxPatronymic.Size = new Size(230, 21);
            textBoxPatronymic.TabIndex = 2;
            textBoxPatronymic.TextChanged += textBoxPatronymic_TextChanged;
            textBoxPatronymic.Enter += selectAll_Enter;
            textBoxPatronymic.Leave += textBoxSNP_Leave;
            // 
            // label55
            // 
            label55.AutoSize = true;
            label55.Location = new Point(17, 76);
            label55.Name = "label55";
            label55.Size = new Size(63, 15);
            label55.TabIndex = 24;
            label55.Text = "Отчество";
            // 
            // textBoxName
            // 
            textBoxName.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                 | AnchorStyles.Right;
            textBoxName.Location = new Point(164, 46);
            textBoxName.MaxLength = 50;
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(230, 21);
            textBoxName.TabIndex = 1;
            textBoxName.TextChanged += textBoxName_TextChanged;
            textBoxName.Enter += selectAll_Enter;
            textBoxName.Leave += textBoxSNP_Leave;
            // 
            // label54
            // 
            label54.AutoSize = true;
            label54.Location = new Point(17, 49);
            label54.Name = "label54";
            label54.Size = new Size(32, 15);
            label54.TabIndex = 22;
            label54.Text = "Имя";
            // 
            // textBoxSurname
            // 
            textBoxSurname.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                    | AnchorStyles.Right;
            textBoxSurname.Location = new Point(164, 19);
            textBoxSurname.MaxLength = 50;
            textBoxSurname.Name = "textBoxSurname";
            textBoxSurname.Size = new Size(230, 21);
            textBoxSurname.TabIndex = 0;
            textBoxSurname.TextChanged += textBoxSurname_TextChanged;
            textBoxSurname.Enter += selectAll_Enter;
            textBoxSurname.Leave += textBoxSNP_Leave;
            // 
            // label53
            // 
            label53.AutoSize = true;
            label53.Location = new Point(17, 22);
            label53.Name = "label53";
            label53.Size = new Size(62, 15);
            label53.TabIndex = 20;
            label53.Text = "Фамилия";
            // 
            // groupBox27
            // 
            groupBox27.Controls.Add(label66);
            groupBox27.Controls.Add(textBoxRegistrationRoom);
            groupBox27.Controls.Add(label65);
            groupBox27.Controls.Add(textBoxRegistrationFlat);
            groupBox27.Controls.Add(label63);
            groupBox27.Controls.Add(label64);
            groupBox27.Controls.Add(comboBoxRegistrationStreet);
            groupBox27.Controls.Add(textBoxRegistrationHouse);
            groupBox27.Dock = DockStyle.Fill;
            groupBox27.Location = new Point(3, 223);
            groupBox27.Name = "groupBox27";
            groupBox27.Size = new Size(400, 134);
            groupBox27.TabIndex = 3;
            groupBox27.TabStop = false;
            groupBox27.Text = "Адрес регистрации";
            // 
            // label66
            // 
            label66.AutoSize = true;
            label66.Location = new Point(17, 109);
            label66.Name = "label66";
            label66.Size = new Size(101, 15);
            label66.TabIndex = 18;
            label66.Text = "Номер комнаты";
            // 
            // textBoxRegistrationRoom
            // 
            textBoxRegistrationRoom.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            textBoxRegistrationRoom.Location = new Point(164, 106);
            textBoxRegistrationRoom.MaxLength = 15;
            textBoxRegistrationRoom.Name = "textBoxRegistrationRoom";
            textBoxRegistrationRoom.Size = new Size(230, 21);
            textBoxRegistrationRoom.TabIndex = 3;
            textBoxRegistrationRoom.TextChanged += textBoxRegistrationRoom_TextChanged;
            textBoxRegistrationRoom.Enter += selectAll_Enter;
            // 
            // label65
            // 
            label65.AutoSize = true;
            label65.Location = new Point(17, 81);
            label65.Name = "label65";
            label65.Size = new Size(106, 15);
            label65.TabIndex = 16;
            label65.Text = "Номер квартиры";
            // 
            // textBoxRegistrationFlat
            // 
            textBoxRegistrationFlat.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            textBoxRegistrationFlat.Location = new Point(164, 78);
            textBoxRegistrationFlat.MaxLength = 15;
            textBoxRegistrationFlat.Name = "textBoxRegistrationFlat";
            textBoxRegistrationFlat.Size = new Size(230, 21);
            textBoxRegistrationFlat.TabIndex = 2;
            textBoxRegistrationFlat.TextChanged += textBoxRegistrationFlat_TextChanged;
            textBoxRegistrationFlat.Enter += selectAll_Enter;
            // 
            // label63
            // 
            label63.AutoSize = true;
            label63.Location = new Point(17, 24);
            label63.Name = "label63";
            label63.Size = new Size(43, 15);
            label63.TabIndex = 12;
            label63.Text = "Улица";
            // 
            // label64
            // 
            label64.AutoSize = true;
            label64.Location = new Point(17, 53);
            label64.Name = "label64";
            label64.Size = new Size(79, 15);
            label64.TabIndex = 13;
            label64.Text = "Номер дома";
            // 
            // comboBoxRegistrationStreet
            // 
            comboBoxRegistrationStreet.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            comboBoxRegistrationStreet.FormattingEnabled = true;
            comboBoxRegistrationStreet.Location = new Point(164, 20);
            comboBoxRegistrationStreet.Name = "comboBoxRegistrationStreet";
            comboBoxRegistrationStreet.Size = new Size(230, 23);
            comboBoxRegistrationStreet.TabIndex = 0;
            comboBoxRegistrationStreet.DropDownClosed += comboBoxRegistrationStreet_DropDownClosed;
            comboBoxRegistrationStreet.SelectedValueChanged += comboBoxRegistrationStreet_SelectedValueChanged;
            comboBoxRegistrationStreet.Enter += selectAll_Enter;
            comboBoxRegistrationStreet.KeyUp += comboBoxRegistrationStreet_KeyUp;
            comboBoxRegistrationStreet.Leave += comboBoxRegistrationStreet_Leave;
            // 
            // textBoxRegistrationHouse
            // 
            textBoxRegistrationHouse.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                              | AnchorStyles.Right;
            textBoxRegistrationHouse.Location = new Point(164, 50);
            textBoxRegistrationHouse.MaxLength = 10;
            textBoxRegistrationHouse.Name = "textBoxRegistrationHouse";
            textBoxRegistrationHouse.Size = new Size(230, 21);
            textBoxRegistrationHouse.TabIndex = 1;
            textBoxRegistrationHouse.TextChanged += textBoxRegistrationHouse_TextChanged;
            textBoxRegistrationHouse.Enter += selectAll_Enter;
            // 
            // groupBox26
            // 
            groupBox26.Controls.Add(dateTimePickerExcludeDate);
            groupBox26.Controls.Add(label2);
            groupBox26.Controls.Add(dateTimePickerIncludeDate);
            groupBox26.Controls.Add(label1);
            groupBox26.Controls.Add(comboBoxIssuedBy);
            groupBox26.Controls.Add(label62);
            groupBox26.Controls.Add(dateTimePickerDateOfDocumentIssue);
            groupBox26.Controls.Add(label61);
            groupBox26.Controls.Add(textBoxDocumentNumber);
            groupBox26.Controls.Add(label60);
            groupBox26.Controls.Add(textBoxDocumentSeria);
            groupBox26.Controls.Add(label59);
            groupBox26.Controls.Add(comboBoxDocumentType);
            groupBox26.Controls.Add(label58);
            groupBox26.Dock = DockStyle.Fill;
            groupBox26.Location = new Point(409, 3);
            groupBox26.Name = "groupBox26";
            groupBox26.Size = new Size(401, 214);
            groupBox26.TabIndex = 2;
            groupBox26.TabStop = false;
            groupBox26.Text = "Документ, удостоверяющий личность";
            // 
            // dateTimePickerExcludeDate
            // 
            dateTimePickerExcludeDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                               | AnchorStyles.Right;
            dateTimePickerExcludeDate.Location = new Point(164, 184);
            dateTimePickerExcludeDate.Name = "dateTimePickerExcludeDate";
            dateTimePickerExcludeDate.ShowCheckBox = true;
            dateTimePickerExcludeDate.Size = new Size(231, 21);
            dateTimePickerExcludeDate.TabIndex = 41;
            dateTimePickerExcludeDate.ValueChanged += dateTimePickerExcludeDate_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 187);
            label2.Name = "label2";
            label2.Size = new Size(109, 15);
            label2.TabIndex = 42;
            label2.Text = "Дата исключения";
            // 
            // dateTimePickerIncludeDate
            // 
            dateTimePickerIncludeDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                               | AnchorStyles.Right;
            dateTimePickerIncludeDate.Location = new Point(164, 157);
            dateTimePickerIncludeDate.Name = "dateTimePickerIncludeDate";
            dateTimePickerIncludeDate.ShowCheckBox = true;
            dateTimePickerIncludeDate.Size = new Size(231, 21);
            dateTimePickerIncludeDate.TabIndex = 39;
            dateTimePickerIncludeDate.ValueChanged += dateTimePickerIncludeDate_ValueChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 160);
            label1.Name = "label1";
            label1.Size = new Size(103, 15);
            label1.TabIndex = 40;
            label1.Text = "Дата включения";
            // 
            // comboBoxIssuedBy
            // 
            comboBoxIssuedBy.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                      | AnchorStyles.Right;
            comboBoxIssuedBy.FormattingEnabled = true;
            comboBoxIssuedBy.Location = new Point(164, 128);
            comboBoxIssuedBy.Name = "comboBoxIssuedBy";
            comboBoxIssuedBy.Size = new Size(231, 23);
            comboBoxIssuedBy.TabIndex = 4;
            comboBoxIssuedBy.DropDownClosed += comboBoxIssuedBy_DropDownClosed;
            comboBoxIssuedBy.SelectedValueChanged += comboBoxIssuedBy_SelectedValueChanged;
            comboBoxIssuedBy.Enter += selectAll_Enter;
            comboBoxIssuedBy.KeyUp += comboBoxIssuedBy_KeyUp;
            comboBoxIssuedBy.Leave += comboBoxIssuedBy_Leave;
            // 
            // label62
            // 
            label62.AutoSize = true;
            label62.Location = new Point(17, 132);
            label62.Name = "label62";
            label62.Size = new Size(71, 15);
            label62.TabIndex = 38;
            label62.Text = "Кем выдан";
            // 
            // dateTimePickerDateOfDocumentIssue
            // 
            dateTimePickerDateOfDocumentIssue.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                       | AnchorStyles.Right;
            dateTimePickerDateOfDocumentIssue.Location = new Point(164, 101);
            dateTimePickerDateOfDocumentIssue.Name = "dateTimePickerDateOfDocumentIssue";
            dateTimePickerDateOfDocumentIssue.ShowCheckBox = true;
            dateTimePickerDateOfDocumentIssue.Size = new Size(231, 21);
            dateTimePickerDateOfDocumentIssue.TabIndex = 3;
            dateTimePickerDateOfDocumentIssue.ValueChanged += dateTimePickerDateOfDocumentIssue_ValueChanged;
            // 
            // label61
            // 
            label61.AutoSize = true;
            label61.Location = new Point(17, 104);
            label61.Name = "label61";
            label61.Size = new Size(83, 15);
            label61.TabIndex = 36;
            label61.Text = "Дата выдачи";
            // 
            // textBoxDocumentNumber
            // 
            textBoxDocumentNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            textBoxDocumentNumber.Location = new Point(164, 74);
            textBoxDocumentNumber.MaxLength = 8;
            textBoxDocumentNumber.Name = "textBoxDocumentNumber";
            textBoxDocumentNumber.Size = new Size(231, 21);
            textBoxDocumentNumber.TabIndex = 2;
            textBoxDocumentNumber.TextChanged += textBoxDocumentNumber_TextChanged;
            textBoxDocumentNumber.Enter += selectAll_Enter;
            // 
            // label60
            // 
            label60.AutoSize = true;
            label60.Location = new Point(17, 77);
            label60.Name = "label60";
            label60.Size = new Size(46, 15);
            label60.TabIndex = 34;
            label60.Text = "Номер";
            // 
            // textBoxDocumentSeria
            // 
            textBoxDocumentSeria.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                          | AnchorStyles.Right;
            textBoxDocumentSeria.Location = new Point(164, 47);
            textBoxDocumentSeria.MaxLength = 8;
            textBoxDocumentSeria.Name = "textBoxDocumentSeria";
            textBoxDocumentSeria.Size = new Size(231, 21);
            textBoxDocumentSeria.TabIndex = 1;
            textBoxDocumentSeria.TextChanged += textBoxDocumentSeria_TextChanged;
            textBoxDocumentSeria.Enter += selectAll_Enter;
            // 
            // label59
            // 
            label59.AutoSize = true;
            label59.Location = new Point(17, 50);
            label59.Name = "label59";
            label59.Size = new Size(43, 15);
            label59.TabIndex = 32;
            label59.Text = "Серия";
            // 
            // comboBoxDocumentType
            // 
            comboBoxDocumentType.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                          | AnchorStyles.Right;
            comboBoxDocumentType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDocumentType.FormattingEnabled = true;
            comboBoxDocumentType.Location = new Point(164, 18);
            comboBoxDocumentType.Name = "comboBoxDocumentType";
            comboBoxDocumentType.Size = new Size(231, 23);
            comboBoxDocumentType.TabIndex = 0;
            comboBoxDocumentType.SelectedValueChanged += comboBoxDocumentType_SelectedValueChanged;
            // 
            // label58
            // 
            label58.AutoSize = true;
            label58.Location = new Point(17, 22);
            label58.Name = "label58";
            label58.Size = new Size(94, 15);
            label58.TabIndex = 30;
            label58.Text = "Вид документа";
            // 
            // groupBox28
            // 
            groupBox28.Controls.Add(label67);
            groupBox28.Controls.Add(textBoxResidenceRoom);
            groupBox28.Controls.Add(label68);
            groupBox28.Controls.Add(textBoxResidenceFlat);
            groupBox28.Controls.Add(label69);
            groupBox28.Controls.Add(label70);
            groupBox28.Controls.Add(comboBoxResidenceStreet);
            groupBox28.Controls.Add(textBoxResidenceHouse);
            groupBox28.Dock = DockStyle.Fill;
            groupBox28.Location = new Point(409, 223);
            groupBox28.Name = "groupBox28";
            groupBox28.Size = new Size(401, 134);
            groupBox28.TabIndex = 4;
            groupBox28.TabStop = false;
            groupBox28.Text = "Адрес проживания";
            // 
            // label67
            // 
            label67.AutoSize = true;
            label67.Location = new Point(17, 110);
            label67.Name = "label67";
            label67.Size = new Size(101, 15);
            label67.TabIndex = 26;
            label67.Text = "Номер комнаты";
            // 
            // textBoxResidenceRoom
            // 
            textBoxResidenceRoom.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                          | AnchorStyles.Right;
            textBoxResidenceRoom.Location = new Point(164, 106);
            textBoxResidenceRoom.MaxLength = 15;
            textBoxResidenceRoom.Name = "textBoxResidenceRoom";
            textBoxResidenceRoom.Size = new Size(231, 21);
            textBoxResidenceRoom.TabIndex = 3;
            textBoxResidenceRoom.TextChanged += textBoxResidenceRoom_TextChanged;
            textBoxResidenceRoom.Enter += selectAll_Enter;
            // 
            // label68
            // 
            label68.AutoSize = true;
            label68.Location = new Point(17, 81);
            label68.Name = "label68";
            label68.Size = new Size(106, 15);
            label68.TabIndex = 24;
            label68.Text = "Номер квартиры";
            // 
            // textBoxResidenceFlat
            // 
            textBoxResidenceFlat.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                          | AnchorStyles.Right;
            textBoxResidenceFlat.Location = new Point(164, 78);
            textBoxResidenceFlat.MaxLength = 15;
            textBoxResidenceFlat.Name = "textBoxResidenceFlat";
            textBoxResidenceFlat.Size = new Size(231, 21);
            textBoxResidenceFlat.TabIndex = 2;
            textBoxResidenceFlat.TextChanged += textBoxResidenceFlat_TextChanged;
            textBoxResidenceFlat.Enter += selectAll_Enter;
            // 
            // label69
            // 
            label69.AutoSize = true;
            label69.Location = new Point(17, 24);
            label69.Name = "label69";
            label69.Size = new Size(43, 15);
            label69.TabIndex = 20;
            label69.Text = "Улица";
            // 
            // label70
            // 
            label70.AutoSize = true;
            label70.Location = new Point(17, 53);
            label70.Name = "label70";
            label70.Size = new Size(79, 15);
            label70.TabIndex = 21;
            label70.Text = "Номер дома";
            // 
            // comboBoxResidenceStreet
            // 
            comboBoxResidenceStreet.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            comboBoxResidenceStreet.FormattingEnabled = true;
            comboBoxResidenceStreet.Location = new Point(164, 20);
            comboBoxResidenceStreet.Name = "comboBoxResidenceStreet";
            comboBoxResidenceStreet.Size = new Size(231, 23);
            comboBoxResidenceStreet.TabIndex = 0;
            comboBoxResidenceStreet.DropDownClosed += comboBoxResidenceStreet_DropDownClosed;
            comboBoxResidenceStreet.SelectedValueChanged += comboBoxResidenceStreet_SelectedValueChanged;
            comboBoxResidenceStreet.Enter += selectAll_Enter;
            comboBoxResidenceStreet.KeyUp += comboBoxResidenceStreet_KeyUp;
            comboBoxResidenceStreet.Leave += comboBoxResidenceStreet_Leave;
            // 
            // textBoxResidenceHouse
            // 
            textBoxResidenceHouse.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            textBoxResidenceHouse.Location = new Point(164, 50);
            textBoxResidenceHouse.MaxLength = 10;
            textBoxResidenceHouse.Name = "textBoxResidenceHouse";
            textBoxResidenceHouse.Size = new Size(231, 21);
            textBoxResidenceHouse.TabIndex = 1;
            textBoxResidenceHouse.TextChanged += textBoxResidenceHouse_TextChanged;
            textBoxResidenceHouse.Enter += selectAll_Enter;
            // 
            // dataGridViewTenancyPersons
            // 
            dataGridViewTenancyPersons.AllowUserToAddRows = false;
            dataGridViewTenancyPersons.AllowUserToDeleteRows = false;
            dataGridViewTenancyPersons.AllowUserToResizeRows = false;
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
            dataGridViewTenancyPersons.Columns.AddRange(surname, name, patronymic, date_of_birth, id_kinship);
            tableLayoutPanel11.SetColumnSpan(dataGridViewTenancyPersons, 2);
            dataGridViewTenancyPersons.Dock = DockStyle.Fill;
            dataGridViewTenancyPersons.Location = new Point(3, 363);
            dataGridViewTenancyPersons.MultiSelect = false;
            dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            dataGridViewTenancyPersons.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyPersons.Size = new Size(807, 198);
            dataGridViewTenancyPersons.TabIndex = 0;
            dataGridViewTenancyPersons.DataError += dataGridViewTenancyPersons_DataError;
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
            date_of_birth.MinimumWidth = 130;
            date_of_birth.Name = "date_of_birth";
            date_of_birth.ReadOnly = true;
            // 
            // id_kinship
            // 
            id_kinship.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_kinship.HeaderText = "Отношение/связь";
            id_kinship.MinimumWidth = 100;
            id_kinship.Name = "id_kinship";
            // 
            // TenancyPersonsViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(660, 420);
            BackColor = Color.White;
            ClientSize = new Size(819, 570);
            Controls.Add(tableLayoutPanel11);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyPersonsViewport";
            Padding = new Padding(3);
            Text = "Участники найма №{0}";
            tableLayoutPanel11.ResumeLayout(false);
            groupBox23.ResumeLayout(false);
            groupBox23.PerformLayout();
            groupBox27.ResumeLayout(false);
            groupBox27.PerformLayout();
            groupBox26.ResumeLayout(false);
            groupBox26.PerformLayout();
            groupBox28.ResumeLayout(false);
            groupBox28.PerformLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).EndInit();
            ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
