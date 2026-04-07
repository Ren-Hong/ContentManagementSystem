namespace Cms.Contract.Controllers.RBAC.Models
{
    public class RoleOptionResponseModel
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }
    }
}

