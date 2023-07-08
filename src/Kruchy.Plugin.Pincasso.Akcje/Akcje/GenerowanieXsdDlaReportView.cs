using Kruchy.Plugin.Akcje.Akcje.Generowanie.Xsd.Komponenty;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Kruchy.Plugin.Pincasso.Akcje.Akcje
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

            DefinedItem klasaView = DajKlaseReportView();

            if (klasaView == null)
                return;

            var aktualnyProjekt = solution.AktualnyProjekt;

            var dokument = DajNowyDokumentLubWczytajIstniejacy(klasaView, sciezkaDoXsd);
            ZapiszDokument(dokument, sciezkaDoXsd);

            var elementHeader = DajElementHeader(dokument);

            UzupelnijDefinicjeWgKlasy(klasaView, dokument, elementHeader);

            ZapiszDokument(dokument, sciezkaDoXsd);

            solutionExplorer.OpenFile(sciezkaDoXsd);

            if (!aktualnyProjekt.Files.Any(o => o.FullPath == sciezkaDoXsd))
                aktualnyProjekt.AddFile(sciezkaDoXsd);
        }

        private void UzupelnijDefinicjeWgKlasy(
            DefinedItem klasaView,
            XmlDocument dokument,
            XmlElement element)
        {
            var elementComplexType = SzukajElementuWgNazwy(element, "xs:complexType");
            var elementSequenceWHeader = SzukajElementuWgNazwy(elementComplexType, "xs:sequence");

            foreach (var wlasciwosc in klasaView.Properties.Where(o => !Kolekcja(o)))
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

            UzupelnijDaneOdnosniePolTypuKolekcja(klasaView, dokument);
        }

        private void UzupelnijDaneOdnosniePolTypuKolekcja(
            DefinedItem klasaView,
            XmlDocument dokument)
        {
            var rootElement = dokument.DocumentElement;

            foreach (var wlasciwosc in klasaView.Properties.Where(o => Kolekcja(o)))
                UzupelnijDefinicjePolaTypuKolekcja(dokument, rootElement, wlasciwosc);
        }

        private void UzupelnijDefinicjePolaTypuKolekcja(
            XmlDocument dokument,
            XmlElement rootElement,
            Property wlasciwosc)
        {
            var elementDlaWlasciwosci =
                SzukajElementuWgAttrybutuName(rootElement, wlasciwosc.Nazwa);

            string nazwaKlasy = DajNazweKlasyObiektuKolekcjonowanego(wlasciwosc);

            if (elementDlaWlasciwosci == null)
            {
                elementDlaWlasciwosci = CreateElementDefinicjiObiektu(dokument, wlasciwosc.Nazwa);
                rootElement.AppendChild(elementDlaWlasciwosci);
            }

            var klasaObiektuKolekcjonowanego = DajKlaseWProjekcieWgNazwy(nazwaKlasy);

            UzupelnijDefinicjeWgKlasy(
                klasaObiektuKolekcjonowanego,
                dokument,
                elementDlaWlasciwosci);
        }

        private DefinedItem DajKlaseWProjekcieWgNazwy(string nazwaKlasy)
        {
            var plikZKlasa =
                solution
                    .AktualnyProjekt
                        .Files
                            .Single(o => o.NameWithoutExtension == nazwaKlasy);

            var klasaObiektuKolekcjonowanego =
                Parser
                    .ParsujPlik(plikZKlasa.FullPath)
                        .DefinedItems
                            .Single(o => o.Name == nazwaKlasy);
            return klasaObiektuKolekcjonowanego;
        }

        private string DajNazweKlasyObiektuKolekcjonowanego(Property wlasciwosc)
        {
            var regex = new Regex(@"<([A-Za-z0-9_]+)>");
            var match = regex.Match(wlasciwosc.NazwaTypu);

            var nazwaKlasy = match.Groups[1].Value;
            return nazwaKlasy;
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
            var mapowania = Konfiguracja.GetInstance(solution).MapowaniaTypowXsd();

            var mapowanie = mapowania.SingleOrDefault(o => o.NazwaTypu == nazwaTypu);

            if (mapowanie != null)
                return mapowanie.NazwaWXsd;

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

        private XmlDocument DajNowyDokumentLubWczytajIstniejacy(
            DefinedItem klasaView,
            string sciezkaDoXsd)
        {
            if (File.Exists(sciezkaDoXsd))
            {
                var ladowanyDokument = new XmlDocument();
                ladowanyDokument.Load(sciezkaDoXsd);
                return ladowanyDokument;
            }

            var nowyDokument = new XmlDocument();

            var xmlDeclaration =
                nowyDokument.CreateXmlDeclaration("1.0", "UTF-8", null);

            var root = nowyDokument.DocumentElement;
            nowyDokument.InsertBefore(xmlDeclaration, root);

            var element1 = CreateRootElement(klasaView, nowyDokument);

            element1.AppendChild(CreateElementDefinicjiObiektu(nowyDokument, "header"));

            ZapiszDokument(nowyDokument, sciezkaDoXsd);

            solution.AktualnyProjekt.AddFile(sciezkaDoXsd);

            return nowyDokument;
        }

        private void ZapiszDokument(XmlDocument dokument, string sciezkaDoXsd)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            using (var fileStream = new FileStream(sciezkaDoXsd, FileMode.OpenOrCreate))
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

        private static XmlElement CreateRootElement(DefinedItem klasaView, XmlDocument doc)
        {
            XmlElement element1 = doc.CreateElement("xs", "schema", NamespaceXS);
            doc.AppendChild(element1);

            element1.SetAttribute("id", DajNazweKlasyWXsd(klasaView));
            element1.SetAttribute("targetNamespace", DajTargetNamespace(klasaView));
            element1.SetAttribute("elementFormDefault", "qualified");
            element1.SetAttribute("xmlns", DajTargetNamespace(klasaView));
            element1.SetAttribute("xmlns:mstns", DajTargetNamespace(klasaView));
            element1.SetAttribute("xmlns:xs", "http://www.w3.org/2001/XMLSchema");
            return element1;
        }

        private static string DajNazweKlasyWXsd(DefinedItem klasa)
        {
            var nazwa = klasa.Name;
            return DajNazweKlasyWXsd(nazwa);
        }

        private static string DajNazweKlasyWXsd(string nazwa)
        {
            var stringReportView = "ReportView";
            if (nazwa.EndsWith(stringReportView))
                return nazwa.Substring(0, nazwa.Length - stringReportView.Length);
            else
                return nazwa;
        }

        private static string DajTargetNamespace(DefinedItem klasaView)
        {
            return string.Format("http://tempuri.org/{0}.xsd", DajNazweKlasyWXsd(klasaView));
        }

        private DefinedItem DajKlaseReportView()
        {
            var sparsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            var klasy =
                sparsowane
                    .DefinedItems
                        .Where(o => o.KindOfItem == RodzajObiektu.Klasa);

            DefinedItem klasaView;
            klasaView =
                sparsowane
                    .SzukajKlasyWLinii(solution.AktualnyDokument.GetCursorLineNumber());

            if (klasaView == null)
                klasaView = klasy.FirstOrDefault();

            return klasaView;
        }
    }
}
