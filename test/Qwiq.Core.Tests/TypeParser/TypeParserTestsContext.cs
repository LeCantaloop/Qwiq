using Microsoft.Qwiq.Tests.Common;

namespace Microsoft.Qwiq
{
    public abstract class TypeParserTestsContext : ContextSpecification
    {
        protected object Actual;

        protected object Expected;

        protected ITypeParser TypeParser;

        public override void Given()
        {
            TypeParser = Qwiq.TypeParser.Default;
        }
    }
}