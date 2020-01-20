using System.Configuration;
using Piatka.Infrastructure.AppSettings;
using Pincasso.Administracja.Core.Services;
using Pincasso.Core.Utils;

namespace Pincasso.MvcApp.WebServices.Logging
{
    public class KonfiguracjaLogowaniaKomunikatowWebowych
         : IKonfiguracjaLogowaniaKomunikatowWebowych
    {
        private const string Prefix_Kluczy_Wartosci = "KomunikatyWebSerwis:";

        private const string Domyslny_Katalog_Komunikatow = "web-service-messages";

        private readonly IAppSettingsService appSettingsService;

        public string KatalogKomunikatow
        {
            get
            {
                return SciezkiUtils.DajPelnaSciezke(wartosc);
            }
        }

        public int GlebokoscPodzialuNaKatalogi
        {
            get
            {
                return 2;
            }
        }

        public int? LiczbaGodzinPrzechowywania
        {
            get
            {
                return 1;
            }
        }
    }
}