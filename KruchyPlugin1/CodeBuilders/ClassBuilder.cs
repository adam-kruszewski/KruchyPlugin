﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyCompany.KruchyPlugin1.CodeBuilders
{
    class ClassBuilder : ICodeBuilder
    {
        private string modyfikator { get; set; }
        private string nazwa { get; set; }
        private string nazwaNadklasy { get; set; }
        private IList<ICodeBuilder> metody { get; set; }
        private IList<ICodeBuilder> konstruktory { get; set; }
        private IList<ICodeBuilder> atrybuty { get; set; }

        public ClassBuilder()
        {
            metody = new List<ICodeBuilder>();
            konstruktory = new List<ICodeBuilder>();
            atrybuty = new List<ICodeBuilder>();
        }

        public ClassBuilder ZNazwa(string nazwa)
        {
            this.nazwa = nazwa;
            return this;
        }

        public ClassBuilder ZModyfikatorem(string modyfikator)
        {
            this.modyfikator = modyfikator;
            return this;
        }

        public ClassBuilder ZNadklasa(string nazwa)
        {
            nazwaNadklasy = nazwa;
            return this;
        }

        public ClassBuilder DodajKonstruktor(ICodeBuilder konstruktor)
        {
            konstruktory.Add(konstruktor);
            return this;
        }

        public ClassBuilder DodajMetode(ICodeBuilder metoda)
        {
            metody.Add(metoda);
            return this;
        }

        public ClassBuilder DodajAtrybut(ICodeBuilder atrybut)
        {
            atrybuty.Add(atrybut);
            return this;
        }

        public string Build(string wciecie = "")
        {
            var outputBuilder = new StringBuilder();

            foreach (var a in atrybuty)
                outputBuilder.Append(a.Build(StaleDlaKodu.JednostkaWciecia));

            outputBuilder.Append(wciecie);
            if (!string.IsNullOrEmpty(modyfikator))
                outputBuilder.Append(modyfikator + " ");
            outputBuilder.Append("class ");
            outputBuilder.Append(nazwa);
            if (!string.IsNullOrEmpty(nazwaNadklasy))
                outputBuilder.Append(" : " + nazwaNadklasy);
            outputBuilder.AppendLine();
            outputBuilder.AppendLine(wciecie + "{");
            
            foreach (var k in konstruktory)
                outputBuilder.AppendLine(
                    k.Build(StaleDlaKodu.WielokrotnoscWciecia(2)));

            foreach (var m in metody)
                outputBuilder.AppendLine(
                    m.Build(StaleDlaKodu.WielokrotnoscWciecia(2)));
            
            outputBuilder.AppendLine(wciecie + "}");
            return outputBuilder.ToString();
        }
    }
}
