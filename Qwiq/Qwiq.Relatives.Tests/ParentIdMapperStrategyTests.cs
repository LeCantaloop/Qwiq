using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mocks;
using Microsoft.IE.Qwiq.Relatives.Mapper;
using Microsoft.IE.Qwiq.Relatives.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.IE.Qwiq.Relatives.Tests
{
    public abstract class ParentIdMapperStrategyContextSpecification<T> : ContextSpecification where T : new()
    {
        protected IWorkItemMapperStrategy ParentIdStrategy { get; set; }
        protected IWorkItemMapper Instance { get; set; }
        protected IWorkItemStore WorkItemStore { get; set; }
        protected IEnumerable<IWorkItem> WorkItems { get; set; }
        protected IEnumerable<T> MappedIssues { get; set; }
        protected IEnumerable<T> ExpectedMappedIssues { get; set; }

        public override void Given()
        {
            ParentIdStrategy = new ParentIdMapperStrategy(WorkItemStore);
            Instance = new WorkItemMapper(new[] { ParentIdStrategy });
        }

        public override void When()
        {
            MappedIssues = Instance.Create<T>(WorkItems);
        }
    }

    [TestClass]
    public class given_a_non_empty_set_of_workitems_with_a_workitemstore_with_populated_links_when_it_is_mapped : ParentIdMapperStrategyContextSpecification<MockParentIdIssue>
    {
        public override void Given()
        {
            var workItemLinks = new[]
            {
                new MockWorkItemLinkInfo
                {
                    TargetId = 1,
                    SourceId = 2,
                    LinkTypeId = -2
                }
            };

            var workItems = new[]
            {
                new MockWorkItem
                {
                    Id = 2,
                    ChangedDate = new DateTime(2010, 10, 10),
                    Type = MockParentIdIssue.CustomWorkItemType
                }
            };

            var expectedMockWorkItems = new[]
            {
                new MockParentIdIssue
                {
                    ParentId = 1
                }
            };

            WorkItemStore = new MockWorkItemStore(Enumerable.Empty<IWorkItem>(), workItemLinks);
            WorkItems = workItems;
            ExpectedMappedIssues = expectedMockWorkItems;

            base.Given();
        }

        [TestMethod]
        public void the_mapped_issues_have_the_expected_parent_ids()
        {
            MappedIssues.Select(x => x.ParentId)
                .ShouldContainOnly(ExpectedMappedIssues.Select(x => x.ParentId));
        }
    }

    [TestClass]
    public class given_an_empty_set_of_workitems_when_it_is_mapped : ParentIdMapperStrategyContextSpecification<MockParentIdIssue>
    {
        public override void Given()
        {
            var workItems = Enumerable.Empty<IWorkItem>();

            WorkItemStore = new MockWorkItemStore(Enumerable.Empty<IWorkItem>(), Enumerable.Empty<IWorkItemLinkInfo>());
            WorkItems = workItems;

            base.Given();
        }

        [TestMethod]
        public void the_mapped_issues_should_be_empty()
        {
            MappedIssues.ShouldBeEmpty();
        }
    }

    [TestClass]
    public class given_a_set_of_workitems_with_a_workitemstore_with_no_links_to_map_when_it_is_mapped : ParentIdMapperStrategyContextSpecification<MockParentIdIssue>
    {
        public override void Given()
        {
            var workItems = new[]
            {
                new MockWorkItem
                {
                    Id = 2,
                    ChangedDate = new DateTime(2010, 10, 10),
                    Type = MockParentIdIssue.CustomWorkItemType
                }
            };

            var expectedMockWorkItems = new[]
            {
                new MockParentIdIssue
                {
                    ParentId = 0
                }
            };

            WorkItemStore = new MockWorkItemStore(Enumerable.Empty<IWorkItem>(), Enumerable.Empty<IWorkItemLinkInfo>());
            WorkItems = workItems;
            ExpectedMappedIssues = expectedMockWorkItems;

            base.Given();
        }

        [TestMethod]
        public void the_issues_are_mapped_with_no_ids()
        {
            MappedIssues.Select(x => x.ParentId)
                .ShouldContainOnly(ExpectedMappedIssues.Select(x => x.ParentId));
        }
    }

    [TestClass]
    public class given_a_set_of_workitems_with_a_workitemstore_with_some_populated_links_when_it_is_mapped : ParentIdMapperStrategyContextSpecification<MockParentIdIssue>
    {
        public override void Given()
        {
            var workItemLinks = new[]
            {
                new MockWorkItemLinkInfo
                {
                    TargetId = 1,
                    SourceId = 2,
                    LinkTypeId = -2
                }
            };

            var workItems = new[]
            {
                new MockWorkItem
                {
                    Id = 2,
                    ChangedDate = new DateTime(2010, 10, 10),
                    Type = MockParentIdIssue.CustomWorkItemType
                },
                new MockWorkItem
                {
                    Id = 4,
                    ChangedDate = new DateTime(2011, 11, 11),
                    Type = MockParentIdIssue.CustomWorkItemType
                }
            };

            var expectedMockWorkItems = new[]
            {
                new MockParentIdIssue
                {
                    ParentId = 1
                },
                new MockParentIdIssue
                {
                    ParentId = 0
                }
            };

            WorkItemStore = new MockWorkItemStore(Enumerable.Empty<IWorkItem>(), workItemLinks);
            WorkItems = workItems;
            ExpectedMappedIssues = expectedMockWorkItems;

            base.Given();
        }

        [TestMethod]
        public void the_mapped_issues_have_the_expected_parent_ids()
        {
            MappedIssues.Select(x => x.ParentId)
                .ShouldContainOnly(ExpectedMappedIssues.Select(x => x.ParentId));
        }
    }

    [TestClass]
    public class given_a_set_of_workitems_with_a_workitemstore_with_a_populated_link_a_linktypeid_of_0_when_it_is_mapped : ParentIdMapperStrategyContextSpecification<MockParentIdIssue>
    {
        public override void Given()
        {
            var workItemLinks = new[]
            {
                new MockWorkItemLinkInfo
                {
                    TargetId = 1,
                    SourceId = 2,
                    LinkTypeId = -2
                },
                new MockWorkItemLinkInfo
                {
                    TargetId = 3,
                    SourceId = 4,
                    LinkTypeId = 0
                }
            };

            var workItems = new[]
            {
                new MockWorkItem
                {
                    Id = 2,
                    ChangedDate = new DateTime(2010, 10, 10),
                    Type = MockParentIdIssue.CustomWorkItemType
                },
                new MockWorkItem
                {
                    Id = 4,
                    ChangedDate = new DateTime(2011, 11, 11),
                    Type = MockParentIdIssue.CustomWorkItemType
                }
            };

            var expectedMockWorkItems = new[]
            {
                new MockParentIdIssue
                {
                    ParentId = 1
                },
                new MockParentIdIssue
                {
                    ParentId = 0
                }
            };

            WorkItemStore = new MockWorkItemStore(Enumerable.Empty<IWorkItem>(), workItemLinks);
            WorkItems = workItems;
            ExpectedMappedIssues = expectedMockWorkItems;

            base.Given();
        }

        [TestMethod]
        public void the_mapped_issues_have_the_expected_parent_ids()
        {
            MappedIssues.Select(x => x.ParentId)
                .ShouldContainOnly(ExpectedMappedIssues.Select(x => x.ParentId));
        }
    }

    [TestClass]
    public class given_a_set_of_workitems_and_a_type_without_parent_id_when_it_is_mapped : ParentIdMapperStrategyContextSpecification<SimpleMockModel>
    {
        public override void Given()
        {
            var workItemLinks = new[]
            {
                new MockWorkItemLinkInfo
                {
                    TargetId = 1,
                    SourceId = 2,
                    LinkTypeId = -2
                },
                new MockWorkItemLinkInfo
                {
                    TargetId = 3,
                    SourceId = 4,
                    LinkTypeId = 0
                }
            };

            var workItems = new[]
            {
                new MockWorkItem
                {
                    Id = 2,
                    ChangedDate = new DateTime(2010, 10, 10),
                    Type = new MockWorkItemType { Name = "SimpleMockWorkItem" }
                },
                new MockWorkItem
                {
                    Id = 4,
                    ChangedDate = new DateTime(2011, 11, 11),
                    Type = new MockWorkItemType { Name = "SimpleMockWorkItem" }
                }
            };

            var expectedMockWorkItems = new[]
            {
                new SimpleMockModel(),
                new SimpleMockModel()
            };

            WorkItemStore = new MockWorkItemStore(Enumerable.Empty<IWorkItem>(), workItemLinks);
            WorkItems = workItems;
            ExpectedMappedIssues = expectedMockWorkItems;

            base.Given();
        }

        [TestMethod]
        public void the_mapped_issues_should_have_default_ID_values()
        {
            MappedIssues.Select(x => x.Id)
                .ShouldContainOnly(ExpectedMappedIssues.Select(x => x.Id));
        }

        [TestMethod]
        public void the_mapped_issues_should_have_default_Priority_values()
        {
            MappedIssues.Select(x => x.Priority)
                .ShouldContainOnly(ExpectedMappedIssues.Select(x => x.Priority));
        }

        [TestMethod]
        public void there_should_be_two_mapped_issues()
        {
            MappedIssues.Count().ShouldEqual(2);
        }
    }
}
