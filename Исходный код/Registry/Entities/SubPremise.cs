using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "premises", MasterFieldName = "id_premises")]
    [Relation(MasterTableName = "object_states", MasterFieldName = "id_state")]
    [Relation(SlaveTableName = "funds_sub_premises_assoc")]
    [Relation(SlaveTableName = "tenancy_sub_premises_assoc")]
    [Relation(SlaveTableName = "resettle_sub_premises_from_assoc")]
    [Relation(SlaveTableName = "resettle_sub_premises_to_assoc")]
    [DataTable(Name = "sub_premises", HasDeletedMark = true)]
    public sealed class SubPremise : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdSubPremises { get; set; }
        public int? IdPremises { get; set; }

        [DataColumn(DefaultValue = 1)]
        public int? IdState { get; set; }
        public string SubPremisesNum { get; set; }
        public string Description { get; set; }
        
        [DataColumn(DefaultValue = 0)]
        public double? TotalArea { get; set; }

        [DataColumn(DefaultValue = 0)]
        public double? LivingArea { get; set; }
        public DateTime? StateDate { get; set; }
        public string CadastralNum { get; set; }

        [DataColumn(DefaultValue = 0)]
        public decimal? CadastralCost { get; set; }

        [DataColumn(DefaultValue = 0)]
        public decimal? BalanceCost { get; set; }
        public string Account { get; set; }
    }
}
