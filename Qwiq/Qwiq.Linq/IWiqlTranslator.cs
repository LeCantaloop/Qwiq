using System.Linq.Expressions;

namespace Microsoft.IE.Qwiq.Linq
{
    public interface IWiqlTranslator
    {
        TranslatedQuery Translate(Expression expression);
    }
}