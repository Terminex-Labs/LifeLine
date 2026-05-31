using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Enums;
using Shared.WPF.ViewModels.Abstract;

namespace LifeLine.HrPanel.Desktop.Models
{
    public sealed class PersonalDocumentDisplay: BaseViewModel
    {
        private readonly PersonalDocumentResponse _model;

        private readonly IReadOnlyCollection<DocumentTypeDisplay> _documentTypes;

        public PersonalDocumentDisplay(PersonalDocumentResponse model, IReadOnlyCollection<DocumentTypeDisplay> documentTypes, SaveStatus saveStatus)
        {
            _model = model;
            _documentTypes = documentTypes;

            _documentNumber = model.Number;
            _documentSeries = model.Series;
            _fileKey = model.FileKey;
            SaveStatus = saveStatus;

            SetDocumentType(_model.DocumentTypeId.ToString());
        }

        public SaveStatus SaveStatus
        {
            get => field;
            set => SetProperty(ref field, value);
        }
        public void SetSaveStatus(SaveStatus saveStatus) => SaveStatus = saveStatus;

        public Guid PersonalDocumentId => _model.Id;
        public Guid DocumentTypeId => _model.DocumentTypeId;

        private string _documentNumber;
        public string DocumentNumber
        {
            get => _documentNumber;
            set => SetProperty(ref _documentNumber, value);
        }

        private string? _documentSeries;
        public string? DocumentSeries
        {
            get => _documentSeries;
            set => SetProperty(ref _documentSeries, value);
        }

        //SelectedDocumentType
        private DocumentTypeDisplay _documentType = null!;
        public DocumentTypeDisplay DocumentType
        {
            get => _documentType;
            set => SetProperty(ref _documentType, value);
        }
        public void SetDocumentType(string id) => DocumentType = _documentTypes.FirstOrDefault(x => x.Id.ToString() == id)!;

        private string? _fileKey;
        public string? FileKey
        {
            get => _fileKey;
            set => SetProperty(ref _fileKey, value);
        }

        public byte[]? FileBytes { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; } = "application/pdf"; 
        
        [System.ComponentModel.Browsable(false)]
        //public bool HasFileForUpload => FileBytes != null || (!string.IsNullOrWhiteSpace(FileKey) && System.IO.File.Exists(FileKey));

        public PersonalDocumentResponse GetUnderLineModel() => _model;
    }
}
