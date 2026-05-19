using LifeLine.MVVM.Models.MSSQL_DB;
using LifeLine.Services.DataBaseServices;
using LifeLine.Services.DialogServices;
using LifeLine.Services.NavigationPages;
using LifeLine.Utils.Enum;
using LifeLine.Utils.Helper;
using MasterAnalyticsDeadByDaylight.Command;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Drawing;

namespace LifeLine.MVVM.ViewModel
{
    public class AddEmployeeVM : BaseViewModel, IUpdatableWindow
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IDialogService _dialogService;
        private readonly IDataBaseService _dataBaseService;
        private readonly INavigationPage _navigationPage;

        public AddEmployeeVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _dialogService = _serviceProvider.GetService<IDialogService>();
            _dataBaseService = _serviceProvider.GetService<IDataBaseService>();
            _navigationPage = _serviceProvider.GetService<INavigationPage>();
            
            IsCustomPopupCB = false;

            GetEmployeeData();
            GetPositionData();
            GetGenderData();
        }

        public void Update(object value)
        {

        }

        #region Popup

        private bool _isCustomPopupCB;
        public bool IsCustomPopupCB
        {
            get => _isCustomPopupCB;
            set
            {
                _isCustomPopupCB = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Свойства

        private string _textBoxSecondName;
        public string TextBoxSecondName
        {
            get => _textBoxSecondName;
            set
            {
                _textBoxSecondName = value;
                OnPropertyChanged();
            }
        }

        private string _textBoxFirstName;
        public string TextBoxFirstName
        {
            get => _textBoxFirstName;
            set
            {
                _textBoxFirstName = value;
                OnPropertyChanged();
            }
        }

        private string _textBoxLastName;
        public string TextBoxLastName
        {
            get => _textBoxLastName;
            set
            {
                _textBoxLastName = value;
                OnPropertyChanged();
            }
        }

        private DateTime _dateBirth;
        public DateTime DateBirth
        {
            get => _dateBirth;
            set
            {
                _dateBirth = value;
                OnPropertyChanged();
            }
        }

        private DateTime _dateAddition;
        public DateTime DateAddition
        {
            get => _dateAddition;
            set
            {
                _dateAddition = value;
                OnPropertyChanged();
            }
        }

        private DateTime _dateTakingOffice;
        public DateTime DateTakingOffice
        {
            get => _dateTakingOffice;
            set
            {
                _dateTakingOffice = value;
                OnPropertyChanged();
            }
        }

        private byte[] _selectImage;
        public byte[] SelectImage
        {
            get => _selectImage;
            set
            {
                _selectImage = value;
                OnPropertyChanged();
            }
        }

        private decimal? _textBoxSalary;
        public decimal? TextBoxSalary
        {
            get => _textBoxSalary;
            set
            {
                _textBoxSalary = value;
                OnPropertyChanged();
            }
        }

        private string _textBoxLogin;
        public string TextBoxLogin
        {
            get => _textBoxLogin;
            set
            {
                _textBoxLogin = value;
                OnPropertyChanged();
            }
        }

        private string _textBoxPassword;
        public string TextBoxPassword
        {
            get => _textBoxPassword;
            set
            {
                _textBoxPassword = value;
                OnPropertyChanged();
            }
        }

        private string _searchEmployeeTB;
        public string SearchEmployeeTB
        {
            get => _searchEmployeeTB;
            set
            {
                _searchEmployeeTB = value;
                // Первый вариант
                Task.Run(SearchEmployeeAsync);
                OnPropertyChanged();
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;

                if (value == null)
                {
                    return;
                }

                TextBoxSecondName = value.SecondName;
                TextBoxFirstName = value.FirstName;
                TextBoxLastName = value.LastName;

                SelectImage = value.Avatar;

                TextBoxSalary = value.Salary;

                TextBoxLogin = value.Login;
                TextBoxPassword = value.Password;

                //DepPos = $"Отдел: {value.IdPositionNavigation.IdPositionListNavigation.IdDepartmentNavigation.DepartmentName};\nДолжность: {value.IdPositionNavigation.IdPositionListNavigation.PositionListName}";

                ComboBoxSelectedPositionList = PositionList.FirstOrDefault(x => x.IdPositionList == value.IdPositionNavigation.IdPositionListNavigation.IdPositionList);
                ComboBoxSelectedGender = GenderList.FirstOrDefault(x => x.IdGender == value.IdGender);

                OnPropertyChanged();
            }
        }

        private string _depPos;
        public string DepPos
        {
            get => _depPos;
            set
            {
                _depPos = value;
                OnPropertyChanged();
            }
        }

        private Position _comboBoxSelectedPositionList;
        public Position ComboBoxSelectedPositionList
        {
            get => _comboBoxSelectedPositionList;
            set
            {
                _comboBoxSelectedPositionList = value;

                if (value == null)
                {
                    DepPos = string.Empty;
                }
                else
                {
                    DepPos = $"Отдел: {value.IdPositionListNavigation.IdDepartmentNavigation.DepartmentName};\nДолжность: {value.IdPositionListNavigation.PositionListName}";
                }
                
                IsCustomPopupCB = false;
                OnPropertyChanged();
            }
        }

        private Gender _comboBoxSelectedGender;
        public Gender ComboBoxSelectedGender
        {
            get => _comboBoxSelectedGender;
            set
            {
                _comboBoxSelectedGender = value;
                OnPropertyChanged();
            }
        }

        private const int _NUMBER_ITEM_PAGE = 4;

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        private int _totalPage;
        public int TotalPage
        {
            get => _totalPage;
            set
            {
                _totalPage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Employee> EmployeeList { get; set; } = [];
        private List<Employee> _allEmployees { get; set; } = [];

        public ObservableCollection<Position> PositionList { get; set; } = [];

        public ObservableCollection<Gender> GenderList { get; set; } = [];

        #endregion
        
        //------------------------------------------------------------------------------------------------

        #region Команды

        private RelayCommand _addEmployeeCommand;
        public RelayCommand AddEmployeeCommand { get => _addEmployeeCommand ??= new(obj => { AddEmployeeAsync(); }); }

        private RelayCommand _updateEmployeeCommand;
        public RelayCommand UpdateEmployeeCommand { get => _updateEmployeeCommand ??= new(obj => { UpdateEmployeeAsync(); }); }

        private RelayCommand _deleteEmployeeCommand;
        public RelayCommand DeleteEmployeeCommand => _deleteEmployeeCommand ??= new RelayCommand(DeleteEmployeeAsync);

        private RelayCommand _openPopupPositionDepartment;
        public RelayCommand OpenPopupPositionDepartment { get => _openPopupPositionDepartment ??= new(obj => { OpenPopupPosDep(); }); }

        private RelayCommand _selectedImageCommand;
        public RelayCommand SelectedImageCommand { get => _selectedImageCommand ??= new(obj => { SelectedImage(); }); }

        private RelayCommand _clearImageCommand;
        public RelayCommand ClearImageCommand { get => _clearImageCommand ??= new(obj => { SelectImage = null; }); }

        private RelayCommand _previousPageCommand;
        public RelayCommand PreviousPageCommand { get => _previousPageCommand ??= new(obj => { CurrentPage--; LoadEmployee(); }); }

        private RelayCommand _nextPageCommand;
        public RelayCommand NextPageCommand { get => _nextPageCommand ??= new(obj => { CurrentPage++; LoadEmployee(); }); }

        private RelayCommand _openProfileEmployeeCommand;
        public RelayCommand OpenProfileEmployeeCommand => _openProfileEmployeeCommand ??= new RelayCommand(OpenProfileEmployee);

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Методы

        private async void AddEmployeeAsync()
        {
            if (string.IsNullOrEmpty(TextBoxSecondName) && string.IsNullOrEmpty(TextBoxFirstName) && string.IsNullOrEmpty(TextBoxLastName))
            {
                _dialogService.ShowMessage("Вы не заполнили поля ФИО сотрудника!");
                return;
            }
            if (ComboBoxSelectedPositionList == null)
            {
                _dialogService.ShowMessage("Вы не выбрали должность сотрудника!");
                return;
            }
            if (TextBoxSalary == 0)
            {
                _dialogService.ShowMessage("Вы не назначили заработную плату сотрудника!");
                return;
            }
            if (ComboBoxSelectedGender == null)
            {
                _dialogService.ShowMessage("Вы не указали пол сотрудника!");
                return;
            }
            if (string.IsNullOrEmpty(TextBoxLogin) && string.IsNullOrEmpty(TextBoxPassword))
            {
                _dialogService.ShowMessage("Вы не назначили логин и пароль сотрудника!");
                return;
            }
            if (await _dataBaseService.ExistsAsync<Employee>(x => x.Login.ToLower() == TextBoxLogin.ToLower()))
            {
                _dialogService.ShowMessage("Такой логин уже занят!");
                return;
            }

            Employee employee = new()
            {
                SecondName = TextBoxSecondName,
                FirstName = TextBoxFirstName,
                LastName = TextBoxLastName,

                Avatar = SelectImage,

                Salary = TextBoxSalary,

                IdPosition = ComboBoxSelectedPositionList.IdPosition,
                IdGender = ComboBoxSelectedGender.IdGender,

                Login = TextBoxLogin,
                Password = TextBoxPassword,
            };

            await _dataBaseService.AddAsync(employee);

            ClearingDataEntry();
            GetEmployeeData();
        }

        private async void UpdateEmployeeAsync()
        {
            if (SelectedEmployee == null) { return; }

            var employeeToUpdate = await _dataBaseService.FindIdAsync<Employee>(SelectedEmployee.IdEmployee);

            if (employeeToUpdate != null)
            {
                bool exists = await _dataBaseService.ExistsAsync<Employee>(x => x.Login.ToLower() == employeeToUpdate.Login.ToLower());

                if (exists)
                {
                    if (_dialogService.ShowMessageButton($"Вы точно хотите изменить данные «{employeeToUpdate.SecondName} {employeeToUpdate.FirstName} {employeeToUpdate.LastName}»!", "Предупреждение!!!", MessageButtons.YesNo) == MessageButtons.Yes)
                    {
                        employeeToUpdate.SecondName = TextBoxSecondName;
                        employeeToUpdate.FirstName = TextBoxFirstName;
                        employeeToUpdate.LastName = TextBoxLastName;

                        employeeToUpdate.Avatar = SelectImage;

                        employeeToUpdate.IdPosition = ComboBoxSelectedPositionList.IdPosition;

                        employeeToUpdate.Salary = TextBoxSalary;

                        employeeToUpdate.IdGender = ComboBoxSelectedGender.IdGender;

                        employeeToUpdate.Login = TextBoxLogin;
                        employeeToUpdate.Password = TextBoxPassword;

                        await _dataBaseService.UpdateAsync(employeeToUpdate);

                        ClearingDataEntry();
                        GetEmployeeData();
                    }
                }
                else
                {
                    employeeToUpdate.SecondName = TextBoxSecondName;
                    employeeToUpdate.FirstName = TextBoxFirstName;
                    employeeToUpdate.LastName = TextBoxLastName;

                    employeeToUpdate.Avatar = SelectImage;

                    employeeToUpdate.IdPosition = ComboBoxSelectedPositionList.IdPosition;

                    employeeToUpdate.Salary = TextBoxSalary;

                    employeeToUpdate.IdGender = ComboBoxSelectedGender.IdGender;

                    employeeToUpdate.Login = TextBoxLogin;
                    employeeToUpdate.Password = TextBoxPassword;

                    await _dataBaseService.UpdateAsync(employeeToUpdate);

                    ClearingDataEntry();
                    GetEmployeeData();
                }
            }
        }

        private async void DeleteEmployeeAsync(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is Employee employee)
                {
                    if (_dialogService.ShowMessageButton($"Вы точно хотите удалить сотрудника " +
                            $"«{employee.SecondName} {employee.FirstName} {employee.LastName}»!",
                            "Предупреждение!!!",
                            MessageButtons.YesNo) == MessageButtons.Yes)
                    {
                        await _dataBaseService.DeleteAsync(employee);

                        ClearingDataEntry();
                        GetEmployeeData();
                    }
                }
            }
        }

        private void GetEmployeeData()
        {
            _allEmployees.Clear();

            var querySearch = _dataBaseService.GetDataTable<Employee>(x => x
                        .Include(x => x.IdPositionNavigation).ThenInclude(x => x.IdAccessLevelNavigation)
                        .Include(x => x.IdPositionNavigation).ThenInclude(x => x.IdPositionListNavigation)
                        .Include(x => x.IdPositionNavigation).ThenInclude(x => x.IdPositionListNavigation.IdDepartmentNavigation)
                        .Include(x => x.IdGenderNavigation)

                        .Include(x => x.IdPositionNavigation)
                                .ThenInclude(x => x.IdPositionListNavigation)
                                    .ThenInclude(x => x.IdDepartmentNavigation)
                            .Include(x => x.IdGenderNavigation));

            if (!string.IsNullOrWhiteSpace(SearchEmployeeTB))
            {
                string searchLover = SearchEmployeeTB.ToLower();

                querySearch = 
                    querySearch.
                        Where(x =>
                            x.SecondName.ToLower().Contains(searchLover) ||
                            x.FirstName.ToLower().Contains(searchLover) ||
                            x.LastName.ToLower().Contains(searchLover) ||
                            x.IdPositionNavigation.IdPositionListNavigation.IdDepartmentNavigation.DepartmentName.ToLower().Contains(searchLover) ||
                            x.IdPositionNavigation.IdPositionListNavigation.PositionListName.ToLower().Contains(searchLover) ||
                            x.Login.ToLower().Contains(searchLover) ||
                            x.IdGenderNavigation.GenderName.ToLower().Contains(searchLover));
            }

            List<Employee> employeeList = querySearch.ToList();

            _allEmployees.AddRange(employeeList);

            CalculateTotalPage(employeeList.Count);

            LoadEmployee();
        }

        private void CalculateTotalPage(int count)
        {
            TotalPage = (int)Math.Ceiling((double)count / _NUMBER_ITEM_PAGE);
        }

        private void LoadEmployee()
        {
            EmployeeList.Clear();

            var items = _allEmployees.Skip((CurrentPage - 1) * _NUMBER_ITEM_PAGE).Take(_NUMBER_ITEM_PAGE);

            foreach (var item in items)
            {
                EmployeeList.Add(item);
            }
        }

        private void OpenProfileEmployee(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is Employee employee)
                {
                    _navigationPage.NavigateTo("ProfileEmployee", employee);
                }
            }
        }

        private async void GetPositionData()
        {
            PositionList.Clear();

            var positionList = await _dataBaseService.GetDataTableAsync<Position>(x => x
                .Include(x => x.IdPositionListNavigation)
                .Include(x => x.IdPositionListNavigation.IdDepartmentNavigation));

            foreach (var item in positionList)
            {
                PositionList.Add(item);
            }
        }

        private async void GetGenderData()
        {
            GenderList.Clear();

            var genderList = await _dataBaseService.GetDataTableAsync<Gender>();

            foreach (var item in genderList)
            {
                GenderList.Add(item);
            }
        }

        private async void SearchEmployeeAsync()
        {
            var search =
                await _dataBaseService.GetDataTableAsync<Employee>(x => x
                        .Include(x => x.IdPositionNavigation)
                            .ThenInclude(x => x.IdPositionListNavigation)
                                .ThenInclude(x => x.IdDepartmentNavigation)
                        .Include(x => x.IdGenderNavigation)
                        .Where(x =>
                            x.SecondName.ToLower().Contains(SearchEmployeeTB.ToLower()) ||
                            x.FirstName.ToLower().Contains(SearchEmployeeTB.ToLower()) ||
                            x.LastName.ToLower().Contains(SearchEmployeeTB.ToLower()) ||
                            x.IdPositionNavigation.IdPositionListNavigation.IdDepartmentNavigation.DepartmentName.ToLower().Contains(SearchEmployeeTB.ToLower()) ||
                            x.IdPositionNavigation.IdPositionListNavigation.PositionListName.ToLower().Contains(SearchEmployeeTB.ToLower()) ||
                            x.Login.ToLower().Contains(SearchEmployeeTB.ToLower()) ||
                            x.IdGenderNavigation.GenderName.ToLower().Contains(SearchEmployeeTB.ToLower())));

            App.Current.Dispatcher.Invoke(() =>
            {
                EmployeeList.Clear();

                foreach (var item in search)
                {
                    EmployeeList.Add(item);
                }
            });
        }

        private void ClearingDataEntry()
        {
            TextBoxSecondName = string.Empty;
            TextBoxFirstName = string.Empty;
            TextBoxLastName = string.Empty;

            SelectImage = null;

            TextBoxSalary = 0;

            ComboBoxSelectedPositionList = null;
            ComboBoxSelectedGender = null;

            TextBoxLogin = string.Empty;
            TextBoxPassword = string.Empty;
        }

        private void OpenPopupPosDep()
        {
            if (IsCustomPopupCB)
            {
                IsCustomPopupCB = false;
            }
            if (!IsCustomPopupCB)
            {
                IsCustomPopupCB = true;
            }
        }

        private void SelectedImage()
        {
            //OpenFileDialog openFileDialog = new() { Filter = "Изображения (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png" };

            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    using (Image image = Image.FromFile(openFileDialog.FileName))
            //    {
            //        SelectImage = FileHelper.ImageToBytes(image);
            //    }
            //}
        }

        #endregion
    }
}
