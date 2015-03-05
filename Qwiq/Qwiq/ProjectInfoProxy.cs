namespace Microsoft.IE.Qwiq
{
    public class ProjectInfoProxy : IProjectInfo
    {
        private readonly TeamFoundation.Server.ProjectInfo _projectInfo;

        internal ProjectInfoProxy(TeamFoundation.Server.ProjectInfo projectInfo)
        {
            _projectInfo = projectInfo;
        }

        public string Uri { get { return _projectInfo.Uri; } set { _projectInfo.Uri = value; } }
    }
}
