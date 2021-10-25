namespace Kruchy.Plugin.Akcje.Interfejs
{
    public interface IAddVerbConfigurationWindow
    {
        string Value { get; set; }

        string ClassNameRegex { get; set; }

        string OutputValue { get; set; }

        bool Confirmed { get; }
    }
}