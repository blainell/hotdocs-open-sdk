/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Cloud;
using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk.ServerTest
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class UnitTest1
	{
		public UnitTest1()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		#region GetInterview Tests

		[TestMethod]
		public void GetInterview_Local()
		{
			IServices services = Util.GetLocalServicesInterface();
			Template template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
			string logRef = "GetInterview_Local Unit Test";

			GetInterview(services, template, logRef);
		}

		[TestMethod]
		public void GetInterview_WebService()
		{
			IServices services = Util.GetWebServiceServicesInterface();
			Template template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
			string logRef = "GetInterview_WebService Unit Test";

			GetInterview(services, template, logRef);
		}

		[TestMethod]
		public void GetInterview_Cloud()
		{
			IServices services = Util.GetCloudServicesInterface();
			Template template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
			string logRef = "GetInterview_Cloud Unit Test";

			GetInterview(services, template, logRef);
		}

		private void GetInterview(IServices svc, Template tmp, string logRef)
		{
			// Set up the InterviewOptions for the test.
			string postInterviewUrl = "PostInterview.aspx";
			string styleSheetUrl = "HDServerFiles/Stylesheets";
			string runtimeUrl = "HDServerFiles/js";
			string interviewDefUrl = "GetInterviewDef.ashx";
			string interviewImgUrl = "GetImage.ashx";
			InterviewSettings opts = new InterviewSettings(postInterviewUrl, runtimeUrl, styleSheetUrl, interviewDefUrl, interviewImgUrl);

			// Set up the Marked Variables for the test.
			string[] markedVars = new string[] { };

			InterviewResult result = svc.GetInterview(tmp, null, opts, markedVars, logRef);
			Assert.AreNotEqual(result.HtmlFragment, "");
			Assert.IsTrue(result.HtmlFragment.Contains(opts.PostInterviewUrl));
			Assert.IsTrue(result.HtmlFragment.Contains(runtimeUrl));
			Assert.IsTrue(result.HtmlFragment.Contains(styleSheetUrl));
			Assert.IsTrue(result.HtmlFragment.Contains(interviewDefUrl));
			Assert.IsTrue(result.HtmlFragment.Contains(interviewImgUrl));

			// Now get another interview, but this time specify a url for doc preview and save answers.
			opts.DocumentPreviewUrl = "DocPreview.aspx";
			opts.SaveAnswersUrl = "SaveAnswers.aspx";
			result = svc.GetInterview(tmp, null, opts, markedVars, logRef);
			Assert.IsTrue(result.HtmlFragment.Contains(opts.DocumentPreviewUrl));
			Assert.IsTrue(result.HtmlFragment.Contains(opts.SaveAnswersUrl));

			// Now get another interview, but this time disable the doc preview and save answers urls.
			opts.DisableDocumentPreview = Tristate.True;
			opts.DisableSaveAnswers = Tristate.True;
			result = svc.GetInterview(tmp, null, opts, markedVars, logRef);
			Assert.IsFalse(result.HtmlFragment.Contains(opts.DocumentPreviewUrl));
			Assert.IsFalse(result.HtmlFragment.Contains(opts.SaveAnswersUrl), "No Save Ans Url because it is disabled");
		}

		#endregion

		#region AssembleDocument Tests
		[TestMethod]
		public void AssembleDocument_Local()
		{
			IServices services = Util.GetLocalServicesInterface();
			Template template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
			string logRef = "AssembleDocument_Local Unit Test";

			AssembleDocument(services, template, logRef);
		}

		[TestMethod]
		public void AssembleDocument_WebService()
		{
			IServices services = Util.GetWebServiceServicesInterface();
			Template template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
			string logRef = "AssembleDocument_WebService Unit Test";

			AssembleDocument(services, template, logRef);
		}

		[TestMethod]
		public void AssembleDocument_Cloud()
		{
			IServices services = Util.GetCloudServicesInterface();
			Template template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
			string logRef = "AssembleDocument_Cloud Unit Test";

			AssembleDocument(services, template, logRef);
		}

		private void AssembleDocument(IServices svc, Template tmp, string logRef)
		{
			TextReader answers = new StringReader("");
			AssembleDocumentSettings settings = new AssembleDocumentSettings();
			AssembleDocumentResult result;

			result = svc.AssembleDocument(tmp, answers, settings, logRef);
			Assert.AreEqual(result.PendingAssemblies.Length, 0);
			Assert.AreEqual(0, result.Document.SupportingFiles.Length);

			settings.Format = DocumentType.MHTML;
			result = svc.AssembleDocument(tmp, answers, settings, logRef);
			Assert.AreEqual(0, result.Document.SupportingFiles.Length); // The MHTML is a single file (no external images).

			settings.Format = DocumentType.HTMLwDataURIs;
			result = svc.AssembleDocument(tmp, answers, settings, logRef);
			Assert.AreEqual(0, result.Document.SupportingFiles.Length); // The HTML with Data URIs is a single file (no external images).

			settings.Format = DocumentType.HTML;
			result = svc.AssembleDocument(tmp, answers, settings, logRef);
			Assert.AreEqual(1, result.Document.SupportingFiles.Length); // The HTML contains one external image file.
		}

		#endregion

		#region GetComponentInfo Tests

		[TestMethod]
		public void GetComponentInfo_Local()
		{
			IServices services = Util.GetLocalServicesInterface();
			Template template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
			string logRef = "GetComponentInfo_Local Unit Test";

			GetComponentInfo(services, template, logRef);
		}

		[TestMethod]
		public void GetComponentInfo_WebService()
		{
			IServices services = Util.GetWebServiceServicesInterface();
			Template template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
			string logRef = "GetComponentInfo_WebService Unit Test";

			GetComponentInfo(services, template, logRef);
		}

		[TestMethod]
		public void GetComponentInfo_Cloud()
		{
			IServices services = Util.GetCloudServicesInterface();
			Template template = Util.OpenTemplate("d1f7cade-cb74-4457-a9a0-27d94f5c2d5b");
			string logRef = "GetComponentInfo_Local Unit Test";

			GetComponentInfo(services, template, logRef);
		}

		private void GetComponentInfo(IServices svc, Template tmp, string logRef)
		{
			Server.Contracts.ComponentInfo result = svc.GetComponentInfo(tmp, false, logRef);
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
			IServices services = Util.GetLocalServicesInterface();
			string logRef = "GetAnswers_Local Unit Test";
			GetAnswers(services, logRef);
		}

		[TestMethod]
		public void GetAnswers_WebService()
		{
			IServices services = Util.GetWebServiceServicesInterface();
			string logRef = "GetAnswers_WebService Unit Test";
			GetAnswers(services, logRef);
		}

		[TestMethod]
		public void GetAnswers_Cloud()
		{
			IServices services = Util.GetCloudServicesInterface();
			string logRef = "GetAnswers_Cloud Unit Test";
			GetAnswers(services, logRef);
		}

		private void GetAnswers(IServices svc, string logRef)
		{
			List<TextReader> answersList = new List<TextReader>();
			string testPath = Util.TestFilesPath;

			string filePath = Path.Combine(testPath, "HDInfo_DemoEmpl_Freddy.txt");
			answersList.Add(new StringReader(Util.GetFileContentAsString(filePath)));
			filePath = Path.Combine(testPath, "HDInfo_EmployeeRecognition_Frederick.txt");
			answersList.Add(new StringReader(Util.GetFileContentAsString(filePath)));

			string xml = svc.GetAnswers(answersList.ToArray(), logRef);
			Assert.IsTrue(xml.Length > 0);
		}

		#endregion

		#region Template and TemplateLocation Tests
		[TestMethod]
		public void TestPackagePathLocation()
		{
			string packageID = "d1f7cade-cb74-4457-a9a0-27d94f5c2d5b";
			string templateFileName = "Demo Employment Agreement.docx";
			HotDocs.Sdk.PackagePathTemplateLocation location = Util.CreatePackagePathLocation(packageID);

			Assert.IsTrue(File.Exists(location.PackagePath));

			string switches = "/ni";
			string key = "Test file key";
			Template template = new Template(location, switches, key);

			Assert.AreEqual(templateFileName, template.FileName);
			Assert.AreEqual(key, template.Key);
			Assert.AreEqual(switches, template.Key);

			//TODO: Update the package so that the template title and template type agree.
			Assert.AreEqual(template.GetTitle(), "Employment Agreement (Word RTF version)");

			string filePath = template.GetFullPath();
			Assert.IsTrue(File.Exists(filePath));
			Directory.Delete(Path.GetDirectoryName(filePath));

			//Check the second time since the folder has been deleted.
			filePath = template.GetFullPath();//The folder should come into existence here.
			Assert.IsTrue(File.Exists(filePath));
			Directory.Delete(Path.GetDirectoryName(filePath));//Clean up.

			TestTemplate(template);
		}

		[TestMethod]
		public void TestPathTemplateLocation()
		{
			string templateDir = Path.Combine(Util.TestFilesPath, "DocxDemoEmployment");
			Assert.IsTrue(Directory.Exists(templateDir));
			PathTemplateLocation location = new PathTemplateLocation(templateDir);

			string switches = "/ni";
			string key = "Test file key";
			Template template = new Template("Demo Employment Agreement.docx", location, switches, key);
			Assert.AreEqual(template.GetTitle(), "Employment Agreement");

			string filePath = template.GetFullPath();
			Assert.IsTrue(File.Exists(filePath));

			TestTemplate(template);
		}

		private void TestTemplate(Template template)
		{
			Assert.AreEqual(template.TemplateType, TemplateType.WordDOCX);
			Assert.IsFalse(template.HasInterview);
			Assert.IsTrue(template.GeneratesDocument);
			Assert.AreEqual(template.NativeDocumentType, DocumentType.WordDOCX);
			Assert.AreEqual(template.TemplateType, TemplateType.WordDOCX);

			string locator = template.CreateLocator();
			Template template2 = Template.Locate(locator);

			Assert.AreEqual(template.FileName, template2.FileName);
			Assert.AreEqual(template.Key, template2.Key);
			Assert.AreEqual(template.Switches, template2.Switches);
			Assert.AreEqual(template.GetTitle(), template2.GetTitle());
			Assert.AreEqual(template.GeneratesDocument, template2.GeneratesDocument);
			Assert.AreEqual(template.HasInterview, template2.HasInterview);
			Assert.AreEqual(template.NativeDocumentType, template2.NativeDocumentType);
			Assert.AreEqual(template.TemplateType, template2.TemplateType);

			template.UpdateFileName();
			Assert.AreEqual(template.FileName, template2.FileName);
		}

		#endregion
	}
}
