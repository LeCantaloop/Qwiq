using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Qwiq.Mocks
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CoreFieldDefinitions
    {
        [NotNull]
        public static IFieldDefinition Id { get; } =
            new MockFieldDefinition((int)CoreField.Id, CoreFieldRefNames.NameLookup[CoreFieldRefNames.Id], CoreFieldRefNames.Id);

        [NotNull]
        public static IFieldDefinition WorkItemType { get; } = new MockFieldDefinition(
                                                                                       (int)CoreField.WorkItemType,
                                                                                       CoreFieldRefNames.NameLookup[CoreFieldRefNames
                                                                                                                            .WorkItemType],
                                                                                       CoreFieldRefNames.WorkItemType);

        [ItemNotNull]
        [NotNull]
        public static IEnumerable<IFieldDefinition> All { get; } = CoreFieldRefNames
                .ReferenceNameLookup.Select(s => new MockFieldDefinition(CoreFieldRefNames.CoreFieldIdLookup[s.Value], s.Key, s.Value))
                .ToList();

        [NotNull]
        public static IDictionary<string, IFieldDefinition> NameLookup { get; } =
            All.ToDictionary(k => k.Name, e => e, StringComparer.OrdinalIgnoreCase);

        [NotNull]
        public static IDictionary<string, IFieldDefinition> ReferenceNameLookup { get; } =
            All.ToDictionary(k => k.ReferenceName, e => e, StringComparer.OrdinalIgnoreCase);
    }
}