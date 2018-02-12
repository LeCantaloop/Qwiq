using System.Linq;

using Qwiq.WorkItemStore.Soap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.WorkItemStore.WorkItem.Soap
{
    public abstract class RevisionContextSpecification : SoapWorkItemContextSpecification
    {

    }

    [TestClass]
    public class Given_a_WorkItem_with_Revisions : RevisionContextSpecification
    {
        [TestMethod]
        public void the_WorkItem_has_Revisions()
        {
            Result.Rev.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public void the_number_of_revisions_is_equal_to_the_revision_objects()
        {
            Result.Rev.ShouldEqual(Result.Revisions.Count());
        }

        [TestMethod]
        public void the_ChangedBy_field_on_the_revision_is_not_empty()
        {
            Result.Revisions.First()[CoreFieldRefNames.ChangedBy].ShouldNotBeNull();
        }
    }
}
