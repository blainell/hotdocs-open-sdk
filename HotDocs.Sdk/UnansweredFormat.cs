namespace HotDocs.Sdk
{
    /// <summary>
    /// Dictates what to merge when a value for which no answer is available is merged into document text.
    /// By default this property respects the behavior configured on the server.  Also note that this property
    /// is only respected by HotDocs Cloud Services; HotDocs Server always adheres to the behavior as configured
    /// in the management console.
    /// </summary>
    public enum UnansweredFormat 
    {
        /// <summary>
        /// defers to the behavior configured on the server.
        /// </summary>
        Default, 

        /// <summary>
        /// Does nothing to merge a value for which no answer is available
        /// </summary>
        Nothing, 

        /// <summary>
        /// Provides an underscore to format the unanswered variable
        /// </summary>
        Underscores,


        /// <summary>
        /// Provides an asterisks to format the unanswered variable
        /// </summary>
        Asterisks, 
		
        /// <summary>
        /// Provides the variable name with brackets to format the unanswered variable
        /// </summary>
        VarNameWithBrackets,

        /// <summary>
        /// Provides the variable name with asterisks to format the unanswered variable
        /// </summary>
        VarNameWithAsterisks 
    }
}