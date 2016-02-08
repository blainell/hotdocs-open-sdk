using System;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     <c>DataSourceField</c> provides information about the current field (such as source name, type, etc.)
    /// </summary>
    public class DataSourceField
    {
        internal DataSourceField(string sourceName, DataSourceFieldType fieldType, DataSourceBackfillType backfillType,
            bool isKey)
        {
            if (string.IsNullOrEmpty(sourceName))
                throw new ArgumentNullException("sourceName");

            SourceName = sourceName;
            FieldType = fieldType;
            BackfillType = backfillType;
            IsKey = isKey;
        }

        /// <summary>
        ///     <c>SourceName</c> is a string that describes the source of the current data field.
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        ///     <c>DataSourceFieldType</c> is an enumeration that describes the type (such as text, number, etc.)
        ///     of the current field.
        /// </summary>
        public DataSourceFieldType FieldType { get; private set; }

        /// <summary>
        ///     <c>DataSourceBackfillType</c> is an enumeration that specifies
        ///     whether and under what conditions the current field will be stored back to
        ///     the data source.
        /// </summary>
        public DataSourceBackfillType BackfillType { get; private set; }

        /// <summary>
        ///     <c>IsKey</c> is a boolean key that indicates whether or not the current data source field
        ///     is a key for unique identification.
        /// </summary>
        public bool IsKey { get; private set; }
    }
}