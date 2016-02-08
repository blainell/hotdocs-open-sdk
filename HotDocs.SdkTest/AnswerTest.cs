/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ValueType = HotDocs.Sdk.ValueType;

namespace HotDocs.SdkTest
{
    [TestClass]
    public class AnswerTest
    {
        [TestMethod]
        public void EnumerateValues()
        {
            var anss = new AnswerCollection();
            var ta = anss.CreateAnswer<TextValue>("my answer");
            ta.UserExtendible = false; // user should not be able to add/remove/reorder iterations in the interview
            ta.SetValue(new TextValue("val1", false), 0); // user should not be able to modify this answer
            ta.SetValue(new TextValue("val2", true), 1); // user will be able to modify this answer
            ta.SetValue(new TextValue("val3"), 2); // user will be able to modify this answer
            ta.SetValue<TextValue>("val4", 3); // user will be able to modify this answer

            // now enumerate the values
            AnswerValueEnumerator(ta);

            // now enumerate some Number values
            var tn = anss.CreateAnswer<NumberValue>("my num answer");
            tn.SetValue(new NumberValue(3), 0);
            AnswerValueEnumerator(tn);

            // now enumerate some Date values
            var td = anss.CreateAnswer<DateValue>("my date answer");
            td.SetValue(new DateValue(DateTime.Now), 0);
            AnswerValueEnumerator(td);
        }

        private void AnswerValueEnumerator(Answer a)
        {
            foreach (var v in a.IndexedValues)
            {
                if (v.Answer.Type == ValueType.Text)
                {
                    v.GetValue<TextValue>();
                }
                // or something like
                var val = v.Value;
                if (val.IsAnswered)
                {
                    switch (val.Type)
                    {
                        case ValueType.Text:
                            Convert.ToString(val);
                            break;
                        case ValueType.Number:
                            Convert.ToDouble(val);
                            break;
                        case ValueType.Date:
                        case ValueType.TrueFalse:
                        case ValueType.MultipleChoice:
                            break;
                    }
                }
            }
        }
    }
}