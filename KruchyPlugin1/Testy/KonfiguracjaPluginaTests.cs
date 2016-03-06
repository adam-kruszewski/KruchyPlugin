using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina.Xml;
using NUnit.Framework;

namespace KruchyCompany.KruchyPlugin1.Testy
{
    [TestFixture]
    class KonfiguracjaPluginaTests
    {
        [Test]
        public void TestSerializacji()
        {
            //arrange
            var konf = new KruchyPlugin();
            konf.Usingi.Add(new Namespace());
            konf.Usingi.Add(new Namespace());

            //act
            string wynik;
            using (var sw = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(konf.GetType());
                xmlSerializer.Serialize(sw, konf);
                wynik = sw.ToString();
            }
            
            //assert
            using (var stringReader = new StringReader(wynik))
            using (var xmlReader = new XmlTextReader(stringReader))
            {
                var xmlSerializer = new XmlSerializer(konf.GetType());
                var deserializowane = xmlSerializer.Deserialize(xmlReader);
            }

        }
    }
}
