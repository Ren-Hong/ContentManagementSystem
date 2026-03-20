using Cms.Contract.Repositories.Account.Persistence;
using Cms.Contract.Services.Department.Dtos;

namespace Cms.Contract.Services.Account.Dtos
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

        public required List<DepartmentLookupDto> Departments { get; set; }
    }
}
