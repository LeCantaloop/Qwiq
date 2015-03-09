namespace Microsoft.IE.Qwiq.UnitTests.Mocks
{
    public class MockIdentityDescriptor : IIdentityDescriptor
    {
        public string Identifier
        {
            get { return "01234567-89AB-CDEF-0A1B-2C3D4FAABBCC\\IDontExist@toplevel.domain"; }
        }

        public string IdentityType
        {
            get { return "Microsoft.IdentityModel.Claims.ClaimsIdentity"; }
        }
    }
}