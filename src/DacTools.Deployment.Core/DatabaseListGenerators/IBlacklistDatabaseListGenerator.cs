// Copyright (c) 2019 DrBarnabus

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DacTools.Deployment.Core.DatabaseListGenerators
{
    public interface IBlacklistDatabaseListGenerator
    {
        Task<List<DatabaseInfo>> GetDatabaseInfoListAsync(IReadOnlyList<string> databaseNames = null);
    }
}
