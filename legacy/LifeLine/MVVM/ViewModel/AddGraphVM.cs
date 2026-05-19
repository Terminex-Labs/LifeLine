using LifeLine.MVVM.Models.AppModel;
using LifeLine.MVVM.Models.MSSQL_DB;
using LifeLine.Services.DataBaseServices;
using LifeLine.Services.DialogServices;
using LifeLine.Services.NavigationPages;
using MasterAnalyticsDeadByDaylight.Command;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace LifeLine.MVVM.ViewModel
{
    class AddGraphVM : BaseViewModel, IUpdatableWindow
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IDialogService _dialogService;
        private readonly IDataBaseService _dataBaseService;
        private readonly INavigationPage _navigationPage;

        public AddGraphVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _dialogService = _serviceProvider.GetService<IDialogService>();
            _dataBaseService = _serviceProvider.GetService<IDataBaseService>();
            _navigationPage = _serviceProvider.GetService<INavigationPage>();

            GetDepartmentData();
            GetShiftData();
            //GetTimeTable();
            //GetEmployeeData();

            SelectedDepartment = Departments.FirstOrDefault();

            SetDate();
        }

        public void Update(object value)
        {

        }

        #region Свойства

        private Department _selectedDepartment;
        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                SelectedEmployee = null;
                GetSelectedEmployee();
                //LoadEmployeeTimeTable();
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
                FillOrNull(value);
                OnPropertyChanged();
            }
        }

        private TimeTable _selectedTImeTable;
        public TimeTable SelectedTImeTable
        {
            get => _selectedTImeTable;
            set
            {
                _selectedTImeTable = value;

                if (value != null)
                {
                    SelectedShift = Shifts.FirstOrDefault(x => x.IdShift == value.IdShift);
                    DateWork = value.Date;
                    StartTimeWork = DateTime.Parse(value.TimeStart);
                    EndTimeWork = DateTime.Parse(value.TimeEnd);
                    NoteForGraphic = value.Notes;

                    OnPropertyChanged();
                }
            }
        }

        private Shift _selectedShift;
        public Shift SelectedShift
        {
            get => _selectedShift;
            set
            {
                _selectedShift = value;
                SetShiftTime(value);
                OnPropertyChanged();
            }
        }

        private void SetShiftTime(Shift shift)
        {
            if (shift != null)
            {
                StartTimeWork = shift.IdShift switch
                {
                    1 => DateTime.Today.AddHours(8),
                    2 => DateTime.Today.AddHours(18),
                    3 => DateTime.Today.AddHours(8),
                    4 => DateTime.Today.AddHours(8),
                    _ => DateTime.Today.AddHours(0),
                };

                EndTimeWork = shift.IdShift switch
                {
                    1 => DateTime.Today.AddHours(15),
                    2 => DateTime.Today.AddHours(8),
                    3 => DateTime.Today.AddHours(20),
                    4 => DateTime.Today.AddHours(8),
                    _ => DateTime.Today.AddHours(0),
                };
            }
        }

        private DateTime? _dateWork;
        public DateTime? DateWork
        {
            get => _dateWork;
            set
            {
                _dateWork = value;
                OnPropertyChanged();
            }
        }

        private DateTime _startTimeWork;
        public DateTime StartTimeWork
        {
            get => _startTimeWork;
            set
            {
                _startTimeWork = value;
                OnPropertyChanged();
            }
        }

        private DateTime _endTimeWork;
        public DateTime EndTimeWork
        {
            get => _endTimeWork;
            set
            {
                _endTimeWork = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _selectedTime;

        public DateTime? SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (_selectedTime != value)
                {
                    _selectedTime = value;
                    OnPropertyChanged(nameof(SelectedTime));
                }
            }
        }

        private string _searchDepartmentTB;
        public string SearchDepartmentTB
        {
            get => _searchDepartmentTB;
            set
            {
                _searchDepartmentTB = value;
                SearchDepartmentAsync();
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
                SearchEmployeeAsync();
                OnPropertyChanged();
            }
        }

        private string _noteForGraphic;
        public string NoteForGraphic
        {
            get => _noteForGraphic;
            set
            {
                _noteForGraphic = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _firstDate;
        public DateTime? FirstDate
        {
            get => _firstDate;
            set
            {
                _firstDate = value;
                OnPropertyChanged();
            }
        }
        private DateTime? _secondDate;
        public DateTime? SecondDate
        {
            get => _secondDate;
            set
            {
                _secondDate = value;
                OnPropertyChanged();
            }
        }

        private bool _isPopupOpen = false;
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                _isPopupOpen = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Department> Departments { get; set; } = [];

        public ObservableCollection<Employee> Employees { get; set; } = [];

        public ObservableCollection<Shift> Shifts { get; set; } = [];

        public ObservableCollection<TimeTable> TimeTables { get; set; } = [];

        public ObservableCollection<EmployeeTimeTable> EmployeeTimeTables { get; set; } = [];

        #endregion

        //---------------------------------------------------------------------------------------------------------

        #region Команды

        private RelayCommand _saveGraphicCommand;
        public RelayCommand SaveGraphicCommand { get => _saveGraphicCommand ??= new(obj => { AddTimeTable(); }); }

        private RelayCommand _applyFilterCommand;
        public RelayCommand ApplyFilterCommand { get => _applyFilterCommand ??= new(obj => { GetTimeTable(); }); }

        private RelayCommand _openPopupFilterPeriodCommand;
        public RelayCommand OpenPopupFilterPeriodCommand { get => _openPopupFilterPeriodCommand ??= new(obj => { OpenPopupFilterPeriod(); }); }

        private RelayCommand _testCommand;
        public RelayCommand TestCommand => _testCommand ??= new RelayCommand(TestMethod);

        private void TestMethod(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is TimeTableList timeTableList)
                {
                    _dialogService.ShowMessage($"Проверка: {timeTableList.IdTimeTable}, {timeTableList.Date}");
                }
            }            
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------

        #region Методы

        private async void AddTimeTable()
        {
            if (SelectedEmployee == null)
            {
                _dialogService.ShowMessage("Не был выбран сотрудник!!");
                return;
            }
            if (SelectedShift == null)
            {
                _dialogService.ShowMessage("Не была выбрана смена!!");
                return;
            }

            TimeTable timeTable = new TimeTable()
            {
                IdEmployee = SelectedEmployee.IdEmployee,
                Date = DateWork.Value.Date,
                TimeStart = StartTimeWork.ToString("HH:mm"),
                TimeEnd = EndTimeWork.ToString("HH:mm"),
                IdShift = SelectedShift.IdShift,
                Notes = NoteForGraphic
            };

            await _dataBaseService.AddAsync(timeTable);
            _navigationPage.NavigateTo("ProfileEmployee", timeTable.IdEmployee);
            GetTimeTable();
        }

        private async void GetSelectedEmployee()
        {
            Employees.Clear();

            if (SelectedDepartment != null)
            {
                var getSelectedEmployee = await _dataBaseService.GetDataTableAsync<Employee>(x => x.Where(x => x.IdPositionNavigation.IdPositionListNavigation.IdDepartment == SelectedDepartment.IdDepartment));

                foreach (var item in getSelectedEmployee)
                {
                    Employees.Add(item);
                }
            }
        }

        private async void GetDepartmentData()
        {
            Departments.Clear();

            var departments = await _dataBaseService.GetDataTableAsync<Department>();

            foreach (var item in departments)
            {
                Departments.Add(item);
            }
        }

        private async void GetShiftData()
        {
            Shifts.Clear();

            var shifts = await _dataBaseService.GetDataTableAsync<Shift>();

            foreach (var item in shifts)
            {
                Shifts.Add(item);
            }
        }

        private async void GetTimeTable()
        {
            if (SelectedEmployee != null)
            {
                TimeTables.Clear();

                var timeTable =
                    await
                        _dataBaseService.GetDataTableAsync<TimeTable>(x => x
                            .Include(x => x.IdEmployeeNavigation)
                            .Include(x => x.IdShiftNavigation)
                                .Where(x => x.IdEmployee == SelectedEmployee.IdEmployee)
                                .Where(x => x.Date >= FirstDate && x.Date <= SecondDate));

                foreach (var item in timeTable)
                {
                    TimeTables.Add(item);
                }
            }
        }

        private void SetDate()
        {
            FirstDate = DateTime.Now;
            SecondDate = FirstDate.Value.AddDays(6);
        }

        private void FillOrNull(Employee value)
        {
            if (value != null)
            {
                GetTimeTable();
            }
            else
            {
                TimeTables.Clear();

                SelectedShift = null;
                DateWork = null;
                StartTimeWork = DateTime.MinValue;
                EndTimeWork = DateTime.MinValue;
                NoteForGraphic = string.Empty;
            }
        }

        private async void LoadEmployeeTimeTable()
        {
            EmployeeTimeTables.Clear();

            var employees = await _dataBaseService.GetDataTableAsync<Employee>(x => x.Where(x => x.IdPositionNavigation.IdPositionListNavigation.IdDepartment == SelectedDepartment.IdDepartment));

            foreach (var item in employees)
            {
                var timeTable = _dataBaseService.GetDataTable<TimeTable>(x => x.Include(x => x.IdShiftNavigation).Where(x => x.IdEmployee == item.IdEmployee));
                ObservableCollection<TimeTableList> timeTableLists = new ObservableCollection<TimeTableList>();
                ObservableCollection<ShiftIdName> shiftIdNames = new ObservableCollection<ShiftIdName>();

                foreach (var table in timeTable)
                {
                    timeTableLists.Add(new TimeTableList()
                    {
                        IdTimeTable = table.IdTimeTable,
                        Date = table.Date,
                        TimeStart = table.TimeStart,
                        TimeEnd = table.TimeEnd,
                        ShiftNames = table.IdShiftNavigation.ShiftName,
                        Notes = table.Notes
                    });
                }

                var employeeTimeTable = new EmployeeTimeTable()
                {
                    IdEmployee = item.IdEmployee,
                    Avatar = item.Avatar,
                    SecondName = item.SecondName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    TimeTableLists = timeTableLists
                };
                EmployeeTimeTables.Add(employeeTimeTable);
            }
        }

        private async void SearchEmployeeAsync()
        {
            Employees.Clear();

            var search =
                    await
                        _dataBaseService.GetDataTableAsync<Employee>(x => x
                            .Where(x => x.IdPositionNavigation.IdPositionListNavigation.IdDepartment == SelectedDepartment.IdDepartment)
                            .Where(x => x.SecondName.ToLower().Contains(SearchEmployeeTB) ||
                                x.FirstName.ToLower().Contains(SearchEmployeeTB) ||
                                x.LastName.ToLower().Contains(SearchEmployeeTB)));

            foreach (var item in search)
            {
                Employees.Add(item);
            }
        }

        private void GetEmployeeData()
        {
            Employees.Clear();

            using (EmployeeManagementContext context = new())
            {
                var employees = context.Employees.ToList();

                foreach (var item in employees)
                {
                    Employees.Add(item);
                }
            }
        }

        private async void SearchDepartmentAsync()
        {
            Departments.Clear();

            var search = await _dataBaseService.GetDataTableAsync<Department>(x => x.Where(x => x.DepartmentName.ToLower().Contains(SearchDepartmentTB)));

            foreach (var item in search)
            {
                Departments.Add(item);
            }
        }

        private void OpenPopupFilterPeriod()
        {
            IsPopupOpen = true;
        }

        #endregion
    }
}
