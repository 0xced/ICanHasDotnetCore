using System;
using System.Data.SqlClient;
using System.Diagnostics;
using DbUp;
using ICanHasDotnetCore.Database.AlwaysRun;

namespace ICanHasDotnetCore.Database
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                var appName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName) ?? "Database";
                Console.Error.WriteLine($"usage: {appName} connection-string");
                return 1;
            }

            var connectionString = args[0];
            Console.WriteLine($"Connection String: {connectionString}");
            Console.WriteLine("Ensuring Database");
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
               .SqlDatabase(connectionString)
               .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
               .LogToConsole()
               .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                Debugger.Break();
                return -1;
            }

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                RemoveKnownReplacementsFromStatistics.Run(con);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
