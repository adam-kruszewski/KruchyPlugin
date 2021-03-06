﻿using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu
{
    public class Obiekt
        : ParsowanaJednostka
            , IZPoczatkowaIKoncowaKlamerka
                , IZWlascicielem
    {
        public RodzajObiektu Rodzaj { get; set; }
        public Obiekt Wlasciciel { get; set; }

        public string Nazwa { get; set; }
        public IList<Konstruktor> Konstruktory { get; private set; }
        public IList<Pole> Pola { get; private set; }
        public IList<Pole> NiestatycznePola
        {
            get { return SzukajPolNiestatycznych().ToList(); }
        }

        public IList<Modyfikator> Modyfikatory { get; set; }
        public IList<Property> Propertiesy { get; private set; }
        public IList<Metoda> Metody { get; private set; }
        public List<Atrybut> Atrybuty { get; private set; }
        public IList<ObiektDziedziczony> NadklasaIInterfejsy { get; private set; }

        public IList<Obiekt> ObiektyWewnetrzne { get; private set; }

        public PozycjaWPliku PoczatkowaKlamerka { get; set; }
        public PozycjaWPliku KoncowaKlamerka { get; set; }

        public Obiekt() : base()
        {
            Konstruktory = new List<Konstruktor>();
            Pola = new List<Pole>();
            Propertiesy = new List<Property>();
            Metody = new List<Metoda>();
            Atrybuty = new List<Atrybut>();
            NadklasaIInterfejsy = new List<ObiektDziedziczony>();

            PoczatkowaKlamerka = new PozycjaWPliku();
            KoncowaKlamerka = new PozycjaWPliku();
            ObiektyWewnetrzne = new List<Obiekt>();
            Modyfikatory = new List<Modyfikator>();
        }

        private IEnumerable<Pole> SzukajPolNiestatycznych()
        {
            return Pola.Where(o => !o.Modyfikatory.Any(p => p.Nazwa == "static"));
        }
    }
}