using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class StructureTypeConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_structure_type"], 
                row["structure_type"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static StructureType FromRow(DataRow row)
        {
            return new StructureType
            {
                IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type"),
                StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type")
            };
        }

        public static StructureType FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static StructureType FromRow(DataGridViewRow row)
        {
            return new StructureType
            {
                IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type"),
                StructureTypeName = ViewportHelper.ValueOrNull(row, "structure_type")
            };
        }

        public static void FillRow(StructureType structureType, DataRow row)
        {
            row["id_structure_type"] = ViewportHelper.ValueOrDbNull(structureType.IdStructureType);
            row["structure_type"] = ViewportHelper.ValueOrDbNull(structureType.StructureTypeName);
        }
    }
}
