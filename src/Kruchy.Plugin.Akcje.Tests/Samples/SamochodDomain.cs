using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruchy.Plugin.Akcje.Tests.Samples
{
    public class SamochodDomain : PincassoDomainObject<int>
    {
        public int ID { get; set; }

        public string Marka { get; set; }

        public int Rocznik { get; set; }

        public decimal? Spalanie { get; set; }
    }
}
