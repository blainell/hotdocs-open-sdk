namespace HotDocs.Sdk
{
    /// <summary>
    ///     <c>Tristate</c> is a way to represent boolean values and allow "default" values if defined elsewhere
    /// </summary>
    public enum Tristate
    {
        /// <summary>
        ///     A <c>Default</c> value means the current setting is defined somewhere else, such as on HotDocs server.
        /// </summary>
        Default,

        /// <summary>
        ///     a <c>True</c> value means the respective boolean setting evaluates to the "true" value.
        /// </summary>
        True,

        /// <summary>
        ///     a <c>False</c> value means the respective boolean setting evaluates to the "false" value.
        /// </summary>
        False
    }
}