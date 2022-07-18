using Npgsql;

namespace SimpleCut.Infrastructure.Context
{
    public interface IDbContext : IDisposable
    {
        public NpgsqlConnection Connection { get; }

        public void BeginTransaction();
        public void CommitTransaction();
        public void RollbackTransaction();

        public Task ExecuteInTransation(params Action<NpgsqlConnection>[] batches);
    }
}
