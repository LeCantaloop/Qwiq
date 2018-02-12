using System;

using Tfs = Microsoft.TeamFoundation.Server;

namespace Qwiq.Client.Soap
{
    internal class ProjectProperty : IProjectProperty
    {
        private readonly Tfs.ProjectProperty _projectProperty;

        internal ProjectProperty(Tfs.ProjectProperty projectProperty)
        {
            _projectProperty = projectProperty ?? throw new ArgumentNullException(nameof(projectProperty));
        }
    }
}

