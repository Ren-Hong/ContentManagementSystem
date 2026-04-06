using Cms.Contract.Repositories.Role.Persistence;

namespace Cms.Contract.Controllers.RBAC.Models
{
    public class RoleListResponseModel
    {
        
            public required Guid RoleId { get; set; }

            public required string RoleName { get; set; }

            public RoleStatus Status { get; set; }

            public List<RolePermissionScopeResponseModel>? PermissionScopes { get; set; }
    }

    public class RolePermissionScopeResponseModel
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }

        public required List<RoleScopeResponseModel> Scopes { get; set; }
    }

    public class RoleScopeResponseModel
    {
        public required Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }
    }
}
