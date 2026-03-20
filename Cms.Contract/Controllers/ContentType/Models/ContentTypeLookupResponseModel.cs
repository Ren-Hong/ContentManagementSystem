namespace Cms.Contract.Controllers.ContentType.Models
{
    public class ContentTypeLookupResponseModel
    {
        public Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}
