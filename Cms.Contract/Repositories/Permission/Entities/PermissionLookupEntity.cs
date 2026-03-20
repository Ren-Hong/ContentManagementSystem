namespace Cms.Contract.Repositories.Permission.Entities
{
    public class PermissionLookupEntity
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }
    }
}
