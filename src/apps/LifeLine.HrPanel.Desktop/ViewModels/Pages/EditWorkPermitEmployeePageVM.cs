using LifeLine.Directory.Service.Client.Services.AdmissionStatus;
using LifeLine.Directory.Service.Client.Services.PermitType;
using LifeLine.Employee.Service.Client.Services.Employee.WorkPermit;
using LifeLine.File.Service.Client;
using LifeLine.HrPanel.Desktop.Models;
using Shared.Contracts.Request.EmployeeService.WorkPermit;
using Shared.Contracts.Request.Files;
using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Commands;
using Shared.WPF.Constants;
using Shared.WPF.Enums;
using Shared.WPF.Extensions;
using Shared.WPF.Helpers;
using Shared.WPF.Services.FileDialog;
using Shared.WPF.Services.NavigationService.Pages;
using Shared.WPF.ViewModels.Abstract;
using System.Collections.ObjectModel;
using System.Windows;

namespace LifeLine.HrPanel.Desktop.ViewModels.Pages
{
    public sealed class EditWorkPermitEmployeePageVM : BaseViewModel, IUpdatable, IAsyncInitializable
    {
        private readonly INavigationPage _navigationPage;

        private readonly IFileDialogService _fileDialogService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IPermitTypeReadOnlyService _permitTypeReadOnlyService;
        private readonly IWorkPermitApiServiceFactory _workPermitApiServiceFactory;
        private readonly IAdmissionStatusReadOnlyService _admissionStatusReadOnlyService;

        private bool _isEditMode;

        public EditWorkPermitEmployeePageVM
            (
                INavigationPage navigationPage,

                IFileDialogService fileDialogService,
                IFileStorageService fileStorageService,
                IPermitTypeReadOnlyService permitTypeReadOnlyService,
                IAdmissionStatusReadOnlyService admissionStatusReadOnlyService,
                IWorkPermitApiServiceFactory workPermitApiServiceFactory
            )
        {
            _navigationPage = navigationPage;

            _fileDialogService = fileDialogService;
            _fileStorageService = fileStorageService;
            _permitTypeReadOnlyService = permitTypeReadOnlyService;
            _admissionStatusReadOnlyService = admissionStatusReadOnlyService;
            _workPermitApiServiceFactory = workPermitApiServiceFactory;

            UpdateWorkPermitEmployeeCommand = new RelayCommandAsync(Execute_UpdateWorkPermitEmployeeCommand, CanExecute_UpdateWorkPermitEmployeeCommand);
            SelectFileCommand = new RelayCommand(Execute_SelectFileCommand, CanExecute_SelectFileCommand);
        }

        async Task IAsyncInitializable.InitializeAsync()
        {
            if (IsInitialize)
                return;

            IsInitialize = false;

            await GetAllPermitTypeAsync();
            await GetAllAdmissionStatusAsync();

            CreateNewWorkPermitDisplay();

            if (WorkPermitDisplay != null && PermitTypes.Count > 0 && AdmissionStatuses.Count > 0)
            {
                if (WorkPermitDisplay.PermitType != null)
                    WorkPermitDisplay.PermitType = PermitTypes.FirstOrDefault(x => x.Id == WorkPermitDisplay.PermitType.Id)!;

                if (WorkPermitDisplay.AdmissionStatus != null)
                    WorkPermitDisplay.AdmissionStatus = AdmissionStatuses.FirstOrDefault(x => x.Id == WorkPermitDisplay.AdmissionStatus.Id)!;
            }

            IsInitialize = true;
        }

