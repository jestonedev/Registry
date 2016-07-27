using System;

namespace Registry.Entities
{
    [Relation(MasterTableName = "buildings", MasterFieldName = "id_building")]
    [Relation(MasterTableName = "premises_types", MasterFieldName = "id_premises_type")]
    [Relation(MasterTableName = "premises_kinds", MasterFieldName = "id_premises_kind")]
    [Relation(MasterTableName = "object_states", MasterFieldName = "id_state")]
    [Relation(SlaveTableName = "sub_premises")]
    [Relation(SlaveTableName = "restrictions_premises_assoc")]
    [Relation(SlaveTableName = "ownership_premises_assoc")]
    [Relation(SlaveTableName = "funds_premises_assoc")]
    [Relation(SlaveTableName = "tenancy_premises_assoc")]
    [Relation(SlaveTableName = "resettle_premises_from_assoc")]
    [Relation(SlaveTableName = "resettle_premises_to_assoc")]
    [DataTable(Name = "premises", HasDeletedMark = true)]
    public sealed class Premise : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdPremises { get; set; }
        public int? IdBuilding { get; set; }

        [DataColumn(DefaultValue = 1)]
        public int? IdState { get; set; }
        public string PremisesNum { get; set; }

        [DataColumn(DefaultValue = 0)]
        public double? TotalArea { get; set; }

        [DataColumn(DefaultValue = 0)]
        public double? LivingArea { get; set; }

        [DataColumn(DefaultValue = 0)]
        public double? Height { get; set; }

        [DataColumn(DefaultValue = 0)]
        public short? NumRooms { get; set; }

        [DataColumn(DefaultValue = 0)]
        public short? NumBeds { get; set; }

        [DataColumn(DefaultValue = 1)]
        public int? IdPremisesType { get; set; }

        [DataColumn(DefaultValue = 1)]
        public int? IdPremisesKind { get; set; }

        [DataColumn(DefaultValue = 0)]
        public short? Floor { get; set; }
        public string CadastralNum { get; set; }

        [DataColumn(DefaultValue = 0)]
        public decimal? CadastralCost { get; set; }

        [DataColumn(DefaultValue = 0)]
        public decimal? BalanceCost { get; set; }
        public string Description { get; set; }
        public DateTime? RegDate { get; set; }

        [DataColumn(DefaultValue = false)]
        public bool? IsMemorial { get; set; }
        public string Account { get; set; }
        public DateTime? StateDate { get; set; }
    }
}
