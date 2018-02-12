using System.Diagnostics;

namespace Qwiq.Exceptions
{
    [DebuggerStepThrough]
    internal class TransientExceptionMapper: VssExceptionMapper
    {
        private static readonly int[] TfsServerUnavailableIds = {
            26176, 26173, 24011, 26000, 26012, 26174, 20027, 51507, 26213, 400324, 246018, 26213
        };

        public TransientExceptionMapper() : base(TfsServerUnavailableIds, (m, e) => new TransientException(m, e))
        {
        }
    }
}

