using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport
{
    public partial class TenancyAgreementOnSavePersonManager : Form
    {
        public enum PersonsOperationType
        {
            IncludePersons,
            ExcludePersons
        }

        private readonly PersonsOperationType _personsOperationType;

        public TenancyAgreementOnSavePersonManager(List<TenancyPerson> persons, PersonsOperationType personsOperationType)
        {
            InitializeComponent();
            if (persons == null || persons.Count == 0)
            {
                DialogResult = DialogResult.OK;
                return;
            }
            _personsOperationType = personsOperationType;
            foreach (var person in persons)
            {
                var kinshipRow = DataModel.GetInstance(DataModelType.KinshipsDataModel).FilterDeletedRows().
                    FirstOrDefault(k => k.Field<int?>("id_kinship") == person.IdKinship);
                dataGridView.Rows.Add(true, person.IdPerson, person.IdProcess, person.Surname, person.Name, person.Patronymic,
                    person.DateOfBirth,
                    kinshipRow != null ? kinshipRow.Field<string>("kinship") : "", person.IdKinship);
            }
        }

        private void TenancyAgreementOnSavePersonManager_Shown(object sender, EventArgs e)
        {
            if (_personsOperationType == PersonsOperationType.ExcludePersons)
            {
                Text = @"Исключить участников найма";
                labelDate.Text = @"Дата исключения";
            }
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            var persons = new List<TenancyPerson>();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if ((bool) row.Cells["is_checked"].Value)
                {
                    persons.Add(new TenancyPerson
                    {
                        IdPerson = (int?)row.Cells["id_person"].Value,
                        IdProcess = (int?)row.Cells["id_process"].Value,
                        Surname = row.Cells["surname"].Value.ToString(),
                        Name = row.Cells["name"].Value.ToString(),
                        Patronymic = row.Cells["patronymic"].Value.ToString(),
                        DateOfBirth = (DateTime?)row.Cells["date_of_birth"].Value,
                        IdKinship = (int?)row.Cells["id_kinship"].Value,
                        IncludeDate = _personsOperationType == PersonsOperationType.IncludePersons ? (DateTime?)dateTimePickerDate.Value.Date : null,
                        ExcludeDate = _personsOperationType == PersonsOperationType.ExcludePersons ? (DateTime?)dateTimePickerDate.Value.Date : null,
                        IdDocumentType = 255
                    });
                }
            }
            var tenancyPersons = DataModel.GetInstance(DataModelType.TenancyPersonsDataModel);
            switch (_personsOperationType)
            {
                case PersonsOperationType.IncludePersons:
                    foreach (var person in persons)
                    {
                        var idPerson = tenancyPersons.Insert(person);
                        if (idPerson == -1)
                        {
                            break;
                        }
                        person.IdPerson = idPerson;
                        tenancyPersons.Select().Rows.Add(
                            person.IdPerson, person.IdProcess, person.IdKinship, person.Surname,
                            person.Name, person.Patronymic, person.DateOfBirth,
                            person.IdDocumentType,
                            person.DateOfDocumentIssue, person.DocumentNum, person.DocumentSeria, person.IdDocumentIssuedBy,
                            person.RegistrationIdStreet, person.RegistrationHouse, person.RegistrationFlat, person.RegistrationRoom, person.RegistrationDate,
                            person.ResidenceIdStreet, person.ResidenceHouse, person.ResidenceFlat, person.ResidenceRoom,
                            person.PersonalAccount, person.IncludeDate, person.ExcludeDate, 0);
                    }
                break;
                case PersonsOperationType.ExcludePersons:
                    foreach (var person in persons)
                    {
                        var row = tenancyPersons.Select().Rows.Find(person.IdPerson);
                        if (row == null) continue;
                        var tenancyPerson = PersonFromRow(row);
                        tenancyPerson.ExcludeDate = person.ExcludeDate;
                        var affected = tenancyPersons.Update(tenancyPerson);
                        if (affected == -1)
                        {
                            break;
                        }
                        row.BeginEdit();
                        row["exclude_date"] = person.ExcludeDate;
                        row.EndEdit();
                    }
                    break;
                default:
                    throw new ViewportException("Неизвестный тип операции над участниками найма");
            }
            DialogResult = DialogResult.OK;
        }

        protected TenancyPerson PersonFromRow(DataRow row)
        {
            var tenancyPerson = new TenancyPerson
            {
                IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdKinship = ViewportHelper.ValueOrNull<int>(row, "id_kinship"),
                IdDocumentType = ViewportHelper.ValueOrNull<int>(row, "id_document_type"),
                IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by"),
                Surname = ViewportHelper.ValueOrNull(row, "surname"),
                Name = ViewportHelper.ValueOrNull(row, "name"),
                Patronymic = ViewportHelper.ValueOrNull(row, "patronymic"),
                DateOfBirth = ViewportHelper.ValueOrNull<DateTime>(row, "date_of_birth"),
                DateOfDocumentIssue = ViewportHelper.ValueOrNull<DateTime>(row, "date_of_document_issue"),
                DocumentNum = ViewportHelper.ValueOrNull(row, "document_num"),
                DocumentSeria = ViewportHelper.ValueOrNull(row, "document_seria"),
                RegistrationIdStreet = ViewportHelper.ValueOrNull(row, "registration_id_street"),
                RegistrationHouse = ViewportHelper.ValueOrNull(row, "registration_house"),
                RegistrationFlat = ViewportHelper.ValueOrNull(row, "registration_flat"),
                RegistrationRoom = ViewportHelper.ValueOrNull(row, "registration_room"),
                ResidenceIdStreet = ViewportHelper.ValueOrNull(row, "residence_id_street"),
                ResidenceHouse = ViewportHelper.ValueOrNull(row, "residence_house"),
                ResidenceFlat = ViewportHelper.ValueOrNull(row, "residence_flat"),
                ResidenceRoom = ViewportHelper.ValueOrNull(row, "residence_room"),
                PersonalAccount = ViewportHelper.ValueOrNull(row, "personal_account"),
                IncludeDate = ViewportHelper.ValueOrNull<DateTime>(row, "include_date"),
                ExcludeDate = ViewportHelper.ValueOrNull<DateTime>(row, "exclude_date")
            };
            return tenancyPerson;
        }
    }
}
