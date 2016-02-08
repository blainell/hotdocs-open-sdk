using System.Collections.Generic;
using System.Linq;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     A helper class to store a set of file names
    /// </summary>
    internal class FileNameSet
    {
        private readonly Dictionary<string, string> dict = new Dictionary<string, string>();

        /// <summary>
        ///     Include a filename to the set
        /// </summary>
        /// <param name="fileName">the file name to include</param>
        public void Include(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var key = fileName.ToLower();
                if (!dict.ContainsKey(key))
                {
                    dict[key] = fileName;
                }
            }
        }

        /// <summary>
        ///     Get an array of all file names in the set
        /// </summary>
        /// <returns>an array of all file names in the set</returns>
        public string[] ToArray()
        {
            return dict.Values.ToArray();
        }
    }
}