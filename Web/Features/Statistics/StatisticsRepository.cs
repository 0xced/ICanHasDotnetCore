﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ICanHasDotnetCore.NugetPackages;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    public class StatisticsRepository
    {
        // Only log packages found on Nuget.org
        private static readonly SupportType[] AddStatisticsFor = new[]
            {SupportType.Unsupported, SupportType.Supported, SupportType.PreRelease};

        private readonly string _connectionString;

        public StatisticsRepository(IConfigurationRoot configuration)
        {
            _connectionString = configuration["ConnectionString"];
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new Exception("The configuration setting 'ConnectionString' is not set");
        }

        public async Task AddStatisticsForResult(InvestigationResult result)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    var tasks = result.GetAllDistinctRecursive()
                        .Where(p => AddStatisticsFor.Contains(p.SupportType))
                        .Select(p => AddStatistic(con, p))
                        .ToArray();

                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception writing statistics");
            }
        }

        private async Task AddStatistic(SqlConnection con, PackageResult package)
        {
            const string sql = @"MERGE dbo.PackageStatistics AS t
USING
(
    SELECT @name as 'Name', @latestSupportType as 'LatestSupportType'
) AS s
ON t.Name = s.Name

WHEN MATCHED THEN
    UPDATE SET LatestSupportType = s.LatestSupportType, [Count] = [Count] + 1 

WHEN NOT MATCHED THEN
    INSERT (Name, LatestSupportType, Count)
    VALUES (s.Name, s.LatestSupportType, 1);";

            try
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@name", package.PackageName);
                    cmd.Parameters.AddWithValue("@latestSupportType", package.SupportType.ToString());
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception writing statistic {stat}", package.PackageName);
            }
        }
    }
}