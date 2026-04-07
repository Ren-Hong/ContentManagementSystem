namespace Cms.Contract.Services.RBAC.Dtos
{
    public class PermissionOptionDto
    {
        public Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }
    }
}

