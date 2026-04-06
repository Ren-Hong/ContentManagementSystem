namespace Cms.Contract.Services.UI.Dtos
{
    public class PermissionLookupDto
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }
    }
}

