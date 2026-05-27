using LifeLine.HrPanel.Desktop.Models;
using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Commands;
using Shared.WPF.Constants;
using Shared.WPF.Enums;
using Shared.WPF.Helpers;
using Shared.WPF.Services.Conversion;
using Shared.WPF.Services.FileDialog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace LifeLine.HrPanel.Desktop.ViewModels.Features
{
    internal sealed class PersonalDocumentsVM : BaseEmployeeViewModel
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly IDocumentConversionService _documentConversionService;

        private readonly IReadOnlyCollection<DocumentTypeDisplay> _documentTypes;


        public PersonalDocumentsVM
            (
                IFileDialogService fileDialogService,
                IDocumentConversionService documentConversionService, 

                IReadOnlyCollection<DocumentTypeDisplay> documentTypes
            )
        {
            _fileDialogService = fileDialogService;

            _documentTypes = documentTypes;

            _documentConversionService = documentConversionService;

            PersonalDocumentsView = CollectionViewSource.GetDefaultView(LocalPersonalDocuments);

            PersonalDocumentsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PersonalDocumentDisplay.SaveStatus)));

            SelectMultipleCommand = new RelayCommand(Execute_SelectMultipleCommand);
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
                    SetProp(value).GetAwaiter().GetResult();

                    SetProperty(ref _selectedLocalPersonalDocument, value);
                }
            }
        }

        private async Task SetProp(PersonalDocumentDisplay value)
        {
            Number = value.DocumentNumber;
            Series = value.DocumentSeries;
            DocumentType = value.DocumentType;
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

                foreach (var path in PendingFilePaths.Select(x => x.FilePath))
                {
                    MessageBox.Show(path);
                    if (System.IO.File.Exists(path))
                    {
                        fileBytes.Add(await System.IO.File.ReadAllBytesAsync(path));
                        fileNames.Add(Path.GetFileName(path));
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

                var fileName = $".pdf";

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
                            ContentType = "application/pdf"
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
