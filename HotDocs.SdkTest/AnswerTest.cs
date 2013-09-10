/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotDocs.Sdk;

namespace HotDocs.SdkTest
{
	[TestClass]
	public class AnswerTest
	{
		//public class IndexedValue<T> where T : IValue
		//{
		//	public T Value { get; set; }
		//	public int[] RepeatIndices { get; }
		//}

		[TestMethod]
		public void EnumerateValues()
		{
			var filterItems = new List<string>();

			AnswerCollection anss = new AnswerCollection();
			Answer ta = anss.CreateAnswer<TextValue>("my answer");
			ta.UserExtendible = false; // user should not be able to add/remove/reorder iterations in the interview
			ta.SetValue<TextValue>(new TextValue("val1", false), 0); // user should not be able to modify this answer
			ta.SetValue<TextValue>(new TextValue("val2", true), 1);  // user will be able to modify this answer
			ta.SetValue<TextValue>(new TextValue("val3"), 2);        // user will be able to modify this answer
			ta.SetValue<TextValue>("val4", 3);                       // user will be able to modify this answer

			// now enumerate the values
			foreach (var v in ta.IndexedValues)
			{
				if (v.Answer.Type == Sdk.ValueType.Text)
				{
					TextValue t = v.GetValue<TextValue>();
					string s = t.Value;
				}
				// or something like
				IValue val = v.Value;
				if (val.IsAnswered)
				{
					switch (val.Type)
					{
						case Sdk.ValueType.Text:
							string s = Convert.ToString(val);
							break;
						case Sdk.ValueType.Number:
							double d = Convert.ToDouble(val);
							break;
						case Sdk.ValueType.Date:
						case Sdk.ValueType.TrueFalse:
						case Sdk.ValueType.MultipleChoice:
							break;
					}
				}

			}
		}

	}
}
