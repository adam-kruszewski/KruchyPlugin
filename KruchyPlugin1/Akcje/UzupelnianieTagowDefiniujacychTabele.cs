using System;
using System.Collections.Generic;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class UzupelnianieTagowDefiniujacychTabele
    {
        private const string NamespaceDlaAtrybutowOpisujacychTabele =
            "KomponentyStandardowe.Data";

        private readonly DokumentWrapper dokument;

        public UzupelnianieTagowDefiniujacychTabele(DokumentWrapper dokument)
        {
            this.dokument = dokument;
        }

        public void Uzupelnij()
        {
            var prefiks = SzukajPrefiksu();
            var numerLiniiClass = DajNumerLiniiZClass();
            DodajAtrybutKlasie(prefiks);
            List<int> linieZKolumnami = ZnajdzLinieZKolumnami();
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

        private int DajNumerLiniiZClass()
        {
            var liczbaLinii = dokument.DajLiczbeLinii();
            for (int i = 1; i <= liczbaLinii; i++)
            {
                if (dokument.DajZawartoscLinii(i).Contains("class "))
                    return i;
            }
            throw new ApplicationException("Brak linii rozpoczynającej klasę");
        }

        private void DodajAtrybutKlasie(string prefiks)
        {
            var liniaZClass = DajNumerLiniiZClass();
            dokument.WstawWLinii(
"    [TableDescription(\"NAZWA_TABELI\", \"" + prefiks + "_id\")]\n", liniaZClass);
        }

        private List<int> ZnajdzLinieZKolumnami()
        {
            var wynik = new List<int>();

            var liczbaLinii = dokument.DajLiczbeLinii();
            for (int i = DajNumerLiniiZClass(); i <= liczbaLinii; i++)
            {
                var linia = dokument.DajZawartoscLinii(i);
                if (linia.TrimStart().StartsWith("//"))
                    continue;
                if (linia.StartsWith("["))
                    continue;
                if (WPoprzednichLiniachJestAtrybuReferencedObject(i))
                    continue;

                if (linia.Contains("get;") && linia.Contains("set;"))
                    wynik.Add(i);
            }
            return wynik;
        }

        private bool WPoprzednichLiniachJestAtrybuReferencedObject(int numerLinii)
        {
            for (int i = numerLinii - 1; i >= 1; i--)
            {
                var linia =
                    dokument.DajZawartoscLinii(i).Trim();
                if (!linia.StartsWith("["))
                    return false;
                if (linia.StartsWith("[ReferencedObject"))
                    return true;
            }
            return false;
        }

        private void DodajAtrybutyKolumnowe(List<int> linieKolumn, string prefiks)
        {
            var szablonAtrybutu = "        [ColumnName(\"" + prefiks + "\")]\n";
            for (int i = 0; i < linieKolumn.Count; i++)
            {
                dokument.WstawWLinii(szablonAtrybutu, i + linieKolumn[i]);
            }
        }
    }
}
