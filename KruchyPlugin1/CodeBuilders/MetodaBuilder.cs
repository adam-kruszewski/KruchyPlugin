﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyCompany.KruchyPlugin1.CodeBuilders
{
    class MetodaBuilder : ICodeBuilder
    {
        private IList<string> modyfikatory;
        private string nazwa;
        private string typZwracany;
        private IList<KeyValuePair<string, string>> parametry;
        private IList<string> linie;

        public MetodaBuilder()
        {
            typZwracany = "void";
            modyfikatory = new List<string>();
            parametry = new List<KeyValuePair<string, string>>();
            linie = new List<string>();
        }

        public MetodaBuilder ZNazwa(string nazwa)
        {
            this.nazwa = nazwa;
            return this;
        }

        public MetodaBuilder ZTypemZwracanym(string typ)
        {
            typZwracany = typ;
            return this;
        }

        public MetodaBuilder DodajModyfikator(string modyfikator)
        {
            modyfikatory.Add(modyfikator);
            return this;
        }

        public MetodaBuilder DodajParametr(string typ, string nazwa)
        {
            var nowy = new KeyValuePair<string, string>(typ, nazwa);
            parametry.Add(nowy);
            return this;
        }

        public MetodaBuilder DodajLinie(string linia)
        {
            linie.Add(linia);
            return this;
        }

        public string Build(string wciecie = "")
        {
            var builder = new StringBuilder();

            builder.Append(wciecie);
            builder.Append(string.Join(" ", modyfikatory));
            if (modyfikatory.Any())
                builder.Append(" ");
            builder.Append(typZwracany + " ");
            builder.Append(nazwa);
            builder.Append("(");
            var par = parametry.Select(o => o.Key + " " + o.Value).ToArray();
            builder.Append(string.Join(", ", par));
            builder.Append(")");
            builder.AppendLine();

            builder.AppendLine(wciecie + "{");

            foreach (var linia in linie)
            {
                if (linia.Length > 0)
                    builder.AppendLine(wciecie + StaleDlaKodu.JednostkaWciecia + linia);
                else
                    builder.AppendLine();
            }
            builder.AppendLine(wciecie + "}");
            return builder.ToString();
        }
    }
}