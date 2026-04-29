using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Enums;
using Shared.WPF.ViewModels.Abstract;

namespace LifeLine.HrPanel.Desktop.Models
{
    public sealed class EducationDocumentDisplay : BaseViewModel
    {
        private readonly EducationDocumentResponse _model;

        private readonly IReadOnlyCollection<EducationLevelDisplay> _educationLevels;
        private readonly IReadOnlyCollection<DocumentTypeDisplay> _documentTypes;

        public EducationDocumentDisplay
            (
                EducationDocumentResponse model, 
                IReadOnlyCollection<EducationLevelDisplay> educationLevels,
                IReadOnlyCollection<DocumentTypeDisplay> documentTypes,
                string? filePath,
                SaveStatus saveStatus
            )
        {
            _model = model;

            _educationLevels = educationLevels;
            _documentTypes = documentTypes;

            _documentNumber = model.DocumentNumber;
            _issuedDate = DateTime.Parse(model.IssuedDate);
            _organizationName = model.OrganizationName;
            _qualificationAwardedName = model.QualificationAwardedName;
            _specialtyName = model.SpecialtyName;
            _programName = model.ProgramName;
            //_totalHours = TimeSpan.Parse(model.TotalHours!);
            TimeSpan.TryParse(model.TotalHours, out var resultTimeSpanParse);
            _totalHours = resultTimeSpanParse;
            FilePath = filePath;
            SaveStatus = saveStatus;

            SetEducationLevel(_model.EducationLevelId);
            SetDocumentType(_model.DocumentTypeId);
        }

        public SaveStatus SaveStatus
        {
            get => field;
            set => SetProperty(ref field, value);
        }
        public void SetSaveStatus(SaveStatus saveStatus) => SaveStatus = saveStatus;

        public string EducationDocumentId => _model.Id;
        public string EmployeeId => _model.EmployeeId;
        public string EducationLevelId => _model.EducationLevelId;
        public string DocumentTypeId => _model.DocumentTypeId;

        //DocumentNumber
        private string _documentNumber;
        public string DocumentNumber
        {
            get => _documentNumber;
            set => SetProperty(ref _documentNumber, value);
        }

        //IssuedDate
        private DateTime _issuedDate;
        public DateTime IssuedDate
        {
            get => _issuedDate;
            set => SetProperty(ref _issuedDate, value);
        }

        //OrganizationName
        private string _organizationName;
        public string OrganizationName
        {
            get => _organizationName;
            set => SetProperty(ref _organizationName, value);
        }

        //QualificationAwardedName
        private string? _qualificationAwardedName;
        public string? QualificationAwardedName
        {
            get => _qualificationAwardedName;
            set => SetProperty(ref _qualificationAwardedName, value);
        }

        //SpecialtyName
        private string? _specialtyName;
        public string? SpecialtyName
        {
            get => _specialtyName;
            set => SetProperty(ref _specialtyName, value);
        }

        //ProgramName
        private string? _programName;
        public string? ProgramName
        {
            get => _programName;
            set => SetProperty(ref _programName, value);
        }

        //TotalHours
        private TimeSpan? _totalHours;
        public TimeSpan? TotalHours
        {
            get => _totalHours;
            set => SetProperty(ref _totalHours, value);
        }

        //SelectedEducationLevel
        private EducationLevelDisplay _educationLevel = null!;
        public EducationLevelDisplay EducationLevel
        {
            get => _educationLevel;
            set => SetProperty(ref _educationLevel, value);
        }
        public void SetEducationLevel(string id) => EducationLevel = _educationLevels.FirstOrDefault(x => x.Id == id)!;

        //SelectedDocumentType
        private DocumentTypeDisplay _documentType = null!;
        public DocumentTypeDisplay DocumentType
        {
            get => _documentType;
            set => SetProperty(ref _documentType, value);
        }
        public void SetDocumentType(string id) => DocumentType = _documentTypes.FirstOrDefault(x => x.Id == id)!;

        public string? FilePath
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public byte[]? FileBytes { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; } = "application/pdf";

        [System.ComponentModel.Browsable(false)]
        public bool HasFileForUpload => FileBytes != null || (!string.IsNullOrWhiteSpace(FilePath) && System.IO.File.Exists(FilePath));

        public EducationDocumentResponse GetUnderLineModel() => _model;
    }
}
