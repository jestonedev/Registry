using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "judges", MasterFieldName = "id_judge")]
    [Relation(MasterTableName = "buildings", MasterFieldName = "id_building")]
    [DataTable(HasDeletedMark = true, Name = "judges_buildings_assoc")]
    public sealed class JudgeBuildingAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdAssoc { get; set; }
        public int? IdJudge { get; set; }
        public int? IdBuilding { get; set; }
    }
}
