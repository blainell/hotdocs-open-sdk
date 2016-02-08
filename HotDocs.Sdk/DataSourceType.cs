namespace HotDocs.Sdk
{
    /// <summary>
    /// <c>DataSourceType</c> indicates from where the answers were supplied to the interview or assembly.
    /// </summary>
    public enum DataSourceType
    {
        /// <summary>
        /// Indicates the current answer file is being supplied to the interview or assembly.
        /// </summary>
        CurrentAnswerFile,

        /// <summary>
        /// Indicates a specified answer file is being supplied to the interview or assembly.
        /// </summary>
        AnswerFile,

        /// <summary>
        /// Indicates answers from a database component are being supplied to the interview or assembly.
        /// </summary>
        DatabaseComponent,

        /// <summary>
        /// Indicates answers from a customized source are being supplied to the interview or assembly.
        /// </summary>
        Custom
    };
}