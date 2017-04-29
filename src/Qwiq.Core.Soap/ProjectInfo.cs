using System;

using Tfs = Microsoft.TeamFoundation.Server;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class ProjectInfo : IProjectInfo
    {
        private readonly Tfs.ProjectInfo _projectInfo;

        internal ProjectInfo(Tfs.ProjectInfo projectInfo)
        {
            _projectInfo = projectInfo ?? throw new ArgumentNullException(nameof(projectInfo));
        }

        public string Uri { get => _projectInfo.Uri;
            set => _projectInfo.Uri = value;
        }
    }
}

