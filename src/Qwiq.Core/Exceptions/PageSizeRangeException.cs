using System;
using System.Runtime.Serialization;

namespace Microsoft.Qwiq
{
    [Serializable]
    public class PageSizeRangeException : ApplicationException
    {
        public PageSizeRangeException()
            :base("TF237117: PageSize has to be between 50 and 200")
        {

        }

        protected PageSizeRangeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
