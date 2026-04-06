namespace Cms.Contract.Services.RBAC.Dtos
{
    public class ResetPasswordResponseDto
    {
        public ResetPasswordResult Result { get; set; }
    }

    public enum ResetPasswordResult
    {
        Success,
        UsernameRequired,
        PasswordRequired,
        AccountNotFound
    }
}

