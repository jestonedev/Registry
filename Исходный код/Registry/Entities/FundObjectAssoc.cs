using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class FundObjectAssoc
    {
        public int? IdObject { get; set; }
        public int? IdFund { get; set; }

        public FundObjectAssoc(int? idObject, int? idFund)
        {
            this.IdFund = idFund;
            this.IdObject = idObject;
        }
    }
}
