namespace BlobStorage.Common.Constants;

public static class ContentTypes
{
    public const string Csv = "text/csv";
    public const string Text = "text/plain";
    public const string Json = "application/json";
    public const string Xml = "application/xml";
    public const string Pdf = "application/pdf";
    public const string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public const string ExcelLegacy = "application/vnd.ms-excel";
    public const string OctetStream = "application/octet-stream";

    public static readonly Dictionary<string, string> FileExtensionToContentType = new()
    {
        [".csv"] = Csv,
        [".txt"] = Text,
        [".json"] = Json,
        [".xml"] = Xml,
        [".pdf"] = Pdf,
        [".xlsx"] = Excel,
        [".xls"] = ExcelLegacy
    };

    public static string GetContentType(string fileExtension)
    {
        return FileExtensionToContentType.GetValueOrDefault(
            fileExtension.ToUpperInvariant(),
            OctetStream);
    }
}
