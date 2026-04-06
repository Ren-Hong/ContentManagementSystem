namespace Cms.Contract.Services.RBAC.Dtos
{
    public class UpdateAccountResponseDto
    {
        public UpdateAccountResult Result { get; set; }
    }

    public enum UpdateAccountResult
    {
        Success,
        UsernameRequired,
        PasswordRequired,
        AccountNotFound,
        StatusNotFound,
        RoleAssignmentsRequired,
        RoleNotFound,
        DepartmentNotFound,
    }
}

