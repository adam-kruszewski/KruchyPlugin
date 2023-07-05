using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kruchy.Plugin.Pincasso.Akcje.Akcje
{
    public class UzupelnianieTagowDefiniujacychTabele
    {
        private const string NamespaceDlaAtrybutowOpisujacychTabele =
            "KomponentyStandardowe.Data";

        private readonly IDocumentWrapper dokument;

        public UzupelnianieTagowDefiniujacychTabele(IDocumentWrapper dokument)
        {
            this.dokument = dokument;
        }

        public void Uzupelnij()
        {
            var parsowane = Parser.Parsuj(dokument.GetContent());

            var prefiks = SzukajPrefiksu();
            var numerLiniiClass = DajNumerLiniiZClass(parsowane);
            DodajAtrybutKlasie(prefiks, parsowane);
            parsowane = Parser.Parsuj(dokument.GetContent());
            List<int> linieZKolumnami = ZnajdzLinieZKolumnami(parsowane);
            DodajAtrybutyKolumnowe(linieZKolumnami, prefiks);
            dokument.DodajUsingaJesliTrzeba(NamespaceDlaAtrybutowOpisujacychTabele);
        }

        private string SzukajPrefiksu()
        {
            var liczbaLinii = dokument.GetLineCount();
            for (int i = 1; i <= liczbaLinii; i++)
            {
                var linia = dokument.GetLineContent(i);
                if (linia.ToLower().Contains("//prefix="))
                    return linia.ToLower().Trim().Replace("//prefix=", "");
            }
            return "";
        }

        private int DajNumerLiniiZClass(Plik plik)
        {
            if (plik.DefiniowaneObiekty.Count != 1)
                throw new ApplicationException(
                    "Musi być zdefiniowana dokładnie jedna klasa");

            return plik.DefiniowaneObiekty.First().Poczatek.Row;
        }

        private void DodajAtrybutKlasie(string prefiks, Plik plik)
        {
            var liniaZClass = DajNumerLiniiZClass(plik);
            dokument.InsertInLine(
                PrzygotujTableDescription(prefiks), liniaZClass);
        }

        private static string PrzygotujTableDescription(string prefiks)
        {
            if (prefiks.EndsWith("_"))
                prefiks = prefiks.Substring(0, prefiks.Length - 1);

            var builder =
                new StringBuilder()
                    .Append("    [TableDescription(\"<NAZWA_TABELI>\", \"")
                    .Append(prefiks)
                    .Append("_id\")]")
                    .AppendLine();
            return builder.ToString();
        }

        private List<int> ZnajdzLinieZKolumnami(Plik plik)
        {
            var wynik = new List<int>();

            var propertiesyKolumn = plik
                .DefiniowaneObiekty
                    .First()
                        .Propertiesy
                            .Where(o => o.JestGet && o.JestSet)
                                .Where(o => !MaAtrybutuReferencedObject(o));
            return propertiesyKolumn.Select(o => o.Poczatek.Row).ToList();
        }

        private bool MaAtrybutuReferencedObject(Property property)
        {
            return property.Atrybuty.Any(o => o.Nazwa == "ReferencedObject");
        }

        private void DodajAtrybutyKolumnowe(List<int> linieKolumn, string prefiks)
        {
            var szablonAtrybutu =
                "        [ColumnName(\"" + prefiks + "\")]"
                + new StringBuilder().AppendLine().ToString();
            for (int i = 0; i < linieKolumn.Count; i++)
            {
                dokument.InsertInLine(szablonAtrybutu, i + linieKolumn[i]);
            }
        }
    }
}
