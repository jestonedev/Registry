using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyProcess : Entity
    {
        public int? IdProcess { get; set; }
        public int? IdRentType { get; set; }
        public int? IdWarrant { get; set; }
        public int? IdExecutor { get; set; }
        public string RegistrationNum { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? UntilDismissal { get; set; }
        public string ResidenceWarrantNum { get; set; }
        public DateTime? ResidenceWarrantDate { get; set; }
        public string ProtocolNum { get; set; }
        public DateTime? ProtocolDate { get; set; }     
        public string Description { get; set; }
    }
}
