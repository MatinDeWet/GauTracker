using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using Amazon.S3.Model;
using BlobStorage.Configuration;
using BlobStorage.Contracts;
using BlobStorage.Contracts.Models;
using Microsoft.Extensions.Options;

namespace BlobStorage.Implementation;

/// <summary>
/// <see cref="IBlobStorageService"/> backed by an S3-compatible SeaweedFS endpoint.
/// </summary>
internal sealed class SeaweedBlobStorageService : IBlobStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly BlobStorageOptions _options;

    public SeaweedBlobStorageService(IAmazonS3 s3, IOptions<BlobStorageOptions> options)
    {
        _s3 = s3;
        _options = options.Value;
    }

    public async Task DeleteAsync(string container, string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(container);
        ArgumentException.ThrowIfNullOrEmpty(key);

        await _s3.DeleteObjectAsync(
            new DeleteObjectRequest { BucketName = container, Key = key },
            cancellationToken);
    }

    public async Task UploadAsync(
        string container,
        string key,
        Stream content,
        string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(container);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(content);

        var request = new PutObjectRequest
        {
            BucketName = container,
            Key = key,
            InputStream = content,
            AutoCloseStream = false,
            ContentType = contentType,
        };

        await _s3.PutObjectAsync(request, cancellationToken);
    }

    public BlobUploadTicket CreateUploadUrl(string container, string key, TimeSpan? expiry = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(container);
        ArgumentException.ThrowIfNullOrEmpty(key);

        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.Add(expiry ?? _options.PresignedUrlExpiry);

        string url = _s3.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = container,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = expiresAt.UtcDateTime,
        });

        return new BlobUploadTicket(key, new Uri(url), expiresAt);
    }

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "Stream ownership is transferred to the caller via BlobContent.")]
    public async Task<BlobContent> DownloadAsync(string container, string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(container);
        ArgumentException.ThrowIfNullOrEmpty(key);

        GetObjectResponse response = await _s3.GetObjectAsync(
            new GetObjectRequest { BucketName = container, Key = key },
            cancellationToken);

        return new BlobContent(
            key,
            response.ResponseStream,
            response.Headers.ContentType,
            response.ContentLength);
    }

    public BlobDownloadTicket CreateDownloadUrl(string container, string key, TimeSpan? expiry = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(container);
        ArgumentException.ThrowIfNullOrEmpty(key);

        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.Add(expiry ?? _options.PresignedUrlExpiry);

        string url = _s3.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = container,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = expiresAt.UtcDateTime,
        });

        return new BlobDownloadTicket(key, new Uri(url), expiresAt);
    }
}
