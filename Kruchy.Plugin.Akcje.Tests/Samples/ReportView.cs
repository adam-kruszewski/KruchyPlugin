using System.Collections.Generic;
using AutoMapper;
using Piatka.Infrastructure.Mappings;
using Pincasso.Drukowanie.Core.Services.Impl.PotwierdzenieSaldaArchiwalne;

namespace Pincasso.Drukowanie.Core.Views
{
    public class PotwierdzenieSaldaReportView
    {
        public string Saldo { get; set; }

        public string SaldoNierozksiegowanychWplat { get; set; }

        public string DataWystawienia { get; set; }

        public string DataWydruku { get; set; }

        public string KontoNumer { get; set; }

        public string Pracownik { get; set; }

        public string PracownikDaneKontaktowe { get; set; }

        public string SaldoZaGaz { get; set; }

        public string SaldoZaOdsetki { get; set; }

        public IList<NierozksiegowanaWplataReportView> NierozksiegowaneWplaty { get; set; }

        public IList<NiezaplaconaFakturaReportView> NiezaplaconeFaktury { get; set; }

        public IList<NiezaplaconaNotaOdsetkowaReportView> NiezaplaconeNotyOdsetkowe { get; set; }

        public string SaldoOdsetekWewnetrznych { get; set; }

        public bool PodmiotGospodarczy { get; set; }

        public PotwierdzenieSaldaReportView()
        {
            NierozksiegowaneWplaty = new List<NierozksiegowanaWplataReportView>();
            NiezaplaconeFaktury = new List<NiezaplaconaFakturaReportView>();
            NiezaplaconeNotyOdsetkowe = new List<NiezaplaconaNotaOdsetkowaReportView>();
        }
    }
}