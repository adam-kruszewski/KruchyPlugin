using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Kruchy.Plugin.Akcje.Akcje.Generowanie.Xsd.Komponenty;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class GenerowanieXsdDlaReportView
    {
        private const string NamespaceXS = "http://www.w3.org/2001/XMLSchema";

        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public GenerowanieXsdDlaReportView(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Generuj(IParametryGenerowaniaXsd parametry)
        {
            var sciezkaDoXsd = parametry.SciezkaDoXsd;

            if (string.IsNullOrEmpty(sciezkaDoXsd))
                return;

            var sparsowane = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            Obiekt klasaView = DajKlaseReportView(sparsowane);

            if (klasaView == null)
                return;

            var aktualnyProjekt = solution.AktualnyProjekt;

            var dokument = DajNowyDokumentLubWczytajIstniejacy(klasaView, sciezkaDoXsd);

            var elementHeader = DajElementHeader(dokument);

            UzupelnijDefinicjeWgKlasy(klasaView, dokument, elementHeader);

            ZapiszDokument(dokument, sciezkaDoXsd);
            solutionExplorer.OtworzPlik(sciezkaDoXsd);

            if (!aktualnyProjekt.Pliki.Any(o => o.SciezkaPelna == sciezkaDoXsd))
                aktualnyProjekt.DodajPlik(sciezkaDoXsd);
        }

        private void UzupelnijDefinicjeWgKlasy(
            Obiekt klasaView,
            XmlDocument dokument,
            XmlElement element)
        {
            var elementComplexType = SzukajElementuWgNazwy(element, "xs:complexType");
            var elementSequenceWHeader = SzukajElementuWgNazwy(elementComplexType, "xs:sequence");

            foreach (var wlasciwosc in klasaView.Propertiesy.Where(o => !Kolekcja(o)))
            {
                var elementOdpowiadajacy =
                    SzukajElementuWgAttrybutuName(elementSequenceWHeader, wlasciwosc.Nazwa);

                if (elementOdpowiadajacy == null)
                {
                    elementOdpowiadajacy = dokument.CreateElement("xs", "element", NamespaceXS);
                    elementOdpowiadajacy.SetAttribute("name", wlasciwosc.Nazwa);
                    elementSequenceWHeader.AppendChild(elementOdpowiadajacy);
                }

                UzupelnijDaneOdnosnieTypuINullowalnosci(
                    elementOdpowiadajacy,
                    wlasciwosc);
            }

            UzupelnijDaneOdnosniePolaTypuKolejkcja(klasaView, dokument);
        }

        private void UzupelnijDaneOdnosniePolaTypuKolejkcja(
            Obiekt klasaView,
            XmlDocument dokument)
        {
            var rootElement = dokument.DocumentElement;

            foreach (var wlasciwosc in klasaView.Propertiesy.Where(o => Kolekcja(o)))
            {
                var elementDlaWlasciwosci =
                    SzukajElementuWgAttrybutuName(rootElement, wlasciwosc.Nazwa);

                var regex = new Regex(@"<([A-Za-z0-9_]+)>");
                var match = regex.Match(wlasciwosc.NazwaTypu);

                var nazwaKlasy = match.Groups[1].Value;
                if (elementDlaWlasciwosci == null)
                {
                    elementDlaWlasciwosci = CreateElementDefinicjiObiektu(dokument, nazwaKlasy);
                    rootElement.AppendChild(elementDlaWlasciwosci);
                }

                var plikZKlasa =
                    solution
                        .AktualnyProjekt
                            .Pliki
                                .Single(o => o.NazwaBezRozszerzenia == nazwaKlasy);

                var klasaObiektuKolekcjonowanego =
                    Parser
                        .ParsujPlik(plikZKlasa.SciezkaPelna)
                            .DefiniowaneObiekty
                                .Single(o => o.Nazwa == nazwaKlasy);

                UzupelnijDefinicjeWgKlasy(
                    klasaObiektuKolekcjonowanego,
                    dokument,
                    elementDlaWlasciwosci);
            }
        }

        private void UzupelnijDaneOdnosnieTypuINullowalnosci(
            XmlElement elementOdpowiadajacy,
            Property wlasciwosc)
        {
            elementOdpowiadajacy
                .SetAttribute("type", DajNazweTypuDoXml(wlasciwosc.NazwaTypu));

            if (wlasciwosc.NazwaTypu != "string")
            {
                var nullowalny = wlasciwosc.NazwaTypu.EndsWith("?");
                elementOdpowiadajacy
                    .SetAttribute("nillable", nullowalny.ToString().ToLower());
            }
        }

        private string DajNazweTypuDoXml(string nazwaTypu)
        {
            switch (nazwaTypu)
            {
                case "string":
                    return "xs:string";
                case "decimal":
                case "decimal?":
                    return "xs:decimal";
                case "int":
                case "int?":
                    return "xs:number";
                case "bool":
                case "bool?":
                    return "xs:boolean";
                case "DateTime":
                case "DateTime?":
                    return "xs:datetime";
            }

            return "?????";
        }

        private XmlElement DajElementHeader(XmlDocument dokument)
        {
            var elementGlowny = dokument.DocumentElement;

            return SzukajElementuWgAttrybutuName(elementGlowny, "header");
        }

        private XmlElement SzukajElementuWgNazwy(
            XmlElement elementStartowy,
            string nazwa)
        {
            foreach (XmlElement element in elementStartowy.ChildNodes)
                if (element.Name.ToLower() == nazwa.ToLower())
                    return element;

            return null;
        }

        private XmlElement SzukajElementuWgAttrybutuName(
            XmlElement elementStartowy,
            string nazwa)
        {

            foreach (XmlElement element in elementStartowy.ChildNodes)
                foreach (XmlAttribute a in element.Attributes)
                    if (a.Name.ToLower() == "name" && a.Value.ToLower() == nazwa.ToLower())
                        return element;

            return null;

        }

        private bool Kolekcja(Property wlasciwosc)
        {
            var nazwaTypu = wlasciwosc.NazwaTypu;

            string[] poczatkiKolekcji =
            {
                "List<",
                "IList<",
                "IEnumerable<"
            };

            return poczatkiKolekcji.Any(o => nazwaTypu.StartsWith(o));
        }

        private XmlDocument DajNowyDokumentLubWczytajIstniejacy(Obiekt klasaView, string sciezkaDoXsd)
        {
            if (File.Exists(sciezkaDoXsd))
            {
                var ladowanyDokument = new XmlDocument();
                ladowanyDokument.Load(sciezkaDoXsd);
                return ladowanyDokument;
            }

            XmlDocument nowyDokument = new XmlDocument();

            XmlDeclaration xmlDeclaration = nowyDokument.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = nowyDokument.DocumentElement;
            nowyDokument.InsertBefore(xmlDeclaration, root);

            XmlElement element1 = CreateRootElement(klasaView, nowyDokument);

            element1.AppendChild(CreateElementDefinicjiObiektu(nowyDokument, "header"));

            return nowyDokument;
        }

        private void ZapiszDokument(XmlDocument dokument, string sciezkaDoXsd)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            using (var fileStream = new FileStream(sciezkaDoXsd, FileMode.CreateNew))
            using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            using (var writer = XmlWriter.Create(streamWriter, settings))
            {
                dokument.Save(writer);
            }
        }

        private XmlElement CreateElementDefinicjiObiektu(XmlDocument doc, string nazwa)
        {
            XmlElement elementGlowny = doc.CreateElement("xs", "element", NamespaceXS);
            elementGlowny.SetAttribute("name", nazwa);

            var elementComplexType = doc.CreateElement("xs", "complexType", NamespaceXS);
            elementGlowny.AppendChild(elementComplexType);

            var elementSequence = doc.CreateElement("xs", "sequence", NamespaceXS);
            elementComplexType.AppendChild(elementSequence);

            return elementGlowny;
        }

        private static XmlElement CreateRootElement(Obiekt klasaView, XmlDocument doc)
        {
            XmlElement element1 = doc.CreateElement("xs", "schema", NamespaceXS);
            doc.AppendChild(element1);

            element1.SetAttribute("id", klasaView.Nazwa);
            element1.SetAttribute("targetNamespace", DajTargetNamespace(klasaView));
            element1.SetAttribute("elementFormDefault", "qualified");
            element1.SetAttribute("xmlns", DajTargetNamespace(klasaView));
            element1.SetAttribute("xmlns:mstns", DajTargetNamespace(klasaView));
            element1.SetAttribute("xmlns:xs", "http://www.w3.org/2001/XMLSchema");
            return element1;
        }

        private static string DajTargetNamespace(Obiekt klasaView)
        {
            return string.Format("http://tempuri.org/{0}.xsd", klasaView.Nazwa);
        }

        private Obiekt DajKlaseReportView(
            Plik sparsowane)
        {
            var klasy =
                sparsowane
                    .DefiniowaneObiekty
                        .Where(o => o.Rodzaj == RodzajObiektu.Klasa);

            Obiekt klasaView;
            klasaView =
                sparsowane
                    .SzukajKlasyWLinii(solution.AktualnyDokument.DajNumerLiniiKursora());

            if (klasaView == null)
                klasaView = klasy.FirstOrDefault();

            return klasaView;
        }
    }
}
