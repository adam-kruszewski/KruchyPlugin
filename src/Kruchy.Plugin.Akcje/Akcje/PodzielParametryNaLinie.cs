using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class PodzielParametryNaLinie
    {
        private readonly ISolutionWrapper solution;

        public PodzielParametryNaLinie(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Podziel()
        {
            var dokument = solution.AktualnyDokument;
            var parsowane =
                Parser.Parse(dokument.GetContent());

            var metoda = parsowane
                    .FindMethodByLineNumber(dokument.GetCursorLineNumber());

            if (metoda == null)
            {
                var konstruktor =
                    parsowane
                        .FindConstructorByLineNumber(dokument.GetCursorLineNumber());
                if (konstruktor != null)
                    PodzielNaLinieKonstruktor(konstruktor);
                else
                    MessageBox.Show("Kursor nie jest w metodzie");
                return;
            }

            dokument.Remove(
                metoda.StartingParameterBrace.Row,
                metoda.StartingParameterBrace.Column,
                metoda.ClosingParameterBrace.Row,
                metoda.ClosingParameterBrace.Column + 1);

            dokument.InsertInPlace(
                GenerujNoweParametry(metoda.Parametry, metoda, metoda),
                metoda.StartingParameterBrace.Row,
                metoda.StartingParameterBrace.Column);
        }

        private void PodzielNaLinieKonstruktor(Constructor konstruktor)
        {
            var dokument = solution.AktualnyDokument;

            dokument.Remove(
                konstruktor.StartingParameterBrace.Row,
                konstruktor.StartingParameterBrace.Column,
                konstruktor.ClosingParameterBrace.Row,
                konstruktor.ClosingParameterBrace.Column + 1);

            dokument.InsertInPlace(
                GenerujNoweParametry(konstruktor.Parametry, konstruktor),
                konstruktor.StartingParameterBrace.Row,
                konstruktor.StartingParameterBrace.Column);
        }

        private string GenerujNoweParametry(
            IEnumerable<Parameter> parametryMetody,
            IWithOwner obiekt,
            Method metoda = null)
        {
            var builder = new StringBuilder();
            builder.Append("(");
            var parametry =
                parametryMetody
                        .Select(o => DajDefinicjeParametru(o))
                            .ToArray();
            var lacznikBuilder =
                new StringBuilder()
                    .Append(",")
                    .AppendLine()
                    .Append(StaleDlaKodu.WcieciaDlaParametruMetody);

            var poziomMetody = WyliczPoziomMetody(obiekt.Owner);

            DodajWciecieWgPoziomuMetody(lacznikBuilder, poziomMetody);

            var lacznik = lacznikBuilder.ToString();
            if (parametry.Any())
            {
                builder.AppendLine();
                builder.Append(StaleDlaKodu.WcieciaDlaParametruMetody);
                DodajWciecieWgPoziomuMetody(builder, poziomMetody);
            }
            builder.Append(string.Join(lacznik, parametry));
            builder.Append(")");
            return builder.ToString();
        }

        private void DodajWciecieWgPoziomuMetody(
            StringBuilder lacznikBuilder,
            int poziomMetody)
        {
            if (poziomMetody > 1)
                for (int i = 0; i < poziomMetody - 1; i++)
                    lacznikBuilder.Append(StaleDlaKodu.JednostkaWciecia);
        }

        private int WyliczPoziomMetody(IWithOwner obiekt)
        {
            if (obiekt == null)
                return 0;

            if (obiekt.Owner == null)
                return 1;

            return WyliczPoziomMetody(obiekt.Owner) + 1;
        }

        private string DajDefinicjeParametru(Parameter parametr)
        {
            var builder = new StringBuilder();

            foreach (var atrybut in parametr.Attributes)
            {
                var atrybutBuilder = new AtrybutBuilder().ZNazwa(atrybut.Name);
                foreach (var parametrAtrybutu in atrybut.Parameters)
                {
                    atrybutBuilder.DodajWartoscParametruNieStringowa(parametrAtrybutu.Value);
                }

                builder.Append(atrybutBuilder.Build(true));
                builder.Append(" ");
            }

            if (parametr.WithThis)
                builder.Append("this ");

            if (parametr.WithParams)
                builder.Append("params ");
            if (parametr.WithOut)
                builder.Append("out ");
            if (parametr.WithRef)
                builder.Append("ref ");

            builder.Append(parametr.TypeName + " ");
            builder.Append(parametr.ParameterName);
            builder.Append(DajOpisWartosciDomyslnej(parametr));

            return builder.ToString();
        }

        private string DajOpisWartosciDomyslnej(Parameter parametr)
        {
            if (!string.IsNullOrEmpty(parametr.DefaultValue))
                return " = " + parametr.DefaultValue;
            else
                return string.Empty;
        }
    }
}
