using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyPersonsViewport : FormWithGridViewport
    {
        #region Models
        DataModel kinships;
        DataModel document_types;
        DataModel document_issued_by;
        DataModel kladr;
        #endregion Models

        #region Views
        BindingSource v_kinships;
        BindingSource v_document_types;
        BindingSource v_document_issued_by;
        BindingSource v_registration_street;
        BindingSource v_residence_street;
        #endregion Views

        private TenancyPersonsViewport()
            : this(null)
        {
        }

        public TenancyPersonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridViewTenancyPersons;
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
                if (((DataRowView)GeneralBindingSource[i])["id_kinship"] != DBNull.Value &&
                    Convert.ToInt32(((DataRowView)GeneralBindingSource[i])["id_kinship"], CultureInfo.InvariantCulture) == 1 &&
                    ((DataRowView)GeneralBindingSource[i])["exclude_date"] == DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else
                if (((DataRowView)GeneralBindingSource[i])["exclude_date"] != DBNull.Value)
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
            comboBoxKinship.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_kinship", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxDocumentType.DataSource = v_document_types;
            comboBoxDocumentType.ValueMember = "id_document_type";
            comboBoxDocumentType.DisplayMember = "document_type";
            comboBoxDocumentType.DataBindings.Clear();
            comboBoxDocumentType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_document_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxIssuedBy.DataSource = v_document_issued_by;
            comboBoxIssuedBy.ValueMember = "id_document_issued_by";
            comboBoxIssuedBy.DisplayMember = "document_issued_by";
            comboBoxIssuedBy.DataBindings.Clear();
            comboBoxIssuedBy.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_document_issued_by", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxRegistrationStreet.DataSource = v_registration_street;
            comboBoxRegistrationStreet.ValueMember = "id_street";
            comboBoxRegistrationStreet.DisplayMember = "street_name";
            comboBoxRegistrationStreet.DataBindings.Clear();
            comboBoxRegistrationStreet.DataBindings.Add("SelectedValue", GeneralBindingSource, "registration_id_street", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxResidenceStreet.DataSource = v_residence_street;
            comboBoxResidenceStreet.ValueMember = "id_street";
            comboBoxResidenceStreet.DisplayMember = "street_name";
            comboBoxResidenceStreet.DataBindings.Clear();
            comboBoxResidenceStreet.DataBindings.Add("SelectedValue", GeneralBindingSource, "residence_id_street", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxSurname.DataBindings.Clear();
            textBoxSurname.DataBindings.Add("Text", GeneralBindingSource, "surname", true, DataSourceUpdateMode.Never, "");
            textBoxName.DataBindings.Clear();
            textBoxName.DataBindings.Add("Text", GeneralBindingSource, "name", true, DataSourceUpdateMode.Never, "");
            textBoxPatronymic.DataBindings.Clear();
            textBoxPatronymic.DataBindings.Add("Text", GeneralBindingSource, "patronymic", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDateOfBirth.DataBindings.Clear();
            dateTimePickerDateOfBirth.DataBindings.Add("Value", GeneralBindingSource, "date_of_birth", true, DataSourceUpdateMode.Never, null);
            textBoxPersonalAccount.DataBindings.Clear();
            textBoxPersonalAccount.DataBindings.Add("Text", GeneralBindingSource, "personal_account", true, DataSourceUpdateMode.Never, "");
            textBoxDocumentSeria.DataBindings.Clear();
            textBoxDocumentSeria.DataBindings.Add("Text", GeneralBindingSource, "document_seria", true, DataSourceUpdateMode.Never, "");
            textBoxDocumentNumber.DataBindings.Clear();
            textBoxDocumentNumber.DataBindings.Add("Text", GeneralBindingSource, "document_num", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDateOfDocumentIssue.DataBindings.Clear();
            dateTimePickerDateOfDocumentIssue.DataBindings.Add("Value", GeneralBindingSource, "date_of_document_issue", true, DataSourceUpdateMode.Never, null);
            textBoxRegistrationHouse.DataBindings.Clear();
            textBoxRegistrationHouse.DataBindings.Add("Text", GeneralBindingSource, "registration_house", true, DataSourceUpdateMode.Never, "");
            textBoxRegistrationFlat.DataBindings.Clear();
            textBoxRegistrationFlat.DataBindings.Add("Text", GeneralBindingSource, "registration_flat", true, DataSourceUpdateMode.Never, "");
            textBoxRegistrationRoom.DataBindings.Clear();
            textBoxRegistrationRoom.DataBindings.Add("Text", GeneralBindingSource, "registration_room", true, DataSourceUpdateMode.Never, "");
            textBoxResidenceHouse.DataBindings.Clear();
            textBoxResidenceHouse.DataBindings.Add("Text", GeneralBindingSource, "residence_house", true, DataSourceUpdateMode.Never, "");
            textBoxResidenceFlat.DataBindings.Clear();
            textBoxResidenceFlat.DataBindings.Add("Text", GeneralBindingSource, "residence_flat", true, DataSourceUpdateMode.Never, "");
            textBoxResidenceRoom.DataBindings.Clear();
            textBoxResidenceRoom.DataBindings.Add("Text", GeneralBindingSource, "residence_room", true, DataSourceUpdateMode.Never, "");
            dateTimePickerIncludeDate.DataBindings.Clear();
            dateTimePickerIncludeDate.DataBindings.Add("Value", GeneralBindingSource, "include_date", true, DataSourceUpdateMode.Never, null);
            dateTimePickerExcludeDate.DataBindings.Clear();
            dateTimePickerExcludeDate.DataBindings.Add("Value", GeneralBindingSource, "exclude_date", true, DataSourceUpdateMode.Never, null);
            dataGridViewTenancyPersons.DataSource = GeneralBindingSource;
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
            var row = (GeneralBindingSource.Position >= 0) ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if ((GeneralBindingSource.Position >= 0) && (row["date_of_birth"] != DBNull.Value))
                dateTimePickerDateOfBirth.Checked = true;
            else
            {
                dateTimePickerDateOfBirth.Value = DateTime.Now.Date;
                dateTimePickerDateOfBirth.Checked = false;
            }
            if ((GeneralBindingSource.Position >= 0) && (row["date_of_document_issue"] != DBNull.Value))
                dateTimePickerDateOfDocumentIssue.Checked = true;
            else
            {
                dateTimePickerDateOfDocumentIssue.Value = DateTime.Now.Date;
                dateTimePickerDateOfDocumentIssue.Checked = false;
            }
            if ((GeneralBindingSource.Position >= 0) && (row["include_date"] != DBNull.Value))
                dateTimePickerIncludeDate.Checked = true;
            else
            {
                dateTimePickerIncludeDate.Value = DateTime.Now.Date;
                dateTimePickerIncludeDate.Checked = false;
            }
            if ((GeneralBindingSource.Position >= 0) && (row["exclude_date"] != DBNull.Value))
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

        protected override bool ChangeViewportStateTo(ViewportState state)
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
                            if (GeneralDataModel.EditingNewRecord)
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
            var Position = GeneralBindingSource.Find("id_person", id);
            is_editable = false;
            if (Position > 0)
               GeneralBindingSource.Position = Position;
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

        protected override Entity EntityFromViewport()
        {
            var tenancyPerson = new TenancyPerson();
            if (GeneralBindingSource.Position == -1)
                tenancyPerson.IdPerson = null;
            else
                tenancyPerson.IdPerson = ViewportHelper.ValueOrNull<int>((DataRowView)GeneralBindingSource[GeneralBindingSource.Position], "id_person");
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

        protected override Entity EntityFromView()
        {
            var tenancyPerson = new TenancyPerson();
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
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
                for (var i = 0; i < GeneralBindingSource.Count; i++)
                {
                    if (((DataRowView)GeneralBindingSource[i])["id_kinship"] != DBNull.Value &&
                        (Convert.ToInt32(((DataRowView)GeneralBindingSource[i])["id_kinship"], CultureInfo.InvariantCulture) == 1) &&
                        (((DataRowView)GeneralBindingSource[i])["exclude_date"] == DBNull.Value) &&
                        (Convert.ToInt32(((DataRowView)GeneralBindingSource[i])["id_person"], CultureInfo.InvariantCulture) != tenancyPerson.IdPerson))
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridViewTenancyPersons.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.TenancyPersonsDataModel);
            kinships = DataModel.GetInstance(DataModelType.KinshipsDataModel);
            document_types = DataModel.GetInstance(DataModelType.DocumentTypesDataModel);
            document_issued_by = DataModel.GetInstance(DataModelType.DocumentsIssuedByDataModel);
            kladr = DataModel.GetInstance(DataModelType.KladrStreetsDataModel); 

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            kinships.Select();
            document_types.Select();
            document_issued_by.Select();
            kladr.Select();

            var ds = DataModel.DataSet;

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

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataMember = "tenancy_persons";
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = ds;

            DataBind();

            GeneralDataModel.Select().RowDeleted += TenancyPersonsViewport_RowDeleted;
            GeneralDataModel.Select().RowChanged += TenancyPersonsViewport_RowChanged;
            is_editable = true;
            DataChangeHandlersInit();
            if (GeneralBindingSource.Count == 0)
                InsertRecord();
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
            dataGridViewTenancyPersons.Enabled = false;
            textBoxSurname.Focus();
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
            var tenancyPerson = (TenancyPerson) EntityFromView();
            GeneralBindingSource.AddNew();
            dataGridViewTenancyPersons.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromTenancyPerson(tenancyPerson);
            dateTimePickerDateOfBirth.Checked = (tenancyPerson.DateOfBirth != null);
            dateTimePickerDateOfDocumentIssue.Checked = (tenancyPerson.DateOfDocumentIssue != null);
            textBoxSurname.Focus();
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите этого участника договора?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_person"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                is_editable = true;
                RedrawDataGridRows();
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports(); 
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
            if (GeneralBindingSource.Count > 0)
                viewport.LocatePersonBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_person"] as int?) ?? -1);
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
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        is_editable = false; 
                        dataGridViewTenancyPersons.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        RedrawDataGridRows();
                        if (GeneralBindingSource.Position != -1)
                            dataGridViewTenancyPersons.Rows[GeneralBindingSource.Position].Selected = true;
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
                var idDocument = DataModel.GetInstance(DataModelType.DocumentsIssuedByDataModel).Insert(document);
                if (idDocument == -1) return;
                document.IdDocumentIssuedBy = idDocument;
                DataModel.GetInstance(DataModelType.DocumentsIssuedByDataModel).Select().Rows.
                    Add(document.IdDocumentIssuedBy, document.DocumentIssuedByName);
                comboBoxIssuedBy.SelectedValue = document.IdDocumentIssuedBy;
            }
            var tenancyPerson = (TenancyPerson) EntityFromViewport();
            if (!ValidateTenancyPerson(tenancyPerson))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                case ViewportState.NewRowState:
                    var id_person = GeneralDataModel.Insert(tenancyPerson);
                    if (id_person == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    tenancyPerson.IdPerson = id_person;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    FillRowFromTenancyPerson(tenancyPerson, newRow);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancyPerson.IdPerson == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись об участнике договора без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(tenancyPerson) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
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
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                GeneralDataModel.Select().RowDeleted -= TenancyPersonsViewport_RowDeleted;
                GeneralDataModel.Select().RowChanged -= TenancyPersonsViewport_RowChanged;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            GeneralDataModel.Select().RowDeleted -= TenancyPersonsViewport_RowDeleted;
            GeneralDataModel.Select().RowChanged -= TenancyPersonsViewport_RowChanged;
            Close();
        }

        void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || dataGridViewTenancyPersons.RowCount == 0)
                dataGridViewTenancyPersons.ClearSelection();
            else
            if (GeneralBindingSource.Position >= dataGridViewTenancyPersons.RowCount)
                dataGridViewTenancyPersons.Rows[dataGridViewTenancyPersons.RowCount - 1].Selected = true;
            else
            if (dataGridViewTenancyPersons.Rows[GeneralBindingSource.Position].Selected != true)
                dataGridViewTenancyPersons.Rows[GeneralBindingSource.Position].Selected = true;
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
            if (GeneralBindingSource.Position == -1)
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

        private void textBoxSNP_Leave(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;
            if (!string.IsNullOrEmpty(text))
                textBox.Text = text[0].ToString().ToUpper(CultureInfo.CurrentCulture) + text.Substring(1);
        }
    }
}
