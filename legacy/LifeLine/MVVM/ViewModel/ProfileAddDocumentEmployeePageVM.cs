using LifeLine.MVVM.Models.AppModel;
using LifeLine.MVVM.Models.MSSQL_DB;
using LifeLine.Services.DataBaseServices;
using LifeLine.Services.DialogServices;
using LifeLine.Services.NavigationPages;
using LifeLine.Services.NavigationWindow;
using LifeLine.Utils.Enum;
using LifeLine.Utils.Helper;
using MasterAnalyticsDeadByDaylight.Command;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;

namespace LifeLine.MVVM.ViewModel
{
    class ProfileAddDocumentEmployeePageVM : BaseViewModel, IUpdatablePage
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IDialogService _dialogService;
        private readonly IDataBaseService _dataBaseService;
        private readonly INavigationPage _navigationPage;
        private readonly INavigationWindow _navigationWindow;
        private readonly IOpenFileDialogService _openFileDialogService;

        public ProfileAddDocumentEmployeePageVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _dialogService = _serviceProvider.GetService<IDialogService>();
            _dataBaseService = _serviceProvider.GetService<IDataBaseService>();
            _navigationPage = _serviceProvider.GetService<INavigationPage>();
            _navigationWindow = _serviceProvider.GetService<INavigationWindow>();
            _openFileDialogService = _serviceProvider.GetService<IOpenFileDialogService>();

