namespace Cms.Contract.Services.CMS.Dtos
{
    public class CreateContentTypeResponseDto
    {
        public CreateContentTypeResult Result { get; set; }

        public Guid? TypeId { get; set; }

        public string? TypeName { get; set; }
    }

    public enum CreateContentTypeResult
    {
        Success,
        DepartmentRequired,
        PermissionDenied,
        TypeNameRequired,
        TypeNameDuplicated,
        FieldRequired,
        FieldNameRequired,
        FieldNameDuplicated,
        FieldTypeRequired,
        FieldTypeInvalid
    }
}
