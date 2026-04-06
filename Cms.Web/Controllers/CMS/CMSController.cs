using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Content.Models;
using Cms.Contract.Controllers.ContentType.Models;
using Cms.Contract.Services.CMS.Dtos;
using Cms.Contract.Services.CMS.Interfaces;
using Cms.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.CMS
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class CMSController : ApiControllerBase
    {
        private readonly ICMSService _cmsService;

        public CMSController(ICMSService cmsService)
        {
            _cmsService = cmsService;
        }

        [Authorize(Policy = "Permission.Content.View")]
        [HttpGet("content-types/{typeId:guid}/contents")]
        public async Task<IActionResult> ListContents(Guid typeId)
        {
            var serviceResponse = await _cmsService.ListContentsAsync(typeId, GetCurrentAccountId());

            return Ok(new ApiResponse<IEnumerable<GetContentsResponseModel>>
            {
                Success = true,
                Data = serviceResponse.Select(content => new GetContentsResponseModel
                {
                    ContentId = content.ContentId,
                    RevisionId = content.RevisionId,
                    Version = content.Version,
                    OwnerUsername = content.OwnerUsername,
                    Status = content.Status,
                    CreatedAt = content.CreatedAt,
                    FieldValues = content.FieldValues
                        .Select(field => new GetContentFieldValueModel
                        {
                            FieldId = field.FieldId,
                            FieldName = field.FieldName,
                            FieldType = field.FieldType,
                            FieldValue = field.FieldValue
                        })
                        .ToList()
                }).ToList()
            });
        }

        [Authorize(Policy = "Permission.Content.Create")]
        [HttpPost("content-types/{typeId:guid}/contents")]
        public async Task<IActionResult> CreateContent(Guid typeId, [FromBody] CreateContentRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var serviceResponse = await _cmsService.CreateContentAsync(
                new CreateContentRequestDto
                {
                    TypeId = typeId,
                    OwnerId = GetCurrentAccountId(),
                    FieldValues = requestModel.FieldValues
                        .Select(fieldValue => new CreateContentFieldValueDto
                        {
                            FieldId = fieldValue.FieldId,
                            FieldValue = fieldValue.FieldValue
                        })
                        .ToList()
                }
            );

            if (serviceResponse.Result != CreateContentResult.Success)
            {
                return ErrorResponse(serviceResponse.Result.ToString());
            }

            return Created($"/api/contents/{serviceResponse.ContentId}", new ApiResponse<CreateContentResponseModel>
            {
                Success = true,
                Data = new CreateContentResponseModel
                {
                    ContentId = serviceResponse.ContentId!.Value,
                    RevisionId = serviceResponse.RevisionId!.Value
                }
            });
        }

        [Authorize(Policy = "Permission.Content.Edit")]
        [HttpPut("contents/{contentId:guid}")]
        public async Task<IActionResult> UpdateContent(Guid contentId, [FromBody] UpdateContentRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var serviceResponse = await _cmsService.UpdateContentAsync(
                new UpdateContentRequestDto
                {
                    ContentId = contentId,
                    RevisionId = requestModel.RevisionId,
                    FieldValues = requestModel.FieldValues
                        .Select(fieldValue => new UpdateContentFieldValueDto
                        {
                            FieldId = fieldValue.FieldId,
                            FieldValue = fieldValue.FieldValue
                        })
                        .ToList()
                },
                GetCurrentAccountId()
            );

            if (serviceResponse.Result != UpdateContentResult.Success)
            {
                return ErrorResponse(serviceResponse.Result.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true
            });
        }

        [Authorize(Policy = "Permission.Content.Delete")]
        [HttpDelete("contents/{contentId:guid}")]
        public async Task<IActionResult> DeleteContent(Guid contentId)
        {
            var serviceResponse = await _cmsService.DeleteContentAsync(
                new DeleteContentRequestDto
                {
                    ContentId = contentId
                },
                GetCurrentAccountId()
            );

            if (serviceResponse.Result != DeleteContentResult.Success)
            {
                return ErrorResponse(serviceResponse.Result.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true
            });
        }

        [Authorize(Policy = "Permission.Content.View")]
        [HttpGet("content-types")]
        public async Task<IActionResult> ListContentTypeLookups([FromQuery] ListContentTypeLookupsRequestModel req)
        {
            var dto = new ListContentTypeLookupsRequestDto
            {
                DepartmentId = req.DepartmentId
            };

            var rdto = await _cmsService.ListContentTypeLookupsAsync(dto, GetCurrentAccountId());

            var res = rdto.Select(x => new ContentTypeLookupResponseModel
            {
                TypeId = x.TypeId,
                TypeName = x.TypeName
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<ContentTypeLookupResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpPost("content-types")]
        public async Task<IActionResult> CreateContentType([FromBody] CreateContentTypeRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _cmsService.CreateContentTypeAsync(
                new CreateContentTypeRequestDto
                {
                    DepartmentId = req.DepartmentId,
                    TypeName = req.TypeName,
                    Fields = req.Fields
                        .Select(x => new CreateContentTypeFieldDto
                        {
                            FieldName = x.FieldName,
                            FieldType = x.FieldType,
                            IsRequired = x.IsRequired
                        })
                        .ToList()
                }
            );

            if (rdto.Result != CreateContentTypeResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Created($"/api/content-types/{rdto.TypeId}", new ApiResponse<CreateContentTypeResponseModel>
            {
                Success = true,
                Data = new CreateContentTypeResponseModel
                {
                    TypeId = rdto.TypeId!.Value,
                    TypeName = rdto.TypeName!
                }
            });
        }

        [Authorize(Policy = "Permission.Content.View")]
        [HttpGet("content-types/{typeId:guid}/fields")]
        public async Task<IActionResult> ListContentFields(Guid typeId)
        {
            var rdto = await _cmsService.ListContentFieldsAsync(typeId, GetCurrentAccountId());

            var res = rdto.Select(x => new GetContentFieldsResponseModel
            {
                FieldId = x.FieldId,
                FieldName = x.FieldName,
                FieldCode = x.FieldCode,
                FieldType = x.FieldType,
                IsRequired = x.IsRequired
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<GetContentFieldsResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpPut("content-types/{typeId:guid}")]
        public async Task<IActionResult> UpdateContentType(Guid typeId, [FromBody] UpdateContentTypeRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _cmsService.UpdateContentTypeAsync(
                new UpdateContentTypeRequestDto
                {
                    TypeId = typeId,
                    TypeName = req.TypeName,
                    Fields = req.Fields
                        .Select(x => new UpdateContentTypeFieldDto
                        {
                            FieldId = x.FieldId,
                            FieldName = x.FieldName,
                            FieldType = x.FieldType,
                            IsRequired = x.IsRequired
                        })
                        .ToList()
                }
            );

            if (rdto.Result != UpdateContentTypeResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true
            });
        }

        [HttpDelete("content-types/{typeId:guid}")]
        public async Task<IActionResult> DeleteContentType(Guid typeId)
        {
            var rdto = await _cmsService.DeleteContentTypeAsync(
                new DeleteContentTypeRequestDto
                {
                    TypeId = typeId
                }
            );

            if (rdto.Result != DeleteContentTypeResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true
            });
        }
    }
}
