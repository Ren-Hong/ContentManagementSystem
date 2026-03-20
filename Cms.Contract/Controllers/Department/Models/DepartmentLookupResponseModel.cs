namespace Cms.Contract.Controllers.Department.Models
{
    public class DepartmentLookupResponseModel
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }
    }
}
