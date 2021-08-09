using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrucheBuilderyKodu.Builders
{
    public class MetodaBuilder : ICodeBuilder
    {
        private IList<string> modyfikatory;
        private string nazwa;
        private string typZwracany;
        private IList<KeyValuePair<string, string>> parametry;
        private IList<ICodeBuilder> atrybuty;
        private IList<string> linie;
        private bool jedenParametrWLinii = false;
        private bool rozszerzajaca = false;
        private string inicjalizacjaKonstruktora = null;
        private IEnumerable<string> parametryInicjalizacjiKontruktora = null;

        public MetodaBuilder()
        {
            typZwracany = "void";
            modyfikatory = new List<string>();
            parametry = new List<KeyValuePair<string, string>>();
            atrybuty = new List<ICodeBuilder>();
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

        public MetodaBuilder DodajAtrybut(ICodeBuilder builder)
        {
            atrybuty.Add(builder);
            return this;
        }

        public MetodaBuilder DodajLinie(string linia)
        {
            linie.Add(linia);
            return this;
        }

        public MetodaBuilder JedenParametrWLinii(bool jedenWLinii)
        {
            this.jedenParametrWLinii = jedenWLinii;
            return this;
        }

        public MetodaBuilder Rozszerzajaca(bool rozszerzajaca)
        {
            this.rozszerzajaca = rozszerzajaca;
            return this;
        }

        public MetodaBuilder DodajInicjalizacjeKonstruktora(string slowoKluczowe, IEnumerable<string> parametry)
        {
            inicjalizacjaKonstruktora = slowoKluczowe;
            parametryInicjalizacjiKontruktora = parametry.ToList();
            return this;
        }

        public string Build(string wciecie = "")
        {
            var builder = new StringBuilder();

            DopiszAtrybuty(wciecie, builder);

            builder.Append(wciecie);
            DopiszModyfikatory(builder);
            DopiszTypZwracany(builder);
            builder.Append(nazwa);
            builder.Append("(");
            if (rozszerzajaca)
                builder.Append("this ");
            var par = parametry.Select(o => o.Key + " " + o.Value).ToArray();

            string lacznik = PrzygotujLacznikParametrow(builder);

            builder.Append(string.Join(lacznik, par));
            builder.Append(")");
            DopiszInicjalizacjeKonstruktoraJesliTrzeba(builder);
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

        private void DopiszInicjalizacjeKonstruktoraJesliTrzeba(StringBuilder builder)
        {
            if (!string.IsNullOrEmpty(inicjalizacjaKonstruktora))
            {
                builder.Append(" : ");
                builder.Append(inicjalizacjaKonstruktora);
                builder.Append("(");

                var parametry = string.Join(", ", parametryInicjalizacjiKontruktora);
                builder.Append(parametry);

                builder.Append(")");

            }
        }

        private string PrzygotujLacznikParametrow(StringBuilder builder)
        {
            var lacznik = ", ";
            if (jedenParametrWLinii)
            {
                var lacznikBuilder =
                    new StringBuilder()
                        .Append(",")
                        .AppendLine()
                        .Append(StaleDlaKodu.WciecieDlaMetody)
                        .Append(StaleDlaKodu.JednostkaWciecia);
                lacznik = lacznikBuilder.ToString();
                builder.AppendLine();
                builder.Append(StaleDlaKodu.WciecieDlaMetody + StaleDlaKodu.JednostkaWciecia);
            }

            return lacznik;
        }

        private void DopiszAtrybuty(string wciecie, StringBuilder builder)
        {
            foreach (var attr in atrybuty)
                builder.Append(attr.Build(wciecie));
        }

        private void DopiszTypZwracany(StringBuilder builder)
        {
            if (!string.IsNullOrEmpty(typZwracany))
                builder.Append(typZwracany + " ");
        }

        private void DopiszModyfikatory(StringBuilder builder)
        {
            builder.Append(string.Join(" ", modyfikatory));
            if (modyfikatory.Any(o => !string.IsNullOrEmpty(o)))
                builder.Append(" ");
        }
    }
}
