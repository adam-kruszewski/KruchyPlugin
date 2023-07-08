using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class DefinedItem
        : ParsowanaJednostka
            , IWithName
                , IWithBraces
                    , IWithOwner
                        , IWithComment
                            , IWithDocumentation
    {
        public RodzajObiektu Rodzaj
        {
            get => RodzajObiektuObiekt.RodzajObiektu;
            set
            {
                RodzajObiektuObiekt.RodzajObiektu = value;
            }
        }

        public RodzajObiektuObiekt RodzajObiektuObiekt { get; set; }

        public DefinedItem Owner { get; set; }

        public string Name { get; set; }
        public IList<Constructor> Konstruktory { get; private set; }
        public IList<Pole> Pola { get; private set; }
        public IList<Pole> NiestatycznePola
        {
            get { return SzukajPolNiestatycznych().ToList(); }
        }

        public IList<Modifier> Modyfikatory { get; set; }
        public IList<Property> Propertiesy { get; private set; }
        public IList<Method> Metody { get; private set; }
        public List<Attribute> Atrybuty { get; private set; }
        public IList<ObiektDziedziczony> NadklasaIInterfejsy { get; private set; }

        public IList<DefinedItem> ObiektyWewnetrzne { get; private set; }

        public PlaceInFile StartingBrace { get; set; }
        public PlaceInFile ClosingBrace { get; set; }

        public Comment Comment { get; set; }

        public Documentation Documentation { get; set; }

        public IList<ParametrGeneryczny> ParametryGeneryczne { get; set; }

        public DefinedItem() : base()
        {
            Konstruktory = new List<Constructor>();
            Pola = new List<Pole>();
            Propertiesy = new List<Property>();
            Metody = new List<Method>();
            Atrybuty = new List<Attribute>();
            NadklasaIInterfejsy = new List<ObiektDziedziczony>();

            StartingBrace = new PlaceInFile();
            ClosingBrace = new PlaceInFile();
            ObiektyWewnetrzne = new List<DefinedItem>();
            Modyfikatory = new List<Modifier>();

            RodzajObiektuObiekt = new RodzajObiektuObiekt();
            ParametryGeneryczne = new List<ParametrGeneryczny>();
        }

        private IEnumerable<Pole> SzukajPolNiestatycznych()
        {
            return Pola.Where(o => !o.Modyfikatory.Any(p => p.Name == "static"));
        }
    }
}