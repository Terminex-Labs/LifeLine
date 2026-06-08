using LifeLine.Employee.Service.Domain.ValueObjects.Contracts;
using LifeLine.Employee.Service.Domain.ValueObjects.Employees;
using LifeLine.Employee.Service.Domain.ValueObjects.EmployeeType;
using Shared.Kernel.Primitives;

namespace LifeLine.Employee.Service.Domain.Models
{
    public sealed class Contract : Entity<ContractId>
    {
        public EmployeeId EmployeeId { get; private set; }
        public EmployeeTypeId EmployeeTypeId { get; private set; }
        public ContractNumber ContractNumber { get; private set; } = null!;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public Salary Salary { get; private set; }
        public FileUrl? FileKey { get; private set; }

        public Employee Employee { get; private set; } = null!;

        private Contract() { }
        private Contract
            (
                ContractId id, 
                EmployeeId employeeId, 
                EmployeeTypeId employeeTypeId, 
                ContractNumber contractNumber, 
                DateTime startDate, 
                DateTime endDate, 
                Salary salary,
                FileUrl? fileKey
            ) : base(id)
        {
            EmployeeId = employeeId;
            EmployeeTypeId = employeeTypeId;
            ContractNumber = contractNumber;
            StartDate = startDate.ToUniversalTime();
            EndDate = endDate.ToUniversalTime();
            Salary = salary;
            FileKey = fileKey;
        }

        public static Contract Create
            (
                Guid employeeId, 
                Guid employeeTypeId, 
                string contractNumber, 
                DateTime startDate, 
                DateTime endDate, 
                decimal salary,
                string? bucketName,
                string? fileName
            ) 
            => new Contract
                (
                    ContractId.New(), 
                    EmployeeId.Create(employeeId), 
                    EmployeeTypeId.Create(employeeTypeId), 
                    ContractNumber.Create(contractNumber), 
                    startDate.ToUniversalTime(), 
                    endDate.ToUniversalTime(), 
                    Salary.FromRubles(salary),
                    bucketName != null && fileName != null ? FileUrl.Create(bucketName, fileName).Value : null
                );

        internal void UpdateEmployeeType(EmployeeTypeId employeeTypeId)
        {
            if (employeeTypeId != EmployeeTypeId)
                EmployeeTypeId = employeeTypeId;
        }

        internal void UpdateNumber(ContractNumber contractNumber)
        {
            if (contractNumber != ContractNumber)
                ContractNumber = contractNumber;
        }

        internal void UpdateStartDate(DateTime startDate)
        {
            if (startDate.ToUniversalTime() != StartDate)
                StartDate = startDate.ToUniversalTime();
        }

        internal void UpdateEndDate(DateTime endDate)
        {
            if (endDate.ToUniversalTime() != EndDate)
                EndDate = endDate.ToUniversalTime();
        }

        internal void UpdateSalary(Salary salary)
        {
            if (salary != Salary)
                Salary = salary;
        }

        internal void UpdateFileKey(FileUrl? fileUrl)
        {
            if (fileUrl != FileKey)
                FileKey = fileUrl;
        }
    }
}
