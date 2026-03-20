using Cms.Contract.Repositories.Department.Interfaces;
using Cms.Contract.Services.PermissionScope.Interfaces;
using Cms.Contract.Services.Department.Dtos;
using Cms.Contract.Services.Department.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.Department
{
    public class DepartmentService : IDepartmentService
    {
        private const string ContentViewPermissionCode = "Content.View";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPermissionScopeService _permissionScopeService;

        public DepartmentService(
            IUnitOfWork unitOfWork,
            IDepartmentRepository departmentRepository,
            IPermissionScopeService permissionScopeService
        )
        {
            _unitOfWork = unitOfWork;
            _departmentRepository = departmentRepository;
            _permissionScopeService = permissionScopeService;
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

        public async Task<List<AccessibleDepartmentDto>> ListAccessibleDepartmentsAsync(Guid accountId)
        {
            if (accountId == Guid.Empty)
            {
                return [];
            }

            var hasGlobalScope = await _permissionScopeService.HasGlobalScopeAsync(
                accountId,
                ContentViewPermissionCode
            );

            var rows = hasGlobalScope
                ? await _departmentRepository.ListAccessibleDepartmentsAsync()
                : await GetDepartmentScopedRowsAsync(accountId);

            return rows
                .Select(x => new AccessibleDepartmentDto
                {
                    DepartmentId = x.DepartmentId,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName
                })
                .ToList();
        }

        private async Task<IEnumerable<Cms.Contract.Repositories.Department.Entities.AccessibleDepartmentEntity>> GetDepartmentScopedRowsAsync(Guid accountId)
        {
            var departmentIds = await _permissionScopeService.GetDepartmentScopeDepartmentIdsAsync(
                accountId,
                ContentViewPermissionCode
            );

            if (departmentIds.Count == 0)
            {
                return [];
            }

            return await _departmentRepository.ListAccessibleDepartmentsAsync(departmentIds);
        }
    }
}
