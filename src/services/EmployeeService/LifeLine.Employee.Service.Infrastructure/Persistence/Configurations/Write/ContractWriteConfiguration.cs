using LifeLine.Employee.Service.Domain.Models;
using LifeLine.Employee.Service.Domain.ValueObjects.Contracts;
using LifeLine.Employee.Service.Domain.ValueObjects.Employees;
using LifeLine.Employee.Service.Domain.ValueObjects.EmployeeType;
using LifeLine.Employee.Service.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;

namespace LifeLine.Employee.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class ContractWriteConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedNever()
                   .HasConversion(inDB => inDB.Value, outDB => ContractId.Create(outDB));

            builder.Property(x => x.EmployeeId)
                   .HasColumnName("EmployeeId")
                   .HasConversion(inDB => inDB.Value, outDB => EmployeeId.Create(outDB));

            builder.Property(x => x.EmployeeTypeId)
                   .HasColumnName("EmployeeTypeId")
                   .HasConversion(inDB => inDB.Value, outDB => EmployeeTypeId.Create(outDB));

            builder.Property(x => x.ContractNumber)
                   .HasColumnName("ContractNumber")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasConversion(inDB => inDB.Value, outDB => ContractNumber.Create(outDB));

            builder.HasIndex(x => x.ContractNumber).IsUnique();

            builder.Property(x => x.StartDate)
                   .HasColumnName("StartDate");

            builder.Property(x => x.EndDate)
                   .HasColumnName("EndDate");

            builder.Property(x => x.Salary)
                   .HasColumnName("Salary")
                   .HasConversion(inDB => inDB.Value, outDB => Salary.FromRubles(outDB));

            builder.Property(x => x.FileKey)
                   .HasColumnName("FileKey")
                   .HasConversion(outDB => outDB.HasValue ? outDB.Value.Value : null, inDB => string.IsNullOrWhiteSpace(inDB) ? null : new FileUrl(inDB));

            builder.HasOne<EmployeeType>().WithMany().HasForeignKey(contract => contract.EmployeeTypeId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
