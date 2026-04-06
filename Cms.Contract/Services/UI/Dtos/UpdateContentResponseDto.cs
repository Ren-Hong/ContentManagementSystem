namespace Cms.Contract.Services.UI.Dtos
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
