using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Region : Entity
    {
        public string IdRegion { get; set; }
        public string RegionName { get; set; }
        public Region(string idRegion, string region)
        {
            IdRegion = idRegion;
            RegionName = region;
        }

        public override string ToString()
        {
            return RegionName;
        }
    }
}
