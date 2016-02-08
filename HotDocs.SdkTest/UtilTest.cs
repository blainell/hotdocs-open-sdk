using System;
using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.SdkTest
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void GetMimeType()
        {
            Assert.AreEqual("image/bmp", Util.GetMimeType(".bmp", true));
            Assert.AreEqual("image/bmp", Util.GetMimeType(".bmp", false));

            Assert.AreEqual("application/octet-stream", Util.GetMimeType(".other"));
            Assert.AreEqual("application/octet-stream", Util.GetMimeType(".other", false));

            Assert.AreEqual("image/jpeg", Util.GetMimeType(".jpg"));
            Assert.AreEqual("image/gif", Util.GetMimeType(".gif"));
            Assert.AreEqual("image/tiff", Util.GetMimeType(".tif"));

            Assert.AreEqual("text/html", Util.GetMimeType(".htm"));
            Assert.AreEqual("text/html", Util.GetMimeType(".html"));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException), "")]
        public void GetMimeType_ArgumentNullException()
        {
            // If the file name has no extension (as is the case when it does not contain a period), an exception is thrown.
            Util.GetMimeType("bmp", true);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException), "")]
        public void GetMimeType_ArgumentException()
        {
            // If we say that we only want an image mime type and the file name is something else, an exception is thrown.
            Util.GetMimeType(".cmp", true);
        }
    }
}