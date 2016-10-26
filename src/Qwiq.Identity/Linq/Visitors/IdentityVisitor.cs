using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Qwiq.Identity.Attributes;

namespace Microsoft.Qwiq.Identity.Linq.Visitors
{
    public class IdentityVisitor : ExpressionVisitor
    {
        private readonly IIdentityMapper _mapper;
        private bool _needsIdentityMapping;

        public IdentityVisitor(IIdentityMapper mapper)
        {
            _mapper = mapper;
        }

        [Obsolete("Use the overload which takes an IIdentityMapper.")]
        public IdentityVisitor(IIdentityManagementService identityManagementService, string tenantId,
            params string[] domains) : this(new IdentityMapper(identityManagementService, tenantId, domains))
        {
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

            var newNode = _mapper.Map(node.Value);
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
    }
}

