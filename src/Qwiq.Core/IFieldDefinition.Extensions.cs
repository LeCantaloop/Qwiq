using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public static partial class Extensions
    {
        public static bool IsCloneable([NotNull] this IFieldDefinition definition)
        {
            switch (definition.Id)
            {
                case (int)CoreField.AreaPath:
                case (int)CoreField.RevisedDate:
                case (int)CoreField.ChangedDate:
                case (int)CoreField.State:
                case (int)CoreField.AuthorizedDate:
                case (int)CoreField.Watermark:
                case (int)CoreField.Rev:
                case (int)CoreField.ChangedBy:
                case (int)CoreField.Reason:
                case (int)CoreField.IterationPath:
                case (int)CoreField.CreatedBy:
                case (int)CoreField.History:
                case (int)CoreField.WorkItemType:
                case (int)CoreField.CreatedDate:
                    return false;
            }

            return definition.IsEditable();
        }

        public static bool IsEditable([NotNull] this IFieldDefinition definition)
        {
            switch (definition.Id)
            {
                case (int)CoreField.IterationPath:
                case (int)CoreField.AreaPath:
                    return true;

                case (int)CoreField.Id:
                case (int)CoreField.Rev:
                case (int)CoreField.WorkItemType:
                    return false;
            }

            return !definition.IsComputed();
        }

        public static bool IsComputed([NotNull] this IFieldDefinition definition)
        {
            switch (definition.Id)
            {
                case (int)CoreField.AreaPath:
                case (int)CoreField.NodeName:
                case (int)CoreField.TeamProject:
                case (int)CoreField.IterationPath:
                    return true;
            }

            return false;
        }
    }
}
