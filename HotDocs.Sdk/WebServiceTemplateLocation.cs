using System;
using System.Configuration;
using System.IO;
using System.Net.Http;

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
                HttpResponseMessage result = client.GetAsync(string.Format(HostAddress + "/?packageid={0}&filename={1}", PackageID, fileName)).Result;

                if (result.IsSuccessStatusCode)
                {
                    return result.Content.ReadAsStreamAsync().Result;
                }

                using (var sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
                {
                    throw new Exception(String.Format("The file '{0}' could not be retrieved: {1}", fileName,sr.ReadToEnd()));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="templatename"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Stream GetFile(string fileName, string templatename)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = client.GetAsync(string.Format(HostAddress + "/?packageid={0}&templatename={1}&filename={2}", PackageID,templatename, fileName)).Result;

                if (result.IsSuccessStatusCode)
                {
                    return result.Content.ReadAsStreamAsync().Result;
                }
                using (var sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
                {
                    throw new Exception(String.Format("The file '{0}' could not be retrieved: {1}", fileName, sr.ReadToEnd()));
                }
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

