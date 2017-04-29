using System;
using System.Linq.Expressions;

using Microsoft.Qwiq.Identity;

namespace Microsoft.Qwiq.Linq.Visitors
{
    /// <summary>
    ///     Represents a visitor used to detect and rewrite identity values in the expression tree.
    /// </summary>
    /// <seealso cref="IdentityComboStringVisitor" />
    public class IdentityMappingVisitor : IdentityComboStringVisitor
    {
        private readonly IIdentityValueConverter _valueConverter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityMappingVisitor" /> class.
        /// </summary>
        /// <param name="valueConverter">An instance of <see cref="IIdentityValueConverter" /> used to convert identity values.</param>
        /// <exception cref="ArgumentNullException">valueConverter</exception>
        public IdentityMappingVisitor(IIdentityValueConverter valueConverter)
        {
            _valueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
        }

        /// <summary>
        ///     Visits the <see cref="T:System.Linq.Expressions.ConstantExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (!NeedsIdentityMapping) return base.VisitConstant(node);

            var newNode = _valueConverter.Map(node.Value);
            return Expression.Constant(newNode);
        }
    }
}