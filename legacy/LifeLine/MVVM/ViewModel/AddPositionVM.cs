using LifeLine.MVVM.Models.MSSQL_DB;
using LifeLine.Services.DataBaseServices;
using LifeLine.Services.DialogServices;
using LifeLine.Services.NavigationPages;
using LifeLine.Utils.Enum;
using MasterAnalyticsDeadByDaylight.Command;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace LifeLine.MVVM.ViewModel
{
    class AddPositionVM : BaseViewModel, IUpdatableWindow
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IDialogService _dialogService;
        private readonly IDataBaseService _dataBaseService;

        public AddPositionVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _dialogService = _serviceProvider.GetService<IDialogService>();
            _dataBaseService = _serviceProvider.GetService<IDataBaseService>();

            GetPositionMainData();
            GetPositionData();
            GetDepartmentData();
            GetAccessData();
        }

        public void Update(object value)
        {

        }

        #region Свойства

        private string _searchPositionTB;
        public string SearchPositionTB
        {
            get => _searchPositionTB;
            set
            {
                _searchPositionTB = value;
                Task.Run(SearchPositionAsync);
                //SearchPosition();
                OnPropertyChanged();
            }
        }

        private Department _comboBoxSelectedDepartment;
        public Department ComboBoxSelectedDepartment
        {
            get => _comboBoxSelectedDepartment;
            set
            {
                _comboBoxSelectedDepartment = value;
                GetPositionData();
                OnPropertyChanged();
            }
        }

        private PositionList _comboBoxSelectedPosition;
        public PositionList ComboBoxSelectedPosition
        {
            get => _comboBoxSelectedPosition;
            set
            {
                _comboBoxSelectedPosition = value;
                OnPropertyChanged();
            }
        }

        private AccessLevel _comboBoxSelectedAccess;
        public AccessLevel ComboBoxSelectedAccess
        {
            get => _comboBoxSelectedAccess;
            set
            {
                _comboBoxSelectedAccess = value;
                OnPropertyChanged();
            }
        }

        private Position _selectedPosition;
        public Position SelectedPosition
        {
            get => _selectedPosition;
            set
            {
                if (value == null) { return; }

                _selectedPosition = value;

                ComboBoxSelectedDepartment = DepartmentList.FirstOrDefault(x => x.IdDepartment == value.IdPositionListNavigation.IdDepartment);
                ComboBoxSelectedPosition = PositionList.FirstOrDefault(x => x.IdPositionList == value.IdPositionList);
                ComboBoxSelectedAccess = AccessList.FirstOrDefault(x => x.IdAccessLevel == value.IdAccessLevel);

                OnPropertyChanged();
            }
        }

        public ObservableCollection<Position> PositionMainList { get; set; } = [];

        public ObservableCollection<PositionList> PositionList { get; set; } = [];

        public ObservableCollection<Department> DepartmentList { get; set; } = [];

        public ObservableCollection<AccessLevel> AccessList { get; set; } = [];

        #endregion


        #region Команды

        private RelayCommand _addPositionCommand;
        public RelayCommand AddPositionCommand { get => _addPositionCommand ??= new(obj => { AddPosition(); }); }

        private RelayCommand _updatePositionCommand;
        public RelayCommand UpdatePositionCommand { get => _updatePositionCommand ??= new(obj => { UpdatePosition(); }); }

        private RelayCommand _deletePositionCommand;
        public RelayCommand DeletePositionCommand => _deletePositionCommand ??= new RelayCommand(DeletePosition);

        #endregion


        #region Методы

        private async void AddPosition()
        {
            if (ComboBoxSelectedDepartment == null) { _dialogService.ShowMessage("Вы не выбрали отдел", "Предупреждение!!!"); return; }
            if (ComboBoxSelectedPosition == null) { _dialogService.ShowMessage("Вы не выбрали должность", "Предупреждение!!!"); return; }
            if (ComboBoxSelectedAccess == null) { _dialogService.ShowMessage("Вы не выбрали уровень доступа", "Предупреждение!!!"); return; }

            Position position = new Position()
            {
                IdPositionList = ComboBoxSelectedPosition.IdPositionList,
                IdAccessLevel = ComboBoxSelectedAccess.IdAccessLevel
            };

            await _dataBaseService.AddAsync(position);

            GetPositionMainData();
        }

        private async void UpdatePosition()
        {
            if (SelectedPosition == null) { return; }

            var departmetn = await _dataBaseService.FindIdAsync<Position>(SelectedPosition.IdPosition);

            if (departmetn != null)
            {
                if (_dialogService.ShowMessageButton($"Вы точно хотите изменить: \n{SelectedPosition.IdPositionListNavigation.PositionListName}\nна\n{ComboBoxSelectedPosition.PositionListName}",
                                                     "Предупреждение!!",
                                                     MessageButtons.YesNo) == MessageButtons.Yes)
                {
                    departmetn.IdPositionList = ComboBoxSelectedPosition.IdPositionList;
                    departmetn.IdAccessLevel = ComboBoxSelectedAccess.IdAccessLevel;

                    await _dataBaseService.UpdateAsync(departmetn);

                    ComboBoxSelectedDepartment = null;
                    ComboBoxSelectedPosition = null;
                    ComboBoxSelectedAccess = null;

                    GetPositionMainData();
                }
            }
        }

        private void DeletePosition(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is Position position)
                {
                    if (_dialogService.ShowMessageButton($"Вы точно хотите удалить:" +
                                                         $"\nДолжность: {position.IdPositionListNavigation.PositionListName}",
                                                         "Предупреждение!!",
                                                         MessageButtons.YesNo) == MessageButtons.Yes)
                    {
                        _dataBaseService.DeleteAsync(position);

                        GetPositionMainData();
                    }
                }
            }
        }

        private async void GetPositionMainData()
        {
            PositionMainList.Clear();

            var querySearch = 
                await _dataBaseService.GetDataTableAsync<Position>(x => x
                    .Include(x => x.IdPositionListNavigation).ThenInclude(x => x.IdDepartmentNavigation)
                    .Include(x => x.IdAccessLevelNavigation)
                    .OrderBy(x => x.IdPositionListNavigation.IdDepartmentNavigation.DepartmentName));

            if (!string.IsNullOrWhiteSpace(SearchPositionTB))
            {
                string searchLower = SearchPositionTB.ToLower();

                querySearch =
                    querySearch
                        .Where(x => x.IdPositionListNavigation.PositionListName.ToLower().Contains(SearchPositionTB.ToLower()) ||
                                    x.IdPositionListNavigation.IdDepartmentNavigation.DepartmentName.ToLower().Contains(SearchPositionTB.ToLower()) ||
                                    x.IdPositionListNavigation.PositionListName.ToLower().Contains(SearchPositionTB.ToLower()) ||
                                    x.IdAccessLevelNavigation.AccessLevelName.ToLower().Contains(SearchPositionTB.ToLower()));
            }

            List<Position> positionMainList = querySearch.ToList();

            foreach (var item in positionMainList)
            {
                PositionMainList.Add(item);
            }
        }

        private async void GetPositionData()
        {
            PositionList.Clear();

            if (ComboBoxSelectedDepartment == null)
            {
                var positionList = await _dataBaseService.GetDataTableAsync<PositionList>(x => x.OrderBy(x => x.PositionListName));

                foreach (var item in positionList)
                {
                    PositionList.Add(item);
                }

                PositionList.FirstOrDefault();
            }
            else
            {
                var positionList = await _dataBaseService.GetDataTableAsync<PositionList>(x => x.Where(x => x.IdDepartment == ComboBoxSelectedDepartment.IdDepartment));

                foreach (var item in positionList)
                {
                    PositionList.Add(item);
                }

                PositionList.FirstOrDefault();
            }
        }

        private async void GetDepartmentData()
        {
            var departmentList = await _dataBaseService.GetDataTableAsync<Department>(x => x.OrderBy(x => x.DepartmentName));

            foreach (var item in departmentList)
            {
                DepartmentList.Add(item);
            }
        }

        private async void GetAccessData()
        {
            var accessList = await _dataBaseService.GetDataTableAsync<AccessLevel>(x => x.OrderBy(x => x.AccessLevelName));

            foreach (var item in accessList)
            {
                AccessList.Add(item);
            }
        }

        private async void SearchPositionAsync()
        {
            var searchPosition =
                    await _dataBaseService.GetDataTableAsync<Position>(x => x
                        .Include(x => x.IdPositionListNavigation).ThenInclude(x => x.IdDepartmentNavigation).Include(x => x.IdAccessLevelNavigation)
                            .Where(x => x.IdPositionListNavigation.PositionListName.ToLower().Contains(SearchPositionTB.ToLower()) ||
                                        x.IdPositionListNavigation.IdDepartmentNavigation.DepartmentName.ToLower().Contains(SearchPositionTB.ToLower()) ||
                                        x.IdPositionListNavigation.PositionListName.ToLower().Contains(SearchPositionTB.ToLower()) ||
                                        x.IdAccessLevelNavigation.AccessLevelName.ToLower().Contains(SearchPositionTB.ToLower())));

            App.Current.Dispatcher.Invoke(() =>
            {
                PositionMainList.Clear();

                foreach (var item in searchPosition)
                {
                    PositionMainList.Add(item);
                }
            });
        }

        #endregion
    }
}
