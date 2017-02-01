using Tfs = Microsoft.TeamFoundation.Server;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class ProjectInfoProxy : IProjectInfo
    {
        private readonly Tfs.ProjectInfo _projectInfo;

        internal ProjectInfoProxy(Tfs.ProjectInfo projectInfo)
        {
            _projectInfo = projectInfo;
        }

        public string Uri { get { return _projectInfo.Uri; } set { _projectInfo.Uri = value; } }
    }
}

