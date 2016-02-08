namespace HotDocs.Sdk
{
    /// <summary>
    ///     <c>DataSourceBackfillType</c> enumerates the ways in which modified answers
    ///     may be written back to the original data source.
    /// </summary>
    public enum DataSourceBackfillType
    {
        /// <summary>
        ///     Indicates never store the current field back to the data source.
        /// </summary>
        Never,

        /// <summary>
        ///     Indicates always store the current field back to the data source.
        /// </summary>
        Always,

        /// <summary>
        ///     Indicates to prompt whether to store the current field back to
        ///     the data source.
        /// </summary>
        Prompt,

        /// <summary>
        ///     Indicates the the current field should be specifically prevented from
        ///     storing back to the data source.
        /// </summary>
        DoNotAllow
    }
}