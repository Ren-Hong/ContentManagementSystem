using Cms.Contract.Repositories.UnitOfWork.Interfaces;
using System.Data;

namespace Cms.Infrastructure.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;

        public IDbTransaction? Transaction { get; private set; }

        public UnitOfWork(IDbConnection connection)
        {
            _connection = connection;
        }

        public void BeginTransaction()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            Transaction = _connection.BeginTransaction(); // ?™æ®µ?ƒçµ¦ Tx äº¤æ??©å»º
        }

        public void Commit()
        {
            try
            {
                Transaction?.Commit();
            }
            finally
            {
                Transaction?.Dispose();
                Transaction = null;
            }
        }

        public void Rollback()
        {
            try
            {
                Transaction?.Rollback();
            }
            finally
            {
                Transaction?.Dispose();
                Transaction = null;
            }
        }
    }
}

