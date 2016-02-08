namespace HotDocs.Sdk.Server
{
    /// <summary>
    /// This delegate type allows a host application to request notification immediately following the SDK assembling
    /// a document during the execution of the WorkSession.AssembleDocuments method.
    /// </summary>
    /// <param name="template">The Template from which a new document was just assembled.</param>
    /// <param name="result">The AssemblyResult object associated with the assembled document.  The SDK has not yet
    /// processed this AssemblyResult.  The Document inside the result will be added to the Document array
    /// that will eventually be returned by AssembleDocuments.  The Answers will become the new answers for subsequent
    /// work in this WorkSession.</param>
    /// <param name="userState">Whatever state object is passed by the host application into
    /// WorkSession.AssembleDocuments will be passed back out to the host application in this userState parameter.</param>
    public delegate void PostAssembleDocumentDelegate(Template template, AssembleDocumentResult result, object userState);
}