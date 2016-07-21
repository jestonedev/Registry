using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class DocumentIssuedByConverter
    {
        public static DocumentIssuedBy FromRow(DataRow row)
        {
            return new DocumentIssuedBy
            {
                IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by"),
                DocumentIssuedByName = ViewportHelper.ValueOrNull(row, "document_issued_by")
            };
        }

        public static DocumentIssuedBy FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static DocumentIssuedBy FromRow(DataGridViewRow row)
        {
            return new DocumentIssuedBy
            {
                IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by"),
                DocumentIssuedByName = ViewportHelper.ValueOrNull(row, "document_issued_by")
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return new[] { 
                row["id_document_issued_by"], 
                row["document_issued_by"]
            };
        }
    }
}
