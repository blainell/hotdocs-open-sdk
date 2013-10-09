(function($){

if(!$.Templates)
{
	$.Templates={};
}

$.Templates.Tmpl_sample_0020interview_0020template_002Ecmp=
{
	InterviewVersion:"6.5.0.0",
	
	Interview:function()
	{
		$.PsU();
		this.INTERVIEW_Script();
		$.PpU();
	},
	
	INTERVIEW_Script:function()
	{
		var result=null;
		var condResult=null;
		if($.GtU())return result;
		
		$.PsS("INTERVIEW","C");
		$.Ask("Dialog1",false);
		$.PpS();
		return result;
	},
	
	ComponentFileName:"sample interview template.cmp",
	
	DefineComponents:function()
	{
		var tpl=this;
		var compDict={};
		var stack=new $.Stack();
		var item;
		var pref;
		var fmt;
		
		pref={
			n:"(TEMPLATE_ID)",
			t:"PRF",
			s:"ffd7560f-0e8d-4623-b75a-92056d84abce"
		};
		compDict[pref.n]=pref;
		
		pref={
			n:"(MARK_ANSWER_FIELDS)",
			t:"PRF",
			s:"True"
		};
		compDict[pref.n]=pref;
		
		item={
			n:"INTERVIEW",
			t:"VCO",
			y:"TX",
			s:tpl.INTERVIEW_Script
		};
		compDict[item.n]=item;
		
		{
			// Dialog: Dialog1
			stack.Push([]);
			
			item={
				n:"Hello interview template",
				t:"VTX",
				fw:"F"
			};
			compDict[item.n]=item;
			stack.Peek().push(item);
			
			item={
				n:"Dialog1",
				t:"VDI",
				l:stack.Peek(),
				pl:[
					0
				]
			};
			compDict[item.n]=item;
			stack.Pop();
		}
		
		return compDict;
	}
};
window['HOTDOC$'] = $;
})(typeof HOTDOC$ == 'undefined' ? {} : HOTDOC$);
