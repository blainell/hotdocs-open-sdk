/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System.Collections.Generic;
using System.Configuration;
using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk
{
	/// <summary>
	/// HotDocs uses <c>DateOrder</c> setting during assembly when it needs to interpret ambiguously-written date strings
	/// (for example, date format examples in which the day, month and/or year are all expressed as numbers).
	/// By default this property respects the behavior configured on the server.  Also note that this property
	/// is only respected by HotDocs Cloud Services; HotDocs Server always adheres to the behavior as configured
	/// in the management console.
	/// </summary>
	public enum DateOrder {

		/// <summary>
		/// defers to the behavior configured on the server.
		/// </summary>
		Default, 

		/// <summary>
		/// Uses Day/Month/Year (dd/mm/yyyy) format for date strings
		/// </summary>
		DMY, 
		
		/// <summary>
		/// Uses Month/Day/Year (mm/dd/yyyy) format for date strings
		/// </summary>
		MDY 
	}


	/// <summary>
	/// Dictates what to merge when a value for which no answer is available is merged into document text.
	/// By default this property respects the behavior configured on the server.  Also note that this property
	/// is only respected by HotDocs Cloud Services; HotDocs Server always adheres to the behavior as configured
	/// in the management console.
	/// </summary>
	public enum UnansweredFormat 
	{
		/// <summary>
		/// defers to the behavior configured on the server.
		/// </summary>
		Default, 

		/// <summary>
		/// Does nothing to merge a value for which no answer is available
		/// </summary>
		Nothing, 

		/// <summary>
		/// Provides an underscore to format the unanswered variable
		/// </summary>
		Underscores,


		/// <summary>
		/// Provides an asterisks to format the unanswered variable
		/// </summary>
		Asterisks, 
		
		/// <summary>
		/// Provides the variable name with brackets to format the unanswered variable
		/// </summary>
		VarNameWithBrackets,

		/// <summary>
		/// Provides the variable name with asterisks to format the unanswered variable
		/// </summary>
		VarNameWithAsterisks 
	}

	/// <summary>
	/// AssembleDocumentSettings encapsulates all the document assembly settings that can be specified in
	/// the HotDocs Server Open SDK.
	/// </summary>
	public class AssembleDocumentSettings : Settings
	{
		/// <summary>
		/// Creates a new copy of the default assembly settings.  Override the built-in defaults with your preferred
		/// defaults by adding appropriate settings to the appSettings section of your web.config file.
		/// </summary>
		public AssembleDocumentSettings()
			: this(AssembleDocumentSettings.Default)
		{
		}

		/// <summary>
		/// Copy constructor -- this constructor is invoked to make a new copy of the (static) Default AssembleDocumentSettings
		/// (as originally read from web.config).
		/// </summary>
		/// <param name="source">The source settings to be copied.</param>
		public AssembleDocumentSettings(AssembleDocumentSettings source)
		{
			Format = source.Format;
			OutputOptions = source.OutputOptions;

			RetainTransientAnswers = source.RetainTransientAnswers;
			UseMarkupSyntax = source.UseMarkupSyntax;

			DefaultUnansweredFormat = source.DefaultUnansweredFormat;
			HonorCmpUnansweredFormat = source.HonorCmpUnansweredFormat;
			DateOrderPreference = source.DateOrderPreference;
			DefaultDateFormat = source.DefaultDateFormat;
		}

		/// <summary>
		/// Private constructor for reading defaults from web.config.
		/// This constructor should only be invoked the first time the defaults are required.
		/// </summary>
		/// <param name="readDefaults">Indicates if the default settings should be read or not.</param>
		private AssembleDocumentSettings(bool readDefaults)
		{
			_settings = new Dictionary<string, string>();

			Format = DocumentType.Native;
			OutputOptions = null;

			RetainTransientAnswers = Util.ReadConfigurationBoolean("RetainTransientAnswers", false);
			UseMarkupSyntax = Util.ReadConfigurationBoolean("UseMarkupSyntax", false);

			// "shared" or "common" properties (these are also InterviewSettings)
			DefaultUnansweredFormat = Util.ReadConfigurationEnum<UnansweredFormat>("DefaultUnansweredFormat", UnansweredFormat.Default);
			HonorCmpUnansweredFormat = Util.ReadConfigurationTristate("HonorCmpUnansweredFormat", Tristate.Default);
			DateOrderPreference = Util.ReadConfigurationEnum<DateOrder>("DateOrder", DateOrder.Default);
			DefaultDateFormat = ConfigurationManager.AppSettings["DefaultDateFormat"];
		}

		static AssembleDocumentSettings()
		{
			s_default = null;
		}

		private static AssembleDocumentSettings s_default;

		private static AssembleDocumentSettings Default
		{
			get
			{
				// Avoid reading defaults out of web.config until/unless they are actually needed
				if (s_default == null)
					s_default = new AssembleDocumentSettings(true);
				return s_default;
			}
		}

		/* ------------------------------------------------------------------------------------------------
		 * these first two properties are optional, but they are not read from web.config or anywhere else...
		 * if the defaults are not desirable, your host application must set them explicitly each time you
		 * create an AssemblyDocumentSettings instance.
		 */

		/// <summary>
		/// The output format into which the requested document should be assembled or converted.
		/// If you request a format that is different from the template's native format, the assembled
		/// document will be automatically converted into the requested format (if possible).
		/// <para>This property initializes to "Native" by default, which means a document will be generated
		/// in the format native to whichever template is used.</para>
		/// <para>An exception will be thrown if you request an OutputFormat that is incompatible with the template format.</para>
		/// </summary>
		public DocumentType Format { get; set; }

		/// <summary>
		/// If the requested OutputFormat is PDF, HTML or PlainText, this property can specify additional
		/// options specific to that output format.  It is null by default, which causes HotDocs to output
		/// the requested document using its own internal defaults.
		/// </summary>
		public OutputOptions OutputOptions { get; set; }

		/* ------------------------------------------------------------------------------------------------
		 * remaining properties will be looked up in web.config & initialized from there if they exist,
		 * or initialized to the indicated default values if they don't.
		 */

		/// <summary>
		/// If false, 'save="false"' answers will be filtered out of resulting answer set. Defaults to false.
		/// Note that regardless of this property, HotDocs will not filter out 'save="false"' answers if
		/// the assembly results in additional pending assemblies.
		/// </summary>
		public bool RetainTransientAnswers { get; set; }
		/// <summary>
		/// Dictates whether this document will be assembled traditionally ("false") or using markup view ("true").
		/// </summary>
		/// <remarks>This property corresponds to the "Assemble marked-up documents" setting in the HotDocs
		/// Server management console, the AssembleMarkupDocument property in the HotDocs Server .NET API,
		/// and the AssembleMarkedDocuments setting in Cloud Services.</remarks>
		public bool UseMarkupSyntax { get; set; }
		/// <summary>
		/// Dictates what to merge (by default) when a value for which no answer is available is merged into document text.
		/// By default this property respects the behavior configured on the server.  Also note that this property
		/// is only respected by HotDocs Cloud Services; HotDocs Server always adheres to the behavior as configured
		/// in the management console.
		/// </summary>
		/// <remarks>Corresponds to the "Unanswered variable placeholder" HotDocs setting.</remarks>
		public UnansweredFormat DefaultUnansweredFormat
		{
			get
			{
				switch (GetSettingString("UnansweredFormat"))
				{
					case "Nothing":
						return UnansweredFormat.Nothing;
					case "Underscores":
						return UnansweredFormat.Underscores;
					case "Asterisks":
						return UnansweredFormat.Asterisks;
					case "[Variable]":
						return UnansweredFormat.VarNameWithBrackets;
					case "*** Variable ***":
						return UnansweredFormat.VarNameWithAsterisks;
					default:
						return UnansweredFormat.Default;
				}
			}
			set
			{
				string val = null;
				switch (value)
				{
					case UnansweredFormat.Nothing:
						val = "Nothing";
						break;
					case UnansweredFormat.Underscores:
						val = "Underscores";
						break;
					case UnansweredFormat.Asterisks:
						val = "Asterisks";
						break;
					case UnansweredFormat.VarNameWithBrackets:
						val = "";
						break;
					case UnansweredFormat.VarNameWithAsterisks:
						val = "";
						break;
				}
				SetSettingString("UnansweredFormat", val);
			}
		}
		/// <summary>
		/// Determines whether template author preferences (as specified in a HotDocs component file) should override the
		/// default UnansweredFormat specified by the host application.
		/// By default this property respects the behavior configured on the server.  Also note that this property
		/// is only respected by HotDocs Cloud Services; HotDocs Server always adheres to the behavior as configured
		/// in the management console.
		/// </summary>
		/// <remarks>Setting this to True corresponds to the behavior of desktop HotDocs, where template
		/// authors can override the user-selected default Unanswered format.</remarks>
		public Tristate HonorCmpUnansweredFormat
		{
			get
			{
				return GetSettingTristate("HonorCmpUnansFormat");
			}
			set
			{
				SetSettingString("HonorCmpUnansFormat", TristateToString(value));
			}
		}
		/// <summary>
		/// HotDocs uses this setting during assembly when it needs to interpret ambiguously-written date strings
		/// (for example, date format examples in which the day, month and/or year are all expressed as numbers).
		/// By default this property respects the behavior configured on the server.  Also note that this property
		/// is only respected by HotDocs Cloud Services; HotDocs Server always adheres to the behavior as configured
		/// in the management console.
		/// </summary>
		public DateOrder DateOrderPreference
		{
			get
			{
				switch ((GetSettingString("DateOrder") ?? "").ToUpper())
				{
					case "MDY":
						return DateOrder.MDY;
					case "DMY":
						return DateOrder.DMY;
					default:
						return DateOrder.Default;
				}
			}
			set
			{
				string dateOrder = null;
				switch (value)
				{
					case DateOrder.MDY:
						dateOrder = "MDY";
						break;
					case DateOrder.DMY:
						dateOrder = "DMY";
						break;
				}
				SetSettingString("DateOrder", dateOrder);
			}
		}
		/// <summary>
		/// Specifies the default format example to be used when merging date values into text.
		/// By default this property respects the behavior configured on the server.  Also note that this property
		/// is only respected by HotDocs Cloud Services; HotDocs Server always adheres to the behavior as configured
		/// in the management console.
		/// </summary>
		public string DefaultDateFormat
		{
			get
			{
				return GetSettingString("DefaultDateFormat");
			}
			set
			{
				SetSettingString("DefaultDateFormat", value);
			}
		}

		/// <summary>
		/// Collection of settings that can be specified when requesting a document assembly.
		/// Values may be provided for the following settings:
		/// <list type="table">
		/// <listheader>
		/// <term>Setting Name</term>
		/// <description>Description</description>
		/// </listheader>
		/// <item><term>DefaultDateFormat</term>
		/// <description>"June 3, 1990", etc.<br/>
		/// Specifies the default format example to be used when merging date values into text.</description></item>
		/// <item><term>DateOrder</term>
		/// <description>"MDY" or "DMY"<br/>
		/// HotDocs uses this setting during assembly when it needs to interpret ambiguously-written date strings
		/// (for example, date format examples in which the day, month and/or year are all expressed as numbers).</description></item>
		/// <item><term>UnansweredFormat</term>
		/// <description>"Nothing" / "Underscores" / "Asterisks" / "[Variable]" / "*** Variable ***"<br/>
		/// Dictates what to merge (by default) when a value for which no answer is available is merged into document text.
		/// Corresponds to the "Unanswered variable placeholder" HotDocs setting.</description></item>
		/// <item><term>HonorCmpUnansFormat</term>
		/// <description>"true" or "false"<br/>
		/// Determines whether template author preferences (as specified in a HotDocs component file) should override the
		/// default UnansweredFormat specified by the host application. ("true" corresponds to the behavior of desktop HotDocs.)</description></item>
		/// <item><term>RequestTimeout</term>
		/// <description>(number of seconds) By default, HotDocs Cloud Services will assume a request to assemble a document has hung if the
		/// response takes longer than (a certain time). Setting RequestTimeout to another value can raise or lower the
		/// threshhold before HotDocs gives up on a request. For performance reasons, HotDocs Cloud Services may ignore
		/// values over a certain threshhold.</description></item>
		/// <item><term>MaxRepeatCount</term>
		/// <description>e.g. "100"<br/>
		/// The number of iterations through a WHILE loop HotDocs will perform before generating an error.
		/// For performance reasons, HotDocs Cloud Services may ignore values over a certain threshhold.</description></item>
		/// <item><term>MaxRecursionDepth</term>
		/// <description>e.g. "100"<br/>
		/// The maximum depth that may be reached by a recursive script in HotDocs before generating an error.
		/// For performance reasons, HotDocs Cloud Services may ignore values over a certain threshhold.</description></item>
		/// <item><term>AssembleMarkedDocuments</term>
		/// <description>"true" or "false"<br/>
		/// Dictates whether this document will be assembled traditionally ("false") or using markup view ("true").</description></item>
		/// </list>
		/// When you do not explicitly provide values for any setting, the default values specified
		/// in the HotDocs Cloud Services Administrative Portal will be used.
		/// </summary>
		public Dictionary<string, string> Settings
		{
			get
			{
				return _settings;
			}
		}

		/* ----------------------------------------------------------------------------------------
		 * some previously-included or discussed properties that have been intentionally omitted:
		 */

		//public int MaxWhileCount { get; set; } // defaults to Default (only changeable for CS)
		//public int MaxRecursionDepth { get; set; } // defaults to Default (only changeable for CS)
	}

}
