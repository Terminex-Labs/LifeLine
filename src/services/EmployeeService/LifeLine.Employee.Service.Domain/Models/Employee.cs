using LifeLine.Employee.Service.Domain.Exceptions;
using LifeLine.Employee.Service.Domain.ValueObjects.Contracts;
using LifeLine.Employee.Service.Domain.ValueObjects.Employees;
using LifeLine.Employee.Service.Domain.ValueObjects.EmployeeType;
using LifeLine.Employee.Service.Domain.ValueObjects.Genders;
using LifeLine.Employee.Service.Domain.ValueObjects.Shared;
using LifeLine.Employee.Service.Domain.ValueObjects.WorkPermits;
using Shared.Domain.Exceptions;
using Shared.Domain.ValueObjects;
using Shared.Kernel.Guard;
using Shared.Kernel.Guard.Extensions;
using Shared.Kernel.Primitives;

namespace LifeLine.Employee.Service.Domain.Models
{
    public sealed class Employee : Aggregate<EmployeeId>
    {
        public Surname Surname { get; private set; } = null!;
        public Name Name { get; private set; } = null!;
        public Patronymic? Patronymic { get; private set; }
        public DateTime DateEntry { get; private set; }
        public Rating Rating { get; private set; }
        //public ImageKey? Avatar { get; private set; }
        public FileUrl? PersonalPhoto { get; private set; }
        public GenderId GenderId { get; private set; }
        public bool IsActive { get; private set; }

        public Gender Gender { get; private set; } = null!;
        public ContactInformation? ContactInformation { get; private set; }

        private readonly List<WorkPermit> _workPermits = [];
        public IReadOnlyCollection<WorkPermit> WorkPermits => _workPermits.AsReadOnly();

        private readonly List<EducationDocument> _educationDocuments = [];
        public IReadOnlyCollection<EducationDocument> EducationDocuments => _educationDocuments.AsReadOnly();

        private readonly List<EmployeeSpecialty> _employeeSpecialties = [];
        public IReadOnlyCollection<EmployeeSpecialty> EmployeeSpecialties => _employeeSpecialties.AsReadOnly();

        private readonly List<PersonalDocument> _personalDocuments = [];
        public IReadOnlyCollection<PersonalDocument> PersonalDocuments => _personalDocuments.AsReadOnly();

        private readonly List<Assignment> _assignments = [];
        public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();

        private readonly List<Contract> _contracts = [];
        public IReadOnlyCollection<Contract> Contracts => _contracts.AsReadOnly();

        private Employee() { }
        private Employee(EmployeeId id, Surname surname, Name name, Patronymic? patronymic, GenderId genderId) : base(id)
        {
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Rating = Rating.DefaultRating;
            PersonalPhoto = FileUrl.Empty;
            GenderId = genderId;
        }

        /// <summary>
        /// Создание НОВОГО объекта сотрудника
        /// </summary>
        /// <param name="surname"></param>
        /// <param name="name"></param>
        /// <param name="patronymic"></param>
        /// <param name="genderId"></param>
        /// <exception cref="EmptyIdentifierException"></exception>
        /// <exception cref="EmptySurnameException"></exception>
        /// <exception cref="EmptyNameException"></exception>
        /// <exception cref="EmptyPatronymicException"></exception>
        /// <exception cref="LengthException"></exception>
        /// <exception cref="IncorrectStringException"></exception>
        /// <returns cref="Employee">НОВЫЙ объект Employee</returns>
        public static Employee Create(string surname, string name, string? patronymic, Guid genderId)
            => new(EmployeeId.New(), Surname.Create(surname), Name.Create(name), patronymic != null ? Patronymic.Create(patronymic) : null, GenderId.Create(genderId));

        #region Employee

        /// <summary>
        /// При входе меняет дату входа
        /// </summary>
        public void Sign() => DateEntry = DateTime.UtcNow;

        /// <summary>
        /// Обновление поля ФАМИЛИИ сотрудника
        /// </summary>
        /// <param name="surname"></param>
        /// <exception cref="IdenticalValuesException"></exception>
        public void UpdateSurname(Surname surname)
        {
            if (surname != Surname)
                Surname = surname;
        }

        /// <summary>
        /// Обновление поля ИМЕНИ сотрудника
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="IdenticalValuesException"></exception>
        public void UpdateName(Name name)
        {
            if (name != Name)
                Name = name;
        }

