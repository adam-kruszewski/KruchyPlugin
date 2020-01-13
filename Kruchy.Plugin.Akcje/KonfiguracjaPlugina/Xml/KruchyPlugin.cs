﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml
{
    public class KruchyPlugin
    {
        public List<Namespace> Usingi { get; set; }
        public bool SortowanieZaleznosciSerwisow { get; set; }

        public List<PrzejdzDo> PrzejdzDo { get; set; }

        public KruchyPlugin()
        {
            Usingi = new List<Namespace>();
            PrzejdzDo = new List<PrzejdzDo>();
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