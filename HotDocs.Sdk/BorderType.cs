namespace HotDocs.Sdk
{
    /// <summary>
    ///     <c>BorderType</c> specifies the border style for the answer summary table:
    ///     None, Plain or Sculpted. Applicable only to 2-column answer summary layout.
    ///     By default the property defers to the behavior configured on the server.
    /// </summary>
    public enum BorderType
    {
        /// <summary>
        ///     defers to the behavior configured on the server.
        /// </summary>
        Default,

        /// <summary>
        ///     No border is applied
        /// </summary>
        None,

        /// <summary>
        ///     Plain border
        /// </summary>
        Plain,

        /// <summary>
        ///     Sculpted border
        /// </summary>
        Sculpted
    }
}