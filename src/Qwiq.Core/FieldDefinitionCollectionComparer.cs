using System;
using System.Linq;

namespace Microsoft.Qwiq
{
    public class FieldDefinitionCollectionComparer : GenericComparer<IFieldDefinitionCollection>
    {
        internal static readonly string[] SkippedFields =
            {
                FieldRefNames.AttachedFiles, FieldRefNames.RelatedFiles,
                FieldRefNames.LinkedFiles, FieldRefNames.BisLinks,
                FieldRefNames.RelatedLinks
            };

        internal static readonly Func<IFieldDefinition, bool> SkippedFieldsPredicate =
            p => !SkippedFields.Contains(p.ReferenceName, StringComparer.OrdinalIgnoreCase);

        private FieldDefinitionCollectionComparer()
        {
        }

        public static FieldDefinitionCollectionComparer Instance => Nested.Instance;

        public override bool Equals(IFieldDefinitionCollection x, IFieldDefinitionCollection y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            // The SOAP client does not return four field definitions:
            //  - System.AttachedFiles
            //  - System.RelatedLinks
            //  - System.LinkedFiles
            //  - System.BISLinks
            //  - System.RelatedLinks

            var source = y.Where(SkippedFieldsPredicate).ToList();
            var expected = x.Where(SkippedFieldsPredicate).ToList();

            if (source.Count != expected.Count) return false;

            foreach (var field in expected)
            {
                if (!y.Contains(field.ReferenceName)) return false;

                var tf = y[field.ReferenceName];
                if (!FieldDefinitionComparer.Instance.Equals(field, tf)) return false;

                // Removes the first occurrence, so if there are duplicates we'll still get a valid mismatch
                source.Remove(field);
            }

            // If there are any items left then fail
            if (source.Any()) return false;

            return true;
        }

        public override int GetHashCode(IFieldDefinitionCollection obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            // IMPORTANT: The collections must be in the same order to produce the same hash
            var hash = 27;
            foreach (var definition in obj.Where(SkippedFieldsPredicate).OrderBy(p => p.ReferenceName))
            {
                var defHash = definition.GetHashCode();
                hash = (13 * hash) ^ defHash;
            }

            return hash;
        }

        private class Nested
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly FieldDefinitionCollectionComparer Instance =
                new FieldDefinitionCollectionComparer();

            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}