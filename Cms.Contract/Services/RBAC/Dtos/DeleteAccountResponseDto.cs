namespace Cms.Contract.Services.RBAC.Dtos
{
    public class DeleteAccountResponseDto
    {
        public DeleteAccountResult Result { get; set; }
    }

    public enum DeleteAccountResult
    {
        Success,
        UsernameRequired,
        AccountNotFound
    }
}

