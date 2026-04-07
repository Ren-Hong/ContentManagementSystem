namespace Cms.Contract.Controllers.RBAC.Models
{
    public class ScopeOptionResponseModel
    {
        public required Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }

        public required string ScopeCode { get; set; }
    }
}

