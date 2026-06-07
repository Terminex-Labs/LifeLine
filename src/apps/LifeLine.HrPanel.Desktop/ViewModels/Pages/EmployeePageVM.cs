using LifeLine.Directory.Service.Client.Services.AdmissionStatus;
using LifeLine.Directory.Service.Client.Services.Department;
using LifeLine.Directory.Service.Client.Services.DocumentType;
using LifeLine.Directory.Service.Client.Services.EducationLevel;
using LifeLine.Directory.Service.Client.Services.PermitType;
using LifeLine.Directory.Service.Client.Services.Position.Factories;
using LifeLine.Directory.Service.Client.Services.Status;
using LifeLine.Employee.Service.Client.Services.Employee;
using LifeLine.Employee.Service.Client.Services.Employee.Assignment;
using LifeLine.Employee.Service.Client.Services.Employee.ContactInformation;
using LifeLine.Employee.Service.Client.Services.Employee.EducationDocument;
using LifeLine.Employee.Service.Client.Services.Employee.EmployeeSpecialtry;
using LifeLine.Employee.Service.Client.Services.Employee.PersonalDocument;
using LifeLine.Employee.Service.Client.Services.Employee.WorkPermit;
using LifeLine.Employee.Service.Client.Services.EmployeeType;
using LifeLine.Employee.Service.Client.Services.Gender;
using LifeLine.Employee.Service.Client.Services.Specialty;
using LifeLine.File.Service.Client;
using LifeLine.HrPanel.Desktop.Enums;
using LifeLine.HrPanel.Desktop.Models;
using LifeLine.HrPanel.Desktop.Services.Document.DocumentDeletion;
using LifeLine.HrPanel.Desktop.Services.Document.DocumentProcessing;
using LifeLine.HrPanel.Desktop.Services.Document.DocumentSave;
using LifeLine.HrPanel.Desktop.Services.Document.DocumentUpdate;
using LifeLine.HrPanel.Desktop.Services.FilePreview;
using LifeLine.HrPanel.Desktop.Services.GenerateImage;
using LifeLine.HrPanel.Desktop.Services.GeneratePdf;
using LifeLine.HrPanel.Desktop.ViewModels.Features;
using Shared.Contracts.Request.EmployeeService.Assignment;
using Shared.Contracts.Request.EmployeeService.ContactInformation;
using Shared.Contracts.Request.EmployeeService.EducationDocument;
using Shared.Contracts.Request.EmployeeService.Employee;
using Shared.Contracts.Request.EmployeeService.EmployeeSpecialty;
using Shared.Contracts.Request.EmployeeService.PersonalDocument;
using Shared.Contracts.Request.EmployeeService.WorkPermit;
using Shared.Contracts.Request.Files;
using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Commands;
using Shared.WPF.Enums;
using Shared.WPF.Extensions;
using Shared.WPF.Helpers;
using Shared.WPF.Services.Conversion;
using Shared.WPF.Services.FileDialog;
using Shared.WPF.Services.NavigationService.Pages;
using Shared.WPF.ViewModels.Abstract;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using Terminex.Common.Results;

namespace LifeLine.HrPanel.Desktop.ViewModels.Pages
{
    internal sealed class EmployeePageVM : BasePageViewModel, IUpdatable, IAsyncInitializable
    {

        private readonly INavigationPage _navigationPage;

        private readonly IFileDialogService _fileDialogService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFilePreviewService _filePreviewService;
        private readonly IGeneratePdfService _generatePdfService;
        private readonly IDocumentSaveService _documentSaveService;
        private readonly IDocumentUpdateService _documentUpdateService;
        private readonly IGenerateImageService _generateImageService;
        private readonly IImageCompressionService _imageCompressionService;
        private readonly IDocumentConversionService _documentConversionService;
        private readonly IDocumentProcessingService _documentProcessingService;
        private readonly IDocumentDeletionService _documentDeletionService;

        private readonly IEmployeeService _employeeService;
        private readonly IGenderReadOnlyService _genderReadOnlyService;
        private readonly IStatusReadOnlyService _statusReadOnlyService;
        private readonly ISpecialtyReadOnlyService _specialtyReadOnlyService;
        private readonly IPermitTypeReadOnlyService _permitTypeReadOnlyService;
        private readonly IDepartmentReadOnlyService _departmentReadOnlyService;
        private readonly IAssignmentApiServiceFactory _assignmentApiServiceFactory;
        private readonly IWorkPermitApiServiceFactory _workPermitApiServiceFactory;
        private readonly IDocumentTypeReadOnlyService _documentTypeReadOnlyService;
        private readonly IEmployeeTypeReadOnlyService _employeeTypeReadOnlyService;
        private readonly IEducationLevelReadOnlyService _educationLevelReadOnlyService;
        private readonly IAdmissionStatusReadOnlyService _admissionStatusReadOnlyService;
        private readonly IPositionReadOnlyApiServiceFactory _positionReadOnlyApiServiceFactory;
        private readonly IPersonalDocumentApiServiceFactory _personalDocumentApiServiceFactory;
        private readonly IEducationDocumentApiServiceFactory _educationDocumentApiServiceFactory;
        private readonly IEmployeeSpecialtyApiServiceFactory _employeeSpecialtyApiServiceFactory;
        private readonly IContactInformationApiServiceFactory _contactInformationApiServiceFactory;

