namespace Cms.Contract.Controllers.RBAC.Models
{
    public class DepartmentLookupResponseModel
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }
    }
}
