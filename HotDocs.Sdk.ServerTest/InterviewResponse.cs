using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace HotDocs.Sdk.ServerTest
{
	/// <summary>
	/// These are unit tests for HotDocs.Sdk.Server.InterviewResponse
	/// </summary>
	[TestClass]
	public class InterviewResponse
	{
		private readonly System.Collections.Specialized.NameValueCollection form;

		public InterviewResponse()
		{
			form = new System.Collections.Specialized.NameValueCollection();
		}

		[TestMethod]
		public void InterviewResponse_GetAnswers()
		{
			form.Clear();

			Assert.AreEqual("", Sdk.InterviewResponse.GetAnswers(form));

			form.Add("HDInfo", "one");
			form.Add("HDInfo", "two");
			Assert.AreEqual("onetwo", Sdk.InterviewResponse.GetAnswers(form));
		}

		[TestMethod]
		public void InterviewResponse_GetAnswerStream()
		{
			form.Clear();

			form.Add("HDInfo", "three");
			form.Add("HDInfo", "four");

			Stream s = Sdk.InterviewResponse.GetAnswerStream(form);
			using (StreamReader reader = new StreamReader(s, Encoding.UTF8))
			{
				Assert.AreEqual("threefour", reader.ReadToEnd());
			}
		}
	}
}
