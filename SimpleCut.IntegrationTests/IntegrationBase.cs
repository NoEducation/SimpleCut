using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Respawn;
using Respawn.Graph;
using SimpleCut.Common.Exceptions;
using SimpleCut.DbMigrator;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Infrastructure.Cqrs;
using SimpleCut.Infrastructure.Dependency;
namespace SimpleCut.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected IConfiguration Configuration { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IDbContext Context { get; private set; }
        protected IDispatcher Dispatcher { get; private set; }

        private static Checkpoint _checkpoint;
        private string _connectionString;

        // TODO.DA musze dodać tu jakiegoś uzytkownika testowego do bazy
        [OneTimeSetUp]
        public void OnTimeSetUp()
        {
            Configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", false)
              .AddJsonFile("appsettings.Development.json", true)
              .Build();

            _connectionString = Configuration.GetConnectionString("DefaultConnection");

            ServiceProvider = new ServiceCollection()
                .AddScoped<IDbContext>(
                     x => new DbContext(_connectionString))
                .AddCqrs()
                .AddServices()
                .AddOptions()
                .BuildServiceProvider();

            UpgradeDatabase();
        }

        [SetUp]
        public Task SetUp()
        {
            Context = ServiceProvider.GetService<IDbContext>();
            Dispatcher = ServiceProvider.GetService<IDispatcher>();

            _checkpoint = new Checkpoint
            {
                //TablesToIgnore = new string[] { "schemaversions" },
                TablesToIgnore = new Table[]
                {
                    new Table("schemaversions")
                },
                DbAdapter = DbAdapter.Postgres,
            };

            return Task.CompletedTask;
        }

        [TearDown]
        public async Task TearDown()
        {
            var keepData = Configuration.GetValue<bool>("Database:KeepDatabaseStateAfterTestRun");

            if (!keepData)
                await ResetStateAsync();

            if (Context != null)
                Context.Dispose();
        }

        private void UpgradeDatabase()
        {
            var migrator = new DbUpgrader();

            var result = migrator.UpgradeDatabase(_connectionString, false);

            if (!result.Successful)
                throw new SimpleCutExecutionException(result.Error.Message);
        }

        private async Task ResetStateAsync()
        {
            await Context.Connection.OpenAsync();

            //TODO.DA command psql -U postgres -d lportal -c "GRANT ALL PRIVILEGES ON TABLE pg_catalog.pg_largeobject TO postgres;"
            // should be executed
            await _checkpoint.Reset(Context.Connection);

            await Context.Connection.CloseAsync();
        }
    }
}
