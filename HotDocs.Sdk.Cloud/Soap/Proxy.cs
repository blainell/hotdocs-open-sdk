/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk.Cloud.Soap
{
    internal class Proxy : ClientBase<ICore>, ICore
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

        public Proxy(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public Proxy(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public AssemblyResult AssembleDocument(string subscriberID, string packageID, string templateName,
            BinaryObject[] answers, OutputFormat format, OutputOptions outputOptions,
            Dictionary<string, string> settings, string billingRef, DateTime timestamp, BinaryObject templatePackage,
            string hmac)
        {
            return Channel.AssembleDocument(subscriberID, packageID, templateName, answers, format, outputOptions,
                settings, billingRef, timestamp, templatePackage, hmac);
        }

        public BinaryObject[] GetInterview(string subscriberID, string packageID, string templateName,
            BinaryObject[] answers, InterviewFormat format, string[] markedVariables, string tempImageUrl,
            Dictionary<string, string> settings, string billingRef, DateTime timestamp, BinaryObject templatePackage,
            string hmac)
        {
            return Channel.GetInterview(subscriberID, packageID, templateName, answers, format, markedVariables,
                tempImageUrl, settings, billingRef, timestamp, templatePackage, hmac);
        }

        public ComponentInfo GetComponentInfo(string subscriberID, string packageID, string templateName,
            bool includeDialogs, string billingRef, DateTime timestamp, BinaryObject templatePackage, string hmac)
        {
            return Channel.GetComponentInfo(subscriberID, packageID, templateName, includeDialogs, billingRef, timestamp,
                templatePackage, hmac);
        }

        public BinaryObject GetAnswers(string subscriberID, BinaryObject[] answers, string billingRef,
            DateTime timestamp, string hmac)
        {
            return Channel.GetAnswers(subscriberID, answers, billingRef, timestamp, hmac);
        }
    }
}