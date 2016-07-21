using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ResettlePersonConverter
    {
        public static object[] ToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_person"], 
                dataRowView["id_process"], 
                dataRowView["surname"], 
                dataRowView["name"],
                dataRowView["patronymic"],
                dataRowView["document_num"],
                dataRowView["document_seria"],
                dataRowView["founding_doc"]
            };
        }

        public static ResettlePerson FromRow(DataRow row)
        {
           return new ResettlePerson
            {
                IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                Surname = ViewportHelper.ValueOrNull(row, "surname"),
                Name = ViewportHelper.ValueOrNull(row, "name"),
                Patronymic = ViewportHelper.ValueOrNull(row, "patronymic"),
                DocumentNum = ViewportHelper.ValueOrNull(row, "document_num"),
                DocumentSeria = ViewportHelper.ValueOrNull(row, "document_seria"),
                FoundingDoc = ViewportHelper.ValueOrNull(row, "founding_doc")
            };
        }

        public static void FillRow(ResettlePerson resettlePerson, DataRow row)
        {
            row["id_person"] = ViewportHelper.ValueOrDBNull(resettlePerson.IdPerson);
            row["id_process"] = ViewportHelper.ValueOrDBNull(resettlePerson.IdProcess);
            row["surname"] = ViewportHelper.ValueOrDBNull(resettlePerson.Surname);
            row["name"] = ViewportHelper.ValueOrDBNull(resettlePerson.Name); 
            row["patronymic"] = ViewportHelper.ValueOrDBNull(resettlePerson.Patronymic); 
            row["document_num"] = ViewportHelper.ValueOrDBNull(resettlePerson.DocumentNum); 
            row["document_seria"] = ViewportHelper.ValueOrDBNull(resettlePerson.DocumentSeria);
            row["founding_doc"] = ViewportHelper.ValueOrDBNull(resettlePerson.FoundingDoc);
        }
    }
}
