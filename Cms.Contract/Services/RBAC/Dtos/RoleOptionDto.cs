namespace Cms.Contract.Services.RBAC.Dtos
{
    public class RoleOptionDto
    {
        public Guid RoleId { get; set; }

        public required string RoleName { get; set; }
    }
}

