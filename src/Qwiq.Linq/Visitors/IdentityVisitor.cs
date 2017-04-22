using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq.Visitors
{
    public class IdentityVisitor : ExpressionVisitor
    {
        private static readonly HashSet<string> IdentityProperties;

        private bool _needsIdentityMapping;

        static IdentityVisitor()
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

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _needsIdentityMapping = NeedsIdentityMapping(new[] { node.Left, node.Right });

            var newNode = base.VisitBinary(node);
            _needsIdentityMapping = false;

            return newNode;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (!_needsIdentityMapping) return base.VisitConstant(node);

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

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _needsIdentityMapping = NeedsIdentityMapping(node.Arguments);

            // TODO: Support cases: item["Assigned To"] and item.Fields["Assigned To"]
            var newNode = base.VisitMethodCall(node);
            _needsIdentityMapping = false;

            return newNode;
        }

        private static bool NeedsIdentityMapping(IEnumerable<Expression> expressions)
        {
            return expressions.OfType<MemberExpression>().Any(arg => IdentityProperties.Contains(arg.Member.Name));
        }
    }
}