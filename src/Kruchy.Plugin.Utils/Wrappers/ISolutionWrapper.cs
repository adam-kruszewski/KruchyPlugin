using System.Collections.Generic;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface ISolutionWrapper
    {
        string PelnaNazwa { get; }

        string Nazwa { get; }

        string Katalog { get; }

        IPlikWrapper AktualnyPlik { get; }

        IProjectWrapper AktualnyProjekt { get; }

        IDokumentWrapper AktualnyDokument { get; }

        IList<IProjectWrapper> Projekty { get; }

        IProjectWrapper ZnajdzProjekt(string nazwa);
    }
}
