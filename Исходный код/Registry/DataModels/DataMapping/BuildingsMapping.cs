using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Registry.Entities;

namespace Registry.DataModels.DataMapping
{
    public sealed class BuildingsMapping : TableMapping
    {
        public BuildingsMapping()
        {
            TableName = "buildings";
        }

        public override List<ColumnMapping> GetColumnsMapping()
        {
            return new List<ColumnMapping>
            {
                new ColumnMapping
                {
                    DbColumn = "id_building",
                    EntityColumn = typeof(Building).GetProperty("IdBuilding")
                },
                new ColumnMapping
                {
                    DbColumn = "id_state",
                    EntityColumn = typeof(Building).GetProperty("IdState")
                },
                new ColumnMapping
                {
                    DbColumn = "id_structure_type",
                    EntityColumn = typeof(Building).GetProperty("IdStructureType")
                },
                new ColumnMapping
                {
                    DbColumn = "id_street",
                    EntityColumn = typeof(Building).GetProperty("IdStreet")
                },
                new ColumnMapping
                {
                    DbColumn = "house",
                    EntityColumn = typeof(Building).GetProperty("House")
                },
                new ColumnMapping
                {
                    DbColumn = "floors",
                    EntityColumn = typeof(Building).GetProperty("Floors")
                },
                new ColumnMapping
                {
                    DbColumn = "num_premises",
                    EntityColumn = typeof(Building).GetProperty("NumPremises")
                },
                new ColumnMapping
                {
                    DbColumn = "num_rooms",
                    EntityColumn = typeof(Building).GetProperty("NumRooms")
                },
                new ColumnMapping
                {
                    DbColumn = "num_apartments",
                    EntityColumn = typeof(Building).GetProperty("NumApartments")
                },
                new ColumnMapping
                {
                    DbColumn = "num_shared_apartments",
                    EntityColumn = typeof(Building).GetProperty("NumSharedApartments")
                },
                // TODO: to be continue
            };
        }

        public override List<RelationMapping> GetRelationsMapping()
        {
            return new List<RelationMapping>
            {
                new RelationMapping
                {
                    MasterColumnName = "",
                    MasterTableName = "",
                    SlaveColumnName = "",
                    SlaveTableName = ""
                }
            };
        }

        public override List<ColumnMapping> GetPrimaryKeyColumnsMapping()
        {
            return GetColumnsMapping().Where(v => v.DbColumn == "id_building").ToList();
        }
    }
}
