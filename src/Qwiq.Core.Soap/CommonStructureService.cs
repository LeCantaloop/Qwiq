using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.Server;

namespace Qwiq.Client.Soap
{
    internal class CommonStructureService : ICommonStructureService
    {
        private readonly Tfs.ICommonStructureService4 _service;

        internal CommonStructureService(Tfs.ICommonStructureService4 service)
        {
            _service = service;
        }

        public string CreateNode(string nodeName, string parentNodeUri, DateTime? startDate, DateTime? finishDate)
        {
            return _service.CreateNode(nodeName, parentNodeUri, startDate, finishDate);
        }

        public XmlElement GetNodesXml(string[] nodeUris, bool childNodes)
        {
            return _service.GetNodesXml(nodeUris, childNodes);
        }

        public IProjectInfo GetProjectFromName(string projectName)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IProjectInfo>(
                new ProjectInfo(_service.GetProjectFromName(projectName)));
        }

        public IEnumerable<IProjectInfo> ListAllProjects()
        {
            return _service.ListAllProjects()
                           .Select(p => ExceptionHandlingDynamicProxyFactory.Create<IProjectInfo>(new ProjectInfo(p)));
        }

        public IEnumerable<INodeInfo> ListStructures(string projectUri)
        {
            return _service.ListStructures(projectUri)
                           .Select(i => ExceptionHandlingDynamicProxyFactory.Create<INodeInfo>(new NodeInfo(i)));
        }

        public void SetIterationDates(string nodeUri, DateTime? startDate, DateTime? finishDate)
        {
            _service.SetIterationDates(nodeUri, startDate, finishDate);
        }
    }
}