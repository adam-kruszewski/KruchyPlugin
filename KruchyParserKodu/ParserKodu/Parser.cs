﻿using System.IO;
using System.Text;
using KruchyParserKodu.Roslyn;

namespace KruchyParserKodu.ParserKodu
{
    public interface IParser
    {
        Plik Parsuj(string zawartosc);
    }

    public class Parser
    {
        static IParser Instance = new RoslynParser();
            //new NRefactoryParser();

        public static Plik Parsuj(string zawartosc)
        {
            return Instance.Parsuj(zawartosc);
        }

        public static Plik ParsujPlik(string nazwaPliku)
        {
            var zawartosc = File.ReadAllText(nazwaPliku, Encoding.UTF8);
            return Parsuj(zawartosc);
        }
    }
}