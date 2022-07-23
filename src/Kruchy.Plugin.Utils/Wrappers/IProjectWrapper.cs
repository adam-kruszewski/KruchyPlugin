using System.Collections.Generic;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface IProjectWrapper
    {
        string Name { get; }

        string Path { get; }

        string DirectoryPath { get; }

        IPlikWrapper[] Files { get; }

        IPlikWrapper AddFile(string sciezka);

        bool ContainsNamespace(string nazwaNamespace);

        IEnumerable<string> GetFilesFromNamespace(string nazwaNamespace);
    }
}