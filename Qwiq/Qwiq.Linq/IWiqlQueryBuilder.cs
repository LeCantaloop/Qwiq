using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq
{
    public interface IWiqlQueryBuilder
    {
        TranslatedQuery BuildQuery(Expression expression);
    }
}

