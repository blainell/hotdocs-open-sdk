/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.SdkTest
{
    [TestClass]
    public class MultipleChoiceValueTest
    {
        [TestMethod]
        public void IsAnswered()
        {
            var v = new MultipleChoiceValue();
            Assert.IsFalse(v.IsAnswered);
        }

        [TestMethod]
        public void Constructors()
        {
            string[] options = {"One", "Two", "Three"};
            var optionsList = new List<string>(options);

            string s = null;
            var v = new MultipleChoiceValue(s);
            Assert.IsFalse(v.IsAnswered);

            v = new MultipleChoiceValue(string.Empty);
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
            Assert.AreEqual(((MultipleChoiceValue) options).Choices.Length, 3);
            Assert.AreEqual(((MultipleChoiceValue) options).Choices[0], "One");

            // Test implicit conversion of List<string> to MultipleChoiceValue
            Assert.AreEqual(((MultipleChoiceValue) optionsList).Choices.Length, 3);
            Assert.AreEqual(((MultipleChoiceValue) optionsList).Choices[1], "Two");
        }

        [TestMethod]
        public void Equals()
        {
            var v1 = new MultipleChoiceValue("One|Two|Three");
            Assert.IsTrue(v1.Equals(new TextValue("One")));
            Assert.IsTrue(v1.Equals(new TextValue("oNe")));
            Assert.IsTrue(v1.Equals(new TextValue("TWO")));
            Assert.IsFalse(v1.Equals(new MultipleChoiceValue("Two")));

            var v2 = new MultipleChoiceValue("One|TWO|ThRee");
            Assert.IsTrue(v1.Equals(v2));

            v2 = new MultipleChoiceValue("Three|Two|One");
            Assert.IsTrue(v1.Equals(v2));

            v2 = new MultipleChoiceValue("Three|Two|One|Four");
            Assert.IsFalse(v1.Equals(v2));

            v2 = new MultipleChoiceValue("One|Four|Three");
            Assert.IsFalse(v1.Equals(v2));
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals1()
        {
            var v = new MultipleChoiceValue("Choice");
            var uv = new MultipleChoiceValue();

            uv.Equals(v);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals2()
        {
            var v = new MultipleChoiceValue("Choice");
            var uv = new MultipleChoiceValue();

            v.Equals(uv);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals3()
        {
            var uv1 = new MultipleChoiceValue();
            var uv2 = new MultipleChoiceValue();

            uv1.Equals(uv2);
        }

        [TestMethod]
        public void Casts()
        {
            var v = MultipleChoiceValue.Unanswered;
            Assert.IsFalse(v.IsAnswered);

            v = new MultipleChoiceValue("Hello World");
            Assert.IsTrue(v.IsAnswered);
            Assert.AreEqual("Hello World", v.Value);
        }
    }
}