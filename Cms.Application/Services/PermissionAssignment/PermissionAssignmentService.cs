using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.PermissionAssignment.Interfaces;
using Cms.Contract.Services.PermissionAssignment.Dtos;
using Cms.Contract.Services.PermissionAssignment.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.PermissionAssignment
{
    public class PermissionAssignmentService : IPermissionAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionAssignmentRepository _permissionAssignmentRepository;

        public PermissionAssignmentService(
            IUnitOfWork unitOfWork,
            IPermissionAssignmentRepository permissionAssignmentRepository
        )
        {
            _unitOfWork = unitOfWork;
            _permissionAssignmentRepository = permissionAssignmentRepository;
        }

        public async Task<PagedResult<PermissionAssignmentListResponseDto>> ListPermissionAssignmentsAsync(PageRequest pageRequest)
        {
            var paged = await _permissionAssignmentRepository
                .ListPermissionAssignmentRowsAsync(pageRequest);

            var grouped = paged.Items
                .GroupBy(x => new { x.AccountId, x.Username })
                .Select(accountGroup => new PermissionAssignmentListResponseDto
                {
                    AccountId = accountGroup.Key.AccountId,
                    Username = accountGroup.Key.Username,

                    PermissionDepartments =
                        accountGroup
                        .GroupBy(p => new { p.PermissionId, p.PermissionName })
                        .Select(permissionGroup => new PermissionDepartmentResponseDto
                        {
                            PermissionId = permissionGroup.Key.PermissionId,
                            PermissionName = permissionGroup.Key.PermissionName,

                            Departments =
                                permissionGroup
                                .Select(d => new AssignedDepartmentResponseDto
                                {
                                    DepartmentId = d.DepartmentId,
                                    DepartmentName = d.DepartmentName,
                                    ValidFrom = d.ValidFrom,
                                    ValidTo = d.ValidTo
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            return new PagedResult<PermissionAssignmentListResponseDto>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                Items = grouped
            };
        }
    }
}
