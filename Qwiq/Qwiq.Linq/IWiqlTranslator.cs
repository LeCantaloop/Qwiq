using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq
{
    public interface IWiqlTranslator
    {
        TranslatedQuery Translate(Expression expression);
    }
}
