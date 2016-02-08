using System.Configuration;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     <c>CustomDataSourcesSection</c> contains the settings used for data sources
    /// </summary>
    public class CustomDataSourcesSection : ConfigurationSection
    {
        /// <summary>
        ///     <c>DataSources</c> returns the data sources for the current configuration
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof (DataSourcesCollection))]
        public DataSourcesCollection DataSources
        {
            get { return (DataSourcesCollection) base[""]; }
        }
    }
}