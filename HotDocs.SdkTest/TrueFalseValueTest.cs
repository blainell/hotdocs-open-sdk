/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HotDocs.SdkTest
{
	[TestClass]
	public class TrueFalseValueTest
	{
		[TestMethod]
		public void IsAnswered()
		{
			TrueFalseValue v = new TrueFalseValue();
			Assert.IsFalse(v.IsAnswered);
		}

		[TestMethod]
		public void Equals()
		{
			TrueFalseValue v1 = new TrueFalseValue(true);

			Assert.IsTrue(v1.Equals(new TrueFalseValue(true)));
			Assert.IsFalse(v1.Equals(new TrueFalseValue(false)));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals1()
		{
			TrueFalseValue v = new TrueFalseValue(true);
			TrueFalseValue uv = new TrueFalseValue();

			uv.Equals(v);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals2()
		{
			TrueFalseValue v = new TrueFalseValue(false);
			TrueFalseValue uv = new TrueFalseValue();

			v.Equals(uv);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals3()
		{
			TrueFalseValue uv1 = new TrueFalseValue();
			TrueFalseValue uv2 = new TrueFalseValue();

			uv1.Equals(uv2);
		}

		[TestMethod]
		public void Casts()
		{
			TrueFalseValue v = TrueFalseValue.Unanswered;
			Assert.IsFalse(v.IsAnswered);

			v = false;
			Assert.IsTrue(v.IsAnswered);
			Assert.AreEqual(false, v.Value);
		}

	}
}
