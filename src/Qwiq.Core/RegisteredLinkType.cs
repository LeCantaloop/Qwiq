namespace Microsoft.Qwiq
{
    public class RegisteredLinkType : IRegisteredLinkType
    {
        public RegisteredLinkType(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}