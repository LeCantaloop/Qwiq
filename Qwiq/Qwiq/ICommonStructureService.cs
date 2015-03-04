using System;

namespace Microsoft.IE.Qwiq
{
    public interface ICommonStructureService
    {
        string CreateNode(string nodeName, string parentNodeUri, DateTime? startDate, DateTime? finishDate);
        void SetIterationDates(string nodeUri, DateTime? startDate, DateTime? finishDate);
    }
}
