using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mapper.Linq;
using Microsoft.Qwiq.Mapper.Mocks;

namespace Microsoft.Qwiq.Mapper
{
    public abstract class GenericQueryBuilderContextSpecification<T> : QueryableContextSpecification<T>
    {
        protected string Expected;
        protected string Actual;

        protected override IPropertyInspector CreatePropertyInspector()
        {
            return new PropertyInspector(new MockPropertyReflector());
        }

        protected override IFieldMapper CreateFieldMapper()
        {
            var fieldMapper = base.CreateFieldMapper();
            Expected = "SELECT " + string.Join(", ", fieldMapper.GetFieldNames(typeof(T))) + " FROM WorkItems";

            return fieldMapper;
        }
    }
}