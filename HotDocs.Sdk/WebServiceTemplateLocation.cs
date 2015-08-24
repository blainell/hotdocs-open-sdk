using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace HotDocs.Sdk
{
    public class WebServiceTemplateLocation : PackageTemplateLocation
    {

        private string _templateDir = null;

        public string HostAddress { get; protected set; }

        public WebServiceTemplateLocation(string packageID, string hostAddress)
            : base(packageID)
        {
            HostAddress = hostAddress;
            _templateDir = GetTemplateDirectory();
        }

        public override TemplateLocation Duplicate()
        {
            var location = new WebServiceTemplateLocation(PackageID, HostAddress) { _templateDir = _templateDir };
            return location;
        }

        public override int GetHashCode()
        {
            const int prime = 397;
            int result = HostAddress.ToLower().GetHashCode(); // package path must be case-insensitive
            result = (result * prime) ^ PackageID.GetHashCode(); // combine the hashes
            return result;
        }

        public override bool Equals(TemplateLocation other)
        {
            var otherPackagePathLoc = other as WebServiceTemplateLocation;
            return (otherPackagePathLoc != null)
                   && string.Equals(PackageID, otherPackagePathLoc.PackageID, StringComparison.Ordinal)
                   && string.Equals(HostAddress, otherPackagePathLoc.HostAddress, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(_templateDir, otherPackagePathLoc._templateDir, StringComparison.Ordinal);
        }

        public override Stream GetFile(string fileName)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = client.GetAsync(string.Format(HostAddress + "/0/{0}?filename={1}", PackageID, fileName)).Result;

                if (result.IsSuccessStatusCode)
                {
                    return result.Content.ReadAsStreamAsync().Result;
                }

                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception(result.ReasonPhrase);
                }

                throw new Exception(String.Format("The server returned a '{0}' when searching for the file '{1}'", result.StatusCode.ToString(), fileName));
            }
        }

        public override sealed string GetTemplateDirectory()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = client.GetAsync(HostAddress + "/GetDirectory").Result;
                return result.Content.ReadAsStringAsync().Result;
            }
        }

        protected override string SerializeContent()
        {
            return PackageID + "|" + HostAddress;
        }

        protected override void DeserializeContent(string content)
        {
            if (content == null)
                throw new ArgumentNullException();

            string[] tokens = content.Split(new char[] { '|' });
            if (tokens.Length != 2)
                throw new Exception("Invalid template location.");

            PackageID = tokens[0];
            HostAddress = tokens[1];
        }

        public override Stream GetPackageStream()
        {
            throw new Exception("The package file stream is not available from the cache");
        }

        public override TemplatePackageManifest GetPackageManifest()
        {
            using (Stream stream = GetFile("manifest.xml"))
            {
                return TemplatePackageManifest.FromStream(stream);
            }
        }
    }
}

