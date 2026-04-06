using Cms.Contract.Repositories.Account.Persistence;

namespace Cms.Contract.Services.RBAC.Dtos
{
    public class AccountListResponseDto
    {
        public required Guid AccountId { get; set; }

        public required string Username { get; set; }

        public AccountStatus Status { get; set; }

        public required List<AccountRoleAssignmentResponseDto> RoleAssignments { get; set; }
    }

    public class AccountRoleAssignmentResponseDto
    {
        public required Guid RoleId { get; set; }
        
        public required string RoleName { get; set; }

        public required List<AssignedDepartmentDto> Departments { get; set; }
    }

    public class AssignedDepartmentDto
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }
    }
}


