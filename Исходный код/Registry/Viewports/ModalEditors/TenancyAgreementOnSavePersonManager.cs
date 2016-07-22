using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;

namespace Registry.Viewport.ModalEditors
{
    internal partial class TenancyAgreementOnSavePersonManager : Form
    {
        public enum PersonsOperationType
        {
            IncludePersons,
            ExcludePersons,
            ChangePersons
        }

        private readonly PersonsOperationType _personsOperationType;

        public TenancyAgreementOnSavePersonManager(List<TenancyPerson> persons, PersonsOperationType personsOperationType)
        {
            InitializeComponent();
            if ((persons == null || persons.Count == 0))
            {
                DialogResult = DialogResult.OK;
                return;
            }
            _personsOperationType = personsOperationType;
           
            foreach (var person in persons)
            {
                var kinshipRow = DataModel.GetInstance<KinshipsDataModel>().FilterDeletedRows().
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
            if (_personsOperationType == PersonsOperationType.ChangePersons)
            {
                Text = @"Сменить участников найма";
                labelDate.Text = @"Дата изменения";
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
            var tenancyPersons = DataModel.GetInstance<TenancyPersonsDataModel>();
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
                        var tenancyPerson = TenancyPersonConverter.FromRow(row);
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
                case PersonsOperationType.ChangePersons:
                    if (persons.Count != 2)
                        break;
                    for (var i = 0; i < persons.Count; i++ )
                    {                          
                        var row = tenancyPersons.Select().Rows.Find(persons[i].IdPerson);
                        if (row == null) continue;
                        var tenancyPerson = TenancyPersonConverter.FromRow(row);
                        if(i == 0)
                        {
                            tenancyPerson.ExcludeDate = dateTimePickerDate.Value.Date;    
                        }
                        else
                        {
                            tenancyPerson.IncludeDate = dateTimePickerDate.Value.Date;
                        }
                        tenancyPerson.IdKinship = persons[i].IdKinship;   
                        var affected = tenancyPersons.Update(tenancyPerson);
                        if (affected == -1)
                        {
                            break;
                        }
                        if (i == 0)
                        {
                            row.BeginEdit();
                            row["exclude_date"] = tenancyPerson.ExcludeDate;
                            row["id_kinship"] = tenancyPerson.IdKinship;
                            row.EndEdit();
                        }
                        else
                        {
                            row.BeginEdit();
                            row["include_date"] = tenancyPerson.IncludeDate;
                            row["id_kinship"] = tenancyPerson.IdKinship;
                            row.EndEdit();
                        }
                    }
                    break;
                default:
                    throw new ViewportException("Неизвестный тип операции над участниками найма");
            }
            DialogResult = DialogResult.OK;
        }
    }
}
