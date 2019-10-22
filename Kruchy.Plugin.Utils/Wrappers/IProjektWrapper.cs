using System.Collections.Generic;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface IProjektWrapper
    {
        string Nazwa { get; }

        string Sciezka { get; }

        string SciezkaDoKatalogu { get; }

        IPlikWrapper[] Pliki { get; }

        PlikWrapper DodajPlik(string sciezka);

        bool NamespaceNalezyDoProjektu(string nazwaNamespace);

        IEnumerable<string> DajPlikiZNamespace(string nazwaNamespace);
    }
}