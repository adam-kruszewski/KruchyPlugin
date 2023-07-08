using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class DefinedItem
        : ParsedUnit
            , IWithName
                , IWithBraces
                    , IWithOwner
                        , IWithComment
                            , IWithDocumentation
    {
        public KindOfItem KindOfItem
        {
            get => KindOfObjectUnit.KindOfItem;
            set
            {
                KindOfObjectUnit.KindOfItem = value;
            }
        }

        public KindOfItemUnit KindOfObjectUnit { get; set; }

        public DefinedItem Owner { get; set; }

        public string Name { get; set; }
        public IList<Constructor> Constructors { get; private set; }
        public IList<Field> Fields { get; private set; }
        public IList<Field> NonStaticFields
        {
            get { return FindNonStaticFields().ToList(); }
        }

        public IList<Modifier> Modifiers { get; set; }
        public IList<Property> Properties { get; private set; }
        public IList<Method> Methods { get; private set; }
        public List<Attribute> Attributes { get; private set; }
        public IList<DerivedObject> SuperClassAndInterfaces { get; private set; }

        public IList<DefinedItem> InternalDefinedItems { get; private set; }

        public PlaceInFile StartingBrace { get; set; }
        public PlaceInFile ClosingBrace { get; set; }

        public Comment Comment { get; set; }

        public Documentation Documentation { get; set; }

        public IList<GenericParameter> GenericParameters { get; set; }

        public DefinedItem() : base()
        {
            Constructors = new List<Constructor>();
            Fields = new List<Field>();
            Properties = new List<Property>();
            Methods = new List<Method>();
            Attributes = new List<Attribute>();
            SuperClassAndInterfaces = new List<DerivedObject>();

            StartingBrace = new PlaceInFile();
            ClosingBrace = new PlaceInFile();
            InternalDefinedItems = new List<DefinedItem>();
            Modifiers = new List<Modifier>();

            KindOfObjectUnit = new KindOfItemUnit();
            GenericParameters = new List<GenericParameter>();
        }

        private IEnumerable<Field> FindNonStaticFields()
        {
            return Fields.Where(o => !o.Modifiers.Any(p => p.Name == "static"));
        }
    }
}