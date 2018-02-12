using System;
using System.Collections.Generic;
using System.Xml;

namespace Qwiq
{
    public interface ICommonStructureService
    {
        string CreateNode(string nodeName, string parentNodeUri, DateTime? startDate, DateTime? finishDate);
        void SetIterationDates(string nodeUri, DateTime? startDate, DateTime? finishDate);
        IProjectInfo GetProjectFromName(string projectName);
        IEnumerable<INodeInfo> ListStructures(string projectUri);
        XmlElement GetNodesXml(string[] nodeUris, bool childNodes);
        IEnumerable<IProjectInfo> ListAllProjects();
    }
}

