using System.Xml.Serialization;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     Catalogs the types of dependency a HotDocs template can have on another file.
    ///     NOTE: It needs to be kept in sync with the HotDocs desktop and server IDL files!
    /// </summary>
    public enum DependencyType
    {
        /// <summary>
        ///     No (or an unrecognized) dependency.
        /// </summary>
        [XmlEnum("noDependency")] NoDependency = 0,

        /// <summary>
        ///     The "base" component file of a template; that is, the component file with the same base file name as the template.
        /// </summary>
        [XmlEnum("baseCmpFile")] BaseCmpFile = 1,

        /// <summary>
        ///     An additional component file (if any) that is "pointed to" by the template's base component file.
        /// </summary>
        [XmlEnum("pointedToCmpFile")] PointedToCmpFile = 2,

        /// <summary>
        ///     Another template referred to in an INSERT instruction in this template.
        /// </summary>
        [XmlEnum("templateInsert")] TemplateInsert = 3,

        /// <summary>
        ///     A clause referred to in an INSERT instruction in this template.
        /// </summary>
        [XmlEnum("clauseInsert")] ClauseInsert = 4,

        /// <summary>
        ///     A clause library referred to in an INSERT instruction in this template.
        /// </summary>
        [XmlEnum("clauseLibraryInsert")] ClauseLibraryInsert = 5,

        /// <summary>
        ///     An image file (typically JPG or PNG) referred to in an INSERT instruction in this template.
        /// </summary>
        [XmlEnum("imageInsert")] ImageInsert = 6,

        /// <summary>
        ///     An image file (typically JPG or PNG) displayed as part of the interview of this template.
        /// </summary>
        [XmlEnum("interviewImage")] InterviewImage = 7,

        /// <summary>
        ///     The name of a text, multiple choice or computed variable referred to in an INSERT instruction in this template.
        ///     This variable's answer (or the computation's result) at runtime will determine a template to be inserted.
        /// </summary>
        [XmlEnum("variableTemplateInsert")] VariableTemplateInsert = 8,

        /// <summary>
        ///     The name of a text, multiple choice or computed variable referred to in an INSERT instruction in this template.
        ///     This variable's answer (or the computation's result) at runtime will determine an image to be inserted.
        /// </summary>
        [XmlEnum("variableImageInsert")] VariableImageInsert = 9,

        /// <summary>
        ///     The name of a variable that was referred to by the template but does not exist in the component file.
        ///     Indicates an error condition.
        /// </summary>
        [XmlEnum("missingVariable")] MissingVariable = 10,

        /// <summary>
        ///     The name of a file that was referred to by the template but does not exist on disk or in the package.
        ///     Indicates an error condition.
        /// </summary>
        [XmlEnum("missingFile")] MissingFile = 11,

        /// <summary>
        ///     Another template referred to in an ASSEMBLE instruction in this template.
        /// </summary>
        [XmlEnum("assemble")] Assemble = 12,

        /// <summary>
        ///     A publisher map file (.hdpmx)
        /// </summary>
        [XmlEnum("publisherMapFile")] PublisherMapFile = 13,

        /// <summary>
        ///     A user map file (.hdumx)
        /// </summary>
        [XmlEnum("userMapFile")] UserMapFile = 14,

        /// <summary>
        ///     Another template otherwise designated as required by this template. This includes each item
        ///     listed in this template's "Additional Templates" component file property.  This is used to
        ///     list templates that may potentially need to be inserted via a "variable" INSERT instruction.
        /// </summary>
        [XmlEnum("additionalTemplate")] AdditionalTemplate = 15
    }
}