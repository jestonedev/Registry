namespace Registry.Entities
{
    public class ResettleObject : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdAssoc { get; set; }
        public virtual int? IdObject { get; set; }
        public int? IdProcess { get; set; }
    }

    [Relation(MasterTableName = "resettle_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "buildings", MasterFieldName = "id_building")]
    [DataTable(HasDeletedMark = true, Name = "resettle_buildings_from_assoc")]
    public sealed class ResettleBuildingFrom : ResettleObject
    {
        [DataColumn(Name = "id_building")]
        public override int? IdObject { get; set; }
    }
}
