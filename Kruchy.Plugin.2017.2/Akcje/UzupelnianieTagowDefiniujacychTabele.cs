using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class UzupelnianieTagowDefiniujacychTabele
    {
        private const string NamespaceDlaAtrybutowOpisujacychTabele =
            "KomponentyStandardowe.Data";

        private readonly IDokumentWrapper dokument;

        public UzupelnianieTagowDefiniujacychTabele(IDokumentWrapper dokument)
        {
            this.dokument = dokument;
        }

        public void Uzupelnij()
        {
            var parsowane = Parser.Parsuj(dokument.DajZawartosc());

            var prefiks = SzukajPrefiksu();
            var numerLiniiClass = DajNumerLiniiZClass(parsowane);
            DodajAtrybutKlasie(prefiks, parsowane);
            parsowane = Parser.Parsuj(dokument.DajZawartosc());
            List<int> linieZKolumnami = ZnajdzLinieZKolumnami(parsowane);
            DodajAtrybutyKolumnowe(linieZKolumnami, prefiks);
            dokument.DodajUsingaJesliTrzeba(NamespaceDlaAtrybutowOpisujacychTabele);
        }

        private string SzukajPrefiksu()
        {
            var liczbaLinii = dokument.DajLiczbeLinii();
            for (int i = 1; i <= liczbaLinii; i++)
            {
                var linia = dokument.DajZawartoscLinii(i);
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

            return plik.DefiniowaneObiekty.First().Poczatek.Wiersz;
        }

        private void DodajAtrybutKlasie(string prefiks, Plik plik)
        {
            var liniaZClass = DajNumerLiniiZClass(plik);
            dokument.WstawWLinii(
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
            return propertiesyKolumn.Select(o => o.Poczatek.Wiersz).ToList();
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
                dokument.WstawWLinii(szablonAtrybutu, i + linieKolumn[i]);
            }
        }
    }
}