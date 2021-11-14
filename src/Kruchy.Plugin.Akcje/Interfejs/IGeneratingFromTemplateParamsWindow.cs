using Kruchy.Plugin.Utils.Wrappers;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Interfejs
{
    public interface IGeneratingFromTemplateParamsWindow
    {
        string Directory { get; set; }

        IProjektWrapper SelectedProject { get; }

        IEnumerable<IProjektWrapper> Projects { set; }
    }
}