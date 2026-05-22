namespace Shared.Contracts.Request.Files
{
    public sealed record DeleteFileRequest(string BucketName, string FileName);
}
