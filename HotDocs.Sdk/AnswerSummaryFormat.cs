namespace HotDocs.Sdk
{
    /// <summary>
    /// Answer summaries can be formatted in 1-column or 2-column format.
    /// By default the property defers to the default configured on the server.
    /// </summary>
    public enum AnswerSummaryFormat {
        /// <summary>
        /// defers to the behavior configured on the server.
        /// </summary>
        Default, 

        /// <summary>
        /// One column format for the answer summaries
        /// </summary>
        OneColumn, 
		
        /// <summary>
        /// Two column format for the answer summaries
        /// </summary>
        TwoColumns 
    }
}