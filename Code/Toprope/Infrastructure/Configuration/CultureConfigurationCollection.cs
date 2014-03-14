using System;
using System.Configuration;

namespace Toprope.Infrastructure.Configuration
{
    /// <summary>
    /// Represents a collection of culture settings.
    /// </summary>
    public class CultureConfigurationCollection : ConfigurationElementCollection
    {
        #region Properties

        /// <summary>
        /// Gets the collection type.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        /// <summary>
        /// Gets culture configuration with the specified culture name.
        /// </summary>
        /// <param name="cultureName">Culture name.</param>
        /// <returns>Culture configuration with the specified culture name.</returns>
        public new CultureConfiguration this[string cultureName]
        {
            get { return (CultureConfiguration)BaseGet(cultureName); }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public CultureConfigurationCollection() { }

        /// <summary>
        /// Returns the default culture.
        /// </summary>
        /// <returns>Default culture.</returns>
        public CultureConfiguration GetDefault()
        {
            CultureConfiguration ret = null;

            foreach(object i in this)
            {
                if (i != null && i is CultureConfiguration)
                {
                    if ((i as CultureConfiguration).IsDefault)
                    {
                        ret = (i as CultureConfiguration);
                        break;
                    }
                }
            }

            if (ret == null)
                ret = CultureConfiguration.Default;

            return ret;
        }

        /// <summary>
        /// Creates new element.
        /// </summary>
        /// <returns>New element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new CultureConfiguration();
        }

        /// <summary>
        /// Retrieves a key of a given element.
        /// </summary>
        /// <param name="element">Element whose key to retrieve.</param>
        /// <returns>A key of a given element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            object ret = null;

            if (element != null && Type.Equals(element.GetType(), typeof(CultureConfiguration)))
                ret = ((CultureConfiguration)element).CultureName;

            return ret;
        }
    }
}