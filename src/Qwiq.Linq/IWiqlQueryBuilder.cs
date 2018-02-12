using System.Linq.Expressions;

namespace Qwiq.Linq
{
    public interface IWiqlQueryBuilder
    {
        TranslatedQuery BuildQuery(Expression expression);
    }
}

