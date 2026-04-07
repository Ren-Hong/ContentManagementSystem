namespace Cms.Contract.Services.RBAC.Dtos
{
    public class ScopeOptionDto
    {
        public Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }

        public required string ScopeCode { get; set; }
    }
}

