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
using SimpleCut.Services.Accounts;

namespace SimpleCut.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected IConfiguration Configuration { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IDbContext Context { get; private set; }
        protected IDispatcher Dispatcher { get; private set; }

        protected IPasswordHasherService Hasher { get; private set; }

        //TODO.DA jeżeli za każdym razem bede tworzym baze od nowa, drop nie jest to pobrzebne 
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
                .AddCqrsModule()
                .AddServicesModule()
                .AddOptionsModule(Configuration)
                .BuildServiceProvider();

            UpgradeDatabase();
        }

        [SetUp]
        public Task SetUp()
        {
            Context = ServiceProvider.GetService<IDbContext>();
            Dispatcher = ServiceProvider.GetService<IDispatcher>();
            Hasher = ServiceProvider.GetService<IPasswordHasherService>();

            _checkpoint = new Checkpoint
            {
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

            var dropDatabase = Configuration.GetValue<bool>("Database:DropDatabaseEachTests");

            var result = migrator.UpgradeDatabase(_connectionString, dropDatabase);

            if (!result.Successful)
                throw new SimpleCutExecutionException(result.Error.Message);
        }

        private async Task ResetStateAsync()
        {
            await Context.Connection.OpenAsync();

            await _checkpoint.Reset(Context.Connection);

            await Context.Connection.CloseAsync();
        }
    }
}
