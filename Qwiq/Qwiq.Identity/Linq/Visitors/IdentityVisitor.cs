using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.IE.Qwiq.Identity.Attributes;

namespace Microsoft.IE.Qwiq.Identity.Linq.Visitors
{
    public class IdentityVisitor : ExpressionVisitor
    {
        private readonly string[] _domains;
        private readonly IIdentityManagementService _identityManagementService;
        private readonly string _tenantId;
        private bool _needsIdentityMapping;

        public IdentityVisitor(IIdentityManagementService identityManagementService, string tenantId, params string[] domains)
        {
            _identityManagementService = identityManagementService;
            _tenantId = tenantId;
            _domains = domains;
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
            if (!_needsIdentityMapping)
            {
                return node;
            }

            var newNode = ReplaceValue(node.Value);
            return Expression.Constant(newNode);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _needsIdentityMapping = NeedsIdentityMapping(node.Arguments);

            var newNode = base.VisitMethodCall(node);
            _needsIdentityMapping = false;

            return newNode;
        }
        private static bool IsIdentityField(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);

            var customAttributes = Enumerable.Empty<IdentityFieldAttribute>();
            if (property != null)
            {
                customAttributes =
                    property.GetCustomAttributes(typeof(IdentityFieldAttribute), true)
                        .Cast<IdentityFieldAttribute>();
            }
            return customAttributes.Any();
        }

        private static bool NeedsIdentityMapping(IEnumerable<Expression> expressions)
        {
            return expressions.OfType<MemberExpression>().Any(arg => IsIdentityField(arg.Expression.Type, arg.Member.Name));
        }

        private object GetDisplayName(string alias)
        {
            try
            {
                var identity = _identityManagementService.GetIdentityForAlias(alias, _tenantId, _domains);
                return identity?.DisplayName;
            }
            catch (NullReferenceException)
            {
                // User is unknown to TFS
            }
            return null;
        }

        private object ReplaceValue(object value)
        {
            return value is string
                ? GetDisplayName(value.ToString())
                : ((IEnumerable<string>)value)?.Select(GetDisplayName);
        }
    }
}
