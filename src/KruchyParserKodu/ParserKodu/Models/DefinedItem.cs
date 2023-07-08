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
        public RodzajObiektu KindOfItem
        {
            get => KindOfObjectUnit.RodzajObiektu;
            set
            {
                KindOfObjectUnit.RodzajObiektu = value;
            }
        }

        public RodzajObiektuObiekt KindOfObjectUnit { get; set; }

        public DefinedItem Owner { get; set; }

        public string Name { get; set; }
        public IList<Constructor> Constructors { get; private set; }
        public IList<Pole> Fields { get; private set; }
        public IList<Pole> NonStaticFields
        {
            get { return FindNonStaticFields().ToList(); }
        }

        public IList<Modifier> Modifiers { get; set; }
        public IList<Property> Properties { get; private set; }
        public IList<Method> Methods { get; private set; }
        public List<Attribute> Attributes { get; private set; }
        public IList<ObiektDziedziczony> SuperClassAndInterfaces { get; private set; }

        public IList<DefinedItem> InternalDefinedItems { get; private set; }

        public PlaceInFile StartingBrace { get; set; }
        public PlaceInFile ClosingBrace { get; set; }

        public Comment Comment { get; set; }

        public Documentation Documentation { get; set; }

        public IList<ParametrGeneryczny> GenericParameters { get; set; }

        public DefinedItem() : base()
        {
            Constructors = new List<Constructor>();
            Fields = new List<Pole>();
            Properties = new List<Property>();
            Methods = new List<Method>();
            Attributes = new List<Attribute>();
            SuperClassAndInterfaces = new List<ObiektDziedziczony>();

            StartingBrace = new PlaceInFile();
            ClosingBrace = new PlaceInFile();
            InternalDefinedItems = new List<DefinedItem>();
            Modifiers = new List<Modifier>();

            KindOfObjectUnit = new RodzajObiektuObiekt();
            GenericParameters = new List<ParametrGeneryczny>();
        }

        private IEnumerable<Pole> FindNonStaticFields()
        {
            return Fields.Where(o => !o.Modyfikatory.Any(p => p.Name == "static"));
        }
    }
}