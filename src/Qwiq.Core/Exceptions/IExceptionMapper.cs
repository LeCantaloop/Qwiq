using System;


namespace Qwiq.Exceptions
{
    public interface IExceptionMapper
    {
        Exception? Map(Exception ex);
    }
}

