using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Qwiq.Linq.Visitors
{
    /// <summary>
    ///     Represents a visitor to detect identity values in the expression tree.
    /// </summary>
    /// <seealso cref="ExpressionVisitor" />
    public class IdentityComboStringVisitor : ExpressionVisitor
    {
        private static readonly HashSet<string> IdentityProperties;

        static IdentityComboStringVisitor()
        {
            IdentityProperties =
                    new HashSet<string>(Comparer.OrdinalIgnoreCase)
                        {
                            "AssignedTo",
                            CoreFieldRefNames.AssignedTo,
                            CoreFieldRefNames.NameLookup[CoreFieldRefNames.AssignedTo],
                            "ChangedBy",
                            CoreFieldRefNames.ChangedBy,
                            CoreFieldRefNames.NameLookup[CoreFieldRefNames.ChangedBy],
                            "CreatedBy",
                            CoreFieldRefNames.CreatedBy,
                            CoreFieldRefNames.NameLookup[CoreFieldRefNames.CreatedBy]
                        };
        }

        /// <summary>
        ///     Gets a value indicating whether the visited expression needs identity mapping.
        /// </summary>
        /// <value><c>true</c> if the expression needs identity mapping; otherwise, <c>false</c>.</value>
        protected virtual bool NeedsIdentityMapping { get; private set; }

        /// <summary>
        /// Determine if the <paramref name="expressions"/> need identity mapping.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <returns><c>true</c> if any of the expressions need identity mapping; otherwise, <c>false</c>.</returns>
        protected virtual bool ExpressionsNeedIdentityMapping(IEnumerable<Expression> expressions)
        {
            return expressions.OfType<MemberExpression>().Any(arg => IdentityProperties.Contains(arg.Member.Name));
        }

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

            void Validate(string identity)
            {
                var i = new IdentityFieldValue(identity);
                if (string.IsNullOrEmpty(i.AccountName))
                    Trace.TraceWarning(
                                       "'{0}' is not a combo-string and may be ambiguous. Use the combo-string to unambiguously refer to an identity.",
                                       identity);
            }

            var v = node.Value;
            if (v is string s) Validate(s);
            else if (v is IEnumerable<string> sv) foreach (var s1 in sv) Validate(s1);
            else Trace.TraceWarning("Unrecognized value in identity field. Type: {0}, Value: {1}", node.Type, node.Value);

            return base.VisitConstant(node);
        }

        /// <summary>
        ///     Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            NeedsIdentityMapping = ExpressionsNeedIdentityMapping(node.Arguments);

            // TODO: Support cases: item["Assigned To"] and item.Fields["Assigned To"]
            var newNode = base.VisitMethodCall(node);
            NeedsIdentityMapping = false;

            return newNode;
        }
    }
}