// Copyright (c) 2021 DrBarnabus

namespace DacTools.Deployment.Core.DatabaseListGenerators
{
    public record DatabaseInfo(int Id, string Name)
    {
        public override string ToString() => $"{Name}:{Id}";
    };
}
