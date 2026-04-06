using Cms.Contract.Repositories.Account.Persistence;

namespace Cms.Contract.Controllers.RBAC.Models
{
    public class AccountListResponseModel
    {
        public required Guid AccountId { get; set; }

        public required string Username { get; set; }

        public AccountStatus Status { get; set; }

        public required List<AccountRoleAssignmentResponseModel> RoleAssignments { get; set; }
    }

    public class AccountRoleAssignmentResponseModel
    {
        public required Guid RoleId { get; set; }
        
        public required string RoleName { get; set; }

        public required List<DepartmentLookupResponseModel> Departments { get; set; }
    }
}
