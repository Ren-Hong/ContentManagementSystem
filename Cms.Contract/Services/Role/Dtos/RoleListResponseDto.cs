using Cms.Contract.Repositories.Role.Persistence;

namespace Cms.Contract.Services.Role.Dtos
{
    public class RoleListResponseDto
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }

        public RoleStatus Status { get; set; }

        public required List<RolePermissionScopeResponseDto> PermissionScopes { get; set; }
    }

    public class RolePermissionScopeResponseDto
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }

        public required List<RoleScopeResponseDto> Scopes { get; set; }
    }

    public class RoleScopeResponseDto
    {
        public required Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }
    }
}
