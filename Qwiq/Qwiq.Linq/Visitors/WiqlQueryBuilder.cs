using System.Linq.Expressions;

namespace Microsoft.IE.Qwiq.Linq.Visitors
{
    public class WiqlQueryBuilder : IWiqlQueryBuilder
    {
        private readonly IWiqlTranslator _translator;
        private readonly ExpressionVisitor[] _visitors;

        public WiqlQueryBuilder(IWiqlTranslator translator, params ExpressionVisitor[] visitors)
        {
            _translator = translator;
            _visitors = visitors;
        }

        public TranslatedQuery BuildQuery(Expression expression)
        {
            foreach (var visitor in _visitors)
            {
                expression = visitor.Visit(expression);
            }

            return _translator.Translate(expression);
        }
    }
}
