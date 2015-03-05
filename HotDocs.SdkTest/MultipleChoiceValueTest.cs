/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HotDocs.SdkTest
{
	[TestClass]
	public class MultipleChoiceValueTest
	{
		[TestMethod]
		public void IsAnswered()
		{
			MultipleChoiceValue v = new MultipleChoiceValue();
			Assert.IsFalse(v.IsAnswered);
		}

		[TestMethod]
		public void Constructors()
		{
			string[] options = { "One", "Two", "Three" };
			List<string> optionsList = new List<string>(options);

			string s = null;
			MultipleChoiceValue v = new MultipleChoiceValue(s);
			Assert.IsFalse(v.IsAnswered);

			v = new MultipleChoiceValue(String.Empty);
			Assert.IsTrue(v.IsAnswered);

			v = new MultipleChoiceValue(new string[0]);
			Assert.IsFalse(v.IsAnswered);

			v = new MultipleChoiceValue(options);
			Assert.IsTrue(v.IsAnswered);
			Assert.AreEqual("One|Two|Three", v.Value);
			Assert.AreEqual("One", v.Choices[0]);
			Assert.AreEqual("Two", v.Choices[1]);
			Assert.AreEqual("Three", v.Choices[2]);

			v = new MultipleChoiceValue("One|Two|Three");
			Assert.IsTrue(v.IsAnswered);
			Assert.AreEqual("One|Two|Three", v.Value);
			Assert.AreEqual("One", v.Choices[0]);
			Assert.AreEqual("Two", v.Choices[1]);
			Assert.AreEqual("Three", v.Choices[2]);

			// Test implicit conversion of string[] to MultipleChoiceValue
			Assert.AreEqual(((MultipleChoiceValue)options).Choices.Length, 3);
			Assert.AreEqual(((MultipleChoiceValue)options).Choices[0], "One");

			// Test implicit conversion of List<string> to MultipleChoiceValue
			Assert.AreEqual(((MultipleChoiceValue)optionsList).Choices.Length, 3);
			Assert.AreEqual(((MultipleChoiceValue)optionsList).Choices[1], "Two");
		}

		[TestMethod]
		public void Equals()
		{
			MultipleChoiceValue v1 = new MultipleChoiceValue("One|Two|Three");
			Assert.IsTrue(v1.Equals(new TextValue("One")));
			Assert.IsTrue(v1.Equals(new TextValue("oNe")));
			Assert.IsTrue(v1.Equals(new TextValue("TWO")));
			Assert.IsFalse(v1.Equals(new MultipleChoiceValue("Two")));

			MultipleChoiceValue v2 = new MultipleChoiceValue("One|TWO|ThRee");
			Assert.IsTrue(v1.Equals(v2));

			v2 = new MultipleChoiceValue("Three|Two|One");
			Assert.IsTrue(v1.Equals(v2));

			v2 = new MultipleChoiceValue("Three|Two|One|Four");
			Assert.IsFalse(v1.Equals(v2));

			v2 = new MultipleChoiceValue("One|Four|Three");
			Assert.IsFalse(v1.Equals(v2));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals1()
		{
			MultipleChoiceValue v = new MultipleChoiceValue("Choice");
			MultipleChoiceValue uv = new MultipleChoiceValue();

			uv.Equals(v);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals2()
		{
			MultipleChoiceValue v = new MultipleChoiceValue("Choice");
			MultipleChoiceValue uv = new MultipleChoiceValue();

			v.Equals(uv);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals3()
		{
			MultipleChoiceValue uv1 = new MultipleChoiceValue();
			MultipleChoiceValue uv2 = new MultipleChoiceValue();

			uv1.Equals(uv2);
		}

		[TestMethod]
		public void Casts()
		{
			MultipleChoiceValue v = MultipleChoiceValue.Unanswered;
			Assert.IsFalse(v.IsAnswered);

			v = new MultipleChoiceValue("Hello World");
			Assert.IsTrue(v.IsAnswered);
			Assert.AreEqual("Hello World", v.Value);
		}

	}
}
