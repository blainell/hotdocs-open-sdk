/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.SdkTest
{
    [TestClass]
    public class DateValueTest
    {
        [TestMethod]
        public void IsAnswered()
        {
            var v = new DateValue();
            Assert.IsFalse(v.IsAnswered);
        }

        [TestMethod]
        public void Equals()
        {
            var v = new DateValue(1961, 6, 6);

            Assert.IsTrue(v.Equals(new DateValue(1961, 6, 6)));
            Assert.IsFalse(v.Equals(new DateValue(1961, 6, 5)));
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals1()
        {
            var v = new DateValue(1961, 6, 6);
            var uv = new DateValue();

            uv.Equals(v);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals2()
        {
            var v = new DateValue(1961, 6, 6);
            var uv = new DateValue();

            v.Equals(uv);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals3()
        {
            var uv1 = new DateValue();
            var uv2 = new DateValue();

            uv1.Equals(uv2);
        }

        [TestMethod]
        public void Casts()
        {
            var v = DateValue.Unanswered;
            Assert.IsFalse(v.IsAnswered);

            v = new DateTime(1961, 6, 6);
            Assert.IsTrue(v.IsAnswered);
            Assert.AreEqual(new DateTime(1961, 6, 6), v.Value);
        }
    }
}