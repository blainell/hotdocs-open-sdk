/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments where missing.
//TODO: Add method parameter validation.
//TODO: Add appropriate unit tests.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HotDocs.Sdk.Server.Contracts;
using hdsi = HotDocs.Server.Interop;

namespace HotDocs.Sdk.Server.Local
{
	public class Services : IServices
	{
		const int READ_BUF_SIZE = 0x10000;

		private HotDocs.Server.Application _app;
		private string _tempPath = null;

		public Services(string tempPath)
		{
			_app = new HotDocs.Server.Application();
			_tempPath = tempPath;
		}

		#region IServices implementation

		public ComponentInfo GetComponentInfo(Template template, bool includeDialogs, string logRef)
		{
			if (string.IsNullOrWhiteSpace(logRef))
				throw new ArgumentNullException("logRef", @"Local.Services.GetInterviewDefinition: the ""logRef"" parameter pased in was null or empty");
			// Validate input parameters, creating defaults as appropriate.
			if (template == null)
				throw new ArgumentNullException("template", @"Local.Services.GetInterviewDefinition: the ""template"" parameter passed in was null or empty, logRef: " + logRef);

			string templateFilePath = template.GetFullPath();

			//Get the hvc path. GetInterviewInformation will also validate the template file name.
			string hvcPath, interviewPath;
			GetInterviewInformation(templateFilePath, out hvcPath, out interviewPath);
			if (hvcPath.Length == 0)
				throw new Exception("Invalid templateID");
			if (!File.Exists(hvcPath))
				throw new HotDocs.Server.HotDocsServerException("You do not have read access to the variable collection file (.hvc)");

			// Instantiate a ComponentInfo object to hold the data to be returned
			ComponentInfo cmpInfo = new ComponentInfo();

			// Load the HVC variable collection
			using (HotDocs.Server.VariableCollection varCol = new HotDocs.Server.VariableCollection(hvcPath))
			{
				for (int i = 0; i < varCol.Count; i++)
					cmpInfo.AddVariable(new Contracts.VariableInfo { Name = varCol.VarName(i), Type = AnsTypeToString(varCol.VarType(i)) });
			}

			// Load the Dialogs (if requested)
			if (includeDialogs)
			{
				// Load the component collection from disk
				using (var cmpColl = new HotDocs.Server.ComponentCollection())
				{
					cmpColl.Open(templateFilePath, hdsi.CMPOpenOptions.LoadAllCompLibs);
					// Iterate through the component collection and add dialogs to ComponentInfo
					foreach (HotDocs.Server.Component cmp in cmpColl)
					{
						if (cmp.Type == hdsi.hdCmpType.hdDialog)
						{
							// Fetch dialog properties (AnswerSource, Repeat, variable list and mappingNames) from the component collection
							object[] variableNames = null;
							object[] mappingNames = null;
							DialogInfo dlgInfo = new DialogInfo();
							dlgInfo.Name = cmp.Name;

							using (HotDocs.Server.ComponentProperties properties = cmp.Properties)
							{
								//TODO: Use more descriptive variable names.
								using (HotDocs.Server.ComponentProperty p = (HotDocs.Server.ComponentProperty)properties["Variables"])
								{
									variableNames = (object[])p.Value;
								};
								using (HotDocs.Server.ComponentProperty p = (HotDocs.Server.ComponentProperty)properties["IsRepeated"])
								using (HotDocs.Server.ComponentProperty p2 = (HotDocs.Server.ComponentProperty)properties["IsSpread"])
								{
									dlgInfo.Repeat = (bool)p.Value || (bool)p2.Value;
								};
								using (HotDocs.Server.ComponentProperty p = (HotDocs.Server.ComponentProperty)properties["AnswerSource"])
								{
									dlgInfo.AnswerSource = ((p != null) && (!String.IsNullOrWhiteSpace((string)p.Value))) ? (string)p.Value : null;
								};
								if (dlgInfo.AnswerSource != null)
								{
									using (HotDocs.Server.ComponentProperty p = (HotDocs.Server.ComponentProperty)properties["MappingNames"])
									{
										mappingNames = (object[])p.Value;
									};
								}
							}

							// Add the mapping information to the dialog
							//TODO: Note that i < variableNames.Length, but i indexes mappingNames. Do the lengths of the two lists match?
							for (int i = 0; i < variableNames.Length; i++)
							{
								string variableName = variableNames[i].ToString();
								if (cmpInfo.IsDefinedVariable(variableName))
								{
									string mappingName = (mappingNames != null) ? mappingNames[i].ToString() : null;
									dlgInfo.Items.Add(new DialogItemInfo
									{
										Name = variableName,
										Mapping = String.IsNullOrWhiteSpace(mappingName) ? null : mappingName
									});
								}
							}

							// Adds the dialog to ComponentInfo object IF it contains any variables that were in the HVC
							if (dlgInfo.Items.Count > 0)
								cmpInfo.AddDialog(dlgInfo);
						}
					}
				}
			}
			return cmpInfo;
		}

