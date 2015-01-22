using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class AggregatedAddress
    {
        public int IdProcess { get; set; }
        public string Address { get; set; }

        public AggregatedAddress(int idProcess, string address)
        {
            this.IdProcess = idProcess;
            this.Address = address;
        }
    }
}
