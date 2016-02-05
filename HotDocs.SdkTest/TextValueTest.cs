/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HotDocs.SdkTest
{
    
    
    /// <summary>
    ///This is a test class for TextValueTest and is intended
    ///to contain all TextValueTest Unit Tests
    ///</summary>
	[TestClass()]
	public class TextValueTest
	{


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

	
		[TestMethod()]
		public void TextValueStaticTest1()
		{
			Assert.IsFalse(TextValue.Unanswered.IsAnswered);
			Assert.AreEqual(TextValue.Unanswered.Type, HotDocs.Sdk.ValueType.Text);
			Assert.IsTrue(TextValue.Unanswered.UserModifiable);
			Assert.IsNull(TextValue.Unanswered.Value);
			Assert.IsInstanceOfType(TextValue.Unanswered, typeof(TextValue));
		}

		[TestMethod()]
		public void TextValueStaticTest2()
		{
			Assert.IsFalse(TextValue.UnansweredLocked.IsAnswered);
			Assert.AreEqual(TextValue.UnansweredLocked.Type, HotDocs.Sdk.ValueType.Text);
			Assert.IsFalse(TextValue.UnansweredLocked.UserModifiable);
			Assert.IsNull(TextValue.UnansweredLocked.Value);
			Assert.IsInstanceOfType(TextValue.UnansweredLocked, typeof(TextValue));
		}

		/// <summary>
		///A test for TextValue Constructor
		///</summary>
		[TestMethod()]
		public void TextValueConstructorTest()
		{
			TextValue target = new TextValue("Test\nValue");
			Assert.IsTrue(target.IsAnswered);
			Assert.AreEqual(target.Type, HotDocs.Sdk.ValueType.Text);
			Assert.IsTrue(target.UserModifiable);
			Assert.AreEqual(target.Value, "Test\r\nValue");
		}

		/// <summary>
		///A test for TextValue Constructor
		///</summary>
		[TestMethod()]
		public void TextValueConstructorTest1()
		{
			TextValue target = new TextValue("Test\nValue", true);
			Assert.IsTrue(target.IsAnswered);
			Assert.AreEqual(target.Type, HotDocs.Sdk.ValueType.Text);
			Assert.IsTrue(target.UserModifiable);
			Assert.AreEqual(target.Value, "Test\r\nValue");
		}

		/// <summary>
		///A test for TextValue Constructor
		///</summary>
		[TestMethod()]
		public void TextValueConstructorTest2()
		{
			TextValue target = new TextValue("Test\nValue", false);
			Assert.IsTrue(target.IsAnswered);
			Assert.AreEqual(target.Type, HotDocs.Sdk.ValueType.Text);
			Assert.IsFalse(target.UserModifiable);
			Assert.AreEqual(target.Value, "Test\r\nValue");
		}

		/// <summary>
		///A test for Equals
		///</summary>
		[TestMethod()]
		public void EqualsTest()
		{
			TextValue target = new TextValue("Test");
			object obj = "Test"; // string
			bool expected = false; // we expect them not to be equal because they are not the same type.
			// Implicit conversion from string to TextValue does not take effect here.
			// However, even if it did, that might be fine too... this test is just asserting the expected (but not required) behavior.
			bool actual;
			actual = target.Equals(obj);
			Assert.AreEqual(expected, actual);

			obj = new TextValue("Test");
			expected = true;
			actual = target.Equals(obj);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Equals
		///</summary>
		[TestMethod()]
		public void EqualsTest1()
		{
			TextValue target = new TextValue("my value");
			TextValue operand = new TextValue("my value");
			bool expected = true;
			bool actual;
			actual = target.Equals(operand);
			Assert.AreEqual(expected, actual);

			operand = new TextValue("my other value");
			expected = false;
			actual = target.Equals(operand);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for NormalizeLineBreaks
		///</summary>
		[TestMethod()]
		public void NormalizeLineBreaksTest()
		{
			string text = "\r\nline 1\nline 2\rline 3\r\nline 4\n\rline 5\r";
			string expected = "\r\nline 1\r\nline 2\r\nline 3\r\nline 4\r\n\r\nline 5\r\n";
			string actual;
			actual = TextValue.NormalizeLineBreaks(text);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void IsAnswered()
		{
			TextValue v = new TextValue();
			Assert.IsFalse(v.IsAnswered);
		}

		[TestMethod]
		public void Equals()
		{
			TextValue v1 = new TextValue("Hello World");
			TextValue v2 = new TextValue("hELLO wORLD");

			Assert.IsTrue(v1.Equals(v2));
			Assert.IsFalse(v1.Equals(new TextValue("Hello Worl")));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals1()
		{
			TextValue v = new TextValue("Hello World");
			TextValue uv = new TextValue();

			uv.Equals(v);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals2()
		{
			TextValue v = new TextValue("Hello World");
			TextValue uv = new TextValue();

			v.Equals(uv);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals3()
		{
			TextValue uv1 = new TextValue();
			TextValue uv2 = new TextValue();

			uv1.Equals(uv2);
		}

		[TestMethod]
		public void Casts()
		{
			TextValue v = TextValue.Unanswered;
			Assert.IsFalse(v.IsAnswered);

			v = "Hello World";
			Assert.IsTrue(v.IsAnswered);
			Assert.AreEqual("Hello World", v.Value);

			v = new MultipleChoiceValue("One|Two|Three");
			Assert.IsTrue(v.IsAnswered);
			Assert.AreEqual("One|Two|Three", v.Value);

			v = new MultipleChoiceValue();
			Assert.IsFalse(v.IsAnswered);
		}

	}
}
