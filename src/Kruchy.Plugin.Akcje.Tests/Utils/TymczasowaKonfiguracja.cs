using System;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.Utils
{
    class TymczasowaKonfiguracja : IDisposable
    {
        Konfiguracja poprzednia;

        public TymczasowaKonfiguracja(
            ISolutionWrapper solution,
            Konfiguracja konfiguracja)
        {
            poprzednia = Konfiguracja.GetInstance(solution);
            Konfiguracja.SetInstance(konfiguracja);
        }

        public void Dispose()
        {
            Konfiguracja.SetInstance(poprzednia);
        }
    }
}