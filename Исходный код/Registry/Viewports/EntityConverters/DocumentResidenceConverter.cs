using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class DocumentResidenceConverter
    {
        public static DocumentResidence FromRow(DataRow row)
        {
            return new DocumentResidence
            {
                IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence"),
                DocumentResidenceName = ViewportHelper.ValueOrNull(row, "document_residence")
            };
        }

        public static DocumentResidence FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static DocumentResidence FromRow(DataGridViewRow row)
        {
            return new DocumentResidence
            {
                IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence"),
                DocumentResidenceName = ViewportHelper.ValueOrNull(row, "document_residence")
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return new[] { 
                row["id_document_residence"], 
                row["document_residence"]
            };
        }
    }
}
