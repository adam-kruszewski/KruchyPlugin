using System;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Documentation : ParsowanaJednostka
    {
        public List<string> Lines { get; set; }

        public string FullText { get; private set; }

        public Documentation()
        {
            Lines = new List<string>();
        }

        public void AddDocumentation(string fullText)
        {
            FullText = fullText;

            var originalLines = fullText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var trimmedLines = originalLines.Select(o => o.Replace("///", "").Trim());

            Lines.AddRange(trimmedLines);
        }
    }
}