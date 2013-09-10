/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk.Server.WebService
{
	public class Proxy : ClientBase<IHDSvc>, IHDSvc
	{

		public Proxy()
		{
		}

		public Proxy(string endpointConfigurationName) :
			base(endpointConfigurationName)
		{
		}

		public Proxy(string endpointConfigurationName, string remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}

		public Proxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}

		public Proxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			base(binding, remoteAddress)
		{
		}

		public ComponentInfo GetComponentInfo(string templateID, bool includeDialogs)
		{
			return base.Channel.GetComponentInfo(templateID, includeDialogs);
		}

		public HotDocs.Sdk.Server.Contracts.TemplateInfo[] GetTemplateInfo(string[] templateIDs)
		{
			return base.Channel.GetTemplateInfo(templateIDs);
		}

		public BinaryObject[] GetInterview(string templateID, BinaryObject[] answers, InterviewFormat format, InterviewOptions options, string[] markedVariables, string formActionUrl, string serverFilesUrl, string styleUrl, string tempInterviewUrl, string saveAnswersUrl, string previewUrl, string interviewDefinitionUrl)
		{
			return base.Channel.GetInterview(templateID, answers, format, options, markedVariables, formActionUrl, serverFilesUrl, styleUrl, tempInterviewUrl, saveAnswersUrl, previewUrl, interviewDefinitionUrl);
		}

		public BinaryObject GetInterviewDefinition(string templateID, string templateName, InterviewFormat format, string templateState)
		{
			return base.Channel.GetInterviewDefinition(templateID, templateName, format, templateState);
		}

		public BinaryObject GetAnswers(BinaryObject[] answers)
		{
			return base.Channel.GetAnswers(answers);
		}

		public AssemblyResult AssembleDocument(string templateID, BinaryObject[] answers, OutputFormat format, AssemblyOptions options, string templateState)
		{
			return base.Channel.AssembleDocument(templateID, answers, format, options, templateState);
		}

		public void BuildSupportFiles(string templateID, string templateKey, HDSupportFilesBuildFlags buildFlags, string templateState)
		{
			base.Channel.BuildSupportFiles(templateID, templateKey, buildFlags, templateState);
		}

		public void RemoveSupportFiles(string templateID, string templateKey, string templateState)
		{
			base.Channel.RemoveSupportFiles(templateID, templateKey, templateState);
		}
	}
}
