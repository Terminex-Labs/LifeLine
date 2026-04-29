using Shared.Contracts.Response.EmployeeService;
using Shared.WPF.Enums;
using Shared.WPF.ViewModels.Abstract;

namespace LifeLine.HrPanel.Desktop.Models
{
    public sealed class WorkPermitDisplay : BaseViewModel
    {
        private readonly WorkPermitResponse _model;

        private readonly IReadOnlyCollection<PermitTypeDisplay> _permitTypes;
        private readonly IReadOnlyCollection<AdmissionStatusDisplay> _admissionStatuses;

        public WorkPermitDisplay
            (
                WorkPermitResponse model, 
                IReadOnlyCollection<PermitTypeDisplay> permitTypes, 
                IReadOnlyCollection<AdmissionStatusDisplay> admissionStatuses,
                string? filePath,
                SaveStatus saveStatus
            )
        {
            _model = model;
            _permitTypes = permitTypes;
            _admissionStatuses = admissionStatuses;

            _workPermitName = model.WorkPermitName;
            _documentSeries = model.DocumentSeries;
            _workPermitNumber = model.WorkPermitNumber;
            _protocolNumber = model.ProtocolNumber;
            _specialtyName = model.SpecialtyName;
            _issuingAuthority = model.IssuingAuthority;
            _issueDate = model.IssueDate;
            _expiryDate = model.ExpiryDate;
            FilePath = filePath;
            SaveStatus = saveStatus;

            SetPermiteType(_model.PermitTypeId);
            SetAdmissionStatus(_model.AdmissionStatusId);
        }

        public SaveStatus SaveStatus
        {
            get => field;
            set => SetProperty(ref field, value);
        }
        public void SetSaveStatus(SaveStatus saveStatus) => SaveStatus = saveStatus; 

        public string WorkPermitId => _model.Id;
        public string EmployeeId => _model.EmployeeId;
        public string PermitTypeId => _model.PermitTypeId;
        public string AdmissionStatusId => _model.AdmissionStatusId;

        //WorkPermitName
        private string _workPermitName;
        public string WorkPermitName
        {
            get => _workPermitName;
            set => SetProperty(ref _workPermitName, value);
        }

        //DocumentSeries
        private string? _documentSeries;
        public string? DocumentSeries
        {
            get => _documentSeries;
            set => SetProperty(ref _documentSeries, value);
        }

        //WorkPermitNumber
        private string _workPermitNumber;
        public string WorkPermitNumber
        {
            get => _workPermitNumber;
            set => SetProperty(ref _workPermitNumber, value);
        }

        //ProtocolNumber
        private string? _protocolNumber;
        public string? ProtocolNumber
        {
            get => _protocolNumber;
            set => SetProperty(ref _protocolNumber, value);
        }

        //SpecialtyName
        private string _specialtyName;
        public string SpecialtyName
        {
            get => _specialtyName;
            set => SetProperty(ref _specialtyName, value);
        }

        //IssuingAuthority
        private string _issuingAuthority;
        public string IssuingAuthority
        {
            get => _issuingAuthority;
            set => SetProperty(ref _issuingAuthority, value);
        }

        //IssueDate
        private DateTime _issueDate;
        public DateTime IssueDate
        {
            get => _issueDate;
            set => SetProperty(ref _issueDate, value);
        }

        //ExpiryDate
        private DateTime _expiryDate;
        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set => SetProperty(ref _expiryDate, value);
        }

        //SelectedPermitType
        private PermitTypeDisplay _permitType = null!;
        public PermitTypeDisplay PermitType
        {
            get => _permitType;
            set => SetProperty(ref _permitType, value);
        }
        public void SetPermiteType(string id) => PermitType = _permitTypes.FirstOrDefault(x => x.Id.ToString() == id)!;

        //SelectedAdmissionStatus
        private AdmissionStatusDisplay _admissionStatus = null!;
        public AdmissionStatusDisplay AdmissionStatus
        {
            get => _admissionStatus;
            set => SetProperty(ref _admissionStatus, value);
        }
        public void SetAdmissionStatus(string id) => AdmissionStatus = _admissionStatuses.FirstOrDefault(x => x.Id.ToString() == id)!;

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

        public WorkPermitResponse GetUnderLineModel() => _model;
    }
}
