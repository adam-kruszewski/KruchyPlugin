﻿using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Metoda
        : MethodConstructorBase
            , IZNawiasamiOtwierajacymiZamykajacymiParametry
                , IZWlascicielem
                    , IZKomentarzem
                        , IZDokumentacja
    {
        public IList<ParametrGeneryczny> ParametryGeneryczne { get; set; }

        public TypZwracany TypZwracany { get; set; }

        public string Nazwa { get; set; }

        public Metoda()
        {
            ParametryGeneryczne = new List<ParametrGeneryczny>();
        }

        public override string ToString()
        {
            return Nazwa;
        }
    }
}