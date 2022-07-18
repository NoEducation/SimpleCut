using Npgsql;
using System.Transactions;

namespace SimpleCut.Infrastructure.Context
{
    public class DbContext : IDbContext
    {
        private NpgsqlConnection _connection;
        private TransactionScope? _transaction;
        private bool disposed = false;

        public NpgsqlConnection Connection { get => _connection; }

        public DbContext(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        public void BeginTransaction()
        {
            if(_transaction is not null)
            {
                throw new InvalidOperationException("One transation is still going. Postgre does not support multiple open transations");
            }

            _transaction = new TransactionScope(TransactionScopeOption.Required,
                    TransactionScopeAsyncFlowOption.Enabled);
        }

        public void CommitTransaction()
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("There is no open transation.");
            }
            
            this._transaction.Complete();
            this._transaction.Dispose();
            this._transaction = null;
        }

        public void RollbackTransaction()
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("There is no open transation.");
            }

            _transaction.Dispose();
            _transaction = null;
        }

        [Obsolete("It will be removed")]
        public async Task ExecuteInTransation(params Action<NpgsqlConnection>[] batches)
        {
            await _connection.OpenAsync();

            using var transation = await _connection.BeginTransactionAsync();

            foreach (var batch in batches)
            {
                batch.Invoke(_connection);
            }

            await transation.CommitAsync();
            await _connection.CloseAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this._transaction?.Dispose();
                    this._connection?.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
