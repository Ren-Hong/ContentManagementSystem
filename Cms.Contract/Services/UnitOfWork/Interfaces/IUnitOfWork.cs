using System.Data;

namespace Cms.Contract.Services.UnitOfWork.Interfaces
{
    /// <summary>
    /// 提供交易生命週期管理操作。
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 取得目前作用中的交易物件；若尚未開始交易則為 <c>null</c>。
        /// </summary>
        IDbTransaction? Transaction { get; }

        /// <summary>
        /// 開始新的交易。
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 提交目前交易。
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滾目前交易。
        /// </summary>
        void Rollback();
    }
}
