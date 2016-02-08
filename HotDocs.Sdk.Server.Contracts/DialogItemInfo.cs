using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     This class provides information about items in a dialog.
    /// </summary>
    [DataContract(Name = "Item")]
    public class DialogItemInfo
    {
        /// <summary>
        ///     The name of the dialog item.
        /// </summary>
        [DataMember(Name = "Name", Order = 1)]
        public string Name { get; set; }

        /// <summary>
        ///     The name of the external data to which the dialog item is mapped. For example, if the dialog is mapped to a
        ///     database, this would be the name of the column in the database associated with this variable.
        /// </summary>
        [DataMember(Name = "Map", Order = 2, EmitDefaultValue = false)]
        public string Mapping { get; set; }
    }
}