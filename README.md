HotDocs Open SDK
================
v0.9, September 2013

The HotDocs Open SDK is a toolkit designed to ease integration between 3rd party applications and HotDocs.
It is implemented in C# and can be utilized by any managed (.NET) application to simplify communication
with HotDocs Cloud Services and/or HotDocs Server. The SDK is comprised of several components that can be
used together or (when it makes sense) individually:

*	**HotDocs.Sdk** - This managed DLL helps with parsing and/or creating various XML-based data files
	necessary for deep HotDocs integrations. For example, you can easily create and parse HotDocs XML
	answer collections. It also exposes methods for parsing and using template and package manifests,
	as well as code to extract files from template packages. *Compatible with .NET 2.0 and later.*

*	**HotDocs.Sdk.Cloud** - This managed DLL (also known as the Cloud Services Client Library)
	simplifies communication with the web services interfaces exposed by HotDocs Cloud Services.
	It exposes both the "Embedded" and "Direct" methods of integrating HotDocs browser interviews
	into your web application, and encapsulates such details as proper error handling. *Compatible with
	.NET 2.0 and later.*

*	**HotDocs.Sdk.Server** - This managed DLL allows applications to transparently integrate with either
	HotDocs Cloud Services (running in the cloud) or HotDocs Server (running on the same machine or
	remotely via web services). It provides an abstraction layer that hides many of the differences
	between those APIs. It also includes a WorkSession class that greatly simplifies implementation of
	basic HotDocs workflows (a.k.a. assembly queues). *Compatible with .NET 4.0 and later.*

*	**HotDocs.Sdk.Server.Contracts** - This small managed DLL contains shared data types and some
	protocols/algorithms necessary for proper communication between client code and HotDocs-provided web
	services. This includes e.g. Windows Communication Foundation (WCF) contracts for data types and
	services, and the standard HMAC algorithm necessary to call Cloud Services. Note: this project is
	included for informational and compatibility reasons. Changes to code in this module are not
	expected. *Compatible with .NET 2.0 and later.*

*	**HotDocs.Sdk.DataServices** - This managed DLL implements an OData data service facilitating
	the new Answer Source capabilities in HotDocs 11 browser interviews. On their own, browser interviews
	can display answer sources based on the "current" answer file only; however, host applications
	incorporating this DLL can also deliver data from server-side answer files. This DLL also provides
	a pattern to follow in implementing custom server-side answer sources. *Compatible with .NET 4.0
	and later.*

*	**SamplePortal** - This is a sample ASP.NET (Web Forms) application that illustrates how to use the
	HotDocs.Sdk.Server abstraction layer for communicating transparently with HotDocs Server and Cloud
	Services.

Requirements
------------
In order to take advantage of this project, you should either have a valid subscriber ID for HotDocs
Cloud Services or have access to a licensed installation of HotDocs Server. You must also have the
ability to write and compile ASP.NET web applications.

Other Platforms
---------------
Parts of the Open SDK have been ported to other languages and platforms to facilitate HotDocs integration
from those platforms:

*	[Java] (https://github.com/HotDocsCorp/hotdocs-cloud-java)
*	[PHP] (https://github.com/HotDocsCorp/hotdocs-cloud-php)
*	[Ruby](https://github.com/pifleo/hotdocs-cloud)

*NOTE: At the present time, these ports are limited and include only minimal portions of the
Cloud Services Client Library -- specifically the interfaces necessary to work with HotDocs Embedded.

Contributing
------------
We welcome pull requests from potential contributors. If you think of a great way to improve HotDocs
integration with another application, we're all ears. Or if you are interested in helping port this
SDK (or any portion of it) onto any other platform or language, please let us know!
