namespace Shared.WPF.Helpers
{
    public static class S3UrlParser
    {
        public static (string? BuckerName, string? FileName) Parse(string dbUrl)
        {
            if (string.IsNullOrWhiteSpace(dbUrl))
                return (null, null);

            var personalPhtotInfo = dbUrl.Split(":");

            var bucketName = personalPhtotInfo[0];
            var fileName = personalPhtotInfo[1];

            return (bucketName, fileName);
        }
    }
}
