using System.Runtime.InteropServices;

namespace Microsoft.Qwiq.Mocks
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct WorkItemTypeDefinitions
    {
        public const string Task = "Task";

        public const string Deliverable = "Deliverable";

        public const string Scenario = "Scenario";

        public const string CustomerPromise = "Customer Promise";

        public const string Bug = "Bug";

        public const string Measure = "Measure";
    }
}