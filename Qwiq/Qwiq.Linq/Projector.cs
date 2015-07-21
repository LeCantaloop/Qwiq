using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.IE.Qwiq.Linq
{
    public static class Projector
    {
        public static object Project(IEnumerable<LambdaExpression> projections, IEnumerable<object> data)
        {
            var projectedData = data;
            foreach (var projection in projections)
            {
                var compiledProjection = projection.Compile();
                projectedData = projectedData.Select(r => compiledProjection.DynamicInvoke(r));
            }

            return projectedData;
        }
    }
}
