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
    internal sealed class EducationDocumentsVM : BaseEmployeeViewModel
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly IDocumentConversionService _documentConversionService;

        private readonly IReadOnlyCollection<DocumentTypeDisplay> _documentTypes;
        private readonly IReadOnlyCollection<EducationLevelDisplay> _educationLevels;

        public EducationDocumentsVM
            (
                IFileDialogService fileDialogService,
                IDocumentConversionService documentConversionService,
                IReadOnlyCollection<DocumentTypeDisplay> documentTypes,
                IReadOnlyCollection<EducationLevelDisplay> educationLevels
            )
        {
            _fileDialogService = fileDialogService;
            _documentConversionService = documentConversionService;

            _documentTypes = documentTypes;
            _educationLevels = educationLevels;

            EducationDocumentsView = CollectionViewSource.GetDefaultView(LocalEducationDocuments);

            EducationDocumentsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(EducationDocumentDisplay.SaveStatus)));

            SelectMultipleCommand = new RelayCommand(Execute_SelectMultipleCommand);
            RemovePendingFileCommand = new RelayCommand<PendingFileItem>(Execute_RemovePendingFileCommand);
            AddEducationDocumentCommandAsync = new RelayCommandAsync(Execute_AddEducationDocumentCommandAsync, CanExecute_AddEducationDocumentCommand);
        }

        private string _documentNumber = null!;
        public string DocumentNumber
        {
            get => _documentNumber;
            set
            {
                SetProperty(ref _documentNumber, value);
                AddEducationDocumentCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private DateTime _issuedDate;
        public DateTime IssuedDate
        {
            get => _issuedDate;
            set
            {
                SetProperty(ref _issuedDate, value);
                AddEducationDocumentCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string _organizationName = null!;
        public string OrganizationName
        {
            get => _organizationName;
            set
            {
                SetProperty(ref _organizationName, value);
                AddEducationDocumentCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string? _qualificationAwardedName;
        public string? QualificationAwardedName
        {
            get => _qualificationAwardedName;
            set
            {
                SetProperty(ref _qualificationAwardedName, value);
                AddEducationDocumentCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string? _specialtyName;
        public string? SpecialtyName
        {
            get => _specialtyName;
            set
            {
                SetProperty(ref _specialtyName, value);
                AddEducationDocumentCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string? _programName;
        public string? ProgramName
        {
            get => _programName;
            set
            {
                SetProperty(ref _programName, value);
                AddEducationDocumentCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private TimeSpan? _totalHours;
        public TimeSpan? TotalHours
        {
            get => _totalHours;
            set
            {
                SetProperty(ref _totalHours, value);
                AddEducationDocumentCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private EducationLevelDisplay _educationLevel = null!;
        public EducationLevelDisplay EducationLevel
        {
            get => _educationLevel;
            set
            {
                SetProperty(ref _educationLevel, value);
                AddEducationDocumentCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private DocumentTypeDisplay _documentType = null!;
        public DocumentTypeDisplay DocumentType
        {
            get => _documentType;
            set
            {
                SetProperty(ref _documentType, value);
                AddEducationDocumentCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        public string? FilePath
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        private EducationDocumentDisplay _selectedEducationDocument = null!;
        public EducationDocumentDisplay SelectedEducationDocument
        {
            get => _selectedEducationDocument;
            set
            {
                if (value != null)
                {
                    SetProp(value);

                    SetProperty(ref _selectedEducationDocument, value);
                }
            }
        }

        private void SetProp(EducationDocumentDisplay value)
        {
            DocumentNumber = value.DocumentNumber;
            IssuedDate = value.IssuedDate;
            OrganizationName = value.OrganizationName;
            QualificationAwardedName = value.QualificationAwardedName;
            SpecialtyName = value.SpecialtyName;
            ProgramName = value.ProgramName;
            TimeSpan.TryParse(value.TotalHours.ToString(), out var resultTimeSpanParse);
            TotalHours = resultTimeSpanParse;
            EducationLevel = value.EducationLevel;
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

        public ObservableCollection<EducationDocumentDisplay> LocalEducationDocuments { get; private init; } = [];
        public ICollectionView EducationDocumentsView  { get; private init; } = null!;

        public RelayCommandAsync AddEducationDocumentCommandAsync { get; private set; }
        private async Task Execute_AddEducationDocumentCommandAsync()
        {
            var filesToProcess = PendingFilePaths.Any()
                ? PendingFilePaths.Select(x => x.FilePath).ToArray()
                : (FilePath != null ? [FilePath] : Array.Empty<string>());

            if (!filesToProcess.Any())
            {
                MessageBox.Show("Выберите хотя бы один файл для добавления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var fileBytes = new List<byte[]>();
                var fileNames = new List<string>();

                foreach (var path in filesToProcess)
                {
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

                var fileName = ".pdf";

                LocalEducationDocuments.Add
                    (
                        new EducationDocumentDisplay
                            (
                                new EducationDocumentResponse
                                    (
                                        string.Empty,
                                        EmployeeId!,
                                        EducationLevel.Id,
                                        DocumentType.Id,
                                        DocumentNumber,
                                        IssuedDate.ToString(),
                                        OrganizationName,
                                        QualificationAwardedName,
                                        SpecialtyName,
                                        ProgramName,
                                        TotalHours.ToString()
                                    ),
                                _educationLevels, 
                                _documentTypes, 
                                FilePath,
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
        private bool CanExecute_AddEducationDocumentCommand()
            => EducationLevel != null && DocumentType != null &&
               !string.IsNullOrWhiteSpace(DocumentNumber) &&
               !string.IsNullOrWhiteSpace(IssuedDate.ToString()) &&
               IssuedDate != DateTime.MinValue &&
               !string.IsNullOrWhiteSpace(OrganizationName);

        public void ClearProperty()
        {
            DocumentNumber = string.Empty;
            IssuedDate = DateTime.UtcNow;
            OrganizationName = string.Empty;
            QualificationAwardedName = string.Empty;
            SpecialtyName = string.Empty;
            ProgramName = string.Empty;
            TotalHours = TimeSpan.Zero;
            EducationLevel = null!;
            DocumentType = null!;
            FilePath = string.Empty;

            PendingFilePaths.Clear();
        }

        private void UpdateIndexes()
        {
            for (int i = 0; i < PendingFilePaths.Count; i++)
                PendingFilePaths[i].Index = i + 1;
        }
    }
}
