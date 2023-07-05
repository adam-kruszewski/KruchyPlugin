
namespace KruchyParserKodu.ParserKodu.Models
{
    public class PlaceInFile
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public PlaceInFile() { }

        public PlaceInFile(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}