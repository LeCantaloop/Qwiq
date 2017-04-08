using Microsoft.Qwiq.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Soap
{
    internal class ProjectCollection : IProjectCollection
    {
        private readonly TeamFoundation.WorkItemTracking.Client.ProjectCollection _valueProjects;

        public ProjectCollection(TeamFoundation.WorkItemTracking.Client.ProjectCollection valueProjects)
        {
            _valueProjects = valueProjects ?? throw new ArgumentNullException(nameof(valueProjects));
        }

        IProject IProjectCollection.this[string projectName] => ExceptionHandlingDynamicProxyFactory.Create<IProject>(
            new Project(_valueProjects[projectName]));

        IProject IProjectCollection.this[Guid id] => ExceptionHandlingDynamicProxyFactory.Create<IProject>(
            new Project(_valueProjects[id]));

        public IEnumerator<IProject> GetEnumerator()
        {
            return _valueProjects.Cast<TeamFoundation.WorkItemTracking.Client.Project>()
                                 .Select(
                                     item => ExceptionHandlingDynamicProxyFactory.Create<IProject>(new Project(item)))
                                 .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}