        public EmployeePageVM
            (
                HttpClient httpClient,

                INavigationPage navigationPage,

                IFileDialogService fileDialogService,
                IFileStorageService fileStorageService,
                IFilePreviewService filePreviewService,
                IGeneratePdfService generatePdfService,
                IDocumentSaveService documentSaveService,
                IDocumentUpdateService documentUpdateService,
                IGenerateImageService generateImageService,
                IImageCompressionService imageCompressionService,
                IDocumentConversionService documentConversionService,
                IDocumentProcessingService documentProcessingService,
                IDocumentDeletionService documentDeletionService,

                IEmployeeService employeeService, 
                IGenderReadOnlyService genderReadOnlyService,
                IStatusReadOnlyService statusReadOnlyService,
                ISpecialtyReadOnlyService specialtyReadOnlyService,
                IPermitTypeReadOnlyService permitTypeReadOnlyService,
                IDepartmentReadOnlyService departmentReadOnlyService,
                IAssignmentApiServiceFactory assignmentApiServiceFactory,
                IWorkPermitApiServiceFactory workPermitApiServiceFactory,
                IDocumentTypeReadOnlyService documentTypeReadOnlyService,
                IEmployeeTypeReadOnlyService employeeTypeReadOnlyService,
                IEducationLevelReadOnlyService educationLevelReadOnlyService,
                IAdmissionStatusReadOnlyService admissionStatusReadOnlyService,
                IPositionReadOnlyApiServiceFactory positionReadOnlyApiServiceFactory,
                IPersonalDocumentApiServiceFactory personalDocumentApiServiceFactory,
                IEducationDocumentApiServiceFactory educationDocumentApiServiceFactory,
                IEmployeeSpecialtyApiServiceFactory employeeSpecialtyApiServiceFactory,
                IContactInformationApiServiceFactory contactInformationApiServiceFactory
            ) 
        {
            _navigationPage = navigationPage;

            _fileDialogService = fileDialogService;
            _fileStorageService = fileStorageService;
            _filePreviewService = filePreviewService;
            _generatePdfService = generatePdfService;
            _documentSaveService = documentSaveService;
            _documentUpdateService = documentUpdateService;
            _generateImageService = generateImageService;
            _imageCompressionService = imageCompressionService;
            _documentConversionService = documentConversionService;
            _documentProcessingService = documentProcessingService;
            _documentDeletionService = documentDeletionService;

            _employeeService = employeeService;
            _genderReadOnlyService = genderReadOnlyService;
            _statusReadOnlyService = statusReadOnlyService;
            _specialtyReadOnlyService = specialtyReadOnlyService;
            _permitTypeReadOnlyService = permitTypeReadOnlyService;
            _departmentReadOnlyService = departmentReadOnlyService;
            _assignmentApiServiceFactory = assignmentApiServiceFactory;
            _workPermitApiServiceFactory = workPermitApiServiceFactory;
            _documentTypeReadOnlyService = documentTypeReadOnlyService;
            _employeeTypeReadOnlyService = employeeTypeReadOnlyService;
            _educationLevelReadOnlyService = educationLevelReadOnlyService;
            _admissionStatusReadOnlyService = admissionStatusReadOnlyService;
            _positionReadOnlyApiServiceFactory = positionReadOnlyApiServiceFactory;
            _personalDocumentApiServiceFactory = personalDocumentApiServiceFactory;
            _educationDocumentApiServiceFactory = educationDocumentApiServiceFactory;
            _employeeSpecialtyApiServiceFactory = employeeSpecialtyApiServiceFactory;
            _contactInformationApiServiceFactory = contactInformationApiServiceFactory;

            PersonalInfo = new();
            PersonalPhoto = new(_fileDialogService, _imageCompressionService);
            ContactInformation = new();
            PersonalDocuments = new(_fileDialogService, _fileStorageService, _filePreviewService, _documentProcessingService, DocumentTypes);
            EducationDocuments = new(_fileDialogService, _fileStorageService, _filePreviewService, _documentProcessingService, DocumentTypes, EducationLevels);
            WorkPermits = new(_fileDialogService, _fileStorageService, _filePreviewService, _documentProcessingService, PermitTypes, AdmissionStatuses);
            Specialties = new();
            AssigmentsContracts = new(_fileDialogService, _documentConversionService, _positionReadOnlyApiServiceFactory, Departments, Managers, Statuses, EmployeeTypes);

            UpdateEmployeePersonalInfoCommand = new RelayCommandAsync(Execute_UpdateEmployeePersonalInfoCommand, CanExecute_UpdateEmployeePersonalInfoCommand);

            UploadPersonalPhoto = new RelayCommandAsync(Execute_UploadPersonalPhoto);
            DeletePersonalPhotoCommand = new RelayCommandAsync(Execute_DeletePersonalPhotoCommand);

            CreatePersonalDocumentCommand = new RelayCommandAsync(Execute_CreatePersonalDocumentCommand, CanExecute_CreatePersonalDocumentCommand);
            UpdatePersonalDocumentCommand = new RelayCommandAsync(Execute_UpdatePersonalDocumentCommand, CanExecute_UpdatePersonalDocumentCommand);
            DeletePersonalDocumentCommand = new RelayCommandAsync<PersonalDocumentDisplay>(Execute_DeletePersonalDocumentCommand, CanExecute_DeletePersonalDocumentCommand);

            UpdateContactInformationCommand = new RelayCommandAsync(Execute_UpdateContactInformationCommand, CanExecute_UpdateContactInformationCommand);

            CreateEducationdocumentCommand = new RelayCommandAsync(Execute_CreateEducationdocumentCommand, CanExecute_CreateEducationdocumentCommand);
            UpdateEducationdocumentCommand = new RelayCommandAsync(Execute_UpdateEducationdocumentCommand, CanExecute_UpdateEducationdocumentCommand);
            DeleteEducationDocumentCommand = new RelayCommandAsync<EducationDocumentDisplay>(Execute_DeleteEducationDocumentCommand, CanExecute_DeleteEducationDocumentCommand);

            CreateWorkPermitCommand = new RelayCommandAsync(Execute_CreateWorkPermitCommand, CanExecute_CreateWorkPermitCommand);
            UpdateWorkPermitCommand = new RelayCommandAsync(Execute_UpdateWorkPermitCommand, CanExecute_UpdateWorkPermitCommand);
            DeleteWorkPermitCommand = new RelayCommandAsync<WorkPermitDisplay>(Execute_DeleteWorkPermitCommand, CanExecute_DeleteWorkPermitCommand);

            CreateEmployeeSpecialtyCommand = new RelayCommandAsync(Execute_CreateEmployeeSpecialtyCommand, CanExecute_CreateEmployeeSpecialtyCommand);
            DeleteEmployeeSpecialtyCommand = new RelayCommandAsync<SpecialtyDisplay>(Execute_DeleteEmployeeSpecialtyCommand, CanExecute_DeleteEmployeeSpecialtyCommand);

            CreateAssignmentContractCommand = new RelayCommandAsync(Execute_CreateAssignmentContractCommand, CanExecute_CreateAssignmentContractCommand);
            UpdateAssignmentContractCommand = new RelayCommandAsync(Execute_UpdateAssignmentContractCommand, CanExecute_UpdateAssignmentContractCommand);
            DeleteAssignmentContractCommand = new RelayCommandAsync<AssignmentContractDisplay>(Execute_DeleteAssignmentContractCommand, CanExecute_DeleteAssignmentContractCommand);

            OpenEditContactInformationEmployeeCommand = new RelayCommand(Execute_OpenEditContactInformationEmployeeCommand, CanExecute_OpenEditContactInformationEmployeeCommand);
            OpenEditPersonalDocumentCommand = new RelayCommand<PersonalDocumentDisplay>(Execute_OpenEditPersonalDocumentCommand, CanExecute_OpenEditPersonalDocumentCommand);
            OpenEditEducationDocumentCommand = new RelayCommand<EducationDocumentDisplay>(Execute_OpenEditEducationDocumentCommand, CanExecute_OpenEditEducationDocumentCommand);
            OpenEditSpecialtyCommand = new RelayCommand<SpecialtyDisplay>(Execute_OpenEditSpecialtyCommand, CanExecute_OpenEditSpecialtyCommand);
            OpenEditWorkPermitCommand = new RelayCommand<WorkPermitDisplay>(Execute_OpenEditWorkPermitCommand, CanExecute_OpenEditWorkPermitCommand);
            OpenEditAssignmentCommand = new RelayCommand<AssignmentContractDisplay>(Execute_OpenEditAssignmentCommand, CanExecute_OpenEditAssignmentCommand);

            CloseModalCommand = new RelayCommand(Execute_CloseModalCommand);

            SoftDeleteEmployeeCommand = new RelayCommandAsync<EmployeeHrDisplay>(Execute_SoftDeleteEmployeeCommand);
        }

        async Task IAsyncInitializable.InitializeAsync()
        {
            if (IsInitialize)
                return;

            IsInitialize = false;

            await GetAllAdmissionStatus();
            await GetAllDepartmentAsync();
            await GetAllEducationLevel();
            await GetAllPositionAsync();
            await GetAllDocumentType();
            await GetAllEmployeeType();
            await GetAllStatusAsync();
            await GetAllPermiteType();
            await GetAllGenderAsync();
            await GetAllSpecialty();
            await GetAllManager();
            await GetAllForHr();

            CreateNewBaseInfoEmployee();

            IsInitialize = true;
        }

