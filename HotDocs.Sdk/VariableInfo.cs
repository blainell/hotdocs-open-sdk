using System;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     <c>VariableInfo</c> contains basic metadata (name and type) about a HotDocs variable. This type is used
    ///     by <c>TemplateManifest</c> to list the variables that may potentially be gathered by an interview
    ///     and/or used in a template.
    /// </summary>
    public class VariableInfo : IEquatable<VariableInfo>, IComparable<VariableInfo>
    {
        private readonly string _key;

        internal VariableInfo(string name, ValueType valueType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
            Type = valueType;

            _key = Name + Type;
        }

        /// <summary>
        ///     <c>Name</c> and <c>Type</c> help to identify the current variable
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     <c>Name</c> and <c>Type</c> help to identify the current variable
        /// </summary>
        public ValueType Type { get; }

        #region IComparable<Variable> Members

        /// <summary>
        ///     <c>CompareTo</c> implements IComparable.CompareTo
        /// </summary>
        /// <param name="other">The object being compared to 'this'</param>
        /// <returns>
        ///     -1, 0, or 1 depending on whether 'other' is less than,
        ///     equal to, or greater than 'this'
        /// </returns>
        public int CompareTo(VariableInfo other)
        {
            return string.CompareOrdinal(other._key, _key);
        }

        #endregion

        #region IEquatable<Variable> Members

        /// <summary>
        ///     <c>Equals</c> determines whether of not 'other' is equivalent to 'this'
        /// </summary>
        /// <param name="other">The object being compared to 'this'</param>
        /// <returns>boolean (true or false)</returns>
        public bool Equals(VariableInfo other)
        {
            return CompareTo(other) == 0;
        }

        #endregion

        /// <summary>
        ///     Overrides Object.Equals
        /// </summary>
        /// <param name="obj">The object to compare with to test for equality.</param>
        /// <returns>A value indicating whether or not the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return (obj != null) && obj is VariableInfo && Equals((VariableInfo) obj);
        }

        /// <summary>
        ///     Overrides Object.GetHashCode()
        /// </summary>
        /// <returns>a number of type int</returns>
        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        /// <summary>
        ///     Overrides Object.ToString()
        /// </summary>
        /// <returns>a string instance similar in content to 'this'</returns>
        public override string ToString()
        {
            return string.Format("Name: {0}  AnswerType: {1}", Name, Type);
        }
    }
}