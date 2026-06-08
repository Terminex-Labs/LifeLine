using LifeLine.Directory.Service.Client.Services.Position.Factories;
using LifeLine.File.Service.Client;
using LifeLine.HrPanel.Desktop.Models;
using LifeLine.HrPanel.Desktop.Services.Document.DocumentProcessing;
using LifeLine.HrPanel.Desktop.Services.FilePreview;
using Shared.Contracts.Request.Files;
using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Commands;
using Shared.WPF.Constants;
using Shared.WPF.Enums;
using Shared.WPF.Extensions;
using Shared.WPF.Helpers;
using Shared.WPF.Services.FileDialog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace LifeLine.HrPanel.Desktop.ViewModels.Features
{
    internal sealed class AssigmentsContractsVM : BaseEmployeeViewModel
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFilePreviewService _filePreviewService;
        private readonly IDocumentProcessingService _documentProcessingService;
        private readonly IPositionReadOnlyApiServiceFactory _positionReadOnlyApiServiceFactory;

        private readonly IReadOnlyCollection<DepartmentDisplay> _departments;
        private readonly IReadOnlyCollection<ManagerDisplay> _managers;
        private readonly IReadOnlyCollection<StatusDisplay> _statuses;
        private readonly IReadOnlyCollection<EmployeeTypeDisplay> _employeeTypes;

        public AssigmentsContractsVM
            (
                IFileDialogService fileDialogService,
                IFileStorageService fileStorageService,
                IFilePreviewService filePreviewService,
                IDocumentProcessingService documentProcessingService,
                IPositionReadOnlyApiServiceFactory positionReadOnlyApiServiceFactory,

                IReadOnlyCollection<DepartmentDisplay> departments,
                IReadOnlyCollection<ManagerDisplay> managers,
                IReadOnlyCollection<StatusDisplay> statuses,
                IReadOnlyCollection<EmployeeTypeDisplay> employeeTypes
            )
        {
            _fileDialogService = fileDialogService;
            _fileStorageService = fileStorageService;
            _filePreviewService = filePreviewService;
            _documentProcessingService = documentProcessingService;
            _positionReadOnlyApiServiceFactory = positionReadOnlyApiServiceFactory;

            _departments = departments;
            _managers = managers;
            _statuses = statuses;
            _employeeTypes = employeeTypes;

            AssignmentsContractsView = CollectionViewSource.GetDefaultView(LocalAssignmentsContracts);

            AssignmentsContractsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(AssignmentContractDisplay.SaveStatus)));

            SelectMultipleCommand = new RelayCommand(Execute_SelectMultipleCommand);
            PreviewCommand = new RelayCommandAsync<PendingFileItem>(Execute_PreviewCommand);
            RemovePendingFileCommand = new RelayCommand<PendingFileItem>(Execute_RemovePendingFileCommand);
            AddAssignmentContractCommandAsync = new RelayCommandAsync(Execute_AddAssignmentContractCommandAsync, CanExecute_AddAssignmentContractCommand);

            _getAllPositionByIdDepartmentCommandAsync = new RelayCommandAsync<DepartmentDisplay>(Execute_GetAllPositionByIdDepartmentCommandAsyn);
        }

        #region Assignment

        private DepartmentDisplay _department = null!;
        public DepartmentDisplay Department
        {
            get => _department;
            set
            {
                if (SetProperty(ref _department, value))
                {
                    if (value != null)
                        _getAllPositionByIdDepartmentCommandAsync.Execute(value);
                    else
                        Positions.Clear();
                }

                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private PositionDisplay _position = null!;
        public PositionDisplay Position
        {
            get => _position;
            set
            {
                SetProperty(ref _position, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private ManagerDisplay? _manager;
        public ManagerDisplay? Manager
        {
            get => _manager;
            set
            {
                SetProperty(ref _manager, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private DateTime _hireDate = DateTime.Now;
        public DateTime HireDate
        {
            get => _hireDate;
            set
            {
                SetProperty(ref _hireDate, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private DateTime? _terminationDate = DateTime.Now;
        public DateTime? TerminationDate
        {
            get => _terminationDate;
            set
            {
                SetProperty(ref _terminationDate, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private StatusDisplay _status = null!;
        public StatusDisplay Status
        {
            get => _status;
            set
            {
                SetProperty(ref _status, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        public string? FilePath
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        #endregion

        #region Contract

        private EmployeeTypeDisplay _employeeType = null!;
        public EmployeeTypeDisplay EmployeeType
        {
            get => _employeeType;
            set
            {
                SetProperty(ref _employeeType, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private string _contractNumber = null!;
        public string ContractNumber
        {
            get => _contractNumber;
            set
            {
                SetProperty(ref _contractNumber, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private DateTime _startDate = DateTime.Now;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                SetProperty(ref _startDate, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                SetProperty(ref _endDate, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        private decimal _salary;
        public decimal Salary
        {
            get => _salary;
            set
            {
                SetProperty(ref _salary, value);
                AddAssignmentContractCommandAsync?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        private AssignmentContractDisplay? _selectedLocalAssignmentContract;
        public AssignmentContractDisplay? SelectedLocalAssignmentContract
        {
            get => _selectedLocalAssignmentContract;
            set
            {
                if (value != null)
                {
                    SetProp(value);

                    SetProperty(ref _selectedLocalAssignmentContract, value);

                    _ = LoadDocumentToQueueAsync(value);
                }
            }
        }

        private void SetProp(AssignmentContractDisplay value)
        {
            Department = value.Department;
            Position = value.Position;
            Manager = value.Manager;
            HireDate = value.HireDate;
            TerminationDate = value.TerminationDate;
            Status = value.Status;

            EmployeeType = value.EmployeeType;
            ContractNumber = value.ContractNumber;
            StartDate = value.StartDate;
            EndDate = value.EndDate;
            Salary = value.Salary;
        }

        private async Task LoadDocumentToQueueAsync(AssignmentContractDisplay document)
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
            if (item == null)
            {
                Debug.WriteLine($"[AssigmentsContractsVM] [Execute_PreviewCommand] item пуст!");
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

        public RelayCommand<PendingFileItem>? RemovePendingFileCommand { get; private set; }
        private void Execute_RemovePendingFileCommand(PendingFileItem item)
        {
            if (item != null && PendingFilePaths.Remove(item))
                UpdateIndexes();
        }

        public ObservableCollection<PositionDisplay> Positions { get; private init; } = [];
        private RelayCommandAsync<DepartmentDisplay> _getAllPositionByIdDepartmentCommandAsync;
        private async Task Execute_GetAllPositionByIdDepartmentCommandAsyn(DepartmentDisplay display)
        {
            if (display == null || display.Id == string.Empty)
            {
                Positions.Clear();
                return;
            }

            var positions = await _positionReadOnlyApiServiceFactory.Create(display.Id.ToString()).GetAllAsync();

            Positions.Load([.. positions.Select(position => new PositionDisplay(position))], cleaning: true);
        }

        public ObservableCollection<AssignmentContractDisplay> LocalAssignmentsContracts { get; private init; } = [];
        public ICollectionView AssignmentsContractsView { get; private init; } = null!;

        public RelayCommandAsync AddAssignmentContractCommandAsync { get; private set; }
        private async Task Execute_AddAssignmentContractCommandAsync()
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
                    Position.Name,
                    EmployeeId!,
                    ContractNumber
                );

            if (processResult.IsFailure)
            {
                MessageBox.Show(processResult.StringMessage);
                return;
            }

            var (pdfBytes, fileName) = processResult.Value;

            LocalAssignmentsContracts.Add
            (
                new AssignmentContractDisplay
                    (
                        new AssignmentResponse
                            (
                                string.Empty,
                                EmployeeId!,
                                Position.Id,
                                Department.Id,
                                Manager?.Id,
                                HireDate,
                                TerminationDate,
                                Status.Id
                            ),
                        new ContractResponse
                            (
                                EmployeeId!,
                                string.Empty,
                                ContractNumber,
                                EmployeeType.Id,
                                StartDate,
                                EndDate,
                                Salary,
                                null
                            ),
                        _departments,
                        Positions,
                        _managers,
                        _statuses,
                        _employeeTypes,
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
        private bool CanExecute_AddAssignmentContractCommand()
            => Department != null && Position != null &&
               EmployeeType != null && Status != null &&
               !string.IsNullOrWhiteSpace(HireDate.ToString()) &&
               !string.IsNullOrWhiteSpace(ContractNumber) &&
               !string.IsNullOrWhiteSpace(StartDate.ToString()) &&
               !string.IsNullOrWhiteSpace(EndDate.ToString()) &&
               !string.IsNullOrWhiteSpace(Salary.ToString());

        public void ClearProperty()
        {
            Department = null!;
            Position = null!;
            Manager = null;
            HireDate = DateTime.Now;
            TerminationDate = DateTime.Now;
            Status = null!;
            FilePath = string.Empty;

            EmployeeType = null!;
            ContractNumber = string.Empty;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            Salary = decimal.Zero;

            PendingFilePaths.Clear();
            SelectedLocalAssignmentContract = null!;
        }

        private void UpdateIndexes()
        {
            for (int i = 0; i < PendingFilePaths.Count; i++)
                PendingFilePaths[i].Index = i + 1;
        }
    }
}
