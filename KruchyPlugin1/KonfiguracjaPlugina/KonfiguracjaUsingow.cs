using System.Collections.Generic;
using System.Linq;
using KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina.Xml;

namespace KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina
{
    class KonfiguracjaUsingow
    {
        public IList<string> NajczesciejUzywane { get; private set; }

        public KonfiguracjaUsingow(List<Namespace> listaZKonfiguracji)
        {
            NajczesciejUzywane = listaZKonfiguracji.Select(o => o.Nazwa).ToList();
        }

        public KonfiguracjaUsingow()
        {
            NajczesciejUzywane = new List<string>();
            DodajDefaultoweDlaPincasso();
        }

        private void DodajDefaultoweDlaPincasso()
        {
            NajczesciejUzywane.Add("Piatka.Infrastructure.Mappings");
            NajczesciejUzywane.Add("FluentAssertions");
            NajczesciejUzywane.Add("Piatka.Infrastructure.Tests.Builders");
            NajczesciejUzywane.Add("System.Linq");
        }
    }
}