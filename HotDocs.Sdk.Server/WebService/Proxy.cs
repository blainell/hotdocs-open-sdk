/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System.ServiceModel;
using System.ServiceModel.Channels;
using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk.Server.WebService
{
    /// <summary>
    ///     <c>Proxy</c> is a class that makes web service calls using the IHDSvc interface.
    /// </summary>
    public class Proxy : ClientBase<IHDSvc>, IHDSvc
    {
        /// <summary>
        /// </summary>
        public Proxy()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public Proxy(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public Proxy(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public Proxy(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public Proxy(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        /// <summary>
        ///     Call GetComponentInfo on HotDocs Server via web services
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="includeDialogs">Indicates whether or not information about dialogs should be included.</param>
        /// <returns></returns>
        public ComponentInfo GetComponentInfo(string templateID, bool includeDialogs)
        {
            return Channel.GetComponentInfo(templateID, includeDialogs);
        }

        /// <summary>
        ///     Call GetTemplateInfo on HotDocs Server via web services
        /// </summary>
        /// <param name="templateIDs"></param>
        /// <returns></returns>
        public Contracts.TemplateInfo[] GetTemplateInfo(string[] templateIDs)
        {
            return Channel.GetTemplateInfo(templateIDs);
        }

        /// <summary>
        ///     Call GetInterview on HotDocs Server via web services.
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="answers"></param>
        /// <param name="format"></param>
        /// <param name="options"></param>
        /// <param name="markedVariables"></param>
        /// <param name="formActionUrl"></param>
        /// <param name="serverFilesUrl"></param>
        /// <param name="styleUrl"></param>
        /// <param name="tempInterviewUrl"></param>
        /// <param name="saveAnswersUrl"></param>
        /// <param name="previewUrl"></param>
        /// <param name="interviewDefinitionUrl"></param>
        /// <returns></returns>
        public BinaryObject[] GetInterview(string templateID, BinaryObject[] answers, InterviewFormat format,
            InterviewOptions options, string[] markedVariables, string formActionUrl, string serverFilesUrl,
            string styleUrl, string tempInterviewUrl, string saveAnswersUrl, string previewUrl,
            string interviewDefinitionUrl)
        {
            return Channel.GetInterview(templateID, answers, format, options, markedVariables, formActionUrl,
                serverFilesUrl, styleUrl, tempInterviewUrl, saveAnswersUrl, previewUrl, interviewDefinitionUrl);
        }

        /// <summary>
        ///     Call GetInterviewDefinition on HotDocs Server via web services.
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="templateName"></param>
        /// <param name="format"></param>
        /// <param name="templateState"></param>
        /// <returns></returns>
        public BinaryObject GetInterviewDefinition(string templateID, string templateName, InterviewFormat format,
            string templateState)
        {
            return Channel.GetInterviewDefinition(templateID, templateName, format, templateState);
        }

        /// <summary>
        ///     Call GetAnswers on HotDocs Server via web services.
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public BinaryObject GetAnswers(BinaryObject[] answers)
        {
            return Channel.GetAnswers(answers);
        }

        /// <summary>
        ///     Call AssembleDocument on HotDocs Server via web services.
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="answers"></param>
        /// <param name="format"></param>
        /// <param name="options"></param>
        /// <param name="templateState"></param>
        /// <returns></returns>
        public AssemblyResult AssembleDocument(string templateID, BinaryObject[] answers, OutputFormat format,
            AssemblyOptions options, string templateState)
        {
            return Channel.AssembleDocument(templateID, answers, format, options, templateState);
        }

        /// <summary>
        ///     Call BuildSupportFiles on HotDocs Server via web services.
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="templateKey"></param>
        /// <param name="build"></param>
        /// <param name="templateState"></param>
        public void BuildSupportFiles(string templateID, string templateKey, HdSupportFilesBuild build,
            string templateState)
        {
            Channel.BuildSupportFiles(templateID, templateKey, build, templateState);
        }

        /// <summary>
        ///     Call RemoveSupportFiles on HotDocs Server via web services.
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="templateKey"></param>
        /// <param name="templateState"></param>
        public void RemoveSupportFiles(string templateID, string templateKey, string templateState)
        {
            Channel.RemoveSupportFiles(templateID, templateKey, templateState);
        }
    }
}