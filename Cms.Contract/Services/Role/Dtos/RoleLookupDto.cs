namespace Cms.Contract.Services.Role.Dtos
{
    public class RoleLookupDto
    {
        public required Guid RoleId { get; set; } 

        public required string RoleName { get; set; }
    }
}
