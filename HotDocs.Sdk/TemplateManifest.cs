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
    ///     <c>TemplateManifest</c> contains the information needed for document assembly creation with
    ///     the specified template file, sometimes called the 'main template'
    /// </summary>
    public class TemplateManifest
    {
        private static readonly XNamespace s_namespace = "http://www.hotdocs.com/schemas/template_manifest/2012";

        internal TemplateManifest()
        {
        }

        /// <summary>
        ///     <c>HotDocsVersion</c> indicates the version of HotDocs that was used to create
        ///     the current template manifest.
        /// </summary>
        public string HotDocsVersion { get; private set; }

        /// <summary>
        ///     <c>TemplateId</c> identifies the template to distinguish it from other templates.
        /// </summary>
        public string TemplateId { get; private set; }

        /// <summary>
        ///     <c>FileName</c> indicates the file name of the current template.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        ///     <c>EffectiveCmpFileName</c> is the component file name that is used
        ///     for document assembly creation with the main template contained in
        ///     this template manifest.
        /// </summary>
        public string EffectiveCmpFileName { get; private set; }

        /// <summary>
        ///     <c>ExpirationDate</c> indicates the current template cannot be used for
        ///     document assembly creations after the given date, except
        ///     possibly for a given number of <c>ExtensionDays</c>
        /// </summary>
        public DateTime? ExpirationDate { get; private set; }

        /// <summary>
        ///     <c>WarningDays</c> indicates for how many days prior to a document's expiration date
        ///     users should be warned about using the current template for document assembly creations.
        /// </summary>
        public int? WarningDays { get; private set; }

        /// <summary>
        ///     <c>ExtensionDays</c> indicates for how many days after the current templates
        ///     expiration date has passed users can still perform document assembly creations.
        /// </summary>
        public int? ExtensionDays { get; private set; }

        /// <summary>
        ///     <c>Title</c> provides the title for the main template
        ///     in the current template manifest.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        ///     <c>Description</c> provides a description for the main template
        ///     in the current template manifest.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        ///     <c>Variables</c> Contains the set of variables associated with the
        ///     main template in the current template manifest.
        /// </summary>
        public VariableInfo[] Variables { get; private set; }

        /// <summary>
        ///     <c>Dependencies</c> indicates a collection of files associated with
        ///     the the main template of the template manifest.
        /// </summary>
        public Dependency[] Dependencies { get; private set; }

        /// <summary>
        ///     <c>AdditionalFiles</c> lists additional files that may be required during interviews,
        ///     assemblies or other processing associated with the template.
        /// </summary>
        public AdditionalFile[] AdditionalFiles { get; private set; }

        /// <summary>
        ///     <c>DataSources</c> lists the data sources (<c>DataSource</c> objects) from which the template
        ///     may (at user request) attempt to retrieve data during an interview.
        /// </summary>
        public DataSource[] DataSources { get; private set; }

        /// <summary>
        ///     <c>ParseManifest</c> parses the XML template manifest indicated by <paramref name="templatePath" />
        ///     and creates a corresponding instance of <c>TemplateManifest</c>.
        /// </summary>
        /// <param name="templatePath">The full path of the template</param>
        /// <param name="parse">Specifies settings related to template manifest creation</param>
        /// <returns>a reference to the newly created <c>TemplateManifest</c></returns>
        public static TemplateManifest ParseManifest(string templatePath, ManifestParse parse)
        {
            var fileName = Path.GetFileName(templatePath);
            TemplateLocation fileLoc = new PathTemplateLocation(Path.GetDirectoryName(templatePath));

            return ParseManifest(fileName, fileLoc, parse);
        }

        /// <summary>
        ///     <c>ParseManifest</c> parses the XML template manifest indicated by <paramref name="fileName" />
        ///     and <paramref name="location" />, and creates a corresponding instance of <c>TemplateManifest</c>.
        /// </summary>
        /// <param name="fileName">The file name of the template</param>
        /// <param name="location">The location of the template, such as a file system folder, a package file, a database, etc.</param>
        /// <param name="parse">Specifies settings related to template manifest creation</param>
        /// <returns>a reference to the newly created <c>TemplateManifest</c></returns>
        public static TemplateManifest ParseManifest(string fileName, TemplateLocation location,
            ManifestParse parse)
        {
            var templateManifest = new TemplateManifest();
            var baseTemplateLoc = new TemplateFileLocation(fileName, location);

            var variables = GetHashSet<VariableInfo>(parse, ManifestParse.ParseVariables);
            var dependencies = GetHashSet<Dependency>(parse, ManifestParse.ParseDependencies);
            var additionalFiles = GetHashSet<AdditionalFile>(parse, ManifestParse.ParseAdditionalFiles);
            var dataSources = GetHashSet<DataSource>(parse, ManifestParse.ParseDataSources);

            var templateQueue = new Queue<TemplateFileLocation>();
            var processedTemplates = new HashSet<TemplateFileLocation>();

            // Add the base template to the queue
            templateQueue.Enqueue(baseTemplateLoc);

            while (templateQueue.Count > 0)
            {
                var templateFileLoc = templateQueue.Dequeue();
                if (!processedTemplates.Contains(templateFileLoc))
                {
                    try
                    {
                        // TODO: Condider re-writing this using an XmlReader so that the entire document does not need to be allocated in a DOM. Doing so
                        // should make processing template manifest files faster. For now using an XDocument to just get things done fast.
                        XDocument manifest;
                        using (
                            var manifestStream =
                                templateFileLoc.FileLocation.GetFile(GetManifestName(templateFileLoc.FileName)))
                        {
                            // Read the template manifest so that it can be parsed.
                            manifest = XDocument.Load(manifestStream, LoadOptions.None);
                        }

                        if (templateFileLoc.Equals(baseTemplateLoc))
                        {
                            // Process the root templateManifest element.
                            templateManifest.HotDocsVersion = manifest.Root.Attribute("hotdocsVersion").Value;
                            templateManifest.TemplateId = manifest.Root.Attribute("templateId").Value;
                            templateManifest.FileName = manifest.Root.Attribute("fileName").Value;
                            templateManifest.EffectiveCmpFileName = manifest.Root.Attribute("effectiveCmpFile").Value;

                            if (manifest.Root.Attribute("expirationDate") != null)
                            {
                                templateManifest.ExpirationDate =
                                    DateTime.Parse(manifest.Root.Attribute("expirationDate").Value);

                                if (manifest.Root.Attribute("warningDays") != null)
                                {
                                    templateManifest.WarningDays =
                                        int.Parse(manifest.Root.Attribute("warningDays").Value);
                                }

                                if (manifest.Root.Attribute("extensionDays") != null)
                                {
                                    templateManifest.ExtensionDays =
                                        int.Parse(manifest.Root.Attribute("extensionDays").Value);
                                }
                            }

                            var titleElem = manifest.Root.Element(s_namespace + "title");
                            templateManifest.Title = titleElem != null ? titleElem.Value.Trim() : string.Empty;

                            var descriptionElem = manifest.Root.Element(s_namespace + "description");
                            templateManifest.Description = descriptionElem != null
                                ? descriptionElem.Value.Trim()
                                : string.Empty;
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
                                    var hintPath = dependencyElem.Attribute("hintPath") != null
                                        ? dependencyElem.Attribute("hintPath").Value
                                        : null;

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
                                            throw new Exception(string.Format("Invalid dependency type '{0}'.",
                                                dependencyElem.Attribute("type").Value));
                                    }
                                    dependencies.Add(new Dependency(dependencyElem.Attribute("fileName").Value, hintPath,
                                        dependencyType));
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
                                            throw new Exception(string.Format("Invalid data source type '{0}'.",
                                                dataSourceElem.Attribute("type").Value));
                                    }

                                    var dataSourceFields = new List<DataSourceField>();
                                    foreach (
                                        var dataSourceFieldElem in
                                            dataSourceElem.Elements(s_namespace + "dataSourceField"))
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
                                                throw new Exception(
                                                    string.Format("Invalid data source field type '{0}'.",
                                                        dataSourceFieldElem.Attribute("type").Value));
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
                                                throw new Exception(
                                                    string.Format("Invalid data source backfill type '{0}'.",
                                                        dataSourceFieldElem.Attribute("backfill").Value));
                                        }

                                        var isKey = dataSourceFieldElem.Attribute("isKey") != null
                                            ? Convert.ToBoolean(dataSourceFieldElem.Attribute("isKey").Value)
                                            : false;

                                        dataSourceFields.Add(
                                            new DataSourceField(dataSourceFieldElem.Attribute("sourceName").Value,
                                                fieldType, backfillType, isKey));
                                    }
                                    dataSources.Add(new DataSource(dataSourceElem.Attribute("id").Value,
                                        dataSourceElem.Attribute("name").Value,
                                        dataSourceType, dataSourceFields.ToArray()));
                                }
                            }
                        }

                        if ((parse & ManifestParse.ParseRecursively) == ManifestParse.ParseRecursively)
                        {
                            // Add any referenced templates to the template queue.
                            var dependenciesElem = manifest.Root.Element(s_namespace + "dependencies");
                            if (dependenciesElem != null)
                            {
                                var templateDependencies =
                                    from d in dependenciesElem.Elements(s_namespace + "dependency")
                                    let type = d.Attribute("type").Value
                                    where type == "templateInsert" || type == "additionalTemplate"
                                    select d;

                                foreach (var templateDependency in templateDependencies)
                                {
                                    templateQueue.Enqueue(GetDependencyFileLocation(baseTemplateLoc,
                                        templateDependency.Attribute("hintPath") != null
                                            ? templateDependency.Attribute("hintPath").Value
                                            : null,
                                        templateDependency.Attribute("fileName").Value));
                                }
                            }

                            // Mark the template as processed so that its manifest will not get processed again.
                            processedTemplates.Add(templateFileLoc);
                        }
                    }
                    catch (Exception e)
                    {
                        var errorText =
                            string.Format(
                                "Failed to read the manifest file for the template:\r\n\r\n     {0}     \r\n\r\n" +
                                "The following error occurred:\r\n\r\n     {1}     ", templateFileLoc.FileName,
                                e.Message);
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

        private static HashSet<T> GetHashSet<T>(ManifestParse parse, ManifestParse flag) where T : class
        {
            return (parse & flag) == flag ? new HashSet<T>() : null;
        }

        private static string GetManifestName(string itemName)
        {
            // If the passed file name appears to already be a manifest file name then just use it.
            if (itemName.EndsWith(".manifest.xml", StringComparison.OrdinalIgnoreCase))
                return itemName;

            return itemName + ".manifest.xml";
        }

        private static TemplateFileLocation GetDependencyFileLocation(TemplateFileLocation itemLoc, string hintPath,
            string fileName)
        {
            if (!string.IsNullOrEmpty(hintPath))
            {
                // A hint path was specified. Use it to locate the dependent file.
                return new TemplateFileLocation(Path.Combine(hintPath, fileName));
            }
            // No hint path was specified. Assume the dependent template is in the same folder as the main (item) template.
            return new TemplateFileLocation(fileName, itemLoc.FileLocation);
        }

        private struct TemplateFileLocation : IEquatable<TemplateFileLocation>
        {
            internal TemplateFileLocation(string fileName, TemplateLocation location)
            {
                FileName = fileName;
                FileLocation = location;
            }

            internal TemplateFileLocation(string filePath)
            {
                FileName = Path.GetFileName(filePath);
                FileLocation = new PathTemplateLocation(Path.GetDirectoryName(filePath));
            }

            internal string FileName { get; }

            internal TemplateLocation FileLocation { get; }

            public override bool Equals(object obj)
            {
                return (obj != null) && obj is TemplateFileLocation && Equals((TemplateFileLocation) obj);
            }

            /// <summary>
            ///     Overrides Object.GetHashCode
            /// </summary>
            /// <returns>a number of type int</returns>
            public override int GetHashCode()
            {
                const int prime = 397;
                var result = FileLocation.GetHashCode(); // hash for the location, combined with
                result = (result*prime) ^ FileName.ToLower().GetHashCode(); // case-insensitive hash for the file name
                return result;
            }

            #region IEquatable<TemplateFileLocation> Members

            /// <summary>
            ///     Implements IEquatable.Equals
            /// </summary>
            /// <param name="other">The object being compared to 'this'</param>
            /// <returns>boolean (true or false)</returns>
            public bool Equals(TemplateFileLocation other)
            {
                return string.Equals(other.FileName, FileName, StringComparison.OrdinalIgnoreCase)
                       && FileLocation.Equals(other.FileLocation);
            }

            #endregion
        }
    }
}