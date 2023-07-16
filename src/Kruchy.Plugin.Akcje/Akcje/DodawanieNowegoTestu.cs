using KruchyCodeBuilders.Builders;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class DodawanieNowegoTestu
    {
        private readonly ISolutionWrapper solution;

        public DodawanieNowegoTestu(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void DodajNowyTest(string nazwaTestu, bool asyncTest = false)
        {
            var konfiguracja = Konfiguracja.GetInstance(solution);

            var builder =
                new MethodBuilder()
                    .WithName(nazwaTestu)
                    .AddModifier("public")
                    .AddAttribute(new AttributeBuilder().WithName(DajNazweAtrybutu(konfiguracja)));

            if (asyncTest)
            {
                builder = builder
                    .AddModifier("async")
                    .WithReturnType("Task");

                solution.CurentDocument?.DodajUsingaJesliTrzeba("System.Threading.Tasks");
            }

            builder.AddLine("//arrange");
            builder.AddLine("");
            builder.AddLine("//act");
            builder.AddLine("");
            builder.AddLine("//assert");
            var dokument = solution.CurentDocument;
            if (dokument == null)
                return;

            var numerLinii = dokument.GetCursorLineNumber();
            var trescMetody = builder.Build(ConstsForCode.DefaultIndentForMethod).TrimEnd();
            dokument.InsertInLine(trescMetody, numerLinii);
            dokument.SetCursorForAddedMethod(numerLinii + 2);
            dokument.DodajUsingaJesliTrzeba(DajUsingaDoDodania(konfiguracja));
        }

        private static string DajNazweAtrybutu(Konfiguracja konfiguracja)
        {
            switch (konfiguracja.Testy().NazwaBiblioteki)
            {
                case "NUnit":
                    return "Test";
                default:
                    return "TestMethod";
            }
        }

        private static string DajUsingaDoDodania(Konfiguracja konfiguracja)
        {
            switch (konfiguracja.Testy().NazwaBiblioteki)
            {
                case "NUnit":
                    return "NUnit.Framework";
                default:
                    return "Microsoft.VisualStudio.TestTools.UnitTesting";
            }
        }
    }
}