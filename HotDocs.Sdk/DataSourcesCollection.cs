using System.Configuration;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     <c>DataSourcesCollection</c> a collection of data sources
    /// </summary>
    public class DataSourcesCollection : ConfigurationElementCollection
    {
        /// <summary>
        ///     <c>CollectionType</c> the type of the collection
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        /// <summary>
        ///     Return the <c>DataSourceElement</c> at the given <c>index</c>
        /// </summary>
        /// <param name="index">the index value</param>
        /// <returns>the <c>DataSourceElement</c> at the given <c>index</c></returns>
        public DataSourceElement this[int index]
        {
            get { return (DataSourceElement) BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        ///     Return the <c>DataSourceElement</c> associated with <c>Name</c>
        /// </summary>
        /// <param name="Name">the string value associated with the returned object</param>
        /// <returns>the <c>DataSourceElement</c> associated with <c>Name</c></returns>
        public new DataSourceElement this[string Name]
        {
            get { return (DataSourceElement) BaseGet(Name); }
        }

        /// <summary>
        ///     <c>CreateNewElement</c> creates a new configuration element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DataSourceElement();
        }

        /// <summary>
        ///     <c>GetElementKey</c> retrieves the name of the specified element.
        /// </summary>
        /// <param name="element">the element</param>
        /// <returns>the name of the specified element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DataSourceElement) element).Name;
        }

        /// <summary>
        ///     Returns the index of <c>dataServiceElement</c>
        /// </summary>
        /// <param name="dataServiceElement">
        ///     the <c>DataSourceElement</c>
        ///     for which the returned index will be returned
        /// </param>
        /// <returns>the index of <c>dataServiceElement</c></returns>
        public int IndexOf(DataSourceElement dataServiceElement)
        {
            return BaseIndexOf(dataServiceElement);
        }

        /// <summary>
        ///     <c>Add</c> adds <c>dataServiceElement</c> to the collection of
        ///     data sources in this class
        /// </summary>
        /// <param name="dataServiceElement">
        ///     The <c>DataSourceElement</c> being added to the
        ///     data sources in this class
        /// </param>
        public void Add(DataSourceElement dataServiceElement)
        {
            BaseAdd(dataServiceElement);
        }

        /// <summary>
        ///     <c>BaseAdd</c> overrides ConfigurationElementCollection.BaseAdd
        /// </summary>
        /// <param name="element">The <c>ConfigurationElement</c> to add to the collection.</param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        /// <summary>
        ///     <c>Remove</c> removes <c>dataServiceElement</c> from the collection
        ///     of data sources in this class.
        /// </summary>
        /// <param name="dataServiceElement">
        ///     The <c>DataSourceElement</c> being removed from the
        ///     data sources in this class instance
        /// </param>
        public void Remove(DataSourceElement dataServiceElement)
        {
            if (BaseIndexOf(dataServiceElement) >= 0)
                BaseRemove(dataServiceElement.Name);
        }

        /// <summary>
        ///     <c>RemoveAt</c> removes the data source located at <c>index</c>
        /// </summary>
        /// <param name="index">the integer based location</param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        ///     <c>Remove</c> removes the data source specified by <c>name</c>
        /// </summary>
        /// <param name="name">
        ///     identifies the data source being removed from the
        ///     data sources in this class instance
        /// </param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        /// <summary>
        ///     <c>Clear</c> removes all data sources in this class instance.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }
    }
}