using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKodu.Roslyn;
using System.IO;
using System.Text;

namespace KruchyParserKodu.ParserKodu
{
    public interface IParser
    {
        FileWithCode Parse(string content);
    }

    public class Parser
    {
        static IParser Instance = new RoslynParser();

        public static FileWithCode Parse(string content)
        {
            return Instance.Parse(content);
        }

        public static FileWithCode ParseFile(string fileName)
        {
            var content = File.ReadAllText(fileName, Encoding.UTF8);
            return Parse(content);
        }
    }
}