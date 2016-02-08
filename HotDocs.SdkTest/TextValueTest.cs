/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ValueType = HotDocs.Sdk.ValueType;

namespace HotDocs.SdkTest
{
    /// <summary>
    ///     This is a test class for TextValueTest and is intended
    ///     to contain all TextValueTest Unit Tests
    /// </summary>
    [TestClass]
    public class TextValueTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void TextValueStaticTest1()
        {
            Assert.IsFalse(TextValue.Unanswered.IsAnswered);
            Assert.AreEqual(TextValue.Unanswered.Type, ValueType.Text);
            Assert.IsTrue(TextValue.Unanswered.UserModifiable);
            Assert.IsNull(TextValue.Unanswered.Value);
            Assert.IsInstanceOfType(TextValue.Unanswered, typeof (TextValue));
        }

        [TestMethod]
        public void TextValueStaticTest2()
        {
            Assert.IsFalse(TextValue.UnansweredLocked.IsAnswered);
            Assert.AreEqual(TextValue.UnansweredLocked.Type, ValueType.Text);
            Assert.IsFalse(TextValue.UnansweredLocked.UserModifiable);
            Assert.IsNull(TextValue.UnansweredLocked.Value);
            Assert.IsInstanceOfType(TextValue.UnansweredLocked, typeof (TextValue));
        }

        /// <summary>
        ///     A test for TextValue Constructor
        /// </summary>
        [TestMethod]
        public void TextValueConstructorTest()
        {
            var target = new TextValue("Test\nValue");
            Assert.IsTrue(target.IsAnswered);
            Assert.AreEqual(target.Type, ValueType.Text);
            Assert.IsTrue(target.UserModifiable);
            Assert.AreEqual(target.Value, "Test\r\nValue");
        }

        /// <summary>
        ///     A test for TextValue Constructor
        /// </summary>
        [TestMethod]
        public void TextValueConstructorTest1()
        {
            var target = new TextValue("Test\nValue", true);
            Assert.IsTrue(target.IsAnswered);
            Assert.AreEqual(target.Type, ValueType.Text);
            Assert.IsTrue(target.UserModifiable);
            Assert.AreEqual(target.Value, "Test\r\nValue");
        }

        /// <summary>
        ///     A test for TextValue Constructor
        /// </summary>
        [TestMethod]
        public void TextValueConstructorTest2()
        {
            var target = new TextValue("Test\nValue", false);
            Assert.IsTrue(target.IsAnswered);
            Assert.AreEqual(target.Type, ValueType.Text);
            Assert.IsFalse(target.UserModifiable);
            Assert.AreEqual(target.Value, "Test\r\nValue");
        }

        /// <summary>
        ///     A test for Equals
        /// </summary>
        [TestMethod]
        public void EqualsTest()
        {
            var target = new TextValue("Test");
            object obj = "Test"; // string
            var expected = false; // we expect them not to be equal because they are not the same type.
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
        ///     A test for Equals
        /// </summary>
        [TestMethod]
        public void EqualsTest1()
        {
            var target = new TextValue("my value");
            var operand = new TextValue("my value");
            var expected = true;
            bool actual;
            actual = target.Equals(operand);
            Assert.AreEqual(expected, actual);

            operand = new TextValue("my other value");
            expected = false;
            actual = target.Equals(operand);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///     A test for NormalizeLineBreaks
        /// </summary>
        [TestMethod]
        public void NormalizeLineBreaksTest()
        {
            var text = "\r\nline 1\nline 2\rline 3\r\nline 4\n\rline 5\r";
            var expected = "\r\nline 1\r\nline 2\r\nline 3\r\nline 4\r\n\r\nline 5\r\n";
            string actual;
            actual = TextValue.NormalizeLineBreaks(text);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsAnswered()
        {
            var v = new TextValue();
            Assert.IsFalse(v.IsAnswered);
        }

        [TestMethod]
        public void Equals()
        {
            var v1 = new TextValue("Hello World");
            var v2 = new TextValue("hELLO wORLD");

            Assert.IsTrue(v1.Equals(v2));
            Assert.IsFalse(v1.Equals(new TextValue("Hello Worl")));
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals1()
        {
            var v = new TextValue("Hello World");
            var uv = new TextValue();

            uv.Equals(v);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals2()
        {
            var v = new TextValue("Hello World");
            var uv = new TextValue();

            v.Equals(uv);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void UnansEquals3()
        {
            var uv1 = new TextValue();
            var uv2 = new TextValue();

            uv1.Equals(uv2);
        }

        [TestMethod]
        public void Casts()
        {
            var v = TextValue.Unanswered;
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