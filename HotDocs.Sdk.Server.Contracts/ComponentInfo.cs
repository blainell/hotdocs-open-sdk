using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     This class provides information about components in a template's interview.
    /// </summary>
    [DataContract]
    public class ComponentInfo
    {
        private readonly Dictionary<string, bool> _varIndex = new Dictionary<string, bool>();

        /// <summary>
        ///     The list of dialogs contained in the template's interview.
        /// </summary>
        [DataMember(Order = 2, EmitDefaultValue = false)] public List<DialogInfo> Dialogs;

        /// <summary>
        ///     The list of variables contained in the template's interview.
        /// </summary>
        [DataMember(Order = 1)] public List<VariableInfo> Variables = new List<VariableInfo>();

        /// <summary>
        ///     This method indicates whether or not the variable is defined.
        /// </summary>
        /// <param name="variableName">The name of the variable to check.</param>
        /// <returns>True if the variable is defined, or false otherwise.</returns>
        public bool IsDefinedVariable(string variableName)
        {
            return _varIndex.ContainsKey(variableName);
        }

        /// <summary>
        ///     This method adds a variable to the list of variables for the component file.
        /// </summary>
        /// <param name="item">The variable to add to the list.</param>
        public void AddVariable(VariableInfo item)
        {
            Variables.Add(item);
            _varIndex[item.Name] = true;
        }

        /// <summary>
        ///     This method adds a dialog to the list of dialogs for the component file.
        /// </summary>
        /// <param name="item">The dialog to add to the list.</param>
        public void AddDialog(DialogInfo item)
        {
            if (Dialogs == null)
                Dialogs = new List<DialogInfo>(16); // default capacity
            Dialogs.Add(item);
        }
    }
}