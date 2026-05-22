namespace LifeLine.EmployeeService.Application.Abstraction.Models
{
    public sealed class EmployeeHrItemView
    {
        public Guid Id { get; set; }
        public string Surname { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Patronymic { get; set; }
        public string? PersonalPhoto { get; set; }
        public bool IsActive { get; set; }

        public List<AssignmentInfo> Assignments { get; set; } = new();
    }

    public sealed class AssignmentInfo
    {
        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public Guid StatusId { get; set; }
    }
}
