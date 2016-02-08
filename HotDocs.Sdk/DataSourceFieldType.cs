namespace HotDocs.Sdk
{
    /// <summary>
    /// <c>DataSourceFieldType</c> lists the possible types of fields in data sources (Text, Number, Date, or TrueFalse).
    /// </summary>
    public enum DataSourceFieldType
    {
        /// <summary>
        /// The type of the data source field is <c>Text</c>
        /// </summary>
        Text,

        /// <summary>
        /// The type of the data source field is <c>Number</c>
        /// </summary>
        Number,

        /// <summary>
        /// The type of the data source field is <c>Date</c>
        /// </summary>
        Date,

        /// <summary>
        /// The type of the data source field is <c>TrueFalse</c>
        /// </summary>
        TrueFalse
    };
}