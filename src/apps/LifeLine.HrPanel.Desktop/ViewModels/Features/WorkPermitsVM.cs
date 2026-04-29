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
    internal sealed class WorkPermitsVM : BaseEmployeeViewModel
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly IDocumentConversionService _documentConversionService;

        private readonly IReadOnlyCollection<PermitTypeDisplay> _permitTypes;
        private readonly IReadOnlyCollection<AdmissionStatusDisplay> _admissionStatuses;

        public WorkPermitsVM
            (
                IFileDialogService fileDialogService, 
                IDocumentConversionService documentConversionService,
                IReadOnlyCollection<PermitTypeDisplay> permitTypes, 
                IReadOnlyCollection<AdmissionStatusDisplay> admissionStatuses
            )
        {
            _fileDialogService = fileDialogService;
            _documentConversionService = documentConversionService;

            _permitTypes = permitTypes;
            _admissionStatuses = admissionStatuses;

            WorkPermitsView = CollectionViewSource.GetDefaultView(LocalWorkPermits);

            WorkPermitsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(WorkPermitDisplay.SaveStatus)));

            SelectMultipleCommand = new RelayCommand(Execute_SelectMultipleCommand);
            RemovePendingFileCommand = new RelayCommand<PendingFileItem>(Execute_RemovePendingFileCommand);
            AddWorkPermitCommandAsync = new RelayCommandAsync(Execute_AddWorkPermitCommandAsync, CanExecute_AddWorkPermitCommand);
            DeleteWorkPermitCommand = new RelayCommand<WorkPermitDisplay>(Execute_DeleteWorkPermitCommand);
        }

        private string _workPermitName = null!;
        public string WorkPermitName
        {
            get => _workPermitName;
            set
            {
                SetProperty(ref _workPermitName, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string? _documentSeries;
        public string? DocumentSeries
        {
            get => _documentSeries;
            set
            {
                SetProperty(ref _documentSeries, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string _workPermitNumber = null!;
        public string WorkPermitNumber
        {
            get => _workPermitNumber;
            set
            {
                SetProperty(ref _workPermitNumber, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string? _protocolNumber;
        public string? ProtocolNumber
        {
            get => _protocolNumber;
            set
            {
                SetProperty(ref _protocolNumber, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string _specialtyName = null!;
        public string SpecialtyName
        {
            get => _specialtyName;
            set
            {
                SetProperty(ref _specialtyName, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string _issuingAuthority = null!;
        public string IssuingAuthority
        {
            get => _issuingAuthority;
            set
            {
                SetProperty(ref _issuingAuthority, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private DateTime _issueDate;
        public DateTime IssueDate
        {
            get => _issueDate;
            set
            {
                SetProperty(ref _issueDate, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private DateTime _expiryDate;
        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set
            {
                SetProperty(ref _expiryDate, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private PermitTypeDisplay _permitType = null!;
        public PermitTypeDisplay PermitType
        {
            get => _permitType;
            set
            {
                SetProperty(ref _permitType, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private AdmissionStatusDisplay _admissionStatus = null!;
        public AdmissionStatusDisplay AdmissionStatus
        {
            get => _admissionStatus;
            set
            {
                SetProperty(ref _admissionStatus, value);
                AddWorkPermitCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        public string? FilePath
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        private WorkPermitDisplay _selectedWorkPermit = null!;
        public WorkPermitDisplay SelectedWorkPermit
        {
            get => _selectedWorkPermit;
            set
            {
                SetProp(value);

                SetProperty(ref _selectedWorkPermit, value);
            }
        }

        private void SetProp(WorkPermitDisplay value)
        {
            WorkPermitName = value.WorkPermitName;
            DocumentSeries = value.DocumentSeries;
            WorkPermitNumber = value.WorkPermitNumber;
            ProtocolNumber = value.ProtocolNumber;
            SpecialtyName = value.SpecialtyName;
            IssuingAuthority = value.IssuingAuthority;
            IssueDate = value.IssueDate;
            ExpiryDate = value.ExpiryDate;
            PermitType = value.PermitType;
            AdmissionStatus = value.AdmissionStatus;
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

        public ObservableCollection<WorkPermitDisplay> LocalWorkPermits { get; private init; } = [];
        public ICollectionView WorkPermitsView { get; private init; } = null!;

        public RelayCommandAsync AddWorkPermitCommandAsync { get; private set; }
        private async Task Execute_AddWorkPermitCommandAsync()
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
                        PermitType.Name,
                        EmployeeId!,
                        fileBytes,
                        fileNames
                    );

                var fileName = $".pdf";

                LocalWorkPermits.Add
                    (
                        new WorkPermitDisplay
                            (
                                new WorkPermitResponse
                                    (
                                        string.Empty,
                                        EmployeeId!,
                                        WorkPermitName,
                                        DocumentSeries,
                                        WorkPermitNumber,
                                        ProtocolNumber,
                                        SpecialtyName,
                                        IssuingAuthority,
                                        IssueDate,
                                        ExpiryDate,
                                        PermitType.Id,
                                        AdmissionStatus.Id
                                    ),
                                _permitTypes, 
                                _admissionStatuses, 
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
        private bool CanExecute_AddWorkPermitCommand()
            => AdmissionStatus != null && PermitType != null &&
            !string.IsNullOrWhiteSpace(WorkPermitName) &&
            !string.IsNullOrWhiteSpace(WorkPermitNumber) &&
            !string.IsNullOrWhiteSpace(SpecialtyName) &&
            !string.IsNullOrWhiteSpace(IssuingAuthority) &&
            IssueDate != DateTime.MinValue && 
            !string.IsNullOrWhiteSpace(IssueDate.ToString()) &&
            ExpiryDate != DateTime.MinValue &&
            !string.IsNullOrWhiteSpace(ExpiryDate.ToString());

        public RelayCommand<WorkPermitDisplay> DeleteWorkPermitCommand { get; private set; }
        private void Execute_DeleteWorkPermitCommand(WorkPermitDisplay display)
            => LocalWorkPermits.Remove(display);

        public void ClearProperty()
        {
            WorkPermitName = string.Empty;
            DocumentSeries = string.Empty;
            WorkPermitNumber = string.Empty;
            ProtocolNumber = string.Empty;
            SpecialtyName = string.Empty;
            IssuingAuthority = string.Empty;
            IssueDate = DateTime.UtcNow;
            ExpiryDate = DateTime.UtcNow;
            PermitType = null!;
            AdmissionStatus = null!;
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
