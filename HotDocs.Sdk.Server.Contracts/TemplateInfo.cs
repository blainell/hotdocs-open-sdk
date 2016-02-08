using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     This class provides details about a template.
    /// </summary>
    [DataContract]
    public class TemplateInfo
    {
        /// <summary>
        ///     The description of the template.
        /// </summary>
        [DataMember(Order = 3, EmitDefaultValue = false)] public string Description;

        /// <summary>
        ///     The ID of the template.
        /// </summary>
        [DataMember(Order = 1)] public string ID;

        /// <summary>
        ///     The title of the template.
        /// </summary>
        [DataMember(Order = 2)] public string Title;
    }
}