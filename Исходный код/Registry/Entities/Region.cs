using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Region
    {
        public string id_region { get; set; }
        public string region { get; set; }
        public Region(string id_region, string region)
        {
            this.id_region = id_region;
            this.region = region;
        }

        public override string ToString()
        {
            return region;
        }
    }
}
