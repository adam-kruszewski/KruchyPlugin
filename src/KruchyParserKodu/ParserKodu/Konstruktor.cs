using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Konstruktor
        : MethodConstructorBase
                , IZPoczatkowaIKoncowaKlamerka
    {
        public string Modyfikator { get; set; }

        public PozycjaWPliku PoczatekParametrow { get; private set; }
        public PozycjaWPliku KoniecParametrow { get; private set; }
        public PozycjaWPliku PoczatkowaKlamerka { get; set; }
        public PozycjaWPliku KoncowaKlamerka { get; set; }

        //jeśli nie ma wołania kontruktora z nadklasy, to pole jest nullem
        public IList<string> ParametryKonstruktoraZNadKlasy;
        public string SlowoKluczoweInicjalizacji { get; set; }

        public Konstruktor() : base()
        {
            PoczatkowaKlamerka = new PozycjaWPliku();
            KoncowaKlamerka = new PozycjaWPliku();
            
        }
    }
}