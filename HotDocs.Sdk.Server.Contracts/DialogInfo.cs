using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     This class provides details about a dialog. In the XML, the name of this class is "Dlg".
    /// </summary>
    [DataContract(Name = "Dlg")]
    public class DialogInfo
    {
        /// <summary>
        ///     A list of items contained in the dialog.
        /// </summary>
        [DataMember(Order = 4)] public List<DialogItemInfo> Items = new List<DialogItemInfo>();

        /// <summary>
        ///     The name of the dialog.
        /// </summary>
        /// <value>The name of this property in the XML is "Name".</value>
        [DataMember(Name = "Name", Order = 1)]
        public string Name { get; set; }

        /// <summary>
        ///     Indicates whether or not the dialog is repeated.
        /// </summary>
        /// <value> The name of this property in the XML is "Rpt".</value>
        [DataMember(Name = "Rpt", Order = 2, EmitDefaultValue = false)]
        public bool Repeat { get; set; }

        /// <summary>
        ///     The name of the answer source associated with the dialog.
        /// </summary>
        /// <value>The name of this property in the XML is "Src".</value>
        [DataMember(Name = "Src", Order = 3, EmitDefaultValue = false)]
        public string AnswerSource { get; set; }
    }
}