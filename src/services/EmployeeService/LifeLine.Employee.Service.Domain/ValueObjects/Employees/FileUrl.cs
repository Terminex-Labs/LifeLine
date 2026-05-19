using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Domain.ValueObjects.Employees
{
    public readonly record struct FileUrl
    {
        public string Value { get; }

        public FileUrl(string value) => Value = value;

        public static Result<FileUrl> Create(string bucketName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(bucketName))
                return Result<FileUrl>.Failure(Error.Validation("Поля были пустые!"));

            var fullName = $"{bucketName}:{fileName}";

            return Result<FileUrl>.Success(new FileUrl(fullName));
        }

        public static FileUrl? Empty => null;

        public (string BucketName, string FileName) GetFullName()
        {
            var strings = Value.Split(':');

            var bucketName = strings[0];
            var fileName = strings[1];

            return (bucketName, fileName);
        }

        public string GetBucketName()
        {
            var strings = Value.Split(':');

            var bucketName = strings[0];

            return bucketName;
        }

        public string GetFileName()
        {
            var strings = Value.Split(':');

            var fileName = strings[1];

            return fileName;
        }

        public override string ToString() => Value;
        public static implicit operator string(FileUrl value) => value.Value;
    }
}
