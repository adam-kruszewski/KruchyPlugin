namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface ISolutionExplorerWrapper
    {
        void OpenFile(string sciezka);

        void OpenFile(IFileWrapper plik);

        void SelectPath(string sciezka);
    }
}