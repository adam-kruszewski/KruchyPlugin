namespace Kruchy.Plugin.Akcje.Interfejs
{
    public interface INewTestWindow
    {
        string Name { get; }

        bool Async { get; }

        void Show();
    }
}