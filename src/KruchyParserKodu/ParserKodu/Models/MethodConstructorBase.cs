﻿using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class MethodConstructorBase
        : ParsowanaJednostka
            , IWithParameterBraces
                , IWithOwner
                    , IWithComment
                        , IWithDocumentation
    {
        public Obiekt Owner { get; set; }

        public IList<Parametr> Parametry { get; private set; }

        public IList<Modyfikator> Modyfikatory { get; set; }

        public List<Atrybut> Atrybuty { get; private set; }

        public PozycjaWPliku StartingParameterBrace { get; set; }

        public PozycjaWPliku ClosingParameterBrace { get; set; }

        public Comment Komentarz { get; set; }

        public Documentation Dokumentacja { get; set; }

        public IList<Instruction> Instructions { get; private set; }

        public bool Prywatna
        {
            get
            {
                return Modyfikatory.Any(o => o.Nazwa == "private");
            }
        }

        public bool Publiczna
        {
            get
            {
                return Modyfikatory.Any(o => o.Nazwa == "public");
            }
        }

        public MethodConstructorBase()
        {
            Parametry = new List<Parametr>();
            Modyfikatory = new List<Modyfikator>();
            Atrybuty = new List<Atrybut>();
            StartingParameterBrace = new PozycjaWPliku();
            ClosingParameterBrace = new PozycjaWPliku();
            Instructions = new List<Instruction>();
        }

        public bool ZawieraModyfikator(string nazwa)
        {
            return Modyfikatory.Any(o => o.Nazwa == nazwa);
        }
    }
}
