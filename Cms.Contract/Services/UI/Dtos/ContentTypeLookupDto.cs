namespace Cms.Contract.Services.UI.Dtos
{
    public class ContentTypeLookupDto
    {
        public Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}