		public InterviewResult GetInterview(Template template, TextReader answers, InterviewSettings settings, IEnumerable<string> markedVariables, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
			if (string.IsNullOrWhiteSpace(logRef))
				throw new ArgumentNullException("logRef", @"Local.Services.GetInterview: the ""logRef"" parameter passed in was null or empty");
			if (template == null)
				throw new ArgumentNullException("template", string.Format(@"Local.Services.GetInterview: the ""template"" parameter passed in was null, logRef: {0}", logRef));

			if (settings == null)
				settings = new InterviewSettings();

			// Add the query string to the interview image url so dialog element images can be located.
			settings.InterviewImageUrlQueryString = "?loc=" + template.CreateLocator() + "&img=";

			// HotDocs Server reads the following settings out of the registry all the time; therefore these items are ignored when running against Server:
			//		settings.AddHdMainDiv
			//		settings.AnswerSummary.*
			//		settings.DefaultDateFormat
			//		settings.DefaultUnansweredFormat
			//		settings.HonorCmpUnansweredFormat
			//		settings.DisableAnswerSummary

			// HotDocs Server does not include the following settings in its .NET or COM APIs, so Util.AppendSdkScriptBlock (below)
			// includes them with the interview script block:
			//		settings.Locale
			//		settings.NextFollowsOutline
			//		settings.ShowAllResourceButtons

			hdsi.interviewFormat fmt;
			switch (settings.Format)
			{
				case InterviewFormat.JavaScript:
					fmt = hdsi.interviewFormat.javascript;
					break;
				case InterviewFormat.Silverlight:
					fmt = hdsi.interviewFormat.Silverlight;
					break;
				default:
					fmt = hdsi.interviewFormat.Unspecified;
					break;
			}

			// Configure the interview options
			hdsi.HDInterviewOptions itvOpts = hdsi.HDInterviewOptions.intOptNoImages; // Instructs HDS not to return images used by the interview; we'll get them ourselves from the template folder.

			// TODO: Application interface provides no way to request the "server default" for each of the following option flags.
			// That would require changing the .NET (and probably COM) interfaces
			if (settings.DisableDocumentPreview == Tristate.True)
				itvOpts |= hdsi.HDInterviewOptions.intOptNoPreview; // Disables (omits) the Document Preview button on the interview toolbar. TODO: Fix TFS #5598 so this actually has an effect.
			if (settings.DisableSaveAnswers == Tristate.True)
				itvOpts |= hdsi.HDInterviewOptions.intOptNoSave; // Disables (omits) the Save Answers button on the interview toolbar. TODO: Fix TFS #5598 so this actually has an effect.
			if (settings.RoundTripUnusedAnswers == Tristate.True)
				itvOpts |= hdsi.HDInterviewOptions.intOptStateless; // Prevents original answer file from being encrypted and sent to the interview and then posted back at the end.

			// Get the interview.
			InterviewResult result = new InterviewResult();
			//TODO: Remove the temp folder.
			TempFolder tempFolder = null;

			try
			{
				StringBuilder htmlFragment;
				using (var ansColl = new HotDocs.Server.AnswerCollection())
				{
					if (answers != null)
					{
						if (answers.Peek() == 0xFEFF)
							answers.Read(); // discard BOM if present
						ansColl.XmlAnswers = answers.ReadToEnd();
					}

					if (markedVariables == null)
						_app.UnansweredVariablesList = new string[0];
					else
						_app.UnansweredVariablesList = markedVariables;

					htmlFragment = new StringBuilder(
						_app.GetInterview(
							template.GetFullPath(),
							template.Key,
							fmt,
							itvOpts,
							settings.InterviewRuntimeUrl,
							settings.StyleSheetUrl + "/" + settings.ThemeName + ".css",
							ansColl,
							settings.PostInterviewUrl,
							settings.Title,
							settings.InterviewDefinitionUrl,
							tempFolder != null ? tempFolder.Path : null, // the path to which HDS should copy interview images; also the path that may become part of the DocumentPreviewStateString & passed to document preview handler
							settings.InterviewImageUrl,
							settings.DisableSaveAnswers != Tristate.True ? settings.SaveAnswersUrl : "", // TODO: After TFS #5598 is fixed, we can go back to just setting the Url here and let HDS do the work of determining whether to use the url or not.
							settings.DisableDocumentPreview != Tristate.True ? settings.DocumentPreviewUrl : "") // TODO: Fix up after TFS #5598 is fixed (as above).
						);
				}
				Util.AppendSdkScriptBlock(htmlFragment, template, settings);

				result.HtmlFragment = htmlFragment.ToString();
			}
			finally
			{
				if (tempFolder != null)
					tempFolder.Dispose();
			}
			return result;
		}

