namespace Cms.Contract.Services.CMS.Dtos
{
    public class DeleteContentTypeResponseDto
    {
        public DeleteContentTypeResult Result { get; set; }
    }

    public enum DeleteContentTypeResult
    {
        Success,
        PermissionDenied,
        TypeNotFound,
        ContentTypeInUse,
        FieldInUse
    }
}
