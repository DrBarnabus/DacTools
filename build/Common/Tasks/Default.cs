// Copyright (c) 2022 DrBarnabus

using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Frosting;
using Common.Extensions;
using System;
using System.Linq;
using System.Reflection;

// ReSharper disable UnusedType.Global

namespace Common.Tasks
{
    [TaskName(nameof(Default))]
    [TaskDescription("Shows this output")]
    public class Default : FrostingTask
    {
        public override void Run(ICakeContext context)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var tasks = entryAssembly?.FindAllDerivedTypes(typeof(IFrostingTask)).Where(x => !x.Name.Contains("Internal")).ToList();
            if (tasks == null) return;

            var defaultTask = tasks.Find(x => x.Name.Contains(nameof(Default)));
            if (tasks.Remove(defaultTask))
            {
                tasks.Insert(0, defaultTask);
            }

            context.Information($"Available targets:{Environment.NewLine}");
            foreach (var task in tasks)
            {
                context.Information($"# {task.GetTaskDescription()}");

                string taskName = task.GetTaskName();
                string target = taskName != nameof(Default) ? $"--target {taskName}" : string.Empty;
                context.Information($"\tdotnet ../run/{entryAssembly?.GetName().Name}.dll {target} {task.GetTaskArguments()}\n");
            }
        }
    }
}
