using System;
#if NETFRAMEWORK || NETSTANDARD2_0
using System.Runtime.Serialization;
#endif

namespace Qwiq
{
    [Serializable]
    public class PageSizeRangeException : ApplicationException
    {
        public PageSizeRangeException()
            : base("TF237117: PageSize has to be between 50 and 200")
        {
        }

#if NETFRAMEWORK || NETSTANDARD2_0
        protected PageSizeRangeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
