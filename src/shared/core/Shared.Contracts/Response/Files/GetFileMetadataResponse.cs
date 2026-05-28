namespace Shared.Contracts.Response.Files
{
    public sealed record GetFileMetadataResponse(string FileName, string ContentType, long Size);
}
