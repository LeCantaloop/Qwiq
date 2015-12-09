using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.IE.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation.Server;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class CommonStructureServiceProxy : ICommonStructureService
    {
        private readonly Tfs.ICommonStructureService4 _service;

        internal CommonStructureServiceProxy(Tfs.ICommonStructureService4 service)
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
            return ExceptionHandlingDynamicProxyFactory.Create<IProjectInfo>(new ProjectInfoProxy(_service.GetProjectFromName(projectName)));
        }

        public IEnumerable<INodeInfo> ListStructures(string projectUri)
        {
            return _service.ListStructures(projectUri).Select(i => ExceptionHandlingDynamicProxyFactory.Create<INodeInfo>(new NodeInfoProxy(i)));
        }

        public XmlElement GetNodesXml(string[] nodeUris, bool childNodes)
        {
            return _service.GetNodesXml(nodeUris, childNodes);
        }

        public IEnumerable<IProjectInfo> ListAllProjects()
        {
            return _service.ListAllProjects().Select(p => ExceptionHandlingDynamicProxyFactory.Create<IProjectInfo>(new ProjectInfoProxy(p)));
        }
    }
}