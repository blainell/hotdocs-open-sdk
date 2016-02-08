using System.Collections.Specialized;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.Sdk.ServerTest
{
    /// <summary>
    ///     These are unit tests for HotDocs.Sdk.Server.InterviewResponse
    /// </summary>
    [TestClass]
    public class InterviewResponse
    {
        private readonly NameValueCollection form;

        public InterviewResponse()
        {
            form = new NameValueCollection();
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

            var s = Sdk.InterviewResponse.GetAnswerStream(form);
            using (var reader = new StreamReader(s, Encoding.UTF8))
            {
                Assert.AreEqual("threefour", reader.ReadToEnd());
            }
        }
    }
}