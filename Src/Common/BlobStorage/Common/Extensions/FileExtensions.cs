namespace BlobStorage.Common.Extensions;
public static class FileExtensions
{
    /// <summary>
    /// Extension method to validate if a file name has a valid file extension.
    /// </summary>
    /// <param name="fileName">The file name to validate</param>
    /// <param name="allowedExtensions">Array of allowed extensions</param>
    /// <returns>True if the file extension is valid, false otherwise</returns>
    public static bool IsValidFileExtension(this string? fileName, params string[] allowedExtensions)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        string extension = Path.GetExtension(fileName).ToUpperInvariant();
        return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Extension method to validate if a content type is valid.
    /// </summary>
    /// <param name="contentType">The content type to validate</param>
    /// <param name="allowedContentTypes">Array of allowed content types</param>
    /// <returns>True if the content type is valid, false otherwise</returns>
    public static bool IsValidContentType(this string? contentType, params string[] allowedContentTypes)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        return allowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Extension method to convert file size from megabytes to bytes.
    /// </summary>
    /// <param name="sizeInMB">Size in megabytes</param>
    /// <returns>Size in bytes</returns>
    public static long ToBytes(this int sizeInMB)
    {
        return sizeInMB * 1024L * 1024L;
    }

    /// <summary>
    /// Extension method to convert file size from megabytes to bytes.
    /// </summary>
    /// <param name="sizeInMB">Size in megabytes</param>
    /// <returns>Size in bytes</returns>
    public static long ToBytes(this double sizeInMB)
    {
        return (long)(sizeInMB * 1024 * 1024);
    }

    /// <summary>
    /// Extension method to convert bytes to megabytes.
    /// </summary>
    /// <param name="sizeInBytes">Size in bytes</param>
    /// <returns>Size in megabytes</returns>
    public static double ToMegabytes(this long sizeInBytes)
    {
        return sizeInBytes / (1024.0 * 1024.0);
    }

    /// <summary>
    /// Extension method to format file size in a human-readable format.
    /// </summary>
    /// <param name="sizeInBytes">Size in bytes</param>
    /// <returns>Formatted file size string (e.g., "1.5 MB", "2.3 GB")</returns>
    public static string ToHumanReadableSize(this long sizeInBytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        double size = sizeInBytes;
        int order = 0;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Extension method to check if a file size exceeds the maximum allowed size.
    /// </summary>
    /// <param name="fileSizeInBytes">File size in bytes</param>
    /// <param name="maxSizeInMB">Maximum allowed size in megabytes</param>
    /// <returns>True if file size exceeds the limit, false otherwise</returns>
    public static bool ExceedsMaxSize(this long fileSizeInBytes, int maxSizeInMB)
    {
        long maxSizeInBytes = maxSizeInMB.ToBytes();
        return fileSizeInBytes > maxSizeInBytes;
    }
}
