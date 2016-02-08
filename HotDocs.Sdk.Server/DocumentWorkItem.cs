using System;
using System.Collections.Generic;

namespace HotDocs.Sdk.Server
{
    /// <summary>
    /// A DocumentWorkItem represents a document that needs to be (or has been) generated as part of a WorkSession.
    /// It keeps track of the template used to generate the document, and for documents that have already been generated,
    /// it also tracks the unanswered variables encountered during assembly.
    /// </summary>
    [Serializable]
    public class DocumentWorkItem : WorkItem // internally the DocumentWorkItem keeps track of where, in temporary storage, its assembled document is stored
    {
        /// <summary>
        /// The constructor is internal; it is only called from the WorkSession class (and maybe the InterviewWorkItem class).
        /// </summary>
        /// <param name="template">The template upon which the work item is based.</param>
        internal DocumentWorkItem(Template template) : this(template, new string[0]) { }
        internal DocumentWorkItem(Template template, string[] unansweredVariables)
            : base(template)
        {
            UnansweredVariables = unansweredVariables;
        }
        // properties/state

        /// <summary>
        /// A list of variable names for which no answer was present during assembly of the document.
        /// Before an assembly has occurred, this property will be null.  After an assembly is complete,
        /// this property will be an array of strings. Each string is the name of a variable for which
        /// an answer was called for during assembly (either to merge into the document, or in the evaluation
        /// of some other expression or rule encountered during assembly), but for which no answer was
        /// present in the AnswerCollection.
        /// </summary>
        /// <remarks>
        /// Necessary because a host application may want or need this information as part of the state associated with
        /// a WorkSession.
        /// </remarks>
        public IEnumerable<string> UnansweredVariables { get; internal set; }
    }
}