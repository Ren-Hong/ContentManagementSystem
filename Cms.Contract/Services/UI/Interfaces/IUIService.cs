using Cms.Contract.Services.UI.Dtos;

namespace Cms.Contract.Services.UI.Interfaces
{
    public interface IUIService
    {
        Task<List<RoleLookupDto>> ListRoleLookupsAsync();

        Task<List<PermissionLookupDto>> ListPermissionLookupsAsync();

        Task<List<ScopeLookupDto>> ListScopeLookupsAsync();

        Task<List<DepartmentLookupDto>> ListDepartmentLookupsAsync();
    }
}

