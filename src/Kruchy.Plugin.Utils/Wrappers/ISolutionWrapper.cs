using System.Collections.Generic;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface ISolutionWrapper
    {
        string FullName { get; }

        string Name { get; }

        string Directory { get; }

        IFileWrapper CurrentFile { get; }

        IProjectWrapper CurrentProject { get; }

        IDocumentWrapper CurenctDocument { get; }

        IList<IProjectWrapper> Projects { get; }

        IProjectWrapper FindProject(string name);
    }
}
