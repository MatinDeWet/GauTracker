namespace Observability.Options;

public class ObservabilityOptions
{
    public const string SectionName = "Logging:Observability";

    public string ApplicationName { get; set; }

    public string Url { get; set; }

    public string Organization { get; set; }

    public string StreamName { get; set; } = "default";

    public string Login { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string MinimumLevel { get; set; } = "Information";
}
