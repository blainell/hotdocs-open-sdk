using System;
using System.Collections.Generic;
using System.IO;
using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.SdkTest
{
	/// <summary>
	///This is a test class for the Template and TemplateLocation classes and is intended
	///to contain all Template-related Unit Tests
	///</summary>
	[TestClass]
	public class TemplateTest
	{
		public TemplateTest()
		{
			HotDocs.Sdk.TemplateLocation.RegisterLocation(typeof(HotDocs.Sdk.PathTemplateLocation));
			HotDocs.Sdk.TemplateLocation.RegisterLocation(typeof(HotDocs.Sdk.PackagePathTemplateLocation));
		}

		[TestMethod]
		public void TestPackagePathLocation()
		{
			string packageID = "d1f7cade-cb74-4457-a9a0-27d94f5c2d5b";
			string templateFileName = "Demo Employment Agreement.docx";
			HotDocs.Sdk.PackagePathTemplateLocation location = CreatePackagePathLocation(packageID);

			Assert.IsTrue(File.Exists(location.PackagePath));

			string switches = "/ni";
			string key = "Test file key";
			Template template = new Template(location, switches, key);

			Assert.AreEqual(templateFileName, template.FileName);
			Assert.AreEqual(key, template.Key);
			Assert.AreEqual(switches, template.Switches);

			//TODO: Update the package so that the template title and template type agree.
			Assert.AreEqual("Employment Agreement (Word RTF version)", template.GetTitle().Trim());

			string filePath = template.GetFullPath();
			Assert.IsTrue(File.Exists(filePath));
			//Directory.Delete(Path.GetDirectoryName(filePath));

			//Check the second time since the folder has been deleted.
			//filePath = template.GetFullPath();//The folder should come into existence here.
			//Assert.IsTrue(File.Exists(filePath));
			//Directory.Delete(Path.GetDirectoryName(filePath));//Clean up.

			TestTemplate(template);
		}

		[TestMethod]
		public void TestPathTemplateLocation()
		{
			string templateDir = Path.Combine(GetSamplePortalTemplateDir(), "TestTemplates");
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

		[TestMethod]
		public void TestTemplateLocationEquatability()
		{
			string templateDir = Path.Combine(GetSamplePortalTemplateDir(), "TestTemplates");
			string dir1 = Path.Combine(templateDir, "temp");
			string dir2 = Path.Combine(templateDir, "tĕmp");
			string dir3 = Path.Combine(templateDir, "tëmp");

			// ensure PathTemplateLocation is case insensitive and not diacritic insensitive
			var loc1a = new PathTemplateLocation(dir1);
			var loc1b = new PathTemplateLocation(dir1.ToUpper());
			var loc2a = new PathTemplateLocation(dir2);
			var loc2b = new PathTemplateLocation(dir2.ToUpper());
			var loc3 = new PathTemplateLocation(dir3);
			PathTemplateLocation loc4 = null;
			PathTemplateLocation loc5 = null;

			// ensure PathTemplateLocation is case insensitive
			Assert.AreEqual(loc1a, loc1b);
			Assert.AreEqual(loc1a.GetHashCode(), loc1b.GetHashCode());
			Assert.AreEqual(loc2a, loc2b);
			Assert.AreEqual(loc2a.GetHashCode(), loc2b.GetHashCode());
			// ensure PathTemplateLocation is not unicode/diacritic insensitive
			Assert.AreNotEqual(loc1a, loc2a);
			Assert.AreNotEqual(loc1a.GetHashCode(), loc2a.GetHashCode());
			Assert.AreNotEqual(loc1a, loc3);
			Assert.AreNotEqual(loc1a.GetHashCode(), loc3.GetHashCode());
			Assert.AreNotEqual(loc2b, loc3);
			Assert.AreNotEqual(loc2b.GetHashCode(), loc3.GetHashCode());
			// ensure we can tell the difference between nulls & non
			Assert.AreNotEqual(loc1a, loc4);
			Assert.AreEqual(loc4, loc5); // null template locations are equal to each other, I suppose

			// ensure hashing is working pretty much like we expect it to
			var set = new HashSet<TemplateLocation>();
			set.Add(loc1a);
			Assert.IsTrue(set.Contains(loc1a));
			Assert.IsTrue(set.Contains(loc1b));
			Assert.IsFalse(set.Contains(loc2a));
			Assert.IsFalse(set.Contains(loc2b));
			Assert.IsFalse(set.Contains(loc3));
			Assert.IsFalse(set.Contains(loc4));
			Assert.IsFalse(set.Contains(loc5));
			set.Add(loc3);
			Assert.IsTrue(set.Contains(loc1a));
			Assert.IsTrue(set.Contains(loc1b));
			Assert.IsFalse(set.Contains(loc2a));
			Assert.IsFalse(set.Contains(loc2b));
			Assert.IsTrue(set.Contains(loc3));
			Assert.IsFalse(set.Contains(loc4));
			Assert.IsFalse(set.Contains(loc5));
			set.Add(loc2b);
			Assert.IsTrue(set.Contains(loc1a));
			Assert.IsTrue(set.Contains(loc1b));
			Assert.IsTrue(set.Contains(loc2a));
			Assert.IsTrue(set.Contains(loc2b));
			Assert.IsFalse(set.Contains(loc4));
			Assert.IsFalse(set.Contains(loc5));
			set.Add(loc4);
			Assert.IsTrue(set.Contains(loc4));
			Assert.IsTrue(set.Contains(loc5));

			// now test PackagePathTemplateLocation too (& in combination)
			var pkgloc1a = new PackagePathTemplateLocation(@"2f98a9f3-d106-44a8-ba16-e01d8cd65cbd", Path.Combine(dir1, "temp.pkg"));
			var pkgloc1b = new PackagePathTemplateLocation(@"2f98a9f3-d106-44a8-ba16-e01d8cd65cbd", Path.Combine(dir1, "TEMP.pkg"));
			var pkgloc2a = new PackagePathTemplateLocation(@"2f98a9f3-d106-44a8-ba16-e01d8cd65cbD", Path.Combine(dir1, "TEMP.pkg"));
			var pkgloc3a = new PackagePathTemplateLocation(@"l'identité unique", Path.Combine(dir3, "tëmp.pkg"));
			var pkgloc3b = new PackagePathTemplateLocation(@"l'identité unique", Path.Combine(dir3, "TËMP.pkg"));
			var pkgloc4a = new PackagePathTemplateLocation(@"l'identité unique", Path.Combine(dir2, "tĕmp.pkg"));
			var pkgloc5a = new PackagePathTemplateLocation(@"l'identite unique", Path.Combine(dir2, "TĔMP.pkg"));

			// ensure package ID is case sensitive & package path is case insensitive
			Assert.AreEqual(pkgloc1a, pkgloc1b);
			Assert.AreEqual(pkgloc1a.GetHashCode(), pkgloc1b.GetHashCode());
			Assert.AreNotEqual(pkgloc1a, pkgloc2a);
			Assert.AreNotEqual(pkgloc1a.GetHashCode(), pkgloc2a.GetHashCode());
			Assert.AreNotEqual(pkgloc1b, pkgloc2a);
			Assert.AreNotEqual(pkgloc1b.GetHashCode(), pkgloc2a.GetHashCode());
			// ensure package ID & path are diacritic sensitive
			Assert.AreEqual(pkgloc3a, pkgloc3b);
			Assert.AreEqual(pkgloc3a.GetHashCode(), pkgloc3b.GetHashCode());
			Assert.AreNotEqual(pkgloc3a, pkgloc4a);
			Assert.AreNotEqual(pkgloc3a.GetHashCode(), pkgloc4a.GetHashCode());
			Assert.AreNotEqual(pkgloc3b, pkgloc5a);
			Assert.AreNotEqual(pkgloc3b.GetHashCode(), pkgloc5a.GetHashCode());

			// TODO: check comparisons/equality of different types of TemplateLocations with each other.
			// (should always be not equal, hopefully even if null?) 
		}

		#region Utility methods

		private static PackagePathTemplateLocation CreatePackagePathLocation(string packageID)
		{
			//Note that in this sample portal, the package ID is used to construct the package file name, but this does not need to be the case.
			string templateDirectory = Path.Combine(GetSamplePortalTemplateDir(), "TestTemplates");
			string packagePath = Path.Combine(templateDirectory, packageID + ".pkg");

			if (!File.Exists(packagePath))
				throw new Exception("The template does not exist.");
			return new HotDocs.Sdk.PackagePathTemplateLocation(packageID, packagePath);
		}

		private static string GetSamplePortalTemplateDir()
		{
			string samplePortalRoot = GetSamplePortalRootedPath();
			string filesDir = Path.Combine(samplePortalRoot, "Files");
			return Path.Combine(filesDir, "Templates");
		}

		private static string GetSamplePortalRootedPath()
		{
			string sRet;
			// assemblyBinariesFolder should be bin\Debug or bin\Release, within the current solution folder:
			string assemblyBinariesFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			string serverTestRoot = Directory.GetParent(Directory.GetParent(assemblyBinariesFolder).ToString()).ToString();
			string sdkRoot = Directory.GetParent(serverTestRoot).ToString();
			sRet = Path.Combine(sdkRoot, "SamplePortal");
			return sRet;
		}

		#endregion // Utility methods

	}
}
