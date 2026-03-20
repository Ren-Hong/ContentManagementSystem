namespace Cms.Contract.Controllers.Role.Models
{
    public class RoleLookupResponseModel
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }
    }
}
