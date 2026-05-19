using LifeLine.Employee.Service.Domain.Models;
using LifeLine.Employee.Service.Domain.ValueObjects.Employees;
using LifeLine.Employee.Service.Domain.ValueObjects.Genders;
using LifeLine.Employee.Service.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;

namespace LifeLine.Employee.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class EmployeeWriteConfiguration : IEntityTypeConfiguration<Domain.Models.Employee>
    {
        public void Configure(EntityTypeBuilder<Domain.Models.Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedNever()
                   .HasConversion(inDB => inDB.Value, outDB => EmployeeId.Create(outDB));

            builder.Property(x => x.Surname)
                   .HasColumnName("Surname")
                   .HasMaxLength(Surname.MAX_LENGTH)
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasConversion(inDB => inDB.Value, outDB => Surname.Create(outDB));

            builder.Property(x => x.Name)
                   .HasColumnName("Name")
                   .HasMaxLength(Name.MAX_LENGTH)
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasConversion(inDB => inDB.Value, outDB => Name.Create(outDB));

            builder.Property(x => x.Patronymic)
                   .HasColumnName("Patronymic")
                   .IsRequired(false)
                   .HasMaxLength(Patronymic.MAX_LENGTH)
                   .UseCollation(PostgresConstants.COLLATION_NAME)
                   .HasConversion(inDB => inDB != null ? inDB.Value : null, outDB => outDB != null ? Patronymic.Create(outDB) : null);

            builder.Property(x => x.DateEntry)
                   .HasColumnName("DateEntry");

            builder.Property(x => x.Rating)
                   .HasColumnName("Rating")
                   .HasConversion(inDB => inDB.Value, outDB => Rating.Create(outDB));

            builder.Property(x => x.PersonalPhoto)
                   .HasColumnName("PersonalPhoto")
                   .IsRequired(false)
                   .HasConversion(outDB => outDB.HasValue ? outDB.Value.Value : null, inDB => string.IsNullOrWhiteSpace(inDB) ? null : new FileUrl(inDB));
                   //.HasConversion(inDB => inDB != null ? inDB.Value : null, outDB => outDB != null ? FileUrl.Create(outDB) : null);

            builder.Property(x => x.GenderId)
                   .HasColumnName("GenderId")
                   .HasConversion(inDB => inDB.Value, outDB => GenderId.Create(outDB));

            builder.Property(x => x.IsActive)
                   .HasColumnName("IsActive")
                   .HasDefaultValue(true);

            builder.HasOne(x => x.Gender).WithMany().HasForeignKey(x => x.GenderId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ContactInformation).WithOne(x => x.Employee).HasForeignKey<ContactInformation>(x => x.EmployeeId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.WorkPermits).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(w => w.WorkPermits).HasField("_workPermits").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(e => e.EducationDocuments).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(e => e.EducationDocuments).HasField("_educationDocuments").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(e => e.EmployeeSpecialties).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(e => e.EmployeeSpecialties).HasField("_employeeSpecialties").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(e => e.PersonalDocuments).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(e => e.PersonalDocuments).HasField("_personalDocuments").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(e => e.Assignments).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(e => e.Assignments).HasField("_assignments").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(e => e.Contracts).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(e => e.Contracts).HasField("_contracts").UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
