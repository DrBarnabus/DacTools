// Copyright (c) 2021 DrBarnabus

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;

namespace DacTools.Deployment.Core.DatabaseListGenerators
{
    public class WhitelistDatabaseListGenerator : IWhitelistDatabaseListGenerator
    {
        private const string QueryText = "SELECT database_id, name FROM sys.databases WHERE database_id > 4 AND name = @DB";

        private readonly Arguments _arguments;

        public WhitelistDatabaseListGenerator(IOptions<Arguments> arguments)
        {
            _arguments = arguments.Value;
        }

        public async Task<List<DatabaseInfo>> GetDatabaseInfoListAsync(IReadOnlyList<string>? databaseNames = null, CancellationToken cancellationToken = default)
        {
            if (databaseNames is null || !databaseNames.Any())
                return new List<DatabaseInfo>();

            var databaseInfos = new List<DatabaseInfo>();

            foreach (string databaseName in databaseNames)
                databaseInfos.Add(await GetDatabaseInfoFromNameAsync(databaseName, cancellationToken));

            return databaseInfos;
        }

        private async Task<DatabaseInfo> GetDatabaseInfoFromNameAsync(string databaseName, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_arguments.MasterConnectionString))
            using (var command = new SqlCommand(QueryText, connection))
            {
                command.Parameters.AddWithValue("@DB", databaseName);

                await command.Connection.OpenAsync(cancellationToken);
                var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                    return new DatabaseInfo(reader.GetInt32(0), reader.GetString(1));

                throw new InvalidOperationException($"Failed to get database info for '{databaseName}'.");
            }
        }
    }
}
