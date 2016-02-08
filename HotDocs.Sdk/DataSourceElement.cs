using System.Configuration;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     <c>DataSourceElement</c> is a class representing a data source.
    ///     It has <c>Name</c> and <c>Address</c> properties
    /// </summary>
    public class DataSourceElement : ConfigurationElement
    {
        /// <summary>
        ///     Returns the <c>Name</c> associated with this data source element
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        ///     Returns the <c>Address</c> associated with this data source element
        /// </summary>
        [ConfigurationProperty("address", IsRequired = true)]
        public string Address
        {
            get { return (string) this["address"]; }
            set { this["address"] = value; }
        }
    }
}