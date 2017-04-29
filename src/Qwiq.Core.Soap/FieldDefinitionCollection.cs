using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class FieldDefinitionCollection : Qwiq.FieldDefinitionCollection
    {
        internal FieldDefinitionCollection(Tfs.FieldDefinitionCollection innerCollection)
            : base(
                innerCollection.Cast<Tfs.FieldDefinition>()
                               .Select(
                                   field => ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(
                                       new FieldDefinition(field))))
        {
        }
    }
}