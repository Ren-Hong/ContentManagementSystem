namespace Cms.Contract.Services.Permission.Dtos
{
    public class PermissionLookupDto
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }
    }
}