		public Stream GetInterviewDefinition(string state, string templateFile, InterviewFormat format)
		{
			// Validate input parameters, creating defaults as appropriate.
			if (string.IsNullOrEmpty(state))
				throw new ArgumentNullException("state", @"Local.Services.GetInterviewDefinition: the ""state"" parameter passed in was null or empty");

			if (string.IsNullOrEmpty(templateFile))
				throw new ArgumentNullException("templateFile", @"Local.Services.GetInterviewDefinition: the ""templateFile"" parameter passed in was null or empty");

			string interviewDefPath = _app.GetInterviewDefinitionFromState(state, templateFile,
				  format == InterviewFormat.Silverlight
					? hdsi.interviewFormat.Silverlight
					: hdsi.interviewFormat.javascript);
			return File.OpenRead(interviewDefPath);
		}
		//TODO: Move this to the private area.
		private HotDocs.Server.OutputOptions ConvertOutputOptions(OutputOptions sdkOpts)
		{
			HotDocs.Server.OutputOptions hdsOpts = null;
			if (sdkOpts is PdfOutputOptions)
			{
				PdfOutputOptions sdkPdfOpts = (PdfOutputOptions)sdkOpts;
				HotDocs.Server.PdfOutputOptions hdsPdfOpts = new HotDocs.Server.PdfOutputOptions();

				hdsPdfOpts.Author = sdkPdfOpts.Author;
				hdsPdfOpts.Comments = sdkPdfOpts.Comments;
				hdsPdfOpts.Company = sdkPdfOpts.Company;
				hdsPdfOpts.Keywords = sdkPdfOpts.Keywords;
				hdsPdfOpts.Subject = sdkPdfOpts.Subject;
				hdsPdfOpts.Title = sdkPdfOpts.Title;

				//Remember that these are PDF passwords which are not highly secure.
				hdsPdfOpts.OwnerPassword = sdkPdfOpts.OwnerPassword;
				hdsPdfOpts.UserPassword = sdkPdfOpts.UserPassword;

				hdsi.PdfOutputFlags hdsFlags = 0;
				if (sdkPdfOpts.EmbedFonts == Tristate.True)//TODO: What do we do in the case of Default.
					hdsFlags |= hdsi.PdfOutputFlags.pdfOut_EmbedFonts;
				if (sdkPdfOpts.KeepFillablePdf)
					hdsFlags |= hdsi.PdfOutputFlags.pdfOut_KeepFillablePdf;
				if (sdkPdfOpts.PdfA)
					hdsFlags |= hdsi.PdfOutputFlags.pdfOut_PdfA;
				if (sdkPdfOpts.TaggedPdf)
					hdsFlags |= hdsi.PdfOutputFlags.pdfOut_TaggedPdf;
				if (sdkPdfOpts.TruncateFields)
					hdsFlags |= hdsi.PdfOutputFlags.pdfOut_TruncateFields;
				hdsPdfOpts.PdfOutputFlags = hdsFlags;

				hdsi.PdfPermissions hdsPerm = 0;
				if (sdkPdfOpts.Permissions.HasFlag(PdfPermissions.Copy))
					hdsPerm |= hdsi.PdfPermissions.COPY;
				if (sdkPdfOpts.Permissions.HasFlag(PdfPermissions.Modify))
					hdsPerm |= hdsi.PdfPermissions.MOD;
				if (sdkPdfOpts.Permissions.HasFlag(PdfPermissions.Print))
					hdsPerm |= hdsi.PdfPermissions.PRINT;
				hdsPdfOpts.PdfPermissions = hdsPerm;

				hdsOpts = hdsPdfOpts;
			}
			else if (sdkOpts is HtmlOutputOptions)
			{
				HtmlOutputOptions sdkHtmOpts = (HtmlOutputOptions)sdkOpts;
				HotDocs.Server.HtmlOutputOptions hdsHtmOpts = new HotDocs.Server.HtmlOutputOptions();

				hdsHtmOpts.Author = sdkHtmOpts.Author;
				hdsHtmOpts.Comments = sdkHtmOpts.Comments;
				hdsHtmOpts.Company = sdkHtmOpts.Company;
				hdsHtmOpts.Keywords = sdkHtmOpts.Keywords;
				hdsHtmOpts.Subject = sdkHtmOpts.Subject;
				hdsHtmOpts.Title = sdkHtmOpts.Title;

				hdsHtmOpts.Encoding = sdkHtmOpts.Encoding;

				hdsOpts = hdsHtmOpts;
			}
			else if (sdkOpts is TextOutputOptions)
			{
				TextOutputOptions sdkTxtOpts = (TextOutputOptions)sdkOpts;
				HotDocs.Server.TextOutputOptions hdsTxtOpts = new HotDocs.Server.TextOutputOptions();
				hdsTxtOpts.Encoding = sdkTxtOpts.Encoding;
				hdsOpts = hdsTxtOpts;
			}

			return hdsOpts;
		}

