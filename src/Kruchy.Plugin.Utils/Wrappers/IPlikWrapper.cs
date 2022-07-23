namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface IPlikWrapper
    {
        string Nazwa { get; }

        string NazwaBezRozszerzenia { get; }

        string SciezkaPelna { get; }

        string Katalog { get; }

        string SciezkaWzgledna { get; }

        IProjectWrapper Projekt { get; }

        IDocumentWrapper Dokument { get; }
    }
}