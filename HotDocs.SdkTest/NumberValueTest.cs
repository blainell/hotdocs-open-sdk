/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HotDocs.SdkTest
{
	[TestClass]
	public class NumberValueTest
	{
		[TestMethod]
		public void IsAnswered()
		{
			NumberValue v = new NumberValue();
			Assert.IsFalse(v.IsAnswered);
		}

		[TestMethod]
		public void Equals()
		{
			NumberValue v = new NumberValue(47.65);

			Assert.IsTrue(v.Equals(new NumberValue(47.65)));
			Assert.IsFalse(v.Equals(new NumberValue(47.66)));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals1()
		{
			NumberValue v = new NumberValue(47.65);
			NumberValue uv = new NumberValue();

			uv.Equals(v);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals2()
		{
			NumberValue v = new NumberValue(47.65);
			NumberValue uv = new NumberValue();

			v.Equals(uv);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals3()
		{
			NumberValue uv1 = new NumberValue();
			NumberValue uv2 = new NumberValue();

			uv1.Equals(uv2);
		}

		[TestMethod]
		public void Casts()
		{
			NumberValue v = NumberValue.Unanswered;
			Assert.IsFalse(v.IsAnswered);

			v = 47.65;
			Assert.IsTrue(v.IsAnswered);
			Assert.AreEqual(47.65, v.Value);

			v = 47;
			Assert.IsTrue(v.IsAnswered);
			Assert.AreEqual(47.0, v.Value);
		}

	}
}
