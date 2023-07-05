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

        public PlaceInFile PoczatekParametrow { get; private set; }
        public PlaceInFile KoniecParametrow { get; private set; }
        public PlaceInFile StartingBrace { get; set; }
        public PlaceInFile ClosingBrace { get; set; }

        //jeśli nie ma wołania kontruktora z nadklasy, to pole jest nullem
        public IList<string> ParametryKonstruktoraZNadKlasy;
        public string SlowoKluczoweInicjalizacji { get; set; }

        public Konstruktor() : base()
        {
            StartingBrace = new PlaceInFile();
            ClosingBrace = new PlaceInFile();
            
        }
    }
}