		public AssembleDocumentResult AssembleDocument(Template template, TextReader answers, AssembleDocumentSettings settings, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
			if (string.IsNullOrWhiteSpace(logRef))
				throw new ArgumentNullException("logRef", @"Local.Services.AssembleDocument: the ""logRef"" parameter pased in was null or empty");
			if (template == null)
				throw new ArgumentNullException("template", "Local.Services.AssembleDocument: The template must not be null, logRef: " + logRef);

			if (settings == null)
				settings = new AssembleDocumentSettings();

			HotDocs.Server.AnswerCollection ansColl = new HotDocs.Server.AnswerCollection();
			ansColl.OverlayXMLAnswers(answers == null ? "" : answers.ReadToEnd());
			HotDocs.Server.OutputOptions outputOptions = ConvertOutputOptions(settings.OutputOptions);

			//TODO: Make sure files get cleaned up.
			string docPath = CreateTempDocDirAndPath(template, settings.Format);

			//TODO: Review this.
			int savePendingAssembliesCount = _app.PendingAssemblyCmdLineStrings.Count;

			_app.AssembleDocument(
				template.GetFullPath(),//Template path
				hdsi.HDAssemblyOptions.asmOptMarkupView,
				ansColl,
				docPath,
				outputOptions);

			//Prepare the post-assembly answer set.
			HotDocs.Sdk.AnswerCollection resultAnsColl = new AnswerCollection();
			resultAnsColl.ReadXml(new StringReader(ansColl.XmlAnswers));
			if (!settings.RetainTransientAnswers && _app.PendingAssemblyCmdLineStrings.Count == 0)
			{
				//Discard transient answers.
				IEnumerable<Answer> transAnswers = from a in resultAnsColl where !a.Save select a;
				foreach (Answer ans in transAnswers)
					resultAnsColl.RemoveAnswer(ans.Name);
			}

			//Prepare the image information for the browser.
			DocumentType docType = settings.Format;
			List<NamedStream> supportingFiles = new List<NamedStream>();

			if (docType == DocumentType.Native)
			{
				docType = Document.GetDocumentType(docPath);
			}
			else if (docType == DocumentType.HTMLwDataURIs)
			{
				File.WriteAllText(docPath, Util.EmbedImagesInURIs(docPath));  // Overwrite .html file.  If the consumer requested both HTML and HTMLwDataURIs, they'll only get the latter.
			}
			else if (docType == DocumentType.MHTML)
			{

				string mhtmlFilePath = Path.Combine(Path.GetDirectoryName(docPath), Path.GetFileNameWithoutExtension(docPath) + ".mhtml");
				using (StreamWriter htmFile = File.CreateText(mhtmlFilePath))
				{
					htmFile.Write(Util.HtmlToMultiPartMime(docPath));
				}
			}
			else if (docType == DocumentType.HTML)
			{
				string targetFilenameNoExtention = Path.GetFileNameWithoutExtension(docPath);
				foreach (string img in Directory.EnumerateFiles(Path.GetDirectoryName(docPath)))
				{
					string ext = Path.GetExtension(img).ToLower();
					if (Path.GetFileName(img).StartsWith(targetFilenameNoExtention) && (ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".png" || ext == ".bmp"))
						supportingFiles.Add(LoadFileIntoNamedStream(img));
				}
			}

			//Prepare the unanswered variables list for the assembly result.
			//TODO: Just build an array from _app.UnansweredVariablesList???
			List<string> list = new List<string>();
			foreach (string unans in _app.UnansweredVariablesList)
				list.Add(unans);
			string[] unansweredVariables = list.ToArray();

			//Build the list of pending assemblies.
			List<Template> pendingAssemblies = new List<Template>();
			for (int i = savePendingAssembliesCount; i < _app.PendingAssemblyCmdLineStrings.Count; i++)
			{
				string cmdLine = _app.PendingAssemblyCmdLineStrings[i];
				string path, switches;
				Util.ParseHdAsmCmdLine(cmdLine, out path, out switches);
				pendingAssemblies.Add(new Template(Path.GetFileName(path), template.Location.Duplicate(), switches));
			}

			FileStream stream = File.OpenRead(docPath);
			Document document = new Document(template, stream, docType, supportingFiles.ToArray(), unansweredVariables);
			AssembleDocumentResult result = new AssembleDocumentResult(document, resultAnsColl.XmlAnswers, pendingAssemblies.ToArray(), unansweredVariables);

			return result;
		}

