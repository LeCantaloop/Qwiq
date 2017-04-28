using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.Qwiq.Identity;
using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Linq.Visitors
{
    /// <summary>
    ///     Represents a visitor used to detect and rewrite identity values in the expression tree.
    /// </summary>
    /// <seealso cref="IdentityComboStringVisitor" />
    public class IdentityFieldAttributeVisitor : ExpressionVisitor
    {
        private readonly IIdentityValueConverter _valueConverter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityFieldAttributeVisitor" /> class.
        /// </summary>
        /// <param name="valueConverter">An instance of <see cref="IIdentityValueConverter" /> used to convert identity values.</param>
        public IdentityFieldAttributeVisitor(IIdentityValueConverter valueConverter)
        {
            Contract.Requires(valueConverter != null);
            
            _valueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
        }

        private bool NeedsIdentityMapping { get; set; }

        /// <summary>
        ///     Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            NeedsIdentityMapping = ExpressionsNeedIdentityMapping(new[] { node.Left, node.Right });

            var newNode = base.VisitBinary(node);
            NeedsIdentityMapping = false;

            return newNode;
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

        /// <summary>
        ///     Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            NeedsIdentityMapping = ExpressionsNeedIdentityMapping(node.Arguments);

            var newNode = base.VisitMethodCall(node);
            NeedsIdentityMapping = false;

            return newNode;
        }

        private static bool ExpressionsNeedIdentityMapping(IEnumerable<Expression> expressions)
        {
            return expressions.OfType<MemberExpression>().Any(arg => IsIdentityField(arg.Expression.Type, arg.Member.Name));
        }

        private static bool IsIdentityField(Type type, string propertyName)
        {
            // REVIEW: Use IPropertyInspector instead of reflection
            var property = type.GetProperty(propertyName);

            var customAttributes = Enumerable.Empty<IdentityFieldAttribute>();
            if (property != null)
                customAttributes = property.GetCustomAttributes(typeof(IdentityFieldAttribute), true).Cast<IdentityFieldAttribute>();
            return customAttributes.Any();
        }
    }
}