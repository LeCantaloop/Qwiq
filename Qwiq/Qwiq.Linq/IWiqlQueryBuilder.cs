using System.Linq.Expressions;

namespace Microsoft.IE.Qwiq.Linq
{
    public interface IWiqlQueryBuilder
    {
        TranslatedQuery BuildQuery(Expression expression);
    }
}
