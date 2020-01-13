using System.Collections.Generic;
using System.Linq;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;

namespace Kruchy.Plugin.Akcje.KonfiguracjaPlugina
{
    public class KonfiguracjaUsingow
    {
        public IList<NajczesciejUzywanyUsing> NajczesciejUzywane { get; private set; }

        public KonfiguracjaUsingow(List<Namespace> listaZKonfiguracji)
        {
            NajczesciejUzywane =
                listaZKonfiguracji
                    .Select(o => new NajczesciejUzywanyUsing(o.Nazwa, o.NamespaceUzycia))
                        .ToList();
        }

        public KonfiguracjaUsingow()
        {
            NajczesciejUzywane = new List<NajczesciejUzywanyUsing>();
            DodajDefaultoweDlaPincasso();
        }

        private void DodajDefaultoweDlaPincasso()
        {
            DodajNajczesciejUzywanyUsing("Piatka.Infrastructure.Mappings");
            DodajNajczesciejUzywanyUsing("FluentAssertions");
            DodajNajczesciejUzywanyUsing("Piatka.Infrastructure.Tests.Builders");
            DodajNajczesciejUzywanyUsing("System.Linq");
        }

        private void DodajNajczesciejUzywanyUsing(
            string nazwa,
            string namespaceUzycia = null)
        {
            NajczesciejUzywane.Add(
                new NajczesciejUzywanyUsing(nazwa, namespaceUzycia));
        }
    }

    public class NajczesciejUzywanyUsing
    {
        public string Nazwa { get; set; }
        public string NamespaceUzycia { get; set; }

        public NajczesciejUzywanyUsing(string nazwa, string namespaceUzycia)
        {
            Nazwa = nazwa;
            NamespaceUzycia = namespaceUzycia;
        }
    }
}
