using LifeLine.Employee.Service.Domain.Models;
using LifeLine.Employee.Service.Domain.ValueObjects.EducationDocuments;
using LifeLine.Employee.Service.Domain.ValueObjects.Employees;
using LifeLine.Employee.Service.Domain.ValueObjects.Shared;
using LifeLine.Employee.Service.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;

namespace LifeLine.Employee.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class EducationDocumentWriteConfiguration : IEntityTypeConfiguration<EducationDocument>
    {
        public void Configure(EntityTypeBuilder<EducationDocument> builder)
        {
            builder.ToTable("EducationDocuments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedNever()
                   .HasConversion(inDB => inDB.Value, outDB => EducationDocumentId.Create(outDB));

            builder.Property(x => x.EmployeeId)
                   .HasColumnName("EmployeeId")
                   .HasConversion(inDB => inDB.Value, outDB => EmployeeId.Create(outDB));

            builder.Property(x => x.EducationLevelId)
                   .HasColumnName("EducationLevelId")
                   .HasConversion(inDB => inDB.Value, outDB => EducationLevelId.Create(outDB));

            builder.Property(x => x.DocumentTypeId)
                   .HasColumnName("DocumentTypeId")
                   .HasConversion(inDB => inDB.Value, outDB => DocumentTypeId.Create(outDB));

            builder.Property(x => x.DocumentNumber)
                   .HasColumnName("DocumentNumber")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(DocumentNumber.MAX_LENGTH)
                   .HasConversion(inDB => inDB.Value, outDB => DocumentNumber.Create(outDB));

            builder.Property(x => x.IssuedDate)
                   .HasColumnName("IssuedDate");

            builder.Property(x => x.OrganizationName)
                   .HasColumnName("OrganizationName")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(IssuingAuthority.MAX_LENGTH)
                   .HasConversion(inDB => inDB.Value, outDB => IssuingAuthority.Create(outDB));

            builder.Property(x => x.QualificationAwardedName)
                   .HasColumnName("QualificationAwardedName")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(QualificationAwardedName.MAX_LENGTH)
                   .HasConversion(inDB => inDB != null ? inDB.Value : null, outDB => outDB != null ? QualificationAwardedName.Create(outDB) : null);

            builder.Property(x => x.SpecialtyName)
                   .HasColumnName("SpecialtyName")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(SpecialtyName.MAX_LENGTH)
                   .HasConversion(inDB => inDB != null ? inDB.Value : null, outDB => outDB != null ? SpecialtyName.Create(outDB) : null);

            builder.Property(x => x.ProgramName)
                   .HasColumnName("ProgramName")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasMaxLength(ProgramEducationName.MAX_LENGTH)
                   .HasConversion(inDB => inDB != null ? inDB.Value : null, outDB => outDB != null ? ProgramEducationName.Create(outDB) : null);

            //builder.Property(x => x.TotalHours)
            //       .HasColumnName("TotalHours")
            //       .HasConversion(inDB => inDB != null ? inDB.Value : (TimeSpan?)null, outDB => outDB != null ? Hours.Create(outDB.Value.TotalHours) : (Hours?)null);

            builder.Property(x => x.TotalHours)
                   .HasColumnName("TotalHours")
                   .HasColumnType("numeric")
                   .HasConversion(hours => hours != null ? hours.Value.Value.TotalHours : (double?)null, value => value.HasValue ? Hours.Create(value.Value) : null);

            builder.Property(x => x.FileKey)
                   .HasColumnName("FileKey")
                   .HasConversion(outDB => outDB.HasValue ? outDB.Value.Value : null, inDB => string.IsNullOrWhiteSpace(inDB) ? null : new FileUrl(inDB));
        }
    }
}
