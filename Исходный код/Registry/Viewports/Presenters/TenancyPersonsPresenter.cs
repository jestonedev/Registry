using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class TenancyPersonsPresenter: Presenter
    {
        public TenancyPersonsPresenter() : base(new TenancyPersonsViewModel(), null, null)
        {
            
        }

        internal bool DeleteRecord()
        {
            var row = ViewModel["general"].CurrentRow;
            var columnName = ViewModel["general"].PrimaryKeyFirst;
            if (ViewModel["general"].Model.Delete((int)row[columnName]) == -1)
                return false;
            row.Delete();
            return true;
        }

        internal int InsertDocumentIssuedBy(DocumentIssuedBy documentIssuedBy)
        {
            var idDocument = ViewModel["documents_issued_by"].Model.Insert(documentIssuedBy);
            if (idDocument == -1) return -1;
            ViewModel["documents_issued_by"].DataSource.Rows.Add(idDocument, documentIssuedBy.DocumentIssuedByName);
            return idDocument;
        }

        internal bool InsertRecord(TenancyPerson tenancyPerson)
        {
            var idPerson = ViewModel["general"].Model.Insert(tenancyPerson);
            if (idPerson == -1)
            {
                ViewModel["general"].Model.EditingNewRecord = false;
                return false;
            }
            tenancyPerson.IdPerson = idPerson;
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<TenancyPerson>.FillRow(tenancyPerson, row);
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        internal bool UpdateRecord(TenancyPerson tenancyPerson)
        {
            if (tenancyPerson.IdPerson == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить запись об участнике договора без внутренного номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(tenancyPerson) == -1)
                return false;
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<TenancyPerson>.FillRow(tenancyPerson, row);
            return true;
        }

        public void ImportPersonsFromMsp()
        {
            var persons = TenancyService.GetTenancyPersonsFromMsp((int) ParentRow["id_process"]);
            foreach (var person in persons)
            {
                var idPerson = ViewModel["general"].Model.Insert(person);
                if (idPerson == -1)
                    return;
                person.IdPerson = idPerson;
                var row = (DataRowView)ViewModel["general"].BindingSource.AddNew();
                if (row == null) break;
                EntityConverter<TenancyPerson>.FillRow(person, row);
            }
        }
    }
}
