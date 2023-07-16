using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCodeBuilders.Builders;
using KruchyCodeBuilders.Utils;
using KruchyParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using System.Linq;
using System.Text;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class InicjowaniePolaWKonstruktorze
    {
        private readonly ISolutionWrapper solution;

        public InicjowaniePolaWKonstruktorze(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Inicjuj()
        {
            string nazwaDoZainicjowania = null;
            string typDoZainicjowania = null;

            var parsowane =
                solution.ParsujZawartoscAktualnegoDokumetu();
            int numerLinii = solution.CurentDocument.GetCursorLineNumber();
            var properties =
                parsowane
                    .FindPropertyByLineNumber(numerLinii);

            if (properties != null)
            {
                nazwaDoZainicjowania = properties.Name;
                typDoZainicjowania = properties.TypeName;
            }
            else
            {
                var pole = parsowane.FindFieldByLineNumber(numerLinii);
                if (pole == null)
                    return;

                nazwaDoZainicjowania = pole.Name;
                typDoZainicjowania = pole.TypeName;
            }

            Inicjuj(nazwaDoZainicjowania, typDoZainicjowania, parsowane, numerLinii);
        }

        private void Inicjuj(string nazwa, string typ, FileWithCode parsowane, int numerLinii)
        {
            var klasa = parsowane.FindClassByLineNumber(numerLinii);

            var poziomKlasy = klasa.WyliczPoziomMetody();

            if (klasa.Constructors.Any())
            {
                var konstruktor = klasa.Constructors.First();
                solution.CurentDocument.InsertInLine(
                    DajZawartoscDoDodania(nazwa, typ, true, poziomKlasy),
                    konstruktor.ClosingBrace.Row);
            }
            else
            {
                var zawartoscKontruktora =
                    GenerujZawartoscKontruktora(
                        klasa,
                        DajZawartoscDoDodania(nazwa, typ, false, poziomKlasy));
                solution.CurentDocument.InsertInLine(
                    new StringBuilder().AppendLine() + zawartoscKontruktora,
                    parsowane.FindFirstLineForConstructor(numerLinii));
            }
        }

        private string DajZawartoscDoDodania(
            string nazwa,
            string typ,
            bool koncowyEnter,
            int poziomKlasy)
        {
            var builder = new StringBuilder();
            builder.Append(ConstsForCode.DefaultIndentForMethodContent);

            builder.DodajWciecieWgPoziomuMetody(poziomKlasy);

            builder.Append(nazwa);
            builder.Append(" = new ");
            builder.Append(PrzygotujTypDoKonstrukcji(typ));
            builder.Append("();");
            if (koncowyEnter)
                builder.AppendLine();
            return builder.ToString();
        }

        private string PrzygotujTypDoKonstrukcji(string typ)
        {
            if (typ.StartsWith("I") && typ.Length > 1 && char.IsUpper(typ[1]))
                return typ.Substring(1);

            return typ;
        }

        private string GenerujZawartoscKontruktora(
            DefinedItem klasa,
            string zawartoscDoDodania)
        {
            var builder = new MethodBuilder();
            builder.ParameterInSigleLine(false);
            builder.WithName(klasa.Name);
            builder.WithReturnType("");
            builder.AddModifier("public");
            builder.AddLine(zawartoscDoDodania.TrimStart());
            var zawartoscKontruktora = builder.Build(ConstsForCode.DefaultIndentForMethod);
            return zawartoscKontruktora;
        }
    }
}
