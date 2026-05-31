using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Shared.WPF.Helpers
{
    public static class FileHelper
    {
        private static readonly HttpClient _httpClient = new();

        #region Универсальные методы

        /// <summary>
        /// Загружает файлы по URL и возвращает байты
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<byte[]?> BytesFromUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.WriteLine($"[FileHelper] [BytesFromUrlAsync] URL пуст!");
                return null;
            }

            try
            {
                return await _httpClient.GetByteArrayAsync(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileHelper] [BytesFromUrlAsync] Ошибка загрузки URL - '{url}':\nСообщение ошибки: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Сохраняет байты во временный файл и возвращает путь
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<string?> SaveToTempFileAsync(byte[]? fileBytes, string fileName)
        {
            if (fileBytes == null || fileBytes.Length == 0 || string.IsNullOrWhiteSpace(fileName))
            {
                Debug.WriteLine($"[FileHelper] [SaveToTempFileAsync] FileBytes и FileName пусты!");
                return null;
            }

            try
            {
                var safeFileName = Path.GetFileName(fileName);
                var tempPath = Path.Combine(Path.GetTempPath(), safeFileName);

                await File.WriteAllBytesAsync(tempPath, fileBytes);
                return tempPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileHelper] [SaveToTempFileAsync] Ошибка сохранения временного файла:\n{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Конвертирует byte[] в MemoryStream
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public static MemoryStream? ToStream(byte[]? fileBytes)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                Debug.WriteLine($"[FileHelper] [ToStream] FileBytes пуст!");
                return null;
            }

            return new MemoryStream(fileBytes);
        }

        /// <summary>
        /// Определяет тип файла по расширению
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileType GetFileType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            return ext switch
            {
                ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".webp" => FileType.Image,
                ".pdf" => FileType.Pdf,
                ".doc" or ".docx" => FileType.Word,
                ".xls" or ".xlsx" => FileType.Excel,
                ".txt" => FileType.Text,
                _ => FileType.Unknown
            };
        }

        /// <summary>
        /// Полученает Mime-тип по расширению
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".bmp" => "image/bmp",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        #endregion

        #region Изображения

        /// <summary>
        /// Загружает изображение по URL и возвращает ImageSourse
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<ImageSource?> ImageFromUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Ссылка на изображение пуста!");
                Debug.WriteLine($"[FileHelper] [ImageFromUrlAsync] URL пуст!");
                return null;
            }

            try
            {
                var imageBytes = await BytesFromUrlAsync(url);

                if (imageBytes == null)
                {
                    Debug.WriteLine($"[FileHelper] [ImageFromUrlAsync] ImageBytes пуст!");
                    return null;
                }

                return await Task.Run(() =>
                {
                    using var stream = new MemoryStream(imageBytes);
                    var bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    return (ImageSource)bitmap;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileHelper] [ImageFromUrlAsync] Ошибка создания изображения из URL - '{url}':{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Загружает изображение из локального файла
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ImageSource? ImageFromFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Debug.WriteLine($"[FileHelper] [ImageFromFilePath] FilePath пуст!");
                return null;
            }

            try
            {
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath);
                bitmapImage.CacheOption |= BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileHelper] [ImageFromFilePath] Ошибка загрузки изображения из '{filePath}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Конвертирет ImageSourse в byte[] (PNG)
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        public static byte[]? ImageToBytes(ImageSource? imageSource)
        {
            if (imageSource is not BitmapSource bitmapSourse)
                return null;

            using var stream = new MemoryStream();
            var encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSourse));
            encoder.Save(stream);

            return stream.ToArray();
        }


        /// <summary>
        /// Конвертирует byte[] в BitmapImage
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        public static BitmapImage? BitmapImageFromBytes(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                Debug.WriteLine($"[FileHelper] [BitmapImageFromBytes] ImageBytes пуст!");
                return null;
            }

            using var stream = new MemoryStream(imageBytes);

            var bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        public static BitmapImage? BitmapImageFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            try
            {
                using var stream = new MemoryStream();

                bitmap.Save(stream, ImageFormat.Png);

                stream.Seek(0, SeekOrigin.Begin);

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileHelper] [BitmapImageFromBitmap] Ошибка конвертации Bitmap: {ex.Message}");
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Типы файлов для классификации
        /// </summary>
        public enum FileType
        {
            Unknown,
            Image,
            Pdf,
            Word,
            Excel,
            Text
        }
    }
}
