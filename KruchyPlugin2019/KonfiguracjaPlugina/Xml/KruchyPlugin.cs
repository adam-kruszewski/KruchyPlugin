﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina.Xml
{
    public class KruchyPlugin
    {
        public List<Namespace> Usingi { get; set; }
        public bool SortowanieZaleznosciSerwisow { get; set; }

        public KruchyPlugin()
        {
            Usingi = new List<Namespace>();
        }
    }

    public class Namespace
    {
        public string Nazwa { get; set; }

        [XmlAttribute("Uzycie")]
        public string NamespaceUzycia { get; set; }

        public override string ToString()
        {
            return Nazwa;
        }
    }
}
