using System.Collections.Generic;
using System.IO;
using System.Xml;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina
{
    class KonfiguracjaUsingow
    {
        private readonly SolutionWrapper solution;

        public IList<string> NajczesciejUzywane { get; private set; }
        public string NazwaSolution
        {
            get { return solution.PelnaNazwa; }
        }

        public KonfiguracjaUsingow(SolutionWrapper solution)
        {
            this.solution = solution;
            var sciezkaPlikuKonfiguracji = DajSciezkePlikuKonfiguracji();
            NajczesciejUzywane = new List<string>();

            if (!string.IsNullOrEmpty(sciezkaPlikuKonfiguracji) &&
                    File.Exists(sciezkaPlikuKonfiguracji))
                WczytajPlik(sciezkaPlikuKonfiguracji);
            else
                DodajDefaultoweDlaPincasso();
        }

        private void DodajDefaultoweDlaPincasso()
        {
            NajczesciejUzywane.Add("Piatka.Infrastructure.Mappings");
            NajczesciejUzywane.Add("FluentAssertions");
            NajczesciejUzywane.Add("Piatka.Infrastructure.Tests.Builders");
            NajczesciejUzywane.Add("System.Linq");
        }

        private string DajSciezkePlikuKonfiguracji()
        {
            var pelnaSciezkaSolution = solution.PelnaNazwa;
            return pelnaSciezkaSolution + ".kruchy.xml";
        }

        private void WczytajPlik(string sciezkaPlikuKonfiguracji)
        {
            var dokument = new XmlDocument();
            dokument.Load(sciezkaPlikuKonfiguracji);

            var wybrane =
                dokument.DocumentElement.SelectNodes("//Usingi//Namespace/text()");
            foreach (XmlNode n in wybrane)
                NajczesciejUzywane.Add(n.Value);
        }
    }
}