using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Comment : ParsowanaJednostka
    {
        public List<string> Lines { get; set; }

        public Comment()
        {
            Lines = new List<string>();
        }

        public void AddLine(string line)
        {
            Lines.Add(line);
        }

        public void AddFullComment(string comment)
        {

        }
    }
}
