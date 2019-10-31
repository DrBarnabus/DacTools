// Copyright (c) 2019 DrBarnabus

namespace DacTools.Deployment.Core.DatabaseListGenerators
{
    public class DatabaseInfo
    {
        public int Id { get; }
        public string Name { get; }

        public DatabaseInfo(int id, string name) => (Id, Name) = (id, name);
    }
}
