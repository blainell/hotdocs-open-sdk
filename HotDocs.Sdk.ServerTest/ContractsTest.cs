using System;
using System.Collections.Generic;
using System.Globalization;
using HotDocs.Sdk.Server.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.Sdk.ServerTest
{
    /// <summary>
    ///     These are unit tests for HotDocs.Sdk.Server.Contracts
    /// </summary>
    [TestClass]
    public class ContractsTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void HmacTest()
        {
            var signingKey = "MyCoolSigningKey";
            string hmac;

            // Ensure that passing a null in the HMAC parameter list will throw an exception.
            try
            {
                Assert.AreEqual("", HMAC.CalculateHMAC(signingKey, null));
                Assert.Fail(); // Should have thrown null exception.
            }
            catch (ArgumentNullException)
            {
                Assert.IsTrue(true);
            }

            // Ensure that the calculated HMAC is valid.
            hmac = HMAC.CalculateHMAC(signingKey, "");
            try
            {
                HMAC.ValidateHMAC(hmac, signingKey, "");
                Assert.AreEqual("m1TlQn+PKjvYOIsV5zJBwVqI8LM=", hmac);
            }
            catch (HMACException)
            {
                Assert.Fail();
            }

            // Ensure that ValidateHMAC throws exception with invalid HMAC
            hmac = "SomethingInvalid";
            try
            {
                HMAC.ValidateHMAC(hmac, signingKey, "");
                Assert.Fail(); // Should have thrown exception.
            }
            catch (HMACException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.Fail(); // Should not have thrown generic exception.
            }

            // string
            Assert.AreEqual("test", HMAC.Canonicalize("test"));

            // int
            Assert.AreEqual("1", HMAC.Canonicalize(1));
            Assert.AreEqual("0", HMAC.Canonicalize(0));
            Assert.AreEqual("-1", HMAC.Canonicalize(-1));

            // Enum
            Assert.AreEqual("Copy", HMAC.Canonicalize(PdfPermissions.Copy));

            // bool
            Assert.AreEqual("true", HMAC.Canonicalize(true).ToLower(CultureInfo.InvariantCulture));
            Assert.AreEqual("false", HMAC.Canonicalize(false).ToLower(CultureInfo.InvariantCulture));

            // DateTime
            var d = DateTime.Now;
            Assert.AreEqual(d.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"), HMAC.Canonicalize(d));

            // Dictionary<string, string>
            var dict = new Dictionary<string, string> {{"first", "one"}, {"second", "two"}};
            Assert.AreEqual("first=one\nsecond=two", HMAC.Canonicalize(dict));

            // Other
            var o = new object();
            Assert.AreEqual("", HMAC.Canonicalize(o));
        }

        [TestMethod]
        public void ComponentInfoTest()
        {
            var TestVariableName = "MyVarName";
            var vi = new Server.Contracts.VariableInfo();
            vi.Name = TestVariableName;


            var di = new DialogInfo();
            Assert.AreEqual(0, di.Items.Count);

            var ci = new ComponentInfo();
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