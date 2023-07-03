using System;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Dokumentacja : ParsowanaJednostka
    {
        public List<string> Linie { get; set; }

        public string PelnyTeskt { get; private set; }

        public Dokumentacja()
        {
            Linie = new List<string>();
        }

        public void DodajDokumentacje(string pelnyText)
        {
            PelnyTeskt = pelnyText;

            var linieOryginalne = pelnyText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var liniePrzyciete = linieOryginalne.Select(o => o.Replace("///", "").Trim());

            Linie.AddRange(liniePrzyciete);
        }
    }
}