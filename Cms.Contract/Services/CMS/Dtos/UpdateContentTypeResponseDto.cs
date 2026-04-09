namespace Cms.Contract.Services.CMS.Dtos
{
    public class UpdateContentTypeResponseDto
    {
        public UpdateContentTypeResult Result { get; set; }
    }

    public enum UpdateContentTypeResult
    {
        Success,
        PermissionDenied,
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
