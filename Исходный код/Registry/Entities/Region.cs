using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Region
    {
        public string IdRegion { get; set; }
        public string RegionName { get; set; }
        public Region(string idRegion, string region)
        {
            this.IdRegion = idRegion;
            this.RegionName = region;
        }

        public override string ToString()
        {
            return RegionName;
        }
    }
}
