namespace Microsoft.Qwiq
{
    internal interface IQueryFactory
    {
        IQuery Create(string wiql, bool dayPrecision);
    }
}

