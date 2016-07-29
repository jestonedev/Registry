using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyPersonsViewport : FormWithGridViewport
    {
        #region Models

        private DataModel _kinships;
        private DataModel _documentTypes;
        private DataModel _documentIssuedBy;
        private DataModel _kladr;
        #endregion Models

        #region Views

        private BindingSource _vKinships;
        private BindingSource _vDocumentTypes;
        private BindingSource _vDocumentIssuedBy;
        private BindingSource _vRegistrationStreet;
        private BindingSource _vResidenceStreet;
        #endregion Views

        private TenancyPersonsViewport()
            : this(null, null)
        {
        }

        public TenancyPersonsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridViewTenancyPersons;
        }

        private void RedrawDataGridRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (var i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
            {
                if (GeneralBindingSource.Count <= i) break;
                if (((DataRowView) GeneralBindingSource[i])["id_kinship"] != DBNull.Value &&
                    Convert.ToInt32(((DataRowView) GeneralBindingSource[i])["id_kinship"], CultureInfo.InvariantCulture) ==
                    1 &&
                    ((DataRowView) GeneralBindingSource[i])["exclude_date"] == DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else if (((DataRowView) GeneralBindingSource[i])["exclude_date"] != DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                else
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void DataBind()
        {
            comboBoxKinship.DataSource = _vKinships;
            comboBoxKinship.ValueMember = "id_kinship";
            comboBoxKinship.DisplayMember = "kinship";
            comboBoxKinship.DataBindings.Clear();
            comboBoxKinship.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_kinship", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxDocumentType.DataSource = _vDocumentTypes;
            comboBoxDocumentType.ValueMember = "id_document_type";
            comboBoxDocumentType.DisplayMember = "document_type";
            comboBoxDocumentType.DataBindings.Clear();
            comboBoxDocumentType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_document_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxIssuedBy.DataSource = _vDocumentIssuedBy;
            comboBoxIssuedBy.ValueMember = "id_document_issued_by";
            comboBoxIssuedBy.DisplayMember = "document_issued_by";
            comboBoxIssuedBy.DataBindings.Clear();
            comboBoxIssuedBy.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_document_issued_by", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxRegistrationStreet.DataSource = _vRegistrationStreet;
            comboBoxRegistrationStreet.ValueMember = "id_street";
            comboBoxRegistrationStreet.DisplayMember = "street_name";
            comboBoxRegistrationStreet.DataBindings.Clear();
            comboBoxRegistrationStreet.DataBindings.Add("SelectedValue", GeneralBindingSource, "registration_id_street", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxResidenceStreet.DataSource = _vResidenceStreet;
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
            id_kinship.DataSource = _vKinships;
            id_kinship.DisplayMember = "kinship";
            id_kinship.ValueMember = "id_kinship";
            id_kinship.DataPropertyName = "id_kinship";
            registration_date.DataPropertyName = "registration_date";
        }

        private void UnbindedCheckBoxesUpdate()
        {
            if (GeneralBindingSource.Count == 0) return;
            var row = GeneralBindingSource.Position >= 0 ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if ((GeneralBindingSource.Position >= 0) && (row != null) && (row["date_of_birth"] != DBNull.Value))
                dateTimePickerDateOfBirth.Checked = true;
            else
            {
                dateTimePickerDateOfBirth.Value = DateTime.Now.Date;
                dateTimePickerDateOfBirth.Checked = false;
            }
            if ((GeneralBindingSource.Position >= 0) && (row != null) && (row["date_of_document_issue"] != DBNull.Value))
                dateTimePickerDateOfDocumentIssue.Checked = true;
            else
            {
                dateTimePickerDateOfDocumentIssue.Value = DateTime.Now.Date;
                dateTimePickerDateOfDocumentIssue.Checked = false;
            }
            if ((GeneralBindingSource.Position >= 0) && (row != null) && (row["include_date"] != DBNull.Value))
                dateTimePickerIncludeDate.Checked = true;
            else
            {
                dateTimePickerIncludeDate.Value = DateTime.Now.Date;
                dateTimePickerIncludeDate.Checked = false;
            }
            if ((GeneralBindingSource.Position >= 0) && (row != null) && (row["exclude_date"] != DBNull.Value))
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
            if (AccessControl.HasPrivelege(Priveleges.TenancyWrite)) return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private void ViewportFromTenancyPerson(TenancyPerson tenancyPerson)
        {
            comboBoxKinship.SelectedValue = ViewportHelper.ValueOrDbNull(tenancyPerson.IdKinship);
            comboBoxDocumentType.SelectedValue = ViewportHelper.ValueOrDbNull(tenancyPerson.IdDocumentType);
            comboBoxIssuedBy.SelectedValue = ViewportHelper.ValueOrDbNull(tenancyPerson.IdDocumentIssuedBy);
            comboBoxRegistrationStreet.SelectedValue = ViewportHelper.ValueOrDbNull(tenancyPerson.RegistrationIdStreet);
            comboBoxResidenceStreet.SelectedValue = ViewportHelper.ValueOrDbNull(tenancyPerson.ResidenceIdStreet);
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
            var row = (DataRowView) GeneralBindingSource[GeneralBindingSource.Position];
            var tenancyPerson = new TenancyPerson
            {
                IdPerson = GeneralBindingSource.Position == -1 ? null : ViewportHelper.ValueOrNull<int>(row, "id_person")
            };
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
            tenancyPerson.RegistrationDate = ViewportHelper.ValueOrNull<DateTime>(row, "registration_date");
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
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return EntityConverter<TenancyPerson>.FromRow(row);
        }

        private bool ValidateTenancyPerson(TenancyPerson tenancyPerson)
        {
            if (tenancyPerson.Surname == null)
            {
                MessageBox.Show(@"Необходимо указать фамилию участника найма", @"Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxSurname.Focus();
                return false;
            }
            if (tenancyPerson.Name == null)
            {
                MessageBox.Show(@"Необходимо указать имя участника найма", @"Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxName.Focus();
                return false;
            }
            if (tenancyPerson.IdKinship == null)
            {
                MessageBox.Show(@"Необходимо выбрать родственную связь", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxKinship.Focus();
                return false;
            }
            if (tenancyPerson.IdKinship == 1)
            {
                for (var i = 0; i < GeneralBindingSource.Count; i++)
                {
                    var row = (DataRowView) GeneralBindingSource[i];
                    if (row["id_kinship"] == DBNull.Value || ((int)row["id_kinship"] != 1) ||
                       (row["exclude_date"] != DBNull.Value) || ((int)row["id_person"] == tenancyPerson.IdPerson)) continue;
                    MessageBox.Show(@"В процессе найма может быть только один наниматель", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    comboBoxKinship.Focus();
                    return false;
                }
            }
            if (tenancyPerson.IdDocumentType == null)
            {
                MessageBox.Show(@"Необходимо выбрать вид документа, удостоверяющего личность", @"Ошибка",
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
            GeneralDataModel = EntityDataModel<TenancyPerson>.GetInstance();
            _kinships = DataModel.GetInstance<KinshipsDataModel>();
            _documentTypes = DataModel.GetInstance<DocumentTypesDataModel>();
            _documentIssuedBy = DataModel.GetInstance<EntityDataModel<DocumentIssuedBy>>();
            _kladr = DataModel.GetInstance<KladrStreetsDataModel>(); 

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            _kinships.Select();
            _documentTypes.Select();
            _documentIssuedBy.Select();
            _kladr.Select();

            var ds = DataModel.DataSet;

            if ((ParentType == ParentTypeEnum.Tenancy) && (ParentRow != null))
                Text = string.Format(CultureInfo.InvariantCulture, "Участники найма №{0}", ParentRow["id_process"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            _vKinships = new BindingSource
            {
                DataMember = "kinships",
                DataSource = ds
            };

            _vRegistrationStreet = new BindingSource
            {
                DataMember = "kladr",
                DataSource = ds
            };

            _vResidenceStreet = new BindingSource
            {
                DataMember = "kladr",
                DataSource = ds
            };

            _vDocumentTypes = new BindingSource
            {
                DataMember = "document_types",
                DataSource = ds
            };

            _vDocumentIssuedBy = new BindingSource
            {
                DataMember = "documents_issued_by",
                DataSource = ds,
                Sort = "document_issued_by"
            };

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);
            GeneralBindingSource.DataMember = "tenancy_persons";
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = ds;

            DataBind();

            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", TenancyPersonsViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", TenancyPersonsViewport_RowChanged);
            IsEditable = true;
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
            IsEditable = false;
            GeneralBindingSource.AddNew();
            dataGridViewTenancyPersons.Enabled = false;
            textBoxSurname.Focus();
            IsEditable = true;
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
            IsEditable = false;
            var tenancyPerson = (TenancyPerson) EntityFromView();
            GeneralBindingSource.AddNew();
            dataGridViewTenancyPersons.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromTenancyPerson(tenancyPerson);
            dateTimePickerDateOfBirth.Checked = (tenancyPerson.DateOfBirth != null);
            dateTimePickerDateOfDocumentIssue.Checked = (tenancyPerson.DateOfDocumentIssue != null);
            textBoxSurname.Focus();
            IsEditable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите этого участника договора?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_person"]) == -1)
                    return;
                IsEditable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                IsEditable = true;
                RedrawDataGridRows();
                ViewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports(); 
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override bool CanCancelRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            _vRegistrationStreet.Filter = "";
            _vResidenceStreet.Filter = "";
            _vDocumentIssuedBy.Filter = "";
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        IsEditable = false; 
                        dataGridViewTenancyPersons.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        RedrawDataGridRows();
                        if (GeneralBindingSource.Position != -1)
                            dataGridViewTenancyPersons.Rows[GeneralBindingSource.Position].Selected = true;
                    }
                    ViewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridViewTenancyPersons.Enabled = true;
                    IsEditable = false;
                    DataBind();
                    ViewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            if (comboBoxIssuedBy.SelectedValue == null && !string.IsNullOrEmpty(comboBoxIssuedBy.Text))
            {
                var document = new DocumentIssuedBy {DocumentIssuedByName = comboBoxIssuedBy.Text};
                var idDocument = DataModel.GetInstance<EntityDataModel<DocumentIssuedBy>>().Insert(document);
                if (idDocument == -1) return;
                document.IdDocumentIssuedBy = idDocument;
                DataModel.GetInstance<EntityDataModel<DocumentIssuedBy>>().Select().Rows.
                    Add(document.IdDocumentIssuedBy, document.DocumentIssuedByName);
                comboBoxIssuedBy.SelectedValue = document.IdDocumentIssuedBy;
            }
            var tenancyPerson = (TenancyPerson) EntityFromViewport();
            if (!ValidateTenancyPerson(tenancyPerson))
                return;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                case ViewportState.NewRowState:
                    var idPerson = GeneralDataModel.Insert(tenancyPerson);
                    if (idPerson == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    tenancyPerson.IdPerson = idPerson;
                    IsEditable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    EntityConverter<TenancyPerson>.FillRow(tenancyPerson, newRow);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancyPerson.IdPerson == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись об участнике договора без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(tenancyPerson) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    IsEditable = false;
                    EntityConverter<TenancyPerson>.FillRow(tenancyPerson, row);
                    break;
            }
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            ViewportState = ViewportState.ReadState;
            dataGridViewTenancyPersons.Enabled = true;
            IsEditable = true;
            MenuCallback.EditingStateUpdate();
        }

        private void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
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
            _vRegistrationStreet.Filter = "";
            _vResidenceStreet.Filter = "";
            _vDocumentIssuedBy.Filter = "";
            UnbindedCheckBoxesUpdate();
            if (GeneralBindingSource.Position == -1)
                return;
            if (ViewportState == ViewportState.NewRowState)
                return;
            dataGridViewTenancyPersons.Enabled = true;
            ViewportState = ViewportState.ReadState;
            IsEditable = true;
        }

        private void TenancyPersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            RedrawDataGridRows();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        private void TenancyPersonsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
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

        private void comboBoxIssuedBy_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxIssuedBy.Items.Count == 0)
                comboBoxIssuedBy.SelectedIndex = -1;
        }

        private void comboBoxIssuedBy_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back))
            {
                var text = comboBoxIssuedBy.Text;
                var selectionStart = comboBoxIssuedBy.SelectionStart;
                var selectionLength = comboBoxIssuedBy.SelectionLength;
                _vDocumentIssuedBy.Filter = "document_issued_by like '%" + comboBoxIssuedBy.Text + "%'";
                comboBoxIssuedBy.Text = text;
                comboBoxIssuedBy.SelectionStart = selectionStart;
                comboBoxIssuedBy.SelectionLength = selectionLength;
            }
        }

        private void comboBoxIssuedBy_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxIssuedBy.Text))
                comboBoxIssuedBy.SelectedValue = DBNull.Value;
        }

        private void comboBoxResidenceStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxResidenceStreet.Items.Count == 0)
                comboBoxResidenceStreet.SelectedIndex = -1;
        }

        private void comboBoxResidenceStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxResidenceStreet.Text;
                var selectionStart = comboBoxResidenceStreet.SelectionStart;
                var selectionLength = comboBoxResidenceStreet.SelectionLength;
                _vResidenceStreet.Filter = "street_name like '%" + comboBoxResidenceStreet.Text + "%'";
                comboBoxResidenceStreet.Text = text;
                comboBoxResidenceStreet.SelectionStart = selectionStart;
                comboBoxResidenceStreet.SelectionLength = selectionLength;
            }
        }

        private void comboBoxResidenceStreet_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxResidenceStreet.Text))
            {
                comboBoxResidenceStreet.SelectedValue = DBNull.Value;
                return;
            }
            if (comboBoxResidenceStreet.Items.Count > 0)
            {
                if (comboBoxResidenceStreet.SelectedValue == null)
                    comboBoxResidenceStreet.SelectedValue = _vResidenceStreet[_vResidenceStreet.Position];
                comboBoxResidenceStreet.Text = ((DataRowView)_vResidenceStreet[_vResidenceStreet.Position])["street_name"].ToString();
            }
            if (comboBoxResidenceStreet.SelectedValue == null)
                comboBoxResidenceStreet.Text = "";
        }

        private void comboBoxRegistrationStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxRegistrationStreet.Items.Count == 0)
                comboBoxRegistrationStreet.SelectedIndex = -1;
        }

        private void comboBoxRegistrationStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxRegistrationStreet.Text;
                var selectionStart = comboBoxRegistrationStreet.SelectionStart;
                var selectionLength = comboBoxRegistrationStreet.SelectionLength;
                _vRegistrationStreet.Filter = "street_name like '%" + comboBoxRegistrationStreet.Text + "%'";
                comboBoxRegistrationStreet.Text = text;
                comboBoxRegistrationStreet.SelectionStart = selectionStart;
                comboBoxRegistrationStreet.SelectionLength = selectionLength;
            }
        }

        private void comboBoxRegistrationStreet_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxRegistrationStreet.Text))
            {
                comboBoxRegistrationStreet.SelectedValue = DBNull.Value;
                return;
            }
            if (comboBoxRegistrationStreet.Items.Count > 0)
            {
                if (comboBoxRegistrationStreet.SelectedValue == null)
                    comboBoxRegistrationStreet.SelectedValue = _vRegistrationStreet[_vRegistrationStreet.Position];
                comboBoxRegistrationStreet.Text = ((DataRowView)_vRegistrationStreet[_vRegistrationStreet.Position])["street_name"].ToString();
            }
            if (comboBoxRegistrationStreet.SelectedValue == null)
                comboBoxRegistrationStreet.Text = "";
        }

        private void textBoxSNP_Leave(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;
            if (!string.IsNullOrEmpty(text))
                textBox.Text = text[0].ToString().ToUpper(CultureInfo.CurrentCulture) + text.Substring(1);
        }

        private void buttonImportFromMSP_Click(object sender, EventArgs e)
        {
            if ((GeneralBindingSource.Count == 0 || (GeneralBindingSource.Count == 1 && ViewportState == ViewportState.NewRowState))
                && ParentType == ParentTypeEnum.Tenancy)
            {
                var premisesAssoc = DataModel.GetInstance<TenancyPremisesAssocDataModel>();
                var premises = EntityDataModel<Premise>.GetInstance();
                var buildings = DataModel.GetInstance<EntityDataModel<Building>>();
                var streets = DataModel.GetInstance<KladrStreetsDataModel>();
                var currentPremise = (from premisesAssocRow in premisesAssoc.FilterDeletedRows()
                    join premisesRow in premises.FilterDeletedRows()
                        on premisesAssocRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                    join buildingsRow in buildings.FilterDeletedRows()
                        on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                    join streetsRow in streets.FilterDeletedRows()
                        on buildingsRow.Field<string>("id_street") equals streetsRow.Field<string>("id_street")
                    where premisesAssocRow.Field<int>("id_process") == (int)ParentRow["id_process"]
                    select new
                    {
                        premises_num = premisesRow.Field<string>("premises_num"),
                        house = buildingsRow.Field<string>("house"),
                        street_name = streetsRow.Field<string>("street_name")
                    }).ToList();
                if (currentPremise.Count == 0)
                {
                    MessageBox.Show(@"Нельзя импортировать данные из МСП, т.к. на процесс найма не завязано ни одного помещения", 
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1); 
                    return;
                }
                CancelRecord();
                using (var connection = new SqlConnection(Settings.RegistrySettings.MspConnectionString))
                {
                    var command = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = @"
                        SELECT v.family, name, father, v.birth, v.reg_date
                        FROM (
                        SELECT 
                          CASE SUBSTRING(ts.base_month,1,3) 
                          WHEN 'Jan' THEN SUBSTRING(ts.base_month,4,4)+'01'
                          WHEN 'Feb' THEN SUBSTRING(ts.base_month,4,4)+'02'
                          WHEN 'Mar' THEN SUBSTRING(ts.base_month,4,4)+'03'
                          WHEN 'Apr' THEN SUBSTRING(ts.base_month,4,4)+'04'
                          WHEN 'May' THEN SUBSTRING(ts.base_month,4,4)+'05'
                          WHEN 'Jun' THEN SUBSTRING(ts.base_month,4,4)+'06'
                          WHEN 'Jul' THEN SUBSTRING(ts.base_month,4,4)+'07'
                          WHEN 'Aug' THEN SUBSTRING(ts.base_month,4,4)+'08'
                          WHEN 'Sep' THEN SUBSTRING(ts.base_month,4,4)+'09'
                          WHEN 'Oct' THEN SUBSTRING(ts.base_month,4,4)+'10'
                          WHEN 'Nov' THEN SUBSTRING(ts.base_month,4,4)+'11'
                          WHEN 'Dec' THEN SUBSTRING(ts.base_month,4,4)+'12'
                          ELSE ts.base_month
                          END AS mon, family, name, father, birth, c AS reg_date
                        FROM dbo.t_bks ts
                        WHERE ts.street = @street AND house = @house AND RTRIM(LTRIM(flat)) = @flat) v
                        WHERE mon = (
                          SELECT 
                          MAX(CASE SUBSTRING(ts.base_month,1,3) 
                          WHEN 'Jan' THEN SUBSTRING(ts.base_month,4,4)+'01'
                          WHEN 'Feb' THEN SUBSTRING(ts.base_month,4,4)+'02'
                          WHEN 'Mar' THEN SUBSTRING(ts.base_month,4,4)+'03'
                          WHEN 'Apr' THEN SUBSTRING(ts.base_month,4,4)+'04'
                          WHEN 'May' THEN SUBSTRING(ts.base_month,4,4)+'05'
                          WHEN 'Jun' THEN SUBSTRING(ts.base_month,4,4)+'06'
                          WHEN 'Jul' THEN SUBSTRING(ts.base_month,4,4)+'07'
                          WHEN 'Aug' THEN SUBSTRING(ts.base_month,4,4)+'08'
                          WHEN 'Sep' THEN SUBSTRING(ts.base_month,4,4)+'09'
                          WHEN 'Oct' THEN SUBSTRING(ts.base_month,4,4)+'10'
                          WHEN 'Nov' THEN SUBSTRING(ts.base_month,4,4)+'11'
                          WHEN 'Dec' THEN SUBSTRING(ts.base_month,4,4)+'12'
                          ELSE ts.base_month
                          END) AS mon
                          FROM dbo.t_bks ts
                          WHERE ts.street = @street AND house = @house AND RTRIM(LTRIM(flat)) = @flat)
                            AND mon = (
                                SELECT 
                                MAX(CASE SUBSTRING(ts.base_month,1,3) 
                                WHEN 'Jan' THEN SUBSTRING(ts.base_month,4,4)+'01'
                                WHEN 'Feb' THEN SUBSTRING(ts.base_month,4,4)+'02'
                                WHEN 'Mar' THEN SUBSTRING(ts.base_month,4,4)+'03'
                                WHEN 'Apr' THEN SUBSTRING(ts.base_month,4,4)+'04'
                                WHEN 'May' THEN SUBSTRING(ts.base_month,4,4)+'05'
                                WHEN 'Jun' THEN SUBSTRING(ts.base_month,4,4)+'06'
                                WHEN 'Jul' THEN SUBSTRING(ts.base_month,4,4)+'07'
                                WHEN 'Aug' THEN SUBSTRING(ts.base_month,4,4)+'08'
                                WHEN 'Sep' THEN SUBSTRING(ts.base_month,4,4)+'09'
                                WHEN 'Oct' THEN SUBSTRING(ts.base_month,4,4)+'10'
                                WHEN 'Nov' THEN SUBSTRING(ts.base_month,4,4)+'11'
                                WHEN 'Dec' THEN SUBSTRING(ts.base_month,4,4)+'12'
                                ELSE ts.base_month
                                END) AS mon
                                FROM dbo.t_bks ts
                              )
                        ORDER BY mon;"
                    };
                    var streetName = currentPremise.First().street_name;
                    var prefixes = new []
                    {
                        "пер. ", "ул. ", "пр-кт. ", "б-р. ",
                        "гск. ", "проезд. ", "пл-ка. ","туп. "
                    };
                    var streetParts = streetName.Split(prefixes, StringSplitOptions.RemoveEmptyEntries);
                    streetName = streetParts[streetParts.Length - 1];
                    var street = new SqlParameter("street", SqlDbType.NVarChar) { Value = streetName };
                    command.Parameters.Add(street);

                    var house = new SqlParameter("house", SqlDbType.NVarChar) { Value = currentPremise.First().house };
                    command.Parameters.Add(house);

                    var flat = new SqlParameter("flat", SqlDbType.NVarChar) { Value = currentPremise.First().premises_num };
                    command.Parameters.Add(flat);

                    try
                    {
                        connection.Open();
                        var reader = command.ExecuteReader();
                        IsEditable = false;
                        while (reader.Read())
                        {
                            var surnameValue = reader.GetString(0);
                            var nameValue = reader.GetString(1);
                            var patronymicValue = reader.GetString(2);
                            var birthValue = reader.GetDateTime(3);
                            var regDateValue = reader.GetDateTime(4);
                            var person = new TenancyPerson
                            {
                                Surname = surnameValue,
                                Name = nameValue,
                                Patronymic = patronymicValue,
                                DateOfBirth = birthValue,
                                IdProcess = (int)ParentRow["id_process"],
                                IdKinship = 64,
                                IdDocumentType = 255,
                                RegistrationDate = regDateValue
                            };
                            var idPerson = GeneralDataModel.Insert(person);
                            if (idPerson == -1)
                                return;
                            person.IdPerson = idPerson;
                            var row = (DataRowView)GeneralBindingSource.AddNew();
                            if (row == null) continue;
                            EntityConverter<TenancyPerson>.FillRow(person, row);
                        }
                        IsEditable = true;
                    }
                    catch (SqlException err)
                    {
                        MessageBox.Show(string.Format("Ошибка подключения к базе данных. Подробнее: {0}", err.Message), @"Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        IsEditable = true;
                    }
                }
                return;
            }
            MessageBox.Show(@"Нельзя импортировать участников найма, т.к. в списке уже присутствуют участники", @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }
    }
}
