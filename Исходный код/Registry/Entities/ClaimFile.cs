using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public class ClaimFile: Entity
    {
        public int IdFile { get; set; }
        public int IdClaim { get; set; }
        public string DisplayName { get; set; }
        public string FileName { get; set; }
    }
}
