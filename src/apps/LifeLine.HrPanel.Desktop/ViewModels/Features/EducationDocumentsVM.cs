using LifeLine.File.Service.Client;
using LifeLine.HrPanel.Desktop.Models;
using LifeLine.HrPanel.Desktop.Services.Document.DocumentProcessing;
using LifeLine.HrPanel.Desktop.Services.FilePreview;
using Shared.Contracts.Request.Files;
using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Commands;
using Shared.WPF.Constants;
using Shared.WPF.Enums;
using Shared.WPF.Helpers;
using Shared.WPF.Services.FileDialog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace LifeLine.HrPanel.Desktop.ViewModels.Features
{
    internal sealed class EducationDocumentsVM : BaseEmployeeViewModel
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFilePreviewService _filePreviewService;
        private readonly IDocumentProcessingService _documentProcessingService;

        private readonly IReadOnlyCollection<DocumentTypeDisplay> _documentTypes;
        private readonly IReadOnlyCollection<EducationLevelDisplay> _educationLevels;

        public EducationDocumentsVM
            (
                IFileDialogService fileDialogService,
                IFileStorageService fileStorageService,
                IFilePreviewService filePreviewService,
                IDocumentProcessingService documentProcessingService,

                IReadOnlyCollection<DocumentTypeDisplay> documentTypes,
                IReadOnlyCollection<EducationLevelDisplay> educationLevels
            )
        {
            _fileDialogService = fileDialogService;
            _fileStorageService = fileStorageService;
            _filePreviewService = filePreviewService;
            _documentProcessingService = documentProcessingService;

            _documentTypes = documentTypes;
            _educationLevels = educationLevels;

            EducationDocumentsView = CollectionViewSource.GetDefaultView(LocalEducationDocuments);

            EducationDocumentsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(EducationDocumentDisplay.SaveStatus)));

            SelectMultipleCommand = new RelayCommand(Execute_SelectMultipleCommand);
            PreviewCommand = new RelayCommandAsync<PendingFileItem>(Execute_PreviewCommand);
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

        private EducationDocumentDisplay? _selectedLocalEducationDocument = null!;
        public EducationDocumentDisplay? SelectedLocalEducationDocument
        {
            get => _selectedLocalEducationDocument;
            set
            {
                if (value != null)
                {
                    SetProp(value);

                    SetProperty(ref _selectedLocalEducationDocument, value);

                    _ = LoadDocumentToQueueAsync(value);
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

        private async Task LoadDocumentToQueueAsync(EducationDocumentDisplay document)
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

            var pendingItem = PendingFileItem.FromMetadata(PendingFilePaths.Count + 1, metadataResult.Value, document.FileKey);

            PendingFilePaths.Add(pendingItem);
            UpdateIndexes();
        }

        public ObservableCollection<PendingFileItem> PendingFilePaths { get; private set; } = [];

        public RelayCommand SelectMultipleCommand { get; private set; }
        private void Execute_SelectMultipleCommand()
        {
            var paths = _fileDialogService.GetFiles($"Выберите файлы: {FileDialogConsts.EDUCATION_DOCUMENT}", FileFilters.ImagesAndPdf);

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

        public RelayCommandAsync<PendingFileItem>? PreviewCommand { get; private set; }
        private async Task Execute_PreviewCommand(PendingFileItem item)
        {
            if (item == null)
            {
                Debug.WriteLine($"[EducationDocumentsVM] [Execute_PreviewCommand] item пуст!");
                return;
            }

            try
            {
                string? tempPath = null;

                if (item.IsRemoteFile && !string.IsNullOrWhiteSpace(item.S3Url))
                    tempPath = await _filePreviewService.DownloadRemoteFileToTempAsync(item.S3Url, item.FileName);
                else if (!string.IsNullOrWhiteSpace(item.FilePath) && System.IO.File.Exists(item.FilePath))
                    tempPath = _filePreviewService.CopyLocalFileToTempAsync(item.FilePath, item.FileName);

                if (string.IsNullOrWhiteSpace(tempPath))
                {
                    MessageBox.Show("Не удалось подготовить файл для просмотра", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _filePreviewService.OpenInDefaultApplication(tempPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PreviewCommand] Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ObservableCollection<EducationDocumentDisplay> LocalEducationDocuments { get; private init; } = [];
        public ICollectionView EducationDocumentsView  { get; private init; } = null!;

        public RelayCommandAsync AddEducationDocumentCommandAsync { get; private set; }
        private async Task Execute_AddEducationDocumentCommandAsync()
        {
            if (!PendingFilePaths.Any())
            {
                MessageBox.Show("Выберите хотя бы один файл для добавления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var processResult = await _documentProcessingService.ProcessFilesToPdfAsync
                (
                    PendingFilePaths,
                    DocumentType.Name,
                    EmployeeId!,
                    DocumentNumber
                );

            if (processResult.IsFailure)
            {
                MessageBox.Show(processResult.StringMessage);
                return;
            }

            var (pdfBytes, fileName) = processResult.Value;

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
                                    TotalHours.ToString(),
                                    null
                                ),
                            _educationLevels,
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

            PendingFilePaths.Clear();
            SelectedLocalEducationDocument = null;
        }

        private void UpdateIndexes()
        {
            for (int i = 0; i < PendingFilePaths.Count; i++)
                PendingFilePaths[i].Index = i + 1;
        }
    }
}
