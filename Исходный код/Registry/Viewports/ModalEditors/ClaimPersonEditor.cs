using System;
using System.Data;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Security;

namespace Registry.Viewport.ModalEditors
{
    internal partial class ClaimPersonEditor : Form
    {
        private ViewportState _state = ViewportState.NewRowState;
        private ClaimPerson _claimPerson;
        private readonly DataModel _claimPersons = EntityDataModel<ClaimPerson>.GetInstance();

        public DataRow ParentRow { get; set; }

        public ViewportState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value == ViewportState.ReadState)
                {
                    throw new ViewportException("Передано некорректное состояние формы редактирования");
                }
                if (value == ViewportState.ModifyRowState)
                {
                    Text = @"Изменить участника";
                    vButtonSave.Text = @"Изменить";
                }
                else
                {
                    Text = @"Добавить участника";
                    vButtonSave.Text = @"Добавить";
                }
                _state = value;
            }
        }

        public ClaimPerson ClaimPerson
        {
            get
            {
                var сlaimPerson = new ClaimPerson
                {
                    Surname = ViewportHelper.ValueOrNull(textBoxSurname),
                    Name = ViewportHelper.ValueOrNull(textBoxName),
                    Patronymic = ViewportHelper.ValueOrNull(textBoxPatronymic),
                    DateOfBirth = ViewportHelper.ValueOrNull(dateTimePickerDateOfBirth),
                    PlaceOfBirth = ViewportHelper.ValueOrNull(textBoxPlaceOfBirth),
                    WorkPlace = ViewportHelper.ValueOrNull(textBoxWorkPlace),
                    IsClaimer = checkBoxIsClaimer.Checked
                };
                if (_state == ViewportState.ModifyRowState)
                    сlaimPerson.IdPerson = _claimPerson.IdPerson;
                return сlaimPerson;
            }
            set
            {
                _claimPerson = value;
                if (value == null)
                    return;
                textBoxSurname.Text = value.Surname;
                textBoxName.Text = value.Name;
                textBoxPatronymic.Text = value.Patronymic;
                dateTimePickerDateOfBirth.Value = value.DateOfBirth ?? DateTime.Now.Date;
                textBoxPlaceOfBirth.Text = value.PlaceOfBirth;
                textBoxWorkPlace.Text = value.WorkPlace;
                checkBoxIsClaimer.Checked = value.IsClaimer;
            }
        }

        public ClaimPersonEditor()
        {
            InitializeComponent();
        }

        private bool ValidatePermissions()
        {
            return AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        private bool ValidateData(ClaimPerson claimPerson)
        {
            if (ValidatePermissions() == false)
                return false;
            return true;
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            var claimPerson = ClaimPerson;
            if (!ValidateData(claimPerson))
                return;
            var idClaim = ParentRow != null ? (int)ParentRow["id_claim"] : -1;
            if (idClaim == -1)
            {
                MessageBox.Show(@"Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            claimPerson.IdClaim = idClaim;

            if (_state == ViewportState.NewRowState)
            {
                var idPerson = _claimPersons.Insert(claimPerson);
                if (idPerson == -1)
                    return;
                _claimPersons.EditingNewRecord = true;
                _claimPersons.Select().Rows.Add(
                    idPerson, 
                    claimPerson.IdClaim,
                    claimPerson.Surname,
                    claimPerson.Name,
                    claimPerson.Patronymic,
                    claimPerson.DateOfBirth,
                    claimPerson.PlaceOfBirth,
                    claimPerson.WorkPlace,
                    claimPerson.IsClaimer
                 );
                _claimPersons.EditingNewRecord = false;
            } else
            {
                if (_claimPersons.Update(claimPerson) == -1)
                    return;
                var row = _claimPersons.Select().Rows.Find(claimPerson.IdPerson);
                EntityConverter<ClaimPerson>.FillRow(claimPerson, row);
            }
            DialogResult = DialogResult.OK;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                    SendKeys.Send("{TAB}");
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
