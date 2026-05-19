using System;
using System.Text;
using System.Collections.Generic;

namespace Shared.Contracts.Request.EmployeeService.Employee
{
    public sealed record AddPersonalPhotoRequest(string BucketName, string FileName);
}
