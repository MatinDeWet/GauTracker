namespace BlobStorage.Common.Options;

public class BlobServiceOptions
{
    public const string SectionName = "BlobService";

    public List<string> AllowedFileExtensions { get; set; } = [];

    public string DefaultConnectionStringKey { get; set; } = "MinIO";

    public string Endpoint { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public bool UseSSL { get; set; } = true;

    public string Region { get; set; } = "us-east-1";
}
