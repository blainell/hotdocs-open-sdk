/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HotDocs.Sdk
{
	/// <summary>
	/// These flags specify which portions of a template manifest (or manifests) should be parsed (when calling ParseManifest).
	/// </summary>
	[Flags]
	public enum ManifestParseFlags
	{
		/// <summary>
		/// Find the basic metadata about a template: its version, title, description, effective component file, expiration date, etc.
		/// </summary>
		ParseTemplateInfo		= 0x0001,
		/// <summary>
		/// Find the variables that are used by the template or its interview.
		/// </summary>
		ParseVariables			= 0x0002,
		/// <summary>
		/// Find the dependencies this template has on other templates, component files, images, etc.
		/// </summary>
		ParseDependencies		= 0x0004,
		/// <summary>
		/// Find additional dependencies this template may have on other files that are not typically managed by HotDocs.
		/// </summary>
		ParseAdditionalFiles	= 0x0008,
		/// <summary>
		/// Find the answer sources used in this template.
		/// </summary>
		ParseDataSources		= 0x0010,
		/// <summary>
		/// When this flag is set, information will be compiled by recursively parsing the manifests for this template
		/// and any templates inserted by this one as well.
		/// </summary>
		ParseRecursively		= 0x0020,
		/// <summary>
		/// Reads and parses all available information in the manifest.
		/// </summary>
		ParseAll				= ParseVariables|ParseDependencies|ParseAdditionalFiles|ParseDataSources
	};

	public enum DataSourceFieldType
	{
		Text,
		Number,
		Date,
		TrueFalse
	};

	public enum DataSourceBackfillType
	{
		Never,
		Always,
		Prompt,
		DoNotAllow
	};

	public enum DataSourceType
	{
		CurrentAnswerFile,
		AnswerFile,
		DatabaseComponent,
		Custom
	};

	public class VariableInfo : IEquatable<VariableInfo>, IComparable<VariableInfo>
	{
		private string _key;

		internal VariableInfo(string name, ValueType valueType)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			Name = name;
			Type = valueType;

			_key = Name + Type.ToString();
		}

		public string Name { get; private set; }

		public ValueType Type { get; private set; }
		
		public override bool Equals(object obj)
		{
			return (obj != null) && (obj is VariableInfo) && Equals((VariableInfo)obj);
		}

		public override int GetHashCode()
		{
			return _key.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("Name: {0}  AnswerType: {1}", Name, Type);
		}

		#region IEquatable<Variable> Members

		public bool Equals(VariableInfo other)
		{
			return CompareTo(other) == 0;
		}

		#endregion

		#region IComparable<Variable> Members

		public int CompareTo(VariableInfo other)
		{
			return string.CompareOrdinal(other._key, _key);
		}

		#endregion
	}

	public class AdditionalFile : IEquatable<AdditionalFile>, IComparable<AdditionalFile>
	{
		internal AdditionalFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");

			FileName = fileName;
		}

		public string FileName { get; private set; }

		public override bool Equals(object obj)
		{
			return (obj != null) && (obj is AdditionalFile) && Equals((AdditionalFile)obj);
		}

		public override int GetHashCode()
		{
			return FileName.ToLower().GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("FileName: {0}", FileName);
		}

		#region IEquatable<AdditionalFile> Members

		public bool Equals(AdditionalFile other)
		{
			return CompareTo(other) == 0;
		}

		#endregion

		#region IComparable<AdditionalFile> Members

		public int CompareTo(AdditionalFile other)
		{
			return string.Compare(other.FileName, FileName, true);
		}

		#endregion
	}

	public class DataSourceField
	{
		internal DataSourceField(string sourceName, DataSourceFieldType fieldType, DataSourceBackfillType backfillType, bool isKey)
		{
			if (string.IsNullOrEmpty(sourceName))
				throw new ArgumentNullException("sourceName");

			SourceName = sourceName;
			FieldType = fieldType;
			BackfillType = backfillType;
			IsKey = isKey;
		}

		public string SourceName { get; private set; }

		public DataSourceFieldType FieldType { get; private set; }

		public DataSourceBackfillType BackfillType { get; private set; }

		public bool IsKey { get; private set; }
	}

	public class DataSource : IEquatable<DataSource>, IComparable<DataSource>
	{
		internal DataSource(string id, string name, DataSourceType type, DataSourceField[] fields)
		{
			if (string.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			Id = id;
			Name = name;
			Type = type;
			Fields = fields;
		}

		public string Id { get; private set; }

		public string Name { get; private set; }

		public DataSourceType Type { get; private set; }

		public DataSourceField[] Fields { get; private set; }

		public override bool Equals(object obj)
		{
			return (obj != null) && (obj is DataSource) && Equals((DataSource)obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("Id: {0}  Name: {1}  Type: {2}", Id, Name, Type);
		}

		#region IEquatable<DataSource> Members

		public bool Equals(DataSource other)
		{
			return CompareTo(other) == 0;
		}

		#endregion

		#region IComparable<DataSource> Members

		public int CompareTo(DataSource other)
		{
			return string.CompareOrdinal(other.Id, Id);
		}

		#endregion
	}

	public class TemplateManifest
	{
		private static readonly XNamespace s_namespace = "http://www.hotdocs.com/schemas/template_manifest/2012";

		private struct TemplateFilePath : IEquatable<TemplateFilePath>
		{
			private string _filePath;

			internal TemplateFilePath(string filePath)
			{
				_filePath = filePath;
			}

			internal string FilePath
			{
				get { return _filePath; }
			}

			public override bool Equals(object obj)
			{
				return (obj != null) && (obj is TemplateFilePath) && Equals((TemplateFilePath)obj);
			}

			public override int GetHashCode()
			{
				return _filePath.ToLower().GetHashCode();
			}

			#region IEquatable<TemplatePath> Members

			public bool Equals(TemplateFilePath other)
			{
				return string.Equals(other.FilePath, FilePath, StringComparison.OrdinalIgnoreCase);
			}

			#endregion
		}

		internal TemplateManifest() {}

		public string HotDocsVersion { get; private set; }

		public string TemplateId { get; private set; }

		public string FileName { get; private set; }

		public string EffectiveCmpFileName { get; private set; }

		public DateTime? ExpirationDate { get; private set; }

		public int? WarningDays { get; private set; }

		public int? ExtensionDays { get; private set; }

		public string Title { get; private set; }

		public string Description { get; private set; }

		public VariableInfo[] Variables { get; private set; }

		public Dependency[] Dependencies { get; private set; }

		public AdditionalFile[] AdditionalFiles { get; private set; }

		public DataSource[] DataSources { get; private set; }

		// TODO: Condider re-writing this using an XmlReader so that the entire document does not need to be allocated in a DOM. Doing so
		// should make processing template manifest files faster. For now using an XDocument to just get things done fast.
		public static TemplateManifest ParseManifest(string templatePath, ManifestParseFlags parseFlags)
		{
			TemplateManifest templateManifest = new TemplateManifest();
			TemplateFilePath baseTemplatePath = new TemplateFilePath(templatePath);

			HashSet<VariableInfo> variables = GetHashSet<VariableInfo>(parseFlags, ManifestParseFlags.ParseVariables);
			HashSet<Dependency> dependencies = GetHashSet<Dependency>(parseFlags, ManifestParseFlags.ParseDependencies);
			HashSet<AdditionalFile> additionalFiles = GetHashSet<AdditionalFile>(parseFlags, ManifestParseFlags.ParseAdditionalFiles);
			HashSet<DataSource> dataSources = GetHashSet<DataSource>(parseFlags, ManifestParseFlags.ParseDataSources);

			Queue<TemplateFilePath> templateQueue = new Queue<TemplateFilePath>();
			HashSet<TemplateFilePath> processedTemplates = new HashSet<TemplateFilePath>();

			// Add the base template to the queue
			templateQueue.Enqueue(baseTemplatePath);

			while (templateQueue.Count > 0)
			{
				TemplateFilePath templateFilePath = templateQueue.Dequeue();
				if (!processedTemplates.Contains(templateFilePath))
				{
					try
					{
						// Read the template manifest so that it can be parsed.
						XDocument manifest = XDocument.Load(GetManifestPath(templateFilePath.FilePath), LoadOptions.None);

						if (templateFilePath.Equals(baseTemplatePath))
						{
							// Process the root templateManifest element.
							templateManifest.HotDocsVersion = manifest.Root.Attribute("hotdocsVersion").Value;
							templateManifest.TemplateId = manifest.Root.Attribute("templateId").Value;
							templateManifest.FileName = manifest.Root.Attribute("fileName").Value;
							templateManifest.EffectiveCmpFileName = manifest.Root.Attribute("effectiveCmpFile").Value;

							if (manifest.Root.Attribute("expirationDate") != null)
							{
								templateManifest.ExpirationDate = DateTime.Parse(manifest.Root.Attribute("expirationDate").Value);

								if (manifest.Root.Attribute("warningDays") != null)
								{
									templateManifest.WarningDays = int.Parse(manifest.Root.Attribute("warningDays").Value);
								}

								if (manifest.Root.Attribute("extensionDays") != null)
								{
									templateManifest.ExtensionDays = int.Parse(manifest.Root.Attribute("extensionDays").Value);
								}
							}

							var titleElem = manifest.Root.Element(s_namespace + "title");
							templateManifest.Title = (titleElem != null) ? titleElem.Value : string.Empty;

							var descriptionElem = manifest.Root.Element(s_namespace + "description");
							templateManifest.Description = (descriptionElem != null) ? descriptionElem.Value : string.Empty;
						}

						if (variables != null)
						{
							// Process the variables element.
							var variablesElem = manifest.Root.Element(s_namespace + "variables");
							if (variablesElem != null)
							{
								// Add any not yet encountered variable to the variables collection.
								foreach (var variableElem in variablesElem.Elements(s_namespace + "variable"))
								{
									ValueType valueType;
									switch (variableElem.Attribute("type").Value)
									{
										case "text":
											valueType = ValueType.Text;
											break;
										case "number":
											valueType = ValueType.Number;
											break;
										case "date":
											valueType = ValueType.Date;
											break;
										case "trueFalse":
											valueType = ValueType.TrueFalse;
											break;
										case "multipleChoice":
											valueType = ValueType.MultipleChoice;
											break;
										default:
											valueType = ValueType.Unknown;
											break;
									}
									variables.Add(new VariableInfo(variableElem.Attribute("name").Value, valueType));
								}
							}
						}

						if (dependencies != null)
						{
							// Process the dependencies element.
							var dependenciesElem = manifest.Root.Element(s_namespace + "dependencies");
							if (dependenciesElem != null)
							{
								// Add any not yet encountered dependency to the dependencies collection.
								foreach (var dependencyElem in dependenciesElem.Elements(s_namespace + "dependency"))
								{
									string hintPath = (dependencyElem.Attribute("hintPath") != null) ? dependencyElem.Attribute("hintPath").Value : null;

									DependencyType dependencyType;
									switch (dependencyElem.Attribute("type").Value)
									{
										case "baseCmpFile":
											dependencyType = DependencyType.BaseCmpFile;
											break;
										case "pointedToCmpFile":
											dependencyType = DependencyType.PointedToCmpFile;
											break;
										case "templateInsert":
											dependencyType = DependencyType.TemplateInsert;
											break;
										case "clauseInsert":
											dependencyType = DependencyType.ClauseInsert;
											break;
										case "clauseLibraryInsert":
											dependencyType = DependencyType.ClauseLibraryInsert;
											break;
										case "imageInsert":
											dependencyType = DependencyType.ImageInsert;
											break;
										case "interviewImage":
											dependencyType = DependencyType.InterviewImage;
											break;
										case "assemble":
											dependencyType = DependencyType.Assemble;
											break;
										case "publisherMapFile":
											dependencyType = DependencyType.PublisherMapFile;
											break;
										case "userMapFile":
											dependencyType = DependencyType.UserMapFile;
											break;
										case "additionalTemplate":
											dependencyType = DependencyType.AdditionalTemplate;
											break;
										default:
											throw new Exception(string.Format("Invalid dependency type '{0}'.", dependencyElem.Attribute("type").Value));
									}
									dependencies.Add(new Dependency(dependencyElem.Attribute("fileName").Value, hintPath, dependencyType));
								}
							}
						}

						if (additionalFiles != null)
						{
							// Process the additionalFiles element.
							var additionalFilesElem = manifest.Root.Element(s_namespace + "additionalFiles");
							if (additionalFilesElem != null)
							{
								// Add any not yet encountered additionalFile to the additionalFiles collection.
								foreach (var fileElem in additionalFilesElem.Elements(s_namespace + "file"))
								{
									additionalFiles.Add(new AdditionalFile(fileElem.Attribute("fileName").Value));
								}
							}
						}

						if (dataSources != null)
						{
							// Process the dataSources element.
							var dataSourcesElem = manifest.Root.Element(s_namespace + "dataSources");
							if (dataSourcesElem != null)
							{
								// Add any not yet encountered dataSource to the dataSources collection.
								foreach (var dataSourceElem in dataSourcesElem.Elements(s_namespace + "dataSource"))
								{
									DataSourceType dataSourceType;
									switch (dataSourceElem.Attribute("type").Value)
									{
										case "currentAnswerFile":
											dataSourceType = DataSourceType.CurrentAnswerFile;
											break;
										case "answerFile":
											dataSourceType = DataSourceType.AnswerFile;
											break;
										case "databaseComponent":
											dataSourceType = DataSourceType.DatabaseComponent;
											break;
										case "custom":
											dataSourceType = DataSourceType.Custom;
											break;
										default:
											throw new Exception(string.Format("Invalid data source type '{0}'.", dataSourceElem.Attribute("type").Value));
									}

									List<DataSourceField> dataSourceFields = new List<DataSourceField>();
									foreach (var dataSourceFieldElem in dataSourceElem.Elements(s_namespace + "dataSourceField"))
									{
										DataSourceFieldType fieldType;
										switch (dataSourceFieldElem.Attribute("type").Value)
										{
											case "text":
												fieldType = DataSourceFieldType.Text;
												break;
											case "number":
												fieldType = DataSourceFieldType.Number;
												break;
											case "date":
												fieldType = DataSourceFieldType.Date;
												break;
											case "trueFalse":
												fieldType = DataSourceFieldType.TrueFalse;
												break;
											default:
												throw new Exception(string.Format("Invalid data source field type '{0}'.", dataSourceFieldElem.Attribute("type").Value));
										}

										DataSourceBackfillType backfillType;
										switch (dataSourceFieldElem.Attribute("backfill").Value)
										{
											case "never":
												backfillType = DataSourceBackfillType.Never;
												break;
											case "always":
												backfillType = DataSourceBackfillType.Always;
												break;
											case "prompt":
												backfillType = DataSourceBackfillType.Prompt;
												break;
											case "doNotAllow":
												backfillType = DataSourceBackfillType.DoNotAllow;
												break;
											default:
												throw new Exception(string.Format("Invalid data source backfill type '{0}'.", dataSourceFieldElem.Attribute("backfill").Value));
										}

										bool isKey = (dataSourceFieldElem.Attribute("isKey") != null) ? Convert.ToBoolean(dataSourceFieldElem.Attribute("isKey").Value) : false;

										dataSourceFields.Add(new DataSourceField(dataSourceFieldElem.Attribute("sourceName").Value, fieldType, backfillType, isKey));
									}
									dataSources.Add(new DataSource(dataSourceElem.Attribute("id").Value, dataSourceElem.Attribute("name").Value,
										dataSourceType, dataSourceFields.ToArray()));
								}
							}
						}

						if ((parseFlags & ManifestParseFlags.ParseRecursively) == ManifestParseFlags.ParseRecursively)
						{
							// Add any referenced templates to the template queue.
							var dependenciesElem = manifest.Root.Element(s_namespace + "dependencies");
							if (dependenciesElem != null)
							{
								var templateDependencies = from d in dependenciesElem.Elements(s_namespace + "dependency")
														   let type = d.Attribute("type").Value
														   where type == "templateInsert" || type == "additionalTemplate"
														   select d;

								foreach (var templateDependency in templateDependencies)
								{
									templateQueue.Enqueue(new TemplateFilePath(GetDependencyFilePath(templatePath, 
										(templateDependency.Attribute("hintPath") != null) ? templateDependency.Attribute("hintPath").Value : null, 
										templateDependency.Attribute("fileName").Value)));
								}
							}

							// Mark the template as processed so that its manifest will not get processed again.
							processedTemplates.Add(templateFilePath);
						}
					}
					catch (Exception e)
					{
						string errorText = String.Format("Failed to read the manifest file for the template:\r\n\r\n     {0}     \r\n\r\n" +
							"The following error occurred:\r\n\r\n     {1}     ", templateFilePath.FilePath, e.Message);
						throw new Exception(errorText, e);
					}
				}
			}

			if (variables != null)
				templateManifest.Variables = variables.ToArray();

			if (dependencies != null)
				templateManifest.Dependencies = dependencies.ToArray();

			if (additionalFiles != null)
				templateManifest.AdditionalFiles = additionalFiles.ToArray();

			if (dataSources != null)
				templateManifest.DataSources = dataSources.ToArray();

			return templateManifest;
		}

		private static HashSet<T> GetHashSet<T>(ManifestParseFlags parseFlags, ManifestParseFlags flag) where T : class
		{
			return ((parseFlags & flag) == flag) ? new HashSet<T>() : null;
		}

		private static string GetManifestPath(string itemPath)
		{
			// If the passed file appears to already be a manifest file path then just use it.
			if (itemPath.EndsWith(".manifest.xml", StringComparison.OrdinalIgnoreCase))
				return itemPath;

			return Path.Combine(Path.GetDirectoryName(itemPath), Path.GetFileName(itemPath) + ".manifest.xml");
		}

		private static string GetDependencyFilePath(string itemPath, string hintPath, string fileName)
		{
			string filePath;
			if (!string.IsNullOrEmpty(hintPath))
			{
				// A hint path was specified. Use it to locate the dependent file.
				filePath = Path.Combine(hintPath, fileName);
			}
			else
			{
				// No hint path was specified. Assume the dependent template is in the same folder as the main (item) template.
				filePath = Path.Combine(Path.GetDirectoryName(itemPath), fileName);
			}

			return filePath;
		}
	}
}
