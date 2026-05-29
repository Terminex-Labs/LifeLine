using LifeLine.File.Service.Client;
using LifeLine.HrPanel.Desktop.Models;
using LifeLine.HrPanel.Desktop.Services.GeneratePdf;
using Shared.Contracts.Request.Files;
using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Commands;
using Shared.WPF.Constants;
using Shared.WPF.Enums;
using Shared.WPF.Extensions;
using Shared.WPF.Helpers;
using Shared.WPF.Services.Conversion;
using Shared.WPF.Services.FileDialog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Data;
using Terminex.Common.Results;

namespace LifeLine.HrPanel.Desktop.ViewModels.Features
{
    internal sealed class PersonalDocumentsVM : BaseEmployeeViewModel
    {
        private readonly IGeneratePdfService _generatePdfService;

        private readonly IFileDialogService _fileDialogService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IDocumentConversionService _documentConversionService;

        private readonly IReadOnlyCollection<DocumentTypeDisplay> _documentTypes;


        public PersonalDocumentsVM
            (
                IGeneratePdfService generatePdfService,

                IFileDialogService fileDialogService,
                IFileStorageService fileStorageService,
                IDocumentConversionService documentConversionService, 

                IReadOnlyCollection<DocumentTypeDisplay> documentTypes
            )
        {
            _generatePdfService = generatePdfService;

            _fileDialogService = fileDialogService;
            _fileStorageService = fileStorageService;

            _documentTypes = documentTypes;

            _documentConversionService = documentConversionService;

            PersonalDocumentsView = CollectionViewSource.GetDefaultView(LocalPersonalDocuments);

            PersonalDocumentsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PersonalDocumentDisplay.SaveStatus)));

            SelectMultipleCommand = new RelayCommand(Execute_SelectMultipleCommand);
            PreviewCommand = new RelayCommandAsync<PendingFileItem>(Execute_PreviewCommand);
            RemovePendingFileCommand = new RelayCommand<PendingFileItem>(Execute_RemovePendingFileCommand);
            AddPersonalDocumentCommand = new RelayCommandAsync(Execute_AddPersonalDocumentCommand, CanExecute_AddPersonalDocumentCommand);
        }

        private string _number = null!;
        public string Number
        {
            get => _number;
            set
            {
                SetProperty(ref _number, value);
                AddPersonalDocumentCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _series;
        public string? Series
        {
            get => _series;
            set
            {
                SetProperty(ref _series, value);
                AddPersonalDocumentCommand?.RaiseCanExecuteChanged();
            }
        }

        private DocumentTypeDisplay _documentType = null!;
        public DocumentTypeDisplay DocumentType
        {
            get => _documentType;
            set
            {
                SetProperty(ref _documentType, value);
                AddPersonalDocumentCommand?.RaiseCanExecuteChanged();
            }
        }

        private PersonalDocumentDisplay _selectedLocalPersonalDocument = null!;
        public PersonalDocumentDisplay SelectedLocalPersonalDocument
        {
            get => _selectedLocalPersonalDocument;
            set
            {
                if (value != null)
                {
                    SetProp(value);

                    SetProperty(ref _selectedLocalPersonalDocument, value);

                    _ = LoadDocumentToQueueAsync(value);
                }
            }
        }

        private void SetProp(PersonalDocumentDisplay value)
        {
            Number = value.DocumentNumber;
            Series = value.DocumentSeries;
            DocumentType = value.DocumentType;
        }

        private async Task LoadDocumentToQueueAsync(PersonalDocumentDisplay document)
        {
            PendingFilePaths.Clear();

            if (document.SaveStatus != SaveStatus.DataBase)
                return;

            if (string.IsNullOrWhiteSpace(document.FileKey))
                return;

            var (bucketName, fileName) = S3UrlParser.Parse(document.FileKey);

            var metadataResult = await _fileStorageService.GetFileMetadataAsync(new GetFileMetadataRequest(bucketName!, fileName!));

            if (metadataResult.IsFailure || metadataResult.Value == null)
            {
                MessageBox.Show($"Не удалось получить метаданные: {metadataResult.StringMessage}");
                return;
            }

            var pendingItem = PendingFileItem.FromMetadata(PendingFilePaths.Count + 1, metadataResult.Value);

            PendingFilePaths.Add(pendingItem);
            UpdateIndexes();
        }

        public ObservableCollection<PendingFileItem> PendingFilePaths { get; private set; } = [];

        public RelayCommand SelectMultipleCommand { get; private set; }
        private void Execute_SelectMultipleCommand()
        {
            var paths = _fileDialogService.GetFiles($"Выберите файлы: {FileDialogConsts.PERSONAL_DOCUMENT}", FileFilters.ImagesAndPdf);

            if (paths?.Any() == true)
            {
                var startIndex = PendingFilePaths.Count + 1;
                foreach (var path in paths)
                    PendingFilePaths.Add(new PendingFileItem(startIndex++, path));

                UpdateIndexes();
            }
        }

        public RelayCommandAsync<PendingFileItem>? PreviewCommand { get; private set; }
        private async Task Execute_PreviewCommand(PendingFileItem item)
        {
            List<Error> errors = [];

            byte[]? fileBytes = null;
            var pathForCheck = item.IsRemoteFile ? item.FileName : item.FilePath;

            Func<Task> func = SelectedLocalPersonalDocument.SaveStatus switch
            {
                SaveStatus.Local => async () =>
                {
                    Debug.WriteLine("Пока не реализовано!");
                },
                SaveStatus.DataBase => async () =>
                {
                    fileBytes = await _generatePdfService.GenerateAsync(SelectedLocalPersonalDocument.FileKey);

                    if (fileBytes == null || fileBytes.Length == 0)
                    {
                        MessageBox.Show("Не удалось загрузить файл для просмотра", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    await OpenPdfExternally(fileBytes, pathForCheck);
                },
                _ => async () => Result.Success()
            };

            await func();

            errors.ShowError();
        }

        private async Task OpenPdfExternally(byte[] pdfBytes, string fileName)
        {
            var tempPath = await PdfHelper.SaveToTempFile(pdfBytes, fileName);

            Process.Start(new ProcessStartInfo
            {
                FileName = tempPath,
                UseShellExecute = true 
            });
        }

        public RelayCommand<PendingFileItem>? RemovePendingFileCommand { get; private set; }
        private void Execute_RemovePendingFileCommand(PendingFileItem item)
        {
            if (item != null && PendingFilePaths.Remove(item))
                UpdateIndexes();
        }

        public ObservableCollection<PersonalDocumentDisplay> LocalPersonalDocuments { get; init; } = [];
        public ICollectionView PersonalDocumentsView { get; private init; } = null!;

        public RelayCommandAsync? AddPersonalDocumentCommand { get; private set; }
        private async Task Execute_AddPersonalDocumentCommand()
        {
            if (!PendingFilePaths.Any())
            {
                MessageBox.Show("Выберите хотя бы один файл для добавления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var fileBytes = new List<byte[]>();
                var fileNames = new List<string>();
                var contentType = new List<string>();

                foreach (var pendingFile in PendingFilePaths)
                {
                    if (System.IO.File.Exists(pendingFile.FilePath))
                    {
                        fileBytes.Add(await System.IO.File.ReadAllBytesAsync(pendingFile.FilePath));
                        fileNames.Add(Path.GetFileName(pendingFile.FileName));
                        contentType.Add(Path.GetExtension(pendingFile.ContentType));

                        //MessageBox.Show($"Файл: {pendingFile.FileName}\n" +
                        //                $"Размер: {pendingFile.FileSize} байт\n" +
                        //                $"Тип: {pendingFile.ContentType}\n");
                    }
                }

                if (!fileBytes.Any())
                {
                    MessageBox.Show("Не удалось прочитать выбранные файлы", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var pdfBytes = await _documentConversionService.ConvertImagesToPdfAsync
                    (
                        DocumentType.Name,
                        EmployeeId!,
                        fileBytes,
                        fileNames
                    );

                var fileName = $"{Number}.pdf";

                LocalPersonalDocuments.Add
                    (
                        new PersonalDocumentDisplay
                            (
                                new PersonalDocumentResponse
                                    (
                                        Guid.Empty,
                                        Guid.Parse(DocumentType.Id),
                                        Number,
                                        Series,
                                        null
                                    ),
                                _documentTypes,
                                SaveStatus.Local
                            )
                        {
                            FileBytes = pdfBytes,
                            FileName = fileName,
                            ContentType = "application/pdf",
                        }
                    );

                ClearProperty();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке файла: {ex.Message}", "Ошибка конвертации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecute_AddPersonalDocumentCommand()
            => DocumentType != null && !string.IsNullOrWhiteSpace(Number);

        public void ClearProperty()
        {
            Number = string.Empty;
            Series = string.Empty;
            DocumentType = null!;

            PendingFilePaths.Clear();
        }

        private void UpdateIndexes()
        {
            for (int i = 0; i < PendingFilePaths.Count; i++)
                PendingFilePaths[i].Index = i + 1;
        }
    }
}
