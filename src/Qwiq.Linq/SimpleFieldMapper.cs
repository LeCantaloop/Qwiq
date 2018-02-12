using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwiq.Linq
{
    // REVIEW: A new type should be created that retrieves the field collection from the WIT


    // TODO: Create a new field mapper that translates known IWorkItem properties to their ReferenceName values

    public class SimpleFieldMapper : IFieldMapper
    {
        private static readonly Dictionary<string, string> Mappings = new Dictionary<string, string>
        {
            {"AreaPath", "Area Path"},
            {"AssignedTo", "Assigned To"},
            {"ChangedBy", "Changed By"},
            {"ChangedDate", "Changed Date"},
            {"CreatedBy", "Created By"},
            {"CreatedDate", "Created Date"},
            {"IterationPath", "Iteration Path"},
            {"RevisedDate", "Revised Date"}
        };

        // REVIEW: Replace with more constrained set of fields
        private static readonly IEnumerable<string> FieldNames = new[] {"*"};

        public IEnumerable<string> GetWorkItemType(Type type)
        {
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetFieldNames(Type type)
        {
            return FieldNames;
        }

        public string GetFieldName(Type type, string propertyName)
        {
            return Mappings.TryGetValue(propertyName, out string name) ? name : propertyName;
        }
    }
}