﻿
namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public abstract class ParsowanaJednostka
    {
        public PozycjaWPliku Poczatek { get; set; }
        public PozycjaWPliku Koniec { get; set; }

        public ParsowanaJednostka()
        {
            Poczatek = new PozycjaWPliku();
            Koniec = new PozycjaWPliku();
        }
    }
}