/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

namespace HotDocs.Sdk
{
    /// <summary>
    ///     AnswerSummaryOptions encapsulates the interview options related to Answer summaries.
    ///     They are partitioned into their own class (rather than being part of InterviewOptions) because
    ///     <list type="number">
    ///         <item>
    ///             <description>they're very specialized;</description>
    ///         </item>
    ///         <item>
    ///             <description>they are all irrelevant if the host application disables answer summaries; and</description>
    ///         </item>
    ///         <item>
    ///             <description>at some point in the future, answer summaries may be dealt with independently of interviews.</description>
    ///         </item>
    ///     </list>
    /// </summary>
    public class AnswerSummaryOptions
    {
        private static AnswerSummaryOptions s_default;

        static AnswerSummaryOptions()
        {
            s_default = null;
        }

        /// <summary>
        ///     Creates a new copy of the default answer summary options.  Override the built-in defaults with your preferred
        ///     defaults by adding appropriate settings to the appSettings section of your web.config file.
        /// </summary>
        public AnswerSummaryOptions() : this(Default)
        {
        }

        /// <summary>
        ///     Copy constructor -- this constructor is invoked to make a new copy of the (static) Default AnswerSummaryOptions
        ///     (as originally read from web.config).
        /// </summary>
        /// <param name="source">The source options to be copied.</param>
        internal AnswerSummaryOptions(AnswerSummaryOptions source)
        {
            Format = source.Format;
            Border = source.Border;
            PercentWidthForPrompt = source.PercentWidthForPrompt;
            BulletAnswers = source.BulletAnswers;
            BulletPrompts = source.BulletPrompts;
            IndentAnswers = source.IndentAnswers;
            IndentPrompts = source.IndentPrompts;
        }

        /// <summary>
        ///     Private constructor for reading defaults from web.config.
        ///     This constructor should only be invoked the first time the defaults are required.
        /// </summary>
        /// <param name="readDefaults">Determines if default values should be read (from a config file if one exists) or not.</param>
        private AnswerSummaryOptions(bool readDefaults)
        {
            if (readDefaults)
            {
                // all properties will be looked up in web.config & initialized from there if they exist,
                // or initialized to the indicated default values if they don't.

                Format = Util.ReadConfigurationEnum("AnswerSummaryFormat", AnswerSummaryFormat.Default);
                Border = Util.ReadConfigurationEnum("AnswerSummaryBorderType", BorderType.Default);
                PercentWidthForPrompt = Util.ReadConfigurationInt("AnswerSummaryPromptPercentWidth", 0);
                BulletAnswers = Util.ReadConfigurationTristate("AnswerSummaryBulletAnswers", Tristate.Default);
                BulletPrompts = Util.ReadConfigurationTristate("AnswerSummaryBulletPrompts", Tristate.Default);
                IndentAnswers = Util.ReadConfigurationTristate("AnswerSummaryIndentAnswers", Tristate.Default);
                IndentPrompts = Util.ReadConfigurationTristate("AnswerSummaryIndentPrompts", Tristate.Default);
            }
        }

        internal static AnswerSummaryOptions Default
        {
            get
            {
                // Avoid reading defaults out of web.config until/unless they are actually needed
                if (s_default == null)
                    s_default = new AnswerSummaryOptions(true);
                return s_default;
            }
        }

        /// <summary>
        ///     Answer summaries can be formatted in 1-column or 2-column format.
        ///     By default the property defers to the default configured on the server.
        /// </summary>
        /// <remarks>
        ///     This property is equivalent to the "Answer Summary Settings > Format" setting in the HotDocs Server
        ///     management console and the AsNumberOfColumns setting in Core Services.
        /// </remarks>
        public AnswerSummaryFormat Format { get; set; }

        /// <summary>
        ///     Specifies the border style for the answer summary table:
        ///     None, Plain or Sculpted. Applicable only to 2-column answer summary layout.
        ///     By default the property defers to the behavior configured on the server.
        /// </summary>
        /// <remarks>
        ///     This property is equivalent to the "Answer Summary Settings > Borders" setting in
        ///     the HotDocs Server management console and the AsBorders setting in Core Services.
        /// </remarks>
        public BorderType Border { get; set; }

        /// <summary>
        ///     Specifies the percentage of the total interview width reserved for prompts.
        ///     Applicable only to 2-column answer summary layout.
        ///     A range of 20 to 80 is recommended. The default value is 0, which defers to the value configured
        ///     on the server.
        /// </summary>
        /// <remarks>
        ///     This property is equivalent to the "Answer Summary Settings > Percentage of width taken by prompt"
        ///     setting in the HotDocs Server management console and the AsPercentOfWidthForPrompt setting in Core Services.
        /// </remarks>
        public int PercentWidthForPrompt { get; set; }

        /// <summary>
        ///     Indents the left margin of each prompt with respect to dialog titles. Applicable only to 1-column answer summary
        ///     layout.
        ///     By default the property defers to the behavior configured on the server.
        /// </summary>
        /// <remarks>
        ///     This property is equivalent to the "Answer Summary Settings > Indent prompts" setting
        ///     in the HotDocs Server management console and the AsIndentPrompts setting in Core Services.
        /// </remarks>
        public Tristate IndentPrompts { get; set; }

        /// <summary>
        ///     Precedes each prompt with a bullet character. Applicable only to 1-column answer summary layout.
        ///     By default the property defers to the behavior configured on the server.
        /// </summary>
        /// <remarks>
        ///     This property is equivalent to the "Answer Summary Settings > Precede prompts with bullet" setting
        ///     in the HotDocs Server management console and the AsBulletPrompts setting in Core Services.
        /// </remarks>
        public Tristate BulletPrompts { get; set; }

        /// <summary>
        ///     Indents the left margin of each answer with respect to its prompt. Applicable only to 1-column answer summary
        ///     layout.
        ///     By default the property defers to the behavior configured on the server.
        /// </summary>
        /// <remarks>
        ///     This property is equivalent to the "Answer Summary Settings > Indent answers" setting
        ///     in the HotDocs Server management console and the AsIndentAnswers setting in Core Services.
        /// </remarks>
        public Tristate IndentAnswers { get; set; }

        /// <summary>
        ///     Precedes each answer with a bullet character. Applicable only to 1-column answer summary layout.
        ///     By default the property defers to the behavior configured on the server.
        /// </summary>
        /// <remarks>
        ///     This property is equivalent to the "Answer Summary Settings > Precede answers with bullet" setting
        ///     in the HotDocs Server management console and the AsBulletAnswers setting in Core Services.
        /// </remarks>
        public Tristate BulletAnswers { get; set; }
    }
}