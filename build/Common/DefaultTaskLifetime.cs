// Copyright (c) 2021 DrBarnabus

using Cake.Core;
using Cake.Frosting;
using Common.Extensions;

namespace Common
{
    public class DefaultTaskLifetime : FrostingTaskLifetime
    {
        public override void Setup(ICakeContext context, ITaskSetupContext info) => context.StartGroup($"Task: {info.Task.Name}");

        public override void Teardown(ICakeContext context, ITaskTeardownContext info) => context.EndGroup();
    }
}