            GetTypeDocument();
        }

        public void Update(object value)
        {
            if (value is Employee employee)
            {
                CurrentUser = employee;
                _currentUserRole = employee;
                EmployeeVisibilityManager();
                GetDocument();
            }

            // TODO : Хз что это
            if (value is bool)
            {
                GetDocument();
            }

            if (value is Patient patient)
            {
                UserPatient = patient;
                _currentUserRole = patient;
                PatientVisibilityManager();
                GetDocument();
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------------------------------

        #region Visibility

        private Visibility _employeeFIOtbVisibility;
        public Visibility EmployeeFIOtbVisibility
        {
            get => _employeeFIOtbVisibility;
            set
            {
                _employeeFIOtbVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _patientFIOtbVisibility;
        public Visibility PatientFIOtbVisibility
        {
            get => _patientFIOtbVisibility;
            set
            {
                _patientFIOtbVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _employeeDocumentsLWVisibility;
        public Visibility EmployeeDocumentsLWVisibility
        {
            get => _employeeDocumentsLWVisibility;
            set
            {
                _employeeDocumentsLWVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _patientDocumentsLWVisibility;
        public Visibility PatientDocumentsLWVisibility
        {
            get => _patientDocumentsLWVisibility;
            set
            {
                _patientDocumentsLWVisibility = value;
                OnPropertyChanged();
            }
        }

        private void EmployeeVisibilityManager()
        {
            EmployeeDocumentsLWVisibility = Visibility.Visible;
            PatientDocumentsLWVisibility = Visibility.Collapsed;
            PatientFIOtbVisibility = Visibility.Collapsed;
            EmployeeFIOtbVisibility = Visibility.Visible;
        }

        private void PatientVisibilityManager()
        {
            EmployeeDocumentsLWVisibility = Visibility.Collapsed;
            PatientDocumentsLWVisibility = Visibility.Visible;
            PatientFIOtbVisibility = Visibility.Visible;
            EmployeeFIOtbVisibility = Visibility.Collapsed;
        }

        #endregion

        #region Свойства

        private object _currentUserRole = null;

        private Employee _currentUser;
        public Employee CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        private Patient _userPatient;
        public Patient UserPatient
        {
            get => _userPatient;
            set
            {
                _userPatient = value;
                OnPropertyChanged();
            }
        }

            #region Заполнение

        private TypeDocument _selectedTypeDocument;
        public TypeDocument SelectedTypeDocument
        {
            get => _selectedTypeDocument;
            set
            {
                _selectedTypeDocument = value;
                OnPropertyChanged();
            }
        }

        private string _numberDocumentTB;
        public string NumberDocumentTB
        {
            get => _numberDocumentTB;
            set
            {
                _numberDocumentTB = value;
                OnPropertyChanged();
            }
        }

        private string _placeOfIssueTB;
        public string PlaceOfIssueTB
        {
            get => _placeOfIssueTB;
            set
            {
                _placeOfIssueTB = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _dateOfIssueDP;
        public DateTime? DateOfIssueDP
        {
            get => _dateOfIssueDP;
            set
            {
                _dateOfIssueDP = value;
                OnPropertyChanged();
            }
        }

        private string _documentFile;
        public string DocumentFile
        {
            get => _documentFile;
            set
            {
                _documentFile = value;
                OnPropertyChanged();
            }
        }

        private ImageDocumentEmployee _selectImage;
        public ImageDocumentEmployee SelectImage
        {
            get => _selectImage;
            set
            {
                _selectImage = value;
                OnPropertyChanged();
            }
        }

        private string _searchDocumentEmployeeTB;
        public string SearchDocumentEmployeeTB
        {
            get => _searchDocumentEmployeeTB;
            set
            {
                _searchDocumentEmployeeTB = value;
                SearchDocumentEmployeeAsync();
                OnPropertyChanged();
            }
        }

            #endregion

            #region ListView

        public ObservableCollection<Document> Documents { get; set; } = [];
        public ObservableCollection<DocumentPatient> DocumentsPatient { get; set; } = [];
        public ObservableCollection<TypeDocument> TypeDocuments { get; set; } = [];
        public ObservableCollection<ImageDocumentEmployee> ImageDocumentEmployees { get; set; } = [];
        public List<Document> SelectedFilesEmployee { get; set; } = [];
        public List<DocumentPatient> SelectedFilesPatient { get; set; } = [];

            #endregion

        #endregion

        // ---------------------------------------------------------------------------------------------------------------------------------------------

        #region Команды

        private RelayCommand _addDocumentEmployeeCommand;
        public RelayCommand AddDocumentEmployeeCommand { get => _addDocumentEmployeeCommand ??= new(obj => { AddDocumentEmployee(); }); }

        private RelayCommand _deleteDocumentEmployeeCommand;
        public RelayCommand DeleteDocumentEmployeeCommand => _deleteDocumentEmployeeCommand ??= new RelayCommand(DeleteDocumentEmployee);

        private RelayCommand _deleteALLDocumentEmployeeCommand;
        public RelayCommand DeleteALLDocumentEmployeeCommand { get => _deleteALLDocumentEmployeeCommand ??= new(obj => { DeleteALLDocumentEmployee(); }); }

        private RelayCommand _backToProfileEmployeeCommand;
        public RelayCommand BackToProfileEmployeeCommand { get => _backToProfileEmployeeCommand ??= new(obj => { BackToProfileEmployee(); }); }

        private RelayCommand _selectedImageCommand;
        public RelayCommand SelectedImageCommand { get => _selectedImageCommand ??= new(obj => { SelectedImage(); }); }

        private RelayCommand _clearImageCommand;
        public RelayCommand ClearImageCommand { get => _clearImageCommand ??= new(obj => { ImageDocumentEmployees.Clear(); }); }

        private RelayCommand _deleteOneImageCommand;
        public RelayCommand DeleteOneImageCommand => _deleteOneImageCommand ??= new RelayCommand(DeleteOneImage);

        private RelayCommand _openDocumentWithUpdateCommand;
        public RelayCommand OpenDocumentWithUpdateCommand => _openDocumentWithUpdateCommand ??= new RelayCommand(OpenDocumentWithUpdate);

        private RelayCommand _openPreviewDocumentWithUpdateCommand;
        public RelayCommand OpenPreviewDocumentWithUpdateCommand => _openPreviewDocumentWithUpdateCommand ??= new RelayCommand(OpenPreviewDocumentWithUpdate);

        private RelayCommand _openDocumentNewWindowCommand;
        public RelayCommand OpenDocumentNewWindowCommand => _openDocumentNewWindowCommand ??= new RelayCommand(OpenDocumentNewWindow);

        #endregion

        // ---------------------------------------------------------------------------------------------------------------------------------------------

        #region Методы

        private void AddDocumentEmployee()
        {
            Action action = _currentUserRole switch
            {
                Employee => async () =>
                {
                    DateOnly date;
                    try
                    {
                        date = new DateOnly(DateOfIssueDP.Value.Year, DateOfIssueDP.Value.Month, DateOfIssueDP.Value.Day);
                    }
                    catch (Exception)
                    {
                        date = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    }

                    if (SelectedTypeDocument == null)
                    {
                        _dialogService.ShowMessage("Вы не выбрали «Тип документа»!!");
                        return;
                    }

                    foreach (var item in ImageDocumentEmployees)
                    {
                        Document document = new Document()
                        {
                            IdEmployee = CurrentUser.IdEmployee,
                            IdTypeDocument = SelectedTypeDocument.IdTypeDocument,
                            Number = NumberDocumentTB,
                            PlaceOfIssue = PlaceOfIssueTB,
                            DateOfIssue = date,
                            DocumentImage = item.Image,
                            DocumentFile = item.NameImage,
                        };

                        await _dataBaseService.AddAsync(document);
                    }

                    GetDocument();
                    ClearInputData();
                },

                Patient => async () =>
                {
                    DateOnly date;
                    try
                    {
                        date = new DateOnly(DateOfIssueDP.Value.Year, DateOfIssueDP.Value.Month, DateOfIssueDP.Value.Day);
                    }
                    catch (Exception)
                    {
                        date = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    }

                    if (SelectedTypeDocument == null)
                    {
                        _dialogService.ShowMessage("Вы не выбрали «Тип документа»!!");
                        return;
                    }

                    foreach (var item in ImageDocumentEmployees)
                    {
                        DocumentPatient docPatient = new DocumentPatient()
                        {
                            IdPatient = UserPatient.IdPatient,
                            IdTypeDocument = SelectedTypeDocument.IdTypeDocument,
                            Number = NumberDocumentTB,
                            PlaceOfIssue = PlaceOfIssueTB,
                            DateOfIssue = date.ToString(),
                            DocumentImage = item.Image,
                            DocumentFile = item.NameImage,
                        };

                        await _dataBaseService.AddAsync(docPatient);
                    }

                    GetDocument();
                    ClearInputData();
                },

                _ => () => throw new Exception()
            };
            action?.Invoke();
        }

        private void DeleteDocumentEmployee(object parameter)
        {
            if (parameter != null)
            {
                Action action = _currentUserRole switch
                {
                    Employee => async () =>
                    {
                        if (parameter is Document document)
                        {
                            if (_dialogService.ShowMessageButton($"Вы действительно хотите удалить документ «{document.IdTypeDocumentNavigation.TypeDocumentName}»\n" +
                                   $"у сотрудника «{document.IdEmployeeNavigation.SecondName} {document.IdEmployeeNavigation.FirstName} {document.IdEmployeeNavigation.LastName}»!",
                                   "Предупреждение!!", MessageButtons.YesNo) == MessageButtons.Yes)
                            {
                                await _dataBaseService.DeleteAsync(document);
                                GetDocument();
                            }
                        }
                    },

                    Patient => async () =>
                    {
                        if (parameter is DocumentPatient docPatient)
                        {
                            if (_dialogService.ShowMessageButton($"Вы действительно хотите удалить документ «{docPatient.IdTypeDocumentNavigation.TypeDocumentName}»\n" +
                                   $"у пациента «{docPatient.IdPatientNavigation.SecondName} {docPatient.IdPatientNavigation.FirstName} {docPatient.IdPatientNavigation.LastName}»!",
                                   "Предупреждение!!", MessageButtons.YesNo) == MessageButtons.Yes)
                            {
                                await _dataBaseService.DeleteAsync(docPatient);
                                GetDocument();
                            }
                        }
                    },

                    _ => () => throw new Exception()
                };
                action?.Invoke();
            }
        }

        private void DeleteALLDocumentEmployee()
        {
            Action action = _currentUserRole switch
            {
                Employee => async() =>
                {
                    if (SelectedFilesEmployee.Count > 1)
                    {
                        if (_dialogService.ShowMessageButton($"Вы действительно хотите удалить выбранные элементы «{SelectedFilesEmployee.Count}»!!", "Предупреждение!!", MessageButtons.YesNo) == MessageButtons.Yes)
                        {
                            foreach (var item in SelectedFilesEmployee)
                            {
                                await _dataBaseService.DeleteAsync(item);
                            }
                            GetDocument();
                        }
                    }
                    else
                    {
                        _dialogService.ShowMessage("Вы не выбрали ни одного элемента!!");
                    }
                },

                Patient => async () =>
                {
                    if (SelectedFilesPatient.Count > 1)
                    {
                        if (_dialogService.ShowMessageButton($"Вы действительно хотите удалить выбранные элементы «{SelectedFilesPatient.Count}»!!", "Предупреждение!!", MessageButtons.YesNo) == MessageButtons.Yes)
                        {
                            foreach (var item in SelectedFilesPatient)
                            {
                                await _dataBaseService.DeleteAsync(item);
                            }
                            GetDocument();
                        }
                    }
                    else
                    {
                        _dialogService.ShowMessage("Вы не выбрали ни одного элемента!!");
                    }
                },

                _ => () => throw new Exception("Что-то там не найдено")
            };
            action?.Invoke();
        }

        private void BackToProfileEmployee()
        {
            Action action = _currentUserRole switch
            {
                Employee => () => _navigationPage.NavigateTo("ProfileEmployee", CurrentUser),

                Patient => () => _navigationPage.NavigateTo("ProfileEmployee", UserPatient),

                _ => () => throw new Exception("Такого типа нет!!")
            };
            action?.Invoke();
        }

        private void SelectedImage()
        {
            //var files = _openFileDialogService.OpenDialog();
            //if (files == null) { return; }

            //if (files.Length != 0)
            //{
            //    foreach (var item in files)
            //    {
            //        byte[] imageByte;
            //        FileInfo fileInfo = new FileInfo(item);

            //        using (Image image = Image.FromFile(item))
            //        {
            //            imageByte = FileHelper.ImageToBytes(image);
            //        }

            //        ImageDocumentEmployee imageDocumentEmployee = new ImageDocumentEmployee();

            //        imageDocumentEmployee.Image = imageByte;
            //        imageDocumentEmployee.NameImage = fileInfo.Name;

            //        ImageDocumentEmployees.Add(imageDocumentEmployee);
            //    }
            //}
        }

        private void DeleteOneImage(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is ImageDocumentEmployee imageDocumentEmployee)
                {
                    ImageDocumentEmployees.Remove(imageDocumentEmployee);
                }
            }
        }

        private void OpenDocumentWithUpdate(object parameter)
        {
            if (parameter != null)
            {
                Action action = _currentUserRole switch
                {
                    Employee => () =>
                    {
                        if (parameter is Document imageDoc)
                        {
                            List<object> obj = [imageDoc, CurrentUser];
                            _navigationWindow.NavigateTo("PreviewDocumentWithUpdate", obj);
                        }
                    },

                    Patient => () =>
                    {
                        if (parameter is DocumentPatient imageDoc)
                        {
                            List<object> obj = [imageDoc, UserPatient];
                            _navigationWindow.NavigateTo("PreviewDocumentWithUpdate", obj);
                        }
                    },

                    _ => () => throw new Exception("Такого типа нет!!")
                };
                action?.Invoke();
            }
        }

        private void OpenPreviewDocumentWithUpdate(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is ImageDocumentEmployee image)
                {
                    _navigationWindow.NavigateTo("PreviewDocumentWithUpdate", image);
                }
            }
        }

        private void OpenDocumentNewWindow(object parameter)
        {
            if (parameter != null)
            {
                Action action = _currentUserRole switch
                {
                    Employee => () =>
                    {
                        if (parameter is Document imageDoc)
                        {
                            List<object> obj = [imageDoc, CurrentUser.Avatar];
                            _navigationWindow.NavigateTo("PreviewDocumentNewWindow", obj);
                        }
                    },

                    Patient => () =>
                    {
                        if (parameter is DocumentPatient imageDoc)
                        {
                            _navigationWindow.NavigateTo("PreviewDocumentNewWindow", imageDoc);
                        }
                    },

                    _ => () => throw new Exception("Такого типа нет!!")
                };
                action?.Invoke();
            }
        }

        private void GetDocument(/*object user*/)
        {
            Action action = _currentUserRole switch
            {
                Employee => async () =>
                {
                    Documents.Clear();

                    var document =
                        await _dataBaseService.GetDataTableAsync<Document>(x => x
                            .Where(x => x.IdEmployee == CurrentUser.IdEmployee)
                                .Include(x => x.IdTypeDocumentNavigation)
                                .Include(x => x.IdEmployeeNavigation)
                                    .OrderBy(x => x.IdTypeDocument));

                    foreach (var item in document)
                    {
                        Documents.Add(item);
                    }
                },

                Patient => async () =>
                {
                    DocumentsPatient.Clear();

                    var docPatient =
                        await _dataBaseService.GetDataTableAsync<DocumentPatient>(x => x
                            .Where(x => x.IdPatient == UserPatient.IdPatient)
                                .Include(x => x.IdTypeDocumentNavigation)
                                .Include(x => x.IdPatientNavigation)
                                    .OrderBy(x => x.IdTypeDocument));

                    foreach (var item in docPatient)
                    {
                        DocumentsPatient.Add(item);
                    }
                },

                _ => () => throw new Exception("Нет такого типа!!")
            };
            action?.Invoke();


            #region Первый вариант

            //if (user is Employee employee)
            //{
            //    Documents.Clear();

            //    var document = 
            //        await _dataBaseService.GetDataTableAsync<Document>(x => x
            //            .Where(x => x.IdEmployee == employee.IdEmployee)
            //                .Include(x => x.IdTypeDocumentNavigation)
            //                .Include(x => x.IdEmployeeNavigation)
            //                    .OrderBy(x => x.IdTypeDocument));

            //    foreach (var item in document)
            //    {
            //        Documents.Add(item);
            //    }
            //}

            //if (user is Patient patient)
            //{
            //    DocumentsPatient.Clear();

            //    var docPatient = 
            //        await _dataBaseService.GetDataTableAsync<DocumentPatient>(x => x
            //            .Where(x => x.IdPatient == patient.IdPatient)
            //                .Include(x => x.IdTypeDocumentNavigation)
            //                .Include(x => x.IdPatientNavigation)
            //                    .OrderBy(x => x.IdTypeDocument));

            //    foreach (var item in docPatient)
            //    {
            //        DocumentsPatient.Add(item);
            //    }
            //}

            #endregion
        }

        private void ClearInputData()
        {
            SelectedTypeDocument = null;
            NumberDocumentTB = string.Empty;
            PlaceOfIssueTB = string.Empty;
            DateOfIssueDP = DateTime.Now;
            SelectImage = null;
            ImageDocumentEmployees.Clear();
        }

        private async void GetTypeDocument()
        {
            TypeDocuments.Clear();

            var typeDocument = await _dataBaseService.GetDataTableAsync<TypeDocument>();

            foreach (var item in typeDocument)
            {
                TypeDocuments.Add(item);
            }
        }

        private void SearchDocumentEmployeeAsync()
        {
            //var search = 
            //    await 
            //        _dataBaseService.GetDataTableAsync<Document>(x => x
            //            .Include(x => x.IdTypeDocumentNavigation)
            //                .Where(x => x.IdTypeDocumentNavigation.TypeDocumentName.Contains(SearchDocumentEmployeeTB) ||
            //                            x.Number.Contains(SearchDocumentEmployeeTB) ||
            //                            x.PlaceOfIssue.ToLower().Contains(SearchDocumentEmployeeTB.ToLower()) ||
            //                            x.DateOfIssue.ToString().Contains(SearchDocumentEmployeeTB.ToString()) ||
            //                            x.DocumentFile.ToLower().Contains(SearchDocumentEmployeeTB)));

            Action action = _currentUserRole switch
            {
                Employee => async () =>
                {
                    var search =
                            await
                                _dataBaseService.GetDataTableAsync<Document>(x => x
                                    .Include(x => x.IdTypeDocumentNavigation)
                                        .Where(x => x.IdTypeDocumentNavigation.TypeDocumentName.Contains(SearchDocumentEmployeeTB) ||
                                                    x.DocumentFile.ToLower().Contains(SearchDocumentEmployeeTB)));

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Documents.Clear();

                        foreach (var item in search)
                        {
                            Documents.Add(item);
                        }
                    });
                },

                Patient => async () =>
                {
                    var search =
                            await
                                _dataBaseService.GetDataTableAsync<DocumentPatient>(x => x
                                    .Include(x => x.IdTypeDocumentNavigation)
                                        .Where(x => x.IdTypeDocumentNavigation.TypeDocumentName.Contains(SearchDocumentEmployeeTB) ||
                                                    x.DocumentFile.ToLower().Contains(SearchDocumentEmployeeTB)));

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        DocumentsPatient.Clear();

                        foreach (var item in search)
                        {
                            DocumentsPatient.Add(item);
                        }
                    });
                },

                _ => () => throw new Exception("Нет такого типа!!")
            };
            action?.Invoke();
        }

        #endregion
    }
}
