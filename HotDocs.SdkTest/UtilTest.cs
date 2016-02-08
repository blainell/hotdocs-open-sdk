using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.SdkTest
{
	[TestClass]
	public class UtilTest
	{
		[TestMethod]
		public void GetMimeType()
		{
			Assert.AreEqual("image/bmp", HotDocs.Sdk.Util.GetMimeType(".bmp", true));
			Assert.AreEqual("image/bmp", HotDocs.Sdk.Util.GetMimeType(".bmp", false));

			Assert.AreEqual("application/octet-stream", HotDocs.Sdk.Util.GetMimeType(".other"));
			Assert.AreEqual("application/octet-stream", HotDocs.Sdk.Util.GetMimeType(".other", false));

			Assert.AreEqual("image/jpeg", HotDocs.Sdk.Util.GetMimeType(".jpg"));
			Assert.AreEqual("image/gif", HotDocs.Sdk.Util.GetMimeType(".gif"));
			Assert.AreEqual("image/tiff", HotDocs.Sdk.Util.GetMimeType(".tif"));

			Assert.AreEqual("text/html", HotDocs.Sdk.Util.GetMimeType(".htm"));
			Assert.AreEqual("text/html", HotDocs.Sdk.Util.GetMimeType(".html"));
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException), "")]
		public void GetMimeType_ArgumentNullException()
		{
			// If the file name has no extension (as is the case when it does not contain a period), an exception is thrown.
			HotDocs.Sdk.Util.GetMimeType("bmp", true);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException), "")]
		public void GetMimeType_ArgumentException()
		{
			// If we say that we only want an image mime type and the file name is something else, an exception is thrown.
			HotDocs.Sdk.Util.GetMimeType(".cmp", true);
		}
	}
}
