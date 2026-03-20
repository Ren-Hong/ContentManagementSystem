namespace Cms.Contract.Repositories.ContentType.Entities
{
    public class ContentTypeLookupEntity
    {
        public required Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}
