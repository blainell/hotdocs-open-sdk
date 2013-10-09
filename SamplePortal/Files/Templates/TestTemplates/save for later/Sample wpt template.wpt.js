(function($){

if(!$.Templates)
{
	$.Templates={};
}

$.Templates.Tmpl_Sample_0020wpt_0020template_002Ewpt=
{
	InterviewVersion:"6.5.0.0",
	
	TemplateInterview:function()
	{
		var condResult=null;
		
		$.Mrg("sample wpt template field");
	},
	
	ComponentFileName:"Sample wpt template.cmp",
	
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
			s:"d361e037-9aab-4dc6-ae8a-728ac2b8bc7e"
		};
		compDict[pref.n]=pref;
		
		pref={
			n:"(MARK_ANSWER_FIELDS)",
			t:"PRF",
			s:"True"
		};
		compDict[pref.n]=pref;
		
		item={
			n:"sample wpt template field",
			t:"VTX",
			fw:"F"
		};
		compDict[item.n]=item;
		
		return compDict;
	}
};
window['HOTDOC$'] = $;
})(typeof HOTDOC$ == 'undefined' ? {} : HOTDOC$);
