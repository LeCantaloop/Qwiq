using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Linq
{
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