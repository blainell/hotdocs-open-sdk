﻿/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDocs.SdkTest
{
    [TestClass]
    public class AnswerSetTest
    {
        [TestMethod]
        public void ReadXml()
        {
            var anss = new AnswerCollection();
            // this test checks some answer XML generated by browser interviews. It was problematic originally because of its empty <RptValue> elements
            // (expressed as paired open/close elements with nothing between them, rather than single elements with open/close combined in the same element)
            // NOTE: added a lot of oddly formed and malformed answers to this answer collection, for purposes of
            // trying to ensure that we read answer XML in a more robust manner.  Some 3rd parties generate XML
            // that is schema valid, but is otherwise (by HotDocs standards) quite odd.  For example, answers marked
            // "unanswered" but which have answer data present, or which use collapsed XML elements in ways
            // HotDocs typically does not.
            anss.ReadXml(@"<?xml version=""1.0"" standalone=""yes""?>
<AnswerSet title="""" version=""1.1"" useMangledNames=""false"">
	<Answer name=""Editor Full Name"">
		<TextValue unans=""true"" />
	</Answer>
	<Answer name=""Text000"">
		<TextValue/>
	</Answer>
	<Answer name=""Text010"">
		<TextValue></TextValue>
	</Answer>
	<Answer name=""Text020"">
		<TextValue>Test Answer</TextValue>
	</Answer>
	<Answer name=""Text030"">
		<TextValue>
    Another
Test Answer
        </TextValue>
	</Answer>
	<Answer name=""Text040"">
		<TextValue unans=""true""></TextValue>
	</Answer>
	<Answer name=""Text050"">
		<TextValue unans=""true"">An Invalid Test Answer</TextValue>
	</Answer>
	<Answer name=""Number000"">
		<NumValue/>
	</Answer>
	<Answer name=""Number010"">
		<NumValue unans=""true"" />
	</Answer>
	<Answer name=""Number020"">
		<NumValue unans=""true""></NumValue>
	</Answer>
	<Answer name=""Number030"">
		<NumValue>5.0000000</NumValue>
	</Answer>
	<Answer name=""Number040"">
		<NumValue>123</NumValue>
	</Answer>
	<Answer name=""Number050"">
		<NumValue unans=""true"">234 or anything else</NumValue>
	</Answer>
	<Answer name=""Date000"">
		<DateValue/>
	</Answer>
	<Answer name=""Date010"">
		<DateValue></DateValue>
	</Answer>
	<Answer name=""Date020"">
		<DateValue unans=""true"" />
	</Answer>
	<Answer name=""Date030"">
		<DateValue unans=""true"" ></DateValue>
	</Answer>
	<Answer name=""Date Completed"">
		<DateValue>31-10-2015</DateValue>
	</Answer>
	<Answer name=""TF000"">
		<TFValue/>
	</Answer>
	<Answer name=""TF010"">
		<TFValue></TFValue>
	</Answer>
	<Answer name=""TF020"">
		<TFValue unans=""true"" />
	</Answer>
	<Answer name=""TF030"">
		<TFValue unans=""true""></TFValue>
	</Answer>
	<Answer name=""TF040"">
		<TFValue>true</TFValue>
	</Answer>
	<Answer name=""TF050"">
		<TFValue unans=""false"">False</TFValue>
	</Answer>
	<Answer name=""TF060"">
		<TFValue unans=""true"">Ignored</TFValue>
	</Answer>
    <Answer name=""MCVar000"">
        <MCValue/>
    </Answer>
    <Answer name=""MCVar010"">
        <MCValue unans=""true"" />
    </Answer>
    <Answer name=""MCVar020"">
        <MCValue></MCValue>
    </Answer>
    <Answer name=""MCVar030"">
        <MCValue unans=""true""></MCValue>
    </Answer>
    <Answer name=""MCVar031"">
        <MCValue>
            <SelValue/>
        </MCValue>
    </Answer>
    <Answer name=""MCVar032"">
        <MCValue unans=""true"">
            <SelValue unans=""true""/>
        </MCValue>
    </Answer>
    <Answer name=""MCVar033"">
        <MCValue unans=""true"">
            <SelValue></SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar034"">
        <MCValue unans=""true"">
            <SelValue unans=""true""></SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar035"">
        <MCValue unans=""true"">
            <SelValue>sel1</SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar040"">
        <MCValue>
            <SelValue/>
        </MCValue>
    </Answer>
    <Answer name=""MCVar050"">
        <MCValue>
            <SelValue unans=""true""/>
        </MCValue>
    </Answer>
    <Answer name=""MCVar060"">
        <MCValue>
            <SelValue></SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar070"">
        <MCValue>
            <SelValue unans=""true""></SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar080"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue>sel2</SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar090"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue></SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar100"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue/>
        </MCValue>
    </Answer>
    <Answer name=""MCVar110"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue unans=""true""></SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar120"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue unans=""true""/>
        </MCValue>
    </Answer>
    <Answer name=""MCVar130"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue></SelValue>
            <SelValue>sel3</SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar140"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue/>
            <SelValue>sel3</SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar150"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue unans=""true""></SelValue>
            <SelValue>sel3</SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar160"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue unans=""true""/>
            <SelValue>sel3</SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar170"">
        <MCValue>
            <SelValue>sel1</SelValue>
            <SelValue unans=""true"">sel2</SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar180"">
        <MCValue>
            <SelValue unans=""true"">sel1</SelValue>
            <SelValue unans=""true"">sel2</SelValue>
        </MCValue>
    </Answer>
    <Answer name=""MCVar190"">
        <MCValue>
            <SelValue unans=""true"">sel1</SelValue>
            <SelValue>sel2</SelValue>
        </MCValue>
    </Answer>
	<Answer name=""Author Full Name"">
		<RptValue>
			<RptValue>
				<TextValue>A</TextValue>
				<TextValue unans=""true"" />
			</RptValue>
			<RptValue></RptValue>
		</RptValue>
	</Answer>
	<Answer name=""Book Title"">
		<RptValue>
			<RptValue>
				<RptValue>
					<TextValue>A</TextValue>
					<TextValue unans=""true"" />
				</RptValue>
				<RptValue></RptValue>
			</RptValue>
			<RptValue></RptValue>
		</RptValue>
	</Answer>
</AnswerSet>
");
            Assert.IsTrue(anss.AnswerCount == 52);
            IAnswer ans;

            // ensure that lookup of incorrect typed answer fails
            Assert.IsFalse(anss.TryGetAnswer("Editor Full Name", ValueType.Number, out ans));

            // test various attributes of an answer
            Assert.IsTrue(anss.TryGetAnswer("Editor Full Name", ValueType.Text, out ans));
            Assert.IsFalse(ans.IsRepeated);
            Assert.IsTrue(ans.Save);
            Assert.IsTrue(ans.UserExtendible);
            Assert.IsTrue(ans.Type == ValueType.Text);
            Assert.IsFalse(ans.GetAnswered());
            Assert.IsFalse(ans.GetValue<TextValue>().IsAnswered);
            Assert.IsTrue(ans.GetValue<TextValue>().Type == ValueType.Text);
            Assert.IsTrue(ans.GetValue<TextValue>().UserModifiable);

            // ensure lookup of name with incorrect casing fails
            Assert.IsFalse(anss.TryGetAnswer("author full name", ValueType.Text, out ans));

            // check some repeated answer indexing rules
            Assert.IsTrue(anss.TryGetAnswer("Author Full Name", ValueType.Text, out ans));
            Assert.IsTrue(ans.IsRepeated);
            Assert.IsTrue(ans.GetChildCount() == 1);
            Assert.IsTrue(ans.GetChildCount(0) == 1);
            Assert.IsTrue(ans.GetChildCount(1) == 0);
            Assert.IsTrue(ans.GetValue<TextValue>(0, 0).Value == "A");
            Assert.IsFalse(ans.GetValue<TextValue>(0, 1).IsAnswered);
            Assert.IsFalse(ans.GetValue<TextValue>(1).IsAnswered);
            Assert.IsFalse(ans.GetValue<TextValue>(1, 0).IsAnswered);
            // unusual HotDocs indexing rules
            Assert.IsTrue(ans.GetValue<TextValue>().IsAnswered);
            Assert.IsTrue(ans.GetValue<TextValue>().Value == "A");
            Assert.IsTrue(ans.GetValue<TextValue>(0).IsAnswered);
            Assert.IsTrue(ans.GetValue<TextValue>(0).Value == "A");
            Assert.IsTrue(ans.GetValue<TextValue>(0, 0, 0).IsAnswered);
            Assert.IsTrue(ans.GetValue<TextValue>(0, 0, 0).Value == "A");

            // ensure that lookup of non-existing answer fails
            Assert.IsFalse(anss.TryGetAnswer("does not exist", ValueType.Text, out ans));
        }
    }
}