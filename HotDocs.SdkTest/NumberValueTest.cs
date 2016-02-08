/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.SdkTest
{
    [TestClass]
    public class NumberValueTest
    {
        [TestMethod]
        public void IsAnswered()
        {
            var v = new NumberValue();
            Assert.IsFalse(v.IsAnswered);
        }

        [TestMethod]
        public void Equals()
        {
            var v = new NumberValue(47.65);

            Assert.IsTrue(v.Equals(new NumberValue(47.65)));
            Assert.IsFalse(v.Equals(new NumberValue(47.66)));
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals1()
        {
            var v = new NumberValue(47.65);
            var uv = new NumberValue();

            uv.Equals(v);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals2()
        {
            var v = new NumberValue(47.65);
            var uv = new NumberValue();

            v.Equals(uv);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals3()
        {
            var uv1 = new NumberValue();
            var uv2 = new NumberValue();

            uv1.Equals(uv2);
        }

        [TestMethod]
        public void Casts()
        {
            var v = NumberValue.Unanswered;
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