using LifeLine.MVVM.Models.AppModel;
using LifeLine.MVVM.Models.MSSQL_DB;
using LifeLine.Services.DataBaseServices;
using LifeLine.Services.DialogServices;
using LifeLine.Services.NavigationPages;
using LifeLine.Utils.Enum;
using LifeLine.Utils.Helper;
using MasterAnalyticsDeadByDaylight.Command;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;

namespace LifeLine.MVVM.ViewModel
{
    class PreviewDocumentVM : BaseViewModel, IUpdatableWindow
    {
        #region Трансформация картинки

        private double _scaleX = 1.0;
        public double ScaleX
        {
            get => _scaleX;
            set
            {
                _scaleX = value;
                OnPropertyChanged();
            }
        }

        private double _scaleY = 1.0;
        public double ScaleY
        {
            get => _scaleY;
            set
            {
                _scaleY = value;
                OnPropertyChanged();
            }
        }

        private double _translateX = 0.0;
        public double TranslateX
        {
            get => _translateX;
            set
            {
                _translateX = value;
                OnPropertyChanged();
            }
        }

        private double _translateY = 0.0;
        public double TranslateY
        {
            get => _translateY;
            set
            {
                _translateY = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _resetTransformCommand;
        public RelayCommand ResetTransformCommand { get => _resetTransformCommand ??= new(obj => { ResetTransform(); }); }

        private void ResetTransform()
        {
            ScaleX = 1.0;
            ScaleY = 1.0;

            TranslateX = 0.0;
            TranslateY = 0.0;
        }

        #endregion

        private readonly IServiceProvider _serviceProvider;

        private readonly IDialogService _dialogService;
        private readonly IDataBaseService _dataBaseService;
        private readonly INavigationPage _navigationPage;
        private readonly IOpenFileDialogService _openFileDialogService;

        public PreviewDocumentVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _dialogService = _serviceProvider.GetService<IDialogService>();
            _dataBaseService = _serviceProvider.GetService<IDataBaseService>();
            _navigationPage = _serviceProvider.GetService<INavigationPage>();
            _openFileDialogService = _serviceProvider.GetService<IOpenFileDialogService>();
        }

        public void Update(object value)
        {
            if (value is List<object> obj)
            {
                if (obj[0] is Document document && obj[1] is Employee employee)
                {
                    VisibilityVisibleManager();
                    VisibilityAvatarTypeDocVisibleManager();

                    UserEmployee = employee;
                    _currentUserRole = employee;

                    Documents = document;
                    AvatarEmp = UserEmployee.Avatar; /*(byte[])obj[1];*/
                }

                if (obj[0] is DocumentPatient documentPatient && obj[1] is Patient patient)
                {
                    VisibilityVisibleManager();
                    VisibilityAvatarTypeDocCollapsedManager();

                    UserPatient = patient;
                    _currentUserRole = patient;

                    DocumentsPatient = documentPatient;
                    DocImage = documentPatient.DocumentImage;
                }
            }

            if (value is ImageDocumentEmployee image)
            {
                VisibilityCollapsedManager();
                VisibilityAvatarTypeDocVisibleManager();

                DocImage = image.Image;
            }

            //if (value is DocumentPatient docPatient)
            //{
            //    VisibilityVisibleManager();
            //    VisibilityAvatarTypeDocCollapsedManager();

            //    DocumentsPatient = docPatient;
            //    DocImage = docPatient.DocumentImage;
            //}
        }

        #region Visibility + MaxHeight

        private Visibility _avatarVisibility;
        public Visibility AvatarVisibility
        {
            get => _avatarVisibility;
            set
            {
                _avatarVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _typeDocEmployeeTBVisibility;
        public Visibility TypeDocEmployeeTBVisibility
        {
            get => _typeDocEmployeeTBVisibility;
            set
            {
                _typeDocEmployeeTBVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _typeDocPatientTBVisibility;
        public Visibility TypeDocPatientTBVisibility
        {
            get => _typeDocPatientTBVisibility;
            set
            {
                _typeDocPatientTBVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _infoDocumentBorderVisibility;
        public Visibility InfoDocumentBorderVisibility
        {
            get => _infoDocumentBorderVisibility;
            set
            {
                _infoDocumentBorderVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _saveDeleteButtonBorderVisibility;
        public Visibility SaveDeleteButtonBorderVisibility
        {
            get => _saveDeleteButtonBorderVisibility;
            set
            {
                _saveDeleteButtonBorderVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _saveChangeButtonBorderVisibility;
        public Visibility SaveChangeButtonBorderVisibility
        {
            get => _saveChangeButtonBorderVisibility;
            set
            {
                _saveChangeButtonBorderVisibility = value;
                OnPropertyChanged();
            }
        }

        private int _maxHeightImage;
        public int MaxHeightImage
        {
            get => _maxHeightImage;
            set
            {
                _maxHeightImage = value;
                OnPropertyChanged();
            }
        }

        private void VisibilityCollapsedManager()
        {
            InfoDocumentBorderVisibility = Visibility.Collapsed;
            SaveDeleteButtonBorderVisibility = Visibility.Collapsed;
            SaveChangeButtonBorderVisibility = Visibility.Collapsed;
            MaxHeightImage = 950;
        }

        private void VisibilityVisibleManager()
        {
            InfoDocumentBorderVisibility = Visibility.Visible;
            SaveDeleteButtonBorderVisibility = Visibility.Visible;
            SaveChangeButtonBorderVisibility = Visibility.Visible;
            MaxHeightImage = 675;
        }

        private void VisibilityAvatarTypeDocVisibleManager()
        {
            AvatarVisibility = Visibility.Visible;
            TypeDocEmployeeTBVisibility = Visibility.Visible;
        }

        private void VisibilityAvatarTypeDocCollapsedManager()
        {
            AvatarVisibility = Visibility.Collapsed;
            TypeDocEmployeeTBVisibility = Visibility.Collapsed;
            TypeDocPatientTBVisibility = Visibility.Visible;
        }

        #endregion

        #region Свойства

        private object _currentUserRole = null;

        private Employee _userEmployee;
        public Employee UserEmployee
        {
            get => _userEmployee;
            set
            {
                _userEmployee = value;
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

        private Document _documents;
        public Document Documents
        {
            get => _documents;
            set
            {
                _documents = value;

                NumberDocumentTB = value.Number;
                PlaceOfIssueTB = value.PlaceOfIssue;

                DateOnly? nullableDateOnly = new DateOnly(value.DateOfIssue.Value.Year, value.DateOfIssue.Value.Month, value.DateOfIssue.Value.Day);
                // Проверка на наличие значения
                DateTime? nullableDateTime = nullableDateOnly.HasValue
                    ? nullableDateOnly.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null;
                DateOfIssueDP = nullableDateTime;

                DocImage = value.DocumentImage;
                DocFile = value.DocumentFile;

                OnPropertyChanged();
            }
        }

        private DocumentPatient _documentsPatient;
        public DocumentPatient DocumentsPatient
        {
            get => _documentsPatient;
            set
            {
                _documentsPatient = value;

                NumberDocumentTB = value.Number;
                PlaceOfIssueTB = value.PlaceOfIssue;

                //DateOnly? nullableDateOnly = new DateOnly(value.DateOfIssue, value.DateOfIssue.Value.Month, value.DateOfIssue.Value.Day);
                // Проверка на наличие значения
                //DateTime? nullableDateTime = nullableDateOnly.HasValue
                //    ? nullableDateOnly.Value.ToDateTime(TimeOnly.MinValue)
                //    : (DateTime?)null;
                DateOfIssueDP = DateTime.Parse(value.DateOfIssue);

                //DocImage = value.DocumentImage;
                DocFile = value.DocumentFile;

                OnPropertyChanged();
            }
        }

        private byte[] _avatarEmp;
        public byte[] AvatarEmp
        {
            get => _avatarEmp;
            set
            {
                _avatarEmp = value;
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

        private byte[] _docImage;
        public byte[] DocImage
        {
            get => _docImage;
            set
            {
                _docImage = value;
                OnPropertyChanged();
            }
        }

        private string _docFile;
        public string DocFile
        {
            get => _docFile;
            set
            {
                _docFile = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Команды

        private RelayCommand _updateDocumentEmployeeCommand;
        public RelayCommand UpdateDocumentEmployeeCommand { get => _updateDocumentEmployeeCommand ??= new(obj => { UpdateDocumentEmployee(); }); }

        private RelayCommand _selectedImageCommand;
        public RelayCommand SelectedImageCommand { get => _selectedImageCommand ??= new(obj => { SelectedImage(); }); }

        #endregion

        #region Методы

        private void UpdateDocumentEmployee()
        {
            Action action = _currentUserRole switch
            {
                Employee => async() =>
                {
                    DateOnly dateOnly = new DateOnly(DateOfIssueDP.Value.Year, DateOfIssueDP.Value.Month, DateOfIssueDP.Value.Day);

                    var doc = await _dataBaseService.FindIdAsync<Document>(Documents.IdDocument);

                    if (doc != null)
                    {
                        bool exists = await _dataBaseService.ExistsAsync<Document>(x => x.IdDocument == doc.IdDocument);

                        if (exists)
                        {
                            if (_dialogService.ShowMessageButton($"Вы точно хотите изменить «{doc.DocumentFile}»!!", "Предупреждение!!", MessageButtons.YesNo) == MessageButtons.Yes)
                            {
                                doc.Number = NumberDocumentTB;
                                doc.PlaceOfIssue = PlaceOfIssueTB;
                                doc.DateOfIssue = dateOnly;
                                doc.DocumentImage = DocImage;
                                doc.DocumentFile = DocFile;

                                await _dataBaseService.UpdateAsync(doc);
                                _navigationPage.NavigateTo("ProfileAddDocumentEmployee", true);
                            }
                        }
                        else
                        {
                            doc.Number = NumberDocumentTB;
                            doc.PlaceOfIssue = PlaceOfIssueTB;
                            doc.DateOfIssue = dateOnly;
                            doc.DocumentImage = DocImage;
                            doc.DocumentFile = DocFile;

                            await _dataBaseService.UpdateAsync(doc);
                            _navigationPage.NavigateTo("ProfileAddDocumentEmployee", true);
                        }
                    }
                },

                Patient => async() =>
                {
                    DateOnly dateOnly = new DateOnly(DateOfIssueDP.Value.Year, DateOfIssueDP.Value.Month, DateOfIssueDP.Value.Day);

                    var doc = await _dataBaseService.FindIdAsync<DocumentPatient>(DocumentsPatient.IdDocumentPatient);

                    if (doc != null)
                    {
                        bool exists = await _dataBaseService.ExistsAsync<DocumentPatient>(x => x.IdDocumentPatient == doc.IdDocumentPatient);

                        if (exists)
                        {
                            if (_dialogService.ShowMessageButton($"Вы точно хотите изменить «{doc.DocumentFile}»!!", "Предупреждение!!", MessageButtons.YesNo) == MessageButtons.Yes)
                            {
                                doc.Number = NumberDocumentTB;
                                doc.PlaceOfIssue = PlaceOfIssueTB;
                                doc.DateOfIssue = dateOnly.ToString();
                                doc.DocumentImage = DocImage;
                                doc.DocumentFile = DocFile;

                                await _dataBaseService.UpdateAsync(doc);
                                _navigationPage.NavigateTo("ProfileAddDocumentEmployee", true);
                            }
                        }
                        else
                        {
                            doc.Number = NumberDocumentTB;
                            doc.PlaceOfIssue = PlaceOfIssueTB;
                            doc.DateOfIssue = dateOnly.ToString();
                            doc.DocumentImage = DocImage;
                            doc.DocumentFile = DocFile;

                            await _dataBaseService.UpdateAsync(doc);
                            _navigationPage.NavigateTo("ProfileAddDocumentEmployee", true);
                        }
                    }
                },

                _ => () => throw new Exception("Нет такого типа!!")
            };
            action?.Invoke();
        }

        private void SelectedImage()
        {
            //var file = _openFileDialogService.OpenDialog();

            //foreach (var item in file)
            //{
            //    byte[] imageByte;
            //    FileInfo fileInfo = new FileInfo(item);

            //    using (Image image = Image.FromFile(item))
            //    {
            //        imageByte = FileHelper.ImageToBytes(image);
            //    }

            //    DocImage = imageByte;
            //    DocFile = fileInfo.Name;
            //}
        }

        #endregion
    }
}
