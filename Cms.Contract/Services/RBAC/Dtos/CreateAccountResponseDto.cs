namespace Cms.Contract.Services.RBAC.Dtos
{
    public class CreateAccountResponseDto
    {
        public CreateAccountResult Result { get; set; }
    }

    public enum CreateAccountResult
    {
        Success,
        UsernameRequired,
        PasswordRequired,
        UsernameDuplicated,
        RoleAssignmentsRequired,
        RoleNotFound,
        DepartmentNotFound,
    }
}

