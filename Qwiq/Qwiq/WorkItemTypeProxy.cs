namespace Microsoft.IE.Qwiq
{
    public class WorkItemTypeProxy : IWorkItemType
    {
        private readonly TeamFoundation.WorkItemTracking.Client.WorkItemType _type;

        internal WorkItemTypeProxy(TeamFoundation.WorkItemTracking.Client.WorkItemType type)
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
    }
}