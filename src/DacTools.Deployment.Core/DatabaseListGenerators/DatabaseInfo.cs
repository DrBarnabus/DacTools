// Copyright (c) 2019 DrBarnabus

namespace DacTools.Deployment.Core.DatabaseListGenerators
{
    public class DatabaseInfo
    {
        public DatabaseInfo(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }

        public override string ToString() => $"{Name}:{Id}";
    }
}
