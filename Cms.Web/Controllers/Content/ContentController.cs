using System.Security.Claims;
using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Content.Models;
using Cms.Contract.Services.Content.Dtos;
using Cms.Contract.Services.Content.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Content
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [Authorize(Policy = "Permission.Content.View")]
        [HttpGet("content-types/{typeId:guid}/contents")]
        public async Task<IActionResult> ListContents(Guid typeId)
        {
            var serviceResponse = await _contentService.ListContentsAsync(typeId, GetCurrentAccountId());

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

            var ownerClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerId = Guid.TryParse(ownerClaim, out var parsedOwnerId)
                ? parsedOwnerId
                : Guid.Empty;

            var serviceResponse = await _contentService.CreateContentAsync(
                new CreateContentRequestDto
                {
                    TypeId = typeId,
                    OwnerId = ownerId,
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

            var serviceResponse = await _contentService.UpdateContentAsync(
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
            var serviceResponse = await _contentService.DeleteContentAsync(
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

        private Guid GetCurrentAccountId()
        {
            var ownerClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(ownerClaim, out var accountId)
                ? accountId
                : Guid.Empty;
        }
    }
}
