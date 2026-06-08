using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Enums;
using Shared.WPF.ViewModels.Abstract;

namespace LifeLine.HrPanel.Desktop.Models
{
    public sealed class AssignmentContractDisplay : BaseViewModel
    {
        private readonly AssignmentResponse _assignmentModel;
        private readonly ContractResponse _contractModel;

        private readonly IReadOnlyCollection<DepartmentDisplay> _departments;
        private readonly IReadOnlyCollection<PositionDisplay> _positions;
        private readonly IReadOnlyCollection<ManagerDisplay?> _managers;
        private readonly IReadOnlyCollection<StatusDisplay> _statuses;
        private readonly IReadOnlyCollection<EmployeeTypeDisplay> _employeeTypes;

        public AssignmentContractDisplay
            (
                AssignmentResponse assignmentModel,
                ContractResponse contractModel,

                IReadOnlyCollection<DepartmentDisplay> departments,
                IReadOnlyCollection<PositionDisplay> positions,
                IReadOnlyCollection<ManagerDisplay?> managers,
                IReadOnlyCollection<StatusDisplay> statuses,
                IReadOnlyCollection<EmployeeTypeDisplay> employeeTypes,
                SaveStatus saveStatus
            )
        {
            _assignmentModel = assignmentModel;
            _contractModel = contractModel;

            _departments = departments;
            _positions = positions;
            _managers = managers;
            _statuses = statuses;
            _employeeTypes = employeeTypes;

            SetDepartment(_assignmentModel.DepartmentId.ToString());
            SetPosition(_assignmentModel.PositionId.ToString());
            SetManager(_assignmentModel.ManagerId?.ToString());
            SetStatus(_assignmentModel.StatusId.ToString());

            _hireDate = assignmentModel.HireDate;
            _terminationDate = assignmentModel.TerminationDate;
            _contractNumber = contractModel.ContractNumber;
            _startDate = contractModel.ContractStartDate;
            _endDate = contractModel.ContractEndDate;
            _salary = contractModel.Salary;
            _fileKey = contractModel.ContractFileKey;
            SaveStatus = saveStatus;

            SetEmployeeType(_contractModel.EmployeeTypeId);
        }

        public SaveStatus SaveStatus
        {
            get => field;
            set => SetProperty(ref field, value);
        }
        public void SetSaveStatus(SaveStatus saveStatus) => SaveStatus = saveStatus;

        public override string ToString()
        {
            return $"{Department.Id} - {Position.Id} - {Manager?.Id} - {HireDate} - {TerminationDate} - {Status.Id} - {EmployeeType.Id} - {ContractNumber} - {StartDate} - {EndDate} - {Salary} - {FileKey}";
        }

        #region Assignment

        public string AssignmentId => _assignmentModel.AssignmentId.ToString();

        private DepartmentDisplay _department = null!;
        public DepartmentDisplay Department
        {
            get => _department;
            set => SetProperty(ref _department, value);
        }
        public void SetDepartment(string id) => Department = _departments.FirstOrDefault(x => x.Id == id)!;

        private PositionDisplay _position = null!;
        public PositionDisplay Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }
        public void SetPosition(string id) => Position = _positions.FirstOrDefault(x => x.Id == id)!;

        private ManagerDisplay? _manager;
        public ManagerDisplay? Manager
        {
            get => _manager;
            set => SetProperty(ref _manager, value);
        }
        public void SetManager(string? id) => Manager = _managers.FirstOrDefault(x => x!.Id == id)!;

        private DateTime _hireDate;
        public DateTime HireDate
        {
            get => _hireDate;
            set => SetProperty(ref _hireDate, value);
        }

        private DateTime? _terminationDate;
        public DateTime? TerminationDate
        {
            get => _terminationDate;
            set => SetProperty(ref _terminationDate, value);
        }

        private StatusDisplay _status = null!;
        public StatusDisplay Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        public void SetStatus(string id) => Status = _statuses.FirstOrDefault(x => x.Id == id)!;

        private string? _fileKey;
        public string? FileKey
        {
            get => _fileKey;
            set => SetProperty(ref _fileKey, value);
        }

        #endregion

        #region Contract

        public string ContractId => _contractModel.ContractId.ToString();

        private EmployeeTypeDisplay _employeeType = null!;
        public EmployeeTypeDisplay EmployeeType
        {
            get => _employeeType;
            set => SetProperty(ref _employeeType, value);
        }
        public void SetEmployeeType(string id) => EmployeeType = _employeeTypes.FirstOrDefault(x => x.Id == id)!;

        private string _contractNumber;
        public string ContractNumber
        {
            get => _contractNumber;
            set => SetProperty(ref _contractNumber, value);
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private decimal _salary;
        public decimal Salary
        {
            get => _salary;
            set => SetProperty(ref _salary, value);
        }

        #endregion

        public byte[]? FileBytes { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; } = "application/pdf";

        public AssignmentResponse GetUnderLineModelAssignment() => _assignmentModel;
        public ContractResponse GetUnderLineModelContract() => _contractModel;
    }
}
