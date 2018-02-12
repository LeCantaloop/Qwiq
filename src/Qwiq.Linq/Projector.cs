using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

using JetBrains.Annotations;

namespace Qwiq.Linq
{
    public static class Projector
    {
        public static object Project([NotNull] IEnumerable<LambdaExpression> projections, [NotNull] IEnumerable<object> data)
        {
            if (projections == null) throw new ArgumentNullException(nameof(projections));
            if (data == null) throw new ArgumentNullException(nameof(data));
            Contract.Requires(projections != null);
            Contract.Requires(data != null);

            var projectedData = data;
            foreach (var projection in projections)
            {
                Debug.Assert(projection != null, "projection != null");
                var compiledProjection = projection.Compile();
                projectedData = projectedData.Select(r => compiledProjection.DynamicInvoke(r));
            }

            return projectedData;
        }
    }
}

