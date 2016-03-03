using System.Linq.Expressions;
using Microsoft.IE.IEPortal.Data.TeamFoundationServer.Linq;
using Microsoft.IE.Qwiq.Linq;
using Microsoft.IE.Qwiq.Linq.WiqlExpressions;
using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Relatives.WiqlExpressions;

namespace Microsoft.IE.Qwiq.Relatives.Linq
{
    public class RelativesAwareWiqlTranslator : WiqlTranslator
    {
        public RelativesAwareWiqlTranslator(IFieldMapper fieldMapper) : base(fieldMapper)
        {
        }

        protected override Translator GetTranslator()
        {
            return new RelativesAwareTranslator(FieldMapper);
        }

        protected class RelativesAwareTranslator : Translator
        {
            public RelativesAwareTranslator(IFieldMapper fieldMapper) : base(fieldMapper)
            {
                Query = new RelativesAwareTranslatedQuery();
            }

            public override Expression Visit(Expression expression)
            {
                if (expression == null)
                {
                    return null;
                }

                if ((WiqlExpressionType)expression.NodeType == WiqlExpressionType.Relatives)
                {
                    return VisitParents((RelativesExpression)expression);
                }
                return base.Visit(expression);
            }

            protected virtual Expression VisitParents(RelativesExpression expression)
            {
                var relativeQuery = (RelativesAwareTranslatedQuery) Query;
                if (relativeQuery != null)
                {
                    relativeQuery.Relatives = expression;
                }

                return expression;
            }

            protected override Expression VisitSelect(SelectExpression expression)
            {
                base.VisitSelect(expression);

                var nodeType = ((WiqlExpressionType) expression.Projection.Body.NodeType);
                if (nodeType == WiqlExpressionType.Relatives)
                {
                    Visit(expression.Projection.Body);
                    Query.Projections.RemoveAt(Query.Projections.Count - 1);
                }
                return expression;
            }
        }
    }
}