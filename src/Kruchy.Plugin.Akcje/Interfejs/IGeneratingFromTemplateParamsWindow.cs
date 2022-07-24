using Kruchy.Plugin.Utils.Wrappers;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Interfejs
{
    public interface IGeneratingFromTemplateParamsWindow
    {
        string Directory { get; set; }

        IProjectWrapper SelectedProject { get; }

        IEnumerable<IProjectWrapper> Projects { set; }

        IEnumerable<VariableToFill> VariablesToFill { set; }

        IDictionary<string, object> VariablesValues { get; }

        bool CanSelectDirectory { set; }

        bool Cancelled { get; }
    }
}