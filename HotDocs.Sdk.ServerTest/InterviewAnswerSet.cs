using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace HotDocs.Sdk.ServerTest
{
	/// <summary>
	/// These are unit tests for HotDocs.Sdk.Server.InterviewAnswerSet
	/// </summary>
	[TestClass]
	public class InterviewAnswerSet
	{
		public InterviewAnswerSet() { }

		[TestMethod]
		public void InterviewAnswerSet_Constructor()
		{
			HotDocs.Sdk.Server.InterviewAnswerSet ans = new Server.InterviewAnswerSet();

			Assert.IsNotNull(ans);
			Assert.IsTrue(ans is HotDocs.Sdk.AnswerCollection);
		}

		[TestMethod]
		public void InterviewAnswerSet_Clear()
		{
			HotDocs.Sdk.Server.InterviewAnswerSet ans = new Server.InterviewAnswerSet();
			ans.DecodeInterviewAnswers("[HDSANS(372,396)]77u/PD94bWwgdmVyc2lvbj0iMS4wIiBzdGFuZGFsb25lPSJ5ZXMiPz4NCjxBbnN3ZXJTZXQgdGl0bGU9IiIgdmVyc2lvbj0iMS4xIiB1c2VNYW5nbGVkTmFtZXM9ImZhbHNlIj4NCgk8QW5zd2VyIG5hbWU9IkVtcGxveWVlIEZpcnN0IE5hbWUiPg0KCQk8VGV4dFZhbHVlPmpvaG48L1RleHRWYWx1ZT4NCgk8L0Fuc3dlcj4NCgk8QW5zd2VyIG5hbWU9IkVtcGxveWVlIExhc3QgTmFtZSI+DQoJCTxUZXh0VmFsdWU+YnJvd2JuPC9UZXh0VmFsdWU+DQoJPC9BbnN3ZXI+DQo8L0Fuc3dlclNldD4=|UEsDBBQAAQAIAKuAT0OhnXgspQAAAA8BAAARAAAASERPcmlnaW5hbFhtbC5hbnj5BGng8QSDJLDP0lJehO77IaYdKOXEDSUQEe+BFG7G84s2wI3vNJBMt8vEdmczTlvVMvjVGDU1tOhSe9hw97s3PcbXfbJ81QStgzzbJdweoGaCVoD1bfVr8+kd6RSISQ6o7NFEEK4tRLzo2oJc+uOiokPuo8xm/mMJoULIOJTQBlQgtPWV1OZOpycGuIQcAL0u/coKsZex8XCGM813phsxU7ckvQ1QSwECLQAUAAEACACrgE9DoZ14LKUAAAAPAQAAEQAAAAAAAAAAAIAAAAAAAAAASERPcmlnaW5hbFhtbC5hbnhQSwUGAAAAAAEAAQA/AAAA1AAAAAAA");

			Assert.AreEqual(2, ans.Count<HotDocs.Sdk.Answer>());
			ans.Clear();
			Assert.AreEqual(0, ans.Count<HotDocs.Sdk.Answer>());
		}

		[TestMethod]
		public void InterviewAnswerSet_DecodeInterviewAnswers()
		{
			HotDocs.Sdk.Server.InterviewAnswerSet ans = new Server.InterviewAnswerSet();
			ans.Clear();
			string answerStr = "[HDSANS(372,396)]77u/PD94bWwgdmVyc2lvbj0iMS4wIiBzdGFuZGFsb25lPSJ5ZXMiPz4NCjxBbnN3ZXJTZXQgdGl0bGU9IiIgdmVyc2lvbj0iMS4xIiB1c2VNYW5nbGVkTmFtZXM9ImZhbHNlIj4NCgk8QW5zd2VyIG5hbWU9IkVtcGxveWVlIEZpcnN0IE5hbWUiPg0KCQk8VGV4dFZhbHVlPmpvaG48L1RleHRWYWx1ZT4NCgk8L0Fuc3dlcj4NCgk8QW5zd2VyIG5hbWU9IkVtcGxveWVlIExhc3QgTmFtZSI+DQoJCTxUZXh0VmFsdWU+YnJvd2JuPC9UZXh0VmFsdWU+DQoJPC9BbnN3ZXI+DQo8L0Fuc3dlclNldD4=|UEsDBBQAAQAIAKuAT0OhnXgspQAAAA8BAAARAAAASERPcmlnaW5hbFhtbC5hbnj5BGng8QSDJLDP0lJehO77IaYdKOXEDSUQEe+BFG7G84s2wI3vNJBMt8vEdmczTlvVMvjVGDU1tOhSe9hw97s3PcbXfbJ81QStgzzbJdweoGaCVoD1bfVr8+kd6RSISQ6o7NFEEK4tRLzo2oJc+uOiokPuo8xm/mMJoULIOJTQBlQgtPWV1OZOpycGuIQcAL0u/coKsZex8XCGM813phsxU7ckvQ1QSwECLQAUAAEACACrgE9DoZ14LKUAAAAPAQAAEQAAAAAAAAAAAIAAAAAAAAAASERPcmlnaW5hbFhtbC5hbnhQSwUGAAAAAAEAAQA/AAAA1AAAAAAA";
			Assert.AreEqual(0, ans.Count<HotDocs.Sdk.Answer>());

			ans.DecodeInterviewAnswers(answerStr);

			Assert.AreEqual(2, ans.Count<HotDocs.Sdk.Answer>());

			using (Stream s = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(answerStr)))
			{
				ans.DecodeInterviewAnswers(s);
				Assert.AreEqual(2, ans.Count<HotDocs.Sdk.Answer>());
				string encodedAns = ans.EncodeInterviewAnswers();
				Assert.IsFalse(string.IsNullOrEmpty(encodedAns));
			}

			using (TextReader tr = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""))))
			{
				TextReader tr2 = HotDocs.Sdk.Server.InterviewAnswerSet.GetDecodedInterviewAnswers(tr);
				string ansStr = tr2.ReadToEnd();
				Assert.IsTrue(string.IsNullOrEmpty(ansStr));
				ans.DecodeInterviewAnswers(ansStr);
				Assert.IsFalse(string.IsNullOrEmpty(ans.EncodeInterviewAnswers()));
			}
		}

		[TestMethod]
		public void InterviewAnswerSet_EncodeInterviewAnswers()
		{
			HotDocs.Sdk.Server.InterviewAnswerSet ans = new Server.InterviewAnswerSet();
			ans.Clear();
			string answerStr = "[HDSANS(372,396)]77u/PD94bWwgdmVyc2lvbj0iMS4wIiBzdGFuZGFsb25lPSJ5ZXMiPz4NCjxBbnN3ZXJTZXQgdGl0bGU9IiIgdmVyc2lvbj0iMS4xIiB1c2VNYW5nbGVkTmFtZXM9ImZhbHNlIj4NCgk8QW5zd2VyIG5hbWU9IkVtcGxveWVlIEZpcnN0IE5hbWUiPg0KCQk8VGV4dFZhbHVlPmpvaG48L1RleHRWYWx1ZT4NCgk8L0Fuc3dlcj4NCgk8QW5zd2VyIG5hbWU9IkVtcGxveWVlIExhc3QgTmFtZSI+DQoJCTxUZXh0VmFsdWU+YnJvd2JuPC9UZXh0VmFsdWU+DQoJPC9BbnN3ZXI+DQo8L0Fuc3dlclNldD4=|UEsDBBQAAQAIAKuAT0OhnXgspQAAAA8BAAARAAAASERPcmlnaW5hbFhtbC5hbnj5BGng8QSDJLDP0lJehO77IaYdKOXEDSUQEe+BFG7G84s2wI3vNJBMt8vEdmczTlvVMvjVGDU1tOhSe9hw97s3PcbXfbJ81QStgzzbJdweoGaCVoD1bfVr8+kd6RSISQ6o7NFEEK4tRLzo2oJc+uOiokPuo8xm/mMJoULIOJTQBlQgtPWV1OZOpycGuIQcAL0u/coKsZex8XCGM813phsxU7ckvQ1QSwECLQAUAAEACACrgE9DoZ14LKUAAAAPAQAAEQAAAAAAAAAAAIAAAAAAAAAASERPcmlnaW5hbFhtbC5hbnhQSwUGAAAAAAEAAQA/AAAA1AAAAAAA";
			Assert.AreEqual(0, ans.Count<HotDocs.Sdk.Answer>());

			ans.DecodeInterviewAnswers(answerStr);

			Assert.AreEqual(2, ans.Count<HotDocs.Sdk.Answer>());

			using (Stream s = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(answerStr)))
			{
				ans.DecodeInterviewAnswers(s);
				Assert.AreEqual(2, ans.Count<HotDocs.Sdk.Answer>());
				string encodedAns = ans.EncodeInterviewAnswers();
				Assert.IsFalse(string.IsNullOrEmpty(encodedAns));

				MemoryStream ms = new MemoryStream();
				ans.EncodeInterviewAnswers(ms);
			}
		}

		[TestMethod]
		public void InterviewAnswerSet_GetDecodedInterviewAnswers()
		{
			HotDocs.Sdk.Server.InterviewAnswerSet ans = new Server.InterviewAnswerSet();

			using (TextReader tr = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""))))
			{
				TextReader tr2 = HotDocs.Sdk.Server.InterviewAnswerSet.GetDecodedInterviewAnswers(tr);
				string ansStr = tr2.ReadToEnd();
				Assert.IsTrue(string.IsNullOrEmpty(ansStr));
				ans.DecodeInterviewAnswers(ansStr);
				Assert.IsFalse(string.IsNullOrEmpty(ans.EncodeInterviewAnswers()));

				TextWriter tw = new StringWriter();
				ans.EncodeInterviewAnswers(tw);

			}

			using (TextReader tr = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("[badanswerfile"))))
			{
				try
				{
					TextReader tr2 = HotDocs.Sdk.Server.InterviewAnswerSet.GetDecodedInterviewAnswers(tr);
					Assert.Fail(); // This should have thrown an exception since the answers are bad.
				}
				catch (ArgumentException ex)
				{
					Assert.IsTrue(ex.Message.Contains("Error parsing interview answers."));
				}


			}

			using (TextReader tr = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("<badanswerfile"))))
			{
				try
				{
					TextReader tr2 = HotDocs.Sdk.Server.InterviewAnswerSet.GetDecodedInterviewAnswers(tr);
					//Assert.Fail(); // This should have thrown an exception since the answers are bad.
				}
				catch (ArgumentException ex)
				{
					Assert.IsTrue(ex.Message.Contains("Error parsing interview answers."));
				}
			}

			using (TextReader tr = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("77u/PD94bWwgdmVyc2lvbj0iMS4wIiBzdGFuZGFsb25lPSJ5ZXMiPz4NCjxBbnN3ZXJTZXQgdGl0bGU9IiIgdmVyc2lvbj0iMS4xIiB1c2VNYW5nbGVkTmFtZXM9ImZhbHNlIj4NCgk8QW5zd2VyIG5hbWU9IkVtcGxveWVlIEZpcnN0IE5hbWUiPg0KCQk8VGV4dFZhbHVlPmpvaG48L1RleHRWYWx1ZT4NCgk8L0Fuc3dlcj4NCgk8QW5zd2VyIG5hbWU9IkVtcGxveWVlIExhc3QgTmFtZSI+DQoJCTxUZXh0VmFsdWU+YnJvd2JuPC9UZXh0VmFsdWU+DQoJPC9BbnN3ZXI+DQo8L0Fuc3dlclNldD4="))))
			{
				try
				{
					TextReader tr2 = HotDocs.Sdk.Server.InterviewAnswerSet.GetDecodedInterviewAnswers(tr);
					//Assert.Fail(); // This should have thrown an exception since the answers are bad.
				}
				catch (ArgumentException ex)
				{
					Assert.IsTrue(ex.Message.Contains("Error parsing interview answers."));
				}
			}

			using (TextReader tr = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes("\xfeff"))))))
			{
				try
				{
					TextReader tr2 = HotDocs.Sdk.Server.InterviewAnswerSet.GetDecodedInterviewAnswers(tr);
					//Assert.Fail(); // This should have thrown an exception since the answers are bad.
				}
				catch (ArgumentException ex)
				{
					Assert.IsTrue(ex.Message.Contains("Error parsing interview answers."));
				}
			}
		}


	}
}
