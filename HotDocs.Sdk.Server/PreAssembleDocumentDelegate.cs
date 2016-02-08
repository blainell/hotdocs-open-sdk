namespace HotDocs.Sdk.Server
{
    /// <summary>
    /// This delegate type allows a host application to request notification immediately prior to the SDK assembling
    /// a specific document (during the execution of the WorkSession.AssembleDocuments method).
    /// </summary>
    /// <param name="template">The Template from which a new document is about to be assembled.</param>
    /// <param name="answers">The AnswerCollection which will be used for the assembly.  This represents the current
    /// state of the answer session for the entire WorkSession, and if modified, will modify the answers from here
    /// through to the end of the answer session.</param>
    /// <param name="settings">The AssembleDocumentSettings that will be used for this specific document assembly. This is
    /// a copy of the WorkSession's DefaultAssemblySettings; if modified it affects only the current assembly.</param>
    /// <param name="userState">Whatever state object is passed by the host application into
    /// WorkSession.AssembleDocuments will be passed back out to the host application in this userState parameter.</param>
    public delegate void PreAssembleDocumentDelegate(Template template, AnswerCollection answers, AssembleDocumentSettings settings, object userState);
}