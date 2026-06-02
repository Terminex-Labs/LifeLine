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
using LifeLine.HrPanel.Desktop.Services.FilePreview;
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
using Shared.Contracts.Response.Files;
using Shared.WPF.Commands;
using Shared.WPF.Enums;
using Shared.WPF.Extensions;
using Shared.WPF.Services.Conversion;
using Shared.WPF.Services.FileDialog;
using Shared.WPF.ViewModels.Abstract;
using System.Collections.ObjectModel;
using System.Windows;
using Terminex.Common.Results;

namespace LifeLine.HrPanel.Desktop.ViewModels.Pages
{
    internal sealed class EmployeeCreatePageVM : BasePageViewModel, IAsyncInitializable
    {
        private readonly IEmployeeService _employeeService;
        private readonly IFileDialogService _fileDialogService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFilePreviewService _filePreviewService;
        private readonly IGenderReadOnlyService _genderReadOnlyService;
        private readonly IStatusReadOnlyService _statusReadOnlyService;
        private readonly ISpecialtyReadOnlyService _specialtyReadOnlyService;
        private readonly IPermitTypeReadOnlyService _permitTypeReadOnlyService;
        private readonly IDepartmentReadOnlyService _departmentReadOnlyService;
        private readonly IDocumentTypeReadOnlyService _documentTypeReadOnlyService;
        private readonly IEmployeeTypeReadOnlyService _employeeTypeReadOnlyService;
        private readonly IEducationLevelReadOnlyService _educationLevelReadOnlyService;
        private readonly IAdmissionStatusReadOnlyService _admissionStatusReadOnlyService;
        private readonly IPositionReadOnlyApiServiceFactory _positionReadOnlyApiServiceFactory;

        private readonly IContactInformationApiServiceFactory _contactInformationApiServiceFactory;
        private readonly IPersonalDocumentApiServiceFactory _personalDocumentApiServiceFactory;
        private readonly IEducationDocumentApiServiceFactory _educationDocumentApiServiceFactory;
        private readonly IWorkPermitApiServiceFactory _workPermitApiServiceFactory;
        private readonly IEmployeeSpecialtyApiServiceFactory _employeeSpecialtyApiServiceFactory;
        private readonly IAssignmentApiServiceFactory _assignmentApiServiceFactory;

        private readonly IDocumentConversionService _documentConversionService;
        private readonly IImageCompressionService _imageCompressionService;

