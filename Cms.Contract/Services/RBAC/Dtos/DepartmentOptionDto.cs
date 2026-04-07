namespace Cms.Contract.Services.RBAC.Dtos
{
    public class DepartmentOptionDto
    {
        public Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }
    }
}

