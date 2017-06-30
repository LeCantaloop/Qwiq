namespace Microsoft.Qwiq
{
    internal class IdentityDescriptorComparer : GenericComparer<IIdentityDescriptor>
    {
        internal new static IdentityDescriptorComparer Default => Nested.Instance;

        private IdentityDescriptorComparer()
        {

        }

        // ReSharper disable ClassNeverInstantiated.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class Nested
            // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly IdentityDescriptorComparer Instance = new IdentityDescriptorComparer();
            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }
        }
    }
}