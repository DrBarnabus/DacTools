// Copyright (c) 2021 DrBarnabus

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> FindAllDerivedTypes(this Assembly assembly, Type baseType) =>
            assembly.GetExportedTypes()
                .Select(type => new { type, info = type.GetTypeInfo() })
                .Where(t => baseType.IsAssignableFrom(t.type) && t.info.IsClass && !t.info.IsAbstract)
                .Select(t => t.type);
    }
}