        public EmployeeCreatePageVM
            (

                IEmployeeService employeeService,
                IFileDialogService fileDialogService,
                IFileStorageService fileStorageService,
                IFilePreviewService filePreviewService,
                IGeneratePdfService generatePdfService,
                IGenderReadOnlyService genderReadOnlyService,
                IStatusReadOnlyService statusReadOnlyService,
                ISpecialtyReadOnlyService specialtyReadOnlyService,
                IPermitTypeReadOnlyService permitTypeReadOnlyService,
                IDepartmentReadOnlyService departmentReadOnlyService,
                IDocumentTypeReadOnlyService documentTypeReadOnlyService,
                IEmployeeTypeReadOnlyService employeeTypeReadOnlyService,
                IEducationLevelReadOnlyService educationLevelReadOnlyService,
                IAdmissionStatusReadOnlyService admissionStatusReadOnlyService,
                IPositionReadOnlyApiServiceFactory positionReadOnlyApiServiceFactory,

                IContactInformationApiServiceFactory contactInformationApiServiceFactory,
                IPersonalDocumentApiServiceFactory personalDocumentApiServiceFactory,
                IEducationDocumentApiServiceFactory educationDocumentApiServiceFactory,
                IWorkPermitApiServiceFactory workPermitApiServiceFactory,
                IEmployeeSpecialtyApiServiceFactory employeeSpecialtyApiServiceFactory,
                IAssignmentApiServiceFactory assignmentApiServiceFactory,

                IDocumentConversionService documentConversionService, 
                IImageCompressionService imageCompressionService
            )
        {
            _employeeService = employeeService;
            _fileDialogService = fileDialogService;
            _fileStorageService = fileStorageService;
            _filePreviewService = filePreviewService;
            _genderReadOnlyService = genderReadOnlyService;
            _statusReadOnlyService = statusReadOnlyService;
            _specialtyReadOnlyService = specialtyReadOnlyService;
            _permitTypeReadOnlyService = permitTypeReadOnlyService;
            _departmentReadOnlyService = departmentReadOnlyService;
            _documentTypeReadOnlyService = documentTypeReadOnlyService;
            _employeeTypeReadOnlyService = employeeTypeReadOnlyService;
            _educationLevelReadOnlyService = educationLevelReadOnlyService;
            _admissionStatusReadOnlyService = admissionStatusReadOnlyService;
            _positionReadOnlyApiServiceFactory = positionReadOnlyApiServiceFactory;

            _contactInformationApiServiceFactory = contactInformationApiServiceFactory;
            _personalDocumentApiServiceFactory = personalDocumentApiServiceFactory;
            _educationDocumentApiServiceFactory = educationDocumentApiServiceFactory;
            _workPermitApiServiceFactory = workPermitApiServiceFactory;
            _employeeSpecialtyApiServiceFactory = employeeSpecialtyApiServiceFactory;
            _assignmentApiServiceFactory = assignmentApiServiceFactory;

            _documentConversionService = documentConversionService;
            _imageCompressionService = imageCompressionService;

            PersonalInfo = new();
            PersonalPhoto = new(_fileDialogService, _imageCompressionService);
            ContactInformation = new();
            PersonalDocuments = new(_fileDialogService, _fileStorageService, _filePreviewService, _documentConversionService, DocumentTypes);
            EducationDocuments = new(_fileDialogService, _fileStorageService, _filePreviewService, _documentConversionService, DocumentTypes, EducationLevels);
            WorkPermits = new(_fileDialogService, _documentConversionService, PermitTypes, AdmissionStatuses);
            Specialties = new();
            AssigmentsContracts = new(_fileDialogService, _documentConversionService, _positionReadOnlyApiServiceFactory, Departments, Managers, Statuses, EmployeeTypes);

            ExecuteStepCommand = new RelayCommandAsync<EmployeeCreationSteps>(Execute_StepCommand, CanExecute_StepCommand);
        }

        async Task IAsyncInitializable.InitializeAsync()
        {
            if (IsInitialize)
                return;

            IsInitialize = false;

            await GetAllSpecialty();
            await GetAllGenderAsync();
            await GetAllStatusAsync();
            await GetAllManagerAsync();
            await GetAllPermitTypeAsync();
            await GetAllDepartmentAsync();
            await GetAllDocumentTypeAsync();
            await GetAllEmployeeTypeAsync();
            await GetAllEducationLevelAsync();
            await GetAllAdmissionStatusAsync();

            IsInitialize = true;
        }

        #region Features new ViewModel

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

        private SpecialtiesVM ?_specialties;
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

        public EmployeeCreationSteps Steps
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public RelayCommandAsync<EmployeeCreationSteps> ExecuteStepCommand { get; private set; }
        private async Task Execute_StepCommand(EmployeeCreationSteps value)
        {
            Func<Task<Result>> func = Steps switch
            {
                EmployeeCreationSteps.PersonalInfo => async () => await CreateEmployeeAsync(),
                EmployeeCreationSteps.PersonalPhoto => async () => await AddPersonalPhotoAsync(),
                EmployeeCreationSteps.ContactInformation => async () => await CreateContactInformation(),
                EmployeeCreationSteps.PersonalDocuments => async () => await CreatePersonalDocuments(),
                EmployeeCreationSteps.EducationDocuments => async () => await CreateEducationDocuments(),
                EmployeeCreationSteps.WorkPermits => async () => await CreateWorkPermits(),
                EmployeeCreationSteps.Specialties => async () => await CreateEmployeeSpecialties(),
                EmployeeCreationSteps.AssigmentsContracts => async () => await CreateAssignmentContracts(),

                _ => async () => Result.Success()
            };
            var result = await func();

            if (result.IsFailure)
            {
                MessageBox.Show(result.StringMessage);
                return;
            }

            Steps = value;
            ClearLocalLists();
        }
        private bool CanExecute_StepCommand(EmployeeCreationSteps value) => true;

