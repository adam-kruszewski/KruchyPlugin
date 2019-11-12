using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using KruchyCompany.KruchyPlugin1.Akcje;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class UzupelnianieKonstruktoraZDziedziczeniemTests 
    {
        [Test]
        public void UzupelniaKonstruktorGdyKlasaDziedziczy()
        {
            //arrange
            var solution = new SolutionWrapper(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("UzupelnianieKonstruktoraPrzyNadklasie.cs"));

            //act
            new UzupelnianieKontruktora(solution).Uzupelnij();

            //assert
            var zawartoscPoZmianie = solution.AktualnyDokument.DajZawartosc();
        }
    }
}
