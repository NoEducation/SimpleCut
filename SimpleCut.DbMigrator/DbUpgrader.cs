using DbUp;
using DbUp.Engine;

namespace SimpleCut.DbMigrator
{
    public class DbUpgrader
    {
        public List<SqlScript> allExecutedScripts = new List<SqlScript>();

        public DatabaseUpgradeResult UpgradeDatabase(string connectionString, string path)
        {
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
                .WithScriptsFromFileSystem(filesInPath);

            var result = builder.Build().PerformUpgrade();

            allExecutedScripts.AddRange(result.Scripts);

            return result;
        }
    }
}
