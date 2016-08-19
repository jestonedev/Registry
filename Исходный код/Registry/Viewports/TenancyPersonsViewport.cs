using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.Presenters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyPersonsViewport : FormWithGridViewport
    {
        private TenancyPersonsViewport()
            : this(null, null)
        {
        }

        public TenancyPersonsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new TenancyPersonsPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridViewTenancyPersons;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        private void RedrawDataGridRows()
        {
            if (DataGridView.Rows.Count == 0)
                return;
            for (var i = 0; i < DataGridView.Rows.Count; i++)
            {
                if (Presenter.ViewModel["general"].BindingSource.Count <= i) break;
                var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[i];
                if (ViewportHelper.ValueOrNull<int>(row, "id_kinship") == 1 && row["exclude_date"] == DBNull.Value)
                    DataGridView.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else if (row["exclude_date"] != DBNull.Value)
                    DataGridView.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                else
                    DataGridView.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindSource(comboBoxKinship, Presenter.ViewModel["kinships"].BindingSource, "kinship",
                 Presenter.ViewModel["kinships"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxKinship, "SelectedValue", bindingSource,
                Presenter.ViewModel["kinships"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindSource(comboBoxDocumentType, Presenter.ViewModel["document_types"].BindingSource, "document_type",
                Presenter.ViewModel["document_types"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxDocumentType, "SelectedValue", bindingSource,
                Presenter.ViewModel["document_types"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindSource(comboBoxIssuedBy, Presenter.ViewModel["documents_issued_by"].BindingSource, "document_issued_by",
                Presenter.ViewModel["documents_issued_by"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxIssuedBy, "SelectedValue", bindingSource,
                Presenter.ViewModel["documents_issued_by"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindSource(comboBoxRegistrationStreet, Presenter.ViewModel["registration_kladr"].BindingSource, "street_name", 
                Presenter.ViewModel["registration_kladr"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxRegistrationStreet, "SelectedValue", bindingSource,
                "registration_id_street", DBNull.Value);

            ViewportHelper.BindSource(comboBoxResidenceStreet, Presenter.ViewModel["residence_kladr"].BindingSource, "street_name",
                Presenter.ViewModel["residence_kladr"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxResidenceStreet, "SelectedValue", bindingSource,
                "residence_id_street", DBNull.Value);

            ViewportHelper.BindProperty(textBoxSurname, "Text", bindingSource, "surname", "");
            ViewportHelper.BindProperty(textBoxName, "Text", bindingSource, "name", "");
            ViewportHelper.BindProperty(textBoxPatronymic, "Text", bindingSource, "patronymic", "");
            ViewportHelper.BindProperty(dateTimePickerDateOfBirth, "Value", bindingSource, "date_of_birth", null);
            ViewportHelper.BindProperty(textBoxPersonalAccount, "Text", bindingSource, "personal_account", "");
            ViewportHelper.BindProperty(textBoxDocumentSeria, "Text", bindingSource, "document_seria", "");
            ViewportHelper.BindProperty(textBoxDocumentNumber, "Text", bindingSource, "document_num", "");
            ViewportHelper.BindProperty(dateTimePickerDateOfDocumentIssue, "Value", bindingSource, "date_of_document_issue", null);
            ViewportHelper.BindProperty(textBoxRegistrationHouse, "Text", bindingSource, "registration_house", "");
            ViewportHelper.BindProperty(textBoxRegistrationFlat, "Text", bindingSource, "registration_flat", "");
            ViewportHelper.BindProperty(textBoxRegistrationRoom, "Text", bindingSource, "registration_room", "");
            ViewportHelper.BindProperty(textBoxResidenceHouse, "Text", bindingSource, "residence_house", "");
            ViewportHelper.BindProperty(textBoxResidenceFlat, "Text", bindingSource, "residence_flat", "");
            ViewportHelper.BindProperty(textBoxResidenceRoom, "Text", bindingSource, "residence_room", "");
            ViewportHelper.BindProperty(dateTimePickerIncludeDate, "Value", bindingSource, "include_date", null);
            ViewportHelper.BindProperty(dateTimePickerExcludeDate, "Value", bindingSource, "exclude_date", null);

            dataGridViewTenancyPersons.DataSource = Presenter.ViewModel["general"].BindingSource;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            date_of_birth.DataPropertyName = "date_of_birth";
            registration_date.DataPropertyName = "registration_date";
            ViewportHelper.BindSource(id_kinship, Presenter.ViewModel["kinships"].BindingSource, "kinship",
                Presenter.ViewModel["kinships"].PrimaryKeyFirst);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if ((row != null) && (row["date_of_birth"] != DBNull.Value))
                dateTimePickerDateOfBirth.Checked = true;
            else
            {
                dateTimePickerDateOfBirth.Value = DateTime.Now.Date;
                dateTimePickerDateOfBirth.Checked = false;
            }
            if ((row != null) && (row["date_of_document_issue"] != DBNull.Value))
                dateTimePickerDateOfDocumentIssue.Checked = true;
            else
            {
                dateTimePickerDateOfDocumentIssue.Value = DateTime.Now.Date;
                dateTimePickerDateOfDocumentIssue.Checked = false;
            }
            if ((row != null) && (row["include_date"] != DBNull.Value))
                dateTimePickerIncludeDate.Checked = true;
            else
            {
                dateTimePickerIncludeDate.Value = DateTime.Now.Date;
                dateTimePickerIncludeDate.Checked = false;
            }
            if ((row != null) && (row["exclude_date"] != DBNull.Value))
                dateTimePickerExcludeDate.Checked = true;
            else
            {
                dateTimePickerExcludeDate.Value = DateTime.Now.Date;
                dateTimePickerExcludeDate.Checked = false;
            }
            if (row != null)
            {
                comboBoxRegistrationStreet.SelectedValue = row["registration_id_street"];
                comboBoxResidenceStreet.SelectedValue = row["residence_id_street"];
            }
            else
            {
                comboBoxRegistrationStreet.SelectedValue = DBNull.Value;
                comboBoxResidenceStreet.SelectedValue = DBNull.Value;
            }
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
            dateTimePickerDateOfBirth.Checked = tenancyPerson.DateOfBirth != null;
            dateTimePickerDateOfDocumentIssue.Value = ViewportHelper.ValueOrDefault(tenancyPerson.DateOfDocumentIssue);
            dateTimePickerDateOfDocumentIssue.Checked = tenancyPerson.DateOfDocumentIssue != null;
            dateTimePickerIncludeDate.Value = ViewportHelper.ValueOrDefault(tenancyPerson.IncludeDate);
            dateTimePickerIncludeDate.Checked = tenancyPerson.IncludeDate != null;
            dateTimePickerExcludeDate.Value = ViewportHelper.ValueOrDefault(tenancyPerson.ExcludeDate);
            dateTimePickerExcludeDate.Checked = tenancyPerson.ExcludeDate != null;
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var tenancyPerson = new TenancyPerson
            {
                IdPerson = row == null ? null : ViewportHelper.ValueOrNull<int>(row, "id_person"),
                IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                IdKinship = ViewportHelper.ValueOrNull<int>(comboBoxKinship),
                IdDocumentType = ViewportHelper.ValueOrNull<int>(comboBoxDocumentType),
                IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(comboBoxIssuedBy),
                RegistrationIdStreet = ViewportHelper.ValueOrNull(comboBoxRegistrationStreet),
                ResidenceIdStreet = ViewportHelper.ValueOrNull(comboBoxResidenceStreet),
                Surname = ViewportHelper.ValueOrNull(textBoxSurname),
                PersonalAccount = ViewportHelper.ValueOrNull(textBoxPersonalAccount),
                DocumentSeria = ViewportHelper.ValueOrNull(textBoxDocumentSeria),
                DocumentNum = ViewportHelper.ValueOrNull(textBoxDocumentNumber),
                RegistrationHouse = ViewportHelper.ValueOrNull(textBoxRegistrationHouse),
                RegistrationFlat = ViewportHelper.ValueOrNull(textBoxRegistrationFlat),
                RegistrationRoom = ViewportHelper.ValueOrNull(textBoxRegistrationRoom),
                RegistrationDate = ViewportHelper.ValueOrNull<DateTime>(row, "registration_date"),
                ResidenceHouse = ViewportHelper.ValueOrNull(textBoxResidenceHouse),
                ResidenceFlat = ViewportHelper.ValueOrNull(textBoxResidenceFlat),
                ResidenceRoom = ViewportHelper.ValueOrNull(textBoxResidenceRoom),
                DateOfBirth = ViewportHelper.ValueOrNull(dateTimePickerDateOfBirth),
                DateOfDocumentIssue = ViewportHelper.ValueOrNull(dateTimePickerDateOfDocumentIssue),
                IncludeDate = ViewportHelper.ValueOrNull(dateTimePickerIncludeDate),
                ExcludeDate = ViewportHelper.ValueOrNull(dateTimePickerExcludeDate)
            };
            if (tenancyPerson.Surname != null)
                tenancyPerson.Surname = tenancyPerson.Surname[0].ToString().ToUpper(CultureInfo.CurrentCulture) + tenancyPerson.Surname.Substring(1);
            tenancyPerson.Name = ViewportHelper.ValueOrNull(textBoxName);
            if (tenancyPerson.Name != null)
                tenancyPerson.Name = tenancyPerson.Name[0].ToString().ToUpper(CultureInfo.CurrentCulture) + tenancyPerson.Name.Substring(1);
            tenancyPerson.Patronymic = ViewportHelper.ValueOrNull(textBoxPatronymic);
            if (tenancyPerson.Patronymic != null)
                tenancyPerson.Patronymic = tenancyPerson.Patronymic[0].ToString().ToUpper(CultureInfo.CurrentCulture) + tenancyPerson.Patronymic.Substring(1);
            return tenancyPerson;
        }

        protected override Entity EntityFromView()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
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
                for (var i = 0; i < Presenter.ViewModel["general"].BindingSource.Count; i++)
                {
                    var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[i];
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
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;
            Presenter.ParentRow = ParentRow;
            Presenter.ParentType = ParentType;

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            if ((ParentType == ParentTypeEnum.Tenancy) && (ParentRow != null))
                Text = string.Format(CultureInfo.InvariantCulture, "Участники найма №{0}", ParentRow["id_process"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            DataBind();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", TenancyPersonsViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", TenancyPersonsViewport_RowChanged);
            
            DataChangeHandlersInit();

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());

            IsEditable = true;

            if (Presenter.ViewModel["general"].BindingSource.Count == 0)
                InsertRecord();
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            DataGridView.Enabled = false;
            textBoxSurname.Focus();
            IsEditable = true;
        }

        public override bool CanCopyRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) && !Presenter.ViewModel["general"].Model.EditingNewRecord
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var tenancyPerson = (TenancyPerson)EntityFromView();
            Presenter.ViewModel["general"].BindingSource.AddNew();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            ViewportFromTenancyPerson(tenancyPerson);
            DataGridView.Enabled = false;
            dateTimePickerDateOfBirth.Checked = tenancyPerson.DateOfBirth != null;
            dateTimePickerDateOfDocumentIssue.Checked = tenancyPerson.DateOfDocumentIssue != null;
            dateTimePickerIncludeDate.Checked = tenancyPerson.IncludeDate != null;
            dateTimePickerExcludeDate.Checked = tenancyPerson.ExcludeDate != null;
            textBoxSurname.Focus();
            IsEditable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null)
                && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите этого участника договора?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            IsEditable = false;
            if (!((TenancyPersonsPresenter)Presenter).DeleteRecord())
            {
                IsEditable = true;
                return;
            }
            IsEditable = true;
            RedrawDataGridRows();
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
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
            Presenter.ViewModel["residence_kladr"].BindingSource.Filter = "";
            Presenter.ViewModel["registration_kladr"].BindingSource.Filter = "";
            Presenter.ViewModel["documents_issued_by"].BindingSource.Filter = "";
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    Presenter.ViewModel["general"].Model.EditingNewRecord = false;
                    var row = Presenter.ViewModel["general"].CurrentRow;
                    if (row != null)
                    {
                        IsEditable = false; 
                        row.Delete();
                        RedrawDataGridRows();
                        if (Presenter.ViewModel["general"].CurrentRow != null)
                        {
                            DataGridView.Rows[Presenter.ViewModel["general"].BindingSource.Position].Selected = true;
                        }
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            DataGridView.Enabled = true;
            ViewportState = ViewportState.ReadState;
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
                var idDocument = ((TenancyPersonsPresenter) Presenter).InsertDocumentIssuedBy(document);
                if (idDocument == -1) return;
                comboBoxIssuedBy.SelectedValue = idDocument;
            }
            var tenancyPerson = (TenancyPerson) EntityFromViewport();
            if (!ValidateTenancyPerson(tenancyPerson))
                return;
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((TenancyPersonsPresenter)Presenter).InsertRecord(tenancyPerson))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((TenancyPersonsPresenter)Presenter).UpdateRecord(tenancyPerson))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            ViewportState = ViewportState.ReadState;
            DataGridView.Enabled = true;
            IsEditable = true;
            MenuCallback.EditingStateUpdate();
        }

        protected override void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            if (Presenter.ViewModel["general"].CurrentRow == null || DataGridView.RowCount == 0)
                DataGridView.ClearSelection();
            else
                if (bindingSource.Position >= DataGridView.RowCount)
                DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                else if (DataGridView.Rows[bindingSource.Position].Selected != true)
                    DataGridView.Rows[bindingSource.Position].Selected = true;

            var isEditable = IsEditable;
            Presenter.ViewModel["residence_kladr"].BindingSource.Filter = "";
            Presenter.ViewModel["registration_kladr"].BindingSource.Filter = "";
            Presenter.ViewModel["documents_issued_by"].BindingSource.Filter = "";
            UnbindedCheckBoxesUpdate();
            IsEditable = isEditable;

            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
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
            if (e.Action != DataRowAction.Delete) return;
            UnbindedCheckBoxesUpdate();
            RedrawDataGridRows();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
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
            if ((e.KeyCode < Keys.A || e.KeyCode > Keys.Z) && (e.KeyCode != Keys.Back)) return;
            var text = comboBoxIssuedBy.Text;
            var selectionStart = comboBoxIssuedBy.SelectionStart;
            var selectionLength = comboBoxIssuedBy.SelectionLength;
            Presenter.ViewModel["documents_issued_by"].BindingSource.Filter = "document_issued_by like '%" + comboBoxIssuedBy.Text + "%'";
            comboBoxIssuedBy.Text = text;
            comboBoxIssuedBy.SelectionStart = selectionStart;
            comboBoxIssuedBy.SelectionLength = selectionLength;
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
            if ((e.KeyCode < Keys.A || e.KeyCode > Keys.Z) && (e.KeyCode != Keys.Back) && (e.KeyCode != Keys.Delete) &&
                (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9))
                return;
            var text = comboBoxResidenceStreet.Text;
            var selectionStart = comboBoxResidenceStreet.SelectionStart;
            var selectionLength = comboBoxResidenceStreet.SelectionLength;
            Presenter.ViewModel["residence_kladr"].BindingSource.Filter = "street_name like '%" + comboBoxResidenceStreet.Text + "%'";
            comboBoxResidenceStreet.Text = text;
            comboBoxResidenceStreet.SelectionStart = selectionStart;
            comboBoxResidenceStreet.SelectionLength = selectionLength;
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
                    comboBoxResidenceStreet.SelectedValue = Presenter.ViewModel["residence_kladr"].CurrentRow;
                comboBoxResidenceStreet.Text = Presenter.ViewModel["residence_kladr"].CurrentRow["street_name"].ToString();
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
            if ((e.KeyCode < Keys.A || e.KeyCode > Keys.Z) && (e.KeyCode != Keys.Back) && (e.KeyCode != Keys.Delete) &&
                (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9))
                return;
            var text = comboBoxRegistrationStreet.Text;
            var selectionStart = comboBoxRegistrationStreet.SelectionStart;
            var selectionLength = comboBoxRegistrationStreet.SelectionLength;
            Presenter.ViewModel["registration_kladr"].BindingSource.Filter = "street_name like '%" + comboBoxRegistrationStreet.Text + "%'";
            comboBoxRegistrationStreet.Text = text;
            comboBoxRegistrationStreet.SelectionStart = selectionStart;
            comboBoxRegistrationStreet.SelectionLength = selectionLength;
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
                    comboBoxRegistrationStreet.SelectedValue = Presenter.ViewModel["registration_kladr"].CurrentRow;
                comboBoxRegistrationStreet.Text = Presenter.ViewModel["registration_kladr"].CurrentRow["street_name"].ToString();
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
            if (Presenter.ViewModel["general"].BindingSource.Count == 0 ||
                (Presenter.ViewModel["general"].BindingSource.Count == 1 && ViewportState == ViewportState.NewRowState))
            {
                CancelRecord();
                IsEditable = false;
                ((TenancyPersonsPresenter) Presenter).ImportPersonsFromMsp();
                IsEditable = true;
                MenuCallback.EditingStateUpdate();
            }
            else
            {
                MessageBox.Show(@"Нельзя импортировать участников найма, т.к. в списке уже присутствуют участники",
                    @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
    }
}
