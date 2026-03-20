using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.ContentType.Models
{
    public class UpdateContentTypeBodyModel
    {
        [Required(ErrorMessage = "請輸入問卷名稱")]
        public required string TypeName { get; set; }

        [Required(ErrorMessage = "請至少保留一個欄位")]
        [MinLength(1, ErrorMessage = "請至少保留一個欄位")]
        public required List<UpdateContentTypeFieldModel> Fields { get; set; }
    }
}
