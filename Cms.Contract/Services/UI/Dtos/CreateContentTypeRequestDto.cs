namespace Cms.Contract.Services.UI.Dtos
{
    public class CreateContentTypeRequestDto
    {
        public Guid DepartmentId { get; set; }

        public required string TypeName { get; set; }

        public required List<CreateContentTypeFieldDto> Fields { get; set; }
    }

    public class CreateContentTypeFieldDto
    {
        public required string FieldName { get; set; }

        public required string FieldType { get; set; }

        public bool IsRequired { get; set; }
    }
}
