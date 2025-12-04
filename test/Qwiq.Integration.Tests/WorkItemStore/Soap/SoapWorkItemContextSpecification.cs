using System;
using System.Diagnostics;

namespace Qwiq.WorkItemStore.Soap
{
    public abstract class SoapWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {
        protected override IWorkItemStore Create()
        {
            return TimedAction(() =>
            {
                try
                {
                    return IntegrationSettings.CreateSoapStore();
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging instead of silently swallowing
                    Debug.WriteLine($"Failed to create SOAP WorkItemStore: {ex.GetType().Name}: {ex.Message}");
                    throw;
                }
            }, "SOAP", "WIS Create");
        }
    }
}