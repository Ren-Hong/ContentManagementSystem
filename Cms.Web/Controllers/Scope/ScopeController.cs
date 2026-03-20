using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Scope.Models;
using Cms.Contract.Services.Scope.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Scope
{
    [ApiController]
    [Route("api/scopes")]
    [Authorize(Roles = "Admin")]
    public class ScopeController : ControllerBase
    {
        private readonly IScopeService _scopeService;

        public ScopeController(IScopeService scopeService)
        {
            _scopeService = scopeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListScopeLookups()
        {
            var rdto = await _scopeService.ListScopeLookupsAsync();

            var res = rdto.Select(x => new ScopeLookupResponseModel
            {
                ScopeName = x.ScopeName,
                ScopeCode = x.ScopeCode,
                ScopeId = x.ScopeId,
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<ScopeLookupResponseModel>>
            {
                Success = true,
                Data = res
            });
        }
    }
}
