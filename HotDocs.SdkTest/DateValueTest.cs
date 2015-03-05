/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HotDocs.SdkTest
{
	[TestClass]
	public class DateValueTest
	{
		[TestMethod]
		public void IsAnswered()
		{
			DateValue v = new DateValue();
			Assert.IsFalse(v.IsAnswered);
		}

		[TestMethod]
		public void Equals()
		{
			DateValue v = new DateValue(1961, 6, 6);

			Assert.IsTrue(v.Equals(new DateValue(1961, 6, 6)));
			Assert.IsFalse(v.Equals(new DateValue(1961, 6, 5)));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals1()
		{
			DateValue v = new DateValue(1961, 6, 6);
			DateValue uv = new DateValue();

			uv.Equals(v);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals2()
		{
			DateValue v = new DateValue(1961, 6, 6);
			DateValue uv = new DateValue();

			v.Equals(uv);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnansEquals3()
		{
			DateValue uv1 = new DateValue();
			DateValue uv2 = new DateValue();

			uv1.Equals(uv2);
		}

		[TestMethod]
		public void Casts()
		{
			DateValue v = DateValue.Unanswered;
			Assert.IsFalse(v.IsAnswered);

			v = new DateTime(1961, 6, 6);
			Assert.IsTrue(v.IsAnswered);
			Assert.AreEqual(new DateTime(1961, 6, 6), v.Value);
		}
	}
}
