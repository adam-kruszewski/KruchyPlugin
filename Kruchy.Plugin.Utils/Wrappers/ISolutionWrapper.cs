using System.Collections.Generic;
using EnvDTE80;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface ISolutionWrapper
    {
        string PelnaNazwa { get; }

        string Nazwa { get; }

        string Katalog { get; }

        IPlikWrapper AktualnyPlik { get; }

        IProjektWrapper AktualnyProjekt { get; }

        IDokumentWrapper AktualnyDokument { get; }

        DTE2 DTE { get; }

        IList<IProjektWrapper> Projekty { get; }

        IProjektWrapper ZnajdzProjekt(string nazwa);
    }
}
