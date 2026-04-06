namespace Cms.Contract.Services.RBAC.Dtos
{
    public class DeleteRoleResponseDto
    {
        public DeleteRoleResult Result { get; set; }
    }

    public enum DeleteRoleResult
    {
        Success,
        RoleNameRequired,
        RoleNotFound
    }
}