        void IUpdatable.Update<TData>(TData value, TransmittingParameter parameter)
        {
            if (parameter is TransmittingParameter.Create)
            {
                if (value is PersonalDocumentDisplay createPersonalDocument)
                {
                    PersonalDocumentsList.Add(createPersonalDocument);
                    ModalVisibility = Visibility.Collapsed;
                }

                if (value is EducationDocumentDisplay createEducationDocument)
                {
                    EducationDocumentsList.Add(createEducationDocument);
                    ModalVisibility = Visibility.Collapsed;
                }

                if (value is SpecialtyDisplay createSpecialty)
                {
                    SpecialtiesCollection.Add(createSpecialty);
                    ModalVisibility = Visibility.Collapsed;
                }

                if (value is WorkPermitDisplay createWorkPermit)
                {
                    WorkPermitsList.Add(createWorkPermit);
                    ModalVisibility = Visibility.Collapsed;
                }

                if (value is AssignmentContractDisplay createAssignmentContrac)
                {
                    AssignmentContractsList.Add(createAssignmentContrac);
                    ModalVisibility = Visibility.Collapsed;
                }
            }

            if (parameter is TransmittingParameter.Update)
            {
                if (value is ValueTuple<EmployeeDetailsDisplay, GenderDisplay, ContactInformationDisplay> employeeUpdateData)
                {
                    CurrentEmployeeDetails = employeeUpdateData.Item1;
                    GenderDisplay = employeeUpdateData.Item2;
                    ContactInformationDisplay = employeeUpdateData.Item3;
                }

                if (value is ValueTuple<SpecialtyDisplay, SpecialtyDisplay> specialtyUpdate)
                {
                    SpecialtiesCollection.Remove(SpecialtiesCollection.FirstOrDefault(x => x.SpecialtyId == specialtyUpdate.Item1.SpecialtyId)!);
                    SpecialtiesCollection.Add(specialtyUpdate.Item2);
                    SpecialtyDisplay = specialtyUpdate.Item2;
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

        private EmployeeHrDisplay _newEmployeeHr = null!;
        public EmployeeHrDisplay NewEmployeeHr
        {
            get => _newEmployeeHr;
            private set => SetProperty(ref _newEmployeeHr, value);
        }
        private void CreateNewBaseInfoEmployee()
        {
            NewEmployeeHr = new(new EmployeeHrItemResponse(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, true, []), Departments, Positions, Statuses);

            //NewEmployeeHr.PropertyChanged += async (s, e) =>
            //{
            //    if (e.PropertyName == nameof(EmployeeHrDisplay.Department))
            //        await GetAllPositionByIdDepartmentAsync();
            //};
        }

        private GenderDisplay _genderDisplay = null!;
        public GenderDisplay GenderDisplay
        {
            get => _genderDisplay;
            set => SetProperty(ref _genderDisplay, value);
        }

        private AssignmentContractDisplay _assignmentContractDisplay = null!;
        public AssignmentContractDisplay AssignmentContractDisplay
        {
            get => _assignmentContractDisplay;
            set => SetProperty(ref _assignmentContractDisplay, value);
        }

        private ContactInformationDisplay _contactInformationDisplay = null!;
        public ContactInformationDisplay ContactInformationDisplay
        {
            get => _contactInformationDisplay;
            set => SetProperty(ref _contactInformationDisplay, value);
        }

        private EducationDocumentDisplay _educationDocumentDisplay = null!;
        public EducationDocumentDisplay EducationDocumentDisplay
        {
            get => _educationDocumentDisplay;
            set => SetProperty(ref _educationDocumentDisplay, value);
        }

        private PersonalDocumentDisplay _personalDocumentDisplay = null!;
        public PersonalDocumentDisplay PersonalDocumentDisplay
        {
            get => _personalDocumentDisplay;
            set => SetProperty(ref _personalDocumentDisplay, value);
        }

        private SpecialtyDisplay _specialtyDisplay = null!;
        public SpecialtyDisplay SpecialtyDisplay
        {
            get => _specialtyDisplay;
            set => SetProperty(ref _specialtyDisplay, value);
        }

        private WorkPermitDisplay _workPermitDisplay = null!;
        public WorkPermitDisplay WorkPermitDisplay
        {
            get => _workPermitDisplay;
            set => SetProperty(ref _workPermitDisplay, value);
        }

        #endregion

        private bool _isAction = true;
        public bool IsAction
        {
            get => _isAction;
            set
            {
                if (value) 
                    ActionProp = TypeAction.Create;
                else
                    ActionProp = TypeAction.Update;

                SetProperty(ref _isAction, value);
            }
        }

        public TypeAction ActionProp
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        #region Feature ViewModel

        private PersonalInfoVM? _personalInfo;
        public PersonalInfoVM? PersonalInfo
        {
            get => _personalInfo;
            set => SetProperty(ref _personalInfo, value);
        }

        private PersonalPhotoVM? _personalPhoto;
        public PersonalPhotoVM? PersonalPhoto
        {
            get => _personalPhoto;
            set => SetProperty(ref _personalPhoto, value);
        }

        private ContactInformationVM? _сontactInformation;
        public ContactInformationVM? ContactInformation
        {
            get => _сontactInformation;
            set => SetProperty(ref _сontactInformation, value);
        }

        private PersonalDocumentsVM? _personalDocuments;
        public PersonalDocumentsVM? PersonalDocuments
        {
            get => _personalDocuments;
            set => SetProperty(ref _personalDocuments, value);
        }

        private EducationDocumentsVM? _educationDocuments;
        public EducationDocumentsVM? EducationDocuments
        {
            get => _educationDocuments;
            set => SetProperty(ref _educationDocuments, value);
        }

        private WorkPermitsVM? _workPermits;
        public WorkPermitsVM? WorkPermits
        {
            get => _workPermits;
            set => SetProperty(ref _workPermits, value);
        }

        private SpecialtiesVM? _specialties;
        public SpecialtiesVM? Specialties
        {
            get => _specialties;
            set => SetProperty(ref _specialties, value);
        }

        private AssigmentsContractsVM? _assigmentsContracts;
        public AssigmentsContractsVM? AssigmentsContracts
        {
            get => _assigmentsContracts;
            set => SetProperty(ref _assigmentsContracts, value);
        }

        #endregion

        #region Установка значений

        private EmployeeHrDisplay _selectedEmployee = null!;
        public EmployeeHrDisplay SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                SetProperty(ref _selectedEmployee, value);

                OpenEditContactInformationEmployeeCommand?.RaiseCanExecuteChanged();
                OpenEditPersonalDocumentCommand?.RaiseCanExecuteChanged();
                OpenEditEducationDocumentCommand?.RaiseCanExecuteChanged();
                OpenEditSpecialtyCommand?.RaiseCanExecuteChanged();
                OpenEditWorkPermitCommand?.RaiseCanExecuteChanged();
                OpenEditAssignmentCommand?.RaiseCanExecuteChanged();

                ClearLocalLists();

                _ = GetEmployeeDetailsAsync(value.Id);
            }
        }

        // Получение деталей сотрудника
        private async Task GetEmployeeDetailsAsync(string id)
        {
            var details = await _employeeService.GetDetailsAsync(id);

            if (details == null)
            {
                MessageBox.Show("Не удалось получить детали пользователя!");
                return;
            }

            CurrentEmployeeDetails = new(details);

            //GenderDisplay = new(new GenderResponse(details.Gender.GenderId.ToString(), details.Gender.GenderName));

            PersonalInfo.EmployeeId = details.EmployeeId.ToString();
            PersonalInfo!.Surname = details.Surname;
            PersonalInfo!.Name = details.Name;
            PersonalInfo!.Patronymic = details.Patronymic;
            PersonalInfo.Gender = new GenderResponse(details.Gender.GenderId.ToString(), details.Gender.GenderName);

            PersonalPhoto.EmployeeId = details.EmployeeId.ToString();
            PersonalPhoto.PhotoUrl = details.PersonalPhoto;
            PersonalPhoto.Photo = await _generateImageService.GenerateAsync(details.PersonalPhoto);

            ContactInformation.EmployeeId = details.EmployeeId.ToString();
            ContactInformation.PersonalPhone = details.ContactInformation.PersonalPhone;
            ContactInformation.CorporatePhone = details.ContactInformation.CorporatePhone;
            ContactInformation.PersonalEmail = details.ContactInformation.PersonalEmail;
            ContactInformation.CorporateEmail = details.ContactInformation.CorporateEmail;
            ContactInformation.PostalCode = details.ContactInformation.PostalCode;
            ContactInformation.Region = details.ContactInformation.Region;
            ContactInformation.City = details.ContactInformation.City;
            ContactInformation.Street = details.ContactInformation.Street;
            ContactInformation.Building = details.ContactInformation.Building;
            ContactInformation.Apartment = details.ContactInformation.Apartment;

            ContactInformationDisplay = new
                (
                    new ContactInformationResponse
                        (
                            details.ContactInformation!.ContactInformationId!,
                            details.ContactInformation.PersonalPhone!,
                            details.ContactInformation.CorporatePhone,
                            details.ContactInformation.PersonalEmail!,
                            details.ContactInformation.CorporateEmail!,
                            details.ContactInformation.PostalCode!,
                            details.ContactInformation.Region!,
                            details.ContactInformation.City!,
                            details.ContactInformation.Street!,
                            details.ContactInformation.Building!,
                            details.ContactInformation.Apartment
                        )
                );

            PersonalDocuments.EmployeeId = details.EmployeeId.ToString();
            PersonalDocuments!.LocalPersonalDocuments.Load
                (
                    details.PersonalDocuments?.Select
                        (
                            x => new PersonalDocumentDisplay
                                (
                                    new PersonalDocumentResponse
                                        (
                                            x.PersonalDocumentId,
                                            x.PersonalDocumentTypeId,
                                            x.PersonalDocumentNumber,
                                            x.PersonalDocumentSeries,
                                            x.PersonalDocumentFileKey
                                        ), DocumentTypes, SaveStatus.DataBase
                                )
                        ).ToList()
                );

            EducationDocuments.EmployeeId = details.EmployeeId.ToString();
            EducationDocuments!.LocalEducationDocuments.Load
                (
                    details.EducationDocuments?.Select
                        (
                            x => new EducationDocumentDisplay
                                (
                                    new EducationDocumentResponse
                                        (
                                            x.EducationDocumentId.ToString(),
                                            details.EmployeeId.ToString(),
                                            x.EducationLevelId.ToString(),
                                            x.EducationDocumentTypeId.ToString(),
                                            x.EducationDocumentNumber,
                                            x.EducationIssuedDate.ToString(),
                                            x.OrganizationName,
                                            x.QualificationAwardedName,
                                            x.EducationSpecialtyName,
                                            x.ProgramName,
                                            x.TotalHours.ToString(),
                                            x.EducationDocumentFileKey
                                        ), EducationLevels, DocumentTypes, SaveStatus.DataBase
                                )
                        ).ToList()
                );

            WorkPermits.EmployeeId = details.EmployeeId.ToString();
            WorkPermits!.LocalWorkPermits.Load
                (
                    details.WorkPermits?.Select
                        (
                            x => new WorkPermitDisplay
                                (
                                    new WorkPermitResponse
                                        (
                                            x.WorkPermitId.ToString(),
                                            details.EmployeeId.ToString(),
                                            x.WorkPermitName,
                                            x.WorkPermitDocumentSeries,
                                            x.WorkPermitNumber,
                                            x.ProtocolNumber,
                                            x.WorkPermitSpecialtyName,
                                            x.IssuingAuthority,
                                            x.WorkPermitIssueDate,
                                            x.WorkPermitExpiryDate,
                                            x.WorkPermitFileKey,
                                            x.PermitTypeId.ToString(),
                                            x.AdmissionStatusId.ToString()
                                        ),
                                    PermitTypes, AdmissionStatuses, SaveStatus.DataBase
                                )
                        ).ToList()
                );

            Specialties.EmployeeId = details.EmployeeId.ToString();
            Specialties!.LocalEmployeeSpecialties.Load
                (
                    details.Specialties?.Select
                        (
                            x => new SpecialtyDisplay
                                (
                                    new SpecialtyResponse
                                        (
                                            x.SpecialtyId.ToString(),
                                            x.SpecialtyName,
                                            x.SpecialtyDescription
                                        )
                                )
                        ).ToList()
                );

            AssigmentsContracts.EmployeeId = details.EmployeeId.ToString();
            foreach (var item in details.Assignments!)
            {
                var contractsResponse = details.Contracts?.FirstOrDefault(x => x.ContractId == item.ContractId);

                var display = new AssignmentContractDisplay
                    (
                        new AssignmentResponse
                            (
                                item.AssignmentId.ToString(),
                                details.EmployeeId.ToString(),
                                item.PositionId.ToString(),
                                item.DepartmentId.ToString(),
                                item.ManagerId.ToString(),
                                item.HireDate,
                                item.TerminationDate,
                                item.StatusId.ToString()
                            ),
                        new ContractResponse
                            (
                                details.EmployeeId.ToString(),
                                contractsResponse!.ContractId.ToString(),
                                contractsResponse.ContractNumber,
                                contractsResponse.EmployeeTypeId.ToString(),
                                contractsResponse.ContractStartDate,
                                contractsResponse.ContractEndDate,
                                contractsResponse.Salary,
                                contractsResponse?.ContractFileKey
                            ),
                        Departments, Positions, Managers, Statuses, EmployeeTypes,
                        string.Empty, SaveStatus.DataBase
                    );

                AssigmentsContracts.LocalAssignmentsContracts.Add(display);
                //AssignmentContractsList.Add(display);
            }

            #region Пока не надо

            //AssignmentContracts.Load
            //    (
            //        details.Assignments?.Join
            //            (
            //                details.Contracts ?? Enumerable.Empty<ContractDetailsResponseData>(),
            //                assignment => assignment.ContractId,
            //                contract => contract.ContractId,
            //                (assignment, contract) => new AssignmentContractDisplay
            //                    (
            //                        new AssignmentResponse
            //                            (
            //                                assignment.AssignmentId,
            //                                details.EmployeeId,
            //                                assignment.PositionId,
            //                                assignment.DepartmentId,
            //                                assignment.ManagerId,
            //                                assignment.HireDate,
            //                                assignment.TerminationDate,
            //                                assignment.StatusId
            //                            ),
            //                        new ContractResponse
            //                            (
            //                                details.EmployeeId.ToString(),
            //                                contract.ContractId.ToString(),
            //                                contract.ContractNumber,
            //                                contract.EmployeeType,
            //                                contract.ContractStartDate,
            //                                contract.ContractEndDate,
            //                                contract.Salary,
            //                                contract.ContractFileKey
            //                            ),
            //                        Departments, Positions, Managers, Statuses, EmployeeTypes
            //                    )
            //            ).ToList()
            //    );

            #endregion
        }

        #endregion

        #region Получение данных

        public ObservableCollection<EmployeeHrDisplay> EmployeeHrs { get; private init; } = [];
        private async Task GetAllForHr()
        {
            var response = await _employeeService.GetAllForHrAsync();

            foreach (var item in response)
            {
                var display = new EmployeeHrDisplay(item, Departments, Positions, Statuses);

                if (item.Assignments.Count > 0)
                {
                    display.SetDepartment(item.Assignments.FirstOrDefault()!.DepartmentId);
                    display.SetPosition(item.Assignments.FirstOrDefault()!.PositionId);
                    display.SetStatus(item.Assignments.FirstOrDefault()!.StatusId);
                }

                display.SetImage(await _generateImageService.GenerateAsync(item.PersonalPhoto));

                EmployeeHrs.Add(display);
            }
        }

        public ObservableCollection<DepartmentDisplay> Departments { get; private init; } = [];
        private async Task GetAllDepartmentAsync()
        {
            var departments = await _departmentReadOnlyService.GetAllAsync();

            Departments.Load([.. departments.Select(department => new DepartmentDisplay(department))]);
        }

        public ObservableCollection<PositionDisplay> Positions { get; private init; } = [];
        private async Task GetAllPositionAsync()
        {
            var positions = await _positionReadOnlyApiServiceFactory.Create(Guid.NewGuid().ToString()).GetAllPosition();

            Positions.Load([.. positions.Select(position => new PositionDisplay(position))]);
        }

        public ObservableCollection<StatusDisplay> Statuses { get; private init; } = [];
        private async Task GetAllStatusAsync()
        {
            var statuses = await _statusReadOnlyService.GetAllAsync();

            Statuses.Load([.. statuses.Select(status => new StatusDisplay(status))]);
        }

        public ObservableCollection<SpecialtyDisplay> SpecialtiesCollection { get; private init; } = [];
        private async Task GetAllSpecialty()
        {
            var specialties = await _specialtyReadOnlyService.GetAllAsync();

            SpecialtiesCollection.Load([.. specialties.Select(specialty => new SpecialtyDisplay(specialty))]);
        }

        public ObservableCollection<ManagerDisplay> Managers { get; private init; } = [];
        private async Task GetAllManager()
        {
            var managers = await _employeeService.GetAllAsync();

            Managers.Load([.. managers.Select(manager => new ManagerDisplay(manager))]);
        }

        public ObservableCollection<EmployeeTypeDisplay> EmployeeTypes { get; private init; } = [];
        private async Task GetAllEmployeeType()
        {
            var employeeTypes = await _employeeTypeReadOnlyService.GetAllAsync();

            EmployeeTypes.Load([.. employeeTypes.Select(employeeType => new EmployeeTypeDisplay(employeeType))]);
        }

        public ObservableCollection<EducationLevelDisplay> EducationLevels { get; private init; } = [];
        private async Task GetAllEducationLevel()
        {
            var educationLevels = await _educationLevelReadOnlyService.GetAllAsync();

            EducationLevels.Load([.. educationLevels.Select(educationLevel => new EducationLevelDisplay(educationLevel))]);
        }

        public ObservableCollection<DocumentTypeDisplay> DocumentTypes { get; private init; } = [];
        private async Task GetAllDocumentType()
        {
            var documentTypes = await _documentTypeReadOnlyService.GetAllAsync();

            DocumentTypes.Load([.. documentTypes.Select(documentType => new DocumentTypeDisplay(documentType))]);
        }

        public ObservableCollection<PermitTypeDisplay> PermitTypes { get; private init; } = [];
        private async Task GetAllPermiteType()
        {
            var permitTypes = await _permitTypeReadOnlyService.GetAllAsync();

            PermitTypes.Load([.. permitTypes.Select(permiteType => new PermitTypeDisplay(permiteType))]);
        }

        public ObservableCollection<AdmissionStatusDisplay> AdmissionStatuses { get; private init; } = [];
        private async Task GetAllAdmissionStatus()
        {
            var admissionStatuses = await _admissionStatusReadOnlyService.GetAllAsync();

            AdmissionStatuses.Load([.. admissionStatuses.Select(admissionStatus => new AdmissionStatusDisplay(admissionStatus))]);
        }

        public ObservableCollection<GenderResponse> Genders { get; private init; } = [];
        private async Task GetAllGenderAsync() => Genders.Load(await _genderReadOnlyService.GetAllAsync());

        public ObservableCollection<PersonalDocumentDisplay> PersonalDocumentsList { get; private init; } = [];
        public ObservableCollection<EducationDocumentDisplay> EducationDocumentsList { get; private init; } = [];
        public ObservableCollection<WorkPermitDisplay> WorkPermitsList { get; private init; } = [];
        public ObservableCollection<AssignmentContractDisplay> AssignmentContractsList { get; private init; } = [];

        private void ClearLocalLists()
        {
            PersonalInfo!.ClearProperty();
            PersonalPhoto!.ClearProperty();
            ContactInformation!.ClearProperty();

            PersonalDocuments!.ClearProperty();
            PersonalDocuments!.LocalPersonalDocuments.Clear();

            EducationDocuments!.ClearProperty();
            EducationDocuments!.LocalEducationDocuments.Clear();

            WorkPermits!.ClearProperty();
            WorkPermits!.LocalWorkPermits.Clear();

            Specialties!.ClearProperty();
            Specialties!.LocalEmployeeSpecialties.Clear();

            AssigmentsContracts!.ClearProperty();
            AssigmentsContracts!.LocalAssignmentsContracts.Clear();
        }

        #endregion

        #region Модальное окно редактирования

        // Видимость модального окна
        private Visibility _modalVisibility = Visibility.Collapsed;
        public Visibility ModalVisibility
        {
            get => _modalVisibility;
            set => SetProperty(ref _modalVisibility, value);
        }

        // Открытие модального окна редактирования контактной информации
        public RelayCommand OpenEditContactInformationEmployeeCommand { get; private set; }
        private void Execute_OpenEditContactInformationEmployeeCommand()
        {
            if (ModalVisibility == Visibility.Collapsed)
            {
                ModalVisibility = Visibility.Visible;

                _navigationPage.NavigateTo(FrameName.ModalFrame, PageName.EditContactInformationEmployeePage);
                _navigationPage.TransmittingValue
                    (
                        (
                            CurrentEmployeeDetails, 
                            GenderDisplay, 
                            ContactInformationDisplay
                        ), 
                        FrameName.ModalFrame, 
                        PageName.EditContactInformationEmployeePage, 
                        TransmittingParameter.None
                    );
            }
        }
        private bool CanExecute_OpenEditContactInformationEmployeeCommand() => SelectedEmployee != null;

        // Открытие модального окна редактирования персонального документа
        public RelayCommand<PersonalDocumentDisplay> OpenEditPersonalDocumentCommand { get; private set; }
        private void Execute_OpenEditPersonalDocumentCommand(PersonalDocumentDisplay display)
        {
            if (ModalVisibility == Visibility.Collapsed)
            {
                ModalVisibility = Visibility.Visible;

                _navigationPage.NavigateTo(FrameName.ModalFrame, PageName.EditPersonalDocumentEmployeePage);
                _navigationPage.TransmittingValue
                    (
                        (
                            CurrentEmployeeDetails,
                            display
                        ),
                        FrameName.ModalFrame,
                        PageName.EditPersonalDocumentEmployeePage,
                        TransmittingParameter.None
                    );
            }
        }
        private bool CanExecute_OpenEditPersonalDocumentCommand(PersonalDocumentDisplay display) => SelectedEmployee != null;

        // Открытие модального окна редактирования образовательного документа
        public RelayCommand<EducationDocumentDisplay> OpenEditEducationDocumentCommand { get; private set; }
        private void Execute_OpenEditEducationDocumentCommand(EducationDocumentDisplay display)
        {
            if (ModalVisibility == Visibility.Collapsed)
            {
                ModalVisibility = Visibility.Visible;

                _navigationPage.NavigateTo(FrameName.ModalFrame, PageName.EditEducationDocumentEmployeePage);
                _navigationPage.TransmittingValue
                    (
                        (
                            CurrentEmployeeDetails,
                            display
                        ),
                        FrameName.ModalFrame,
                        PageName.EditEducationDocumentEmployeePage,
                        TransmittingParameter.None
                    );
            }
        }
        private bool CanExecute_OpenEditEducationDocumentCommand(EducationDocumentDisplay display) => SelectedEmployee != null;

        // Открытие модального окна редактирования специальности
        public RelayCommand<SpecialtyDisplay> OpenEditSpecialtyCommand { get; private set; }
        private void Execute_OpenEditSpecialtyCommand(SpecialtyDisplay display)
        {
            if (ModalVisibility == Visibility.Collapsed)
            {
                ModalVisibility = Visibility.Visible;

                _navigationPage.NavigateTo(FrameName.ModalFrame, PageName.EditSpecialtyEmployeePage);
                _navigationPage.TransmittingValue
                    (
                        (
                            CurrentEmployeeDetails,
                            display
                        ),
                        FrameName.ModalFrame,
                        PageName.EditSpecialtyEmployeePage,
                        TransmittingParameter.None
                    );
            }
        }
        private bool CanExecute_OpenEditSpecialtyCommand(SpecialtyDisplay display) => SelectedEmployee != null;

        // Открытие модального окна редактирования рабочего разрешения
        public RelayCommand<WorkPermitDisplay> OpenEditWorkPermitCommand { get; private set; }
        private void Execute_OpenEditWorkPermitCommand(WorkPermitDisplay display)
        {
            if (ModalVisibility == Visibility.Collapsed)
            {
                ModalVisibility = Visibility.Visible;

                _navigationPage.NavigateTo(FrameName.ModalFrame, PageName.EditWorkPermitEmployeePage);
                _navigationPage.TransmittingValue
                    (
                        (
                            CurrentEmployeeDetails,
                            display
                        ),
                        FrameName.ModalFrame,
                        PageName.EditWorkPermitEmployeePage,
                        TransmittingParameter.None
                    );
            }
        }
        private bool CanExecute_OpenEditWorkPermitCommand(WorkPermitDisplay display) => SelectedEmployee != null;

        // Открытие модального окна редактирования разначения
        public RelayCommand<AssignmentContractDisplay> OpenEditAssignmentCommand { get; private set; }
        private void Execute_OpenEditAssignmentCommand(AssignmentContractDisplay display)
        {
            if (ModalVisibility == Visibility.Collapsed)
            {
                ModalVisibility = Visibility.Visible;

                _navigationPage.NavigateTo(FrameName.ModalFrame, PageName.EditAssignmentEmployeePage);
                _navigationPage.TransmittingValue
                    (
                        (
                            CurrentEmployeeDetails,
                            display
                        ),
                        FrameName.ModalFrame,
                        PageName.EditAssignmentEmployeePage,
                        TransmittingParameter.None
                    );
            }
        }
        private bool CanExecute_OpenEditAssignmentCommand(AssignmentContractDisplay display) => SelectedEmployee != null;

        // Закрытие модального окна
        public RelayCommand CloseModalCommand { get; private set; }
        private void Execute_CloseModalCommand()
        {
            if (ModalVisibility == Visibility.Visible)
                ModalVisibility = Visibility.Collapsed;
        }

        #endregion

        #region EditPersonalInfo

        // UPDATE
        public RelayCommandAsync UpdateEmployeePersonalInfoCommand { get; private set; }
        private async Task Execute_UpdateEmployeePersonalInfoCommand()
        {
            if (PersonalInfo == null)
            {
                MessageBox.Show("Данные не заполнены!");
                return;
            }

            var dbResult = await _employeeService.UpdateEmployeeAsync
                (
                    PersonalInfo.EmployeeId!,
                    new UpdateEmployeeRequest
                        (
                            PersonalInfo.Surname!, 
                            PersonalInfo.Name!, 
                            PersonalInfo.Patronymic, 
                            PersonalInfo.Gender!.Id
                        )
                );

            if (dbResult.IsFailure)
            {
                MessageBox.Show($"{dbResult.Errors}");
                return;
            }
        }
        private bool CanExecute_UpdateEmployeePersonalInfoCommand() => true;

        #endregion

        #region EditPersonalPhoto

        // UPLOAD
        public RelayCommandAsync UploadPersonalPhoto { get; private set; }
        private async Task Execute_UploadPersonalPhoto()
        {
            var avatarBytes = PersonalPhoto!.GetCompressedBytes();
            var fileName = PersonalPhoto.GetFileName();

            if (avatarBytes == null || string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Аватарка не нвыбрана!");
                return;
            }

            var fileResult = await _fileStorageService.UploadFileAsync
                (
                    new UploadFileRequest
                        (
                            FileConst.BUCKET_NAME,
                            nameof(PersonalPhoto),
                            FileConst.BuildEmployeeFolder
                                (
                                    PersonalPhoto.EmployeeId!,
                                    EmployeeFolderType.PersonalPhoto
                                ),
                            //FilePath: null,
                            FileBytes: avatarBytes,
                            FileName: fileName
                        )
                );

            if (fileResult.IsFailure)
            {
                MessageBox.Show(fileResult.StringMessage);
                return;
            }

            var dbResult = await _employeeService.AddPersonalPhoto
                (
                    PersonalPhoto.EmployeeId!,
                    new AddPersonalPhotoRequest
                        (
                            FileConst.BUCKET_NAME, 
                            fileResult.Value!.FileName
                        )
                );

            if (dbResult.IsFailure)
            {
                MessageBox.Show(dbResult.StringMessage);
                return;
            }

            EmployeeHrs.FirstOrDefault(x => x.Id == PersonalPhoto.EmployeeId)!.PersonalPhoto = PersonalPhoto.Photo;
        }

        // DELETE
        public RelayCommandAsync DeletePersonalPhotoCommand { get; private set; }
        private async Task Execute_DeletePersonalPhotoCommand()
        {
            var dbResult = await _employeeService.DeletePersonalPhoto(PersonalPhoto!.EmployeeId!);

            if (dbResult.IsFailure)
            {
                MessageBox.Show($"Ошибка удаления файла из базы данных!\n{dbResult.StringMessage}");
                return;
            }

            if (string.IsNullOrWhiteSpace(PersonalPhoto.PhotoUrl))
                return;

            var (BucketName, FileName) = S3UrlParser.Parse(PersonalPhoto.PhotoUrl);

            var fileResult = await _fileStorageService.DeleteFileAsync(new DeleteFileRequest(BucketName!, FileName!));

            if (fileResult.IsFailure)
            {
                MessageBox.Show($"Ошибка удаления файла из S3 хранилища!\n{fileResult.StringMessage}");
                return;
            }

            PersonalPhoto.ClearProperty();

            var employee = EmployeeHrs.FirstOrDefault(x => x.Id == PersonalPhoto.EmployeeId)!;

            employee.PersonalPhoto = null;
            employee.PersonalPhotoUrlDB = null;
        }

        #endregion

        #region EditContactInformation

        // UPDATE
        public RelayCommandAsync UpdateContactInformationCommand { get; private set; }
        private async Task Execute_UpdateContactInformationCommand()
        {
            if (ContactInformation == null)
            {
                MessageBox.Show("Данные не заполнены!");
                return;
            }


            var dbResult = await _contactInformationApiServiceFactory.Create(ContactInformation.EmployeeId.ToString())
                .UpdateContactInformationAsync
                    (
                        new UpdateContactInformationRequest
                            (
                                ContactInformationDisplay.ContactInformationId,
                                ContactInformation.EmployeeId,
                                ContactInformation.PersonalPhone,
                                ContactInformation.CorporatePhone,
                                ContactInformation.PersonalEmail,
                                ContactInformation.CorporateEmail,
                                ContactInformation.PostalCode,
                                ContactInformation.Region,
                                ContactInformation.City,
                                ContactInformation.Street,
                                ContactInformation.Building,
                                ContactInformation.Apartment
                            )
                    );

            if (dbResult.IsFailure)
            {
                MessageBox.Show($"{dbResult.Errors}");
                return;
            }
        }
        private bool CanExecute_UpdateContactInformationCommand() => true;

        #endregion

        #region EditPersonalDocument

        // CREATE
        public RelayCommandAsync CreatePersonalDocumentCommand { get; private set; }
        private async Task Execute_CreatePersonalDocumentCommand()
        {
            if (PersonalDocuments?.LocalPersonalDocuments == null)
            {
                MessageBox.Show("Данные отсутствуют!");
                return;
            }

            var documentsToSave = PersonalDocuments.LocalPersonalDocuments
                .Where(x => x.SaveStatus == SaveStatus.Local).ToList();

            if (documentsToSave.Count <= 0)
            {
                MessageBox.Show("Документы для сохранения отсутствуют!");
                return;
            }

            var result = await _documentSaveService.SaveLocalDocumentsAsync
            (
                documentsToSave: documentsToSave,

                uploadRequest: doc => new UploadFilesDataRequest
                (
                    BucketName: FileConst.BUCKET_NAME,
                    AdditionalName: doc.DocumentType.Name,
                    SubFolder: FileConst.BuildEmployeeFolder
                    (
                        PersonalDocuments.EmployeeId!,
                        EmployeeFolderType.PersonalDocument
                    ),
                    FileBytes: doc.FileBytes,
                    FileName: doc.FileName,
                    ContentType: doc.ContentType ?? "application/pdf"
                ),

                dbRequest: (doc, s3FileName) => new CreateDataPersonalDocumentRequest
                (
                    doc.DocumentTypeId.ToString(),
                    doc.DocumentNumber,
                    doc.DocumentSeries,
                    FileConst.BUCKET_NAME,
                    s3FileName
                ),

                dbSaveAsync: async requests => await _personalDocumentApiServiceFactory.Create(PersonalDocuments.EmployeeId!)
                    .CreateManyAsync(new CreateManyPersonalDocumentsRequest([.. requests])),

                markAsSavedAction: doc =>
                {
                    doc.SetSaveStatus(SaveStatus.DataBase);
                    PersonalDocuments.PersonalDocumentsView.Refresh();
                    PersonalDocuments.ClearProperty();
                }
            );

            if (result.IsFailure)
            {
                MessageBox.Show(result.StringMessage);
                return;
            }
        }
        private bool CanExecute_CreatePersonalDocumentCommand() => true;

        // UPDATE
        public RelayCommandAsync UpdatePersonalDocumentCommand { get; private set; }
        private async Task Execute_UpdatePersonalDocumentCommand()
        {
            if (!PersonalDocuments.PendingFilePaths.Any())
            {
                MessageBox.Show("Выберите хотя бы один файл для добавления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedDoc = PersonalDocuments.SelectedLocalPersonalDocument;

            if (selectedDoc == null)
                return;

            var processResult = await _documentProcessingService.ProcessFilesToPdfAsync
            (
                PersonalDocuments.PendingFilePaths,
                PersonalDocuments.DocumentType.Name,
                PersonalDocuments.EmployeeId!,
                PersonalDocuments.Number
            );

            if (processResult.IsFailure)
            {
                MessageBox.Show(processResult.StringMessage);
                return;
            }

            var (pdfBytes, fileName) = processResult.Value;

            var result = await _documentUpdateService.UpdateDocumentFileAsync
            (
                newFileBytes: pdfBytes,
                newFileName: fileName,
                oldFileKey: selectedDoc.FileKey,

                uploadRequest: (bytes, fName) => new UploadFileRequest
                (
                    FileConst.BUCKET_NAME,
                    PersonalDocuments.DocumentType.Name,
                    FileConst.BuildEmployeeFolder
                    (
                        PersonalDocuments.EmployeeId!,
                        EmployeeFolderType.PersonalDocument
                    ),
                    FileBytes: bytes,
                    FileName: fName
                ),

                dbRequest: s3FileName => new UpdatePersonalDocumentRequest
                (
                    PersonalDocuments.DocumentType.Id,
                    PersonalDocuments.Number,
                    PersonalDocuments.Series,
                    FileConst.BUCKET_NAME,
                    s3FileName
                ),

                dbUpdateAsync: async request => await _personalDocumentApiServiceFactory.Create(PersonalDocuments.EmployeeId!)
                    .UpdatePersonalDocumentAsync(selectedDoc.PersonalDocumentId, request)
            );

            if (result.IsFailure)
            {
                MessageBox.Show(result.StringMessage);
                return;
            }

            PersonalDocuments.ClearProperty();
        }
        private bool CanExecute_UpdatePersonalDocumentCommand() => true;
            //=> PersonalDocuments!.SelectedLocalPersonalDocument != null && PersonalDocuments!.DocumentType != null && !string.IsNullOrWhiteSpace(PersonalDocuments!.Number);

        // DELETE
        public RelayCommandAsync<PersonalDocumentDisplay> DeletePersonalDocumentCommand { get; private set; }
        private async Task Execute_DeletePersonalDocumentCommand(PersonalDocumentDisplay display)
        {
            if (display == null)
                return;

            var result = await _documentDeletionService.DeleteDocumentAsync
            (
                document: display,
                collection: PersonalDocuments!.LocalPersonalDocuments,

                getId: doc => doc.PersonalDocumentId,
                getFileKey: doc => doc.FileKey,
                dbDeleteAsync: async id => await _personalDocumentApiServiceFactory.Create(PersonalDocuments.EmployeeId!)
                    .DeletePersonalDocumentAsync(id),
                refreshView: () =>
                {
                    PersonalDocuments.PersonalDocumentsView.Refresh();
                    PersonalDocuments.ClearProperty();
                }
            );

            if (result.IsFailure)
            {
                MessageBox.Show(result.StringMessage);
                return;
            }
        }
        private bool CanExecute_DeletePersonalDocumentCommand(PersonalDocumentDisplay display) => SelectedEmployee != null;
        #endregion

        #region EditEducationDocument

        // CREATE
        public RelayCommandAsync CreateEducationdocumentCommand { get; private set; }
        private async Task Execute_CreateEducationdocumentCommand()
        {
            if (EducationDocuments == null)
            {
                MessageBox.Show("Данные отсутствуют!");
                return;
            }

            var documentsToSave = EducationDocuments.LocalEducationDocuments
                .Where(x => x.SaveStatus == SaveStatus.Local).ToList();

            if (documentsToSave.Count <= 0)
                return;

            var result = await _documentSaveService.SaveLocalDocumentsAsync
            (
                documentsToSave: documentsToSave,

                uploadRequest: doc => new UploadFilesDataRequest
                (
                    BucketName: FileConst.BUCKET_NAME,
                    AdditionalName: doc.DocumentType.Name,
                    SubFolder: FileConst.BuildEmployeeFolder
                    (
                        EducationDocuments.EmployeeId!,
                        EmployeeFolderType.EducationDocument
                    ),
                    FileBytes: doc.FileBytes,
                    FileName: doc.FileName,
                    ContentType: doc.ContentType ?? "application/pdf"
                ),

                dbRequest: (doc, s3FileName) => new CreateDataEducationDocumentReqeust
                (
                    doc.EducationLevel.Id,
                    doc.DocumentType.Id,
                    doc.DocumentNumber,
                    doc.IssuedDate.ToString(),
                    doc.OrganizationName,
                    doc.QualificationAwardedName,
                    doc.SpecialtyName,
                    doc.ProgramName,
                    doc.TotalHours,
                    FileConst.BUCKET_NAME,
                    s3FileName
                ),

                dbSaveAsync: async requests => await _educationDocumentApiServiceFactory.Create(EducationDocuments.EmployeeId!)
                    .CreateManyAsync(new CreateManyEducationDocumentsReqeust([.. requests])),

                markAsSavedAction: doc =>
                {
                    doc.SetSaveStatus(SaveStatus.DataBase);
                    EducationDocuments.EducationDocumentsView.Refresh();
                    EducationDocuments.ClearProperty();
                }
            );

            if (result.IsFailure)
            {
                MessageBox.Show($"{result.StringMessage}");
                return;
            }
        }
        private bool CanExecute_CreateEducationdocumentCommand() => true;

        // UPDATE
        public RelayCommandAsync UpdateEducationdocumentCommand { get; private set; }
        private async Task Execute_UpdateEducationdocumentCommand()
        {
            if (!EducationDocuments.PendingFilePaths.Any())
            {
                MessageBox.Show("Выберите хотя бы один файл для добавления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedDoc = EducationDocuments.SelectedLocalEducationDocument;

            if (selectedDoc == null)
                return;

            var processResult = await _documentProcessingService.ProcessFilesToPdfAsync
                (
                    EducationDocuments.PendingFilePaths,
                    EducationDocuments.DocumentType.Name,
                    EducationDocuments.EmployeeId!,
                    EducationDocuments.DocumentNumber
                );

            if (processResult.IsFailure)
            {
                MessageBox.Show(processResult.StringMessage);
                return;
            }

            var (pdfBytes, fileName) = processResult.Value;

            var result = await _documentUpdateService.UpdateDocumentFileAsync
                (
                    newFileBytes: pdfBytes,
                    newFileName: fileName,
                    oldFileKey: selectedDoc.FileKey,

                    uploadRequest: (bytes, fName) => new UploadFileRequest
                        (
                            FileConst.BUCKET_NAME,
                            EducationDocuments.DocumentType.Name,
                            FileConst.BuildEmployeeFolder
                                (
                                    EducationDocuments.EmployeeId!,
                                    EmployeeFolderType.EducationDocument
                                ),
                            FileBytes: bytes,
                            FileName: fName
                        ),

                    dbRequest: s3FileName => new UpdateEducationDocumentRequest
                        (
                            EducationDocuments.EducationLevel!.Id,
                            EducationDocuments.DocumentType!.Id,
                            EducationDocuments.DocumentNumber,
                            EducationDocuments.IssuedDate,
                            EducationDocuments.OrganizationName,
                            EducationDocuments.QualificationAwardedName,
                            EducationDocuments.SpecialtyName,
                            EducationDocuments.ProgramName,
                            EducationDocuments.TotalHours,
                            FileConst.BUCKET_NAME,
                            s3FileName
                        ),

                    dbUpdateAsync: async request => await _educationDocumentApiServiceFactory.Create(EducationDocuments.EmployeeId!)
                        .UpdateEducationDocumentAsync(Guid.Parse(selectedDoc.EducationDocumentId), request)
                );

            if (result.IsFailure)
            {
                MessageBox.Show(result.StringMessage);
                return;
            }

            EducationDocuments.ClearProperty();
        }
        private bool CanExecute_UpdateEducationdocumentCommand() => true;

        // DELETE
        public RelayCommandAsync<EducationDocumentDisplay> DeleteEducationDocumentCommand { get; private set; }
        private async Task Execute_DeleteEducationDocumentCommand(EducationDocumentDisplay display)
        {
            if (display == null)
                return;

            var result = await _documentDeletionService.DeleteDocumentAsync
            (
                document: display,
                collection: EducationDocuments!.LocalEducationDocuments,
                getId: doc => Guid.Parse(doc.EducationDocumentId),
                getFileKey: doc => doc.FileKey,
                dbDeleteAsync: async id => await _educationDocumentApiServiceFactory.Create(EducationDocuments.EmployeeId!)
                    .DeleteEducationDocumentAsync(id),
                refreshView: () =>
                {
                    EducationDocuments.EducationDocumentsView.Refresh();
                    EducationDocuments.ClearProperty();
                }
            );

            if (result.IsFailure)
            {
                MessageBox.Show(result.StringMessage);
                return;
            }
        }
        private bool CanExecute_DeleteEducationDocumentCommand(EducationDocumentDisplay display) => SelectedEmployee != null;
        
        #endregion

        #region EditWorkPermit

        // CREATE
        public RelayCommandAsync CreateWorkPermitCommand { get; private set; }
        private async Task Execute_CreateWorkPermitCommand()
        {
            if (WorkPermits == null)
            {
                MessageBox.Show("Данные отсутствуют!");
                return;
            }

            var documentsToSave = WorkPermits.LocalWorkPermits
                .Where(x => x.SaveStatus == SaveStatus.Local).ToList();

            if (documentsToSave.Count <= 0)
                return;

            var result = await _documentSaveService.SaveLocalDocumentsAsync
                (
                    documentsToSave: documentsToSave,

                    uploadRequest: doc => new UploadFilesDataRequest
                    (
                        BucketName: FileConst.BUCKET_NAME,
                        AdditionalName: doc.PermitType.Name,
                        SubFolder: FileConst.BuildEmployeeFolder
                        (
                            WorkPermits.EmployeeId!,
                            EmployeeFolderType.WorkPermit
                        ),
                        FileBytes: doc.FileBytes,
                        FileName: doc.FileName,
                        ContentType: doc.ContentType ?? "application/pdf"
                    ),

                    dbRequest: (doc, s3FileName) => new CreateManyDataWorkPermitsRequest
                    (
                        doc.WorkPermitName,
                        doc.DocumentSeries,
                        doc.WorkPermitNumber,
                        doc.ProtocolNumber,
                        doc.SpecialtyName,
                        doc.IssuingAuthority,
                        doc.IssueDate.ToString(),
                        doc.ExpiryDate.ToString(),
                        FileConst.BUCKET_NAME,
                        s3FileName,
                        doc.PermitType.Id,
                        doc.AdmissionStatus.Id
                    ),

                    dbSaveAsync: async requests => await _workPermitApiServiceFactory.Create(WorkPermits.EmployeeId!)
                        .CreateManyAsync(new CreateManyWorkPermitsRequest([.. requests])),

                    markAsSavedAction: doc =>
                    {
                        doc.SetSaveStatus(SaveStatus.DataBase);
                        WorkPermits.WorkPermitsView.Refresh();
                        WorkPermits.ClearProperty();
                    }
                );

            if (result.IsFailure)
            {
                MessageBox.Show($"{result.StringMessage}");
                return;
            }
        }
        private bool CanExecute_CreateWorkPermitCommand() => true;

        // UPDATE
        public RelayCommandAsync UpdateWorkPermitCommand { get; private set; }
        private async Task Execute_UpdateWorkPermitCommand()
        {
            if (!WorkPermits.PendingFilePaths.Any())
            {
                MessageBox.Show("Выберите хотя бы один файл для добавления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedDoc = WorkPermits.SelectedLocalWorkPermit;

            if (selectedDoc == null)
                return;

            var processResult = await _documentProcessingService.ProcessFilesToPdfAsync
                (
                    WorkPermits.PendingFilePaths,
                    WorkPermits.PermitType.Name,
                    WorkPermits.EmployeeId!,
                    WorkPermits.WorkPermitNumber
                );

            if (processResult.IsFailure)
            {
                MessageBox.Show(processResult.StringMessage);
                return;
            }

            var (pdfBytes, fileName) = processResult.Value;

            var result = await _documentUpdateService.UpdateDocumentFileAsync
                (
                    newFileBytes: pdfBytes,
                    newFileName: fileName,
                    oldFileKey: selectedDoc.FileKey,

                    uploadRequest: (bytes, fName) => new UploadFileRequest
                        (
                            FileConst.BUCKET_NAME,
                            WorkPermits.PermitType.Name,
                            FileConst.BuildEmployeeFolder
                                (
                                    WorkPermits.EmployeeId!,
                                    EmployeeFolderType.WorkPermit
                                ),
                            FileBytes: bytes,
                            FileName: fName
                        ),

                    dbRequest: s3FileName => new UpdateWorkPermitRequest
                        (
                            WorkPermits.WorkPermitName,
                            WorkPermits.DocumentSeries,
                            WorkPermits.WorkPermitNumber,
                            WorkPermits.ProtocolNumber,
                            WorkPermits.SpecialtyName,
                            WorkPermits.IssuingAuthority,
                            WorkPermits.IssueDate,
                            WorkPermits.ExpiryDate,
                            FileConst.BUCKET_NAME,
                            s3FileName,
                            WorkPermits.PermitType.Id,
                            WorkPermits.AdmissionStatus.Id
                        ),

                    dbUpdateAsync: async requests => await _workPermitApiServiceFactory.Create(WorkPermits.EmployeeId!)
                        .UpdateWorkPermitAsync(Guid.Parse(selectedDoc.WorkPermitId), requests)
                );

            if (result.IsFailure)
            {
                MessageBox.Show(result.StringMessage);
                return;
            }

            WorkPermits.ClearProperty();
        }
        private bool CanExecute_UpdateWorkPermitCommand() => true;

        // DELETE
        public RelayCommandAsync<WorkPermitDisplay> DeleteWorkPermitCommand { get; private set; }
        private async Task Execute_DeleteWorkPermitCommand(WorkPermitDisplay display)
        {
            if (display == null)
                return;

            var result = await _documentDeletionService.DeleteDocumentAsync
            (
                document: display,
                collection: WorkPermits!.LocalWorkPermits,
                getId: doc => Guid.Parse(doc.WorkPermitId),
                getFileKey: doc => doc.FileKey,
                dbDeleteAsync: async id => await _workPermitApiServiceFactory.Create(WorkPermits.EmployeeId!)
                    .DeleteWorkPermitAsync(id),
                refreshView: () =>
                {
                    WorkPermits.WorkPermitsView.Refresh();
                    WorkPermits.ClearProperty();
                }
            );

            if (result.IsFailure)
            {
                MessageBox.Show(result.StringMessage);
                return;
            }
        }
        private bool CanExecute_DeleteWorkPermitCommand(WorkPermitDisplay display) => SelectedEmployee != null;

        #endregion

        #region EditEmployeeSpecialty

        // CREATE
        public RelayCommandAsync CreateEmployeeSpecialtyCommand { get; private set; }
        private async Task Execute_CreateEmployeeSpecialtyCommand()
        {
            var employeeSpecialtiesService = _employeeSpecialtyApiServiceFactory.Create(Specialties.EmployeeId);

            var dbResult = await employeeSpecialtiesService.CreateAsync(new CreateEmployeeSpecialtyRequest(Guid.Parse(Specialties.SelectedSpecialty.SpecialtyId)));

            if (dbResult.IsFailure)
            {
                MessageBox.Show($"{dbResult.Errors}");
                return;
            }

            Specialties.LocalEmployeeSpecialties.Add(Specialties.SelectedSpecialty);
            Specialties.ClearProperty();
        }
        private bool CanExecute_CreateEmployeeSpecialtyCommand() => true;

        // DELETE
        public RelayCommandAsync<SpecialtyDisplay> DeleteEmployeeSpecialtyCommand { get; private set; }
        private async Task Execute_DeleteEmployeeSpecialtyCommand(SpecialtyDisplay display)
        {
            List<Error> errors = [];

            var result = await _employeeSpecialtyApiServiceFactory.Create(CurrentEmployeeDetails.EmployeeId).DeleteEmployeeSpecialtyAsync(Guid.Parse(display.SpecialtyId));

            if (!result.IsSuccess)
            {
                MessageBox.Show("Не удалось удалить специальность!");
                return;
            }

            Specialties.LocalEmployeeSpecialties.Remove(display);
            Specialties.ClearProperty();
        }
        private bool CanExecute_DeleteEmployeeSpecialtyCommand(SpecialtyDisplay display) => SelectedEmployee != null;

        #endregion

        #region EditAssignmentContract

        // CREATE
        public RelayCommandAsync CreateAssignmentContractCommand { get; private set; }
        private async Task Execute_CreateAssignmentContractCommand()
        {
            var assignmentService = _assignmentApiServiceFactory.Create(AssigmentsContracts!.EmployeeId!);

            var dbResult = await assignmentService.CreateManyAsync
                (
                    new CreateManyAssignmentsReqeust
                        (
                            [.. AssigmentsContracts.LocalAssignmentsContracts.Where(x => x.SaveStatus == SaveStatus.Local)
                                .Select
                                (
                                    x => new CreateManyDataAssignmentsReqeust
                                        (
                                            x.Position.Id,
                                            x.Department.Id,
                                            x.Manager?.Id,
                                            x.HireDate,
                                            x.TerminationDate,
                                            x.Status.Id,
                                            new CreateManyDataAssignmentContractReqeust
                                                (
                                                    x.EmployeeType.Id,
                                                    x.ContractNumber,
                                                    x.StartDate,
                                                    x.EndDate,
                                                    x.Salary,
                                                    null
                                                )
                                        )
                                )
                            ]
                        )
                );

            if (dbResult.IsFailure)
            {
                MessageBox.Show($"{dbResult.StringMessage}");
                return;
            }

            var filesToUpload = AssigmentsContracts.LocalAssignmentsContracts.Where(x => x.HasFileForUpload)
                .Select
                    (
                        x => new UploadFilesDataRequest
                            (
                                FileConst.BUCKET_NAME,
                                x.Position.Name,
                                FileConst.BuildEmployeeFolder
                                    (
                                        AssigmentsContracts.EmployeeId!,
                                        EmployeeFolderType.Assignment
                                    ),
                                FilePath: null,
                                FileBytes: x.FileBytes,
                                FileName: x.FileName,
                                ContentType: x.ContentType ?? "application/pdf"
                            )
                    )
                    .Where(x => x != null)
                    .ToArray();

            if (filesToUpload.Any())
            {
                var uploadResult = await _fileStorageService.UploadFilesAsync(new UploadFilesRequest(filesToUpload.ToList()!));

                if (uploadResult.IsFailure)
                {
                    MessageBox.Show($"{uploadResult.StringMessage}");
                    return;
                }
            }

            foreach (var item in AssigmentsContracts.LocalAssignmentsContracts)
                item.SetSaveStatus(SaveStatus.DataBase);

            AssigmentsContracts.AssignmentsContractsView.Refresh();
            AssigmentsContracts.ClearProperty();
        }
        private bool CanExecute_CreateAssignmentContractCommand() => true;

        // UPDATE
        public RelayCommandAsync UpdateAssignmentContractCommand { get; private set; }
        private async Task Execute_UpdateAssignmentContractCommand()
        {
            var assignmentService = _assignmentApiServiceFactory.Create(AssigmentsContracts!.EmployeeId!);

            var dbResult = await assignmentService.UpdateAssignmentAsync
                (
                    Guid.Parse(AssigmentsContracts.SelectedAssignmentContract.AssignmentId),
                    Guid.Parse(AssigmentsContracts.SelectedAssignmentContract.ContractId),
                    new UpdateAssignmentRequest
                        (
                            Guid.Parse(AssigmentsContracts.Position.Id),
                            Guid.Parse(AssigmentsContracts.Department.Id),
                            Guid.Parse(AssigmentsContracts.Manager.Id),
                            AssigmentsContracts.HireDate,
                            AssigmentsContracts.TerminationDate,
                            Guid.Parse(AssigmentsContracts.Status.Id),
                            new UpdateAssignmentContractRequest
                                (
                                    Guid.Parse(AssigmentsContracts.EmployeeType.Id),
                                    AssigmentsContracts.ContractNumber,
                                    AssigmentsContracts.StartDate,
                                    AssigmentsContracts.EndDate,
                                    AssigmentsContracts.Salary,
                                    null
                                )
                        )
                );

            if (dbResult.IsFailure)
            {
                MessageBox.Show($"{dbResult.Errors}");
                return;
            }

            AssigmentsContracts.AssignmentsContractsView.Refresh();
            AssigmentsContracts.ClearProperty();
        }
        private bool CanExecute_UpdateAssignmentContractCommand() => true;

        // DELETE
        public RelayCommandAsync<AssignmentContractDisplay> DeleteAssignmentContractCommand { get; private set; }
        private async Task Execute_DeleteAssignmentContractCommand(AssignmentContractDisplay display)
        {
            List<Error> errors = [];

            Func<Task> func = display.SaveStatus switch
            {
                SaveStatus.Local => async () =>
                {
                    AssigmentsContracts.LocalAssignmentsContracts.Remove(display);
                    AssigmentsContracts.AssignmentsContractsView.Refresh();
                    AssigmentsContracts.ClearProperty();
                },
                SaveStatus.DataBase => async () =>
                {
                    var result = await _assignmentApiServiceFactory.Create(CurrentEmployeeDetails.EmployeeId).DeleteAssignmentContractAsync(Guid.Parse(display.AssignmentId));

                    if (!result.IsSuccess)
                        errors.AddRange(result.Errors);

                    AssigmentsContracts.LocalAssignmentsContracts.Remove(display);
                    AssigmentsContracts.AssignmentsContractsView.Refresh();
                    AssigmentsContracts.ClearProperty();
                },

                _ => async () => Result.Success()
            };

            await func();

            errors.ShowError();
        }
        private bool CanExecute_DeleteAssignmentContractCommand(AssignmentContractDisplay display) => SelectedEmployee != null;

        #endregion

        #region SoftDeleteEmployeeCommand

        public RelayCommandAsync<EmployeeHrDisplay> SoftDeleteEmployeeCommand { get; private set; }
        private async Task Execute_SoftDeleteEmployeeCommand(EmployeeHrDisplay display)
        {
            var result = await _employeeService.SoftDeleteAsync(display.Id);

            if (!result.IsSuccess)
            {
                MessageBox.Show("Не удалось деактивировать пользователя!");
                return;
            }

            EmployeeHrs.Remove(display);
            CurrentEmployeeDetails = null!;
        }

        #endregion
    }
}
