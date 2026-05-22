namespace Shared.Contracts.Request.Files
{
    public sealed record PresignedUrlRequest(string BucketName, string FileName);
}
