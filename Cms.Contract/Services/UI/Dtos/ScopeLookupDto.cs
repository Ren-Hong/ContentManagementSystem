namespace Cms.Contract.Services.UI.Dtos
{
    public class ScopeLookupDto
    {
        public required Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }

        public required string ScopeCode { get; set; }
    }
}

