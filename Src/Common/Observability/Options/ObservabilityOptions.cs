namespace Observability.Options;

public class ObservabilityOptions
{
    public const string SectionName = "Logging:Observability";

    public string ApplicationName { get; set; }

    public string Dns { get; set; }

    public string MinimumBreadcrumbLevel { get; set; } = "Information";

    public string MinimumEventLevel { get; set; } = "Error";
}
