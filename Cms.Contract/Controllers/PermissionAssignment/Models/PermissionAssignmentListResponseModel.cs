namespace Cms.Contract.Controllers.PermissionAssignment.Models
{
    public class PermissionAssignmentListResponseModel
    {
        public required Guid AccountId { get; set; }
        
        public required string Username { get; set; }

        public List<PermissionDepartmentResponseModel>? PermissionDepartments { get; set; }

    }

    public class PermissionDepartmentResponseModel
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }

        public List<AssignedDepartmentResponseModel>? Departments { get; set; }
    }

    public class AssignedDepartmentResponseModel
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
    }
}
