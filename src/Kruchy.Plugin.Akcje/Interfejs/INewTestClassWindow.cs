using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using System;

namespace Kruchy.Plugin.Akcje.Interfejs
{
    public interface INewTestClassWindow
    {
        string ClassName { get; set; }
        string TestType { get; }
        string TestedInterface { get; set; }
        string Directory { get; }
        Konfiguracja Konfiguracja { set; }
        bool Cancelled { get; }
        Func<string> GetDirectoryFromModuleFunc { set; }
    }
}
