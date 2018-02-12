using System.Runtime.InteropServices;

namespace Qwiq.Mapper
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct WorkItemField
    {
        public string Name;
        public string Value;
    }
}
