namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface ISolutionExplorerWrapper
    {
        void OpenFile(string sciezka);

        void OpenFile(IPlikWrapper plik);

        void SelectPath(string sciezka);
    }
}