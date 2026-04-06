namespace Cms.Contract.Services.UI.Dtos
{
    public class DeleteContentTypeResponseDto
    {
        public DeleteContentTypeResult Result { get; set; }
    }

    public enum DeleteContentTypeResult
    {
        Success,
        TypeNotFound,
        ContentTypeInUse,
        FieldInUse
    }
}
