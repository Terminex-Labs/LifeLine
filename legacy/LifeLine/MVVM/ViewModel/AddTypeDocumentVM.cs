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
    class AddTypeDocumentVM : BaseViewModel, IUpdatableWindow
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IDialogService _dialogService;
        private readonly IDataBaseService _dataBaseService;

        public AddTypeDocumentVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _dialogService = _serviceProvider.GetService<IDialogService>();
            _dataBaseService = _serviceProvider.GetService<IDataBaseService>();

            GetTypeOfPersonOnComboBox();
            GetTypeDocument();

            ComboBoxTypeOfPersone = TypeOfPersoneList.First();
        }

        public void Update(object value)
        {

        }


        #region Свойства

        private string _textBoxTypeDocuments;
        public string TextBoxTypeDocuments
        {
            get => _textBoxTypeDocuments;
            set
            {
                _textBoxTypeDocuments = value;
                OnPropertyChanged();
            }
        }

        private TypeOfPersone _comboBoxTypeOfPersone;
        public TypeOfPersone ComboBoxTypeOfPersone
        {
            get => _comboBoxTypeOfPersone;
            set
            {
                _comboBoxTypeOfPersone = value;
                OnPropertyChanged();
            }
        }

        private string _searchTypeDocumentLists;
        public string SearchTypeDocumentLists
        {
            get => _searchTypeDocumentLists;
            set
            {
                _searchTypeDocumentLists = value;
                Task.Run(SearchTypeDocumentListName);
                OnPropertyChanged();
            }
        }

        private TypeDocument _selectTypeDocumentLists;
        public TypeDocument SelectTypeDocumentLists
        {
            get => _selectTypeDocumentLists;
            set
            {
                _selectTypeDocumentLists = value;

                if (value == null)
                {
                    return;
                }

                TextBoxTypeDocuments = value.TypeDocumentName;
                ComboBoxTypeOfPersone = TypeOfPersoneList.FirstOrDefault(cbtop => cbtop.IdTypeOfPersone == SelectTypeDocumentLists.IdTypeOfPersone);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TypeDocument> TypeDocumentList { get; set; } = [];

        public ObservableCollection<TypeOfPersone> TypeOfPersoneList { get; set; } = [];

        #endregion


        #region Команды

        private RelayCommand _addTypeDocumentsCommand;
        public RelayCommand AddTypeDocumentsCommand { get => _addTypeDocumentsCommand ??= new(obj => { AddTypeDocumentsAsync(); }); }

        private RelayCommand _updateTypeDocumentsCommand;
        public RelayCommand UpdateTypeDocumentsCommand { get => _updateTypeDocumentsCommand ??= new(obj => { UpdateDepartamentAsync(); }); }

        private RelayCommand _deleteDepartmentCommand;
        public RelayCommand DeleteDepartmentCommand => _deleteDepartmentCommand ??= new RelayCommand(DeleteDepartment);

        #endregion


        #region Методы

        private async void AddTypeDocumentsAsync()
        {
            using (EmployeeManagementContext context = new EmployeeManagementContext())
            {
                if (string.IsNullOrWhiteSpace(TextBoxTypeDocuments))
                {
                    _dialogService.ShowMessage("Вы не заполнили поле!!");
                    //MessageBox.Show("Вы не заполнили поле!!");
                    return;
                }
                if (await _dataBaseService.ExistsAsync<TypeDocument>(x => x.TypeDocumentName.ToLower() == TextBoxTypeDocuments.ToLower()))
                {
                    _dialogService.ShowMessage("Такое поле уже есть!!");
                    //MessageBox.Show("Такое поле уже есть!!");
                }
                else
                {
                    if (ComboBoxTypeOfPersone.IdTypeOfPersone == 0)
                    {
                        _dialogService.ShowMessage("Вы не выбрали на кого будут записываться документы!!");
                        //MessageBox.Show("Вы не выбрали на кого будут записываться документы!!");
                        return;
                    }

                    TypeDocument typeDocument = new TypeDocument()
                    {
                        TypeDocumentName = TextBoxTypeDocuments,
                        IdTypeOfPersone = ComboBoxTypeOfPersone.IdTypeOfPersone,
                    };

                    await _dataBaseService.AddAsync(typeDocument);

                    TypeDocumentList.Clear();
                    TextBoxTypeDocuments = string.Empty;

                    GetTypeDocument();
                }
            }
        }

        private async void UpdateDepartamentAsync()
        {
            using (EmployeeManagementContext context = new EmployeeManagementContext())
            {
                if (SelectTypeDocumentLists == null || ComboBoxTypeOfPersone.IdTypeOfPersone == 0) { return; }

                var updateTypeDocumentLists = await _dataBaseService.FindIdAsync<TypeDocument>(SelectTypeDocumentLists.IdTypeDocument);

                if (updateTypeDocumentLists != null)
                {
                    bool exists = await _dataBaseService.ExistsAsync<TypeDocument>(x => x.TypeDocumentName.ToLower() == TextBoxTypeDocuments.ToLower()) || string.IsNullOrWhiteSpace(TextBoxTypeDocuments);
                    
                    if (exists)
                    {
                        if (_dialogService.ShowMessageButton($"Такой {SelectTypeDocumentLists.TypeDocumentName} уже есть!!\nИли пустой!!", "Предупреждение!!!", MessageButtons.YesNo) == MessageButtons.Yes)
                        {
                            updateTypeDocumentLists.TypeDocumentName = TextBoxTypeDocuments;
                            updateTypeDocumentLists.IdTypeOfPersone = ComboBoxTypeOfPersone.IdTypeOfPersone;

                            await _dataBaseService.UpdateAsync(updateTypeDocumentLists);

                            TypeDocumentList.Clear();
                            TextBoxTypeDocuments = string.Empty;

                            GetTypeDocument();
                        }
                    }
                    else
                    {
                        updateTypeDocumentLists.TypeDocumentName = TextBoxTypeDocuments;
                        updateTypeDocumentLists.IdTypeOfPersone = ComboBoxTypeOfPersone.IdTypeOfPersone;

                        await _dataBaseService.UpdateAsync(updateTypeDocumentLists);

                        TypeDocumentList.Clear();
                        TextBoxTypeDocuments = string.Empty;

                        GetTypeDocument();
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private async void DeleteDepartment(object parametr)
        {
            using (EmployeeManagementContext context = new EmployeeManagementContext())
            {
                if (parametr != null)
                {
                    if (parametr is TypeDocument typeDocumentLists)
                    {
                       if (_dialogService.ShowMessageButton($"Вы точно хотите удалить {typeDocumentLists.TypeDocumentName}?", "Предупреждение!!!", MessageButtons.YesNo) == MessageButtons.Yes)
                        {
                            await _dataBaseService.DeleteAsync(typeDocumentLists);

                            TypeDocumentList.Clear();
                            GetTypeDocument();
                        }
                    }
                }
            }
        }

        private async void GetTypeDocument()
        {
            using (EmployeeManagementContext context = new EmployeeManagementContext())
            {
                var querySearch = await _dataBaseService.GetDataTableAsync<TypeDocument>(x => x.Include(x => x.IdTypeOfPersoneNavigation).OrderBy(x => x.IdTypeOfPersone));

                if (!string.IsNullOrWhiteSpace(SearchTypeDocumentLists))
                {
                    string searchLover = SearchTypeDocumentLists.ToLower();

                    querySearch = querySearch.Where(stdl => stdl.TypeDocumentName.ToLower().Contains(searchLover)).OrderBy(x => x.IdTypeOfPersone);
                }

                List<TypeDocument> typeDocuments = querySearch.ToList();

                foreach (var item in typeDocuments)
                {
                    TypeDocumentList.Add(item);
                }
            }
        }

        private void GetTypeOfPersonOnComboBox()
        {
            using (EmployeeManagementContext context = new EmployeeManagementContext())
            {
                var typeOfPersone = context.TypeOfPersones.ToList();

                TypeOfPersoneList.Insert(0, new TypeOfPersone { TypeOfPersoneName = "Выберите чьи документы будут добавлятся!!" });

                foreach (var item in typeOfPersone)
                {
                    TypeOfPersoneList.Add(item);
                }
            }
        }

        private async void SearchTypeDocumentListName()
        {
            using (EmployeeManagementContext context = new EmployeeManagementContext())
            {
                var searchTypeDocumentListName = await 
                    context.TypeDocuments
                        .Include(x => x.IdTypeOfPersoneNavigation)
                            .Where(stdl => stdl.TypeDocumentName.ToLower().Contains(SearchTypeDocumentLists.ToLower())).OrderBy(x => x.IdTypeOfPersone).ToListAsync();

                App.Current.Dispatcher.Invoke(() =>
                {
                    TypeDocumentList.Clear();

                    foreach (var item in searchTypeDocumentListName)
                    {
                        TypeDocumentList.Add(item);
                    }
                });
            }
        }

        #endregion
    }
}
