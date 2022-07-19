using DbUp;
using DbUp.Engine;
using Npgsql;
using System.Reflection;

namespace SimpleCut.DbMigrator
{
    public class DbUpgrader
    {
        public List<SqlScript> allExecutedScripts = new List<SqlScript>();

        public DatabaseUpgradeResult UpgradeDatabase(string connectionString, string path, bool dropDatabase = false)
        {
            if(dropDatabase)
                DropDatabase(connectionString);

            EnsureDatabase.For.PostgresqlDatabase(connectionString);

            DatabaseUpgradeResult upgradeResult = PerformUpgrade(connectionString, path);

            return new DatabaseUpgradeResult(allExecutedScripts, upgradeResult.Successful, upgradeResult.Error, upgradeResult.ErrorScript);
        }

        private DatabaseUpgradeResult PerformUpgrade(string connectionString, string filesInPath)
        {
            var builder = DeployChanges
                .To
                .PostgresqlDatabase(connectionString)
                .LogToConsole()
                .WithTransaction()
                .WithVariablesDisabled()
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly());

            var result = builder.Build().PerformUpgrade();

            allExecutedScripts.AddRange(result.Scripts);

            return result;
        }

        public static void DropDatabase(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);

            var databaseName = builder.Database;
            builder.Database = "postgres";

            using var connection = new NpgsqlConnection(builder.ToString());
            
            connection.Open();

            using (var command = new NpgsqlCommand($"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = \'{databaseName}\'; DROP DATABASE IF EXISTS \"{databaseName}\"", connection))
            {
                command.ExecuteNonQuery();
            }

            connection.Close();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Dropped database {databaseName}!");
            Console.ResetColor();
        }
    }
}
