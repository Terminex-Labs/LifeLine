
namespace LifeLine.HrPanel.Desktop.Services.FilePreview
{
    public interface IFilePreviewService
    {
        Task<string?> DownloadRemoteFileToTempAsync(string url, string fileName);
        string? CopyLocalFileToTempAsync(string sourcePath, string fileName);
        void OpenInDefaultApplication(string filePath);
    }
}