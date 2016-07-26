using System;
using System.Data;
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
            row["id_process"] = ViewportHelper.ValueOrDbNull(resettleProcess.IdProcess);
            row["resettle_date"] = ViewportHelper.ValueOrDbNull(resettleProcess.ResettleDate);
            row["debts"] = ViewportHelper.ValueOrDbNull(resettleProcess.Debts);
            row["id_document_residence"] = ViewportHelper.ValueOrDbNull(resettleProcess.IdDocumentResidence);
            row["description"] = ViewportHelper.ValueOrDbNull(resettleProcess.Description);
            row["doc_number"] = ViewportHelper.ValueOrDbNull(resettleProcess.DocNumber);
            row.EndEdit();
        }
    }
}
