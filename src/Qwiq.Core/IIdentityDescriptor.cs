namespace Qwiq
{
    /// <summary>
    /// Represents an identity descriptor <see cref="IdentityType"/> + <see cref="Identifier"/>.
    /// </summary>
    public interface IIdentityDescriptor
    {
        string Identifier { get; }
        string IdentityType { get; }
    }
}
