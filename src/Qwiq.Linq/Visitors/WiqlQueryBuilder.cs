using System;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq.Visitors
{
    public class WiqlQueryBuilder : IWiqlQueryBuilder
    {
        private readonly IWiqlTranslator _translator;

        private readonly ExpressionVisitor[] _visitors;

        /// <summary>
        /// Initializes a new instance of the <see cref="WiqlQueryBuilder"/> class.
        /// </summary>
        /// <param name="translator">The translator used to visit the <see cref="Expression"/> tree.</param>
        public WiqlQueryBuilder(IWiqlTranslator translator)
            : this(translator, new PartialEvaluator(), new IdentityComboStringVisitor(), new QueryRewriter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WiqlQueryBuilder"/> class.
        /// </summary>
        /// <param name="translator">The translator used to visit the <see cref="Expression"/> tree.</param>
        /// <param name="visitors">The visitors used to visit the <see cref="Expression"/> during <see cref="BuildQuery"/>.</param>
        public WiqlQueryBuilder(IWiqlTranslator translator, params ExpressionVisitor[] visitors)
        {
            _translator = translator ?? throw new ArgumentNullException(nameof(translator));
            _visitors = visitors ?? throw new ArgumentNullException(nameof(visitors));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WiqlQueryBuilder" /> class with a default instance of
        ///     <see cref="WiqlTranslator" />.
        /// </summary>
        public WiqlQueryBuilder()
            : this(new WiqlTranslator())
        {
        }

        public TranslatedQuery BuildQuery(Expression expression)
        {
            // Each visitor fist the expression
            expression = _visitors.Aggregate(expression, (current, visitor) => visitor.Visit(current));

            return _translator.Translate(expression);
        }
    }
}