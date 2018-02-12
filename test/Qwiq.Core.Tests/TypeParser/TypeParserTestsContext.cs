using Qwiq.Tests.Common;

namespace Qwiq
{
    public abstract class TypeParserTestsContext : ContextSpecification
    {
        protected object Actual { get; set; }

        protected object Expected { get; set; }

        protected ITypeParser Parser { get; set; }

        public override void Given()
        {
            Parser = TypeParser.Default;
        }
    }
}