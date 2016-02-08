namespace HotDocs.Sdk
{
    /// <summary>
    /// HotDocs uses <c>DateOrder</c> setting during assembly when it needs to interpret ambiguously-written date strings
    /// (for example, date format examples in which the day, month and/or year are all expressed as numbers).
    /// By default this property respects the behavior configured on the server.  Also note that this property
    /// is only respected by HotDocs Cloud Services; HotDocs Server always adheres to the behavior as configured
    /// in the management console.
    /// </summary>
    public enum DateOrder {

        /// <summary>
        /// defers to the behavior configured on the server.
        /// </summary>
        Default, 

        /// <summary>
        /// Uses Day/Month/Year (dd/mm/yyyy) format for date strings
        /// </summary>
        DMY, 
		
        /// <summary>
        /// Uses Month/Day/Year (mm/dd/yyyy) format for date strings
        /// </summary>
        MDY 
    }
}