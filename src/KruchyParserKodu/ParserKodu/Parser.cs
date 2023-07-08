using System.IO;
using System.Text;
using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKodu.Roslyn;

namespace KruchyParserKodu.ParserKodu
{
    public interface IParser
    {
        FileWithCode Parsuj(string zawartosc);
    }

    public class Parser
    {
        static IParser Instance = new RoslynParser();
            //new NRefactoryParser();

        public static FileWithCode Parsuj(string zawartosc)
        {
            return Instance.Parsuj(zawartosc);
        }

        public static FileWithCode ParsujPlik(string nazwaPliku)
        {
            var zawartosc = File.ReadAllText(nazwaPliku, Encoding.UTF8);
            return Parsuj(zawartosc);
        }
    }
}