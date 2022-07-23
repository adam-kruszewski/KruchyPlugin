namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface IFileWrapper
    {
        string Name { get; }

        string NameWithoutExtension { get; }

        string FullPath { get; }

        string Directory { get; }

        string RelativePath { get; }

        IProjectWrapper Project { get; }

        IDocumentWrapper Document { get; }
    }
}