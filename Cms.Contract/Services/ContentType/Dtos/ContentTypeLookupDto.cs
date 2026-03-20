namespace Cms.Contract.Services.ContentType.Dtos
{
    public class ContentTypeLookupDto
    {
        public Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}
