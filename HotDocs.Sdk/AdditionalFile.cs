using System;

namespace HotDocs.Sdk
{
    /// <summary>
    /// <c>AdditionalFile</c> is used by <c>TemplateManifest</c> to identify files that were listed in the manifest
    /// as miscellaneous dependencies. Such files are not typically referred to <i>directly</i> by the template
    /// or component file, but were explicitly declared by the template author as being necessary for the proper operation
    /// of the template. Examples may include image files that could potentially be referred to by a dynamic INSERT IMAGE
    /// instruction, or other files that may be required to be in the same location as the template in custom integration
    /// scenarios.
    /// </summary>
    public class AdditionalFile : IEquatable<AdditionalFile>, IComparable<AdditionalFile>
    {
        internal AdditionalFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            FileName = fileName;
        }

        /// <summary>
        /// <c>FileName</c> specifies the file name but its location or path.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Overrides IEquatable.Equals
        /// </summary>
        /// <param name="obj">The object being compared to 'this'</param>
        /// <returns>boolean (true or false)</returns>
        public override bool Equals(object obj)
        {
            return (obj != null) && (obj is AdditionalFile) && Equals((AdditionalFile)obj);
        }

        /// <summary>
        /// Overrides Object.GetHashCode
        /// </summary>
        /// <returns>a number of type int</returns>
        public override int GetHashCode()
        {
            return FileName.ToLower().GetHashCode();
        }

        /// <summary>
        /// Overrides Object.ToString
        /// </summary>
        /// <returns>a string instance similar in content to 'this'</returns>
        public override string ToString()
        {
            return String.Format("FileName: {0}", FileName);
        }

        #region IEquatable<AdditionalFile> Members

        /// <summary>
        /// Implements IEquatable.Equals
        /// </summary>
        /// <param name="other">The object being compared to 'this'</param>
        /// <returns>boolean (true or false)</returns>
        public bool Equals(AdditionalFile other)
        {
            return CompareTo(other) == 0;
        }

        #endregion

        #region IComparable<AdditionalFile> Members

        /// <summary>
        /// Implements IComparable.CompareTo
        /// </summary>
        /// <param name="other">The object being compared to 'this'</param>
        /// <returns>-1, 0, or 1 depending on whether 'other' is less than,
        /// equal to, or greater than 'this'</returns>
        public int CompareTo(AdditionalFile other)
        {
            return string.Compare(other.FileName, FileName, true);
        }

        #endregion
    }
}