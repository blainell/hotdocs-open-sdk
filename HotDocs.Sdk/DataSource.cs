using System;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     <c>DataSource</c> has information about the current data source, such as id, name, type, and fields.
    /// </summary>
    public class DataSource : IEquatable<DataSource>, IComparable<DataSource>
    {
        internal DataSource(string id, string name, DataSourceType type, DataSourceField[] fields)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Id = id;
            Name = name;
            Type = type;
            Fields = fields;
        }

        /// <summary>
        ///     <c>Id</c> is a string the identifies the current data source.
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     <c>Name</c> is a string that provides a name for the current data source.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     <c>Type</c> is an enumeration that indicates the type of the current data source.
        /// </summary>
        public DataSourceType Type { get; }

        /// <summary>
        ///     <c>Fields</c> a group of data fields associated with the current data source.
        /// </summary>
        public DataSourceField[] Fields { get; private set; }

        #region IComparable<DataSource> Members

        /// <summary>
        ///     Overrides IComparable&lt;DataSource&gt;.CompareTo
        /// </summary>
        /// <param name="other">The object being compared to 'this'</param>
        /// <returns>
        ///     -1, 0, or 1 depending on whether 'other' is less than,
        ///     equal to, or greater than 'this'
        /// </returns>
        public int CompareTo(DataSource other)
        {
            return string.CompareOrdinal(other.Id, Id);
        }

        #endregion

        #region IEquatable<DataSource> Members

        /// <summary>
        ///     Overrides IEquatable<DataSource>.Equals</DataSource>
        /// </summary>
        /// <param name="other">The object being compared to 'this'</param>
        /// <returns>boolean (true or false)</returns>
        public bool Equals(DataSource other)
        {
            return CompareTo(other) == 0;
        }

        #endregion

        /// <summary>
        ///     Overrides Object.Equals
        /// </summary>
        /// <param name="obj">The object being compared to 'this'</param>
        /// <returns>boolean (true or false)</returns>
        public override bool Equals(object obj)
        {
            return (obj != null) && obj is DataSource && Equals((DataSource) obj);
        }

        /// <summary>
        ///     Overrides Object.GetHashCode
        /// </summary>
        /// <returns>a number of type int</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        ///     Overrides Object.ToString
        /// </summary>
        /// <returns>a string instance similar in content to 'this'</returns>
        public override string ToString()
        {
            return string.Format("Id: {0}  Name: {1}  Type: {2}", Id, Name, Type);
        }
    }
}