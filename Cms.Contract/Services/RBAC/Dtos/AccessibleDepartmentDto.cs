namespace Cms.Contract.Services.RBAC.Dtos
{
    public class AccessibleDepartmentDto
    {
        public Guid DepartmentId { get; set; }

        public required string DepartmentCode { get; set; }

        public required string DepartmentName { get; set; }
    }
}

