using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    internal interface IQueryFactory
    {
        IQuery Create(string wiql, bool dayPrecision = false);

        IQuery Create(IEnumerable<int> ids, string wiql);
    }
}

