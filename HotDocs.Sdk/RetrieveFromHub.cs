namespace HotDocs.Sdk
{
    /// <summary>
    /// For use with cloud services specifying if you want to use a package located in the HotDocs Template Hub
    /// </summary>
    public enum RetrieveFromHub
    {
        /// <summary>
        /// Don't use a package from the template hub
        /// </summary>
        No,
        /// <summary>
        /// Use a package from the template hub where the templateId is equal to the packageId you are requesting (The live version of that template will be requested).
        /// </summary>
        ByTemplateId,
        /// <summary>
        /// Use a package from the template hub where the templateVersionId is equal to the packageId you are requesting.
        /// </summary>
        ByVersionId
    }
}
