using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;
using Should.Core.Exceptions;

namespace Microsoft.Qwiq.Core.Tests
{
    public abstract class ProjectContextSpecificationSpecification : WorkItemStoreComparisonContextSpecification
    {
        protected List<IProject> RestProjects { get; set; }

        protected List<IProject> SoapProjects { get; set; }



        public override void When()
        {
            RestProjects = Rest.Projects.ToList();
            SoapProjects = Soap.Projects.ToList();
        }
    }

    [TestClass]
    public class Given_projects_from_each_WorkItemStore_implementation : ProjectContextSpecificationSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Number_of_projects_is_equal()
        {
            RestProjects.Count.ShouldEqual(SoapProjects.Count);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Project_names_are_equal()
        {
            RestProjects.Select(s => s.Name).ToList().ShouldContainOnly(SoapProjects.Select(s => s.Name).ToList());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Project_WITs_are_equal()
        {
            var soap = SoapProjects.ToDictionary(k => k.Guid, e => e);
            var rest = RestProjects.ToDictionary(k => k.Guid, e => e);

            foreach (var sp in soap)
            {
                IProject rp;
                if (!rest.TryGetValue(sp.Key, out rp))
                {
                    Trace.TraceWarning("REST collection does not contain project {0} ({1})", sp.Key, sp.Value.Name);
                    continue;
                }

                try
                {
                    rp.WorkItemTypes.ShouldContainOnly(sp.Value.WorkItemTypes, WorkItemTypeComparer.Instance);
                }
                catch (AssertFailedException e)
                {
                    throw new Exception($"Project {sp.Key} ({sp.Value.Name}) contains differences.", e);
                }
            }
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Project_Area_Paths_are_equal()
        {
            foreach (var sp in SoapProjects)
            {
                var rp = RestProjects.Find(p => ProjectComparer.Instance.Equals(sp, p));

                rp?.AreaRootNodes.ShouldContainOnly(sp.AreaRootNodes);
            }
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Project_Iteration_Paths_are_equal()
        {
            foreach (var sp in SoapProjects)
            {
                var rp = RestProjects.Find(p => ProjectComparer.Instance.Equals(sp, p));

                rp?.IterationRootNodes.ShouldContainOnly(sp.IterationRootNodes);
            }
        }
    }
}