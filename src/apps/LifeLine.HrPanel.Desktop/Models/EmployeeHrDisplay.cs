using Shared.Contracts.Response.DirectoryService;
using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.ViewModels.Abstract;
using System.Windows.Media;

namespace LifeLine.HrPanel.Desktop.Models
{
    public sealed class EmployeeHrDisplay : BaseViewModel
    {
        public EmployeeHrDisplay
            (
                EmployeeHrItemResponse model,
                IReadOnlyCollection<DepartmentDisplay> departments,
                IReadOnlyCollection<PositionDisplay> positions,
                IReadOnlyCollection<StatusDisplay> statuses
            )
        {
            _model = model;

            Surname = _model.Surname;
            Name = _model.Name;
            Patronymic = _model.Patronymic;

            _departments = departments;
            _positions = positions;
            _statuses = statuses;
        }

        private readonly IReadOnlyCollection<DepartmentDisplay> _departments;
        private readonly IReadOnlyCollection<PositionDisplay> _positions;
        private readonly IReadOnlyCollection<StatusDisplay> _statuses;

        private readonly EmployeeHrItemResponse _model;

        public string Id => _model.Id;
        public string DepartmentId => _model.Assignments.FirstOrDefault()!.DepartmentId;
        public string PositionId => _model.Assignments.FirstOrDefault()!.PositionId;
        public string StatusId => _model.Assignments.FirstOrDefault()!.StatusId;

        private ImageSource? _personalPhoto;
        public ImageSource? PersonalPhoto
        {
            get => _personalPhoto;
            set => SetProperty(ref _personalPhoto, value);
        }

        public void SetImage(ImageSource? image) => PersonalPhoto = image;

        public string? PersonalPhotoUrlDB => _model.PersonalPhoto;

        private string _surname = null!;
        public string Surname
        {
            get => _surname;
            set => SetProperty(ref _surname, value);
        }

        private string _name = null!;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string? _patronymic;
        public string? Patronymic
        {
            get => _patronymic;
            set => SetProperty(ref _patronymic, value);
        }

        private string? _department;
        public string? Department
        {
            get => _department;
            set => SetProperty(ref _department, value);
        }
        public void SetDepartment(string id) => Department = _departments.FirstOrDefault(x => x.Id == id)!.Name;

        private string? _position;
        public string? Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }
        public void SetPosition(string id) => Position = _positions.FirstOrDefault(x => x.Id == id)!.Name;

        private string? _status;
        public string? Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        public void SetStatus(string id) => Status = _statuses.FirstOrDefault(x => x.Id == id)!.Name;
    }
}
