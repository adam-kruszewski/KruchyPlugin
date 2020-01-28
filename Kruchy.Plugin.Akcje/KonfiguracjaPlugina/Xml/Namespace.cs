using System.Xml.Serialization;

namespace Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml
{
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