namespace Cms.Contract.Controllers.Department.Models
{
    public class AccessibleDepartmentResponseModel
    {
        public required string DepartmentName { get; set; }

        public required string DepartmentCode { get; set; }

        public Guid DepartmentId { get; set; }
    }
}
