namespace Cms.Contract.Controllers.CMS.Models
{
    public class ContentTypeLookupResponseModel
    {
        public Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}
