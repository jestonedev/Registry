using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "restrictions", MasterFieldName = "id_restriction")]
    [Relation(MasterTableName = "premises", MasterFieldName = "id_premises")]
    [DataTable(Name = "restrictions_premises_assoc", HasDeletedMark = true)]
    public sealed class RestrictionPremisesAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true, IncludeIntoInsert = true)]
        public int? IdRestriction { get; set; }
        public int? IdPremises{ get; set; }

        [DataColumn(IncludeIntoInsert = false, IncludeIntoUpdate = false)]
        public DateTime? Date { get; set; }

        public RestrictionPremisesAssoc(int? idPremises, int? idRestriction, DateTime? date)
        {
            IdPremises = idPremises;
            IdRestriction = idRestriction;
            Date = date;
        }
    }
}
