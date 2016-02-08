using System.Collections.Generic;

namespace HotDocs.Sdk.Cloud.Rest
{
    /// <summary>
    ///     Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Adds a key-value pair to a dictionary unless the value is null.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddIfNotNull(this Dictionary<string, string> dict, string key, object value)
        {
            if (value != null)
            {
                dict.Add(key, value.ToString());
            }
        }
    }
}