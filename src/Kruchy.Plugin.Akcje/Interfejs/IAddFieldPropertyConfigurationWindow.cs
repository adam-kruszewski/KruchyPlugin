namespace Kruchy.Plugin.Akcje.Interfejs
{
    public interface IAddFieldPropertyConfigurationWindow
    {
        string Value { get; set; }

        string ClassNameRegex { get; set; }

        string FieldPropertyTypeRegex { get; set; }

        string OutputValue { get; set; }

        bool Confirmed { get; }
    }
}
