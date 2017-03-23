using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Qwiq
{
    public class PageSizeRangeException : ApplicationException
    {
        public PageSizeRangeException()
            :base("TF237117: PageSize has to be between 50 and 200")
        {
            
        }
    }
}
