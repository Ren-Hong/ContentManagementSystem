namespace Cms.Contract.Repositories.Department.Entities
{
    public class DepartmentLookupEntity
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }
    }
}
