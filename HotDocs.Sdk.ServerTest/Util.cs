/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Local;

namespace HotDocs.Sdk.ServerTest
{
    internal static class Util
    {
        public static string TestFilesPath
        {
            get
            {
                var _asm = Assembly.GetExecutingAssembly();
                var testFilesPath = Path.Combine(Path.GetDirectoryName(_asm.Location), "TestFiles");
                return testFilesPath;
            }
        }

        public static StreamReader GetTestFile(string fileName)
        {
            var _assembly = Assembly.GetExecutingAssembly();
            return new StreamReader(_assembly.GetManifestResourceStream("HotDocs.Sdk.ServerTest.TestFiles." + fileName));
        }

        public static string GetFileContentAsString(string filePath)
        {
            string content;
            using (var fs = File.OpenRead(filePath))
            {
                TextReader rdr = new StreamReader(fs);
                content = rdr.ReadToEnd();
            }
            return content;
        }

        public static PackagePathTemplateLocation CreatePackagePathLocation(string packageID)
        {
            //Note that in this sample portal, the package ID is used to construct the package file name, but this does not need to be the case.
            var templateDirectory = Path.Combine(GetSamplePortalTemplateDir(), "TestTemplates");
            var packagePath = Path.Combine(templateDirectory, packageID + ".pkg");

            if (!File.Exists(packagePath))
                throw new Exception("The template does not exist.");
            return new PackagePathTemplateLocation(packageID, packagePath);
        }

        public static Template OpenTemplate(string packageID)
        {
            return new Template(CreatePackagePathLocation(packageID));
        }

        public static IServices GetLocalServicesInterface()
        {
            return new Services(Path.GetTempPath());
        }

        public static IServices GetWebServiceServicesInterface()
        {
            var endPointName = ConfigurationManager.AppSettings["WebServiceEndPoint"];

            var templatePath = Path.Combine(GetSamplePortalTemplateDir());

            return new Server.WebService.Services(endPointName, templatePath);
        }

        public static IServices GetCloudServicesInterface()
        {
            var cloudSigningKey = ConfigurationManager.AppSettings["SigningKey"];
            var cloudSubscriberID = ConfigurationManager.AppSettings["SubscriberID"];
            return new Server.Cloud.Services(cloudSubscriberID, cloudSigningKey);
        }

        public static string GetTestProjectPath()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var root = "";
            if (Path.IsPathRooted(assemblyPath))
                root = Path.GetPathRoot(assemblyPath);
            var tokens = assemblyPath.Substring(root.Length)
                .Split(Path.DirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (tokens.Length <= 3)
                throw new Exception("Invalid path.");
            var subTokens = new string[tokens.Length - 3];
            //Get the path minus two folders and the file name, hence three.
            Array.Copy(tokens, subTokens, tokens.Length - 3);
            var testProjectPath = Path.Combine(subTokens);
            if (root.Length > 0)
                testProjectPath = Path.Combine(root, testProjectPath);
            return testProjectPath;
        }

        public static string GetSamplePortalAnswersDir()
        {
            var samplePortalRoot = GetSamplePortalRootedPath();
            var filesDir = Path.Combine(samplePortalRoot, "Files");
            return Path.Combine(filesDir, "Answers");
        }

        public static string GetTestAnswersDir()
        {
            return Path.Combine(GetSamplePortalAnswersDir(), "TestAnswers");
        }

        public static string GetSamplePortalTemplateDir()
        {
            var samplePortalRoot = GetSamplePortalRootedPath();
            var filesDir = Path.Combine(samplePortalRoot, "Files");
            return Path.Combine(filesDir, "Templates");
        }

        #region Private methods

        private static string GetSamplePortalRootedPath()
        {
            string sRet;
            // assemblyBinariesFolder should be bin\Debug or bin\Release, within the current solution folder:
            var assemblyBinariesFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var serverTestRoot = Directory.GetParent(Directory.GetParent(assemblyBinariesFolder).ToString()).ToString();
            var sdkRoot = Directory.GetParent(serverTestRoot).ToString();
            sRet = Path.Combine(sdkRoot, "SamplePortal");
            return sRet;
        }

        #endregion // Private methods
    }
}