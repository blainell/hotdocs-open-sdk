using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk.ServerTest
{
	/// <summary>
	/// These are unit tests for HotDocs.Sdk.Server.Contracts
	/// </summary>
	[TestClass]
	public class ContractsTest
	{
		public ContractsTest()
		{
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

		[TestMethod]
		public void HmacTest()
		{
			string signingKey = "MyCoolSigningKey";
			string hmac;

			// Ensure that passing a null in the HMAC parameter list will throw an exception.
			try
			{
				Assert.AreEqual("", HotDocs.Sdk.Server.Contracts.HMAC.CalculateHMAC(signingKey, null));
				Assert.Fail(); // Should have thrown null exception.
			}
			catch (ArgumentNullException)
			{
				Assert.IsTrue(true);
			}

			// Ensure that the calculated HMAC is valid.
			hmac = HotDocs.Sdk.Server.Contracts.HMAC.CalculateHMAC(signingKey, "");
			try
			{
				HotDocs.Sdk.Server.Contracts.HMAC.ValidateHMAC(hmac, signingKey, "");
				Assert.AreEqual("m1TlQn+PKjvYOIsV5zJBwVqI8LM=", hmac);
			}
			catch (HotDocs.Sdk.Server.Contracts.HMACException)
			{
				Assert.Fail();
			}

			// Ensure that ValidateHMAC throws exception with invalid HMAC
			hmac = "SomethingInvalid";
			try
			{
				HotDocs.Sdk.Server.Contracts.HMAC.ValidateHMAC(hmac, signingKey, "");
				Assert.Fail(); // Should have thrown exception.
			}
			catch (HotDocs.Sdk.Server.Contracts.HMACException)
			{
				Assert.IsTrue(true);
			}
			catch (Exception)
			{
				Assert.Fail(); // Should not have thrown generic exception.
			}

			// string
			Assert.AreEqual("test", HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize("test"));

			// int
			Assert.AreEqual("1", HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize(1));
			Assert.AreEqual("0", HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize(0));
			Assert.AreEqual("-1", HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize(-1));

			// Enum
			Assert.AreEqual("Copy", HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize(PdfPermissions.Copy));

			// bool
			Assert.AreEqual("true", HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize(true).ToLower(CultureInfo.InvariantCulture));
			Assert.AreEqual("false", HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize(false).ToLower(CultureInfo.InvariantCulture));

			// DateTime
			DateTime d = DateTime.Now;
			Assert.AreEqual(d.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"), HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize(d));

			// Dictionary<string, string>
			Dictionary<string, string> dict = new Dictionary<string, string>() { { "first", "one" }, { "second", "two" } };
			Assert.AreEqual("first=one\nsecond=two", HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize(dict));

			// Other
			object o = new object();
			Assert.AreEqual("", HotDocs.Sdk.Server.Contracts.HMAC.Canonicalize(o));



		}

		[TestMethod]
		public void ComponentInfoTest()
		{
			string TestVariableName = "MyVarName";
			HotDocs.Sdk.Server.Contracts.DialogItemInfo dii = new Server.Contracts.DialogItemInfo();
			HotDocs.Sdk.Server.Contracts.VariableInfo vi = new Server.Contracts.VariableInfo();
			vi.Name = TestVariableName;

			HotDocs.Sdk.Server.Contracts.TemplateInfo ti = new Server.Contracts.TemplateInfo();

			HotDocs.Sdk.Server.Contracts.DialogInfo di = new Server.Contracts.DialogInfo();
			Assert.AreEqual(0, di.Items.Count);

			HotDocs.Sdk.Server.Contracts.ComponentInfo ci = new Server.Contracts.ComponentInfo();
			Assert.IsFalse(ci.IsDefinedVariable(TestVariableName));
			ci.AddDialog(di);
			Assert.AreEqual(1, ci.Dialogs.Count);

			ci.AddVariable(vi);
			Assert.AreEqual(1, ci.Variables.Count);
			Assert.AreEqual(TestVariableName, ci.Variables[0].Name);
			Assert.IsTrue(ci.IsDefinedVariable(TestVariableName));
		}
	}
}
