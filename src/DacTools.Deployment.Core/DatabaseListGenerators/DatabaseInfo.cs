// Copyright (c) 2022 DrBarnabus

namespace DacTools.Deployment.Core.DatabaseListGenerators;

public record DatabaseInfo(int Id, string Name)
{
    public override string ToString()
    {
        return $"{Name}:{Id}";
    }
}
