using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ResettleProcessConverter
    {
        public static ResettleProcess FromRow(DataRow row)
        {
            return new ResettleProcess
            {
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                Debts = ViewportHelper.ValueOrNull<decimal>(row, "debts"),
                ResettleDate = ViewportHelper.ValueOrNull<DateTime>(row, "resettle_date"),
                DocNumber = ViewportHelper.ValueOrNull(row, "doc_number"),
                IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
        }

        public static ResettleProcess FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static void FillRow(ResettleProcess resettleProcess, DataRowView row)
        {
            row.BeginEdit();
            row["id_process"] = ViewportHelper.ValueOrDBNull(resettleProcess.IdProcess);
            row["resettle_date"] = ViewportHelper.ValueOrDBNull(resettleProcess.ResettleDate);
            row["debts"] = ViewportHelper.ValueOrDBNull(resettleProcess.Debts);
            row["id_document_residence"] = ViewportHelper.ValueOrDBNull(resettleProcess.IdDocumentResidence);
            row["description"] = ViewportHelper.ValueOrDBNull(resettleProcess.Description);
            row["doc_number"] = ViewportHelper.ValueOrDBNull(resettleProcess.DocNumber);
            row.EndEdit();
        }
    }
}
