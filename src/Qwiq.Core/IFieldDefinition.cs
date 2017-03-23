using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Qwiq
{
    public interface IFieldDefinition
    {
        string Name { get; }
        string ReferenceName { get; }
    }
}
