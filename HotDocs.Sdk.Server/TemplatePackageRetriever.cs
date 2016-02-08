using System.IO;

namespace HotDocs.Sdk.Server
{
    internal delegate Stream TemplatePackageRetriever(string packageID, string state, out bool closeStream);
}