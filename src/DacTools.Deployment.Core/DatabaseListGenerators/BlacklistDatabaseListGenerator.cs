// Copyright (c) 2019 DrBarnabus

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment.Core.DatabaseListGenerators
{
    public class BlacklistDatabaseListGenerator : IBlacklistDatabaseListGenerator
    {
        private const string QueryText = "SELECT database_id, name FROM sys.databases WHERE owner_sid <> 0x01 AND name <> 'master'";

        private readonly Arguments _arguments;

        public BlacklistDatabaseListGenerator(IOptions<Arguments> arguments)
        {
            _arguments = arguments.Value;
        }

        public async Task<List<DatabaseInfo>> GetDatabaseInfoListAsync(IReadOnlyList<string> databaseNames = null)
        {
            var allDatabases = await GetAllNonSystemDatabasesAsync();

            if (databaseNames is null || !databaseNames.Any())
                return allDatabases;
            
            allDatabases.RemoveAll(d => !databaseNames.Contains(d.Name));
            return allDatabases;
        }

        private async Task<List<DatabaseInfo>> GetAllNonSystemDatabasesAsync()
        {
            using (var connection = new SqlConnection(_arguments.MasterConnectionString))
            using (var command = new SqlCommand(QueryText, connection))
            {
                var databaseInfos = new List<DatabaseInfo>();

                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    databaseInfos.Add(new DatabaseInfo(reader.GetInt32(0), reader.GetString(1)));

                return databaseInfos;
            }
        }
    }
}