        public void Update<TData>(TData value, TransmittingParameter parameter)
        {
            if (value is ValueTuple<EmployeeDetailsDisplay, WorkPermitDisplay> tuple)
            {
                CurrentEmployeeDetails = tuple.Item1;
                var incomingDocument = tuple.Item2;

                if (incomingDocument != null)
                {
                    _isEditMode = true;

                    WorkPermitDisplay = incomingDocument;

                    if (WorkPermitDisplay != null && PermitTypes.Count > 0 && AdmissionStatuses.Count > 0)
                    {
                        WorkPermitDisplay.PermitType = PermitTypes.FirstOrDefault(x => x.Id == WorkPermitDisplay.PermitType.Id)!;
                        WorkPermitDisplay.AdmissionStatus = AdmissionStatuses.FirstOrDefault(x => x.Id == WorkPermitDisplay.AdmissionStatus.Id)!;
                    }
                }
                else
                {
                    _isEditMode = false;

                    CreateNewWorkPermitDisplay();
                }

                UpdateWorkPermitEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        #region Display

        private EmployeeDetailsDisplay _currentEmployeeDetails = null!;
        public EmployeeDetailsDisplay CurrentEmployeeDetails
        {
            get => _currentEmployeeDetails;
            set => SetProperty(ref _currentEmployeeDetails, value);
        }

        public ObservableCollection<PermitTypeDisplay> PermitTypes { get; private init; } = [];
        private async Task GetAllPermitTypeAsync()
        {
            var permiteTypes = await _permitTypeReadOnlyService.GetAllAsync();

            PermitTypes.Load([.. permiteTypes.Select(permitType => new PermitTypeDisplay(permitType))]);
        }

        public ObservableCollection<AdmissionStatusDisplay> AdmissionStatuses { get; private init; } = [];
        private async Task GetAllAdmissionStatusAsync()
        {
            var admissionStatuses = await _admissionStatusReadOnlyService.GetAllAsync();

            AdmissionStatuses.Load([.. admissionStatuses.Select(admissionStatus => new AdmissionStatusDisplay(admissionStatus))]);
        }

        private WorkPermitDisplay _workPermitDisplay = null!;
        public WorkPermitDisplay WorkPermitDisplay
        {
            get => _workPermitDisplay;
            set => SetProperty(ref _workPermitDisplay, value);
        }
        private void CreateNewWorkPermitDisplay() 
            => WorkPermitDisplay = new WorkPermitDisplay
                (
                    new WorkPermitResponse
                        (
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            DateTime.Now,
                            DateTime.Now,
                            string.Empty,
                            string.Empty
                        ),
                    PermitTypes,
                    AdmissionStatuses,
                    string.Empty,
                    SaveStatus.Local
                );

        public string? FilePath
        {
            get => field;
            set
            {
                SetProperty(ref field, value);
                UpdateWorkPermitEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        public RelayCommandAsync? UpdateWorkPermitEmployeeCommand { get; private set; }
        private async Task Execute_UpdateWorkPermitEmployeeCommand()
        {
            if (_isEditMode)
            {
                // UPDATE
                var resultMiniO = await _fileStorageService.UploadFileAsync
                    (
                        new UploadFileRequest
                            (
                                FileConst.BUCKET_NAME,
                                WorkPermitDisplay.WorkPermitName,
                                FileConst.BuildEmployeeFolder
                                    (
                                        CurrentEmployeeDetails.EmployeeId,
                                        EmployeeFolderType.WorkPermit
                                    ),
                                FilePath!
                            )
                    );

                if (resultMiniO.IsFailure)
                {
                    MessageBox.Show(resultMiniO.StringMessage);
                    return;
                }

                var resultUpdate = await _workPermitApiServiceFactory.Create(CurrentEmployeeDetails.EmployeeId).UpdateWorkPermitAsync
                (
                    Guid.Parse(WorkPermitDisplay.WorkPermitId),
                    new UpdateWorkPermitRequest
                        (
                            WorkPermitDisplay.WorkPermitName,
                            WorkPermitDisplay.DocumentSeries,
                            WorkPermitDisplay.WorkPermitNumber,
                            WorkPermitDisplay.ProtocolNumber,
                            WorkPermitDisplay.SpecialtyName,
                            WorkPermitDisplay.IssuingAuthority,
                            WorkPermitDisplay.IssueDate,
                            WorkPermitDisplay.ExpiryDate,
                            WorkPermitDisplay.PermitType.Id,
                            WorkPermitDisplay.AdmissionStatus.Id
                        )
                );

                if (resultUpdate.IsSuccess)
                    _navigationPage.TransmittingValue(WorkPermitDisplay, FrameName.MainFrame, PageName.EmployeePage, TransmittingParameter.Update);
                else
                    MessageBox.Show($"Обновление разрешения на работу: {resultUpdate.StringMessage}");
            }
            else
            {
                // CREATE
                var resultMiniO = await _fileStorageService.UploadFileAsync
                    (
                        new UploadFileRequest
                            (
                                FileConst.BUCKET_NAME,
                                WorkPermitDisplay.WorkPermitName,
                                FileConst.BuildEmployeeFolder
                                    (
                                        CurrentEmployeeDetails.EmployeeId,
                                        EmployeeFolderType.WorkPermit
                                    ),
                                FilePath!
                            )
                    );

                if (resultMiniO.IsFailure)
                {
                    MessageBox.Show(resultMiniO.StringMessage);
                    return;
                }

                var resultCreate =  await _workPermitApiServiceFactory.Create(CurrentEmployeeDetails.EmployeeId).CreateAsync
                (
                    new CreateWorkPermitRequest
                        (
                            WorkPermitDisplay.WorkPermitName,
                            WorkPermitDisplay.DocumentSeries,
                            WorkPermitDisplay.WorkPermitNumber,
                            WorkPermitDisplay.ProtocolNumber,
                            WorkPermitDisplay.SpecialtyName,
                            WorkPermitDisplay.IssuingAuthority,
                            WorkPermitDisplay.IssueDate,
                            WorkPermitDisplay.ExpiryDate,
                            Guid.Parse(WorkPermitDisplay.PermitType.Id),
                            Guid.Parse(WorkPermitDisplay.AdmissionStatus.Id)
                        )
                );

                if (resultCreate.IsSuccess)
                    _navigationPage.TransmittingValue(WorkPermitDisplay, FrameName.MainFrame, PageName.EmployeePage, TransmittingParameter.Create);
                else
                    MessageBox.Show($"Внесение разрешения на работу: {resultCreate.StringMessage}");
            }
        }
        private bool CanExecute_UpdateWorkPermitEmployeeCommand() 
            => WorkPermitDisplay.PermitType != null && WorkPermitDisplay.AdmissionStatus != null;

        public RelayCommand SelectFileCommand { get; private set; }
        private void Execute_SelectFileCommand()
            => FilePath = _fileDialogService.GetFile($"Выберите файл: {FileDialogConsts.WORK_PERMIT}", FileFilters.Images);
        private bool CanExecute_SelectFileCommand() => true;
    }
}
