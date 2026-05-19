namespace LifeLine.File.Service.Client
{
    public class FileConst
    {
        public const string BUCKET_NAME = "life-line";
        public const string EMPLOYEE_FOLDER = "employees";

        public const string PATIENT_FOLDER = "patients";
        public const string ANALIZES_FOLDER = "analyzes";

        public static string BuildEmployeeFolder(string employeeId, EmployeeFolderType folder)
            => $"/{EMPLOYEE_FOLDER}/{employeeId}/{folder.Value}";

        public static string BuildPatientFolder(string patientId, EmployeeFolderType folder)
            => $"/{PATIENT_FOLDER}/{patientId}/{folder.Value}";
    }

    public readonly struct EmployeeFolderType(string value)
    {
        public readonly string Value = value;

        public readonly static EmployeeFolderType PersonalPhoto     = new ("personal photo");

        public readonly static EmployeeFolderType PersonalDocument  = new ("personal document");
        public readonly static EmployeeFolderType EducationDocument = new ("education document");
        public readonly static EmployeeFolderType Specialty         = new ("specialty");
        public readonly static EmployeeFolderType WorkPermit        = new ("work permit");
        public readonly static EmployeeFolderType Assignment        = new ("assignment");
    }

    public readonly struct PatientFolderType(string value)
    {
        public readonly string Value = value;

        public readonly static PatientFolderType PersonalDocument   = new ("personal document");

        public readonly static PatientFolderType MRT                = new ($"{FileConst.ANALIZES_FOLDER}/mrt");
        public readonly static PatientFolderType KT                 = new ($"{FileConst.ANALIZES_FOLDER}/kt");
        public readonly static PatientFolderType X_Ray              = new ($"{FileConst.ANALIZES_FOLDER}/x ray");
        public readonly static PatientFolderType UltrasoundScan     = new ($"{FileConst.ANALIZES_FOLDER}/ultrasound scan");
    }
}
