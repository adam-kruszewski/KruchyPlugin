
namespace KruchyParserKodu.ParserKodu.Models
{
    public class PozycjaWPliku
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public PozycjaWPliku() { }

        public PozycjaWPliku(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}