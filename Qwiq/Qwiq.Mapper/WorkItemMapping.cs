namespace Microsoft.IE.Qwiq.Mapper
{
    public class WorkItemMapping
    {
        public WorkItemMapping(IWorkItem workItem, object mappedWorkItem)
        {
            WorkItem = workItem;
            MappedWorkItem = mappedWorkItem;
        }

        public IWorkItem WorkItem { get; set; }
        public object MappedWorkItem { get; set; }
    }
}
