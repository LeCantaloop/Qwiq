using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    [TestClass]
    public class Given_a_Project_from_each_WorkItemStore_implementation : ProjectComparisonContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Each_project_equals_eachother()
        {
            RestProject.ShouldEqual(SoapProject, ProjectComparer.Instance);
            RestProject.GetHashCode().ShouldEqual(SoapProject.GetHashCode());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Each_project_contains_the_same_Area_paths()
        {
            RestProject.AreaRootNodes.ShouldContainOnly(SoapProject.AreaRootNodes, NodeComparer.Instance);
            RestProject.AreaRootNodes.ShouldEqual(SoapProject.AreaRootNodes, Comparer.NodeCollectionComparer);
            RestProject.AreaRootNodes.GetHashCode().ShouldEqual(SoapProject.AreaRootNodes.GetHashCode());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Each_project_contains_the_same_Iteration_paths()
        {
            RestProject.IterationRootNodes.ShouldContainOnly(SoapProject.IterationRootNodes, NodeComparer.Instance);
            RestProject.IterationRootNodes.ShouldEqual(SoapProject.IterationRootNodes, Comparer.NodeCollectionComparer);
            RestProject.IterationRootNodes.GetHashCode().ShouldEqual(SoapProject.IterationRootNodes.GetHashCode());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Each_project_contains_the_same_WorkItemTypes_with_the_same_FieldDefinitions()
        {
            var exceptions = new List<Exception>();
            foreach (var wit in SoapProject.WorkItemTypes)
            {
                var sw = wit;
                try
                {
                    var rw = RestProject.WorkItemTypes[sw.Name];
                    rw.ShouldNotBeNull($"No WIT '{sw.Name}' in REST");

                    // We can't do a simple ShouldContainsOnly check here because the REST client is returning fields the SOAP client is not

                    var rwfs = rw.FieldDefinitions.Where(FieldDefinitionCollectionComparer.SkippedFieldsPredicate)
                                 .OrderBy(p => p.ReferenceName)
                                 .ToList();
                    var swfs = sw.FieldDefinitions.Where(FieldDefinitionCollectionComparer.SkippedFieldsPredicate)
                                 .OrderBy(p => p.ReferenceName)
                                 .ToList();

                    try
                    {
                        rwfs.ShouldContainOnly(swfs);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }

                    foreach (var swfd in swfs)
                    {
                        var rwfd = rw.FieldDefinitions[swfd.ReferenceName];

                        rwfd.ShouldEqual(swfd, $"{rw.Name}:{rwfd.ReferenceName}:Equals");
                        rwfd.GetHashCode()
                            .ShouldEqual(swfd.GetHashCode(), $"{rw.Name}:{rwfd.ReferenceName}:GetHashCode");
                    }

                    rw.FieldDefinitions.ShouldEqual(sw.FieldDefinitions, FieldDefinitionCollectionComparer.Instance);
                    rw.FieldDefinitions.GetHashCode()
                      .ShouldEqual(
                          sw.FieldDefinitions.GetHashCode(),
                          $"{rw.Name}:{nameof(rw.FieldDefinitions)}:GetHashCode");
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            try
            {
                RestProject.WorkItemTypes.ShouldEqual(
                    SoapProject.WorkItemTypes,
                    WorkItemTypeCollectionComparer.Instance);
                RestProject.WorkItemTypes.GetHashCode()
                           .ShouldEqual(
                               SoapProject.WorkItemTypes.GetHashCode(),
                               $"{nameof(RestProject.WorkItemTypes)}:GetHashCode");
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }

            if (exceptions.Any()) throw new AggregateException(exceptions.EachToUsefulString(), exceptions);
        }
    }
}