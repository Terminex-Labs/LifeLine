using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.ViewModels.Abstract;

namespace LifeLine.HrPanel.Desktop.Models
{
    public sealed class EmployeeDetailsDisplay(EmployeeFullDetailsResponse model) : BaseViewModel
    {
        private EmployeeFullDetailsResponse _model = model;

        public string EmployeeId => _model.EmployeeId.ToString();
        public string GenderId => _model.Gender.GenderId.ToString();

        private string _surname = model.Surname;
        public string Surname
        {
            get => _surname;
            set => SetProperty(ref _surname, value);
        }

        private string _name = model.Name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string? _patronymic = model.Patronymic;
        public string? Patronymic
        {
            get => _patronymic;
            set => SetProperty(ref _patronymic, value);
        }

        private DateTime _dateEntry = model.DateEntry;
        public DateTime DateEntry
        {
            get => _dateEntry;
            set => SetProperty(ref _dateEntry, value);
        }

        private double _rating = model.Rating;
        public double Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        private string? _avatar = model.PersonalPhoto;
        public string? Avatar
        {
            get => _avatar;
            set => SetProperty(ref _avatar, value);
        }

        private string? _genderName = model.Gender.GenderName;
        public string? Gender
        {
            get => _genderName;
            set => SetProperty(ref _genderName, value);
        }

        public void RevertChanges()
        {
            Surname = _model.Surname;
            Name = _model.Name;
            Patronymic = _model.Patronymic;
            DateEntry = _model.DateEntry;
            Rating = _model.Rating;
            Avatar = _model.PersonalPhoto;
            Gender = _model.Gender.GenderName;
        }

        public void CommitChanges()
        {
            _model = _model with
            {
                Surname = Surname,
                Name = Name,
                Patronymic = Patronymic,
                DateEntry = DateEntry,
                Rating = Rating,
                PersonalPhoto = Avatar,
                Gender = new GenderDetailsResponseData(Guid.Parse(GenderId), Gender!)
            };
        }
    }
}