        /// <summary>
        /// Обновление поля ОТЧЕСТВА сотрудника
        /// </summary>
        /// <param name="patronymic"></param>
        /// <exception cref="IdenticalValuesException"></exception>
        public void UpdatePatronymic(Patronymic patronymic)
        {
            if (patronymic != Patronymic)
                Patronymic = patronymic;
        }

        /// <summary>
        /// Обновление поля ГЕНДЕРА сотрудника
        /// </summary>
        /// <param name="genderId"></param>
        /// <exception cref="IdenticalValuesException"></exception>
        public void UpdateGenderId(GenderId genderId)
        {
            if (genderId != GenderId)
                GenderId = genderId;
        }

        /// <summary>
        /// Добавление поля Персональной фотографии сотруднику
        /// </summary>
        /// <param name="personalPhoto"></param>
        public void AddPersonalPhoto(FileUrl personalPhoto)
        {
            if (personalPhoto != PersonalPhoto)
                PersonalPhoto = personalPhoto;
        }

        public void DeletePersonalPhoto() => PersonalPhoto = null;

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        #endregion

        #region ContactInformation

        /// <exception cref="ExistContactInformationException"></exception>
        /// <exception cref="EmptyIdentifierException"></exception>
        /// <exception cref="PhoneNumberException"></exception>
        /// <exception cref="EmailAddressException"></exception>
        public void AddContactInformation(string personalPhone, string? corporatePhone, string personalEmail, string? corporateEmail, Address address)
        {
            GuardException.Against.That(this.ContactInformation != null, () => new ExistContactInformationException("Контактная информация уже указана! Вы можете её изменить!"));

            var info = ContactInformation.Create(this.Id, personalPhone, corporatePhone, personalEmail, corporateEmail, address);

            this.ContactInformation = info;
        }

        /// <exception cref="EmptyContactInformationException"></exception>
        public void UpdatePersonalPhone(string personalPhone)
        {
            GuardException.Against.That(this.ContactInformation == null, () => new EmptyContactInformationException($"Контактной информации у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            this.ContactInformation!.UpdatePersonalPhone(Phone.Create(personalPhone));
        }

        /// <exception cref="EmptyContactInformationException"></exception>
        public void UpdateCorporatePhone(string? corporatePhone)
        {
            GuardException.Against.That(this.ContactInformation == null, () => new EmptyContactInformationException($"Контактной информации у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            this.ContactInformation!.UpdateCorporatePhone(corporatePhone != null ? Phone.Create(corporatePhone) : Phone.Null);
        }

        /// <exception cref="EmptyContactInformationException"></exception>
        public void UpdatePersonalEmail(string personalEmail)
        {
            GuardException.Against.That(this.ContactInformation == null, () => new EmptyContactInformationException($"Контактной информации у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            this.ContactInformation!.UpdatePersonalEmail(Email.Create(personalEmail));
        }

        /// <exception cref="EmptyContactInformationException"></exception>
        public void UpdateCorporateEmail(string? corporateEmail)
        {
            GuardException.Against.That(this.ContactInformation == null, () => new EmptyContactInformationException($"Контактной информации у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            this.ContactInformation!.UpdateCorporateEmail(corporateEmail != null ? Email.Create(corporateEmail) : Email.Null);
        }

        public void UpdateAddress(Address address)
        {
            GuardException.Against.That(this.ContactInformation == null, () => new EmptyContactInformationException($"Контактной информации у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            this.ContactInformation!.UpdateAddress(address);
        }
        #endregion

        #region WorkPermit

        public void AddWorkPermit
            (
                string workPermitName,
                string? documentSeries,
                string workPermitNumber,
                string? protocolNumber,
                string specialtyName,
                string issuingAuthority,
                DateTime issueDate,
                DateTime expiryDate,
                Guid permitTypeId,
                Guid admissionStatusId
            )
        {
            var workPermit = WorkPermit.Create
                (
                    this.Id,
                    workPermitName,
                    documentSeries,
                    workPermitNumber,
                    protocolNumber,
                    specialtyName,
                    issuingAuthority,
                    issueDate,
                    expiryDate,
                    permitTypeId,
                    admissionStatusId
                );

            _workPermits.Add(workPermit);
        }

        public void UpdateNameWP(Guid id, string workPermitName)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Наименование: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var workPermit = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(workPermit == null, () => new NotFoundDocumentException($"Разрешение на работу не найдено!"));

            workPermit!.UpdateName(ProgramEducationName.Create(workPermitName));
        }

        public void UpdateDocumentSeriesWP(Guid id, string? documentSeries)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Серия документа: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var workPermit = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(workPermit == null, () => new NotFoundDocumentException($"Разрешение на работу не найдено!"));

            workPermit!.UpdateSeries(documentSeries != null ? DocumentSeries.Create(documentSeries) : DocumentSeries.Null);
        }

        public void UpdateDocumentNumberWP(Guid id, string workPermitNumber)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Номер документа: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var workPermit = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(workPermit == null, () => new NotFoundDocumentException($"Разрешение на работу не найдено!"));

            workPermit!.UpdateWorkPermitNumber(DocumentNumber.Create(workPermitNumber));
        }

        public void UpdateProtocolNumberWP(Guid id, string? protocolNumber)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Номер протокола: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var workPermit = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(workPermit == null, () => new NotFoundDocumentException($"Разрешение на работу не найдено!"));

            workPermit!.UpdateProtocolNumber(protocolNumber != null ? ProtocolNumber.Create(protocolNumber) : ProtocolNumber.Null);
        }

        public void UpdateSpecialtyNameWP(Guid id, string specialtyName)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Наименование специальности: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var workPermit = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(workPermit == null, () => new NotFoundDocumentException($"Разрешение на работу не найдено!"));

            workPermit!.UpdateSpecialtyName(SpecialtyName.Create(specialtyName));
        }

        public void UpdateIssuingAuthorityWP(Guid id, string issuingAuthority)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Орган выдачи: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var workPermit = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(workPermit == null, () => new NotFoundDocumentException($"Разрешение на работу не найдено!"));

            workPermit!.UpdateIssuingAuthority(IssuingAuthority.Create(issuingAuthority));
        }

        public void UpdateIssueDateWP(Guid id, DateTime issueDate)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Дата выдачи: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var workPermit = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(workPermit == null, () => new NotFoundDocumentException($"Разрешение на работу не найдено!"));

            workPermit!.UpdateIssueDate(issueDate);
        }

        public void UpdateExpiryDateWP(Guid id, DateTime expiryDate)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Дата окончания: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var workPermit = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(workPermit == null, () => new NotFoundDocumentException($"Разрешение на работу не найдено!"));

            workPermit!.UpdateExpiryDate(expiryDate);
        }

        public void UpdatePermitTypeIdWP(Guid id, Guid permitTypeId)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Тип разрешения: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var document = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Разрешения на работу не найдено!"));

            document!.UpdatePermitType(PermitTypeId.Create(permitTypeId));
        }

        public void UpdateAdmissionStatusIdWP(Guid id, Guid admissionStatusId)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Статус допуска: Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var document = this.WorkPermits.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Разрешения на работу не найдено!"));

            document!.UpdateAdmissionStatus(AdmissionStatusId.Create(admissionStatusId));
        }

