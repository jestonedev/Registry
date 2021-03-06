﻿using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "claim_states")]
    [Relation(SlaveTableName = "claim_persons")]
    [Relation(SlaveTableName = "claim_court_orders")]
    [Relation(MasterTableName = "payments_accounts", MasterFieldName = "id_account")]
    [DataTable(HasDeletedMark = true, Name = "claims")]
    public sealed class Claim : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdClaim { get; set; }
        public int? IdAccount { get; set; }
        public DateTime? AtDate { get; set; }
        public DateTime? StartDeptPeriod { get; set; }
        public DateTime? EndDeptPeriod { get; set; }
        public string Description { get; set; }

        [DataColumn(DefaultValue = 0)]
        public decimal? AmountTenancy { get; set; }

        [DataColumn(DefaultValue = 0)]
        public decimal? AmountDgi { get; set; }
        [DataColumn(DefaultValue = 0)]
        public decimal? AmountPadun { get; set; }
        [DataColumn(DefaultValue = 0)]
        public decimal? AmountPkk { get; set; }

        [DataColumn(DefaultValue = 0)]
        public decimal? AmountPenalties { get; set; }
    }
}
