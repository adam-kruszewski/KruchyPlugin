using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Konstruktor
        : MethodConstructorBase
                , IWithBraces
    {
        public string Modyfikator { get; set; }

        public PozycjaWPliku PoczatekParametrow { get; private set; }
        public PozycjaWPliku KoniecParametrow { get; private set; }
        public PozycjaWPliku StartingBrace { get; set; }
        public PozycjaWPliku ClosingBrace { get; set; }

        //jeśli nie ma wołania kontruktora z nadklasy, to pole jest nullem
        public IList<string> ParametryKonstruktoraZNadKlasy;
        public string SlowoKluczoweInicjalizacji { get; set; }

        public Konstruktor() : base()
        {
            StartingBrace = new PozycjaWPliku();
            ClosingBrace = new PozycjaWPliku();
            
        }
    }
}