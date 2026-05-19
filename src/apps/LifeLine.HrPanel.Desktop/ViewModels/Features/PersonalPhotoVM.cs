using Shared.WPF.Commands;
using Shared.WPF.Constants;
using Shared.WPF.Helpers;
using Shared.WPF.Services.Conversion;
using Shared.WPF.Services.FileDialog;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LifeLine.HrPanel.Desktop.ViewModels.Features
{
    internal sealed class PersonalPhotoVM : BaseEmployeeViewModel
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly IImageCompressionService _imageCompressionService;

        public PersonalPhotoVM(IFileDialogService fileDialogService, IImageCompressionService imageCompressionService)
        {
            _fileDialogService = fileDialogService;
            _imageCompressionService = imageCompressionService;

            SelectCommandAsync = new RelayCommandAsync(Execute_SelectCommandAsync);
        }

        public ImageSource? Ava
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        private byte[]? _compressedBytes;
        private string? _fileName;

        public RelayCommandAsync? SelectCommandAsync { get; private set; }
        private async Task Execute_SelectCommandAsync()
        {
            var path = _fileDialogService.GetFile($"Выберите файл: {FileDialogConsts.AVATAR}", FileFilters.Images);

            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
                return;

            try
            {
                var originalBytes = await System.IO.File.ReadAllBytesAsync(path);
                _compressedBytes = await _imageCompressionService.CompressImageAsync
                    (
                        originalBytes,
                        fileName: path,
                        quality: 85,
                        maxDimension: 512,
                        cancellationToken: default
                    );

                _fileName = Path.GetFileName(path);
                Ava = ImageHelper.ToImageFromFilePath(path);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show
                    (
                        $"Ошибка при обработке изображения: {ex.Message}",
                        "Ошибка",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error
                    );
            }
        }

        public byte[]? GetCompressedBytes() => _compressedBytes;
        public string? GetFileName() => _fileName;

        public void ClearProperty()
        {
            Ava = null;
            _compressedBytes = null;
            _fileName = string.Empty;
        }
    }
}
