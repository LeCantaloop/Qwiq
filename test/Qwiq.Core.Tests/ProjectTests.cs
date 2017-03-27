using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    public abstract class ProjectContextSpecification : ContextSpecification
    {
        protected List<IProject> RestProjects { get; set; }

        protected List<IProject> SoapProjects { get; set; }

        protected IWorkItemStore Rest { get; set; }

        protected IWorkItemStore Soap { get; set; }

        public override void When()
        {
            RestProjects = Rest.Projects.ToList();
            SoapProjects = Soap.Projects.ToList();
        }

        public override void Given()
        {
            var credentials = Credentials.CredentialsFactory.CreateCredentials((string)null);
            var fac = WorkItemStoreFactory.GetInstance();
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            Soap = fac.Create(uri, credentials, ClientType.Soap);
            Rest = fac.Create(uri, credentials, ClientType.Rest);
        }
    }

    [TestClass]
    public class Given_projects_from_each_WorkItemStore_implementation : ProjectContextSpecification
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
            foreach (var sp in SoapProjects)
            {
                var rp = RestProjects.Find(p => ProjectComparer.Instance.Equals(sp, p));

                rp?.WorkItemTypes.ShouldContainOnly(sp.WorkItemTypes, WorkItemTypeComparer.Instance);
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