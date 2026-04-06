namespace Cms.Contract.Services.RBAC.Dtos
{
    public class UpdateRoleResponseDto
    {
        public UpdateRoleResult Result { get; set; }
    }

    public enum UpdateRoleResult
    {
        Success,
        RoleNameRequired,
        PermissionRequired,
        RoleNameNotFound,
        StatusNotFound,
        PermissionNotFound,
        ScopeNotFound
    }
}

