namespace Cms.Contract.Services.PermissionAssignment.Dtos
{
    public class PermissionAssignmentListResponseDto
    {
        public required Guid AccountId { get; set; }
        
        public required string Username { get; set; }

        public List<PermissionDepartmentResponseDto>? PermissionDepartments { get; set; }

    }

    public class PermissionDepartmentResponseDto
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }

        public List<AssignedDepartmentResponseDto>? Departments { get; set; }
    }

    public class AssignedDepartmentResponseDto
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
    }
}
