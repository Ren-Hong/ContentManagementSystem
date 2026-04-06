using Cms.Contract.Repositories.Role.Persistence;

namespace Cms.Contract.Services.RBAC.Dtos
{
    public class UpdateRoleRequestDto
    {
        public required string RoleName { get; set; }

        public required List<PermissionScopeDto> PermissionScopes { get; set; }

        public RoleStatus Status { get; set; }
    }
}

