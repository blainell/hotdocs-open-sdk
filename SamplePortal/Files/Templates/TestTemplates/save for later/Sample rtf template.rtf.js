(function($){

if(!$.Templates)
{
	$.Templates={};
}

$.Templates.Tmpl_Sample_0020rtf_0020template_002Ertf=
{
	InterviewVersion:"6.5.0.0",
	
	TemplateInterview:function()
	{
		var condResult=null;
		
		$.Mrg("Hello Sample RTF");
	},
	
	ComponentFileName:"Sample rtf template.cmp",
	
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
			s:"4713a2b2-6535-4214-a4dc-aec858f327b6"
		};
		compDict[pref.n]=pref;
		
		pref={
			n:"(MARK_ANSWER_FIELDS)",
			t:"PRF",
			s:"True"
		};
		compDict[pref.n]=pref;
		
		item={
			n:"Hello Sample RTF",
			t:"VTX",
			fw:"F"
		};
		compDict[item.n]=item;
		
		return compDict;
	}
};
window['HOTDOC$'] = $;
})(typeof HOTDOC$ == 'undefined' ? {} : HOTDOC$);
