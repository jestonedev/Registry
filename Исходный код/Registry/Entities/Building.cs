using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "kladr", MasterFieldName = "id_street")]
    [Relation(MasterTableName = "object_states", MasterFieldName = "id_state")]
    [Relation(MasterTableName = "structure_types", MasterFieldName = "id_structure_type")]
    [Relation(MasterTableName = "heating_type", MasterFieldName = "id_heating_type")]
    [Relation(SlaveTableName = "premises")]
    [Relation(SlaveTableName = "restrictions_buildings_assoc")]
    [Relation(SlaveTableName = "ownership_buildings_assoc")]
    [Relation(SlaveTableName = "funds_buildings_assoc")]
    [Relation(SlaveTableName = "tenancy_buildings_assoc")]
    [Relation(SlaveTableName = "resettle_buildings_from_assoc")]
    [Relation(SlaveTableName = "resettle_buildings_to_assoc")]
    [DataTable(Name = "buildings", HasDeletedMark = true)]
    public sealed class Building : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdBuilding { get; set; }

        [DataColumn(DefaultValue = 1)]
        public int? IdState { get; set; }

        [DataColumn(DefaultValue = 4)]
        public int? IdStructureType { get; set; }

        public string IdStreet { get; set; }

        public string House { get; set; }

        [DataColumn(DefaultValue = 5)]
        public short? Floors { get; set; }

        [DataColumn(DefaultValue = 0)]
        public int? NumPremises { get; set; }

        [DataColumn(DefaultValue = 0)]
        public int? NumRooms { get; set; }

        [DataColumn(DefaultValue = 0)]
        public int? NumApartments { get; set; }

        [DataColumn(DefaultValue = 0)]
        public int? NumSharedApartments { get; set; }

        [DataColumn(DefaultValue = 0)]
        public double? LivingArea { get; set; }

        [DataColumn(DefaultValue = 0)]
        public double? TotalArea { get; set; }

        public string CadastralNum { get; set; }

        [DataColumn(DefaultValue = 0)]
        public decimal? CadastralCost { get; set; }

        [DataColumn(DefaultValue = 0)]
        public decimal? BalanceCost { get; set; }

        public string Description { get; set; }

        public int? StartupYear { get; set; }

        [DataColumn(DefaultValue = true)]
        public bool? Improvement { get; set; }

        [DataColumn(DefaultValue = false)]
        public bool? Elevator { get; set; }

        [DataColumn(DefaultValue = false)]
        public bool? RubbishChute { get; set; }

        [DataColumn(DefaultValue = 0)]
        public double? Wear { get; set; }

        public DateTime? StateDate { get; set; }

        [DataColumn(DefaultValue = true)]
        public bool? Plumbing { get; set; }

        [DataColumn(DefaultValue = true)]
        public bool? HotWaterSupply { get; set; }

        [DataColumn(DefaultValue = true)]
        public bool? Canalization { get; set; }

        [DataColumn(DefaultValue = true)]
        public bool? Electricity { get; set; }

        [DataColumn(DefaultValue = false)]
        public bool? RadioNetwork { get; set; }

        [DataColumn(DefaultValue = 4)]
        public int? IdHeatingType { get; set; }

        [DataColumn(Name = "BTI_rooms")]
        public string RoomsBTI { get; set; }
        public string HousingCooperative { get; set; }
        public DateTime? RegDate { get; set; }
    }
}
