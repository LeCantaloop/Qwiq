using System.Diagnostics;

using Castle.DynamicProxy;

namespace Microsoft.Qwiq.Exceptions
{
    /// <summary>
    /// Provides a base class for Castle intercepted types
    /// </summary>
    /// <remarks>
    /// This inspects the incoming proxy objects and ensures Object.Equals(Object) and Object.GetHashCode() are directed to the appropriate location.
    /// </remarks>
    [DebuggerStepThrough]
    public class ProxyBase
    {
        /// <summary>
        /// Provides hook into object providing Equals(Object)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>
        /// Intercepted interfaces can provide IEquatable`1 implementations, but that will not forward calls to
        /// Object.Equals(Object) or Object.GetHashCode() since neither is part of the interface.
        /// </remarks>
        public override bool Equals(object obj)
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            var proxy = this as IProxyTargetAccessor;
            // ReSharper restore SuspiciousTypeConversion.Global
            if (proxy == null)
            {
                return base.Equals(obj);
            }
            var target = proxy.DynProxyGetTarget();
            if (target == null)
            {
                return base.Equals(obj);
            }
            return target.Equals(obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            var proxy = this as IProxyTargetAccessor;
            // ReSharper restore SuspiciousTypeConversion.Global
            if (proxy == null)
            {
                return base.GetHashCode();
            }
            var target = proxy.DynProxyGetTarget();
            if (target == null)
            {
                return base.GetHashCode();
            }
            return target.GetHashCode();
        }

        public override string ToString()
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            var proxy = this as IProxyTargetAccessor;
            // ReSharper restore SuspiciousTypeConversion.Global
            if (proxy == null)
            {
                return base.ToString();
            }
            var target = proxy.DynProxyGetTarget();
            if (target == null)
            {
                return base.ToString();
            }
            return target.ToString();
        }
    }
}