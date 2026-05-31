using LifeLine.Employee.Service.Domain.Models;
using LifeLine.Employee.Service.Domain.ValueObjects.Employees;
using LifeLine.Employee.Service.Domain.ValueObjects.PersonalDocuments;
using LifeLine.Employee.Service.Domain.ValueObjects.Shared;
using LifeLine.Employee.Service.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;

namespace LifeLine.Employee.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class PersonalDocumentWriteConfiguration : IEntityTypeConfiguration<PersonalDocument>
    {
        public void Configure(EntityTypeBuilder<PersonalDocument> builder)
        {
            builder.ToTable("PersonalDocuments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedNever()
                   .HasConversion(inDB => inDB.Value, outDB => PersonalDocumentId.Create(outDB));

            builder.Property(x => x.EmployeeId)
                   .HasColumnName("EmployeeId")
                   .HasConversion(inDB => inDB.Value, outDB => EmployeeId.Create(outDB));

            builder.Property(x => x.DocumentTypeId)
                   .HasColumnName("DocumentTypeId")
                   .HasConversion(inDB => inDB.Value, outDB => DocumentTypeId.Create(outDB));

            builder.Property(x => x.DocumentNumber)
                   .HasColumnName("Number")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasConversion(inDB => inDB.Value, outDB => DocumentNumber.Create(outDB));

            builder.Property(x => x.DocumentSeries)
                   .HasColumnName("Series")
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasConversion(inDB => inDB != null ? inDB.Value : null, outDB => outDB != null ? DocumentSeries.Create(outDB) : null);

            builder.Property(x => x.ImageKey)
                   .HasColumnName("FileKey")
                   .HasConversion(outDB => outDB.HasValue ? outDB.Value.Value : null, inDB => string.IsNullOrWhiteSpace(inDB) ? null : new FileUrl(inDB));
        }
    }
}
