using System.IO;

namespace LifeLine.HrPanel.Desktop.Services.GeneratePdf
{
    public interface IGeneratePdfService
    {
        Task<byte[]?> GenerateAsync(string? s3Url);
        Task<Stream?> GenerateAsStreamAsync(string? s3Url);
    }
}