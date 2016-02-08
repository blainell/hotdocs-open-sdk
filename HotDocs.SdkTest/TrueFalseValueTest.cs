/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.SdkTest
{
    [TestClass]
    public class TrueFalseValueTest
    {
        [TestMethod]
        public void IsAnswered()
        {
            var v = new TrueFalseValue();
            Assert.IsFalse(v.IsAnswered);
        }

        [TestMethod]
        public void Equals()
        {
            var v1 = new TrueFalseValue(true);

            Assert.IsTrue(v1.Equals(new TrueFalseValue(true)));
            Assert.IsFalse(v1.Equals(new TrueFalseValue(false)));
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals1()
        {
            var v = new TrueFalseValue(true);
            var uv = new TrueFalseValue();

            uv.Equals(v);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals2()
        {
            var v = new TrueFalseValue(false);
            var uv = new TrueFalseValue();

            v.Equals(uv);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals3()
        {
            var uv1 = new TrueFalseValue();
            var uv2 = new TrueFalseValue();

            uv1.Equals(uv2);
        }

        [TestMethod]
        public void Casts()
        {
            var v = TrueFalseValue.Unanswered;
            Assert.IsFalse(v.IsAnswered);

            v = false;
            Assert.IsTrue(v.IsAnswered);
            Assert.AreEqual(false, v.Value);
        }
    }
}