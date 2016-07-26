using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyRentPeriod : Entity
    {
        public int? IdRentPeriod { get; set; }
        public int? IdProcess { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? UntilDismissal { get; set; }
    }
}
