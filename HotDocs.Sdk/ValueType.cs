namespace HotDocs.Sdk
{
    /// <summary>
    /// An enumeration for the various types of HotDocs answer values.
    /// </summary>
    public enum ValueType {
        /// <summary>
        /// A value of some type that is not directly understood by the Open SDK.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// A value of some type that is not directly understood by the Open SDK.
        /// This is a synonym for Unknown, and is included only for backwards compatibility.
        /// </summary>
        Other = 0,
        /// <summary>
        /// A text value. HotDocs Text variables have answers of this type.
        /// </summary>
        Text = 1,
        /// <summary>
        /// A numeric value. HotDocs Number variables have answers of this type.
        /// </summary>
        Number,
        /// <summary>
        /// A date value. HotDocs Date variables have answers of this type.
        /// </summary>
        Date,
        /// <summary>
        /// A true/false (Boolean) value. HotDocs True/False variables and grouped child dialogs have answers of this type.
        /// </summary>
        TrueFalse,
        /// <summary>
        /// A multiple choice value, which is represented by an array of one or more text strings.
        /// HotDocs Multiple Choice variables have answers of this type.
        /// </summary>
        MultipleChoice };
}