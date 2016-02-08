using System;

namespace HotDocs.Sdk
{
    /// <summary>
    /// These flags enumerate which portions of a template manifest (or manifests) can be parsed (when calling <c>ParseManifest</c>).
    /// </summary>
    [Flags]
    public enum ManifestParseFlags
    {
        /// <summary>
        /// Find the basic metadata about a template: its version, title, description, effective component file, expiration date, etc.
        /// </summary>
        ParseTemplateInfo		= 0x0001,
        /// <summary>
        /// Find the variables that are used by the template or its interview.
        /// </summary>
        ParseVariables			= 0x0002,
        /// <summary>
        /// Find the dependencies this template has on other templates, component files, images, etc.
        /// </summary>
        ParseDependencies		= 0x0004,
        /// <summary>
        /// Find additional dependencies this template may have on other files that are not typically managed by HotDocs.
        /// </summary>
        ParseAdditionalFiles	= 0x0008,
        /// <summary>
        /// Find the answer sources used in this template.
        /// </summary>
        ParseDataSources		= 0x0010,
        /// <summary>
        /// When this flag is set, information will be compiled by recursively parsing the manifests for this template
        /// and any templates inserted by this one as well.
        /// </summary>
        ParseRecursively		= 0x0020,
        /// <summary>
        /// Reads and parses all available information in the manifest.
        /// </summary>
        ParseAll				= ParseVariables|ParseDependencies|ParseAdditionalFiles|ParseDataSources
    };
}