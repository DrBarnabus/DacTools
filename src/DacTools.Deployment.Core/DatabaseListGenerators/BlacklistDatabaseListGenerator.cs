// Copyright (c) 2021 DrBarnabus

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment.Core.DatabaseListGenerators
{
    public class BlacklistDatabaseListGenerator : IBlacklistDatabaseListGenerator
    {
        private const string QueryText = "SELECT database_id, name FROM sys.databases WHERE database_id > 4";

        private readonly Arguments _arguments;

        public BlacklistDatabaseListGenerator(IOptions<Arguments> arguments)
        {
            _arguments = arguments.Value;
        }

        public async Task<List<DatabaseInfo>> GetDatabaseInfoListAsync(IReadOnlyList<string>? databaseNames = null, CancellationToken cancellationToken = default)
        {
            var allDatabases = await GetAllNonSystemDatabasesAsync(cancellationToken);

            if (databaseNames is null || !databaseNames.Any())
                return allDatabases;

            allDatabases.RemoveAll(d => databaseNames.Contains(d.Name));
            return allDatabases;
        }

        private async Task<List<DatabaseInfo>> GetAllNonSystemDatabasesAsync(CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_arguments.MasterConnectionString))
            using (var command = new SqlCommand(QueryText, connection))
            {
                await command.Connection.OpenAsync(cancellationToken);
                var reader = await command.ExecuteReaderAsync(cancellationToken);

                var databaseInfos = new List<DatabaseInfo>();
                while (await reader.ReadAsync(cancellationToken))
                    databaseInfos.Add(new DatabaseInfo(reader.GetInt32(0), reader.GetString(1)));

                return databaseInfos;
            }
        }
    }
}
