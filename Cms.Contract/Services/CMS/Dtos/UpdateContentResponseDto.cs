namespace Cms.Contract.Services.CMS.Dtos
{
    public class UpdateContentResponseDto
    {
        public UpdateContentResult Result { get; set; }
    }

    public enum UpdateContentResult
    {
        Success,
        ContentNotFound,
        PermissionDenied,
        RevisionNotFound,
        FieldValueRequired,
        FieldNotFound,
        RequiredFieldMissing
    }
}
