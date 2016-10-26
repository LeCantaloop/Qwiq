using Microsoft.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    public class WorkItemTypeProxy : IWorkItemType
    {
        private readonly Tfs.WorkItemType _type;

        internal WorkItemTypeProxy(Tfs.WorkItemType type)
        {
            _type = type;
        }

        public string Description
        {
            get { return _type.Description; }
        }

        public string Name
        {
            get { return _type.Name; }
        }

        public IWorkItem NewWorkItem()
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(_type.NewWorkItem()));
        }
    }
}
