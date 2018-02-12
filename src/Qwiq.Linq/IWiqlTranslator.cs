using System.Linq.Expressions;

namespace Qwiq.Linq
{
    public interface IWiqlTranslator
    {
        TranslatedQuery Translate(Expression expression);
    }
}
