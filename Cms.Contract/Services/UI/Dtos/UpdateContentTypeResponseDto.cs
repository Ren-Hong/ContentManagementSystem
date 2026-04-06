namespace Cms.Contract.Services.UI.Dtos
{
    public class UpdateContentTypeResponseDto
    {
        public UpdateContentTypeResult Result { get; set; }
    }

    public enum UpdateContentTypeResult
    {
        Success,
        TypeNotFound,
        TypeNameRequired,
        TypeNameDuplicated,
        FieldRequired,
        FieldNameRequired,
        FieldNameDuplicated,
        FieldTypeRequired,
        FieldTypeInvalid,
        FieldNotFound,
        FieldInUse
    }
}
