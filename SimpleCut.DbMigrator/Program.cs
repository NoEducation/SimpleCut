using Microsoft.Extensions.Configuration;

namespace SimpleCut.DbMigrator
{
    public class Program
    {
        private static string _connectionName = "DefaultConnection";
        private static IConfiguration? _configuration;

        static int Main(string[] args)
        {
            var connectionString = GetConnectionString();
            var dropDatabase = _configuration.GetValue<bool>("DropDatabase");

            var upgrader = new DbUpgrader();

            try
            {
                Console.WriteLine("DB Update connectionString: " + connectionString);

                var result = upgrader.UpgradeDatabase(connectionString,
                    Path.Combine(Directory.GetCurrentDirectory()), dropDatabase);

                if (!result.Successful)
                {
                    FastHandleError(result.Error);
                }

                foreach (var script in result.Scripts)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Executed script: {script.Name}");
                }
            }
            catch (Exception ex)
            {
                FastHandleError(ex);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Db upgrade success!");
            Console.ResetColor();

            #if DEBUG
                Console.ReadLine();
            #endif

            return 0;
        }

        private static void FastHandleError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ResetColor();

            #if DEBUG
                Console.ReadLine();
            #endif

            using (var errWriter = Console.Error)
            {
                errWriter.WriteLine(ex);
                errWriter.Flush();
            }

            Environment.Exit(-1);
        }

        private static string GetConnectionString()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            return _configuration.GetConnectionString(_connectionName);
        }

    }
}