		public string GetAnswers(IEnumerable<TextReader> answers, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
			if (string.IsNullOrWhiteSpace(logRef))
				throw new ArgumentNullException("logRef", @"Local.Services.GetAnswers: the ""logRef"" parameter pased in was null or empty");
			if (answers == null)
				throw new ArgumentNullException("answers", @"Local.Services.GetAnswers: The ""answers"" parameter must not be null, logRef: " + logRef);

			string result = "";
			using (HotDocs.Server.AnswerCollection hdsAnsColl = new HotDocs.Server.AnswerCollection())
			{
				foreach (TextReader tr in answers)
					hdsAnsColl.OverlayXMLAnswers(tr.ReadToEnd());
				result = hdsAnsColl.XmlAnswers;
			}
			return result;
		}

		public void BuildSupportFiles(Template template, HDSupportFilesBuildFlags flags)
		{
			if (template == null)
				throw new ArgumentNullException("template", @"Local.Services.BuildSupportFiles: the ""template"" parameter passed in was null");
			using (HotDocs.Server.Application app = new HotDocs.Server.Application())
			{
				hdsi.HDServerBuildFlags hdBuildFlags = 0;
				if ((flags & HDSupportFilesBuildFlags.BuildJavaScriptFiles) != 0)
					hdBuildFlags |= hdsi.HDServerBuildFlags.BuildJavaScriptFiles;
				if ((flags & HDSupportFilesBuildFlags.BuildSilverlightFiles) != 0)
					hdBuildFlags |= hdsi.HDServerBuildFlags.BuildSilverlightFiles;
				if ((flags & HDSupportFilesBuildFlags.ForceRebuildAll) != 0)
					hdBuildFlags |= hdsi.HDServerBuildFlags.ForceRebuildAll;
				if ((flags & HDSupportFilesBuildFlags.IncludeAssembleTemplates) != 0)
					hdBuildFlags |= hdsi.HDServerBuildFlags.IncludeAssembleTemplates;

				app.BuildSupportFiles(template.GetFullPath(), template.Key, hdBuildFlags);
			}
		}

		public void RemoveSupportFiles(Template template)
		{
			if (template == null)
				throw new ArgumentNullException("template", @"Local.Services.RemoveSupportFiles: the ""template"" parameter passed in was null");
			using (HotDocs.Server.Application app = new HotDocs.Server.Application())
			{
				app.RemoveSupportFiles(template.GetFullPath(), template.Key);
			}
		}

