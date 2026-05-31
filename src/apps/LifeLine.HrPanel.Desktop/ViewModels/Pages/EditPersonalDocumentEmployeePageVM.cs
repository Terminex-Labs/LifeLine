using LifeLine.Directory.Service.Client.Services.DocumentType;
using LifeLine.Employee.Service.Client.Services.Employee.PersonalDocument;
using LifeLine.File.Service.Client;
using LifeLine.HrPanel.Desktop.Models;
using Shared.Contracts.Request.EmployeeService.PersonalDocument;
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
    public sealed class EditPersonalDocumentEmployeePageVM : BasePageViewModel, IUpdatable, IAsyncInitializable
    {
        private readonly INavigationPage _navigationPage;
        private readonly IFileStorageService _fileStorageService;
        private readonly IDocumentTypeReadOnlyService _documentTypeReadOnlyService;
        private readonly IPersonalDocumentApiServiceFactory _personalDocumentApiServiceFactory;
        private readonly IFileDialogService _fileDialogService;

        private bool _isEditMode;

        public EditPersonalDocumentEmployeePageVM
            (
                INavigationPage navigationPage,
                IFileDialogService fileDialogService,
                IFileStorageService fileStorageService,
                IDocumentTypeReadOnlyService documentTypeReadOnlyService,
                IPersonalDocumentApiServiceFactory personalDocumentApiServiceFactory
            )
        {
            _navigationPage = navigationPage;
            _fileDialogService = fileDialogService;
            _fileStorageService = fileStorageService;
            _documentTypeReadOnlyService = documentTypeReadOnlyService;
            _personalDocumentApiServiceFactory = personalDocumentApiServiceFactory;

            UpdatePersonalDocumentEmployeeCommand = new RelayCommandAsync(Execute_UpdatePersonalDocumentEmployeeCommand, CanExecute_UpdatePersonalDocumentEmployeeCommand);
            SelectFileCommand = new RelayCommand(Execute_SelectFileCommand, CanExecute_SelectFileCommand);
        }

        async Task IAsyncInitializable.InitializeAsync()
        {
            if (IsInitialize) 
                return;

            IsInitialize = false;

            await GetAllDocumentTypeAsync();

            CreateNewPersonalDocumentDisplay();

            if (PersonalDocumentDisplay != null && SelectedDocumentType == null && _isEditMode)
            {
                if (PersonalDocumentDisplay.DocumentType != null)
                    SelectedDocumentType = DocumentTypes.FirstOrDefault(x => x.Id == PersonalDocumentDisplay.DocumentType.Id);
            }

            IsInitialize = true;
        }

        public void Update<TData>(TData value, TransmittingParameter parameter)
        {
            if (value is ValueTuple<EmployeeDetailsDisplay, PersonalDocumentDisplay?> tuple)
            {
                CurrentEmployeeDetails = tuple.Item1;
                var incomingDocument = tuple.Item2;

                if (incomingDocument != null)
                {
                    // === РЕЖИМ РЕДАКТИРОВАНИЯ ===
                    _isEditMode = true;

                    PersonalDocumentDisplay = incomingDocument;

                    if (DocumentTypes.Count > 0 && PersonalDocumentDisplay.DocumentType != null)
                        SelectedDocumentType = DocumentTypes.FirstOrDefault(x => x.Id == PersonalDocumentDisplay.DocumentType.Id);
                }
                else
                {
                    // === РЕЖИМ СОЗДАНИЯ ===
                    _isEditMode = false;

                    CreateNewPersonalDocumentDisplay();

                    SelectedDocumentType = null;
                }

                UpdatePersonalDocumentEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        #region Display

        private EmployeeDetailsDisplay _currentEmployeeDetails = null!;
        public EmployeeDetailsDisplay CurrentEmployeeDetails
        {
            get => _currentEmployeeDetails;
            set => SetProperty(ref _currentEmployeeDetails, value);
        }

        private DocumentTypeDisplay? _selectedDocumentType;
        public DocumentTypeDisplay? SelectedDocumentType
        {
            get => _selectedDocumentType;
            set
            {
                SetProperty(ref _selectedDocumentType, value);
                UpdatePersonalDocumentEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<DocumentTypeDisplay> DocumentTypes { get; private init; } = [];
        private async Task GetAllDocumentTypeAsync()
        {
            var documentTypes = await _documentTypeReadOnlyService.GetAllAsync();

            DocumentTypes.Load(documentTypes.Select(documentType => new DocumentTypeDisplay(documentType)).ToList());
        }

        private PersonalDocumentDisplay _personalDocumentDisplay = null!;
        public PersonalDocumentDisplay PersonalDocumentDisplay
        {
            get => _personalDocumentDisplay;
            set => SetProperty(ref _personalDocumentDisplay, value);
        }
        private void CreateNewPersonalDocumentDisplay()
            => PersonalDocumentDisplay = new PersonalDocumentDisplay(new PersonalDocumentResponse(Guid.Empty, Guid.Empty, string.Empty, string.Empty, string.Empty), DocumentTypes, SaveStatus.Local);

        #endregion

        public string? FilePath
        {
            get => field;
            set
            {
                SetProperty(ref field, value);
                UpdatePersonalDocumentEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public RelayCommandAsync UpdatePersonalDocumentEmployeeCommand { get; private set; }
        private async Task Execute_UpdatePersonalDocumentEmployeeCommand()
        {
            if (_isEditMode)
            {
                // UPDATE
                var resultMiniO = await _fileStorageService.UploadFileAsync
                    (
                        new UploadFileRequest
                            (
                                FileConst.BUCKET_NAME,
                                SelectedDocumentType!.Name,
                                FileConst.BuildEmployeeFolder
                                    (
                                        CurrentEmployeeDetails.EmployeeId, 
                                        EmployeeFolderType.PersonalDocument
                                    ),
                                FilePath!
                            )
                    );

                if (resultMiniO.IsFailure)
                {
                    MessageBox.Show(resultMiniO.StringMessage);
                    return;
                }

                var resultUpdate = await _personalDocumentApiServiceFactory.Create(CurrentEmployeeDetails.EmployeeId).UpdatePersonalDocumentAsync
                (
                    PersonalDocumentDisplay.PersonalDocumentId,
                    new UpdatePersonalDocumentRequest
                    (
                        SelectedDocumentType.Id,
                        PersonalDocumentDisplay.DocumentNumber,
                        PersonalDocumentDisplay.DocumentSeries,
                        FileConst.BUCKET_NAME,
                        PersonalDocumentDisplay.FileName
                    )
                );

                if (resultUpdate.IsSuccess)
                    _navigationPage.TransmittingValue(PersonalDocumentDisplay, FrameName.MainFrame, PageName.EmployeePage, TransmittingParameter.Update);
                else
                    MessageBox.Show($"Обновление персональных документов: {resultUpdate.StringMessage}");
            }
            else
            {
                // CREATE
                var resultMiniO = await _fileStorageService.UploadFileAsync
                    (
                        new UploadFileRequest
                            (
                                FileConst.BUCKET_NAME,
                                SelectedDocumentType!.Name,
                                FileConst.BuildEmployeeFolder
                                    (
                                        CurrentEmployeeDetails.EmployeeId, 
                                        EmployeeFolderType.PersonalDocument
                                    ),
                                FilePath!
                            )
                    );

                if (resultMiniO.IsFailure)
                {
                    MessageBox.Show(resultMiniO.StringMessage);
                    return;
                }

                var resultCreate = await _personalDocumentApiServiceFactory.Create(CurrentEmployeeDetails.EmployeeId).CreateAsync
                (
                    new CreatePersonalDocumentRequest
                    (
                        Guid.Parse(SelectedDocumentType.Id),
                        PersonalDocumentDisplay.DocumentNumber,
                        PersonalDocumentDisplay.DocumentSeries
                        // TODO : Добавить id для pdf файла 
                    )
                );

                if (resultCreate.IsSuccess)
                {
                    PersonalDocumentDisplay.SetDocumentType(SelectedDocumentType.Id);

                    _navigationPage.TransmittingValue(PersonalDocumentDisplay, FrameName.MainFrame, PageName.EmployeePage, TransmittingParameter.Create);
                }
                else
                    MessageBox.Show($"Внесение персональных документов: {resultCreate.StringMessage}");
            }
        }
        private bool CanExecute_UpdatePersonalDocumentEmployeeCommand() 
            => SelectedDocumentType != null && !string.IsNullOrWhiteSpace(FilePath);

        public RelayCommand SelectFileCommand { get; private set; }
        private void Execute_SelectFileCommand()
            => FilePath = _fileDialogService.GetFile($"Выберите файл: {FileDialogConsts.PERSONAL_DOCUMENT}", FileFilters.Images);
        private bool CanExecute_SelectFileCommand() => true;
    }
}
