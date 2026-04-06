using System.Security.Claims;
using Cms.Contract.Controllers.Api;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Api
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult ValidationErrorResponse()
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

        protected IActionResult ErrorResponse(string errorCode)
        {
            return StatusCode(MapStatusCode(errorCode), new ApiResponse
            {
                Success = false,
                ErrorCode = errorCode
            });
        }

        protected Guid GetCurrentAccountId()
        {
            var ownerClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(ownerClaim, out var accountId)
                ? accountId
                : Guid.Empty;
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
