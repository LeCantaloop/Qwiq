using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.WorkItemStore
{

    public abstract class WorkItemContextSpecification<T> : WorkItemStoreContextSpecification<T>
        where T : IWorkItemStore
    {
        private const int Id = TestData.BasicWorkItemId;

        protected IWorkItem Result { get; private set; } = null!;

        [TestMethod]
        [TestCategory("localOnly")]
        public void Reading_Id_from_Fields_property_with_ReferenceName_equals_the_property_value()
        {
            Result.Fields[CoreFieldRefNames.Id]?.Value?.ToString().ShouldEqual(Result.Id.ToString(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Reading_Id_from_this_operator_with_ReferenceName_equals_the_property_value()
        {
            Result[CoreFieldRefNames.Id]?.ToString().ShouldEqual(Result.Id.ToString(CultureInfo.InvariantCulture));
        }

        public override void When()
        {
            Result = WorkItemStore!.Query(Id)!;
        }
    }
}