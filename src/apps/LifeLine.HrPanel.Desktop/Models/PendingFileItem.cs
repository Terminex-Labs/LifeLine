using Shared.Contracts.Response.Files;
using Shared.WPF.ViewModels.Abstract;
using System.IO;
using System.Windows.Media.Imaging;

namespace LifeLine.HrPanel.Desktop.Models
{
    public sealed class PendingFileItem : BaseViewModel
    {
        private int _index;
        private string _fileName = null!;
        private string? _filePath;
        //private string? _s3Url;
        private BitmapImage? _thumbnail;
        private bool _isSelected;
        private long _fileSize;
        private string _contentType = null!;
        private bool _isRemoteFile;

        public static PendingFileItem FromMetadata(int index, GetFileMetadataResponse metadata)
        {
            return new PendingFileItem
                (
                    index: index,
                    fileName: metadata.FileName,
                    filePath: metadata.FileName,
                    fileSize: metadata.Size,
                    contentType: metadata.ContentType
                );
        }

        public PendingFileItem(int index, string filePath)
        {
            Index = index;
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);

            LoadMetadata();

            _ = GenerateThumbnailAsync();
        }

        private PendingFileItem(int index, string fileName, string filePath, long fileSize, string contentType)
        {
            Index = index;
            FileName = fileName;
            FilePath = filePath;
            FileSize = fileSize;
            ContentType = contentType;
            IsRemoteFile = true;

            _ = GenerateThumbnailAsync();
        }

        /// <summary>
        /// Порядковый номер файла в очереди (1, 2, 3...)
        /// </summary>
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        /// <summary>
        /// Имя файла с расширением
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        /// <summary>
        /// Полный путь к файлу на диске
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        /// <summary>
        /// URL файла в s3
        /// </summary>
        //public string? S3Url
        //{
        //    get => _s3Url;
        //    set => SetProperty(ref _s3Url, value);
        //}

        /// <summary>
        /// Превью изображения (для отображения в UI)
        /// </summary>
        public BitmapImage? Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        /// <summary>
        /// Выбран ли элемент в списке (для визуального выделения)
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long FileSize
        {
            get => _fileSize;
            set => SetProperty(ref _fileSize, value);
        }

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public string ContentType
        {
            get => _contentType;
            set => SetProperty(ref _contentType, value);
        }

        /// <summary>
        /// Флаг удаленного файла
        /// </summary>
        public bool IsRemoteFile
        {
            get => _isRemoteFile;
            set => SetProperty(ref _isRemoteFile, value);
        }

        /// <summary>
        /// Размер файла в удобном формате (например, "2.4 МБ")
        /// </summary>
        public string FileSizeFormatted => FormatBytes(FileSize);

        /// <summary>
        /// Расширение файла в верхнем регистре (PDF, JPG, PNG)
        /// </summary>
        public string FileExtension
        {
            get
            {
                var path = IsRemoteFile ? FileName : FilePath;
                return string.IsNullOrEmpty(path)
                    ? string.Empty
                    : Path.GetExtension(path).TrimStart('.').ToUpperInvariant();
            }
        }

        #region Helpers

        private void LoadMetadata()
        {
            try
            {
                var fileInfo = new FileInfo(FilePath);

                FileSize = fileInfo.Length;
                ContentType = GetContentType(FilePath);
            }
            catch
            {
                FileSize = 0;
                ContentType = "application/octet-stream";
            }
        }

        private static string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".bmp" => "image/bmp",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        private async Task GenerateThumbnailAsync()
        {
            try
            {
                var pathForCheck = IsRemoteFile ? FileName : FilePath;

                if (string.IsNullOrEmpty(pathForCheck))
                    return;

                if (IsImageFile(pathForCheck))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(FilePath);
                    bitmap.DecodePixelWidth = 150;
                    bitmap.DecodePixelHeight = 150;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    Thumbnail = bitmap;
                }
                else if (IsPdfFile(pathForCheck))
                {
                    Thumbnail = CreatePdfPlaceholder();
                }
            }
            catch
            {
                Thumbnail = null;
            }
        }

        private static bool IsImageFile(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            var ext = Path.GetExtension(path).ToLower();
            return ext is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif";
        }

        private static bool IsPdfFile(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return Path.GetExtension(path).ToLower() == ".pdf";
        }

        private static BitmapImage CreatePdfPlaceholder()
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            // Если у вас есть иконка в ресурсах:
            // bitmap.UriSource = new Uri("pack://application:,,,/LifeLine.HrPanel.Desktop;component/Resources/pdf-icon.png");
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private static string FormatBytes(long bytes)
        {
            string[] suffixes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1 && counter < suffixes.Length - 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:0.##} {suffixes[counter]}";
        }

        #endregion
    }
}
