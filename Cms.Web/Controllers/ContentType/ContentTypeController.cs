using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.ContentType.Models;
using Cms.Contract.Services.ContentType.Dtos;
using Cms.Contract.Services.ContentType.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cms.Web.Controllers.ContentType
{
    [ApiController]
    [Route("api/content-types")]
    [Authorize]
    public class ContentTypeController : ControllerBase
    {
        private readonly IContentTypeService _contentTypeService;

        public ContentTypeController(IContentTypeService contentTypeService)
        {
            _contentTypeService = contentTypeService;
        }

        [Authorize(Policy = "Permission.Content.View")]
        [HttpGet]
        public async Task<IActionResult> ListContentTypeLookups([FromQuery] ListContentTypeLookupsRequestModel req)
        {
            var dto = new ListContentTypeLookupsRequestDto
            {
                DepartmentId = req.DepartmentId
            };

            var rdto = await _contentTypeService.ListContentTypeLookupsAsync(dto, GetCurrentAccountId());

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

        [HttpPost]
        public async Task<IActionResult> CreateContentType([FromBody] CreateContentTypeRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _contentTypeService.CreateContentTypeAsync(
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
        [HttpGet("{typeId:guid}/fields")]
        public async Task<IActionResult> ListContentFields(Guid typeId)
        {
            var rdto = await _contentTypeService.ListContentFieldsAsync(typeId, GetCurrentAccountId());

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

        [HttpPut("{typeId:guid}")]
        public async Task<IActionResult> UpdateContentType(Guid typeId, [FromBody] UpdateContentTypeRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _contentTypeService.UpdateContentTypeAsync(
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

        [HttpDelete("{typeId:guid}")]
        public async Task<IActionResult> DeleteContentType(Guid typeId)
        {
            var rdto = await _contentTypeService.DeleteContentTypeAsync(
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

        private Guid GetCurrentAccountId()
        {
            var ownerClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(ownerClaim, out var accountId)
                ? accountId
                : Guid.Empty;
        }

        private IActionResult ValidationErrorResponse()
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                ValidationErrors = ModelState
                    .Where(entry => entry.Value!.Errors.Any())
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToList()
                    )
            });
        }

        private IActionResult ErrorResponse(string errorCode)
        {
            return StatusCode(MapStatusCode(errorCode), new ApiResponse
            {
                Success = false,
                ErrorCode = errorCode
            });
        }

        private static int MapStatusCode(string errorCode)
        {
            if (errorCode.Contains("PermissionDenied", StringComparison.Ordinal))
            {
                return StatusCodes.Status403Forbidden;
            }

            if (errorCode.Contains("NotFound", StringComparison.Ordinal))
            {
                return StatusCodes.Status404NotFound;
            }

            if (
                errorCode.Contains("Required", StringComparison.Ordinal)
                || errorCode.Contains("Invalid", StringComparison.Ordinal)
            )
            {
                return StatusCodes.Status400BadRequest;
            }

            return StatusCodes.Status409Conflict;
        }
    }
}
