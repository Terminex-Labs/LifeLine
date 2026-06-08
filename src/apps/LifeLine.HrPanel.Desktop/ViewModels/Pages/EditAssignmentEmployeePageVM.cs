using LifeLine.Directory.Service.Client.Services.Department;
using LifeLine.Directory.Service.Client.Services.Position.Factories;
using LifeLine.Directory.Service.Client.Services.Status;
using LifeLine.Employee.Service.Client.Services.Employee;
using LifeLine.Employee.Service.Client.Services.Employee.Assignment;
using LifeLine.Employee.Service.Client.Services.EmployeeType;
using LifeLine.File.Service.Client;
using LifeLine.HrPanel.Desktop.Models;
using Shared.Contracts.Request.EmployeeService.Assignment;
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
    public sealed class EditAssignmentEmployeePageVM : BaseViewModel, IUpdatable, IAsyncInitializable
    {
        private readonly INavigationPage _navigationPage;

        private readonly IEmployeeService _employeeService;
        private readonly IFileDialogService _fileDialogService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IStatusReadOnlyService _statusReadOnlyService;
        private readonly IDepartmentReadOnlyService _departmentReadOnlyService;
        private readonly IAssignmentApiServiceFactory _assignmentApiServiceFactory;
        private readonly IEmployeeTypeReadOnlyService _employeeTypeReadOnlyService;
        private readonly IPositionReadOnlyApiServiceFactory _positionReadOnlyApiServiceFactory;

        private bool _isEditMode;

        public EditAssignmentEmployeePageVM
            (
                INavigationPage navigationPage,

                IEmployeeService employeeService,
                IFileDialogService fileDialogService,
                IFileStorageService fileStorageService,
                IStatusReadOnlyService statusReadOnlyService,
                IDepartmentReadOnlyService departmentReadOnlyService,
                IAssignmentApiServiceFactory assignmentApiServiceFactory,
                IEmployeeTypeReadOnlyService employeeTypeReadOnlyService,
                IPositionReadOnlyApiServiceFactory positionReadOnlyApiServiceFactory
            )
        {
            _navigationPage = navigationPage;

            _employeeService = employeeService;
            _fileDialogService = fileDialogService;
            _fileStorageService = fileStorageService;
            _statusReadOnlyService = statusReadOnlyService;
            _departmentReadOnlyService = departmentReadOnlyService;
            _assignmentApiServiceFactory = assignmentApiServiceFactory;
            _employeeTypeReadOnlyService = employeeTypeReadOnlyService;
            _positionReadOnlyApiServiceFactory = positionReadOnlyApiServiceFactory;

            UpdateAssignmentEmployeeCommand = new RelayCommandAsync(Execute_UpdateAssignmentEmployeeCommand, CanExecute_UpdateAssignmentEmployeeCommand);
            SelectFileCommand = new RelayCommand(Execute_SelectFileCommand, CanExecute_SelectFileCommand);
        }

        async Task IAsyncInitializable.InitializeAsync()
        {
            if (IsInitialize)
                return;

            IsInitialize = false;

            await GetAllDepartmentAsync();
            await GetAllManagerAsync();
            await GetAllStatusAsync();
            await GetAllEmployeeTypeAsync();

            CreateNewAssignmentContractDisplay();

            if (AssignmentContractDisplay != null && Departments.Count > 0 && Statuses.Count > 0 && EmployeeTypes.Count > 0)
            {
                //if (AssignmentContractDisplay.Department != null)
                //{
                //    AssignmentContractDisplay.Department = Departments.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Department.Id)!;
                //    await GetAllPositionByIdDepartmentAsync();
                //    AssignmentContractDisplay.Position = Positions.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Position.Id)!;
                //}
                if (AssignmentContractDisplay.Department != null)
                {
                    AssignmentContractDisplay.Department = Departments.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Department.Id)!;
                    // Можно вызвать так:
                    await GetAllPositionByIdDepartmentAsync(AssignmentContractDisplay.Department.Id);

                    AssignmentContractDisplay.Position = Positions.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Position.Id)!;
                }

                if (AssignmentContractDisplay.Manager != null)
                    AssignmentContractDisplay.Manager = Managers.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Manager?.Id);
                
                if (AssignmentContractDisplay.Status != null)
                    AssignmentContractDisplay.Status = Statuses.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Status.Id)!;
                
                if (AssignmentContractDisplay.EmployeeType != null)
                    AssignmentContractDisplay.EmployeeType = EmployeeTypes.FirstOrDefault(x => x.Id == AssignmentContractDisplay.EmployeeType.Id)!;
            }

            IsInitialize = true;
        }

        public void Update<TData>(TData value, TransmittingParameter parameter)
        {
            if (value is ValueTuple<EmployeeDetailsDisplay, AssignmentContractDisplay> tuple)
            {
                CurrentEmployeeDetails = tuple.Item1;
                var incomingDocument = tuple.Item2;

                if (incomingDocument != null)
                {
                    _isEditMode = true;

                    AssignmentContractDisplay = incomingDocument;

                    if (AssignmentContractDisplay != null && Departments.Count > 0 && Statuses.Count > 0 && EmployeeTypes.Count > 0)
                    {
                        //if (AssignmentContractDisplay.Department != null)
                        //{
                        //    AssignmentContractDisplay.Department = Departments.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Department.Id)!;
                        //    await GetAllPositionByIdDepartmentAsync();
                        //    AssignmentContractDisplay.Position = Positions.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Position.Id)!;
                        //}
                        if (AssignmentContractDisplay.Department != null)
                        {
                            AssignmentContractDisplay.Department = Departments.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Department.Id)!;
                            // Можно вызвать так:
                            GetAllPositionByIdDepartmentAsync(AssignmentContractDisplay.Department.Id);

                            AssignmentContractDisplay.Position = Positions.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Position.Id)!;
                        }

                        if (AssignmentContractDisplay.Manager != null)
                            AssignmentContractDisplay.Manager = Managers.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Manager?.Id);

                        if (AssignmentContractDisplay.Status != null)
                            AssignmentContractDisplay.Status = Statuses.FirstOrDefault(x => x.Id == AssignmentContractDisplay.Status.Id)!;

                        if (AssignmentContractDisplay.EmployeeType != null)
                            AssignmentContractDisplay.EmployeeType = EmployeeTypes.FirstOrDefault(x => x.Id == AssignmentContractDisplay.EmployeeType.Id)!;
                    }
                    else
                    {
                        _isEditMode = false;

                        CreateNewAssignmentContractDisplay();
                    }

                    UpdateAssignmentEmployeeCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        #region Display

        private EmployeeDetailsDisplay _currentEmployeeDetails = null!;
        public EmployeeDetailsDisplay CurrentEmployeeDetails
        {
            get => _currentEmployeeDetails;
            set => SetProperty(ref _currentEmployeeDetails, value);
        }

        public ObservableCollection<DepartmentDisplay> Departments { get; private init; } = [];
        private async Task GetAllDepartmentAsync()
        {
            var departments = await _departmentReadOnlyService.GetAllAsync();

            Departments.Load([.. departments.Select(department => new DepartmentDisplay(department))]);
        }

        public ObservableCollection<PositionDisplay> Positions { get; private init; } = [];
        //private async Task GetAllPositionByIdDepartmentAsync()
        //{
        //    var positions = await _positionReadOnlyApiServiceFactory.Create(AssignmentContractDisplay.Department.Id).GetAllAsync();

        //    Positions.Load([.. positions.Select(position => new PositionDisplay(position))], cleaning: true);
        //}

        // Измененный метод получения позиций (теперь принимает ID)
        private async Task GetAllPositionByIdDepartmentAsync(string? departmentId = null)
        {
            // Если ID не передан, берем из текущего свойства (для начальной инициализации)
            if (departmentId == null)
            {
                if (AssignmentContractDisplay?.Department == null) 
                    return;

                departmentId = AssignmentContractDisplay.Department.Id;
            }

            var positions = await _positionReadOnlyApiServiceFactory.Create(departmentId).GetAllAsync();

            // Используем Application.Current.Dispatcher если нужно, но ObservableCollection обычно сама справляется,
            // если включена синхронизация, или через BindingOperations.EnableCollectionSynchronization
            Positions.Clear();
            Positions.Load([.. positions.Select(position => new PositionDisplay(position))], cleaning: true);
        }

        public ObservableCollection<StatusDisplay> Statuses { get; private init; } = [];
        private async Task GetAllStatusAsync()
        {
            var statuses = await _statusReadOnlyService.GetAllAsync();

            Statuses.Load([.. statuses.Select(status => new StatusDisplay(status))]);
        }

        public ObservableCollection<EmployeeTypeDisplay> EmployeeTypes { get; private init; } = [];
        private async Task GetAllEmployeeTypeAsync()
        {
            var employeeTypes = await _employeeTypeReadOnlyService.GetAllAsync();

            EmployeeTypes.Load([.. employeeTypes.Select(employeeType => new EmployeeTypeDisplay(employeeType))]);
        }

        public ObservableCollection<ManagerDisplay> Managers { get; private init; } = [];
        private async Task GetAllManagerAsync()
        {
            var managers = await _employeeService.GetAllAsync();

            Managers.Load([.. managers.Select(manager => new ManagerDisplay(manager))]);
        }

        //private AssignmentContractDisplay _assignmentContractDisplay = null!;
        //public AssignmentContractDisplay AssignmentContractDisplay
        //{
        //    get => _assignmentContractDisplay;
        //    set
        //    {
        //        SetProperty(ref _assignmentContractDisplay, value);

        //        AssignmentContractDisplay.PropertyChanged += async (s, e) =>
        //        {
        //            if (e.PropertyName == nameof(AssignmentContractDisplay.Department))
        //                if (AssignmentContractDisplay.Department != null)
        //                    await GetAllPositionByIdDepartmentAsync();
        //        };
        //    }
        //}

        private AssignmentContractDisplay _assignmentContractDisplay = null!;
        public AssignmentContractDisplay AssignmentContractDisplay
        {
            get => _assignmentContractDisplay;
            set
            {
                // 1. Отписываемся от старого объекта, если он был
                if (_assignmentContractDisplay != null)
                    _assignmentContractDisplay.PropertyChanged -= OnAssignmentContractDisplayChanged;

                // 2. Присваиваем новое значение
                SetProperty(ref _assignmentContractDisplay!, value);

                UpdateAssignmentEmployeeCommand?.RaiseCanExecuteChanged();

                // 3. Подписываемся на новый объект, ТОЛЬКО если он не null
                if (_assignmentContractDisplay != null)
                    _assignmentContractDisplay.PropertyChanged += OnAssignmentContractDisplayChanged;
            }
        }

        // Вынесенный обработчик события
        private async void OnAssignmentContractDisplayChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Безопасное приведение sender. Мы работаем с тем объектом, который вызвал событие,
            // а не с глобальным свойством VM (которое может быть уже null).
            if (sender is AssignmentContractDisplay display)
            {
                if (e.PropertyName == nameof(AssignmentContractDisplay.Department))
                {
                    if (display.Department != null)
                    {
                        // Передаем ID явно, чтобы метод загрузки не зависел от состояния VM
                        await GetAllPositionByIdDepartmentAsync(display.Department.Id);
                    }
                }
            }
        }
        private void CreateNewAssignmentContractDisplay()
        {
            AssignmentContractDisplay = new AssignmentContractDisplay
                (
                    new AssignmentResponse
                        (
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            DateTime.Now,
                            DateTime.Now,
                            string.Empty
                        ),

                    new ContractResponse
                        (
                            string.Empty, 
                            string.Empty, 
                            string.Empty,
                            string.Empty,
                            DateTime.Now,
                            DateTime.Now,
                            0,
                            null
                        ),
                    Departments,
                    Positions,
                    Managers,
                    Statuses,
                    EmployeeTypes,
                    SaveStatus.Local
                );
        }

        public string? FilePath
        {
            get => field;
            set
            {
                SetProperty( ref field, value );
                UpdateAssignmentEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        public RelayCommandAsync? UpdateAssignmentEmployeeCommand { get; private set; }
        private async Task Execute_UpdateAssignmentEmployeeCommand()
        {
            if (_isEditMode)
            {
                // UPDATE
                try
                {
                    var resultMiniO = await _fileStorageService.UploadFileAsync
                        (
                            new UploadFileRequest
                                (
                                    FileConst.BUCKET_NAME,
                                    AssignmentContractDisplay.Position.Name,
                                    FileConst.BuildEmployeeFolder
                                        (
                                            CurrentEmployeeDetails.EmployeeId,
                                            EmployeeFolderType.Assignment
                                        ),
                                    FilePath!
                                )
                        );

                    if (resultMiniO.IsFailure)
                    {
                        MessageBox.Show(resultMiniO.StringMessage);
                        return;
                    }

                    if (AssignmentContractDisplay == null)
                    {
                        MessageBox.Show("Данные о назначении не загружены");
                        return;
                    }

                    var resultUpdate = await _assignmentApiServiceFactory.Create(CurrentEmployeeDetails.EmployeeId).UpdateAssignmentAsync
                        (
                            Guid.Parse(AssignmentContractDisplay.AssignmentId),
                            Guid.Parse(AssignmentContractDisplay.ContractId),
                            new UpdateAssignmentRequest
                                (
                                    Guid.Parse(AssignmentContractDisplay.Position.Id),
                                    Guid.Parse(AssignmentContractDisplay.Department.Id),
                                    AssignmentContractDisplay.Manager?.Id != null ? Guid.Parse(AssignmentContractDisplay.Manager.Id) : null,
                                    AssignmentContractDisplay.HireDate,
                                    AssignmentContractDisplay.TerminationDate,
                                    Guid.Parse(AssignmentContractDisplay.Status.Id),
                                    new UpdateAssignmentContractRequest
                                        (
                                            Guid.Parse(AssignmentContractDisplay.EmployeeType.Id),
                                            AssignmentContractDisplay.ContractNumber,
                                            AssignmentContractDisplay.StartDate,
                                            AssignmentContractDisplay.EndDate,
                                            AssignmentContractDisplay.Salary,
                                            null, null
                                        )
                                )
                        );

                    if (resultUpdate.IsSuccess)
                        _navigationPage.TransmittingValue(AssignmentContractDisplay, FrameName.MainFrame, PageName.EmployeePage, TransmittingParameter.Update);
                    else
                        MessageBox.Show($"Обновление назначения и контракта: {resultUpdate.StringMessage}");
                }
                catch (ArgumentNullException ex)
                {
                    MessageBox.Show($"Ошибка данных: {ex.ParamName} - {ex.Message}");
                }
                catch (FormatException ex)
                {
                    MessageBox.Show($"Ошибка преобразования GUID: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления: {ex.Message}");
                }
            }
            else
            {
                // CREATE
                var resultMiniO = await _fileStorageService.UploadFileAsync
                    (
                        new UploadFileRequest
                            (
                                FileConst.BUCKET_NAME,
                                AssignmentContractDisplay.Position.Name,
                                FileConst.BuildPatientFolder
                                    (
                                        CurrentEmployeeDetails.EmployeeId,
                                        EmployeeFolderType.Assignment
                                    ),
                                FilePath!
                            )
                    );

                if (resultMiniO.IsFailure)
                {
                    MessageBox.Show(resultMiniO.StringMessage);
                    return;
                }

                var resultCreate = await _assignmentApiServiceFactory.Create(CurrentEmployeeDetails.EmployeeId).CreateAsync
                    (
                        new CreateAssignmentRequest
                            (
                                Guid.Parse(AssignmentContractDisplay.Position.Id),
                                Guid.Parse(AssignmentContractDisplay.Department.Id),
                                AssignmentContractDisplay.Manager?.Id != null ? Guid.Parse(AssignmentContractDisplay.Manager.Id) : null,
                                AssignmentContractDisplay.HireDate,
                                AssignmentContractDisplay.TerminationDate,
                                Guid.Parse(AssignmentContractDisplay.Status.Id),
                                new CreateAssignmentContractRequest
                                    (
                                        Guid.Parse(AssignmentContractDisplay.EmployeeType.Id),
                                        AssignmentContractDisplay.ContractNumber,
                                        AssignmentContractDisplay.StartDate,
                                        AssignmentContractDisplay.EndDate,
                                        AssignmentContractDisplay.Salary
                                    )
                            )
                    );

                if (resultCreate.IsSuccess)
                    _navigationPage.TransmittingValue(AssignmentContractDisplay, FrameName.MainFrame, PageName.EmployeePage, TransmittingParameter.Create);
                else
                    MessageBox.Show($"Создание назначения и контракта: {resultCreate.StringMessage}");
            }

            //var resultUpdate = await _assignmentApiServiceFactory.Create(CurrentEmployeeDetails.EmployeeId).UpdateAssignmentAsync
            //    (
            //        Guid.Parse(AssignmentContractDisplay.AssignmentId),
            //        Guid.Parse(AssignmentContractDisplay.ContractId),
            //        new UpdateAssignmentRequest
            //            (
            //                Guid.Parse(AssignmentContractDisplay.Position.Id),
            //                Guid.Parse(AssignmentContractDisplay.Department.Id),
            //                AssignmentContractDisplay.Manager?.Id != null ? Guid.Parse(AssignmentContractDisplay.Manager.Id) : null,
            //                AssignmentContractDisplay.HireDate,
            //                AssignmentContractDisplay.TerminationDate,
            //                Guid.Parse(AssignmentContractDisplay.Status.Id),
            //                new UpdateAssignmentContractRequest
            //                    (
            //                        Guid.Parse(AssignmentContractDisplay.EmployeeType.Id),
            //                        AssignmentContractDisplay.ContractNumber,
            //                        AssignmentContractDisplay.StartDate,
            //                        AssignmentContractDisplay.EndDate,
            //                        AssignmentContractDisplay.Salary,
            //                        null
            //                    )
            //            )
            //    );

            //if (resultUpdate.IsSuccess)
            //    _navigationPage.TransmittingValue(AssignmentContractDisplay, FrameName.MainFrame, PageName.EmployeePage, TransmittingParameter.Update);
            //else
            //    MessageBox.Show($"Обновление назначения: {resultUpdate.StringMessage}");
        }
        private bool CanExecute_UpdateAssignmentEmployeeCommand() => true;
            //=> AssignmentContractDisplay != null &&
            //   AssignmentContractDisplay.Position != null &&
            //   AssignmentContractDisplay.Department != null &&
            //   AssignmentContractDisplay.Status != null &&
            //   AssignmentContractDisplay.EmployeeType != null &&
            //   AssignmentContractDisplay.HireDate != DateTime.MinValue &&
            //   !string.IsNullOrWhiteSpace(AssignmentContractDisplay.ContractNumber) &&
            //   AssignmentContractDisplay.StartDate != DateTime.MinValue &&
            //   AssignmentContractDisplay.EndDate != DateTime.MinValue &&
            //   AssignmentContractDisplay.Salary > 0;


        public RelayCommand SelectFileCommand { get; private set; }
        private void Execute_SelectFileCommand()
            => FilePath = _fileDialogService.GetFile($"Выберите файл: {FileDialogConsts.ASSIGNMENT}", FileFilters.Images);
        private bool CanExecute_SelectFileCommand() => true;
    }
}
