namespace Cms.Contract.Controllers.CMS.Models
{
    public class CreateContentTypeResponseModel
    {
        public Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}
