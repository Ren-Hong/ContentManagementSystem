namespace Cms.Contract.Services.RBAC.Dtos
{
    public class CreateRoleResponseDto
    {
        public CreateRoleResult Result { get; set; }
    }

    public enum CreateRoleResult
    {
        Success,
        RoleNameRequired,
        RoleCodeRequired,
        RoleNameDuplicated,
        PermissionRequired,
        PermissionNotFound,
        ScopeNotFound
    }
}


