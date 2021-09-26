// Copyright (c) 2021 DrBarnabus

using Cake.Frosting;
using Common.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Common.Extensions
{
    public static class TypeExtensions
    {
        public static string GetTaskDescription(this Type task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));

            var attribute = task.GetCustomAttribute<TaskDescriptionAttribute>();
            return attribute != null ? attribute.Description : string.Empty;
        }

        public static string GetTaskName(this Type task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));

            var attribute = task.GetCustomAttribute<TaskNameAttribute>();
            return attribute != null ? attribute.Name : task.Name;
        }

        public static string GetTaskArguments(this Type task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));

            var attributes = task.GetCustomAttributes<TaskArgumentAttribute>().ToArray();
            if (!attributes.Any())
                return string.Empty;

            var arguments = attributes.Select(attribute => $"[--{attribute.Name} ({string.Join(" | ", attribute.PossibleValues)})]");
            return string.Join(" ", arguments);
        }
    }
}
