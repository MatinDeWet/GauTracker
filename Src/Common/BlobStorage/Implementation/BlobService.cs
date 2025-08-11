using System.Globalization;
using System.Reactive.Linq;
using BlobStorage.Common.Constants;
using BlobStorage.Common.Extensions;
using BlobStorage.Common.Options;
using BlobStorage.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;

namespace BlobStorage.Implementation;

public sealed class BlobService : IBlobService, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IMinioClient _minioClient;
    private readonly BlobServiceOptions _options;

    public BlobService(IConfiguration configuration, IOptions<BlobServiceOptions> options)
    {
        _configuration = configuration;
        _options = options.Value;

        // Try to get connection string first, fallback to individual settings
        string? connectionString = _configuration.GetConnectionString(_options.DefaultConnectionStringKey);

        if (!string.IsNullOrEmpty(connectionString))
        {
            // Parse connection string (format: endpoint=...;accessKey=...;secretKey=...;useSSL=...)
            (string Endpoint, string AccessKey, string SecretKey, bool UseSSL) = ParseConnectionString(connectionString);

            #pragma warning disable CA2000 // Dispose objects before losing scope - MinioClient will be disposed in Dispose method
            _minioClient = new MinioClient()
                .WithEndpoint(Endpoint)
                .WithCredentials(AccessKey, SecretKey)
                .WithSSL(UseSSL)
                .WithRegion(_options.Region)
                .Build();
            #pragma warning restore CA2000
        }
        else
        {
            // Use individual configuration settings
            if (string.IsNullOrEmpty(_options.Endpoint) || string.IsNullOrEmpty(_options.AccessKey) || string.IsNullOrEmpty(_options.SecretKey))
            {
                throw new InvalidOperationException("MinIO configuration is incomplete. Provide either a connection string or all required settings (Endpoint, AccessKey, SecretKey).");
            }

            #pragma warning disable CA2000 // Dispose objects before losing scope - MinioClient will be disposed in Dispose method
            _minioClient = new MinioClient()
                .WithEndpoint(_options.Endpoint)
                .WithCredentials(_options.AccessKey, _options.SecretKey)
                .WithSSL(_options.UseSSL)
                .WithRegion(_options.Region)
                .Build();
            #pragma warning restore CA2000
        }
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string containerName,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        fileStream.ValidateForUpload(nameof(fileStream));
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);

        string fileExtension = Path.GetExtension(fileName).ToUpperInvariant();
        if (!_options.AllowedFileExtensions.Contains(fileExtension))
        {
            throw new InvalidOperationException($"File type '{fileExtension}' is not allowed. Only {string.Join(", ", _options.AllowedFileExtensions)} files are supported.");
        }

        // Ensure bucket exists
        bool bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(containerName),
            cancellationToken);

        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(containerName).WithLocation(_options.Region),
                cancellationToken);
        }

        string blobName = GenerateGuidBlobName(fileName);
        string contentType = ContentTypes.GetContentType(fileExtension);

        // Create or enhance metadata with original filename information
        var enhancedMetadata = new Dictionary<string, string>();
        
        // Add user-provided metadata first
        if (metadata != null)
        {
            foreach (KeyValuePair<string, string> kvp in metadata)
            {
                enhancedMetadata[kvp.Key] = kvp.Value;
            }
        }
        
        // Add original filename metadata (this will override user metadata if they use the same key)
        enhancedMetadata["original-filename"] = fileName;
        enhancedMetadata["upload-timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);

        fileStream.ResetPosition();

        PutObjectArgs putObjectArgs = new PutObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType)
            .WithHeaders(enhancedMetadata);

        PutObjectResponse blobResponse = await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

        return blobResponse.ObjectName;
    }

    public async Task<Stream> DownloadBlobAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);

        // Check if object exists
        try
        {
            await _minioClient.StatObjectAsync(
                new StatObjectArgs().WithBucket(containerName).WithObject(blobName),
                cancellationToken);
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{containerName}'.");
        }

        var memoryStream = new MemoryStream();

        await _minioClient.GetObjectAsync(
            new GetObjectArgs()
                .WithBucket(containerName)
                .WithObject(blobName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream)),
            cancellationToken);

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<bool> DeleteBlobAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);

        try
        {
            await _minioClient.RemoveObjectAsync(
                new RemoveObjectArgs().WithBucket(containerName).WithObject(blobName),
                cancellationToken);
            return true;
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            return false;
        }
    }

    public async Task<Dictionary<string, string>> GetBlobMetadataAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);

        try
        {
            ObjectStat statObject = await _minioClient.StatObjectAsync(
                new StatObjectArgs().WithBucket(containerName).WithObject(blobName),
                cancellationToken);

            var metadata = new Dictionary<string, string>();

            if (statObject.MetaData != null)
            {
                foreach (KeyValuePair<string, string> kvp in statObject.MetaData)
                {
                    metadata[kvp.Key] = kvp.Value;
                }
            }

            return metadata;
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{containerName}'.");
        }
    }

    private static string GenerateGuidBlobName(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        string guidName = Guid.NewGuid().ToString("N"); // 32 character hex string without dashes

        return $"{guidName}{extension}";
    }

    private static (string Endpoint, string AccessKey, string SecretKey, bool UseSSL) ParseConnectionString(string connectionString)
    {
        var parameters = connectionString.Split(';')
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part.Split('=', 2))
            .Where(split => split.Length == 2)
            .ToDictionary(split => split[0].Trim().ToUpperInvariant(), split => split[1].Trim());

        string endpoint = parameters.GetValueOrDefault("endpoint", "");
        string accessKey = parameters.GetValueOrDefault("accesskey", "");
        string secretKey = parameters.GetValueOrDefault("secretkey", "");
        bool useSSL = bool.Parse(parameters.GetValueOrDefault("usessl", "true"));

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Connection string must contain endpoint, accessKey, and secretKey parameters.");
        }

        return (endpoint, accessKey, secretKey, useSSL);
    }

    public void Dispose()
    {
        _minioClient?.Dispose();
    }
}
