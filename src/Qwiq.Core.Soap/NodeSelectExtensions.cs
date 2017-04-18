using System;
using System.Globalization;

// ReSharper disable CheckNamespace
namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
// ReSharper restore CheckNamespace
{
    public static class NodeSelectExtensions
    {
        /// <summary>
        /// Gets the <see cref="NodeSelect.AsOf"/> as <see cref="DateTime"/> in UTC.
        /// </summary>
        /// <param name="nodeSelect"></param>
        /// <returns>If value is not already in UTC <see langword="null" /> is returned.</returns>
        public static DateTime? GetAsOfUtc(this NodeSelect nodeSelect)
        {
            if (nodeSelect.AsOf == null ||
                !DateTime.TryParse(((NodeItem)nodeSelect.AsOf).Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result))
                return null;
            if (result.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeSelect.AsOf), "Specified date must be UTC");
            }
            return result;
        }
    }
}