using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Relatives.WiqlExpressions;

namespace Microsoft.IE.IEPortal.Data.TeamFoundationServer.Linq
{
    public class RelativesAwareTranslatedQuery : TranslatedQuery
    {
        public RelativesExpression Relatives { get; set; }
    }
}
