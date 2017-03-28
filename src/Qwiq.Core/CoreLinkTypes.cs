using System.Runtime.InteropServices;

namespace Microsoft.Qwiq
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CoreLinkTypes
    {
        public const int Related = 1;

        public const int Parent = 2;

        public const int Child = -2;

        public const int Predecessor = 3;

        public const int Successor = -3;
    }
}