namespace Cms.Contract.Services.RBAC.Dtos
{
    public class LoginRequestDto
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }
}