        private async Task<Result> CreateEmployeeAsync()
        {
            var result = await _employeeService.AddAsync<CreateEmployeeRequest, EmployeeIdResponse>
                (
                    new CreateEmployeeRequest
                        (
                            PersonalInfo!.Surname!,
                            PersonalInfo.Name!,
                            PersonalInfo.Patronymic,
                            PersonalInfo.Gender!.Id
                        )
                );

            if (result.IsFailure)
                return Result.Failure(result.Errors);

            SetEmployeeId(result.Value.EmployeeId.ToString());

            return Result.Success();
        }

        private async Task<Result> AddPersonalPhotoAsync()
        {
            var avatarBytes = PersonalPhoto!.GetCompressedBytes();
            var fileName = PersonalPhoto.GetFileName();

            if (avatarBytes == null || string.IsNullOrWhiteSpace(fileName))
                return Result.Failure(Error.New(ErrorCode.Null, "Аватарка не нвыбрана!"));

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
                            FilePath: null,
                            FileBytes: avatarBytes,
                            FileName: fileName
                        )
                );

            if (fileResult.IsFailure)
                return Result.Failure(fileResult.Errors);

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
                return Result.Failure(dbResult.Errors);

            return Result.Success();
        }

        private async Task<Result> CreateContactInformation()
        {
            var contactInformationService = _contactInformationApiServiceFactory.Create(ContactInformation!.EmployeeId!);

            var result = await contactInformationService.CreateAsync
                (
                    new CreateContactInformationRequest
                        (
                            ContactInformation.PersonalPhone,
                            string.IsNullOrWhiteSpace(ContactInformation.CorporatePhone) ? null : ContactInformation.CorporatePhone,
                            ContactInformation.PersonalEmail,
                            string.IsNullOrWhiteSpace(ContactInformation.CorporateEmail) ? null : ContactInformation.CorporateEmail,
                            ContactInformation.PostalCode,
                            ContactInformation.Region,
                            ContactInformation.City,
                            ContactInformation.Street,
                            ContactInformation.Building,
                            string.IsNullOrWhiteSpace(ContactInformation.Apartment) ? null : ContactInformation.Apartment
                        )
                );

            return result;
        }

        private async Task<Result> CreatePersonalDocuments()
        {
            if (PersonalDocuments == null)
                return Result.Failure(Error.Validation("Данные отсутствуют!"));

            var documentsToSave = PersonalDocuments.LocalPersonalDocuments
                .Where(x => x.SaveStatus == SaveStatus.Local).ToList();

            if (documentsToSave.Count == 0)
                return Result.Failure(Error.Validation("Документы для сохранения отсутствуют!"));

            //var documentsWithFiles = documentsToSave.Where(x => x.HasFileForUpload).ToList();

            IReadOnlyList<UploadFileResponse>? uploadResponses = null;

            if (documentsToSave.Count > 0)
            {
                var uploadRequests = documentsToSave
                    .Select(doc => new UploadFilesDataRequest
                        (
                            BucketName: FileConst.BUCKET_NAME,
                            AdditionalName: doc.DocumentType.Name,
                            SubFolder: FileConst.BuildEmployeeFolder
                                (
                                    PersonalDocuments.EmployeeId!,
                                    EmployeeFolderType.PersonalDocument
                                ),
                            FilePath: null,
                            FileBytes: doc.FileBytes,
                            FileName: doc.FileName,
                            ContentType: doc.ContentType ?? "application/pdf"
                        )).ToList();

                var uploadResult = await _fileStorageService.UploadFilesAsync(new UploadFilesRequest(uploadRequests));

                if (uploadResult.IsFailure)
                    return Result.Failure(uploadResult.Errors);

                uploadResponses = uploadResult.Value;
            }

            var uploadedFileNames = new Queue<string>(uploadResponses?.Select(f => f.FileName) ?? Array.Empty<string>());

            var dbRequests = documentsToSave
                .Select(doc => new CreateDataPersonalDocumentRequest
                (
                    doc.DocumentTypeId.ToString(),
                    doc.DocumentNumber,
                    doc.DocumentSeries,
                    FileConst.BUCKET_NAME,
                    uploadedFileNames.TryDequeue(out var fileName) ? fileName : null
                )).ToArray();

            var service = _personalDocumentApiServiceFactory.Create(PersonalDocuments.EmployeeId!);
            var dbResult = await service.CreateManyAsync(new CreateManyPersonalDocumentsRequest([.. dbRequests]));

            if (dbResult.IsFailure)
                return Result.Failure(dbResult.Errors);

            return Result.Success();
        }

        private async Task<Result> CreateEducationDocuments()
        {
            if (EducationDocuments == null)
                return Result.Failure(Error.Validation("Данные отсутствуют!"));

            var documentsToSave = EducationDocuments.LocalEducationDocuments
                .Where(x => x.SaveStatus == SaveStatus.Local).ToList();

            if (documentsToSave.Count == 0)
                return Result.Failure(Error.Validation("Документы для сохранения отсутствуют!"));

            IReadOnlyList<UploadFileResponse>? uploadResponses = null;

            if (documentsToSave.Count > 0)
            {
                var uploadRequests = documentsToSave
                    .Select(doc => new UploadFilesDataRequest
                        (
                            BucketName: FileConst.BUCKET_NAME,
                            AdditionalName: doc.DocumentType.Name,
                            SubFolder: FileConst.BuildEmployeeFolder
                                (
                                    EducationDocuments.EmployeeId!,
                                    EmployeeFolderType.EducationDocument
                                ),
                            FilePath: null,
                            FileBytes: doc.FileBytes,
                            FileName: doc.FileName,
                            ContentType: doc.ContentType ?? "application/pdf"
                        )).ToList();

                var uploadResult = await _fileStorageService.UploadFilesAsync(new UploadFilesRequest(uploadRequests));

                if (uploadResult.IsFailure)
                    return Result.Failure(uploadResult.Errors);

                uploadResponses = uploadResult.Value;
            }

            var uploadedFileNames = new Queue<string>(uploadResponses?.Select(f => f.FileName) ?? Array.Empty<string>());

            var dbRequests = documentsToSave
                .Select(doc => new CreateDataEducationDocumentReqeust
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
                    uploadedFileNames.TryDequeue(out var fileName) ? fileName : null
                ));

            var service = _educationDocumentApiServiceFactory.Create(EducationDocuments!.EmployeeId!);

            var dbResult = await service.CreateManyAsync(new CreateManyEducationDocumentsReqeust([.. dbRequests]));

            if (dbResult.IsFailure)
                return Result.Failure(dbResult.Errors);

            return Result.Success();
        }

        private async Task<Result> CreateWorkPermits()
        {
            var workPermitService = _workPermitApiServiceFactory.Create(WorkPermits!.EmployeeId!);

            var dbResult = await workPermitService.CreateManyAsync
                (
                    new CreateManyWorkPermitsRequest
                        (
                            [.. WorkPermits.LocalWorkPermits.Select
                                (
                                    x => new CreateManyDataWorkPermitsRequest
                                        (
                                            x.WorkPermitName,
                                            x.DocumentSeries,
                                            x.WorkPermitNumber,
                                            x.ProtocolNumber,
                                            x.SpecialtyName,
                                            x.IssuingAuthority,
                                            x.IssueDate.ToString(),
                                            x.ExpiryDate.ToString(),
                                            x.PermitType.Id,
                                            x.AdmissionStatus.Id
                                        )
                                )
                            ]
                        )
                );

            if (dbResult.IsFailure)
                return Result.Failure(dbResult.Errors);

            var filesToUpload = WorkPermits.LocalWorkPermits.Where(x => x.HasFileForUpload)
                .Select(x =>
                {
                    if (x.FileBytes != null && !string.IsNullOrWhiteSpace(x.FileName))
                    {
                        return new UploadFilesDataRequest
                            (
                                FileConst.BUCKET_NAME,
                                x.PermitType.Name,
                                FileConst.BuildEmployeeFolder
                                    (
                                        WorkPermits.EmployeeId!,
                                        EmployeeFolderType.WorkPermit
                                    ),
                                FilePath: null,
                                FileBytes: x.FileBytes,
                                FileName: x.FileName,
                                ContentType: x.ContentType ?? "application/pdf"
                            );
                    }
                    else if (!string.IsNullOrWhiteSpace(x.FileName) && System.IO.File.Exists(x.FilePath))
                    {
                        return new UploadFilesDataRequest
                            (
                                FileConst.BUCKET_NAME,
                                x.PermitType.Name,
                                FileConst.BuildEmployeeFolder
                                    (
                                        WorkPermits.EmployeeId!,
                                        EmployeeFolderType.WorkPermit
                                    ),
                                FilePath: x.FilePath,
                                FileBytes: null,
                                FileName: null,
                                ContentType: null
                            );
                    }

                    return null;
                })
                .Where(x => x != null)
                .ToArray();

            if (filesToUpload.Any())
            {
                var uploadResult = await _fileStorageService.UploadFilesAsync(new UploadFilesRequest(filesToUpload.ToList()!));

                if (uploadResult.IsFailure)
                    return Result.Failure(uploadResult.Errors);
            }

            return Result.Success();
        }

        private async Task<Result> CreateEmployeeSpecialties()
        {
            var employeeSpecialtiesService = _employeeSpecialtyApiServiceFactory.Create(Specialties!.EmployeeId!);

            var result = await employeeSpecialtiesService.CreateManyAsync
                (
                    new CreateManyEmployeeSpecialtiesRequest
                        (
                            [.. Specialties.LocalEmployeeSpecialties.Select
                                (
                                    x => new CreateManyDataEmployeeSpecialtiesRequest(x.SpecialtyId)
                                )
                            ]
                        )
                );

            return result;
        }

        private async Task<Result> CreateAssignmentContracts()
        {
            var assignmentService = _assignmentApiServiceFactory.Create(AssigmentsContracts!.EmployeeId!);

            var dbResult = await assignmentService.CreateManyAsync
                (
                    new CreateManyAssignmentsReqeust
                        (
                            [.. AssigmentsContracts.LocalAssignmentsContracts.Select
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
                return Result.Failure(dbResult.Errors);

            var filesToUpload = AssigmentsContracts.LocalAssignmentsContracts.Where(x => x.HasFileForUpload)
                .Select(x =>
                {
                    if (x.FileBytes != null && !string.IsNullOrWhiteSpace(x.FileName))
                    {
                        return new UploadFilesDataRequest
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
                            );
                    }
                    else if (!string.IsNullOrWhiteSpace(x.FilePath) && System.IO.File.Exists(x.FilePath))
                    {
                        return new UploadFilesDataRequest
                            (
                                FileConst.BUCKET_NAME,
                                x.Position.Name,
                                FileConst.BuildEmployeeFolder
                                    (
                                        AssigmentsContracts.EmployeeId!,
                                        EmployeeFolderType.Assignment
                                    ),
                                FilePath: x.FilePath,
                                FileBytes: null,
                                FileName: null,
                                ContentType: null
                            );
                    }

                    return null;
                })
                .Where(x => x != null)
                .ToArray();

            if (filesToUpload.Any())
            {
                var uploadResult = await _fileStorageService.UploadFilesAsync(new UploadFilesRequest(filesToUpload.ToList()!));

                if (uploadResult.IsFailure)
                    return Result.Failure(uploadResult.Errors);
            }

            return Result.Success();
        }

        private void SetEmployeeId(string id)
        {
            PersonalInfo!.EmployeeId = id;
            PersonalPhoto!.EmployeeId = id;
            ContactInformation!.EmployeeId = id;
            PersonalDocuments!.EmployeeId = id;
            EducationDocuments!.EmployeeId = id;
            WorkPermits!.EmployeeId = id;
            Specialties!.EmployeeId = id;
            AssigmentsContracts!.EmployeeId = id;
        }

        #endregion

        #region Получение данных

        // GENDERS
        public ObservableCollection<GenderResponse> Genders { get; private init; } = [];
        private async Task GetAllGenderAsync() => Genders.Load(await _genderReadOnlyService.GetAllAsync());

        // DOCUMENT TYPE
        public ObservableCollection<DocumentTypeDisplay> DocumentTypes { get; private init; } = [];
        private async Task GetAllDocumentTypeAsync()
        {
            var listResponse = await _documentTypeReadOnlyService.GetAllAsync();

            foreach (var item in listResponse)
                DocumentTypes.Add(new DocumentTypeDisplay(item));
        }

        // ADMISSION STATUS
        public ObservableCollection<AdmissionStatusDisplay> AdmissionStatuses { get; private init; } = [];
        private async Task GetAllAdmissionStatusAsync()
        {
            var listResponse = await _admissionStatusReadOnlyService.GetAllAsync();

            foreach (var item in listResponse)
                AdmissionStatuses.Add(new AdmissionStatusDisplay(item));
        }

        // PERMIT TYPE
        public ObservableCollection<PermitTypeDisplay> PermitTypes { get; private init; } = [];
        private async Task GetAllPermitTypeAsync()
        {
            var listResponse = await _permitTypeReadOnlyService.GetAllAsync();

            foreach (var item in listResponse)
                PermitTypes.Add(new PermitTypeDisplay(item));
        }

        // EDUCATION LEVEL
        public ObservableCollection<EducationLevelDisplay> EducationLevels { get; private init; } = [];
        private async Task GetAllEducationLevelAsync()
        {
            var listResponse = await _educationLevelReadOnlyService.GetAllAsync();

            foreach (var item in listResponse)
                EducationLevels.Add(new EducationLevelDisplay(item));
        }

        // EMPLOYEE TYPE
        public ObservableCollection<EmployeeTypeDisplay> EmployeeTypes { get; private init; } = [];
        private async Task GetAllEmployeeTypeAsync()
        {
            var listResponse = await _employeeTypeReadOnlyService.GetAllAsync();

            foreach (var item in listResponse)
                EmployeeTypes.Add(new EmployeeTypeDisplay(item));
        }

        // DEPARTMENT
        public ObservableCollection<DepartmentDisplay> Departments { get; private init; } = [];
        private async Task GetAllDepartmentAsync()
        {
            var departments = await _departmentReadOnlyService.GetAllAsync();

            Departments.Load([.. departments.Select(department => new DepartmentDisplay(department))]);
        }

        // STATUS
        public ObservableCollection<StatusDisplay> Statuses { get; private init; } = [];
        private async Task GetAllStatusAsync()
        {
            var listResponse = await _statusReadOnlyService.GetAllAsync();

            foreach (var item in listResponse)
                Statuses.Add(new StatusDisplay(item));
        }

        // SPECIALTY
        public ObservableCollection<SpecialtyDisplay> SpecialtiesCollection { get; private init; } = [];
        private async Task GetAllSpecialty()
        {
            var specialties = await _specialtyReadOnlyService.GetAllAsync();

            foreach (var item in specialties)
                SpecialtiesCollection.Add(new SpecialtyDisplay(item));
        }

        // MANAGER
        public ObservableCollection<ManagerDisplay> Managers { get; private init; } = [];
        private async Task GetAllManagerAsync()
        {
            var listResponse = await _employeeService.GetAllAsync();

            foreach (var item in listResponse)
                Managers.Add(new ManagerDisplay(item));
        }

        #endregion

        #region Доп. методы

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
    }
}
