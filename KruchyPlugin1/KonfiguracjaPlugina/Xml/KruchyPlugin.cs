using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina.Xml
{
    public class KruchyPlugin
    {
        public List<Namespace> Usingi { get; set; }

        public KruchyPlugin()
        {
            Usingi = new List<Namespace>();
        }
    }

    public class Namespace
    {
        public string Nazwa { get; set; }

        public override string ToString()
        {
            return Nazwa;
        }
    }
}
