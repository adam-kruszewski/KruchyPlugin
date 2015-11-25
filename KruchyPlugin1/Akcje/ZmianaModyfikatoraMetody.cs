using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KruchyCompany.KruchyPlugin1.CodeBuilders;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class ZmianaModyfikatoraMetody
    {
        private readonly DokumentWrapper dokument;
        private readonly string[] modyfikatory = 
            {"public", "private", "internal", "protected"};


        public ZmianaModyfikatoraMetody(DokumentWrapper dokument)
        {
            this.dokument = dokument;
        }

        public void ZmienNa(string modyfikator)
        {
            var liniaKursora = dokument.DajNumerLiniiKursora();
            var zawartoscLinii = dokument.DajZawartoscLinii(liniaKursora);

            var slowa = zawartoscLinii.Trim().Split(new char[] {' ', '\t'});
            if (modyfikatory.Contains(slowa[0]))
                ZmienModyfikatorWLinii(modyfikator, liniaKursora, slowa[0]);
            else
                DodajModyfikatorWLinii(modyfikator, liniaKursora, zawartoscLinii);
        }

        private void DodajModyfikatorWLinii(
            string modyfikator,
            int numerLinii,
            string zawartoscLinii)
        {
            if (zawartoscLinii.Length < StaleDlaKodu.WciecieDlaMetody.Length)
                return;

            dokument.WstawWMiejscu(
                modyfikator + " ",
                numerLinii,
                1 + StaleDlaKodu.WciecieDlaMetody.Length);
        }

        private void ZmienModyfikatorWLinii(
            string modyfikator, int numerLinii, string modyfikatorObecny)
        {
            var linia = dokument.DajZawartoscLinii(numerLinii);
            int index = 0;
            while (linia[index] == ' ' || linia[index] == '\t')
                index++;
            dokument.UsunWMiejscu(numerLinii, index + 1, modyfikatorObecny.Length);
            dokument.WstawWMiejscu(modyfikator, numerLinii, index + 1);
        }
    }
}
