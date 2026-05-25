using LifeLine.Employee.Service.Domain.ValueObjects.Employees;
using LifeLine.Employee.Service.Domain.ValueObjects.PersonalDocuments;
using LifeLine.Employee.Service.Domain.ValueObjects.Shared;
using Shared.Domain.ValueObjects;
using Shared.Kernel.Primitives;

namespace LifeLine.Employee.Service.Domain.Models
{
    public sealed class PersonalDocument : Entity<PersonalDocumentId>
    {
        public EmployeeId EmployeeId { get; private set; }
        public DocumentTypeId DocumentTypeId { get; private set; }
        public DocumentNumber DocumentNumber { get; private set; } = null!;
        public DocumentSeries? DocumentSeries { get; private set; }
        public FileUrl? ImageKey { get; private set; }

        public Employee Employee { get; private set; } = null!;

        private PersonalDocument() { }
        private PersonalDocument
            (
                PersonalDocumentId id, 
                EmployeeId employeeId,
                DocumentTypeId documentTypeId, 
                DocumentNumber documentNumber, 
                DocumentSeries? documentSeries,
                FileUrl? imageKey
            ) : base(id)
        {
            EmployeeId = employeeId;
            DocumentTypeId = documentTypeId;
            DocumentNumber = documentNumber;
            DocumentSeries = documentSeries;
            ImageKey = imageKey;
        }

        internal static PersonalDocument Create(Guid employeeId, Guid documentTypeId, string documentNumber, string? documentSeries, string? bucketName, string? fileName)
            => new PersonalDocument
                (
                    PersonalDocumentId.New(), 
                    EmployeeId.Create(employeeId),
                    DocumentTypeId.Create(documentTypeId), 
                    DocumentNumber.Create(documentNumber), 
                    documentSeries != null ? DocumentSeries.Create(documentSeries) : null,
                    bucketName != null && fileName != null ? FileUrl.Create(bucketName, fileName).Value : null
                );

        internal void UpdateDocumentType(DocumentTypeId documentTypeId)
        {
            if (documentTypeId != DocumentTypeId)
                DocumentTypeId = documentTypeId;
        }

        internal void UpdateDocumentNumber(DocumentNumber documentNumber)
        {
            if (documentNumber != DocumentNumber)
                DocumentNumber = documentNumber;
        }

        internal void UpdateDocumentSeries(DocumentSeries? documentSeries)
        {
            if (documentSeries != DocumentSeries)
                DocumentSeries = documentSeries;
        }
    }
}
