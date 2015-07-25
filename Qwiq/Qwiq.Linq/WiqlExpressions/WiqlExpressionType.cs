namespace Microsoft.IE.Qwiq.Linq.WiqlExpressions
{
    internal enum WiqlExpressionType
    {
        Where = 1000, // Start with a high number to make sure our enum values don't collide
        In,
        Under,
        Order,
        AsOf,
        Contains,
        Select
    }
}
