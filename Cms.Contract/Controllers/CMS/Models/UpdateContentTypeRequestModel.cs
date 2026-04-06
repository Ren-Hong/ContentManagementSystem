using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.CMS.Models
{
    public class UpdateContentTypeRequestModel
    {
        [Required(ErrorMessage = "請輸入問卷名稱")]
        public required string TypeName { get; set; }

        [Required(ErrorMessage = "請至少保留一個欄位")]
        [MinLength(1, ErrorMessage = "請至少保留一個欄位")]
        public required List<UpdateContentTypeFieldModel> Fields { get; set; }
    }

    public class UpdateContentTypeFieldModel
    {
        public Guid? FieldId { get; set; }

        [Required(ErrorMessage = "請輸入欄位名稱")]
        public required string FieldName { get; set; }

        [Required(ErrorMessage = "請選擇欄位型別")]
        public required string FieldType { get; set; }

        public bool IsRequired { get; set; }
    }
}
