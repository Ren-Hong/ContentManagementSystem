namespace Cms.Contract.Services.RBAC.Dtos
{
    public class ResetPasswordRequestDto
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }
}

