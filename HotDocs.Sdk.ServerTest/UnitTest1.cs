﻿/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Cloud;
using HotDocs.Sdk.Server.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.Sdk.ServerTest
{
    /// <summary>
    ///     Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            TemplateLocation.RegisterLocation(typeof (PackagePathTemplateLocation));
            TemplateLocation.RegisterLocation(typeof (PathTemplateLocation));
        }

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region IServices Constructor Tests

        [TestMethod]
        public void Services_Constructor_Cloud()
        {
            // Verify that we can construct a new Cloud.Services object or get an appropriate exception if one or both of the parameters are null.

            CloudServiceConstructorTester("subscriberID", "signingKey");
            CloudServiceConstructorTester(null, "signingKey");
            CloudServiceConstructorTester("subscriberID", null);
            CloudServiceConstructorTester(null, null);
        }

        private void CloudServiceConstructorTester(string id, string key)
        {
            try
            {
                IServices services = new Services(id, key);
                Assert.IsTrue(services is Services);

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(key))
                    Assert.Fail(); // We should have had an exception before reaching this.
            }
            catch (ArgumentNullException)
            {
                Assert.IsTrue(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(key));
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        #endregion

        #region GetInterview Tests

        [Ignore]
        [TestMethod]
        public void GetInterview_Local()
        {
            var svc = Util.GetLocalServicesInterface();
            var logRef = "GetInterview_Local Unit Test";
            GetInterview(svc, logRef);
        }

        [Ignore]
        [TestCategory("WebServiceIntegration"), TestMethod]
        public void GetInterview_WebService()
        {
            var svc = Util.GetWebServiceServicesInterface();
            var logRef = "GetInterview_WebService Unit Test";
            GetInterview(svc, logRef);
        }

        [Ignore]
        [TestCategory("CloudIntegration"), TestMethod]
        public void GetInterview_Cloud()
        {
            var svc = Util.GetCloudServicesInterface();
            var logRef = "GetInterview_Cloud Unit Test";
            GetInterview(svc, logRef);
        }

        private void GetInterview(IServices svc, string logRef)
        {
            // Set up the InterviewOptions for the test.
            var tmp = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
            var postInterviewUrl = "PostInterview.aspx";
            var styleSheetUrl = "HDServerFiles/Stylesheets";
            var runtimeUrl = "HDServerFiles/js";
            var interviewDefUrl = "GetInterviewFile.ashx";
            var settings = new InterviewSettings(postInterviewUrl, runtimeUrl, styleSheetUrl, interviewDefUrl);

            // Set up the Marked Variables for the test.
            string[] markedVars = null;

            InterviewResult result;

            // Make sure that the parameters are being validated correctly.
            try
            {
                svc.GetInterview(null, null, null, null, null);
                Assert.Fail();
                // If we get here then the exceptions were not fired as they should have been with all null parameters.
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(": template") >= 0);
            }
            catch (Exception)
            {
                Assert.Fail(); // Not expecting a generic exception.
            }

            result = svc.GetInterview(tmp, null, settings, markedVars, logRef);
            Assert.AreNotEqual(result.HtmlFragment, "");
            Assert.IsTrue(result.HtmlFragment.Contains(settings.PostInterviewUrl));
            Assert.IsTrue(result.HtmlFragment.Contains(runtimeUrl));
            Assert.IsTrue(result.HtmlFragment.Contains(styleSheetUrl));
            Assert.IsTrue(result.HtmlFragment.Contains(interviewDefUrl));
            Assert.IsTrue(result.HtmlFragment.Contains("hdMainDiv"));
            Assert.IsFalse(result.HtmlFragment.Contains("Employee Name\": { t: \"TX\", m:true"));
            // Employee Name should not be "marked"

            // Now get another interview, but this time specify a url for doc preview and save answers.
            settings.DocumentPreviewUrl = "DocPreview.aspx";
            settings.SaveAnswersUrl = "SaveAnswers.aspx";
            settings.Format = InterviewFormat.JavaScript; // explicitly set format to JS.
            result = svc.GetInterview(tmp, null, settings, markedVars, logRef);
            Assert.IsTrue(result.HtmlFragment.Contains(settings.DocumentPreviewUrl));
            Assert.IsTrue(result.HtmlFragment.Contains(settings.SaveAnswersUrl));
            Assert.IsTrue(result.HtmlFragment.Contains("HDJavaScriptInterview"));

            // Now get another interview, but this time do the following:
            // 1. Disable the doc preview and save answers urls.
            // 2. Do not include the hdMainDiv.
            // 3. "Mark" the Employee Name variable.
            // 4. Set the interview format to Silverlight.
            settings.DisableDocumentPreview = true;
            settings.DisableSaveAnswers = true;
            settings.AddHdMainDiv = Tristate.False;
            settings.Format = InterviewFormat.Silverlight;
            markedVars = new[] {"Employee Name"};
            result = svc.GetInterview(tmp, null, settings, markedVars, logRef);
            Assert.IsFalse(result.HtmlFragment.Contains(settings.DocumentPreviewUrl));
            Assert.IsFalse(result.HtmlFragment.Contains(settings.SaveAnswersUrl),
                "No Save Ans Url because it is disabled");
            Assert.IsTrue(result.HtmlFragment.Contains("Employee Name\": { t: \"TX\", m:true"));
            // This interview does "mark" Employee Name.
            Assert.IsTrue(result.HtmlFragment.Contains("HDSilverlightInterview"));

            // Only HotDocs Cloud Services honors the AddHdMainDiv property of InterviewSettings, so only bother checking it if we are running a test against cloud services.
            if (svc is Services)
                Assert.IsFalse(result.HtmlFragment.Contains("hdMainDiv"));

            // Now try once more with a null value for settings to allow the default settings to be used.
            // Also, in this test we are using an actual answer file so that we can test using answers.
            TextReader answers = Util.GetTestFile("Freddy.xml");
            result = svc.GetInterview(tmp, answers, null, markedVars, logRef);
            Assert.IsTrue(result.HtmlFragment.Contains("Freddy"));
        }

        #endregion

        #region AssembleDocument Tests

        [Ignore]
        [TestMethod]
        public void AssembleDocument_Local()
        {
            var svc = Util.GetLocalServicesInterface();
            var logRef = "AssembleDocument_Local Unit Test";
            AssembleDocument(svc, logRef);
        }

        [Ignore]
        [TestCategory("WebServiceIntegration"), TestMethod]
        public void AssembleDocument_WebService()
        {
            var svc = Util.GetWebServiceServicesInterface();
            var logRef = "AssembleDocument_WebService Unit Test";
            AssembleDocument(svc, logRef);
        }

        [Ignore]
        [TestCategory("CloudIntegration"), TestMethod]
        public void AssembleDocument_Cloud()
        {
            var svc = Util.GetCloudServicesInterface();
            var logRef = "AssembleDocument_Cloud Unit Test";
            AssembleDocument(svc, logRef);
        }

        private void AssembleDocument(IServices svc, string logRef)
        {
            var tmp = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
            TextReader answers = new StringReader("");
            var settings = new AssembleDocumentSettings();
            AssembleDocumentResult result;

            // Verify that a null template throws the right exception.
            try
            {
                result = svc.AssembleDocument(null, answers, settings, logRef);
                Assert.Fail(); // We should not have reached this point.
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsTrue(ex.Message.Contains("template"));
            }
            catch (Exception)
            {
                Assert.Fail(); // We are not expecting a generic exception.
            }

            // Pass a null for settings, answers, and logRef to ensure that defaults are used.
            result = svc.AssembleDocument(tmp, null, null, null);
            Assert.AreEqual(result.PendingAssembliesCount, 0);
            Assert.AreEqual(0, result.Document.SupportingFiles.Count());
            Assert.AreEqual(0, result.PendingAssembliesCount);
            ;

            settings.Format = DocumentType.MHTML;
            result = svc.AssembleDocument(tmp, answers, settings, logRef);
            Assert.AreEqual(0, result.Document.SupportingFiles.Count());
            // The MHTML is a single file (no external images).

            settings.Format = DocumentType.HTMLwDataURIs;
            result = svc.AssembleDocument(tmp, answers, settings, logRef);
            Assert.AreEqual(0, result.Document.SupportingFiles.Count());
            // The HTML with Data URIs is a single file (no external images).

            settings.Format = DocumentType.HTML;
            result = svc.AssembleDocument(tmp, answers, settings, logRef);
            Assert.AreEqual(1, result.Document.SupportingFiles.Count()); // The HTML contains one external image file.

            //Assemble a document with an answer file containing a transient answer.
            var transAnsPath = Path.Combine(Util.GetTestAnswersDir(), "TransAns.anx");
            TextReader transAns = new StreamReader(new FileStream(transAnsPath, FileMode.Open));
            svc.AssembleDocument(tmp, transAns, null, logRef);

            // Now try with another template, which contains an ASSEMBLE instruction.
            tmp = Util.OpenTemplate("TemplateWithAssemble");
            result = svc.AssembleDocument(tmp, null, null, logRef);
            Assert.AreEqual(1, result.PendingAssembliesCount);
        }

        #endregion

        #region GetComponentInfo Tests

        [Ignore]
        [TestMethod]
        public void GetComponentInfo_Local()
        {
            var services = Util.GetLocalServicesInterface();
            var logRef = "GetComponentInfo_Local Unit Test";
            GetComponentInfo(services, logRef);
        }

        [Ignore]
        [TestCategory("WebServiceIntegration"), TestMethod]
        public void GetComponentInfo_WebService()
        {
            var services = Util.GetWebServiceServicesInterface();
            var logRef = "GetComponentInfo_WebService Unit Test";
            GetComponentInfo(services, logRef);
        }

        [Ignore]
        [TestCategory("CloudIntegration"), TestMethod]
        public void GetComponentInfo_Cloud()
        {
            var services = Util.GetCloudServicesInterface();
            var logRef = "GetComponentInfo_Local Unit Test";
            GetComponentInfo(services, logRef);
        }

        private void GetComponentInfo(IServices svc, string logRef)
        {
            var tmp = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
            ComponentInfo result;

            // Ensure that invalid parameters are throwing appropriate exceptions.
            try
            {
                result = svc.GetComponentInfo(null, false, null);
                Assert.Fail(); // Should not reach here with a null template.
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsTrue(ex.Message.Contains("template"));
            }
            catch (Exception)
            {
                Assert.Fail(); // We are not expecting any generic exceptions.
            }

            result = svc.GetComponentInfo(tmp, false, logRef);
            Assert.IsNull(result.Dialogs); // We did not request dialogs.
            Assert.AreEqual(result.Variables.Count, 20);

            // Now try it again, but request dialogs this time.
            result = svc.GetComponentInfo(tmp, true, logRef);
            Assert.IsNotNull(result.Dialogs); // We did request dialogs this time.
            Assert.AreEqual(result.Dialogs.Count, 4);
            Assert.AreEqual(result.Variables.Count, 20);
        }

        #endregion

        #region GetAnswers Tests

        [TestMethod]
        public void GetAnswers_Local()
        {
            var services = Util.GetLocalServicesInterface();
            var logRef = "GetAnswers_Local Unit Test";
            GetAnswers(services, logRef);
        }

        [Ignore]
        [TestCategory("WebServiceIntegration"), TestMethod]
        public void GetAnswers_WebService()
        {
            var services = Util.GetWebServiceServicesInterface();
            var logRef = "GetAnswers_WebService Unit Test";
            GetAnswers(services, logRef);
        }

        [Ignore]
        [TestCategory("CloudIntegration"), TestMethod]
        public void GetAnswers_Cloud()
        {
            var services = Util.GetCloudServicesInterface();
            var logRef = "GetAnswers_Cloud Unit Test";
            GetAnswers(services, logRef);
        }

        private void GetAnswers(IServices svc, string logRef)
        {
            var answersList = new List<TextReader>();
            string xml;

            // Ensure that invalid parameters are throwing an appropriate exception.
            try
            {
                xml = svc.GetAnswers(null, null);
                Assert.Fail(); // Should have failed instead of reaching here.
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("answers") >= 0);
            }

            // Perform all of these tests a number of times using different formats of the answers each time.
            // The .txt files are base64 encoded versions of the matching .xml files.
            for (var i = 0; i < 4; i++)
            {
                var answerFile1 = "Freddy.txt";
                ;
                var answerFile2 = "Frederick.txt";
                switch (i)
                {
                    case 0:
                        answerFile1 = "Freddy.txt";
                        answerFile2 = "Frederick.txt";
                        break;
                    case 1:
                        answerFile1 = "Freddy.txt";
                        answerFile2 = "Frederick.xml";
                        break;
                    case 2:
                        answerFile1 = "Freddy.xml";
                        answerFile2 = "Frederick.txt";
                        break;
                    case 3:
                        answerFile1 = "Freddy.xml";
                        answerFile2 = "Frederick.xml";
                        break;
                }

                // Get the Freddy answer file.
                answersList.Clear();
                answersList.Add(Util.GetTestFile(answerFile1));
                xml = svc.GetAnswers(answersList.ToArray(), logRef);
                Assert.IsTrue(xml.IndexOf("running the company") >= 0);
                Assert.IsTrue(xml.IndexOf("Freddy") >= 0);

                // Get the Frederick answer file.
                answersList.Clear();
                answersList.Add(Util.GetTestFile(answerFile2));
                xml = svc.GetAnswers(answersList.ToArray(), logRef);
                Assert.IsTrue(xml.IndexOf("Here is your new car") >= 0);
                Assert.IsTrue(xml.IndexOf("Frederick") >= 0);

                // Get the combined answer file with Frederick answers overlayed on top of Freddy answers.
                answersList.Clear();
                answersList.Add(Util.GetTestFile(answerFile1));
                answersList.Add(Util.GetTestFile(answerFile2));
                xml = svc.GetAnswers(answersList.ToArray(), logRef);
                Assert.IsTrue(xml.IndexOf("running the company") >= 0);
                Assert.IsTrue(xml.IndexOf("Here is your new car") >= 0);
                Assert.IsTrue(xml.IndexOf("Frederick") >= 0);
                Assert.IsTrue(xml.IndexOf("Freddy") < 0);

                // Get the combined answer file with Freddy answers overlayed on top of Frederick answers.
                answersList.Clear();
                answersList.Add(Util.GetTestFile(answerFile2));
                answersList.Add(Util.GetTestFile(answerFile1));
                xml = svc.GetAnswers(answersList.ToArray(), logRef);
                Assert.IsTrue(xml.IndexOf("running the company") >= 0);
                Assert.IsTrue(xml.IndexOf("Here is your new car") >= 0);
                Assert.IsTrue(xml.IndexOf("Freddy") >= 0);
                Assert.IsTrue(xml.IndexOf("Frederick") < 0);
            }
        }

        #endregion

        #region BuildSupportFiles

        [Ignore]
        [TestMethod]
        public void BuildSupportFiles_Local()
        {
            BuildSupportFiles(Util.GetLocalServicesInterface());
        }

        [Ignore]
        [TestCategory("WebServiceIntegration"), TestMethod]
        public void BuildSupportFiles_WebService()
        {
            BuildSupportFiles(Util.GetWebServiceServicesInterface());
        }

        [Ignore]
        [TestCategory("CloudIntegration"), TestMethod]
        public void BuildSupportFiles_Cloud()
        {
            BuildSupportFiles(Util.GetCloudServicesInterface());
        }

        private void BuildSupportFiles(IServices svc)
        {
            var template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");

            try
            {
                var flags = HDSupportFilesBuildFlags.BuildJavaScriptFiles;
                flags |= HDSupportFilesBuildFlags.BuildSilverlightFiles;
                flags |= HDSupportFilesBuildFlags.ForceRebuildAll;
                flags |= HDSupportFilesBuildFlags.IncludeAssembleTemplates;
                svc.BuildSupportFiles(template, flags);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            // Try building support files with a "null" template.
            try
            {
                svc.BuildSupportFiles(null, HDSupportFilesBuildFlags.BuildJavaScriptFiles);
                Assert.Fail(); // Should have thrown exception.
            }
            catch (ArgumentNullException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        #endregion

        #region RemoveSupportFiles

        [Ignore]
        [TestMethod]
        public void RemoveSupportFiles_Local()
        {
            RemoveSupportFiles(Util.GetLocalServicesInterface());
        }

        [Ignore]
        [TestCategory("WebServiceIntegration"), TestMethod]
        public void RemoveSupportFiles_WebService()
        {
            RemoveSupportFiles(Util.GetWebServiceServicesInterface());
        }

        [Ignore]
        [TestCategory("CloudIntegration"), TestMethod]
        public void RemoveSupportFiles_Cloud()
        {
            RemoveSupportFiles(Util.GetCloudServicesInterface());
        }

        private void RemoveSupportFiles(IServices svc)
        {
            var tmp = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
            try
            {
                svc.RemoveSupportFiles(tmp);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            // Try removing support files with a "null" template.
            try
            {
                svc.RemoveSupportFiles(null);
                Assert.Fail(); // Should have thrown exception.
            }
            catch (ArgumentNullException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        #endregion

        #region GetInterviewFile Tests

        [Ignore]
        [TestMethod]
        public void GetInterviewFile_Local()
        {
            GetInterviewFile(Util.GetLocalServicesInterface());
        }

        [Ignore]
        [TestCategory("WebServiceIntegration"), TestMethod]
        public void GetInterviewFile_WebService()
        {
            GetInterviewFile(Util.GetWebServiceServicesInterface());
        }

        [Ignore]
        [TestCategory("CloudIntegration"), TestMethod]
        public void GetInterviewFile_Cloud()
        {
            GetInterviewFile(Util.GetCloudServicesInterface());
        }

        private void GetInterviewFile(IServices svc)
        {
            Template template = null;
            string fileName = null;
            string fileType = null;

            for (var i = 0; i < 7; i++)
            {
                switch (i)
                {
                    case 1:
                        template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
                        break;
                    case 2:
                        fileName = "Demo Employment Agreement.docx";
                        break;
                    case 3:
                        fileName = "";
                        fileType = "js";
                        break;
                    case 4:
                        fileName = "Demo Employment Agreement.docx";
                        fileType = "js";
                        break;
                    case 5:
                        fileType = "dll";
                        break;
                    case 6:
                        fileName = "Demo Employment Agreement.docx.js";
                        fileType = "img";
                        break;
                }

                try
                {
                    using (var definitionFile = svc.GetInterviewFile(i == 0 ? null : template, fileName, fileType))
                    {
                        if (template == null || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileType))
                            Assert.Fail(); // Should have hit an exception instead of reaching this.

                        Assert.IsTrue(definitionFile.Length > 0);
                    }
                }
                catch (ArgumentNullException ex)
                {
                    if (template == null)
                        Assert.IsTrue(ex.Message.Contains("template"));
                    else if (string.IsNullOrEmpty(fileName))
                        Assert.IsTrue(ex.Message.Contains("fileName"));
                    else if (string.IsNullOrEmpty(fileType))
                        Assert.IsTrue(ex.Message.Contains("fileType"));
                    else
                        Assert.Fail();
                    // After the first three times, we don't pass any invalid parameters so this should not happen.
                }
            }
        }

        #endregion
    }
}