using Microsoft.IE.Qwiq.Linq;
using Microsoft.IE.Qwiq.Relatives.WiqlExpressions;

namespace Microsoft.IE.IEPortal.Data.TeamFoundationServer.Linq
{
    public class RelativesAwareTranslatedQuery : TranslatedQuery
    {
        public RelativesExpression Relatives { get; set; }
    }
}