		#endregion
		//TODO: Move this to HotDocs.Sdk.Template?
		private string GetDocExtension(Template template, DocumentType docType)
		{
			string ext = "";
			switch (docType)
			{
				case DocumentType.HFD:
					ext = ".hfd";
					break;
				case DocumentType.HPD:
					ext = ".hpd";
					break;
				case DocumentType.HTML:
					ext = ".htm";
					break;
				case DocumentType.HTMLwDataURIs:
					ext = ".htm";
					break;
				case DocumentType.MHTML:
					ext = ".htm";
					break;
				case DocumentType.Native:
					{
						string templateExt = Path.GetExtension(template.FileName);
						if (templateExt == ".hpt")
							ext = ".pdf";
						else if (templateExt == ".ttx")
							ext = ".txt";
						else if (templateExt == ".wpt")
							ext = ".wpd";
						else
							ext = templateExt;
						break;
					}
				case DocumentType.PDF:
					ext = ".pdf";
					break;
				case DocumentType.PlainText:
					ext = ".txt";
					break;
				//DOC isn't supported because DOC files aren't generated on a server.
				//case DocumentType.WordDOC:
				//	ext = ".doc";
				//	break;
				case DocumentType.WordDOCX:
					ext = ".docx";
					break;
				case DocumentType.WordPerfect:
					ext = ".wpd";
					break;
				case DocumentType.WordRTF:
					ext = ".rtf";
					break;
				//TODO: Make sure all values are properly handled. XML is missing here.
				default:
					throw new Exception("Unsupported document type.");
			}
			return ext;
		}

		private string CreateTempDocDirAndPath(Template template, DocumentType docType)
		{
			//TODO: Don't re-use fullPath.
			//TODO: Make sure the created files and folders get cleaned up.
			string fullPath;
			string ext = GetDocExtension(template, docType);
			do
			{
				fullPath = Path.Combine(_tempPath, Path.GetRandomFileName());
			} while (Directory.Exists(fullPath));
			Directory.CreateDirectory(fullPath);
			fullPath = Path.Combine(fullPath, Path.GetRandomFileName() + ext);
			using (File.Create(fullPath)) { }
			return fullPath;
		}

		//TODO: This is not used.
		private void FreeTempDocDir(string docPath)
		{
			string dir = Path.GetDirectoryName(docPath);
			Directory.Delete(dir, true);
		}

		private Stream LoadFileIntoMemStream(string filePath)
		{
			MemoryStream memStream = new MemoryStream();
			using (FileStream fs = File.OpenRead(filePath))
			{
				byte[] buffer = new byte[READ_BUF_SIZE];
				int offset = 0, bytesRead = 0;
				do
				{
					bytesRead = fs.Read(buffer, offset, READ_BUF_SIZE);
					if (bytesRead > 0)
						memStream.Write(buffer, 0, bytesRead);
					offset += bytesRead;
				} while (bytesRead == READ_BUF_SIZE);
			}
			return memStream;
		}

		private NamedStream LoadFileIntoNamedStream(string filePath)
		{
			return new NamedStream(filePath, LoadFileIntoMemStream(filePath));
		}

		private const string c_templateIDRegEx = @"^[^\\/:*?""<>|\r\n]+\.(?:docx|rtf|wpt|hpt|hft|ttx|cmp){1}\z";

		private static bool CheckTemplateId(string val)
		{
			return (val != null
				&& Regex.IsMatch(val, c_templateIDRegEx, RegexOptions.IgnoreCase));
		}
		//TODO: Get rid of interviewPath???
		private static void GetInterviewInformation(string templateFilePath, out string hvcPath, out string interviewPath)
		{
			string templateID = Path.GetFileName(templateFilePath);

			if (!CheckTemplateId(templateID))
				throw new HotDocs.Server.HotDocsServerException("Invalid template ID");

			//calculate the interview path and hvc path. These files should be in the same directory
			//and have the same name as the template file. They should only differ by extension.
			interviewPath = System.IO.Path.ChangeExtension(templateFilePath, ".js");
			hvcPath = System.IO.Path.ChangeExtension(templateFilePath, ".hvc");
		}

		private static string AnsTypeToString(hdsi.ansType type)
		{
			switch (type)
			{
				case hdsi.ansType.ansTypeText:
					return "Text";
				case hdsi.ansType.ansTypeNumber:
					return "Number";
				case hdsi.ansType.ansTypeDate:
					return "Date";
				case hdsi.ansType.ansTypeTF:
					return "True/False";
				case hdsi.ansType.ansTypeMC:
					return "Multiple Choice";
				//TODO: Do we need DocText support?
				default:
					return "Unknown";
			}
		}
	}
}
