namespace BlobStorage.Common.Extensions;

public static class StreamExtensions
{
    /// <summary>
    /// Ensures the stream is at position 0 and returns the stream for chaining
    /// </summary>
    public static Stream ResetPosition(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
        return stream;
    }

    /// <summary>
    /// Validates that the stream is not null and has content
    /// </summary>
    public static void ValidateForUpload(this Stream stream, string parameterName = "stream")
    {
        ArgumentNullException.ThrowIfNull(stream, parameterName);

        if (stream.CanSeek && stream.Length == 0)
        {
            throw new ArgumentException("Stream cannot be empty.", parameterName);
        }
    }
}
