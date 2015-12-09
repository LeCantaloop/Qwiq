using System;

namespace Microsoft.IE.Qwiq.Exceptions
{
    internal class InvalidOperationExceptionMapper : VssExceptionMapper
    {
        private static readonly int[] TfsBadRequestIds = {
            26180, 20024, 20015, 20012, 24001, 20018, 20001, 26202, 51587, 51586, 26214, 26181, 20016, 26194, 400276,
            20022, 26071, 20014, 51011, 26079, 26061, 51584, 26196, 20013, 14045, 26073, 20019, 20026, 26182, 26201,
            26195, 51012, 51583, 20017, 51580, 51581, 51010, 26036, 26039, 26051, 26052, 51609, 51612, 51005, 51009, 51601
        };

        public InvalidOperationExceptionMapper() : base(TfsBadRequestIds, (m, e) => new InvalidOperationException(m, e))
        {
        }
    }
}
