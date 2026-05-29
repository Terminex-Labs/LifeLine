using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace Shared.WPF.Helpers
{
    public static class PdfHelper
    {
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Загружает файл по URL и возвращает байты (универсальный метод)
        /// </summary>
        public static async Task<byte[]?> BytesFromUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.WriteLine("[PdfHelper] Url пуст");
                return null;
            }

            try
            {
                return await _httpClient.GetByteArrayAsync(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PdfHelper] Ошибка загрузки '{url}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Конвертирует byte[] в MemoryStream (для передачи в PDF-вьюер)
        /// </summary>
        public static MemoryStream? ToStream(byte[]? pdfBytes)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
                return null;

            return new MemoryStream(pdfBytes);
        }

        /// <summary>
        /// Сохраняет байты во временный файл и возвращает путь (для открытия во внешнем приложении)
        /// </summary>
        public static async Task<string?> SaveToTempFile(byte[]? pdfBytes, string filename)
        {
            if (pdfBytes == null || pdfBytes.Length == 0 || string.IsNullOrWhiteSpace(filename))
                return null;

            try
            {
                var tempPath = Path.Combine(Path.GetTempPath(), filename);
                await File.WriteAllBytesAsync(tempPath, pdfBytes);
                return tempPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PdfHelper] Ошибка сохранения временного файла: {ex.Message}");
                return null;
            }
        }
    }
}
