using Cms.Contract.Repositories.Department.Interfaces;
using Cms.Contract.Repositories.Permission.Interfaces;
using Cms.Contract.Repositories.Role.Interfaces;
using Cms.Contract.Repositories.Scope.Interfaces;
using Cms.Contract.Services.UI.Dtos;
using Cms.Contract.Services.UI.Interfaces;

namespace Cms.Application.Services.UI
{
    public class UIService : IUIService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IScopeRepository _scopeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public UIService(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IScopeRepository scopeRepository,
            IDepartmentRepository departmentRepository
        )
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _scopeRepository = scopeRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<List<RoleLookupDto>> ListRoleLookupsAsync()
        {
            var rows = await _roleRepository.ListRoleLookupsAsync();

            return rows
                .Select(x => new RoleLookupDto
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName
                })
                .ToList();
        }

        public async Task<List<PermissionLookupDto>> ListPermissionLookupsAsync()
        {
            var rows = await _permissionRepository.ListPermissionLookupsAsync();

            return rows
                .Select(x => new PermissionLookupDto
                {
                    PermissionId = x.PermissionId,
                    PermissionName = x.PermissionName
                })
                .ToList();
        }

        public async Task<List<ScopeLookupDto>> ListScopeLookupsAsync()
        {
            var rows = await _scopeRepository.ListScopeLookupsAsync();

            return rows
                .Select(x => new ScopeLookupDto
                {
                    ScopeId = x.ScopeId,
                    ScopeCode = x.ScopeCode,
                    ScopeName = x.ScopeName
                })
                .ToList();
        }

        public async Task<List<DepartmentLookupDto>> ListDepartmentLookupsAsync()
        {
            var rows = await _departmentRepository.ListDepartmentLookupsAsync();

            return rows
                .Select(x => new DepartmentLookupDto
                {
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.DepartmentName
                })
                .ToList();
        }
    }
}

