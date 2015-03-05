using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.TeamFoundation.Server;

namespace Microsoft.IE.Qwiq
{
    public class CommonStructureServiceProxy : ICommonStructureService
    {
        private readonly ICommonStructureService4 _service;

        public CommonStructureServiceProxy(ICommonStructureService4 service)
        {
            _service = service;
        }

        public string CreateNode(string nodeName, string parentNodeUri, DateTime? startDate, DateTime? finishDate)
        {
            return _service.CreateNode(nodeName, parentNodeUri, startDate, finishDate);
        }

        public void SetIterationDates(string nodeUri, DateTime? startDate, DateTime? finishDate)
        {
            _service.SetIterationDates(nodeUri, startDate, finishDate);
        }

        public IProjectInfo GetProjectFromName(string projectName)
        {
            return new ProjectInfoProxy(_service.GetProjectFromName(projectName));
        }

        public IEnumerable<INodeInfo> ListStructures(string projectUri)
        {
            return _service.ListStructures(projectUri).Select(i => new NodeInfoProxy(i));
        }

        public XmlElement GetNodesXml(string[] nodeUris, bool childNodes)
        {
            return _service.GetNodesXml(nodeUris, childNodes);
        }

        public IEnumerable<IProjectInfo> ListAllProjects()
        {
            return _service.ListAllProjects().Select(p => new ProjectInfoProxy(p));
        }
    }
}