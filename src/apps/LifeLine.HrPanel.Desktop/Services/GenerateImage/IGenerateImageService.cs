using System.Windows.Media;

namespace LifeLine.HrPanel.Desktop.Services.GenerateImage
{
    public interface IGenerateImageService
    {
        Task<ImageSource?> GenerateAsync(string? personalPhoto);
        Task<byte[]?> GenerateBytesAsync(string? personalPhoto);
    }
}