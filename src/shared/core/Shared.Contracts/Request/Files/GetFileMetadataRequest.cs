namespace Shared.Contracts.Request.Files
{
    public sealed record GetFileMetadataRequest(string BucketName, string ObjectPath);
}
