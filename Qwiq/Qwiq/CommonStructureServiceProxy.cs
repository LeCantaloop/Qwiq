using System;
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
    }
}