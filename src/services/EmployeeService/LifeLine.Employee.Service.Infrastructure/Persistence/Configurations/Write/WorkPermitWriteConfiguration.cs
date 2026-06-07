using LifeLine.Employee.Service.Domain.Models;
using LifeLine.Employee.Service.Domain.ValueObjects.Employees;
using LifeLine.Employee.Service.Domain.ValueObjects.Shared;
using LifeLine.Employee.Service.Domain.ValueObjects.WorkPermits;
using LifeLine.Employee.Service.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;

namespace LifeLine.Employee.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class WorkPermitWriteConfiguration : IEntityTypeConfiguration<WorkPermit>
    {
        public void Configure(EntityTypeBuilder<WorkPermit> builder)
        {
            builder.ToTable("WorkPermits");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedNever()
                   .HasConversion(inDB => inDB.Value, outDB => WorkPermitId.Create(outDB));

            builder.Property(x => x.EmployeeId)
                   .HasColumnName("EmployeeId")
                   .HasConversion(inDB => inDB.Value, outDB => EmployeeId.Create(outDB));

            builder.Property(x => x.WorkPermitName)
                   .HasColumnName("WorkPermitName")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(ProgramEducationName.MAX_LENGTH)
                   .HasConversion(inDB => inDB.Value, outDB => ProgramEducationName.Create(outDB));

            builder.Property(x => x.DocumentSeries)
                   .HasColumnName("DocumentSeries")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(DocumentSeries.MAX_LENGTH)
                   .HasConversion(inDB => inDB != null ? inDB.Value : null, outDB => outDB != null ? DocumentSeries.Create(outDB) : null);

            builder.Property(x => x.WorkPermitNumber)
                   .HasColumnName("WorkPermitNumber")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(DocumentNumber.MAX_LENGTH)
                   .HasConversion(inDB => inDB.Value, outDB => DocumentNumber.Create(outDB));

            builder.Property(x => x.ProtocolNumber)
                   .HasColumnName("ProtocolNumber")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(ProtocolNumber.MAX_LENGTH)
                   .HasConversion(inDB => inDB != null ? inDB.Value : null, outDB => outDB != null ? ProtocolNumber.Create(outDB) : null);

            builder.Property(x => x.SpecialtyName)
                   .HasColumnName("SpecialtyName")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(SpecialtyName.MAX_LENGTH)
                   .HasConversion(x => x.Value, outDB => SpecialtyName.Create(outDB));

            builder.Property(x => x.IssuingAuthority)
                   .HasColumnName("IssuingAuthority")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(IssuingAuthority.MAX_LENGTH)
                   .HasConversion(inDB => inDB.Value, outDB => IssuingAuthority.Create(outDB));

            builder.Property(x => x.IssueDate)
                   .HasColumnName("IssueDate");

            builder.Property(x => x.ExpiryDate)
                   .HasColumnName("ExpiryDate");

            builder.Property(x => x.FileKey)
                   .HasColumnName("FileKey")
                   .HasConversion(outDB => outDB.HasValue ? outDB.Value.Value : null, inDB => string.IsNullOrWhiteSpace(inDB) ? null : new FileUrl(inDB));

            builder.Property(x => x.PermitTypeId)
                   .HasColumnName("PermitTypeId")
                   .HasConversion(inDB => inDB.Value, outDB => PermitTypeId.Create(outDB));

            builder.Property(x => x.AdmissionStatusId)
                   .HasColumnName("AdmissionStatusId")
                   .HasConversion(inDB =>inDB.Value, outDB => AdmissionStatusId.Create(outDB));
        }
    }
}