        public void DeleteWorkPermit(Guid workPermitId)
        {
            GuardException.Against.That(WorkPermits.Count == 0, () => new EmptyWorkPermitException($"Разрешения на работу у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var document = this.WorkPermits.FirstOrDefault(d => d.Id == workPermitId);

            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Разрешения на работу не найдено!"));

            _workPermits.Remove(document!);
        }

        #endregion

        #region EducationDocument

        public void AddEducationDocument
            (
                Guid educationLevelId,
                Guid documentTypeId,
                string documentNumber,
                DateTime issuedDate,
                string organizationName,
                string? qualificationAwardedName,
                string? specialtyName,
                string? programName,
                TimeSpan? totalHours
            )
        {
            var educationDocument = EducationDocument.Create
                (
                    this.Id,
                    educationLevelId,
                    documentTypeId,
                    documentNumber,
                    issuedDate,
                    organizationName,
                    qualificationAwardedName,
                    specialtyName,
                    programName,
                    totalHours
                );

            _educationDocuments.Add(educationDocument);
        }

        public void UpdateEducationLevel(Guid id, Guid educationLevelId)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Уровень Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));

            document!.UpdateEducationLevel(EducationLevelId.Create(educationLevelId));
        }

        public void UpdateDocumentTypeED(Guid id,  Guid documentTypeId)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Тип Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));
            
            document!.UpdateDocumentType(DocumentTypeId.Create(documentTypeId));
        }

        public void UpdateDocumentNumberED(Guid id, string documentNumber)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Номер Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));
            
            document!.UpdateDocumentNumber(DocumentNumber.Create(documentNumber));
        }

        public void UpdateIssuedDate(Guid id, DateTime issuedDate)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Дата выдачи Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));
            
            document!.UpdateIssuedDate(issuedDate);
        }

        public void UpdateOrganizationName(Guid id, string organizationName)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Название организации Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));
            
            document!.UpdateOrganizationName(IssuingAuthority.Create(organizationName));
        }

        public void UpdateQualificationAwardedName(Guid id, string? qualificationAwardedName)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Название квалификации Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));
            
            document!.UpdateQualificationAwardedName(qualificationAwardedName != null ? QualificationAwardedName.Create(qualificationAwardedName) : QualificationAwardedName.Null);
        }

        public void UpdateSpecialtyName(Guid id, string? specialtyName)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Название специальности Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));
            
            document!.UpdateSpecialtyName(specialtyName != null ? SpecialtyName.Create(specialtyName) : SpecialtyName.Null);
        }

        public void UpdateProgramName(Guid id, string? programName)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Название программы Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));
            
            document!.UpdateProgramName(programName != null ? ProgramEducationName.Create(programName) : ProgramEducationName.Null);
        }

        public void UpdateTotalHours(Guid id, TimeSpan? totalHours)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Общее количество часов Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));
            
            document!.UpdateTotalHours(totalHours != null ? Hours.Create(totalHours.Value.TotalHours) : Hours.Null);
        }

        public void DeleteEducationDocument(Guid educationDocumentId)
        {
            GuardException.Against.That(EducationDocuments.Count == 0, () => new EmptyEducationDocumentException($"Документов об образовании у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var document = this.EducationDocuments.FirstOrDefault(d => d.Id == educationDocumentId);

            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Документ об образовании не найден!"));

            _educationDocuments.Remove(document!);
        }

        #endregion

        #region EmployeeSpecialty

        // TODO : Сделать проверки
        public void AddSpecialty(Guid specialtyId) 
            => _employeeSpecialties.Add(EmployeeSpecialty.Create(this.Id, specialtyId));

        public void RemoveSpecialty(Guid specialtyIdOld) => 
            _employeeSpecialties.Remove(_employeeSpecialties.FirstOrDefault(x => x.SpecialtyId == specialtyIdOld));

        #endregion

        #region PersonalDocument

        public void AddPersonalDocument
            (
                Guid documentTypeId,
                string documentNumber,
                string? documentSeries,
                ImageKey? fileKey
            )
        {
            var personalDocument = PersonalDocument.Create
                (
                    this.Id,
                    documentTypeId,
                    documentNumber,
                    documentSeries,
                    fileKey
                );

            _personalDocuments.Add(personalDocument);
        }

        public void UpdateDocumentTypePD(Guid id, Guid documentTypeId)
        {
            GuardException.Against.That(PersonalDocuments.Count == 0, () => new EmptyPersonalDocumentException($"Тип Персональных документов у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var document = this.PersonalDocuments.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Персональный документ не найден!"));

            document!.UpdateDocumentType(DocumentTypeId.Create(documentTypeId));
        }

        public void UpdateDocumentNumberPD(Guid id, string documentNumber)
        {
            GuardException.Against.That(PersonalDocuments.Count == 0, () => new EmptyPersonalDocumentException($"Номер Персональных документов у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.PersonalDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Персональный документ не найден!"));

            document!.UpdateDocumentNumber(DocumentNumber.Create(documentNumber));
        }

        public void UpdateDocumentSeries(Guid id, string? documentSeries)
        {
            GuardException.Against.That(PersonalDocuments.Count == 0, () => new EmptyPersonalDocumentException($"Серия Персональных документов у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var document = this.PersonalDocuments.FirstOrDefault(d => d.Id == id);
            
            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Персональный документ не найден!"));
            
            document!.UpdateDocumentSeries(documentSeries != null ? DocumentSeries.Create(documentSeries) : DocumentSeries.Null);
        }

        public void DeletePersonalDocument(Guid personalDocumentId)
        {
            GuardException.Against.That(PersonalDocuments.Count == 0, () => new EmptyPersonalDocumentException($"Серия Персональных документов у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var document = this.PersonalDocuments.FirstOrDefault(d => d.Id == personalDocumentId);

            GuardException.Against.That(document == null, () => new NotFoundDocumentException($"Персональный документ не найден!"));

            _personalDocuments.Remove(document!);

            //this.AddDomainEvent(new PersonalDocumentDeletedEvent(document.Id));
        }

        #endregion

        #region Contract

        public Contract AddContract
            (
                Guid employeeTypeId,
                string contractNumber,
                DateTime startDate,
                DateTime endDate,
                decimal salary,
                ImageKey? fileKey
            )
        {
            var contract = Contract.Create
                (
                    this.Id,
                    employeeTypeId,
                    contractNumber,
                    startDate,
                    endDate,
                    salary,
                    fileKey
                );

            _contracts.Add(contract);

            return contract;
        }

        public void UpdateAssignmentContractEmploymentTypeId(Guid id, Guid employeeTypeId)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Тип сотрудника: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);
            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            var contract = this.Contracts.FirstOrDefault(c => c.Id == assignment!.ContractId);
            GuardException.Against.That(contract == null, () => new NotFoundContractException($"Контракт не найден!"));

            contract!.UpdateEmployeeType(EmployeeTypeId.Create(employeeTypeId));
        }

        public void UpdateAssignmentContractContractNumber(Guid id, string contractNumber)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Номер контракта: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);
            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            var contract = this.Contracts.FirstOrDefault(c => c.Id == assignment!.ContractId);
            GuardException.Against.That(contract == null, () => new NotFoundContractException($"Контракт не найден!"));

            contract!.UpdateNumber(ContractNumber.Create(contractNumber));
        }

        public void UpdateAssignmentContractStartDate(Guid id, DateTime startDate)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Дата начала: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);
            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            var contract = this.Contracts.FirstOrDefault(c => c.Id == assignment!.ContractId);
            GuardException.Against.That(contract == null, () => new NotFoundContractException($"Контракт не найден!"));

            contract!.UpdateStartDate(startDate);
        }

        public void UpdateAssignmentContractEndDate(Guid id, DateTime endDate)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Дата окончания: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);
            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            var contract = this.Contracts.FirstOrDefault(c => c.Id == assignment!.ContractId);
            GuardException.Against.That(contract == null, () => new NotFoundContractException($"Контракт не найден!"));

            contract!.UpdateEndDate(endDate);
        }

        public void UpdateAssignmentContractSalary(Guid id, decimal salary)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Зарплата: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);
            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            var contract = this.Contracts.FirstOrDefault(c => c.Id == assignment!.ContractId);
            GuardException.Against.That(contract == null, () => new NotFoundContractException($"Контракт не найден!"));

            contract!.UpdateSalary(Salary.FromRubles(salary));
        }

        #endregion

        #region Assignemnt

        public void AddAssignment
            (
                Guid positionId,
                Guid departmentId,
                Guid? managerId,
                DateTime hireDate,
                DateTime? terminationDate,
                Guid statusId,
                Guid contractId
            )
        {
            var assignment = Assignment.Create
                (
                    this.Id,
                    positionId,
                    departmentId,
                    managerId,
                    hireDate,
                    terminationDate,
                    statusId,
                    contractId
                );

            _assignments.Add(assignment);
        }

        public void UpdateAssignmentPositionId(Guid id, Guid positionId)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Должность: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            assignment!.UpdatePosition(PositionId.Create(positionId));
        }

        public void UpdateAssignmentDepartmentId(Guid id, Guid departmentId)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Отдел: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            assignment!.UpdateDepartment(DepartmentId.Create(departmentId));
        }

        public void UpdateAssignmentManagerId(Guid id, Guid? managerId)
        {
            //GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Должность: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            //var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);

            //GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            //assignment!.UpdatePosition(EmployeeId.Create(managerId));
        }

        public void UpdateAssignmentHireDate(Guid id, DateTime hireDate)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Дата найма: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            assignment!.UpdateHireDate(hireDate);
        }

        public void UpdateAssignmentTerminationDate(Guid id, DateTime? terminationDate)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Дата расторжения: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            assignment!.UpdateTerminationDate(terminationDate);
        }

        public void UpdateAssignmentStatusId(Guid id, Guid statusId)
        {
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Статус: Назначение у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == id);

            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));

            assignment!.UpdateStatus(StatusId.Create(statusId));
        }

        public void DeleteAssignmentContract(Guid assignmentId)
        {
            // Проверка на наличие назначений
            GuardException.Against.That(Assignments.Count == 0, () => new EmptyAssignmentException($"Назначения у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));

            var assignment = this.Assignments.FirstOrDefault(d => d.Id == assignmentId);

            GuardException.Against.That(assignment == null, () => new NotFoundAssignmentException($"Назначение не найдено!"));


            // Проверка на наличие контрактов
            GuardException.Against.That(Contracts.Count == 0, () => new EmptyContractException($"Контракт у пользователя: '{Surname} {Name} {Patronymic}' не существует!"));
            
            var contract = this.Contracts.FirstOrDefault(c => c.Id == assignment!.ContractId);

            GuardException.Against.That(contract == null, () => new NotFoundContractException($"Контракт не найден!"));

            // Удаление контракта и назначения
            _contracts.Remove(contract!);
            _assignments.Remove(assignment!);
        }

        #endregion
    }
}
