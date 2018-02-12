using System;
using JetBrains.Annotations;

namespace Qwiq
{
    public abstract class TypeParserFirstChanceExceptionContext : TypeParserTestsContext
    {
        [CanBeNull]
        public Exception FirstChanceException { get; set; }

        public override void Given()
        {
            base.Given();
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
        }

        private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            FirstChanceException = e.Exception;
        }
    }
}