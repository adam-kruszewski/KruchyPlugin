using FluentAssertions;
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

            zawartoscPoZmianie.Should().Be(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruchy.Plugin.Akcje.Tests.Samples
{
    class UzupelnianieKonstruktoraPrzyNadklasie : Klasa1
    {
        private readonly string serwis1;
        private readonly string serwis2;
        
        public UzupelnianieKonstruktoraPrzyNadklasie(
            string serwis1,
            string serwis2,
            string serwis0) : base(serwis0)
        {
            this.serwis1 = serwis1;
            this.serwis2 = serwis2;
        }
    }
}
");
        }
    }
}
