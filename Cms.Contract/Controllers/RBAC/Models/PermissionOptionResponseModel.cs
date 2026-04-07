namespace Cms.Contract.Controllers.RBAC.Models
{
    public class PermissionOptionResponseModel
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }
    }
}

