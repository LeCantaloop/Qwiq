using Qwiq.Linq;
using Qwiq.Mapper.Attributes;
using Qwiq.Mapper.Linq;
using Qwiq.Mapper.Mocks;

namespace Qwiq.Mapper
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