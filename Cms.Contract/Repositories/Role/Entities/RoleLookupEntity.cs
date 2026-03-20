namespace Cms.Contract.Repositories.Role.Entities
{
    public class RoleLookupEntity
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }
    }
}
